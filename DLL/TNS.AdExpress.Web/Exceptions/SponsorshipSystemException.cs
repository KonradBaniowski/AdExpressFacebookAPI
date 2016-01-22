#region Informations
// Auteur: D. Mussuma 
// Date de création: 05/12/2006 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// DynamicSystemException thrown whenever an error occured while accessing functionalities from dynamic analysis module
	/// </summary>
	public class SponsorshipSystemException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SponsorshipSystemException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SponsorshipSystemException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SponsorshipSystemException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
