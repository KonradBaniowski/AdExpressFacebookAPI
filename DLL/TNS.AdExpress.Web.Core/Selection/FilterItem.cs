using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Web.Core.Selection {
    /// <summary>
    /// Filter item represent an object containing information concerning an element intended to be used as a filter
    /// the class definition is generic and it could be used for diffrent types of filters
    /// </summary>
    public class FilterItem {

        #region Variables
        /// <summary>
        /// Item identifiant
        /// </summary>
        private Int64 _id;
        /// <summary>
        /// Item label
        /// </summary>
        private string _label;
        /// <summary>
        /// Is Fiter Enable
        /// </summary>
        private bool _isEnable = true;
        #endregion

        #region Accessors
        /// <summary>
        /// Get Item identifiant
        /// </summary>
        public Int64 Id {
            get { return _id; }
        }
        /// <summary>
        /// Get Item label
        /// </summary>
        public string Label {
            get { return _label; }
        }
        /// <summary>
        /// Get / Set Is Fiter Enable
        /// </summary>
        public bool IsEnable {
            get { return _isEnable; }
            set { _isEnable = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Item identifiant</param>
        /// <param name="label">Item label</param>
        /// <param name="isEnable">Is Item Filter Enable</param>
        public FilterItem(Int64 id, string label, bool isEnable) {
            _id = id;
            _label = label;
            _isEnable = isEnable;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Item identifiant</param>
        /// <param name="label">Item label</param>
        public FilterItem(Int64 id, string label):this(id, label, true) {
        }
        #endregion

    }
}
