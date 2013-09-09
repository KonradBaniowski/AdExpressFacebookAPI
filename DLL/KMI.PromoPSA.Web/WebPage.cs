using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using KMI.PromoPSA.Web.Domain;

namespace KMI.PromoPSA.Web
{
    public class WebPage : System.Web.UI.Page {
  
        #region Variables
        /// <summary>
        /// Langue du site
        /// </summary>
        public int _siteLanguage = 33;
        /// <summary>
        /// Specify whereas to use themes or not
        /// </summary>
        public bool _useThemes = true;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public WebPage() {
            try {
                if (HttpContext.Current.Request.QueryString.Get("siteLanguage") != null && 
                    WebApplicationParameters.AllowedLanguages.ContainsKey(int.Parse(HttpContext.Current.Request.QueryString.Get("siteLanguage")))) {
                    _siteLanguage = int.Parse(HttpContext.Current.Request.QueryString.Get("siteLanguage"));
                }
                else {
                    _siteLanguage = WebApplicationParameters.DefaultLanguage;
                }
            }
            catch (System.Exception) {
                _siteLanguage = WebApplicationParameters.DefaultLanguage;
            }
        }
        #endregion

        #region Events

        #region OnPreInit
        /// <summary>
        /// OnPreInit event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreInit(EventArgs e) {
            // TODO Gestion des exceptions
            if (_useThemes)
                this.Theme = WebApplicationParameters.Themes[_siteLanguage].Name;
            this.Response.Charset = WebApplicationParameters.AllowedLanguages[_siteLanguage].Charset;
        }
        #endregion

        #region OnInit
        /// <summary>
        /// OnInit Event
        /// </summary>
        /// <param name="e">Parameter e</param>
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
        }
        #endregion

        #region redirect
        /// <summary>
        /// Redirect to error page
        /// </summary>
        /// <param name="errorId">Error Id</param>
        protected virtual void Redirect(int errorId) {
            Response.Redirect(Page.ResolveUrl("~/error.aspx?id=" + errorId));
        }
        #endregion

        #endregion
    }
}
