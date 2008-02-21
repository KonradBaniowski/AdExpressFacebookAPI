#region Informations
// Auteur: G. Facon
// Date de création: 12/08/2005
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// System: Génération du code pour l'affichage d(attente
	/// </summary>
	public class LoadingSystemException : BaseException{
	
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public LoadingSystemException(){

		}
		
		/// <summary>
		/// constructeur
		/// </summary>
		/// <param name="message">message</param>
		public LoadingSystemException(string message):base(message) {
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public LoadingSystemException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
