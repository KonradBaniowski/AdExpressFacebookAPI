using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Helper.Exceptions
{
    /// <summary>
    /// Rules: Synthèse
    /// </summary>
    public class SynthesisRulesException : BaseException
    {

        #region Constructeur

        /// <summary>
        /// Constructeur de base
        /// </summary>
        public SynthesisRulesException() : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        public SynthesisRulesException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="innerException">Exception source</param>
        public SynthesisRulesException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
        #endregion
    }
}
