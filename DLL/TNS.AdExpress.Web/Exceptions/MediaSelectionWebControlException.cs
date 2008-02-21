#region Informations
// Auteur: G. Facon
// Date de création: 04/05/2004
// Date de modification: 04/05/2004
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// WebControl: Sélection des media
	/// </summary>
	public class MediaSelectionWebControlException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaSelectionWebControlException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public MediaSelectionWebControlException(string message):base(message){		
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaSelectionWebControlException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
