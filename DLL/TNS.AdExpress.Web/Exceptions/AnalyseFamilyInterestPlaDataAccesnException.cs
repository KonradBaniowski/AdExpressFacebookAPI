#region Informations
// Auteur: ?
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// AnalyseFamilyInterestPlanException thorwn whenever an error occures while accessing data for AnalyseFamilyInterest : APPM
	/// </summary>
	public class AnalyseFamilyInterestPlanDataAccessException:BaseException{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AnalyseFamilyInterestPlanDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AnalyseFamilyInterestPlanDataAccessException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AnalyseFamilyInterestPlanDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
