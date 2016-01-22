#region Informations
// Author: G. Facon
// Creation Date: 21/02/2008
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Bastet.Web {
    /// <summary>
    /// Web site Theme configuration
    /// </summary>
    public class WebServiceRightConfiguration {

        #region Variables
        /// <summary>
        /// URL
        /// </summary>
        private string _url;
        /// <summary>
        /// TNSName
        /// </summary>
        private string _tnsName;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Theme id</param>
        /// <param name="name">Theme Name</param>
        /// <param name="siteLanguage">site Language</param>
        public WebServiceRightConfiguration(string url, string tnsName) {
            _url = url;
            _tnsName = tnsName;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get URL
        /// </summary>
        public string URL{
            get { return _url; }
        }
        /// <summary>
        /// Get TNSName
        /// </summary>
        public string TnsName {
            get { return _tnsName; }
        }
        #endregion
    }
}
