using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Alert.Domain {


    public class AlertInformation {

        #region Variables
        /// <summary>
        /// Use for Delete or not in Garbage
        /// </summary>
        private bool _delete = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="delete">Use for Delete or not in Garbage</param>
        public AlertInformation(bool delete) {
            this._delete = delete;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Use for Delete or not in Garbage
        /// </summary>
        public bool Delete {
            get { return (this._delete); }
        }
        #endregion

        
    }
}
