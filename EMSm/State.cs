using System;
using System.Collections.Generic;
using System.Diagnostics;

[assembly: CLSCompliant(true)]
[assembly: System.Resources.NeutralResourcesLanguageAttribute("en")]

namespace EM.EMSm
{
    /// <summary>
    /// Represents the base class for state-classes.
    /// Please refer to https://www.eforge.net/EMSm for further documentation.
    /// </summary>
    public abstract class State
    {
        #region consts

        private const string StatePathSepStr = "->";  //character which separates each state in the state-path-hierarchy      

        #endregion

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

        /// <summary>
        /// Gets or sets the vars dictionary.
        /// </summary>
        /// <value>
        /// The vars dictionary.
        /// </value>
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

        /// <summary>
        /// Gets the transitions table. 
        /// This table has to be implemented (override) in each state,
        /// which has inner-states
        /// Please refer to https://www.eforge.net/EMSm for further information
        /// </summary>
        /// <value>
        /// The transitions table.
        /// </value>
        public virtual TransitionsTable TransitionsTable { get; }

        /// <summary>
        /// Gets or sets the name of the state
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the state path of the current active hierarchical states
        /// Is also used to restore the current state (e.g. after surprise-power-down)
        /// </summary>
        /// <value>
        /// The state path.
        /// </value>
        /// <exception cref="ArgumentNullException">value</exception>
        /// <exception cref="EM.EMSm.InvalidStatePathException">
        /// </exception>
        public string StatePath
        {
            get
            {
                string currentStateName = this.Name;
                if (this.context != null)
                {
                    currentStateName += $"{StatePathSepStr}{this.context.CurrentState.StatePath}";
                }
                return currentStateName;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                int sepIdx = value.IndexOf(StatePathSepStr, StringComparison.Ordinal);
                if (sepIdx > 0)
                {
                    //check first state name
                    string firstStateName = value.Substring(0, sepIdx);
                    if (!firstStateName.Contains(this.Name, StringComparison.Ordinal))
                        throw new InvalidStatePathException($"{EM.EMSm.Properties.Resources.StateNotFoundMessage} (name:\"{firstStateName}\")");
                    //check if context for innerStatePath is available
                    string innerStatePath = value.Remove(0, sepIdx + StatePathSepStr.Length);
                    if (this.context == null)
                        throw new InvalidStatePathException($"{EM.EMSm.Properties.Resources.InvalidStatePathMessage} (Inner StatePath:\"{innerStatePath}\")");
                    //extract secondStateName and forward it to the context
                    string secondStateName = innerStatePath;
                    sepIdx = secondStateName.IndexOf(StatePathSepStr, StringComparison.Ordinal);
                    if (sepIdx > 0)
                        secondStateName = secondStateName.Remove(sepIdx);
                    this.context.SetCurrentStateFromName(secondStateName);
                    //forward statePath to the inner state
                    this.context.CurrentState.StatePath = innerStatePath;
                }
                else
                {
                    //if no sep-string -> validate name of actual state
                    if (!value.Contains(this.Name, StringComparison.Ordinal))
                        throw new InvalidStatePathException($"{EM.EMSm.Properties.Resources.StateNotFoundMessage} (name:\"{value}\")");
                }
            }
        }

        /// <summary>
        /// Gets the current active inner state
        /// </summary>
        /// <value>
        /// The active inner state.
        /// </value>
        public State CurrentInnerState
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


        #region protected methods

        /// <summary>
        /// Runs once a transition to the state occurs.
        /// Can be implemented (override) in every state.
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        protected virtual void Entry()
        {
            
        }

        /// <summary>
        /// Here, the state-logic can be implemented (override).
        /// Runs on every RunCycle.
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        /// <returns>New transition if a switch to another state is desired or even null if this state should be remain the active one.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
        protected virtual Enum Do()
        {
            return null;
        }

        /// <summary>
        /// Runs once before a transition to another state happens.
        /// Can be implemented (override) in every state.
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
        protected virtual void Exit()
        {
            
        }

        /// <summary>
        /// Determines whether a new command is available.
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        /// <typeparam name="T">Can be any specified command enum.</typeparam>
        /// <returns>
        ///   <c>true</c> if a new command is available otherwise, <c>false</c>.
        /// </returns>
        protected bool IsCommandAvailable<T>() where T : Enum
        {
            return ((this.command != null) && (this.command.Cmd is T));
        }

        /// <summary>
        /// Gets an injected command-enum.
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        /// <typeparam name="T">Can be any specified command enum.</typeparam>
        /// <returns>Command enum of the injected command.</returns>
        /// <exception cref="EM.EMSm.InvalidConfigException"></exception>
        protected T GetCommand<T>() where T : Enum
        {
            if ((this.command != null) && (this.command.Cmd is T))
                return (T)this.command.Cmd;
            else
                return default;
        }

        /// <summary>
        /// Gets an injected command.
        /// </summary>
        /// <returns>The injected command.</returns>
        /// <exception cref="EM.EMSm.InvalidConfigException"></exception>
        protected Command GetCommand()
        {
            if (this.command != null)
            {
                return this.command;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets the type of the injected command arguments or null if there are no command args available.
        /// </summary>
        /// <returns>command args or null, if not available.</returns>
        protected Type GetCommandArgsType()
        {
            if ((this.command != null) && (this.command.CmdArgs != null))
                return this.command.CmdArgs.GetType();
            else
                return null;
        }

        /// <summary>
        /// Gets the command arguments.
        /// </summary>
        /// <typeparam name="T">Type of the command arguments.</typeparam>
        /// <returns>The command arguments of the current injected command.</returns>
        protected T GetCommandArgs<T>()
        {
            if ((this.command != null) && (this.command.CmdArgs is T))
                return (T)this.command.CmdArgs;
            else
                return default;
        }


        /// <summary>
        /// Gets a injected variable.
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        /// <typeparam name="T">Type of the injected variable.</typeparam>
        /// <param name="name">The name of the requested variable.</param>
        /// <returns>The injected variable</returns>
        /// <exception cref="EM.EMSm.VarNotFoundException">
        /// </exception>
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

        /// <summary>
        /// Runs one cycle of the state-machine. Every Do-Methods of the 
        /// active hierarchical states are executed.
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        /// <returns>A transition to a new state or null.</returns>
        internal Enum RunInternalCycle()
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

        /// <summary>
        /// Runs one cycle of the state-machine. Every Do-Methods of the 
        /// active hierarchical states are executed.
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        public void RunCycle()
        {
            this.RunInternalCycle();
        }

        /// <summary>
        /// Injects a command. Every State has access to this command
        /// during the next executed RunCycle().
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        /// <param name="command">The command which should be injected</param>
        public void InjectCommand(Command command)
        {
            lock (syncRoot)
            {
                if (this.newCommand == null)
                    this.newCommand = command;
            }
        }

        /// <summary>
        /// Injects a command. Every State has access to this command
        /// during the next executed RunCycle().
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        /// <typeparam name="T">Type of the command enum</typeparam>
        /// <param name="cmd">The command which should be injected.</param>
        /// <param name="cmdArgs">The command arguments which should be injected.</param>
        public void InjectCommand<T>(Enum cmd, T cmdArgs)
        {
            this.InjectCommand(new Command(cmd, cmdArgs));
        }

        /// <summary>
        /// Injects a command. Every State has access to this command
        /// during the next executed RunCycle().
        /// Please refer to https://www.eforge.net/EMSm for further documentation.
        /// </summary>
        /// <param name="cmd">The command which should be injected.</param>
        public void InjectCommand(Enum cmd)
        {
            this.InjectCommand(new Command(cmd, null));
        }


        /// <summary>
        /// Injects a variable. Every State can consume this injected variable.
        /// </summary>
        /// <param name="name">The name of the injected variable.</param>
        /// <param name="var">The value of the injected variable.</param>
        public void InjectVar(string name, object var)
        {
            if (this.varsDictionary == null)
                this.varsDictionary = new Dictionary<string, object>();
            this.varsDictionary.Add(name, var);
        }

        /// <summary>
        /// Resets the state an all its inner states.
        /// </summary>
        internal void Reset()
        {
            Debug.WriteLine($"{this.Name}: Execute Exit()");
            Exit();
            this.context?.Reset();
            this.isRunning = false;
        }

        #endregion

        #region events

        /// <summary>
        /// Occurs when the state path has changed.
        /// </summary>
        public event EventHandler<StatePathChangedEventArgs> StatePathChanged;
        protected virtual void OnStatePathChanged(StatePathChangedEventArgs e)
        {
            StatePathChanged?.Invoke(this, e);
        }

        #endregion
    }
}
