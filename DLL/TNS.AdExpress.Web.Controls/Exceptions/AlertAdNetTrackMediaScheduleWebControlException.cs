#region Informations
/*
 * 
 * Auteur: D. Mussuma
 * Creation : 30/03/2007
 * Modification :
 *		Author - Date - Descriptoin
 */
#endregion

using System;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// AlertAdNetTrackMediaScheduleWebControlException thrown whenever it's not possible to build mediaschedule for AdNetTrack webcontrol.
	/// </summary>
	public class AlertAdNetTrackMediaScheduleWebControlException:BaseException{ 
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AlertAdNetTrackMediaScheduleWebControlException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public AlertAdNetTrackMediaScheduleWebControlException(string message):base(message){
		
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception Source</param>
		public AlertAdNetTrackMediaScheduleWebControlException(string message, Exception innerException):base(message, innerException){
		
		}


		#endregion
	}
}
