#region Information
//  Author : G. Facon, G. Ragneau
//  Creation  date: 29/02/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.DB.Constantes;

namespace TNS.AdExpress.DataBaseDescription {

    /// <summary>
    /// Customer Connection
    /// </summary>
    public class CustomerConnection {

        #region Variables
        /// <summary>
        /// Connection Id
        /// </summary>
        protected CustomerConnectionIds _id;
        /// <summary>
        /// DataSource
        /// </summary>
        protected string _dataSource;
        /// <summary>
        /// Connection Timeout
        /// </summary>
        protected int _connectionTimeOut=-1;
        /// <summary>
        /// Pooling
        /// </summary>
        protected bool _pooling=false;
        /// <summary>
        /// Pool size
        /// </summary>
        protected int _decrPoolSize=-1;
        /// <summary>
        /// Database type
        /// </summary>
        protected DataSource.Type _type;
        /// <summary>
        /// Max pool size
        /// </summary>
        protected int _maxPoolSize=-1;
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
        public string DataSource {
            set { _dataSource=value; }
        }
        /// <summary>
        /// Connection Timeout
        /// </summary>
        public int ConnectionTimeOut {
            set { _connectionTimeOut=value; }
        }
        /// <summary>
        /// Pooling
        /// </summary>
        public bool Pooling {
            set { _pooling=value; }
        }
        /// <summary>
        /// Pool size
        /// </summary>
        public int DecrPoolSize {
            set { _decrPoolSize=value; }
        }
        /// <summary>
        /// Database type
        /// </summary>
        public DataSource.Type Type {
            set { _type=value; }
        }
        /// <summary>
        /// Max pool size
        /// </summary>
        public int MaxPoolSize {
            set { _maxPoolSize=value; }
        }

        #endregion
    }
}
