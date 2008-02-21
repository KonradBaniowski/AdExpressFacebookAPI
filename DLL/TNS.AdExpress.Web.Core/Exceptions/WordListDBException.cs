#region Informations
// Auteur: G. Facon
// Date de création: 25/04/04 
// Date de modification: 25/04/04
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// Classe d'erreur de la base de données pour les traductions
	/// </summary>
	public class WordListDBException:BaseException{

		#region Constructeurs

		/// <summary>
		/// Constructeur
		/// </summary>
		public WordListDBException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public WordListDBException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public WordListDBException(string message,System.Exception innerException):base(message,innerException){
		}

		#endregion
	}
}
