﻿using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.Exceptions {
    /// <summary>
    /// Description résumée de ReportingSystemException.
    /// </summary>
    public class ReportingSystemException : BaseException {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public ReportingSystemException()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">message d'erreur</param>
        public ReportingSystemException(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public ReportingSystemException(string message, System.Exception innerException)
            : base(message, innerException) {
        }
        #endregion

    }
}
