#region Infomration
/*
Author ; G. RAGNEAU
Creation : 27/07/2005
Last Modification : 
*/
#endregion

using System;
using System.Data;
using System.Text;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;

using RulesFct = TNS.AdExpress.Web.Rules.Results;
using WebFnc = TNS.AdExpress.Web.Functions;
using WebCst = TNS.AdExpress.Constantes.Web;
using ExcelFct = TNS.AdExpress.Web.UI.ExcelWebPage;
using TxtFct = TNS.AdExpress.Web.Functions.Text;
using TNS.FrameWork;
using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.UI.Results.APPM{
	/// <summary>
	/// Build the user interface for the APPM module, tab "Valorisation and efficiency by media"
	/// </summary>
	public class InsertionPlanUI{

		/// <summary>
		/// Generate HTML Code so as to display the detail of the insertions
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User session</param>
		/// <param name="dateBegin">Beginning of the study</param>
		/// <param name="dateEnd">End  of the study</param>
		/// <param name="idBaseTarget">Default target</param>
		/// <param name="idMedia">Media to detail</param>
		/// <param name="idWave">Study wave</param>
		/// <param name="excel">Specify if it must fit excel code</param>
		/// <returns>HTML Code</returns>
		public static string GetHtml(IDataSource dataSource, WebSession webSession , int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idMedia, Int64 idWave, bool excel){

			DataTable dtResult = RulesFct.APPM.InsertionPlanRules.GetData(dataSource, webSession , dateBegin, dateEnd, idBaseTarget, idMedia, idWave);

			StringBuilder html = new StringBuilder();

			if (dtResult.Rows.Count > 0){	
				html.Append("<TABLE  bgColor=\"#ffffff\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px;\"");
				html.Append("cellPadding=\"0\" cellSpacing=\"3\" align=\"center\" border=\"0\">");					

				#region TEMP : Info sur le clic droit de la souris
				if (!excel){
					html.Append("<tr><td>");
					html.Append("\n<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#DED8E5\">");
					html.Append("\n<tr><td>");
					html.Append("\n<script language=\"javascript\" type=\"text/javascript\">");
					html.Append("\nif(hasRightFlashVersion==true){");
					html.Append("\ndocument.writeln('<object id=\"infoOptionFlash\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"400\" height=\"20\" VIEWASTEXT>');");
					html.Append("\ndocument.writeln('<param name=\"movie\" value=\"/Flash/"+webSession.SiteLanguage+"/infoOptionsOneLine.swf\">');");
					html.Append("\ndocument.writeln('<param name=\"quality\" value=\"high\">');");
					html.Append("\ndocument.writeln('<param name=\"menu\" value=\"false\">');");
					html.Append("\ndocument.writeln('<param name=\"wmode\" value=\"transparent\">');");
					html.Append("\ndocument.writeln('<embed src=\"/Flash/"+webSession.SiteLanguage+"/infoOptionsOneLine.swf\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"400\" height=\"20\"></embed>');");
					html.Append("\ndocument.writeln('</object></td>');");
					html.Append("\n}\nelse{");
					html.Append("\ndocument.writeln('<img src=\"/Images/"+webSession.SiteLanguage+"/FlashReplacement/infoOptionsOneLine.gif\"></td>');");
					html.Append("\n}");
					html.Append("\n</script>");
					html.Append("\n</tr>");
					html.Append("\n</table>");
					html.Append("</td></tr>");
				}
				#endregion

				#region Excel export
				if (!excel){
					//Label table
					html.Append("<tr height=\"10\" vAlign=\"center\"><td class=\"txtViolet14Bold\">");
					html.Append(GestionWeb.GetWebWord(1727,webSession.SiteLanguage)); 
					html.Append("<br><br>" + dtResult.Rows[0]["label"]);
					html.Append(" " + GestionWeb.GetWebWord(1729,webSession.SiteLanguage));
					html.Append(" " + WebFnc.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate,webSession.PeriodType).ToShortDateString());
					html.Append(" " + GestionWeb.GetWebWord(1730,webSession.SiteLanguage));
					html.Append(" " + WebFnc.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType).ToShortDateString());

					html.Append("</td></tr>");
				}
				html.Append("<tr align=\"center\"><td><br>");
				#endregion

				#region css styles
				string header_css = (!excel)?"astd0":"astd0x";
				string md_css="asl0";
				string pr_css=(!excel)?"asl4":"asl4x";

				#endregion

				#region table open
				html.Append("<table cellPadding=0 cellSpacing=1px border=0>");
				#endregion

				#region headers
				html.Append("<tr class=\"" + header_css + "\">");
				html.Append("<td></td>");
				if(!excel){
					html.Append("<td>" + GestionWeb.GetWebWord(1731, webSession.SiteLanguage) + "</td>");
				}
				html.Append("<td>" + GestionWeb.GetWebWord(895, webSession.SiteLanguage) + "</td>");
				html.Append("<td>" + GestionWeb.GetWebWord(1420, webSession.SiteLanguage) + "</td>");
				html.Append("<td>" + GestionWeb.GetWebWord(1732, webSession.SiteLanguage) + "</td>");
				html.Append("<td>" + GestionWeb.GetWebWord(1712, webSession.SiteLanguage) + "</td>");
				html.Append("<td>" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
				html.Append("</tr>");
				#endregion

				#region table fill
				string css = "";
				foreach(DataRow row in dtResult.Rows){
					if(row[0].ToString()== Right.type.mediaAccess.ToString()){
						css = md_css;
					}
					else{
						css = pr_css;
					}
					html.Append("<tr class=\"" + css +"\">");
					html.Append("<td nowrap align=\"left\">");
					html.Append(row["label"] + "</td>");
					
					if(!excel){
						//Lien vers la PopUp Justificatif
						if(row[0].ToString()== Right.type.mediaAccess.ToString()){
							html.Append("<td></td>");
						}
						else{						
							//html.Append("<td align=\"center\"><a href=\"javascript:openPopUpJustificatif('"+webSession.IdSession+"','"+ row["idMedia"].ToString().Trim() +"','"+ row["idProduct"].ToString().Trim() +"','"+ row["mediaPaging"].ToString().Trim() +"','"+ DateString.DateTimeToYYYYMMDD((DateTime)row["date"]) +"','"+ DateString.DateTimeToYYYYMMDD((DateTime)row["dateParution"]) +"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
							html.Append("<td align=\"center\"><a href=\"javascript:openPopUpJustificatif('" + webSession.IdSession + "','" + row["idMedia"].ToString().Trim() + "','" + row["idProduct"].ToString().Trim() + "','" + row["mediaPaging"].ToString().Trim() + "','" + DateString.DateTimeToYYYYMMDD((DateTime)row["date"]) + "','" + DateString.DateTimeToYYYYMMDD((DateTime)row["dateCover"]) + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
						}
					}

					//if (row["dateParution"]!= DBNull.Value){//
					//html.Append("<td nowrap align=\"center\">" + ((DateTime)row["dateParution"]).ToShortDateString() + "</td>");
					//}
					//else{
						if (row["date"] != DBNull.Value){
							html.Append("<td nowrap align=\"center\">" + ((DateTime)row["Date"]).ToShortDateString() + "</td>");
						}
						else{
							html.Append("<td></td>");
						}
					//}
					html.Append("<td nowrap align=\"center\">" + row["format"] + "</td>");
					html.Append("<td nowrap align=\"center\">" + row["location"] + "</td>");
					html.Append("<td nowrap>" + WebFnc.Units.ConvertUnitValueAndPdmToString(row["budget"].ToString(),WebCst.CustomerSessions.Unit.euro,false)+ "</td>");
					html.Append("<td nowrap>" + WebFnc.Units.ConvertUnitValueAndPdmToString(row["pdm"].ToString(),webSession.Unit,true)+ "&nbsp;%</td>");
					html.Append("</tr>");
				}
				#endregion						

				#region table close
				html.Append("</table></td></tr></table>");
				#endregion
			
				if (excel){
					html.Insert(0, ExcelFct.GetAppmLogo() + ExcelFct.GetExcelHeader(webSession, false, false,false,false,true,GestionWeb.GetWebWord(1727,webSession.SiteLanguage)));
					html.Append(ExcelFct.GetFooter(webSession));
				}
			}
			return html.ToString();
		}

		#region Excel
		/// <summary>
		/// Excel of Insertion APPM
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dateBegin">Beginning Datet</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="idBaseTarget">base target</param>
		/// <param name="idWave">id of the wave</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="idMedia">Media to detail</param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetExcel(IDataSource dataSource, WebSession webSession , int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idMedia, Int64 idWave){
			#region variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
			#endregion
			t.Append(ExcelFct.GetAppmLogo());
			t.Append(GetHtml(dataSource, webSession, dateBegin, dateEnd, idBaseTarget,idMedia,idWave,true));
			t.Append(ExcelFct.GetFooter(webSession));
			return Convertion.ToHtmlString(t.ToString());
		}
		#endregion

	}
}
