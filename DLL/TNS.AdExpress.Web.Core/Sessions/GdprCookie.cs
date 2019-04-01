using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNS.AdExpress.Web.Core.Sessions
{
    /// <summary>
    /// Cookie Settings Model
    /// </summary>
    public class GdprCookie
    {
        /// <summary>
        /// prefs
        /// </summary>
        public List<string> prefs { get; set; }

        /// <summary>
        /// site Language
        /// </summary>
        public int siteLanguage { get; set; }

        /// <summary>
        /// Stored In Database
        /// </summary>
        public bool storedInDb { get; set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        public string creationDate { get; set; }

        /// <summary>
        /// Expiration Date
        /// </summary>
        public string expDate { get; set; }

        /// <summary>
        /// GUID
        /// </summary>
        public string guid { get; set; }
    }
}
