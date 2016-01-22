#region Informations
// Auteur: D. V. Mussuma
// Date de création: 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{

	/// <summary>
	/// Classe gérant les exceptions du module de sessions
	/// </summary>
	public class WebSessionException : BaseException{
		
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public WebSessionException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public WebSessionException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public WebSessionException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
