#region Informations
// Auteur: D. V. Mussuma 
// Date de cr�ation: 30/01/2007 
// Date de modification: 
#endregion

using System;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebRules=TNS.AdExpress.Web.Rules;
using WebUI=TNS.AdExpress.Web.UI;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using  WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.FrameWork.WebResultUI;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpress.Web.BusinessFacade.Results
{
	/// <summary>
	/// Acc�s au r�sultat des justificatifs Presse
	/// </summary>
	public class ProofSystem
	{
		/// <summary>
		/// Obtient � la construction du tableau des insertions des justificatifs Presse 
		/// </summary>		
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML du tableau des justificatifs Presse </returns>
		public static ResultTable GetResultTable(WebSession webSession){
			try{

				#region Formattage des dates 
				string periodBeginning = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd");
				string periodEnd = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd");
				#endregion

				//Construction du tableau de r�sultats				
				return(WebRules.Results.ProofRules.GetResultTable(webSession,periodBeginning,periodEnd));
							
			}
			catch(System.Exception err){
				throw(new WebExceptions.ProofSystemException("Impossible d'obtenir le r�sultat",err));
			}
		}
	}
}
