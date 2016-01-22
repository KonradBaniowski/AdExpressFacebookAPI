using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.Exceptions {
    /// <summary>
    /// Exception lors de l'exécution d'une fontions tools
    /// </summary>
    public class FunctionsException : BaseException {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public FunctionsException()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">message d'erreur</param>
        public FunctionsException(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public FunctionsException(string message, System.Exception innerException)
            : base(message, innerException) {
        }

        #endregion
    }
}
