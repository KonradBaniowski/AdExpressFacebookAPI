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
	/// DashBoardMediaSelectionException  thrown whenever an error occured while selecting media for dashboard module
	/// </summary>
	public class DashBoardMediaSelectionException : BaseException {
		#region constructeurs
		/// <summary>
		///  Constructeur
		/// </summary>
		public DashBoardMediaSelectionException():base() {
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message</param>
		public DashBoardMediaSelectionException(string message):base(message) {
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DashBoardMediaSelectionException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
