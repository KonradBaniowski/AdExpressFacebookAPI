#region Information
/*
 * Author : 
 * Creation : 
 * Modifications:
 *		G Ragneau - 03/08/2006 - Add result pages management
 * 
 * 
 * 
 * 
 * */
#endregion

using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Translation;
using WebCst = TNS.AdExpress.Constantes.Web;
using WebFunction = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Classification;
using TNS.Ares.Domain.LS;
using TNS.Ares.Alerts.DAL;
using TNS.Ares.Domain.Layers;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.Alert.Domain;
using TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Web.Controls.Headers
{
    /// <summary>
    /// Composant du menu contextuel du bouton droit de la souris
    /// </summary>
    [ToolboxData("<{0}:MenuWebControl runat=server></{0}:MenuWebControl>")]
    public class MenuWebControl : System.Web.UI.WebControls.WebControl
    {

        #region Constantes
        /// <summary>
        /// Help window width
        /// </summary>
        protected const string HELP_PAGE_WIDTH = "710";
        /// <summary>
        /// Help window height
        /// </summary>
        protected const string HELP_PAGE_HEIGHT = "700";
        /// <summary>
        /// Selection Detail window width
        /// </summary>
        protected const string SELECTION_PAGE_WIDTH = "750";
        /// <summary>
        /// Selection Detail window height
        /// </summary>
        protected const string SELECTION_PAGE_HEIGHT = "700";
        /// <summary>
        /// Excel menu name
        /// </summary>
        protected const string EXCEL_MENU = "exportMenu";
        /// <summary>
        /// Main menu name
        /// </summary>
        protected const string MAIN_MENU = "menu";
        #endregion

        #region Variables
        /// <summary>
        /// Session du client
        /// </summary>
        protected WebSession _webSession = null;
        /// <summary>
        /// Current Module
        /// </summary>
        protected Module _module = null;
        /// <summary>
        /// Url suivante
        /// </summary>
        protected string nextUrl = "";
        /// <summary>
        /// Use to forbid the saving actions to display (kind of rescue wheel)
        /// </summary>
        protected bool _forbidSave = false;
        /// <summary>
        /// Use to forbid the recall actions to display (kind of rescue wheel)
        /// </summary>
        protected bool _forbidRecall = false;
        /// <summary>
        /// Use to forbid the actions to display opttional pages (kind of rescue wheel)
        /// </summary>
        protected bool _forbidOptionsPages = false;
        /// <summary>
        /// Use to forbid the actions to display help and selection detail pages (kind of rescue wheel)
        /// </summary>
        protected bool _forbidHelpPages = false;

        /// <summary>
        /// Boolean to force the displaying of detail selection
        /// </summary>
        protected bool _forceDetailSelection = false;

        /// <summary>
        /// Force the menu to display the print function
        /// </summary>
        protected string _forcePrint = string.Empty;
        /// <summary>
        /// Force the menu to display the excel export function
        /// </summary>
        protected string _forceExcelUnit = string.Empty;
        /// <summary>
        /// Force the menu to display HELP function
        /// </summary>
        protected string _forceHelp = string.Empty;
        /// <summary>
        /// Use to forbid the actions to display result icon
        /// </summary>
        protected bool _forbidResultIcon = false;
        /// <summary>
        /// Traduction code of option to force the menu to display the print function
        /// </summary>
        protected long _forcePrintTraductionCode = -1;
        /// <summary>
        /// List of opttional pages to forbid
        /// </summary>
        protected ArrayList _forbidOptionsPagesList = null;
        /// <summary>
        /// Print html pop up page
        /// </summary>
        protected bool _displayHtmlPrint = false;
        /// <summary>
        /// Force the menu to display the pdf export function
        /// </summary>
        protected string _forcePdfExportResult = string.Empty;
        /// <summary>
        /// Parameters to concatenate to the URL
        /// </summary>
        protected string _urlParameters = string.Empty;
        /// <summary>
        /// ID label export text
        /// </summary>
        private int _textExportWebtextId = 1913;
        #endregion

        #region Accesseurs
        /// <summary>
        /// Définit la session du client
        /// </summary>
        [Bindable(false)]
        public WebSession CustomerWebSession
        {
            set { _webSession = value; }
        }
        /// <summary>
        /// Obtient l'adresse de la page suivante
        /// </summary>
        public string NextUrl
        {
            get { return (this.nextUrl); }
        }
        /// <summary>
        /// Get / Set _forbidSave. Use to forbid the saving actions to display (kind of rescue wheel)
        /// </summary>
        public bool ForbidSave
        {
            get { return _forbidSave; }
            set { _forbidSave = value; }
        }
        /// <summary>
        /// Get / Set _forbidRecall. Use to forbid the recall actions to display (kind of rescue wheel)
        /// </summary>
        public bool ForbidRecall
        {
            get { return _forbidRecall; }
            set { _forbidRecall = value; }
        }
        /// <summary>
        /// Get / Set _forbidOptionPages. Use to forbid the actions to display optional pages (kind of rescue wheel)
        /// </summary>
        public bool ForbidOptionPages
        {
            get { return _forbidOptionsPages; }
            set { _forbidOptionsPages = value; }
        }
        /// <summary>
        /// Get / Set _forbidHelpPages. Use to forbid the actions to display help and selection detail pages (kind of rescue wheel)
        /// </summary>
        public bool ForbidHelpPages
        {
            get { return _forbidHelpPages; }
            set { _forbidHelpPages = value; }
        }

        /// <summary>
        /// Use to force the displaying of detail selection
        /// </summary>
        public bool ForceDetailSelection
        {
            get { return _forceDetailSelection; }
            set { _forceDetailSelection = value; }
        }

        /// <summary>
        /// Force the menu to display the print function
        /// </summary>
        public string ForcePrint
        {
            get { return _forcePrint; }
            set { _forcePrint = value; }
        }
        /// <summary>
        /// Force the menu to display the excel export function
        /// </summary>
        public string ForceExcelUnit
        {
            get { return _forceExcelUnit; }
            set { _forceExcelUnit = value; }
        }
        /// <summary>
        /// Force the menu to display the help function
        /// </summary>
        public string ForceHelp
        {
            get { return _forceHelp; }
            set { _forceHelp = value; }
        }

        /// <summary>
        /// Use to forbid the actions to display result icon
        /// </summary>
        public bool ForbidResultIcon
        {
            get { return _forbidResultIcon; }
            set { _forbidResultIcon = value; }
        }

        /// <summary>
        /// Traduction code of option to force the menu to display the print function
        /// </summary>
        public long ForcePrintTraductionCode
        {
            get { return _forcePrintTraductionCode; }
            set { _forcePrintTraductionCode = value; }
        }

        /// <summary>
        /// Get / Set _forbidOptionPagesList. Use to forbid optional pages 
        /// </summary>
        public ArrayList ForbidOptionPagesList
        {
            get { return _forbidOptionsPagesList; }
            set { _forbidOptionsPagesList = value; }
        }

        /// <summary>
        /// Print html pop up page
        /// </summary>
        public bool DisplayHtmlPrint
        {
            get { return _displayHtmlPrint; }
            set { _displayHtmlPrint = value; }
        }

        /// <summary>
        /// Force the menu to display the pdf export function
        /// </summary>
        public string ForcePdfExportResult
        {
            get { return _forcePdfExportResult; }
            set { _forcePdfExportResult = value; }
        }
        /// <summary>
        /// Let the user specify additional parameters to concatenate to the URL
        /// </summary>
        public string UrlPameters
        {
            get { return _urlParameters; }
            set { _urlParameters = value; }
        }

        /// <summary>
        /// ID label export text
        /// </summary>
        public int TextExportWebtextId
        {
            set { _textExportWebtextId = value; }
        }

        #endregion

        #region JavaScript
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetMenuCss()
        {
            return ("<link rel=\"stylesheet\" type=\"text/css\" href=\"/Css/Menu.css\">");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetMenuJavaScript()
        {
            return ("<script type=\"text/javascript\" src=\"/scripts/jsdomenu.js\"></script>");
        }
        #endregion

        #region Evènements

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            try
            {
                _module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
                if (this.Page.IsPostBack)
                {
                    string nomInput = Page.Request.Form.GetValues("__EVENTTARGET")[0];
                    if (nomInput == this.ID)
                    {
                        string valueInput = Page.Request.Form.GetValues("__EVENTARGUMENT")[0];
                        foreach (SelectionPageInformation currentPage in _module.SelectionsPages)
                        {
                            if (currentPage.Id == int.Parse(valueInput))
                            {
                                nextUrl = currentPage.Url;
                                break;
                            }
                            else
                            {
                                if ((((SelectionPageInformation)currentPage).HtSubSelectionPageInformation != null) && (((SelectionPageInformation)currentPage).HtSubSelectionPageInformation.Count > 0))
                                {
                                    IDictionaryEnumerator myEnumerator = ((SelectionPageInformation)currentPage).HtSubSelectionPageInformation.GetEnumerator();
                                    while (myEnumerator.MoveNext())
                                    {
                                        if (((PageInformation)myEnumerator.Value).Id == int.Parse(valueInput))
                                        {
                                            nextUrl = ((PageInformation)myEnumerator.Value).Url;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        foreach (OptionalPageInformation currentPage in _module.OptionalsPages)
                        {
                            if (currentPage.Id == int.Parse(valueInput))
                            {
                                nextUrl = currentPage.Url;
                                break;
                            }
                        }

                        if (int.Parse(valueInput) == 9999)
                        {
                            nextUrl = _module.GetResultPageInformation(Convert.ToInt32(_webSession.CurrentTab)).Url;
                        }
                    }
                }
            }
            catch (System.Exception err)
            {
                string t = err.Message;
            }
        }
        #endregion

        #region PréRender
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {

            if (!Page.ClientScript.IsClientScriptBlockRegistered(WebFunction.Script.GET_SELECTION_JS_REGISTER))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), WebFunction.Script.GET_SELECTION_JS_REGISTER, WebFunction.Script.GetSelectionDetailJS());
            }
            if (!Page.ClientScript.IsClientScriptBlockRegistered(WebFunction.Script.RESIZABLE_POPUP_REGISTER))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), WebFunction.Script.RESIZABLE_POPUP_REGISTER, WebFunction.Script.ReSizablePopUp());
            }
            if (!Page.ClientScript.IsClientScriptBlockRegistered(WebFunction.Script.NEW_WINDOW_REGISTER))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), WebFunction.Script.NEW_WINDOW_REGISTER, WebFunction.Script.OpenNewWindow());
            }

            //if(! Page.ClientScript.IsClientScriptBlockRegistered("MenuCSS"))Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"MenuCSS",GetMenuCss());
            if (!Page.ClientScript.IsClientScriptBlockRegistered("MenuJavaScript")) Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MenuJavaScript", GetMenuJavaScript());
            if (!Page.ClientScript.IsClientScriptBlockRegistered("AddJsEvent")) Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AddJsEvent", WebFunction.Script.AddJsEvent());
            base.OnPreRender(e);

        }

        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output)
        {
            if (_webSession == null)
                output.Write("AdExpress Menu");
            output.Write(GetMenuCreation());
        }
        #endregion

        #endregion

        #region Méthodes internes
        /// <summary>
        /// Méthode pour la construction de la fonction javascript du menu contextuel
        /// </summary>
        /// <returns>Code Javascript</returns>
        private string GetMenuCreation()
        {
            bool export = false;
            string jsTmp = string.Empty;
            StringBuilder js = new StringBuilder(10000);
            js.Append("\r\n<script>");
            js.Append("\r\n\tfunction createjsDOMenu() {");
            js.Append("\r\n\t\t" + MAIN_MENU + " = new jsDOMenu(205);");

            // Module loading
            _module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);

            if (_module != null)
            {
                // Get current page
                PageInformation pInfo = _module.GetPageInformation(this.Page.Request.Url.AbsolutePath, _webSession.CurrentTab);

                if (pInfo != null)
                {
                    // Recall Selection
                    if (pInfo != null && pInfo.AllowRecall && !this._forbidRecall)
                    {
                        jsTmp = GetRecallSelectionItem(MAIN_MENU, pInfo);
                        js.Append(jsTmp);
                    }

                    // Result pages managment
                    export = false;
                    if (pInfo != null && pInfo.GetType() == typeof(ResultPageInformation))
                    {
                        // Separator
                        if (jsTmp.Length > 0)
                            js.Append(GetSeparator(MAIN_MENU));

                        // Selection refine
                        jsTmp = GetRefineSelectionItem(MAIN_MENU);
                        js.Append(jsTmp);

                        // Separator
                        if (jsTmp.Length > 0)
                            js.Append(GetSeparator(MAIN_MENU));

                        // Save
                        if (!_forbidSave)
                        {
                            jsTmp = this.GetSaveItem(MAIN_MENU);
                            js.Append(jsTmp);
                        }

                        // Alert
                        if (AlertConfiguration.IsActivated
                            && ((ResultPageInformation)pInfo).CreateAlertUrl != ""
                            && _webSession.CustomerLogin.HasModuleAssignmentAlertsAdExpress()
                            && _webSession.CustomerLogin.IsModuleAssignmentValidDateAlertsAdExpress())
                        {
                            DataAccessLayer layer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
                            TNS.FrameWork.DB.Common.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
                            IAlertDAL alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, new object[] { src }, null, null, null);
                            int nbUserAlert = alertDAL.GetAlerts(_webSession.CustomerLogin.IdLogin).Count;
                            if (_webSession.CustomerLogin.GetNbAlertsAdExpress() > nbUserAlert)
                            {
                                jsTmp = this.GetCreateAlertItem(MAIN_MENU, (ResultPageInformation)pInfo, true);
                            }
                            else
                            {
                                // Grisé
                                jsTmp = this.GetCreateAlertItem(MAIN_MENU, (ResultPageInformation)pInfo, false);
                            }
                            js.Append(jsTmp);
                        }

                        // Export
                        int jsLength = js.Length;
                        jsTmp = this.GetExportItems(MAIN_MENU, (ResultPageInformation)pInfo);
                        js.Append(jsTmp);
                        if (js.Length > jsLength)
                            export = true;
                    }
                }

                // Force Print
                string exportMenu = EXCEL_MENU;
                if (!export)
                {
                    exportMenu = MAIN_MENU;
                }
                if (_displayHtmlPrint)
                {
                    js.Append(this.GetExportSubMenu("htmlPrintItem", GestionWeb.GetWebWord(2044, _webSession.SiteLanguage), exportMenu, "javascript:self.print();", "printHTMLMenuIcon"));
                    jsTmp = " ";
                }
                if (_forcePrint.Length > 0)
                {
                    if (this._forcePrintTraductionCode > -1)
                        js.Append(this.GetExportSubMenu("printMenuItem", GestionWeb.GetWebWord(this._forcePrintTraductionCode, _webSession.SiteLanguage), exportMenu, "javascript:OpenNewWindow('" + _forcePrint + "');", "printMenuIcon"));
                    else js.Append(this.GetExportSubMenu("printMenuItem", GestionWeb.GetWebWord(1996, _webSession.SiteLanguage), exportMenu, "javascript:OpenNewWindow('" + _forcePrint + "');", "printMenuIcon"));
                    jsTmp = " ";
                }

                // Force Excel Export
                if (_forceExcelUnit.Length > 0)
                {
                    js.Append(this.GetExportSubMenu("excelUnitItem", GestionWeb.GetWebWord(1997, _webSession.SiteLanguage), exportMenu, "javascript:OpenNewWindow('" + _forceExcelUnit + "');", "excelUnitMenuIcon"));
                    jsTmp = " ";
                }

                // Force Pdf Export
                if (_forcePdfExportResult.Length > 0)
                {
                    js.Append(this.GetExportSubMenu("pdfExportResultItem", GestionWeb.GetWebWord(2017, _webSession.SiteLanguage), exportMenu, "javascript:popupOpenBis('" + _forcePdfExportResult + "','470','210','yes');", "pdfExportMenuIcon"));
                    jsTmp = " ";
                }

                // Other
                if (!_forbidHelpPages && pInfo != null)
                {
                    // Separator
                    if (jsTmp.Length > 0)
                        js.Append(GetSeparator(MAIN_MENU));

                    // Rappel de sélection
                    js.Append(GetDetailSelectionItem(MAIN_MENU));

                    // Aide
                    js.Append(GetHelpItem(MAIN_MENU));

                    jsTmp = " ";
                }
                else if (_forceHelp.Length > 0)
                {
                    // Separator
                    if (jsTmp.Length > 0)
                        js.Append(GetSeparator(MAIN_MENU));


                    if (ForceDetailSelection)
                    {
                        // Rappel de sélection
                        js.Append(GetDetailSelectionItem(MAIN_MENU));
                    }


                    // Aide
                    if (_forceHelp.IndexOf("?") > 0) _forceHelp += "&siteLanguage=" + _webSession.SiteLanguage;
                    else _forceHelp += "?siteLanguage=" + _webSession.SiteLanguage;
                    js.Append(this.GetExportSubMenu("helpItem", GestionWeb.GetWebWord(1988, _webSession.SiteLanguage), MAIN_MENU,
                        "javascript:popupRecallOpen('" + _forceHelp + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "','" + HELP_PAGE_WIDTH + "','" + HELP_PAGE_HEIGHT + "');"
                        , "helpMenuIcon"));

                    jsTmp = " ";
                }
            }

            // Separator
            if (jsTmp.Length > 0)
                js.Append(GetSeparator(MAIN_MENU));

            // Links (display in all pages)
            js.Append(GetLinksItem(MAIN_MENU));

            js.Append("\r\n\t\tsetPopUpMenu(" + MAIN_MENU + ");");
            js.Append("\r\n\t\tactivatePopUpMenuBy(1, 2);");
            js.Append("\r\n\t}");
            js.Append("\r\n\taddJsEvent(window,\"load\",initjsDOMenu);");
            js.Append("\r\n</script>");

            return (js.ToString());
        }

        /// <summary>
        /// Recall selection
        /// </summary>
        /// <param name="menuObjectName">Nom de l'objet menu</param>
        /// <param name="pInfo">Current Page Information</param>
        /// <returns>Code Javascript</returns>
        private string GetRecallSelectionItem(string menuObjectName, PageInformation pInfo)
        {
            string js = string.Empty;
            int countValidatedSelection = 0;
            SelectionPageInformation selectedPageInformation = null;
            foreach (SelectionPageInformation currentpageInformation in _module.SelectionsPages)
            {
                if ((_webSession.CheckUnivers(currentpageInformation.ValidationMethod) || currentpageInformation.Url.Equals(this.Page.Request.Url.AbsolutePath))
                    && (this._forbidOptionsPagesList == null || !this._forbidOptionsPagesList.Contains(currentpageInformation.Id))
                    )
                {
                    countValidatedSelection++;
                    if (currentpageInformation.Url != this.Page.Request.Url.AbsolutePath && currentpageInformation.ShowLink)
                    {
                        js += "\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(currentpageInformation.MenuTextId, _webSession.SiteLanguage) + "\", \"" + currentpageInformation.IconeName + "Item\",\"javascript:__doPostBack('" + this.ID + "','" + currentpageInformation.Id + "');\"));";
                        js += "\r\n\t\t" + menuObjectName + ".items." + currentpageInformation.IconeName + "Item.showIcon(\"" + currentpageInformation.IconeName + "MenuIcon\", \"" + currentpageInformation.IconeName + "MenuIcon\");";
                    }
                    if ((((SelectionPageInformation)currentpageInformation).HtSubSelectionPageInformation != null) && (((SelectionPageInformation)currentpageInformation).HtSubSelectionPageInformation.Count > 0))
                    {
                        IDictionaryEnumerator myEnumerator = ((SelectionPageInformation)currentpageInformation).HtSubSelectionPageInformation.GetEnumerator();
                        while (myEnumerator.MoveNext())
                        {
                            if (((SubSelectionPageInformation)myEnumerator.Value).Url != this.Page.Request.Url.AbsolutePath && ((SubSelectionPageInformation)myEnumerator.Value).ShowLink)
                            {
                                js += "\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(((SubSelectionPageInformation)myEnumerator.Value).MenuTextId, _webSession.SiteLanguage) + "\", \"" + ((SubSelectionPageInformation)myEnumerator.Value).IconeName + "Item\",\"javascript:__doPostBack('" + this.ID + "','" + ((SubSelectionPageInformation)myEnumerator.Value).Id + "');\"));";
                                js += "\r\n\t\t" + menuObjectName + ".items." + ((SubSelectionPageInformation)myEnumerator.Value).IconeName + "Item.showIcon(\"" + ((SubSelectionPageInformation)myEnumerator.Value).IconeName + "MenuIcon\", \"" + ((SubSelectionPageInformation)myEnumerator.Value).IconeName + "MenuIcon\");";
                            }
                        }
                    }
                    if (currentpageInformation.Url == this.Page.Request.Url.AbsolutePath)
                        selectedPageInformation = currentpageInformation;
                }
                if (countValidatedSelection == _module.SelectionsPages.Count && pInfo.GetType() != typeof(ResultPageInformation) && !this._forbidResultIcon)
                {
                    ResultPageInformation resultPageInformation = _module.GetResultPageInformation(Convert.ToInt32(_webSession.CurrentTab));
                    if (selectedPageInformation == null)
                        js += "\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(resultPageInformation.MenuTextId, _webSession.SiteLanguage) + "\", \"resultItem\",\"javascript:__doPostBack('" + this.ID + "','9999');\"));";
                    else if (selectedPageInformation.FunctionName.Length == 0)
                        js += "\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(resultPageInformation.MenuTextId, _webSession.SiteLanguage) + "\", \"resultItem\",\"javascript:__doPostBack('" + this.ID + "','9999');\"));";
                    else
                        js += "\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(resultPageInformation.MenuTextId, _webSession.SiteLanguage) + "\", \"resultItem\",\"javascript:" + selectedPageInformation.FunctionName + "('" + this.ID + "');\"));";
                    js += "\r\n\t\t" + menuObjectName + ".items.resultItem.showIcon(\"resultMenuIcon\", \"resultMenuIcon\");";
                }
            }
            return (js);
        }

        /// <summary>
        /// Refine selection
        /// </summary>
        /// <param name="menuObjectName">Parent menu name</param>
        /// <returns>Code Javascript</returns>
        private string GetRefineSelectionItem(string menuObjectName)
        {
            string js = string.Empty;
            if (!this._forbidOptionsPages)
            {
                foreach (OptionalPageInformation cInfo in _module.OptionalsPages)
                {
                    if ((_forbidOptionsPagesList == null || !_forbidOptionsPagesList.Contains(cInfo.Id)) && cInfo.ShowLink)
                    {
                        js += "\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(cInfo.IdWebText, _webSession.SiteLanguage) + "\", \"" + cInfo.IconeName + "Item\",\"javascript:__doPostBack('" + this.ID + "','" + cInfo.Id + "');\"));";
                        js += "\r\n\t\t" + menuObjectName + ".items." + cInfo.IconeName + "Item.showIcon(\"" + cInfo.IconeName + "MenuIcon\", \"" + cInfo.IconeName + "MenuIcon\");";
                    }
                }
            }
            return (js);
        }

        /// <summary>
        /// Rappel de sélection
        /// </summary>
        /// <param name="menuObjectName">Nom de l'objet menu</param>
        /// <returns>Code Javascript</returns>
        private string GetDetailSelectionItem(string menuObjectName)
        {
            string js = string.Empty;
            js += "\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(1989, _webSession.SiteLanguage) + "\", \"selectedItem\",\"javascript:popupRecallOpen('/Private/Selection/DetailSelection.aspx?idSession=" + _webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "','" + SELECTION_PAGE_WIDTH + "','" + SELECTION_PAGE_HEIGHT + "');\"));";
            js += "\r\n\t\t" + menuObjectName + ".items.selectedItem.showIcon(\"selectedMenuIcon\", \"selectedMenuIcon\");";
            return (js);
        }

        /// <summary>
        /// Aide
        /// </summary>
        /// <param name="menuObjectName">Nom de l'objet menu</param>
        /// <returns>Code Javascript</returns>
        private string GetHelpItem(string menuObjectName)
        {
            string js = string.Empty;
            js += "\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(1988, _webSession.SiteLanguage) + "\", \"helpItem\",\"javascript:popupRecallOpen('"
                + ((PageInformation)_module.GetPageInformation(this.Page.Request.Url.AbsolutePath, _webSession.CurrentTab)).HelpUrl + "?siteLanguage=" + _webSession.SiteLanguage + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "','" + HELP_PAGE_WIDTH + "','" + HELP_PAGE_HEIGHT + "');\"));";
            js += "\r\n\t\t" + menuObjectName + ".items.helpItem.showIcon(\"helpMenuIcon\", \"helpMenuIcon\");";
            return (js);
        }

        /// <summary>
        /// Liens
        /// </summary>
        /// <param name="menuObjectName">Nom de l'objet menu</param>
        /// <returns>Code Javascript</returns>
        private string GetLinksItem(string menuObjectName)
        {
            string href = string.Empty;
            string hrefTmp = string.Empty;
            string languageString = string.Empty;
            string idSessionString = string.Empty;
            bool firstParameter = true;

            string js = string.Empty;
            RightMenuLinks links = WebApplicationParameters.RightMenuLinksInformations;
            foreach (RightMenuLinksItem cItem in links.RightMenuLinksList)
            {
                href = "";
                hrefTmp = "";
                languageString = "";
                idSessionString = "";
                firstParameter = true;

                #region Building URL
                if (cItem.UseLanguage)
                {
                    languageString = "siteLanguage=" + _webSession.SiteLanguage;
                    if (firstParameter)
                    {
                        languageString = "?" + languageString;
                        firstParameter = false;
                    }
                    else
                    {
                        languageString = "&" + languageString;
                    }
                }
                if (cItem.UseSessionId)
                {
                    idSessionString = "idSession=" + _webSession.IdSession;
                    if (firstParameter)
                    {
                        idSessionString = "?" + idSessionString;
                        firstParameter = false;
                    }
                    else
                    {
                        idSessionString = "&" + idSessionString;
                    }
                }
                href = cItem.Url + languageString + idSessionString;
                #endregion

                if (cItem.DisplayInPopUp)
                {
                    if (cItem.JavascriptFunctionName.Length > 0 && cItem.WidthPopUp.Length > 0 && cItem.HeightPopUp.Length > 0)
                    {
                        js += "\r\n\t\t" + menuObjectName
                            + ".addMenuItem(new menuItem(\""
                            + GestionWeb.GetWebWord(cItem.WebTextId, _webSession.SiteLanguage)
                            + "\", \"" + cItem.IconName + "Item\",\"javascript:" + cItem.JavascriptFunctionName + "('" + href + "','" + cItem.WidthPopUp + "','" + cItem.HeightPopUp + "','yes');\"));";
                        js += "\r\n\t\t" + menuObjectName + ".items."
                            + cItem.IconName + "Item.showIcon(\"" + cItem.IconName + "\", \"" + cItem.IconName + "\");";
                    }
                }
                else
                {
                    if (cItem.Target.Length > 0)
                        hrefTmp += href + "','_blank";
                    js += "\r\n\t\t" + menuObjectName
                        + ".addMenuItem(new menuItem(\""
                        + GestionWeb.GetWebWord(cItem.WebTextId, _webSession.SiteLanguage)
                        + "\", \"" + cItem.IconName + "Item\",\"code:window.open('" + hrefTmp + "')\"));";
                    js += "\r\n\t\t" + menuObjectName + ".items."
                        + cItem.IconName + "Item.showIcon(\"" + cItem.IconName + "\", \"" + cItem.IconName + "\");";
                }
            }
            return (js);
        }

        /// <summary>
        /// Séparateur
        /// </summary>
        /// <param name="menuObjectName">nom de l'objet menu</param>
        /// <returns>Code Javascript</returns>
        private string GetSeparator(string menuObjectName)
        {
            string js = string.Empty;
            js += "\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"-\"));";
            return (js);
        }

        /// <summary>
        /// Generate scripts for save item
        /// </summary>
        /// <param name="menuObjectName">Name of menu object</param>
        /// <returns>Javascript Code</returns>
        private string GetSaveItem(string menuObjectName)
        {
            return ("\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(1990, _webSession.SiteLanguage) + "\", \"saveMenuItem\",\"javascript:popupOpenBis('/Private/MyAdExpress/MySessionSavePopUp.aspx?idSession=" + _webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "&param=" + generateNumber() + "','470','270','no');\"));"
                + "\r\n\t\t" + menuObjectName + ".items.saveMenuItem.showIcon(\"saveMenuIcon\", \"saveMenuIcon\");");
        }

        /// <summary>
        /// Alert
        /// </summary>
        /// <param name="menuObjectName">nom de l'objet menu</param>
        /// <param name="pInfo"></param>
        /// <param name="isValid"></param>
        /// <returns>Code Javascript</returns>
        private string GetCreateAlertItem(string menuObjectName, ResultPageInformation pInfo, bool isValid)
        {
            if (isValid)
                return ("\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(2578, _webSession.SiteLanguage) + "\", \"alertMenuItem\",\"javascript:popupOpenBis('" + pInfo.CreateAlertUrl + "?idSession=" + _webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "&param=" + generateNumber() + "','470','450','no');\"));"
                   + "\r\n\t\t" + menuObjectName + ".items.alertMenuItem.showIcon(\"alertMenuIcon\", \"alertMenuIcon\");");
            else
                return ("\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(2578, _webSession.SiteLanguage) + "\", \"alertMenuItem\", \"\", false, \"jsdomenuitemdisabled\", \"jsdomenuitemdisabledover\"));"
                   + "\r\n\t\t" + menuObjectName + ".items.alertMenuItem.showIcon(\"alertMenuIcon\", \"alertMenuIcon\");");
        }

        /// <summary>
        /// Generate scripts for export menus
        /// </summary>
        /// <param name="menuObjectName">Name of menu object</param>
        /// <returns>Javascript Code</returns>
        /// <param name="pInfo">Information of the result page</param>
        private string GetExportItems(string menuObjectName, ResultPageInformation pInfo)
        {

            StringBuilder js = new StringBuilder();

            if (pInfo.CanDisplayPrintExcelPage())
            {
                js.Append(this.GetExportSubMenu("printMenuItem", GestionWeb.GetWebWord(1996, _webSession.SiteLanguage), EXCEL_MENU, "javascript:OpenNewWindow('" + pInfo.PrintExcelUrl + "?idSession=" + this._webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "');", "printMenuIcon"));
            }

            if (pInfo.CanDisplayPrintBisExcelPage())
            {
                js.Append(this.GetExportSubMenu("printColItem", GestionWeb.GetWebWord(2013, _webSession.SiteLanguage), EXCEL_MENU, "javascript:OpenNewWindow('" + pInfo.PrintBisExcelUrl + "?idSession=" + this._webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "&type=2');", "printColMenuIcon"));
            }

            if (pInfo.CanDisplayValueExcelPage())
            {
                js.Append(this.GetExportSubMenu("excelUnitItem", GestionWeb.GetWebWord(1997, _webSession.SiteLanguage), EXCEL_MENU, "javascript:OpenNewWindow('" + pInfo.ValueExcelUrl + "?idSession=" + this._webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "');", "excelUnitMenuIcon"));
            }

            if (pInfo.CanDisplayRawExcelPage())
            {
                js.Append(this.GetExportSubMenu("excelExportItem", GestionWeb.GetWebWord(2014, _webSession.SiteLanguage), EXCEL_MENU, "javascript:OpenNewWindow('" + pInfo.RawExcelUrl + "?idSession=" + this._webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "');", "excelMenuIcon"));
            }

            if (pInfo.CanDisplayExportJpegPage() && this._webSession.Graphics)
            {//&& _jpegFormatFromWebPage
                js.Append(this.GetExportSubMenu("jpegExportItem", GestionWeb.GetWebWord(2016, _webSession.SiteLanguage), EXCEL_MENU, "javascript:popupOpenBis('" + pInfo.ExportJpegUrl + "?idSession=" + _webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "','950','800','yes');", "jpegExportMenuIcon"));
            }

            if (pInfo.CanDisplayRemotePdfPage())
            {
                switch (_module.Id)
                {
                    case WebCst.Module.Name.INDICATEUR:
                        js.Append(this.GetExportSubMenu("pdfExportItem", GestionWeb.GetWebWord(2015, _webSession.SiteLanguage), EXCEL_MENU, "javascript:popupOpenBis('" + pInfo.RemotePdfUrl + "?idSession=" + _webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "','470','210','yes');", "pdfExportMenuIcon"));
                        break;
                    case WebCst.Module.Name.BILAN_CAMPAGNE:
                        js.Append(this.GetExportSubMenu("pdfExportItem", GestionWeb.GetWebWord(2015, _webSession.SiteLanguage), EXCEL_MENU, "javascript:popupOpenBis('" + pInfo.RemotePdfUrl + "?idSession=" + _webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "','600','400','yes');", "pdfExportMenuIcon"));
                        break;
                }
            }

            if (pInfo.CanDisplayRemoteTextPage())
            {
                js.Append(this.GetExportSubMenu("textExportItem", GestionWeb.GetWebWord(_textExportWebtextId, _webSession.SiteLanguage), EXCEL_MENU, "javascript:popupOpenBis('" + pInfo.RemoteTextUrl + "?idSession=" + _webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "','470','210','yes');", "textExportMenuIcon"));
            }

            if (pInfo.CanDisplayRemoteExcelPage())
            {
                js.Append(this.GetExportSubMenu("excelExportItem", GestionWeb.GetWebWord(1923, _webSession.SiteLanguage), EXCEL_MENU, "javascript:popupOpenBis('" + pInfo.RemoteExceltUrl + "?idSession=" + _webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "','470','210','yes');", "excelExportMenuIcon"));
            }

            if (pInfo.CanDisplayRemoteResultPdfPage())
            {
                js.Append(this.GetExportSubMenu("pdfExportResultItem", GestionWeb.GetWebWord(2017, _webSession.SiteLanguage), EXCEL_MENU, "javascript:popupOpenBis('" + pInfo.RemoteResultPdfUrl + "?idSession=" + _webSession.IdSession + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "','470','210','yes');", "pdfExportMenuIcon"));
            }
            if (CanDisplayRemoteCreativeExportUrl(pInfo))
            {
                js.Append(this.GetExportSubMenu("creativeExportItem", GestionWeb.GetWebWord(2932, _webSession.SiteLanguage), EXCEL_MENU, "javascript:popupOpenBis('" + pInfo.RemoteCreativeExportUrl + "?idSession=" + _webSession.IdSession + "&resultType=" + Anubis.Constantes.Result.type.dedoum.GetHashCode() + ((_urlParameters.Length > 0) ? "&" + _urlParameters : "") + "','470','300','yes');", "exportCreativesMenuIcon"));

            }
            if (js.Length > 0)
            {
                js.Insert(0, this.GetExportMainMenu(menuObjectName));
            }

            return js.ToString();

        }

        /// <summary>
        /// Generate Scripts for main menu Export
        /// </summary>
        /// <param name="menuObjectName">Name of parent menu</param>
        /// <returns>Javascript Code</returns>
        private string GetExportMainMenu(string menuObjectName)
        {

            string js = string.Empty;

            js += "\r\n\t\t" + menuObjectName + ".addMenuItem(new menuItem(\"" + GestionWeb.GetWebWord(1992, _webSession.SiteLanguage) + "\", \"exportItem\", \"\"));";
            js += "\r\n\t\t " + EXCEL_MENU + " = new jsDOMenu(220);";
            js += "\r\n\t\t" + menuObjectName + ".items.exportItem.setSubMenu(" + EXCEL_MENU + ")";

            return js;
        }

        /// <summary>
        /// Generate scripts for an export sub menu item
        /// </summary>
        /// <param name="name">Name of the item</param>
        /// <param name="text">Text to display</param>
        /// <param name="parent">Parent menu</param>
        /// <param name="link">Item link</param>
        /// <param name="style">Item style</param>
        /// <returns>Javascript Code</returns>
        private string GetExportSubMenu(string name, string text, string parent, string link, string style)
        {
            return ("\r\n\t\t" + parent + ".addMenuItem(new menuItem(\"" + text + "\", \"" + name + "\",\"" + link + "\"));"
                + "\r\n\t\t" + parent + ".items." + name + ".showIcon(\"" + style + "\", \"" + style + "\");");
        }

        /// <summary>
        /// Generate an aleatory number
        /// </summary>
        /// <returns>Number as a string</returns>
        private string generateNumber()
        {
            DateTime dt = DateTime.Now;
            return (dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString() + dt.Millisecond.ToString() + new Random().Next(1000));
        }

        /// <summary>
        /// Get if can display option to export creatives
        /// </summary>
        /// <param name="pInfo">Result Page Information</param>
        /// <returns></returns>
        protected  bool CanDisplayRemoteCreativeExportUrl(ResultPageInformation pInfo)
        {
            
            if   (pInfo.CanDisplayRemoteCreativeExportUrl() && _module.Id == WebCst.Module.Name.ANALYSE_CONCURENTIELLE && _webSession.CustomerLogin.CustormerFlagAccess(Flags.ID_EXPORT_INTERNET_EVALIANT_CREATIVE_FLAG))
            {
                VehicleInformation vehicleInformation = null;
                string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, Constantes.Customer.Right.type.vehicleAccess);
                if(!string.IsNullOrEmpty(vehicleSelection)) vehicleInformation = VehiclesInformation.Get(long.Parse(vehicleSelection));
                return (vehicleInformation != null && vehicleInformation.Id == Vehicles.names.adnettrack);
            }
            return false;
        }
        #endregion

    }
}
