#region Informations
/*
 * 
 * Auteur: G. Facon
 * Creation : 14/02/2007
 * Modification :
 *		Author - Date - Descriptoin
 */
#endregion

using System;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// AlertsCreationsResultsWebControlException thrown whenever it's not possible to build inseretions webcontrol.
	/// </summary>
	public class AlertsCreationsResultsWebControlException:BaseException{ 
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AlertsCreationsResultsWebControlException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public AlertsCreationsResultsWebControlException(string message):base(message){
		
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception Source</param>
		public AlertsCreationsResultsWebControlException(string message, Exception innerException):base(message, innerException){
		
		}


		#endregion
	}
}
