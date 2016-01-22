#region Informations
// Auteur: G. Ragneau
// Date de création: 25/10/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Exceptions{
	/// <summary>
	/// Exception de la couche d'accès aux données des requêtes clients
	/// </summary>
	public class RequestsDataAccessException:BaseException{
			
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public RequestsDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public RequestsDataAccessException(string message):base(message){
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public RequestsDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}

		#endregion

	}
}