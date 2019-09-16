using System;
using System.Collections.Generic;
using System.Text;

namespace EM.EMSm
{
    internal class Context
    {
        #region private fields

        private readonly Dictionary<Enum, State> transitionsDict = new Dictionary<Enum, State>();
        private readonly StateFactory stateFactory = new StateFactory();

        #endregion

        #region properties

        public State InitialState { get; private set; }
        public State CurrentState { get; private set; }

        #endregion

        #region constructor

        public Context(TransitionsTable transitionsTable)
        {
            if (transitionsTable == null)
                throw new ArgumentNullException(nameof(transitionsTable));
            //Find inital state
            foreach (var transitionEntry in transitionsTable)
            {
                this.transitionsDict.Add(transitionEntry.Transition, stateFactory.CreateState(transitionEntry.StateType, transitionEntry.StateName));

                if (transitionEntry.Transition.ToString().Contains("Initial", StringComparison.Ordinal))
                    this.InitialState = this.transitionsDict[transitionEntry.Transition];
            }

            if (this.InitialState == null)
                throw new InvalidConfigException(EM.EMSm.Properties.Resources.NoInitialStateDefinedMessage);

            this.CurrentState = this.InitialState;
        }

        #endregion

        #region public methods

        public void Operate()
        {
            Enum transition;
            if ((transition = this.CurrentState.RunCycle()) != null)
            {
                this.CurrentState.Reset();
                this.CurrentState = this.transitionsDict[transition];
            }
        }

        public void Reset()
        {
            this.CurrentState.Reset();
            this.CurrentState = InitialState;
        }

        #endregion    
    }
}
