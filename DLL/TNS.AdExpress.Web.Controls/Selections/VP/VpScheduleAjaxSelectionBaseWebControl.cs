//using TNS.AdExpress.Web.Core.Sessions;
//using System.ComponentModel;
//using System.Web.UI;
//using System.Text;
//using System.Collections.Generic;
//using TNS.AdExpress.Domain.Translation;
//using TNS.FrameWork.Exceptions;
//using System;
//using System.IO;
//namespace TNS.AdExpress.Web.Controls.Selections.VP
//{
//    /// <summary>
//    /// Affiche le r�sultat d'une alerte plan media
//    /// </summary>
//    [DefaultProperty("Text"),
//      ToolboxData("<{0}:VpScheduleResultBaseWebControl runat=server></{0}:VpScheduleResultBaseWebControl>")]
//    public abstract class VpScheduleAjaxSelectionBaseWebControl : VpScheduleSelectionBaseWebControl {

//        #region Variables
//        /// <summary>
//        /// Timeout des scripts utilis�s par AjaxPro
//        /// </summary>
//        protected int _ajaxProTimeOut = 60;
//        /// <summary>
//        /// Theme name
//        /// </summary>
//        protected string _themeName = string.Empty;
//        #endregion

//        #region Accesors

//        #region AjaxProTimeOut
//        /// <summary>
//        /// Obtient ou d�finit le Timeout des scripts utilis�s par AjaxPro
//        /// </summary>
//        [Bindable(true),
//        Category("Ajax"),
//        Description("Timeout des scripts utilis�s par AjaxPro"),
//        DefaultValue("60")]
//        public int AjaxProTimeOut {
//            get { return _ajaxProTimeOut; }
//            set { _ajaxProTimeOut = value; }
//        }
//        #endregion

//        #region InitializeResultToLoad
//        /// <summary>
//        /// Obtient ou d�finit le Timeout des scripts utilis�s par AjaxPro
//        /// </summary>
//        public bool InitializeResultToLoad { get; set; }
//        #endregion

//        #endregion

//        #region Javascript

//        #region GetJavascript
//        /// <summary>
//        /// Get Validation Javascript Method
//        /// </summary>
//        /// <returns>Validation Javascript Method</returns>
//        protected override string GetValidationJavascriptContent() {
//            return "get_" + this.ID + "();";
//        }
//        #endregion

//        #region AjaxProTimeOutScript
//        /// <summary>
//        /// G�n�re le code JavaSript pour ajuster le time out d'AjaxPro
//        /// </summary>
//        /// <returns>Code JavaScript</returns>
//        private string AjaxProTimeOutScript() {
//            StringBuilder js = new StringBuilder(100);
//            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
//            js.Append("\r\nAjaxPro.timeoutPeriod=" + _ajaxProTimeOut.ToString() + "*1000;");
//            js.Append("\r\n-->\r\n</SCRIPT>");
//            return (js.ToString());
//        }
//        #endregion

//        #region AjaxEventScript
//        /// <summary>
//        /// Evenement Ajax
//        /// </summary>
//        /// <returns></returns>
//        private string AjaxEventScript() {

//            StringBuilder js = new StringBuilder(1000);
//            js.Append("\r\n<script language=\"javascript\">\r\n");
//            js.Append("\r\nfunction get_" + this.ID + "(){");
//            js.Append("\r\n\tvar oN=document.getElementById('" + this.ID + "');");
//            js.Append("\r\n\toN.innerHTML='" + GetAjaxInitialisationHTML().Replace("'", "\\'") + "';");
//            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".GetData('" + this._webSession.IdSession + "', resultParameters_" + this.ID + ",styleParameters_" + this.ID + ",get_" + this.ID + "_callback);");
//            js.Append("\r\n}");

//            js.Append("\r\nfunction get_" + this.ID + "_callback(res){");
//            js.Append("\r\n\tvar oN=document.getElementById('" + this.ID + "');");
//            js.Append("\r\n\toN.innerHTML=res.value;");
//            // js.Append("\r\n\tUpdateParameters(oN);");
//            js.Append("\r\n}\r\n");

//            if (InitializeResultToLoad) {
//                js.Append("\r\nif(window.addEventListener)");
//                js.Append("\r\n\twindow.addEventListener(\"load\", get_" + this.ID + ", false);");
//                js.Append("\r\nelse if(window.attachEvent)");
//                js.Append("\r\n\twindow.attachEvent(\"onload\",get_" + this.ID + "); ");
//            }

//            js.Append(GetAjaxEventScript());

//            js.Append("\r\n\r\n</script>");
//            return (js.ToString());
//        }

//         /// <summary>
//        /// Get Evenement Ajax
//        /// </summary>
//        /// <returns>Evenement Ajax</returns>
//        protected virtual string GetAjaxEventScript() {
//            return string.Empty;
//        }
//        #endregion

//        #endregion

//        #region Ev�nements

//        #region Initialisation
//        /// <summary>
//        /// Initialisation
//        /// </summary>
//        /// <param name="e">Arguments</param>
//        protected override void OnInit(EventArgs e) {
//            base.OnInit(e);
//            InitializeResultToLoad = true;
//        }
//        #endregion

//        #region Load
//        /// <summary>
//        /// Chargement du composant
//        /// </summary>
//        /// <param name="e">Arguments</param>
//        protected override void OnLoad(EventArgs e) {
//            /*AjaxPro.Utility.RegisterTypeForAjax(this.GetType(), this.Page);*/
//            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript1", "/ajaxpro/prototype.ashx");
//            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript2", "/ajaxpro/core.ashx");
//            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript3", "/ajaxpro/converter.ashx");
//            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript4" + this.ID, "/ajaxpro/" + this.GetType().Namespace + "." + this.GetType().Name + ",TNS.AdExpress.Web.Controls.ashx");

//            base.OnLoad(e);
//        }
//        #endregion

//        #region Pr�Render
//        /// <summary>
//        /// Pr�rendu
//        /// </summary>
//        /// <param name="e">Arguments</param>
//        protected override void OnPreRender(EventArgs e) {

//            base.OnPreRender(e);
//            _themeName = this.Page.Theme;
//        }
//        #endregion

//        #region Render
//        /// <summary> 
//        /// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
//        /// </summary>
//        /// <param name="output"> Le writer HTML vers lequel �crire </param>
//        protected override void Render(HtmlTextWriter output) {
//            StringBuilder html = new StringBuilder(1000);
//            html.Append(AjaxProTimeOutScript());
//            html.Append(ResultParametersScript());
//            html.Append(StyleParametersScript());
//            html.Append(AjaxEventScript());
//            output.Write(html.ToString());
//            base.Render(output);
//        }
//        #endregion

//        #endregion

//        #region Ajax Methods

//        #region Enregistrement des param�tres de construction du r�sultat
//        /// <summary>
//        /// G�n�ration du JavaScript pour les param�tres du r�sultat
//        /// </summary>
//        /// <returns>Script</returns>
//        protected string ResultParametersScript() {
//            StringBuilder js = new StringBuilder(3000);
//            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
//            js.Append("\r\n\t var resultParameters_" + this.ID + " = new Object();");
//            js.Append(SetResultParametersScript());
//            js.Append("\r\n\t SetResultParameters_" + this.ID + "(resultParameters_" + this.ID + ");");
//            js.Append("\r\n-->\r\n</SCRIPT>");
//            return (js.ToString());
//        }

//        /// <summary>
//        /// G�n�ration du JavaScript pour d�finir les param�tres de r�sultat
//        /// </summary>
//        /// <remarks>
//        /// Pour surcharge lors de l'heritage, il faut g�n�rer une fonction JavaScript comme ci-dessous:
//        /// <code>
//        /// function SetResultParameters(obj){
//        /// ...
//        /// }
//        /// </code>
//        /// </remarks>
//        /// <returns></returns>
//        private string SetResultParametersScript() {
//            StringBuilder js = new StringBuilder(3000);
//            js.Append("\r\n\tfunction SetResultParameters_" + this.ID + "(obj){");
//            js.Append(SetCurrentResultParametersScript());
//            js.Append("\r\n\t}");
//            return (js.ToString());
//        }
//        protected virtual string SetCurrentResultParametersScript() {
//            return (string.Empty);
//        }
//        #endregion

//        #region Enregistrement des param�tres pour les styles
//        /// <summary>
//        /// G�n�ration du JavaScript des param�tres pour les styles
//        /// </summary>
//        /// <returns>Script</returns>
//        protected string StyleParametersScript() {
//            StringBuilder js = new StringBuilder(3000);
//            js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
//            js.Append("\r\n\t var styleParameters_" + this.ID + " = new Object();");
//            js.Append(SetStyleParametersScript());
//            js.Append("\r\n\t SetCssStyles_" + this.ID + "(styleParameters_" + this.ID + ");");
//            js.Append("\r\n-->\r\n</SCRIPT>");
//            return (js.ToString());
//        }

//        /// <summary>
//        /// G�n�ration du JavaScript pour d�finir les param�tres des styles
//        /// </summary>
//        /// <remarks>
//        /// Pour surcharge lors de l'heritage, il faut g�n�rer une fonction JavaScript comme ci-dessous:
//        /// <code>
//        /// function SetCssStyles(obj){
//        /// ...
//        /// }
//        /// </code>
//        /// </remarks>
//        /// <returns>Script</returns>
//        private string SetStyleParametersScript() {
//            StringBuilder js = new StringBuilder(3000);
//            js.Append("\r\n\nfunction SetCssStyles_" + this.ID + "(obj){");
//            js.Append("\r\n\t obj.Theme = '" + _themeName + "';");
//            js.Append("\r\n\t obj.ID = '" + this.ID + "';");
//            js.Append(SetCurrentStyleParametersScript());
//            js.Append("\r\n }");
//            return (js.ToString());
//        }
//        protected virtual string SetCurrentStyleParametersScript() {
//            return (string.Empty);
//        }
//        #endregion

//        #region Chargement des param�tres AjaxPro.JavaScriptObject et WebSession
//        /// <summary>
//        /// Charge les param�tres des r�sultats navigant entre le client et le serveur
//        /// </summary>
//        /// <param name="o">Tableau de param�tres javascript</param>
//        private void LoadResultParameters(AjaxPro.JavaScriptObject o) {
//            if (o != null) {
//                LoadCurrentResultParameters(o);
//            }
//        }
//        protected virtual void LoadCurrentResultParameters(AjaxPro.JavaScriptObject o) {
//        }
//        /// <summary>
//        /// Charge les param�tres des sytles navigant entre le client et le serveur
//        /// </summary>
//        /// <param name="o">Tableau de param�tres javascript</param>
//        private void LoadStyleParameters(AjaxPro.JavaScriptObject o) {
//            if (o != null) {
//                if (o.Contains("Theme")) {
//                    _themeName = o["Theme"].Value.Replace("\"", "");
//                }
//                if (o.Contains("ID")) {
//                    this.ID = o["ID"].Value.Replace("\"", "");
//                }
//                LoadCurrentStyleParameters(o);
//            }
//        }
//        protected virtual void LoadCurrentStyleParameters(AjaxPro.JavaScriptObject o) {
//        }
//        #endregion

//        #region GetData
//        /// <summary>
//        /// Get VP schedule HTML  code
//        /// </summary>
//        /// <param name="o">Result parameters (session Id, theme...)</param>
//        /// <returns>Code HTML</returns>
//        [AjaxPro.AjaxMethod]
//        public virtual string GetData(string idSession, AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters) {
//            string html;
//            try {
//                this.LoadResultParameters(resultParameters);
//                this.LoadStyleParameters(styleParameters);
//                _webSession = (WebSession)WebSession.Load(idSession);

//                html = GetAjaxHTML();

//            }
//            catch (System.Exception err) {
//                return (OnAjaxMethodError(err, _webSession));
//            }
//            return (html);
//        }
//        #endregion

//        #endregion

//        #region Methods

//        #region GetAjaxHTML
//        /// <summary>
//        /// Compute VP schedule
//        /// </summary>
//        /// <param name="webSession">Client Session</param>
//        /// <returns>Code HTMl</returns>
//        protected abstract string GetAjaxHTML();
//        #endregion

//        #region GetHTML
//        /// <summary>
//        /// Get  loading HTML  
//        /// </summary>
//        /// <returns></returns>
//        protected override string GetHTML() {
//            return GetAjaxInitialisationHTML();
//        }
//        #endregion

//        #region GetAjaxInitialisationHTML
//        /// <summary>
//        /// Get  loading HTML  
//        /// </summary>
//        /// <returns></returns>
//        protected virtual string GetAjaxInitialisationHTML() {
//            return ("<div align=\"center\" width = \"100%\"><img src=\"/App_Themes/" + _themeName + "/Images/Common/waitAjax.gif\"></div>");
//        }
//        #endregion

//        #region GetMessageError

//        /// <summary>
//        /// Message d'erreur
//        /// </summary>
//        /// <param name="customerSession">Session du client</param>
//        /// <param name="code">Code message</param>
//        /// <returns>Message d'erreur</returns>
//        protected string GetMessageError(WebSession customerSession, int code) {
//            string errorMessage = "<div align=\"center\" class=\"txtViolet11Bold\">";
//            if (customerSession != null)
//                errorMessage += GestionWeb.GetWebWord(code, customerSession.SiteLanguage) + ". " + GestionWeb.GetWebWord(2099, customerSession.SiteLanguage);
//            else
//                errorMessage += GestionWeb.GetWebWord(code, 33) + ". " + GestionWeb.GetWebWord(2099, 33);

//            errorMessage += "</div>";
//            return errorMessage;
//        }
//        #endregion

//        #region OnAjaxMethodError
//        /// <summary>
//        /// Appel� sur erreur � l'ex�cution des m�thodes Ajax
//        /// </summary>
//        /// <param name="errorException">Exception</param>
//        /// <param name="customerSession">Session utilisateur</param>
//        /// <returns>Message d'erreur</returns>
//        protected string OnAjaxMethodError(Exception errorException, WebSession customerSession) {
//            TNS.AdExpress.Web.Exceptions.CustomerWebException cwe = null;
//            try {
//                BaseException err = (BaseException)errorException;
//                cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(err.Message, err.GetHtmlDetail(), customerSession);
//            }
//            catch (System.Exception) {
//                try {
//                    cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(errorException.Message, errorException.StackTrace, customerSession);
//                }
//                catch (System.Exception es) {
//                    throw (es);
//                }
//            }
//            cwe.SendMail();
//            return GetMessageError(customerSession, 1973);
//        }
//        #endregion

//        #endregion
//    }
//}

