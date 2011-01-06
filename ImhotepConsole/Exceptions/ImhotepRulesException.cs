using System;

namespace TNS.AdExpress.Imhotep.Exceptions
{
    /// <summary>
    /// Description résumée de ImhotepRulesException.
    /// </summary>
    public class ImhotepRulesException : System.Exception
    {
        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public ImhotepRulesException()
            : base()
        {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">message</param>
        public ImhotepRulesException(string message)
            : base(message)
        {
        }
        #endregion
    }
}
