#region Informations
// Author: K. Shehzad
// Date of creation: 02/08/2005 
// Date of modification:
#endregion

using System;
using System.Data;
using System.Text;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Translation;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.FrameWork;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Units;
namespace TNS.AdExpress.Web.UI.Results.APPM
{
	/// <summary>
	/// This class generates the HTML and Excel for the APPM PDV Plan.
	/// </summary>
	public class PDVPlanUI
	{
		#region HTML
		/// <summary>
		/// This method generates the HTML for the PDV Plan table.
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetHTML(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget)
		{
			#region CSS Styles
			string header_css = "astd0";
			string table_css="asl0";
			string vh_css="asl0";
			string ct_css="asl3";
			string md_css="asl4";
			#endregion

			#region variables
			StringBuilder html=null;
			string percentage=string.Empty;
			#endregion
			
			#region get Data
			DataTable PDVPlanData=TNS.AdExpress.Web.Rules.Results.APPM.PDVPlanRules.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,false);
			#endregion

			#region HTML

			try{
				if(PDVPlanData!=null && PDVPlanData.Rows.Count>0){
					html = new StringBuilder();
					
					#region construction of the HTML table
					html.Append("<table cellPadding=0 cellSpacing=1px border=0>");

					#region Table Headers
					html.Append("<tr class=\"" + header_css + "\">");
					html.Append("<td>" +""+"</td>");
                    html.Append("<td>" + Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].WebTextId, webSession.SiteLanguage)) + "</td>");
                    html.Append("<td>" + Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].WebTextId, webSession.SiteLanguage)) + "</td>");
                    html.Append("<td>" + Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].WebTextId, webSession.SiteLanguage)) + "</td>");
                    html.Append("<td>" + Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].WebTextId, webSession.SiteLanguage)) + "</td>");
					html.Append("<td>" + GestionWeb.GetWebWord(1735, webSession.SiteLanguage) + "</td>");
					
					html.Append("</tr>");
					#endregion

					#region Traversing and filling the table
					int i=0;
					bool pdm=false; //Pour gérer l'affichage des pourcentages
					foreach(DataRow dr in PDVPlanData.Rows){
						#region styles
						if(i<2){
							table_css=vh_css;
							percentage="";
							pdm=false;
						}
						else if(i==2){
							table_css=md_css;
							percentage="%";
							pdm=true;
						}
						else{
							table_css=ct_css;
							percentage="";
							pdm=false;
						}
						#endregion
//						html.Append("<tr class=\"" + table_css +"\">");
//						html.Append("<td align= \"left\" nowrap>"+dr["products"]+"</td>");
//						html.Append("<td nowrap>"+Convert.ToDouble(dr["euros"]).ToString("# ### ##0.##")+percentage+"</td>");
//						html.Append("<td nowrap>"+Convert.ToDouble(dr["pages"]).ToString("# ### ##0.##")+percentage+"</td>");
//						html.Append("<td nowrap>"+Convert.ToDouble(dr["insertions"]).ToString("# ### ##0.##")+percentage+"</td>");
//						html.Append("<td nowrap>"+Convert.ToDouble(dr["GRP"]).ToString("# ### ##0.##")+percentage+"</td>");
//						html.Append("<td nowrap>"+Convert.ToDouble(dr["GRPBaseTarget"]).ToString("# ### ##0.##")+percentage+"</td>");
//						html.Append("</tr>");

						html.Append("<tr class=\"" + table_css +"\">");
						html.Append("<td align= \"left\" nowrap>"+dr["products"]+"</td>");
						html.Append("<td nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["euros"].ToString(),WebConstantes.CustomerSessions.Unit.euro,pdm)+percentage+"</td>");
						html.Append("<td nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["pages"].ToString(),WebConstantes.CustomerSessions.Unit.pages,pdm)+percentage+"</td>");
						html.Append("<td nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["insertions"].ToString(),WebConstantes.CustomerSessions.Unit.insertion,pdm)+percentage+"</td>");
						html.Append("<td nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["GRP"].ToString(),WebConstantes.CustomerSessions.Unit.grp,pdm)+percentage+"</td>");
						html.Append("<td nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["GRPBaseTarget"].ToString(),WebConstantes.CustomerSessions.Unit.grp,pdm)+percentage+"</td>");
						html.Append("</tr>");

						i++;
					}
					#endregion 

					html.Append("</table>");
					#endregion
				}			
			}	
			catch(System.Exception e){
				throw(new WebExceptions.PDVPlanUIException("Error while constructing the HTML table of PDV Plan ",e));
			}

			#endregion

			#region no data
			if(html==null||html.Length<=0)
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
				
			#endregion

			return html.ToString();
		}
		#endregion

		#region EXCEL
		/// <summary>
		/// Excel of PDVPlan APPM
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dateBegin">Beginning Datet</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">base target</param>		
		/// <param name="additionalTarget">supplementary target</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetExcel(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget)
		{
			#region variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
			#endregion

			#region Rappel des paramètres
			// Paramètres du tableau
            t.Append(ExcelFunction.GetAppmLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession, GestionWeb.GetWebWord(1728, webSession.SiteLanguage)));
			#endregion
			
			//Get data for the Excel Sheet
			t.Append(Convertion.ToHtmlString(TNS.AdExpress.Web.UI.Results.APPM.PDVPlanUI.GetHTML(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget)));

			t.Append(ExcelFunction.GetFooter(webSession));
			return t.ToString();
			
		}
		#endregion
	}
}
