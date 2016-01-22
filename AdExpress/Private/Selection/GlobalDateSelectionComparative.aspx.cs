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

namespace AdExpress.Private.Selection {
    /// <summary>
    /// Page de sélection des dates (global)
    /// </summary>
    public partial class GlobalDateSelectionComparative : TNS.AdExpress.Web.UI.SelectionWebPage {

        #region Constantes
        /// <summary>
        /// L'Id de la sub section qui represente la page SponsorshipForm
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

                validateButton1.ImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/valider_up.gif";
                validateButton1.RollOverImageUrl = "/App_Themes/" + this.Theme + "/Images/Culture/button/valider_down.gif";
                validateButton1.Attributes.Add("style", "cursor:pointer;");
                #endregion

                string selectionType = "";

                CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);

                if (_webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA) {

                    if (Page.Request.Form.GetValues("selectionType") != null) selectionType = Page.Request.Form.GetValues("selectionType")[0];
                
                }

                GlobalCalendarWebControl1.PeriodSelectionTitle = GestionWeb.GetWebWord(2275, _webSession.SiteLanguage);
                GlobalCalendarWebControl1.Language = _webSession.SiteLanguage;
                
                if (_webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE) {
                    GlobalCalendarWebControl1.PeriodRestrictedLabel = GestionWeb.GetWebWord(2280, _webSession.SiteLanguage);
                    GlobalCalendarWebControl1.StartYear = dateDAL.GetCalendarStartDate();
                    if (DateTime.Now.Month == 12) GlobalCalendarWebControl1.StopYear = (DateTime.Now.AddYears(1)).Year;
                    else {
                        GlobalCalendarWebControl1.StopYear = DateTime.Now.Year;
                    }
                    isDynamicModule = true;
                }
                else {
                    GlobalCalendarWebControl1.PeriodRestrictedLabel = GestionWeb.GetWebWord(2284, _webSession.SiteLanguage); 
                    GlobalCalendarWebControl1.StartYear = dateDAL.GetCalendarStartDate();
                }

                GlobalCalendarWebControl1.IsRestricted = ModulesList.GetModule(_webSession.CurrentModule).DisplayIncompleteDateInCalendar;
                List<Int64> selectedVehicleList = new List<Int64>();
                if (GlobalCalendarWebControl1.IsRestricted) {
                    string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
                    if (vehicleSelection == null) throw (new VehicleException("Selection of media type is not correct"));
                    selectedVehicleList = new List<Int64>((new List<string>(vehicleSelection.Split(','))).ConvertAll<Int64>(ConvertStringToInt64));

                    if (IsPostBack) {
                        if (this.ViewState["FirstDayNotEnabledVS"] != null)
                            GlobalCalendarWebControl1.FirstDayNotEnable = (DateTime)this.ViewState["FirstDayNotEnabledVS"];
                    }
                    else {
                        GlobalCalendarWebControl1.FirstDayNotEnable = dateDAL.GetFirstDayNotEnabled(selectedVehicleList, GlobalCalendarWebControl1.StartYear);
                        ViewState.Add("FirstDayNotEnabledVS", GlobalCalendarWebControl1.FirstDayNotEnable);
                    }
                }

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
                    validateButton1_Click(this, null);
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
            MenuWebControl2.CustomerWebSession = _webSession;
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

        #endregion

        #region Méthodes internes

        #region Validation des calendriers
        /// <summary>
        /// Traitement des dates d'un calendrier
        /// </summary>
        public void calendarValidation() {
            // On sauvegarde les données
            try {

                _webSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(_webSession.CustomerPeriodSelected.StartDate, _webSession.CustomerPeriodSelected.EndDate, GlobalCalendarWebControl1.SelectedStartDate.ToString(), GlobalCalendarWebControl1.SelectedEndDate.ToString());

                _webSession.Save();
            }
            catch (System.Exception e) {
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

        #region ConvertStringToInt64
        /// <summary>
        /// Convert String To Int64
        /// </summary>
        /// <param name="p">String parameter</param>
        /// <returns>Int64 Result</returns>
        private Int64 ConvertStringToInt64(string p) {
            return Int64.Parse(p);
        }
        #endregion

    }

}
