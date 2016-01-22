#region Informations
// Auteur: ?
// Date de cr�ation:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{

	/// <summary>
	/// Erreur lors de l'utilisation des listes
	/// </summary>
	public class UniversListException:BaseException{

		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public UniversListException():base(){
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public UniversListException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public UniversListException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
