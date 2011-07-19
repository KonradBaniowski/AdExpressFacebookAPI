using System;
using System.Collections.Generic;
using System.Web;
using TNS.FrameWork.Exceptions;

namespace WebServiceSetAdScopeLogin.Exceptions
{
    public class AdScopeLoginServiceException : BaseException
    {
        #region Constructeurs
        /// <summary>
        /// Constructeor
        /// </summary>
        public AdScopeLoginServiceException()
            : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public AdScopeLoginServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public AdScopeLoginServiceException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}