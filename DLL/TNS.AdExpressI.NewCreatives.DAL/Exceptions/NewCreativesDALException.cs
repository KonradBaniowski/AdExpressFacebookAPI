#region Informations
/*
 * Author : B.Masson
 * Created on 29/09/2008
 * Modifications:
 *      Author - Date - Description
 */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.NewCreatives.DAL.Exceptions {
    /// <summary>
    /// Excepion class for new creatives data access layer
    /// </summary>
    public class NewCreativesDALException : BaseException {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public NewCreativesDALException(): base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public NewCreativesDALException(string message): base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public NewCreativesDALException(string message, System.Exception innerException): base(message, innerException) {
        }
        #endregion

    }
}
