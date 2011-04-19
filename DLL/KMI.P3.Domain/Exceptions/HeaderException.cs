using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.P3.Domain.Exceptions
{
    /// <summary>
    /// Thrown when is impossible to load the data relating to the page headings
    /// </summary>
    public class HeaderException : BaseException
    {

        #region Constructors
        /// <summary>
        /// Base constructor
        /// </summary>
        public HeaderException()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public HeaderException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner Exception</param>
        public HeaderException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
