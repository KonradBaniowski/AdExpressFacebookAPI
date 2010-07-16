#region Informations
// Auteur: G. Facon
// Date de création: 14/04/2006
// Date de modification:
#endregion

#region Namespaces
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Controls.Results.MediaPlan;
using AjaxPro;

using TNS.AdExpress.Domain.Classification;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web;
#endregion

namespace AdExpress.Private.Results{
    /// <summary>
    /// Page de calendrier d'action d'un plan media
    /// </summary>
    public partial class MediaPlanResults : TNS.AdExpress.Web.UI.BaseResultWebPage{

        #region Variables
        /// <summary>
        /// Identifiant Session
        /// </summary>
        public string idsession;
        /// <summary>
        /// Code HTML du résultat
        /// </summary>
        public string SetZoom = "";
        /// <summary>
        /// Code HTML du résultat
        /// </summary>
        public string zoomButton = "";
        /// <summary>
        /// Liste d'annonceurs
        /// </summary>
        protected string listAdvertiser = "";
        /// <summary>
        /// Script qui gère la sélection des annonceurs
        /// </summary>
        public string advertiserScript;
        /// <summary>
        /// Texte de l'option "Tout sélectionner"
        /// </summary>
        public string allChecked;
        /// <summary>
        /// Bouton "Revenir à la sélection originale"
        /// </summary>
        protected System.Web.UI.WebControls.LinkButton firstSubSelectionLinkButton;
        /// <summary>
        /// Script de fermeture du flash d'attente
        /// </summary>
        public string divClose = LoadingSystem.GetHtmlCloseDiv();
        /// <summary>
        /// Capture de l'évènement responsbale du postBack
        /// </summary>
        protected int eventButton = 0;
        /// <summary>
        /// Analysis Period
        /// </summary>
        protected MediaSchedulePeriod _period;
        /// <summary>
        /// Zoom Period
        /// </summary>
        protected string _zoom;
        /// <summary>
        /// Initial Period Detail Saving
        /// </summary>
        ConstantesPeriod.DisplayLevel _savePeriod = ConstantesPeriod.DisplayLevel.monthly;
        #endregion

        #region Variable MMI
        /// <summary>
        /// Contrôle du titre du module
        /// </summary>
        /// <summary>
        /// Contrôle des options d'analyse 
        ///</summary>
        /// <summary>
        /// 		/// Contrôle de la navigation inter module (n'est pas utilisé)
        /// </summary>
        /// <summary>
        /// Contrôle de texte "Votre sélection annonceurs"
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
        /// <summary>
        /// Contrôle du header d'AdExpress
        /// </summary>
        /// <summary>
        /// Controle creation
        /// </summary>
        protected TNS.AdExpress.Web.Controls.Selections.CreativeSelectionWebControl creativeSelectionWebControl;
        /// <summary>
        /// Validation des options
        /// </summary>
        /// <summary>
        /// Contextual Menu
        /// </summary>
        /// <summary>
        /// Sélection des niveaux de détail
        /// </summary>

        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public MediaPlanResults()
            : base()
        {
        }
        #endregion

        #region Evènements

        #region Chargement de la page
        /// <summary>
        /// Evènement de chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, System.EventArgs e){
            try{
				bool withZoomDateEventArguments = false;

                #region Flash d'attente
                if (Page.Request.Form.GetValues("__EVENTTARGET") != null)
                {
                    string nomInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];
					if (nomInput != MenuWebControl2.ID) {
						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
						Page.Response.Flush();
					}
					else {
						if (Page.Request.Form.GetValues("__EVENTARGUMENT")[0] == "7") withZoomDateEventArguments = true;
					}
					
                }
                else
                {
                    Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                    Page.Response.Flush();
                }
                #endregion

                #region Url Suivante
                //				_nextUrl=this.recallWebControl.NextUrl;
                if (_nextUrl.Length != 0)
                {
                    _webSession.Source.Close();
					Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + ((withZoomDateEventArguments && _zoom != null && _zoom.Length > 0) ? "&zoomDate=" + _zoom + "&detailPeriod=" + _savePeriod.GetHashCode() : ""));
                }

                #endregion

                #region Validation du menu
                if (Page.Request.QueryString.Get("validation") == "ok")
                {
                    _webSession.Save();
                }
                #endregion

                #region Texte et langage du site
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}
                Moduletitlewebcontrol2.CustomerWebSession = _webSession;
                ModuleBridgeWebControl1.CustomerWebSession = _webSession;
                InformationWebControl1.Language = _webSession.SiteLanguage;
                //				ExportWebControl1.CustomerWebSession=_webSession;
                //Bouton valider dans la sous sélection
                //okImageButton.ImageUrl = "/Images/" + _webSession.SiteLanguage + "/button/valider_up.gif";
                //okImageButton.RollOverImageUrl = "/Images/" + _webSession.SiteLanguage + "/button/valider_down.gif";
                #endregion

                #region Period Detail
                if (_zoom == null || _zoom == string.Empty)
                {
                    if (!IsPostBack)
                    {
                        PeriodDetailWebControl1.Select(_webSession.DetailPeriod);
                    }
                    else {
                        _webSession.DetailPeriod = PeriodDetailWebControl1.SelectedValue;
                    }
                }
                else
                {
                    zoomButton = string.Format("<tr><td align=\"left\"><object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"30\" height=\"8\" VIEWASTEXT><param name=movie value=\"/App_Themes/"+this.Theme+"/Flash/Common/Arrow_Back.swf\"><param name=quality value=\"high\"><param name=menu value=\"false\"><embed src=\"/App_Themes/"+this.Theme+"/Flash/Common/Arrow_Back.swf\" width=\"30\" height=\"8\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" menu=\"false\"></embed></object><a class=\"roll06\" href=\"/Private/Results/MediaPlanResults.aspx?idSession={0}\">{2}</a></td></tr><tr><td height=\"5\"></td></tr>",
                        _webSession.IdSession,
                        _webSession.SiteLanguage,
                        GestionWeb.GetWebWord(2309, _webSession.SiteLanguage));

                }
                #endregion

				//Annuler l'univers de version
				if (_webSession.DetailPeriod != ConstantesPeriod.DisplayLevel.dayly) {
					_webSession.IdSlogans = new ArrayList();
					_webSession.SloganIdZoom = -1;
					//_webSession.Save();
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
        /// <param name="e">Arguments</param>
        protected void Page_UnLoad(object sender, System.EventArgs e)
        {
        }
        #endregion

        #region Initialisation
        /// <summary>
        /// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
        /// </summary>
        /// <param name="e"></param>
        override protected void OnInit(EventArgs e)
        {
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
        private void InitializeComponent()
        {
            this.Unload += new System.EventHandler(this.Page_UnLoad);
        }
        #endregion

        #region DeterminePostBack
        /// <summary>
        /// Détermine la valeur de PostBack
        /// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
        /// </summary>
        /// <returns>DeterminePostBackMode</returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode(){
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            GenericMediaLevelDetailSelectionWebControl1.CustomerWebSession = _webSession;
            ResultsOptionsWebControl1.CustomerWebSession = _webSession;
            MenuWebControl2.CustomerWebSession = _webSession;
            PeriodDetailWebControl1.Session = _webSession;
			PeriodDetailWebControl1.LanguageCode = _webSession.SiteLanguage;
            GenericMediaScheduleWebControl1.CustomerWebSession = _webSession;
            SubPeriodSelectionWebControl1.WebSession = _webSession;

            #region Period Detail
            _zoom = Page.Request.QueryString.Get("zoomDate");
            if (_zoom != null && _zoom != string.Empty)
            {
                if (Page.Request.Form.GetValues("zoomParam") != null && Page.Request.Form.GetValues("zoomParam")[0].Length > 0)
                {
                    _zoom = Page.Request.Form.GetValues("zoomParam")[0];
                }
                MenuWebControl2.UrlPameters = string.Format("zoomDate={0}", _zoom);
                MenuWebControl2.ForbidSave = true;
                PeriodDetailWebControl1.Visible = false;
                SubPeriodSelectionWebControl1.Visible = true;
                SubPeriodSelectionWebControl1.AllPeriodAllowed = false;
                _savePeriod = _webSession.DetailPeriod;
                _webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.dayly;
                GenericMediaScheduleWebControl1.ZoomDate = _zoom;
                SubPeriodSelectionWebControl1.ZoomDate = _zoom;
                SubPeriodSelectionWebControl1.JavascriptRefresh = "SetZoom";
                SubPeriodSelectionWebControl1.PeriodContainerName = GenericMediaScheduleWebControl1.ZoomDateContainer;
                zoomParam.Value = _zoom;

                #region SetZoom
                StringBuilder js = new StringBuilder();
                js.Append("\r\n<script type=\"text/javascript\">");
                js.Append("\r\nfunction SetZoom(){");
                js.AppendFormat("\r\n\tif ({0} == '')", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.Append("\r\n\t{");
                js.AppendFormat("\r\n\t\tdocument.location='/Private/Results/MediaPlanResults.aspx?idSession={0}'", _webSession.IdSession);
                js.Append("\r\n\t}");
                js.Append("\r\n\telse {");
                js.AppendFormat("\r\n\t\tvar date = document.getElementById(\"zoomParam\").value;");
                js.AppendFormat("\r\n\t\t{0}();", GenericMediaScheduleWebControl1.RefreshDataMethod);
                js.AppendFormat("\r\n\t\texportMenu.items.printMenuItem.actionOnClick = exportMenu.items.printMenuItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.AppendFormat("\r\n\t\texportMenu.items.excelUnitItem.actionOnClick = exportMenu.items.excelUnitItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.AppendFormat("\r\n\t\texportMenu.items.pdfExportResultItem.actionOnClick = exportMenu.items.pdfExportResultItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.AppendFormat("\r\n\t\tmenu.items.selectedItem.actionOnClick = menu.items.selectedItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.AppendFormat("\r\n\t\tdocument.getElementById(\"zoomParam\").value = {0};", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.Append("\r\n\t}");
                js.Append("\r\n}");
                js.Append("\r\n</script>");
                SetZoom = js.ToString();
                #endregion

                #region script
                if (!this.ClientScript.IsClientScriptBlockRegistered("SetZoom")) this.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetZoom", SetZoom);
                #endregion

            }
            else
            {
                PeriodDetailWebControl1.Visible = true;
                SubPeriodSelectionWebControl1.Visible = false;
                _webSession.DetailPeriod = PeriodDetailWebControl1.SelectedValue;
                if (_webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly)
                {
                    if (Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, ConstantesPeriod.Type.dateToDate)
                        < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                    {
                        _webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
                    }
                }


            }
            #endregion
			
			InitializeProductWebControl1.CustomerWebSession = _webSession;
            InitializeMediaWebcontrol1.CustomerWebSession = _webSession;
			SetSloganUniverseOptions();

            #region Option par media (Evaliant)
            string vehicleListId = _webSession.GetSelection(_webSession.SelectionUniversMedia, Right.type.vehicleAccess);
            string[] vehicles = vehicleListId.Split(',');
            foreach(string cVehicle in vehicles) {
                switch(VehiclesInformation.DatabaseIdToEnum(Int64.Parse(cVehicle))){
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack:
					case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile:
                        //ResultsOptionsWebControl1.AutopromoEvaliantOption = true;
						ResultsOptionsWebControl1.AutopromoEvaliantOption = VehiclesInformation.Get(Int64.Parse(cVehicle)).Autopromo; 
                        break;
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.newspaper:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.magazine:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress:
                        ResultsOptionsWebControl1.InsertOption = true;
                        break;
                }
            }
            #endregion

            return (tmp);
        }
        #endregion

        #region PreRender
        /// <summary>
        /// PreRendu De la page
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {
            try
            {

                #region MAJ _webSession
                if (_zoom != null && _zoom != string.Empty)
                {
                    _webSession.DetailPeriod = _savePeriod;
                }
                _webSession.LastReachedResultUrl = Page.Request.Url.AbsolutePath;
                _webSession.Save();
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

        #endregion

        #region Abstract Methods
        /// <summary>
        /// Get next Url from contextual menu
        /// </summary>
        /// <returns></returns>
        protected override string GetNextUrlFromMenu()
        {
            return this.MenuWebControl2.NextUrl;
        }
        #endregion

		#region Méthodes internes
		/// <summary>
		/// Indique si le client peut affiner l'univers de versions
		/// </summary>		
		private void SetSloganUniverseOptions() {
			if ((!WebFunctions.ProductDetailLevel.CanCustomizeUniverseSlogan(_webSession) || !_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)) //droits affiner univers Versions
					||  _webSession.DetailPeriod != ConstantesPeriod.DisplayLevel.dayly
				) {
				InitializeProductWebControl1.Visible = false;
                ArrayList forbiddenOptions = new ArrayList();
                forbiddenOptions.Add(7);
                MenuWebControl2.ForbidOptionPagesList = forbiddenOptions;
				//MenuWebControl2.ForbidOptionPages = true;
			}
		}
		#endregion

    }
}
