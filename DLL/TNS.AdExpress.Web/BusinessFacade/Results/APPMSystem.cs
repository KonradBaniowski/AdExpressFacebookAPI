#region Informations
/*
Auteur: K. Shehzad 
Date de création: 11/07/2005 
Modification :
	G. RAGNEAU, 27/07/2005, valorisation and efficiency per support or insert
	G. RAGNEAU, 01/08/2005, html code for insertion page
	G. Facon	11/08/2005	New Exception Management
*/
#endregion

using System;
using System.Web.UI;

using TNS.FrameWork.DB.Common;
using CustomerCst=TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;

using APPMUIs = TNS.AdExpress.Web.UI.Results.APPM;
using WebFnc = TNS.AdExpress.Web.Functions;

namespace TNS.AdExpress.Web.BusinessFacade.Results{
	/// <summary>
	/// Construction d'un résultat pour APPM.
	/// </summary>
	public class APPMSystem{
		
		#region HTML

		#region GetHtml
		/// <summary>
		/// Accès à la construction du tableau d'APPM
		/// </summary>
		/// <param name="page">Page</param>
		/// <param name="appmChart">APPMChart web control</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <returns>Code HTML du APPM</returns>
        public static string GetHtml(Page page, object appmChart, WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource)
        {
			try{
			
				#region Paramétrage des dates
				//Formatting date to be used in the tabs which use APPM Press table
				int dateBegin = int.Parse(WebFnc.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
				int dateEnd = int.Parse(WebFnc.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
				#endregion

				#region targets
				//base target
                Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
				//additional target
                Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
				#endregion

				#region Wave
                Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,CustomerCst.Right.type.aepmWaveAccess));									
				#endregion

				switch (webSession.CurrentTab) {
					case APPM.synthesis:	
						return APPMUIs.SynthesisUI.GetHTML(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);					
					case APPM.periodicityPlan:
						#region Appel de la méthode graphique
						if(webSession.Graphics){
							if(webSession.Graphics){
								APPMUIs.APPMChartUI chart = (APPMUIs.APPMChartUI)appmChart;
								APPMUIs.PeriodicityPlanChartUI.PeriodicityPlanChart(chart,webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,true);
								return "";
							}
		
							#endregion
						}else{
							return APPMUIs.PeriodicityPlanUI.PeriodicityPlan(webSession,dataSource,idWave,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,false);
						}
						return "";
                        						
					case APPM.interestFamily:
						#region Appel de la méthode graphique
						if(webSession.Graphics){
							if(webSession.Graphics){
								APPMUIs.APPMChartUI chart = (APPMUIs.APPMChartUI)appmChart;
								APPMUIs.AnalyseFamilyInterestPlanChartUI.InterestFamilyPlanChart(chart,webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,true);
								return "";
							}
							#endregion
						}
						return APPMUIs.AnalyseFamilyInterestPlanUI.InterestFamilyPlan(webSession,dataSource,idWave,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,false);
						
					
					case APPM.supportPlan:
						try{
							if (!page.ClientScript.IsClientScriptBlockRegistered("PopUpInsertion")){
								page.ClientScript.RegisterClientScriptBlock(page.GetType(),"PopUpInsertion",WebFnc.Script.PopUpInsertion(false));
							}
						}
						catch(System.Exception){
							//TODO exception not catched because of the APPM Pdf generation... (page = null)
						}
						return APPMUIs.SupportPlanUI.GetHtml(dataSource,webSession,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,idWave,false);
					case APPM.affinities:
						return APPMUIs.AffinitiesUI.GetHTML(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idWave,false);	
					case APPM.PDVPlan:
						if(webSession.Graphics){
							APPMUIs.APPMChartUI chart = (APPMUIs.APPMChartUI)appmChart;
							APPMUIs.PDVPlanChartUI.ConstructChart(chart,webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,true);
							return "";
						}
						return APPMUIs.PDVPlanUI.GetHTML(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);		
					case APPM.mediaPlanByVersion :
						return  APPMUIs.MediaPlanUI.GetWithVersionHTML(webSession,dataSource,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,false).HTMLCode;		
					case APPM.mediaPlan:
						return  APPMUIs.MediaPlanUI.GetHTML(webSession,dataSource,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,false);		
					default:
						return APPMUIs.SynthesisUI.GetHTML(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);										

				}			
			}
			catch(System.Exception err){
				throw(new APPMBusinessFacadeException("Impossible de calculer le résultat HTML d'APPM ",err));
			}
		}
		#endregion

		#endregion

		#region GetInsertionHtml
		/// <summary>
		/// Generate html UI for APPM Insertions (html page or excel), for a specific media
		/// </summary>
		/// <param name="page">Page</param>
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User session</param>
		/// <param name="idMedia">Media ID to study</param>
		/// <param name="excel">Specify if the webPage is on html or excel format</param>
		/// <returns>UI webpage Code</returns>
        public static string GetInsertionHtml(Page page, TNS.FrameWork.DB.Common.IDataSource dataSource, WebSession webSession, Int64 idMedia, bool excel)
        {
			
			#region Paramétrage des dates
			int dateBegin = int.Parse(WebFnc.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
			int dateEnd = int.Parse(WebFnc.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
			#endregion

			#region targets
			//base target
            Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
			//additional target
            Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
			#endregion

			#region Wave
            Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,CustomerCst.Right.type.aepmWaveAccess));									
			#endregion

			#region Script
			if (!page.ClientScript.IsClientScriptBlockRegistered("openPopUpJustificatif")){
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openPopUpJustificatif", WebFnc.Script.OpenPopUpJustificatif());
			}
			#endregion

			return UI.Results.APPM.InsertionPlanUI.GetHtml(dataSource, webSession , dateBegin, dateEnd, idBaseTarget, idMedia, idWave, excel);
		}
		#endregion

		#region Version Synthesis Html
		/// <summary>
		/// Generate html UI for APPM Version Synthesis  (html page or excel), for a specific Version
		/// </summary>		
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User session</param>
		/// <param name="idVersion">Version ID to study</param>
		/// <param name="firstInsertionDate">Version first insertion date</param>
		/// <param name="excel">Specify if the webPage is on html or excel format</param>
		/// <returns>UI webpage Code</returns>
        public static string GetVersionSynthesisHtml(TNS.FrameWork.DB.Common.IDataSource dataSource, WebSession webSession, string idVersion, string firstInsertionDate, bool excel)
        {
			#region targets
			//base target
            Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
			//additional target
            Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
			#endregion
			
			if(excel)
				return APPMUIs.SynthesisUI.GetExcel(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,idVersion,firstInsertionDate,excel);										
			else
				return APPMUIs.SynthesisUI.GetHTML(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,idVersion,firstInsertionDate,excel);										
		}
		#endregion

		#region HTML Justificatif
		/// <summary>
		/// Méthode pour la construction du code html pour la page du justificatif
		/// </summary>
		/// <param name="page">Page</param>
		/// <param name="dataSource">Source de données</param>
		/// <param name="webSession">Session</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="date">Date</param>
		/// <param name="dateCover">Date faciale</param>
		/// <param name="pageNumber">Numéro de page</param>
		/// <returns>Code html</returns>
		public static string GetProofHtml(Page page, TNS.FrameWork.DB.Common.IDataSource dataSource, WebSession webSession, Int64 idMedia, Int64 idProduct, int date, int dateCover, string pageNumber)
        {
			try{
				return UI.Results.APPM.ProofUI.GetHtml(page, dataSource, webSession, idMedia, idProduct, date, dateCover, pageNumber);
			}
			catch(System.Exception err){
				throw(new APPMBusinessFacadeException("Impossible de calculer le résultat HTML du justificatif",err));
			}
		}
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
			Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
			//additional target
			Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
			#endregion

			#region Wave
			Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,CustomerCst.Right.type.aepmWaveAccess));									
			#endregion

			try {
				switch (webSession.CurrentTab) {
					case TNS.AdExpress.Constantes.FrameWork.Results.APPM.synthesis:	
						return APPMUIs.SynthesisUI.GetExcel(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);	
					case APPM.periodicityPlan:
						return APPMUIs.PeriodicityPlanUI.GetExcel(webSession,dataSource,idWave,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,true);
					case APPM.affinities:
						return APPMUIs.AffinitiesUI.GetExcel(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idWave,true);			
					case APPM.supportPlan:
						return APPMUIs.SupportPlanUI.GetHtml(dataSource,webSession,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,idWave,true);
					case APPM.PDVPlan:
						return APPMUIs.PDVPlanUI.GetExcel(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);
					case APPM.interestFamily:
						return APPMUIs.AnalyseFamilyInterestPlanUI.GetExcel(webSession,dataSource,idWave,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,true);
					case APPM.mediaPlanByVersion :
						return APPMUIs.LocationPlanTypesUI.GetExcel(webSession,dataSource,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget);
					case APPM.mediaPlan:
						return APPMUIs.MediaPlanUI.GetExcel(webSession,dataSource,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget);
					default:
						return APPMUIs.SynthesisUI.GetExcel(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);
				}
			}
			catch(System.Exception err){
				throw(new APPMBusinessFacadeException("Impossible de calculer le résultat Excel d'APPM",err));
			}
			
		}
		#endregion

		#endregion

		#region JPEG
		/// <summary>
		/// This method is used for the jpeg format of the APPM charts
		/// </summary>
		/// <param name="page">Page</param>
		/// <param name="appmChart">APPMChart web control</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <returns>Code HTML du APPM</returns>
        public static void GetJPEG(Page page, APPMUIs.APPMChartUI appmChart, WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource)
        {
			try{
			
				#region Paramétrage des dates
				int dateBegin = int.Parse(WebFnc.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
				int dateEnd = int.Parse(WebFnc.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
				#endregion

				#region targets
				//base target
				Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
				//additional target
				Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
				#endregion

				#region Wave
				Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,CustomerCst.Right.type.aepmWaveAccess));									
				#endregion

				switch (webSession.CurrentTab) {
					case APPM.PDVPlan:
						APPMUIs.PDVPlanChartUI.ConstructChart(appmChart,webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,false);
						break;
					case APPM.periodicityPlan:
						APPMUIs.PeriodicityPlanChartUI.PeriodicityPlanChart(appmChart,webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,false);
						break;
					case APPM.interestFamily:
						APPMUIs.AnalyseFamilyInterestPlanChartUI.InterestFamilyPlanChart(appmChart,webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,false);
						break;
					default:
						break;	
				}
			}
			catch(System.Exception err){
				throw(new APPMBusinessFacadeException("Impossible de génèrer le JPEG pour le APPM Charts",err));
			}
		}
		#endregion
	}
}
