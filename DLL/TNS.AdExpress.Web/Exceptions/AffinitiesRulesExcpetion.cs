#region Information
// Author: K. Shehzad
// Creation: 27/07/2005 
// Last modifications:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions {
	/// <summary>
	/// AffinitiesRulesExcpetion thrown when error while processing affinities data : APPM
	/// </summary>
	public class AffinitiesRulesExcpetion:BaseException {
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AffinitiesRulesExcpetion():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AffinitiesRulesExcpetion(string message):base(message){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AffinitiesRulesExcpetion(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
