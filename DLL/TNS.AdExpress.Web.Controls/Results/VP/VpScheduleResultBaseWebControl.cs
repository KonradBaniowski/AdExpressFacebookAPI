using TNS.AdExpress.Web.Core.Sessions;
using System.ComponentModel;
using System.Web.UI;
using System.Text;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Exceptions;
using System;
namespace TNS.AdExpress.Web.Controls.Results.VP
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:VpScheduleResultBaseWebControl runat=server></{0}:VpScheduleResultBaseWebControl>")]
    public /*abstract*/ class VpScheduleResultBaseWebControl : System.Web.UI.WebControls.WebControl {

        #region Variables
        /// <summary>
        /// Timeout des scripts utilisés par AjaxPro
        /// </summary>
        protected int _ajaxProTimeOut = 60;

        /// <summary>
        /// Session client
        /// </summary>
        protected WebSession _webSession = null;
        /// <summary>
        /// Theme name
        /// </summary>
        protected string _themeName = string.Empty;
        #endregion

        #region Accesors

        #region AjaxProTimeOut
        /// <summary>
        /// Obtient ou définit le Timeout des scripts utilisés par AjaxPro
        /// </summary>
        [Bindable(true),
        Category("Ajax"),
        Description("Timeout des scripts utilisés par AjaxPro"),
        DefaultValue("60")]
        public int AjaxProTimeOut {
            get { return _ajaxProTimeOut; }
            set { _ajaxProTimeOut = value; }
        }
        #endregion

        #region WebSession
        /// <summary>
        /// Obtient ou définit la Sesion du client
        /// </summary>
        [Bindable(false)]
        public WebSession WebSession {
            get { return (_webSession); }
            set {
                _webSession = value;
            }
        }
        #endregion

        #endregion

        #region Javascript

        #region AjaxProTimeOutScript
        /// <summary>
        /// Génère le code JavaSript pour ajuster le time out d'AjaxPro
        /// </summary>
        /// <returns>Code JavaScript</returns>
        private string AjaxProTimeOutScript() {
            StringBuilder js = new StringBuilder(100);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
            js.Append("\r\nAjaxPro.timeoutPeriod=" + _ajaxProTimeOut.ToString() + "*1000;");
            js.Append("\r\n-->\r\n</SCRIPT>");
            return (js.ToString());
        }
        #endregion

        #region AjaxEventScript
        /// <summary>
        /// Evenement Ajax
        /// </summary>
        /// <returns></returns>
        protected string AjaxEventScript() {

            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n");
            js.Append("\r\nfunction get_" + this.ID + "(){");
            js.Append("\r\n\tvar oN=document.getElementById('res_" + this.ID + "');");
            js.Append("\r\n\toN.innerHTML='" + GetLoadingHTML() + "';");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".GetData('"+this._webSession.IdSession+"', resultParameters_" + this.ID + ",styleParameters_" + this.ID + ",get_" + this.ID + "_callback);");
            js.Append("\r\n}");

            js.Append("\r\nfunction get_" + this.ID + "_callback(res){");
            js.Append("\r\n\tvar oN=document.getElementById('res_" + this.ID + "');");
            js.Append("\r\n\toN.innerHTML=res.value;");
            // js.Append("\r\n\tUpdateParameters(oN);");
            js.Append("\r\n}\r\n");
            js.Append("\r\naddEvent(window, \"load\", get_" + this.ID + ");");

            js.Append("\r\n\r\n</SCRIPT>");
            return (js.ToString());
        }
        #endregion

        #endregion

        #region Evènements

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e) {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType(), this.Page);
            base.OnLoad(e);
        }
        #endregion

        #region PréRender
        /// <summary>
        /// Prérendu
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e) {

            base.OnPreRender(e);
            _themeName = this.Page.Theme;
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {
            StringBuilder html = new StringBuilder(1000);
            html.Append(AjaxProTimeOutScript());
            html.Append(ResultParametersScript());
            html.Append(StyleParametersScript());
            html.Append(AjaxEventScript());
            html.Append(GetLoadingHTML());
            output.Write(html.ToString());
        }
        #endregion

        #endregion

        #region Ajax Methods

        #region Enregistrement des paramètres de construction du résultat
        /// <summary>
        /// Génération du JavaScript pour les paramètres du résultat
        /// </summary>
        /// <returns>Script</returns>
        protected string ResultParametersScript() {
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
            js.Append("\r\n\t var resultParameters_"+this.ID+" = new Object();");
            js.Append(SetResultParametersScript());
            js.Append("\r\n\t SetResultParameters_" + this.ID + "(resultParameters_" + this.ID + ");");
            js.Append("\r\n-->\r\n</SCRIPT>");
            return (js.ToString());
        }

        /// <summary>
        /// Génération du JavaScript pour définir les paramètres de résultat
        /// </summary>
        /// <remarks>
        /// Pour surcharge lors de l'heritage, il faut générer une fonction JavaScript comme ci-dessous:
        /// <code>
        /// function SetResultParameters(obj){
        /// ...
        /// }
        /// </code>
        /// </remarks>
        /// <returns></returns>
        private string SetResultParametersScript() {
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n\tfunction SetResultParameters_" + this.ID + "(obj){");
            js.Append(SetCurrentResultParametersScript());
            js.Append("\r\n\t}");
            return (js.ToString());
        }
        protected virtual string SetCurrentResultParametersScript() {
            return (string.Empty);
        }
        #endregion

        #region Enregistrement des paramètres pour les styles
        /// <summary>
        /// Génération du JavaScript des paramètres pour les styles
        /// </summary>
        /// <returns>Script</returns>
        protected string StyleParametersScript() {
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
            js.Append("\r\n\t var styleParameters_" + this.ID + " = new Object();");
            js.Append(SetStyleParametersScript());
            js.Append("\r\n\t SetCssStyles_" + this.ID + "(styleParameters_" + this.ID + ");");
            js.Append("\r\n-->\r\n</SCRIPT>");
            return (js.ToString());
        }

        /// <summary>
        /// Génération du JavaScript pour définir les paramètres des styles
        /// </summary>
        /// <remarks>
        /// Pour surcharge lors de l'heritage, il faut générer une fonction JavaScript comme ci-dessous:
        /// <code>
        /// function SetCssStyles(obj){
        /// ...
        /// }
        /// </code>
        /// </remarks>
        /// <returns>Script</returns>
        private string SetStyleParametersScript() {
            StringBuilder js = new StringBuilder(3000);
            js.Append("\r\n\nfunction SetCssStyles_"+this.ID+"(obj){");
            js.Append("\r\n\t obj.Theme = '" + _themeName + "';");
            js.Append(SetCurrentStyleParametersScript());
            js.Append("\r\n }");
            return (js.ToString());
        }
        protected virtual string SetCurrentStyleParametersScript() {
            return (string.Empty);
        }
        #endregion

        #region Chargement des paramètres AjaxPro.JavaScriptObject et WebSession
        /// <summary>
        /// Charge les paramètres des résultats navigant entre le client et le serveur
        /// </summary>
        /// <param name="o">Tableau de paramètres javascript</param>
        protected void LoadResultParameters(AjaxPro.JavaScriptObject o) {
            if (o != null) {
                LoadCurrentResultParameters(o);
            }
        }
        protected virtual void LoadCurrentResultParameters(AjaxPro.JavaScriptObject o) {
        }
        /// <summary>
        /// Charge les paramètres des sytles navigant entre le client et le serveur
        /// </summary>
        /// <param name="o">Tableau de paramètres javascript</param>
        protected void LoadStyleParameters(AjaxPro.JavaScriptObject o) {
            if (o != null) {
                if (o.Contains("Theme")) {
                    _themeName = o["Theme"].Value;
                    _themeName = _themeName.Replace("\"", "");
                }
                LoadCurrentStyleParameters(o);
            }
        }
        protected virtual void LoadCurrentStyleParameters(AjaxPro.JavaScriptObject o) {
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get VP schedule HTML  code
        /// </summary>
        /// <param name="o">Result parameters (session Id, theme...)</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public virtual string GetData(string idSession, AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters) {
            return string.Empty;
        }
        #endregion

        #endregion

        #region Methods

        #region GetHTML
        /// <summary>
        /// Compute VP schedule
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <returns>Code HTMl</returns>
        //protected abstract string GetHTML();
        #endregion

        #region GetLoadingHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected string GetLoadingHTML() {
            return ("<div id=\"res_" + this.ID + "\"><div align=\"center\" width = \"100%\"><img src=\"/App_Themes/" + _themeName + "/Images/Common/waitAjax.gif\"></div></div>");
        }
        #endregion

        #region GetMessageError

        /// <summary>
        /// Message d'erreur
        /// </summary>
        /// <param name="customerSession">Session du client</param>
        /// <param name="code">Code message</param>
        /// <returns>Message d'erreur</returns>
        protected string GetMessageError(WebSession customerSession, int code) {
            string errorMessage = "<div align=\"center\" class=\"txtViolet11Bold\">";
            if (customerSession != null)
                errorMessage += GestionWeb.GetWebWord(code, customerSession.SiteLanguage) + ". " + GestionWeb.GetWebWord(2099, customerSession.SiteLanguage);
            else
                errorMessage += GestionWeb.GetWebWord(code, 33) + ". " + GestionWeb.GetWebWord(2099, 33);

            errorMessage += "</div>";
            return errorMessage;
        }
        #endregion

        #region OnAjaxMethodError
        /// <summary>
        /// Appelé sur erreur à l'exécution des méthodes Ajax
        /// </summary>
        /// <param name="errorException">Exception</param>
        /// <param name="customerSession">Session utilisateur</param>
        /// <returns>Message d'erreur</returns>
        protected string OnAjaxMethodError(Exception errorException, WebSession customerSession) {
            TNS.AdExpress.Web.Exceptions.CustomerWebException cwe = null;
            try {
                BaseException err = (BaseException)errorException;
                cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(err.Message, err.GetHtmlDetail(), customerSession);
            }
            catch (System.Exception) {
                try {
                    cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(errorException.Message, errorException.StackTrace, customerSession);
                }
                catch (System.Exception es) {
                    throw (es);
                }
            }
            cwe.SendMail();
            return GetMessageError(customerSession, 1973);
        }
        #endregion

        #endregion
    }
}

