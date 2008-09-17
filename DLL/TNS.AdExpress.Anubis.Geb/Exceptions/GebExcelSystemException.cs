#region Informations
// Auteur : G. Facon
// Date de création : 20/06/2006
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Anubis.Geb.Exceptions{
	/// <summary>
	/// Classe d'exception dans le traitement de Geb
	/// </summary>
	public class GebExcelSystemException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public GebExcelSystemException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public GebExcelSystemException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
		public GebExcelSystemException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
