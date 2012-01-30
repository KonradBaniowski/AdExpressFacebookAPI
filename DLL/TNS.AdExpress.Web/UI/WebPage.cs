#region Information
// Author : G Facon
// Creation Date: 20/02/2008
// Modifications: 
//		
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.UI {
    /// <summary>
    /// Base class for public and private AdExpress page
    /// </summary>
    public class WebPage: System.Web.UI.Page {

        #region Variables
        /// <summary>
        /// Langue du site
        /// </summary>
        public int _siteLanguage=33;
        /// <summary>
        /// Specify whereas to use themes or not
        /// </summary>
        public bool _useThemes = true;
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        public WebPage() {
            try {
                if(HttpContext.Current.Request.QueryString.Get("siteLanguage")!=null) {
                    _siteLanguage=int.Parse(HttpContext.Current.Request.QueryString.Get("siteLanguage"));
                }
                else {
                    _siteLanguage = WebApplicationParameters.DefaultLanguage;
                }
            }
            catch(System.Exception) {
                _siteLanguage = WebApplicationParameters.DefaultLanguage;
            }
        }
        #endregion

        #region Events

        #region On PreInit
        /// <summary>
        /// On preinit event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreInit(EventArgs e) {
            // TODO Gestion des exceptions
            if (_useThemes)
                this.Theme = WebApplicationParameters.Themes[_siteLanguage].Name;

            this.Response.Charset = WebApplicationParameters.AllowedLanguages[_siteLanguage].Charset;
            this.Response.ContentEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_siteLanguage].ContentEncoding);
        }
        #endregion

        #region OnLoad
        /// <summary>
        /// On Load event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Translate.SetAllTextLanguage(this, _siteLanguage);
            AddScritps();
        }
        /// <summary>
        /// Add javascripts
        /// </summary>
        protected virtual void AddScritps(){
         

            if (!Page.ClientScript.IsClientScriptBlockRegistered("CookiesJScript"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CookiesJScript", TNS.AdExpress.Web.Functions.Script.CookiesJScript());
            }
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "activateActiveX", ResolveClientUrl("~/scripts/activateActiveX.js"));

        }
        #endregion

        #region Gestion des erreurs
        /// <summary>
        /// Evènement d'erreur
        /// </summary>
        /// <param name="e">Argument</param>
        protected override void OnError(EventArgs e) {
            EventArgs errorArgs = e;
            if (e.GetType() != typeof(TNS.AdExpress.Web.UI.ErrorEventArgs)) {
                base.OnError(errorArgs);
                return;
            }
            if (e == EventArgs.Empty) {
                base.OnError(errorArgs);
                return;
            }
            TNS.AdExpress.Web.Exceptions.CustomerWebException cwe = null;
            try {
                BaseException err = ((BaseException)((ErrorEventArgs)e)[ErrorEventArgs.argsName.error]);
                cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException((System.Web.UI.Page)(((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.sender]), err.Message, err.GetHtmlDetail(), ((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.custormerSession]));
            }
            catch (System.Exception) {
                try {
                    cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException((System.Web.UI.Page)(((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.sender]), ((System.Exception)((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.error]).Message, ((System.Exception)((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.error]).StackTrace, ((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.custormerSession]));
                }
                catch (System.Exception es) {
                    throw (es);
                }
            }
            cwe.SendMail();
            manageCustomerError(cwe, errorArgs);

        }

        /// <summary>
        /// Traite l'affichage d'erreur en fonction du mode compilation
        /// </summary>
        protected virtual void manageCustomerError(object source, EventArgs errorArgs) {
#if DEBUG
            throw ((TNS.AdExpress.Web.Exceptions.CustomerWebException)source);
#else
				// Script
				if (!Page.ClientScript.IsClientScriptBlockRegistered("redirectError")){
                    TNS.AdExpress.Web.Core.Sessions.WebSession webSession = null;
                    try {
                        webSession = ((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)errorArgs)[ErrorEventArgs.argsName.custormerSession]);
                    }
                    catch { webSession = null; }
					//Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"redirectError",WebFunctions.Script.redirectError(((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)e)[ErrorEventArgs.argsName.custormerSession]).SiteLanguage.ToString()));	
                    if (webSession != null) {
                        Response.Write(TNS.AdExpress.Web.Functions.Script.RedirectError(webSession.SiteLanguage.ToString()));
                    }
                    else {
                        Response.Write(TNS.AdExpress.Web.Functions.Script.RedirectError(_siteLanguage.ToString()));
                    }
					Response.Flush();
					Response.End();
				}
				//this.Response.Redirect("/Public/Message.aspx?msgCode=5&siteLanguage="+((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)e)[ErrorEventArgs.argsName.custormerSession]).SiteLanguage);
#endif
        }
        #endregion

        #endregion
    }
}
