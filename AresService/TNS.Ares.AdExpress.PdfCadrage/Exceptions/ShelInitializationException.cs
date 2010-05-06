using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.AdExpress.PdfCadrage.Exceptions {
    /// <summary>
    /// Classe d'exception d'initialization Shell
    /// </summary>
    public class ShelInitializationException : BaseException {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public ShelInitializationException()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public ShelInitializationException(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public ShelInitializationException(string message, System.Exception innerException)
            : base(message, innerException) {
        }
        #endregion

    }
}
