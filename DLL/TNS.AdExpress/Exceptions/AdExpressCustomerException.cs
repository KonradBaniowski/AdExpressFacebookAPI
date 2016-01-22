#region Informations
// Auteur: A. Obermeyer 
// Date de création: 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Exceptions{
	/// <summary>
	/// Description résumée de AdExpressCustomerExcpetion.
	/// </summary>
	public class AdExpressCustomerException:BaseException{

		#region Constructeur

		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AdExpressCustomerException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AdExpressCustomerException(string message):base(message){
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AdExpressCustomerException(string message,System.Exception innerException):base(message,innerException){
		}

		#endregion


	}
}
