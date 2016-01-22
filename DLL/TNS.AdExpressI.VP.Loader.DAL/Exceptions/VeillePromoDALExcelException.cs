using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.VP.Loader.DAL.Exceptions {
    /// <summary>
    /// ResultDALException Exception Class
    /// </summary>
    public class VeillePromoDALExcelException:BaseException {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public VeillePromoDALExcelException()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public VeillePromoDALExcelException(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public VeillePromoDALExcelException(string message, System.Exception innerException)
            : base(message, innerException) {
        }
        #endregion

    }
}
