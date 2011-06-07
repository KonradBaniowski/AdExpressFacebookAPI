using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.DataBaseDescription {
    /// <summary>
    /// Matching Table
    /// </summary>
    public class MatchingTable {

        #region variables
        /// <summary>
        /// Default Table Id
        /// </summary>
        private TableIds _defaultTableId;
        /// <summary>
        /// Table Id
        /// </summary>
        private TableIds _tableId;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="defaultTableId">Default Table Id</param>
        /// <param name="tableId">Table Id</param>
        public MatchingTable(TableIds defaultTableId, TableIds tableId) {
            _defaultTableId = defaultTableId;
            _tableId = tableId;
        }
        #endregion

        #region Accessors

        /// <summary>
        /// Get Default Table Id
        /// </summary>
        public TableIds DefaultTableId {
            get { return (_defaultTableId); }
        }

        /// <summary>
        /// Get Table Id
        /// </summary>
        public TableIds TableId {
            get { return (_tableId); }
        }

        #endregion
    }
}
