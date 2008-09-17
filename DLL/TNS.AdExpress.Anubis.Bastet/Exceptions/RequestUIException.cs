#region Informations
// Auteur: D. V. Mussuma
// Date de création: 16/12/2006
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Bastet.Exceptions{
	/// <summary>
	/// Classe d'exception de l'affichage des requêtes lesplus suavegardées
	/// </summary>
	public class RequestUIException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public RequestUIException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public RequestUIException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public RequestUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
