using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.PromoPSA.Web.Domain.Exceptions {
    /// <summary>
    /// Promo PSA Web Services XL Exception
    /// </summary>
    public class PromoPSAWebServicesXLException : BaseException {

        #region Constructors
        /// <summary>
        /// Base constructor
        /// </summary>
        public PromoPSAWebServicesXLException() : base() {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public PromoPSAWebServicesXLException(string message) : base(message) {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner Exception</param>
        public PromoPSAWebServicesXLException(string message, System.Exception innerException) : base(message, innerException) {
        }
        #endregion

    }
}
