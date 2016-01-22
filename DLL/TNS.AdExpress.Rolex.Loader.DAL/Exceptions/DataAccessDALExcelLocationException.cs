using TNS.AdExpress.Rolex.Loader.Domain;

namespace TNS.AdExpress.Rolex.Loader.DAL.Exceptions
{
    public class DataAccessDALExcelLocationException : DataAccessDALExcelException
    {
     #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public DataAccessDALExcelLocationException(CellExcel cellExcel)
            : base(cellExcel) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellExcel">cell excel </param>
        /// <param name="message">Message d'erreur</param>
        public DataAccessDALExcelLocationException(CellExcel cellExcel, string message)
            : base(cellExcel, message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="cellExcel">cell excel </param>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public DataAccessDALExcelLocationException(CellExcel cellExcel, string message, System.Exception innerException)
            : base(cellExcel, message, innerException) {
        }
        #endregion
    }
}
