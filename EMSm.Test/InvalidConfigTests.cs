using EM.EMSm;
using EMSm.Test.TestStates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EMSm.Test
{
    [TestClass]
    public class InvalidConfigTests
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void Ctor_NoInitialTransitionEntryDefined_ShouldThrowInvalidConfigException()
        {
            Assert.ThrowsException<InvalidConfigException>(() => new TestSMWithoutInitialState());
        }

        [TestMethod]
        public void RunCycle_NoNoneCommandDefined_ShouldThrowInvalidConfigException()
        {
            StateWithoutNoneCommands state = new StateWithoutNoneCommands();
            Assert.ThrowsException<InvalidConfigException>(() => state.RunCycle());
        }

        [TestCleanup]
        public void TearDown()
        {
        }
    }
}
