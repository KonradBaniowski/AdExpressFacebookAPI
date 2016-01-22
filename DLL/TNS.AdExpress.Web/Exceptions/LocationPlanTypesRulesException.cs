#region Informations
// Auteur: D. V. Mussuma
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Business Rules :  APPM Emplacement du plan
	/// </summary>
	public class LocationPlanTypesRulesException :BaseException{
	
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public LocationPlanTypesRulesException(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message</param>
		public LocationPlanTypesRulesException(string message):base(message) {			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public LocationPlanTypesRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
