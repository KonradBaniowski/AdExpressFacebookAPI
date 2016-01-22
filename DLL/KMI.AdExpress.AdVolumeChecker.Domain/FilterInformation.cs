using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.AdVolumeChecker.Domain {
    /// <summary>
    /// Filter Information
    /// </summary>
    public class FilterInformation {

        #region Variables
        /// <summary>
        /// Filter field
        /// </summary>
        private string _field = string.Empty;
        /// <summary>
        /// Filter operator
        /// </summary>
        private string _operator = string.Empty;
        /// <summary>
        /// Filter ids
        /// </summary>
        private string _ids = string.Empty;
        #endregion

        #region Contructor
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="field">Filter field</param>
        /// <param name="operator_">Filter operator</param>
        /// <param name="ids">Filter ids</param>
        public FilterInformation(string field, string operator_, string ids) {
            _field = field;
            _operator = operator_;
            _ids = ids;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Filter Field
        /// </summary>
        public string Field {
            get { return (_field); }
        }
        /// <summary>
        /// Get Filter Operator
        /// </summary>
        public string Operator {
            get { return (_operator); }
        }
        /// <summary>
        /// Get Filter Ids
        /// </summary>
        public string Ids {
            get { return (_ids); }
        }
        #endregion


    }
}
