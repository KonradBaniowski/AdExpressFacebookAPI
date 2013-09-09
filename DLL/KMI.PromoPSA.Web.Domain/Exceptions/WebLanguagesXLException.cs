using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.PromoPSA.Web.Domain.Exceptions {
    /// <summary>
    /// Web language data access Excpetion
    /// </summary>
    public class WebLanguagesXLException : BaseException {

        #region Constructors
        /// <summary>
        /// Base constructor
        /// </summary>
        public WebLanguagesXLException() : base() {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public WebLanguagesXLException(string message) : base(message) {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner Exception</param>
        public WebLanguagesXLException(string message, System.Exception innerException) : base(message, innerException) {
        }
        #endregion

    }
}
