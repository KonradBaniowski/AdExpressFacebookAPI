#region Informations
// Auteur: A. Obermeyer 
// Date de création: 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Exceptions{
	/// <summary>
	/// Exception de l'accès client à AdExpress
	/// </summary>
	public class AdExpressCustomerDBException:BaseException{

		#region Constructeur

		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AdExpressCustomerDBException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AdExpressCustomerDBException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AdExpressCustomerDBException(string message,System.Exception innerException):base(message,innerException){
		}

		#endregion
	}
}
