using System;
using EM.EMSm;
using EMSm.Test.TestStates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EMSm.Test
{
    [TestClass]
    public class StateSMTests
    {
        private TestSM testSM = null;

        [TestInitialize]
        public void Setup()
        {
            this.testSM = new TestSM();
        }

        [TestMethod]
        public void RunCycle_FirstRunOfState_ShouldExecuteEntryMethod()
        {
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.IsEntryExecuted);
        }

        [TestMethod]
        public void RunCycle_FirstRunOfState_ShouldExecuteDoMethod()
        {
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.IsDoExecuted);
        }

        [TestMethod]
        public void RunCycle_RunOfState_ShouldExecuteDoMethodOnly()
        {
            this.testSM.RunCycle();
            this.testSM.RunCycle();
            Assert.IsTrue((this.testSM.EntryExecutedCtr == 1) && (this.testSM.DoExecutedCtr == 2) && (this.testSM.ExitExecutedCtr == 0));
        }

        [TestMethod]
        public void RunCycle_RunOfStateLeave_ShouldExecuteExitMethod()
        {
            TestState innerState = (TestState)this.testSM.CurrentInnerState;
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Enable);
            this.testSM.RunCycle();
            Assert.IsTrue(innerState.IsExitExecuted);
        }

        [TestMethod]
        public void RunCycle_RunOfStateLeave_ShouldExecuteExitMethodOnly()
        {
            TestState innerState = (TestState)this.testSM.CurrentInnerState;
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Enable);
            this.testSM.RunCycle();
            Assert.IsTrue((innerState.EntryExecutedCtr == 1) && (innerState.DoExecutedCtr == 1) && (innerState.ExitExecutedCtr == 1));
        }

        [TestMethod]
        public void RunCycle_RunOfStateInitial_ShouldDiabledStateAsInitialState()
        {
            Assert.IsTrue(this.testSM.CurrentInnerState is DisabledState);
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.CurrentInnerState is DisabledState);
        }

        [TestMethod]
        public void RunCycle_RunOfStateTransition_ShouldSwitchToEnabledState()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Enable);
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.CurrentInnerState is EnabledState);
        }

        [TestMethod]
        public void RunCycle_RunOfStateTransition_ShouldSwitchToEnabledStateToDisabledState()
        {
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.CurrentInnerState is DisabledState);
            this.testSM.InjectCommand(Commands.Enable);
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.CurrentInnerState is EnabledState);
            this.testSM.InjectCommand(Commands.Disable);
            this.testSM.RunCycle();
            Assert.IsTrue(this.testSM.CurrentInnerState is DisabledState);
        }

        [TestMethod]
        public void RunCycle_RunOfStateTransition_InitialDisabledStateShouldBeTheSameInstanceAsDisabledState()
        {
            this.testSM.RunCycle();
            ((TestState)this.testSM.CurrentInnerState).InstanceId = 0x12345678;
            this.testSM.InjectCommand(Commands.Enable);
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Disable);
            this.testSM.RunCycle();
            Assert.IsTrue(((TestState)this.testSM.CurrentInnerState).InstanceId == 0x12345678);
        }

        [TestMethod]
        public void InjectCommand_InjectNoCommand_ShouldBeNoneInAllInnerStates()
        {
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommand == Commands.None);
                innerState = (TestState)innerState.CurrentInnerState;
            }
        }

        [TestMethod]
        public void InjectCommand_InjectCommand_ShouldBeAvailableInAllInnerStates()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Disable);
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommand == Commands.Disable);
                innerState = (TestState)innerState.CurrentInnerState;
            }
        }

        [TestMethod]
        public void InjectCommand_InjectCommand_ShouldBeAvailableInAllInnerStatesOnlyForOneRunCycle()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Disable);
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommand == Commands.Disable);
                innerState = (TestState)innerState.CurrentInnerState;
            }
            this.testSM.RunCycle();
            innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommand == Commands.None);
                innerState = (TestState)innerState.CurrentInnerState;
            }
        }

        [TestMethod]
        public void InjectCommand_InjectCommandEnum_ShouldBeAvailableInAllInnerStates()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Disable);
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommand == Commands.Disable);
                innerState = (TestState)innerState.CurrentInnerState;
            }
        }

        [TestMethod]
        public void InjectCommand_InjectCommandWithSimpleArgs_ArgsShouldBeAvailableInAllInnerStates()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand<int>(Commands.Disable, 6);
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastSimpleCommandArgs == 6);

                innerState = (TestState)innerState.CurrentInnerState;
            }
        }

        [TestMethod]
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

                innerState = (TestState)innerState.CurrentInnerState;
            }
        }

        [TestMethod]
        public void InjectCommand_InjectCommandWithoutArgs_ArgsShouldBeNullInAllInnerStates()
        {
            this.testSM.RunCycle();
            this.testSM.InjectCommand(Commands.Disable);
            this.testSM.RunCycle();
            TestState innerState = this.testSM;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.LastCommandArgs == null);

                innerState = (TestState)innerState.CurrentInnerState;
            }
        }

        [TestMethod]
        public void StatePathSet_SetStatePathWithOneInnerState_StatesShouldBeSetCorrectly()
        {
            string newStatePath = "TestSM->Enabled";
            this.testSM.StatePath = newStatePath;
            Assert.IsTrue(this.testSM.StatePath.Contains(newStatePath, StringComparison.Ordinal));
        }

        [TestMethod]
        public void StatePathSet_SetStatePathWithTwoInnerState_StatesShouldBeSetCorrectly()
        {
            string newStatePath = "TestSM->Enabled->InnerDisabled";
            this.testSM.StatePath = newStatePath;
            Assert.AreEqual(this.testSM.StatePath, newStatePath);
        }

        [TestMethod]
        public void StatePathSet_SetInvalidStatePath_ShouldThrowArgumentNullException()
        {
            string newStatePath = null;
            Assert.ThrowsException<ArgumentNullException>(() => this.testSM.StatePath = newStatePath);
        }

        [TestMethod]
        public void StatePathSet_SetInvalidStatePath_ShouldThrowInvalidStatePathException()
        {
            string newStatePath = "TestSm->Enabled->InnerDisabled";
            Assert.ThrowsException<InvalidStatePathException>(() => this.testSM.StatePath = newStatePath);
            newStatePath = "TestSM->Enabled->InnerDisabled->Invalid";
            Assert.ThrowsException<InvalidStatePathException>(() => this.testSM.StatePath = newStatePath);
            
            newStatePath = "TestSM->Enabled->InnerDisabled->";
            Assert.ThrowsException<InvalidStatePathException>(() => this.testSM.StatePath = newStatePath);
            newStatePath = string.Empty;
            Assert.ThrowsException<InvalidStatePathException>(() => this.testSM.StatePath = newStatePath);
        }

        [TestMethod]
        public void StatePathSet_SetInvalidStatePath_ShouldThrowStateNotFoundException()
        {
            string newStatePath = "TestSM->nabled->InnerDisabled";
            Assert.ThrowsException<StateNotFoundException>(() => this.testSM.StatePath = newStatePath);
            newStatePath = "TestSM->Enabled->Innerisabled";
            Assert.ThrowsException<StateNotFoundException>(() => this.testSM.StatePath = newStatePath);
        }


        [TestCleanup]
        public void TearDown()
        {
            this.testSM = null;
        }
    }
}
