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
	/// APPMMediaPlanUIExcpetion thrown while constructing HTML for Media Plan : APPM
	/// </summary>
	public class APPMMediaPlanUIExcpetion:BaseException 
	{
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public APPMMediaPlanUIExcpetion():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public APPMMediaPlanUIExcpetion(string message):base(message){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public APPMMediaPlanUIExcpetion(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

