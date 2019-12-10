using System;
using EM.EMSm;


namespace EMSm.Test.TestStates
{
    /// <summary>
    /// Test base class
    /// </summary>
    /// <seealso cref="EM.EMSm.State" />
    internal class TestState : State
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
}
