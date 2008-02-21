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
	/// AnalyseFamilyInterestPlanRulesException thrown whenever an error occured while processing data for Interest Plan : APPM
	/// </summary>
	public class AnalyseFamilyInterestPlanRulesException:BaseException{
	
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AnalyseFamilyInterestPlanRulesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AnalyseFamilyInterestPlanRulesException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AnalyseFamilyInterestPlanRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

