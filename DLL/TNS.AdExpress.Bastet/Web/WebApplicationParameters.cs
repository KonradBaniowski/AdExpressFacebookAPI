#region Informations
// Author: G. Facon
// Creation date: 21/02/2008 
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Bastet.XmlLoader;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.Exceptions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Bastet.Web {

    /// <summary>
    /// Web Application Parameters
    /// </summary>
    public class WebApplicationParameters {

        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME="Configuration";
        #endregion

        #region variables
        /// <summary>
        /// Web site name
        /// </summary>
        protected static string _webSiteName;
        /// <summary>
        /// Configuration directory root
        /// </summary>
        private static string _configurationDirectoryRoot;
        /// <summary>
        /// Configuration directory root of the country
        /// </summary>
        private static string _countryConfigurationDirectoryRoot;
        /// <summary>
        /// Configuration country code
        /// </summary>
        private static string _countryCode;
        /// <summary>
        /// Database description
        /// </summary>
        private static DataBase _dataBase;        	
        /// <summary>
        /// Default Language
        /// </summary>
        protected static int _defaultLanguage;
		/// <summary>
		/// Default data language
		/// </summary>
		protected static int _defaultDataLanguage;
        /// <summary>
        /// Allowed languages List 
        /// </summary>
        protected static Dictionary<Int64,WebLanguage> _allowedLanguages;
        /// <summary>
        /// Themes List 
        /// </summary>
        /// <remarks>The key is the site language</remarks>
        protected static Dictionary<Int64, WebTheme> _themes;
        /// <summary>
        /// Collections of layers which can be called in all web site
        /// <example></example>
        /// </summary>
        protected static Dictionary<TNS.AdExpress.Constantes.Web.Layers.Id, Domain.Layers.CoreLayer> _coreLayers = null;
        /// <summary>
        /// WebService Right Configuration
        /// </summary>
        protected static WebServiceRightConfiguration _webServiceRightConfiguration = null;
        /// <summary>
        /// Group Contact Filter Configuration
        /// </summary>
        protected static List<Int64> _groupContactFilterList = null;
        #endregion
        
        #region Contructeur
        /// <summary>
        /// Constructor
        /// </summary>
        static WebApplicationParameters() {
            try {
                //Initialization
                _configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
                _webSiteName = WebParamtersXL.LoadSiteName(new XmlReaderDataSource(_configurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME));
                _countryCode = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(_configurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME));
                _countryConfigurationDirectoryRoot = _configurationDirectoryRoot + _countryCode + @"\";
                _defaultDataLanguage = WebLanguagesXL.LoadDefaultDataLanguage(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
                _defaultLanguage = WebLanguagesXL.LoadDefaultLanguage(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
                _allowedLanguages = WebLanguagesXL.LoadLanguages(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
                _themes = WebThemesXL.LoadThemes(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBTHEMES_CONFIGURATION_FILENAME));
                _dataBase = new DataBase(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));
                _coreLayers = CoreLayersXL.Load(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.CORE_LAYERS_CONFIGURATION_FILENAME));
                _webServiceRightConfiguration = WebServiceRightConfigurationXL.Load(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEB_SERVICE_RIGHT_CONFIGURATION));
                _groupContactFilterList = GroupContactFilterXL.Load(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Bastet.Constantes.Web.ConfigurationFile.GROUP_CONTACT_FILTER_CONFIGURATION_FILENAME));

                // Initialisation des descriptions des éléments de niveaux de détail
                DetailLevelItemsInformation.Init(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + ConfigurationFile.GENERIC_DETAIL_LEVEL_ITEMS_CONFIGURATION_FILENAME));

                // Initialisation des descriptions des niveaux de détail
                DetailLevelsInformation.Init(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + ConfigurationFile.GENERIC_DETAIL_LEVEL_CONFIGURATION_FILENAME)); 		
            }
            catch (Exception e) {
                throw new WebApplicationInitialisationException("Initialization Error in WebApplicationParameters constructor", e);
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Web site name
        /// </summary>
        public static string WebSiteName {
            get { return _webSiteName; }
        }
        /// <summary>
        /// Get country code
        /// </summary>
        public static string CountryCode
        {
            get { return (_countryCode); }
        }
        /// <summary>
        /// Get configuration directory root
        /// </summary>
        public static string ConfigurationDirectoryRoot {
            get { return _configurationDirectoryRoot; }
        }
        /// <summary>
        /// Get country configuration directory root
        /// </summary>
        public static string CountryConfigurationDirectoryRoot {
            get { return _countryConfigurationDirectoryRoot; }
        }
        
        /// <summary>
        /// Get Default Language
        /// </summary>
        public static int DefaultLanguage {
            get { return _defaultLanguage; }
        }
		/// <summary>
		/// Get Default data Language
		/// </summary>
		public static int DefaultDataLanguage {
			get { return _defaultDataLanguage; }
		}
        /// <summary>
        /// Get allowed languages List 
        /// </summary>
        public static Dictionary<Int64,WebLanguage> AllowedLanguages {
            get { return _allowedLanguages; }
        }
        /// <summary>
        /// Get themes List 
        /// </summary>
        public static Dictionary<Int64, WebTheme> Themes {
            get { return _themes; }
        }
        /// <summary>
        /// Get Database description
        /// </summary>
        public static DataBase DataBaseDescription{
            get { return _dataBase; }
        }
        /// <summary>
        /// Get Collections of layers which can be called in all web site
        /// </summary>
        public static Dictionary<TNS.AdExpress.Constantes.Web.Layers.Id, Domain.Layers.CoreLayer> CoreLayers {
            get { return _coreLayers; }
        }
        /// <summary>
        /// Get WebService Right Configuration
        /// </summary>
        public static WebServiceRightConfiguration WebServiceRightConfiguration {
            get { return _webServiceRightConfiguration; }
        }

        /// <summary>
        /// Get Group contact Filter Configuration
        /// </summary>
        public static List<Int64> GroupContactFilterList {
            get { return _groupContactFilterList; }
        }
        #endregion

    }
}
