using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.P3.Web.Core.Exceptions
{
    /// <summary>
    /// Cweb session exception class
    /// </summary>
    public class WebSessionException : BaseException
    {

        #region Constructeurs
        /// <summary>
        /// Constructeor
        /// </summary>
        public WebSessionException()
            : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public WebSessionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public WebSessionException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
