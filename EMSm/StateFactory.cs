using System;
using System.Collections.Generic;
using System.Text;

namespace EM.EMSm
{
    internal static class StateFactory
    {
        static private Dictionary<string, State> _states = new Dictionary<string, State>();

        public static State CreateState(Type type, string name)
        {
            if (!_states.ContainsKey(name))
            {
                _states.Add(name, (State)Activator.CreateInstance(type));
                _states[name].Name = name;
            }
            return _states[name];
        }
    }
}
