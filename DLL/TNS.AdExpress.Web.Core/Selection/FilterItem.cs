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
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Item identifiant</param>
        /// <param name="label">Item label</param>
        public FilterItem(Int64 id, string label) {
            _id = id;
            _label = label;
        }
        #endregion

    }
}
