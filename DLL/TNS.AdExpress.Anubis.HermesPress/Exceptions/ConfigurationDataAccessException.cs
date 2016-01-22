#region Informations
// Auteur : B.Masson
// Date de création : 13/03/2007
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Anubis.HermesPress.Exceptions{
	/// <summary>
	/// Classe d'exception ConfigurationDataAccess
	/// </summary>
	public class ConfigurationDataAccessException:System.Exception{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ConfigurationDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ConfigurationDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public ConfigurationDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
