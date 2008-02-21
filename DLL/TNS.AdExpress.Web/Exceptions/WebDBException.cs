#region Informations
// Auteur: G. Facon 
// Date de création: 25/04/04 
// Date de modification: 25/04/04
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{

	/// <summary>
	/// Classe gérant les exceptions du module de traduction
	/// </summary>
	public class WebDBException:BaseException{

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public WebDBException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public WebDBException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public WebDBException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
