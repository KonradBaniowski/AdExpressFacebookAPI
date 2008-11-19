#region Informations
// Author: B.Masson
// Creation Date: 29/10/2008
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Web {
    /// <summary>
    /// Rss feed description
    /// </summary>
    public class Rss {

        #region Variables
        /// <summary>
        /// Display or not rss feed on the page
        /// </summary>
        private bool _display = false;
        /// <summary>
        /// HTTP link
        /// </summary>
        private string _link = string.Empty;
        /// <summary>
        /// File path
        /// </summary>
        private string _filePath = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rss() {
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get if display or not rss feed on the page
        /// </summary>
        public bool Display {
            get { return (_display); }
            set { _display = value; }
        }
        /// <summary>
        /// Get file link
        /// </summary>
        public string Link {
            get { return (_link); }
            set { _link = value; }
        }

        /// <summary>
        /// Get file path
        /// </summary>
        public string FilePath {
            get { return (_filePath); }
            set { _filePath = value; }
        }
        #endregion

    }
}
