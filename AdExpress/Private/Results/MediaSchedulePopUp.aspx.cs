using System;
using System.Text;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;


//OpenGenericMediaSchedule 
namespace AdExpress.Private.Results
{


    public partial class MediaSchedulePopUp : TNS.AdExpress.Web.UI.PrivateWebPage
    {

        #region Variables
        /// <summary>
        /// Code html de fermeture du flash d'attente
        /// </summary>
        public string divClose = LoadingSystem.GetHtmlCloseDiv();
        /// <summary>
        /// Analysis Period
        /// </summary>
        protected MediaSchedulePeriod _period;
        /// <summary>
        /// Zoom Period
        /// </summary>
        protected string _zoom;
        /// <summary>
        /// Cancel Zoom Button
        /// </summary>
        public string zoomButton = string.Empty;
        /// <summary>
        /// Change zoom period
        /// </summary>
        public string SetZoom = string.Empty;
        /// <summary>
        /// Initial Period Detail Saving
        /// </summary>
        ConstantesPeriod.DisplayLevel _savePeriod = ConstantesPeriod.DisplayLevel.monthly;
        /// <summary>
        /// Current Unit Saved
        /// </summary>
        private WebConstantes.CustomerSessions.Unit _savedUnit;
        /// <summary>
        /// Niveau de la nomenclature produit
        /// </summary>
        string Level = string.Empty;
        /// <summary>
        /// Id de nomenclature
        /// </summary>
        string id = string.Empty;
        /// <summary>
        /// Current Module Save
        /// </summary>
        long _saveModule;

        private bool _isNewCreativesModule = false;

        /// <summary>
        /// Zoom Period
        /// </summary>
        protected string _idUnit= string.Empty;
        #endregion


        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                #region Flash d'attente
                Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                Page.Response.Flush();
                #endregion

                #region Texte et langage du site
                InformationWebControl1.Language = _webSession.SiteLanguage;
                #endregion

                #region Period Detail
                if (string.IsNullOrEmpty(_zoom))
                {
                    if (!IsPostBack)
                    {
                        OptionLayerWebControl1.PeriodDetailControl.Select(_webSession.DetailPeriod);
                        if (_isNewCreativesModule)
                        {
                            OptionLayerWebControl1.UnitControl.Select(!string.IsNullOrEmpty(_idUnit) 
                                ? (WebConstantes.CustomerSessions.Unit)Convert.ToInt32(_idUnit) : _webSession.Unit);
                            GenericMediaScheduleWebControl1.CurrentUnit = !string.IsNullOrEmpty(_idUnit) 
                                ? (WebConstantes.CustomerSessions.Unit)Convert.ToInt32(_idUnit) : _webSession.Unit;
                            _webSession.Unit = GenericMediaScheduleWebControl1.CurrentUnit;
                        }
                    }
                    else
                    {
                        _webSession.DetailPeriod = OptionLayerWebControl1.PeriodDetailControl.SelectedValue;
                        if (_isNewCreativesModule)
                        {
                            _webSession.Unit = OptionLayerWebControl1.UnitControl.SelectedValue;
                            GenericMediaScheduleWebControl1.CurrentUnit = _webSession.Unit;
                        }
                    }
                }
                else
                {
                    if(_isNewCreativesModule)
                    {
                       
                        if (!IsPostBack)
                        {
                            if (!string.IsNullOrEmpty(_idUnit))
                            {
                                GenericMediaScheduleWebControl1.CurrentUnit =
                                    (WebConstantes.CustomerSessions.Unit) Convert.ToInt32(_idUnit);
                                OptionLayerWebControl1.UnitControl.Select(GenericMediaScheduleWebControl1.CurrentUnit);
                                _webSession.Unit = GenericMediaScheduleWebControl1.CurrentUnit;
                            }
                        }
                        else
                        {
                            GenericMediaScheduleWebControl1.CurrentUnit = OptionLayerWebControl1.UnitControl.SelectedValue;
                            _webSession.Unit = GenericMediaScheduleWebControl1.CurrentUnit;
                            _idUnit = GenericMediaScheduleWebControl1.CurrentUnit.GetHashCode().ToString();
                        }
                    }
                    zoomButton = "<tr bgcolor=\"#ffffff\" ><td colspan=\"2\" align=\"left\"><object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\"";
                    zoomButton += " codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"30\"";
                    zoomButton +=
                        string.Format(
                            " height=\"8\" VIEWASTEXT><param name=movie value=\"/App_Themes/{0}/Flash/Common/Arrow_Back.swf\">"
                            , Theme);
                    zoomButton += "<param name=quality value=\"high\"><param name=menu value=\"false\">";
                    zoomButton +=
                        string.Format(
                            "<embed src=\"/App_Themes/{0}/Flash/Common/Arrow_Back.swf\" width=\"30\" height=\"8\" quality=\"high\"",
                            Theme);
                    zoomButton +=
                        " pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" menu=\"false\">";

                    if (_isNewCreativesModule)
                    {
                        zoomButton += string.Format("</embed></object><a class=\"roll06\" href=\"/Private/Results/MediaSchedulePopUp.aspx?idSession={0}&u={2}\">{1}</a>",
                                                    _webSession.IdSession,
                                                    GestionWeb.GetWebWord(2309, _webSession.SiteLanguage), GenericMediaScheduleWebControl1.CurrentUnit.GetHashCode());
                    }
                    else
                    {
                        zoomButton += string.Format("</embed></object><a class=\"roll06\" href=\"/Private/Results/MediaSchedulePopUp.aspx?idSession={0}\">{1}</a>",
                       _webSession.IdSession,
                       GestionWeb.GetWebWord(2309, _webSession.SiteLanguage));
                    }

                    zoomButton += "</td></tr><tr><td bgColor=\"#ffffff\" height=\"5\"></td></tr>";

                }
                #endregion

                DetailPeriodWebControl1.WebSession = _webSession;
                DetailPeriodWebControl1.ModuleId = TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA;
                if (!string.IsNullOrEmpty(_zoom)) DetailPeriodWebControl1.DisplayContent = false;

                #region Menu
                MenuWebControl1.ForcePrint = (_isNewCreativesModule) ? string.Format("/Private/Results/Excel/MediaPlanResults.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}&u={4}",
                    this._webSession.IdSession,
                    _zoom,
                    id,
                    Level, _webSession.Unit.GetHashCode())
                    : string.Format("/Private/Results/Excel/MediaPlanResults.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}",
                    this._webSession.IdSession,
                    _zoom,
                    id,
                    Level);

                MenuWebControl1.ForceExcelUnit = (_isNewCreativesModule) ? string.Format("/Private/Results/ValueExcel/MediaPlanResults.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}&u={4}",
                    this._webSession.IdSession,
                    _zoom,
                    id,
                    Level, _webSession.Unit.GetHashCode())
                    : string.Format("/Private/Results/ValueExcel/MediaPlanResults.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}",
                    this._webSession.IdSession,
                    _zoom,
                    id,
                    Level);

                var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
                if (module != null && module.GetResultPageInformation(0) != null && module.GetResultPageInformation(0).CanDisplayRawExcelPage())
                {
                    MenuWebControl1.ForceRawExcel = (_isNewCreativesModule) ? string.Format("/Private/Results/RawExcel/MediaPlanResults.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}&u={4}",
               _webSession.IdSession,
               _zoom,
               id,
                     Level, _webSession.Unit.GetHashCode())
                    : string.Format("/Private/Results/RawExcel/MediaPlanResults.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}",
               _webSession.IdSession,
               _zoom,
               id,
               Level);
                }

                MenuWebControl1.ForcePdfExportResult = (_isNewCreativesModule) ? 
                    string.Format("/Private/MyAdExpress/PdfSavePopUp.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}&m={4}&u={5}",
                    this._webSession.IdSession,
                    _zoom,
                    id,
                    Level, WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA,_webSession.Unit.GetHashCode())
                    : string.Format("/Private/MyAdExpress/PdfSavePopUp.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}&m={4}",
                    this._webSession.IdSession,
                    _zoom,
                    id,
                    Level, WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
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

        #region DeterminePostBackMode

        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            _isNewCreativesModule =
                (_webSession.CurrentModule == WebConstantes.Module.Name.NEW_CREATIVES);
            #region Session Init
            MenuWebControl1.CustomerWebSession = _webSession;
            MenuWebControl1.ForbidHelpPages = true;

            OptionLayerWebControl1.CustomerWebSession = _webSession;
            OptionLayerWebControl1.PeriodDetailControl.ListCssClass = "txtNoir11Bold";

            GenericMediaScheduleWebControl1.CustomerWebSession = _webSession;
           

            SubPeriodSelectionWebControl1.WebSession = _webSession;
            _saveModule = _webSession.CurrentModule;
            _savedUnit = _webSession.Unit;
            // On force l'initialisation du composant avec les valeurs du plan media
            OptionLayerWebControl1.ForceModuleId = WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;

            if (_isNewCreativesModule)
            {
                OptionLayerWebControl1.UnitControl.ListCssClass = "txtNoir11Bold";
                GenericMediaScheduleWebControl1.UseCurrentUnit = true;              
                OptionLayerWebControl1.DisplayUnitOption = true;
                _idUnit = Page.Request.QueryString.Get("u");
            }
            #endregion

            #region Period Init
            _zoom = Page.Request.QueryString.Get("zoomDate");
         
            if (!string.IsNullOrEmpty(_zoom))
            {
                if (_isNewCreativesModule)
                {
                    OptionLayerWebControl1.DisplayUnitOption = true;                   
                    if (!IsPostBack && !string.IsNullOrEmpty(_idUnit))
                    {                      
                        GenericMediaScheduleWebControl1.CurrentUnit = (WebConstantes.CustomerSessions.Unit)Convert.ToInt32(_idUnit);
                        OptionLayerWebControl1.UnitControl.Select(GenericMediaScheduleWebControl1.CurrentUnit);
                    }             
                    else GenericMediaScheduleWebControl1.CurrentUnit = OptionLayerWebControl1.UnitControl.SelectedValue;
                }

                if (Page.Request.Form.GetValues("zoomParam") != null && Page.Request.Form.GetValues("zoomParam")[0].Length > 0)
                {
                    _zoom = Page.Request.Form.GetValues("zoomParam")[0];
                }
                OptionLayerWebControl1.DisplayPeriodDetailOption = false;
   
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
                var js = new StringBuilder();
                js.Append("\r\n<script type=\"text/javascript\">");
                js.Append("\r\nfunction SetZoom(){");
                js.AppendFormat("\r\n\tif ({0} == '')", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.Append("\r\n\t{");
                js.AppendFormat("\r\n\t\tdocument.location='/Private/Results/MediaSchedulePopUp.aspx?idSession={0}'"
                    , _webSession.IdSession);
                js.Append("\r\n\t}");
                js.Append("\r\n\telse {");
                js.AppendFormat("\r\n\t\t{0}();", GenericMediaScheduleWebControl1.RefreshDataMethod);
                js.AppendFormat("\r\n\t\tvar date = document.getElementById(\"zoomParam\").value;");
                js.AppendFormat("\r\n\t\tmenu.items.printMenuItem.actionOnClick = menu.items.printMenuItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});"
                    , SubPeriodSelectionWebControl1.PeriodContainerName);
                js.AppendFormat("\r\n\t\tmenu.items.excelUnitItem.actionOnClick = menu.items.excelUnitItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});"
                    , SubPeriodSelectionWebControl1.PeriodContainerName);
                js.AppendFormat("\r\n\t\tmenu.items.pdfExportResultItem.actionOnClick = menu.items.pdfExportResultItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});"
                    , SubPeriodSelectionWebControl1.PeriodContainerName);

                //Debut export Excel brute
                js.AppendFormat("\r\n\t\tmenu.itemsexcelExportItem.actionOnClick = menu.items.excelExportItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});"
                    , SubPeriodSelectionWebControl1.PeriodContainerName);
                //Fin export Excel brute

                js.AppendFormat("\r\n\t\tdocument.getElementById(\"zoomParam\").value = {0};"
                    , SubPeriodSelectionWebControl1.PeriodContainerName);
                js.Append("\r\n\t}");
                js.Append("\r\n}");
                js.Append("\r\n</script>");
                SetZoom = js.ToString();

                #region script
                if (!this.ClientScript.IsClientScriptBlockRegistered("SetZoom")) this.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetZoom", SetZoom);
                #endregion

                #endregion
            }
            else
            {

                if (_isNewCreativesModule)
                {
                    OptionLayerWebControl1.DisplayUnitOption = true;
                    if (!string.IsNullOrEmpty(_idUnit))
                    {                  
                        GenericMediaScheduleWebControl1.CurrentUnit = (WebConstantes.CustomerSessions.Unit)Convert.ToInt32(_idUnit);
                        OptionLayerWebControl1.UnitControl.Select(GenericMediaScheduleWebControl1.CurrentUnit);
                    } 
                    else GenericMediaScheduleWebControl1.CurrentUnit = OptionLayerWebControl1.UnitControl.SelectedValue;
                }

                SubPeriodSelectionWebControl1.Visible = false;
                OptionLayerWebControl1.DisplayPeriodDetailOption = true;       
                _webSession.DetailPeriod = OptionLayerWebControl1.PeriodDetailControl.SelectedValue;
                
                if (_webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly)
                {
                    DateTime begin = Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                    if (begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                    {
                        _webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
                    }
                }


            }
            #endregion

            #region Classification Filter Init
            if (Page.Request.QueryString.Get("id") != null) id = Page.Request.QueryString.Get("id").ToString();
            if (Page.Request.QueryString.Get("Level") != null) Level = Page.Request.QueryString.Get("Level").ToString();

            if (id.Length > 0 && Level.Length > 0)
            {
                SetProduct(int.Parse(id), int.Parse(Level));
            }
            #endregion

            return tmp;

        }
        #endregion

        #region OnPreRender

        protected override void OnPreRender(EventArgs e)
        {

            try
            {

                #region MAJ _webSession
                if (!string.IsNullOrEmpty(_zoom))
                {
                    _webSession.DetailPeriod = _savePeriod;
                }
                _webSession.Unit = _savedUnit;
                _webSession.CurrentModule = _saveModule;
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

        #region Identifiant des éléments de la nomenclature produit
        /// <summary>
        /// Set Product classification filter
        /// </summary>
        /// <param name="id">Element ID</param>
        /// <param name="level">Element Classification level</param>
        private void SetProduct(int id, int level)
        {
            WebFunctions.ProductDetailLevel.SetProductLevel(_webSession, id, level);
        }
        #endregion

    }

}