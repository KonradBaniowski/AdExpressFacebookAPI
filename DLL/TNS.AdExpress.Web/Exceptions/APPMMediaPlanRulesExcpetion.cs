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
	/// APPMMediaPlanUIExcpetion thrown while formatting Media Plan data : APPM
	/// </summary>
	public class APPMMediaPlanRulesExcpetion:BaseException 
	{
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public APPMMediaPlanRulesExcpetion():base()
		{			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public APPMMediaPlanRulesExcpetion(string message):base(message)
		{			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public APPMMediaPlanRulesExcpetion(string message,System.Exception innerException):base(message,innerException)
		{
		}
		#endregion
	}
}
