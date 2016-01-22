using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.StaticNavSession.Exceptions {
    /// <summary>
    /// Pas de données
    /// </summary>
    public class NoDataException : BaseException {

        #region Constructeur

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public NoDataException()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public NoDataException(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public NoDataException(string message, System.Exception innerException)
            : base(message, innerException) {
        }
        #endregion
    }
}
