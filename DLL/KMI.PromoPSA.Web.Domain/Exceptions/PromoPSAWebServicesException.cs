using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.PromoPSA.Web.Domain.Exceptions {
    /// <summary>
    /// Promo PSA Web Services Exception
    /// </summary>
    public class PromoPSAWebServicesException : BaseException {

        #region Constructors
        /// <summary>
        /// Base constructor
        /// </summary>
        public PromoPSAWebServicesException() : base() {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public PromoPSAWebServicesException(string message) : base(message) {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner Exception</param>
        public PromoPSAWebServicesException(string message, System.Exception innerException) : base(message, innerException) {
        }
        #endregion

    }
}
