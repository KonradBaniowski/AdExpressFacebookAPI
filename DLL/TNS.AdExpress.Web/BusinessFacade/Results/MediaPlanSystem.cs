#region Informations
/*
Auteur: D. V. Mussuma 
Date de création: 28/08/2005 
Modification :	
*/
#endregion
using System;
using System.Web.UI;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using ResultsAPPM=TNS.AdExpress.Web.UI.Results.APPM;
using ModuleName=TNS.AdExpress.Constantes.Web.Module.Name;
using TNS.AdExpress.Web.Exceptions;
using WebFnc = TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using CustomerCst=TNS.AdExpress.Constantes.Customer;

namespace TNS.AdExpress.Web.BusinessFacade.Results
{
	/// <summary>
	/// Construction d'un résultat pour un plan média.
	/// </summary>
	public class MediaPlanSystem
	{
		#region HTML
		/// <summary>
		/// Construction d'un résultat pour un plan média.
		/// </summary>
		/// <param name="page">page</param>
		/// <param name="webSession">session du client</param>
		/// <param name="dataSource">source de données</param>
		/// <param name="zoomDate">date</param>
		/// <param name="url">lien</param>
		/// <returns>HTML tableau de résultats</returns>
        public static string GetHtml(Page page, WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, string zoomDate, string url)
        {
			
			//base target
			Int64 idBaseTarget=0;
			//additional target
			Int64 idAdditionalTarget=0;

			try{		
				switch(webSession.CurrentModule){
					case ModuleName.BILAN_CAMPAGNE :
						//base target
                        idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
						//additional target
                        idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
						try{
                            if (!page.ClientScript.IsClientScriptBlockRegistered("PopUpInsertion")){
                                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "PopUpInsertion", WebFnc.Script.PopUpInsertion(false));
							}
						}
						catch(System.Exception){
							//TODO exception not catched because of the APPM Pdf generation... (page = null)
						}
						return ResultsAPPM.MediaPlanUI.GetZoomHTML(webSession,dataSource,zoomDate,idBaseTarget,idAdditionalTarget,false,url);					
						
					default :
						return GenericMediaPlanAlertUI.GetMediaPlanAlertWithMediaDetailLevelHtmlUI(page,GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevelForZoom(webSession,zoomDate),webSession,zoomDate,url);
						//return MediaPlanAlertUI.GetMediaPlanAlertUI(page,MediaPlanAlertRules.GetFormattedTable(webSession,zoomDate),webSession,zoomDate,url);
				}
			}
			catch(System.Exception err){
				throw(new MediaPlanSystemException("Impossible de calculer le résultat HTML du plan média ",err));
			}
		}
		#endregion

		#region Excel 

		#region Excel Zoom
		/// <summary>
		/// Construction d'un résultat pour un plan média au format excel.
		/// </summary>
		/// <param name="page">page</param>
		/// <param name="webSession">session du client</param>
		/// <param name="dataSource">source de données</param>
		/// <param name="zoomDate">date</param>
		/// <param name="url">lien</param>
		/// <param name="showValue">Montrer les ip dans le tableaux</param>
		/// <returns>HTML tableau de résultats</returns>
        public static string GetExcelUI(Page page, WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource, string zoomDate, string url, bool showValue)
        {
			//base target
			Int64 idBaseTarget=0;
			//additional target
			Int64 idAdditionalTarget=0;

			try{		
				switch(webSession.CurrentModule){
					case ModuleName.BILAN_CAMPAGNE :
						//base target
                        idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
						//additional target
                        idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
						try{
                            if (!page.ClientScript.IsClientScriptBlockRegistered("PopUpInsertion")){
                                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "PopUpInsertion", WebFnc.Script.PopUpInsertion(false));
							}
						}
						catch(System.Exception){
							//TODO exception not catched because of the APPM Pdf generation... (page = null)
						}
						return ResultsAPPM.MediaPlanUI.GetZoomExcel(webSession,dataSource,zoomDate,idBaseTarget,idAdditionalTarget,url);					
						
					default :
						return GenericMediaPlanAlertUI.GetMediaPlanAlertWithMediaDetailLevelExcelUI(GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevelForZoom(webSession, zoomDate),webSession,zoomDate,zoomDate,showValue);
						//return MediaPlanAlertUI.GetMediaPlanAlertExcelUI(MediaPlanAlertRules.GetFormattedTable(webSession, zoomDate),webSession,zoomDate,zoomDate);
				}
			}
			catch(System.Exception err){
				throw(new MediaPlanSystemException("Impossible de calculer le résultat HTML du plan média ",err));
			}
		}
		#endregion
		#endregion
	}
}
