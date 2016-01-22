#region Informations
// Auteur: ?
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// DashBoardDataAccessException  thrown whenever an error occured while generating UI for dashboard module
	/// </summary>
	public class DashBoardUIException : BaseException{
		#region constructeurs
		/// <summary>
		/// Constructeur 
		/// </summary>
		public DashBoardUIException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DashBoardUIException(string message):base(message) {			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DashBoardUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
