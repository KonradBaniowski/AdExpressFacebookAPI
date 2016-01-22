using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.DB.BusinessFacade.Oracle;

namespace WebServiceSetAdScopeLogin.Parameters
{
    public class ServiceParams
    {
      
        /// Configuration directory root
        /// </summary>
        private static string _configurationDirectoryRoot;

        /// <summary>
        /// MAU01 data source
        /// </summary>
        private static IDataSource _source;

         

        
        #region Contructor
        /// <summary>
        /// Constructor
        /// </summary>
        static ServiceParams()
        {
        }
        #endregion

        #region Initialize
        public static void Initialize()
        {
            _configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + WebServiceSetAdScopeLogin.Constantes.ConfigFile.CONFIGURATION_DIRECTORY_NAME + @"\";
            _source = new OracleDataSource(DataBaseConfigurationBussinessFacade.GetOne(_configurationDirectoryRoot + WebServiceSetAdScopeLogin.Constantes.ConfigFile.CONFIGURATION_DATABASE_FILENAME).ConnectionString);
        }
        #endregion

        /// <summary>
        /// Get configuration directory root
        /// </summary>
        public static string ConfigurationDirectoryRoot
        {
            get { return _configurationDirectoryRoot; }
        }
        /// <summary>
        /// Get data source
        /// </summary>
        public static IDataSource Source
        {
            get {
                if (_source == null) _source = new OracleDataSource(DataBaseConfigurationBussinessFacade.GetOne(_configurationDirectoryRoot + WebServiceSetAdScopeLogin.Constantes.ConfigFile.CONFIGURATION_DATABASE_FILENAME).ConnectionString);
                return _source;
            }
        } 
    }
}