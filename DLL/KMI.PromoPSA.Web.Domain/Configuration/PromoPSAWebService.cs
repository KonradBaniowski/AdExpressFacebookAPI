using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.Web.Domain.Configuration {

    #region WebService
    /// <summary>
    /// Valeurs des Services
    /// </summary>
    public class WebServices {
        /// <summary>
        /// Services
        /// </summary>
        public enum Names {
            /// <summary>
            /// Banner
            /// </summary>
            banner = 0,
            /// <summary>
            /// Dispacher
            /// </summary>
            dispacher = 1,
            /// <summary>
            /// Service
            /// </summary>
            service = 2,
            /// <summary>
            /// Isis rights
            /// </summary>
            rights = 3
        }
    }
    #endregion

    /// <summary>
    /// Promo PSA Web Services Configuration
    /// </summary>
    public class PromoPSAWebService {

        #region Variables
        /// <summary>
        /// Web Service Name
        /// </summary>
        protected WebServices.Names _webServiceName;
        /// <summary>
        /// Url
        /// </summary>
        protected string _url;
        /// <summary>
        /// Timeout
        /// </summary>
        protected int _timeout = 60;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webServiceName">Web Service Name</param>
        /// <param name="url">URL</param>
        /// <param name="timeout">Timeout</param>
        public PromoPSAWebService(WebServices.Names webServiceName, string url, int timeout) {
            if (url == null) throw (new ArgumentNullException("url parameter is null"));
            if (url.Length<=0) throw (new ArgumentException("url parameter is invalid"));
            _webServiceName = webServiceName;
            _url = url;
            _timeout = timeout;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get WebService Name
        /// </summary>
        public WebServices.Names WebServiceName {
            get { return (_webServiceName); }
        }
        /// <summary>
        /// get URL
        /// </summary>
        public string Url {
            get { return (_url); }
            set { _url = (value); }
        }
        /// <summary>
        /// get Timeout
        /// </summary>
        public int Timeout {
            get { return (_timeout); }
            set { _timeout = (value); }
        }
        #endregion

    }
}
