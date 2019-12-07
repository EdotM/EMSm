using System;
using EM.EMSm;

namespace EMSm.Test.TestStates
{
    internal class StateWithoutNoneCommands : State
    {
        protected override Enum Do()
        {
            if (this.GetCommand<CommandsWithoutNone>() == CommandsWithoutNone.Disable)
                return Transitions.DisableCommandReceived;
            return base.Do();
        }
    }
}
