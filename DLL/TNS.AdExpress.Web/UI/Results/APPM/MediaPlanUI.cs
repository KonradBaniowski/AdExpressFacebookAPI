#region Information
//Authors: K.Shehzad, D.Mussuma
//Date of Creation: 24/08/2005
//Date of modification:
//07/08/2006 D. Mussuma integration of Media  plan by Versions
//25/08/2006 Y. Rkaina integration of Media  plan by Versions For export
#endregion
using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Globalization;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Translation;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.FrameWork;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;

using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Date;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Results;

namespace TNS.AdExpress.Web.UI.Results.APPM
{
	/// <summary>
	/// This class is used to generate user interface for plan media in HTML and EXCEL format.
	/// </summary>
	public class MediaPlanUI
	{
		#region HTML

        //Media Plan
		#region Media Plan
        ///// <summary>
        ///// This method generates the HTML for the Media Plan.
        ///// </summary>
        ///// <param name="webSession">clients session</param>
        ///// <param name="dataSource">dataSource for creating Datasets </param>
        ///// <param name="dateBegin">Beginning Date</param>
        ///// <param name="dateEnd">Ending Date</param>
        ///// <param name="baseTarget">Base target</param>
        ///// <param name="additionalTarget">supplementary target</param>
        ///// <param name="excel">boolean to indicate whether the HTML generated is for HTML table or Excel </param>
        ///// <returns>HTML string for the synthesis table</returns>
        //public static string GetHTML(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,bool excel)
        //{
        //    #region Variables
        //    StringBuilder html=null;
        //    object[,] tab=null;
        //    int FIRST_PERIOD_INDEX=0;
        //    int nbColYear=0;
        //    int prevYear=0;
        //    int nbPeriodInYear=0;
        //    string currentCategoryName=string.Empty;
        //    string prevYearString=string.Empty;
        //    string HTML=string.Empty;
        //    string HTML2=string.Empty;
        //    string mediaCSS1=string.Empty;
        //    string mediaCSS2=string.Empty;
        //    string supportCSS=string.Empty;
        //    bool premier=true;
        //    #endregion

        //    #region Constants
        //    const string PLAN_MEDIA_1_CLASSE="p6";
        //    const string PLAN_MEDIA_2_CLASSE="p7";	
        //    const string PLAN_MEDIA_XLS_1_CLASSE="pmmediaxls1";
        //    const string PLAN_MEDIA_XLS_2_CLASSE="pmmediaxls2";
        //    #endregion

        //    #region Styles
        //    if(excel){
        //        mediaCSS1=PLAN_MEDIA_XLS_1_CLASSE;
        //        mediaCSS2=PLAN_MEDIA_XLS_2_CLASSE;
        //        supportCSS="p2excel";
        //    }
        //    else{
        //        mediaCSS1=	PLAN_MEDIA_1_CLASSE;
        //        mediaCSS2=PLAN_MEDIA_2_CLASSE;
        //        supportCSS="p2";
        //    }
        //    #endregion

        //    #region Get Data
        //    try{
        //        tab=TNS.AdExpress.Web.Rules.Results.APPM.MediaPlanRules.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,webSession.DetailPeriod);
        //        #endregion
			
        //    #region HTML
	
        //        if(tab!=null) {
        //            html=new StringBuilder(1000);
        //            int nbColTab=tab.GetLength(1),j,i;

        //            #region number of years
        //            nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
        //            if (nbColYear>0) nbColYear++;
        //            FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_INDEX;			
        //            #endregion
					
        //            #region Colunm labels
        //            html.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0 align=left>\r\n\t<tr>");					
        //            html.Append("\n\t\t\t\t\t<td class=\""+supportCSS+"\" rowspan=2>"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
						
        //            #region Years and months/weeks
        //            prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
        //            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++) {
        //                if(prevYear!=int.Parse(tab[0,j].ToString().Substring(0,4))) {
        //                    if(nbPeriodInYear<3)prevYearString="&nbsp;";
        //                    else prevYearString=prevYear.ToString();							
        //                    HTML2+="<td colspan="+nbPeriodInYear+" class=\"pmannee\">"+prevYearString+"</td>";
        //                    nbPeriodInYear=0;
        //                    prevYear=int.Parse(tab[0,j].ToString().Substring(0,4));

        //                }
        //                if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly) {
        //                    if(excel)
        //                        HTML+="<td class=\"p10\" style=\"width:15pt\">&nbsp;"+TNS.FrameWork.Date.MonthString.Get(int.Parse(tab[0,j].ToString().Substring(4,2)),webSession.SiteLanguage,1)+"&nbsp;</td>";												
        //                    else
        //                        HTML += "<td class=\"p10\" width=\"17px\"><a class=\"p10\" href=\"" + CstWeb.Links.APPM_ZOOM_PLAN_MEDIA + "?idSession=" + webSession.IdSession + "&zoomDate=" + tab[0, j].ToString() + "\">&nbsp;" + TNS.FrameWork.Date.MonthString.Get(int.Parse(tab[0, j].ToString().Substring(4, 2)), webSession.SiteLanguage, 1) + "&nbsp;</td>";												
        //                }
        //                else {	
        //                    if(excel)
        //                        HTML+="<td class=\"p10\" style=\"width:15pt\">&nbsp;"+tab[0,j].ToString().Substring(4,2)+"&nbsp;</td>";
        //                    else
        //                        HTML += "<td class=\"p10\" width=\"17px\"><a class=\"p10\" href=\"" + CstWeb.Links.APPM_ZOOM_PLAN_MEDIA + "?idSession=" + webSession.IdSession + "&zoomDate=" + tab[0, j].ToString() + "\">&nbsp;" + tab[0, j].ToString().Substring(4, 2) + "&nbsp;<a></td>";							
        //                }
        //                nbPeriodInYear++;
        //            }
        //            if(nbPeriodInYear<3)prevYearString="&nbsp;";
        //            else prevYearString=prevYear.ToString();

        //            HTML2+="<td colspan="+nbPeriodInYear+" class=\"pmannee2\">"+prevYearString+"</td>";
        //            html.Append(HTML2+"</tr><tr>");
        //            html.Append(HTML+"</tr>");
        //            if(!excel)html.Append("\r\n\t<tr></tr>");					

        //            #endregion					
					
        //            #endregion				

        //            #region Traversing the table
        //            for(i=1;i<tab.GetLength(0);i++) {
        //                for(j=1;j<tab.GetLength(1);j++) {
        //                    switch(j) {
								
        //                        case FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX:
        //                            if(tab[i,j]!=null) {
        //                                if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.ID_CATEGORY_COUMN_INDEX]!=null && (Int64)tab[i,FrameWorkResultConstantes.MediaPlanAPPM.ID_CATEGORY_COUMN_INDEX]==-1)
        //                                    html.Append("\r\n\t<tr >\r\n\t\t<td class=\"pmtotal\" nowrap>&nbsp;"+tab[i,j]+"</td>");
        //                                else 
        //                                    html.Append("\r\n\t<tr >\r\n\t\t<td class=\"pmcategory\" nowrap>&nbsp;"+tab[i,j]+"</td>");
        //                                j=j+3;
										
        //                            }
        //                            break;

        //                        case FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX:
        //                            if(tab[i,j]!=null) {
        //                                if(premier) {
        //                                    html.Append("\r\n\t<tr >\r\n\t\t<td class=\""+mediaCSS1+"\" nowrap>&nbsp;&nbsp;"+tab[i,j]+"</td>");
        //                                    premier=false;
        //                                }
        //                                else {
        //                                    html.Append("\r\n\t<tr >\r\n\t\t<td class=\""+mediaCSS2+"\" nowrap>&nbsp;&nbsp;"+tab[i,j]+"</td>");										
        //                                    premier=true;
        //                                }
        //                                j=j+1;
        //                            }
									
        //                            break;
        //                        case FrameWorkResultConstantes.MediaPlanAPPM.ID_MEDIA_COLUMN_INDEX:									
        //                        case FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX:
        //                            break;
        //                        default:
								
        //                            if(tab[i,j]==null && (tab[i,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]!=null || tab[i,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]!=null )) {
        //                                html.Append("<td class=\"p3\">&nbsp;</td>");
        //                                break;
        //                            }
        //                            if((tab[i,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]!=null || tab[i,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]!=null) && (tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlan.graphicItemType))) {
        //                                switch((FrameWorkResultConstantes.MediaPlan.graphicItemType)tab[i,j]) {
        //                                    case FrameWorkResultConstantes.MediaPlan.graphicItemType.present:
        //                                        html.Append("<td class=\"p4\">&nbsp;</td>");
        //                                        break;
        //                                    case FrameWorkResultConstantes.MediaPlan.graphicItemType.extended:
        //                                        html.Append("<td class=\"p5\">&nbsp;</td>");
        //                                        break;
        //                                    default:
        //                                        html.Append("<td class=\"p3\">&nbsp;</td>");
        //                                        break;
        //                                }
        //                            }
        //                            break;
        //                    }
							
        //                }
        //                if((tab[i,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]!=null || tab[i,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]!=null))
        //                    html.Append("</tr>");
        //            }
        //            html.Append("</table>");	
        //            #endregion			

								
        //        }
        //    }
        //    catch(System.Exception e){
        //        throw(new WebExceptions.APPMMediaPlanUIExcpetion("Error while constructing the HTML table of Media plan ",e));
        //    }
        //    #endregion

        //    #region no data
        //    if(html==null||html.Length<=0)
        //        return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
				
        //    #endregion

        //    return html.ToString();

        //}
		#endregion

        //Media Plan Zoom
		#region Media Plan Zoom

        ///// <summary>
        ///// This method generates the HTML for the Media Plan by day.
        ///// </summary>
        ///// <param name="webSession">clients session</param>
        ///// <param name="dataSource">dataSource for creating Datasets </param>		
        ///// <param name="baseTarget">Base target</param>
        ///// <param name="additionalTarget">supplementary target</param>
        ///// <param name="excel">boolean to indicate whether the HTML generated is for HTML table or Excel </param>
        ///// <param name="url">link to next page</param>
        ///// <param name="zoomDate">detail date</param>
        ///// <returns>HTML string for the synthesis table</returns>
        //public static string GetZoomHTML(WebSession webSession,IDataSource dataSource, string zoomDate,Int64 baseTarget,Int64 additionalTarget,bool excel,string url) {
			
        //    #region Variables
        //    StringBuilder html=null;
        //    object[,] tab=null;
        //    int FIRST_PERIOD_INDEX=0;
        //    int nbColYear=0;
        //    int prevYear=0;
        //    int nbPeriodInYear=0;
        //    string currentCategoryName=string.Empty;
        //    string prevYearString=string.Empty;
        //    string HTML=string.Empty;
        //    string HTML2=string.Empty;
        //    string mediaCSS1=string.Empty;
        //    string mediaCSS2=string.Empty;
        //    string supportCSS=string.Empty;
        //    bool premier=true;
						
        //    //Formatting date to be used in the tabs which use APPM Press table
        //    int dateBegin = 0;
        //    int dateEnd = 0;
					
			
        //    #endregion

        //    #region Constants
        //    const string PLAN_MEDIA_1_CLASSE="p6";
        //    const string PLAN_MEDIA_2_CLASSE="p7";	
        //    const string PLAN_MEDIA_XLS_1_CLASSE="pmmediaxls1";
        //    const string PLAN_MEDIA_XLS_2_CLASSE="pmmediaxls2";
        //    #endregion

        //    #region Styles
        //    if(excel){
        //        mediaCSS1=PLAN_MEDIA_XLS_1_CLASSE;
        //        mediaCSS2=PLAN_MEDIA_XLS_2_CLASSE;
        //        supportCSS="p2excel";
        //    }
        //    else{
        //        mediaCSS1=	PLAN_MEDIA_1_CLASSE;
        //        mediaCSS2=PLAN_MEDIA_2_CLASSE;
        //        supportCSS="p2";
        //    }
        //    #endregion

								
        //    //Formatting date to be used in the tabs which use APPM Press table
        //    dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(zoomDate,webSession.PeriodType).ToString("yyyyMMdd"));
        //    dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(zoomDate,webSession.PeriodType).ToString("yyyyMMdd"));
	

        //    #region Get Data
        //    try{
        //        tab=TNS.AdExpress.Web.Rules.Results.APPM.MediaPlanRules.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,WebConstantes.CustomerSessions.Period.DisplayLevel.dayly);
        //        #endregion
			
        //        #region HTML
	
        //        if(tab!=null) {
        //            html=new StringBuilder(1000);
        //            int nbColTab=tab.GetLength(1),j,i;

        //            #region number of years
        //            nbColYear = int.Parse(dateBegin.ToString().Substring(0,4)) - int.Parse(dateEnd.ToString().Substring(0,4));
        //            if (nbColYear>0) nbColYear++;
        //            FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_INDEX;			
        //            #endregion
					
        //            #region Colunm labels
        //            html.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0 align=left>\r\n\t<tr>");					
        //            if(!excel){
        //                html.Append("\n\t\t\t\t\t<td class=\""+supportCSS+"\" rowspan=4>"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
        //                html.Append("\n\t\t\t\t\t<td class=\""+supportCSS+"\" rowspan=4>"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
        //            }else{
        //                html.Append("\n\t\t\t\t\t<td class=\""+supportCSS+"\" rowspan=3>"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
        //            }
        //            #region Years and days
        //            prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
        //            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++) {

        //                if(excel)
        //                    HTML+="<td class=\"p10\" style=\"width:15pt\">&nbsp;"+tab[0,j].ToString().Substring(6,2)+"&nbsp;</td>";
        //                else									
        //                    HTML+="<td class=\"p10\" width=\"17px\">&nbsp;"+tab[0,j].ToString().Substring(6,2)+"&nbsp;</td>";							

        //                nbPeriodInYear++;
        //            }
        //            if(nbPeriodInYear<3)prevYearString="&nbsp;";
        //            else prevYearString=prevYear.ToString();

        //            #region Période précédente
        //            if (webSession.PeriodBeginningDate != zoomDate && !excel) {
        //                if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
        //                    html.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
        //                        (new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(-1).ToString("yyyyMM")
        //                        +"\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
        //                }
        //                else{
        //                    html.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
        //                    AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
        //                    tmp.SubWeek(1);
        //                    if(tmp.Week.ToString().Length<2)html.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
        //                    else html.Append(tmp.Year.ToString() + tmp.Week.ToString());
        //                    html.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
        //                }
        //            }
        //            else
        //                html.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\">&nbsp;</td>");
        //            #endregion
        //            nbPeriodInYear=nbPeriodInYear-2;
        //            HTML2+="<td colspan="+nbPeriodInYear+" class=\"pmannee2\">"+WebFunctions.Dates.getPeriodTxt(webSession, zoomDate)+"</td>";				
        //            html.Append(HTML2);
        //            #region Période suivante
        //            if (webSession.PeriodEndDate != zoomDate && !excel){
        //                if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
        //                    html.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
        //                        (new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(1).ToString("yyyyMM")
        //                        +"\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
        //                }
        //                else{
        //                    html.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
        //                    AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
        //                    tmp.Increment();
        //                    if(tmp.Week.ToString().Length<2)html.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
        //                    else html.Append(tmp.Year.ToString() + tmp.Week.ToString());
        //                    html.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
        //                }
        //            }
        //            else
        //                html.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\">&nbsp;</td>");					
        //            #endregion
        //            html.Append("</tr>");
        //            html.Append("<tr>"+HTML+"</tr>");
        //            if(!excel)html.Append("\r\n\t<tr></tr>");					

        //            #endregion	
								
        //            #region Périodes
					
        //            string dayClass="";
        //            string day;
        //            DateTime date = DateTime.Now;
        //            html.Append("\r\n\t<tr>");
        //            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
						
        //                date = new DateTime(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)),int.Parse(tab[0,j].ToString().Substring(6,2)));
        //                day=TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,date.DayOfWeek.ToString()).ToString();
        //                if(day.Equals(GestionWeb.GetWebWord(545,webSession.SiteLanguage))
        //                    || day.Equals(GestionWeb.GetWebWord(546,webSession.SiteLanguage))
        //                    ){
        //                    if(!excel)dayClass="p132";
        //                    else dayClass="p133";
        //                }
        //                else{
        //                    if(!excel)dayClass="p131";
        //                    else dayClass="p132";							
        //                }	
        //                html.Append("<td class=\""+dayClass+"\">"+TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,date.DayOfWeek.ToString())+"</td>");
        //            }
        //            html.Append("\r\n\t</tr>");
        //            #endregion

				
					
        //            #endregion				

        //            #region Traversing the table
        //            for(i=1;i<tab.GetLength(0);i++) {
        //                for(j=1;j<tab.GetLength(1);j++) {
        //                    switch(j) {
								
        //                        case FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX:
        //                            if(tab[i,j]!=null) {
        //                                if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.ID_CATEGORY_COUMN_INDEX]!=null && (Int64)tab[i,FrameWorkResultConstantes.MediaPlanAPPM.ID_CATEGORY_COUMN_INDEX]==-1){
        //                                    html.Append("\r\n\t<tr >\r\n\t\t<td class=\"pmtotal\" nowrap>&nbsp;"+tab[i,j]+"</td>");
        //                                    if(!excel) html.Append("\r\n\t\n\t\t<td class=\"pmtotal\" nowrap>&nbsp;</td>");
        //                                }
        //                                else {
        //                                    html.Append("\r\n\t<tr >\r\n\t\t<td class=\"pmcategory\" nowrap>&nbsp;"+tab[i,j]+"</td>");
        //                                    if(!excel) html.Append("\r\n\t\n\t\t<td class=\"pmcategory\" nowrap>&nbsp;</td>");
        //                                }
        //                                j=j+3;
										
        //                            }
        //                            break;

        //                        case FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX:
        //                            if(tab[i,j]!=null) {
        //                                if(premier) {
        //                                    html.Append("\r\n\t<tr >\r\n\t\t<td class=\""+mediaCSS1+"\" nowrap>&nbsp;&nbsp;"+tab[i,j]+"</td>");
        //                                    if(!excel){
        //                                        html.Append("\r\n\t\t<td class=\""+mediaCSS1+"\" align=\"center\" nowrap><a class=\"acl1\" href=\"javascript:PopUpInsertion('"+webSession.IdSession + "','"
        //                                            + tab[i,FrameWorkResultConstantes.MediaPlanAPPM.ID_MEDIA_COLUMN_INDEX] + "');\"> <img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
        //                                    }
        //                                    premier=false;
        //                                }
        //                                else {
        //                                    html.Append("\r\n\t<tr >\r\n\t\t<td class=\""+mediaCSS2+"\" nowrap>&nbsp;&nbsp;"+tab[i,j]+"</td>");										
        //                                    if(!excel){ html.Append("\r\n\t\t<td class=\""+mediaCSS2+"\" align=\"center\" nowrap><a class=\"acl1\" href=\"javascript:PopUpInsertion('"+webSession.IdSession + "','"
        //                                                    + tab[i,FrameWorkResultConstantes.MediaPlanAPPM.ID_MEDIA_COLUMN_INDEX] + "');\"> <img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
        //                                    }
        //                                    premier=true;
        //                                }
        //                                j=j+1;
        //                            }
									
        //                            break;
        //                        case FrameWorkResultConstantes.MediaPlanAPPM.ID_MEDIA_COLUMN_INDEX:									
        //                        case FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX:
        //                            break;
        //                        default:
								
        //                            if(tab[i,j]==null && (tab[i,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]!=null || tab[i,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]!=null )) {
        //                                html.Append("<td class=\"p3\">&nbsp;</td>");
        //                                break;
        //                            }
        //                            if((tab[i,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]!=null || tab[i,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]!=null) && (tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlan.graphicItemType))) {
        //                                switch((FrameWorkResultConstantes.MediaPlan.graphicItemType)tab[i,j]) {
        //                                    case FrameWorkResultConstantes.MediaPlan.graphicItemType.present:
        //                                        html.Append("<td class=\"p4\">&nbsp;</td>");
        //                                        break;
        //                                    case FrameWorkResultConstantes.MediaPlan.graphicItemType.extended:
        //                                        html.Append("<td class=\"p5\">&nbsp;</td>");
        //                                        break;
        //                                    default:
        //                                        html.Append("<td class=\"p3\">&nbsp;</td>");
        //                                        break;
        //                                }
        //                            }
        //                            break;
        //                    }
							
        //                }
        //                if((tab[i,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]!=null || tab[i,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]!=null))
        //                    html.Append("</tr>");
        //            }
        //            html.Append("</table>");	
        //            #endregion			

								
        //        }
        //    }
        //    catch(System.Exception e){
        //        throw(new WebExceptions.APPMMediaPlanUIExcpetion(" Error while constructing the HTML table of Media plan ",e));
        //    }
        //    #endregion

        //    #region no data
        //    if(html==null||html.Length<=0)
        //        return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
				
        //    #endregion

        //    return html.ToString();

        //}

		#endregion

		#region Media Plan by version
		/// <summary>
		/// This method generates the HTML for the Media Plan.
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <param name="isExported">boolean to indicate whether the HTML generated is for HTML table or Export in any other format </param>
		/// <returns>HTML string for the synthesis table</returns>
		public static MediaPlanResultData GetWithVersionHTML(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,bool isExported) {
			
			#region Variables
			object[,] tab=null;
			int sloganIndex=-1;
			int colorNumberToUse=0;
			int colorItemIndex=1;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
			StringBuilder t = new StringBuilder(10000);
			string presentClass="p4";
			string extendedClass="p5";
			string stringItem="";
			string mediaCSS1=string.Empty;
			string mediaCSS2=string.Empty;
			string supportCSS=string.Empty;
			int nbColTab,j,i;
			int nbline;
			string oldMonthDate="-1",monthsHtml=""; //monthDate=""
			string oldWeekDate="-1",weeksHtml=""; //weekDate="",
			int monthColspan=0,weekColspan=0;//,versionColSpan=0;
			bool firstMonth=true,firstWeek=true;
			MediaPlanResultData mediaPlanResultData=null;
			string classe="L1";
			Int64 oldIdL2=0,oldIdL1=0, oldIdL3=0; //,oldIdL4=0;
			bool isFirstVersionOfMedia=false;
			int nbDaysInMonth=0,nbDaysInWeek=0,nbWeek=0;
			int h=0;
			bool isNewVersionLine=false;
			int boderRightWidth=1;
			int nbPixelByDay=4;
			try{webSession.SloganColors.Add((Int64)0,"pm0");}catch(System.Exception){}
            //int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_COLUMN_INDEX;
			
			
			#endregion
			
			#region Constants
			const string PLAN_MEDIA_1_CLASSE="p6";
			const string PLAN_MEDIA_2_CLASSE="p7";	
			
			#endregion

			#region Styles
			
				mediaCSS1=	PLAN_MEDIA_1_CLASSE;
				mediaCSS2=PLAN_MEDIA_2_CLASSE;
				supportCSS="p2";
			
			#endregion


			try{

				//Get Data
				tab=TNS.AdExpress.Web.Rules.Results.APPM.MediaPlanRules.GetFormattedTable(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,WebConstantes.CustomerSessions.Period.DisplayLevel.dayly);
				
				#region No Data
				 mediaPlanResultData=new MediaPlanResultData();
				if(tab==null || tab.GetLength(0)==0){
//					mediaPlanResultData.HTMLCode="<div  ><table  ><tr><td class=\"txtViolet11Bold\" align=center>"+GestionWeb.GetWebWord(177,webSession.SiteLanguage);
//					mediaPlanResultData.HTMLCode+="</td></tr></table></div>";
					mediaPlanResultData.HTMLCode="";
					return(mediaPlanResultData);
				}
				#endregion

				#region Beginning table construction

				 nbColTab=tab.GetLength(1);
				nbline=tab.GetLength(0);
				t.Append(" <!-- Debut tableau plan media par version --->");
				t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0 align=left>\r\n\t<tr>");
				t.Append("\r\n\t\t<td rowspan=\"2\"   class=\""+supportCSS+"\" nowrap>"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
				t.Append("\r\n\t\t<td rowspan=\"2\" class=\""+supportCSS+"\" nowrap>"+GestionWeb.GetWebWord(1994,webSession.SiteLanguage)+"</td>");
//				if(!isExported)t.Append("\r\n\t\t<td  rowspan=\"2\" class=\""+supportCSS+"\" nowrap>"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
				
				#region Dates labelling
				//Dates labelling
				for( h=FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_COLUMN_INDEX;h<nbColTab;h++){					
					//Months labelling
					if(firstMonth){
						monthsHtml+="<td class=\"pmX\" height=\"100%\" >";
						monthsHtml+="<table cellpadding=\"0\" cellspacing=\"0\" height=\"100%\">";
						monthsHtml+="<tr width=\"100%\">";
					}
					if(firstWeek){
						weeksHtml+="<td class=\"pmX\" height=\"100%\" >";
						weeksHtml+="<table cellpadding=\"0\" cellspacing=\"0\" height=\"100%\">";
						weeksHtml+="<tr width=\"100%\">";
					}

					if(tab[0,h].ToString().Length>0 && !tab[0,h].ToString().Equals(oldMonthDate) && !firstMonth){
						monthColspan=(monthColspan*nbPixelByDay)-boderRightWidth;
						monthsHtml+="\r\n\t\t<td   width=\""+monthColspan+"px\" class=\"pmb\" align=center  nowrap>"; 
						if(nbDaysInMonth>4)monthsHtml+=MonthString.GetCharacters(int.Parse(oldMonthDate.ToString().Substring(4,2)),cultureInfo,3)+" "+oldMonthDate.ToString().Substring(2,2);
						else
                            monthsHtml += "<font class=\"txtViolet\">.</font>";
						monthsHtml+="</td>";
						
						monthColspan=0;
						nbDaysInMonth=0;
					}
					oldMonthDate = tab[0,h].ToString();
					firstMonth=false;
					monthColspan++;
					nbDaysInMonth++;
					//Weeks  labelling
					
					if(firstWeek)weeksHtml+="<tr>";
					if(tab[1,h].ToString().Length>0 && !tab[1,h].ToString().Equals(oldWeekDate) && !firstWeek){
						weekColspan=(weekColspan*nbPixelByDay)-boderRightWidth;
						weeksHtml+="\r\n\t\t<td width=\""+weekColspan+"px\" class=\"pmc\"   align=center nowrap>"; 
						if(nbDaysInWeek>2)weeksHtml+=oldWeekDate.ToString(); //GestionWeb.GetWebWord(848, webSession.SiteLanguage) +"&nbsp;"+
                        else weeksHtml += "<font class=\"txtMediumPurple\">.</font>";
						weeksHtml+="</td>";
						weekColspan=0;
						nbDaysInWeek=0;
						nbWeek++;
					}
					oldWeekDate = tab[1,h].ToString();
					firstWeek=false;
					weekColspan++;
					nbDaysInWeek++;
				}
				monthColspan=(monthColspan*nbPixelByDay)-boderRightWidth;
//				if(nbWeek==0)//decrease font size if only one week selected
//					monthsHtml+="\r\n\t\t<td align=center STYLE=\"font-size: 8px;\" class=\"pmb\" width=\""+monthColspan+"px\"  nowrap>"; //class=\""+supportCSS+"\" 
//				else
					monthsHtml+="\r\n\t\t<td align=center  class=\"pmb\" width=\""+monthColspan+"px\"  nowrap>"; //class=\""+supportCSS+"\" 
				if(nbDaysInMonth>4) {
					
					monthsHtml+=MonthString.GetCharacters(int.Parse(oldMonthDate.ToString().Substring(4,2)),cultureInfo,3)+" "+oldMonthDate.ToString().Substring(2,2);
				}
				else
                    monthsHtml += "<font class=\"txtViolet\">.</font>";
				monthsHtml+="</td>";
				monthsHtml+="</tr></table></td>";//On ferme le tableau contenant les cellules affichant les mois
				monthsHtml+="</tr>";
				
				weekColspan=(weekColspan*nbPixelByDay)-boderRightWidth;
				weeksHtml+="\r\n\t\t<td   class=\"pmc\" width=\""+weekColspan+"px\" align=center nowrap>";// class=\"p7bold\"
				if(nbDaysInWeek>2)weeksHtml+=oldWeekDate.ToString(); //GestionWeb.GetWebWord(848, webSession.SiteLanguage) +"&nbsp;"+
                else weeksHtml += "<font class=\"txtMediumPurple\">.</font>";
				weeksHtml+="</td>";	
				weeksHtml+="</tr></table></td>";//On ferme le tableau contenant les cellules affichant les semaines
				weeksHtml+="</tr>";

				t.Append(monthsHtml);
				t.Append(weeksHtml);
//				if(!isExported)t.Append("\r\n\t<tr><td colspan=\""+h+2+"\" nowrap></tr>");	
				#endregion

				#region Traversing the table
				
				i=0;
				
					sloganIndex=GetSloganIdIndex(webSession);
					for(i=3;i<nbline;i++){

						#region Gestion des couleurs des accroches
						if(sloganIndex!=-1	&& tab[i,sloganIndex]!=null && 
							((webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan)==webSession.GenericMediaDetailLevel.GetNbLevels)||
							(webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan)<webSession.GenericMediaDetailLevel.GetNbLevels && tab[i,sloganIndex+1]==null))){
							if(!webSession.SloganColors.ContainsKey((Int64)tab[i,sloganIndex])){
								colorNumberToUse=(colorItemIndex%30)+1;
								webSession.SloganColors.Add((Int64)tab[i,sloganIndex],"pm"+colorNumberToUse.ToString());																
								mediaPlanResultData.VersionsDetail.Add((Int64)tab[i,sloganIndex],new VersionItem((Int64)tab[i,sloganIndex],"pm"+colorNumberToUse.ToString())); 
								colorItemIndex++;
								
							}
							if((Int64)tab[i,sloganIndex]!=0 && !mediaPlanResultData.VersionsDetail.ContainsKey((Int64)tab[i,sloganIndex])){								
								mediaPlanResultData.VersionsDetail.Add((Int64)tab[i,sloganIndex],new VersionItem((Int64)tab[i,sloganIndex],webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString()));
								
							}
							presentClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
							extendedClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
							stringItem="";
						}
						else{
							stringItem="";
							presentClass=extendedClass="pma";

						}
						#endregion
						isNewVersionLine=true;
						for(j=0;j<nbColTab;j++){
							switch(j){
									#region Level 1
								case FrameWorkResultConstantes.MediaPlanAPPM.L1_COLUMN_INDEX:
								
									if(tab[i,j]!=null && tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]!=null && 
										oldIdL1!=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX].ToString())){	
										classe="pmtotal";																																					
										t.Append("\r\n\t<tr>\r\n\t\t<td  class=\""+classe+"\" nowrap>"+GestionWeb.GetWebWord(1401,webSession.SiteLanguage)+"</td><td width=\"100px\" class=\""+classe+"\" nowrap>&nbsp;</td>");	//L1 is called "Total" for APPM		<td width=\"50px\"class=\""+classe+"\" nowrap>&nbsp;</td>																
										if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]!=null)oldIdL1=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX].ToString());										
										isFirstVersionOfMedia=false;
									}
									break;
									#endregion

									#region Level 2
								case FrameWorkResultConstantes.MediaPlanAPPM.L2_COLUMN_INDEX:
									if(tab[i,j]!=null && tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX]!=null 
										&& oldIdL2!=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX].ToString())){
										classe="L2";																				
										t.Append("\r\n\t<tr>\r\n\t\t<td width=\"100px\" class=\""+classe+"\" nowrap>&nbsp;"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L2_COLUMN_INDEX]+"</td><td width=\"100px\" class=\""+classe+"\" nowrap>&nbsp;</td>"); //<td width=\"50px\" class=\""+classe+"\" nowrap>&nbsp;</td>
										if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX]!=null)oldIdL2=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX].ToString());										
										isFirstVersionOfMedia=false;
									}
									break;
									#endregion

									#region Level 3
								case  FrameWorkResultConstantes.MediaPlanAPPM.L3_COLUMN_INDEX:
									if(tab[i,j]!=null && tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]!=null 
										&& oldIdL3!=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX].ToString())){																														
										classe="L3";	
										t.Append("\r\n\t<tr>\r\n\t\t<td  class=\""+classe+"\" nowrap>&nbsp;"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L3_COLUMN_INDEX]+"</td>");
										if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]!=null)oldIdL3=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX].ToString());										
										isFirstVersionOfMedia=true;
									}
									break;
									#endregion

									#region Level 4
								case  FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX:
									if(tab[i,j]!=null && tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX]!=null ){
																			
										if (isFirstVersionOfMedia)//Version should be presented on the same line that the media
											t.Append("\r\n\t\t<td class=\""+classe+"\" nowrap>"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX]+"</td>"); //<td width=\"50px\" align=\"center\" class=\""+classe+"\" nowrap><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX]+",-1,1','');\" nowrap><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>
										else{//next Version of the media
											classe="L4";
											t.Append("\r\n\t<tr><td  class=\""+classe+"\" nowrap>&nbsp;</td>\r\n\t\t<td width=\"100px\" class=\""+classe+"\" nowrap>"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX]+"</td>"); //<td width=\"50px\" align=\"center\" class=\""+classe+"\" nowrap><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX]+",-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>
										}
										isFirstVersionOfMedia=false;
									}
									break;
									#endregion

								case FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX:
									break;

								default:

									if(!isFirstVersionOfMedia && tab[i,j]!=null && tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlan.graphicItemType)){
										switch((FrameWorkResultConstantes.MediaPlan.graphicItemType)tab[i,j]){
											case FrameWorkResultConstantes.MediaPlanAPPM.graphicItemType.present:
												if(isNewVersionLine){
													t.Append("<td class=\"pmX\" height=\"100%\" >");
													t.Append("<table cellpadding=\"0\" cellspacing=\"0\" height=\"100%\">");
													t.Append("<tr width=\"100%\">");													
													isNewVersionLine=false;
												}
												t.Append("<td class=\""+presentClass+"\"  style=\"width:4px;\" nowrap>"+stringItem+"</td>"); 
												break;
											case FrameWorkResultConstantes.MediaPlanAPPM.graphicItemType.extended:
												if(isNewVersionLine){
													t.Append("<td class=\"pmX\" height=\"100%\" >");
													t.Append("<table cellpadding=\"0\" cellspacing=\"0\" height=\"100%\">");
													t.Append("<tr width=\"100%\">");												
													isNewVersionLine=false;
												}
												t.Append("<td class=\""+extendedClass+"\" style=\"width:4px;\" nowrap></td>"); 
												break;
											default:
												if(isNewVersionLine){
													t.Append("<td class=\"pmX\" height=\"100%\" >");
													t.Append("<table cellpadding=\"0\" cellspacing=\"0\" height=\"100%\">");
													t.Append("<tr width=\"100%\">");													
													isNewVersionLine=false;
												}
												t.Append("<td class=\"pma\" nowrap></td>");
												break;
										}
									}
									break;
							}
						}
						if(!isFirstVersionOfMedia){
							t.Append("</tr></table></td>");//ON ferme le tableau contenant les cellules affichant l'activité publicitaire
							t.Append("</tr>"); //ON ferme la ligne c'est  le niveau support
						}
					}
				
				
				#endregion

				//Fin du tableau
				t.Append("</table>");
				t.Append(" <!-- Fin tableau plan media par version --->");
				#endregion
					
				#region debug
				#if(DEBUG)
			
//				int i,j,nbline,nbCol;
//				nbline=tab.GetLength(0);
//				nbCol=tab.GetLength(1);
//			
//				string HTML="<table><tr>";
//
//					for(i=0;i<nbline;i++){
//						for(j=0;j<nbCol;j++){
//							if(tab[i,j]!=null) {							
//								HTML+="<td>"+tab[i,j].ToString()+"</td>";
//							}
//							else HTML+="<td>&nbsp;</td>";
//						}
//						HTML+="</tr><tr>";
//					}
//				
//				HTML+="</tr></table>";
//				return HTML;
				#endif
				#endregion
			}
			catch(System.Exception e){
				throw(new WebExceptions.APPMMediaPlanUIExcpetion("Error while constructing the HTML table of Media plan with Versions ",e));
			}
			
			// On vide le tableau
			tab=null;			
			mediaPlanResultData.HTMLCode=t.ToString();
			return(mediaPlanResultData);		
		}
		#endregion 

		#region Media Plan by version For Export
		/// <summary>
		/// This method generates the HTML for the Media Plan.
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <param name="isExported">boolean to indicate whether the HTML generated is for HTML table or Export in any other format </param>
		/// <param name="htmlHeader">Entête du fichier HTML</param>
		/// <param name="partieHTML">Liste des parties du code HTML</param>
		/// <returns>HTML string for the synthesis table</returns>
		public static MediaPlanResultData GetWithVersionExportHTML(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,bool isExported,ref System.Text.StringBuilder htmlHeader, ref ArrayList partieHTML) 
		{
			
			#region Variables
			object[,] tab=null;
			int sloganIndex=-1;
			int colorNumberToUse=0;
			int colorItemIndex=1;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
			StringBuilder t = new StringBuilder(10000);
			string presentClass="p4";
			string extendedClass="p5";
			string stringItem="";
			string mediaCSS1=string.Empty;
			string mediaCSS2=string.Empty;
			string supportCSS=string.Empty;
			int nbColTab,j,i;
			int nbline;
			string oldMonthDate="-1",monthsHtml=""; //monthDate=""
			string oldWeekDate="-1",weeksHtml=""; //weekDate="",
			int monthColspan=0,weekColspan=0;//,versionColSpan=0;
			bool firstMonth=true,firstWeek=true;
			MediaPlanResultData mediaPlanResultData=null;
			string classe="L1";
			Int64 oldIdL2=0,oldIdL1=0, oldIdL3=0; //,oldIdL4=0;
			bool isFirstVersionOfMedia=false;
			int nbDaysInMonth=0,nbDaysInWeek=0,nbWeek=0;
			int h=0;
			bool isNewVersionLine=false;
			int boderRightWidth=1;
			int nbPixelByDay=4;
			try{webSession.SloganColors.Add((Int64)0,"pm0");}
			catch(System.Exception){}
            //int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_COLUMN_INDEX;
			
			
			#endregion
			
			#region Constants
			const string PLAN_MEDIA_1_CLASSE="p6";
			const string PLAN_MEDIA_2_CLASSE="p7";	
			
			#endregion

			#region Styles
			
			mediaCSS1=	PLAN_MEDIA_1_CLASSE;
			mediaCSS2=PLAN_MEDIA_2_CLASSE;
			supportCSS="p2";
			
			#endregion


			try
			{

				//Get Data
				tab=TNS.AdExpress.Web.Rules.Results.APPM.MediaPlanRules.GetFormattedTable(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,WebConstantes.CustomerSessions.Period.DisplayLevel.dayly);
				
				#region No Data
				mediaPlanResultData=new MediaPlanResultData();
				if(tab==null || tab.GetLength(0)==0)
				{
					//					mediaPlanResultData.HTMLCode="<div  ><table  ><tr><td class=\"txtViolet11Bold\" align=center>"+GestionWeb.GetWebWord(177,webSession.SiteLanguage);
					//					mediaPlanResultData.HTMLCode+="</td></tr></table></div>";
					mediaPlanResultData.HTMLCode="";
					return(mediaPlanResultData);
				}
				#endregion

				#region Beginning table construction

				nbColTab=tab.GetLength(1);
				nbline=tab.GetLength(0);
				t.Append(" <!-- Debut tableau plan media par version --->");
				t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0 align=left>\r\n\t<tr>");
				t.Append("\r\n\t\t<td rowspan=\"2\" width=\"185px\"  class=\""+supportCSS+"\" nowrap>"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
				t.Append("\r\n\t\t<td rowspan=\"2\" class=\""+supportCSS+"\" nowrap>"+GestionWeb.GetWebWord(1994,webSession.SiteLanguage)+"</td>");
				//				if(!isExported)t.Append("\r\n\t\t<td  rowspan=\"2\" class=\""+supportCSS+"\" nowrap>"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
				
				#region Dates labelling
				//Dates labelling
				for( h=FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_COLUMN_INDEX;h<nbColTab;h++)
				{					
					//Months labelling
					if(firstMonth)
					{
						monthsHtml+="<td class=\"pmX\" height=\"100%\" >";
						monthsHtml+="<table cellpadding=\"0\" cellspacing=\"0\" height=\"100%\">";
						monthsHtml+="<tr width=\"100%\">";
					}
					if(firstWeek)
					{
						weeksHtml+="<td class=\"pmX\" height=\"100%\" >";
						weeksHtml+="<table cellpadding=\"0\" cellspacing=\"0\" height=\"100%\">";
						weeksHtml+="<tr width=\"100%\">";
					}

					if(tab[0,h].ToString().Length>0 && !tab[0,h].ToString().Equals(oldMonthDate) && !firstMonth)
					{
						monthColspan=(monthColspan*nbPixelByDay)-boderRightWidth;
						monthsHtml+="\r\n\t\t<td   width=\""+monthColspan+"px\" class=\"pmb\" align=center  nowrap>"; 
						if(nbDaysInMonth>4)monthsHtml+=MonthString.GetCharacters(int.Parse(oldMonthDate.ToString().Substring(4,2)),cultureInfo,3)+" "+oldMonthDate.ToString().Substring(2,2);
						else 
							monthsHtml+="<font color=\"#644883\">.</font>";
						monthsHtml+="</td>";
						
						monthColspan=0;
						nbDaysInMonth=0;
					}
					oldMonthDate = tab[0,h].ToString();
					firstMonth=false;
					monthColspan++;
					nbDaysInMonth++;
					//Weeks  labelling
					
					if(firstWeek)weeksHtml+="<tr>";
					if(tab[1,h].ToString().Length>0 && !tab[1,h].ToString().Equals(oldWeekDate) && !firstWeek)
					{
						weekColspan=(weekColspan*nbPixelByDay)-boderRightWidth;
						weeksHtml+="\r\n\t\t<td width=\""+weekColspan+"px\" class=\"pmc\"   align=center nowrap>"; 
						if(nbDaysInWeek>2)weeksHtml+=oldWeekDate.ToString(); //GestionWeb.GetWebWord(848, webSession.SiteLanguage) +"&nbsp;"+
						else weeksHtml+="<font color=\"#D0C8DA\">.</font>";
						weeksHtml+="</td>";
						weekColspan=0;
						nbDaysInWeek=0;
						nbWeek++;
					}
					oldWeekDate = tab[1,h].ToString();
					firstWeek=false;
					weekColspan++;
					nbDaysInWeek++;
				}
				monthColspan=(monthColspan*nbPixelByDay)-boderRightWidth;
				//				if(nbWeek==0)//decrease font size if only one week selected
				//					monthsHtml+="\r\n\t\t<td align=center STYLE=\"font-size: 8px;\" class=\"pmb\" width=\""+monthColspan+"px\"  nowrap>"; //class=\""+supportCSS+"\" 
				//				else
				monthsHtml+="\r\n\t\t<td align=center  class=\"pmb\" width=\""+monthColspan+"px\"  nowrap>"; //class=\""+supportCSS+"\" 
				if(nbDaysInMonth>4) 
				{
					
					monthsHtml+=MonthString.GetCharacters(int.Parse(oldMonthDate.ToString().Substring(4,2)),cultureInfo,3)+" "+oldMonthDate.ToString().Substring(2,2);
				}
				else 
					monthsHtml+="<font color=\"#644883\">.</font>";
				monthsHtml+="</td>";
				monthsHtml+="</tr></table></td>";//On ferme le tableau contenant les cellules affichant les mois
				monthsHtml+="</tr>";
				
				weekColspan=(weekColspan*nbPixelByDay)-boderRightWidth;
				weeksHtml+="\r\n\t\t<td   class=\"pmc\" width=\""+weekColspan+"px\" align=center nowrap>";// class=\"p7bold\"
				if(nbDaysInWeek>2)weeksHtml+=oldWeekDate.ToString(); //GestionWeb.GetWebWord(848, webSession.SiteLanguage) +"&nbsp;"+
				else weeksHtml+="<font color=\"#D0C8DA\">.</font>";
				weeksHtml+="</td>";	
				weeksHtml+="</tr></table></td>";//On ferme le tableau contenant les cellules affichant les semaines
				weeksHtml+="</tr>";

				t.Append(monthsHtml);
				t.Append(weeksHtml);
				//				if(!isExported)t.Append("\r\n\t<tr><td colspan=\""+h+2+"\" nowrap></tr>");	
				#endregion

				htmlHeader.Append(t.ToString());


				Double w,width;
				int nbLines=0,nbLinesIndex=0;

				if(nbWeek<30) {
					width=894;
					w=1;
					nbLines=(int)Math.Round(w*(width/20))-4;
				}
				else {
					nbLines=(int)nbWeek+15;
				}

				#region Traversing the table
				
				i=0;
				
				sloganIndex=GetSloganIdIndex(webSession);
				for(i=3;i<nbline;i++)
				{

					#region Gestion des couleurs des accroches
					if(sloganIndex!=-1	&& tab[i,sloganIndex]!=null && 
						((webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan)==webSession.GenericMediaDetailLevel.GetNbLevels)||
						(webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan)<webSession.GenericMediaDetailLevel.GetNbLevels && tab[i,sloganIndex+1]==null)))
					{
						if(!webSession.SloganColors.ContainsKey((Int64)tab[i,sloganIndex]))
						{
							colorNumberToUse=(colorItemIndex%30)+1;
							webSession.SloganColors.Add((Int64)tab[i,sloganIndex],"pm"+colorNumberToUse.ToString());																
							mediaPlanResultData.VersionsDetail.Add((Int64)tab[i,sloganIndex],new ExportAPPMVersionItem((Int64)tab[i,sloganIndex],"pm"+colorNumberToUse.ToString())); 
							colorItemIndex++;
								
						}
						if((Int64)tab[i,sloganIndex]!=0 && !mediaPlanResultData.VersionsDetail.ContainsKey((Int64)tab[i,sloganIndex]))
						{								
							mediaPlanResultData.VersionsDetail.Add((Int64)tab[i,sloganIndex],new ExportAPPMVersionItem((Int64)tab[i,sloganIndex],webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString()));
								
						}
						presentClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
						extendedClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
						stringItem="";
					}
					else
					{
						stringItem="";
						presentClass=extendedClass="pma";

					}
					#endregion
					isNewVersionLine=true;
					for(j=0;j<nbColTab;j++)
					{
						switch(j)
						{
								#region Level 1
							case FrameWorkResultConstantes.MediaPlanAPPM.L1_COLUMN_INDEX:
								
								if(tab[i,j]!=null && tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]!=null && 
									oldIdL1!=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX].ToString()))
								{	
									classe="pmtotal";																																					
									t.Append("\r\n\t<tr>\r\n\t\t<td  class=\""+classe+"\" nowrap>"+GestionWeb.GetWebWord(1401,webSession.SiteLanguage)+"</td><td width=\"100px\" class=\""+classe+"\" nowrap>&nbsp;</td>");	//L1 is called "Total" for APPM		<td width=\"50px\"class=\""+classe+"\" nowrap>&nbsp;</td>																
									if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]!=null)oldIdL1=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX].ToString());										
									isFirstVersionOfMedia=false;
								}
								break;
								#endregion

								#region Level 2
							case FrameWorkResultConstantes.MediaPlanAPPM.L2_COLUMN_INDEX:
								if(tab[i,j]!=null && tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX]!=null 
									&& oldIdL2!=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX].ToString()))
								{
									classe="L2";																				
									t.Append("\r\n\t<tr>\r\n\t\t<td width=\"100px\" class=\""+classe+"\" nowrap>&nbsp;"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L2_COLUMN_INDEX]+"</td><td width=\"100px\" class=\""+classe+"\" nowrap>&nbsp;</td>"); //<td width=\"50px\" class=\""+classe+"\" nowrap>&nbsp;</td>
									if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX]!=null)oldIdL2=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX].ToString());										
									isFirstVersionOfMedia=false;
								}
								break;
								#endregion

								#region Level 3
							case  FrameWorkResultConstantes.MediaPlanAPPM.L3_COLUMN_INDEX:
								if(tab[i,j]!=null && tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]!=null 
									&& oldIdL3!=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX].ToString()))
								{																														
									classe="L3";	
									t.Append("\r\n\t<tr>\r\n\t\t<td  class=\""+classe+"\" nowrap>&nbsp;"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L3_COLUMN_INDEX]+"</td>");
									if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]!=null)oldIdL3=Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX].ToString());										
									isFirstVersionOfMedia=true;
								}
								break;
								#endregion

								#region Level 4
							case  FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX:
								if(tab[i,j]!=null && tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX]!=null )
								{
																			
									if (isFirstVersionOfMedia)//Version should be presented on the same line that the media
										t.Append("\r\n\t\t<td class=\""+classe+"\" nowrap>"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX]+"</td>"); //<td width=\"50px\" align=\"center\" class=\""+classe+"\" nowrap><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX]+",-1,1','');\" nowrap><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>
									else
									{//next Version of the media
										classe="L4";
										t.Append("\r\n\t<tr><td  class=\""+classe+"\" nowrap>&nbsp;</td>\r\n\t\t<td width=\"100px\" class=\""+classe+"\" nowrap>"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L4_COLUMN_INDEX]+"</td>"); //<td width=\"50px\" align=\"center\" class=\""+classe+"\" nowrap><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX]+",-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>
									}
									isFirstVersionOfMedia=false;
								}
								break;
								#endregion

							case FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX:
								break;

							default:

								if(!isFirstVersionOfMedia && tab[i,j]!=null && tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlan.graphicItemType))
								{
									switch((FrameWorkResultConstantes.MediaPlan.graphicItemType)tab[i,j])
									{
										case FrameWorkResultConstantes.MediaPlanAPPM.graphicItemType.present:
											if(isNewVersionLine)
											{
												t.Append("<td class=\"pmX\" height=\"100%\" >");
												t.Append("<table cellpadding=\"0\" cellspacing=\"0\" height=\"100%\">");
												t.Append("<tr width=\"100%\">");													
												isNewVersionLine=false;
											}
											t.Append("<td class=\""+presentClass+"\"  style=\"width:4px;\" nowrap>&nbsp;"+stringItem+"</td>"); 
											break;
										case FrameWorkResultConstantes.MediaPlanAPPM.graphicItemType.extended:
											if(isNewVersionLine)
											{
												t.Append("<td class=\"pmX\" height=\"100%\" >");
												t.Append("<table cellpadding=\"0\" cellspacing=\"0\" height=\"100%\">");
												t.Append("<tr width=\"100%\">");												
												isNewVersionLine=false;
											}
                                            t.Append("<td class=\"" + extendedClass + "\" style=\"width:4px;\" nowrap>&nbsp;</td>"); 
											break;
										default:
											if(isNewVersionLine)
											{
												t.Append("<td class=\"pmX\" height=\"100%\" >");
												t.Append("<table cellpadding=\"0\" cellspacing=\"0\" height=\"100%\">");
												t.Append("<tr width=\"100%\">");													
												isNewVersionLine=false;
											}
                                            t.Append("<td class=\"pma\" nowrap>&nbsp;</td>");
											break;
									}
								}
								break;
						}
					}
					if(!isFirstVersionOfMedia)
					{
						t.Append("</tr></table></td>");//ON ferme le tableau contenant les cellules affichant l'activité publicitaire
						t.Append("</tr>"); //ON ferme la ligne c'est  le niveau support
						nbLinesIndex++;
						if(nbLinesIndex == nbLines){
							partieHTML.Add(t.ToString());

							t= new StringBuilder(10000);
							t.Append(htmlHeader.ToString());
							nbLinesIndex=0;
						}
					}
				}
				
				
				#endregion

				//Fin du tableau
				t.Append("</table>");
				t.Append(" <!-- Fin tableau plan media par version --->");
				partieHTML.Add(t.ToString());
				#endregion
					
				#region debug
#if(DEBUG)
			
				//				int i,j,nbline,nbCol;
				//				nbline=tab.GetLength(0);
				//				nbCol=tab.GetLength(1);
				//			
				//				string HTML="<table><tr>";
				//
				//					for(i=0;i<nbline;i++){
				//						for(j=0;j<nbCol;j++){
				//							if(tab[i,j]!=null) {							
				//								HTML+="<td>"+tab[i,j].ToString()+"</td>";
				//							}
				//							else HTML+="<td>&nbsp;</td>";
				//						}
				//						HTML+="</tr><tr>";
				//					}
				//				
				//				HTML+="</tr></table>";
				//				return HTML;
#endif
				#endregion
			}
			catch(System.Exception e)
			{
				throw(new WebExceptions.APPMMediaPlanUIExcpetion("Error while constructing the HTML table of Media plan with Versions ",e));
			}
			
			// On vide le tableau
			tab=null;			
			mediaPlanResultData.HTMLCode=t.ToString();
			return(mediaPlanResultData);		
		}
		#endregion 

		#endregion
        //Excel
		#region Excel
        ///// <summary>
        ///// This method generates the HTML for the excel of Media Plan.
        ///// </summary>
        ///// <param name="webSession">clients session</param>
        ///// <param name="dataSource">dataSource for creating Datasets </param>
        ///// <param name="dateBegin">Beginning Date</param>
        ///// <param name="dateEnd">Ending Date</param>
        ///// <param name="baseTarget">Base target</param>
        ///// <param name="additionalTarget">supplementary target</param>
        ///// <returns>HTML string for the synthesis table</returns>
        //public static string GetExcel(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget)
        //{
        //    #region variables
        //    System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
        //    #endregion

        //    #region Rappel des paramètres
        //    // Paramètres du tableau
        //    t.Append(ExcelFunction.GetAppmLogo(webSession));
        //    t.Append(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1474,webSession.SiteLanguage)));
        //    #endregion

        //    t.Append(Convertion.ToHtmlString(GetHTML(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,true)));
        //    t.Append(ExcelFunction.GetFooter(webSession));
        //    return t.ToString();
        //}
        //Excel zoom
		#region Excel zoom
        ///// <summary>
        ///// This method generates the HTML for the excel of Media Plan detail by day.
        ///// </summary>
        ///// <param name="webSession">clients session</param>
        ///// <param name="dataSource">dataSource for creating Datasets </param>
        ///// <param name="zoomDate">detail date</param>
        ///// <param name="baseTarget">Base target</param>
        ///// <param name="additionalTarget">supplementary target</param>		
        ///// <param name="url">url</param>
        ///// <returns>HTML string for the synthesis table</returns>
        //public static string GetZoomExcel(WebSession webSession,IDataSource dataSource, string zoomDate,Int64 baseTarget,Int64 additionalTarget,string url) {
        //    #region variables
        //    System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
        //    #endregion
			
        //    #region Rappel des paramètres
        //    // Paramètres du tableau
        //    t.Append(ExcelFunction.GetAppmLogo(webSession));
        //    t.Append(ExcelFunction.GetExcelHeader(webSession, GestionWeb.GetWebWord(1474, webSession.SiteLanguage)));
        //    #endregion

        //    t.Append(Convertion.ToHtmlString(GetZoomHTML(webSession,dataSource,zoomDate,baseTarget,additionalTarget,true,url)));
        //    t.Append(ExcelFunction.GetFooter(webSession));
        //    return t.ToString();
        //}
		#endregion
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Obtient la colonne contenant le id_slogan
		/// Si le détail support ne contient pas le niveau slogan, elle retoune -1
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>Colonne contenant l'id_slogan, -1 si pas de slogan</returns>
		private static int GetSloganIdIndex(WebSession webSession){
			int rank=webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan);
			switch(rank){
				case 1:
					return(FrameWorkResultConstantes.MediaPlanAPPM.L1_ID_COLUMN_INDEX);
				case 2:
					return(FrameWorkResultConstantes.MediaPlanAPPM.L2_ID_COLUMN_INDEX);
				case 3:
					return(FrameWorkResultConstantes.MediaPlanAPPM.L3_ID_COLUMN_INDEX);
				case 4:
					return(FrameWorkResultConstantes.MediaPlanAPPM.L4_ID_COLUMN_INDEX);
				default:
					return(-1);
			}
		}
		#endregion
	}
}
