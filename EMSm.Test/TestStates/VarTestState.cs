using System;

namespace EMSm.Test.TestStates
{
    internal class VarTestState : TestState
    {
        public int TestVar { get; set; }

        protected override Enum Do()
        {
            this.TestVar = this.GetVar<int>("testVar");
            return base.Do();
        }
    }
}
