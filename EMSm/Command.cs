using System;
using System.Collections.Generic;
using System.Text;

namespace EM.EMSm
{
    public class Command
    {
        public Enum Cmd { get; private set; }

        public object CmdArgs { get; private set; }

        public Command(Enum cmd, object cmdArgs)
        {
            this.Cmd = cmd;
            this.CmdArgs = cmdArgs;
        }

        public Command(Enum cmd) : this(cmd, null)
        {
        }
    }
}
