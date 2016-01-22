#region Informations
// Auteur: D. Mussuma
// Création: 25/05/2009
// Modification:
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;
namespace TNS.AdExpress.Domain.Exceptions {
	public class InfoNewsXLException : BaseException {
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public InfoNewsXLException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public InfoNewsXLException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public InfoNewsXLException(string message, System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion
	}
}
