#region Informations
// Auteur: D. Mussuma
// Date de création:
// Date de modification: 30/07/209
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions {
	/// <summary>
	/// Exception class for Customer data filters
	/// </summary>
	public class CustomerDataFiltersException : BaseException {
		
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public CustomerDataFiltersException():base(){
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public CustomerDataFiltersException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">inner exception</param>
		public CustomerDataFiltersException(string message, System.Exception innerException)
			: base(message, innerException) {
		}
		#endregion
	}
}
