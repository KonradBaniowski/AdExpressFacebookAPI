#region Informations
// Author: K. Shehzad 
// Date of creation: 28/07/2005 
#endregion
using System;
using System.Data;
using System.Text;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using APPMUIs = TNS.AdExpress.Web.UI.Results.APPM;
using TNS.FrameWork;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.UI.Results.APPM
{
	/// <summary>
	/// This class is used to create the UserInterface for the Affinities results.
	/// </summary>
	public class AffinitiesUI
	{
		#region HTML
		/// <summary>
		/// Affinties APPM
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dateBegin">Beginning Datet</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">base target</param>
		/// <param name="idWave">id of the wave</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="excel">Boolean to indicate that the resul if excel or html</param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetHTML(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 idWave,bool excel)
		{
			#region variables
			StringBuilder html=null;
			string classCss1="p2";
			string classCss2="insertionHeader";
            string classCssImg = "affinitiesBorderImg";
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo;
            #endregion

			#region getting data 
			DataTable affinitiesData=TNS.AdExpress.Web.Rules.Results.APPM.AffintiesRules.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,idWave);
			#endregion

			try{
				if(affinitiesData!=null && affinitiesData.Rows.Count>0){
					#region HTML
					html=new StringBuilder(3500);

					#region table headings
					html.Append("<table border=0 cellpadding=0 cellspacing=0 width=400 >");
					html.Append("\r\n\t<tr height=\"30px\">");
					html.Append("<td align= \"center\" class=\""+classCss1+"\" nowrap >"+GestionWeb.GetWebWord(1708,webSession.SiteLanguage)+ "</td>");
					if(!excel)
                        html.Append("<td class=\"" + classCssImg + "\"><img width=1px></td>");
					html.Append("<td align= \"center\" class=\""+classCss1+"\" nowrap >"+GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ "</td>");
					html.Append("<td align= \"center\" class=\""+classCss1+"\" nowrap >"+GestionWeb.GetWebWord(1686,webSession.SiteLanguage)+ "</td>");
					if(!excel)
                        html.Append("<td class=\"" + classCssImg + "\"><img width=1px></td>");
					html.Append("<td align= \"center\" class=\""+classCss1+"\" nowrap >"+GestionWeb.GetWebWord(1685,webSession.SiteLanguage)+ "</td>");
					html.Append("<td align= \"center\" class=\""+classCss1+"\" nowrap >"+GestionWeb.GetWebWord(1686,webSession.SiteLanguage)+ "</td>");
					html.Append("</tr>");
					#endregion

					#region traversing the table
					foreach(DataRow row in affinitiesData.Rows)
					{
						if(Convert.ToInt64(row["id_target"])==baseTarget)
							classCss2="acl1";
						else
							classCss2="insertionHeader";
						html.Append("\r\n\t<tr height=\"20px\">");
                        html.Append("<td align= \"left\" class=\"" + classCss2 + "\" nowrap>" + row["target"] + "</td>");
						if(!excel)
                            html.Append("<td class=\"" + classCssImg + "\"><img width=1px></td>");
                        html.Append("<td class=\"" + classCss2 + "\" nowrap >" + WebFunctions.Units.ConvertUnitValueAndPdmToString(row["totalGRP"], WebConstantes.CustomerSessions.Unit.grp, false, fp) + "</td>");
						html.Append("<td class=\""+classCss2+"\" nowrap >"+Convert.ToDouble(row["GRPAffinities"]).ToString("# ### ##0")+ "</td>");
						if(!excel)
                            html.Append("<td class=\"" + classCssImg + "\"><img width=1px></td>");
                        html.Append("<td class=\"" + classCss2 + "\" nowrap >" + WebFunctions.Units.ConvertUnitValueAndPdmToString(Math.Round(Convert.ToDouble(row["cgrp"])), WebConstantes.CustomerSessions.Unit.grp, false, fp) + "</td>");
						html.Append("<td class=\""+classCss2+"\" nowrap >"+Convert.ToDouble(row["cgrpAffinities"]).ToString("# ### ##0")+ "</td>");
						html.Append("</tr>");				
					}
					#endregion
			
					html.Append("</table>");
					#endregion
				}
			}
			catch(System.Exception e){
				throw(new WebExceptions.AffinitiesUIException("Erro while generating the HTML code for the Affinities ",e));
			}

			#region no data
			if(html==null||html.Length<=0)
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
				
			#endregion

			return html.ToString();
		}
		#endregion

		#region Excel
		/// <summary>
		/// Excel of Affinties APPM
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dateBegin">Beginning Datet</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">base target</param>
		/// <param name="idWave">id of the wave</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="excel">Boolean to indicate that the resul if excel or html</param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetExcel(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 idWave,bool excel)
		{
			#region Rappel des paramètres
			// Paramètres du tableau
			
			string parameters=(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1687,webSession.SiteLanguage)));
			#endregion

			string excelTable=Convertion.ToHtmlString(APPMUIs.AffinitiesUI.GetHTML(webSession,dataSource,dateBegin,dateEnd,baseTarget,idWave,excel));
            return ExcelFunction.GetAppmLogo(webSession) + parameters + excelTable + Convertion.ToHtmlString(ExcelFunction.GetFooter(webSession));
		}
		#endregion
	}
}
