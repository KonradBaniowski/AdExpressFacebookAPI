#region Informations
// Auteur : B.Masson
// Date de création : 12/02/2007
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Hermes.ScheduleServer.Exceptions{
	/// <summary>
	/// Classe d'exception du chargement des rules
	/// </summary>
	public class ScheduleDataAccessException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ScheduleDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ScheduleDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public ScheduleDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}