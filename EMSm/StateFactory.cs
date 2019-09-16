using System;
using System.Collections.Generic;
using System.Text;

namespace EM.EMSm
{
    internal class StateFactory
    {
        #region private fields

        private readonly Dictionary<string, State> states = new Dictionary<string, State>();

        #endregion

        #region public methods

        public State CreateState(Type type, string name)
        {
            if (!this.states.ContainsKey(name))
            {
                this.states.Add(name, (State)Activator.CreateInstance(type));
                this.states[name].Name = name;
            }
            return states[name];
        }

        #endregion
    }
}
