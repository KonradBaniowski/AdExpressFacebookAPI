#region Informations
// Auteur: G. Facon 
// Date de création: 22/11/2007  
// Date de modification: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Constantes.FrameWork.Results;
using ConstantePeriod=TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork.Date;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Functions;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.UI.Results {
    /// <summary>
    /// Media Schedule user interface generator
    /// </summary>
    public class GenericMediaScheduleUI
    {

        #region HTML

        #region HTML UI
        /// <summary>
        /// Common Media Schedule UI
        /// </summary>
        /// <param name="tab">Data</param>
        /// <param name="webSession">Customer DB</param>
        /// <param name="period">Study period</param>
        /// <param name="zoom">Zoom param (only for links to other tables..)</param>
        /// <returns>HTML code</returns>
        public static MediaPlanResultData GetHtml(object[,] tab,WebSession webSession, MediaSchedulePeriod period, string zoom) {
            
            Int64 i = -1;
            return (GetHtml(tab, webSession, period, zoom, null, ref i, false, -1,false));

        }
        #endregion

        #region AdNetTrack HTML
        /// <summary>
        /// Génère le code HTML pour afficher un calendrier d'action sur une ou plusieurs périodes.
        /// Elles se base sur un tableau contenant les données
        /// tab vide : message "Pas de données"
        /// tab non vide:
        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
        ///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
        ///		Génération du code HTML des entêtes de colonne
        ///		Génération du code HTML du calendrier d'action
        /// </summary>
        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="zoomDate">Période à prendre en compte (un mois ou une semaine)</param>
        /// <param name="period">Périod d'étude</param>
        /// <returns>Code Généré</returns>
        public static MediaPlanResultData GetAdNetTrackHtml(WebSession webSession, MediaSchedulePeriod period, object[,] tab, string zoomDate)//, Page page, string url, string urlParameters, Int64 moduleId, int universId, string currentPage)
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            MediaPlanResultData mediaPlanResultData = new MediaPlanResultData();

            #region Pas de données à afficher
            if (tab.GetLength(0) == 0)
            {
                mediaPlanResultData.HTMLCode = string.Format("<div align=\"center\" class=\"txtViolet11Bold\">{0}<br><br></div>",
                    GestionWeb.GetWebWord(177, webSession.SiteLanguage));
                return (mediaPlanResultData);

            }
            #endregion

            #region Totaux / Années
            //MAJ GR : Colonnes totaux par année si nécessaire
            int k = 0;
            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
            int nbColYear = period.End.Year - period.Begin.Year;
            if (nbColYear > 0) nbColYear++;
            //int nbColYear = 0;
            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX + nbColYear;
            //fin MAJ
            #endregion

            #region Variables
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
            for (int h = 0; h < nbCharTotal; h++)
            {
                t.Append("&nbsp;");
            }
            t.Append("</td>");


            //// MAJ GR : On affiche les années si nécessaire
            //for (k = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++)
            //{
            //    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + tab[0, k].ToString() + "</td>");
            //    for (int h = 0; h < nbCharTotal + 5; h++)
            //    {
            //        t.Append("&nbsp;");
            //    }
            //    t.Append("</td>");
            //}
            ////fin MAJ

            #endregion         
          
            #region Période
            StringBuilder dateHtml = new StringBuilder();
            dateHtml.Append("<tr>");
            StringBuilder headerHtml = new StringBuilder();
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
                        headerHtml.AppendFormat("<td colspan=\"{0}\" class=\"pt\" align=center>{1}</td>",
                            nbPeriodInMonth,
                            Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM")));
                    }
                    else
                    {
                        headerHtml.AppendFormat("<td colspan=\"{0}\" class=\"pt\" align=center>&nbsp;</td>", nbPeriodInMonth);
                    }
                    nbPeriodInMonth = 0;
                    previousMonth = currentDay.Month;
                }
                nbPeriodInMonth++;
                dateHtml.AppendFormat("<td class=\"pp\">&nbsp;{0}&nbsp;</td>", currentDay.ToString("dd"));
            }
            if (nbPeriodInMonth >= 8)
            {
                headerHtml.AppendFormat("<td colspan=\"{0}\" class=\"pt\" align=center>{1}</td>",
                    nbPeriodInMonth,
                    Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM")));
            }
            else
            {
                headerHtml.AppendFormat("<td colspan=\"{0}\" class=\"pt\" align=center>&nbsp;</td>",
                    nbPeriodInMonth);
            }
            dateHtml.Append("</tr>");
            string dayClass = "";
            char day;
            dateHtml.Append("\r\n\t<tr>");
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                day = TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession, (new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2)))).DayOfWeek.ToString()).ToCharArray()[0];
                if (day == GestionWeb.GetWebWord(545, webSession.SiteLanguage).ToCharArray()[0]
                    || day == GestionWeb.GetWebWord(546, webSession.SiteLanguage).ToCharArray()[0]
                    )
                {
                    dayClass = "pdw";
                }
                else
                {
                    dayClass = "pd";
                }
                dateHtml.AppendFormat("<td class=\"{0}\">{1}</td>"
                    , dayClass
                    , day.ToString());
            }
            dateHtml.Append("\r\n\t</tr>");

            headerHtml.Append("</tr>");
            t.Append(headerHtml);
            t.Append(dateHtml);
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
                                    if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) classe = "L0";
                                    else
                                    {
                                        classe = "L1";
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

        #endregion

        #region Excel

        #region Excel UI
        /// <summary>
        /// Common Media Schedule UI
        /// </summary>
        /// <param name="tab">Data</param>
        /// <param name="webSession">Customer DB</param>
        /// <param name="period">Study period</param>
        /// <param name="zoom">Zoom param (only for links to other tables..)</param>
        /// <param name="showValue"></param>
        /// <param name="periodDisplayLevel">Niveau de détail de l'affichage des périodes</param>
        /// <returns>HTML code for Excel pages</returns>
        public static MediaPlanResultData GetExcel(object[,] tab, WebSession webSession, MediaSchedulePeriod period, string zoom, bool showValue, int periodDisplayLevel) {
            return GetExcel(tab, webSession, period, zoom, showValue, periodDisplayLevel, false);
        }

        /// <summary>
        /// Common Media Schedule UI
        /// </summary>
        /// <param name="tab">Data</param>
        /// <param name="webSession">Customer DB</param>
        /// <param name="period">Study period</param>
        /// <param name="zoom">Zoom param (only for links to other tables..)</param>
        /// <param name="showValue"></param>
        /// <param name="periodDisplayLevel">Niveau de détail de l'affichage des périodes</param>
        /// <param name="isCreative">True if result is creative plan media</param>
        /// <returns>HTML code for Excel pages</returns>
        public static MediaPlanResultData GetExcel(object[,] tab, WebSession webSession, MediaSchedulePeriod period, string zoom, bool showValue, int periodDisplayLevel, bool isCreative)
        {

            #region Pas de données à afficher
            MediaPlanResultData mediaPlanResultData = new MediaPlanResultData();
            if (tab.GetLength(0) == 0)
            {
                if (isCreative) {
                    mediaPlanResultData.HTMLCode = " <Style><!-- "
                        + " .txtViolet11Bold{ font-family: Arial, Helvetica, sans-serif; font-size: 11px; color: #644883; font-weight: bold; } "
                        + " --></Style> "
                        + " <div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage) + "</div> ";
                    return (mediaPlanResultData);
                }
                else {
                    mediaPlanResultData.HTMLCode = "<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage)
						//+ "<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/" + webSession.SiteLanguage + "/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/" + webSession.SiteLanguage + "/button/back_up.gif';\">"
						//+ "<img src=\"/Images/" + webSession.SiteLanguage + "/button/back_up.gif\" border=0 name=bouton></a>
						+"</div>";
                    return (mediaPlanResultData);
                }

            }
            #endregion

            #region Totaux / Années
            //MAJ GR : Colonnes totaux par année si nécessaire
            int k = 0;
            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. 
            int nbColYear = period.End.Year - period.Begin.Year;
            if (nbColYear > 0) nbColYear++;
            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX + nbColYear;
            //fin MAJ
            #endregion

            #region Variables
            string classe;
            int nbPeriodInYear = 0;
            string HTML = "";
            string HTML2 = "";
            string prevYearString;
            int prevYear = 0;
            // ?? A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
            // ?? que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
            // ?? affichée, utilisez getLength-1
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
            string periodClass;
            #endregion

            #region Rappel de sélection
            if (isCreative) {
                t.Append(ExcelFunction.GetExcelHeaderForCreativeMediaPlan(webSession));
            }
            else {
                t.Append(ExcelFunction.GetLogo());
                if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA) {
                    t.Append(ExcelFunction.GetExcelHeader(webSession, true, false, zoom, periodDisplayLevel));
                }
                else {
                    t.Append(ExcelFunction.GetExcelHeaderForMediaPlanPopUp(webSession, false, "", "", zoom, periodDisplayLevel));
                }
            }
            #endregion

            #region Colonnes

            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");

            #region Period
            switch (period.PeriodDetailLEvel)
            {
                case ConstantePeriod.DisplayLevel.monthly:
                case ConstantePeriod.DisplayLevel.weekly:

                    prevYear = int.Parse(tab[0, FIRST_PERIOD_INDEX].ToString().Substring(0, 4));
                    for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
                    {
                        if (prevYear != int.Parse(tab[0, j].ToString().Substring(0, 4)))
                        {
                            HTML2 += "<td width=\"" + nbPeriodInYear * 20 + "px\" colspan=" + nbPeriodInYear + " class=\"paX\">" + prevYear + "</td>";
                            nbPeriodInYear = 0;
                            prevYear = int.Parse(tab[0, j].ToString().Substring(0, 4));

                        }

                        switch (period.PeriodDetailLEvel)
                        {
                            case ConstantePeriod.DisplayLevel.monthly:

                                #region On gère l'affichage de la couleur de la période
                                // Si c'est la première période et la date de début n'est pas le premier du mois
                                periodClass = "ppX";
                                if ((j == FIRST_PERIOD_INDEX && period.Begin.Day != 1)
                                   || (j == (nbColTab - 1) && period.End.Day != period.End.AddDays(1 - period.End.Day).AddMonths(1).AddDays(-1).Day))
                                {
                                    periodClass = "ppiX";
                                }
                                // Si c'est la dernière période et la date de fin n'est pas le dernier jour du mois
                                #endregion

                                HTML += "<td style=\"width:15pt\" class=\"" + periodClass + "\">" + TNS.FrameWork.Date.MonthString.Get(int.Parse(tab[0, j].ToString().Substring(4, 2)), webSession.SiteLanguage, 1) + "</td>";

                                break;
                            case ConstantePeriod.DisplayLevel.weekly:
                                periodClass = "ppX";
                                if ((j == FIRST_PERIOD_INDEX && period.Begin.DayOfWeek != DayOfWeek.Monday)
                                   || (j == (nbColTab - 1) && period.End.DayOfWeek != DayOfWeek.Sunday))
                                {
                                    periodClass = "ppiX";
                                }

                                HTML += "<td style=\"width:15pt\" class=\"" + periodClass + "\">"+ tab[0,j].ToString().Substring(4,2) + "</td>";

                                break;

                        }
                        nbPeriodInYear++;
                    }
                    // On calcule la dernière date à afficher
                    HTML2 += "<td width=\"" + nbPeriodInYear * 20 + "px\" colspan=" + nbPeriodInYear + " class=\"paX\">" + prevYear + "</td>";

                    t.Append("\r\n\t\t<td rowspan=\"2\" width=\"230px\" class=\"ptX\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "</td>");
                    t.Append("\r\n\t\t<td rowspan=\"2\" class=\"ptX\">" + GestionWeb.GetWebWord(805, webSession.SiteLanguage) + "</td>");
                    t.Append("\r\n\t\t<td rowspan=\"2\" class=\"ptX\">" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");
                    // MAJ GR : On affiche les années si nécessaire
                    for (k = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++)
                    {
                        t.Append("\r\n\t\t<td rowspan=\"2\" class=\"ptX\">" + tab[0, k].ToString() + "</td>");
                    }


                    t.Append(HTML2 + "</tr><tr>");
                    t.Append(HTML + "</tr>");

                    break;
                case ConstantePeriod.DisplayLevel.dayly:
                    t.Append("\r\n\t\t<td rowspan=\"3\" width=\"200px\" class=\"ptX\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
                    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"ptX\">" + GestionWeb.GetWebWord(805, webSession.SiteLanguage));
                    t.Append("</td>");
                    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"ptX\">" + GestionWeb.GetWebWord(806, webSession.SiteLanguage) + "</td>");

                    // Years necessary if the period consists of several years
                    for (k = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k < FIRST_PERIOD_INDEX; k++)
                    {
                        t.Append("\r\n\t\t<td rowspan=\"3\" class=\"ptX\">" + tab[0, k].ToString() + "</td>");
                    }

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
                        dateHtml += "<td class=\"ppX\">&nbsp;" + currentDay.ToString("dd") + "&nbsp;</td>";
                    }
                    if (nbPeriodInMonth >= 8) headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"ptX\" align=center>"
                                                  + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM")))
                                                  + "</td>";
                    else headerHtml += "<td colspan=\"" + nbPeriodInMonth + "\" class=\"ptX\" align=center>"
                             + "&nbsp"
                             + "</td>";
                    dateHtml += "</tr>";

                    string dayClass = "";
                    char day;
                    dateHtml += "\r\n\t<tr>";
                    for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
                    {
                        day = TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession, (new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2)))).DayOfWeek.ToString()).ToCharArray()[0];
                        if (day == GestionWeb.GetWebWord(545, webSession.SiteLanguage).ToCharArray()[0]
                            || day == GestionWeb.GetWebWord(546, webSession.SiteLanguage).ToCharArray()[0]
                            )
                        {
                            dayClass = "pdwX";
                        }
                        else
                        {
                            dayClass = "pdX";
                        }
                        dateHtml += "<td class=\"" + dayClass + "\">" + TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession, (new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2)))).DayOfWeek.ToString()) + "</td>";
                    }
                    dateHtml += "\r\n\t</tr>";

                    headerHtml += "</tr>";
                    t.Append(headerHtml);
                    t.Append(dateHtml);
                    break;
            }
            #endregion

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
                                    //vérification que la catégorie n est pas les chaines thematiques
                                    if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) 
                                        classe = "L0X";
                                    else
                                    {
                                        classe = "L1X";
                                    }
                                    if (tab[i, j].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
                                    {
                                        i = int.MaxValue - 2;
                                        j = int.MaxValue - 2;
                                        break;
                                    }
                                    // On traite le cas des pages
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"" + classe + "\">" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX] + "</td><td class=\"" + classe + "nb\">" + totalUnit + "</td><td class=\"" + classe + "nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
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
                                    // On traite le cas des pages
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2X\">&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX] + "</td><td class=\"L2Xnb\">" + totalUnit + "</td><td class=\"L2Xnb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                    //aller aux colonnes du calendrier d'action
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"L2Xnb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 9 + k].ToString(), webSession.Unit) + "</td>");
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
                                    //vérification que la catégorie n est pas les chaines thematiques
                                    // On traite le cas des pages
                                    totalUnit = WebFunctions.Units.ConvertUnitValueToString(tab[i, FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(), webSession.Unit);
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3X\">&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX] + "</td><td class=\"L3Xnb\">" + totalUnit + "</td><td class=\"L3Xnb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                    currentCategoryName = tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX].ToString();
                                    //aller aux colonnes du calendrier d'action
                                    //MAJ GR : totaux par années si nécessaire
                                    for (k = 1; k <= nbColYear; k++)
                                    {
                                        t.Append("<td class=\"L3Xnb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 8 + k].ToString(), webSession.Unit) + "</td>");
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
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4X\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td><td class=\"L4Xnb\">" + totalUnit + "</td><td class=\"L4Xnb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                else
                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4X\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td><td class=\"L4Xnb\">" + totalUnit + "</td><td class=\"L4Xnb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>");
                                //MAJ GR : totaux par années si nécessaire
                                for (k = 1; k <= nbColYear; k++)
                                {
                                    t.Append("<td class=\"L4Xnb\">" + WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 7 + k].ToString(), webSession.Unit) + "</td>");
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
                                            if (showValue) 
                                                t.Append("<td class=\"" + presentClass + "\">" + Functions.Units.ConvertUnitValueToString(((MediaPlanItem)tab[i, j]).Unit.ToString(), webSession.Unit) + "</td>");
                                            else
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
                throw (new WebExceptions.MediaPlanUIException("erreur i=" + i));
            }
            //t.Append("</table></td></tr></table>");			
            t.Append("</table>");
            #endregion

            // On vide le tableau
            tab = null;
            if (!isCreative) {
                t.Append(ExcelFunction.GetFooter(webSession));
            }

            mediaPlanResultData.HTMLCode = t.ToString();
            return (mediaPlanResultData);

        }



        #endregion

        #region AdNetTrack Excel UI
        /// <summary>
        /// Génère le code HTML pour afficher un calendrier d'action sur une ou plusieurs périodes.
        /// Elles se base sur un tableau contenant les données
        /// tab vide : message "Pas de données"
        /// tab non vide:
        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
        ///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
        ///		Génération du code HTML des entêtes de colonne
        ///		Génération du code HTML du calendrier d'action
        /// </summary>
        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="zoomDate">Période à prendre en compte (un mois ou une semaine)</param>
        /// <param name="period">Périod d'étude</param>
        /// <returns>Code Généré</returns>
        public static MediaPlanResultData GetAdNetTrackExcel(WebSession webSession, MediaSchedulePeriod period, object[,] tab, string zoomDate)//, Page page, string url, string urlParameters, Int64 moduleId, int universId, string currentPage)
        {

            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            MediaPlanResultData mediaPlanResultData = new MediaPlanResultData();

            #region Pas de données à afficher
            if (tab.GetLength(0) == 0)
            {
                mediaPlanResultData.HTMLCode = string.Format("<div align=\"center\" class=\"txtViolet11Bold\">{0}<br><br></div>",
                    GestionWeb.GetWebWord(177, webSession.SiteLanguage));
                return (mediaPlanResultData);

            }
            #endregion

            #region Totaux / Années
            //MAJ GR : Colonnes totaux par année si nécessaire
            int k = 0;
            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
            int nbColYear = period.End.Year - period.Begin.Year;
            if (nbColYear > 0) nbColYear++;
            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX + nbColYear;
            //fin MAJ
            #endregion

            #region Variables
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
            for (int h = 0; h < nbCharTotal; h++)
            {
                t.Append("&nbsp;");
            }
            t.Append("</td>");


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

            #region Période
            StringBuilder dateHtml = new StringBuilder();
            dateHtml.Append("<tr>");
            StringBuilder headerHtml = new StringBuilder();
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
                        headerHtml.AppendFormat("<td colspan=\"{0}\" class=\"pt\" align=center>{1}</td>",
                            nbPeriodInMonth,
                            Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM")));
                    }
                    else
                    {
                        headerHtml.AppendFormat("<td colspan=\"{0}\" class=\"pt\" align=center>&nbsp;</td>", nbPeriodInMonth);
                    }
                    nbPeriodInMonth = 0;
                    previousMonth = currentDay.Month;
                }
                nbPeriodInMonth++;
                dateHtml.AppendFormat("<td class=\"pp\">&nbsp;{0}&nbsp;</td>", currentDay.ToString("dd"));
            }
            if (nbPeriodInMonth >= 8)
            {
                headerHtml.AppendFormat("<td colspan=\"{0}\" class=\"pt\" align=center>{1}</td>",
                    nbPeriodInMonth,
                    Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM")));
            }
            else
            {
                headerHtml.AppendFormat("<td colspan=\"{0}\" class=\"pt\" align=center>&nbsp;</td>",
                    nbPeriodInMonth);
            }
            dateHtml.Append("</tr>");
            string dayClass = "";
            char day;
            dateHtml.Append("\r\n\t<tr>");
            for (j = FIRST_PERIOD_INDEX; j < nbColTab; j++)
            {
                day = TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession, (new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2)))).DayOfWeek.ToString()).ToCharArray()[0];
                if (day == GestionWeb.GetWebWord(545, webSession.SiteLanguage).ToCharArray()[0]
                    || day == GestionWeb.GetWebWord(546, webSession.SiteLanguage).ToCharArray()[0]
                    )
                {
                    dayClass = "pdw";
                }
                else
                {
                    dayClass = "pd";
                }
                dateHtml.AppendFormat("<td class=\"{0}\">{1}</td>"
                    , dayClass
                    , day.ToString());
            }
            dateHtml.Append("\r\n\t</tr>");

            headerHtml.Append("</tr>");
            t.Append(headerHtml);
            t.Append(dateHtml);
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
                                    if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) classe = "L0";
                                    else
                                    {
                                        classe = "L1";
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

        #endregion

        #region Pdf
        /// <summary>
        /// Build HTML code for PDF export
        /// </summary>
        /// <param name="tab">Data</param>
        /// <param name="webSession">Customer Session</param>
        /// <param name="period">Study period</param>
        /// <param name="header">table header (out paramters)</param>
        /// <param name="nbWeek">Nb of displayed weeks (out parameters)</param>
        /// <returns>Media plan result (html code + versions codes if required)</returns>
        public static MediaPlanResultData GetPdf(object[,] tab, WebSession webSession, MediaSchedulePeriod period, StringBuilder header, ref Int64 nbWeek, int idVehicle)
        {
            return GetHtml(tab, webSession, period, string.Empty, header,ref nbWeek, true, idVehicle, false);
        }

        #endregion

        #region HTML Creative
        /// <summary>
        /// Common Media Schedule UI
        /// </summary>
        /// <param name="tab">Data</param>
        /// <param name="webSession">Customer DB</param>
        /// <param name="period">Study period</param>
        /// <param name="isCreative">True if result is a Creative PM</param>
        /// <returns>HTML code</returns>
        public static MediaPlanResultData GetHtml(object[,] tab, WebSession webSession, MediaSchedulePeriod period, bool isCreative) {

            Int64 i = -1;
            return (GetHtml(tab, webSession, period, "", null, ref i, false, -1, isCreative));

        }
        #endregion

        #region Méthode interne
        /// <summary>
        /// Obtient la colonne contenant le id_slogan
        /// Si le détail support ne contient pas le niveau slogan, elle retoune -1
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <returns>Colonne contenant l'id_slogan, -1 si pas de slogan</returns>
        private static int GetSloganIdIndex(WebSession webSession) {
            int rank=webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan);
            switch(rank) {
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
        /// <summary>
        /// Build HTML code for PDF export
        /// </summary>
        /// <param name="tab">Data</param>
        /// <param name="webSession">Customer Session</param>
        /// <param name="period">Study period</param>
        /// <param name="header">table header (out paramters)</param>
        /// <param name="nbWeek">Nb of displayed weeks (out parameters)</param>
        /// <param name="isExport">True if result is an export</param>
        /// <param name="zoom">Zoom if required</param>
        /// <param name="idVehicle">Identifiant du media</param>
        /// <param name="isCreative">True if result is a creative PM</param>
        /// <returns>Media plan result (html code + versions codes if required)</returns>
        private static MediaPlanResultData GetHtml(object[,] tab,WebSession webSession, MediaSchedulePeriod period, string zoom, StringBuilder header, ref Int64 nbWeek, bool isExport, int idVehicle, bool isCreative) {
            
            #region Pas de données à afficher
            MediaPlanResultData mediaPlanResultData=new MediaPlanResultData();
            if(tab.GetLength(0)==0) {
                if (isCreative) {
                    mediaPlanResultData.HTMLCode = "<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage)+"</div>";
                    return (mediaPlanResultData);
                }
                else {
                    mediaPlanResultData.HTMLCode = "<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage)
                        + "<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/" + webSession.SiteLanguage + "/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/" + webSession.SiteLanguage + "/button/back_up.gif';\">"
                        + "<img src=\"/Images/" + webSession.SiteLanguage + "/button/back_up.gif\" border=0 name=bouton></a></div>";
                    return (mediaPlanResultData);
                }
            }
            #endregion

            #region Totaux / Années
            //MAJ GR : Colonnes totaux par année si nécessaire
            int k=0;
            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. 
            int nbColYear = period.End.Year - period.Begin.Year;
            if(nbColYear>0) nbColYear++;
            int FIRST_PERIOD_INDEX=FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
            //fin MAJ
            #endregion

            #region Variables
            string classe;
            int nbPeriodInYear=0;
            string HTML="";
            string HTML2=""; 
            string prevYearString;
            int prevYear=0;
            // ?? A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
            // ?? que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
            // ?? affichée, utilisez getLength-1
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
            string periodClass;
            string tmpString = string.Empty;
            #endregion

            #region Module
            Int64 moduleId;
            //WebConstantes.Module.Type moduleType=(TNS.AdExpress.Web.Core.Navigation.ModulesList.GetModule(webSession.CurrentModule)).ModuleType;
            moduleId=WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;
            #endregion

            #region On calcule la taille de la colonne Total
            int nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit).Length;
            int nbSpace=(nbtot-1)/3;
            int nbCharTotal=nbtot+nbSpace-5;
            if(nbCharTotal<5) nbCharTotal=0;
            #endregion

            #region Colonnes

            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
            // Product Column (Force nowrap in this column)
            t.AppendFormat("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"pt\">" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "{0}</td>", (!isExport) ? string.Empty : "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
            // Total Column
            t.Append("\r\n\t\t<td rowspan=3 class=\"pt\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
            for(int h=0;h<nbCharTotal;h++) {
                t.Append("&nbsp;");
            }
            t.Append("</td>");
            //PDM
            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
            //Version
            bool showVersions=false;
            if (!isExport && !isCreative) {
                if (webSession.CustomerLogin.FlagsList[Flags.ID_SLOGAN_ACCESS_FLAG] != null) {
                    showVersions = true;
                    t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + GestionWeb.GetWebWord(1994, webSession.SiteLanguage) + "</td>");
                }
                // Insertions
                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">" + GestionWeb.GetWebWord(2245, webSession.SiteLanguage) + "</td>");
            }
            // Years necessary if the period consists of several years
            for(k=FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX;k<FIRST_PERIOD_INDEX;k++) {
                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">"+tab[0,k].ToString()+"</td>");
            }

            #region Period
            switch(period.PeriodDetailLEvel) {
                case ConstantePeriod.DisplayLevel.monthly:
                case ConstantePeriod.DisplayLevel.weekly:
                    prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
                    for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++) {
                        if(prevYear!=int.Parse(tab[0,j].ToString().Substring(0,4))) {
                            if(nbPeriodInYear<3) prevYearString="&nbsp;";//prevYear.ToString().Substring(3,1);
                            else prevYearString=prevYear.ToString();
                            HTML2+="<td colspan="+nbPeriodInYear+" class=\"pa1\">"+prevYearString+"</td>";
                            nbPeriodInYear=0;
                            prevYear=int.Parse(tab[0,j].ToString().Substring(0,4));

                        }

                        switch (period.PeriodDetailLEvel)
                        {
                            case ConstantePeriod.DisplayLevel.monthly:

                                #region On gère l'affichage de la couleur de la période
                                // Si c'est la première période et la date de début n'est pas le premier du mois
                                periodClass = "pp";
                                if ((j == FIRST_PERIOD_INDEX && period.Begin.Day != 1)
                                   || (j == (nbColTab - 1) && period.End.Day != period.End.AddDays(1 - period.End.Day).AddMonths(1).AddDays(-1).Day))
                                {
                                    periodClass = "ppi";
                                }
                                // Si c'est la dernière période et la date de fin n'est pas le dernier jour du mois
                                #endregion

                                if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_POTENTIELS 
                                    || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                                    || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                                    || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                                    || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE)
                                {
                                    HTML += "<td class=\"" + periodClass + "\" width=\"17px\"><a class=\"pp\" href=\"" + Links.MEDIA_SCHEDULE_POP_UP + "?idSession=" + webSession.IdSession + "&zoomDate=" + tab[0, j].ToString() + "\">&nbsp;" + TNS.FrameWork.Date.MonthString.Get(int.Parse(tab[0, j].ToString().Substring(4, 2)), webSession.SiteLanguage, 1) + "&nbsp;</td>";
                                }
                                else
                                {
                                    HTML += "<td class=\"" + periodClass + "\" width=\"17px\"><a class=\"pp\" href=\"" + Links.ZOOM_PLAN_MEDIA + "?idSession=" + webSession.IdSession + "&zoomDate=" + tab[0, j].ToString() + "\">&nbsp;" + TNS.FrameWork.Date.MonthString.Get(int.Parse(tab[0, j].ToString().Substring(4, 2)), webSession.SiteLanguage, 1) + "&nbsp;</td>";
                                }

                                break;
                            case ConstantePeriod.DisplayLevel.weekly:
                                periodClass = "pp";
                                if ((j == FIRST_PERIOD_INDEX && period.Begin.DayOfWeek != DayOfWeek.Monday)
                                   || (j == (nbColTab - 1) && period.End.DayOfWeek != DayOfWeek.Sunday))
                                {
                                    periodClass = "ppi";
                                }
                                if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_POTENTIELS
                                    || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                                    || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                                    || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                                    || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE)
                                {
                                    HTML += "<td class=\"" + periodClass + "\" width=\"17px\"><a class=\"pp\" href=\"" + Links.MEDIA_SCHEDULE_POP_UP + "?idSession=" + webSession.IdSession + "&zoomDate=" + tab[0, j].ToString() + "\">&nbsp;" + tab[0, j].ToString().Substring(4, 2) + "&nbsp;<a></td>";
                                }
                                else
                                {
                                    HTML += "<td class=\"" + periodClass + "\" width=\"17px\"><a class=\"pp\" href=\"" + Links.ZOOM_PLAN_MEDIA + "?idSession=" + webSession.IdSession + "&zoomDate=" + tab[0, j].ToString() + "\">&nbsp;" + tab[0, j].ToString().Substring(4, 2) + "&nbsp;<a></td>";
                                }
                                break;

                        }
                        nbPeriodInYear++;
                    }
                    // On calcule la dernière date à afficher
                    if(nbPeriodInYear<3) prevYearString="&nbsp;";//prevYear.ToString().Substring(3,1);
                    else prevYearString=prevYear.ToString();

                    HTML2+="<td colspan="+nbPeriodInYear+" class=\"pa\">"+prevYearString+"</td>";
                    t.Append(HTML2+"</tr><tr>");

                    t.Append(HTML+"</tr>");
                    if (isExport)
                    {
                        header.Append(t.ToString());
                        nbWeek = (Int64)Math.Round((double)(nbColTab - FIRST_PERIOD_INDEX) / 7);
                    }
                    t.Append("\r\n\t<tr>");

                    break;
                case ConstantePeriod.DisplayLevel.dayly:
                    string dateHtml="<tr>";
                    string headerHtml="";
                    DateTime currentDay=DateString.YYYYMMDDToDateTime((string)tab[0,FIRST_PERIOD_INDEX]);
                    int previousMonth=currentDay.Month;
                    currentDay=currentDay.AddDays(-1);
                    int nbPeriodInMonth=0;
                    for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++) {
                        currentDay=currentDay.AddDays(1);
                        if(currentDay.Month!=previousMonth) {
                            if(nbPeriodInMonth>=8) {
                                headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
                                    +Dates.getPeriodTxt(webSession,currentDay.AddDays(-1).ToString("yyyyMM"))
                                    +"</td>";
                            }
                            else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
                                     +"&nbsp"
                                     +"</td>";
                            nbPeriodInMonth=0;
                            previousMonth=currentDay.Month;
                        }
                        nbPeriodInMonth++;
                        dateHtml+="<td class=\"pp\">&nbsp;"+currentDay.ToString("dd")+"&nbsp;</td>";
                    }
                    if(nbPeriodInMonth>=8) headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
                                               +Dates.getPeriodTxt(webSession,currentDay.ToString("yyyyMM"))
                                               +"</td>";
                    else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
                             +"&nbsp"
                             +"</td>";
                    dateHtml+="</tr>";

                    string dayClass="";
                    char day;
                    dateHtml+="\r\n\t<tr>";
                    for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++) {
                        day=TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,(new DateTime(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)),int.Parse(tab[0,j].ToString().Substring(6,2)))).DayOfWeek.ToString()).ToCharArray()[0];
                        if(day==GestionWeb.GetWebWord(545,webSession.SiteLanguage).ToCharArray()[0]
                            ||day==GestionWeb.GetWebWord(546,webSession.SiteLanguage).ToCharArray()[0]
                            ) {
                            dayClass="pdw";
                        }
                        else {
                            dayClass="pd";
                        }
                        dateHtml+="<td class=\""+dayClass+"\">"+TNS.AdExpress.Web.Functions.Dates.getDayOfWeek(webSession,(new DateTime(int.Parse(tab[0,j].ToString().Substring(0,4)),int.Parse(tab[0,j].ToString().Substring(4,2)),int.Parse(tab[0,j].ToString().Substring(6,2)))).DayOfWeek.ToString())+"</td>";
                    }
                    dateHtml+="\r\n\t</tr>";

                    headerHtml+="</tr>";
                    t.Append(headerHtml);
                    t.Append(dateHtml);
                    if (isExport)
                    {
                        header.Append(t.ToString());
                        nbWeek = (Int64)Math.Round((double)(nbColTab - FIRST_PERIOD_INDEX) / 7);
                    }
                    break;
            }
            #endregion

            #endregion

            #region Calendrier d'actions
            i=0;
            try {
                sloganIndex=GetSloganIdIndex(webSession);
                for(i=1;i<nbline;i++) {

                    #region Gestion des couleurs des accroches
                    if(sloganIndex!=-1&&tab[i,sloganIndex]!=null&&
                        ((webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan)==webSession.GenericMediaDetailLevel.GetNbLevels)||
                        (webSession.GenericMediaDetailLevel.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan)<webSession.GenericMediaDetailLevel.GetNbLevels&&tab[i,sloganIndex+1]==null))) {
                        if(!webSession.SloganColors.ContainsKey((Int64)tab[i,sloganIndex])) {
                            colorNumberToUse=(colorItemIndex%30)+1;
                            webSession.SloganColors.Add((Int64)tab[i,sloganIndex],"pc"+colorNumberToUse.ToString());
                            if (!isExport && !isCreative) {
                                mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new VersionItem((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString())); //TODO
                            }
                            else if(isExport){
                                switch ((DBClassificationConstantes.Vehicles.names)idVehicle) {
                                    case DBClassificationConstantes.Vehicles.names.directMarketing:
                                        mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportMDVersionItem((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString()));
                                        break;
                                    case DBClassificationConstantes.Vehicles.names.outdoor:
                                        mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportOutdoorVersionItem((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString()));
                                        break;
                                    default:
                                        mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportVersionItem((Int64)tab[i, sloganIndex], "pc" + colorNumberToUse.ToString())); //TODO
                                        break;
                                }

                            }
                            colorItemIndex++;
                        }
                        if((Int64)tab[i,sloganIndex]!=0&&!mediaPlanResultData.VersionsDetail.ContainsKey((Int64)tab[i,sloganIndex])) {
                            if (!isExport && !isCreative) {
                                mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new VersionItem((Int64)tab[i, sloganIndex], webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString()));
                            }
                            else if(isExport){
                                switch ((DBClassificationConstantes.Vehicles.names)idVehicle) {
                                    case DBClassificationConstantes.Vehicles.names.directMarketing:
                                        mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportMDVersionItem((Int64)tab[i, sloganIndex], webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString()));
                                        break;
                                    case DBClassificationConstantes.Vehicles.names.outdoor:
                                        mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportOutdoorVersionItem((Int64)tab[i, sloganIndex], webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString()));
                                        break;
                                    default:
                                        mediaPlanResultData.VersionsDetail.Add((Int64)tab[i, sloganIndex], new ExportVersionItem((Int64)tab[i, sloganIndex], webSession.SloganColors[(Int64)tab[i, sloganIndex]].ToString()));
                                        break;
                                }

                            }
                        }
                        presentClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
                        extendedClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
                        stringItem="x";
                    }
                    else {
                        presentClass="p4";
                        extendedClass="p5";
                        stringItem="&nbsp;";
                    }
                    #endregion

                    for(j=0;j<nbColTab;j++) {
                        switch(j) {
                            #region Level 1
                            case DetailledMediaPlan.L1_COLUMN_INDEX:
                                if(tab[i,j]!=null) {
                                    string tmpHtml="";
                                    string tmpHtml2="";
                                    //vérification que la catégorie n est pas les chaines thematiques
                                    if (i == DetailledMediaPlan.TOTAL_LINE_INDEX) classe = "L0";
                                    else
                                    {
                                        classe = "L1";
                                        if (!isExport && !isCreative && tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] != null)
                                        {
                                            tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + ",-1,-1,-1,-1','" + zoom + "','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                            tmpHtml2 = "<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + ",-1,-1,-1,-1','" + zoom + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        }
                                    }
                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)) {
                                        i=int.MaxValue-2;
                                        j=int.MaxValue-2;
                                        break;
                                    }
                                    // On traite le cas des pages
                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
                                    t.AppendFormat("\r\n\t<tr>\r\n\t\t<td class=\"" + classe + "{0}\">" 
                                        + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX] + "</td><td class=\"" + classe + "nb{0}\">" 
                                        + totalUnit + "</td><td class=\"" + classe + "nb{0}\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" 
                                        + ((!isExport && !isCreative) ? ((showVersions) ? "<td align=\"center\" class=\"" + classe + "\">" + tmpHtml + "</td>" : "") + "<td align=\"center\" class=\"" + classe + "\">" + tmpHtml2 + "</td>" : string.Empty)
                                        ,(isExport)?"export":string.Empty);
                                    //t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+Int64.Parse(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ###")+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
                                    //aller aux colonnes du calendrier d'action
                                    //MAJ GR : totaux par années si nécessaire
                                    for(k=1;k<=nbColYear;k++) {
                                        tmpString = WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 10 + k].ToString(), webSession.Unit);
                                        if (!tmpString.TrimEnd(' ').Equals(""))
                                            t.AppendFormat("<td class=\"" + classe + "nb{0}\" nowrap>" + tmpString + "</td>", (isExport) ? "export" : string.Empty);
                                        else
                                            t.AppendFormat("<td class=\"" + classe + "nb{0}\" nowrap>&nbsp;</td>", (isExport) ? "export" : string.Empty);
                                    }
                                    j=j+10+nbColYear;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 2
                            case DetailledMediaPlan.L2_COLUMN_INDEX:
                                if(tab[i,j]!=null) {
                                    string tmpHtml="";
                                    string tmpHtml2="";
                                    //vérification que la catégorie n est pas les chaines thematiques
                                    //									if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+",-1,-1,-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                    if (!isExport && !isCreative && tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] != null) {
                                        tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + ",-1,-1,-1','" + zoom + "','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        tmpHtml2 = "<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + ",-1,-1,-1','" + zoom + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                    }
                                    // On traite le cas des pages
                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
                                    t.AppendFormat("\r\n\t<tr>\r\n\t\t<td class=\"L2{0}\">&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX] + "</td><td class=\"L2nb\">" + totalUnit + "</td><td class=\"L2nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((!isExport && !isCreative) ? ((showVersions) ? "<td align=\"center\" class=\"L2\">" + tmpHtml + "</td>" : "") + "<td align=\"center\" class=\"L2\">" + tmpHtml2 + "</td>" : string.Empty), (isExport) ? "export" : string.Empty);
                                    currentCategoryName=tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX].ToString();
                                    //aller aux colonnes du calendrier d'action
                                    //MAJ GR : totaux par années si nécessaire
                                    for(k=1;k<=nbColYear;k++) {
                                        tmpString = WebFunctions.Units.ConvertUnitValueToString(tab[i,j+9+k].ToString(),webSession.Unit);
                                        if (!tmpString.TrimEnd(' ').Equals(""))
                                            t.AppendFormat("<td class=\"L2nb{0}\">" + tmpString + "</td>", (isExport) ? "export" : string.Empty);
                                        else
                                            t.AppendFormat("<td class=\"L2nb{0}\">&nbsp;</td>", (isExport) ? "export" : string.Empty);
                                    }
                                    j=j+9+nbColYear;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 3
                            case DetailledMediaPlan.L3_COLUMN_INDEX:
                                if(tab[i,j]!=null) {
                                    string tmpHtml="";
                                    string tmpHtml2="";
                                    //vérification que la catégorie n est pas les chaines thematiques
                                    //									if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]+",-1,-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                    if (!isExport && !isCreative && tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] != null) {
                                        tmpHtml = "<a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + ",-1,-1','" + zoom + "','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                        tmpHtml2 = "<a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + ",-1,-1','" + zoom + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
                                    }
                                    // On traite le cas des pages
                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
                                    t.AppendFormat("\r\n\t<tr>\r\n\t\t<td class=\"L3{0}\">&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX] + "</td><td class=\"L3nb\">" + totalUnit + "</td><td class=\"L3nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((!isExport && !isCreative) ? ((showVersions) ? "<td align=\"center\" class=\"L3\">" + tmpHtml + "</td>" : "") + "<td align=\"center\" class=\"L3\">" + tmpHtml2 + "</td>" : string.Empty), (isExport) ? "export" : string.Empty);
                                    currentCategoryName=tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX].ToString();
                                    //aller aux colonnes du calendrier d'action
                                    //MAJ GR : totaux par années si nécessaire
                                    for(k=1;k<=nbColYear;k++) {
                                        tmpString = WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 8 + k].ToString(), webSession.Unit);
                                        if (!tmpString.TrimEnd(' ').Equals(""))
                                            t.AppendFormat("<td class=\"L3nb{0}\">" + tmpString + "</td>", (isExport) ? "export" : string.Empty);
                                        else
                                            t.AppendFormat("<td class=\"L3nb{0}\">&nbsp;</td>", (isExport) ? "export" : string.Empty);
                                    }
                                    j=j+8+nbColYear;
                                    //fin MAJ
                                }
                                break;
                            #endregion

                            #region Level 4
                            case DetailledMediaPlan.L4_COLUMN_INDEX:
                                // On traite le cas des pages
                                totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
                                //On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
                                if(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]!=null)
                                    t.AppendFormat("\r\n\t<tr>\r\n\t\t<td class=\"L4{0}\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td><td class=\"L4nb\">" + totalUnit + "</td><td class=\"L4nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((!isExport && !isCreative) ? ((showVersions) ? "<td align=\"center\" class=\"L4\"><a href=\"javascript:OpenCreatives('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX] + ",-1','" + zoom + "','-1','" + moduleId.ToString() + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>" : "") + "<td align=\"center\" class=\"L4\"><a href=\"javascript:OpenInsertions('" + webSession.IdSession + "','" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] + "," + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX] + ",-1','" + zoom + "');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>" : string.Empty), (isExport) ? "export" : string.Empty);
                                else
                                    t.AppendFormat("\r\n\t<tr>\r\n\t\t<td class=\"L4{0}\">&nbsp;&nbsp;&nbsp;" + tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX] + "</td><td class=\"L4nb\">" + totalUnit + "</td><td class=\"L4nb\">" + ((double)tab[i, FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00") + "</td>" + ((!isExport && !isCreative) ? ((showVersions) ? "<td align=\"center\" class=\"L4\"></td>" : "") + "<td align=\"center\" class=\"L4\"></td>" : string.Empty), (isExport) ? "export" : string.Empty);
                                //MAJ GR : totaux par années si nécessaire
                                for(k=1;k<=nbColYear;k++) {
                                    tmpString = WebFunctions.Units.ConvertUnitValueToString(tab[i, j + 7 + k].ToString(), webSession.Unit);
                                    if (!tmpString.TrimEnd(' ').Equals(""))
                                        t.AppendFormat("<td class=\"L4nb{0}\">" + tmpString + "</td>", (isExport) ? "export" : string.Empty);
                                    else
                                        t.AppendFormat("<td class=\"L4nb{0}\">&nbsp;</td>", (isExport) ? "export" : string.Empty);
                                }
                                //fin MAJ
                                j=j+7+nbColYear;//nbColYear ajouter GF
                                break;
                            #endregion
                            default:
                                if(tab[i,j]==null) {
                                    t.Append("<td class=\"p3\">&nbsp;</td>");
                                    break;
                                }
                                if(tab[i,j].GetType()==typeof(MediaPlanItem)) {
                                    switch(((MediaPlanItem)tab[i,j]).GraphicItemType) {
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
                                            t.Append("<td class=\""+presentClass+"\">"+stringItem+"</td>");
                                            break;
                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
                                            t.Append("<td class=\""+extendedClass+"\">&nbsp;</td>");
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
            catch(System.Exception err) {
                throw (new WebExceptions.MediaPlanUIException("erreur i="+i));
            }
            //t.Append("</table></td></tr></table>");			
            t.Append("</table>");
            #endregion

            // On vide le tableau
            tab=null;
            mediaPlanResultData.HTMLCode=t.ToString();
            return (mediaPlanResultData);

        }
        #endregion
    }
}
