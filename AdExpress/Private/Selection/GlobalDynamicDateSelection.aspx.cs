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
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.Date;

namespace AdExpress.Private.Selection {
    /// <summary>
    /// Page de sélection des dates (global avec limitaion d'une période)
    /// </summary>
    public partial class GlobalDynamicDateSelection : TNS.AdExpress.Web.UI.SelectionWebPage {

        #region Variables
        /// <summary>
        /// Contrôle txt n dernieres annees
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Translation.AdExpressText yearAdExpressText;
        /// <summary>
        /// Option de période sélectionné
        /// </summary>
        int selectedIndex = -1;
        /// <summary>
        /// Date de début de chargemebt des données
        /// </summary>
        private string downloadBeginningDate = "";
        /// <summary>
        /// Date de fin de chargement des données
        /// </summary>
        private string downloadEndDate = "";
        /// <summary>
        /// Dernier mois (YYYYMM) dont les données sont complètement disponibles
        /// </summary>
        protected string _lastCompleteMonth = null;
        /// <summary>
        /// Media sélectionné
        /// </summary>
        protected long _selectedVehicle = -1;
        #endregion

        #region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
        public GlobalDynamicDateSelection() : base() {
	    }
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
                TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls, _webSession.SiteLanguage);
                ModuleTitleWebControl1.CustomerWebSession = _webSession;
                InformationWebControl1.Language = _webSession.SiteLanguage;

                validateButton2.ImageUrl = "/Images/" + _siteLanguage + "/button/valider_up.gif";
                validateButton2.RollOverImageUrl = "/Images/" + _siteLanguage + "/button/valider_down.gif";
                #endregion

                
                GlobalCalendarWebControl1.PeriodSelectionTitle = GestionWeb.GetWebWord(2275, _webSession.SiteLanguage);
                GlobalCalendarWebControl1.PeriodRestrictedLabel = GestionWeb.GetWebWord(2280, _webSession.SiteLanguage);
                if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE) {
                    GlobalCalendarWebControl1.StartYear = DateTime.Now.AddYears(-1).Year;
                    if (DateTime.Now.Month == 12) GlobalCalendarWebControl1.StopYear = (DateTime.Now.AddYears(1)).Year;
                    else {
                        GlobalCalendarWebControl1.StopYear = DateTime.Now.Year;
                    }
                }
                else {
                    GlobalCalendarWebControl1.StartYear = DateTime.Now.AddYears(-2).Year;
                }

                GlobalCalendarWebControl1.IsRestricted = true;
                selectedVehicle = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID;
                GlobalCalendarWebControl1.FirstDayNotEnable = GetFirstDayNotEnabled(selectedVehicle, GlobalCalendarWebControl1.StartYear);


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
            if (this.IsPostBack) {
                
                string valueInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];

                #region Url Suivante
                if (_nextUrlOk) {
                    if (selectedIndex >= 0 && selectedIndex <= 7)
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
        #endregion

        #region Déchargement de la page
        /// <summary>
        /// Evènement de déchargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Argument</param>
        protected void Page_UnLoad(object sender, System.EventArgs e) {
        }
        #endregion

        #region Initialisation

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Evènement d'initialisation
        /// </summary>
        /// <param name="e">Argument</param>
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent() {
            this.Unload += new System.EventHandler(this.Page_UnLoad);
        }
        #endregion

        #endregion

        #region DeterminePostBackMode
        /// <summary>
        /// On l'utilise pour l'initialisation de certains composants
        /// </summary>
        /// <returns>?</returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            
            monthDateList.WebSession = _webSession;
            weekDateList.WebSession = _webSession;
            MenuWebControl2.CustomerWebSession = _webSession;

            previousWeekCheckBox.Language = previousMonthCheckbox.Language =
            previousYearCheckbox.Language = currentYearCheckbox.Language = _webSession.SiteLanguage;

            return tmp;
        }
        #endregion

        #region Validation des calendriers
        /// <summary>
        /// Evènement de validation des calendriers
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Argument</param>
        protected void validateButton1_Click(object sender, System.EventArgs e) {
            try {
                calendarValidation();
                _webSession.Save();
                DBFunctions.closeDataBase(_webSession);
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
            }
            catch (System.Exception ex) {
                Response.Write("<script language=\"JavaScript\">alert(\"" + ex.Message + "\");</script>");
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
                DateDll.AtomicPeriodWeek week;
                DateTime monthPeriod;

                switch (selectedIndex) {
                    //Année courante
                    case 0:
                        _webSession.PeriodType = CstPeriodType.currentYear;
                        _webSession.PeriodLength = 1;

                        //Dates de chargement des données pour Internet
                        if (_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()) {
                            if (_lastCompleteMonth != null && _lastCompleteMonth.Length > 0 && int.Parse(_lastCompleteMonth.Substring(0, 4)) == DateTime.Now.Year) {
                                _webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
                                _webSession.PeriodEndDate = _lastCompleteMonth;
                            }
                            else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157, _webSession.SiteLanguage));
                        }
                        else {
                            //Dates de chargement des données pour les autres médias
                            if (DateTime.Now.Month == 1) {
                                throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(1612, _webSession.SiteLanguage));
                            }
                            else {
                                WebFunctions.Dates.DownloadDates(_webSession, ref downloadBeginningDate, ref downloadEndDate, CstPeriodType.currentYear);
                                _webSession.PeriodBeginningDate = downloadBeginningDate;
                                _webSession.PeriodEndDate = downloadEndDate;
                            }
                        }
                        _webSession.DetailPeriod = CstPeriodDetail.monthly;
                        break;

                    //N derniers mois
                    case 1:
                        if (int.Parse(monthDateList.SelectedValue) != 0) {
                            _webSession.PeriodType = CstPeriodType.nLastMonth;
                            _webSession.PeriodLength = int.Parse(monthDateList.SelectedValue);

                            //Dates de chargement des données pour Internet
                            if (_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()) {
                                if (_lastCompleteMonth != null && _lastCompleteMonth.Length > 0) {
                                    _webSession.PeriodEndDate = _lastCompleteMonth;
                                    monthPeriod = new DateTime(int.Parse(_lastCompleteMonth.Substring(0, 4)), int.Parse(_lastCompleteMonth.Substring(4, 2)), 01);
                                    _webSession.PeriodBeginningDate = monthPeriod.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");
                                }
                                else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157, _webSession.SiteLanguage));
                            }
                            else {
                                //Dates de chargement des données pour les autres médias
                                WebFunctions.Dates.LastLoadedMonth(ref downloadBeginningDate, ref downloadEndDate, CstPeriodType.nLastMonth);
                                monthPeriod = new DateTime(int.Parse(downloadBeginningDate.Substring(0, 4)), int.Parse(downloadBeginningDate.Substring(4, 2)), 01);
                                _webSession.PeriodBeginningDate = monthPeriod.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM");
                                _webSession.PeriodEndDate = downloadEndDate;
                            }

                            _webSession.DetailPeriod = CstPeriodDetail.monthly;
                        }
                        else {
                            throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
                        }
                        break;

                    //N dernières semaines
                    case 2:
                        if (int.Parse(weekDateList.SelectedValue) != 0) {
                            //dernière semaine chargée
                            WebFunctions.Dates.LastLoadedWeek(ref downloadBeginningDate, ref downloadEndDate);
                            _webSession.PeriodType = CstPeriodType.nLastWeek;
                            _webSession.PeriodLength = int.Parse(weekDateList.SelectedValue);
                            week = new DateDll.AtomicPeriodWeek(int.Parse(downloadEndDate.Substring(0, 4)), int.Parse(downloadEndDate.Substring(4, 2)));
                            _webSession.PeriodEndDate = downloadEndDate;
                            week.SubWeek(_webSession.PeriodLength - 1);
                            _webSession.PeriodBeginningDate = week.Year.ToString() + ((week.Week > 9) ? "" : "0") + week.Week.ToString();
                            _webSession.DetailPeriod = CstPeriodDetail.weekly;
                        }
                        else {
                            throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
                        }
                        break;

                    //Année précédente
                    case 3:
                        _webSession.PeriodType = CstPeriodType.previousYear;
                        _webSession.PeriodLength = 1;

                        //Dates de chargement des données pour Internet
                        if (_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()) {
                            if (_lastCompleteMonth != null && _lastCompleteMonth.Length > 0 && int.Parse(_lastCompleteMonth.Substring(0, 4)) > DateTime.Now.AddYears(-2).Year) {
                                _webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
                                _webSession.PeriodEndDate = (int.Parse(_lastCompleteMonth.Substring(0, 4)) == DateTime.Now.Year) ? DateTime.Now.AddYears(-1).ToString("yyyy12") : _lastCompleteMonth;
                            }
                            else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157, _webSession.SiteLanguage));
                        }
                        else {
                            //Dates de chargement des données pour les autres médias
                            WebFunctions.Dates.DownloadDates(_webSession, ref downloadBeginningDate, ref downloadEndDate, CstPeriodType.previousYear);
                            _webSession.PeriodBeginningDate = downloadBeginningDate;
                            _webSession.PeriodEndDate = downloadEndDate;
                        }
                        _webSession.DetailPeriod = CstPeriodDetail.monthly;
                        break;

                    //Mois précédent
                    case 4:
                        //Dates de chargement des données pour Internet
                        if (_selectedVehicle == DBClassificationConstantes.Vehicles.names.internet.GetHashCode()) {
                            if (_lastCompleteMonth != null && _lastCompleteMonth.Length > 0 && int.Parse(_lastCompleteMonth) >= int.Parse(DateTime.Now.AddMonths(-1).ToString("yyyyMM")))
                                _webSession.PeriodEndDate = _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
                            else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157, _webSession.SiteLanguage));
                        }
                        else {
                            //Dates de chargement des données pour les autres médias
                            WebFunctions.Modules.ActivePreviousAtomicPeriod(CstPeriodType.previousMonth, _webSession);
                            _webSession.PeriodEndDate = _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");

                        }
                        _webSession.PeriodType = CstPeriodType.previousMonth;
                        _webSession.PeriodLength = 1;
                        _webSession.DetailPeriod = CstPeriodDetail.monthly;
                        break;

                    //Semaine précédente
                    case 5:
                        WebFunctions.Modules.ActivePreviousAtomicPeriod(CstPeriodType.previousWeek, _webSession);
                        _webSession.PeriodType = CstPeriodType.previousWeek;
                        _webSession.PeriodLength = 1;
                        week = new DateDll.AtomicPeriodWeek(DateTime.Now.AddDays(-7));
                        _webSession.PeriodBeginningDate = _webSession.PeriodEndDate = week.Year.ToString() + ((week.Week > 9) ? "" : "0") + week.Week.ToString();
                        _webSession.DetailPeriod = CstPeriodDetail.weekly;
                        break;
                    default:
                        throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
                }
                _webSession.Save();
                DBFunctions.closeDataBase(_webSession);
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession);
            }
            catch (System.Exception ex) {
                Response.Write("<script language=\"JavaScript\">alert(\"" + ex.Message + "\");</script>");
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

                _webSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly;
                _webSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
                _webSession.PeriodBeginningDate = GlobalCalendarWebControl1.SelectedStartDate.ToString();
                _webSession.PeriodEndDate = GlobalCalendarWebControl1.SelectedEndDate.ToString();

                endDate = new DateTime(Convert.ToInt32(_webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(_webSession.PeriodEndDate.Substring(6, 2)));

                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);

                _webSession.Save();
            }
            catch (System.Exception e) {
                throw (new AdExpressException.AnalyseDateSelectionException(GestionWeb.GetWebWord(885, _webSession.SiteLanguage)));
            }
        }
        #endregion

        #region GetFirstDayNotEnable
        /// <summary>
        /// Renvoie le premier jour du calendrier à partir duquel les données ne sont pas encore chargées
        /// </summary>
        /// <returns>Le premier jour du calendrier à partir duquel les données ne sont pas encore chargées</returns>
        public DateTime GetFirstDayNotEnabled(long selectedVehicle, int startYear) {
            AtomicPeriodWeek week = new AtomicPeriodWeek(DateTime.Now);
            DateTime firstDayOfWeek = week.FirstDay;
            DateTime publicationDate;
            string lastDate = string.Empty;

            switch ((DBClassificationConstantes.Vehicles.names)selectedVehicle) {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    lastDate = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(_webSession, selectedVehicle,_webSession.Source);
                    if (lastDate.Length == 0) lastDate = startYear + "0101";
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    firstDayOfWeek = publicationDate.AddDays(1);
                    return firstDayOfWeek;
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    firstDayOfWeek = firstDayOfWeek.AddDays(-7);
                    return (firstDayOfWeek);
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    if (!((int)DateTime.Now.DayOfWeek >= 5) && !((int)DateTime.Now.DayOfWeek == 0)) {
                        firstDayOfWeek = firstDayOfWeek.AddDays(-7);
                    }
                    return (firstDayOfWeek);
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    lastDate = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(_webSession, selectedVehicle,_webSession.Source);
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    firstDayOfWeek = publicationDate.AddDays(7);
                    return firstDayOfWeek;
                case DBClassificationConstantes.Vehicles.names.internet:
                    lastDate = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(_webSession, selectedVehicle,_webSession.Source);
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    publicationDate = publicationDate.AddMonths(1);
                    firstDayOfWeek = new DateTime(publicationDate.Year, publicationDate.Month, 1);
                    return firstDayOfWeek;
            }

            return firstDayOfWeek;

        }
        #endregion

        #endregion

        #region Implémentation méthodes abstraites
        /// <summary>
        /// Event launch to fire validation of the page
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Arguments</param>
        protected override void ValidateSelection(object sender, System.EventArgs e) {
            this.Page_PreRender(sender, e);
        }
        /// <summary>
        /// Retrieve next Url from the menu
        /// </summary>
        /// <returns>Next Url</returns>
        protected override string GetNextUrlFromMenu() {
            return (this.MenuWebControl2.NextUrl);
        }
        #endregion

    }
}
