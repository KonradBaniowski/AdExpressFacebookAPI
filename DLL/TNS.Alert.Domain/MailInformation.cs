using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Alert.Domain {

    public class MailInformation {

        #region Variables
        /// <summary>
        /// Target Host (Use for Construct link to webSite in Mail)
        /// </summary>
        private string _targetHost = string.Empty;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="targetHost">Target Host (Use for Construct link to webSite in Mail)</param>
        public MailInformation(string targetHost) {
            this._targetHost = targetHost;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Target Host (Use for Construct link to webSite in Mail)
        /// </summary>
        public string TargetHost {
            get { return (this._targetHost); }
        }
        #endregion

        
    }
}
