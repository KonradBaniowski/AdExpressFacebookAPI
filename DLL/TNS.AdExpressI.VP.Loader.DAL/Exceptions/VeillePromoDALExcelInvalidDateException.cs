using TNS.AdExpress.VP.Loader.Domain;

namespace TNS.AdExpressI.VP.Loader.DAL.Exceptions {
    /// <summary>
    /// ResultDALException Exception Class
    /// </summary>
    public class VeillePromoDALExcelInvalidDateException : VeillePromoDALExcelCellException {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public VeillePromoDALExcelInvalidDateException(CellExcel cellExcel)
            : base(cellExcel) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public VeillePromoDALExcelInvalidDateException(CellExcel cellExcel, string message)
            : base(cellExcel, message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public VeillePromoDALExcelInvalidDateException(CellExcel cellExcel, string message, System.Exception innerException)
            : base(cellExcel, message, innerException) {
        }
        #endregion

    }
}
