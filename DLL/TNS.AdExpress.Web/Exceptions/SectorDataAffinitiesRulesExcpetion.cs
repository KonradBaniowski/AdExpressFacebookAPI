#region Information
// Author: Y. R'kaina
// Creation: 29/01/2007 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// AffinitiesRulesExcpetion thrown when error while processing affinities data : Sector Data APPM
	/// </summary>
	public class SectorDataAffinitiesRulesExcpetion:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SectorDataAffinitiesRulesExcpetion():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SectorDataAffinitiesRulesExcpetion(string message):base(message){
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SectorDataAffinitiesRulesExcpetion(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}
