using System;
using EM.EMSm;
using NUnit.Framework;

namespace EMSm.Test
{

    #region TestState

    class TestState : State
    {
        public int InstanceId { get; set; }

        public bool IsEntryExecuted { get; set; }

        public int EntryExecutedCtr { get; set; }

        public bool IsDoExecuted { get; set; }

        public int DoExecutedCtr { get; set; }

        public bool IsExitExecuted { get; set; }

        public int ExitExecutedCtr { get; set; }

        public Commands LastCommand { get; set; }

        protected override void Entry()
        {
            this.IsEntryExecuted = true;
            this.EntryExecutedCtr++;
            base.Entry();
        }

        protected override Enum Do()
        {
            this.IsDoExecuted = true;
            this.DoExecutedCtr++;
            this.LastCommand = this.GetCommand<Commands>();

            return base.Do();
        }

        protected override void Exit()
        {
            this.IsExitExecuted = true;
            this.ExitExecutedCtr++;
            base.Exit();
        }
    }


    #endregion

    #region TestSM

    enum Commands
    {
        None,
        Enable,
        Disable,
    }

    enum Transitions
    {
        Initial,
        EnableCommandReceived,
        DisableCommandReceived,
    }

    class EnabledState : TestState
    {
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

    class DisabledState : TestState
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

    class TestSM : TestState
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

    #endregion

    public class StateSMTests
    {
        private TestSM testSM = null;

        [SetUp]
        public void Setup()
        {
            this.testSM = new TestSM();
        }

        [Test]
        public void RunCycle_FirstRunOfState_ShouldExecuteEntryMethod()
        {
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.IsEntryExecuted);
        }

        [Test]
        public void RunCycle_FirstRunOfState_ShouldExecuteDoMethod()
        {
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.IsDoExecuted);
        }

        [Test]
        public void RunCycle_RunOfState_ShouldExecuteDoMethodOnly()
        {
            this.testSM.RunCycle();
            this.testSM.RunCycle();
            Assert.IsTrue((this.testSM.EntryExecutedCtr == 1) && (this.testSM.DoExecutedCtr == 2) && (this.testSM.ExitExecutedCtr == 0));
        }

        [Test]
        public void RunCycle_RunOfStateLeave_ShouldExecuteExitMethod()
        {
            TestState innerState = (TestState)this.testSM.InnerState;
            this.testSM.RunCycle();
            this.testSM.InjectCommand(new Command(Commands.Enable));
            this.testSM.RunCycle();
            Assert.IsTrue(innerState.IsExitExecuted);
        }

        [Test]
        public void RunCycle_RunOfStateLeave_ShouldExecuteExitMethodOnly()
        {
            TestState innerState = (TestState)this.testSM.InnerState;
            this.testSM.RunCycle();
            this.testSM.InjectCommand(new Command(Commands.Enable));
            this.testSM.RunCycle();
            Assert.IsTrue((innerState.EntryExecutedCtr == 1) && (innerState.DoExecutedCtr == 1) && (innerState.ExitExecutedCtr == 1));
        }

        [Test]
        public void RunCycle_RunOfStateInitial_ShouldDiabledStateAsInitialState()
        {
            Assert.IsTrue(this.testSM.InnerState is DisabledState);
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.InnerState is DisabledState);
        }

        [Test]
        public void RunCycle_RunOfStateTransition_ShouldSwitchToEnabledState()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand(new Command(Commands.Enable));
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.InnerState is EnabledState);
        }

        [Test]
        public void RunCycle_RunOfStateTransition_ShouldSwitchToEnabledStateToDisabledState()
        {
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.InnerState is DisabledState);
            this.testSM.InjectCommand(new Command(Commands.Enable));
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.InnerState is EnabledState);
            this.testSM.InjectCommand(new Command(Commands.Disable));
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.InnerState is DisabledState);
        }

        [Test]
        public void RunCycle_RunOfStateTransition_InitialDisabledStateShouldBeTheSameInstanceAsDisabledState()
        {
            this.testSM.RunCycle();
            ((TestState)this.testSM.InnerState).InstanceId = 0x12345678;
            this.testSM.InjectCommand(new Command(Commands.Enable));
            this.testSM.RunCycle();
            this.testSM.InjectCommand(new Command(Commands.Disable));
            this.testSM.RunCycle();
            Assert.IsTrue(((TestState)this.testSM.InnerState).InstanceId == 0x12345678);
        }

        [Test]
        public void InjectCommand_InjectNoCommand_ShouldBeNoneInAllInnerStates()
        {
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommand == Commands.None);
                innerState = (TestState)innerState.InnerState;
            }
        }

        [Test]
        public void InjectCommand_InjectCommand_ShouldBeAvailableInAllInnerStates()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand(new Command(Commands.Disable));
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommand == Commands.Disable);
                innerState = (TestState)innerState.InnerState;
            }
        }

        [Test]
        public void InjectCommand_InjectCommand_ShouldBeAvailableInAllInnerStatesOnlyForOneRunCycle()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand(new Command(Commands.Disable));
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommand == Commands.Disable);
                innerState = (TestState)innerState.InnerState;
            }
            this.testSM.RunCycle();
            innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommand == Commands.None);
                innerState = (TestState)innerState.InnerState;
            }
        }

        [TearDown]
        public void TearDown()
        {
            this.testSM = null;
        }
    }
}
