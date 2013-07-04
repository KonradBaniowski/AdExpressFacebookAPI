using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using System.Text;
using System.Collections;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Controls.Selections {
    /// <summary>
    /// Composant pour la construction du code html pour le rappel de sélection des dates
    /// </summary>
    [DefaultProperty("Text"),
        ToolboxData("<{0}:DetailSelectionWebControl runat=server></{0}:DetailSelectionWebControl>")]
    public class DetailPeriodWebControl : System.Web.UI.WebControls.WebControl {

        #region Variables
        /// <summary>
        /// Session
        /// </summary>
        private WebSession _webSession;
        /// <summary>
        /// Date de début
        /// </summary>
        private string _periodBeginning = String.Empty;
        /// <summary>
        /// Date de fin
        /// </summary>
        private string _periodEnd = String.Empty;
        /// <summary>
        /// Display content
        /// </summary>
        private bool _displayContent = true;
        /// <summary>
        /// Format de date en texte
        /// </summary>
        private bool _dateFormatText = false;
        /// <summary>
        /// Style d'un titre
        /// </summary>
        private string _cssTitle = String.Empty;
        /// <summary>
        /// Style d'une donnée d'un titre
        /// </summary>
        private string _cssTitleData = String.Empty;
        /// <summary>
        /// Style de Background Color 
        /// </summary>
        private string _cssBackgroundColor = "";
        /// <summary>
        /// Style vérifié du titre global du tableau > GetCssTitleGlobalHTML()
        /// </summary>
        private string cssTitleGlobal = "";
        /// <summary>
        /// Style vérifié d'un titre > GetCssTitleHTML()
        /// </summary>
        private string cssTitle = "";
        /// <summary>
        /// Style vérifié d'une donnée d'un titre > GetCssTitleDataHTML()
        /// </summary>
        private string cssTitleData = "";
        /// <summary>
        /// Style vérifié du Background Color
        /// </summary>
        private string cssBackgroundColor = "";
        #endregion

        #region Accesseurs
        /// <summary>
        /// Session
        /// </summary>
        public WebSession WebSession {
            get { return _webSession; }
            set { 
                _webSession = value;
                _periodBeginning = _webSession.PeriodBeginningDate;
                _periodEnd = _webSession.PeriodEndDate;
            }
        }
        /// <summary>
        /// Date de début
        /// </summary>
        public string PeriodBeginning {
            get { return _periodBeginning; }
            set { _periodBeginning = value; }
        }
        /// <summary>
        /// Date de fin
        /// </summary>
        public string PeriodEnd {
            get { return _periodEnd; }
            set { _periodEnd = value; }
        }
        /// <summary>
        /// Get / Set display content
        /// </summary>
        public bool DisplayContent {
            get { return _displayContent; }
            set { _displayContent = value; }
        }
        /// <summary>
        /// Format de date en texte
        /// </summary>
        public bool DateFormatText {
            get { return _dateFormatText; }
            set { _dateFormatText = value; }
        }
        /// <summary>
        /// Style d'un titre
        /// </summary>
        public string CssTitle {
            get { return _cssTitle; }
            set {
                _cssTitle = value;
                cssTitle = GetCssTitleHTML();
            }
        }
        /// <summary>
        /// Style d'une donnée d'un titre
        /// </summary>
        public string CssTitleData {
            get { return _cssTitleData; }
            set {
                _cssTitleData = value;
                cssTitleData = GetCssTitleDataHTML();
            }
        }
        /// <summary>
        /// Style de Background color
        /// </summary>
        public string CssBackgroundColor {
            get { return _cssBackgroundColor; }
            set {
                _cssBackgroundColor = value;
                cssBackgroundColor = GetCssBackgroundColorHTML();
            }
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {
            output.Write(GetHeader());
        }
        #endregion

        #region Vérification des styles CSS
        /// <summary>
        /// Vérification du style
        /// </summary>
        /// <returns>HTML</returns>
        private string GetCssTitleHTML() {
            if (_cssTitle != null && _cssTitle.Length > 0) return ("class=\"" + _cssTitle + "\"");
            return ("");
        }
        /// <summary>
        /// Vérification du style
        /// </summary>
        /// <returns>HTML</returns>
        private string GetCssTitleDataHTML() {
            if (_cssTitleData != null && _cssTitleData.Length > 0) return ("class=\"" + _cssTitleData + "\"");
            return ("");
        }
        /// <summary>
        /// Vérification du style
        /// </summary>
        /// <returns>HTML</returns>
        private string GetCssBackgroundColorHTML() {
            if (_cssBackgroundColor != null && _cssBackgroundColor.Length > 0) return ("class=\"" + _cssBackgroundColor + "\"");
            return ("");
        }
        #endregion

        #region GetHeader
        /// <summary>
        /// Génère l'en tête html pour les exports Excel
        /// </summary>
        /// <returns>HTML</returns>
        public string GetHeader() {

            if (!_displayContent) return "";

            #region Variables
            StringBuilder t = new System.Text.StringBuilder();
            ArrayList detailSelections = null;
            Dictionary<WebConstantes.DetailSelection.Type, string> tdList = new Dictionary<WebConstantes.DetailSelection.Type, string>();
            string tmpHTML = string.Empty;
            #endregion

            try {

                #region Début du tableau
                t.Append("<div align=\"left\"><table class=\"" + CssBackgroundColor + "\" cellpadding=2 cellspacing=0>");
                #endregion

                //_webSession.CustomerLogin.ModuleList();
                TNS.AdExpress.Domain.Web.Navigation.Module currentModule = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
                if (currentModule.Id == WebConstantes.Module.Name.TENDACES
                    || (currentModule.Id == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE && _webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS)) return "";
                

                try {
                    detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation((int)_webSession.CurrentTab)).DetailSelectionItemsType;
                }
                catch (System.Exception) {
                    if (currentModule.Id == WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
                        detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation(5)).DetailSelectionItemsType;
                }

                if (_webSession.isDatesSelected()) {
                    tmpHTML = GetDateSelected(_webSession, currentModule, _dateFormatText, _periodBeginning, _periodEnd);
                    if (tmpHTML.Length > 0)
                        tdList.Add(WebConstantes.DetailSelection.Type.dateSelected, tmpHTML);
                }

                if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                    // Période de l'étude
                    if (_webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule) {
                        tmpHTML = GetStudyDate(_webSession);
                        if (tmpHTML.Length > 0)
                            tdList.Add(WebConstantes.DetailSelection.Type.studyDate, tmpHTML);
                    }

                    // Période comparative
                    if (_webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule) {
                        tmpHTML = GetComparativeDate(_webSession, currentModule.Id);
                        if (tmpHTML.Length > 0)
                            tdList.Add(WebConstantes.DetailSelection.Type.comparativeDate, tmpHTML);
                    }

                    // Type Sélection comparative
                    if (_webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule) {
                        tmpHTML = GetComparativePeriodTypeDetail(_webSession, currentModule.Id);
                        if (tmpHTML.Length > 0)
                            tdList.Add(WebConstantes.DetailSelection.Type.comparativePeriodType, tmpHTML);
                    }
                }
                else if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                    ) {
                    // Période de l'étude
                    if (_webSession.isStudyPeriodSelected()) {
                        tmpHTML = GetStudyDate(_webSession);
                        if (tmpHTML.Length > 0)
                            tdList.Add(WebConstantes.DetailSelection.Type.studyDate, tmpHTML);
                    }

                    // Période comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isPeriodComparative()) {
                            tmpHTML = GetComparativeDate(_webSession, currentModule.Id);
                            if (tmpHTML.Length > 0)
                                tdList.Add(WebConstantes.DetailSelection.Type.comparativeDate, tmpHTML);
                        }
                    }

                    // Type Sélection comparative
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isComparativePeriodTypeSelected()) {
                            tmpHTML = GetComparativePeriodTypeDetail(_webSession, currentModule.Id);
                            if (tmpHTML.Length > 0)
                                tdList.Add(WebConstantes.DetailSelection.Type.comparativePeriodType, tmpHTML);
                        }
                    }

                    // Type disponibilité des données
                    if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {
                        if (_webSession.isPeriodDisponibilityTypeSelected()) {
                            tmpHTML = GetPeriodDisponibilityTypeDetail(_webSession);
                            if (tmpHTML.Length > 0)
                                tdList.Add(WebConstantes.DetailSelection.Type.periodDisponibilityType, tmpHTML);
                        }
                    }
                }

                /*foreach (int currentType in detailSelections) {
                    switch ((WebConstantes.DetailSelection.Type)currentType) {
                        case WebConstantes.DetailSelection.Type.dateSelected:
                            tmpHTML = GetDateSelected(_webSession, currentModule, _dateFormatText, _periodBeginning, _periodEnd);
                            if(tmpHTML.Length>0)
                                tdList.Add(WebConstantes.DetailSelection.Type.dateSelected, tmpHTML);
                            break;
                        case WebConstantes.DetailSelection.Type.studyDate:
                            tmpHTML = GetStudyDate(_webSession);
                            if(tmpHTML.Length>0)
                                tdList.Add(WebConstantes.DetailSelection.Type.studyDate, tmpHTML);
                            break;
                        case WebConstantes.DetailSelection.Type.comparativeDate:
                            tmpHTML = GetComparativeDate(_webSession);
                            if (tmpHTML.Length > 0)
                                tdList.Add(WebConstantes.DetailSelection.Type.comparativeDate, tmpHTML);
                            break;
                        case WebConstantes.DetailSelection.Type.comparativePeriodType:
                            tmpHTML = GetComparativePeriodTypeDetail(_webSession);
                            if (tmpHTML.Length > 0)
                                tdList.Add(WebConstantes.DetailSelection.Type.comparativePeriodType, tmpHTML);
                            break;
                        case WebConstantes.DetailSelection.Type.periodDisponibilityType:
                            tmpHTML = GetPeriodDisponibilityTypeDetail(_webSession);
                            if (tmpHTML.Length > 0)
                                tdList.Add(WebConstantes.DetailSelection.Type.periodDisponibilityType, tmpHTML);
                            break;
                        default:
                            break;
                    }
                }*/

                t.Append("<tr>");
                t.Append(tdList[WebConstantes.DetailSelection.Type.dateSelected]);
                if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.comparativePeriodType))
                    t.Append(tdList[WebConstantes.DetailSelection.Type.comparativePeriodType]);
                t.Append("</tr>");

                t.Append("<tr>");
                if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.studyDate))
                    t.Append(tdList[WebConstantes.DetailSelection.Type.studyDate]);
                else if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.comparativeDate))
                    t.Append(tdList[WebConstantes.DetailSelection.Type.comparativeDate]);
                if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.periodDisponibilityType))
                    t.Append(tdList[WebConstantes.DetailSelection.Type.periodDisponibilityType]);
                t.Append("</tr>");

                t.Append("<tr>");
                if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.studyDate) && tdList.ContainsKey(WebConstantes.DetailSelection.Type.comparativeDate))
                    t.Append(tdList[WebConstantes.DetailSelection.Type.comparativeDate]);
                t.Append("</tr>");

                //t.Append(GetBlankLine());
                t.Append("</table></div><br>");

                // On libère htmodule pour pouvoir le sauvegarder dans les tendances
                //_webSession.CustomerLogin.HtModulesList.Clear();

                return Convertion.ToHtmlString(t.ToString());
            }
            catch (System.Exception err) {
                throw (new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des paramètres dans le fichier Excel", err));
            }
        }
        #endregion

        #region Méthodes internes d'affichage par rapport à la déclaration dans XML

        #region Dates sélectionnées
        /// <summary>
        /// Dates sélectionnées
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="currentModule">Module en cours</param>
        /// <param name="dateFormatText">Booléen date en format texte</param>
        /// <param name="periodBeginning">Date de début</param>
        /// <param name="periodEnd">Date de fin</param>
        /// <returns>HTML</returns>
        /// <remarks>Date format to be like for example novembre 2004 - janvier 2005</remarks>
        protected virtual string GetDateSelected(WebSession webSession, TNS.AdExpress.Domain.Web.Navigation.Module currentModule, bool dateFormatText, string periodBeginning, string periodEnd) {
            StringBuilder html = new StringBuilder();
            string startDate = "";
            string endDate = "";

            if ((currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO) && webSession.DetailPeriodBeginningDate.Length > 0 && webSession.DetailPeriodBeginningDate != "0" && webSession.DetailPeriodEndDate.Length > 0 && webSession.DetailPeriodEndDate != "0") {
                // Affichage de la période mensuelle si elle est sélectionné dans les options de résultat
                startDate = WebFunctions.Dates.getPeriodTxt(webSession, webSession.DetailPeriodBeginningDate);
                endDate = WebFunctions.Dates.getPeriodTxt(webSession, webSession.DetailPeriodEndDate);

                html.Append("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(1541, webSession.SiteLanguage) + " :</font> " + startDate);
                if (!startDate.Equals(endDate))
                    html.Append(" - " + endDate);
                html.Append("</td>");
            }
            else {
                if (dateFormatText) {
                    startDate = WebFunctions.Dates.getPeriodTxt(webSession, webSession.PeriodBeginningDate);
                    endDate = WebFunctions.Dates.getPeriodTxt(webSession, webSession.PeriodEndDate);

                    html.Append("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(1541, webSession.SiteLanguage) + " :</font> " + startDate);
                    if (!startDate.Equals(endDate))
                        html.Append(" - " + endDate);
                    html.Append("</td>");
                }
                else {
                    if (periodBeginning.Length == 0 || periodEnd.Length == 0) {
                        startDate = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType), webSession.SiteLanguage);
                        endDate = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType), webSession.SiteLanguage);

                        html.Append("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(1541, webSession.SiteLanguage) + " :</font> " + startDate);
                        if (!startDate.Equals(endDate))
                            html.Append(" - " + endDate);
                        html.Append("</td>");
                    }
                    else {
                        // Predefined date
                        startDate = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodBeginningDate(periodBeginning, webSession.PeriodType), webSession.SiteLanguage);
                        endDate = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodEndDate(periodEnd, webSession.PeriodType), webSession.SiteLanguage);

                        html.Append("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(1541, webSession.SiteLanguage) + " :</font> " + startDate);
                        if (!startDate.Equals(endDate))
                            html.Append(" - " + endDate);
                        html.Append("</td>");
                    }
                }
            }
            return (html.ToString());
        }
        #endregion

        #region Période de l'étude
        /// <summary>
        /// Generate html code for study period detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        private string GetStudyDate(WebSession webSession) {

            StringBuilder html = new StringBuilder();
            string startDate;
            string endDate;

            if (webSession.PeriodBeginningDate != webSession.CustomerPeriodSelected.StartDate || webSession.PeriodEndDate != webSession.CustomerPeriodSelected.EndDate) {

                startDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.StartDate.ToString(), webSession.SiteLanguage);
                endDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.EndDate.ToString(), webSession.SiteLanguage);

                html.Append("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2291, webSession.SiteLanguage) + " </font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + startDate);
                if (!startDate.Equals(endDate))
                    html.Append(" - " + endDate);
                html.Append("</td>");

            }

            return (html.ToString());

        }
        #endregion

        #region Péiode comparative
        /// <summary>
        /// Generate html code for comparative period detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        private string GetComparativeDate(WebSession webSession, long moduleId) {

            StringBuilder html = new StringBuilder();
            string startDate;
            string endDate;
            DateTime dateBeginDT;
            DateTime dateEndDT;


            if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                // get date begin and date end according to period type
                dateBeginDT = Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
                dateEndDT = Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);

                // get comparative date begin and date end
                dateBeginDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateBeginDT.Date, webSession.ComparativePeriodType);
                dateEndDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateEndDT.Date, webSession.ComparativePeriodType);

                // Formating date begin and date end
                startDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(dateBeginDT.ToString("yyyyMMdd"), webSession.SiteLanguage);
                endDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(dateEndDT.ToString("yyyyMMdd"), webSession.SiteLanguage);
            }
            else {
                startDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.ComparativeStartDate.ToString(), webSession.SiteLanguage);
                endDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.ComparativeEndDate.ToString(), webSession.SiteLanguage);
            }

            //startDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.ComparativeStartDate.ToString(), webSession.SiteLanguage);
            //endDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.ComparativeEndDate.ToString(), webSession.SiteLanguage);

            html.Append("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2292, webSession.SiteLanguage) + " </font>&nbsp; " + startDate);
            if (!startDate.Equals(endDate))
                html.Append(" - " + endDate);
            html.Append("</td>");

            return (html.ToString());

        }
        #endregion

        #region Type de la période comparative
        /// <summary>
        /// Generate html code for comparative period detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        private string GetComparativePeriodTypeDetail(WebSession webSession, long moduleId) {

            globalCalendar.comparativePeriodType comparativePeriodType;

            if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                comparativePeriodType = webSession.ComparativePeriodType;
            else
                comparativePeriodType = webSession.CustomerPeriodSelected.ComparativePeriodType;

            switch (comparativePeriodType) {
                case WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate:
                    return ("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2293, webSession.SiteLanguage) + "</font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + GestionWeb.GetWebWord(2295, webSession.SiteLanguage) + "</td>");
                case WebConstantes.globalCalendar.comparativePeriodType.dateToDate:
                    return ("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2293, webSession.SiteLanguage) + "</font>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + GestionWeb.GetWebWord(2294, webSession.SiteLanguage) + "</td>");
                default:
                    return "";
            }

        }
        #endregion

        #region Type de la disponibilité des données
        /// <summary>
        /// Generate html code for period disponibility type detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        private string GetPeriodDisponibilityTypeDetail(WebSession webSession) {

            switch (webSession.CustomerPeriodSelected.PeriodDisponibilityType) {
                case WebConstantes.globalCalendar.periodDisponibilityType.currentDay:
                    return ("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2296, webSession.SiteLanguage) + "</font> " + GestionWeb.GetWebWord(2297, webSession.SiteLanguage) + "</td>");
                case WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                    return ("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2296, webSession.SiteLanguage) + "</font> " + GestionWeb.GetWebWord(2298, webSession.SiteLanguage) + "</td>");
                default:
                    return "";
            }
        }
        #endregion

        #region Ligne séparatrice
        /// <summary>
        /// Ligne séparatrice
        /// </summary>
        /// <returns>HTML</returns>
        private static string GetBlankLine() {
            return ("<tr><td>&nbsp;</td></tr>");
        }
        #endregion

        #endregion

    }
}
