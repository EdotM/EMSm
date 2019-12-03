using System;
using EM.EMSm;
using NUnit.Framework;

namespace EMSm.Test
{
    #region VarTestState

    class VarTestState : TestState
    {
        public int TestVar { get; set; }

        protected override Enum Do()
        {
            this.TestVar = this.GetVar<int>("testVar");
            return base.Do();
        }
    }

    #endregion

    public class VarTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InjectVar_InjectNoVar_ShouldThrowException()
        {
            VarTestState state = new VarTestState();
            Assert.Throws<VarNotFoundException>(() => state.RunCycle());
        }

        [Test]
        public void InjectVar_InjectVar_ShouldBeAvailableInAllInnerStates()
        {
            int var = 0x55AA55AA;
            VarTestState state = new VarTestState();
            state.InjectVar("testVar", var);
            state.RunCycle();
            VarTestState innerState = state;
            while (innerState != null)
            {
                Assert.IsTrue(innerState.TestVar == var);
                innerState = (VarTestState)innerState.CurrentInnerState;
            }
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
