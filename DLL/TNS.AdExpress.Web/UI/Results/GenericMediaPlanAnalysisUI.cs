#region Informations
// Auteur: G. Facon 
// Date de création: 14/04/2006  
// Date de modification: 
#endregion

using System;
using System.Web;
using System.Windows.Forms;
using System.Web.UI;
using System.Globalization;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Constantes.FrameWork.Results;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork.Date;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Interface Utilisateur d'un plan Media
	/// Cette classe génère suivant le format de sortie le code pour afficher 
	/// un plan media.
	/// </summary>
	public class GenericMediaPlanAnalysisUI{ 

		#region Sortie HTML Avec détail support (Web)

		/// <summary>
		/// Crée le code HTML pour afficher une plan média
		/// </summary>
		/// <param name="tab">Tableau contenant les données</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="page">Page qui affiche le Plan Média</param>
		/// <returns>Code HTML</returns>
		public static string GetMediaPlanAnalysisWithMediaDetailLevelHtmlUI(object[,] tab,WebSession webSession){
			if(tab.GetLength(0)==0)return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
			
			#region Variables
			string classe;
			string HTML="";
			string HTML2="";
			string currentCategoryName="tit";
			string totalUnit="";
			int nbPeriodInYear=0;
			string prevYearString;
			int prevYear=0;
			int nbColTab=tab.GetLength(1),j,i;
			int nbPeriod=nbColTab-5;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
			#endregion

			#region Détermine s'il y a plusieurs années pour les totaux par année
			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
			//fin MAJ
			#endregion

			#region On calcule la taille de la colonne Total
			int nbtot=10;
			nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[DetailledMediaPlan.TOTAL_LINE_INDEX,DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit).Length;
			int nbSpace=(nbtot-1)/3;
			int nbCharTotal=nbtot+nbSpace-5;
			if(nbCharTotal<5)nbCharTotal=0;
			#endregion

			System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
						
			#region Libellé des colonnes
			t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0 >\r\n\t<tr>");
			
			t.Append("\r\n\t\t<td rowspan=2 width=\"250px\" class=\"pt\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");			
			t.Append("\r\n\t\t</td>");
			
			t.Append("\r\n\t\t<td rowspan=2 class=\"pt\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
			for(int h=0;h<nbCharTotal;h++){
				t.Append("&nbsp;");
			}
			t.Append("</td>");
			t.Append("\r\n\t\t<td rowspan=2 class=\"pt\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
		
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++){
				t.Append("\r\n\t\t<td rowspan=2 class=\"pt\">"+tab[0,k].ToString());
				for(int h=0;h<nbCharTotal+5;h++){
					t.Append("&nbsp;");
				}
				t.Append("</td>");
			}
			//fin MAJ

			// On affiche les mois
			prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				if(prevYear!=int.Parse(tab[0,j].ToString().Substring(0,4))){
					if(nbPeriodInYear<3)prevYearString="&nbsp;";//prevYear.ToString().Substring(3,1);
					else prevYearString=prevYear.ToString();
					HTML2+="<td colspan="+nbPeriodInYear+" class=\"pa1\">"+prevYearString+"</td>";
					nbPeriodInYear=0;
					prevYear=int.Parse(tab[0,j].ToString().Substring(0,4));

				}
				if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly){
				
					if(webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_POTENTIELS || webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
						|| webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE 
						|| webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE  ){
						HTML+="<td class=\"pp\" width=\"17px\"><a class=\"pp\" href=\""+ CstWeb.Links.ZOOM_PLAN_MEDIA_POP_UP + "?idSession=" + webSession.IdSession + "&zoomDate="+tab[0,j].ToString()+"\">&nbsp;"+TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(tab[0,j].ToString().Substring(4,2)),cultureInfo,1)+"&nbsp;</td>";
					}else{
                        HTML += "<td class=\"pp\" width=\"17px\"><a class=\"pp\" href=\"" + CstWeb.Links.ZOOM_PLAN_MEDIA + "?idSession=" + webSession.IdSession + "&zoomDate=" + tab[0, j].ToString() + "\">&nbsp;" + TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(tab[0, j].ToString().Substring(4, 2)), cultureInfo, 1) + "&nbsp;</td>";
					}					
				}
				else{
					if(webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_POTENTIELS || webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
						|| webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
						){
						HTML+="<td class=\"pp\" width=\"17px\"><a class=\"pp\" href=\""+ CstWeb.Links.ZOOM_PLAN_MEDIA_POP_UP + "?idSession=" + webSession.IdSession + "&zoomDate="+tab[0,j].ToString()+"\">&nbsp;"+tab[0,j].ToString().Substring(4,2)+"&nbsp;<a></td>";
					}
					else{
						HTML+="<td class=\"pp\" width=\"17px\"><a class=\"pp\" href=\""+ CstWeb.Links.ZOOM_PLAN_MEDIA + "?idSession=" + webSession.IdSession + "&zoomDate="+tab[0,j].ToString()+"\">&nbsp;"+tab[0,j].ToString().Substring(4,2)+"&nbsp;<a></td>";
					}
				}
				nbPeriodInYear++;
			}
			// On calcule la dernière date à afficher
			if(nbPeriodInYear<3)prevYearString="&nbsp;";//prevYear.ToString().Substring(3,1);
			else prevYearString=prevYear.ToString();

			HTML2+="<td colspan="+nbPeriodInYear+" class=\"pa\">"+prevYearString+"</td>";
			t.Append(HTML2+"</tr><tr>");
			

			t.Append(HTML+"</tr>");
			t.Append("\r\n\t<tr>");
			#endregion

			#region Tableau de résultat
			i=0;
			try{
				for(i=1;i<tab.GetLength(0);i++){
					for(j=0;j<tab.GetLength(1);j++){
						switch(j){
							#region Niveau L1
							case DetailledMediaPlan.L1_COLUMN_INDEX:
								if(tab[i,j]!=null){
									if(i==DetailledMediaPlan.TOTAL_LINE_INDEX) classe="L0";
									else classe="L1";
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									t.Append("\r\n\t<tr>\r\n\t\t<td class=\""+classe+"\">"+tab[i,j]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+10+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+10+nbColYear;
									//fin MAJ
								}
								break;
							#endregion
							
							#region Niveau L2
							case DetailledMediaPlan.L2_COLUMN_INDEX:
								if(tab[i,j]!=null){
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2\">&nbsp;"+tab[i,j]+"</td><td class=\"L2nb\">"+totalUnit+"</td><td class=\"L2nb\">"+((double)tab[i,DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									currentCategoryName=tab[i,j].ToString();
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										t.Append("<td class=\"L2nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+9+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+9+nbColYear;
									//fin MAJ
								}
								break;
							#endregion

							#region Niveau L3
							case DetailledMediaPlan.L3_COLUMN_INDEX:
								if(tab[i,j]!=null){
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);

									t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\"L3nb\">"+totalUnit+"</td><td class=\"L3nb\">"+((double)tab[i,DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										t.Append("<td class=\"L3nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+8+k].ToString(),webSession.Unit)+"</td>");
									}
									//fin MAJ
									j=j+8+nbColYear;
								}
								break;
							#endregion

							#region Niveau L4
							case DetailledMediaPlan.L4_COLUMN_INDEX:
								if(tab[i,j]!=null){
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);

									t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\"L4nb\">"+totalUnit+"</td><td class=\"L4nb\">"+((double)tab[i,DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										t.Append("<td class=\"L4nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+7+k].ToString(),webSession.Unit)+"</td>");
									}
									//fin MAJ

									j=j+7+nbColYear;
								}
								break;
							#endregion

							default:
								
								if(tab[i,j]==null){
									t.Append("<td class=\"p3\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(MediaPlanItem)){
									switch(((MediaPlanItem)tab[i,j]).GraphicItemType){
										case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
											t.Append("<td class=\"p4\">&nbsp;</td>");
											break;
										case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
											t.Append("<td class=\"p5\">-</td>");
											break;
										default:
											t.Append("<td class=\"p3\">-</td>");
											break;
									}
								}
								break;
						}
					}
					t.Append("</tr>");
				}
			}
			catch(System.Exception e){
				throw(new WebExceptions.MediaPlanUIException("erreur i="+i.ToString()+",j="+j.ToString(),e));
			}
			// Fin du tableau
			//t.Append("</tr></table></td></tr></table>");
			t.Append("</table>");
			#endregion

			// On vide le tableau
			tab=null;

			return(t.ToString());

		}
		#endregion

		#region Sortie HTML (Web) Pour Creative
		/// <summary>
		/// Crée le code HTML pour afficher une plan média à partir de Creative Explorer
		/// </summary>
		/// <param name="tab">Tableau contenant les données</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="page">Page qui affiche le Plan Média</param>
		/// <returns>Code HTML</returns>
		public static string GetCreativeMediaPlanAnalysisHtmlUI(Page page,object[,] tab,WebSession webSession){
			if(tab.GetLength(0)==0)return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
			
			#region Variables
			string classe;
			string HTML="";
			string HTML2="";
			string currentCategoryName="tit";
			string totalUnit="";
			int nbPeriodInYear=0;
			string prevYearString;
			int prevYear=0;
			int nbColTab=tab.GetLength(1),j,i;
			int nbPeriod=nbColTab-5;
			bool premier=true;
			const string PLAN_MEDIA_1_CLASSE="p6";
			const string PLAN_MEDIA_2_CLASSE="p7";
			const string PLAN_MEDIA_NB_1_CLASSE="p8";
			const string PLAN_MEDIA_NB_2_CLASSE="p9";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
			#endregion

			#region Détermine s'il y a plusieurs années pour les totaux par année
			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
			//fin MAJ
			#endregion

			#region On calcule la taille de la colonne Total
			int nbtot=10;
			nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,4].ToString(),webSession.Unit).Length;
			//			if(webSession.Unit==WebConstantes.CustomerSessions.Unit.pages) nbtot=double.Parse(tab[1,4].ToString()).ToString("############.##").Length;
			//			else nbtot=Int64.Parse(tab[1,4].ToString()).ToString("### ### ### ### ###").Length;
			int nbSpace=(nbtot-1)/3;
			int nbCharTotal=nbtot+nbSpace-5;
			if(nbCharTotal<5)nbCharTotal=0;
			#endregion

			System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
			
			// On enregistre le script DynamicMediaPlan qui rend les calendriers d'action dynamique
			//if (!page.ClientScript.IsClientScriptBlockRegistered("DynamicMediaPlan"))page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DynamicMediaPlan",TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan());


			#region Libellé des colonnes
			t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0 >\r\n\t<tr>");
			//t.Append("\r\n\t\t<td colspan=3  class=\"p2\">&nbsp;</td>");
			//t.Append("\r\n\t\t<td rowspan=2 width=\"300px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
			//t.Append("\r\n\t\t<td  class=\"pmpicto\">");
			t.Append("\n\t\t\t\t\t<td rowspan=2 width=\"250px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
			
			t.Append("\r\n\t\t<td rowspan=2 class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
			for(int h=0;h<nbCharTotal;h++){
				t.Append("&nbsp;");
			}
			t.Append("</td>");
			t.Append("\r\n\t\t<td align=\"center\" rowspan=2 class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
		
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++){
				t.Append("\r\n\t\t<td rowspan=2 class=\"p2\">"+tab[0,k].ToString());
				for(int h=0;h<nbCharTotal+5;h++){
					t.Append("&nbsp;");
				}
				t.Append("</td>");
			}
			//fin MAJ

			// On affiche les mois
			prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				if(prevYear!=int.Parse(tab[0,j].ToString().Substring(0,4))){
					if(nbPeriodInYear<3)prevYearString="&nbsp;";//prevYear.ToString().Substring(3,1);
					else prevYearString=prevYear.ToString();
					HTML2+="<td colspan="+nbPeriodInYear+" class=\"pmannee\">"+prevYearString+"</td>";
					nbPeriodInYear=0;
					prevYear=int.Parse(tab[0,j].ToString().Substring(0,4));

				}
				if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly){
						HTML+="<td class=\"p10\" width=\"17px\">&nbsp;"+TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(tab[0,j].ToString().Substring(4,2)),cultureInfo,1)+"&nbsp;</td>";
				}
				else{
						HTML+="<td class=\"p10\" width=\"17px\">&nbsp;"+tab[0,j].ToString().Substring(4,2)+"&nbsp;</td>";
				}
				nbPeriodInYear++;
			}
			// On calcule la dernière date à afficher
			if(nbPeriodInYear<3)prevYearString="&nbsp;";//prevYear.ToString().Substring(3,1);
			else prevYearString=prevYear.ToString();

			HTML2+="<td colspan="+nbPeriodInYear+" class=\"pmannee2\">"+prevYearString+"</td>";
			t.Append(HTML2+"</tr><tr>");
			

			t.Append(HTML+"</tr>");
			t.Append("\r\n\t<tr>");
			#endregion

			#region Tableau de résultat
			i=0;
			try{
				for(i=1;i<tab.GetLength(0);i++){
					for(j=0;j<tab.GetLength(1);j++){
						switch(j){
								// Total ou vehicle
							case MediaPlan.VEHICLE_COLUMN_INDEX:
								if(tab[i,j]!=null){
									if(i==MediaPlan.TOTAL_LINE_INDEX) classe="pmtotal";
									else classe="pmvehicle";
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4].ToString(),webSession.Unit);
									t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,j]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,j+5]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										if(WebFunctions.Units.ConvertUnitValueToString(tab[i,j+5+k].ToString(),webSession.Unit).ToString().Trim().Length==0)
											t.Append("<td class=\""+classe+"nb\">&nbsp;</td>");
										else
											t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+5+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+5+nbColYear;
									//fin MAJ
								}
								break;
								// Category
							case MediaPlan.CATEGORY_COLUMN_INDEX:
								if(tab[i,j]!=null){
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3].ToString(),webSession.Unit);
									t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\">\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,j]+"</td><td class=\"pmcategorynb\">"+totalUnit+"</td><td class=\"pmcategorynb\">"+((double)tab[i,j+4]).ToString("0.00")+"</td>");
									currentCategoryName=tab[i,j].ToString();
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										if(WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4+k].ToString(),webSession.Unit).Trim().Length==0)
											t.Append("<td class=\"pmcategorynb\">&nbsp;</td>");
										else
											t.Append("<td class=\"pmcategorynb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+4+nbColYear;
									//fin MAJ
								}
								break;
								// Media
							case MediaPlan.MEDIA_COLUMN_INDEX:
								// On traite le cas des pages
								totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+2].ToString(),webSession.Unit);
								if(premier){
									t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										if(WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit).Trim().Length==0)
											t.Append("<td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">&nbsp;</td>");
										else
											t.Append("<td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit)+"</td>");
									}
									//fin MAJ
									premier=false;
								}
								else{
									t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										if(WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit).Trim().Length==0)
											t.Append("<td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">&nbsp;</td>");
										else
											t.Append("<td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit)+"</td>");
									}
									//fin MAJ
									premier=true;
								}	
								j=j+3+nbColYear;
								break;
							default:
								
								if(tab[i,j]==null){
									t.Append("<td class=\"p3\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlan.graphicItemType)){
									switch((FrameWorkResultConstantes.MediaPlan.graphicItemType)tab[i,j]){
										case FrameWorkResultConstantes.MediaPlan.graphicItemType.present:
											t.Append("<td class=\"p4\">&nbsp;</td>");
											break;
										case FrameWorkResultConstantes.MediaPlan.graphicItemType.extended:
											t.Append("<td class=\"p5\">-</td>");
											break;
										default:
											t.Append("<td class=\"p3\">-</td>");
											break;
									}
								}
								break;
						}
					}
					t.Append("</tr>");
				}
			}
			catch(System.Exception e){
				throw(new System.Exception("erreur i="+i.ToString()+",j="+j.ToString(),e));
			}
			// Fin du tableau
			//t.Append("</tr></table></td></tr></table>");
			t.Append("</table>");
			#endregion

			// On vide le tableau
			tab=null;

			return(t.ToString());

		}
		#endregion

		#region Sortie Ms Excel Pour Creative
		
		/// <summary>
		/// Crée le code HTML pour Excel pour afficher un plan media
		/// </summary>
		/// <param name="tab">Tableau contenant les données</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML</returns>
		public static string GetCreativeMediaPlanAnalysisExcelUI(object[,] tab,WebSession webSession){
			
			#region variables
			if(tab.GetLength(0)==0)return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
			string classe;
			string HTML="";
			string HTML2="";
			int nbPeriodInYear=0;
			int prevYear=0;
			int nbColTab=tab.GetLength(1),j,i;
			int nbPeriod=nbColTab-5;
			bool premier=true;
			const string PLAN_MEDIA_XLS_1_CLASSE="pmmediaxls1";
			const string PLAN_MEDIA_XLS_2_CLASSE="pmmediaxls2";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
            
			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
			//fin MAJ
			
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			string totalUnit="";
			#endregion

			//t.Append(ExcelFunction.GetExcelHeader(webSession,true,true));
			// Rappel de la sélection
			string dateBegin,dateEnd;
			dateBegin=DateString.dateTimeToDD_MM_YYYY((new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.Substring(4,2)))).FirstDay,webSession.SiteLanguage);
			dateEnd=DateString.dateTimeToDD_MM_YYYY((new AtomicPeriodWeek(int.Parse(webSession.PeriodEndDate.Substring(0,4)),int.Parse(webSession.PeriodEndDate.Substring(4,2)))).LastDay,webSession.SiteLanguage);
			#region Ajout CSS
			t.Append("<Style><!--");
			t.Append(".TexteTitreRappelScanpub { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color: #FFFFFF; font-weight:bold; }");
			t.Append(".TexteVioletPetitScanpub { font-family: Arial, Helvetica, sans-serif; font-size: 12px; color: #5A5A90; font-weight:bold }");
			t.Append(".TexteVioletGrandScanpub { font-family: Arial, Helvetica, sans-serif; font-size: 13px; color: #5A5A90; font-weight:bold }");
			t.Append(".TexteGris2Scanpub { font-family: Arial, Helvetica, sans-serif; font-size: 10px; color: #454545; }");
			t.Append(".Arial7rouge {  font-family: Arial, Helvetica, sans-serif; font-size: 8pt; font-weight: normal ; color: #DE3252 }");
			t.Append(".Arial7gris {  font-family: Arial, Helvetica, sans-serif; font-size: 8pt; font-weight: normal ; color: #666666 }");
			t.Append(".Arial10gris {  font-family: Arial, Helvetica, sans-serif; font-size: 10pt; font-weight: normal ; color: #666666 }");
			t.Append("td.p1{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-decoration : none;background-color: #644883;padding-left:5px;padding-right:5px;}");
			t.Append("td.p2{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-decoration : none;background-color: #644883;padding-left:5px;padding-right:5px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;}");
			t.Append("td.p2excel{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-decoration : none;background-color: #644883;padding-left:5px;padding-right:5px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:0px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;}");
			t.Append("td.pmannee{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-align:center;text-decoration : none;background-color: #644883;padding-left:5px;padding-right:5px;border-left-width:0px;border-top-width:0px;border-bottom-width:0px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;}");
			t.Append("td.pmannee2{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-align:center;text-decoration : none;background-color: #644883;padding-left:5px;padding-right:5px;border-left-width:0px;border-top-width:0px;border-bottom-width:0px;border-bottom-style:solid;border-right-width: 0px;}");
			t.Append("td.p10{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;text-align:center;border-left-width:0px;border-top-width:0px;border-bottom-width:0px;border-right-color:#644883;border-right-width: 1px; border-right-style:solid;background-color: #B1A3C1;width:17px;}");
			t.Append("td.pmtotal{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;background-color: #FFFFFF;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;}");
			t.Append("td.pmvehicle{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-decoration : none;background-color: #9D9885;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;}");
			t.Append("td.pmcategory{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;background-color: #B1A3C1;padding:0px,0px,0px,2px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;}");
			t.Append("td.pmmediaxls1{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight:normal;text-decoration : none;background-color: #9999FF;padding:0px,0px,0px,2px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;}");
			t.Append("td.pmmediaxls2{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight:normal;text-decoration : none;background-color: #CCCCFF;	padding:0px,0px,0px,2px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;}");
			t.Append("td.pmperiodxls{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;text-align:center;border-left-width:0px;border-top-width:0px;border-bottom-width:0px;border-right-color:#644883;border-right-width: 1px; border-right-style:solid;background-color: #B1A3C1;width:15pt;}");
			t.Append("td.pmperioditemxls1{border-left-width:0px;border-top-width:0px;border-bottom-color:#B1A3C1;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#B1A3C1;border-right-width: 1px; border-right-style:solid;background-color: #B1A3C1;width:15pt;}");
			t.Append("td.pmperioditemxls{border-left-width:0px;border-top-width:0px;border-bottom-color:#B1A3C1;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#B1A3C1;border-right-width: 1px; border-right-style:solid;background-color: #666699;width:15pt;}");
			t.Append("td.pmperioditemxls2{border-left-width:0px;border-top-width:0px;border-bottom-color:#B1A3C1;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#B1A3C1;border-right-width: 1px; border-right-style:solid;background-color: #9999FF;width:15pt;}");
			t.Append("td.pmtotalnb{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;background-color: #FFFFFF;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;text-align:right;}");
			t.Append("td.pmvehiclenb{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-decoration : none;background-color: #9D9885;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;text-align:right}");
			t.Append("td.pmcategorynb{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;background-color: #B1A3C1;padding:0px,0px,0px,2px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;text-align:right}");
			t.Append("</Style>-->");
			#endregion
			t.Append("<table border=0 cellspacing=1 cellpadding=0 bgcolor=#999999 class=\"TexteTitreRappelScanpub\">");
			// Titre
			t.Append("<tr>");
			t.Append("<td align=center bgcolor=#644883>&nbsp;&nbsp;"+GestionWeb.GetWebWord(1791,webSession.SiteLanguage)+"&nbsp;&nbsp;</td>");
			t.Append("</tr>");
			t.Append("<tr>");
			t.Append("<td>");
			t.Append("<table border=0 cellspacing=0 cellpadding=0 bgcolor=#ffffff width=100% >");
			// Media
			t.Append("<tr valign=top>");
			t.Append("<td class=Arial7rouge>&nbsp;&nbsp;"+GestionWeb.GetWebWord(1792,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=Arial7gris>:");
			foreach(TreeNode currentNode in webSession.SelectionUniversMedia.Nodes){
				t.Append("&nbsp;"+((LevelInformation)currentNode.Tag).Text+"<br>");	
			}
			t.Append("</td>");
			t.Append("</tr>");
			//Produit
			t.Append("<tr valign=top>");
			t.Append("<td class=Arial7rouge>&nbsp;&nbsp;"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=Arial7gris>:&nbsp;");
			foreach(TreeNode currentNode in webSession.SelectionUniversAdvertiser.Nodes){
				t.Append(((LevelInformation)currentNode.Tag).Text+"&nbsp;&nbsp;<br>");	
			}
			t.Append("</td>");
			t.Append("</tr>");
			//Date début
			t.Append("<tr>");
			t.Append("<td class=Arial7rouge>&nbsp;&nbsp;"+GestionWeb.GetWebWord(1793,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=Arial7gris>:&nbsp;"+dateBegin+"</td>");
			t.Append("</tr>");
			// Date fin
			t.Append("<tr>");
			t.Append("<td class=Arial7rouge>&nbsp;&nbsp;"+GestionWeb.GetWebWord(1794,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=Arial7gris>:&nbsp;"+dateEnd+"</td>");
			t.Append("</tr>");
			//Unité
			t.Append("<tr>");
			t.Append("<td class=Arial7rouge>&nbsp;&nbsp;"+GestionWeb.GetWebWord(1795,webSession.SiteLanguage)+"</td>");
			t.Append("<td colspan=2 class=Arial7gris>:&nbsp;Euro</td>");
			t.Append("</tr>");
			t.Append("</table>");
			t.Append("</td>");
			t.Append("</tr>");
			t.Append("</table>");


			t.Append("<table border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");

			t.Append("\r\n\t\t<td width=\"230px\" class=\"p2\"></td>");
			t.Append("\r\n\t\t<td class=\"p2\"></td>");
			t.Append("\r\n\t\t<td class=\"p2\"></td>");
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++) {
				t.Append("\r\n\t\t<td class=\"p2\"></td>");
			}
			//fin MAJ
		
			// On affiche les mois
			//t.Append("\r\n\t\t<td colspan="+nbPeriod+" class=\"pmperiods\">");
			//t.Append("\r\n\t\t\t<table cellpadding=0 cellspacing=0 bgcolor=#644883 border=0>\r\n\t\t\t\t<tr>");
			prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				if(prevYear!=int.Parse(tab[0,j].ToString().Substring(0,4))){
					HTML2+="<td width=\""+nbPeriodInYear*20+"px\" colspan="+nbPeriodInYear+" class=\"pmannee\">"+prevYear+"</td>";
					nbPeriodInYear=0;
					prevYear=int.Parse(tab[0,j].ToString().Substring(0,4));

				}
				if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly){
					HTML+="<td style=\"width:15pt\" class=\"p10\">"+TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(tab[0,j].ToString().Substring(4,2)),cultureInfo,1)+"</td>";
				}
				else{
					HTML+="<td style=\"width:15pt\" class=\"p10\">"+tab[0,j].ToString().Substring(4,2)+"</td>";
				}
				nbPeriodInYear++;
			}
			HTML2+="<td width=\""+nbPeriodInYear*20+"px\" colspan="+nbPeriodInYear+" class=\"pmannee2\">"+prevYear+"</td>";
			t.Append(HTML2+"</tr><tr>");
			//t.Append("<td><table cellpadding=0 cellspacing=0 bgcolor=#644883 border=0><tr>"+HTML2+"</tr><tr>"+HTML+"</tr></table></td>");
			t.Append("\r\n\t\t<td width=\"230px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
			t.Append("\r\n\t\t<td class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
			t.Append("\r\n\t\t<td class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++) {
				t.Append("\r\n\t\t<td class=\"p2\">"+tab[0,k].ToString()+"</td>");
			}
			//fin MAJ

			t.Append(HTML);
			//t.Append("\r\n\t\t\t\t</tr>\r\n\t\t\t</table>");
			//t.Append("\r\n\t\t</td>");
			t.Append("\r\n\t</tr>");
			//t.Append("\r\n\t<tr>\r\n\t\t<td bgclor=#FFFFFF colspan=nbColTab><img src=\"images/pixel.gif\" width=1 height=1>\r\n\t</tr>");
			t.Append("\r\n\t<tr>");
			i=0;
			try{
				for(i=1;i<tab.GetLength(0);i++){
					for(j=0;j<tab.GetLength(1);j++){
						switch(j){
								// Total ou vehicle
							case MediaPlan.VEHICLE_COLUMN_INDEX:
								if(tab[i,j]!=null){
									if(i==MediaPlan.TOTAL_LINE_INDEX) classe="pmtotal";
									else classe="pmvehicle";
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									//formattage des unités
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4].ToString(),webSession.Unit);
									//									t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,j]+"</td><td class=\""+classe+"\">"+tab[i,j+4]+"</td><td class=\""+classe+"\">"+((double)tab[i,j+5]).ToString("0.00")+"</td>");
									t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,j]+"</td><td class=\""+classe+"\">"+totalUnit+"</td><td class=\""+classe+"\">"+((double)tab[i,j+5]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++) {
										//t.Append("<td class=\""+classe+"nb\">"+((double)tab[i,j+5+k]).ToString("0.##")+"</td>");
										t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+5+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+5+nbColYear;
									//fin MAJ								
								}
								break;
								// Category
							case MediaPlan.CATEGORY_COLUMN_INDEX:
								if(tab[i,j]!=null){
									//formattage des unités
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3].ToString(),webSession.Unit);
									//									t.Append("\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,j]+"</td><td class=\"pmcategory\">"+tab[i,j+3]+"</td><td class=\"pmcategory\">"+((double)tab[i,j+4]).ToString("0.00")+"</td>");
									t.Append("\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,j]+"</td><td class=\"pmcategory\">"+totalUnit+"</td><td class=\"pmcategory\">"+((double)tab[i,j+4]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++) {
										//t.Append("<td class=\"pmcategorynb\">"+((double)tab[i,j+4+k]).ToString("0.##")+"</td>");
										t.Append("<td class=\"pmcategorynb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+4+nbColYear;
									//fin MAJ
								}
								break;
								// Media
							case MediaPlan.MEDIA_COLUMN_INDEX:
								// formattage des unités
								totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+2].ToString(),webSession.Unit);
								if(premier){
									//									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+tab[i,j+2]+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");																		
									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++) {
										//										t.Append("<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,j+3+k]).ToString("0.##")+"</td>");
										t.Append("<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit)+"</td>");										
									}
									//fin MAJ
									premier=false;
								}
								else{
									//									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+tab[i,j+2]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++) {
										//										t.Append("<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,j+3+k]).ToString("0.##")+"</td>");
										t.Append("<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit)+"</td>");
									}
									//fin MAJ
									premier=true;
								}	
								j=j+3+nbColYear;
								break;
							default:
								
								if(tab[i,j]==null){
									t.Append("<td class=\"pmperioditemxls1\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlan.graphicItemType)){
									switch((FrameWorkResultConstantes.MediaPlan.graphicItemType)tab[i,j]){
										case FrameWorkResultConstantes.MediaPlan.graphicItemType.present:
											t.Append("<td class=\"pmperioditemxls\">&nbsp;</td>");
											break;
										case FrameWorkResultConstantes.MediaPlan.graphicItemType.extended:
											t.Append("<td class=\"pmperioditemxls2\">&nbsp;</td>");
											break;
										default:
											t.Append("<td class=\"pmperioditemxls1\">&nbsp;</td>");
											break;
									}
								}
								break;
						}
					}
					t.Append("</tr><tr>");
				}
			}
			catch(System.Exception e){
				throw(new System.Exception("erreur i="+i.ToString()+",j="+j.ToString(),e));
			}
			// Fin du tableau
			t.Append("</tr></table></td></tr></table>");


			// On vide le tableau
			tab=null;
			return(t.ToString());
		}
		#endregion

		#region Sortie HTML (Web)

		/// <summary>
		/// Crée le code HTML pour afficher une plan média
		/// </summary>
		/// <param name="tab">Tableau contenant les données</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="page">Page qui affiche le Plan Média</param>
		/// <returns>Code HTML</returns>
		public static string GetMediaPlanAnalysisHtmlUI(Page page,object[,] tab,WebSession webSession){
			if(tab.GetLength(0)==0)return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
			
			#region Variables
			string classe;
			string HTML="";
			string HTML2="";
			string currentCategoryName="tit";
			string totalUnit="";
			int nbPeriodInYear=0;
			string prevYearString;
			int prevYear=0;
			int nbColTab=tab.GetLength(1),j,i;
			int nbPeriod=nbColTab-5;
			bool premier=true;
			const string PLAN_MEDIA_1_CLASSE="p6";
			const string PLAN_MEDIA_2_CLASSE="p7";
			const string PLAN_MEDIA_NB_1_CLASSE="p8";
			const string PLAN_MEDIA_NB_2_CLASSE="p9";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
			#endregion

			#region Détermine s'il y a plusieurs années pour les totaux par année
			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
			//fin MAJ
			#endregion

			#region On calcule la taille de la colonne Total
			int nbtot=10;
			nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,4].ToString(),webSession.Unit).Length;
//			if(webSession.Unit==WebConstantes.CustomerSessions.Unit.pages) nbtot=double.Parse(tab[1,4].ToString()).ToString("############.##").Length;
//			else nbtot=Int64.Parse(tab[1,4].ToString()).ToString("### ### ### ### ###").Length;
			int nbSpace=(nbtot-1)/3;
			int nbCharTotal=nbtot+nbSpace-5;
			if(nbCharTotal<5)nbCharTotal=0;
			#endregion

			System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
			
			// On enregistre le script DynamicMediaPlan qui rend les calendriers d'action dynamique
            if (!page.ClientScript.IsClientScriptBlockRegistered("DynamicMediaPlan")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "DynamicMediaPlan", TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan());
			
			#region Libellé des colonnes
			t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0 >\r\n\t<tr>");
			//t.Append("\r\n\t\t<td colspan=3  class=\"p2\">&nbsp;</td>");
			//t.Append("\r\n\t\t<td rowspan=2 width=\"300px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
			t.Append("\r\n\t\t<td rowspan=2 width=\"250px\" class=\"pmpicto\">");
			t.Append("\n\t\t\t<table width=\"250px\" border=0 cellpadding=0 cellspacing=0 bgcolor=#644883>\r\n\t\t\t\t<tr>");
			t.Append("\n\t\t\t\t\t<td class=\"p1\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
			t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:showAllContent3();\"><img id=pictofermer border=0 src=\"/Images/Common/button/picto_fermer.gif\"></a></td>");
			t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:hideAllContent3();\"><img id=pictoouvrir border=0 src=\"/Images/Common/button/picto_ouvrir.gif\"></a></td>");
			t.Append("\n\t\t\t\t\t<td bgcolor=#ffffff width=1px><img src=\"/Images/Common/picto.gif\" width=1px></td>");
			t.Append("\n\t\t\t\t</tr>\n\t\t\t</table>");
			t.Append("\r\n\t\t</td>");
			
			t.Append("\r\n\t\t<td rowspan=2 class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
			for(int h=0;h<nbCharTotal;h++){
				t.Append("&nbsp;");
			}
			t.Append("</td>");
			t.Append("\r\n\t\t<td rowspan=2 class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
		
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++){
				t.Append("\r\n\t\t<td rowspan=2 class=\"p2\">"+tab[0,k].ToString());
				for(int h=0;h<nbCharTotal+5;h++){
					t.Append("&nbsp;");
				}
				t.Append("</td>");
			}
			//fin MAJ

			// On affiche les mois
			prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				if(prevYear!=int.Parse(tab[0,j].ToString().Substring(0,4))){
					if(nbPeriodInYear<3)prevYearString="&nbsp;";//prevYear.ToString().Substring(3,1);
					else prevYearString=prevYear.ToString();
					HTML2+="<td colspan="+nbPeriodInYear+" class=\"pmannee\">"+prevYearString+"</td>";
					nbPeriodInYear=0;
					prevYear=int.Parse(tab[0,j].ToString().Substring(0,4));

				}
				if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly){
				
					if(webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_POTENTIELS || webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
						|| webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE 
						|| webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE  ){
						HTML+="<td class=\"p10\" width=\"17px\"><a class=\"p10\" href=\""+ CstWeb.Links.ZOOM_PLAN_MEDIA_POP_UP + "?idSession=" + webSession.IdSession + "&zoomDate="+tab[0,j].ToString()+"\">&nbsp;"+TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(tab[0,j].ToString().Substring(4,2)),cultureInfo,1)+"&nbsp;</td>";
					}else{
						HTML+="<td class=\"p10\" width=\"17px\"><a class=\"p10\" href=\""+ CstWeb.Links.ZOOM_PLAN_MEDIA + "?idSession=" + webSession.IdSession + "&zoomDate="+tab[0,j].ToString()+"\">&nbsp;"+TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(tab[0,j].ToString().Substring(4,2)),cultureInfo,1)+"&nbsp;</td>";
					}					
				}
				else{
					if(webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_POTENTIELS || webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
						|| webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
						){
						HTML+="<td class=\"p10\" width=\"17px\"><a class=\"p10\" href=\""+ CstWeb.Links.ZOOM_PLAN_MEDIA_POP_UP + "?idSession=" + webSession.IdSession + "&zoomDate="+tab[0,j].ToString()+"\">&nbsp;"+tab[0,j].ToString().Substring(4,2)+"&nbsp;<a></td>";
					}
					else{
						HTML+="<td class=\"p10\" width=\"17px\"><a class=\"p10\" href=\""+ CstWeb.Links.ZOOM_PLAN_MEDIA + "?idSession=" + webSession.IdSession + "&zoomDate="+tab[0,j].ToString()+"\">&nbsp;"+tab[0,j].ToString().Substring(4,2)+"&nbsp;<a></td>";
					}
				}
				nbPeriodInYear++;
			}
			// On calcule la dernière date à afficher
			if(nbPeriodInYear<3)prevYearString="&nbsp;";//prevYear.ToString().Substring(3,1);
			else prevYearString=prevYear.ToString();

			HTML2+="<td colspan="+nbPeriodInYear+" class=\"pmannee2\">"+prevYearString+"</td>";
			t.Append(HTML2+"</tr><tr>");
			

			t.Append(HTML+"</tr>");
			t.Append("\r\n\t<tr>");
			#endregion

			#region Tableau de résultat
			i=0;
			try{
				for(i=1;i<tab.GetLength(0);i++){
					for(j=0;j<tab.GetLength(1);j++){
						switch(j){
							// Total ou vehicle
							case MediaPlan.VEHICLE_COLUMN_INDEX:
								if(tab[i,j]!=null){
									if(i==MediaPlan.TOTAL_LINE_INDEX) classe="pmtotal";
									else classe="pmvehicle";
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4].ToString(),webSession.Unit);
									t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,j]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,j+5]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+5+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+5+nbColYear;
									//fin MAJ
								}
								break;
								// Category
							case MediaPlan.CATEGORY_COLUMN_INDEX:
								if(tab[i,j]!=null){
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3].ToString(),webSession.Unit);
									t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,j]+"');\" style=\"DISPLAY:inline; CURSOR: hand\">\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,j]+"</td><td class=\"pmcategorynb\">"+totalUnit+"</td><td class=\"pmcategorynb\">"+((double)tab[i,j+4]).ToString("0.00")+"</td>");
									currentCategoryName=tab[i,j].ToString();
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										t.Append("<td class=\"pmcategorynb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+4+nbColYear;
									//fin MAJ
								}
								break;
								// Media
							case MediaPlan.MEDIA_COLUMN_INDEX:
								// On traite le cas des pages
								totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+2].ToString(),webSession.Unit);
								if(premier){
									t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										t.Append("<td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit)+"</td>");
									}
									//fin MAJ
									premier=false;
								}
								else{
									t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										t.Append("<td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit)+"</td>");
									}
									//fin MAJ
									premier=true;
								}	
								j=j+3+nbColYear;
								break;
							default:
								
								if(tab[i,j]==null){
									t.Append("<td class=\"p3\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlan.graphicItemType)){
									switch((FrameWorkResultConstantes.MediaPlan.graphicItemType)tab[i,j]){
										case FrameWorkResultConstantes.MediaPlan.graphicItemType.present:
											t.Append("<td class=\"p4\">&nbsp;</td>");
											break;
										case FrameWorkResultConstantes.MediaPlan.graphicItemType.extended:
											t.Append("<td class=\"p5\">-</td>");
											break;
										default:
											t.Append("<td class=\"p3\">-</td>");
											break;
									}
								}
								break;
						}
					}
					t.Append("</tr>");
				}
			}
			catch(System.Exception e){
				throw(new System.Exception("erreur i="+i.ToString()+",j="+j.ToString(),e));
			}
			// Fin du tableau
			//t.Append("</tr></table></td></tr></table>");
			t.Append("</table>");
			#endregion

			// On vide le tableau
			tab=null;

			return(t.ToString());

		}
		#endregion

		#region Sortie Ms Excel Avec détail support
		/// <summary>
		/// Crée le code HTML pour Excel pour afficher un plan media
		/// </summary>
		/// <param name="tab">Tableau contenant les données</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="showValue">Valeurs dans les cellules</param>
		/// <returns>Code HTML</returns>
		public static string GetMediaPlanAnalysisWithMediaDetailLevelExcelUI(object[,] tab,WebSession webSession,bool showValue){
			
			#region variables
			if(tab.GetLength(0)==0)return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
			string classe;
			string HTML="";
			string HTML2="";
			int nbPeriodInYear=0;
			int prevYear=0;
			int nbColTab=tab.GetLength(1),j,i;
			int nbPeriod=nbColTab-5;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization); 

			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
			//fin MAJ
			
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			string totalUnit="";
			#endregion

			#region Rappel de sélection
            t.Append(ExcelFunction.GetLogo(webSession));
			if(webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA || webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_PLAN_MEDIA){
				t.Append(ExcelFunction.GetExcelHeader(webSession,true,true));
			}
			else{
				t.Append(ExcelFunction.GetExcelHeaderForMediaPlanPopUp(webSession,true,null,null));
			}
			#endregion

			t.Append("<table border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");

			t.Append("\r\n\t\t<td width=\"230px\" class=\"ptX\"></td>");
			t.Append("\r\n\t\t<td class=\"ptX\"></td>");
			t.Append("\r\n\t\t<td class=\"ptX\"></td>");
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++) {
				t.Append("\r\n\t\t<td class=\"ptX\"></td>");
			}
			//fin MAJ
		
			// On affiche les mois
			prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				if(prevYear!=int.Parse(tab[0,j].ToString().Substring(0,4))){
					HTML2+="<td width=\""+nbPeriodInYear*20+"px\" colspan="+nbPeriodInYear+" class=\"paX\">"+prevYear+"</td>";
					nbPeriodInYear=0;
					prevYear=int.Parse(tab[0,j].ToString().Substring(0,4));

				}
				if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly){
					HTML+="<td style=\"width:15pt\" class=\"ppX\">"+TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(tab[0,j].ToString().Substring(4,2)),cultureInfo,1)+"</td>";
				}
				else{
					HTML+="<td style=\"width:15pt\" class=\"ppX\">"+tab[0,j].ToString().Substring(4,2)+"</td>";
				}
				nbPeriodInYear++;
			}
			HTML2+="<td width=\""+nbPeriodInYear*20+"px\" colspan="+nbPeriodInYear+" class=\"paX\">"+prevYear+"</td>";
			t.Append(HTML2+"</tr><tr>");
			//t.Append("<td><table cellpadding=0 cellspacing=0 bgcolor=#644883 border=0><tr>"+HTML2+"</tr><tr>"+HTML+"</tr></table></td>");
			t.Append("\r\n\t\t<td width=\"230px\" class=\"ptX\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
			t.Append("\r\n\t\t<td class=\"ptX\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
			t.Append("\r\n\t\t<td class=\"ptX\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++) {
				t.Append("\r\n\t\t<td class=\"ptX\">"+tab[0,k].ToString()+"</td>");
			}
			//fin MAJ

			t.Append(HTML);
			t.Append("\r\n\t</tr>");
			t.Append("\r\n\t<tr>");
			i=0;
			try{
				for(i=1;i<tab.GetLength(0);i++){
					for(j=0;j<tab.GetLength(1);j++){
						switch(j){
							#region L1
							case DetailledMediaPlan.L1_COLUMN_INDEX:
								if(tab[i,j]!=null){
									if(i==DetailledMediaPlan.TOTAL_LINE_INDEX) classe="L0X";
									else classe="L1X";
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									//formattage des unités
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,j]+"</td><td class=\""+classe+"\">"+totalUnit+"</td><td class=\""+classe+"\">"+((double)tab[i,DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++) {
										//t.Append("<td class=\""+classe+"nb\">"+((double)tab[i,j+5+k]).ToString("0.##")+"</td>");
										t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+10+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+10+nbColYear;
									//fin MAJ								
								}
								break;
							#endregion

							#region L2
							case DetailledMediaPlan.L2_COLUMN_INDEX:
								if(tab[i,j]!=null){
									//formattage des unités
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									t.Append("\r\n\t\t<td class=\"L2X\">&nbsp;"+tab[i,j]+"</td><td class=\"L2Xnb\">"+totalUnit+"</td><td class=\"L2Xnb\">"+((double)tab[i,DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++) {
										//t.Append("<td class=\"pmcategorynb\">"+((double)tab[i,j+4+k]).ToString("0.##")+"</td>");
										t.Append("<td class=\"L2Xnb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+9+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+9+nbColYear;
									//fin MAJ
								}
								break;
							#endregion

							#region L3
							case DetailledMediaPlan.L3_COLUMN_INDEX:
								if(tab[i,j]!=null){
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									t.Append("\r\n\t\t<td class=\"L3X\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\"L3Xnb\">"+totalUnit+"</td><td class=\"L3Xnb\">"+((double)tab[i,DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									for(k=1; k<=nbColYear; k++) {
										t.Append("<td class=\"L3Xnb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+8+k].ToString(),webSession.Unit)+"</td>");										
									}
									j=j+8+nbColYear;
								}
								break;
							#endregion

							#region L4
							case DetailledMediaPlan.L4_COLUMN_INDEX:
								if(tab[i,j]!=null){
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									t.Append("\r\n\t\t<td class=\"L4X\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\"L4Xnb\">"+totalUnit+"</td><td class=\"L4Xnb\">"+((double)tab[i,DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									for(k=1; k<=nbColYear; k++) {
										t.Append("<td class=\"L4Xnb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+7+k].ToString(),webSession.Unit)+"</td>");										
									}
									j=j+7+nbColYear;
								}
								break;
							#endregion

							default:
								
								if(tab[i,j]==null){
									t.Append("<td class=\"p3\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(MediaPlanItem)){
									switch(((MediaPlanItem)tab[i,j]).GraphicItemType){
										case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
											if(showValue)t.Append("<td class=\"pmperioditemxls\">"+Functions.Units.ConvertUnitValueToString(((MediaPlanItem)tab[i,j]).Unit.ToString(),webSession.Unit)+"</td>");
											else t.Append("<td class=\"p4\">&nbsp;</td>");
											break;
										case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
											t.Append("<td class=\"p5\">&nbsp;</td>");
											break;
										default:
											t.Append("<td class=\"p3\">&nbsp;</td>");
											break;
									}
								}
								break;
						}
					}
					t.Append("</tr><tr>");
				}
			}
			catch(System.Exception e){
				throw(new WebExceptions.MediaPlanUIException("erreur i="+i.ToString()+",j="+j.ToString(),e));
			}
			// Fin du tableau
			t.Append("</tr></table></td></tr></table>");


			// On vide le tableau
			tab=null;
			t.Append(ExcelFunction.GetFooter(webSession));
			return(t.ToString());
		}
		#endregion

		#region Sortie Ms Excel

		/// <summary>
		/// Crée le code HTML pour Excel pour afficher un plan media
		/// </summary>
		/// <param name="tab">Tableau contenant les données</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML</returns>
		public static string GetMediaPlanAnalysisExcelUI(object[,] tab,WebSession webSession){
			
			#region variables
			if(tab.GetLength(0)==0)return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
			string classe;
			string HTML="";
			string HTML2="";
			int nbPeriodInYear=0;
			int prevYear=0;
			int nbColTab=tab.GetLength(1),j,i;
			int nbPeriod=nbColTab-5;
			bool premier=true;
			const string PLAN_MEDIA_XLS_1_CLASSE="pmmediaxls1";
			const string PLAN_MEDIA_XLS_2_CLASSE="pmmediaxls2";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);

			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
			//fin MAJ
			
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			string totalUnit="";
			#endregion

            t.Append(ExcelFunction.GetLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession,true,true));

			t.Append("<table border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");

			t.Append("\r\n\t\t<td width=\"230px\" class=\"p2\"></td>");
			t.Append("\r\n\t\t<td class=\"p2\"></td>");
			t.Append("\r\n\t\t<td class=\"p2\"></td>");
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++)
			{
				t.Append("\r\n\t\t<td class=\"p2\"></td>");
			}
			//fin MAJ
		
			// On affiche les mois
			//t.Append("\r\n\t\t<td colspan="+nbPeriod+" class=\"pmperiods\">");
			//t.Append("\r\n\t\t\t<table cellpadding=0 cellspacing=0 bgcolor=#644883 border=0>\r\n\t\t\t\t<tr>");
			prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				if(prevYear!=int.Parse(tab[0,j].ToString().Substring(0,4))){
					HTML2+="<td width=\""+nbPeriodInYear*20+"px\" colspan="+nbPeriodInYear+" class=\"pmannee\">"+prevYear+"</td>";
					nbPeriodInYear=0;
					prevYear=int.Parse(tab[0,j].ToString().Substring(0,4));

				}
				if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly){
					HTML+="<td style=\"width:15pt\" class=\"p10\">"+TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(tab[0,j].ToString().Substring(4,2)),cultureInfo,1)+"</td>";
				}
				else{
					HTML+="<td style=\"width:15pt\" class=\"p10\">"+tab[0,j].ToString().Substring(4,2)+"</td>";
				}
				nbPeriodInYear++;
			}
			HTML2+="<td width=\""+nbPeriodInYear*20+"px\" colspan="+nbPeriodInYear+" class=\"pmannee2\">"+prevYear+"</td>";
			t.Append(HTML2+"</tr><tr>");
			//t.Append("<td><table cellpadding=0 cellspacing=0 bgcolor=#644883 border=0><tr>"+HTML2+"</tr><tr>"+HTML+"</tr></table></td>");
			t.Append("\r\n\t\t<td width=\"230px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
			t.Append("\r\n\t\t<td class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
			t.Append("\r\n\t\t<td class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++)
			{
				t.Append("\r\n\t\t<td class=\"p2\">"+tab[0,k].ToString()+"</td>");
			}
			//fin MAJ

			t.Append(HTML);
			//t.Append("\r\n\t\t\t\t</tr>\r\n\t\t\t</table>");
			//t.Append("\r\n\t\t</td>");
			t.Append("\r\n\t</tr>");
			//t.Append("\r\n\t<tr>\r\n\t\t<td bgclor=#FFFFFF colspan=nbColTab><img src=\"images/pixel.gif\" width=1 height=1>\r\n\t</tr>");
			t.Append("\r\n\t<tr>");
			i=0;
			try{
				for(i=1;i<tab.GetLength(0);i++){
					for(j=0;j<tab.GetLength(1);j++){
						switch(j){
								// Total ou vehicle
							case MediaPlan.VEHICLE_COLUMN_INDEX:
								if(tab[i,j]!=null){
									if(i==MediaPlan.TOTAL_LINE_INDEX) classe="pmtotal";
									else classe="pmvehicle";
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									//formattage des unités
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4].ToString(),webSession.Unit);
//									t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,j]+"</td><td class=\""+classe+"\">"+tab[i,j+4]+"</td><td class=\""+classe+"\">"+((double)tab[i,j+5]).ToString("0.00")+"</td>");
									t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,j]+"</td><td class=\""+classe+"\">"+totalUnit+"</td><td class=\""+classe+"\">"+((double)tab[i,j+5]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++)
									{
										//t.Append("<td class=\""+classe+"nb\">"+((double)tab[i,j+5+k]).ToString("0.##")+"</td>");
										t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+5+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+5+nbColYear;
									//fin MAJ								
								}
								break;
								// Category
							case MediaPlan.CATEGORY_COLUMN_INDEX:
								if(tab[i,j]!=null){
									//formattage des unités
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3].ToString(),webSession.Unit);
//									t.Append("\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,j]+"</td><td class=\"pmcategory\">"+tab[i,j+3]+"</td><td class=\"pmcategory\">"+((double)tab[i,j+4]).ToString("0.00")+"</td>");
									t.Append("\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,j]+"</td><td class=\"pmcategory\">"+totalUnit+"</td><td class=\"pmcategory\">"+((double)tab[i,j+4]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++)
									{
										//t.Append("<td class=\"pmcategorynb\">"+((double)tab[i,j+4+k]).ToString("0.##")+"</td>");
										t.Append("<td class=\"pmcategorynb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4+k].ToString(),webSession.Unit)+"</td>");
									}
									j=j+4+nbColYear;
									//fin MAJ
								}
								break;
								// Media
							case MediaPlan.MEDIA_COLUMN_INDEX:
								// formattage des unités
								totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+2].ToString(),webSession.Unit);
								if(premier){
//									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+tab[i,j+2]+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");																		
									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++)
									{
//										t.Append("<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,j+3+k]).ToString("0.##")+"</td>");
										t.Append("<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit)+"</td>");										
									}
									//fin MAJ
									premier=false;
								}
								else{
//									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+tab[i,j+2]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++)
									{
//										t.Append("<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,j+3+k]).ToString("0.##")+"</td>");
										t.Append("<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit)+"</td>");
									}
									//fin MAJ
									premier=true;
								}	
								j=j+3+nbColYear;
								break;
							default:
								
								if(tab[i,j]==null){
									t.Append("<td class=\"pmperioditemxls1\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlan.graphicItemType)){
									switch((FrameWorkResultConstantes.MediaPlan.graphicItemType)tab[i,j]){
										case FrameWorkResultConstantes.MediaPlan.graphicItemType.present:
											t.Append("<td class=\"pmperioditemxls\">&nbsp;</td>");
											break;
										case FrameWorkResultConstantes.MediaPlan.graphicItemType.extended:
											t.Append("<td class=\"pmperioditemxls2\">&nbsp;</td>");
											break;
										default:
											t.Append("<td class=\"pmperioditemxls1\">&nbsp;</td>");
											break;
									}
								}
								break;
						}
					}
					t.Append("</tr><tr>");
				}
			}
			catch(System.Exception e){
				throw(new System.Exception("erreur i="+i.ToString()+",j="+j.ToString(),e));
			}
			// Fin du tableau
			t.Append("</tr></table></td></tr></table>");


			// On vide le tableau
			tab=null;

			t.Append(ExcelFunction.GetFooter(webSession));
			return(t.ToString());

		}
		#endregion
	}
}
