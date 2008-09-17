#region Informations
// Auteur : B.Masson
// Date de création : 21/04/2006
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Anubis.Geb.Exceptions{
	/// <summary>
	/// Classe d'exception dans l'obtention des données du résultat (GebExcelDataAccessException)
	/// </summary>
	public class GebExcelDataAccessException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public GebExcelDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public GebExcelDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public GebExcelDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
