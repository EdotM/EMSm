using System;

namespace EM.EMSm
{
    /// <summary>
    /// Provides the event data of the <see cref="State.StatePathChanged"/> event.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class StatePathChangedEventArgs : EventArgs
    {
        #region properties

        /// <summary>
        /// Gets the old state path.
        /// </summary>
        /// <value>
        /// The old state path.
        /// </value>
        public string OldStatePath { get; private set; }

        /// <summary>
        /// Gets the new state path.
        /// </summary>
        /// <value>
        /// The new state path.
        /// </value>
        public string NewStatePath { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StatePathChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldStatePath">The old state path.</param>
        /// <param name="newStatePath">The new state path.</param>
        public StatePathChangedEventArgs(string oldStatePath, string newStatePath)
        {
            this.OldStatePath = oldStatePath;
            this.NewStatePath = newStatePath;
        }

        #endregion
    }
}
