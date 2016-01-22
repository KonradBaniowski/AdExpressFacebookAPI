#region Informations
// Auteur: D. Mussuma
// Date de création: 05/12/2006
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
	/// Accès au résultat du parrainage
	/// </summary>
	public class SponsorshipSystem
	{
		/// <summary>
		/// Accès à la construction du tableau du parrainage 
		/// </summary>		
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML du tableau du parrainage </returns>
		public static ResultTable GetResultTable(WebSession webSession){
			try{

				#region Formattage des dates 
				string periodBeginning = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd");
				string periodEnd = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd");
				#endregion

				//Construction du tableau de données				
				return(WebRules.Results.TvSponsorshipRules.GetData(webSession,periodBeginning,periodEnd));
							
			}
			catch(System.Exception err){
				throw(new WebExceptions.SponsorshipSystemException("Impossible d'obtenir le résultat",err));
			}
		}
	}
}
