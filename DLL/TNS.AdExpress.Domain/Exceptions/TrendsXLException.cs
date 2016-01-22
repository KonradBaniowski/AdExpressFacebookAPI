#region Informations
// Auteur: D. Mussuma
// Création: 26/01/2010
// Modification:
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions
{
    public class TrendsXLException : BaseException 
    {
        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public TrendsXLException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public TrendsXLException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public TrendsXLException(string message, System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion
    }
}
