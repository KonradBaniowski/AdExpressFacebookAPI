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
	/// DashBoardDataAccessException.  thrown whenever an error occured while retrieving data for DashBoard module
	/// </summary>
	public class DashBoardDataAccessException : BaseException {

		#region constructeurs
		/// <summary>
		/// Constructeur 
		/// </summary>
		public DashBoardDataAccessException():base() {			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DashBoardDataAccessException(string message):base(message) {			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DashBoardDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
