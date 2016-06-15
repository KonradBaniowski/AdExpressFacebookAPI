using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Helper.Exceptions
{
    /// <summary>
    /// VersionUIException thrown whenever an error occured in VersionUI
    /// </summary>
    public class VersionUIException : BaseException
    {

        #region Constructeur

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public VersionUIException() : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public VersionUIException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public VersionUIException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
        #endregion
    }
}
