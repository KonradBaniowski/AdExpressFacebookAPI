#region Informations
// Auteur: ?
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// DashBoardRulesException  thrown whenever an error occured while processing data for DashBoard module
	/// </summary>
	public class DashBoardRulesException : BaseException {
		#region constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public DashBoardRulesException():base() {			
		}
		/// <summary>
		/// Constructeur 
		/// </summary>
		/// <param name="message">message</param>
		public DashBoardRulesException(string message) :base(message){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DashBoardRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
