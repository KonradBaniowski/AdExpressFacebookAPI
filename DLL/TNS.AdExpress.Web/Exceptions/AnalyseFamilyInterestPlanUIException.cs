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
	/// AnalyseFamilyInterestPlanUIException thrown whenever an error occured while generating user interface for interest plan : APPM
	/// </summary>
	public class AnalyseFamilyInterestPlanUIException:BaseException{
			
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AnalyseFamilyInterestPlanUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AnalyseFamilyInterestPlanUIException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public AnalyseFamilyInterestPlanUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}