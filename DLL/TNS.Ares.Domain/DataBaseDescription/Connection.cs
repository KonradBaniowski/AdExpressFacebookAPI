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
using TNS.Ares.Domain.Exceptions;

namespace TNS.Ares.Domain.DataBaseDescription {

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
		/// <summary>
		/// NLS SORT to use any linguistic sort for an ORDER BY clause
		/// <example> France ="FRENCH"</example>
		/// </summary>
		protected string _nlsSort = "";
		/// <summary>
		/// Is encoding utf-8
		/// </summary>
		protected bool _isUTF8 = false;

		#region SQl server keyword values within the ConnectionString
		/// <summary>
		/// The SQL Server Language record name. 
		/// </summary>
		protected string _sqlServerCurrentLanguage = "";
		/// <summary>
		/// true indicates that the SQL Server connection pooler automatically enlists the connection in the creation thread's current transaction context.
		/// </summary>
		protected bool _enliste = false;
		/// <summary>
		/// When false, User ID and Password are specified in the connection. When true, the current Windows account credentials are used for authentication.
		/// </summary>
		protected bool _integratedSecurity = false;
		/// <summary>
		/// When set to false or no (strongly recommended), security-sensitive information, such as the password, 
		/// is not returned as part of the connection if the connection is open or has ever been in an open state. 
		/// Resetting the connection string resets all connection string values including the password. Recognized values are true, false, yes, and no. 
		/// </summary>
		protected bool _persistSecurityInfo = false;
		/// <summary>
		/// true if replication is supported using the connection. 
		/// </summary>
		protected bool _replication = false;
		/// <summary>
		/// The name of the workstation connecting to SQL Server.
		/// </summary>
		protected string _workstationID = "";
		/// <summary>
		/// A string value that indicates the type system the application expects. Possible values are:
		///Type System Version=SQL Server 2000; 
		///Type System Version=SQL Server 2005; 
		///Type System Version=Latest; 
		///When set to SQL Server 2000, the SQL Server 2000 type system is used. The following conversions are performed when connecting to a SQL Server 2005 instance:
		///XML to NTEXT
		///UDT to VARBINARY
		///VARCHAR(MAX), NVARCHAR(MAX) and VARBINARY(MAX) to TEXT, NEXT and IMAGE respectively.
		///When set to SQL Server 2005, the SQL Server 2005 type system is used. No conversions are made for the current version of ADO.NET.
		///When set to Latest, the latest version than this client-server pair can handle is used. This will automatically move forward as the client and server components are upgraded.
		/// </summary>
		protected string _typeSystemVersion = "";

		#endregion

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
		/// <summary>
		/// Set NLS SORT to use any linguistic sort for an ORDER BY clause
		/// </summary>
		public string NlsSort {
			set { _nlsSort = value; }
		}
		/// <summary>
		/// Is  utf-8 encoding
		/// </summary>
		public bool IsUTF8 {
			set { _isUTF8 = value; }
		}
		/// <summary>
		/// The SQL Server Language record name. 
		/// </summary>
		/// <remarks>Corresponds to property Current Language in SQL server</remarks>
		public string SQlServerCurrentLanguage {
			set { _sqlServerCurrentLanguage = value; }
		}
		/// <summary>
		/// Set true indicates that the SQL Server connection pooler automatically enlists the connection in the creation thread's current transaction context. 
		/// </summary>
		public bool Enlist {
			set { _enliste = value; }
		}
		/// <summary>
		/// When false, User ID and Password are specified in the connection. When true, the current Windows account credentials are used for authentication.
		/// </summary>
		public bool IntegratedSecurity {
			set { _integratedSecurity = value; }
		}
		/// <summary>
		/// When set to false or no (strongly recommended), security-sensitive information, such as the password, 
		/// is not returned as part of the connection if the connection is open or has ever been in an open state. 
		/// Resetting the connection string resets all connection string values including the password. Recognized values are true, false, yes, and no. 
		/// </summary>
		public bool PersistSecurityInfo {
			set { _persistSecurityInfo = value; }
		}
		/// <summary>
		/// true if replication is supported using the connection. 
		/// </summary>
		public bool Replication {
			set { _replication = value; }
		}
		/// <summary>
		/// The name of the workstation connecting to SQL Server.
		/// </summary>
		public string WorkstationID {
			set { _workstationID = value; }
		}
		/// <summary>
		/// A string value that indicates the type system the application expects. Possible values are:
		///Type System Version=SQL Server 2000; 
		///Type System Version=SQL Server 2005; 
		///Type System Version=Latest; 
		///When set to SQL Server 2000, the SQL Server 2000 type system is used. The following conversions are performed when connecting to a SQL Server 2005 instance:
		///XML to NTEXT
		///UDT to VARBINARY
		///VARCHAR(MAX), NVARCHAR(MAX) and VARBINARY(MAX) to TEXT, NEXT and IMAGE respectively.
		///When set to SQL Server 2005, the SQL Server 2005 type system is used. No conversions are made for the current version of ADO.NET.
		///When set to Latest, the latest version than this client-server pair can handle is used. This will automatically move forward as the client and server components are upgraded.
		/// </summary>
		public string TypeSystemVersion {
			set { _typeSystemVersion = value; }
		}
        #endregion

        #region Public Methods
        /// <summary>
        /// Get IDataSource
        /// </summary>
        public IDataSource GetDataSource(string login,string password) {
            try {
                SourceFactory sourceFactory=new SourceFactory(_type,login,password,_dataSource);
                sourceFactory.ConnectionTimeOut=_connectionTimeOut;
				sourceFactory.DecrPoolSize = _decrPoolSize;
                sourceFactory.MaxPoolSize=_maxPoolSize;
                sourceFactory.Pooling=_pooling;
				sourceFactory.IsUTF8 = _isUTF8;
				sourceFactory.NlsSort = _nlsSort;

				#region Set SQL server Properties
				sourceFactory.CurrentLanguage = _sqlServerCurrentLanguage;
				sourceFactory.Enlist = _enliste;
				sourceFactory.IntegratedSecurity = _integratedSecurity;
				sourceFactory.PersistSecurityInfo = _persistSecurityInfo;
				sourceFactory.Replication = _replication;
				sourceFactory.TypeSystemVersion = _typeSystemVersion;
				sourceFactory.WorkstationID = _workstationID;
				#endregion

				return (sourceFactory.GetIDataSource());
            }
            catch(System.Exception err) {
                throw (new DefaultConnectionException("Impossible to retreive default connection",err));
            }
        }
		/// <summary>
		/// Get IDataSource
		/// </summary>
		public IDataSource GetDataSource(string login, string password,string nlsSort){
			try {
				
				SourceFactory sourceFactory = new SourceFactory(_type, login, password, _dataSource);
				sourceFactory.ConnectionTimeOut = _connectionTimeOut;
				sourceFactory.DecrPoolSize = _decrPoolSize;
				sourceFactory.MaxPoolSize = _maxPoolSize;
				sourceFactory.Pooling = _pooling;
				sourceFactory.IsUTF8 = _isUTF8;
				sourceFactory.NlsSort = nlsSort;

				#region Set SQL server Properties
				sourceFactory.CurrentLanguage = _sqlServerCurrentLanguage;
				sourceFactory.Enlist = _enliste;
				sourceFactory.IntegratedSecurity = _integratedSecurity;
				sourceFactory.PersistSecurityInfo = _persistSecurityInfo;
				sourceFactory.Replication = _replication;
				sourceFactory.TypeSystemVersion = _typeSystemVersion;
				sourceFactory.WorkstationID = _workstationID;
				#endregion
				return (sourceFactory.GetIDataSource());
			}
			catch (System.Exception err) {
				throw (new DefaultConnectionException("Impossible to retreive default connection", err));
			}
		}
        #endregion
    }
}
