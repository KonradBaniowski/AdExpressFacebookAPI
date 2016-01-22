#region Informations
// Auteur : B.Masson
// Date de création : 19/04/2006
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Geb.Exceptions{
	/// <summary>
	/// Classe d'exception pour AlertByMedia
	/// </summary>
	public class AlertByMediaException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AlertByMediaException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AlertByMediaException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public AlertByMediaException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
