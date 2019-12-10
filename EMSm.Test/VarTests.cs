using EM.EMSm;
using EMSm.Test.TestStates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EMSm.Test
{
    [TestClass]
    public class VarTests
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void InjectVar_InjectNoVar_ShouldThrowException()
        {
            VarTestState state = new VarTestState();
            Assert.ThrowsException<VarNotFoundException>(() => state.RunCycle());
        }

        [TestMethod]
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

        [TestCleanup]
        public void TearDown()
        {
        }
    }
}
