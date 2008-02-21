#region Informations
// Auteur: G. Ragneau
// Date de création: 25/10/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Exceptions{
	/// <summary>
	/// Exception de la couche métier des demandes clients
	/// </summary>
	public class RequestsRulesException:BaseException{
			
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public RequestsRulesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public RequestsRulesException(string message):base(message){
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public RequestsRulesException(string message,System.Exception innerException):base(message,innerException){
		}

		#endregion

	}
}