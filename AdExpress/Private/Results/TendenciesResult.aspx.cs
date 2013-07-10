#region Informations
// Auteur: A. Obermeyer & D. V. Mussuma
// Date de création : 8/02/2005
// Date de modification :
// 
#endregion

#region Namespaces
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TNS.AdExpress.Constantes;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.ModulesDescritpion;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.AdExpressI.Trends.DAL;
using TNS.FrameWork.Date;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using WebFunctions = TNS.AdExpress.Web.Functions;
#endregion

namespace AdExpress.Private.Results
{
    /// <summary>
    /// Page de résultat du module Tendance
    /// </summary>
    public partial class TendenciesResult : TNS.AdExpress.Web.UI.ResultWebPage
    {

        #region Variables MMI
        /// <summary>
        /// Contrôle : Options
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Headers.ResultsOptionsWebControl ResultsOptionsWebControl1;
        #endregion

        #region Variables
        /// <summary>
        /// Résultat HTML
        /// </summary>
        public string result = "";
        /// <summary>
        /// Script de fermeture du flash d'attente
        /// </summary>
        public string divClose = LoadingSystem.GetHtmlCloseDiv();
        /// <summary>
        /// Résultat HTML
        /// </summary>
        public bool displayWeeks = true;
        /// <summary>
        /// 
        /// </summary>
        public bool displayMonthes = true;
        /// <summary>
        /// 
        /// </summary>
        public bool displayCumul = true;
        /// <summary>
        /// 
        /// </summary>
        private bool fromSave = false;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur, chargement de la session
        /// </summary>
        public TendenciesResult()
            : base()
        {
        }
        #endregion

        #region Chargement de la page
        /// <summary>
        /// Chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                #region Gestion du flash d'attente
                if (Page.Request.Form.GetValues("__EVENTTARGET") != null)
                {
                    string nomInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];
                    if (nomInput != MenuWebControl2.ID)
                    {
                        Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                        Page.Response.Flush();
                    }
                }
                else
                {
                    Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                    Page.Response.Flush();
                }
                #endregion

                #region Url Suivante
                _nextUrl = this.MenuWebControl2.NextUrl;
                if (_nextUrl.Length != 0)
                {
                    _webSession.Source.Close();
                    Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
                }
                #endregion

                #region Textes et Langage du site
                Moduletitlewebcontrol2.CustomerWebSession = _webSession;
                InformationWebControl1.Language = _webSession.SiteLanguage;
                #endregion

                #region Sélection du vehicle
                string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, Right.type.vehicleAccess);
                if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
                VehicleInformation _vehicleInformation = VehiclesInformation.Get(long.Parse(vehicleSelection));
                if (_vehicleInformation == null) throw (new WebExceptions.CompetitorRulesException("La sélection de médias est incorrecte"));
                #endregion
                
                if (Page.Request.UrlReferrer != null && Page.Request.UrlReferrer.AbsolutePath.IndexOf("SearchSession.aspx") > 0)
                {
                    fromSave = true;
                }

                #region MAJ _webSession
                _webSession.LastReachedResultUrl = Page.Request.Url.AbsolutePath;
                _webSession.ReachedModule = true;
                #endregion

                #region sélection de la période
                //initialisation de la péiode sélectionnée
                if (!isPeriodSelected(Page) || fromSave) SelectedPeriod(_vehicleInformation.Id);

                if (!isPeriodTypeSelected(Page) && !fromSave) _webSession.DateToDateComparativeWeek = false;

                displayMonthes = ResultsDateListWebControl1.Visible;
                displayWeeks = ResultsDateListWebControl2.Visible;

                //Chargement de la période sélectionnée
                LoadSelectedPeriod(_vehicleInformation.Id);

                #endregion

                VehicleInformation vhInfo = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
                _webSession.DetailLevel = vhInfo.TrendsDefaultMediaSelectionDetailLevel;

                DetailPeriodWebControl1.WebSession = _webSession;

                _webSession.Save();

                #region Script
                string resultsDateListWebControl1 = (ResultsDateListWebControl1.Visible) ? "ResultsDateListWebControl1" : null;
                string resultsDateListWebControl2 = (ResultsDateListWebControl2.Visible) ? "ResultsDateListWebControl2" : null;
                string cumulPeriod = (ResultsDashBoardOptionsWebControl1.CumulPeriodOption) ? "_cumulPeriodCheckBox" : null;
                if (!this.Page.ClientScript.IsClientScriptBlockRegistered("SelectPeriod")) this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenCreationCompetitorAlert", WebFunctions.Script.SelectPeriod(resultsDateListWebControl1, resultsDateListWebControl2, cumulPeriod));
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

        #region Déchargement de la page
        /// <summary>
        /// Evènement de déchargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_UnLoad(object sender, System.EventArgs e)
        {
        }
        #endregion

        #region DeterminePostBackMode
        /// <summary>
        /// Evaluation de l'évènement PostBack:
        ///		base.DeterminePostBackMode();
        ///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            ResultsDashBoardOptionsWebControl1.CustomerWebSession = _webSession;
            ResultsDateListWebControl1.WebSession = _webSession;
            ResultsDateListWebControl2.WebSession = _webSession;
            MenuWebControl2.CustomerWebSession = _webSession;
            _resultWebControl.CustomerWebSession = _webSession;
            return tmp;
        }
        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Evènement d'initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        override protected void OnInit(EventArgs e)
        {

            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.Unload += new System.EventHandler(this.Page_UnLoad);

        }
        #endregion

        #region Méthodes internes

        #region période sélectionnée
        /// <summary>
        /// Sélection de la période d'étude
        /// </summary>
        private void SelectedPeriod(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicleName)
        {
            try
            {
                if (WebApplicationParameters.TrendsInformations.TrendsDateOpeningOptions.ContainsKey(vehicleName))
                {
                    TrendsDateOpeningOption trendsDateOpeningOption = WebApplicationParameters.TrendsInformations.TrendsDateOpeningOptions[vehicleName];

                    bool withYearToDate = trendsDateOpeningOption.OptionsIds.Contains(Tendencies.DateOpeningOption.yearToDate);
                    bool withWeeklyDate = trendsDateOpeningOption.OptionsIds.Contains(Tendencies.DateOpeningOption.weekly);
                    bool withMonthlyDate = trendsDateOpeningOption.OptionsIds.Contains(Tendencies.DateOpeningOption.monthly);

                    //Culum date options
                    if (withYearToDate)
                    {
                        ResultsDashBoardOptionsWebControl1.CumulPeriodOption = true;
                        if (!fromSave && !ChangeCountry()) setYearlyPeriod();
                        if (!ChangeCountry() && trendsDateOpeningOption.DefaultOptionId == Tendencies.DateOpeningOption.yearToDate)
                        {
                            if (!fromSave || (_webSession.PeriodType == WebConstantes.CustomerSessions.Period.Type.cumlDate))
                                ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked = true;
                            if (withMonthlyDate) ResultsDateListWebControl1.SelectedIndex = 0;
                            if (withWeeklyDate) ResultsDateListWebControl2.SelectedIndex = 0;
                        }
                        if (ChangeCountry() && _webSession.PeriodType == WebConstantes.CustomerSessions.Period.Type.cumlDate)
                            ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked = true;
                    }
                    else
                    {
                        ResultsDashBoardOptionsWebControl1.CumulPeriodOption = false;
                        displayCumul = false;
                    }

                    //Monthly date options
                    if (withMonthlyDate)
                    {
                        ResultsDateListWebControl1.Visible = true;
                        if (!ChangeCountry() && trendsDateOpeningOption.DefaultOptionId == Tendencies.DateOpeningOption.monthly)
                        {
                            if (!fromSave && !ChangeCountry())
                            {
                                ResultsDateListWebControl1.SelectedIndex = 1;
                                _webSession.PeriodBeginningDate = ResultsDateListWebControl1.SelectedValue;
                                _webSession.PeriodEndDate = ResultsDateListWebControl1.SelectedValue;
                                _webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.monthly;
                                _webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDateMonth;
                            }
                            if (withYearToDate) ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked = false;
                            if (withWeeklyDate) ResultsDateListWebControl2.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        AdExpressText1.Visible = false;
                        ResultsDateListWebControl1.Visible = false;
                        displayMonthes = false;
                    }

                    //Weekly date options
                    if (withWeeklyDate)
                    {
                        ResultsDateListWebControl2.Visible = true;
                        if (!ChangeCountry() && trendsDateOpeningOption.DefaultOptionId == Tendencies.DateOpeningOption.weekly)
                        {
                            if (!fromSave && !ChangeCountry())
                            {
                                ResultsDateListWebControl2.SelectedIndex = 1;
                                _webSession.PeriodBeginningDate = ResultsDateListWebControl2.SelectedValue;
                                _webSession.PeriodEndDate = ResultsDateListWebControl2.SelectedValue;
                                _webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.weekly;
                                _webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDateWeek;
                            }
                            if (withYearToDate) ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked = false;
                            if (withMonthlyDate) ResultsDateListWebControl1.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        AdExpressText2.Visible = false;
                        ResultsDateListWebControl2.Visible = false;
                        displayWeeks = false;
                    }
                }

            }
            catch (Exception e)
            {
                Response.Write("<script language=\"JavaScript\">alert('" + e.Message + "');</script>");
            }
        }
        #endregion

        #region chargement de la période sélectionnée
        /// <summary>
        /// Récupère la période sélectionnée
        /// </summary>
        private void LoadSelectedPeriod(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicleName)
        {
            #region sélection de la période
            if (Request.Form.Get("__EVENTTARGET") == "okImageButton")
            {
                //Sélection période cumulée			               
                if (Page.Request.Form.GetValues("_cumulPeriodCheckBox") != null && Page.Request.Form.GetValues("_cumulPeriodCheckBox")[0] != null && !Page.Request.Form.GetValues("_cumulPeriodCheckBox")[0].ToString().Equals("0"))
                {
                    setYearlyPeriod();
                    ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked = true;
                }
                //Sélection période mensuelle              
                else if (Page.Request.Form.GetValues("ResultsDateListWebControl1") != null && Page.Request.Form.GetValues("ResultsDateListWebControl1")[0] != null && !Page.Request.Form.GetValues("ResultsDateListWebControl1")[0].ToString().Equals("0"))
                {
                    _webSession.PeriodBeginningDate = Int64.Parse(Page.Request.Form.GetValues("ResultsDateListWebControl1")[0]).ToString();
                    _webSession.PeriodEndDate = Int64.Parse(Page.Request.Form.GetValues("ResultsDateListWebControl1")[0]).ToString();
                    _webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.monthly;
                    _webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDateMonth;
                    ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked = false;
                }
                //Sélection période hebdomadaire            
                else if (Page.Request.Form.GetValues("ResultsDateListWebControl2") != null && Page.Request.Form.GetValues("ResultsDateListWebControl2")[0] != null && !Page.Request.Form.GetValues("ResultsDateListWebControl2")[0].ToString().Equals("0"))
                {
                    _webSession.PeriodBeginningDate = Int64.Parse(Page.Request.Form.GetValues("ResultsDateListWebControl2")[0]).ToString();
                    _webSession.PeriodEndDate = Int64.Parse(Page.Request.Form.GetValues("ResultsDateListWebControl2")[0]).ToString();
                    _webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.weekly;
                    _webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDateWeek;
                    ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked = false;
                }
            }

            if (fromSave)
            {
                ListItem it = null;
                int index = -1;
                if (_webSession.PeriodType == WebConstantes.CustomerSessions.Period.Type.cumlDate)
                {
                    ResultsDashBoardOptionsWebControl1.IsCumulPeriodChecked = true;
                }
                else if (_webSession.PeriodType == WebConstantes.CustomerSessions.Period.Type.dateToDateMonth
                    && displayMonthes)
                {

                    try
                    {
                        it = ResultsDateListWebControl1.Items.FindByValue(_webSession.PeriodBeginningDate);
                        index = ResultsDateListWebControl1.Items.IndexOf(it);
                        ResultsDateListWebControl1.SelectedIndex = index;
                    }
                    catch (System.Exception)
                    {
                        ResultsDateListWebControl1.SelectedIndex = 1;
                        _webSession.PeriodBeginningDate = _webSession.PeriodEndDate = ResultsDateListWebControl1.SelectedValue;
                    }

                }
                else if (_webSession.PeriodType == WebConstantes.CustomerSessions.Period.Type.dateToDateWeek
                    && displayWeeks)
                {

                    try
                    {
                        it = ResultsDateListWebControl2.Items.FindByValue(_webSession.PeriodBeginningDate);
                        index = ResultsDateListWebControl2.Items.IndexOf(it);
                        ResultsDateListWebControl2.SelectedIndex = index;
                    }
                    catch (System.Exception)
                    {
                        ResultsDateListWebControl2.SelectedIndex = 1;
                        _webSession.PeriodBeginningDate = _webSession.PeriodEndDate = ResultsDateListWebControl2.SelectedValue;
                    }

                }

            }
            #endregion
            _webSession.Save();
        }
        #endregion

        /// <summary>
        /// Vérifie si l'utilisateur à sélectionné une période
        /// </summary>
        /// <returns>vrai si une période a été sélectionnée</returns>
        private bool isPeriodSelected(System.Web.UI.Page Page)
        {
            try
            {
                if ((Page.Request.Form.GetValues("_cumulPeriodCheckBox") != null && Page.Request.Form.GetValues("_cumulPeriodCheckBox")[0] != null)
                    || (Page.Request.Form.GetValues("ResultsDateListWebControl1") != null && Page.Request.Form.GetValues("ResultsDateListWebControl1")[0] != null && !Page.Request.Form.GetValues("ResultsDateListWebControl1")[0].ToString().Equals("0"))
                    || (Page.Request.Form.GetValues("ResultsDateListWebControl2") != null && Page.Request.Form.GetValues("ResultsDateListWebControl2")[0] != null && !Page.Request.Form.GetValues("ResultsDateListWebControl2")[0].ToString().Equals("0"))
                    ) return true;
            }
            catch (Exception)
            { return false; }

            return false;
        }

        /// <summary>
        /// Check if the user has selected a period type
        /// </summary>
        /// <returns>True if a period type has been selected</returns>
        private bool isPeriodTypeSelected(System.Web.UI.Page Page) {
            try {
                if (Page.Request.Form.GetValues("comparativeTypeRadioButton") != null && Page.Request.Form.GetValues("comparativeTypeRadioButton")[0] != null) return true;
            }
            catch (Exception) { return false; }

            return false;
        }

        /// <summary>
        /// Met la période cumulée dans la session cliente
        /// </summary>
        private void setYearlyPeriod()
        {
            string periodBeginningDate = "01";
            int week_N = 0;
            _webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.yearly;
            AtomicPeriodWeek currentPeriod1 = new AtomicPeriodWeek(DateTime.Now);
            currentPeriod1.SubWeek(2);
            week_N = currentPeriod1.Week;
            if (week_N > 0)
            {
                _webSession.PeriodBeginningDate = DateTime.Now.Year.ToString() + periodBeginningDate;
                _webSession.PeriodEndDate = currentPeriod1.Week.ToString().Length == 1 ? currentPeriod1.Year.ToString() + "0" + currentPeriod1.Week.ToString() : currentPeriod1.Year.ToString() + currentPeriod1.Week.ToString();
                _webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDateWeek;
            }
            else
            {
                _webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString() + periodBeginningDate;
                currentPeriod1 = new AtomicPeriodWeek(DateTime.Now.AddYears(-1));
                _webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString() + currentPeriod1.Week;
                _webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDateWeek;
            }
            _webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.cumlDate;
            _webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.monthly;
            _webSession.PeriodBeginningDate = TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE;
            _webSession.PeriodEndDate = TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE;
            _webSession.Save();
        }

        /// <summary>
        /// Change Country
        /// </summary>
        /// <returns>ChangeCountry bool</returns>
        private bool ChangeCountry()
        {
            return (Page.Request.UrlReferrer == null && !Page.IsPostBack && _webSession.LastReachedResultUrl.Trim().Equals(Page.Request.Url.AbsolutePath.Trim()));
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Retrieve next Url from contextual menu
        /// </summary>
        /// <returns></returns>
        protected override string GetNextUrlFromMenu()
        {
            return MenuWebControl2.NextUrl;
        }
        #endregion
    }
}
