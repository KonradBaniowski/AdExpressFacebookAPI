
#region Namespaces
using System;
using System.Text;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Functions;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using WebConstantes = TNS.AdExpress.Constantes.Web;

#endregion


namespace AdExpress.Private.Results
{
    
/// <summary>
/// Celebrities module result page
/// </summary>
public partial class CelebritiesResults : TNS.AdExpress.Web.UI.BaseResultWebPage
{
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
    ///Text  Controle
    /// </summary>
    protected TNS.AdExpress.Web.Controls.Translation.AdExpressText AdExpressText1;
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public CelebritiesResults(): base()
    {
    }
    #endregion


    #region Events

    #region Page Load
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, System.EventArgs e)
    {
        try
        {
            bool withZoomDateEventArguments = false;

            #region Flash d'attente
            if (Page.Request.Form.GetValues("__EVENTTARGET") != null)
            {
                string nomInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];
                if (nomInput != MenuWebControl2.ID)
                {
                    Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage, Page));
                    Page.Response.Flush();
                }
                else
                {
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
            if (_nextUrl.Length != 0)
            {
                _webSession.Source.Close();
                Response.Redirect(_nextUrl + "?idSession=" + _webSession.IdSession + ((withZoomDateEventArguments && !string.IsNullOrEmpty(_zoom)) ? "&zoomDate=" + _zoom + "&detailPeriod=" + _webSession.DetailPeriod.GetHashCode() : ""));
            }
            #endregion

            #region Validation du menu
            if (Page.Request.QueryString.Get("validation") == "ok")
            {
                _webSession.Save();
            }
            #endregion

            #region Texte et langage du site

            Moduletitlewebcontrol2.CustomerWebSession = _webSession;
            InformationWebControl1.Language = _webSession.SiteLanguage;

            #endregion

            #region Period Detail
            if (!string.IsNullOrEmpty(_zoom))
            {
                zoomButton = string.Format("<tr><td align=\"left\"><object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"30\" height=\"8\" VIEWASTEXT><param name=movie value=\"/App_Themes/" + this.Theme + "/Flash/Common/Arrow_Back.swf\"><param name=quality value=\"high\"><param name=menu value=\"false\"><embed src=\"/App_Themes/" + this.Theme + "/Flash/Common/Arrow_Back.swf\" width=\"30\" height=\"8\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" menu=\"false\"></embed></object><a class=\"roll06\" href=\"/Private/Results/MediaPlanResults.aspx?idSession={0}\">{2}</a></td></tr><tr><td height=\"5\"></td></tr>",
                    _webSession.IdSession,
                    _webSession.SiteLanguage,
                    GestionWeb.GetWebWord(2309, _webSession.SiteLanguage));
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
    }
    #endregion

    #region DeterminePostBack
    /// <summary>
    /// Détermine la valeur de PostBack
    /// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
    /// </summary>
    /// <returns>DeterminePostBackMode</returns>
    protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
    {
        System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
        ResultsOptionsWebControl1.CustomerWebSession = _webSession;
        MenuWebControl2.CustomerWebSession = _webSession;
        GenericMediaScheduleWebControl1.CustomerWebSession = _webSession;
        GenericMediaScheduleWebControl1.Module = ModulesList.GetModule(WebConstantes.Module.Name.CELEBRITIES);
        SubPeriodSelectionWebControl1.WebSession = _webSession;

        #region Period Detail
        _zoom = Page.Request.QueryString.Get("zoomDate");
        if (!string.IsNullOrEmpty(_zoom))
        {
            string[] strings = Page.Request.Form.GetValues("zoomParam");
            if (strings != null && (strings[0].Length > 0))
            {
                _zoom = strings[0];
            }
            MenuWebControl2.UrlPameters = string.Format("zoomDate={0}", _zoom);
            MenuWebControl2.ForbidSave = true;
            SubPeriodSelectionWebControl1.Visible = true;
            SubPeriodSelectionWebControl1.AllPeriodAllowed = false;

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
            js.AppendFormat("\r\n\t\tdocument.location='/Private/Results/CelebritiesResults.aspx?idSession={0}'", _webSession.IdSession);
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

        #endregion

            #region Script
            if (!this.ClientScript.IsClientScriptBlockRegistered("SetZoom"))
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetZoom", SetZoom);
            #endregion

        }
        else
        {
            SubPeriodSelectionWebControl1.Visible = false;
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

        

        return (tmp);
    }

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
}
}