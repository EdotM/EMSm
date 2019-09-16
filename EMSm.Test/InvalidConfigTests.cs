using System;
using EM.EMSm;
using NUnit.Framework;

namespace EMSm.Test
{
    #region NoInitialTransitionEntryTest

    class TestSMWithoutInitialState : State
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

    #endregion

    #region NoNoneCommand-Test

    enum CommandsWithoutNone
    {
        Enable,
        Disable,
    }

    class StateWithoutNoneCommands : State
    {
        protected override Enum Do()
        {
            if (this.GetCommand<CommandsWithoutNone>() == CommandsWithoutNone.Disable)
                return Transitions.DisableCommandReceived;
            return base.Do();
        }
    }

    #endregion
    
    public class InvalidConfigTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Ctor_NoInitialTransitionEntryDefined_ShouldThrowInvalidConfigException()
        {
            Assert.Throws<InvalidConfigException>(() => new TestSMWithoutInitialState());
        }

        [Test]
        public void RunCycle_NoNoneCommandDefined_ShouldThrowInvalidConfigException()
        {
            StateWithoutNoneCommands state = new StateWithoutNoneCommands();
            Assert.Throws<InvalidConfigException>(() => state.RunCycle());
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
