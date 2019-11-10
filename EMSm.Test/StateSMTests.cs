using System;
using EM.EMSm;
using NUnit.Framework;

namespace EMSm.Test
{

    #region TestCommandArgs

    class ComplexRefType
    {
        public int IntVar { get; set; }

        public string StrVar { get; set; }

    }

    struct ComplexValType
    {
        public int IntVar { get; set; }

        public string StrVar { get; set; }

    }
    class TestCommandArgs
    {
        public int IntVar { get; set; }

        public string StrVar { get; set; }

        public ComplexRefType RefType { get; set; }
        
        public ComplexValType ValType { get; set; }
    }

    #endregion

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

        public TestCommandArgs LastCommandArgs { get; set; }

        public int LastSimpleCommandArgs { get; set; }

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
            Type commandArgsType;
            if ((commandArgsType = this.GetCommandArgsType()) != null)
            {
                if (commandArgsType == typeof(int))
                    this.LastSimpleCommandArgs = this.GetCommandArgs<int>();
                else if (commandArgsType == typeof(TestCommandArgs))
                    this.LastCommandArgs = this.GetCommandArgs<TestCommandArgs>();
            }

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
            this.testSM.InjectCommand(Commands.Enable);
            this.testSM.RunCycle();
            Assert.IsTrue(innerState.IsExitExecuted);
        }

        [Test]
        public void RunCycle_RunOfStateLeave_ShouldExecuteExitMethodOnly()
        {
            TestState innerState = (TestState)this.testSM.InnerState;
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Enable);
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
            this.testSM.InjectCommand(Commands.Enable);
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.InnerState is EnabledState);
        }

        [Test]
        public void RunCycle_RunOfStateTransition_ShouldSwitchToEnabledStateToDisabledState()
        {
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.InnerState is DisabledState);
            this.testSM.InjectCommand(Commands.Enable);
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.InnerState is EnabledState);
            this.testSM.InjectCommand(Commands.Disable);
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.InnerState is DisabledState);
        }

        [Test]
        public void RunCycle_RunOfStateTransition_InitialDisabledStateShouldBeTheSameInstanceAsDisabledState()
        {
            this.testSM.RunCycle();
            ((TestState)this.testSM.InnerState).InstanceId = 0x12345678;
            this.testSM.InjectCommand(Commands.Enable);
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Disable);
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
            this.testSM.InjectCommand(Commands.Disable);
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
            this.testSM.InjectCommand(Commands.Disable);
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

        [Test]
        public void InjectCommand_InjectCommandEnum_ShouldBeAvailableInAllInnerStates()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Disable);
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommand == Commands.Disable);
                innerState = (TestState)innerState.InnerState;
            }
        }

        [Test]
        public void InjectCommand_InjectCommandWithSimpleArgs_ArgsShouldBeAvailableInAllInnerStates()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand<int>(Commands.Disable, 6);
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastSimpleCommandArgs == 6);

                innerState = (TestState)innerState.InnerState;
            }
        }

        [Test]
        public void InjectCommand_InjectCommandWithArgs_ArgsShouldBeAvailableInAllInnerStates()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand<TestCommandArgs>(Commands.Disable, new TestCommandArgs() { IntVar = 3, StrVar = "Hallo", RefType = (new ComplexRefType() { IntVar = 2, StrVar = "HalloRef" }), ValType = (new ComplexValType() { IntVar = 1, StrVar = "HalloVal" }) });
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommandArgs != null);
                Assert.IsTrue(innerState.LastCommandArgs.IntVar == 3);
                Assert.IsTrue(innerState.LastCommandArgs.StrVar == "Hallo");
                Assert.IsTrue(innerState.LastCommandArgs.RefType != null);
                Assert.IsTrue(innerState.LastCommandArgs.RefType.IntVar == 2);
                Assert.IsTrue(innerState.LastCommandArgs.RefType.StrVar == "HalloRef");
                Assert.IsTrue(innerState.LastCommandArgs.ValType.IntVar == 1);
                Assert.IsTrue(innerState.LastCommandArgs.ValType.StrVar == "HalloVal");

                innerState = (TestState)innerState.InnerState;
            }
        }

        [Test]
        public void InjectCommand_InjectCommandWithoutArgs_ArgsShouldBeNullInAllInnerStates()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Disable);
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommandArgs == null);

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
