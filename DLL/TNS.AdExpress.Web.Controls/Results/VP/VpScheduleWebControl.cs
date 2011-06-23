#region Informations
// Auteur: G. Facon 
// Date de création: 13/07/2006
// Date de modification:
//		G Ragneau - 08/08/2006 - Set GetHtml as public so as to access it 
//		G Ragneau - 08/08/2006 - GetHTML : Force media plan alert module and restaure it after process (<== because of version zoom);
//		G Ragneau - 05/05/2008 - GetHTML : implement layers
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using AjaxPro;
using TNS.AdExpress.Web.Controls.Headers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using FrmFct = TNS.FrameWork.WebResultUI.Functions;
using TNS.FrameWork.Date;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.WebResultUI;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CustomCst = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.Insertions;
using TNS.AdExpressI.VP;
namespace TNS.AdExpress.Web.Controls.Results.VP
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:GenericMediaScheduleWebControl runat=server></{0}:GenericMediaScheduleWebControl>")]
    public class VpScheduleWebControl : System.Web.UI.WebControls.WebControl
    {

        #region Variables
        /// <summary>
        /// Timeout des scripts utilisés par AjaxPro
        /// </summary>
        protected int _ajaxProTimeOut = 60;

        /// <summary>
        /// Session client
        /// </summary>
        protected WebSession _customerWebSession = null;
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
        public int AjaxProTimeOut
        {
            get { return _ajaxProTimeOut; }
            set { _ajaxProTimeOut = value; }
        }
        #endregion

        #region CustomerWebSession
        /// <summary>
        /// Obtient ou définit la Sesion du client
        /// </summary>
        [Bindable(false)]
        public WebSession CustomerWebSession
        {
            get { return (_customerWebSession); }
            set
            {
                _customerWebSession = value;
            }
        }
        #endregion

        #region Theme name
        /// <summary>
        /// Set or Get the theme name
        /// </summary>
        public string ThemeName
        {
            get { return _themeName; }
            set { _themeName = value; }
        }
        #endregion

        #endregion

        #region Javascript

        #region AjaxProTimeOutScript
        /// <summary>
        /// Génère le code JavaSript pour ajuster le time out d'AjaxPro
        /// </summary>
        /// <returns>Code JavaScript</returns>
        private string AjaxProTimeOutScript()
        {
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
        protected string AjaxEventScript()
        {

            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n<SCRIPT language=javascript>\r\n");
            js.Append("\r\nfunction get_" + this.ID + "(){");
            js.Append("\r\n\tvar oN=document.getElementById('res_" + this.ID + "');");
            js.Append("\r\n\toN.innerHTML='" + GetLoadingHTML() + "';");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".GetData(o_vpSchedule,get_" + this.ID + "_callback);");
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
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript1", "/ajaxpro/prototype.ashx");
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript2", "/ajaxpro/core.ashx");
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript3", "/ajaxpro/converter.ashx");
            this.Page.ClientScript.RegisterClientScriptInclude("AjaxScript4", "/ajaxpro/TNS.AdExpress.Web.Controls.Results.VP.VpScheduleWebControl,TNS.AdExpress.Web.Controls.ashx");

            #region parameters
            StringBuilder js = new StringBuilder();
            js.Append("\r\n<SCRIPT language=javascript>\r\n");
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("IdSession", this.CustomerWebSession.IdSession);
            parameters.Add("themeName", _themeName);
            js.Append(FrmFct.Scripts.GetAjaxParametersScripts("vpSchedule", "o_vpSchedule", parameters));
            js.Append("\r\n</SCRIPT>\r\n");
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AjaxScript4", js.ToString());
            #endregion

            base.OnLoad(e);
        }
        #endregion

        #region PréRender
        /// <summary>
        /// Prérendu
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {

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

            base.Render(output);
            StringBuilder html = new StringBuilder(1000);
            html.Append(AjaxProTimeOutScript());
            html.Append(AjaxEventScript());
            html.Append(GetLoadingHTML());
            output.Write(html.ToString());
        }
        #endregion

        #endregion

        #region Ajax Methods

        #region GetData
        /// <summary>
        /// Get VP schedule HTML  code
        /// </summary>
        /// <param name="o">Result parameters (session Id, theme...)</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public string GetData(AjaxPro.JavaScriptObject o)
        {
            string html;
            try
            {

                LoadParams(o);

                html = GetHTML(this.CustomerWebSession);

            }
            catch (System.Exception err)
            {
                return (OnAjaxMethodError(err, this.CustomerWebSession));
            }
            return (html);
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
        protected virtual string GetHTML(WebSession webSession)
        {
            StringBuilder html = new StringBuilder(10000);          
            try
            {
                //TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
                TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(158);
                object[] param = param = new object[1];
                param[0] = _customerWebSession;
                IVeillePromo vpScheduleResult = (IVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryRulesLayer.AssemblyName, _module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                html.Append(vpScheduleResult.GetHtml());

                html.Append(" TEST ");
            }
            catch (System.Exception err)
            {
                return (OnAjaxMethodError(err, webSession));
            }
            return (html.ToString());
        }
        #endregion

        #region Load parameters AjaxPro.JavaScriptObject
        /// <summary>
        /// Charge les paramètres navigant entre le client et le serveur
        /// </summary>
        /// <param name="o">Tableau de paramètres javascript</param>
        protected void LoadParams(AjaxPro.JavaScriptObject o)
        {

            if (o != null)
            {

                if (o.Contains("IdSession"))
                {
                    this._customerWebSession = (WebSession)WebSession.Load(o["IdSession"].Value.Replace("\"", ""));
                }

                if (o.Contains("themeName"))
                {
                    _themeName = o["themeName"].Value.Replace("\"", "");
                }
            }
        }
        #endregion

        #region GetLoadingHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected string GetLoadingHTML()
        {
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
        protected string GetMessageError(WebSession customerSession, int code)
        {
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
        protected string OnAjaxMethodError(Exception errorException, WebSession customerSession)
        {
            TNS.AdExpress.Web.Exceptions.CustomerWebException cwe = null;
            try
            {
                BaseException err = (BaseException)errorException;
                cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(err.Message, err.GetHtmlDetail(), customerSession);
            }
            catch (System.Exception)
            {
                try
                {
                    cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(errorException.Message, errorException.StackTrace, customerSession);
                }
                catch (System.Exception es)
                {
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

