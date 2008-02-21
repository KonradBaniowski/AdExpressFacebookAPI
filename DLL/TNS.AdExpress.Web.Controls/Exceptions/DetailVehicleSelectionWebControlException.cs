#region Informations
/*
 * 
 * Auteur: D. Mussuma
 * Creation : 21/07/2006
 * Modification :
 *		Author - Date - Descriptoin
 */
#endregion

using System;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// DetailVehicleSelectionWebControlException thrown whenever it's not possible to build media webcontrol.
	/// </summary>
	public class DetailVehicleSelectionWebControlException:BaseException{ 
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DetailVehicleSelectionWebControlException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public DetailVehicleSelectionWebControlException(string message):base(message){
		
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception Source</param>
		public DetailVehicleSelectionWebControlException(string message, Exception innerException):base(message, innerException){
		
		}


		#endregion
	}
}
