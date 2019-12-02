using System;
using System.Collections.Generic;
using System.Text;

namespace EM.EMSm
{
    /// <summary>
    /// Holds the information which transition endsup in which state
    /// </summary>
    public class TransitionEntry
    {
        #region properties

        /// <summary>
        /// Gets or sets the transition.
        /// </summary>
        /// <value>
        /// The transition.
        /// </value>
        public Enum Transition { get; set; }

        /// <summary>
        /// Gets or sets the type of the state.
        /// </summary>
        /// <value>
        /// The type of the state.
        /// </value>
        public Type StateType { get; set; }

        /// <summary>
        /// Gets or sets the name of the state.
        /// </summary>
        /// <value>
        /// The name of the state.
        /// </value>
        public string StateName { get; set; }

        #endregion
    }

    /// <summary>
    /// List of <see cref="TransitionEntry"/> class. This represents the transitions table.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{EM.EMSm.TransitionEntry}" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<Pending>")]
    public class TransitionsTable : List<TransitionEntry>
    { }
}
