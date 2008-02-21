#region Informations
// Auteur: D. V. Mussuma 
// Date de cr�ation: 13/04/2006 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions
{
	/// <summary>
	/// L�ve une exception pour le contr�le de s�lections des N derniers dates
	/// </summary>
	public class LastMonthWeekCalendarWebControlException:BaseException {
		
		#region Constructeur		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public LastMonthWeekCalendarWebControlException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public LastMonthWeekCalendarWebControlException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public LastMonthWeekCalendarWebControlException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
	
		
}
	
	

