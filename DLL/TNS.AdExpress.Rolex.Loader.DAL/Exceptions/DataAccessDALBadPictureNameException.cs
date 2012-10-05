using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Rolex.Loader.DAL.Exceptions
{
 public   class DataAccessDALBadPictureNameException : BaseException
    {
     #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public DataAccessDALBadPictureNameException()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public DataAccessDALBadPictureNameException(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public DataAccessDALBadPictureNameException(string message, System.Exception innerException)
            : base(message, innerException) {
        }
        #endregion
    }
}
