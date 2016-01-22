#region Information
// Author: K. Shehzad
// Creation: 24/08/2005 
// Last modifications:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions 
{
	/// <summary>
	/// APPMMediaPlanUIExcpetion thrown while Executing the query for Media Plan: APPM
	/// </summary>
	public class APPMMediaPlanDataAccessException:BaseException 
	{
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public APPMMediaPlanDataAccessException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public APPMMediaPlanDataAccessException(string message):base(message){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public APPMMediaPlanDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
