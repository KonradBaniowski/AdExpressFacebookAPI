using TNS.AdExpress.VP.Loader.Domain;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.VP.Loader.Exceptions {
    /// <summary>
    /// ResultDALException Exception Class
    /// </summary>
    public class VeillePromoExcelCellException:BaseException {

        #region Variables
        /// <summary>
        /// Cell Excel
        /// </summary>
        private readonly CellExcel _cellExcel;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public VeillePromoExcelCellException(CellExcel cellExcel)
            : base() {
                _cellExcel = cellExcel;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public VeillePromoExcelCellException(CellExcel cellExcel, string message)
            : base(message) {
                _cellExcel = cellExcel;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public VeillePromoExcelCellException(CellExcel cellExcel, string message, System.Exception innerException)
            : base(message, innerException) {
                _cellExcel = cellExcel;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Cell Excel
        /// </summary>
        public CellExcel CellExcel { get { return _cellExcel; } }
        #endregion

    }
}
