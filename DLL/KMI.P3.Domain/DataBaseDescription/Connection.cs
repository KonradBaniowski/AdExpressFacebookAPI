using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.DB.Constantes;
using KMI.P3.Domain.Exceptions;
namespace KMI.P3.Domain.DataBaseDescription
{

    /// <summary>
    /// Customer Connection
    /// </summary>
    public class CustomerConnection
    {

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
        protected int _connectionTimeOut = -1;
        /// <summary>
        /// Pooling
        /// </summary>
        protected bool _pooling = false;
        /// <summary>
        /// Pool size
        /// </summary>
        protected int _decrPoolSize = -1;
        /// <summary>
        /// Database type
        /// </summary>
        protected DataSource.Type _type;
        /// <summary>
        /// Max pool size
        /// </summary>
        protected int _maxPoolSize = -1;
        /// <summary>
        /// NLS SORT to use any linguistic sort for an ORDER BY clause
        /// <example> France ="FRENCH"</example>
        /// </summary>
        protected string _nlsSort = "";
        /// <summary>
        /// Is encoding utf-8
        /// </summary>
        protected bool _isUTF8 = false;

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
        public CustomerConnection(CustomerConnectionIds id)
        {
            _id = id;
        }
        #endregion

        #region Accessors

        /// <summary>
        /// DataSource
        /// </summary>
        public string DataSource
        {
            set { _dataSource = value; }
        }
        /// <summary>
        /// Connection Timeout
        /// </summary>
        public int ConnectionTimeOut
        {
            set { _connectionTimeOut = value; }
        }
        /// <summary>
        /// Pooling
        /// </summary>
        public bool Pooling
        {
            set { _pooling = value; }
        }
        /// <summary>
        /// Pool size
        /// </summary>
        public int DecrPoolSize
        {
            set { _decrPoolSize = value; }
        }
        /// <summary>
        /// Database type
        /// </summary>
        public DataSource.Type Type
        {
            set { _type = value; }
        }
        /// <summary>
        /// Max pool size
        /// </summary>
        public int MaxPoolSize
        {
            set { _maxPoolSize = value; }
        }
        /// <summary>
        /// Set NLS SORT to use any linguistic sort for an ORDER BY clause
        /// </summary>
        public string NlsSort
        {
            set { _nlsSort = value; }
        }
        /// <summary>
        /// Is  utf-8 encoding
        /// </summary>
        public bool IsUTF8
        {
            set { _isUTF8 = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get IDataSource
        /// </summary>
        public IDataSource GetDataSource(string login, string password)
        {
            try
            {
                SourceFactory sourceFactory = new SourceFactory(_type, login, password, _dataSource);
                sourceFactory.ConnectionTimeOut = _connectionTimeOut;
                sourceFactory.DecrPoolSize = _decrPoolSize;
                sourceFactory.MaxPoolSize = _maxPoolSize;
                sourceFactory.Pooling = _pooling;
                return (sourceFactory.GetIDataSource());
            }
            catch (System.Exception err)
            {
                throw (new DefaultConnectionException("Impossible to retreive default connection", err));
            }
        }
        /// <summary>
        /// Get IDataSource
        /// </summary>
        public IDataSource GetDataSource(string login, string password, string nlsSort)
        {
            try
            {
                SourceFactory sourceFactory = new SourceFactory(_type, login, password, _dataSource);
                sourceFactory.ConnectionTimeOut = _connectionTimeOut;
                sourceFactory.DecrPoolSize = _decrPoolSize;
                sourceFactory.MaxPoolSize = _maxPoolSize;
                sourceFactory.Pooling = _pooling;
                sourceFactory.IsUTF8 = _isUTF8;
                sourceFactory.NlsSort = nlsSort;
                return (sourceFactory.GetIDataSource());
            }
            catch (System.Exception err)
            {
                throw (new DefaultConnectionException("Impossible to retreive default connection", err));
            }
        }
        #endregion
    }
}
