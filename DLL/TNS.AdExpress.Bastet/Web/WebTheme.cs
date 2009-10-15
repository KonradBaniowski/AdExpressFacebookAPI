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
    public class WebTheme {

        #region Variables
        /// <summary>
        /// Id
        /// </summary>
        private int _id;
        /// <summary>
        /// Theme name
        /// </summary>
        private string _name;
        /// <summary>
        /// Site language Id
        /// </summary>
        private int _siteLanguage;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Theme id</param>
        /// <param name="name">Theme Name</param>
        /// <param name="siteLanguage">site Language</param>
        public WebTheme(int id,string name,int siteLanguage) {
            if(id<0) throw (new ArgumentException("The theme Id cannot be inferior to 0"));
            _id=id;
            if(name==null) throw (new ArgumentNullException("The name parameter cannot be null"));
            if(name.Length==0) throw (new ArgumentException("Invalid name parameter"));
            _name=name;
            if(siteLanguage<0) throw (new ArgumentException("The site language Id cannot be inferior to 0"));
            _siteLanguage=siteLanguage;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get theme Id
        /// </summary>
        public int Id {
            get { return _id; }
        }
        /// <summary>
        /// Get theme name
        /// </summary>
        public string Name {
            get { return _name; }
        }
        /// <summary>
        /// Get site language Id
        /// </summary>
        public int SiteLanguage {
            get { return _siteLanguage; }
            set { _siteLanguage = value; }
        } 
        #endregion
    }
}
