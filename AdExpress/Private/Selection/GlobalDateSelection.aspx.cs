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
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Date;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;
using TNS.AdExpress.Constantes.Web;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Exceptions;

namespace AdExpress.Private.Selection
{
    /// <summary>
    /// Page de sélection des dates (global)
    /// </summary>
    public partial class GlobalDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage
    {

        #region Constantes
        /// <summary>
        /// Day selection value
        /// </summary>
        private const string DAY_SELECTION = "2";      
        /// <summary>
        /// L'Id de la sub section qui represente la page du calendrier de comparaison
        /// </summary>
        protected const int COMPARAISON_CALENDAR_FORM_ID = 9;
        #endregion

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
        /// <summary>
        /// Display Button Comparative
        /// </summary>
        public bool _displayButtonComparative = false;
        #endregion

        #region Evènements

        #region Chargement de la page
        /// <summary>
        /// Evènement de chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Argument</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                #region Test Cedexis
                //Test Cedexis
                if (WebApplicationParameters.CountryCode == TNS.AdExpress.Constantes.Web.CountryCode.FRANCE &&
                !Page.ClientScript.IsClientScriptBlockRegistered("CedexisScript"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "CedexisScript", TNS.AdExpress.Web.Functions.Script.CedexisScript());
                }
                #endregion

                _displayButtonComparative = WebApplicationParameters.UseComparativeLostWon
                    && !string.IsNullOrEmpty(this._currentModule.GetSubSectionURL(COMPARAISON_CALENDAR_FORM_ID, Page.Request.Url.AbsolutePath, true));

                #region Option de période sélectionnée
                if (Request.Form.GetValues("selectedItemIndex") != null) selectedIndex = int.Parse(Request.Form.GetValues("selectedItemIndex")[0]);
                #endregion
                
                #region Textes et langage du site               
                ModuleTitleWebControl1.CustomerWebSession = _webSession;
                InformationWebControl1.Language = _webSession.SiteLanguage;
                #endregion

                #region Get period selection type
                string dateSelectedItemValue = string.Empty;
                string[] dateSelectedItems; 
                if (Request.Form.GetValues("dateSelectedItem") != null) dateSelectedItemValue = Request.Form.GetValues("dateSelectedItem")[0];
                dateSelectedItems = dateSelectedItemValue.Split(',');
                if (dateSelectedItems[0] != null && dateSelectedItems[0].Length > 0) {
                    if (dateSelectedItems[0] == DAY_SELECTION)
                        _webSession.PeriodSelectionType = globalCalendar.periodSelectiontype.day;
                    else
                        _webSession.PeriodSelectionType = globalCalendar.periodSelectiontype.other;
                }
                validateButton1.ImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/valider_up.gif";
                validateButton1.RollOverImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/valider_down.gif";
                validateButton1.Attributes.Add("style", "cursor:pointer;");
                validateButton1.OnClientClick = "javascript:return valid(this.id); function valid(id){ PostBack(id); return false;}";

                validateButton2.ImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/valider_up.gif";
                validateButton2.RollOverImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/valider_down.gif";
                validateButton2.Attributes.Add("style", "cursor:pointer;");
                validateButton2.OnClientClick = "javascript:return valid(this.id); function valid(id){ PostBack(id); return false;}";

                buttonComparative.ImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/comparative_up.gif";
                buttonComparative.RollOverImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/comparative_down.gif";
                buttonComparative.Attributes.Add("style", "cursor:pointer;");
                buttonComparative.OnClientClick = "javascript:return valid(this.id); function valid(id){ PostBack(id); return false;}";
                #endregion

                string selectionType = "";
                string disponibilityType = "";

                CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

                if (_webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA
                    && _webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_MANDATAIRES
                    && _webSession.CurrentModule != WebConstantes.Module.Name.CELEBRITIES) {

                    if (_webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                        || (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                            && WebApplicationParameters.UseTypeOptionPeriodLostWon))
                    {

                        if (Page.Request.Form.GetValues("selectionType") != null)
                            selectionType = Page.Request.Form.GetValues("selectionType")[0];
                        if (selectionType.Equals("dateWeekComparative"))
                            comparativePeriodCalendarType = WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate;

                    }
                    else
                    {
                        comparativePeriodCalendarType = globalCalendar.comparativePeriodType.dateToDate;
                    }
                    if (_webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                        || (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                            && WebApplicationParameters.UseDiponibilityOptionPeriodLostWon))
                    {

                        if (Page.Request.Form.GetValues("disponibilityType") != null)
                            disponibilityType = Page.Request.Form.GetValues("disponibilityType")[0];
                        if (disponibilityType.Equals("lastPeriod"))
                            periodCalendarDisponibilityType = WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod;

                    }
                    else
                    {
                        periodCalendarDisponibilityType = WebConstantes.globalCalendar.periodDisponibilityType.currentDay;
                    }


                }
                GlobalCalendarWebControl1.PeriodSelectionTitle = GestionWeb.GetWebWord(2275, _webSession.SiteLanguage);
                GlobalCalendarWebControl1.Language = _webSession.SiteLanguage;

                if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE)
                {
                    GlobalCalendarWebControl1.PeriodRestrictedLabel = GestionWeb.GetWebWord(2280, _webSession.SiteLanguage);
                    GlobalCalendarWebControl1.StartYear = dateDAL.GetCalendarStartDate();
                    if (DateTime.Now.Month == 12) GlobalCalendarWebControl1.StopYear = (DateTime.Now.AddYears(1)).Year;
                    else
                    {
                        GlobalCalendarWebControl1.StopYear = DateTime.Now.Year;
                    }
                    isDynamicModule = true;
                }
                else
                {
                    GlobalCalendarWebControl1.PeriodRestrictedLabel = GestionWeb.GetWebWord(2284, _webSession.SiteLanguage);
                    GlobalCalendarWebControl1.StartYear = dateDAL.GetCalendarStartDate();
                }

                GlobalCalendarWebControl1.IsRestricted = ModulesList.GetModule(_webSession.CurrentModule).DisplayIncompleteDateInCalendar;
                List<Int64> selectedVehicleList = new List<Int64>();
                if (GlobalCalendarWebControl1.IsRestricted)
                {
                    string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                    if (vehicleSelection == null) throw (new VehicleException("Selection of media type is not correct"));
                    selectedVehicleList = new List<Int64>((new List<string>(vehicleSelection.Split(','))).ConvertAll<Int64>(ConvertStringToInt64));

                    if (IsPostBack)
                    {
                        if (this.ViewState["FirstDayNotEnabledVS"] != null)
                            GlobalCalendarWebControl1.FirstDayNotEnable = (DateTime)this.ViewState["FirstDayNotEnabledVS"];
                    }
                    else
                    {
                        GlobalCalendarWebControl1.FirstDayNotEnable = dateDAL.GetFirstDayNotEnabled(selectedVehicleList, GlobalCalendarWebControl1.StartYear);
                        ViewState.Add("FirstDayNotEnabledVS", GlobalCalendarWebControl1.FirstDayNotEnable);
                    }
                }

                #region Script
                //Gestion de la sélection comparative
                if (!Page.ClientScript.IsClientScriptBlockRegistered("PostBack"))
                {
                    if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                        && !WebApplicationParameters.UseDiponibilityOptionPeriodLostWon
                        && !WebApplicationParameters.UseTypeOptionPeriodLostWon)
                    {
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PostBack", TNS.AdExpress.Web.Functions.Script.PostBackWithoutDispoAndTypePeriod());
                    }
                    else
                    {
                        if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE)
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PostBack", TNS.AdExpress.Web.Functions.Script.PostBack(_webSession.SiteLanguage, true, validateButton1.ID, validateButton2.ID, buttonComparative.ID, "comparativeLink", monthDateList.ID, weekDateList.ID, dayDateList.ID, previousWeekCheckBox.ID, previousDayCheckBox.ID, currentYearCheckbox.ID, previousYearCheckbox.ID, previousMonthCheckbox.ID, "dateSelectedItem"));
                        else
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PostBack", TNS.AdExpress.Web.Functions.Script.PostBack(_webSession.SiteLanguage, false, validateButton1.ID, validateButton2.ID, buttonComparative.ID, "comparativeLink", monthDateList.ID, weekDateList.ID, dayDateList.ID, previousWeekCheckBox.ID, previousDayCheckBox.ID, currentYearCheckbox.ID, previousYearCheckbox.ID, previousMonthCheckbox.ID, "dateSelectedItem"));
                    }
                }
                #endregion
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
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
        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            try
            {
                if (this.IsPostBack)
                {

                    string valueInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];

                    #region Url Suivante
                    if (_nextUrlOk)
                    {
                        if (selectedIndex >= 0 && selectedIndex <= 8)
                            validateButton2_Click(this, null);
                        else
                            validateButton1_Click(this, null);
                    }
                    else
                    {
                        if (valueInput == validateButton1.ID)
                            validateButton1_Click(this, null);
                        else if (valueInput == buttonComparative.ID)
                            validateButtonComparative_Click(this, null);
                        else
                            validateButton2_Click(this, null);
                    }
                    #endregion
                }
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
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
        protected void Page_UnLoad(object sender, System.EventArgs e)
        {
            _webSession.Source.Close();
            _webSession.Save();
        }
        #endregion

        #region DeterminePostBackMode
        /// <summary>
        /// On l'utilise pour l'initialisation de certains composants
        /// </summary>
        /// <returns>?</returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            try
            {
                yearDateList.WebSession = _webSession;
                yearDateList.NbYearsToDisplay = WebApplicationParameters.DataNumberOfYear;
                monthDateList.WebSession = _webSession;
                weekDateList.WebSession = _webSession;
                dayDateList.WebSession = _webSession;
                MenuWebControl2.CustomerWebSession = _webSession;

                previousWeekCheckBox.Language = previousMonthCheckbox.Language = previousYearCheckbox.Language = previousDayCheckBox.Language = currentYearCheckbox.Language = _webSession.SiteLanguage;
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
            return tmp;
        }
        #endregion

        #region Validation du calendrier
        /// <summary>
        /// Evènement de validation du calendrier
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Argument</param>
        protected void validateButton1_Click(object sender, System.EventArgs e)
        {
            try
            {
                calendarValidation();
                _webSession.Source.Close();
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
            }
            catch (System.Exception ex)
            {
                testSelection = "<script language=\"JavaScript\">alert(\"" + ex.Message + "\");</script>";
            }
        }
        #endregion

        #region Validation du calendrier puis go to Comparative page
        /// <summary>
        /// Evènement de validation du calendrier
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Argument</param>
        protected void validateButtonComparative_Click(object sender, System.EventArgs e)
        {
            try
            {
                calendarValidation();
                _webSession.Source.Close();
                _nextUrl = this._currentModule.GetSubSectionURL(COMPARAISON_CALENDAR_FORM_ID, Page.Request.Url.AbsolutePath, true);
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
            }
            catch (System.Exception ex)
            {
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
        protected void validateButton2_Click(object sender, System.EventArgs e)
        {
            try
            {
                CoreLayer cl = WebApplicationParameters.CoreLayers[WebConstantes.Layers.Id.date];
                IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null);
                int selectedValue = -1;

                switch (selectedIndex)
                {
                    case 0:
                        selectedValue = int.Parse(yearDateList.SelectedValue);
                        break;
                    case 1:
                        selectedValue = int.Parse(monthDateList.SelectedValue);
                        break;
                    case 2:
                        selectedValue = int.Parse(weekDateList.SelectedValue);
                        break;
                    case 3:
                        selectedValue = int.Parse(dayDateList.SelectedValue);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        break;
                    default:
                        throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
                }
                if (selectedValue == 0) throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
                date.SetDate(ref _webSession, GlobalCalendarWebControl1.FirstDayNotEnable, periodCalendarDisponibilityType, comparativePeriodCalendarType, selectedIndex, selectedValue);

                _webSession.Source.Close();
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
            }
            catch (AdExpressException.AnalyseDateSelectionException ex)
            {
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
        public void calendarValidation()
        {
            // On sauvegarde les données
            try
            {
                DateTime endDate;
                DateTime beginDate;
                DateTime lastDayEnable = DateTime.Now;

                _webSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly;
                _webSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
                _webSession.PeriodBeginningDate = GlobalCalendarWebControl1.SelectedStartDate.ToString();
                _webSession.PeriodEndDate = GlobalCalendarWebControl1.SelectedEndDate.ToString();

                endDate = new DateTime(Convert.ToInt32(_webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(6, 2)));
                beginDate = new DateTime(Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(6, 2)));

                if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE)
                {

                    switch (periodCalendarDisponibilityType)
                    {

                        case WebConstantes.globalCalendar.periodDisponibilityType.currentDay:
                            lastDayEnable = DateTime.Now;
                            break;
                        case WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                            lastDayEnable = GlobalCalendarWebControl1.FirstDayNotEnable.AddDays(-1);
                            break;

                    }

                    if (CompareDateEnd(lastDayEnable, endDate) || CompareDateEnd(beginDate, DateTime.Now))
                        _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate, true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                    else
                    {

                        switch (periodCalendarDisponibilityType)
                        {

                            case WebConstantes.globalCalendar.periodDisponibilityType.currentDay:
                                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                                break;
                            case WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodCalendarType, periodCalendarDisponibilityType);
                                break;
                        }
                    }
                }
                else
                {
                    if (CompareDateEnd(DateTime.Now, endDate) || CompareDateEnd(beginDate, DateTime.Now))
                        _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);
                    else
                        _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
                }

                _webSession.Save();
            }
            catch (System.Exception e)
            {
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
        private bool CompareDateEnd(DateTime dateBegin, DateTime dateEnd)
        {
            if (dateEnd < dateBegin)
                return true;
            else
                return false;
        }
        #endregion

        #endregion

        #region Implémentation méthodes abstraites
        protected override void ValidateSelection(object sender, System.EventArgs e)
        {
            this.Page_PreRender(sender, e);
        }
        protected override string GetNextUrlFromMenu()
        {
            return (this.MenuWebControl2.NextUrl);
        }
        #endregion

        #region ConvertStringToInt64
        /// <summary>
        /// Convert String To Int64
        /// </summary>
        /// <param name="p">String parameter</param>
        /// <returns>Int64 Result</returns>
        private Int64 ConvertStringToInt64(string p)
        {
            return Int64.Parse(p);
        }
        #endregion

    }

}
