#region Informations
// Auteur: B.Masson
// Date de création:
// Date de modification: 29/11/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Gestion des exceptions de VehicleListDataAccessException
	/// </summary>
	public class VehicleListDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public VehicleListDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public VehicleListDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public VehicleListDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
