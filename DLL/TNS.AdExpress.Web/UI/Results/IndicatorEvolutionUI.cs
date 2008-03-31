#region Informations
// Auteur: A. Obermeyer 
// Date de création : 21/10/2004 
// Date de modification : 21/10/2004 
//		10/05/2005	K.Shehzad	Chagement d'en tête Excel
//		12/08/2005	G. Facon	Nom de fonction
#endregion

using System;
using System.Data;
using System.Web.UI;
using System.Collections;
using System.Windows.Forms;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Exceptions;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using WebFunctions=TNS.AdExpress.Web.Functions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Génère le code HTML pour l'indicateur Evolution
	/// </summary>
	public class IndicatorEvolutionUI{

		#region Méthodes 
		
		#region HTML
		/// <summary>
		/// Affichage d'un tableau Référence ou Annonceur
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="tableType">Type du tableau</param>
		/// <param name="excel">True si excel</param>
		/// <returns>Code</returns>
		public static string GetEvolutionIndicatorUI(WebSession webSession,FrameWorkConstantes.Results.EvolutionRecap.ElementType tableType,bool excel){
		
			#region Variables

			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//			bool firstLine=true;
			string styleClassTitle="";
			string styleClassNumber="";
		//	string competitor="";			
			string PeriodDate="";
			long last;
		

			#endregion

			#region Constantes
			
			const string P2="p2";
			// Pour les libellés
			const string L1="p7";
			const string competitorStyle="p14";
			const string competitorExcelStyle="p142";
			const string referenceStyle="p15";
			// Pour les chiffres
			const string L3="p9";
			const string competitorStyleNB="p141";
			const string competitorExcelStyleNB="p143";

			const string referenceStyleNB="p151";			
			
			const string  noData="mediumPurple1";
			//Pas de données
			//const string noResult="acl4";

			#endregion
			//string imageSortAsc="/Images/Common/fl_tri_croi1.gif";
			//string imageSortDesc="/Images/Common/fl_tri_decroi1.gif";		

			string absolutEndPeriod = WebFunctions.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
			
			if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
				throw new NoDataException();

			DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
			DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
			
//			DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//			DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);
			
			PeriodDate=PeriodBeginningDate.Date.ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.ToString("dd/MM/yyyy");

			object[,] tab=TNS.AdExpress.Web.Rules.Results.IndicatorEvolutionRules.GetFormattedTable(webSession,tableType);
			
			last=tab.GetLongLength(0)-1;


			if(tab.GetLongLength(0)==0){
                t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
				t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
				if( tableType==FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser){
					t.Append(GestionWeb.GetWebWord(177,webSession.SiteLanguage)+PeriodDate);
				}
				t.Append("</td></tr></table>");
				return t.ToString();

			}

            t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");

			#region 1ere ligne
			t.Append("\r\n\t<tr height=\"30px\" >");
			
			#region Hausse
			if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.EvolutionRecap.ElementType.product){
				t.Append("<td class=\""+P2+"\" nowrap valign=\"top\"  align=\"center\">"+GestionWeb.GetWebWord(1315,webSession.SiteLanguage)+"<br>"+PeriodDate+"</td>");
			}
			else{
				t.Append("<td class=\""+P2+"\" nowrap valign=\"top\"  align=\"center\">"+GestionWeb.GetWebWord(1210,webSession.SiteLanguage)+"<br>"+PeriodDate+"</td>");
			}

			//t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1170,webSession.SiteLanguage)+"&nbsp;<a href=\"/Private/Results/IndicatorSeasonalityResults.aspx?idSession="+webSession.IdSession+"&sortOrder=asc\"><img src="+imageSortAsc+" border=0></a>&nbsp;<a href=\"/Private/Results/IndicatorSeasonalityResults.aspx?idSession="+webSession.IdSession+"&sortOrder=desc\"><img src="+imageSortDesc+" border=0></a></td>");
			t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1170,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1212,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1213,webSession.SiteLanguage)+"</td>");
			#endregion		
			
			t.Append("<td>&nbsp;</td>");

			#region Baisse
			if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.EvolutionRecap.ElementType.product){
				t.Append("<td class=\""+P2+"\" nowrap valign=\"top\"  align=\"center\">"+GestionWeb.GetWebWord(1209,webSession.SiteLanguage)+"<br>"+PeriodDate+"</td>");
			}
			else{
				t.Append("<td class=\""+P2+"\" nowrap valign=\"top\"  align=\"center\">"+GestionWeb.GetWebWord(1211,webSession.SiteLanguage)+"<br>"+PeriodDate+"</td>");
			}
			t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1170,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1212,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1213,webSession.SiteLanguage)+"</td>");
			#endregion

			t.Append("</tr>");
			#endregion
			
			// Parcours du rapide
			for(int i=0;i<tab.GetLongLength(0) && i<10 ;i++){

				#region Style css
				if(tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==2){
					styleClassTitle=competitorStyle;
					styleClassNumber=competitorStyleNB;
					if(excel){
						styleClassTitle=competitorExcelStyle;
						styleClassNumber=competitorExcelStyleNB;
					}
				}
				else if(tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)tab[i,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==1){
					styleClassTitle=referenceStyle;
					styleClassNumber=referenceStyleNB;
				}
				else{
					styleClassTitle=L1;
					styleClassNumber=L3;
				}
				#endregion
								
				t.Append("\r\n\t<tr>");

				#region Hausse
				if(double.Parse(tab[i,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString())>0){
					t.Append("<td class=\""+styleClassTitle+"\"  nowrap>"+tab[i,FrameWorkConstantes.Results.EvolutionRecap.PRODUCT].ToString()+"</td>");
//					t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+double.Parse(tab[i,FrameWorkConstantes.Results.EvolutionRecap.TOTAL_N].ToString()).ToString("### ### ###")+"</td>");
					t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+ WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkConstantes.Results.EvolutionRecap.TOTAL_N].ToString(),webSession.Unit)+"</td>");

					if(!Double.IsInfinity(double.Parse(tab[i,FrameWorkConstantes.Results.EvolutionRecap.EVOLUTION].ToString()))){
						t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+double.Parse(tab[i,FrameWorkConstantes.Results.EvolutionRecap.EVOLUTION].ToString()).ToString("0.##")+"</td>");
					}else{
						t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+GestionWeb.GetWebWord(1214,webSession.SiteLanguage)+"</td>");
					}
//					t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+double.Parse(tab[i,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString()).ToString("### ### ###")+"</td>");
					t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+ WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString(),webSession.Unit)+"</td>");
				}
				else {
					if(i!=10){	
						t.Append("<td class=\""+noData+" whiteRightBorder\" colspan=4></td>");
					}else{
						t.Append("<td class=\""+noData+" whiteRightBottomBorder\" colspan=4></td>");
					}
				
				}
				#endregion

				t.Append("<td>&nbsp;</td>");

				#region Style css
				if(tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==2){
					styleClassTitle=competitorStyle;
					styleClassNumber=competitorStyleNB;
					if(excel){
						styleClassTitle=competitorExcelStyle;
						styleClassNumber=competitorExcelStyleNB;
					}
				}
				else if(tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]!=null && (int)tab[last,FrameWorkConstantes.Results.EvolutionRecap.COMPETITOR]==1){
					styleClassTitle=referenceStyle;
					styleClassNumber=referenceStyleNB;
				}
				else{
					styleClassTitle=L1;
					styleClassNumber=L3;
				}
				#endregion

				#region Baisse
				if(double.Parse(tab[last,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString())<0){
					t.Append("<td class=\""+styleClassTitle+"\"  nowrap>"+tab[last,FrameWorkConstantes.Results.EvolutionRecap.PRODUCT].ToString()+"</td>");
//					t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+double.Parse(tab[last,FrameWorkConstantes.Results.EvolutionRecap.TOTAL_N].ToString()).ToString("### ### ###")+"</td>");
					t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[last,FrameWorkConstantes.Results.EvolutionRecap.TOTAL_N].ToString(),webSession.Unit)+"</td>");
					t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+double.Parse(tab[last,FrameWorkConstantes.Results.EvolutionRecap.EVOLUTION].ToString()).ToString("0.##")+"</td>");
//					t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+double.Parse(tab[last,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString()).ToString("### ### ###")+"</td>");
					t.Append("<td class=\""+styleClassNumber+"\"  nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[last,FrameWorkConstantes.Results.EvolutionRecap.ECART].ToString(),webSession.Unit)+"</td>");
				}
				else {
					if(i!=10){	
						t.Append("<td class=\""+noData+" whiteRightBorder\" colspan=4></td>");
					}else{
						t.Append("<td class=\""+noData+" whiteRightBottomBorder\" colspan=4></td>");
					}
				
				}
				#endregion

				t.Append("\r\n\t</tr>");	
					
				last--;
			}
			t.Append("</table>");
			return t.ToString();		
		}

		/// <summary>
		/// Affichage de l'ensemble des tableaux
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="excel">True si excel</param>
		/// <returns>Code</returns>
		public static string GetAllEvolutionIndicatorUI(WebSession webSession,bool excel){
		
			#region Variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);			
			DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
			
			string productTable="";
			string advertiserTable="";

			#endregion

			advertiserTable=GetEvolutionIndicatorUI(webSession,FrameWorkConstantes.Results.EvolutionRecap.ElementType.advertiser,excel);
			productTable=GetEvolutionIndicatorUI(webSession,FrameWorkConstantes.Results.EvolutionRecap.ElementType.product,excel);
			
			// Cas année N-2
			if(DateTime.Now.Year>webSession.DownLoadDate){
				if(PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-3)) {
                    t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
					t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");				
					t.Append(GestionWeb.GetWebWord(177,webSession.SiteLanguage));			
					t.Append("</td></tr></table>");
					return t.ToString();
				}
			}
			else{
				if(PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-2)) {
                    t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
					t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");				
					t.Append(GestionWeb.GetWebWord(177,webSession.SiteLanguage));			
					t.Append("</td></tr></table>");
					return t.ToString();
				}
			}
			
			#region Tableau global
            t.Append("<table class=\"tableFont\">");
			t.Append("<tr>");
			t.Append("<td valign=\"top\">");
			t.Append(advertiserTable);
			t.Append("</td>");
			t.Append("</tr>");
			if(excel){
				t.Append("<tr>");
				t.Append("<td valign=\"top\">&nbsp;");
				t.Append("</td>");
				t.Append("</tr>");
			
			}
			t.Append("<tr>");
			t.Append("<td valign=\"top\">");
			t.Append(productTable);
			t.Append("</td>");
			t.Append("</tr>");
			t.Append("</table>");
			#endregion

			return t.ToString();
		
		}
		#endregion

		#region Sortie Excel
		/// <summary>
		/// Sortie Excel de l'indicateur Evolution
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML pour Excel</returns>
		public static string GetAllEvolutionIndicatorExcelUI(WebSession webSession){
		
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

			#region Rappel des paramètres
			// Paramètres du tableau
			string temp="";
			if(webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION)
			{
				temp=GestionWeb.GetWebWord(1207,webSession.SiteLanguage);
			}
			else if(webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.SEASONALITY)
			{
				temp=GestionWeb.GetWebWord(1139,webSession.SiteLanguage);
			}
            t.Append(ExcelFunction.GetLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession,false,true,false,temp));

			#endregion

			t.Append(GetAllEvolutionIndicatorUI(webSession,true));
			t.Append(ExcelFunction.GetFooter(webSession));
			return Convertion.ToHtmlString(t.ToString());
		
		}
		#endregion

		#endregion
	}
}
