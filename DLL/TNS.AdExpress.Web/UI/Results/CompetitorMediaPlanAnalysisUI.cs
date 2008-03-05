#region Information
// Auteur : A.Obermeyer
// Créé le : 24/09/2004
// Modifié le : 
//		10/05/2005	K.Shehzad	Addition of Agglomeration colunm for Outdoor creations 
//		12/08/2005	G. Facon	Nom de fonctions et exceptions
#endregion

using System;
using System.Collections;
using System.Web;
using System.Web.UI;
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

namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Interface Utilisateur d'un plan Media Concurentiel
	/// Cette classe génère suivant le format de sortie le code pour afficher 
	/// un plan media concurentiel.
	/// </summary>
	public class CompetitorMediaPlanAnalysisUI{
		
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
//			string currentMediaName="tot";
			string totalUnit="";
			int nbPeriodInYear=0;
			string prevYearString;
			int prevYear=0;
			int nbColTab=tab.GetLength(1),j,i;
			int nbPeriod=nbColTab-5;
		//	bool premier=true;
			bool premierVehicle=true;
			bool premierCategory=true;
		//	bool premierAdvertiser=true;
			bool premierTotal=true;
			#endregion

			#region Constantes
		//	const string PLAN_MEDIA_1_CLASSE="p6";
			const string PLAN_MEDIA_2_CLASSE="p6";
		//	const string PLAN_MEDIA_NB_1_CLASSE="p8";
			const string PLAN_MEDIA_NB_2_CLASSE="p8";
			const string PLAN_MEDIA_1_ADVERTISER_CLASSE="asl5b";
			const string PLAN_MEDIA_NB_1_ADVERTISER_CLASSE="asl5nbb";
			#endregion

			#region Détermine s'il y a plusieurs années pour les totaux par année
			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.CompetitorMediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
			//fin MAJ
			#endregion
			
			#region On calcule la taille de la colonne Total
			int nbtot=10;
			//nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,5].ToString(),webSession.Unit).Length;	
			nbtot = WebFunctions.Units.ConvertUnitValueToString(tab[1, CompetitorMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit).Length;			
			int nbSpace=(nbtot-1)/3;
			//int nbCharTotal=nbtot+nbSpace-5;
			int nbCharTotal = nbtot + nbSpace - CompetitorMediaPlan.TOTAL_COLUMN_INDEX;
			if(nbCharTotal<5)nbCharTotal=0;
			#endregion

			System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
			
			// On enregistre le script DynamicMediaPlan qui rend les calendriers d'action dynamique
            if (!page.ClientScript.IsClientScriptBlockRegistered("DynamicMediaPlan")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "DynamicMediaPlan", TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan());
			
			#region Libellé des colonnes
			t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");		
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
			for(k=FrameWorkResultConstantes.CompetitorMediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++){
				t.Append("\r\n\t\t<td rowspan=2 class=\"p2\">"+tab[0,k].ToString());
				//for(int h=0;h<nbCharTotal+5;h++){
				for (int h = 0; h < nbCharTotal + CompetitorMediaPlan.TOTAL_COLUMN_INDEX; h++) {
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
					HTML+="<td class=\"p10\"><a class=\"p10\" href=\""+ CstWeb.Links.ZOOM_COMPETITOR_PLAN_MEDIA + "?idSession=" + webSession.IdSession + "&zoomDate="+tab[0,j].ToString()+"\">&nbsp;"+TNS.FrameWork.Date.MonthString.Get(int.Parse(tab[0,j].ToString().Substring(4,2)),webSession.SiteLanguage,1)+"&nbsp;&nbsp;</td>";
				}
				else{
					HTML+="<td class=\"p10\"><a class=\"p10\" href=\""+ CstWeb.Links.ZOOM_COMPETITOR_PLAN_MEDIA + "?idSession=" + webSession.IdSession + "&zoomDate="+tab[0,j].ToString()+"\">"+tab[0,j].ToString().Substring(4,2)+"<a></td>";
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

			i=0;
			bool firstTotal=true;
			string space="";

			#region Calendrier d'actions
			try{
				for(i=1;i<tab.GetLength(0);i++){
					for(j=0;j<tab.GetLength(1);j++){
						switch(j){
								// Total ou vehicle
							case CompetitorMediaPlan.VEHICLE_COLUMN_INDEX:
								if(tab[i,j]!=null){
									//if((tab[i,j].ToString()!=GestionWeb.GetWebWord(204,webSession.SiteLanguage) && tab[i,j].ToString()!=GestionWeb.GetWebWord(205,webSession.SiteLanguage) && tab[i,j].ToString()!=GestionWeb.GetWebWord(206,webSession.SiteLanguage)) && tab[i,j].ToString()!=GestionWeb.GetWebWord(1779,webSession.SiteLanguage) && premierTotal){
									if(premierTotal && tab[i,CompetitorMediaPlan.ID_VEHICLE_COLUMN_INDEX]==null){
										classe="pmtotal";
										if(firstTotal)
										space="";
										else {
										 space="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";}

										firstTotal=false;

									}
									else {
										classe="pmvehicle";
										
										premierTotal=false;
									}
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									
									// On traite le cas des pages
									//totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 5].ToString(), webSession.Unit);
									totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX)].ToString(), webSession.Unit);//Ajouté par DM 31012008
									
									if(totalUnit.Trim()==""){
										totalUnit="0";
									}
									if(premierTotal){
										//t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+space+""+tab[i,j]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,j+6]).ToString("0.00")+"</td>");
										t.Append("\r\n\t<tr id=\"" + tab[i, j] + "Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\"" + classe + "\">" + space + "" + tab[i, j] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX)]).ToString("0.00") + "</td>");//Ajouté par DM 31012008
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++){
											//t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+6+k].ToString(),webSession.Unit)+"</td>");
											t.Append("<td class=\"" + classe + "nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");
										}
										//fin MAJ
									}
									else{
										if(premierVehicle){
											//t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,j]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,j+6]).ToString("0.00")+"</td>");
											t.Append("\r\n\t<tr id=\"" + tab[i, j] + "Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\"" + classe + "\">" + tab[i, j] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX)]).ToString("0.00") + "</td>");//Ajouté par DM 31012008
											//MAJ GR : totaux par années si nécessaire
											for(k=1; k<=nbColYear; k++){
												//t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+6+k].ToString(),webSession.Unit)+"</td>");
												t.Append("<td class=\"" + classe + "nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");//Ajouté par DM 31012008
											}
											//fin MAJ
											premierVehicle=false;
										}
										else{
											classe="p16";
											//t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,j+6]).ToString("0.00")+"</td>");
											t.Append("\r\n\t<tr id=\"" + tab[i, j] + "Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\"" + classe + "\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + tab[i, j] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX)]).ToString("0.00") + "</td>");
											//MAJ GR : totaux par années si nécessaire
											for(k=1; k<=nbColYear; k++){
												//t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+6+k].ToString(),webSession.Unit)+"</td>");
												t.Append("<td class=\"" + classe + "nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");//Ajouté par DM 31012008
											}
											//fin MAJ
										}
									}
									//j=j+6+nbColYear;//Ajouté par DM 31012008
									j = j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX) + nbColYear;
								}
								break;
								// Category
							case CompetitorMediaPlan.CATEGORY_COLUMN_INDEX:
								premierVehicle=true;
								if(tab[i,j]!=null){
									// On traite le cas des pages
									// On met 4 au lieu de 3
									//totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4].ToString(),webSession.Unit);
									totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX)].ToString(), webSession.Unit);//Ajouté par DM 31012008
									if(totalUnit.Trim()==""){
										totalUnit="0";
									}
									// on met 5
									if(premierCategory){
										//t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,j]+"');\" style=\"DISPLAY:inline; CURSOR: hand\">\r\n\t\t<td class=\"pmcategory\">&nbsp;"+tab[i,j]+"</td><td class=\"pmcategorynb\">"+totalUnit+"</td><td class=\"pmcategorynb\">"+((double)tab[i,j+5]).ToString("0.00")+"</td>");
										t.Append("\r\n\t<tr id=\"" + i.ToString() + "Content4\"onclick=\"showHideContent3('" + tab[i, j] + "');\" style=\"DISPLAY:inline; CURSOR: hand\">\r\n\t\t<td class=\"pmcategory\">&nbsp;" + tab[i, j] + "</td><td class=\"pmcategorynb\">" + totalUnit + "</td><td class=\"pmcategorynb\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX)]).ToString("0.00") + "</td>");//Ajouté par DM 31012008
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++){
											//t.Append("<td class=\"pmcategorynb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+5+k].ToString(),webSession.Unit)+"</td>");
											t.Append("<td class=\"pmcategorynb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");//Ajouté par DM 31012008
										}
										//fin MAJ
										premierCategory=false;
										currentCategoryName=tab[i,j].ToString();
									}
									else{
										//t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,j]+"');\" style=\"DISPLAY:inline; \">\r\n\t\t<td class=\"pmcategoryadvertiser\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\"pmcategoryadvertisernb\">"+totalUnit+"</td><td class=\"pmcategoryadvertisernb\">"+((double)tab[i,j+5]).ToString("0.00")+"</td>");
										t.Append("\r\n\t<tr id=\"" + i.ToString() + "Content4\"onclick=\"showHideContent3('" + tab[i, j] + "');\" style=\"DISPLAY:inline; \">\r\n\t\t<td class=\"pmcategoryadvertiser\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + tab[i, j] + "</td><td class=\"pmcategoryadvertisernb\">" + totalUnit + "</td><td class=\"pmcategoryadvertisernb\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX)]).ToString("0.00") + "</td>");
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++){
											//t.Append("<td class=\"pmcategoryadvertisernb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+5+k].ToString(),webSession.Unit)+"</td>");
											t.Append("<td class=\"pmcategoryadvertisernb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");//Ajouté par DM 31012008
										}
										//fin MAJ
									}
									
									
									//j=j+5+nbColYear;
									j = j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX) + nbColYear;
								}
								break;
								// Media
							case CompetitorMediaPlan.MEDIA_COLUMN_INDEX:
								premierCategory=true;
								// On traite le cas des pages
								if(tab[i,j]!=null){
									//currentMediaName=tab[i,j].ToString();
									//totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3].ToString(),webSession.Unit);
									totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.MEDIA_COLUMN_INDEX)].ToString(), webSession.Unit);
									if(totalUnit.Trim()==""){
										totalUnit="0";
									}
									//t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,j+4]).ToString("0.00")+"</td>");
									t.Append("\r\n\t<tr id=\"" + i.ToString() + currentCategoryName + "Content3\" onclick=\"showHideAllContent3('" + i.ToString() + currentCategoryName + "','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\"" + PLAN_MEDIA_2_CLASSE + "\">&nbsp;&nbsp;" + tab[i, j] + "</td><td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.MEDIA_COLUMN_INDEX)]).ToString("0.00") + "</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++){
										//t.Append("<td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+4+k].ToString(),webSession.Unit)+"</td>");
										t.Append("<td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.MEDIA_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");
									}
									//j=j+4+nbColYear;
									j = j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.MEDIA_COLUMN_INDEX) + nbColYear;
									//fin MAJ
								}
								break;
							// Advertiser
							case CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX:
								//totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,j+2].ToString(),webSession.Unit);
								totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX)].ToString(), webSession.Unit);
								if(totalUnit.Trim()==""){
									totalUnit="0";
								} 

								//t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_ADVERTISER_CLASSE+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,j]+"</td><td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+((double)tab[i,j+3]).ToString("0.00")+"</td>");
								t.Append("\r\n\t<tr id=\"" + i.ToString() + currentCategoryName + "Content3\" onclick=\"showHideAllContent3('" + i.ToString() + currentCategoryName + "','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\"" + PLAN_MEDIA_1_ADVERTISER_CLASSE + "\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + tab[i, j] + "</td><td class=\"" + PLAN_MEDIA_NB_1_ADVERTISER_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_NB_1_ADVERTISER_CLASSE + "\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX)]).ToString("0.00") + "</td>");
								//MAJ GR : totaux par années si nécessaire
								for(k=1; k<=nbColYear; k++){
									//t.Append("<td class=\""+PLAN_MEDIA_NB_1_ADVERTISER_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+3+k].ToString(),webSession.Unit)+"</td>");
									t.Append("<td class=\"" + PLAN_MEDIA_NB_1_ADVERTISER_CLASSE + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");
								}
								//fin MAJ
								//			premierAdvertiser=true;
						//		}	
								//MAJ GR
								//j=j+3+nbColYear;
								j = j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX) + nbColYear;
								//fin MAJ
								break;

							default:
								
								if(tab[i,j]==null){
									t.Append("<td class=\"p3\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.CompetitorMediaPlan.graphicItemType)){
									switch((FrameWorkResultConstantes.CompetitorMediaPlan.graphicItemType)tab[i,j]){
										case FrameWorkResultConstantes.CompetitorMediaPlan.graphicItemType.present:
											t.Append("<td class=\"p4\">-</td>");
											break;
										case FrameWorkResultConstantes.CompetitorMediaPlan.graphicItemType.extended:
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

		#region Sortie Ms Excel

		/// <summary>
		/// Crée le code HTML pour Excel pour afficher un plan media
		/// </summary>
		/// <param name="tab">Tableau contenant les données</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML</returns>
		public static string GetMediaPlanAnalysisExcelUI(object[,] tab,WebSession webSession){
			if(tab.GetLength(0)==0)return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
			string classe;
			string HTML="";
			string HTML2="";
			int nbPeriodInYear=0;
			int prevYear=0;
			int nbColTab=tab.GetLength(1),j,i;
			int nbPeriod=nbColTab-5;
			//bool premier=true;
			bool premierVehicle=true;
			bool premierCategory=true;
			bool premierAdvertiser=true;
			bool premierTotal=true;
			const string PLAN_MEDIA_XLS_1_CLASSE="pmmediaxls1";
			const string PLAN_MEDIA_XLS_2_CLASSE="pmmediaxls2";

			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.CompetitorMediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
			//fin MAJ

			System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
            t.Append(ExcelFunction.GetLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession,true,true));

			t.Append("<table border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");

			t.Append("\r\n\t\t<td width=\"230px\" class=\"p2\"></td>");
			t.Append("\r\n\t\t<td class=\"p2\"></td>");
			t.Append("\r\n\t\t<td class=\"p2\"></td>");
			// MAJ GR : case vide
			for(k=FrameWorkResultConstantes.CompetitorMediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++)
			{
				t.Append("\r\n\t\t<td class=\"p2\"></td>");
			}
			//fin MAJ
		
			// On affiche les mois
		
			prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				if(prevYear!=int.Parse(tab[0,j].ToString().Substring(0,4))){
					HTML2+="<td width=\""+nbPeriodInYear*20+"px\" colspan="+nbPeriodInYear+" class=\"pmannee\">"+prevYear+"</td>";
					nbPeriodInYear=0;
					prevYear=int.Parse(tab[0,j].ToString().Substring(0,4));

				}
				if(webSession.DetailPeriod==Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly){
					HTML+="<td style=\"width:15pt\" class=\"p10\">"+TNS.FrameWork.Date.MonthString.Get(int.Parse(tab[0,j].ToString().Substring(4,2)),webSession.SiteLanguage,1)+"</td>";
				}
				else{
					HTML+="<td style=\"width:15pt\" class=\"p10\">"+tab[0,j].ToString().Substring(4,2)+"</td>";
				}
				nbPeriodInYear++;
			}
			HTML2+="<td width=\""+nbPeriodInYear*20+"px\" colspan="+nbPeriodInYear+" class=\"pmannee2\">"+prevYear+"</td>";
			t.Append(HTML2+"</tr><tr>");
			t.Append("\r\n\t\t<td width=\"230px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
			t.Append("\r\n\t\t<td class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
			t.Append("\r\n\t\t<td class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");

			// MAJ GR : On affiche les années si nécessaire
			for(k=FrameWorkResultConstantes.CompetitorMediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++)
			{
				t.Append("\r\n\t\t<td class=\"p2\">"+tab[0,k].ToString()+"</td>");
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
								// Total ou vehicle
							case CompetitorMediaPlan.VEHICLE_COLUMN_INDEX:
								if(tab[i,j]!=null){
									//if((tab[i,j].ToString()!=GestionWeb.GetWebWord(204,webSession.SiteLanguage) && tab[i,j].ToString()!=GestionWeb.GetWebWord(205,webSession.SiteLanguage) && tab[i,j].ToString()!=GestionWeb.GetWebWord(206,webSession.SiteLanguage)) && premierTotal){
									if (premierTotal && tab[i, CompetitorMediaPlan.ID_VEHICLE_COLUMN_INDEX] == null) {
										classe="pmtotal";
									}
									else{ 
										//classe="pmcategoryadvertiser";
										classe = "pmvehicle";
										premierTotal=false;
									}
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									if(premierTotal){
										t.Append("\r\n\t\t<td class=\"" + classe + "\">&nbsp;" + tab[i, j] + "</td><td class=\"" + classe + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX)].ToString(), webSession.Unit) + "</td><td class=\"" + classe + "\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX)]).ToString("0.00") + "</td>");
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++)
										{
											t.Append("<td class=\"" + classe + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");
										}
										//fin MAJ
									}
									else{
										if(premierVehicle){
											t.Append("\r\n\t\t<td class=\"" + classe + "\">&nbsp;" + tab[i, j] + "</td><td class=\"" + classe + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX)].ToString(), webSession.Unit) + "</td><td class=\"" + classe + "\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX)]).ToString("0.00") + "</td>");
											//MAJ GR : totaux par années si nécessaire
											for(k=1; k<=nbColYear; k++)
											{
												t.Append("<td class=\"" + classe + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");
											}
											//fin MAJ
											premierVehicle=false;
										}
										else{
											classe="pmcategoryadvertiser";											
											t.Append("\r\n\t\t<td class=\"" + classe + "\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + tab[i, j] + "</td><td class=\"" + classe + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX)].ToString(), webSession.Unit) + "</td><td class=\"" + classe + "\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX)]).ToString("0.00") + "</td>");
											//MAJ GR : totaux par années si nécessaire
											for(k=1; k<=nbColYear; k++)
											{
												t.Append("<td class=\"" + classe + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");
											}
											//fin MAJ
										}
									}
									//MAJ GR
									j = j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.VEHICLE_COLUMN_INDEX) + nbColYear;
									//fin MAJ
								}
								break;
								// Category
							case CompetitorMediaPlan.CATEGORY_COLUMN_INDEX:
								if(tab[i,j]!=null){
									premierVehicle=true;
									if(premierCategory){
										t.Append("\r\n\t\t<td class=\"pmcategory\">&nbsp;" + tab[i, j] + "</td><td class=\"pmcategory\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX)].ToString(), webSession.Unit) + "</td><td class=\"pmcategory\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX)]).ToString("0.00") + "</td>");
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++)
										{
											t.Append("<td class=\"pmcategorynb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");
										}
										//fin MAJ
										premierCategory=false;
									}else{
										t.Append("\r\n\t\t<td class=\"pmcategoryadvertiser\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + tab[i, j] + "</td><td class=\"pmcategoryadvertiser\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX)].ToString(), webSession.Unit) + "</td><td class=\"pmcategoryadvertiser\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX)]).ToString("0.00") + "</td>");
										//MAJ GR : totaux par années si nécessaire
										for(k=1; k<=nbColYear; k++)
										{
											t.Append("<td class=\"pmcategoryadvertisernb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");
										}
										//fin MAJ
									}
									//MAJ GR
									j = j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.CATEGORY_COLUMN_INDEX) + nbColYear;
									//fin MAJ
								
								
								}
								break;
								// Media
							case CompetitorMediaPlan.MEDIA_COLUMN_INDEX:
								if(tab[i,j]!=null){
									premierCategory=true;
									t.Append("\r\n\t\t<td class=\"" + PLAN_MEDIA_XLS_1_CLASSE + "\">&nbsp;" + tab[i, j] + "</td><td class=\"" + PLAN_MEDIA_XLS_1_CLASSE + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.MEDIA_COLUMN_INDEX)].ToString(), webSession.Unit) + "</td><td class=\"" + PLAN_MEDIA_XLS_1_CLASSE + "\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.MEDIA_COLUMN_INDEX)]).ToString("0.00") + "</td>");
									//MAJ GR : totaux par années si nécessaire
									for(k=1; k<=nbColYear; k++)
									{
										t.Append("<td class=\"" + PLAN_MEDIA_XLS_1_CLASSE + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.MEDIA_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");
									}
									j = j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.MEDIA_COLUMN_INDEX) + nbColYear;
									//fin MAJ
								}
								break;
							// Advertiser
							case CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX:
								if(premierAdvertiser){
									t.Append("\r\n\t\t<td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + tab[i, j] + "</td><td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX)].ToString(), webSession.Unit) + "</td><td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX)]).ToString("0.00") + "</td>");
									premierAdvertiser=false;
								}
								else{
									t.Append("\r\n\t\t<td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + tab[i, j] + "</td><td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.TOTAL_COLUMN_INDEX - CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX)].ToString(), webSession.Unit) + "</td><td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">" + ((double)tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX)]).ToString("0.00") + "</td>");
									premierAdvertiser=true;
								}
								//MAJ GR : totaux par années si nécessaire
								for(k=1; k<=nbColYear; k++)
								{									
									t.Append("<td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX) + k].ToString(), webSession.Unit) + "</td>");
								}
								j = j + (CompetitorMediaPlan.PDM_COLUMN_INDEX - CompetitorMediaPlan.ADVERTISER_COLUMN_INDEX) + nbColYear;
								//fin MAJ
								break;
							default:
								if(tab[i,j]==null){
									t.Append("<td class=\"pmperioditemxls1\">&nbsp;</td>");
									break;
								}
								if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.CompetitorMediaPlan.graphicItemType)){
									switch((FrameWorkResultConstantes.CompetitorMediaPlan.graphicItemType)tab[i,j]){
										case FrameWorkResultConstantes.CompetitorMediaPlan.graphicItemType.present:
											t.Append("<td class=\"pmperioditemxls\">&nbsp;</td>");
											break;
										case FrameWorkResultConstantes.CompetitorMediaPlan.graphicItemType.extended:
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
