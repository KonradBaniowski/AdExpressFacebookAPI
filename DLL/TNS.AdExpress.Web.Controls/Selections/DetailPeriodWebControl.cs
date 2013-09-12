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
using TNS.AdExpress.Web.Core.Selection;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;
using DBConstantes = TNS.AdExpress.Constantes.DB;

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
        /// Module Id
        /// </summary>
        private Int64 _moduleId = -1;
        /// <summary>
        /// Date de début
        /// </summary>
        private string _periodBeginning = String.Empty;
        /// <summary>
        /// Date de fin
        /// </summary>
        private string _periodEnd = String.Empty;
        /// <summary>
        /// Tendencies Last Date
        /// </summary>
        private DateTime _tendenciesLastDate;
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
        /// Module Id
        /// </summary>
        public Int64 ModuleId {
            get { return _moduleId; }
            set {
                _moduleId = value;
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
            Dictionary<WebConstantes.DetailSelection.Type, string> tdList = new Dictionary<WebConstantes.DetailSelection.Type, string>();
            string tmpHTML = string.Empty;
            string colSpan = "8";
            bool addSpace = false;
            #endregion

            try {

                #region Début du tableau
                t.Append("<div align=\"left\"><table class=\"" + CssBackgroundColor + "\" cellpadding=2 cellspacing=0>");
                #endregion

                TNS.AdExpress.Domain.Web.Navigation.Module currentModule;

                if(_moduleId != -1)
                    currentModule = _webSession.CustomerLogin.GetModule(_moduleId);
                else
                    currentModule = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);

                if ((currentModule.Id == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE && _webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS)) return "";
                
                if (_webSession.isDatesSelected()) {
                    tmpHTML = GetDateSelected(_webSession, currentModule, _dateFormatText, _periodBeginning, _periodEnd);
                    if (tmpHTML.Length > 0)
                        tdList.Add(WebConstantes.DetailSelection.Type.dateSelected, tmpHTML);
                }

                if (currentModule.Id == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                    GetMediaScheduleDetail(ref tdList);
                }
                else if (currentModule.Id == WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE || currentModule.Id == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                      || currentModule.Id == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE || currentModule.Id == WebConstantes.Module.Name.ANALYSE_MANDATAIRES
                      || currentModule.Id == WebConstantes.Module.Name.INDICATEUR || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DYNAMIQUE
                      || currentModule.Id == WebConstantes.Module.Name.TENDACES || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO
                    || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO
                    || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE) {
                    GetCommonDetail(ref tdList, currentModule);
                }

                #region HTML core content
                t.Append("<tr>");
                if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.dateSelected) && !tdList.ContainsKey(WebConstantes.DetailSelection.Type.studyDate))
                    t.Append(tdList[WebConstantes.DetailSelection.Type.dateSelected]);
                else if(tdList.ContainsKey(WebConstantes.DetailSelection.Type.studyDate))
                    t.Append(tdList[WebConstantes.DetailSelection.Type.studyDate]);

                if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.comparativeDate))
                    t.Append(tdList[WebConstantes.DetailSelection.Type.comparativeDate]);
                t.Append("</tr>");

                t.Append("<tr>");
                if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.studyDate)) {
                    t.Append(tdList[WebConstantes.DetailSelection.Type.dateSelected]);
                    colSpan = "4";
                    addSpace = true;
                }

                if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.comparativePeriodType) && tdList.ContainsKey(WebConstantes.DetailSelection.Type.periodDisponibilityType))
                    t.Append("<td colspan=" + colSpan + " " + cssTitleData + "><font " + cssTitle + ">" + (addSpace  ? "&nbsp;" : "") + GestionWeb.GetWebWord(3001, _webSession.SiteLanguage) + " : </font>&nbsp; " + tdList[WebConstantes.DetailSelection.Type.comparativePeriodType] + "&nbsp;-&nbsp;"
                                + tdList[WebConstantes.DetailSelection.Type.periodDisponibilityType] + "</td>");
                else if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.comparativePeriodType))
                    t.Append("<td colspan=" + colSpan + " " + cssTitleData + "><font " + cssTitle + ">" + (addSpace ? "&nbsp;" : "") + GestionWeb.GetWebWord(3002, _webSession.SiteLanguage) + " : </font>&nbsp; " + tdList[WebConstantes.DetailSelection.Type.comparativePeriodType] + "</td>");
                else if (tdList.ContainsKey(WebConstantes.DetailSelection.Type.periodDisponibilityType))
                    t.Append("<td colspan=" + colSpan + " " + cssTitleData + "><font " + cssTitle + ">" + (addSpace ? "&nbsp;" : "") + GestionWeb.GetWebWord(3002, _webSession.SiteLanguage) + " : </font>&nbsp; " + tdList[WebConstantes.DetailSelection.Type.periodDisponibilityType] + "</td>");

                t.Append("</tr>");

                t.Append("</table></div><br>");
                #endregion

                return Convertion.ToHtmlString(t.ToString());
            }
            catch (System.Exception err) {
                throw (new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des paramètres dans le fichier Excel", err));
            }
        }
        #endregion

        #region Méthodes internes d'affichage par rapport à la déclaration dans XML

        #region Media Schedule Detail
        /// <summary>
        /// Get Media Schedule Detail
        /// </summary>
        /// <returns>HTML</returns>
        private void GetMediaScheduleDetail(ref Dictionary<WebConstantes.DetailSelection.Type, string> tdList) {

            TNS.AdExpress.Domain.Web.Navigation.Module currentModule = _webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA);
            string tmpHTML = string.Empty;

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
        #endregion

        #region Common Detail
        /// <summary>
        /// Get Common Detail
        /// </summary>
        /// <returns>HTML</returns>
        private void GetCommonDetail(ref Dictionary<WebConstantes.DetailSelection.Type, string> tdList, TNS.AdExpress.Domain.Web.Navigation.Module currentModule) {

            string tmpHTML = string.Empty;

            #region Période de l'étude
            if (_webSession.isStudyPeriodSelected()
                && _webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_MANDATAIRES
                && _webSession.CurrentModule != WebConstantes.Module.Name.INDICATEUR
                && _webSession.CurrentModule != WebConstantes.Module.Name.TABLEAU_DYNAMIQUE
                && _webSession.CurrentModule != WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE
                && _webSession.CurrentModule != WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO
                && _webSession.CurrentModule != WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION
                && _webSession.CurrentModule != WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO) {
                tmpHTML = GetStudyDate(_webSession);
                if (tmpHTML.Length > 0)
                    tdList.Add(WebConstantes.DetailSelection.Type.studyDate, tmpHTML);
            }
            #endregion

            #region Période comparative
            if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE) {
                if (_webSession.isPeriodComparative()) {
                    tmpHTML = GetComparativeDate(_webSession, currentModule.Id);
                    if (tmpHTML.Length > 0)
                        tdList.Add(WebConstantes.DetailSelection.Type.comparativeDate, tmpHTML);
                }
            }
            else if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES
                || _webSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR
                || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DYNAMIQUE
                || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE
                || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO
                || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION
                || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO
                ) {
                if (_webSession.ComparativeStudy) {
                    tmpHTML = GetComparativeDate(_webSession, currentModule.Id);
                    if (tmpHTML.Length > 0)
                        tdList.Add(WebConstantes.DetailSelection.Type.comparativeDate, tmpHTML);
                }
            }
            else if (_webSession.CurrentModule == WebConstantes.Module.Name.TENDACES) {
                tmpHTML = GetComparativeDate(_webSession, currentModule.Id);
                if (tmpHTML.Length > 0)
                    tdList.Add(WebConstantes.DetailSelection.Type.comparativeDate, tmpHTML);
            }
            #endregion

            #region Type Sélection comparative
            if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE) {
                if (_webSession.isComparativePeriodTypeSelected()) {
                    tmpHTML = GetComparativePeriodTypeDetail(_webSession, currentModule.Id);
                    if (tmpHTML.Length > 0)
                        tdList.Add(WebConstantes.DetailSelection.Type.comparativePeriodType, tmpHTML);
                }
            }
            else if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES) {
                if (_webSession.ComparativeStudy && _webSession.isComparativePeriodTypeSelected()) {
                    tmpHTML = GetComparativePeriodTypeDetail(_webSession, currentModule.Id);
                    if (tmpHTML.Length > 0)
                        tdList.Add(WebConstantes.DetailSelection.Type.comparativePeriodType, tmpHTML);
                }
            }
            else if (_webSession.CurrentModule == WebConstantes.Module.Name.TENDACES) {
                tmpHTML = GetComparativePeriodTypeDetail(_webSession, currentModule.Id);
                if (tmpHTML.Length > 0)
                    tdList.Add(WebConstantes.DetailSelection.Type.comparativePeriodType, tmpHTML);
            }
            else if (_webSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR
                || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DYNAMIQUE
                || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE
                || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO
                || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION
                || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO) {
                if (_webSession.ComparativeStudy) {
                    tmpHTML = GetComparativePeriodTypeDetail(_webSession, currentModule.Id);
                    if (tmpHTML.Length > 0)
                        tdList.Add(WebConstantes.DetailSelection.Type.comparativePeriodType, tmpHTML);
                }
            }
            #endregion

            #region Type disponibilité des données
            if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE) {
                if (_webSession.isPeriodDisponibilityTypeSelected()) {
                    tmpHTML = GetPeriodDisponibilityTypeDetail(_webSession);
                    if (tmpHTML.Length > 0)
                        tdList.Add(WebConstantes.DetailSelection.Type.periodDisponibilityType, tmpHTML);
                }
            }
            #endregion

        }
        #endregion

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

            if (currentModule.Id == WebConstantes.Module.Name.TENDACES && periodBeginning == DBConstantes.Hathor.DATE_PERIOD_CUMULATIVE) {
                CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

                _tendenciesLastDate = dateDAL.GetTendenciesLastAvailableDate();
                
                startDate = WebFunctions.Dates.DateToString(new DateTime(DateTime.Now.Year, 1,1), webSession.SiteLanguage);
                endDate = WebFunctions.Dates.DateToString(_tendenciesLastDate, webSession.SiteLanguage);
            }
            else if ((currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION || currentModule.Id == WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO) && webSession.DetailPeriodBeginningDate.Length > 0 && webSession.DetailPeriodBeginningDate != "0" && webSession.DetailPeriodEndDate.Length > 0 && webSession.DetailPeriodEndDate != "0") {
                // Affichage de la période mensuelle si elle est sélectionné dans les options de résultat
                startDate = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.DetailPeriodBeginningDate, webSession.PeriodType), webSession.SiteLanguage);
                endDate = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodEndDate(webSession.DetailPeriodEndDate, webSession.PeriodType), webSession.SiteLanguage);
            }
            else {
                if (dateFormatText) {
                    startDate = WebFunctions.Dates.getPeriodTxt(webSession, webSession.PeriodBeginningDate);
                    endDate = WebFunctions.Dates.getPeriodTxt(webSession, webSession.PeriodEndDate);
                }
                else {
                    if (periodBeginning.Length == 0 || periodEnd.Length == 0) {
                        startDate = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType), webSession.SiteLanguage);
                        endDate = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType), webSession.SiteLanguage);
                    }
                    else {
                        // Predefined date
                        startDate = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodBeginningDate(periodBeginning, webSession.PeriodType), webSession.SiteLanguage);
                        endDate = WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodEndDate(periodEnd, webSession.PeriodType), webSession.SiteLanguage);
                    }
                }
            }

            html.Append("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(1541, webSession.SiteLanguage) + " :</font>&nbsp; " + startDate);
            if (!startDate.Equals(endDate))
                html.Append(" - " + endDate);
            html.Append("</td>");

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

                html.Append("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2291, webSession.SiteLanguage) + " </font>&nbsp; " + startDate);
                if (!startDate.Equals(endDate))
                    html.Append(" - " + endDate);
                html.Append("&nbsp;</td>");

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
            string startDate = "";
            string endDate = "";
            DateTime dateBeginDT;
            DateTime dateEndDT;

            if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                SetComparativeDateMediaSchedule(ref startDate, ref endDate);
            }
            else if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES) {
                SetComparativeDateMandataires(ref startDate, ref endDate);
            }
            else if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
                    || moduleId == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE) {
                dateBeginDT = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).AddYears(-1);
                startDate = WebFunctions.Dates.DateToString(dateBeginDT, webSession.SiteLanguage);
                dateEndDT = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType, true);
                endDate = WebFunctions.Dates.DateToString(dateEndDT, webSession.SiteLanguage);
            }
            else if (_webSession.CurrentModule == WebConstantes.Module.Name.TENDACES) {
                SetComparativeDateTendences(ref startDate, ref endDate);
            }
            else if (_webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE
                    || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO
                    || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION
                    || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO) {
                        SetComparativeDateDashBord(ref startDate, ref endDate);
            }
            else {
                startDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.ComparativeStartDate.ToString(), webSession.SiteLanguage);
                endDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.ComparativeEndDate.ToString(), webSession.SiteLanguage);
            }

            html.Append("<td colspan=4 " + cssTitleData + "><font " + cssTitle + ">&nbsp;" + GestionWeb.GetWebWord(2292, webSession.SiteLanguage) + " </font>&nbsp; " + startDate);
            if (!startDate.Equals(endDate))
                html.Append(" - " + endDate);
            html.Append("</td>");

            return (html.ToString());

        }
        #endregion

        #region Set Comparative Date

        #region Set Comparative Date Media Schedule
        /// <summary>
        /// Set Comparative Date Media Schedule
        /// </summary>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        private void SetComparativeDateMediaSchedule(ref string startDate, ref string endDate) {

            // get date begin and date end according to period type
            DateTime dateBeginDT = Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
            DateTime dateEndDT = Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);

            // get comparative date begin and date end
            dateBeginDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateBeginDT.Date, _webSession.ComparativePeriodType);
            dateEndDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateEndDT.Date, _webSession.ComparativePeriodType);

            // Formating date begin and date end
            startDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(dateBeginDT.ToString("yyyyMMdd"), _webSession.SiteLanguage);
            endDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(dateEndDT.ToString("yyyyMMdd"), _webSession.SiteLanguage);

        }
        #endregion

        #region Set Comparative Date Mandataires
        /// <summary>
        /// Set Comparative Date Mandataires
        /// </summary>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        private void SetComparativeDateMandataires(ref string startDate, ref string endDate) {

            DateTime dateBeginDT = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
            DateTime dateEndDT = WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);
            MediaSchedulePeriod comparativePeriod = new MediaSchedulePeriod(dateBeginDT.AddMonths(-12), dateEndDT.AddMonths(-12), _webSession.DetailPeriod);

            startDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(comparativePeriod.Begin.ToString("yyyyMMdd"), _webSession.SiteLanguage);
            endDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(comparativePeriod.End.ToString("yyyyMMdd"), _webSession.SiteLanguage);

        }
        #endregion

        #region Set Comparative Date Tendences
        /// <summary>
        /// Set Comparative Date Tendences
        /// </summary>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        private void SetComparativeDateTendences(ref string startDate, ref string endDate) {

            DateTime dateBeginDT;
            DateTime dateEndDT;
            AtomicPeriodWeek currentPeriod, beginPreviousPeriod, endPreviousPeriod;

            if (_webSession.PeriodType == CustomerSessions.Period.Type.cumlDate) {
                dateBeginDT = new DateTime(DateTime.Now.AddYears(-1).Year, 1, 1);
                startDate = WebFunctions.Dates.DateToString(dateBeginDT, _webSession.SiteLanguage);

                if (_webSession.DateToDateComparativeWeek) {
                    dateEndDT = _tendenciesLastDate;
                    endDate = WebFunctions.Dates.DateToString(dateEndDT.AddYears(-1), _webSession.SiteLanguage);
                }
                else {
                    dateEndDT = _tendenciesLastDate;
                    currentPeriod = new AtomicPeriodWeek(dateEndDT);
                    endPreviousPeriod = currentPeriod;
                    endPreviousPeriod.SubWeek(52);
                    endDate = WebFunctions.Dates.DateToString(endPreviousPeriod.LastDay, _webSession.SiteLanguage);
                }
            }
            else if (_webSession.DetailPeriod == CustomerSessions.Period.DisplayLevel.weekly) {
                dateBeginDT = WebFunctions.Dates.getPeriodBeginningDate(_periodBeginning, _webSession.PeriodType);
                dateEndDT = WebFunctions.Dates.getPeriodEndDate(_periodEnd, _webSession.PeriodType);
                if (!_webSession.DateToDateComparativeWeek) {
                    currentPeriod = new AtomicPeriodWeek(dateBeginDT);
                    beginPreviousPeriod = currentPeriod;
                    beginPreviousPeriod.SubWeek(52);
                    currentPeriod = new AtomicPeriodWeek(dateEndDT);
                    endPreviousPeriod = currentPeriod;
                    endPreviousPeriod.SubWeek(52);
                    startDate = beginPreviousPeriod.FirstDay.ToString("dd/MM/yyyy");
                    endDate = endPreviousPeriod.LastDay.ToString("dd/MM/yyyy");
                }
                else {
                    startDate = dateBeginDT.AddYears(-1).ToString("dd/MM/yyyy");
                    endDate = dateBeginDT.AddYears(-1).AddDays(6).ToString("dd/MM/yyyy");
                }
            }
            else {
                dateBeginDT = new DateTime(int.Parse(_periodBeginning.Substring(0, 4)), int.Parse(_periodBeginning.Substring(4, 2)), 1);
                dateBeginDT = dateBeginDT.AddYears(-1);
                dateEndDT = new DateTime(int.Parse(_periodEnd.Substring(0, 4)), int.Parse(_periodEnd.Substring(4, 2)), 1);
                dateEndDT = dateEndDT.AddYears(-1);
                dateEndDT = dateEndDT.AddMonths(1).AddDays(-1);
                startDate = WebFunctions.Dates.DateToString(dateBeginDT, _webSession.SiteLanguage);
                endDate = WebFunctions.Dates.DateToString(dateEndDT, _webSession.SiteLanguage);
            }

        }
        #endregion

        #region Set Comparative Date Dash Bord
        /// <summary>
        /// Set Comparative Date Dash Bord
        /// </summary>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        private void SetComparativeDateDashBord(ref string startDate, ref string endDate) {

            DateTime dateBeginDT;
            DateTime dateEndDT;
            AtomicPeriodWeek currentPeriod, beginPreviousPeriod, endPreviousPeriod;

            if (_webSession.DetailPeriod == CstPeriodDetail.monthly) {
                if (!IsDetailPeriod(_webSession)) {
                    dateBeginDT = new DateTime(int.Parse(_webSession.PeriodBeginningDate.Substring(0, 4)), int.Parse(_webSession.PeriodBeginningDate.Substring(4, 2)), 1);
                    startDate = dateBeginDT.AddYears(-1).ToString("dd/MM/yyyy");
                    dateEndDT = new DateTime(int.Parse(_webSession.PeriodEndDate.Substring(0, 4)), int.Parse(_webSession.PeriodEndDate.Substring(4, 2)), 1);
                    dateEndDT = dateEndDT.AddYears(-1).AddMonths(1).AddDays(-1);
                    endDate = dateEndDT.ToString("dd/MM/yyyy");
                }
                else {
                    dateBeginDT = new DateTime(int.Parse(_webSession.DetailPeriodBeginningDate.Substring(0, 4)), int.Parse(_webSession.DetailPeriodBeginningDate.Substring(4, 2)), 1);
                    startDate = dateBeginDT.AddYears(-1).ToString("dd/MM/yyyy");
                    dateEndDT = new DateTime(int.Parse(_webSession.DetailPeriodEndDate.Substring(0, 4)), int.Parse(_webSession.DetailPeriodEndDate.Substring(4, 2)), 1);
                    dateEndDT = dateEndDT.AddYears(-1).AddMonths(1).AddDays(-1);
                    endDate = dateEndDT.ToString("dd/MM/yyyy");
                }
            }
            else if (_webSession.DetailPeriod == CstPeriodDetail.weekly) {
                //semaines sélectionnées
                if (!IsDetailPeriod(_webSession)) {
                    currentPeriod = new AtomicPeriodWeek(int.Parse(_webSession.PeriodBeginningDate.Substring(0, 4)), int.Parse(_webSession.PeriodBeginningDate.Substring(4, 2)));
                    beginPreviousPeriod = currentPeriod;
                    beginPreviousPeriod.SubWeek(52);
                    currentPeriod = new AtomicPeriodWeek(int.Parse(_webSession.PeriodEndDate.Substring(0, 4)), int.Parse(_webSession.PeriodEndDate.Substring(4, 2)));
                    endPreviousPeriod = currentPeriod;
                    endPreviousPeriod.SubWeek(52);
                    startDate = beginPreviousPeriod.FirstDay.ToString("dd/MM/yyyy");
                    endDate = endPreviousPeriod.LastDay.ToString("dd/MM/yyyy");
                }
                else {
                    currentPeriod = new AtomicPeriodWeek(int.Parse(_webSession.DetailPeriodBeginningDate.Substring(0, 4)), int.Parse(_webSession.DetailPeriodBeginningDate.Substring(4, 2)));
                    beginPreviousPeriod = currentPeriod;
                    beginPreviousPeriod.SubWeek(52);
                    currentPeriod = new AtomicPeriodWeek(int.Parse(_webSession.DetailPeriodEndDate.Substring(0, 4)), int.Parse(_webSession.DetailPeriodEndDate.Substring(4, 2)));
                    endPreviousPeriod = currentPeriod;
                    endPreviousPeriod.SubWeek(52);
                    startDate = beginPreviousPeriod.FirstDay.ToString("dd/MM/yyyy");
                    endDate = endPreviousPeriod.LastDay.ToString("dd/MM/yyyy");
                }
            }
            else {
                startDate = "";
                endDate = "";
            }

        }
        #endregion

        #endregion

        #region Type de la période comparative
        /// <summary>
        /// Generate html code for comparative period detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        private string GetComparativePeriodTypeDetail(WebSession webSession, long moduleId) {

            globalCalendar.comparativePeriodType comparativePeriodType;

            if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES
                || moduleId == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
                || moduleId == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
                )
                return (GestionWeb.GetWebWord(2294, webSession.SiteLanguage));
            else if((_webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE
                    || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO
                    || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION
                    || _webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO)) {
                if(webSession.DetailPeriod == CstPeriodDetail.weekly)
                        return (GestionWeb.GetWebWord(2295, webSession.SiteLanguage));
                else
                    return "";
            }
            else if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.TENDACES) {
                if (_webSession.DateToDateComparativeWeek 
                    && (_webSession.DetailPeriod == CustomerSessions.Period.DisplayLevel.weekly
                    || _webSession.PeriodType == CustomerSessions.Period.Type.cumlDate))
                    return (GestionWeb.GetWebWord(2294, webSession.SiteLanguage));
                else if (_webSession.DetailPeriod == CustomerSessions.Period.DisplayLevel.weekly
                        || _webSession.PeriodType == CustomerSessions.Period.Type.cumlDate)
                    return (GestionWeb.GetWebWord(2295, webSession.SiteLanguage));
                else
                    return "";
            }
            else if (moduleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                comparativePeriodType = webSession.ComparativePeriodType;
            else
                comparativePeriodType = webSession.CustomerPeriodSelected.ComparativePeriodType;

            switch (comparativePeriodType) {
                case WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate:
                    return (GestionWeb.GetWebWord(2295, webSession.SiteLanguage));
                case WebConstantes.globalCalendar.comparativePeriodType.dateToDate:
                    return (GestionWeb.GetWebWord(2294, webSession.SiteLanguage));
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
                    return (GestionWeb.GetWebWord(2297, webSession.SiteLanguage));
                case WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                    return (GestionWeb.GetWebWord(2298, webSession.SiteLanguage));
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

        #region Is Detail Period
        /// <summary>
        /// verifie si une periode doit être detaillée
        /// </summary>
        /// <param name="webSession">client</param>
        /// <returns>vraie si la periode est sélectionnée et faux sinon</returns>
        private bool IsDetailPeriod(WebSession webSession) {
            return !(webSession.DetailPeriodBeginningDate.Equals("") || webSession.DetailPeriodBeginningDate.Equals("0"));
        }
        #endregion

        #endregion

    }
}
