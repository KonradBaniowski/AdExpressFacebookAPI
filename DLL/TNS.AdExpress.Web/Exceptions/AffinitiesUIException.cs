#region Information
// Author: K. Shehzad
// Creation: 27/07/2005 
// Last modifications:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions {
	/// <summary>
	/// AffinitiesUIException thrown when error while creating affinities UI : APPM
	/// </summary>
	public class AffinitiesUIException:BaseException {
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AffinitiesUIException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AffinitiesUIException(string message):base(message){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AffinitiesUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
