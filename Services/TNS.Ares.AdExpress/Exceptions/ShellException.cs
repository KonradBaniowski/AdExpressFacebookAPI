using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.AdExpress.Exceptions {
    /// <summary>
    /// Classe d'exception d'initialization Shell
    /// </summary>
    public class ShellException : BaseException {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public ShellException()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public ShellException(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public ShellException(string message, System.Exception innerException)
            : base(message, innerException) {
        }
        #endregion

    }
}
