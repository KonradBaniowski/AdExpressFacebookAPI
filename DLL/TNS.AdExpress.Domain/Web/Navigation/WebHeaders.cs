using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.Web.Navigation {
    /// <summary>
    /// Headers List
    /// </summary>
    public class WebHeaders {

        #region Variables
        /// <summary>
        /// Headers list
        /// </summary>
        private static Dictionary<string,WebHeader> _headers;
        #endregion 

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        static WebHeaders() {
            _headers=HeaderXL.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot+ConfigurationFile.HEADER_CONFIGURATION_FILENAME));
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get headers list
        /// </summary>
        public static Dictionary<string,WebHeader> HeadersList {
            get{return(_headers);}
        }
        #endregion
    }
}
