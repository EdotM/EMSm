using System;

namespace EM.EMSm
{
    /// <summary>
    /// The exception that is thrown when an invald configuration is detected. (e.g. missing "Initial" transition)
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvalidConfigException : Exception
    {
        #region constructor        
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidConfigException(string message) : base(message)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigException"/> class.
        /// </summary>
        public InvalidConfigException()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public InvalidConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion    
    }

    /// <summary>
    /// The exception that is thrown when a var was not found. (e.g. wrong name or not injected)
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class VarNotFoundException : Exception
    {
        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="VarNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public VarNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VarNotFoundException"/> class.
        /// </summary>
        public VarNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VarNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public VarNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }

    /// <summary>
    /// The exception that is thrown when state was not found on setting a new state path
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class StateNotFoundException : Exception
    {
        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StateNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public StateNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateNotFoundException"/> class.
        /// </summary>
        public StateNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public StateNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }

    /// <summary>
    /// The exception that is thrown when context for a given state path was not found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvalidStatePathException : Exception
    {
        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStatePathException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidStatePathException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStatePathException"/> class.
        /// </summary>
        public InvalidStatePathException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStatePathException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public InvalidStatePathException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }

}
