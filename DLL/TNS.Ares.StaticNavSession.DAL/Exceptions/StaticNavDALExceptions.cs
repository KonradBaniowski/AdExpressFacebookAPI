using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.StaticNavSession.DAL.Exceptions {
    public class StaticNavDALExceptions:BaseException {

        #region Constructeur

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public StaticNavDALExceptions()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public StaticNavDALExceptions(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public StaticNavDALExceptions(string message, System.Exception innerException)
            : base(message, innerException) {
        }
        #endregion

    }
}
