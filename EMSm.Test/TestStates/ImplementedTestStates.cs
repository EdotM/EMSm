using EM.EMSm;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMSm.Test.TestStates
{
    internal class InnerState1 : TestState
    {
    }

    internal class InnerState2 : TestState
    {
    }

    internal class EnabledState : TestState
    {
        public override TransitionsTable TransitionsTable
        {
            get => new TransitionsTable {
            new TransitionEntry{
                Transition=Transitions.Initial,
                StateType=typeof(InnerState1),
                StateName="InnerDisabled"},
            new TransitionEntry{
                Transition=Transitions.EnableCommandReceived,
                StateType=typeof(InnerState2),
                StateName="InnerEnabled"},
            new TransitionEntry{
                Transition=Transitions.DisableCommandReceived,
                StateType=typeof(DisabledState),
                StateName="InnerDisabled"},
            };
        }


        protected override void Entry()
        {
            base.Entry();
        }

        protected override Enum Do()
        {
            if (this.GetCommand<Commands>() == Commands.Disable)
                return Transitions.DisableCommandReceived;
            return base.Do();
        }

        protected override void Exit()
        {
            base.Exit();
        }
    }

    internal class DisabledState : TestState
    {
        protected override void Entry()
        {
            base.Entry();
        }

        protected override Enum Do()
        {
            if (this.GetCommand<Commands>() == Commands.Enable)
                return Transitions.EnableCommandReceived;
            return base.Do();
        }

        protected override void Exit()
        {
            base.Exit();
        }
    }

    internal class TestSM : TestState
    {
        public override TransitionsTable TransitionsTable
        {
            get => new TransitionsTable {
            new TransitionEntry{
                Transition=Transitions.Initial,
                StateType=typeof(DisabledState),
                StateName="Disabled"},
            new TransitionEntry{
                Transition=Transitions.EnableCommandReceived,
                StateType=typeof(EnabledState),
                StateName="Enabled"},
            new TransitionEntry{
                Transition=Transitions.DisableCommandReceived,
                StateType=typeof(DisabledState),
                StateName="Disabled"},
            };
        }
    }

}
