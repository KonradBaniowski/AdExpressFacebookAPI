#region Informations
// Auteur: G. Facon 
// Date de création: 06/04/2006 à partir de MediaPlanAlertUI pour l'adaptation des plans media aux nouveaux niveaux de détailles
// Date de modification:
#endregion

using System.Web;
using System;
using System.Collections;
using System.Web.UI;
using System.Globalization;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork;
using TNS.FrameWork.Date;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ExcelFunction = TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Common.Results;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
//using TNS.AdExpress.Domain.Results;


namespace TNS.AdExpress.Web.UI.Results{
    /// <summary>
    /// Construit l'interface utilisateur pour les alertes plan media ou les zooms plan media
    /// </summary>
    public class GenericMediaPlanAlertUI{

        #region Sortie HTML (Web)

        #region Plan Media AdNetTrack
        /// <summary>
        /// Génère le code HTML pour afficher un calendrier d'action AdNetTrack détaillé en jour entre les dates de la session.
        /// Elles se base sur un tableau contenant les données
        /// tab vide : message "Pas de données" + bouton retour
        /// tab non vide:
        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
        ///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
        ///		Génération du code HTML des entêtes de colonne
        ///		Génération du code HTML des périodes (Libellé du mois, intitulés d'une unité de période)
        ///		Génération du code HTML du calendrier d'action
        /// </summary>
        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
        /// <param name="webSession">Session du client</param>
        /// <returns>Code Généré</returns>
        /// <remarks>
        /// Utilise les méthodes:
        ///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
        ///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
        /// </remarks>
        public static MediaPlanResultData GetAdNetTrackMediaPlanAlertWithMediaDetailLevelHtmlUI(object[,] tab, WebSession webSession)
        {

            #region Pas de données à afficher
            MediaPlanResultData mediaPlanResultData = new MediaPlanResultData();
            if (tab.GetLength(0) == 0)
            {
                mediaPlanResultData.HTMLCode = "<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage)
                    + "<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/" + webSession.SiteLanguage + "/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/" + webSession.SiteLanguage + "/button/back_up.gif';\">"
                    + "<img src=\"/Images/" + webSession.SiteLanguage + "/button/back_up.gif\" border=0 name=bouton></a></div>";
                return (mediaPlanResultData);
            }
            #endregion

            #region Variables
            //MAJ GR : Colonnes totaux par année si nécessaire
            int k = 0;
            //int nbColYear = 0;
            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. 
            int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0, 4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0, 4));
            if (nbColYear > 0) nbColYear++;
            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX + nbColYear;
            //fin MAJ

            string classe;
            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
            //affichée, utilisez getLength-1
            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            int nbColTab = tab.GetLength(1), j, i;
            int nbline = tab.GetLength(0);
            //MAJ GR : FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
            int nbPeriod = nbColTab - FIRST_PERIOD_INDEX - 1;
            //fin MAJ
            string currentCategoryName = "tit";
            int colorItemIndex = 1;
            int colorNumberToUse = 0;
            int sloganIndex = -1;
            //webSession.SloganColors.Clear();
            try { webSession.SloganColors.Add((Int64)0, "pc0"); }
            catch (System.Exception) { }
            string presentClass = "p4";
            string extendedClass = "p5";
            string stringItem = "&nbsp;";

            #endregion


            #region Début du tableau
            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
            t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"pt\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            #endregion

            #region Période
            string dateHtml = "<tr>";
            string headerHtml = "";
            //DateTime currentDay = new DateTime(((DateTime)tab[0,FIRST_PERIOD_INDEX]).Ticks);
            DateTime currentDay = DateString.YYYYMMDDToDateTime((string)tab[0, FIRST_PERIOD_INDEX]);
            //new DateTime(int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4)),int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(4,2)),int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(6,2)));
            int previousMonth = currentDay.Month;
            currentDay = currentDay.AddDays(-1);
            int nbPeriodInMonth = 0;
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                currentDay = currentDay.AddDays(1);
                if (currentDay.Month != previousMonth)
                {
                    if (nbPeriodInMonth >= 8)
                    {
                        headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                            + Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM"))
                            + "</td>";
                    }
                    else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                             + "&nbsp"
                             + "</td>";
                    nbPeriodInMonth = 0;
                    previousMonth = currentDay.Month;
                }
                nbPeriodInMonth++;
                dateHtml += "<td class=\"pp\">&nbsp;" + currentDay.ToString("dd") + "&nbsp;</td>";
            }
            if (nbPeriodInMonth >= 8) headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                                          + Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM"))
                                          + "</td>";
            else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                     + "&nbsp"
                     + "</td>";
            dateHtml += "</tr>";

            string dayClass = "";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
            dateHtml += "\r\n\t<tr>";
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                DateTime dt = (new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2))));
                if (dt.DayOfWeek == DayOfWeek.Saturday
                    || dt.DayOfWeek == DayOfWeek.Sunday
                    ) {
                    dayClass = "pdw";
                }
                else
                {
                    dayClass = "pd";
                }
                dateHtml += "<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>";
            }
            dateHtml += "\r\n\t</tr>";

            headerHtml += "</tr>";
            t.Append(headerHtml);
            t.Append(dateHtml);
            #endregion

            #region Calendrier d'actions
            i = 0;
            try
            {
                //				sloganIndex=GetSloganIdIndex(webSession);
                for (i = 1; i < nbline; i++)
                {

                    #region Gestion des couleurs des accroches
                    //					if(sloganIndex!=-1	&& tab[i,sloganIndex]!=null && 
                    //						((webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan)==webSession.GenericMediaDetailLevel.GetNbLevels)||
                    //						(webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan)<webSession.GenericMediaDetailLevel.GetNbLevels && tab[i,sloganIndex+1]==null))){
                    //						if(!webSession.SloganColors.ContainsKey((Int64)tab[i,sloganIndex])){
                    //							colorNumberToUse=(colorItemIndex%30)+1;
                    //							webSession.SloganColors.Add((Int64)tab[i,sloganIndex],"pc"+colorNumberToUse.ToString());
                    //							mediaPlanResultData.VersionsDetail.Add((Int64)tab[i,sloganIndex],new VersionItem((Int64)tab[i,sloganIndex],"pc"+colorNumberToUse.ToString())); //TODO
                    //							colorItemIndex++;
                    //						}
                    //						if((Int64)tab[i,sloganIndex]!=0 && !mediaPlanResultData.VersionsDetail.ContainsKey((Int64)tab[i,sloganIndex])){
                    //							mediaPlanResultData.VersionsDetail.Add((Int64)tab[i,sloganIndex],new VersionItem((Int64)tab[i,sloganIndex],webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString()));
                    //						}
                    //						presentClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
                    //						extendedClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
                    //						stringItem="x";
                    //					}
                    //					else{
                    //						presentClass="p4";
                    //						extendedClass="p5";
                    //						stringItem="&nbsp;";
                    //					}
                    #endregion

                    for (j = 0; j < nbColTab; j++)
                    {
                        switch (j)
                        {
                            #region Level 1
                            case DetailledMediaPlan.L1_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    //vérification que la catégorie n est pas les chaines thematiques

                                    if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) classe = "L0";
                                    else
                                    {
                                        classe = "L1";
                                        tmpHtml = "";
                                    }
                                    if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
                                    {
                                        i = int.MaxValue - 2;
                                        j = int.MaxValue - 2;
                                        break;
                                    }
                                    // On traite le cas des pages
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"" + classe + "\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX] + "</td>");
                                    j = j + 10;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 2
                            case DetailledMediaPlan.L2_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2\">&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX] + "</td>");
                                    currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX].ToString();
                                    j = j + 9;
                                }
                                break;
                            #endregion

                            #region Level 3
                            case DetailledMediaPlan.L3_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3\">&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX] + "</td>");
                                    currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX].ToString();
                                    j = j + 8;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 4
                            case DetailledMediaPlan.L4_COLUMN_INDEX:
                                t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td>");
                                j = j + 7;//nbColYear ajouter GF
                                break;
                            #endregion
                            default:
                                if (tab[i, j] == null)
                                {
                                    t.Append("<td class=\"p3\">&nbsp;</td>");
                                    break;
                                }
                                if (tab[i, j].GetType() == typeof(MediaPlanItem))
                                {
                                    switch (((MediaPlanItem)tab[i, j]).GraphicItemType)
                                    {
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
                                            t.Append("<td class=\"" + presentClass + "\">" + stringItem + "</td>");
                                            break;
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
                                            t.Append("<td class=\"" + extendedClass + "\">&nbsp;</td>");
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
            catch (System.Exception err)
            {
                throw (new WebExceptions.MediaPlanUIException("erreur i=" + i + ",j=" + j, err));
            }
            //t.Append("</table></td></tr></table>");			
            t.Append("</table>");
            #endregion

            // On vide le tableau
            tab = null;
            mediaPlanResultData.HTMLCode = t.ToString();
            return (mediaPlanResultData);

        }
        #endregion

        #region Plan Media AdNetTrack  zoom (Avec détail support)
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
        /// <param name="urlParameters">Paramètres d'Url</param>
        /// <param name="moduleId">L'identificateur du module</param>
        /// <param name="universId">l'identificateur de l'univers</param>
        /// <param name="currentPage">la page courante (pagination)</param>
        /// <returns>Code Généré</returns>
        /// <remarks>
        /// Utilise les méthodes:
        ///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
        ///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
        /// </remarks>
        public static MediaPlanResultData GetAdNetTrackMediaPlanAlertWithMediaDetailLevelHtmlUI(Page page, object[,] tab, WebSession webSession, string zoomDate, string url, string urlParameters, Int64 moduleId, int universId, string currentPage){

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            MediaPlanResultData mediaPlanResultData = new MediaPlanResultData();

            #region Pas de données à afficher
            if (tab.GetLength(0) == 0){

                t.Append("<table align=\"center\"><tr>");

                #region Période précédente
                if (webSession.PeriodBeginningDate != zoomDate){
                    if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
                        t.Append("\r\n\t\t\t\t\t\t<td><a id=\"periodPrevImageButton\" onmouseover=\"periodPrevImageButton_img.src='/Images/Common/period_prev_down.gif'\" "
                            + "onmouseout=\"periodPrevImageButton_img.src='/Images/Common/period_prev_up.gif'\" "
                            + "href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=" +
                            (new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1)).AddMonths(-1).ToString("yyyyMM") + "&id=" + webSession.AdNetTrackSelection.Id + "&idLevel=" + webSession.AdNetTrackSelection.SelectionType.GetHashCode() + "&urlParameters=" + urlParameters + "&universId=" + universId + "&moduleId=" + moduleId + "&page=" + currentPage
                            + "\"><IMG border=0 alt=\"\" name=\"periodPrevImageButton_img\" src=\"/Images/Common/period_prev_up.gif\"></a></td><td>&nbsp;</td>");
                    }
                    else{
                        t.Append("\r\n\t\t\t\t\t\t<td><a  id=\"periodPrevImageButton\" onmouseover=\"periodPrevImageButton_img.src='/Images/Common/period_prev_down.gif'\" "
                            + "onmouseout=\"periodPrevImageButton_img.src='/Images/Common/period_prev_up.gif'\" "
                            + "href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=");
                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
                        tmp.SubWeek(1);
                        if (tmp.Week.ToString().Length < 2) t.Append(tmp.Year.ToString() + "0" + tmp.Week.ToString());
                        else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
                        t.Append("&id=" + webSession.AdNetTrackSelection.Id + "&idLevel=" + webSession.AdNetTrackSelection.SelectionType.GetHashCode() + "&urlParameters=" + urlParameters + "&universId=" + universId + "&moduleId=" + moduleId + "&page=" + currentPage + "\"><IMG border=0 alt=\"\" name=\"periodPrevImageButton_img\" src=\"/Images/Common/period_prev_up.gif\"></a></td><td>&nbsp;</td>");
                    }
                }
                else
                    t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td>");
                #endregion

                t.Append("<td class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage));
                if (zoomDate != null && zoomDate.Length > 0){
                    t.Append("&nbsp;" + GestionWeb.GetWebWord(896, webSession.SiteLanguage) + " " + WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(zoomDate, webSession.PeriodType), webSession.SiteLanguage)
                        + " " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + " " + WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(zoomDate, webSession.PeriodType), webSession.SiteLanguage) + "&nbsp;");
                }

                t.Append("</td>");

                #region Période suivante
                if (webSession.PeriodEndDate != zoomDate){
                    if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
                        t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td><td><a id=\"periodNextImageButton\" onmouseover=\"periodNextImageButton_img.src='/Images/Common/period_next_down.gif'\" "
                            + "onmouseout=\"periodNextImageButton_img.src='/Images/Common/period_next_up.gif'\" "
                            + "href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=" +
                            (new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1)).AddMonths(1).ToString("yyyyMM") + "&id=" + webSession.AdNetTrackSelection.Id + "&idLevel=" + webSession.AdNetTrackSelection.SelectionType.GetHashCode() + "&urlParameters=" + urlParameters + "&universId=" + universId + "&moduleId=" + moduleId + "&page=" + currentPage
                            + "\"><IMG border=0 alt=\"\" name=\"periodNextImageButton_img\" src=\"/Images/Common/period_next_up.gif\"></a></td>");
                    }
                    else{
                        t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td><td><a  id=\"periodNextImageButton\" onmouseover=\"periodNextImageButton_img.src='/Images/Common/period_next_down.gif'\" "
                            + "onmouseout=\"periodNextImageButton_img.src='/Images/Common/period_next_up.gif'\" "
                            + "href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=");
                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
                        tmp.Increment();
                        if (tmp.Week.ToString().Length < 2) t.Append(tmp.Year.ToString() + "0" + tmp.Week.ToString());
                        else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
                        t.Append("&id=" + webSession.AdNetTrackSelection.Id + "&idLevel=" + webSession.AdNetTrackSelection.SelectionType.GetHashCode() + "&urlParameters=" + urlParameters + "&universId=" + universId + "&moduleId=" + moduleId + "&page=" + currentPage + "\"><IMG border=0 alt=\"\" name=\"periodNextImageButton_img\" src=\"/Images/Common/period_next_up.gif\"></a></td>");
                    }
                }
                else
                    t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td>");
                #endregion

                t.Append("</tr></table>");
                mediaPlanResultData.HTMLCode = t.ToString();
                return (mediaPlanResultData);
            }
            #endregion

            #region Variables

            //MAJ GR : Colonnes totaux par année si nécessaire
            int k = 0;
            int nbColYear = 0;
            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
            if (webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
            {
                string periodBeginning = TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(zoomDate, webSession.PeriodType).ToString("yyyyMMdd");
                string periodEnd = TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(zoomDate, webSession.PeriodType).ToString("yyyyMMdd");
                int currentDate = int.Parse(periodBeginning.Substring(0, 4));
                int oldCurrentDate = int.Parse(periodEnd.Substring(0, 4));
                nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0, 4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0, 4));
                if ((nbColYear > 0) && ((currentDate != oldCurrentDate))) nbColYear++;
                else nbColYear = 0;
            }
            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX + nbColYear;
            //fin MAJ

            string classe;
            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
            //affichée, utilisez getLength-1
            int nbColTab = tab.GetLength(1), j, i;
            int nbline = tab.GetLength(0);
            //MAJ GR : FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
            int nbPeriod = nbColTab - FIRST_PERIOD_INDEX - 1;
            //fin MAJ
            string currentCategoryName = "tit";
            //string classCss="";


            #endregion

            #region On calcule la taille de la colonne Total
            int nbtot = WebFunctions.Units.ConvertUnitValueToString(tab[1, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit).Length;
            int nbSpace = (nbtot - 1) / 3;
            int nbCharTotal = nbtot + nbSpace - 5;
            if (nbCharTotal < 5) nbCharTotal = 0;
            #endregion


            #region debut Tableau
            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
            t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"pt\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            for (int h = 0; h < nbCharTotal; h++){
                t.Append("&nbsp;");
            }
            t.Append("</td>");


            // MAJ GR : On affiche les années si nécessaire
            for (k = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++){
                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + tab[0, k].ToString() + "</td>");
                for (int h = 0; h < nbCharTotal + 5; h++){
                    t.Append("&nbsp;");
                }
                t.Append("</td>");
            }
            //fin MAJ

            #endregion

            #region Période précédente
            if (webSession.PeriodBeginningDate != zoomDate){
                if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pyLeft\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=" +
                        (new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1)).AddMonths(-1).ToString("yyyyMM") + "&id=" + webSession.AdNetTrackSelection.Id + "&idLevel=" + webSession.AdNetTrackSelection.SelectionType.GetHashCode() + "&urlParameters=" + urlParameters + "&universId=" + universId + "&moduleId=" + moduleId + "&page=" + currentPage
                        + "\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
                }
                else{
                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pyLeft\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=");
                    AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
                    tmp.SubWeek(1);
                    if (tmp.Week.ToString().Length < 2) t.Append(tmp.Year.ToString() + "0" + tmp.Week.ToString());
                    else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
                    t.Append("&id=" + webSession.AdNetTrackSelection.Id + "&idLevel=" + webSession.AdNetTrackSelection.SelectionType.GetHashCode() + "&urlParameters=" + urlParameters + "&universId=" + universId + "&moduleId=" + moduleId + "&page=" + currentPage + "\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
                }
            }
            else
                t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\">&nbsp;</td>");
            #endregion

            #region Entête de période
            t.Append("\r\n\t\t\t\t\t\t<td colspan=\"" + (nbColTab - FIRST_PERIOD_INDEX - 2) + "\" class=\"pa\" align=center>"
                + Dates.getPeriodTxt(webSession, zoomDate)
                + "</td>");
            #endregion

            #region Période suivante
            if (webSession.PeriodEndDate != zoomDate){
                if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=" +
                        (new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1)).AddMonths(1).ToString("yyyyMM") + "&id=" + webSession.AdNetTrackSelection.Id + "&idLevel=" + webSession.AdNetTrackSelection.SelectionType.GetHashCode() + "&urlParameters=" + urlParameters + "&universId=" + universId + "&moduleId=" + moduleId + "&page=" + currentPage
                        + "\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
                }
                else{
                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=");
                    AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
                    tmp.Increment();
                    if (tmp.Week.ToString().Length < 2) t.Append(tmp.Year.ToString() + "0" + tmp.Week.ToString());
                    else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
                    t.Append("&id=" + webSession.AdNetTrackSelection.Id + "&idLevel=" + webSession.AdNetTrackSelection.SelectionType.GetHashCode() + "&urlParameters=" + urlParameters + "&universId=" + universId + "&moduleId=" + moduleId + "&page=" + currentPage + "\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
                }
            }
            else
                t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\">&nbsp;</td>");
            t.Append("</tr><tr>");
            #endregion

            #region Périodes
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {

                t.Append("<td class=\"pp\">&nbsp;" + ((string)tab[0, j]).Substring(6, 2) + "&nbsp;</td>");
            }

            string dayClass = "";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
            t.Append("\r\n\t</tr><tr>");
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                DateTime dt = (DateString.YYYYMMDDToDateTime((string)tab[0, j]));
                if (dt.DayOfWeek == DayOfWeek.Saturday
                    || dt.DayOfWeek == DayOfWeek.Sunday
                    ) {
                    dayClass = "pdw";
                }
                else
                {
                    dayClass = "pd";
                }
                t.Append("<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>");
            }
            t.Append("\r\n\t</tr>");
            #endregion

            #region Calendrier d'action
            i = 0;
            try
            {
                for (i = 1; i < nbline; i++)
                {
                    for (j = 0; j < nbColTab; j++)
                    {
                        switch (j)
                        {

                            #region Level 1
                            case DetailledMediaPlan.L1_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) classe = "L0";
                                    else
                                    {
                                        classe = "L1";
                                        tmpHtml = "";
                                    }
                                    if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
                                    {
                                        i = int.MaxValue - 2;
                                        j = int.MaxValue - 2;
                                        break;
                                    }

                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"" + classe + "\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX] + "</td>");
                                    j = j + 10 + nbColYear;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 2
                            case DetailledMediaPlan.L2_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX] + "</td>");
                                    currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX].ToString();
                                    j = j + 9 + nbColYear;
                                }
                                break;
                            #endregion

                            #region Level 3
                            case DetailledMediaPlan.L3_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX] + "</td>");
                                    j = j + 8;
                                }
                                break;
                            #endregion

                            #region Level 4
                            case DetailledMediaPlan.L4_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td>");
                                    //aller aux colonnes du calendrier d'action
                                    j = j + 7;
                                }
                                break;
                            #endregion
                            default:
                                if (tab[i, j] == null)
                                {
                                    t.Append("<td class=\"p3\">&nbsp;</td>");
                                    break;
                                }
                                if (tab[i, j].GetType() == typeof(MediaPlanItem))
                                {
                                    switch (((MediaPlanItem)tab[i, j]).GraphicItemType)
                                    {
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
                                            t.Append("<td class=\"p4\">&nbsp;</td>");
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
                    t.Append("</tr>");
                }
            }
            catch (System.Exception e)
            {
                throw (new System.Exception("erreur i=" + i + ",j=" + j, e));
            }
            t.Append("</table>");

            #endregion

            // On vide le tableau
            tab = null;

            mediaPlanResultData.HTMLCode = t.ToString();
            return (mediaPlanResultData);

        }
        #endregion

        #region détail support date de la session
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
        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
        /// <param name="webSession">Session du client</param>
        /// <returns>Code Généré</returns>
        /// <remarks>
        /// Utilise les méthodes:
        ///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
        ///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
        /// </remarks>
        public static MediaPlanResultData GetMediaPlanAlertWithMediaDetailLevelHtmlUI(object[,] tab, WebSession webSession){


                #region Pas de données à afficher
                MediaPlanResultData mediaPlanResultData = new MediaPlanResultData();
                if (tab.GetLength(0) == 0)
                {
                    mediaPlanResultData.HTMLCode = "<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage)
                        + "<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/" + webSession.SiteLanguage + "/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/" + webSession.SiteLanguage + "/button/back_up.gif';\">"
                        + "<img src=\"/Images/" + webSession.SiteLanguage + "/button/back_up.gif\" border=0 name=bouton></a></div>";
                    return (mediaPlanResultData);
                }
                #endregion

                #region Variables
                //MAJ GR : Colonnes totaux par année si nécessaire
                int k = 0;
                //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. 
                int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0, 4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0, 4));
                if (nbColYear > 0) nbColYear++;
                int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX + nbColYear;
                //fin MAJ

                string classe;
                //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
                //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
                //affichée, utilisez getLength-1
                System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
                int nbColTab = tab.GetLength(1), j, i;
                int nbline = tab.GetLength(0);
                //MAJ GR : FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
                int nbPeriod = nbColTab - FIRST_PERIOD_INDEX - 1;
                //fin MAJ
                string currentCategoryName = "tit";
                string totalUnit = "";
                int colorItemIndex = 1;
                int colorNumberToUse = 0;
                int sloganIndex = -1;
                try { webSession.SloganColors.Add((Int64)0, "pc0"); }
                catch (System.Exception) { }
                string presentClass = "p4";
                string extendedClass = "p5";
                string stringItem = "&nbsp;";


                #endregion

                #region Module
                Int64 moduleId;
                WebConstantes.Module.Type moduleType = (ModulesList.GetModule(webSession.CurrentModule)).ModuleType;
                if (moduleType == WebConstantes.Module.Type.alert) moduleId = WebConstantes.Module.Name.ALERTE_PLAN_MEDIA;
                else moduleId = WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;
                #endregion

                #region On calcule la taille de la colonne Total
                int nbtot = WebFunctions.Units.ConvertUnitValueToString(tab[1, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit).Length;
                int nbSpace = (nbtot - 1) / 3;
                int nbCharTotal = nbtot + nbSpace - 5;
                if (nbCharTotal < 5) nbCharTotal = 0;
                #endregion

                #region Début du tableau
                t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
                t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"pt\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
                t.Append("\r\n\t\t<td rowspan=3 class=\"pt\">" + GestionWeb.GetWebWord(805, webSession.SiteLanguage));
                for (int h = 0; h < nbCharTotal; h++)
                {
                    t.Append("&nbsp;");
                }
                t.Append("</td>");
                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
                bool showVersions = false;
                if (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG))
                {
                    showVersions = true;
                    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + GestionWeb.GetWebWord(1994, webSession.SiteLanguage) + "</td>");// Versions
                }
                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + GestionWeb.GetWebWord(2245, webSession.SiteLanguage) + "</td>"); // Insertions
                // MAJ GR : On affiche les années si nécessaire
                for (k = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++)
                {
                    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + tab[0, k].ToString() + "</td>");
                }
                //fin MAJ
                #endregion

                #region Période
                string dateHtml = "<tr>";
                string headerHtml = "";
                DateTime currentDay = DateString.YYYYMMDDToDateTime((string)tab[0, FIRST_PERIOD_INDEX]);
                int previousMonth = currentDay.Month;
                currentDay = currentDay.AddDays(-1);
                int nbPeriodInMonth = 0;
                for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
                {
                    currentDay = currentDay.AddDays(1);
                    if (currentDay.Month != previousMonth)
                    {
                        if (nbPeriodInMonth >= 8)
                        {
                            headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                                + Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM"))
                                + "</td>";
                        }
                        else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                                 + "&nbsp"
                                 + "</td>";
                        nbPeriodInMonth = 0;
                        previousMonth = currentDay.Month;
                    }
                    nbPeriodInMonth++;
                    dateHtml += "<td class=\"pp\">&nbsp;" + currentDay.ToString("dd") + "&nbsp;</td>";
                }
                if (nbPeriodInMonth >= 8) headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                                              + Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM"))
                                              + "</td>";
                else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                         + "&nbsp"
                         + "</td>";
                dateHtml += "</tr>";

                string dayClass = "";
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
                dateHtml += "\r\n\t<tr>";
                for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
                {
                    DateTime dt = (new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2))));
                    if (dt.DayOfWeek == DayOfWeek.Saturday
                    || dt.DayOfWeek == DayOfWeek.Sunday
                    ) {
                        dayClass = "pdw";
                    }
                    else
                    {
                        dayClass = "pd";
                    }
                    dateHtml += "<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>";
                }
                dateHtml += "\r\n\t</tr>";

                headerHtml += "</tr>";
                t.Append(headerHtml);
                t.Append(dateHtml);
                #endregion

                #region Calendrier d'actions
                i = 0;
                try
                {
                    sloganIndex = GetSloganIdIndex(webSession);
                    for (i = 1; i < nbline; i++)
                    {

                        #region Gestion des couleurs des accroches
                        if (sloganIndex != -1 && tab[i, sloganIndex] != null &&
                            ((webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) == webSession.GenericMediaDetailLevel.GetNbLevels) ||
                            (webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) < webSession.GenericMediaDetailLevel.GetNbLevels && tab[i, sloganIndex + 1] == null)))
                        {
                            if (!webSession.SloganColors.ContainsKey((Int64)tab[i, sloganIndex]))
                            {
                                colorNumberToUse = (colorItemIndex % 30) + 1;
                                webSession.SloganColors.Add((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString());
                                mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new VersionItem((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString())); //TODO
                                colorItemIndex++;
                            }
                            if ((Int64)tab[i, sloganIndex] != 0 && !mediaPlanResultData.VersionsDetail.ContainsKey((Int64)tab[i, sloganIndex]))
                            {
                                mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new VersionItem((Int64)tab[i, sloganIndex], webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString()));
                            }
                            presentClass = webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString();
                            extendedClass = webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString();
                            stringItem = "x";
                        }
                        else
                        {
                            presentClass = "p4";
                            extendedClass = "p5";
                            stringItem = "&nbsp;";
                        }
                        #endregion

                        for (j = 0; j < nbColTab; j++)
                        {
                            switch (j)
                            {
                                #region Level 1
                                case DetailledMediaPlan.L1_COLUMN_INDEX:
                                    if (tab[i, j] != null)
                                    {
                                        string tmpHtml = "";
                                        string tmpHtml2 = "";
                                        //vérification que la catégorie n est pas les chaines thematiques
                                        if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) classe = "L0";
                                        else
                                        {
                                            classe = "L1";
                                            if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] != null)
                                            {
                                                tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + ",-1,-1,-1,-1','','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                                tmpHtml2 = "<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + ",-1,-1,-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            }
                                        }
                                        if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
                                        {
                                            i = int.MaxValue - 2;
                                            j = int.MaxValue - 2;
                                            break;
                                        }
                                        // On traite le cas des pages
                                        totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                        t.Append("\r\n\t<tr>\r\n\t\t<td class=\"" + classe + "\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((showVersions) ? "<td align=\"center\" class=\"" + classe + "\">" + tmpHtml + "</td>" : "") + "<td align=\"center\" class=\"" + classe + "\">" + tmpHtml2 + "</td>");
                                        //t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+Int64.Parse(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ###")+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
                                        //aller aux colonnes du calendrier d'action
                                        //MAJ GR : totaux par années si nécessaire
                                        for (k = 1; k <= nbColYear; k++)
                                        {
                                            t.Append("<td class=\"" + classe + "nb\" nowrap>" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 10 + k].ToString(), webSession.Unit) + "</td>");
                                        }
                                        j = j + 10 + nbColYear;
                                        //fin MAJ
                                    }
                                    break;
                                #endregion

                                #region Level 2
                                case DetailledMediaPlan.L2_COLUMN_INDEX:
                                    if (tab[i, j] != null)
                                    {
                                        string tmpHtml = "";
                                        string tmpHtml2 = "";
                                        //vérification que la catégorie n est pas les chaines thematiques
                                        //									if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+",-1,-1,-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] != null)
                                        {
                                            tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + ",-1,-1,-1','','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            tmpHtml2 = "<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + ",-1,-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        }
                                        // On traite le cas des pages
                                        totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                        t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2\">&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX] + "</td><td class=\"L2nb\">" + totalUnit + "</td><td class=\"L2nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((showVersions) ? "<td align=\"center\" class=\"L2\">" + tmpHtml + "</td>" : "") + "<td align=\"center\" class=\"L2\">" + tmpHtml2 + "</td>");
                                        currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX].ToString();
                                        //aller aux colonnes du calendrier d'action
                                        //MAJ GR : totaux par années si nécessaire
                                        for (k = 1; k <= nbColYear; k++)
                                        {
                                            t.Append("<td class=\"L2nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 9 + k].ToString(), webSession.Unit) + "</td>");
                                        }
                                        j = j + 9 + nbColYear;
                                        //fin MAJ
                                    }
                                    break;
                                #endregion

                                #region Level 3
                                case DetailledMediaPlan.L3_COLUMN_INDEX:
                                    if (tab[i, j] != null)
                                    {
                                        string tmpHtml = "";
                                        string tmpHtml2 = "";
                                        //vérification que la catégorie n est pas les chaines thematiques
                                        //									if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]+",-1,-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] != null)
                                        {
                                            tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + ",-1,-1','','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            tmpHtml2 = "<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + ",-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        }
                                        // On traite le cas des pages
                                        totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                        t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3\">&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX] + "</td><td class=\"L3nb\">" + totalUnit + "</td><td class=\"L3nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((showVersions) ? "<td align=\"center\" class=\"L3\">" + tmpHtml + "</td>" : "") + "<td align=\"center\" class=\"L3\">" + tmpHtml2 + "</td>");
                                        currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX].ToString();
                                        //aller aux colonnes du calendrier d'action
                                        //MAJ GR : totaux par années si nécessaire
                                        for (k = 1; k <= nbColYear; k++)
                                        {
                                            t.Append("<td class=\"L3nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 8 + k].ToString(), webSession.Unit) + "</td>");
                                        }
                                        j = j + 8 + nbColYear;
                                        //fin MAJ
                                    }
                                    break;
                                #endregion

                                #region Level 4
                                case DetailledMediaPlan.L4_COLUMN_INDEX:
                                    // On traite le cas des pages
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    //On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
                                    if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] != null)
                                        t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td><td class=\"L4nb\">" + totalUnit + "</td><td class=\"L4nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((showVersions) ? "<td align=\"center\" class=\"L4\"><a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX] + ",-1','','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>" : "") + "<td align=\"center\" class=\"L4\"><a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX] + ",-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
                                    else
                                        t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td><td class=\"L4nb\">" + totalUnit + "</td><td class=\"L4nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((showVersions) ? "<td align=\"center\" class=\"L4\"></td>" : "") + "<td align=\"center\" class=\"L4\"></td>");
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"L4nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 7 + k].ToString(), webSession.Unit) + "</td>");
                                    }
                                    //fin MAJ
                                    j = j + 7 + nbColYear;//nbColYear ajouter GF
                                    break;
                                #endregion
                                default:
                                    if (tab[i, j] == null)
                                    {
                                        t.Append("<td class=\"p3\">&nbsp;</td>");
                                        break;
                                    }
                                    if (tab[i, j].GetType() == typeof(MediaPlanItem))
                                    {
                                        switch (((MediaPlanItem)tab[i, j]).GraphicItemType)
                                        {
                                            case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
                                                t.Append("<td class=\"" + presentClass + "\">" + stringItem + "</td>");
                                                break;
                                            case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
                                                t.Append("<td class=\"" + extendedClass + "\">&nbsp;</td>");
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
                catch (System.Exception err)
                {
                    throw (new WebExceptions.MediaPlanUIException("erreur i=" + i + ",j=" + j, err));
                }
                //t.Append("</table></td></tr></table>");			
                t.Append("</table>");
                #endregion

                // On vide le tableau
                tab = null;
                mediaPlanResultData.HTMLCode = t.ToString();
                return (mediaPlanResultData);

        }
        #endregion

        #region Avec détail support zoom
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
        public static string GetMediaPlanAlertWithMediaDetailLevelHtmlUI(Page page, object[,] tab, WebSession webSession, string zoomDate, string url){
            WebConstantes.CustomerSessions.Period.Type periodType = webSession.PeriodType;
            WebConstantes.CustomerSessions.Period.DisplayLevel periodDisplayLevel = webSession.DetailPeriod;
            string periodBegSave = webSession.PeriodBeginningDate;
            string periodEndSave = webSession.PeriodEndDate;

            try
            {
                if (webSession.PeriodType == WebConstantes.CustomerSessions.Period.Type.dateToDate)
                {
                    webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.dayly;
                    webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDateMonth;
                    webSession.PeriodBeginningDate = webSession.PeriodBeginningDate.Substring(0, 6);
                    webSession.PeriodEndDate = webSession.PeriodEndDate.Substring(0, 6);
                }
                System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

                #region Pas de données à afficher
                if (tab.GetLength(0) == 0)
                {

                    t.Append("<table align=\"center\"><tr>");

                    #region Période précédente
                    if (webSession.PeriodBeginningDate != zoomDate)
                    {
                        if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
                        {
                            t.Append("\r\n\t\t\t\t\t\t<td><a id=\"periodPrevImageButton\" onmouseover=\"periodPrevImageButton_img.src='/Images/Common/period_prev_down.gif'\" "
                            + "onmouseout=\"periodPrevImageButton_img.src='/Images/Common/period_prev_up.gif'\" "
                            + "href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=" +
                                (new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1)).AddMonths(-1).ToString("yyyyMM")
                                + "\"><IMG border=0 alt=\"\" name=\"periodPrevImageButton_img\" src=\"/Images/Common/period_prev_up.gif\"></a></td><td>&nbsp;</td>");
                        }
                        else
                        {
                            t.Append("\r\n\t\t\t\t\t\t<td><a  id=\"periodPrevImageButton\" onmouseover=\"periodPrevImageButton_img.src='/Images/Common/period_prev_down.gif'\" "
                            + "onmouseout=\"periodPrevImageButton_img.src='/Images/Common/period_prev_up.gif'\" "
                            + "href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=");
                            AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
                            tmp.SubWeek(1);
                            if (tmp.Week.ToString().Length < 2) t.Append(tmp.Year.ToString() + "0" + tmp.Week.ToString());
                            else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
                            t.Append("\"><IMG border=0 alt=\"\" name=\"periodPrevImageButton_img\" src=\"/Images/Common/period_prev_up.gif\"></a></td><td>&nbsp;</td>");
                        }
                    }
                    else
                        t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td>");
                    #endregion

                    t.Append("<td class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage) + "</td>");

                    #region Période suivante
                    if (webSession.PeriodEndDate != zoomDate)
                    {
                        if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
                        {
                            t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td><td><a id=\"periodNextImageButton\" onmouseover=\"periodNextImageButton_img.src='/Images/Common/period_next_down.gif'\" "
                                + "onmouseout=\"periodNextImageButton_img.src='/Images/Common/period_next_up.gif'\" "
                                + "href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=" +
                                (new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1)).AddMonths(1).ToString("yyyyMM")
                                + "\"><IMG border=0 alt=\"\" name=\"periodNextImageButton_img\" src=\"/Images/Common/period_next_up.gif\"></a></td>");
                        }
                        else
                        {
                            t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td><td><a  id=\"periodNextImageButton\" onmouseover=\"periodNextImageButton_img.src='/Images/Common/period_next_down.gif'\" "
                                + "onmouseout=\"periodNextImageButton_img.src='/Images/Common/period_next_up.gif'\" "
                                + "href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=");
                            AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
                            tmp.Increment();
                            if (tmp.Week.ToString().Length < 2) t.Append(tmp.Year.ToString() + "0" + tmp.Week.ToString());
                            else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
                            t.Append("\"><IMG border=0 alt=\"\" name=\"periodNextImageButton_img\" src=\"/Images/Common/period_next_up.gif\"></a></td>");
                        }
                    }
                    else
                        t.Append("\r\n\t\t\t\t\t\t<td>&nbsp;</td>");
                    #endregion

                    t.Append("</tr></table>");
                    return (t.ToString());
                }
                #endregion

                #region Variables

                //MAJ GR : Colonnes totaux par année si nécessaire
                int k = 0;
                int nbColYear = 0;
                //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
                if (webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
                {
                    string periodBeginning = TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(zoomDate, webSession.PeriodType).ToString("yyyyMMdd");
                    string periodEnd = TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(zoomDate, webSession.PeriodType).ToString("yyyyMMdd");
                    int currentDate = int.Parse(periodBeginning.Substring(0, 4));
                    int oldCurrentDate = int.Parse(periodEnd.Substring(0, 4));
                    nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0, 4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0, 4));
                    if ((nbColYear > 0) && ((currentDate != oldCurrentDate))) nbColYear++;
                    else nbColYear = 0;
                }
                int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX + nbColYear;
                //fin MAJ

                string classe;
                //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
                //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
                //affichée, utilisez getLength-1
                int nbColTab = tab.GetLength(1), j, i;
                int nbline = tab.GetLength(0);
                //MAJ GR : FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
                int nbPeriod = nbColTab - FIRST_PERIOD_INDEX - 1;
                //fin MAJ
                string currentCategoryName = "tit";
                //string classCss="";
                string totalUnit = "";

                #endregion

                #region Module
                Int64 moduleId;
                WebConstantes.Module.Type moduleType = (ModulesList.GetModule(webSession.CurrentModule)).ModuleType;
                if (moduleType == WebConstantes.Module.Type.alert || zoomDate.Length > 0) moduleId = WebConstantes.Module.Name.ALERTE_PLAN_MEDIA;
                else moduleId = WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;
                #endregion

                #region On calcule la taille de la colonne Total
                int nbtot = WebFunctions.Units.ConvertUnitValueToString(tab[1, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit).Length;
                int nbSpace = (nbtot - 1) / 3;
                int nbCharTotal = nbtot + nbSpace - 5;
                if (nbCharTotal < 5) nbCharTotal = 0;
                #endregion

                #region script
                if (!page.IsClientScriptBlockRegistered("OpenInsertions")) page.RegisterClientScriptBlock("OpenInsertions", WebFunctions.Script.OpenInsertions());
                #endregion

                #region debut Tableau
                t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
                t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"pt\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
                t.Append("\r\n\t\t<td rowspan=3 class=\"pt\">" + GestionWeb.GetWebWord(805, webSession.SiteLanguage));
                for (int h = 0; h < nbCharTotal; h++)
                {
                    t.Append("&nbsp;");
                }
                t.Append("</td>");

                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
                bool showVersions = false;
                if (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG))
                {
                    showVersions = true;
                    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + GestionWeb.GetWebWord(1994, webSession.SiteLanguage) + "</td>"); //Versions
                }
                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + GestionWeb.GetWebWord(2245, webSession.SiteLanguage) + "</td>"); //Insertions

                // MAJ GR : On affiche les années si nécessaire
                for (k = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++)
                {
                    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + tab[0, k].ToString() + "</td>");
                    for (int h = 0; h < nbCharTotal + 5; h++)
                    {
                        t.Append("&nbsp;");
                    }
                    t.Append("</td>");
                }
                //fin MAJ

                #endregion

                #region Période précédente
                if (webSession.PeriodBeginningDate != zoomDate)
                {
                    if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
                    {
                        t.Append("\r\n\t\t\t\t\t\t<td class=\"pyLeft\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=" +
                            (new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1)).AddMonths(-1).ToString("yyyyMM")
                            + "\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
                    }
                    else
                    {
                        t.Append("\r\n\t\t\t\t\t\t<td class=\"pyLeft\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=");
                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
                        tmp.SubWeek(1);
                        if (tmp.Week.ToString().Length < 2) t.Append(tmp.Year.ToString() + "0" + tmp.Week.ToString());
                        else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
                        t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
                    }
                }
                else
                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\">&nbsp;</td>");
                #endregion

                #region Entête de période
                t.Append("\r\n\t\t\t\t\t\t<td colspan=\"" + (nbColTab - FIRST_PERIOD_INDEX - 2) + "\" class=\"pa\" align=center>"
                    + Dates.getPeriodTxt(webSession, zoomDate)
                    + "</td>");
                #endregion

                #region Période suivante
                if (webSession.PeriodEndDate != zoomDate)
                {
                    if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
                    {
                        t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=" +
                            (new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1)).AddMonths(1).ToString("yyyyMM")
                            + "\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
                    }
                    else
                    {
                        t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=");
                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
                        tmp.Increment();
                        if (tmp.Week.ToString().Length < 2) t.Append(tmp.Year.ToString() + "0" + tmp.Week.ToString());
                        else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
                        t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
                    }
                }
                else
                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\">&nbsp;</td>");
                t.Append("</tr><tr>");
                #endregion

                #region Périodes
                for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
                {

                    t.Append("<td class=\"pp\">&nbsp;" + ((string)tab[0, j]).Substring(6, 2) + "&nbsp;</td>");
                }

                string dayClass = "";
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
                t.Append("\r\n\t</tr><tr>");
                for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
                {
                    DateTime dt = (DateString.YYYYMMDDToDateTime((string)tab[0, j]));
                    if (dt.DayOfWeek == DayOfWeek.Saturday
                        || dt.DayOfWeek == DayOfWeek.Sunday
                        ) {
                        dayClass = "pdw";
                    }
                    else
                    {
                        dayClass = "pd";
                    }
                    t.Append("<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>");
                }
                t.Append("\r\n\t</tr>");
                #endregion

                #region Calendrier d'action
                i = 0;
                try
                {
                    for (i = 1; i < nbline; i++)
                    {
                        for (j = 0; j < nbColTab; j++)
                        {
                            switch (j)
                            {

                                #region Level 1
                                case DetailledMediaPlan.L1_COLUMN_INDEX:
                                    if (tab[i, j] != null)
                                    {
                                        string tmpHtml = "";
                                        string tmpHtml2 = "";
                                        if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) classe = "L0";
                                        else
                                        {
                                            classe = "L1";
                                            if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] != null)
                                            {
                                                tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + ",-1,-1,-1,-1','" + zoomDate + "','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                                tmpHtml2 = "<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + ",-1,-1,-1,-1','" + zoomDate + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            }
                                        }
                                        if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
                                        {
                                            i = int.MaxValue - 2;
                                            j = int.MaxValue - 2;
                                            break;
                                        }
                                        // On traite le cas des pages
                                        totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                        t.Append("\r\n\t<tr>\r\n\t\t<td class=\"" + classe + "\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((showVersions) ? "<td align=\"center\" class=\"" + classe + "\">" + tmpHtml + "</td>" : "") + "<td align=\"center\" class=\"" + classe + "\">" + tmpHtml2 + "</td>");
                                        //aller aux colonnes du calendrier d'action
                                        //MAJ GR : totaux par années si nécessaire
                                        for (k = 1; k <= nbColYear; k++)
                                        {
                                            t.Append("<td class=\"" + classe + "nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 10 + k].ToString(), webSession.Unit) + "</td>");
                                        }
                                        j = j + 10 + nbColYear;
                                        //fin MAJ
                                    }
                                    break;
                                #endregion

                                #region Level 2
                                case DetailledMediaPlan.L2_COLUMN_INDEX:
                                    if (tab[i, j] != null)
                                    {
                                        string tmpHtml = "";
                                        string tmpHtml2 = "";
                                        if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] != null)
                                        {
                                            tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + ",-1,-1,-1','" + zoomDate + "','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            tmpHtml2 = "<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + ",-1,-1,-1','" + zoomDate + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        }

                                        totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                        t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX] + "</td><td class=\"L2nb\">" + totalUnit + "</td><td class=\"L2nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((showVersions) ? "<td align=\"center\" class=\"L2\">" + tmpHtml + "</td>" : "") + "<td align=\"center\" class=\"L2\">" + tmpHtml2 + "</td>");
                                        currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX].ToString();
                                        for (k = 1; k <= nbColYear; k++)
                                        {
                                            t.Append("<td class=\"L2nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 9 + k].ToString(), webSession.Unit) + "</td>");
                                        }
                                        j = j + 9 + nbColYear;
                                    }
                                    break;
                                #endregion

                                #region Level 3
                                case DetailledMediaPlan.L3_COLUMN_INDEX:
                                    if (tab[i, j] != null)
                                    {
                                        totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);

                                        t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX] + "</td><td class=\"L3nb\">" + totalUnit + "</td><td class=\"L3nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((showVersions) ? "<td align=\"center\" class=\"L3\"><a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + ",-1,-1','" + zoomDate + "','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>" : "") + "<td align=\"center\" class=\"L3\"><a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + ",-1,-1','" + zoomDate + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
                                        for (k = 1; k <= nbColYear; k++)
                                        {
                                            t.Append("<td class=\"L3nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 8 + k].ToString(), webSession.Unit) + "</td>");
                                        }
                                        j = j + 8;
                                    }
                                    break;
                                #endregion

                                #region Level 4
                                case DetailledMediaPlan.L4_COLUMN_INDEX:
                                    if (tab[i, j] != null)
                                    {
                                        // On traite le cas des pages
                                        totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);

                                        t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td><td class=\"L4nb\">" + totalUnit + "</td><td class=\"L4nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((showVersions) ? "<td align=\"center\" class=\"L4\"><a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX] + ",-1','" + zoomDate + "','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>" : "") + "<td align=\"center\" class=\"L4\"><a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX] + ",-1','" + zoomDate + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
                                        for (k = 1; k <= nbColYear; k++)
                                        {
                                            t.Append("<td class=\"L4nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 7 + k].ToString(), webSession.Unit) + "</td>");
                                        }
                                        //aller aux colonnes du calendrier d'action
                                        j = j + 7;
                                    }
                                    break;
                                #endregion
                                default:
                                    if (tab[i, j] == null)
                                    {
                                        t.Append("<td class=\"p3\">&nbsp;</td>");
                                        break;
                                    }
                                    if (tab[i, j].GetType() == typeof(MediaPlanItem))
                                    {
                                        switch (((MediaPlanItem)tab[i, j]).GraphicItemType)
                                        {
                                            case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
                                                t.Append("<td class=\"p4\">&nbsp;</td>");
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
                        t.Append("</tr>");
                    }
                }
                catch (System.Exception e)
                {
                    throw (new System.Exception("erreur i=" + i + ",j=" + j, e));
                }
                t.Append("</table>");

                #endregion

                // On vide le tableau
                tab = null;

                return (t.ToString());
            }
            finally
            {
                webSession.DetailPeriod = periodDisplayLevel;
                webSession.PeriodType = periodType;
                webSession.PeriodBeginningDate = periodBegSave;
                webSession.PeriodEndDate = periodEndSave;
            }

        }
        #endregion

        #region Sans détail support zoom
        ///// <summary>
        ///// Génère le code HTML pour afficher un calendrier d'action détaillé en jour sur UNE période.
        ///// Elles se base sur un tableau contenant les données
        ///// tab vide : message "Pas de données" + bouton retour
        ///// tab non vide:
        /////		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
        /////		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
        /////		Génération du code HTML des entêtes de colonne
        /////		Génération du code HTML des périodes (période suivante, période courante, période suivante, intitulés d'une unité de période
        /////		Génération du code HTML du calendrier d'action
        ///// </summary>
        ///// <param name="page">Page qui affiche le Plan Média</param>
        ///// <param name="tab">Tableau contenant les données à mettre en forme</param>
        ///// <param name="webSession">Session du client</param>
        ///// <param name="zoomDate">Période à prendre en compte (un mois ou une semaine)</param>
        ///// <param name="url">Lien vers la pge elle-même. Permet de gérer les flèches "Période suivante", "Période précédente"</param>
        ///// <returns>Code Généré</returns>
        ///// <remarks>
        ///// Utilise les méthodes:
        /////		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
        /////		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
        ///// </remarks>
        //public static string GetMediaPlanAlertUI(Page page, object[,] tab, WebSession webSession, string zoomDate, string url){

        //    #region Pas de données à afficher
        //    if (tab.GetLength(0) == 0)
        //    {
        //        return ("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage)
        //            + "<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/" + webSession.SiteLanguage + "/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/" + webSession.SiteLanguage + "/button/back_up.gif';\">"
        //            + "<img src=\"/Images/" + webSession.SiteLanguage + "/button/back_up.gif\" border=0 name=bouton></a></div>");
        //    }
        //    #endregion

        //    #region Variables

        //    //MAJ GR : Colonnes totaux par année si nécessaire
        //    int k = 0;
        //    int nbColYear = 0;
        //    //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
        //    if (nbColYear > 0) nbColYear++;
        //    int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX + nbColYear;
        //    //fin MAJ

        //    string classe;
        //    //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
        //    //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
        //    //affichée, utilisez getLength-1
        //    System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
        //    int nbColTab = tab.GetLength(1), j, i;
        //    int nbline = tab.GetLength(0);
        //    //MAJ GR : FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
        //    int nbPeriod = nbColTab - FIRST_PERIOD_INDEX - 1;
        //    //fin MAJ
        //    bool premier = true;
        //    string currentCategoryName = "tit";
        //    //string classCss="";
        //    const string PLAN_MEDIA_1_CLASSE = "p6";
        //    const string PLAN_MEDIA_2_CLASSE = "p7";
        //    const string PLAN_MEDIA_NB_1_CLASSE = "p8";
        //    const string PLAN_MEDIA_NB_2_CLASSE = "p9";
        //    string totalUnit = "";

        //    #endregion

        //    #region On calcule la taille de la colonne Total
        //    int nbtot = WebFunctions.Units.ConvertUnitValueToString(tab[1, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit).Length;
        //    int nbSpace = (nbtot - 1) / 3;
        //    int nbCharTotal = nbtot + nbSpace - 5;
        //    if (nbCharTotal < 5) nbCharTotal = 0;
        //    #endregion

        //    #region script
        //    if(!page.IsClientScriptBlockRegistered("OpenInsertions")) page.RegisterClientScriptBlock("OpenInsertions",WebFunctions.Script.OpenInsertions());
        //    if(!page.IsClientScriptBlockRegistered("OpenCreatives")) page.RegisterClientScriptBlock("OpenCreatives",WebFunctions.Script.OpenCreatives());
        //    // On enregistre le script DynamicMediaPlan qui rend les calendriers d'action dynamique
        //    if (!page.IsClientScriptBlockRegistered("DynamicMediaPlan")) page.RegisterClientScriptBlock("DynamicMediaPlan", TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan());
        //    #endregion

        //    #region debut Tableau
        //    t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
        //    t.Append("\r\n\t\t<td rowspan=3 width=\"250px\" class=\"p2\">");
        //    t.Append("\n\t\t\t<table width=\"250px\" border=0 cellpadding=0 cellspacing=0 bgcolor=#644883>\r\n\t\t\t\t<tr>");
        //    t.Append("\n\t\t\t\t\t<td class=\"p1\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "</td>");
        //    t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:showAllContent3();\"><img id=pictofermer border=0 src=\"/Images/Common/button/picto_fermer.gif\"></a></td>");
        //    t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:hideAllContent3();\"><img id=pictoouvrir border=0 src=\"/Images/Common/button/picto_ouvrir.gif\"></a></td>");
        //    t.Append("\n\t\t\t\t\t<td  bgcolor=#ffffff class=\"p2\"  width=1px></td>");
        //    t.Append("\n\t\t\t\t</tr>\n\t\t\t</table>");
        //    t.Append("\r\n\t\t</td>");
        //    //t.Append("\r\n\t\t<td rowspan=\"2\" width=\"200px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
        //    t.Append("\r\n\t\t<td rowspan=3 class=\"p2\">" + GestionWeb.GetWebWord(805, webSession.SiteLanguage));
        //    for (int h = 0; h < nbCharTotal; h++)
        //    {
        //        t.Append("&nbsp;");
        //    }
        //    t.Append("</td>");

        //    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
        //    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + GestionWeb.GetWebWord(540, webSession.SiteLanguage) + "</td>");

        //    // MAJ GR : On affiche les années si nécessaire
        //    for (k = FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++)
        //    {
        //        t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + tab[0, k].ToString() + "</td>");
        //        for (int h = 0; h < nbCharTotal + 5; h++)
        //        {
        //            t.Append("&nbsp;");
        //        }
        //        t.Append("</td>");
        //    }
        //    //fin MAJ

        //    #endregion

        //    #region Période précédente
        //    if (webSession.PeriodBeginningDate != zoomDate)
        //    {
        //        if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
        //        {
        //            t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=" +
        //                (new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1)).AddMonths(-1).ToString("yyyyMM")
        //                + "\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
        //        }
        //        else
        //        {
        //            t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=");
        //            AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
        //            tmp.SubWeek(1);
        //            if (tmp.Week.ToString().Length < 2) t.Append(tmp.Year.ToString() + "0" + tmp.Week.ToString());
        //            else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
        //            t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
        //        }
        //    }
        //    else
        //        t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\">&nbsp;</td>");
        //    #endregion

        //    #region Entête de période
        //    t.Append("\r\n\t\t\t\t\t\t<td colspan=\"" + (nbColTab - FIRST_PERIOD_INDEX - 2) + "\" class=\"pmannee2\" align=center>"
        //        + Dates.getPeriodTxt(webSession, zoomDate)
        //        + "</td>");
        //    #endregion

        //    #region Période suivante
        //    if (webSession.PeriodEndDate != zoomDate)
        //    {
        //        if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
        //        {
        //            t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=" +
        //                (new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1)).AddMonths(1).ToString("yyyyMM")
        //                + "\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
        //        }
        //        else
        //        {
        //            t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\"><a href=\"" + url + "?idSession=" + webSession.IdSession + "&zoomDate=");
        //            AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
        //            tmp.Increment();
        //            if (tmp.Week.ToString().Length < 2) t.Append(tmp.Year.ToString() + "0" + tmp.Week.ToString());
        //            else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
        //            t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
        //        }
        //    }
        //    else
        //        t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\">&nbsp;</td>");
        //    t.Append("</tr><tr>");
        //    #endregion

        //    #region Périodes
        //    for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
        //    {

        //        t.Append("<td class=\"p10\">&nbsp;" + ((DateTime)tab[0, j]).ToString("dd") + "&nbsp;</td>");
        //    }

        //    string dayClass = "";
        //    char day;
        //    t.Append("\r\n\t</tr><tr>");
        //    for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
        //    {
        //        day = TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession, ((DateTime)tab[0, j]).DayOfWeek.ToString()).ToCharArray()[0];


        //        if (day == GestionWeb.GetWebWord(545, webSession.SiteLanguage).ToCharArray()[0]
        //            || day == GestionWeb.GetWebWord(546, webSession.SiteLanguage).ToCharArray()[0]
        //            )
        //        {
        //            dayClass = "p132";
        //        }
        //        else
        //        {
        //            dayClass = "p131";
        //        }
        //        t.Append("<td class=\"" + dayClass + "\">" + TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession, ((DateTime)tab[0, j]).DayOfWeek.ToString()) + "</td>");
        //    }
        //    t.Append("\r\n\t</tr>");
        //    #endregion

        //    #region Calendrier d'action
        //    //t.Append("\r\n\t<tr>");
        //    i = 0;
        //    try
        //    {
        //        for (i = 1; i < nbline; i++)
        //        {
        //            for (j = 0; j < nbColTab; j++)
        //            {
        //                switch (j)
        //                {
        //                    // Total ou vehicle
        //                    case MediaPlanAlert.VEHICLE_COLUMN_INDEX:
        //                        if (tab[i, j] != null)
        //                        {
        //                            string tmpHtml = "";
        //                            if (i == MediaPlanAlert.TOTAL_LINE_INDEX) classe = "pmtotal";
        //                            else
        //                            {
        //                                classe = "pmvehicle";
        //                                if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] != null)
        //                                    tmpHtml = "<a href=\"javascript:openCreation('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + ",-1,-1,-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
        //                                else
        //                                    tmpHtml = "";
        //                            }
        //                            //										if(DBClassificationConstantes.Vehicles.names.outdoor==vehicleName){
        //                            //											if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null ){
        //                            //												tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
        //                            //											}else{tmpHtml="";}
        //                            //										}else{
        //                            //											tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
        //                            //										}										
        //                            if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
        //                            {
        //                                i = int.MaxValue - 2;
        //                                j = int.MaxValue - 2;
        //                                break;
        //                            }
        //                            // On traite le cas des pages
        //                            totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
        //                            t.Append("\r\n\t<tr id=\"" + tab[i, j] + "Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\"" + classe + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"" + classe + "\">" + tmpHtml + "</td>");
        //                            //aller aux colonnes du calendrier d'action
        //                            //MAJ GR : totaux par années si nécessaire
        //                            for (k = 1; k <= nbColYear; k++)
        //                            {
        //                                t.Append("<td class=\"" + classe + "nb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 8 + k].ToString(), webSession.Unit) + "</td>");
        //                            }
        //                            j = j + 8 + nbColYear;
        //                            //fin MAJ
        //                        }
        //                        break;
        //                    // Category
        //                    case MediaPlanAlert.CATEGORY_COLUMN_INDEX:
        //                        if (tab[i, j] != null)
        //                        {
        //                            string tmpHtml = "";
        //                            if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX] != null) tmpHtml = "<a href=\"javascript:openCreation('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX] + ",-1,-1,-1','" + zoomDate + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
        //                            // On traite le cas des pages
        //                            totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
        //                            t.Append("\r\n\t<tr id=\"" + i.ToString() + "Content4\"onclick=\"showHideContent3('" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX] + "');\" style=\"DISPLAY:inline; CURSOR: hand\">\r\n\t\t<td class=\"pmcategory\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX] + "</td><td class=\"pmcategorynb\">" + totalUnit + "</td><td class=\"pmcategorynb\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"pmcategory\">" + tmpHtml + "</td>");
        //                            currentCategoryName = tab[i, FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX].ToString();
        //                            //aller aux colonnes du calendrier d'action
        //                            //MAJ GR : totaux par années si nécessaire
        //                            for (k = 1; k <= nbColYear; k++)
        //                            {
        //                                t.Append("<td class=\"pmcategorynb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 7 + k].ToString(), webSession.Unit) + "</td>");
        //                            }
        //                            j = j + 7 + nbColYear;
        //                            //fin MAJ
        //                        }
        //                        break;
        //                    // Media
        //                    case MediaPlanAlert.MEDIA_COLUMN_INDEX:
        //                        // On traite le cas des pages
        //                        totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
        //                        if (premier)
        //                        {
        //                            //On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la catégorie "chaîne thématique"
        //                            if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null)
        //                                t.Append("\r\n\t<tr id=\"" + i.ToString() + currentCategoryName + "Content3\" onclick=\"showHideAllContent3('" + i.ToString() + currentCategoryName + "','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\"" + PLAN_MEDIA_1_CLASSE + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX] + "</td><td class=\"" + PLAN_MEDIA_NB_1_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_NB_1_CLASSE + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"" + PLAN_MEDIA_1_CLASSE + "\"><a href=\"javascript:openCreation('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] + ",-1,-1','" + zoomDate + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
        //                            else
        //                                t.Append("\r\n\t<tr id=\"" + i.ToString() + currentCategoryName + "Content3\" onclick=\"showHideAllContent3('" + i.ToString() + currentCategoryName + "','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\"" + PLAN_MEDIA_1_CLASSE + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX] + "</td><td class=\"" + PLAN_MEDIA_NB_1_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_NB_1_CLASSE + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"" + PLAN_MEDIA_1_CLASSE + "\"></td>");
        //                            //MAJ GR : totaux par années si nécessaire
        //                            for (k = 1; k <= nbColYear; k++)
        //                            {
        //                                t.Append("<td class=\"" + PLAN_MEDIA_1_CLASSE + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 6 + k].ToString(), webSession.Unit) + "</td>");
        //                            }
        //                            //fin MAJ
        //                            premier = false;
        //                        }
        //                        else
        //                        {
        //                            //On verifie si la chaine IdMedia, IdVehicle est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
        //                            if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null)
        //                                t.Append("\r\n\t<tr id=\"" + i.ToString() + currentCategoryName + "Content3\" onclick=\"showHideAllContent3('" + i.ToString() + currentCategoryName + "','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\"" + PLAN_MEDIA_2_CLASSE + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX] + "</td><td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"" + PLAN_MEDIA_2_CLASSE + "\" onclick=\"\" ><a href=\"javascript:openCreation('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] + ",-1,-1','" + zoomDate + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
        //                            else
        //                                t.Append("\r\n\t<tr id=\"" + i.ToString() + currentCategoryName + "Content3\" onclick=\"showHideAllContent3('" + i.ToString() + currentCategoryName + "','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\"" + PLAN_MEDIA_2_CLASSE + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX] + "</td><td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"" + PLAN_MEDIA_2_CLASSE + "\"></td>");
        //                            //MAJ GR : totaux par années si nécessaire
        //                            for (k = 1; k <= nbColYear; k++)
        //                            {
        //                                t.Append("<td class=\"" + PLAN_MEDIA_2_CLASSE + "\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 6 + k].ToString(), webSession.Unit) + "</td>");
        //                            }
        //                            //fin MAJ
        //                            premier = true;
        //                        }
        //                        //aller aux colonnes du calendrier d'action
        //                        j = j + 6;
        //                        break;
        //                    default:
        //                        if (tab[i, j] == null)
        //                        {
        //                            t.Append("<td class=\"p3\">&nbsp;</td>");
        //                            break;
        //                        }
        //                        if (tab[i, j].GetType() == typeof(FrameWorkResultConstantes.MediaPlanAlert.graphicItemType))
        //                        {
        //                            switch ((FrameWorkResultConstantes.MediaPlanAlert.graphicItemType)tab[i, j])
        //                            {
        //                                case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.present:
        //                                    t.Append("<td class=\"p4\">&nbsp;</td>");
        //                                    break;
        //                                case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.extended:
        //                                    t.Append("<td class=\"p5\">&nbsp;</td>");
        //                                    break;
        //                                default:
        //                                    t.Append("<td class=\"p3\">&nbsp;</td>");
        //                                    break;
        //                            }
        //                        }
        //                        break;
        //                }
        //            }
        //            t.Append("</tr>");
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        throw (new System.Exception("erreur i=" + i + ",j=" + j, e));
        //    }
        //    //t.Append("</tr></table></td></tr></table>");


        //    //	t.Append("</table></td></tr></table>");

        //    t.Append("</table>");

        //    #endregion

        //    // On vide le tableau
        //    tab = null;

        //    return (t.ToString());

        //}
        #endregion

        #region Sans détail support date de la session
        ///// <summary>
        ///// Génère le code HTML pour afficher un calendrier d'action détaillé en jour entre les dates de la session.
        ///// Elles se base sur un tableau contenant les données
        ///// tab vide : message "Pas de données" + bouton retour
        ///// tab non vide:
        /////		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
        /////		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
        /////		Génération du code HTML des entêtes de colonne
        /////		Génération du code HTML des périodes (Libellé du mois, intitulés d'une unité de période)
        /////		Génération du code HTML du calendrier d'action
        ///// </summary>
        ///// <param name="page">Page qui affiche le Plan Média</param>
        ///// <param name="tab">Tableau contenant les données à mettre en forme</param>
        ///// <param name="webSession">Session du client</param>
        ///// <returns>Code Généré</returns>
        ///// <remarks>
        ///// Utilise les méthodes:
        /////		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
        /////		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
        ///// </remarks>
        //public static string GetMediaPlanAlertUI(Page page, object[,] tab, WebSession webSession)
        //{

        //    #region Pas de données à afficher
        //    if (tab.GetLength(0) == 0)
        //    {
        //        return ("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage)
        //            + "<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/" + webSession.SiteLanguage + "/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/" + webSession.SiteLanguage + "/button/back_up.gif';\">"
        //            + "<img src=\"/Images/" + webSession.SiteLanguage + "/button/back_up.gif\" border=0 name=bouton></a></div>");
        //    }
        //    #endregion

        //    #region Variables
        //    //MAJ GR : Colonnes totaux par année si nécessaire
        //    int k = 0;
        //    int nbColYear = 0;
        //    //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
        //    if (nbColYear > 0) nbColYear++;
        //    int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX + nbColYear;
        //    //fin MAJ

        //    string classe;
        //    //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
        //    //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
        //    //affichée, utilisez getLength-1
        //    System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
        //    int nbColTab = tab.GetLength(1), j, i;
        //    int nbline = tab.GetLength(0);
        //    //MAJ GR : FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
        //    int nbPeriod = nbColTab - FIRST_PERIOD_INDEX - 1;
        //    //fin MAJ
        //    bool premier = true;
        //    string currentCategoryName = "tit";
        //    string totalUnit = "";
        //    const string PLAN_MEDIA_1_CLASSE = "p6";
        //    const string PLAN_MEDIA_2_CLASSE = "p7";
        //    const string PLAN_MEDIA_NB_1_CLASSE = "p8";
        //    const string PLAN_MEDIA_NB_2_CLASSE = "p9";
        //    #endregion

        //    #region On calcule la taille de la colonne Total
        //    int nbtot = WebFunctions.Units.ConvertUnitValueToString(tab[1, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit).Length;
        //    int nbSpace = (nbtot - 1) / 3;
        //    int nbCharTotal = nbtot + nbSpace - 5;
        //    if (nbCharTotal < 5) nbCharTotal = 0;
        //    #endregion

        //    #region script
        //    if (!page.IsClientScriptBlockRegistered("openCreation")) page.RegisterClientScriptBlock("openCreation", WebFunctions.Script.OpenCreation());
        //    // On enregistre le script DynamicMediaPlan qui rend les calendriers d'action dynamique
        //    if (!page.IsClientScriptBlockRegistered("DynamicMediaPlan")) page.RegisterClientScriptBlock("DynamicMediaPlan", TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan());
        //    #endregion

        //    #region Début du tableau
        //    t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
        //    t.Append("\r\n\t\t<td rowspan=3 width=\"250px\" class=\"p2\">");
        //    t.Append("\n\t\t\t<table width=\"250px\" border=0 cellpadding=0 cellspacing=0 bgcolor=#644883>\r\n\t\t\t\t<tr>");
        //    t.Append("\n\t\t\t\t\t<td class=\"p1\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "</td>");
        //    t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:showAllContent3();\"><img id=pictofermer border=0 src=\"/Images/Common/button/picto_fermer.gif\"></a></td>");
        //    t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:hideAllContent3();\"><img id=pictoouvrir border=0 src=\"/Images/Common/button/picto_ouvrir.gif\"></a></td>");
        //    t.Append("\n\t\t\t\t\t<td bgcolor=#ffffff width=1px class=\"p2\"></td>");
        //    t.Append("\n\t\t\t\t</tr>\n\t\t\t</table>");
        //    t.Append("\r\n\t\t</td>");
        //    t.Append("\r\n\t\t<td rowspan=3 class=\"p2\">" + GestionWeb.GetWebWord(805, webSession.SiteLanguage));
        //    for (int h = 0; h < nbCharTotal; h++)
        //    {
        //        t.Append("&nbsp;");
        //    }
        //    t.Append("</td>");
        //    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
        //    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + GestionWeb.GetWebWord(540, webSession.SiteLanguage) + "</td>");
        //    // MAJ GR : On affiche les années si nécessaire
        //    for (k = FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++)
        //    {
        //        t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + tab[0, k].ToString() + "</td>");
        //    }
        //    //fin MAJ
        //    #endregion

        //    #region Période
        //    string dateHtml = "<tr>";
        //    string headerHtml = "";
        //    DateTime currentDay = new DateTime(((DateTime)tab[0, FIRST_PERIOD_INDEX]).Ticks);
        //    int previousMonth = currentDay.Month;
        //    currentDay = currentDay.AddDays(-1);
        //    int nbPeriodInMonth = 0;
        //    for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
        //    {
        //        currentDay = currentDay.AddDays(1);
        //        if (currentDay.Month != previousMonth)
        //        {
        //            if (nbPeriodInMonth >= 8)
        //            {
        //                headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"p2\" align=center>"
        //                    + Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM"))
        //                    + "</td>";
        //            }
        //            else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"p2\" align=center>"
        //                     + "&nbsp"
        //                     + "</td>";
        //            nbPeriodInMonth = 0;
        //            previousMonth = currentDay.Month;
        //        }
        //        nbPeriodInMonth++;
        //        dateHtml += "<td class=\"p10\">&nbsp;" + currentDay.ToString("dd") + "&nbsp;</td>";
        //    }
        //    if (nbPeriodInMonth >= 8) headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"p2\" align=center>"
        //                                  + Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM"))
        //                                  + "</td>";
        //    else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"p2\" align=center>"
        //             + "&nbsp"
        //             + "</td>";
        //    dateHtml += "</tr>";

        //    string dayClass = "";
        //    char day;
        //    dateHtml += "\r\n\t<tr>";
        //    for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
        //    {
        //        day = TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession, ((DateTime)tab[0, j]).DayOfWeek.ToString()).ToCharArray()[0];
        //        if (day == GestionWeb.GetWebWord(545, webSession.SiteLanguage).ToCharArray()[0]
        //            || day == GestionWeb.GetWebWord(546, webSession.SiteLanguage).ToCharArray()[0]
        //            )
        //        {
        //            dayClass = "p132";
        //        }
        //        else
        //        {
        //            dayClass = "p131";
        //        }
        //        dateHtml += "<td class=\"" + dayClass + "\">" + TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession, ((DateTime)tab[0, j]).DayOfWeek.ToString()) + "</td>";
        //    }
        //    dateHtml += "\r\n\t</tr>";

        //    headerHtml += "</tr>";
        //    t.Append(headerHtml);
        //    t.Append(dateHtml);
        //    #endregion

        //    #region Calendrier d'actions
        //    //t.Append("\r\n\t<tr>");
        //    i = 0;
        //    try
        //    {
        //        for (i = 1; i < nbline; i++)
        //        {
        //            for (j = 0; j < nbColTab; j++)
        //            {
        //                switch (j)
        //                {
        //                    // Total ou vehicle
        //                    case MediaPlanAlert.VEHICLE_COLUMN_INDEX:
        //                        if (tab[i, j] != null)
        //                        {
        //                            string tmpHtml = "";
        //                            //vérification que la catégorie n est pas les chaines thematiques

        //                            if (i == MediaPlanAlert.TOTAL_LINE_INDEX) classe = "pmtotal";
        //                            else
        //                            {

        //                                classe = "pmvehicle";
        //                                if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] != null)
        //                                    tmpHtml = "<a href=\"javascript:openCreation('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + ",-1,-1,-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
        //                                else
        //                                    tmpHtml = "";
        //                            }
        //                            if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
        //                            {
        //                                i = int.MaxValue - 2;
        //                                j = int.MaxValue - 2;
        //                                break;
        //                            }
        //                            // On traite le cas des pages
        //                            totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
        //                            t.Append("\r\n\t<tr id=\"" + tab[i, j] + "Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\"" + classe + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"" + classe + "\">" + tmpHtml + "</td>");
        //                            //t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ###")+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
        //                            //aller aux colonnes du calendrier d'action
        //                            //MAJ GR : totaux par années si nécessaire
        //                            for (k = 1; k <= nbColYear; k++)
        //                            {
        //                                t.Append("<td class=\"" + classe + "nb\">" + ((double)tab[i, j + 8 + k]).ToString("0.##") + "</td>");
        //                            }
        //                            j = j + 8 + nbColYear;
        //                            //fin MAJ
        //                        }
        //                        break;
        //                    // Category
        //                    case MediaPlanAlert.CATEGORY_COLUMN_INDEX:
        //                        if (tab[i, j] != null)
        //                        {
        //                            string tmpHtml = "";
        //                            //vérification que la catégorie n est pas les chaines thematiques
        //                            if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX] != null) tmpHtml = "<a href=\"javascript:openCreation('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX] + ",-1,-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
        //                            // On traite le cas des pages
        //                            totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
        //                            t.Append("\r\n\t<tr id=\"" + i.ToString() + "Content4\"onclick=\"showHideContent3('" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX] + "');\" style=\"DISPLAY:inline; CURSOR: hand\">\r\n\t\t<td class=\"pmcategory\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX] + "</td><td class=\"pmcategorynb\">" + totalUnit + "</td><td class=\"pmcategorynb\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"pmcategory\">" + tmpHtml + "</td>");
        //                            currentCategoryName = tab[i, FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX].ToString();
        //                            //aller aux colonnes du calendrier d'action
        //                            //MAJ GR : totaux par années si nécessaire
        //                            for (k = 1; k <= nbColYear; k++)
        //                            {
        //                                t.Append("<td class=\"pmcategorynb\">" + ((double)tab[i, j + 7 + k]).ToString("0.##") + "</td>");
        //                            }
        //                            j = j + 7 + nbColYear;
        //                            //fin MAJ
        //                        }
        //                        break;
        //                    // Media
        //                    case MediaPlanAlert.MEDIA_COLUMN_INDEX:
        //                        // On traite le cas des pages
        //                        totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
        //                        if (premier)
        //                        {
        //                            //On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
        //                            if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null)
        //                                t.Append("\r\n\t<tr id=\"" + i.ToString() + currentCategoryName + "Content3\" onclick=\"showHideAllContent3('" + i.ToString() + currentCategoryName + "','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\"" + PLAN_MEDIA_1_CLASSE + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX] + "</td><td class=\"" + PLAN_MEDIA_NB_1_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_NB_1_CLASSE + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"" + PLAN_MEDIA_1_CLASSE + "\"><a href=\"javascript:openCreation('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] + ",-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
        //                            else
        //                                t.Append("\r\n\t<tr id=\"" + i.ToString() + currentCategoryName + "Content3\" onclick=\"showHideAllContent3('" + i.ToString() + currentCategoryName + "','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\"" + PLAN_MEDIA_1_CLASSE + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX] + "</td><td class=\"" + PLAN_MEDIA_NB_1_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_NB_1_CLASSE + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"" + PLAN_MEDIA_1_CLASSE + "\"></td>");
        //                            //MAJ GR : totaux par années si nécessaire
        //                            for (k = 1; k <= nbColYear; k++)
        //                            {
        //                                t.Append("<td class=\"" + PLAN_MEDIA_1_CLASSE + "\">" + ((double)tab[i, j + 6 + k]).ToString("0.##") + "</td>");
        //                            }
        //                            //fin MAJ
        //                            premier = false;
        //                        }
        //                        else
        //                        {
        //                            //On verifie si la chaine IdMedia, IdVehicle est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
        //                            if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null)
        //                                t.Append("\r\n\t<tr id=\"" + i.ToString() + currentCategoryName + "Content3\" onclick=\"showHideAllContent3('" + i.ToString() + currentCategoryName + "','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\"" + PLAN_MEDIA_2_CLASSE + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX] + "</td><td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"" + PLAN_MEDIA_2_CLASSE + "\"><a href=\"javascript:openCreation('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] + ",-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
        //                            else
        //                                t.Append("\r\n\t<tr id=\"" + i.ToString() + currentCategoryName + "Content3\" onclick=\"showHideAllContent3('" + i.ToString() + currentCategoryName + "','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\"" + PLAN_MEDIA_2_CLASSE + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX] + "</td><td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_NB_2_CLASSE + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td><td align=\"center\" class=\"" + PLAN_MEDIA_2_CLASSE + "\"></td>");
        //                            //MAJ GR : totaux par années si nécessaire
        //                            for (k = 1; k <= nbColYear; k++)
        //                            {
        //                                t.Append("<td class=\"" + PLAN_MEDIA_2_CLASSE + "\">" + ((double)tab[i, j + 6 + k]).ToString("0.##") + "</td>");
        //                            }
        //                            //fin MAJ
        //                            premier = true;
        //                        }
        //                        j = j + 6;
        //                        break;
        //                    default:
        //                        if (tab[i, j] == null)
        //                        {
        //                            t.Append("<td class=\"p3\">&nbsp;</td>");
        //                            break;
        //                        }
        //                        if (tab[i, j].GetType() == typeof(FrameWorkResultConstantes.MediaPlanAlert.graphicItemType))
        //                        {
        //                            switch ((FrameWorkResultConstantes.MediaPlanAlert.graphicItemType)tab[i, j])
        //                            {
        //                                case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.present:
        //                                    t.Append("<td class=\"p4\">&nbsp;</td>");
        //                                    break;
        //                                case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.extended:
        //                                    t.Append("<td class=\"p5\">&nbsp;</td>");
        //                                    break;
        //                                default:
        //                                    t.Append("<td class=\"p3\">&nbsp;</td>");
        //                                    break;
        //                            }
        //                        }
        //                        break;
        //                }
        //            }
        //            t.Append("</tr>");
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        throw (new System.Exception("erreur i=" + i + ",j=" + j, e));
        //    }
        //    //t.Append("</table></td></tr></table>");			
        //    t.Append("</table>");
        //    #endregion

        //    // On vide le tableau
        //    tab = null;

        //    return (t.ToString());

        //}
        #endregion

        #endregion

        #region Sortie Ms Excel Avec détail support(Web)
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
        /// <param name="showValue">Montre les valeurs dans les cases du calendrier d'action</param>
        /// <returns>Code Généré</returns>
        public static string GetMediaPlanAlertWithMediaDetailLevelExcelUI(object[,] tab, WebSession webSession, string periodBeginning, string periodEnd, bool showValue)
        {

            #region Variables
            string classe;
            //MAJ GR : Colonnes totaux par année si nécessaire
            int k = 0;
            //Bug problème année
            //int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
            int nbColYear = int.Parse(periodEnd.Substring(0, 4)) - int.Parse(periodBeginning.Substring(0, 4));

            if (nbColYear > 0) nbColYear++;
            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX + nbColYear;
            //fin MAJ

            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
            //affichée, utilisez getLength-1
            int nbColTab = tab.GetLength(1), j, i;
            int nbline = tab.GetLength(0);
            int nbPeriod = nbColTab - FIRST_PERIOD_INDEX - 1;
            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            string totalUnit = "";
            int colorItemIndex = 1;
            int colorNumberToUse = 0;
            int sloganIndex = -1;
            //webSession.SloganColors.Clear();
            try { webSession.SloganColors.Add((Int64)0, "pc0"); }
            catch (System.Exception) { }
            string presentClass = "p4";
            string extendedClass = "p5";
            string stringItem = "&nbsp;";
            #endregion

            #region Rappel des paramètres
            //			//t.Append(ExcelFunction.GetExcelHeader(webSession,false,true,false,true,false,periodBeginning,periodEnd,""));
            //			t.Append(ExcelFunction.GetExcelHeader(webSession,true,periodBeginning,periodEnd));

            if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA || webSession.CurrentModule == WebConstantes.Module.Name.ALERTE_PLAN_MEDIA)
            {
                t.Append(ExcelFunction.GetLogo(webSession));
                t.Append(ExcelFunction.GetExcelHeader(webSession, false, periodBeginning, periodEnd));
            }
            else {
                t.Append(ExcelFunction.GetLogo(webSession));
                t.Append(ExcelFunction.GetExcelHeaderForMediaPlanPopUp(webSession, false, periodBeginning, periodEnd));
            }
            #endregion

            #region debut Tableau
            t.Append("<table border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
            t.Append("<td rowspan=\"3\" width=\"200px\" class=\"ptX\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            t.Append("<td rowspan=\"3\" class=\"ptX\">" + GestionWeb.GetWebWord(805, webSession.SiteLanguage));
            t.Append("</td>");
            t.Append("<td rowspan=\"3\" class=\"ptX\">" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
            // MAJ GR : On affiche les années si nécessaire
            for (k = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++)
            {
                t.Append("<td rowspan=\"3\" class=\"ptX\" align=center>" + tab[0, k].ToString() + "</td>");
            }
            //fin MAJ
            #endregion

            #region Période
            string dateHtml = "<tr>";
            string headerHtml = "";
            DateTime currentDay = DateString.YYYYMMDDToDateTime((string)tab[0, FIRST_PERIOD_INDEX]);
            int previousMonth = currentDay.Month;
            currentDay = currentDay.AddDays(-1);
            int nbPeriodInMonth = 0;
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                currentDay = currentDay.AddDays(1);
                if (currentDay.Month != previousMonth)
                {
                    if (nbPeriodInMonth >= 8)
                    {
                        headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"ptX\" align=center>"
                            + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM")))
                            + "</td>";
                    }
                    else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"ptX\" align=center>"
                             + "&nbsp"
                             + "</td>";
                    nbPeriodInMonth = 0;
                    previousMonth = currentDay.Month;
                }
                nbPeriodInMonth++;
                dateHtml += "<td class=\"ppX\">" + currentDay.ToString("dd") + "</td>";
            }
            if (nbPeriodInMonth >= 8) headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"ptX\" align=center>"
                                          + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM")))
                                          + "</td>";
            else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"ptX\" align=center>"
                     + "&nbsp"
                     + "</td>";
            dateHtml += "</tr>";
            // Jour de la semaine
            string dayClass = "";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
            dateHtml += "\r\n\t<tr>";
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                DateTime dt = DateString.YYYYMMDDToDateTime((string)tab[0, j]);
                if (dt.DayOfWeek == DayOfWeek.Saturday
                    || dt.DayOfWeek == DayOfWeek.Sunday
                    ) {
                    dayClass = "pdwX";
                }
                else
                {
                    dayClass = "pdX";
                }
                dateHtml += "<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>";
            }
            dateHtml += "</tr>";


            headerHtml += "</tr>";
            t.Append(headerHtml);
            t.Append(dateHtml);
            #endregion

            #region Tableau
            i = 0;
            try
            {
                sloganIndex = GetSloganIdIndex(webSession);
                for (i = 1; i < nbline; i++)
                {
                    if (sloganIndex != -1 && tab[i, sloganIndex] != null)
                    {
                        if (!webSession.SloganColors.ContainsKey((Int64)tab[i, sloganIndex]))
                        {
                            colorNumberToUse = (colorItemIndex % 30) + 1;
                            webSession.SloganColors.Add((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString());
                            colorItemIndex++;
                        }
                        presentClass = webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString();
                        extendedClass = webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString();
                        stringItem = "x";
                    }
                    else
                    {
                        presentClass = "p4";
                        extendedClass = "p5";
                        stringItem = "&nbsp;";
                    }
                    for (j = 0; j < nbColTab; j++)
                    {
                        switch (j)
                        {
                            #region Level 1
                            case DetailledMediaPlan.L1_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) classe = "L0X";
                                    else classe = "L1X";
                                    if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
                                    {
                                        i = int.MaxValue - 2;
                                        j = int.MaxValue - 2;
                                        break;
                                    }
                                    // On traite les unités
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    //									t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
                                    t.Append("\r\n\t\t<td class=\"" + classe + "\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"" + classe + "nb\" nowrap>" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 10 + k].ToString(), webSession.Unit) + "</td>");
                                    }
                                    //saut jusquà la 1ère colonne du calendrier d'action
                                    j = j + 10 + nbColYear;
                                    //fin MAJ								
                                }
                                break;
                            #endregion

                            #region Level 2
                            case DetailledMediaPlan.L2_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    // On traite les unités
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    t.Append("\r\n\t\t<td class=\"L2X\">&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX] + "</td><td class=\"L2Xnb\">" + totalUnit + "</td><td class=\"L2Xnb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"L2Xnb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 9 + k].ToString(), webSession.Unit) + "</td>");
                                    }
                                    //saut jusquà la 1ère colonne du calendrier d'action
                                    j = j + 9 + nbColYear;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 3
                            case DetailledMediaPlan.L3_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    // On traite les unités
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    t.Append("\r\n\t\t<td class=\"L3X\">&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX] + "</td><td class=\"L3Xnb\">" + totalUnit + "</td><td class=\"L3Xnb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"L3Xnb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 8 + k].ToString(), webSession.Unit) + "</td>");
                                    }
                                    //saut jusquà la 1ère colonne du calendrier d'action
                                    j = j + 8 + nbColYear;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 4
                            case DetailledMediaPlan.L4_COLUMN_INDEX:
                                // On traite le cas des pages
                                totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                t.Append("\r\n\t\t<td class=\"L4X\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td><td class=\"L4Xnb\">" + totalUnit + "</td><td class=\"L4Xnb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                //MAJ GR : totaux par années si nécessaire
                                for (k = 1; k <= nbColYear; k++)
                                {
                                    t.Append("<td class=\"L4Xnb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 7 + k].ToString(), webSession.Unit) + "</td>");
                                }
                                //fin MAJ
                                //saut jusquà la 1ère colonne du calendrier d'action
                                j = j + 7 + nbColYear;
                                //fin MAJ
                                break;
                            #endregion

                            default:
                                if (tab[i, j] == null)
                                {
                                    t.Append("<td class=\"p3\">&nbsp;</td>");
                                    break;
                                }
                                if (tab[i, j].GetType() == typeof(MediaPlanItem))
                                {
                                    switch (((MediaPlanItem)tab[i, j]).GraphicItemType)
                                    {
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
                                            if (showValue) t.Append("<td class=\"" + presentClass + "\">" + Functions.Units.ConvertUnitValueToString(((MediaPlanItem)tab[i, j]).Unit.ToString(), webSession.Unit) + "</td>");
                                            else t.Append("<td class=\"" + presentClass + "\">" + stringItem + "</td>");
                                            break;
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
                                            t.Append("<td class=\"" + extendedClass + "\">&nbsp;</td>");
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
            catch (System.Exception e)
            {
                throw (new System.Exception("erreur i=" + i + ",j=" + j, e));
            }
            t.Append("</tr></table></td></tr></table>");
            #endregion

            // On vide le tableau
            tab = null;
			t.Append(ExcelFunction.GetFooter(webSession));
            return (t.ToString());

        }
        #endregion

        #region Plan Media AdNetTrack Ms Excel (Web)
        /// <summary>
        /// Génère le code HTML pour Ms Excel pour afficher un calendrier d'action AdNetTrack détaillé en jour entre les dates de la session.
        /// Elles se base sur un tableau contenant les données
        /// tab vide : message "Pas de données" + bouton retour
        /// tab non vide:
        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
        ///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
        ///		Génération du code HTML des entêtes de colonne
        ///		Génération du code HTML des périodes (Libellé du mois, intitulés d'une unité de période)
        ///		Génération du code HTML du calendrier d'action
        /// </summary>
        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
        /// <param name="webSession">Session du client</param>
        /// <returns>Code Généré</returns>
        /// <remarks>
        /// Utilise les méthodes:
        ///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
        ///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
        /// </remarks>
        public static MediaPlanResultData GetAdNetTrackMediaPlanAlertWithMediaDetailLevelExcelUI(object[,] tab, WebSession webSession)
        {

            #region Pas de données à afficher
            MediaPlanResultData mediaPlanResultData = new MediaPlanResultData();
            if (tab.GetLength(0) == 0)
            {
                mediaPlanResultData.HTMLCode = "<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage) + "</div>";
                return (mediaPlanResultData);
            }
            #endregion

            #region Variables
            //MAJ GR : Colonnes totaux par année si nécessaire
            int k = 0;
            //int nbColYear = 0;
            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. 
            int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0, 4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0, 4));
            if (nbColYear > 0) nbColYear++;
            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX + nbColYear;
            //fin MAJ

            string classe;
            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
            //affichée, utilisez getLength-1
            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            int nbColTab = tab.GetLength(1), j, i;
            int nbline = tab.GetLength(0);
            //MAJ GR : FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
            int nbPeriod = nbColTab - FIRST_PERIOD_INDEX - 1;
            //fin MAJ
            string currentCategoryName = "tit";
            int colorItemIndex = 1;
            int colorNumberToUse = 0;
            int sloganIndex = -1;
            //webSession.SloganColors.Clear();
            try { webSession.SloganColors.Add((Int64)0, "pc0"); }
            catch (System.Exception) { }
            string presentClass = "p4";
            string extendedClass = "p5";
            string stringItem = "&nbsp;";

            #endregion

			
            #region Début du tableau
            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
            t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"ptX\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            #endregion

            #region Période
            string dateHtml = "<tr>";
            string headerHtml = "";
            //DateTime currentDay = new DateTime(((DateTime)tab[0,FIRST_PERIOD_INDEX]).Ticks);
            DateTime currentDay = DateString.YYYYMMDDToDateTime((string)tab[0, FIRST_PERIOD_INDEX]);
            //new DateTime(int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4)),int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(4,2)),int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(6,2)));
            int previousMonth = currentDay.Month;
            currentDay = currentDay.AddDays(-1);
            int nbPeriodInMonth = 0;
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                currentDay = currentDay.AddDays(1);
                if (currentDay.Month != previousMonth)
                {
                    if (nbPeriodInMonth >= 8)
                    {
                        headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"ptX\" align=center>"
                            + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM")))
                            + "</td>";
                    }
                    else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"ptX\" align=center>"
                             + "&nbsp"
                             + "</td>";
                    nbPeriodInMonth = 0;
                    previousMonth = currentDay.Month;
                }
                nbPeriodInMonth++;
                dateHtml += "<td class=\"pp\">&nbsp;" + currentDay.ToString("dd") + "&nbsp;</td>";
            }
            if (nbPeriodInMonth >= 8) headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"ptX\" align=center>"
                                          + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM")))
                                          + "</td>";
            else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"ptX\" align=center>"
                     + "&nbsp"
                     + "</td>";
            dateHtml += "</tr>";

            string dayClass = "";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
            dateHtml += "\r\n\t<tr>";
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                DateTime dt = (new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2))));
                if (dt.DayOfWeek == DayOfWeek.Saturday
                    || dt.DayOfWeek == DayOfWeek.Sunday
                    ) {
                    dayClass = "pdwX";
                }
                else
                {
                    dayClass = "pdX";
                }
                dateHtml += "<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>";
            }
            dateHtml += "\r\n\t</tr>";

            headerHtml += "</tr>";
			
            t.Append(headerHtml);
            t.Append(dateHtml);
            #endregion

            #region Calendrier d'actions
            i = 0;
            try
            {
                sloganIndex = GetSloganIdIndex(webSession);
                for (i = 1; i < nbline; i++)
                {

                    #region Gestion des couleurs des accroches
                    //					if(sloganIndex!=-1	&& tab[i,sloganIndex]!=null && 
                    //						((webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan)==webSession.GenericMediaDetailLevel.GetNbLevels)||
                    //						(webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan)<webSession.GenericMediaDetailLevel.GetNbLevels && tab[i,sloganIndex+1]==null))){
                    //						if(!webSession.SloganColors.ContainsKey((Int64)tab[i,sloganIndex])){
                    //							colorNumberToUse=(colorItemIndex%30)+1;
                    //							webSession.SloganColors.Add((Int64)tab[i,sloganIndex],"pc"+colorNumberToUse.ToString());
                    //							mediaPlanResultData.VersionsDetail.Add((Int64)tab[i,sloganIndex],new VersionItem((Int64)tab[i,sloganIndex],"pc"+colorNumberToUse.ToString())); //TODO
                    //							colorItemIndex++;
                    //						}
                    //						if((Int64)tab[i,sloganIndex]!=0 && !mediaPlanResultData.VersionsDetail.ContainsKey((Int64)tab[i,sloganIndex])){
                    //							mediaPlanResultData.VersionsDetail.Add((Int64)tab[i,sloganIndex],new VersionItem((Int64)tab[i,sloganIndex],webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString()));
                    //						}
                    //						presentClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
                    //						extendedClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
                    //						stringItem="x";
                    //					}
                    //					else{
                    //						presentClass="p4";
                    //						extendedClass="p5";
                    //						stringItem="&nbsp;";
                    //					}
                    #endregion

                    for (j = 0; j < nbColTab; j++)
                    {
                        switch (j)
                        {
                            #region Level 1
                            case DetailledMediaPlan.L1_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    //vérification que la catégorie n est pas les chaines thematiques

                                    if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) classe = "L0X";
                                    else
                                    {
                                        classe = "L1X";
                                        tmpHtml = "";
                                    }
                                    if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
                                    {
                                        i = int.MaxValue - 2;
                                        j = int.MaxValue - 2;
                                        break;
                                    }
                                    // On traite le cas des pages
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"" + classe + "\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX] + "</td>");
                                    j = j + 10;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 2
                            case DetailledMediaPlan.L2_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2X\">&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX] + "</td>");
                                    currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX].ToString();
                                    j = j + 9;
                                }
                                break;
                            #endregion

                            #region Level 3
                            case DetailledMediaPlan.L3_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3X\">&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX] + "</td>");
                                    currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX].ToString();
                                    j = j + 8;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 4
                            case DetailledMediaPlan.L4_COLUMN_INDEX:
                                t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4X\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td>");
                                j = j + 7;//nbColYear ajouter GF
                                break;
                            #endregion
                            default:
                                if (tab[i, j] == null)
                                {
                                    t.Append("<td class=\"p3\">&nbsp;</td>");
                                    break;
                                }
                                if (tab[i, j].GetType() == typeof(MediaPlanItem))
                                {
                                    switch (((MediaPlanItem)tab[i, j]).GraphicItemType)
                                    {
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
                                            t.Append("<td class=\"" + presentClass + "\">" + stringItem + "</td>");
                                            break;
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
                                            t.Append("<td class=\"" + extendedClass + "\">&nbsp;</td>");
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
            catch (System.Exception err)
            {
                throw (new WebExceptions.MediaPlanUIException("erreur i=" + i + ",j=" + j, err));
            }
            //t.Append("</table></td></tr></table>");			
            t.Append("</table>");
            #endregion
		
            // On vide le tableau
            tab = null;
			mediaPlanResultData.HTMLCode = t.ToString();
            return (mediaPlanResultData);

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
        public static string GetMediaPlanAlertExcelUI(object[,] tab, WebSession webSession, string periodBeginning, string periodEnd)
        {

            #region Variables
            string classe;
            //MAJ GR : Colonnes totaux par année si nécessaire
            int k = 0;
            //Bug problème année
            //int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
            int nbColYear = int.Parse(periodEnd.Substring(0, 4)) - int.Parse(periodBeginning.Substring(0, 4));

            if (nbColYear > 0) nbColYear++;
            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX + nbColYear;
            //fin MAJ

            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
            //affichée, utilisez getLength-1
            int nbColTab = tab.GetLength(1), j, i;
            int nbline = tab.GetLength(0);
            int nbPeriod = nbColTab - FIRST_PERIOD_INDEX - 1;
            bool premier = true;
            const string PLAN_MEDIA_XLS_1_CLASSE = "pmmediaxls1";
            const string PLAN_MEDIA_XLS_2_CLASSE = "pmmediaxls2";
            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            string totalUnit = "";
            #endregion

            #region Rappel des paramètres
            // Paramètres du tableau
            t.Append(ExcelFunction.GetLogo(webSession));
            t.Append(ExcelFunction.GetExcelHeader(webSession, true, periodBeginning, periodEnd));
            
            #endregion

            #region debut Tableau
            t.Append("<table border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
            t.Append("\r\n\t\t<td rowspan=\"3\" width=\"200px\" class=\"p2\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + GestionWeb.GetWebWord(805, webSession.SiteLanguage));
            t.Append("</td>");
            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
            // MAJ GR : On affiche les années si nécessaire
            for (k = FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++)
            {
                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">" + tab[0, k].ToString() + "</td>");
            }
            //fin MAJ
            #endregion

            #region Période
            string dateHtml = "<tr>";
            string headerHtml = "";
            DateTime currentDay = new DateTime(((DateTime)tab[0, MediaPlanAlert.FIRST_PEDIOD_INDEX]).Ticks);
            int previousMonth = currentDay.Month;
            currentDay = currentDay.AddDays(-1);
            int nbPeriodInMonth = 0;
            for (j = MediaPlanAlert.FIRST_PEDIOD_INDEX; j < nbColTab; j++)
            {
                currentDay = currentDay.AddDays(1);
                if (currentDay.Month != previousMonth)
                {
                    if (nbPeriodInMonth >= 8)
                    {
                        headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"p2\" align=center>"
                            + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM")))
                            + "</td>";
                    }
                    else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"p2\" align=center>"
                             + "&nbsp"
                             + "</td>";
                    nbPeriodInMonth = 0;
                    previousMonth = currentDay.Month;
                }
                nbPeriodInMonth++;
                dateHtml += "<td class=\"p10\">" + currentDay.ToString("dd") + "</td>";
            }
            if (nbPeriodInMonth >= 8) headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"p2\" align=center>"
                                          + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM")))
                                          + "</td>";
            else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"p2\" align=center>"
                     + "&nbsp"
                     + "</td>";
            dateHtml += "</tr>";
            // Jour de la semaine
            string dayClass = "";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
            dateHtml += "\r\n\t<tr>";
            for (j = MediaPlanAlert.FIRST_PEDIOD_INDEX; j < nbColTab; j++)
            {
                DateTime dt = ((DateTime)tab[0, j]);
                if (dt.DayOfWeek == DayOfWeek.Saturday
                    || dt.DayOfWeek == DayOfWeek.Sunday
                    ) {
                    dayClass = "p133";
                }
                else
                {
                    dayClass = "p132";
                }
                dateHtml += "<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>";
            }
            dateHtml += "\r\n\t</tr>";


            headerHtml += "</tr>";
            t.Append(headerHtml);
            t.Append(dateHtml);
            #endregion

            #region Tableau
            i = 0;
            try
            {
                for (i = 1; i < nbline; i++)
                {
                    for (j = 0; j < nbColTab; j++)
                    {
                        switch (j)
                        {
                            // Total ou vehicle
                            case MediaPlanAlert.VEHICLE_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    if (i == MediaPlanAlert.TOTAL_LINE_INDEX) classe = "pmtotal";
                                    else classe = "pmvehicle";
                                    if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
                                    {
                                        i = int.MaxValue - 2;
                                        j = int.MaxValue - 2;
                                        break;
                                    }
                                    // On traite les unités
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    //									t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
                                    t.Append("\r\n\t\t<td class=\"" + classe + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX] + "</td><td class=\"" + classe + "\">" + totalUnit + "</td><td class=\"" + classe + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"" + classe + "nb\">" + ((double)tab[i, j + 8 + k]).ToString("0.##") + "</td>");
                                    }
                                    //saut jusquà la 1ère colonne du calendrier d'action
                                    j = j + 8 + nbColYear;
                                    //fin MAJ								
                                }
                                break;
                            // Category
                            case MediaPlanAlert.CATEGORY_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    // On traite les unités
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    t.Append("\r\n\t\t<td class=\"pmcategory\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX] + "</td><td class=\"pmcategory\">" + totalUnit + "</td><td class=\"pmcategory\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"pmcategorynb\">" + ((double)tab[i, j + 7 + k]).ToString("0.##") + "</td>");
                                    }
                                    //saut jusquà la 1ère colonne du calendrier d'action
                                    j = j + 7 + nbColYear;
                                    //fin MAJ
                                }
                                break;
                            // Media
                            case MediaPlanAlert.MEDIA_COLUMN_INDEX:
                                // On traite le cas des pages
                                totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                if (premier)
                                {
                                    t.Append("\r\n\t\t<td class=\"" + PLAN_MEDIA_XLS_1_CLASSE + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX] + "</td><td class=\"" + PLAN_MEDIA_XLS_1_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_XLS_1_CLASSE + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"" + PLAN_MEDIA_XLS_1_CLASSE + "\">" + ((double)tab[i, j + 6 + k]).ToString("0.##") + "</td>");
                                    }
                                    //fin MAJ
                                    premier = false;
                                }
                                else
                                {
                                    t.Append("\r\n\t\t<td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">" + tab[i, FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX] + "</td><td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">" + totalUnit + "</td><td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">" + ((double)tab[i, FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"" + PLAN_MEDIA_XLS_2_CLASSE + "\">" + ((double)tab[i, j + 6 + k]).ToString("0.##") + "</td>");
                                    }
                                    //fin MAJ
                                    premier = true;
                                }
                                // MAJ GR
                                //saut jusquà la 1ère colonne du calendrier d'action
                                j = j + 6 + nbColYear;
                                //fin MAJ
                                break;
                            default:
                                if (tab[i, j] == null)
                                {
                                    t.Append("<td class=\"p3\">&nbsp;</td>");
                                    break;
                                }
                                if (tab[i, j].GetType() == typeof(FrameWorkResultConstantes.MediaPlanAlert.graphicItemType))
                                {
                                    switch ((FrameWorkResultConstantes.MediaPlanAlert.graphicItemType)tab[i, j])
                                    {
                                        case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.present:
                                            t.Append("<td class=\"p4\">&nbsp;</td>");
                                            break;
                                        case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.extended:
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
            catch (System.Exception e)
            {
                throw (new System.Exception("erreur i=" + i + ",j=" + j, e));
            }
            t.Append("</tr></table></td></tr></table>");
            #endregion

            // On vide le tableau
            tab = null;
			t.Append(ExcelFunction.GetFooter(webSession));
            return (t.ToString());

        }
        #endregion

        #region Sortie PDF
        /// <summary>
        /// Génère le code HTML pour afficher un calendrier d'action détaillé en jour entre les dates de la session.
        /// Elles se base sur un tableau contenant les données
        /// tab vide : message "Pas de données" + bouton retour
        /// tab non vide:
        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
        ///		Génération du code HTML des entêtes de colonne
        ///		Génération du code HTML des périodes (Libellé du mois, intitulés d'une unité de période)
        ///		Génération du code HTML du calendrier d'action
        /// </summary>
        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="htmlHeader">Entête du fichier HTML</param>
        /// <param name="nbWeek">Nombre de semaines</param>
        /// <returns>Code Généré</returns>
        /// <remarks>
        /// Utilise les méthodes:
        ///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
        ///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
        /// </remarks>
        public static MediaPlanResultData GetMediaPlanAlertWithMediaDetailLevelPdfUI(object[,] tab, WebSession webSession, ref System.Text.StringBuilder htmlHeader, ref Int64 nbWeek, int idVehicle)
        {

            #region Pas de données à afficher
            MediaPlanResultData mediaPlanResultData = new MediaPlanResultData();
            if (tab.GetLength(0) == 0)
            {
                mediaPlanResultData.HTMLCode = "<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage) + "</div>";
                return (mediaPlanResultData);
            }
            #endregion

            #region Variables
            //MAJ GR : Colonnes totaux par année si nécessaire
            int k = 0;
            //int nbColYear = 0;
            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. 
            int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0, 4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0, 4));
            if (nbColYear > 0) nbColYear++;
            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX + nbColYear;
            //fin MAJ

            string classe;
            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
            //affichée, utilisez getLength-1
            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            int nbColTab = tab.GetLength(1), j, i;
            int nbline = tab.GetLength(0);
            //MAJ GR : FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
            int nbPeriod = nbColTab - FIRST_PERIOD_INDEX - 1;
            //fin MAJ
            string currentCategoryName = "tit";
            string totalUnit = "";
            int colorItemIndex = 1;
            int colorNumberToUse = 0;
            int sloganIndex = -1;
            //webSession.SloganColors.Clear();
            try { webSession.SloganColors.Add((Int64)0, "pc0"); }
            catch (System.Exception) { }
            string presentClass = "p4";
            string extendedClass = "p5";
            string stringItem = "&nbsp;";

            #endregion

            #region On calcule la taille de la colonne Total
            int nbtot = WebFunctions.Units.ConvertUnitValueToString(tab[1, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit).Length;
            int nbSpace = (nbtot - 1) / 3;
            int nbCharTotal = nbtot + nbSpace - 5;
            if (nbCharTotal < 5) nbCharTotal = 0;
            #endregion

            #region script
            // DAns composant: if (!page.IsClientScriptBlockRegistered("openCreation"))page.RegisterClientScriptBlock("openCreation",WebFunctions.Script.OpenCreation());
            #endregion

            #region Début du tableau
            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
            t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"pt\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            t.Append("\r\n\t\t<td rowspan=3 class=\"pt\">" + GestionWeb.GetWebWord(805, webSession.SiteLanguage));
            for (int h = 0; h < nbCharTotal; h++)
            {
                t.Append("&nbsp;");
            }
            t.Append("</td>");
            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
            //t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
            // MAJ GR : On affiche les années si nécessaire
            for (k = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++)
            {
                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + tab[0, k].ToString() + "</td>");
            }
            //fin MAJ
            #endregion

            #region Période
            string dateHtml = "<tr>";
            string headerHtml = "";
            //DateTime currentDay = new DateTime(((DateTime)tab[0,FIRST_PERIOD_INDEX]).Ticks);
            DateTime currentDay = DateString.YYYYMMDDToDateTime((string)tab[0, FIRST_PERIOD_INDEX]);
            //new DateTime(int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4)),int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(4,2)),int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(6,2)));
            int previousMonth = currentDay.Month;
            currentDay = currentDay.AddDays(-1);
            int nbPeriodInMonth = 0;
            nbWeek = (Int64)Math.Round((double)(nbColTab - FIRST_PERIOD_INDEX) / 7);
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                currentDay = currentDay.AddDays(1);
                if (currentDay.Month != previousMonth)
                {
                    if (nbPeriodInMonth >= 8)
                    {
                        headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                            + Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM"))
                            + "</td>";
                    }
                    else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                             + "&nbsp"
                             + "</td>";
                    nbPeriodInMonth = 0;
                    previousMonth = currentDay.Month;
                }
                nbPeriodInMonth++;
                dateHtml += "<td class=\"pp\">&nbsp;" + currentDay.ToString("dd") + "&nbsp;</td>";
            }
            if (nbPeriodInMonth >= 8) headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                                          + Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM"))
                                          + "</td>";
            else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"pt\" align=center>"
                     + "&nbsp"
                     + "</td>";
            dateHtml += "</tr>";

            string dayClass = "";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
            dateHtml += "\r\n\t<tr>";
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                DateTime dt = (new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2))));
                if (dt.DayOfWeek == DayOfWeek.Saturday
                    || dt.DayOfWeek == DayOfWeek.Sunday
                    ) {
                    dayClass = "pdw";
                }
                else
                {
                    dayClass = "pd";
                }
                dateHtml += "<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>";
            }
            dateHtml += "\r\n\t</tr>";

            headerHtml += "</tr>";
            t.Append(headerHtml);
            t.Append(dateHtml);
            #endregion

            htmlHeader.Append(t.ToString());

            #region Calendrier d'actions
            i = 0;
            try
            {
                sloganIndex = GetSloganIdIndex(webSession);
                for (i = 1; i < nbline; i++)
                {

                    #region Gestion des couleurs des accroches
                    if (sloganIndex != -1 && tab[i, sloganIndex] != null &&
                        ((webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) == webSession.GenericMediaDetailLevel.GetNbLevels) ||
                        (webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan) < webSession.GenericMediaDetailLevel.GetNbLevels && tab[i, sloganIndex + 1] == null)))
                    {
                        if (!webSession.SloganColors.ContainsKey((Int64)tab[i, sloganIndex]))
                        {
                            colorNumberToUse = (colorItemIndex % 30) + 1;
                            webSession.SloganColors.Add((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString());
                            switch ((DBCst.Vehicles.names)idVehicle) { 
                                case DBCst.Vehicles.names.directMarketing:
                                    mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportMDVersionItem((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString()));
                                    break;
                                case DBCst.Vehicles.names.outdoor:
                                    mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportOutdoorVersionItem((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString()));
                                    break;
                                default:
                                    mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportVersionItem((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString())); //TODO
                                    break;
                            }
                            colorItemIndex++;
                        }
                        if ((Int64)tab[i, sloganIndex] != 0 && !mediaPlanResultData.VersionsDetail.ContainsKey((Int64)tab[i, sloganIndex]))
                        {
                            switch ((DBCst.Vehicles.names)idVehicle) {
                                case DBCst.Vehicles.names.directMarketing:
                                    mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportMDVersionItem((Int64)tab[i, sloganIndex], webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString()));
                                    break;
                                case DBCst.Vehicles.names.outdoor:
                                     mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportOutdoorVersionItem((Int64)tab[i, sloganIndex], webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString()));
                                    break;
                                default:
                                     mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportVersionItem((Int64)tab[i, sloganIndex], webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString()));
                                    break;
                            }
                        }
                        presentClass = webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString();
                        extendedClass = webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString();
                        stringItem = "x";
                    }
                    else
                    {
                        presentClass = "p4";
                        extendedClass = "p5";
                        stringItem = "&nbsp;";
                    }
                    #endregion

                    for (j = 0; j < nbColTab; j++)
                    {
                        switch (j)
                        {
                            #region Level 1
                            case DetailledMediaPlan.L1_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    //vérification que la catégorie n est pas les chaines thematiques

                                    if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) classe = "L0";
                                    else
                                    {

                                        classe = "L1";
                                        //										if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]!=null)
                                        //											tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+",-1,-1,-1,-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        //										else
                                        //											tmpHtml="";
                                    }
                                    if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
                                    {
                                        i = int.MaxValue - 2;
                                        j = int.MaxValue - 2;
                                        break;
                                    }
                                    // On traite le cas des pages
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"" + classe + "export\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX] + "</td><td class=\"" + classe + "nbexport\">" + totalUnit + "</td><td class=\"" + classe + "nbexport\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");//<td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
                                    //t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+Int64.Parse(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ###")+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
                                    //aller aux colonnes du calendrier d'action
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"" + classe + "nbexport\" nowrap>" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 10 + k].ToString(), webSession.Unit) + "</td>");
                                    }
                                    j = j + 10 + nbColYear;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 2
                            case DetailledMediaPlan.L2_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    //vérification que la catégorie n est pas les chaines thematiques
                                    //if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+",-1,-1,-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                    // On traite le cas des pages
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2export\">&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX] + "</td><td class=\"L2nbexport\">" + totalUnit + "</td><td class=\"L2nbexport\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");//<td align=\"center\" class=\"L2\">"+tmpHtml+"</td>");
                                    currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX].ToString();
                                    //aller aux colonnes du calendrier d'action
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"L2nbexport\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 9 + k].ToString(), webSession.Unit) + "</td>");
                                    }
                                    j = j + 9 + nbColYear;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 3
                            case DetailledMediaPlan.L3_COLUMN_INDEX:
                                if (tab[i, j] != null)
                                {
                                    string tmpHtml = "";
                                    //vérification que la catégorie n est pas les chaines thematiques
                                    //if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]+",-1,-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                    // On traite le cas des pages
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3export\">&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX] + "</td><td class=\"L3nbexport\">" + totalUnit + "</td><td class=\"L3nbexport\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");//<td align=\"center\" class=\"L3\">"+tmpHtml+"</td>");
                                    currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX].ToString();
                                    //aller aux colonnes du calendrier d'action
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"L3nbexport\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 8 + k].ToString(), webSession.Unit) + "</td>");
                                    }
                                    j = j + 8 + nbColYear;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 4
                            case DetailledMediaPlan.L4_COLUMN_INDEX:
                                // On traite le cas des pages
                                totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                //On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
                                if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] != null)
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4export\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td><td class=\"L4nbexport\">" + totalUnit + "</td><td class=\"L4nbexport\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");//<td align=\"center\" class=\"L4\"><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX]+",-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
                                else
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4export\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td><td class=\"L4nbexport\">" + totalUnit + "</td><td class=\"L4nbexport\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");//<td align=\"center\" class=\"L4\"></td>");
                                //MAJ GR : totaux par années si nécessaire
                                for (k = 1; k <= nbColYear; k++)
                                {
                                    t.Append("<td class=\"L4nbexport\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 7 + k].ToString(), webSession.Unit) + "</td>");
                                }
                                //fin MAJ
                                j = j + 7 + nbColYear;//nbColYear ajouter GF
                                break;
                            #endregion
                            default:
                                if (tab[i, j] == null)
                                {
                                    t.Append("<td class=\"p3\">&nbsp;</td>");
                                    break;
                                }
                                if (tab[i, j].GetType() == typeof(MediaPlanItem))
                                {
                                    switch (((MediaPlanItem)tab[i, j]).GraphicItemType)
                                    {
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
                                            t.Append("<td class=\"" + presentClass + "\">" + stringItem + "</td>");
                                            break;
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
                                            t.Append("<td class=\"" + extendedClass + "\">&nbsp;</td>");
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
            catch (System.Exception err)
            {
                throw (new WebExceptions.MediaPlanUIException("erreur i=" + i + ",j=" + j, err));
            }
            //t.Append("</table></td></tr></table>");			
            t.Append("</table>");
            #endregion

            // On vide le tableau
            tab = null;
            mediaPlanResultData.HTMLCode = t.ToString();
            return (mediaPlanResultData);

        }
        #endregion

        #region Méthode interne
        /// <summary>
        /// Obtient la colonne contenant le id_slogan
        /// Si le détail support ne contient pas le niveau slogan, elle retoune -1
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <returns>Colonne contenant l'id_slogan, -1 si pas de slogan</returns>
        private static int GetSloganIdIndex(WebSession webSession)
        {
            int rank = webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan);
            switch (rank)
            {
                case 1:
                    return (DetailledMediaPlan.L1_ID_COLUMN_INDEX);
                case 2:
                    return (DetailledMediaPlan.L2_ID_COLUMN_INDEX);
                case 3:
                    return (DetailledMediaPlan.L3_ID_COLUMN_INDEX);
                case 4:
                    return (DetailledMediaPlan.L4_ID_COLUMN_INDEX);
                default:
                    return (-1);
            }
        }
        #endregion
    }
}
