using TNS.FrameWork.Exceptions;

namespace RolexLoader.Exceptions
{
 public   class ExcelFormatException : BaseException
    {
     #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public ExcelFormatException()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public ExcelFormatException(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public ExcelFormatException(string message, System.Exception innerException)
            : base(message, innerException) {
        }
        #endregion
    }
}
