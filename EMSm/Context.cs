using System;
using System.Collections.Generic;

namespace EM.EMSm
{
    /// <summary>
    /// Provides context functionality of inner states 
    /// </summary>
    internal class Context
    {
        #region private fields

        private readonly Dictionary<Enum, State> transitionsDict = new Dictionary<Enum, State>();
        private readonly StateFactory stateFactory = new StateFactory();

        #endregion

        #region properties        

        /// <summary>
        /// Gets the initial state.
        /// </summary>
        /// <value>
        /// The initial state.
        /// </value>
        public State InitialState { get; private set; }
        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <value>
        /// The current state.
        /// </value>
        public State CurrentState { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="transitionsTable">The transitions table, which holds the structure of the inner states</param>
        /// <exception cref="ArgumentNullException">transitionsTable</exception>
        /// <exception cref="InvalidConfigException"></exception>
        public Context(TransitionsTable transitionsTable)
        {
            if (transitionsTable == null)
                throw new ArgumentNullException(nameof(transitionsTable));
            //Find inital state
            foreach (var transitionEntry in transitionsTable)
            {
                this.transitionsDict.Add(transitionEntry.Transition, stateFactory.CreateState(transitionEntry.StateType, transitionEntry.StateName));
#if NET35
                if (transitionEntry.Transition.ToString().Contains("Initial"))
                    this.InitialState = this.transitionsDict[transitionEntry.Transition];
#else
                if (transitionEntry.Transition.ToString().Contains("Initial", StringComparison.Ordinal))
                    this.InitialState = this.transitionsDict[transitionEntry.Transition];
#endif
            }

            if (this.InitialState == null)
                throw new InvalidConfigException(EM.EMSm.Properties.Resources.NoInitialStateDefinedMessage);

            this.CurrentState = this.InitialState;
        }

#endregion

        #region public methods        

        /// <summary>
        /// Operates the current state, given by the context
        /// </summary>
        public void Operate()
        {
            Enum transition;
            if ((transition = this.CurrentState.RunInternalCycle()) != null)
            {
                this.CurrentState.Reset();
                this.CurrentState = this.transitionsDict[transition];
            }
        }

        /// <summary>
        /// Resets the current state and sets it to initial state
        /// </summary>
        public void Reset()
        {
            this.CurrentState.Reset();
            this.CurrentState = InitialState;
        }

        /// <summary>Sets the current state by provding the name of the state.</summary>
        /// <param name="stateName">Name of the state.</param>
        /// <remarks>This is used for restoring the state-context by providing state paths</remarks>
        public void SetCurrentStateFromName(string stateName)
        {
            this.CurrentState.Reset();
            this.CurrentState = stateFactory.GetState(stateName);
        }

        #endregion
    }
}
