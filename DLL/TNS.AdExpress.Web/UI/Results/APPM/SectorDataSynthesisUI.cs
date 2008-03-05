#region Informations
// Auteur: Y. R'kaina 
// Date de création: 15/01/2007 
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
	/// This class claculates the HTML table for the Sector Data synthesis.
	/// </summary>
	public class SectorDataSynthesisUI{
		
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
			//Hashtable synthesisData=TNS.AdExpress.Web.Rules.Results.APPM.SectorDataSynthesisRules.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget);
			Hashtable synthesisData=new Hashtable();
			#endregion
			
			#region HTML

			try{
				if(synthesisData!=null&&synthesisData.Count>0){
					t=new StringBuilder(3500);
					t.Append("<table  border=0 cellpadding=0 cellspacing=0 width=600 >");
					//titre
					t.Append("\r\n\t<tr height=\"30px\"><td colspan=2 class=\"p2\" align=\"center\" style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid;font-size: 16px\">"+GestionWeb.GetWebWord(1666,webSession.SiteLanguage)+"</td></tr>");	
					//Périod d'analyse
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(381,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["dateBegin"]+" "+GestionWeb.GetWebWord(125,webSession.SiteLanguage)+" "+synthesisData["dateEnd"]+"</td></tr>");	
					//Budget total
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2075,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["budget"]).ToString("# ### ##0.##")+"</td></tr>");	
					//Nombre d'annonceurs
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2073,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["annonceurs"]).ToString("# ### ##0.##")+"</td></tr>");	
					//Nombre de marques
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2074,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["marques"]).ToString("# ### ##0.##")+"</td></tr>");	
					//Nombre de produits
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1393,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["produits"]).ToString("# ### ##0.##")+"</td></tr>");	
					//Nombre d'insertions
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["insertions"]).ToString("# ### ##0.##")+"</td></tr>");	
					//Nombre de pages
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["pages"]).ToString("# ### ##0.##")+"</td></tr>");	
					//Secteur de référence
					//if the competitor univers is not selected we print the groups of the products selected
//					if(webSession.CompetitorUniversAdvertiser.Count<2){
//						styleTitle=InvertStyle(styleTitle);
//						styleValue=InvertStyle(styleValue);
//						string[] groups=synthesisData["group"].ToString().Split(',');
//						t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50% valign=top>"+GestionWeb.GetWebWord(1668,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>");
//						Array.Sort(groups);
//						foreach(string gr in groups){
//							t.Append("&nbsp;&nbsp;&nbsp;&nbsp;"+gr+"<br>");	
//						}
//						t.Append("</td></tr>");
//					}
					//cible selectionnée
//					styleTitle=InvertStyle(styleTitle);
//					styleValue=InvertStyle(styleValue);
//					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1672,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["targetSelected"]+"</td></tr>");	
					// nombre de GRP
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1673,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["GRPNumber"]).ToString("# ### ##0.##")+"</td></tr>");	
					// nombre de GRP 15 et +
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1673,webSession.SiteLanguage) + " " +  synthesisData["baseTarget"] + " : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["GRPNumberBase"]).ToString("# ### ##0.##")+"</td></tr>");
					//Affinité GRP vs cible 15 ans à +																				   
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);					
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2076,webSession.SiteLanguage)+" vs "+synthesisData["baseTarget"]+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+((synthesisData["AffinitieGRP"].ToString().Length > 0)? Convert.ToDouble(synthesisData["AffinitieGRP"]).ToString("# ### ##0.##"):"&nbsp;")+"</td></tr>");	
					// Coût GRP(cible selectionnée)					
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1675,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+((synthesisData["GRPCost"].ToString().Length>0)?Convert.ToDouble(synthesisData["GRPCost"]).ToString("# ### ##0"):"&nbsp;")+"</td></tr>");	
					// Coût GRP(cible 15 ans et +)					
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1675,webSession.SiteLanguage) + " " +  synthesisData["baseTarget"] + " : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+((synthesisData["GRPCostBase"].ToString().Length > 0)?Convert.ToDouble(synthesisData["GRPCostBase"]).ToString("# ### ##0"):"&nbsp;")+"</td></tr>");	
					//Affinité coût GRP vs cible 15 ans à +
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2077,webSession.SiteLanguage)+" vs "+synthesisData["baseTarget"]+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+((synthesisData["AffinitieGRPCost"].ToString().Length>0)?Convert.ToDouble(synthesisData["AffinitieGRPCost"]).ToString("# ### ##0.##"):"&nbsp;")+"</td></tr>");									

					t.Append("</table>");	
				}				
			}
			catch(System.Exception e){
				throw(new WebExceptions.SectorDataSynthesisUIException("Error while constructing the HTML for the Sector Data Synthesis Table ",e));
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
            t.Append(ExcelFunction.GetAppmLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession, GestionWeb.GetWebWord(1666, webSession.SiteLanguage)));
			#endregion

			t.Append(Convertion.ToHtmlString(TNS.AdExpress.Web.UI.Results.APPM.SectorDataSynthesisUI.GetHTML(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget)));
			t.Append(Convertion.ToHtmlString(ExcelFunction.GetFooter(webSession)));
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

		#endregion

	}
}
