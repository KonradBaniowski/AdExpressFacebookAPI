#region Informations
// Auteur : B.Masson
// Date de création : 21/04/2006
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Anubis.Geb.Exceptions{
	/// <summary>
	/// Classe d'exception dans l'écriture du résultat dans le fichier Excel (GebExcelUI)
	/// </summary>
	public class GebExcelUIException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public GebExcelUIException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public GebExcelUIException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public GebExcelUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
