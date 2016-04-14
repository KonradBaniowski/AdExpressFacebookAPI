using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KM.AdExpressI.MyAdExpress.Exceptions
{
    public class MySessionDataAccessException : BaseException
    {
        #region Constructors
        /// <summary>
        /// Base constructor
        /// </summary>
        public MySessionDataAccessException() : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public MySessionDataAccessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner Exception</param>
        public MySessionDataAccessException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
        #endregion
    }
}
