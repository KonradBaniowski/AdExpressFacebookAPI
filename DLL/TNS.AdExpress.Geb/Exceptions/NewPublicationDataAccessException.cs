#region Informations
// Auteur : B.Masson
// Date de création : 13/04/2006
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Geb.Exceptions{
	/// <summary>
	/// Classe d'exception du chargement des nouveaux supports pigés en presse
	/// </summary>
	public class NewPublicationDataAccessException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public NewPublicationDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public NewPublicationDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public NewPublicationDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
