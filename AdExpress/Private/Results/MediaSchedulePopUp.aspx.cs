using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Windows.Forms;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Controls.Results.MediaPlan;
using AjaxPro;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.DataAccess.Classification.ProductBranch;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;


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
        public string zoomButton = "";
        /// <summary>
        /// Change zoom period
        /// </summary>
        public string SetZoom = "";
        /// <summary>
        /// Initial Period Detail Saving
        /// </summary>
        ConstantesPeriod.DisplayLevel _savePeriod = ConstantesPeriod.DisplayLevel.monthly;
        /// <summary>
        /// Niveau de la nomenclature produit
        /// </summary>
        string Level = "";
        /// <summary>
        /// Id de nomenclature
        /// </summary>
        string id = "";
        /// <summary>
        /// Current Module Save
        /// </summary>
        long _saveModule;
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
                if (_zoom == null || _zoom == string.Empty)
                {
                    if (!IsPostBack)
                    {
                        OptionLayerWebControl1.PeriodDetailControl.Select(_webSession.DetailPeriod);
                    }
                    else
                    {
                        _webSession.DetailPeriod = OptionLayerWebControl1.PeriodDetailControl.SelectedValue;
                    }
                }
                else
                {
                    zoomButton = string.Format("<tr bgcolor=\"#ffffff\" ><td colspan=\"2\" align=\"left\"><object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"30\" height=\"8\" VIEWASTEXT><param name=movie value=\"/App_Themes/" + this.Theme + "/Flash/Common/Arrow_Back.swf\"><param name=quality value=\"high\"><param name=menu value=\"false\"><embed src=\"/App_Themes/" + this.Theme + "/Flash/Common/Arrow_Back.swf\" width=\"30\" height=\"8\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" menu=\"false\"></embed></object><a class=\"roll06\" href=\"/Private/Results/MediaSchedulePopUp.aspx?idSession={0}\">{2}</a></td></tr><tr><td bgColor=\"#ffffff\" height=\"5\"></td></tr>",
                        _webSession.IdSession,
                        _webSession.SiteLanguage,
                        GestionWeb.GetWebWord(2309, _webSession.SiteLanguage));

                }
                #endregion

                #region Menu
                MenuWebControl1.ForcePrint = string.Format("/Private/Results/Excel/MediaPlanResults.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}",
                    this._webSession.IdSession,
                    _zoom,
                    id,
                    Level);
                MenuWebControl1.ForceExcelUnit = string.Format("/Private/Results/ValueExcel/MediaPlanResults.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}",
                    this._webSession.IdSession,
                    _zoom,
                    id,
                    Level);
                MenuWebControl1.ForcePdfExportResult = string.Format("/Private/MyAdExpress/PdfSavePopUp.aspx?idSession={0}&zoomDate={1}&id={2}&Level={3}",
                    this._webSession.IdSession,
                    _zoom,
                    id,
                    Level);
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

            #region Session Init
            MenuWebControl1.CustomerWebSession = _webSession;
            MenuWebControl1.ForbidHelpPages = true;

            OptionLayerWebControl1.CustomerWebSession = _webSession;
            OptionLayerWebControl1.PeriodDetailControl.ListCssClass = "txtNoir11Bold";
            GenericMediaScheduleWebControl1.CustomerWebSession = _webSession;
            SubPeriodSelectionWebControl1.WebSession = _webSession;
            _saveModule = _webSession.CurrentModule;
            // On force l'initialisation du composant avec les valeurs du plan media
            OptionLayerWebControl1.ForceModuleId = WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;
            #endregion

            #region Period Init
            _zoom = Page.Request.QueryString.Get("zoomDate");
            if (_zoom != null && _zoom != string.Empty)
            {

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
                StringBuilder js = new StringBuilder();
                js.Append("\r\n<script type=\"text/javascript\">");
                js.Append("\r\nfunction SetZoom(){");
                js.AppendFormat("\r\n\tif ({0} == '')", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.Append("\r\n\t{");
                js.AppendFormat("\r\n\t\tdocument.location='/Private/Results/MediaSchedulePopUp.aspx?idSession={0}'", _webSession.IdSession);
                js.Append("\r\n\t}");
                js.Append("\r\n\telse {");
                js.AppendFormat("\r\n\t\t{0}();", GenericMediaScheduleWebControl1.RefreshDataMethod);
                js.AppendFormat("\r\n\t\tvar date = document.getElementById(\"zoomParam\").value;");
                js.AppendFormat("\r\n\t\tmenu.items.printMenuItem.actionOnClick = menu.items.printMenuItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.AppendFormat("\r\n\t\tmenu.items.excelUnitItem.actionOnClick = menu.items.excelUnitItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.AppendFormat("\r\n\t\tmenu.items.pdfExportResultItem.actionOnClick = menu.items.pdfExportResultItem.actionOnClick.replace(\"zoomDate=\"+date,\"zoomDate=\"+{0});", SubPeriodSelectionWebControl1.PeriodContainerName);
                js.AppendFormat("\r\n\t\tdocument.getElementById(\"zoomParam\").value = {0};", SubPeriodSelectionWebControl1.PeriodContainerName);
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
                OptionLayerWebControl1.DisplayPeriodDetailOption = true;
                SubPeriodSelectionWebControl1.Visible = false;
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
                if (_zoom != null && _zoom != string.Empty)
                {
                    _webSession.DetailPeriod = _savePeriod;
                }
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
            System.Windows.Forms.TreeNode tree = new System.Windows.Forms.TreeNode();
            switch ((DetailLevelItemInformation.Levels)_webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(level))
            {
                case DetailLevelItemInformation.Levels.sector:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess, id, (new PartialSectorLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.sector, tree);
                    break;
                case DetailLevelItemInformation.Levels.subSector:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess, id, (new PartialSubSectorLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.subsector, tree);
                    break;
                case DetailLevelItemInformation.Levels.group:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess, id, (new PartialGroupLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.group, tree);
                    break;
                case DetailLevelItemInformation.Levels.segment:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess, id, (new PartialSegmentLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.segment, tree);
                    break;
                case DetailLevelItemInformation.Levels.product:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productAccess, id, (new PartialProductLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.product, tree);
                    break;
                case DetailLevelItemInformation.Levels.advertiser:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess, id, (new PartialAdvertiserLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.advertiser, tree);
                    break;
                case DetailLevelItemInformation.Levels.brand:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.brandAccess, id, (new PartialBrandLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.brand, tree);
                    break;
                case DetailLevelItemInformation.Levels.holdingCompany:
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess, id, (new PartialHoldingCompanyLevelListDataAccess(id.ToString(), _webSession.DataLanguage, _webSession.Source))[id].ToString());
                    tree.Checked = true;
                    _webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.holding_company, tree);
                    break;
            }
        }
        #endregion

    }

}