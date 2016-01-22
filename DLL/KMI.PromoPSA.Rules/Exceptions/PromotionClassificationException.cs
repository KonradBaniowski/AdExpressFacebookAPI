using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.PromoPSA.Rules.Exceptions
{
   
    /// <summary>
    /// Right Object Exception
    /// </summary>
    public class PromotionClassificationException : BaseException
    {

        #region Constructors
        /// <summary>
        /// Base constructor
        /// </summary>
        public PromotionClassificationException()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public PromotionClassificationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner Exception</param>
        public PromotionClassificationException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
