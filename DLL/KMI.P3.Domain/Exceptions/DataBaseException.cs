using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;
namespace KMI.P3.Domain.Exceptions
{
    /// <summary>
    /// Database Object Exception
    /// </summary>
    public class DataBaseException : BaseException
    {

        #region Constructors
        /// <summary>
        /// Base constructor
        /// </summary>
        public DataBaseException()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public DataBaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner Exception</param>
        public DataBaseException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
