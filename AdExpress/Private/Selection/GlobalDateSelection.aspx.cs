#region Informations
// Auteur: Y. R'kaina 
// Date de création: 12/10/2007
// Date de modification: 
#endregion

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using System.Globalization;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Customer;
using AdExpressWebControles = TNS.AdExpress.Web.Controls;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.AdExpress.Domain.Web.Navigation;
using DateDll = TNS.FrameWork.Date;
using AdExpressException = AdExpress.Exceptions;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.FrameWork.Date;

namespace AdExpress.Private.Selection {
    /// <summary>
    /// Page de sélection des dates (global)
    /// </summary>
    public partial class GlobalDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage {

        #region Variables
        /// <summary>
        /// Option de période sélectionné
        /// </summary>
        int selectedIndex = -1;
        /// <summary>
        /// Pour verifier si on a sélectionné une période
        /// </summary>
        public string testSelection = "";
        /// <summary>
        /// Renvoie vraie si on est dans le module dynamique
        /// </summary>
        public bool isDynamicModule = false;
        /// <summary>
        /// Le type de la disponibilité des données
        /// </summary>
        public WebConstantes.globalCalendar.periodDisponibilityType periodCalendarDisponibilityType = WebConstantes.globalCalendar.periodDisponibilityType.currentDay;
        /// <summary>
        /// Type de la période comparative
        /// </summary>
        public WebConstantes.globalCalendar.comparativePeriodType comparativePeriodCalendarType = WebConstantes.globalCalendar.comparativePeriodType.dateToDate;

        #endregion

        #region Evènements

        #region Chargement de la page
        /// <summary>
        /// Evènement de chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Argument</param>
        protected void Page_Load(object sender, System.EventArgs e) {
            try {

                long selectedVehicle;

                #region Option de période sélectionnée
                if (Request.Form.GetValues("selectedItemIndex") != null) selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
                #endregion
                
                #region Textes et langage du site
                //Modification de la langue pour les Textes AdExpress
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}
                ModuleTitleWebControl1.CustomerWebSession = _webSession;
                InformationWebControl1.Language = _webSession.SiteLanguage;

                //validateButton1.ImageUrl = "/Images/" + _siteLanguage + "/button/valider_up.gif";
                //validateButton1.RollOverImageUrl = "/Images/" + _siteLanguage + "/button/valider_down.gif";
                //validateButton2.ImageUrl = "/Images/" + _siteLanguage + "/button/valider_up.gif";
                //validateButton2.RollOverImageUrl = "/Images/" + _siteLanguage + "/button/valider_down.gif";
                #endregion

                string selectionType = "";
                string disponibilityType = "";

                if (_webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA) {

                    if (Page.Request.Form.GetValues("selectionType") != null) selectionType = Page.Request.Form.GetValues("selectionType")[0];
                    
                    if (Page.Request.Form.GetValues("disponibilityType") != null) disponibilityType = Page.Request.Form.GetValues("disponibilityType")[0];

                    if (selectionType.Equals("dateWeekComparative"))
                        comparativePeriodCalendarType = WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate;

                    if (disponibilityType.Equals("lastPeriod"))
                        periodCalendarDisponibilityType = WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod;
                
                }

                GlobalCalendarWebControl1.PeriodSelectionTitle = GestionWeb.GetWebWord(2275, _webSession.SiteLanguage);
                GlobalCalendarWebControl1.Language = _webSession.SiteLanguage;
                
                if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE) {
                    GlobalCalendarWebControl1.PeriodRestrictedLabel = GestionWeb.GetWebWord(2280, _webSession.SiteLanguage); 
                    GlobalCalendarWebControl1.StartYear = DateTime.Now.AddYears(-1).Year;
                    if (DateTime.Now.Month == 12) GlobalCalendarWebControl1.StopYear = (DateTime.Now.AddYears(1)).Year;
                    else {
                        GlobalCalendarWebControl1.StopYear = DateTime.Now.Year;
                    }
                    isDynamicModule = true;
                }
                else {
                    GlobalCalendarWebControl1.PeriodRestrictedLabel = GestionWeb.GetWebWord(2284, _webSession.SiteLanguage); 
                    GlobalCalendarWebControl1.StartYear = DateTime.Now.AddYears(-2).Year;
                }

                if(_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA)
                    GlobalCalendarWebControl1.IsRestricted = false;
                else{
                    GlobalCalendarWebControl1.IsRestricted = true;
                    selectedVehicle = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID;
                    GlobalCalendarWebControl1.FirstDayNotEnable = WebFunctions.Dates.GetFirstDayNotEnabled(_webSession, selectedVehicle, GlobalCalendarWebControl1.StartYear,_webSession.Source);
                }
                #region Script
                //Gestion de la sélection comparative
                if (!Page.ClientScript.IsClientScriptBlockRegistered("PostBack")) {
                    if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE) 
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PostBack", TNS.AdExpress.Web.Functions.Script.PostBack(_webSession.SiteLanguage,true,GlobalCalendarWebControl1.ID,validateButton2.ID,"comparativeLink",monthDateList.ID,weekDateList.ID,dayDateList.ID,previousWeekCheckBox.ID,previousDayCheckBox.ID,currentYearCheckbox.ID,previousYearCheckbox.ID,previousMonthCheckbox.ID,"dateSelectedItem"));
                    else
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PostBack", TNS.AdExpress.Web.Functions.Script.PostBack(_webSession.SiteLanguage,false, GlobalCalendarWebControl1.ID, validateButton2.ID, "comparativeLink",monthDateList.ID, weekDateList.ID, dayDateList.ID,previousWeekCheckBox.ID,previousDayCheckBox.ID,currentYearCheckbox.ID,previousYearCheckbox.ID,previousMonthCheckbox.ID, "dateSelectedItem"));
                }
                #endregion
            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
        #endregion

        #region PréRendu de la page
        /// <summary>
        /// Evènement de PréRendu
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Argument</param>
        protected void Page_PreRender(object sender, System.EventArgs e) {
            try {
                if (this.IsPostBack) {

                    string valueInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];

                    #region Url Suivante
                    if (_nextUrlOk) {
                        if (selectedIndex >= 0 && selectedIndex<=8)
                            validateButton2_Click(this, null);
                        else
                            validateButton1_Click(this, null);
                    }
                    else {
                        if (valueInput == GlobalCalendarWebControl1.ID)
                            validateButton1_Click(this, null);
                        else
                            validateButton2_Click(this, null);
                    }
                    #endregion
                }
            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
        #endregion

        #region Déchargement de la page
        /// <summary>
        /// Evènement de déchargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Argument</param>
        protected void Page_UnLoad(object sender, System.EventArgs e) {
            _webSession.Source.Close();
            _webSession.Save();
        }
        #endregion

        #region DeterminePostBackMode
        /// <summary>
        /// On l'utilise pour l'initialisation de certains composants
        /// </summary>
        /// <returns>?</returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            yearDateList.WebSession = _webSession;
            monthDateList.WebSession = _webSession;
            weekDateList.WebSession = _webSession;
            dayDateList.WebSession = _webSession;
            MenuWebControl2.CustomerWebSession = _webSession;

            previousWeekCheckBox.Language = previousMonthCheckbox.Language = previousYearCheckbox.Language = previousDayCheckBox.Language = currentYearCheckbox.Language = _webSession.SiteLanguage;

            return tmp;
        }
        #endregion

        #region Validation du calendrier
        /// <summary>
        /// Evènement de validation du calendrier
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Argument</param>
        protected void validateButton1_Click(object sender, System.EventArgs e) {
            try {
                calendarValidation();
                _webSession.Source.Close();
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
            }
            catch (System.Exception ex) {
                testSelection = "<script language=\"JavaScript\">alert(\"" + ex.Message + "\");</script>";
            }
        }
        #endregion

        #region Validation des dates avec sauvegarde
        /// <summary>
        /// Evènement de validation des dates avec sauvegarde
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Argument</param>
        protected void validateButton2_Click(object sender, System.EventArgs e) {
            try {
                DateDll.AtomicPeriodWeek startWeek;
                DateDll.AtomicPeriodWeek endWeek;
                DateDll.AtomicPeriodWeek tmp;
                DateTime dateBegin;
                DateTime dateEnd;
                DateTime lastDayOfWeek;
                DateTime compareDate;
                DateTime lastDayEnable = DateTime.Now;
                DateTime tempDate = DateTime.Now;
                bool isLastCompletePeriod = false;
                DateTime firstDayOfMonth;
                DateTime lastDayOfMonth;
                DateTime previousMonth;
                Int32 lastDayOfMonthInt;

                if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE) {
                    switch (periodCalendarDisponibilityType) {

                        case WebConstantes.globalCalendar.periodDisponibilityType.currentDay:
                            lastDayEnable = DateTime.Now;
                            break;
                        case WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                            lastDayEnable = GlobalCalendarWebControl1.FirstDayNotEnable.AddDays(-1);
                            isLastCompletePeriod = true;
                            break;

                    }
                }

                switch (selectedIndex) {
                    case 0:
                        if (int.Parse(yearDateList.SelectedValue) != 0) {
                            _webSession.PeriodType = CstPeriodType.nLastYear;
                            _webSession.PeriodLength = int.Parse(yearDateList.SelectedValue);
                            _webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - _webSession.PeriodLength).ToString("yyyy0101");
                            _webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMMdd");
                            _webSession.DetailPeriod = CstPeriodDetail.monthly;
                        }
                        else {
                            throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
                        }
                        break;
                    case 1:
                        if (int.Parse(monthDateList.SelectedValue) != 0) {
                            _webSession.PeriodType = CstPeriodType.nLastMonth;
                            _webSession.PeriodLength = int.Parse(monthDateList.SelectedValue);
                            if (isLastCompletePeriod) {
                                firstDayOfMonth = new DateTime(lastDayEnable.Year, lastDayEnable.Month, 1);
                                lastDayOfMonth = (firstDayOfMonth.AddMonths(1)).AddDays(-1);

                                if (lastDayEnable == lastDayOfMonth) {
                                    _webSession.PeriodBeginningDate = lastDayEnable.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM01"); ;
                                    _webSession.PeriodEndDate = lastDayEnable.ToString("yyyyMMdd");
                                }
                                else {
                                    _webSession.PeriodBeginningDate = lastDayEnable.AddMonths(0 - _webSession.PeriodLength).ToString("yyyyMM01"); ;
                                    previousMonth = lastDayEnable.AddMonths(-1);
                                    firstDayOfMonth = new DateTime(previousMonth.Year, previousMonth.Month, 1);
                                    lastDayOfMonthInt = ((firstDayOfMonth.AddMonths(1)).AddDays(-1)).Day;
                                    _webSession.PeriodEndDate = firstDayOfMonth.ToString("yyyyMM") + lastDayOfMonthInt;
                                }

                            }
                            else {
                                _webSession.PeriodBeginningDate = lastDayEnable.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM01"); ;
                                _webSession.PeriodEndDate = lastDayEnable.ToString("yyyyMMdd");
                            }
                            _webSession.DetailPeriod = CstPeriodDetail.monthly;
                        }
                        else {
                            throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
                        }
                        break;
                    case 2:
                        if (int.Parse(weekDateList.SelectedValue) != 0) {
                            _webSession.PeriodType = CstPeriodType.nLastWeek;
                            _webSession.PeriodLength = int.Parse(weekDateList.SelectedValue);
                            startWeek = new DateDll.AtomicPeriodWeek(lastDayEnable);
                            endWeek = new DateDll.AtomicPeriodWeek(lastDayEnable);

                            if (isLastCompletePeriod) {
                                lastDayOfWeek = endWeek.FirstDay.AddDays(6);

                                if (lastDayOfWeek == lastDayEnable) {
                                    dateEnd = lastDayEnable;
                                }
                                else {
                                    startWeek.SubWeek(1);
                                    endWeek.SubWeek(1);
                                    lastDayOfWeek = endWeek.FirstDay.AddDays(6);
                                    dateEnd = lastDayOfWeek;
                                }
                            }
                            else {
                                dateEnd = lastDayEnable;
                            }

                            _webSession.PeriodEndDate = dateEnd.Year.ToString() + dateEnd.Month.ToString("00") + dateEnd.Day.ToString("00");
                            startWeek.SubWeek(_webSession.PeriodLength - 1);
                            dateBegin = startWeek.FirstDay;
                            _webSession.PeriodBeginningDate = dateBegin.Year.ToString() + dateBegin.Month.ToString("00") + dateBegin.Day.ToString("00");

                            _webSession.DetailPeriod = CstPeriodDetail.weekly;
                        }
                        else {
                            throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
                        }
                        break;
                    case 3:
                        _webSession.PeriodType = CstPeriodType.nLastDays;
                        _webSession.PeriodLength = int.Parse(dayDateList.SelectedValue);
                        tempDate = lastDayEnable;
                        _webSession.PeriodBeginningDate = tempDate.AddDays(1 - _webSession.PeriodLength).ToString("yyyyMMdd"); ;
                        _webSession.PeriodEndDate = tempDate.ToString("yyyyMMdd");
                        _webSession.DetailPeriod = CstPeriodDetail.dayly;
                        break;
                    case 4:
                        _webSession.PeriodType = CstPeriodType.previousYear;
                        _webSession.PeriodLength = 1;
                        _webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy0101");
                        _webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy1231");
                        _webSession.DetailPeriod = CstPeriodDetail.monthly;
                        break;
                    case 5:
                        _webSession.PeriodType = CstPeriodType.previousMonth;
                        _webSession.PeriodLength = 1;
                        firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        lastDayOfMonthInt = (firstDayOfMonth.AddDays(-1)).Day;
                        _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM01");
                        _webSession.PeriodEndDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM") + lastDayOfMonthInt;
                        _webSession.DetailPeriod = CstPeriodDetail.monthly;
                        break;
                    case 6:
                        _webSession.PeriodType = CstPeriodType.previousWeek;
                        _webSession.PeriodLength = 1;
                        tmp = new DateDll.AtomicPeriodWeek(DateTime.Now);
                        tmp.SubWeek(1);
                        dateBegin = tmp.FirstDay;
                        _webSession.PeriodBeginningDate = dateBegin.Year.ToString() + dateBegin.Month.ToString("00") + dateBegin.Day.ToString("00");
                        dateEnd = tmp.FirstDay.AddDays(6);
                        _webSession.PeriodEndDate = dateEnd.Year.ToString() + dateEnd.Month.ToString("00") + dateEnd.Day.ToString("00");
                        _webSession.DetailPeriod = CstPeriodDetail.weekly;
                        break;
                    case 7:
                        _webSession.PeriodType = CstPeriodType.previousDay;
                        _webSession.PeriodLength = 2;
                        _webSession.PeriodBeginningDate = _webSession.PeriodEndDate = DateTime.Now.AddDays(1 - _webSession.PeriodLength).ToString("yyyyMMdd");
                        _webSession.DetailPeriod = CstPeriodDetail.dayly;
                        break;
                    case 8:
                        _webSession.PeriodType = CstPeriodType.currentYear;
                        _webSession.PeriodLength = 1;
                        _webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - _webSession.PeriodLength).ToString("yyyy0101");
                        _webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMMdd");
                        _webSession.DetailPeriod = CstPeriodDetail.monthly;
                        break;
                    default:
                        throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
                }

                compareDate = new DateTime(Convert.ToInt32(_webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(6, 2)));

                if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE) {

                    if (CompareDateEnd(lastDayEnable, compareDate))
                        _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate, true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                    else {

                        switch (periodCalendarDisponibilityType) {

                            case WebConstantes.globalCalendar.periodDisponibilityType.currentDay:
                                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                                break;
                            case WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                                break;
                        }
                    }

                }
                else {
                    if (CompareDateEnd(DateTime.Now, compareDate))
                        _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);
                    else
                        _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
                }

                _webSession.Source.Close();
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
            }
            catch (AdExpressException.AnalyseDateSelectionException ex) {
                testSelection = "<script language=\"JavaScript\">alert(\"" + ex.Message + "\");</script>";
            }
        }
        #endregion

        #endregion

        #region Méthodes internes

        #region Validation des calendriers
        /// <summary>
        /// Traitement des dates d'un calendrier
        /// </summary>
        public void calendarValidation() {
            // On sauvegarde les données
            try {
                DateTime endDate;
                DateTime beginDate;
                DateTime lastDayEnable=DateTime.Now;

                _webSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly;
                _webSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
                _webSession.PeriodBeginningDate = GlobalCalendarWebControl1.SelectedStartDate.ToString();
                _webSession.PeriodEndDate = GlobalCalendarWebControl1.SelectedEndDate.ToString();

                endDate = new DateTime(Convert.ToInt32(_webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(6, 2)));
                beginDate = new DateTime(Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(6, 2)));

                if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE) {

                    switch (periodCalendarDisponibilityType) {

                        case WebConstantes.globalCalendar.periodDisponibilityType.currentDay:
                            lastDayEnable = DateTime.Now;
                            break;
                        case WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                            lastDayEnable = GlobalCalendarWebControl1.FirstDayNotEnable.AddDays(-1);
                            break;

                    }

                    if (CompareDateEnd(lastDayEnable, endDate) || CompareDateEnd(beginDate, DateTime.Now))
                        _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate, true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                    else {

                        switch (periodCalendarDisponibilityType) {

                            case WebConstantes.globalCalendar.periodDisponibilityType.currentDay:
                                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                                break;
                            case WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                                break;
                        }
                    }
                }
                else {
                    if (CompareDateEnd(DateTime.Now, endDate) || CompareDateEnd(beginDate, DateTime.Now))
                        _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);
                    else
                        _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
                }

                _webSession.Save();
            }
            catch (System.Exception e) {
                _webSession.PeriodBeginningDate = "";
                _webSession.PeriodEndDate = "";
                throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
            }
        }
        #endregion

        #region comparaison entre la date de fin et la date d'aujourd'hui
        /// <summary>
        /// Verifie si la date de fin est inférieur ou non à la date de début
        /// </summary>
        /// <returns>vrai si la date de fin et inférieur à la date de début</returns>
        private bool CompareDateEnd(DateTime dateBegin, DateTime dateEnd) {
            if (dateEnd < dateBegin)
                return true;
            else
                return false;
        }
        #endregion

        #endregion

        #region Implémentation méthodes abstraites
        protected override void ValidateSelection(object sender, System.EventArgs e) {
            this.Page_PreRender(sender, e);
        }
        protected override string GetNextUrlFromMenu() {
            return (this.MenuWebControl2.NextUrl);
        }
        #endregion

    }

}
