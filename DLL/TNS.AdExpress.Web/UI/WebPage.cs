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
                    // TODO Put Default siteLanguage
                }
            }
            catch(System.Exception) { // TODO Put Default siteLanguage }
        }
        #endregion
    }
}
