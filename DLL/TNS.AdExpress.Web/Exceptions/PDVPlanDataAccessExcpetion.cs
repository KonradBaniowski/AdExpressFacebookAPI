#region Informations
// Author: K. Shehzad
// Date of creation: 02/08/2005 
// Date of modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de données: APPM PDV du plan
	/// </summary>
	public class PDVPlanDataAccessExcpetion:BaseException{

		#region Constructeur		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PDVPlanDataAccessExcpetion():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PDVPlanDataAccessExcpetion(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PDVPlanDataAccessExcpetion(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

