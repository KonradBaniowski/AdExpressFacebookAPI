using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.DataBaseDescription {
    public class CustomerConnection {
        #region Variables
        /// <summary>
        /// Connection Id
        /// </summary>
        private CustomerConnectionIds _id;
        /// <summary>
        /// DataSource
        /// </summary>
        private string _dataSource;
        /// <summary>
        /// Connection Timeout
        /// </summary>
        private int _connectionTimeOut;
        /// <summary>
        /// Pooling
        /// </summary>
        private bool _pooling;
        /// <summary>
        /// Pool size
        /// </summary>
        private int _decrPoolSize;

        /// <summary>
        /// Database type
        /// </summary>
        private DatabaseTypes _type;
        #endregion
        /// <summary>
        /// Max pool size
        /// </summary>
        private int _maxPoolSize;


        #region Accessors

        #endregion
    }
}
