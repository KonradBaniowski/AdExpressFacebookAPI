using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.VP.Loader.Domain.Exceptions {
    /// <summary>
    /// AllClassification Exception Class
    /// </summary>
    public class AllClassificationException:BaseException {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public AllClassificationException()
            : base() {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public AllClassificationException(string message)
            : base(message) {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public AllClassificationException(string message, System.Exception innerException)
            : base(message, innerException) {
        }
        #endregion

    }
}
