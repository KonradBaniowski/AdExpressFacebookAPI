using TNS.AdExpress.VP.Loader.Domain;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.VP.Loader.Exceptions {
    /// <summary>
    /// ResultDALException Exception Class
    /// </summary>
    public class VeillePromoExcelInvalidDateException : VeillePromoExcelCellException {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public VeillePromoExcelInvalidDateException(CellExcel cellExcel)
            : base(cellExcel) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public VeillePromoExcelInvalidDateException(CellExcel cellExcel, string message)
            : base(cellExcel, message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public VeillePromoExcelInvalidDateException(CellExcel cellExcel, string message, System.Exception innerException)
            : base(cellExcel, message, innerException) {
        }
        #endregion

    }
}
