#region Informations
// Auteur: B. Masson
// Date de création: 23/02/2007
// Date de modification:
#endregion

#region Namespaces
using System;
using System.Collections;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.Exceptions;
using TNS.AdExpress.Bastet.DataAccess;
using TNS.AdExpress.Bastet.Rules;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using FrkDate=TNS.FrameWork.Date;
using TNS.FrameWork.Date;
using Cst=TNS.AdExpress.Bastet.Constantes;
using TNS.AdExpress.Bastet.Functions;
using TNS.AdExpress.Bastet.Translation;
using TNS.AdExpress.Bastet.Web;
using System.Globalization;
#endregion

namespace TNS.AdExpress.Bastet.UI{
	/// <summary>
	/// Classe pour l'affichage des indicateurs
	/// </summary>
	public class IndicatorsUI{
		
		#region Get HTML
		/// <summary>
		/// Méthode pour la construction du code HTML pour l'affichage des indicateurs
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="vehicleList">Liste des média</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <returns>Code HTML</returns>
        internal static string GetHtml(IDataSource source, string vehicleList, DateTime dateBegin, DateTime dateEnd, int siteLanguageId, int dataLanguageId) {

			#region Variables
			StringBuilder t = new StringBuilder();
            CultureInfo cultureInfo = WebApplicationParameters.AllowedLanguages[siteLanguageId].CultureInfo;
			#endregion

			#region Get Datas
			DataSet ds = IndicatorsDataAccess.GetDatas(source,vehicleList,dateBegin,dateEnd,dataLanguageId);
			if(ds==null || ds.Tables[0].Rows.Count==0){
                t.Append("<p align=center class=txtRouge12Bold>" + GestionWeb.GetWebWord(53, siteLanguageId) + " " + FrkDate.DateString.YYYYMMDDToDD_MM_YYYY(dateBegin.ToString("yyyyMMdd"), siteLanguageId) + " " + GestionWeb.GetWebWord(54, siteLanguageId) + " " + FrkDate.DateString.YYYYMMDDToDD_MM_YYYY(dateEnd.ToString("yyyyMMdd"), siteLanguageId) + "</p>");
				return(t.ToString());
			}
			#endregion

			#region Stockage des données dans un object[,]
			object[,] tab = IndicatorsRules.GetRules(ds.Tables[0],dateBegin,dateEnd);
			#endregion

			#region Construction du code HTML

			#region Variables
			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = 0;
			//A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = Cst.FIRST_PEDIOD_COlUMN_INDEX+nbColYear;
			//fin MAJ
			
			int nbColTab=tab.GetLength(1),j,i;
			int nbline=tab.GetLength(0);
			//MAJ GR : FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
			int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
			//fin MAJ
			int nbCharTotal=1;
			string diffusion="";
			string classe="L3";
			#endregion

			#region Début du tableau
			t.Append("<table id=\"calendartable\" border=1 cellpadding=0 cellspacing=0>\r\n\t<tr>");
			t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"pt\">"+GestionWeb.GetWebWord(56, siteLanguageId).ToUpper()+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");

			// MAJ GR : On affiche les années si nécessaire
			for(k=Cst.FIRST_PEDIOD_COlUMN_INDEX; k<FIRST_PERIOD_INDEX; k++){
				t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">"+tab[0,k].ToString()+"</td>");
				for(int h=0;h<nbCharTotal+5;h++){
					t.Append("&nbsp;");
				}
				t.Append("</td>");
			}
			//fin MAJ
			#endregion

			#region Période
			string dateHtml="<tr>";
			string headerHtml="";
			DateTime currentDay = (DateTime)tab[0,FIRST_PERIOD_INDEX];
			int previousMonth = currentDay.Month;
			currentDay = currentDay.AddDays(-1);
			int nbPeriodInMonth=0;
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				currentDay = currentDay.AddDays(1);
				if (currentDay.Month!=previousMonth){
					if (nbPeriodInMonth>=8){
						headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
                            + cultureInfo.TextInfo.ToTitleCase(currentDay.AddDays(-1).ToString("MMMM yyyy", cultureInfo)) //Dates.GetPeriodTxt(currentDay.AddDays(-1).ToString("yyyyMM"))
							+"</td>";
					}
					else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
							 +"&nbsp"
							 +"</td>";
					nbPeriodInMonth=0;
					previousMonth = currentDay.Month;
				}
				nbPeriodInMonth++;
				dateHtml+="<td class=\"pp\">&nbsp;"+currentDay.ToString("dd") +"&nbsp;</td>";
			}
			if (nbPeriodInMonth>=8)headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
                                       + cultureInfo.TextInfo.ToTitleCase(currentDay.ToString("MMMM yyyy", cultureInfo))     //Dates.GetPeriodTxt(currentDay.ToString("yyyyMM"))
									   +"</td>";
			else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
					 +"&nbsp"
					 +"</td>";
			dateHtml += "</tr>";

			string dayClass="";
			string day;
			dateHtml+="\r\n\t<tr>";
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
                DateTime dateTime = ((DateTime)tab[0, j]);
                DayOfWeek dayOfWeek = dateTime.DayOfWeek;
                day = TNS.FrameWork.Date.DayString.GetCharacters(dateTime, WebApplicationParameters.AllowedLanguages[siteLanguageId].CultureInfo, 1);
                if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
					dayClass="pdw";
				else
					dayClass="pd";
                dateHtml += "<td class=\"" + dayClass + "\">" + day + "</td>";
			}
			dateHtml+="\r\n\t</tr>";

			headerHtml += "</tr>";
			t.Append(headerHtml);
			t.Append(dateHtml);
			#endregion

			#region Calendrier
			i=0;
			string currentMedia="";
			try{		
				for(i=1;i<nbline;i++){
					for(j=0;j<nbColTab;j++){
						switch(j){
							case Cst.VEHICLE_COLUMN_INDEX: // MEDIA
								if(tab[i,j]!=null){
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L1\">"+tab[i,Cst.VEHICLE_COLUMN_INDEX]+"</td>");
								}
								break;

							case Cst.CATEGORY_COLUMN_INDEX: // CATEGORIE
								if(tab[i,j]!=null){
									t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2\">&nbsp;"+tab[i,Cst.CATEGORY_COLUMN_INDEX]+"</td>");
								}
								break;

							case Cst.ID_DIFFUSION_COLUMN_INDEX: // DIFFUSION
								if(tab[i,Cst.ID_DIFFUSION_COLUMN_INDEX]!=null && tab[i,Cst.ID_DIFFUSION_COLUMN_INDEX]!=DBNull.Value && tab[i,Cst.ID_DIFFUSION_COLUMN_INDEX].ToString().Length>0){
									switch(int.Parse(tab[i,Cst.ID_DIFFUSION_COLUMN_INDEX].ToString())){
										case 1:
											diffusion=" (6h-9h)";
											classe="L3Diffusion";
											break;
										case 2:
											diffusion="";//" (9h-24h)";
											classe="L3";
											break;
										default:
											classe="L3";
											break;
									}
								}
								else
									diffusion="";
								break;
							
							case Cst.MEDIA_COLUMN_INDEX: // SUPPORT
								if(tab[i,j]!=null){
									t.Append("\r\n\t<tr>\r\n\t\t<td class=\""+classe+"\">&nbsp;&nbsp;"+tab[i,Cst.MEDIA_COLUMN_INDEX]+diffusion+"</td>");
									currentMedia=tab[i,Cst.MEDIA_COLUMN_INDEX]+diffusion;
									diffusion="";
								}
								break;

							default: // CELLULE
								if(j>=Cst.FIRST_PEDIOD_COlUMN_INDEX){
									if(tab[i,j]==null){
										t.Append("<td class=pn>&nbsp;</td>");
									}
									else if(Int64.Parse(tab[i,j].ToString())==0){
										t.Append("<td class=pr title=\""+( (DateTime)tab[0,j]).ToString("dd/MM/yyyy")+"-"+currentMedia+"\">&nbsp;</td>");
									}
									else if(Int64.Parse(tab[i,j].ToString())>0){
										t.Append("<td class=pg title=\""+( (DateTime)tab[0,j]).ToString("dd/MM/yyyy")+"-"+currentMedia+"\">&nbsp;</td>");
									}
								}
								break;

						}
					}
					t.Append("</tr>");
				}
			}
			catch(System.Exception err){
				// Exception à changer !!! là c juste pour un test
				throw(new Exceptions.IndicatorsDataAccessException("erreur i="+i+",j="+j,err));
			}			
			t.Append("</table>");
			#endregion

			#endregion

			// On vide le tableau
			tab=null;

			return(t.ToString());
		}
		#endregion

		#region Get Excel
		/// <summary>
		/// Méthode pour la construction du code HTML pour l'affichage des indicateurs
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="vehicleList">Liste des média</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <returns>Code HTML</returns>
        internal static string GetExcel(IDataSource source, string vehicleList, DateTime dateBegin, DateTime dateEnd, int siteLanguageId, int dataLanguageId) {

			#region Variables
			StringBuilder t = new StringBuilder();
            CultureInfo cultureInfo = WebApplicationParameters.AllowedLanguages[siteLanguageId].CultureInfo;
			#endregion

			#region Get Datas
            DataSet ds = IndicatorsDataAccess.GetDatas(source, vehicleList, dateBegin, dateEnd, dataLanguageId);
			if(ds==null || ds.Tables[0].Rows.Count==0){
                t.Append(GestionWeb.GetWebWord(53, siteLanguageId) + " " + FrkDate.DateString.YYYYMMDDToDD_MM_YYYY(dateBegin.ToString("yyyyMMdd"), siteLanguageId) + " " + GestionWeb.GetWebWord(53, siteLanguageId) + " " + FrkDate.DateString.YYYYMMDDToDD_MM_YYYY(dateEnd.ToString("yyyyMMdd"), siteLanguageId));
				return(t.ToString());
			}
			#endregion

			#region Stockage des données dans un object[,]
			object[,] tab = IndicatorsRules.GetRules(ds.Tables[0],dateBegin,dateEnd);
			#endregion

			#region Construction du code HTML

			#region Variables
			//MAJ GR : Colonnes totaux par année si nécessaire
			int k = 0;
			int nbColYear = 0;
			//A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
			if (nbColYear>0) nbColYear++;
			int FIRST_PERIOD_INDEX = Cst.FIRST_PEDIOD_COlUMN_INDEX+nbColYear;
			//fin MAJ
			
			int nbColTab=tab.GetLength(1),j,i;
			int nbline=tab.GetLength(0);
			//MAJ GR : FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
			int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
			//fin MAJ
			int nbCharTotal=1;
			string diffusion="";
			string classe="L3";
			#endregion

			#region Rappel de sélection
//			t.Append("<table style=\"border:solid 1px #808080;\" cellpadding=0 cellspacing=0>");
//			t.Append("<tr><td class=\"excelDataItalic\">Paramètres du tableau :</td></tr>");
//			// Module
//			t.Append("<tr><td class=\"excelData\"><font class=txtBoldGrisExcel>Module :</font> Indicateurs</td></tr>");
//			// Date
//			t.Append("<tr><td class=\"excelData\"><font class=txtBoldGrisExcel>Période sélectionnée :</font> Du "+DateString.YYYYMMDDToDD_MM_YYYY(dateBegin,33)+" au "+DateString.YYYYMMDDToDD_MM_YYYY(dateEnd,33)+"</td></tr>");
//			t.Append("</table>");
//			t.Append("<br>");
			#endregion

			#region Début du tableau
			t.Append("<table id=\"calendartable\" border=1 cellpadding=0 cellspacing=0>\r\n\t<tr>");
			t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"ptX\">SUPPORTS&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");

			// MAJ GR : On affiche les années si nécessaire
			for(k=Cst.FIRST_PEDIOD_COlUMN_INDEX; k<FIRST_PERIOD_INDEX; k++){
				t.Append("\r\n\t\t<td rowspan=\"3\" class=\"ptX\">"+tab[0,k].ToString()+"</td>");
				for(int h=0;h<nbCharTotal+5;h++){
					t.Append("&nbsp;");
				}
				t.Append("</td>");
			}
			//fin MAJ
			#endregion

			#region Période
			string dateHtml="<tr>";
			string headerHtml="";
			DateTime currentDay = (DateTime)tab[0,FIRST_PERIOD_INDEX];
			int previousMonth = currentDay.Month;
			currentDay = currentDay.AddDays(-1);
			int nbPeriodInMonth=0;
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
				currentDay = currentDay.AddDays(1);
				if (currentDay.Month!=previousMonth){
					if (nbPeriodInMonth>=8){
						headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"ptX\" align=center nowrap>"
                            + cultureInfo.TextInfo.ToTitleCase(currentDay.AddDays(-1).ToString("MMMM yyyy", cultureInfo))     //Dates.GetPeriodTxt(currentDay.AddDays(-1).ToString("yyyyMM"))
							+"</td>";
					}
					else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"ptX\" align=center nowrap>"
							 +"&nbsp"
							 +"</td>";
					nbPeriodInMonth=0;
					previousMonth = currentDay.Month;
				}
				nbPeriodInMonth++;
				dateHtml+="<td class=\"ppX\" nowrap>&nbsp;"+currentDay.ToString("dd") +"&nbsp;</td>";
			}
			if (nbPeriodInMonth>=8)headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"ptX\" align=center nowrap>"
                                       + cultureInfo.TextInfo.ToTitleCase(currentDay.ToString("MMMM yyyy", cultureInfo))     //Dates.GetPeriodTxt(currentDay.ToString("yyyyMM"))
									   +"</td>";
			else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"ptX\" align=center nowrap>"
					 +"&nbsp"
					 +"</td>";
			dateHtml += "</tr>";

			string dayClass="";
			string day;
			dateHtml+="\r\n\t<tr>";
			for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
                DateTime dateTime = ((DateTime)tab[0, j]);
                DayOfWeek dayOfWeek = dateTime.DayOfWeek;
                day = TNS.FrameWork.Date.DayString.GetCharacters(dateTime, WebApplicationParameters.AllowedLanguages[siteLanguageId].CultureInfo, 1);
                if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
					dayClass="pdwX";
				else
					dayClass="pdX";
                dateHtml += "<td nowrap class=\"" + dayClass + "\">" + day + "</td>";
			}
			dateHtml+="\r\n\t</tr>";

			headerHtml += "</tr>";
			t.Append(headerHtml);
			t.Append(dateHtml);
			#endregion

			#region Calendrier
			i=0;
			string currentMedia="";
			try{		
				for(i=1;i<nbline;i++){
					for(j=0;j<nbColTab;j++){
						switch(j){
							case Cst.VEHICLE_COLUMN_INDEX: // MEDIA
								if(tab[i,j]!=null){
									if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
										i=int.MaxValue-2;
										j=int.MaxValue-2;
										break;
									}
									t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L1\">"+tab[i,Cst.VEHICLE_COLUMN_INDEX]+"</td>");
								}
								break;

							case Cst.CATEGORY_COLUMN_INDEX: // CATEGORIE
								if(tab[i,j]!=null){
									t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2\">&nbsp;"+tab[i,Cst.CATEGORY_COLUMN_INDEX]+"</td>");
								}
								break;

							case Cst.ID_DIFFUSION_COLUMN_INDEX: // DIFFUSION
								if(tab[i,Cst.ID_DIFFUSION_COLUMN_INDEX]!=null && tab[i,Cst.ID_DIFFUSION_COLUMN_INDEX]!=DBNull.Value && tab[i,Cst.ID_DIFFUSION_COLUMN_INDEX].ToString().Length>0){
									switch(int.Parse(tab[i,Cst.ID_DIFFUSION_COLUMN_INDEX].ToString())){
										case 1:
											diffusion=" (6h-9h)";
											classe="L3DiffusionX";
											break;
										case 2:
											diffusion="";//" (9h-24h)";
											classe="L3";
											break;
										default:
											classe="L3";
											break;
									}
								}
								else
									diffusion="";
								break;
							
							case Cst.MEDIA_COLUMN_INDEX: // SUPPORT
								if(tab[i,j]!=null){
									t.Append("\r\n\t<tr>\r\n\t\t<td class=\""+classe+"\">&nbsp;&nbsp;"+tab[i,Cst.MEDIA_COLUMN_INDEX]+diffusion+"</td>");
									currentMedia=tab[i,Cst.MEDIA_COLUMN_INDEX]+diffusion;
									diffusion="";
								}
								break;

							default: // CELLULE
								if(j>=Cst.FIRST_PEDIOD_COlUMN_INDEX){
									if(tab[i,j]==null){
										t.Append("<td class=pnX>&nbsp;</td>");
									}
									else if(Int64.Parse(tab[i,j].ToString())==0){
										t.Append("<td class=prX >&nbsp;</td>");
									}
									else if(Int64.Parse(tab[i,j].ToString())>0){
										t.Append("<td class=pgX >&nbsp;</td>");
									}
								}
								break;

						}
					}
					t.Append("</tr>");
				}
			}
			catch(System.Exception err){
				// Exception à changer !!! là c juste pour un test
				throw(new Exceptions.IndicatorsDataAccessException("erreur i="+i+",j="+j,err));
			}			
			t.Append("</table>");
			#endregion

			#endregion

			// On vide le tableau
			tab=null;

			return(t.ToString());
		}
		#endregion

	}
}
