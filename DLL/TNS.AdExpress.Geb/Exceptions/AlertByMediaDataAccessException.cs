#region Informations
// Auteur : B.Masson
// Date de création : 13/04/2006
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Geb.Exceptions{
	/// <summary>
	/// Classe d'exception du chargement des alertes par supports
	/// </summary>
	public class AlertByMediaDataAccessException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AlertByMediaDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AlertByMediaDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public AlertByMediaDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
