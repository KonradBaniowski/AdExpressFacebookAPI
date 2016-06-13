using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Helper.Exceptions
{
    /// <summary>
    /// Classe d'exception de la génération des données pour l'analyse plan média
    /// </summary>
    public class MediaCreationDataAccessException : BaseException
    {

        #region Constructeur

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public MediaCreationDataAccessException() : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public MediaCreationDataAccessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public MediaCreationDataAccessException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
        #endregion
    }
}
