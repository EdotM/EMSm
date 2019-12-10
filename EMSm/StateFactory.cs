using System;
using System.Collections.Generic;

namespace EM.EMSm
{
    /// <summary>
    /// Used for initializing and providing states
    /// </summary>
    internal class StateFactory
    {
        #region private fields

        private readonly Dictionary<string, State> states = new Dictionary<string, State>();

        #endregion

        #region public methods

        /// <summary>
        /// Creates or returns a already created state
        /// </summary>
        /// <param name="type">The type of the state.</param>
        /// <param name="name">The name of the state.</param>
        /// <returns></returns>
        public State CreateState(Type type, string name)
        {
            if (!this.states.ContainsKey(name))
            {
                this.states.Add(name, (State)Activator.CreateInstance(type));
                this.states[name].Name = name;
            }
            return states[name];
        }

        /// <summary>
        /// Returns the state which has the given name
        /// </summary>
        /// <param name="name">The name of the needed state</param>
        /// <returns></returns>
        /// <exception cref="EM.EMSm.StateNotFoundException"></exception>
        public State GetState(string name)
        {
            if (!this.states.ContainsKey(name))
                throw new StateNotFoundException($"{EM.EMSm.Properties.Resources.StateNotFoundMessage} (name:\"{name}\")");
            return states[name];
        }

        #endregion
    }
}
