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
                if(HttpContext.Current.Request.QueryString.Get("sitelanguage")!=null) {
                    _siteLanguage=int.Parse(HttpContext.Current.Request.QueryString.Get("sitelanguage"));
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

        #endregion
    }
}
