#region Informations
/*
 * 
 * Auteur: G. Ragneau
 * Creation : 13/07/2006
 * Modification :
 *		Author - Date - Descriptoin
 */
#endregion

using System;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// VersionsVehicleWebControlException thrown whenever it's not possible to build versions webcontrol.
	/// </summary>
	public class VersionsVehicleWebControlException:BaseException{ 
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public VersionsVehicleWebControlException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public VersionsVehicleWebControlException(string message):base(message){
		
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception Source</param>
		public VersionsVehicleWebControlException(string message, Exception innerException):base(message, innerException){
		
		}


		#endregion
	}
}
