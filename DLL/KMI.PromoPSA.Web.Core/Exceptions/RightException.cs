using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.PromoPSA.Web.Core.Exceptions
{
    /// <summary>
    /// Right Object Exception
    /// </summary>
    public class RightException : BaseException
    {

        #region Constructors
        /// <summary>
        /// Base constructor
        /// </summary>
        public RightException()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public RightException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner Exception</param>
        public RightException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
