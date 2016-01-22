#region Informations
// Auteur: G. Facon
// Date de création: 25/04/04 
// Date de modification: 25/04/04
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.DataAccess.Exceptions{
	/// <summary>
	/// Classe d'erreur de la base de données pour les traductions
	/// </summary>
	public class WordListDALException:BaseException{

		#region Constructeurs

		/// <summary>
		/// Constructeur
		/// </summary>
		public WordListDALException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public WordListDALException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public WordListDALException(string message,System.Exception innerException):base(message,innerException){
		}

		#endregion
	}
}
