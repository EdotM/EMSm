using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EM.EMSm
{
    public class StatePathChangedEventArgs : EventArgs
    {
        public string OldStatePath { get; private set; }
        public string NewStatePath { get; private set; }

        public StatePathChangedEventArgs(string oldStatePath, string newStatePath)
        {
            this.OldStatePath = oldStatePath;
            this.NewStatePath = newStatePath;
        }
    }

    public class State
    {
        private object syncRoot = new object();

        private bool isRunning = false;

        private Command newCommand = null;
        private Command command = null;

        private Dictionary<string, object> varsDictionary = null;

        private string oldStatePath = null;
        
        
        protected Context context = null;


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



        protected virtual void Enter()
        {
            
        }

        protected virtual Enum Do()
        {
            return null;
        }

        protected virtual void Exit()
        {
            
        }

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
                    Debug.WriteLine($"{this.Name}: Execute Enter()");
                    Enter();
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

        protected bool IsCommandAvailable<T>() where T : Enum
        {
            return ((this.command != null) && (this.command.Cmd is T));
        }

        protected T GetCommand<T>() where T : Enum
        {
            if ((this.command != null) && (this.command.Cmd is T))
                return (T)this.command.Cmd;
            else
                return default;
        }


        public void InjectVar(string name, object var)
        {
            if (this.varsDictionary == null)
                this.varsDictionary = new Dictionary<string, object>();
            this.varsDictionary.Add(name, var);
        }

        protected T GetVar<T>(string name)
        {
            return (T)this.varsDictionary[name];
        }



        internal void Reset()
        {
            Debug.WriteLine($"{this.Name}: Execute Exit()");
            Exit();
            this.context?.Reset();
            this.isRunning = false;
        }


        public event EventHandler<StatePathChangedEventArgs> StatePathChanged;
        protected virtual void OnStatePathChanged(StatePathChangedEventArgs e)
        {
            if (StatePathChanged != null)
                StatePathChanged(this, e);
        }

    }
}
