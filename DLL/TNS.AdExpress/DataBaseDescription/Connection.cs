#region Information
//  Author : G. Facon, G. Ragneau
//  Creation  date: 29/02/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.DataBaseDescription {

    /// <summary>
    /// Customer Connection
    /// </summary>
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
        private IDataSource _type;
        /// <summary>
        /// Max pool size
        /// </summary>
        private int _maxPoolSize;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        protected CustomerConnection() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Connection Id</param>
        public CustomerConnection(CustomerConnectionIds id) {
            _id=id;
        } 
        #endregion

        #region Accessors

        /// <summary>
        /// DataSource
        /// </summary>
        private string DataSource {
            set { _dataSource=value; }
        }
        /// <summary>
        /// Connection Timeout
        /// </summary>
        private int ConnectionTimeOut {
            set { _connectionTimeOut=value; }
        }
        /// <summary>
        /// Pooling
        /// </summary>
        private bool Pooling {
            set { _pooling=value; }
        }
        /// <summary>
        /// Pool size
        /// </summary>
        private int DecrPoolSize {
            set { _decrPoolSize=value; }
        }
        /// <summary>
        /// Database type
        /// </summary>
        private IDataSource Type {
            set { _type=value; }
        }
        /// <summary>
        /// Max pool size
        /// </summary>
        private int MaxPoolSize {
            set { _maxPoolSize=value; }
        }

        #endregion
    }
}
