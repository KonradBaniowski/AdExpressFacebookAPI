using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.P3.Web.Exceptions
{
    /// <summary>
    /// Classe gérant les exceptions de la classe de base d'un exception
    /// </summary>
    public class WebPageException : BaseException
    {

        #region Constructeurs
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public WebPageException()
            : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public WebPageException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public WebPageException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
