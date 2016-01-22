using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.PromoPSA.Web.Core.Exceptions
{
    public class ExpireSessionException : BaseException
    {

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public ExpireSessionException()
            : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">message d'erreur</param>
        public ExpireSessionException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">message d'erreur</param>
        public ExpireSessionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
