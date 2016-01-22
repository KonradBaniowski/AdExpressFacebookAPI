#region Information
//  Author : G. Facon, G. Ragneau
//  Creation  date: 29/02/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Domain.Exceptions;

namespace TNS.Ares.Domain.DataBaseDescription {
    /// <summary>
    /// Customer connection
    /// </summary>
    public class DefaultConnection:CustomerConnection {

        #region Variables
        /// <summary>
        /// Default Connection Id
        /// </summary>
        private new DefaultConnectionIds _id; 
        /// <summary>
        /// Database user name
        /// </summary>
        private string _login;
        /// <summary>
        /// Database password
        /// </summary>
        private string _password;
        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultConnection(DefaultConnectionIds id){
            _id=id;
        } 
        #endregion

        #region Accessors
        /// <summary>
        /// Set Login
        /// </summary>
        public string Login {
            set { _login=value; }
        }
        /// <summary>
        /// Set password
        /// </summary>
        public string Password {
            set { _password=value; }
        } 
        #endregion


        #region Public Methods
        /// <summary>
        /// Get IDataSource
        /// </summary>
        public IDataSource GetDataSource() {
            try{
                SourceFactory sourceFactory=new SourceFactory(_type,_login,_password,_dataSource);
                sourceFactory.ConnectionTimeOut=_connectionTimeOut;
                sourceFactory.DecrPoolSize=_decrPoolSize;
                sourceFactory.MaxPoolSize=_maxPoolSize;
                sourceFactory.Pooling=_pooling;			
                return(sourceFactory.GetIDataSource());
            }
            catch(System.Exception err){
                throw(new DefaultConnectionException("Impossible to retreive default connection",err));
            }
        }
		/// <summary>
		/// Get IDataSource
		/// </summary>
		public IDataSource GetDataSource(string nlsSort) {
			try {
				SourceFactory sourceFactory = new SourceFactory(_type, _login, _password, _dataSource);
				sourceFactory.ConnectionTimeOut = _connectionTimeOut;
				sourceFactory.DecrPoolSize = _decrPoolSize;
				sourceFactory.MaxPoolSize = _maxPoolSize;
				sourceFactory.Pooling = _pooling;
				sourceFactory.IsUTF8 = _isUTF8;
				sourceFactory.NlsSort = nlsSort;
				return (sourceFactory.GetIDataSource());
			}
			catch (System.Exception err) {
				throw (new DefaultConnectionException("Impossible to retreive default connection", err));
			}
		} 
        #endregion
    }
}
