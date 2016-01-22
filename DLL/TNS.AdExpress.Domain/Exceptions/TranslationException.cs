#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 25/04/04 
// Date de modification: 25/04/04
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions{
	/// <summary>
	/// Classe g�rant les exceptions du module de traduction
	/// </summary>
	public class TranslationException:BaseException{

		#region Constructeurs
		
		/// <summary>
		/// Constructeur
		/// </summary>
		public TranslationException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public TranslationException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public TranslationException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
