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
	/// AnalyseFamilyInterestPlanChartUIException thrown whenever an error occured while generating graphic UI : APPM
	/// </summary>
	public class AnalyseFamilyInterestPlanChartUIException:BaseException{
		#region Constructeur		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AnalyseFamilyInterestPlanChartUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AnalyseFamilyInterestPlanChartUIException(string message):base(message){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AnalyseFamilyInterestPlanChartUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

