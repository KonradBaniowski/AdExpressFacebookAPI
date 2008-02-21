#region Informations
// Auteur : 
// Créé le : 
// Modifié le : 
//		10/05/2005		K.Shehzad	Changement d'en tête Excel
//		12/08/2005		G. Facon	Nom de fonctions et exceptions
#endregion


using System;
using System.Web;

using System.Web.UI;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.FrameWork;
using TNS.FrameWork.Date;

using WebConstantes = TNS.AdExpress.Constantes.Web;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
using WebExceptions=TNS.AdExpress.Web.Exceptions;

using TNS.AdExpress.Web.Functions;
using DBConstantesClassification=TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Construit l'interface utilisateur pour les alertes plan media ou les zooms plan media
	/// </summary>
	public class CompetitorMediaPlanAlertUI{

		#region Sortie HTML (Web)
		/// <summary>
		/// Génère le code HTML pour afficher un calendrier d'action détaillé en jour sur UNE période.
		/// Elles se base sur un tableau contenant les données
		/// tab vide : message "Pas de données" + bouton retour
		/// tab non vide:
		///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
		///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
		///		Génération du code HTML des entêtes de colonne
		///		Génération du code HTML des périodes (période suivante, période courante, période suivante, intitulés d'une unité de période
		///		Génération du code HTML du calendrier d'action
		/// </summary>
		/// <param name="page">Page qui affiche le Plan Média</param>
		/// <param name="tab">Tableau contenant les données à mettre en forme</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="zoomDate">Période à prendre en compte (un mois ou une semaine)</param>
		/// <param name="url">Lien vers la pge elle-même. Permet de gérer les flèches "Période suivante", "Période précédente"</param>
		/// <returns>Code Généré</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
		///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
		/// </remarks>
		public static string GetMediaPlanAlertUI(Page page,object[,] tab,WebSession webSession, string zoomDate, string url){
			
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
			
			#region Pas de données à afficher
			if(tab.GetLength(0)==0){
				t.Append("<table align=\"center\"><tr>");

				#region Période précédente
				if (webSession.PeriodBeginningDate != zoomDate) {
					if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly) {
						t.Append("\r\n\t\t\t\t\t\t<td><a id=\"periodPrevImageButton\" onmouseover=\"periodPrevImageButton_img.src='/Images/Common/period_prev_down.gif'\" "
							+"onmouseout=\"periodPrevImageButton_img.src='/Images/Common/period_prev_up.gif'\" "
							+"href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
							(new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(-1).ToString("yyyyMM")
							+"\"><IMG border=0 alt=\"\" name=\"periodPrevImageButton_img\" src=\"/Images/Common/period_prev_up.gif\"></a></td><td>&nbsp;</td>");
					}
					else {
						t.Append("\r\n\t\t\t\t\t\t<td><a  id=\"periodPrevImageButton\" onmouseover=\"periodPrevImageButton_img.src='/Images/Common/period_prev_down.gif'\" "
							+"onmouseout=\"periodPrevImageButton_img.src='/Images/Common/period_prev_up.gif'\" "
							+"href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
						AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
						tmp.SubWeek(1);
						if(tmp.Week.ToString().Length<2)t.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
						else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
						t.Append("\"><IMG border=0 alt=\"\" name=\"periodPrevImageButton_img\" src=\"/Images/Common/period_prev_up.gif\"></a></td><td>&nbsp;</td>");
					}
				}
				else
					t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td>");
				#endregion
				
				t.Append("<td class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</td>");
				
				#region Période suivante
				if (webSession.PeriodEndDate != zoomDate) {
					if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly) {
						t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td><td><a id=\"periodNextImageButton\" onmouseover=\"periodNextImageButton_img.src='/Images/Common/period_next_down.gif'\" "
							+"onmouseout=\"periodNextImageButton_img.src='/Images/Common/period_next_up.gif'\" "
							+"href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
							(new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(1).ToString("yyyyMM")
							+"\"><IMG border=0 alt=\"\" name=\"periodNextImageButton_img\" src=\"/Images/Common/period_next_up.gif\"></a></td>");
					}
					else {
						t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td><td><a  id=\"periodNextImageButton\" onmouseover=\"periodNextImageButton_img.src='/Images/Common/period_next_down.gif'\" "
							+"onmouseout=\"periodNextImageButton_img.src='/Images/Common/period_next_up.gif'\" "
							+"href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
						AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
						tmp.Increment();
						if(tmp.Week.ToString().Length<2)t.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
						else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
						t.Append("\"><IMG border=0 alt=\"\" name=\"periodNextImageButton_img\" src=\"/Images/Common/period_next_up.gif\"></a></td>");
					}
				}
				else
					t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td>");
				#endregion

				t.Append("</tr></table>");
				return(t.ToString());
			}
			#endregion

			#region Variables
			string classe;
			//A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
			//que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
			//affichée, utilisez getLength-1
			int nbColTab=tab.GetLength(1),j,i;
			int nbline=tab.GetLength(0);
			int nbPeriod=nbColTab-FrameWorkResultConstantes.CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX-1;
			//bool premier=true;
			bool premierTotal=true;
			//bool premierAdvertiser=true;
			bool premierVehicle=true;
			bool premierCategory=true;
			string currentCategoryName="tit";
			const string PLAN_MEDIA_1_CLASSE="p16";
			const string PLAN_MEDIA_2_CLASSE="p6";
			//const string PLAN_MEDIA_NB_1_CLASSE="p8";
			const string PLAN_MEDIA_NB_2_CLASSE="p8";
			
			//const string PLAN_MEDIA_1_ADVERTISER_CLASSE="p12";
			const string PLAN_MEDIA_1_ADVERTISER_CLASSE="asl5b";

			const string PLAN_MEDIA_NB_1_ADVERTISER_CLASSE="asl5nbb";
			string totalUnit="";
			#endregion

            #region Module
            Int64 moduleId;
            WebConstantes.Module.Type moduleType=(TNS.AdExpress.Web.Core.Navigation.ModulesList.GetModule(webSession.CurrentModule)).ModuleType;
            if(moduleType==WebConstantes.Module.Type.alert|| zoomDate.Length>0) moduleId=WebConstantes.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE;
            else moduleId=WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE;
            #endregion

			#region On calcule la taille de la colonne Total
			int nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit).Length;
			int nbSpace=(nbtot-1)/3;
			int nbCharTotal=nbtot+nbSpace-5;
			if(nbCharTotal<5)nbCharTotal=0;
			#endregion
			
			#region script
            if(!page.ClientScript.IsClientScriptBlockRegistered("OpenCreatives")) page.ClientScript.RegisterClientScriptBlock(page.GetType(),"OpenCreatives",WebFunctions.Script.OpenCreatives());
            if(!page.ClientScript.IsClientScriptBlockRegistered("OpenInsertions")) page.ClientScript.RegisterClientScriptBlock(page.GetType(),"OpenInsertions",WebFunctions.Script.OpenInsertions());
			// On enregistre le script DynamicMediaPlan qui rend les calendriers d'action dynamique
            if (!page.ClientScript.IsClientScriptBlockRegistered("DynamicMediaPlan")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "DynamicMediaPlan", TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan());
			#endregion

			#region Sélection du vehicle
			string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
			#endregion

			#region debut Tableau
			t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
			t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"p2\">");
			t.Append("\n\t\t\t<table width=\"250px\" border=0 cellpadding=0 cellspacing=0 bgcolor=#644883>\r\n\t\t\t\t<tr>");
			t.Append("\n\t\t\t\t\t<td class=\"p1\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
			t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:showAllContent3();\"><img id=pictofermer border=0 src=\"/Images/Common/button/picto_fermer.gif\"></a></td>");
			t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:hideAllContent3();\"><img id=pictoouvrir border=0 src=\"/Images/Common/button/picto_ouvrir.gif\"></a></td>");
			t.Append("\n\t\t\t\t\t<td bgcolor=#ffffff width=1px class=\"p2\"></td>");
			t.Append("\n\t\t\t\t</tr>\n\t\t\t</table>");
			t.Append("\r\n\t\t</td>");
			//t.Append("\r\n\t\t<td rowspan=\"2\" width=\"200px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
			t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
			for(int h=0;h<nbCharTotal;h++){
				t.Append("&nbsp;");
			}
			t.Append("</td>");
			
			t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
            bool showVersions = false;
            if(webSession.CustomerLogin.FlagsList[CstDB.Flags.ID_SLOGAN_ACCESS_FLAG] != null) {
                showVersions = true;
                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + GestionWeb.GetWebWord(1994, webSession.SiteLanguage) + "</td>");
            }
            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + GestionWeb.GetWebWord(2245, webSession.SiteLanguage) + "</td>");
			#endregion

			#region Période précédente
			if (webSession.PeriodBeginningDate != zoomDate){
				if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
					t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
						(new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(-1).ToString("yyyyMM")
						+"\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
				}
				else{
					t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
					AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
					tmp.SubWeek(1);
					if(tmp.Week.ToString().Length<2)t.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
					else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
					t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
				}
			}
			else
				t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\">&nbsp;</td>");
			#endregion

			#region Entête de période
			t.Append("\r\n\t\t\t\t\t\t<td colspan=\""+(nbColTab-CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX-2)+"\" class=\"pmannee2\" align=center>"
				+Dates.getPeriodTxt(webSession, zoomDate)
				+"</td>");
			#endregion

			#region Période suivante
			if (webSession.PeriodEndDate != zoomDate){
				if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
					t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
						(new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(1).ToString("yyyyMM")
						+"\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
				}
				else{
					t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
					AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
					tmp.Increment();
					if(tmp.Week.ToString().Length<2)t.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
					else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
					t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
				}
			}
			else
				t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\">&nbsp;</td>");
			t.Append("</tr><tr>");
			#endregion

			#region Périodes
			for(j=CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX;j<nbColTab;j++){
				t.Append("<td class=\"p10\">&nbsp;"+((DateTime)tab[0,j]).ToString("dd")+"&nbsp;</td>");
			}
			string dayClass="";
			char day;
			t.Append("\r\n\t</tr><tr>");
			for(j=CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX;j<nbColTab;j++){
				day=TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,((DateTime)tab[0,j]).DayOfWeek.ToString()).ToCharArray()[0];
							
				
				if(day==GestionWeb.GetWebWord(545,webSession.SiteLanguage).ToCharArray()[0]
					|| day==GestionWeb.GetWebWord(546,webSession.SiteLanguage).ToCharArray()[0]
					){
					dayClass="p132";
				}
				else{
					dayClass="p131";
				}	
				t.Append("<td class=\""+dayClass+"\">"+TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,((DateTime)tab[0,j]).DayOfWeek.ToString())+"</td>");
			}
			t.Append("\r\n\t</tr>");


			#endregion

			#region Calendrier d'action
			//t.Append("\r\n\t<tr>");
			i=0;
			bool firsTotal=true;
			string space="";

			try{
				for(i=1;i<nbline;i++){
					for(j=0;j<nbColTab;j++){
						switch(j){
								// Total ou vehicle
							case CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX:
								if(tab[i,j]!=null){
									string tmpHtml="";
                                    string tmpHtml2 = "";
									//if((tab[i,j].ToString()!=GestionWeb.GetWebWord(204,webSession.SiteLanguage) && tab[i,j].ToString()!=GestionWeb.GetWebWord(205,webSession.SiteLanguage) && tab[i,j].ToString()!=GestionWeb.GetWebWord(206,webSession.SiteLanguage)) && tab[i,j].ToString()!=GestionWeb.GetWebWord(1779,webSession.SiteLanguage) && premierTotal){
									if (IsBelongToTotalLines(webSession,tab[i,j].ToString(),premierTotal)) {
										classe="pmtotal";
										if(firsTotal){
											space="";
										}
										else{
											space="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
										}
										firsTotal=false;
									}
									else{
										classe="pmvehicle";
                                        if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] != null) {
                                            tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + ",-1,-1,-1','" + zoomDate + "','-1','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            tmpHtml2 ="<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + ",-1,-1,-1,-1','" + zoomDate + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        }
										premierTotal=false;
									}
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									if(premierTotal){
										if(totalUnit.Trim()==""){
											tmpHtml="";
                                            tmpHtml2="";
											totalUnit="0";
										}
                                        t.Append("\r\n\t<tr id=\"" + tab[i, j] + "Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\"" + classe + "\">" + space + "" + tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>"+((showVersions)?"<td align=\"center\" class=\"" + classe + "\">" + tmpHtml + "</td>":"")+"<td align=\"center\" class=\"" + classe + "\">" + tmpHtml2 + "</td>");
									}
									else{
										if(premierVehicle){
											if(totalUnit.Trim()==""){
												tmpHtml="";
                                                tmpHtml2="";
												totalUnit="0";
											}
                                            t.Append("\r\n\t<tr id=\"" + tab[i, j] + "Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\"" + classe + "\">&nbsp;" + tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>"+((showVersions)?"<td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>":"")+"<td align=\"center\" class=\""+classe+"\">"+tmpHtml2+"</td>");
											premierVehicle=false;
										}
										else{
											classe=PLAN_MEDIA_1_CLASSE;
                                            if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] != null) {
                                                tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + ",-1,-1,-1','" + zoomDate + "','" + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX] + "','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                                tmpHtml2= "<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + ",-1,-1,-1," + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX] + "','" + zoomDate + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            }
                                            else {
                                                tmpHtml = "";
                                                tmpHtml2 = "";
                                            }
											if(totalUnit.Trim()==""){
												tmpHtml="";
                                                tmpHtml2 = "";
												totalUnit="0";
											}
                                            t.Append("\r\n\t<tr id=\"" + tab[i, j] + "Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\"" + classe + "\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>"+((showVersions)?"<td align=\"center\" class=\"" + classe + "\">" + tmpHtml + "</td>":"")+"<td align=\"center\" class=\"" + classe + "\">" + tmpHtml2 + "</td>");
										}
									}
									
									//aller aux colonnes du calendrier d'action
									j=j+10;
								}
								break;
								// Category
							case CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX:
								if(tab[i,j]!=null){
									premierVehicle=true;
									string tmpHtml="";
                                    string tmpHtml2 = "";
                                    if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]!=null) {
                                        tmpHtml="<a href=\"javascript:OpenCreatives('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+",-1,-1','"+zoomDate+"','-1','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        tmpHtml2="<a href=\"javascript:OpenInsertions('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+",-1,-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                    }
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);

									if(premierCategory){
										if(totalUnit.Trim()==""){
											tmpHtml="";
                                            tmpHtml2="";
											totalUnit="0";
										}
                                        t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"');\" style=\"DISPLAY:inline; CURSOR: hand\">\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategorynb\">"+totalUnit+"</td><td class=\"pmcategorynb\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\"pmcategory\">"+tmpHtml+"</td>":"")+"<td align=\"center\" class=\"pmcategory\">"+tmpHtml2+"</td>");
										premierCategory=false;
										currentCategoryName=tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX].ToString();
									}
									else{
                                        if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] != null) {
                                            tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + "," + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] + ",-1,-1','" + zoomDate + "','" + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX] + "','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            tmpHtml2 ="<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + "," + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX] + ",-1,-1," + tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX] + "','" + zoomDate + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        }
										
										if(totalUnit.Trim()==""){
											tmpHtml="";
                                            tmpHtml2="";
											totalUnit="0";
										}
										if(tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]!=null){
                                            t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"');\" style=\"DISPLAY:inline; \">\r\n\t\t<td class=\"pmcategoryb\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategorynbb\">"+totalUnit+"</td><td class=\"pmcategorynbb\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\"pmcategoryb\">"+tmpHtml+"</td>":"")+"<td align=\"center\" class=\"pmcategoryb\">"+tmpHtml2+"</td>");
										}
                                        else{
                                            t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"');\" style=\"DISPLAY:inline; \">\r\n\t\t<td class=\"pmcategoryb\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategorynbb\">"+totalUnit+"</td><td class=\"pmcategorynbb\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\"pmcategoryb\"></td>":"")+"<td align=\"center\" class=\"pmcategoryb\"></td>");
										}
									}
									//currentCategoryName=tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX].ToString();
									//aller aux colonnes du calendrier d'action
									j=j+9;
								}
								break;
								// Media
							case CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX:
								// On traite le cas des pages
								if(tab[i,j]!=null){									
									premierCategory=true;
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									//	if(premier){
									//On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la catégorie "chaîne thématique"
									if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null){
                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"><a href=\"javascript:OpenCreatives('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1','"+zoomDate+"','-1','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>":"")+"<td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"><a href=\"javascript:OpenInsertions('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
									}
									else
										t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"></td>":"")+"<td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"></td>");
									
									//aller aux colonnes du calendrier d'action
									j=j+8;
								}
								break;
							case CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX:
								break;

								// Advertiser
							case CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX:
								totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
								if(tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]!=null){
                                    t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\"><a href=\"javascript:OpenCreatives('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1','"+zoomDate+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]+"','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>":"")+"<td align=\"center\" class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\"><a href=\"javascript:OpenInsertions('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1,"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]+"','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
								}
                                else{
                                    t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\"></td>":"")+"<td align=\"center\" class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\"></td>");
								}
								j=j+6;
								break;
		

							default:
								if(tab[i,j]==null){
									t.Append("<td class=\"p3\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType)){
									switch((FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType)tab[i,j]){
										case FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType.present:
											t.Append("<td class=\"p4\">&nbsp;</td>");
											break;
										case FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType.extended:
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
					t.Append("</tr>");
				}
			}
			catch(System.Exception e){
				throw(new System.Exception("erreur i="+i+",j="+j,e));
			}
			//t.Append("</tr></table></td></tr></table>");
			//t.Append("</table></td></tr></table>");
			t.Append("</table>");
			#endregion

			// On vide le tableau
			tab=null;

			return(t.ToString());

		}


		/// <summary>
		/// Génère le code HTML pour afficher un calendrier d'action détaillé en jour entre les dates de la session.
		/// Elles se base sur un tableau contenant les données
		/// tab vide : message "Pas de données" + bouton retour
		/// tab non vide:
		///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
		///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
		///		Génération du code HTML des entêtes de colonne
		///		Génération du code HTML des périodes (Libellé du mois, intitulés d'une unité de période)
		///		Génération du code HTML du calendrier d'action
		/// </summary>
		/// <param name="page">Page qui affiche le Plan Média</param>
		/// <param name="tab">Tableau contenant les données à mettre en forme</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code Généré</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
		///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
		/// </remarks>
		public static string GetMediaPlanAlertUI(Page page,object[,] tab,WebSession webSession){
		
			#region Pas de données à afficher
			if(tab.GetLength(0)==0){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/"+webSession.SiteLanguage+"/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/"+webSession.SiteLanguage+"/button/back_up.gif';\">"
					+"<img src=\"/Images/"+webSession.SiteLanguage+"/button/back_up.gif\" border=0 name=bouton></a></div>");
			}
			#endregion

			#region Variables

			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear;
			//fin MAJ

			string classe;
			//A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
			//que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
			//affichée, utilisez getLength-1
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
			int nbColTab=tab.GetLength(1),j,i;
			int nbline=tab.GetLength(0);
			int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
			//	bool premier=true;
			bool premierTotal=true;
			bool premierVehicle=true;
			bool premierCategory=true;
			//Int64 currentVehicle=-1;
			//	bool premierAdvertiser=true;

			string currentCategoryName="tit";
			string totalUnit="";
			const string PLAN_MEDIA_1_CLASSE="p16";
			const string PLAN_MEDIA_2_CLASSE="p6";
			//const string PLAN_MEDIA_2_CLASSE="p7b";
			//const string PLAN_MEDIA_NB_1_CLASSE="p17";
			const string PLAN_MEDIA_NB_2_CLASSE="p8";
			//const string PLAN_MEDIA_NB_2_CLASSE="p9b";
			//const string PLAN_MEDIA_1_ADVERTISER_CLASSE="p12";
			//const string PLAN_MEDIA_NB_1_ADVERTISER_CLASSE="p11";
			//const string PLAN_MEDIA_1_ADVERTISER_CLASSE="p12b";
			
			const string PLAN_MEDIA_1_ADVERTISER_CLASSE="asl5b";

			const string PLAN_MEDIA_NB_1_ADVERTISER_CLASSE="asl5nbb";
		
			#endregion

            #region Module
            Int64 moduleId;
            WebConstantes.Module.Type moduleType=(TNS.AdExpress.Web.Core.Navigation.ModulesList.GetModule(webSession.CurrentModule)).ModuleType;
            if(moduleType==WebConstantes.Module.Type.alert) moduleId=WebConstantes.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE;
            else moduleId=WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE;
            #endregion

			#region Sélection du vehicle
			string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
//			DBConstantesClassification.Vehicles.names vehicleName=(DBConstantesClassification.Vehicles.names)int.Parse(vehicleSelection);
//			if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.PortofolioUIException("La sélection de médias est incorrecte"));
			#endregion

			#region On calcule la taille de la colonne Total
			int nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit).Length;
			int nbSpace=(nbtot-1)/3;
			int nbCharTotal=nbtot+nbSpace-5;
			if(nbCharTotal<5)nbCharTotal=0;
			#endregion
			
			#region script
            if(!page.ClientScript.IsClientScriptBlockRegistered("OpenCreatives")) page.ClientScript.RegisterClientScriptBlock(page.GetType(),"OpenCreatives",WebFunctions.Script.OpenCreatives());
            if(!page.ClientScript.IsClientScriptBlockRegistered("OpenInsertions")) page.ClientScript.RegisterClientScriptBlock(page.GetType(),"OpenInsertions",WebFunctions.Script.OpenInsertions());
			// On enregistre le script DynamicMediaPlan qui rend les calendriers d'action dynamique
            if (!page.ClientScript.IsClientScriptBlockRegistered("DynamicMediaPlan")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "DynamicMediaPlan", TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan());
			#endregion

			#region Début du tableau
			t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
			t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"p2\">");
			t.Append("\n\t\t\t<table width=\"250px\" border=0 cellpadding=0 cellspacing=0 bgcolor=#644883>\r\n\t\t\t\t<tr>");
			t.Append("\n\t\t\t\t\t<td class=\"p1\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
			t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:showAllContent3();\"><img id=pictofermer border=0 src=\"/Images/Common/button/picto_fermer.gif\"></a></td>");
			t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:hideAllContent3();\"><img id=pictoouvrir border=0 src=\"/Images/Common/button/picto_ouvrir.gif\"></a></td>");
			t.Append("\n\t\t\t\t\t<td bgcolor=#ffffff width=1px class=\"p2\"></td>");
			t.Append("\n\t\t\t\t</tr>\n\t\t\t</table>");
			t.Append("\r\n\t\t</td>");
			t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
			for(int h=0;h<nbCharTotal;h++){
				t.Append("&nbsp;");
			}
			t.Append("</td>");
			t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
            bool showVersions = false;
            if(webSession.CustomerLogin.FlagsList[CstDB.Flags.ID_SLOGAN_ACCESS_FLAG] != null) {
                showVersions = true;
                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(1994,webSession.SiteLanguage)+"</td>");
            }
            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(2245,webSession.SiteLanguage)+"</td>");
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++) {
				t.Append("\r\n\t\t<td rowspan=3 class=\"p2\">"+tab[0,k].ToString()+"</td>");
			}
			//fin MAJ
			#endregion

			#region Période
			string dateHtml="<tr>";
			string headerHtml="";
			DateTime currentDay = new DateTime(((DateTime)tab[0,FIRST_PERIOD_INDEX]).Ticks);
			int previousMonth = currentDay.Month;
			currentDay = currentDay.AddDays(-1);
			int nbPeriodInMonth=0;
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				currentDay = currentDay.AddDays(1);
				if (currentDay.Month!=previousMonth){
					if (nbPeriodInMonth>=8){
						headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
							+Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM"))
							+"</td>";
					}
					else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
							 +"&nbsp"
							 +"</td>";
					nbPeriodInMonth=0;
					previousMonth = currentDay.Month;
				}
				nbPeriodInMonth++;
				dateHtml+="<td class=\"p10\">&nbsp;"+currentDay.ToString("dd")+"&nbsp;</td>";
			}
			if (nbPeriodInMonth>=8)headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
									   +Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM"))
									   +"</td>";
			else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
					 +"&nbsp"
					 +"</td>";
			dateHtml += "</tr>";

			string dayClass="";
			char day;
			dateHtml+="\r\n\t<tr>";
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				day=TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,((DateTime)tab[0,j]).DayOfWeek.ToString()).ToCharArray()[0];
				if(day==GestionWeb.GetWebWord(545,webSession.SiteLanguage).ToCharArray()[0]
					|| day==GestionWeb.GetWebWord(546,webSession.SiteLanguage).ToCharArray()[0]
					){
					dayClass="p132";
				}
				else{
					dayClass="p131";
				}	
				dateHtml+="<td class=\""+dayClass+"\">"+TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,((DateTime)tab[0,j]).DayOfWeek.ToString())+"</td>";
			}
			dateHtml+="\r\n\t</tr>";

			headerHtml += "</tr>";



			headerHtml += "</tr>";
			t.Append(headerHtml);
			t.Append(dateHtml);
			#endregion

			#region Calendrier d'actions
			//t.Append("\r\n\t<tr>");
			i=0;
			string space="";
			bool firstTotal=true;
			try{
				for(i=1;i<nbline;i++){
					for(j=0;j<nbColTab;j++){
						switch(j){
							// Total ou vehicle
							case CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX:
								if(tab[i,j]!=null){
									string tmpHtml="";
                                    string tmpHtml2="";
									//if((tab[i,j].ToString()!=GestionWeb.GetWebWord(204,webSession.SiteLanguage) && tab[i,j].ToString()!=GestionWeb.GetWebWord(205,webSession.SiteLanguage) && tab[i,j].ToString()!=GestionWeb.GetWebWord(206,webSession.SiteLanguage)) && tab[i,j].ToString()!=GestionWeb.GetWebWord(1779,webSession.SiteLanguage) &&premierTotal){
									if (IsBelongToTotalLines(webSession,tab[i,j].ToString(),premierTotal)) {
										classe="pmtotal";
										if(firstTotal)
											space="";
										else{
											space="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
										}
										firstTotal=false;
									}
									else{
										classe="pmvehicle";
                                        if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] != null) {
                                            tmpHtml="<a href=\"javascript:OpenCreatives('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1','',-1,'"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            tmpHtml2="<a href=\"javascript:OpenInsertions('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        }
										premierTotal=false;
									}
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									
									
									if(premierTotal){
										if(totalUnit.Trim()==""){
											tmpHtml="";
                                            tmpHtml2="";
											totalUnit="0";
										}
                                        t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+space+""+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>":"")+"<td align=\"center\" class=\""+classe+"\">"+tmpHtml2+"</td>");
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++) {
											t.Append("<td class=\""+classe+"\">"+((double)tab[i,j+10+k]).ToString("0.##")+"</td>");
										}
										//fin MAJ
									}
									else{
										if(premierVehicle){
											if(totalUnit.Trim()==""){
												tmpHtml="";
                                                tmpHtml2="";
												totalUnit="0";
											}
                                            t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>":"")+"<td align=\"center\" class=\""+classe+"\">"+tmpHtml2+"</td>");
											//MAJ GR : totaux par années si nécessaire
											for(k=1; k<=nbColYear; k++) {
												t.Append("<td class=\""+classe+"\">"+((double)tab[i,j+10+k]).ToString("0.##")+"</td>");
											}
											//fin MAJ
											premierVehicle=false;
										}
										else{
											classe=PLAN_MEDIA_1_CLASSE;
                                            if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] != null) {
                                                tmpHtml="<a href=\"javascript:OpenCreatives('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1','','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]+"','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                                tmpHtml2="<a href=\"javascript:OpenInsertions('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1,"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]+"','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            }
                                            else {
                                                tmpHtml="";
                                                tmpHtml2="";
                                            }
											
											
											if(totalUnit.Trim()==""){
												tmpHtml="";
                                                tmpHtml2="";
												totalUnit="0";
											}
                                            t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>":"")+"<td align=\"center\" class=\""+classe+"\">"+tmpHtml2+"</td>");
											//MAJ GR : totaux par années si nécessaire
											for(k=1; k<=nbColYear; k++) {
												t.Append("<td class=\""+classe+"\">"+((double)tab[i,j+10+k]).ToString("0.##")+"</td>");
											}
											//fin MAJ
										}
									}
									
									//aller aux colonnes du calendrier d'action
									j=j+10+nbColYear;
								}
								break;
								// Category
							case CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX:
								if(tab[i,j]!=null){
									premierVehicle=true;
									string tmpHtml="";
                                    string tmpHtml2="";
                                    if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]!=null) {
                                        tmpHtml="<a href=\"javascript:OpenCreatives('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+",-1,-1','','-1','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        tmpHtml2="<a href=\"javascript:OpenInsertions('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+",-1,-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                    }
									// On traite le cas des pages
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);

									if(premierCategory){
										if(totalUnit.Trim()==""){
                                            tmpHtml2="";
											tmpHtml="";
											totalUnit="0";
										}
                                        t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"');\" style=\"DISPLAY:inline; CURSOR: hand\">\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategorynb\">"+totalUnit+"</td><td class=\"pmcategorynb\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\"pmcategory\">"+tmpHtml+"</td>":"")+"<td align=\"center\" class=\"pmcategory\">"+tmpHtml2+"</td>");
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++) {
											t.Append("<td class=\"pmcategory\">"+((double)tab[i,j+9+k]).ToString("0.##")+"</td>");
										}
										//fin MAJ
										premierCategory=false;
										currentCategoryName=tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX].ToString();
									}
									else{
                                        if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]!=null) {
                                            tmpHtml ="<a href=\"javascript:OpenCreatives('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+",-1,-1','','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]+"','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            tmpHtml2="<a href=\"javascript:OpenInsertions('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+",-1,-1,"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]+"','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        }
                                        else {
                                            tmpHtml="";
                                            tmpHtml2="";
                                        }
										
										if(totalUnit.Trim()==""){
											tmpHtml="";
                                            tmpHtml2="";
											totalUnit="0";
										}
										if(tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]!=null){
											t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"');\" style=\"DISPLAY:inline; \">\r\n\t\t<td class=\"pmcategoryb\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategorynbb\">"+totalUnit+"</td><td class=\"pmcategorynbb\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\"pmcategoryb\">"+tmpHtml+"</td>":"")+"<td align=\"center\" class=\"pmcategoryb\">"+tmpHtml2+"</td>");
										}
										else{
                                            t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"');\" style=\"DISPLAY:inline; \">\r\n\t\t<td class=\"pmcategoryb\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategorynbb\">"+totalUnit+"</td><td class=\"pmcategorynbb\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\"pmcategoryb\"></td>":"")+"<td align=\"center\" class=\"pmcategoryb\"></td>");
										}
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++) {
											t.Append("<td class=\"pmcategoryb\">"+((double)tab[i,j+9+k]).ToString("0.##")+"</td>");
										}
										//fin MAJ

									}
									//currentCategoryName=tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX].ToString();
									//aller aux colonnes du calendrier d'action
									j=j+9+nbColYear;
								}
								break;
								// Media
							case CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX:
								// On traite le cas des pages
								if(tab[i,j]!=null){
									premierCategory=true;
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									//	if(premier){
									//On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la catégorie "chaîne thématique"
									if (tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null){
                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"><a href=\"javascript:OpenCreatives('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1','','-1','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>":"")+"<td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"><a href=\"javascript:OpenInsertions('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
									}
									else
                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"></td>":"")+"<td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"></td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++) {
										t.Append("<td class=\""+PLAN_MEDIA_2_CLASSE+"\">"+((double)tab[i,j+8+k]).ToString("0.##")+"</td>");
									}
									//aller aux colonnes du calendrier d'action
									j=j+8+nbColYear;
									//fin MAJ
								}
								break;
							case CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX:
								break;

								// Advertiser
							case CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX:
								totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
								if(tab[i, FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]!=null){
                                    t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\"><a href=\"javascript:OpenCreatives('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1','','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]+"','"+moduleId.ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>":"")+"<td align=\"center\" class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\"><a href=\"javascript:OpenInsertions('"+webSession.IdSession+"','"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_CATEGORY_COlUMN_INDEX]+","+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1,"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ID_ADVERTISER_COLUMN_INDEX]+"','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
								}
								else{
									t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>"+((showVersions)?"<td align=\"center\" class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\"></td>":"")+"<td align=\"center\" class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\"></td>");
								}
								//MAJ GR : totaux par années si nécessaire
								for(k=1; k<=nbColYear; k++) {
									t.Append("<td class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\">"+((double)tab[i,j+6+k]).ToString("0.##")+"</td>");
								}
								j=j+6+nbColYear;
								//fin MAJ
								break;
		

							default:
								if(tab[i,j]==null){
									t.Append("<td class=\"p3\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType)){
									switch((FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType)tab[i,j]){
										case FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType.present:
											t.Append("<td class=\"p4\">&nbsp;</td>");
											break;
										case FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType.extended:
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
					t.Append("</tr>");
					
				}
				
			}
			catch(System.Exception e){
				throw(new WebExceptions.CompetitorMediaPlanAlerUI("erreur i="+i+",j="+j,e));
			}
			//t.Append("</table></td></tr></table>");
			t.Append("</table>");
			#endregion

			// On vide le tableau
			tab=null;

			return(t.ToString());

		}

		#endregion

		#region Sortie Ms Excel (Web)
		/// <summary>
		/// Génère le code HTML pour Ms Excel pour afficher un calendrier d'action entre deux périodes respectant le type de période spécifié dans la session.
		/// Elles se base sur un tableau contenant les données. Cette méthode sert aussi bien pour les zooms que pour les alertes
		///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
		///		Rappel des paramètres de périodes 
		///		Génération du code HTML des périodes (Libellé du mois, intitulés d'une unité de période)
		///		Génération du code HTML du calendrier d'action
		/// </summary>
		/// <returns>Code Généré</returns>
		/// <param name="tab">Tableau contenant les données à mettre en forme</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="periodBeginning">Période de début du calendrier d'action</param>
		/// <param name="periodEnd">Période de fin du calendrier d'action</param>
		/// <returns>Code Généré</returns>
		public static string GetMediaPlanAlertExcelUI(object[,] tab,WebSession webSession, string periodBeginning, string periodEnd){

			#region Variables
			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = int.Parse(periodEnd.Substring(0,4)) - int.Parse(periodBeginning.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear;
			//fin MAJ

			string classe;
			//A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
			//que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
			//affichée, utilisez getLength-1
			int nbColTab=tab.GetLength(1),j,i;
			int nbline=tab.GetLength(0);
			int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
			//	bool premier=true;
			bool premierVehicle=true;
			bool premierCategory=true;
			bool premierAdvertiser=true;
			bool premierTotal=true;
			const string PLAN_MEDIA_XLS_1_CLASSE="pmmediaxls1";
			const string PLAN_MEDIA_XLS_2_CLASSE="pmmediaxls2";
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			string totalUnit="";
			#endregion
		
			#region Rappel des paramètres
			t.Append(ExcelFunction.GetLogo());
			t.Append(ExcelFunction.GetExcelHeader(webSession,true,periodBeginning,periodEnd));
			#endregion

			#region debut Tableau
			t.Append("<table border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
			t.Append("\r\n\t\t<td rowspan=\"3\" width=\"200px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
			t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
			t.Append("</td>");
			t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.CompetitorMediaPlanAlert.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++) {
				t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+tab[0,k].ToString()+"</td>");
			}
			//fin MAJ

			#endregion

			#region Période
			string dateHtml="<tr>";
			string headerHtml="";
			DateTime currentDay = new DateTime(((DateTime)tab[0,FIRST_PERIOD_INDEX]).Ticks);
			int previousMonth = currentDay.Month;
			currentDay = currentDay.AddDays(-1);
			int nbPeriodInMonth=0;
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				currentDay = currentDay.AddDays(1);
				if (currentDay.Month!=previousMonth){
					if (nbPeriodInMonth>=8){
						headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
							+ Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM")))
							+"</td>";
					}
					else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
							 +"&nbsp"
							 +"</td>";
					nbPeriodInMonth=0;
					previousMonth = currentDay.Month;
				}
				nbPeriodInMonth++;
				dateHtml+="<td class=\"p10\">"+currentDay.ToString("dd")+"</td>";
			}
			if (nbPeriodInMonth>=8)headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
									   + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM")))
									   +"</td>";
			else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
					 +"&nbsp"
					 +"</td>";
			dateHtml += "</tr>";

			string dayClass="";
			char day;
			dateHtml+="\r\n\t<tr>";
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				day=TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,((DateTime)tab[0,j]).DayOfWeek.ToString()).ToCharArray()[0];
				if(day==GestionWeb.GetWebWord(545,webSession.SiteLanguage).ToCharArray()[0]
					|| day==GestionWeb.GetWebWord(546,webSession.SiteLanguage).ToCharArray()[0]
					){
					dayClass="p133";
				}
				else{
					dayClass="p132";
				}	
				dateHtml+="<td class=\""+dayClass+"\">"+TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,((DateTime)tab[0,j]).DayOfWeek.ToString())+"</td>";
			}
			dateHtml+="\r\n\t</tr>";

			headerHtml += "</tr>";
			t.Append(headerHtml);
			t.Append(dateHtml);
			#endregion

			#region Tableau
			i=0;
			try{
				for(i=1;i<nbline;i++){
					for(j=0;j<nbColTab;j++){
						switch(j){
								// Total ou vehicle
							case CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX:
								if(tab[i,j]!=null){
									//if((tab[i,j].ToString()!=GestionWeb.GetWebWord(204,webSession.SiteLanguage) && tab[i,j].ToString()!=GestionWeb.GetWebWord(205,webSession.SiteLanguage) && tab[i,j].ToString()!=GestionWeb.GetWebWord(206,webSession.SiteLanguage)) && premierTotal){
									if (IsBelongToTotalLines(webSession,tab[i,j].ToString(),premierTotal)) {
										classe="pmtotal";
									}
									else{
										classe="pmcategory";
										premierTotal=false;
									}
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString().Trim()==""){
										tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]="0";
									}

									// On traite les unités
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
						
									if(premierTotal){
										
										t.Append("\r\n\t\t<td class=\""+classe+"\">&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+totalUnit+"</td><td class=\""+classe+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++) {
											t.Append("<td class=\""+classe+"\">"+((double)tab[i,j+10+k]).ToString("0.##")+"</td>");
										}
										//fin MAJ
									}
									else{
										if(premierVehicle){
											t.Append("\r\n\t\t<td class=\""+classe+"\">&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+totalUnit+"</td><td class=\""+classe+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
											//MAJ GR : totaux par années si nécessaire
											for(k=1; k<=nbColYear; k++) {
												t.Append("<td class=\""+classe+"\">"+((double)tab[i,j+10+k]).ToString("0.##")+"</td>");
											}
											//fin MAJ
											premierVehicle=false;
										}
										else{
											classe="pmcategory";
											t.Append("\r\n\t\t<td class=\""+classe+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+totalUnit+"</td><td class=\""+classe+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
											//MAJ GR : totaux par années si nécessaire
											for(k=1; k<=nbColYear; k++) {
												t.Append("<td class=\""+classe+"\">"+((double)tab[i,j+10+k]).ToString("0.##")+"</td>");
											}
											//fin MAJ
										}
									}
									//MAJ GR
									//saut jusquà la 1ère colonne du calendrier d'action
									j=j+10+nbColYear;
									//MAJ GR
								}
								break;
								// Category
							case CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX:
								if(tab[i,j]!=null){
									premierVehicle=true;
									if(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString().Trim()==""){
										tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]="0";
									}

									//On traite les unités
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);

									if(premierCategory){
//										t.Append("\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategory\">"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+"</td><td class=\"pmcategory\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
										t.Append("\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategory\">"+totalUnit+"</td><td class=\"pmcategory\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++) {
											t.Append("<td class=\"pmcategorynb\">"+((double)tab[i,j+9+k]).ToString("0.##")+"</td>");
										}
										//fin MAJ
										premierCategory=false;
									}else{
//										t.Append("\r\n\t\t<td class=\"pmcategoryadvertiser\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategoryadvertiser\">"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+"</td><td class=\"pmcategoryadvertiser\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
										t.Append("\r\n\t\t<td class=\"pmcategoryadvertiser\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategoryadvertiser\">"+totalUnit+"</td><td class=\"pmcategoryadvertiser\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++) {
											t.Append("<td class=\"pmcategoryadvertisernb\">"+((double)tab[i,j+9+k]).ToString("0.##")+"</td>");
										}
										//fin MAJ
									}
									//MAJ GR
									//saut jusquà la 1ère colonne du calendrier d'action
									j=j+9+nbColYear;
									//fin MAJ
								}
								break;
								// Media
							case CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX:
								if(tab[i,j]!=null){
									premierCategory=true;
									totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
									
//									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									//MAJ GR : totaux par années si nécessaire
									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									for(k=1; k<=nbColYear; k++) {
										t.Append("<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,j+8+k]).ToString("0.##")+"</td>");
									}
									//saut jusquà la 1ère colonne du calendrier d'action
									j=j+8+nbColYear;
									//fin MAJ
										
										
								}
								break;
							
							case CompetitorMediaPlanAlert.ID_MEDIA_COLUMN_INDEX:
								break;
							
								// Advertiser
							case CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX:
								totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
								if(premierAdvertiser){
//									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									premierAdvertiser=false;
								}
								else{
//									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.TOTAL_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.ADVERTISER_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.CompetitorMediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
									premierAdvertiser=true;
								}
								//MAJ GR : totaux par années si nécessaire
								for(k=1; k<=nbColYear; k++) {
									t.Append("<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,j+6+k]).ToString("0.##")+"</td>");
								}
								j=j+6+nbColYear;
								//fin MAJ
								break;

							default:
								if(tab[i,j]==null){
									t.Append("<td class=\"p3\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType)){
									switch((FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType)tab[i,j]){
										case FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType.present:
											t.Append("<td class=\"p4\">&nbsp;</td>");
											break;
										case FrameWorkResultConstantes.CompetitorMediaPlanAlert.graphicItemType.extended:
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
				throw(new System.Exception("erreur i="+i+",j="+j,e));
			}
			t.Append("</tr></table></td></tr></table>");
			#endregion

			// On vide le tableau
			tab=null;

			t.Append(ExcelFunction.GetFooter(webSession));
			return(t.ToString());
		}
		#endregion	

		#region Méthodes Internes
		/// <summary>
		/// Verifei si la ligne fait partie de celles des totaux
		/// </summary>
		/// <param name="stringValue">valeur</param>
		/// <param name="premier">indique si c'est la première ligne</param>
		/// <returns></returns>
		private static bool IsBelongToTotalLines(WebSession webSession, string stringValue, bool premier) {
			if ((stringValue != GestionWeb.GetWebWord(204, webSession.SiteLanguage)
				&& stringValue != GestionWeb.GetWebWord(205, webSession.SiteLanguage)
				&& stringValue != GestionWeb.GetWebWord(206, webSession.SiteLanguage))
				&& stringValue != GestionWeb.GetWebWord(1779, webSession.SiteLanguage)
				&& stringValue != GestionWeb.GetWebWord(207, webSession.SiteLanguage)
				&& premier)
				return true;

			return false;
		}
		#endregion

	}
}
