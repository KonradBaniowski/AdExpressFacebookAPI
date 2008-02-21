#region Informations
// Auteur: Y. R'kaina 
// Date de création: 17/01/2007 
#endregion

using System;
using System.Data;
using System.Text;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Translation;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.UI.Results.APPM{
	/// <summary>
	/// This class claculates the HTML table for the Sector Data Average.
	/// </summary>
	public class SectorDataAverageUI{

		#region HTML

		#region Synthsis HTML
		/// <summary>
		/// This method generates the HTML for the Sector Data Synthesis Tab
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetHTML(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget){
			
			#region variables
			StringBuilder t=null;
			string styleTitle="portofolio2";
			string styleValue="portofolio22";
			#endregion

			#region get Data
			//Hashtable synthesisData=TNS.AdExpress.Web.Rules.Results.APPM.SectorDataAverageRules.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget);
			Hashtable synthesisData=new Hashtable();
			#endregion
			
			#region HTML

			try{
				if(synthesisData!=null&&synthesisData.Count>0){
					t=new StringBuilder(3500);
					t.Append("<table  border=0 cellpadding=0 cellspacing=0 width=600 >");
					//titre
					t.Append("\r\n\t<tr height=\"30px\"><td colspan=2 class=\"p2\" align=\"center\" style=\"BORDER-RIGHT: #ffffff 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid;font-size: 13px\">"+GestionWeb.GetWebWord(2081,webSession.SiteLanguage)+"</td><td class=\"p2\" align=\"center\" style=\"BORDER-RIGHT: #ffffff 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid;font-size: 13px\">Min</td><td class=\"p2\" align=\"center\" style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid;font-size: 13px\">Max</td></tr>");	
					//Périod d'analyse
//					styleTitle=InvertStyle(styleTitle);
//					styleValue=InvertStyle(styleValue);
//					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(381,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["dateBegin"]+" "+GestionWeb.GetWebWord(125,webSession.SiteLanguage)+" "+synthesisData["dateEnd"]+"</td></tr>");	
					//Budget
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1712,webSession.SiteLanguage)+" : "+"</td>"+GetHtmlColumn("avgbudget","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("minbudget","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("maxbudget","# ### ##0.##",styleValue,synthesisData)+"</tr>");	
					//Nombre d'insertions
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+" : "+"</td>"+GetHtmlColumn("avginsertions","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("mininsertions","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("maxinsertions","# ### ##0.##",styleValue,synthesisData)+"</tr>");	
					//Nombre de pages
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+" : "+"</td>"+GetHtmlColumn("avgpages","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("minpages","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("maxpages","# ### ##0.##",styleValue,synthesisData)+"</tr>");	
					//Durée de présence
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2083,webSession.SiteLanguage)+" : "+"</td><td colspan=3 align=center class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["nbWeek"]+"</td></tr>");	
					// nombre de GRP
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td nowrap class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1673,webSession.SiteLanguage)+" : "+"</td>"+GetHtmlColumn("avgGRPNumber","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("minGRPNumber","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("maxGRPNumber","# ### ##0.##",styleValue,synthesisData)+"</tr>");	
					// nombre de GRP 15 et +
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td nowrap class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1673,webSession.SiteLanguage) + " " +  synthesisData["baseTarget"] + " : "+"</td>"+GetHtmlColumn("avgGRPNumberBase","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("minGRPNumberBase","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("maxGRPNumberBase","# ### ##0.##",styleValue,synthesisData)+"</tr>");	
					//Affinité GRP vs cible 15 ans à +																				   
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);					
					t.Append("\r\n\t<tr><td nowrap class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2076,webSession.SiteLanguage)+" vs "+synthesisData["baseTarget"]+" : "+"</td>"+GetHtmlColumn("avgAffinitieGRP","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("minAffinitieGRP","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("maxAffinitieGRP","# ### ##0.##",styleValue,synthesisData)+"</tr>");	
					//Nombre de GRP/semaine active sur cible
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2084,webSession.SiteLanguage)+" : "+"</td>"+GetHtmlColumn("avgGRP","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("minGRP","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("maxGRP","# ### ##0.##",styleValue,synthesisData)+"</tr>");	
					// Coût GRP(cible selectionnée)					
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td nowrap class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1675,webSession.SiteLanguage)+" : "+"</td>"+GetHtmlColumn("avgGRPCost","# ### ##0",styleValue,synthesisData)+GetHtmlColumn("minGRPCost","# ### ##0",styleValue,synthesisData)+GetHtmlColumn("maxGRPCost","# ### ##0",styleValue,synthesisData)+"</tr>");	
					// Coût GRP(cible 15 ans et +)					
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td nowrap class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1675,webSession.SiteLanguage) + " " +  synthesisData["baseTarget"] + " : "+"</td>"+GetHtmlColumn("avgGRPCostBase","# ### ##0",styleValue,synthesisData)+GetHtmlColumn("minGRPCostBase","# ### ##0",styleValue,synthesisData)+GetHtmlColumn("maxGRPCostBase","# ### ##0",styleValue,synthesisData)+"</tr>");	
					//Affinité coût GRP vs cible 15 ans à +
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td nowrap class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2077,webSession.SiteLanguage)+" vs "+synthesisData["baseTarget"]+" : "+"</td>"+GetHtmlColumn("avgAffinitieGRPCost","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("minAffinitieGRPCost","# ### ##0.##",styleValue,synthesisData)+GetHtmlColumn("maxAffinitieGRPCost","# ### ##0.##",styleValue,synthesisData)+"</tr>");	

					t.Append("</table>");	
				}				
			}
			catch(System.Exception e){
				throw(new WebExceptions.SectorDataAverageUIException("Error while constructing the HTML for the Sector Data Average Table ",e));
			}
			#endregion			

			#region no data
			if(t==null||t.Length<=0)
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
			#endregion

			return t.ToString();
		}
		#endregion

		#endregion

		#region Excel
		/// <summary>
		/// Excel de Synthèse sector Data
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible supplementaire</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetExcel(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget){
			#region Variables
			StringBuilder t = new StringBuilder(20000);
			#endregion

			#region Rappel des paramètres
			// Paramètres du tableau
			t.Append(ExcelFunction.GetAppmLogo());
			t.Append(ExcelFunction.GetExcelHeader(webSession, GestionWeb.GetWebWord(1666, webSession.SiteLanguage)));
			#endregion

			t.Append(Convertion.ToHtmlString(TNS.AdExpress.Web.UI.Results.APPM.SectorDataAverageUI.GetHTML(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget)));
			t.Append(ExcelFunction.GetFooter(webSession));
			return t.ToString();
		}
		#endregion

		#region private methods
		/// <summary>
		/// inverts the style for alternating rows
		/// </summary>
		/// <param name="style">the style string to be inverted</param>
		/// <returns>the inverted style</returns>
		private static string InvertStyle(string style)
		{
			string invertedStyle=string.Empty;

			//inverting the title style
			if(style=="portofolio2")
				invertedStyle="portofolio1";
			else if(style=="portofolio1")
				invertedStyle="portofolio2";
				//inverting the value style
			else if(style=="portofolio22")
				invertedStyle="portofolio11";
			else if(style=="portofolio11")
				invertedStyle="portofolio22";
			
			return invertedStyle;
		}
		/// <summary>
		/// renvoie la valeur d'une colonne sous forme HTML
		/// </summary>
		/// <param name="columnLabel">le libellé de la colonne</param>
		/// <param name="format">le format d'affichage</param>
		/// <param name="styleValue">le style à appliquer</param>
		/// <param name="synthesisData">Hashtable qui coontient les données</param>
		/// <returns></returns>
		private static string GetHtmlColumn(string columnLabel, string format, string styleValue, Hashtable synthesisData){
		
			return("<td nowrap class=\""+styleValue+"\" width=16%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData[columnLabel]).ToString(format)+"</td>");
		
		}
		#endregion

	}
}
