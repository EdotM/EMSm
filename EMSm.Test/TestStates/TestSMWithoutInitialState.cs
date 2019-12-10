using EM.EMSm;

namespace EMSm.Test.TestStates
{
    internal class TestSMWithoutInitialState : State
    {
        public override TransitionsTable TransitionsTable
        {
            get => new TransitionsTable {
            new TransitionEntry{
                Transition=Transitions.EnableCommandReceived,
                StateType=typeof(EnabledState),
                StateName="Enabled"},
            new TransitionEntry{
                Transition=Transitions.DisableCommandReceived,
                StateType=typeof(DisabledState),
                StateName="BlinkSlow"},
            };
        }
    }
}
