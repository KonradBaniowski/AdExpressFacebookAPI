#region Information
// Author: K. Shehzad
// Creation: 27/07/2005 
// Last modifications:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions {
	/// <summary>
	/// AffinitiesDataAccessExeption thrown when database access error : APPM
	/// </summary>
	public class AffinitiesDataAccessExeption:BaseException {
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AffinitiesDataAccessExeption():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AffinitiesDataAccessExeption(string message):base(message){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AffinitiesDataAccessExeption(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
