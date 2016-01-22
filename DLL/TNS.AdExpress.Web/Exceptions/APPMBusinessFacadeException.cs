#region Informations
// Auteur: K. Shehzad 
// Date de création: 11/07/2005 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// APPMBusinessFacadeException thrown whenever an error occured while accessing to theAppm.BusinessFacade
	/// </summary>
	public class APPMBusinessFacadeException:BaseException {
		#region Constructeur		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public APPMBusinessFacadeException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public APPMBusinessFacadeException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public APPMBusinessFacadeException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
	
		
}
	
	

