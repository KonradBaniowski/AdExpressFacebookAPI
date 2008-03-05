#region Informations
// Auteur: Y. R'kaina 
// Date de création: 15/01/2007 
#endregion

using System;
using System.Web.UI;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;

using APPMUIs = TNS.AdExpress.Web.UI.Results.APPM;
using APPMRules = TNS.AdExpress.Web.Rules.Results.APPM;
using WebFnc = TNS.AdExpress.Web.Functions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.BusinessFacade.Results{
	/// <summary>
	/// Construction d'un résultat pour Données de cadrage
	/// </summary>
	public class SectorDataSystem{
		
		#region HTML

		#region GetHtml
		/// <summary>
		/// Accès à la construction du tableau d'APPM
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML du APPM</returns>
		public static ResultTable GetHtml(WebSession webSession){
			try{
			
				#region Paramétrage des dates
				//Formatting date to be used in the tabs which use APPM Press table
				int dateBegin = int.Parse(WebFnc.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
				int dateEnd = int.Parse(WebFnc.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
				#endregion

				#region targets
				//base target
				Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,Right.type.aepmBaseTargetAccess));
				//additional target
				Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,Right.type.aepmTargetAccess));									
				#endregion

				#region Wave
				Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,Right.type.aepmWaveAccess));									
				#endregion

				switch (webSession.CurrentTab){
					case APPM.sectorDataSynthesis:	
						return APPMRules.SectorDataSynthesisRules.GetSynthesisFormattedTable(webSession,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);					
					case APPM.sectorDataAverage:	
						return APPMRules.SectorDataAverageRules.GetAverageFormattedTable(webSession,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);					
					case APPM.sectorDataAffinities:
						return APPMRules.SectorDataAffintiesRules.GetData(webSession,webSession.Source,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idWave);					
					case APPM.sectorDataSeasonality:
						if (webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
							return null;
						else
							return APPMRules.SectorDataSeasonalityRules.GetSeasonalityResultTable(webSession,webSession.Source,idWave,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);
					case APPM.sectorDataInterestFamily:
						return APPMRules.AnalyseFamilyInterestPlanRules.GetFamilyInterestResultTable(webSession,webSession.Source,idWave,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);
					case APPM.sectorDataPeriodicity:
						return APPMRules.PeriodicityPlanRules.GetPeriodicityResultTable(webSession,webSession.Source,idWave,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);
					default:
						return APPMRules.SectorDataSynthesisRules.GetSynthesisFormattedTable(webSession,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);										
				}			
			}
			catch(System.Exception err){
				throw(new APPMBusinessFacadeException("Impossible de calculer le résultat HTML du module données de cadrage ",err));
			}
		}
		#endregion

		#endregion

		#region EXCEL

		#region GetExcel
		/// <summary>
		/// Accès à la construction de Excel d'APPM
		/// </summary>
		/// <param name="page">Page</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <returns>Code Excel du APPM</returns>
        public static string GetExcel(Page page, WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource)
        {
			
			#region Paramétrage des dates
			int dateBegin = int.Parse(WebFnc.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
			int dateEnd = int.Parse(WebFnc.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
			#endregion

			#region targets
			//base target
			Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,Right.type.aepmBaseTargetAccess));
			//additional target
			Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,Right.type.aepmTargetAccess));									
			#endregion

			#region Wave
			Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,Right.type.aepmWaveAccess));									
			#endregion

			try{
				switch (webSession.CurrentTab){
					case TNS.AdExpress.Constantes.FrameWork.Results.APPM.sectorDataSynthesis:	
						return APPMUIs.SectorDataSynthesisUI.GetExcel(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);	
					case TNS.AdExpress.Constantes.FrameWork.Results.APPM.sectorDataAverage:
						return APPMUIs.SectorDataAverageUI.GetExcel(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);	
					default:
						return APPMUIs.SectorDataSynthesisUI.GetExcel(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);
				}
			}
			catch(System.Exception err){
				throw(new APPMBusinessFacadeException("Impossible de calculer le résultat Excel du module données de cadrage",err));
			}
		}
		#endregion

		#endregion

	}
}
