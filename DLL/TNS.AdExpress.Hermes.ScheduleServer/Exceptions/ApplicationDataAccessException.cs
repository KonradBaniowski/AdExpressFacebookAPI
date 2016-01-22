#region Informations
// Auteur : B.Masson
// Date de création : 12/02/2007
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Hermes.ScheduleServer.Exceptions{
	/// <summary>
	/// Classe d'exception du chargement de la configuration de l'application
	/// </summary>
	public class ApplicationDataAccessException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ApplicationDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ApplicationDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public ApplicationDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}