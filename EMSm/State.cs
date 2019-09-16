using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

[assembly: CLSCompliant(true)]
[assembly: System.Resources.NeutralResourcesLanguageAttribute("en")]

namespace EM.EMSm
{
    public class StatePathChangedEventArgs : EventArgs
    {
        #region properties

        public string OldStatePath { get; private set; }
        public string NewStatePath { get; private set; }

        #endregion

        #region constructor

        public StatePathChangedEventArgs(string oldStatePath, string newStatePath)
        {
            this.OldStatePath = oldStatePath;
            this.NewStatePath = newStatePath;
        }

        #endregion
    }

    public class State
    {
        #region private fields

        private readonly object syncRoot = new object();

        private bool isRunning = false;

        private Command newCommand = null;
        private Command command = null;

        private Dictionary<string, object> varsDictionary = null;

        private string oldStatePath = null;
        
        private readonly Context context = null;

        #endregion

        #region properties

        private Dictionary<string, object> VarsDictionary
        {
            get
            {
                return this.varsDictionary;
            }
            set
            {
                if (this.varsDictionary == null)
                    this.varsDictionary = value;
            }
        }

        public virtual TransitionsTable TransitionsTable { get; }
        
        public string Name { get; set; }

        public string StatePath
        {
            get
            {
                string currentStateName = this.Name;
                if (this.context != null)
                {
                    currentStateName += $"->{this.context.CurrentState.StatePath}";
                }
                return currentStateName;
            }
        }

        public State InnerState
        {
            get
            {
                return this.context?.CurrentState;
            }
        }

        #endregion

        #region constructor

        public State(string name)
        {
            if (name == null)
                this.Name = this.GetType().Name;
            else
                this.Name = name;

            if (this.TransitionsTable?.Count > 0)
                this.context = new Context(this.TransitionsTable);
        }
        public State() : this(null)
        {
        }

        #endregion

        #region private methods

        private static bool IsCommandsEnumValid(Type commandsEnumType)
        {
            var values = Enum.GetValues(commandsEnumType);
            foreach (var value in values)
            {
                if (value.ToString() == "None")
                    return true;
            }
            return false;
        }

        #endregion

        #region protected methods

        protected virtual void Entry()
        {
            
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
        protected virtual Enum Do()
        {
            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
        protected virtual void Exit()
        {
            
        }

        protected bool IsCommandAvailable<T>() where T : Enum
        {
            return ((this.command != null) && (this.command.Cmd is T));
        }

        protected T GetCommand<T>() where T : Enum
        {
            if (!IsCommandsEnumValid(typeof(T)))
                throw new InvalidConfigException(EM.EMSm.Properties.Resources.NoNoneCommandDefinedMessage);

            if ((this.command != null) && (this.command.Cmd is T))
                return (T)this.command.Cmd;
            else
                return default;
        }

        protected T GetVar<T>(string name)
        {
            if (this.varsDictionary == null)
                throw new VarNotFoundException(EM.EMSm.Properties.Resources.VarNotFoundMessage);
            if (!this.varsDictionary.ContainsKey(name))
                throw new VarNotFoundException(EM.EMSm.Properties.Resources.VarNotFoundMessage);
            return (T)this.varsDictionary[name];
        }

        #endregion

        #region public methods

        public Enum RunCycle()
        {
            Enum transition = null;
            try
            {
                lock (syncRoot)
                {
                    this.command = this.newCommand;
                    this.newCommand = null;
                }

                if (!this.isRunning)
                {
                    Debug.WriteLine($"{this.Name}: Execute Entry()");
                    Entry();
                    this.isRunning = true;
                }

                Debug.WriteLine($"{this.Name}: Execute Do()");
                transition = Do();

                if (context != null)
                {
                    if (this.StatePathChanged != null)  //do StatePathChange-check only if an eventhandler is attached to the StatePathChanged-event
                        this.oldStatePath = this.StatePath;

                    this.context.CurrentState.InjectCommand(this.command);
                    if (this.context.CurrentState.VarsDictionary == null)
                        this.context.CurrentState.VarsDictionary = this.VarsDictionary;
                    this.context.Operate();

                    if (this.StatePathChanged != null)
                    {
                        if (this.StatePath != this.oldStatePath)    //do StatePathChange-check only if an eventhandler is attached to the StatePathChanged-event
                            OnStatePathChanged(new StatePathChangedEventArgs(this.oldStatePath, this.StatePath));
                    }
                }
            }
            finally
            {
                this.command = null;
            }

            return transition;
        }

        public void InjectCommand(Command command)
        {
            lock (syncRoot)
            {
                if (this.newCommand == null)
                    this.newCommand = command;
            }
        }

        public void InjectVar(string name, object var)
        {
            if (this.varsDictionary == null)
                this.varsDictionary = new Dictionary<string, object>();
            this.varsDictionary.Add(name, var);
        }

        internal void Reset()
        {
            Debug.WriteLine($"{this.Name}: Execute Exit()");
            Exit();
            this.context?.Reset();
            this.isRunning = false;
        }

        #endregion

        #region events

        public event EventHandler<StatePathChangedEventArgs> StatePathChanged;
        protected virtual void OnStatePathChanged(StatePathChangedEventArgs e)
        {
            StatePathChanged?.Invoke(this, e);
        }

        #endregion
    }
}
