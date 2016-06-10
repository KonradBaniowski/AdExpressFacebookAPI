using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Helper.Exceptions
{
    /// <summary>
    ///  Classe d'exception de la génération des données pour les variétés 
    /// </summary>
    public class RecapAdvertiserDataAccessException : BaseException
    {

        #region Constructeurs

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public RecapAdvertiserDataAccessException() : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public RecapAdvertiserDataAccessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public RecapAdvertiserDataAccessException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
        #endregion

    }
}
