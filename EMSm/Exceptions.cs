using System;
using System.Collections.Generic;
using System.Text;

namespace EM.EMSm
{
    public class InvalidConfigException : Exception
    {
        #region constructor

        public InvalidConfigException(string message) : base(message)
        {
        }

        public InvalidConfigException()
        {
        }

        public InvalidConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion    
    }

    public class VarNotFoundException : Exception
    {
        #region constructor

        public VarNotFoundException(string message) : base(message)
        {
        }

        public VarNotFoundException()
        {
        }

        public VarNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }

    public class StateNotFoundException : Exception
    {
        #region constructor

        public StateNotFoundException(string message) : base(message)
        {
        }

        public StateNotFoundException()
        {
        }

        public StateNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }

    public class InvalidStatePathException : Exception
    {
        #region constructor

        public InvalidStatePathException(string message) : base(message)
        {
        }

        public InvalidStatePathException()
        {
        }

        public InvalidStatePathException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }

}
