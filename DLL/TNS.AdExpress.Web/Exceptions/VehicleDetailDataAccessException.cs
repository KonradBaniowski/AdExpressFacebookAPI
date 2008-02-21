#region Informations
// Auteur: G. Facon 
// Date de création: 17/06/2004 
// Date de modification: 17/06/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Exception Base de données pour les media
	/// </summary>
	public class VehicleDetailDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public VehicleDetailDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message"></param>
		public VehicleDetailDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public VehicleDetailDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
