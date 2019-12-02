using System;
using System.Collections.Generic;
using System.Text;

namespace EM.EMSm
{
    /// <summary>Holder-class for commands with command-args</summary>
    public class Command
    {
        #region properties

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public Enum Cmd { get; private set; }

        /// <summary>
        /// Gets the command arguments.
        /// </summary>
        /// <value>
        /// The command arguments.
        /// </value>
        public object CmdArgs { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="cmdArgs">The command arguments.</param>
        public Command(Enum cmd, object cmdArgs)
        {
            this.Cmd = cmd;
            this.CmdArgs = cmdArgs;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="cmd">The command.</param>
        public Command(Enum cmd) : this(cmd, null)
        {
        }

        #endregion
    }
}
