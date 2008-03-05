#region Informations
// Auteur: D. V. Mussuma
// Date de création: 9/08/2005 
//Date de modification :
// Modified by: K.Shehzad
// Date of Modification: 12/08/2005  (changing the Exception usage)

#endregion
using System;
using System.Data;
using System.Text;
using ResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Domain.Translation;
using FWkResults=TNS.AdExpress.Constantes.FrameWork.Results;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.UI.Results.APPM
{
	/// <summary>
	/// Description résumée de LocationPlanTypesUI.
	/// </summary>
	public class LocationPlanTypesUI
	{
		#region HTML
		/// <summary>
		/// Affichage des données des types d'emplacement du plan
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dataSource">source de données</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible supplémentaire</param>
		/// <returns>données des types d'emplacement du plan à afficher</returns>
		public static string GetHtml(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget) {
			#region CSS 
			string header_css = "astd0";
			string table_css="asl0";
			string ct_css="asl3";
			int i=0;
			#endregion

			#region variables
			StringBuilder html=null;
			DataTable dt = null;
			#endregion

			
			try{
				 dt = ResultsAPPM.LocationPlanTypesRules.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget);
				
				if(dt!=null && dt.Rows.Count>0){
					html = new StringBuilder();

					html.Append("<table cellPadding=0 cellSpacing=1px border=0>");

					#region En-tête du tableau
					html.Append("<tr class=\"" + header_css + "\">");
					html.Append("<td>" +""+"</td>");
					html.Append("<td>" + HeaderTitle(webSession,webSession.SelectionUniversAEPMTarget.FirstNode.Text) + "</td>");
					html.Append("<td>" + GestionWeb.GetWebWord(1743, webSession.SiteLanguage) + "</td>");
					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						html.Append("<td>" + HeaderTitle(webSession,webSession.SelectionUniversAEPMTarget.LastNode.Text) + "</td>");
						html.Append("<td>" + GestionWeb.GetWebWord(1743, webSession.SiteLanguage) + "</td>");					
					}
					html.Append("</tr>");
					#endregion

					#region Parcours du tableau et création des lignes de la table		
					
					foreach(DataRow dr in dt.Rows){
						if(i>0)table_css=ct_css;
						html.Append("<tr class=\"" + table_css +"\">");
						html.Append("<td align= \"left\" nowrap>"+dr["location"]+"</td>");
						html.Append("<td nowrap>"+Double.Parse(dr["unitBaseTarget"].ToString()).ToString("### ### ### ### ##0")+"</td>");
						html.Append("<td nowrap>"+dr["repartionBaseTarget"].ToString()+"%</td>");
						if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
							html.Append("<td nowrap>"+Double.Parse(dr["unitAdditionalTarget"].ToString()).ToString("### ### ### ### ##0")+"</td>");
							html.Append("<td nowrap>"+dr["repartionAdditionalTarget"]+"%</td>");	
						}
						html.Append("</tr>");	
						i++;
					}
					#endregion 

					html.Append("</table>");

				}else{
					return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
				}
			}catch(System.Exception e){
				throw(new WebExceptions.LocationPlanTypesUIException("Impossible de construire le tableau HTML des types d'emplacements du plan ",e));
			}					

			return html.ToString();

		}
		#endregion 

		#region EXCEL
		/// <summary>
		/// Excel  des types d'emplacement du plan
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending date</param>
		/// <param name="baseTarget">base target</param>		
		/// <param name="dataSource">dataSource for creating Datasets </param>	
		/// <param name="additionalTarget">additional target</param>	
		/// <returns>HTML</returns>
		public static string GetExcel(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget) {
			#region Rappel des paramètres
			// Paramètres du tableau
			string parameters=(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1741,webSession.SiteLanguage)));
			#endregion

			string excelTable=Convertion.ToHtmlString(GetHtml(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget));
			
			return parameters+excelTable ;
			
		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Titre en-tête du tableau
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="targetText">texte</param>
		/// <returns>titre</returns>
		private static string HeaderTitle(WebSession webSession,string targetText){
			switch(webSession.Unit){				
				case WebConstantes.CustomerSessions.Unit.euro:
					return GestionWeb.GetWebWord(1423,webSession.SiteLanguage);				
				case WebConstantes.CustomerSessions.Unit.insertion:				
					return GestionWeb.GetWebWord(940,webSession.SiteLanguage);				
				case WebConstantes.CustomerSessions.Unit.pages:
					return GestionWeb.GetWebWord(943,webSession.SiteLanguage);				
				case WebConstantes.CustomerSessions.Unit.grp:
					return GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+" ("+targetText+")";					
				default:
					throw new WebExceptions.LocationPlanTypesUIException("HeaderTitle(webSession.Unit unit,FWkResults.APPM.Target.Type targetType)-->Le cas de cette unité n'est pas gérer. Pas de champ correspondante.");
					
			}	
		}
		#endregion
	}
}
