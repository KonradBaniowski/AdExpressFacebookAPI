#region Informations
// Auteur: D. V. Mussuma
// Date de création: 29/08/2005
// Date de modification:
#endregion

using System.Web;
using System;
using System.Web.UI;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Date;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.AdExpress.Web.Functions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.UI.Results.APPM
{
//	/// <summary>
//	/// Interface Zoom Media Plan APPM
//	/// </summary>
//	public class ZoomMediaPlanUI
//	{
//		#region Sortie HTML (Web)
//		/// <summary>
//		/// Génère le code HTML pour afficher un calendrier d'action détaillé en jour sur UNE période.
//		/// Elles se base sur un tableau contenant les données
//		/// tab vide : message "Pas de données" + bouton retour
//		/// tab non vide:
//		///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
//		///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
//		///		Génération du code HTML des entêtes de colonne
//		///		Génération du code HTML des périodes (période suivante, période courante, période suivante, intitulés d'une unité de période
//		///		Génération du code HTML du calendrier d'action
//		/// </summary>
//		/// <param name="page">Page qui affiche le Plan Média</param>
//		/// <param name="tab">Tableau contenant les données à mettre en forme</param>
//		/// <param name="webSession">Session du client</param>
//		/// <param name="dataSource">source de données</param>		
//		/// <param name="zoomDate">Période à prendre en compte (un mois ou une semaine)</param>
//		/// <param name="url">Lien vers la pge elle-même. Permet de gérer les flèches "Période suivante", "Période précédente"</param>
//		/// <returns>Code Généré</returns>
//		/// <remarks>
//		/// Utilise les méthodes:
//		///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
//		///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
//		/// </remarks>
//		public static string GetMediaPlanAlertUI(Page page,object[,] tab,WebSession webSession,IDataSource dataSource, string zoomDate, string url){
//			
//			#region Pas de données à afficher
//			if(tab.GetLength(0)==0){
//				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
//					+"<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/"+webSession.SiteLanguage+"/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/"+webSession.SiteLanguage+"/button/back_up.gif';\">"
//					+"<img src=\"/Images/"+webSession.SiteLanguage+"/button/back_up.gif\" border=0 name=bouton></a></div>");
//			}
//			#endregion
//
//			#region Variables
//
//			//MAJ GR : Colonnes totaux par année si nécessaire
//			int k = 0;
//			int nbColYear = 0;
////			//A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
////			if (nbColYear>0) nbColYear++;
////			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear;
////			//fin MAJ
//
//			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_INDEX;
//			
//			string classe;
//			//A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
//			//que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
//			//affichée, utilisez getLength-1
//			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//			int nbColTab=tab.GetLength(1),j,i;
//			int nbline=tab.GetLength(0);
//			//MAJ GR : FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
//			int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
//			//fin MAJ
//			bool premier=true;
//			string currentCategoryName="tit";
//			//string classCss="";
//			const string PLAN_MEDIA_1_CLASSE="p6";
//			const string PLAN_MEDIA_2_CLASSE="p7";
//			const string PLAN_MEDIA_NB_1_CLASSE="p8";
//			const string PLAN_MEDIA_NB_2_CLASSE="p9";
//			string totalUnit="";
//
//			#endregion
//
//			#region On calcule la taille de la colonne Total
////			int nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit).Length;
////			int nbSpace=(nbtot-1)/3;
////			int nbCharTotal=nbtot+nbSpace-5;
////			if(nbCharTotal<5)nbCharTotal=0;
//			//nouvelle version
//			int nbtot=1;
//			int nbSpace=(nbtot-1)/3;
//			int nbCharTotal=nbtot+nbSpace-5;
//			if(nbCharTotal<5)nbCharTotal=0;
//			#endregion
//			
//			#region script
//			if (!page.ClientScript.IsClientScriptBlockRegistered("openCreation"))page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openCreation",WebFunctions.Script.OpenCreation());
//			// On enregistre le script DynamicMediaPlan qui rend les calendriers d'action dynamique
//			if (!page.ClientScript.IsClientScriptBlockRegistered("DynamicMediaPlan"))page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DynamicMediaPlan",TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan());
//			#endregion
//
//			#region debut Tableau
//			t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
//			t.Append("\r\n\t\t<td rowspan=3 width=\"250px\" class=\"p2\">");
//			t.Append("\n\t\t\t<table width=\"250px\" border=0 cellpadding=0 cellspacing=0 bgcolor=#644883>\r\n\t\t\t\t<tr>");
//			t.Append("\n\t\t\t\t\t<td class=\"p1\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
//			t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:showAllContent3();\"><img id=pictofermer border=0 src=\"/Images/Common/button/picto_fermer.gif\"></a></td>");
//			t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:hideAllContent3();\"><img id=pictoouvrir border=0 src=\"/Images/Common/button/picto_ouvrir.gif\"></a></td>");
//			t.Append("\n\t\t\t\t\t<td  bgcolor=#ffffff class=\"p2\"  width=1px></td>");
//			t.Append("\n\t\t\t\t</tr>\n\t\t\t</table>");
//			t.Append("\r\n\t\t</td>");
//			//t.Append("\r\n\t\t<td rowspan=\"2\" width=\"200px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
//			t.Append("\r\n\t\t<td rowspan=3 class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
//			for(int h=0;h<nbCharTotal;h++){
//				t.Append("&nbsp;");
//			}
//			t.Append("</td>");
//			
//			t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
//			t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
//
//			// MAJ GR : On affiche les années si nécessaire
////			for(k=FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++){
////				t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+tab[0,k].ToString()+"</td>");
////				for(int h=0;h<nbCharTotal+5;h++){
////					t.Append("&nbsp;");
////				}
////				t.Append("</td>");
////			}
//			//fin MAJ
//
//			#endregion
//
//			#region Période précédente
//			if (webSession.PeriodBeginningDate != zoomDate) {
//				if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
//					t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
//						(new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(-1).ToString("yyyyMM")
//						+"\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
//				}
//				else{
//					t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
//					AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
//					tmp.SubWeek(1);
//					if(tmp.Week.ToString().Length<2)t.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
//					else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
//					t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
//				}
//			}
//			else
//				t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\">&nbsp;</td>");
//			#endregion
//
//			#region Entête de période
//			t.Append("\r\n\t\t\t\t\t\t<td colspan=\""+(nbColTab-FIRST_PERIOD_INDEX-2)+"\" class=\"pmannee2\" align=center>"
//				+Dates.getPeriodTxt(webSession, zoomDate)
//				+"</td>");
//			#endregion
//
//			#region Période suivante
//			if (webSession.PeriodEndDate != zoomDate){
//				if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
//					t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
//						(new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(1).ToString("yyyyMM")
//						+"\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
//				}
//				else{
//					t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
//					AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
//					tmp.Increment();
//					if(tmp.Week.ToString().Length<2)t.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
//					else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
//					t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
//				}
//			}
//			else
//				t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\">&nbsp;</td>");
//			t.Append("</tr><tr>");
//			#endregion
//
//			#region Périodes
//			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
//
//				t.Append("<td class=\"p10\">&nbsp;"+((DateTime)tab[0,j]).ToString("dd")+"&nbsp;</td>");
//			}
//
//			string dayClass="";
//			char day;
//			t.Append("\r\n\t</tr><tr>");
//			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
//				day=TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,((DateTime)tab[0,j]).DayOfWeek.ToString()).ToCharArray()[0];
//							
//				
//				if(day==GestionWeb.GetWebWord(545,webSession.SiteLanguage).ToCharArray()[0]
//					|| day==GestionWeb.GetWebWord(546,webSession.SiteLanguage).ToCharArray()[0]
//					){
//					dayClass="p132";
//				}
//				else{
//					dayClass="p131";
//				}	
//				t.Append("<td class=\""+dayClass+"\">"+TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,((DateTime)tab[0,j]).DayOfWeek.ToString())+"</td>");
//			}
//			t.Append("\r\n\t</tr>");
//			#endregion
//
//			#region Calendrier d'action
//			//t.Append("\r\n\t<tr>");
//			i=0;
//			try{
//				for(i=1;i<nbline;i++){
//					for(j=0;j<nbColTab;j++){
//						switch(j){
//								// Total ou vehicle
////							case MediaPlanAPPM.VEHICLE_COLUMN_INDEX:
////								if(tab[i,j]!=null){
////									string  tmpHtml="";
////									if(i==MediaPlanAlert.TOTAL_LINE_INDEX) classe="pmtotal";
////									else{
////										classe="pmvehicle";
////										tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
////									}
////									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
////										i=int.MaxValue-2;
////										j=int.MaxValue-2;
////										break;
////									}
////									// On traite le cas des pages
////									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
////									t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
////									//aller aux colonnes du calendrier d'action
////									//MAJ GR : totaux par années si nécessaire
////									for(k=1; k<=nbColYear; k++){
////										t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+8+k].ToString(),webSession.Unit)+"</td>");
////									}
////									j=j+8+nbColYear;
////									//fin MAJ
////								}
////								break;
////								// Category
//							case MediaPlanAPPM.CATEGORY_COLUMN_INDEX:
//								if(tab[i,j]!=null){
//									string tmpHtml="";
//									if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]+",-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//									// On traite le cas des pages
//									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//									t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX]+"');\" style=\"DISPLAY:inline; CURSOR: hand\">\r\n\t\t<td class=\"pmcategory\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategorynb\">"+totalUnit+"</td><td class=\"pmcategorynb\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\"pmcategory\">"+tmpHtml+"</td>");
//									currentCategoryName=tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX].ToString();
//									//aller aux colonnes du calendrier d'action
//									//MAJ GR : totaux par années si nécessaire
//									for(k=1; k<=nbColYear; k++){
//										t.Append("<td class=\"pmcategorynb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+7+k].ToString(),webSession.Unit)+"</td>");
//									}
//									j=j+7+nbColYear;
//									//fin MAJ
//								}
//								break;
//								// Media
//							case MediaPlanAPPM.MEDIA_COLUMN_INDEX:
//								// On traite le cas des pages
//								totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//								if(premier){
//									//On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la catégorie "chaîne thématique"
//									if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null)
//										t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_1_CLASSE+"\"><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
//									else
//										t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_1_CLASSE+"\"></td>");
//									//MAJ GR : totaux par années si nécessaire
//									for(k=1; k<=nbColYear; k++){
//										t.Append("<td class=\""+PLAN_MEDIA_1_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+6+k].ToString(),webSession.Unit)+"</td>");
//									}
//									//fin MAJ
//									premier=false;
//								}
//								else{
//									//On verifie si la chaine IdMedia, IdVehicle est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
//									if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null)
//										t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\" onclick=\"\" ><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
//									else
//										t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"></td>");
//									//MAJ GR : totaux par années si nécessaire
//									for(k=1; k<=nbColYear; k++){
//										t.Append("<td class=\""+PLAN_MEDIA_2_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+6+k].ToString(),webSession.Unit)+"</td>");
//									}
//									//fin MAJ
//									premier=true;
//								}	
//								//aller aux colonnes du calendrier d'action
//								j=j+6;
//								break;
//							default:
//								if(tab[i,j]==null){
//									t.Append("<td class=\"p3\">&nbsp;</td>");
//									break;
//								}
//								if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlanAPPM.graphicItemType)){
//									switch((FrameWorkResultConstantes.MediaPlanAPPM.graphicItemType)tab[i,j]){
//										case FrameWorkResultConstantes.MediaPlanAPPM.graphicItemType.present:
//											t.Append("<td class=\"p4\">&nbsp;</td>");
//											break;
//										case FrameWorkResultConstantes.MediaPlanAPPM.graphicItemType.extended:
//											t.Append("<td class=\"p5\">&nbsp;</td>");
//											break;
//										default:
//											t.Append("<td class=\"p3\">&nbsp;</td>");
//											break;
//									}
//								}
//								break;
//						}
//					}
//					t.Append("</tr>");
//				}
//			}
//			catch(System.Exception e){
//				throw(new System.Exception("erreur i="+i+",j="+j,e));
//			}
//			//t.Append("</tr></table></td></tr></table>");
//			
//
//			//	t.Append("</table></td></tr></table>");
//			
//			t.Append("</table>");
//			
//			#endregion
//
//			// On vide le tableau
//			tab=null;
//
//			return(t.ToString());
//
//		}
//
//
//
//		#endregion
//	}
}
