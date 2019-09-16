using System;
using System.Collections.Generic;
using System.Text;

namespace EM.EMSm
{
    public class TransitionEntry
    {
        #region properties

        public Enum Transition { get; set; }

        public Type StateType { get; set; }

        public string StateName { get; set; }

        #endregion
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<Pending>")]
    public class TransitionsTable : List<TransitionEntry>
    { }
}
