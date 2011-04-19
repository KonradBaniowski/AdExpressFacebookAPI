using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using KMI.P3.Domain.XmlLoader;
using KMI.P3.Constantes.Web;

namespace KMI.P3.Domain.Web.Navigation
{
    /// <summary>
    /// Headers List
    /// </summary>
    public class WebHeaders
    {

        #region Variables
        /// <summary>
        /// Headers list
        /// </summary>
        private static Dictionary<string, WebHeader> _headers;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        static WebHeaders()
        {
            _headers = HeaderXL.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.HEADER_CONFIGURATION_FILENAME));
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get headers list
        /// </summary>
        public static Dictionary<string, WebHeader> HeadersList
        {
            get { return (_headers); }
        }
        #endregion
    }
}
