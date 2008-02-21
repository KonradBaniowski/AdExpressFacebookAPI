#region Informations
// Auteur: B. Masson
// Date de création: 06/07/2005
// Date de modification:
//	16/08/2005	G. Facon	Constructeur avec Exception  
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// Exception de l'affichage d'une bannière depuis le composant
	/// </summary>
	public class DisplayBannerWebControlException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DisplayBannerWebControlException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DisplayBannerWebControlException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DisplayBannerWebControlException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
