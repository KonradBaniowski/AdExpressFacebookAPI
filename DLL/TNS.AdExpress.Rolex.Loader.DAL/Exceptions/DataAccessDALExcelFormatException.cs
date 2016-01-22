using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Rolex.Loader.DAL.Exceptions
{
 public   class DataAccessDALExcelFormatException : BaseException
    {
     #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public DataAccessDALExcelFormatException()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public DataAccessDALExcelFormatException(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public DataAccessDALExcelFormatException(string message, System.Exception innerException)
            : base(message, innerException) {
        }
        #endregion
    }
}
