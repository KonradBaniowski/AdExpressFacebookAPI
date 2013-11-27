using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.PSALoader.Domain {
    /// <summary>
    /// PSA Loader Configuration
    /// </summary>
    public class PSALoaderConfiguration {

        #region Variables
        /// <summary>
        /// Server Name
        /// </summary>
        private string _serverName;
        /// <summary>
        /// App pool
        /// </summary>
        private string _appPool;
        /// <summary>
        /// Login used to recycle the viewer pool
        /// </summary>
        private string _login;
        /// <summary>
        /// Password used to recycle the viewer pool
        /// </summary>
        private string _password;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverName">Server Name</param>
        /// <param name="appPool">App pool</param>
        /// <param name="login">Login used to recycle the viewer pool</param>
        /// <param name="password">Password used to recycle the viewer pool</param>
        public PSALoaderConfiguration(string serverName, string appPool, string login, string password) {
            if(serverName==null) throw (new ArgumentNullException("Parameter serverName is null"));
            if(appPool==null) throw (new ArgumentNullException("Parameter viewerAppPool is null"));
            if(login==null) throw (new ArgumentNullException("Parameter login is null"));
            if(password==null) throw (new ArgumentNullException("Parameter password is null"));
            if(serverName.Length<1) throw (new ArgumentException("Parameter serverName is invalid"));
            if(appPool.Length<1) throw (new ArgumentException("Parameter viewerAppPool is invalid"));
            if(login.Length<1) throw (new ArgumentException("Parameter login is invalid"));
            if(password.Length<1) throw (new ArgumentException("Parameter password is invalid"));
            _serverName=serverName;
            _appPool = appPool;
            _login=login;
            _password=password;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get ADLP viewer server name
        /// </summary>
        public string ServerName {
            get { return (_serverName); }
        }
        /// <summary>
        /// Get ADLP viewer app pool name
        /// </summary>
        public string AppPool {
            get { return (_appPool); }
        }
        /// <summary>
        /// Get login used to recycle the viewer pool
        /// </summary>
        public string Login {
            get { return (_login); }
        }
        /// <summary>
        /// Get password used to recycle the viewer pool
        /// </summary>
        public string Password {
            get { return (_password); }
        }
        #endregion

    }
}
