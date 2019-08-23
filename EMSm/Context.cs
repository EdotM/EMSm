using System;
using System.Collections.Generic;
using System.Text;

namespace EM.EMSm
{

    public class Context
    {
        private Dictionary<Enum, State> transitionsDict = new Dictionary<Enum, State>();

        public State InitialState { get; private set; }
        public State CurrentState { get; private set; }


        public Context(TransitionsTable transitionsTable)
        {
            //Find inital state
            foreach (var transitionEntry in transitionsTable)
            {
                this.transitionsDict.Add(transitionEntry.Transition, StateFactory.CreateState(transitionEntry.StateType, transitionEntry.StateName));

                if (transitionEntry.Transition.ToString().Contains("Initial"))
                    this.InitialState = this.transitionsDict[transitionEntry.Transition];
            }

            if (this.InitialState == null)
                throw new SateEstablishException("No Initial state defined!");
            
            this.CurrentState = this.InitialState;
        }


        public void Operate()
        {
            Enum transition = null;
            if ((transition = this.CurrentState.RunCycle()) != null)
            {
                string oldStatePath = this.CurrentState.StatePath;

                this.CurrentState.Reset();
                this.CurrentState = this.transitionsDict[transition];
            }
        }

        internal void Reset()
        {
            this.CurrentState.Reset();
            this.CurrentState = InitialState;
        }
    }
}
