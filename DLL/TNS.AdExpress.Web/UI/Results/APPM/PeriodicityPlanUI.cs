#region Informations
// Auteur: A.DADOUCH 
// Date de création: 15/07/2005 
// D. V. Mussuma : 21/10/2005  (Adding the unit Keuro management)
#endregion

using System;
using System.Data;
using System.Text;
using System.Collections;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Exceptions;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.FrameWork.DB.Common;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.UI.Results.APPM{

		/// <summary>
		/// Périodicité Plan APPM
		/// </summary>
		public class PeriodicityPlanUI{


		/// <summary>
		/// Périodicité Plan APPM
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		///<param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="idBaseTarget">Identifiant de la cible de base</param>
		/// <param name="idAdditionalTarget">Identifiant de la cible sélectionnée</param>
		/// <param name="excel">le boolean excel</param>
		/// <returns>HTML string </returns>
			public static string PeriodicityPlan(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget, bool excel){
				
				#region Html

				#region variables
				DataTable periodicityPlanData;
				StringBuilder html=new StringBuilder(5000);
				string classCss1="p2";
				string classCss2="acl1";
				string classCss3="insertionHeader";
                string classCssImg = "affinitiesBorderImg";
				string unitName="";
                IFormatProvider fp = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo;
                #endregion					
				
				#region Données
				try{
					periodicityPlanData=TNS.AdExpress.Web.Rules.Results.APPM.PeriodicityPlanRules.PeriodicityPlan( webSession, dataSource,idWave,dateBegin, dateEnd,idBaseTarget,idAdditionalTarget);
					if(periodicityPlanData==null || periodicityPlanData.Rows.Count==0 )throw(new WebExceptions.PeriodicityPlanUIException());
					}
				catch(System.Exception){
								//	throw(new WebExceptions.APPMBusinessFacadeException("pas de données+: "+ee.Message));
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage));
				}
				#endregion

				#region 1er et 2eme Ligne (libelés)
				html.Append("<table  border=0 cellpadding=0 cellspacing=0 width=400 >");
				html.Append("\r\n\t<tr  height=\"20px\">");
				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
					html.Append("<td  class=\""+classCss1+"\" nowrap rowspan=2>"+GestionWeb.GetWebWord(1774,webSession.SiteLanguage)+ "</td>");
				}
				else 
					html.Append("<td  class=\""+classCss1+"\" nowrap>"+GestionWeb.GetWebWord(1774,webSession.SiteLanguage)+ "</td>");
			
				#region sélection par rappot à l'unité choisit
				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){ 
					html.Append("<td  class=\""+classCss1+"\" nowrap colspan=2>"+GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ "&nbsp;&nbsp;&nbsp;(" +periodicityPlanData.Rows[0]["baseTarget"]+ ")</td>");	
					//Colonne separation 
					if(!excel)
                        html.Append("<td class=\"" + classCssImg + "\"><img width=1px></td>");
					html.Append("<td  class=\""+classCss1+"\" nowrap colspan=2>"+GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ "&nbsp;&nbsp;&nbsp;(" +periodicityPlanData.Rows[0]["additionalTarget"]+") </td>");			
					html.Append("</tr>");
					html.Append("\r\n\t<tr height=\"20px\" style=\"DISPLAY:inline\">");
				}
				#endregion				

				//2eme ligne	
				switch (webSession.Unit){
					case WebConstantes.CustomerSessions.Unit.euro:  
						unitName= GestionWeb.GetWebWord(1669,webSession.SiteLanguage);
						break;
					case WebConstantes.CustomerSessions.Unit.kEuro:
						unitName= GestionWeb.GetWebWord(1790,webSession.SiteLanguage);
						break;
					case WebConstantes.CustomerSessions.Unit.insertion:
                        unitName = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].WebTextId, webSession.SiteLanguage));
						break;
					case WebConstantes.CustomerSessions.Unit.grp:  
						unitName= GestionWeb.GetWebWord(573,webSession.SiteLanguage);
						break;
					case WebConstantes.CustomerSessions.Unit.pages:
                        unitName = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].WebTextId, webSession.SiteLanguage));
						break;
					default : break;
				}
				html.Append("<td  class=\""+classCss1+"\" nowrap>"+unitName+"</td>");

				//html.Append("<td  class=\""+classCss1+"\" nowrap>"+GestionWeb.GetWebWord(573,webSession.SiteLanguage)+ "</td>");
				html.Append("<td  class=\""+classCss1+"\" nowrap>"+GestionWeb.GetWebWord(264,webSession.SiteLanguage)+ " </td>");
				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
					if(!excel)
                        html.Append("<td class=\"" + classCssImg + "\"><img width=1px></td>");
					html.Append("<td  class=\""+classCss1+"\" nowrap>"+GestionWeb.GetWebWord(573,webSession.SiteLanguage)+ "</td>");
					html.Append("<td  class=\""+classCss1+"\" nowrap>"+GestionWeb.GetWebWord(264,webSession.SiteLanguage)+ " </td>");
				}
				html.Append("</tr>");	
				#endregion

 				#region 1er ligne du tableau (Données:Total)
				html.Append("\r\n\t<tr height=\"20px\" >");
				html.Append("<td align= \"left\" class=\""+classCss2+"\" nowrap>"+GestionWeb.GetWebWord(1401,webSession.SiteLanguage)+ "</td>");
                html.Append("<td  class=\"" + classCss2 + "\" nowrap >" + WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(periodicityPlanData.Rows[0]["totalBaseTargetUnit"]), webSession.Unit, false, fp) + " </td>");
				html.Append("<td  class=\""+classCss2+"\" nowrap >  "+GestionWeb.GetWebWord(1684,webSession.SiteLanguage)+ " </td>");
				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
					if(!excel)
                        html.Append("<td class=\"" + classCssImg + "\"><img width=1px></td>");
                    html.Append("<td  class=\"" + classCss2 + "\" nowrap >" + WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(periodicityPlanData.Rows[0]["totalAdditionalTargetUnit"]), webSession.Unit, false, fp) + " </td>");
					html.Append("<td  class=\""+classCss2+"\" nowrap >"+GestionWeb.GetWebWord(1684,webSession.SiteLanguage)+ " </td>");
				}
				html.Append("</tr>");
				#endregion

				#region Lignes du tableau
				for(int i=1; i<periodicityPlanData.Rows.Count-1;i++){
                    html.Append("\r\n\t<tr onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV3';\"  class=\"violetBackGroundV3\" height=\"20px\">");
					html.Append("<td  class=\""+classCss3+"\" nowrap >"+periodicityPlanData.Rows[i]["periodicity"]+" </td>");
					html.Append("<td  class=\""+classCss3+"\" nowrap >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(periodicityPlanData.Rows[i]["unitBase"]),webSession.Unit,false, fp)+"</td>");
					html.Append("<td  class=\""+classCss3+"\" nowrap >"+periodicityPlanData.Rows[i]["distributionBase"]+"%</td>");
					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						if(!excel)
                            html.Append("<td class=\"" + classCssImg + "\"><img width=1px></td>");
                        html.Append("<td  class=\"" + classCss3 + "\" nowrap >" + WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(periodicityPlanData.Rows[i]["unitSelected"]), webSession.Unit, false, fp) + "</td>");
						html.Append("<td  class=\""+classCss3+"\" nowrap >"+periodicityPlanData.Rows[i]["distributionSelected"]+"% </td>");
					}
					html.Append("</tr>");
				}
				#endregion

				  html.Append("</table>");
			return html.ToString();
			#endregion
			}	
 
			/// <summary>
			/// Excel de Périodicité plan APPM
			/// </summary>
			/// <param name="webSession">session client</param>
			/// <param name="dataSource">dataSource pour la creation de dataset </param>
			/// <param name="idWave">Identifiant de la vague</param>
			/// <param name="dateBegin">date de début</param>
			/// <param name="dateEnd">date de fin</param>
			/// <param name="idBaseTarget">cible de base</param>
			/// <param name="idAdditionalTarget">cible supplementaire</param>
			/// <param name="excel">boolean pour excel</param>
			/// <returns>Excel pour périodicité</returns>
			public static string GetExcel(WebSession webSession,IDataSource dataSource,Int64 idWave , int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget, bool excel){

				#region Variables
				StringBuilder t=new StringBuilder(20000);
				#endregion

				if(excel){
					#region Rappel des paramètres
					// Paramètres du tableau
                    t.Append(ExcelFunction.GetAppmLogo(webSession));
					t.Append(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1774,webSession.SiteLanguage)));
				
					#endregion

					t.Append(Convertion.ToHtmlString(TNS.AdExpress.Web.UI.Results.APPM.PeriodicityPlanUI.PeriodicityPlan( webSession, dataSource, idWave,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,excel)));
					t.Append(ExcelFunction.GetFooter(webSession));
				}
				return t.ToString();
			}
		}
}
