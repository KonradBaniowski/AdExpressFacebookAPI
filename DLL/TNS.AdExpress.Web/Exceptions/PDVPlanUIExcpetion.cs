#region Informations
// Author: K. Shehzad
// Date of creation: 02/08/2005 
// Date of modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// IHM: APPM PDV du plan
	/// </summary>
	public class PDVPlanUIException:BaseException{
		
		#region Constructeur		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PDVPlanUIException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PDVPlanUIException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PDVPlanUIException(string message,System.Exception innerException):base(message,innerException){
		}

		#endregion
	}
}
