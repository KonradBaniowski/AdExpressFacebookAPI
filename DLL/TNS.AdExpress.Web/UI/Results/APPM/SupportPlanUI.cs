#region Infomration
/*
Author ; G. RAGNEAU
Creation : 27/07/2005
Last Modification : 07/11/2005 Gestion des unités
*/
#endregion

using System;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using CustomerCst=TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;

using RulesFct = TNS.AdExpress.Web.Rules.Results;
using ExcelFct = TNS.AdExpress.Web.UI.ExcelWebPage;
using TxtFct = TNS.AdExpress.Web.Functions.Text;
using TNS.FrameWork;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;


namespace TNS.AdExpress.Web.UI.Results.APPM
{
	/// <summary>
	/// Build the user interface for the APPM module, tab "Valorisation and efficiency by media"
	/// </summary>
	public class SupportPlanUI
	{

		/// <summary>
		/// Generate HTML Code so as to display the detail of the insertions
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User session</param>
		/// <param name="dateBegin">Beginning of the study</param>
		/// <param name="dateEnd">End  of the study</param>
		/// <param name="idBaseTarget">Default target</param>
		/// <param name="idAdditionaleTarget">Other target</param>
		/// <param name="idWave">Study wave</param>
		/// <param name="excel">Specify if it must fit excel code</param>
		/// <returns>HTML Code</returns>
		public static string GetHtml(IDataSource dataSource, WebSession webSession , int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idAdditionaleTarget, Int64 idWave, bool excel){

			DataTable dtResult = RulesFct.APPM.SupportPlanRules.GetData(dataSource, webSession , dateBegin, dateEnd, idBaseTarget, idAdditionaleTarget, idWave);
			
			if(dtResult.Rows.Count<=0){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage));
			}


			StringBuilder html = new StringBuilder();

			bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);


			#region css styles
			string header_css = (!excel)?"astd0":"astd0x";
			string vh_css="asl0";
			string ct_css=(!excel)?"asl3":"asl3x";
			string md_css=(!excel)?"asl4":"asl4x";

			#endregion

			#region table open
			html.Append("<table cellPadding=0 cellSpacing=1px border=0>");
			#endregion

			#region headers
			html.Append("<tr class=\"" + header_css + "\">");
			html.Append("<td rowSpan=\"2\">" + GestionWeb.GetWebWord(1141, webSession.SiteLanguage) + "</td>");
			html.Append("<td colSpan=\"2\">" + GestionWeb.GetWebWord(1682, webSession.SiteLanguage) + "</td>");
			html.Append("<td colSpan=\"3\">" + GestionWeb.GetWebWord(1713, webSession.SiteLanguage) + " " + dtResult.Columns["GRP1"].Caption + "</td>");
			html.Append("<td colSpan=\"3\">" + GestionWeb.GetWebWord(1713, webSession.SiteLanguage) + " " + dtResult.Columns["GRP2"].Caption + "</td>");
			html.Append("</tr>");
			html.Append("<tr class=\"" + header_css + "\">");
			html.Append("<td>" + GestionWeb.GetWebWord(1712, webSession.SiteLanguage) + "</td>");
			html.Append("<td>" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
			html.Append("<td>" + GestionWeb.GetWebWord(1679, webSession.SiteLanguage) + "</td>");
			html.Append("<td>" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
			html.Append("<td>" + GestionWeb.GetWebWord(1685, webSession.SiteLanguage) + "</td>");
			html.Append("<td>" + GestionWeb.GetWebWord(1679, webSession.SiteLanguage) + "</td>");
			html.Append("<td>" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
			html.Append("<td>" + GestionWeb.GetWebWord(1685, webSession.SiteLanguage) + "</td>");
			html.Append("</tr>");
			#endregion

			#region table fill
			string css = "";
			foreach(DataRow row in dtResult.Rows){
                if(row[0].ToString()== CustomerCst.Right.type.mediaAccess.ToString()) {
					css = md_css;
                }
                else if(row[0].ToString()== CustomerCst.Right.type.categoryAccess.ToString()) {
					css = ct_css;
				}else{
					css = vh_css;
				}
				html.Append("<tr class=\"" + css +"\">");
				html.Append("<td nowrap align=\"left\">");
				if (css == md_css && !excel && showProduct){
					html.Append("<a class=\"acl1\" href=\"javascript:PopUpInsertion('"+webSession.IdSession + "','"
						+ row["idMedia"] + "');\">> ");
				}
				html.Append(row["label"] + "</td>");
				for(int i = 3; i < row.ItemArray.Length; i++){
					if(row.ItemArray[i]!=DBNull.Value){
						html.Append("<td nowrap>");
//						if (row.Table.Columns[i].ColumnName.IndexOf("C/GRP")>-1){
//							html.Append(Math.Round(Decimal.Parse(row.ItemArray[i].ToString()),0).ToString("# ### ###"));
//						}
//						else{
//							html.Append(Math.Round(Decimal.Parse(row.ItemArray[i].ToString()),2).ToString("# ### ##0.##"));
//						}
						if (row.Table.Columns[i].ColumnName.IndexOf("budget")>-1){
							html.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(Math.Round(Decimal.Parse(row.ItemArray[i].ToString()),2).ToString(),WebConstantes.CustomerSessions.Unit.euro,false));
						}else if(row.Table.Columns[i].ColumnName.IndexOf("GRP")>-1 && !(row.Table.Columns[i].ColumnName.IndexOf("C/GRP")>-1)){
							html.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(Math.Round(Decimal.Parse(row.ItemArray[i].ToString()),2).ToString(),WebConstantes.CustomerSessions.Unit.grp,false));
						}else if(row.Table.Columns[i].ColumnName.IndexOf("PDM")>-1){
							html.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(Math.Round(Decimal.Parse(row.ItemArray[i].ToString()),2).ToString(),WebConstantes.CustomerSessions.Unit.euro,true));
						}else{
							html.Append(Math.Round(Decimal.Parse(row.ItemArray[i].ToString()),0).ToString("# ### ###"));
						}
						if (row.Table.Columns[i].ColumnName.IndexOf("PDM")>-1){
							html.Append(" %");
						}
					}
					else{
						html.Append("<td nowrap>&nbsp;&nbsp;");
					}
					
					html.Append("</td>");
				}
				html.Append("</tr>");
			}
			#endregion
			
			#region table close
			html.Append("</table>");
			#endregion

			if(!excel){
				return html.ToString();
			}
			else {
                return ExcelFct.GetAppmLogo(webSession)
					+ Convertion.ToHtmlString(ExcelFct.GetExcelHeader(webSession, false, false, false, false, true, GestionWeb.GetWebWord(1733, webSession.SiteLanguage))
					+ html.ToString())
					+ ExcelFct.GetFooter(webSession);
			}

		}

	}
}
