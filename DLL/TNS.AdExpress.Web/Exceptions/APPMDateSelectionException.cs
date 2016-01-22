#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 24/08/2005 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// APPMDateSelectionException thrown whenever an error occured while accessing to APPM Date Selection
	/// </summary>
	public class APPMDateSelectionException:BaseException {
		#region Constructeur		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public APPMDateSelectionException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public APPMDateSelectionException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public APPMDateSelectionException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
	
		
}
	
	

