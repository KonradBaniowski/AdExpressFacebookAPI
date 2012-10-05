using TNS.AdExpress.Rolex.Loader.Domain;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Rolex.Loader.DAL.Exceptions
{
    public class DataAccessDALExcelInvalidDateException : DataAccessDALExcelException
    {
     

     #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public DataAccessDALExcelInvalidDateException(CellExcel cellExcel)
            : base(cellExcel) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellExcel">cell excel </param>
        /// <param name="message">Message d'erreur</param>
        public DataAccessDALExcelInvalidDateException(CellExcel cellExcel, string message)
            : base(cellExcel, message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellExcel">cell excel </param>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public DataAccessDALExcelInvalidDateException(CellExcel cellExcel, string message, System.Exception innerException)
            : base(cellExcel, message, innerException) {
        }
        #endregion

    }
}
