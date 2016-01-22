#region Informations
// Auteur: ?
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// IHM: APPM Emplacement du plan
	/// </summary>
	public class LocationPlanTypesUIException :BaseException{
	
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public LocationPlanTypesUIException(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message</param>
		public LocationPlanTypesUIException(string message):base(message) {			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public LocationPlanTypesUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
