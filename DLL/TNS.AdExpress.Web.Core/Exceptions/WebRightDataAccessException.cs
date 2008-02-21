#region Informations
// Auteur: ?
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// Classe pour gérer les exceptions de la classe WebRightDataAccess
	/// </summary>
	public class WebRightDataAccessException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public WebRightDataAccessException():base(){			
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public WebRightDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public WebRightDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
