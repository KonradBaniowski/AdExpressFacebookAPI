#region Informations
// Auteur: A.DADOUCH 
// Date de création: 05/07/2005 
#endregion

using System;
using System.Data;
using System.Text;
using System.Collections;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Exceptions;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.FrameWork.DB.Common;
using APPMUIs = TNS.AdExpress.Web.UI.Results.APPM;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.UI.Results.APPM{

	/// <summary>
	/// Description résumée de AnalyseFamilyInterestPlanUI.
	/// </summary>
	public class AnalyseFamilyInterestPlanUI{


		/// <summary>
		/// familles d'intérêts Plan APPM
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
		public static string InterestFamilyPlan(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget, bool excel){
				
			#region Html

			#region variables
			DataTable InterestFamilyPlanData;
			StringBuilder html=new StringBuilder(5000);
			string classCss1="p2";
			string classCss2="acl1";
			string classCss3="insertionHeader";
			string unitName="";
			#endregion					
				
			#region Données
			try{
				InterestFamilyPlanData=TNS.AdExpress.Web.Rules.Results.APPM.AnalyseFamilyInterestPlanRules.InterestFamilyPlan( webSession, dataSource,idWave,dateBegin, dateEnd,idBaseTarget,idAdditionalTarget);
				if(InterestFamilyPlanData==null || InterestFamilyPlanData.Rows.Count==0 )throw(new WebExceptions.AnalyseFamilyInterestPlanUIException());
			}
			catch(System.Exception){
				//	throw(new WebExceptions.APPMBusinessFacadeException("pas de données+: "+ee.Message));
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage));
			}
			#endregion

			#region 1er et 2eme Ligne (libelés)
			html.Append("<table border=0 cellpadding=0 cellspacing=0 width=400 >");
			html.Append("\r\n\t<tr  height=\"20px\">");
			if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
				html.Append("<td  class=\""+classCss1+"\" nowrap rowspan=2>"+GestionWeb.GetWebWord(1777,webSession.SiteLanguage)+ "</td>");
			}
			else 
				html.Append("<td  class=\""+classCss1+"\" nowrap>"+GestionWeb.GetWebWord(1777,webSession.SiteLanguage)+ "</td>");

			#region sélection par rappot à l'unité choisit
			if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
				html.Append("<td  class=\""+classCss1+"\" nowrap colspan=2>"+GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ "&nbsp;&nbsp;&nbsp;(" +InterestFamilyPlanData.Rows[0]["baseTarget"]+ ")</td>");	
				//Colonne separation 
				if(!excel)
					html.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 1px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
				html.Append("<td  class=\""+classCss1+"\" nowrap colspan=2>"+GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ "&nbsp;&nbsp;&nbsp;(" +InterestFamilyPlanData.Rows[0]["additionalTarget"]+") </td>");		
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
					unitName= GestionWeb.GetWebWord(940,webSession.SiteLanguage);
					break;
				case WebConstantes.CustomerSessions.Unit.grp:  
					unitName= GestionWeb.GetWebWord(573,webSession.SiteLanguage);
					break;
				case WebConstantes.CustomerSessions.Unit.pages:  
					unitName= GestionWeb.GetWebWord(566,webSession.SiteLanguage);
					break;
				default : break;
			}
			html.Append("<td  class=\""+classCss1+"\" nowrap>"+unitName+"</td>");
			
			//html.Append("<td  class=\""+classCss1+"\" nowrap>"+GestionWeb.GetWebWord(573,webSession.SiteLanguage)+ "</td>");
			html.Append("<td  class=\""+classCss1+"\" nowrap>"+GestionWeb.GetWebWord(264,webSession.SiteLanguage)+ " </td>");
			if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
				if(!excel)
					html.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 1px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
				html.Append("<td  class=\""+classCss1+"\" nowrap>"+GestionWeb.GetWebWord(573,webSession.SiteLanguage)+ "</td>");
				html.Append("<td  class=\""+classCss1+"\" nowrap>"+GestionWeb.GetWebWord(264,webSession.SiteLanguage)+ " </td>");
			}
			html.Append("</tr>");	
			#endregion

			#region 1er ligne du tableau (Données:Total)
			html.Append("\r\n\t<tr height=\"20px\">");
			html.Append("<td  class=\""+classCss2+"\" nowrap>"+GestionWeb.GetWebWord(1401,webSession.SiteLanguage)+ "</td>");		
//			html.Append("<td  class=\""+classCss2+"\" nowrap >"+Convert.ToDouble(InterestFamilyPlanData.Rows[0]["totalBaseTargetUnit"]).ToString("# ### ##0.##")+" </td>");
			html.Append("<td  class=\""+classCss2+"\" nowrap >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(InterestFamilyPlanData.Rows[0]["totalBaseTargetUnit"].ToString(),webSession.Unit,false)+" </td>");
			html.Append("<td  class=\""+classCss2+"\" nowrap >  "+GestionWeb.GetWebWord(1684,webSession.SiteLanguage)+ " </td>");
			if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
				if(!excel)
					html.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 1px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
//				html.Append("<td  class=\""+classCss2+"\" nowrap >"+Convert.ToDouble(InterestFamilyPlanData.Rows[0]["totalAdditionalTargetUnit"]).ToString("# ### ##0.##")+" </td>");
				html.Append("<td  class=\""+classCss2+"\" nowrap >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(InterestFamilyPlanData.Rows[0]["totalAdditionalTargetUnit"].ToString(),webSession.Unit,false)+" </td>");
				html.Append("<td  class=\""+classCss2+"\" nowrap >"+GestionWeb.GetWebWord(1684,webSession.SiteLanguage)+ " </td>");
			}
			html.Append("</tr>");
			#endregion

			#region Lignes du tableau
			for(int i=1; i<InterestFamilyPlanData.Rows.Count-1;i++){
				html.Append("\r\n\t<tr onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#B1A3C1';\"  bgcolor=#B1A3C1 height=\"20px\" >");
				html.Append("<td  class=\""+classCss3+"\" nowrap >"+InterestFamilyPlanData.Rows[i]["InterestFamily"]+" </td>");
//				html.Append("<td  class=\""+classCss3+"\" nowrap >"+Convert.ToDouble(InterestFamilyPlanData.Rows[i]["unitBase"]).ToString("# ### ##0.##")+"</td>");
				html.Append("<td  class=\""+classCss3+"\" nowrap >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(InterestFamilyPlanData.Rows[i]["unitBase"].ToString(),webSession.Unit,false)+"</td>");
				html.Append("<td  class=\""+classCss3+"\" nowrap >"+InterestFamilyPlanData.Rows[i]["distributionBase"]+"%</td>");
				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
					if(!excel)
						html.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 1px solid;BORDER-LEFT: white 1px solid\"><img width=1px></td>");
//					html.Append("<td  class=\""+classCss3+"\" nowrap >"+Convert.ToDouble(InterestFamilyPlanData.Rows[i]["unitSelected"]).ToString("# ### ##0.##")+"</td>");
					html.Append("<td  class=\""+classCss3+"\" nowrap >"+WebFunctions.Units.ConvertUnitValueAndPdmToString(InterestFamilyPlanData.Rows[i]["unitSelected"].ToString(),webSession.Unit,false)+"</td>");
					html.Append("<td  class=\""+classCss3+"\" nowrap >"+InterestFamilyPlanData.Rows[i]["distributionSelected"]+"% </td>");
				}
				html.Append("</tr>");
			}
			#endregion

			html.Append("</table>");
			return html.ToString();
			#endregion
		}	

		/// <summary>
		/// Excel de familles d'intérêts plan APPM
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de dataset </param>
		/// <param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="idBaseTarget">cible de base</param>
		/// <param name="idAdditionalTarget">cible supplementaire</param>
		/// <param name="excel">boolean pour excel</param>
		/// <returns>Excel pour familles d'intérêts</returns>
		public static string GetExcel(WebSession webSession,IDataSource dataSource,Int64 idWave , int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget, bool excel){

			#region Variables
			StringBuilder t=new StringBuilder(5000);
			#endregion

			if(excel){
				#region Rappel des paramètres
				// Paramètres du tableau
				t.Append(ExcelFunction.GetAppmLogo());
				t.Append(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1777,webSession.SiteLanguage)));				
				#endregion
				t.Append(Convertion.ToHtmlString(APPMUIs.AnalyseFamilyInterestPlanUI.InterestFamilyPlan( webSession, dataSource, idWave,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,excel)));
				t.Append(Convertion.ToHtmlString(ExcelFunction.GetFooter(webSession)));

			}
			return t.ToString();
		}
	}
}

