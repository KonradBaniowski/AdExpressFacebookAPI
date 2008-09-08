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
        }
        #endregion

        #region OnLoad
        /// <summary>
        /// On Load event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Translate.SetAllTextLanguage(this,_siteLanguage);

            if (!Page.ClientScript.IsClientScriptBlockRegistered("CookiesJScript")) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CookiesJScript", TNS.AdExpress.Web.Functions.Script.CookiesJScript());
            }

        }
        #endregion

        #endregion
    }
}
