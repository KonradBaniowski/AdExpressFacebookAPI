using System;
using System.Collections.Generic;
using System.Text;
using KMI.P3.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;
using KMI.P3.Domain.DataBaseDescription;

namespace KMI.P3.Domain.Web
{
    public class WebApplicationParameters
    {
        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion

        #region Variables
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
        protected static Dictionary<Int64, WebLanguage> _allowedLanguages;
        /// <summary>
        /// Themes List 
        /// </summary>
        /// <remarks>The key is the site language</remarks>
        protected static Dictionary<Int64, WebTheme> _themes;
        /// <summary>
        /// Configuration country code
        /// </summary>
        private static string _countryCode;
        /// <summary>
        /// Customer types
        /// </summary>
        private static Dictionary<Int64, CustomerTypesXL.CustomerType> _customerType;


        #endregion

        #region Contructor
        /// <summary>
        /// Constructor
        /// </summary>
        static WebApplicationParameters()
        {
        }
        #endregion

        #region Initialize
        public static void Initialize()
        {
            Initialize(null);
        }

        public static void Initialize(string pathDirectory)
        {
            //Initialization
            if (pathDirectory == null || pathDirectory.Length < 1)
            {
                _configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
            }
            else if (pathDirectory[pathDirectory.Length - 1] != '\\')
            {
                _configurationDirectoryRoot = pathDirectory + "\\";
            }
            else
            {
                _configurationDirectoryRoot = pathDirectory;
            }

            _webSiteName = WebParamtersXL.LoadSiteName(new XmlReaderDataSource(_configurationDirectoryRoot + KMI.P3.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME));
            _countryCode = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(_configurationDirectoryRoot + KMI.P3.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME));
            _countryConfigurationDirectoryRoot = _configurationDirectoryRoot + _countryCode + @"\";
            _defaultDataLanguage = WebLanguagesXL.LoadDefaultDataLanguage(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + KMI.P3.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
            _defaultLanguage = WebLanguagesXL.LoadDefaultLanguage(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + KMI.P3.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
            _allowedLanguages = WebLanguagesXL.LoadLanguages(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + KMI.P3.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
            _themes = WebThemesXL.LoadThemes(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + KMI.P3.Constantes.Web.ConfigurationFile.WEBTHEMES_CONFIGURATION_FILENAME));
            _dataBase = new DataBase(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + KMI.P3.Constantes.Web.ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));
            _customerType = CustomerTypesXL.LoadCustomerTypes(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + KMI.P3.Constantes.Web.ConfigurationFile.CUSTOMERTYPES_CONFIGURATION_FILENAME));
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Web site name
        /// </summary>
        public static string WebSiteName
        {
            get { return _webSiteName; }
        }
        /// <summary>
        /// Get configuration directory root
        /// </summary>
        public static string ConfigurationDirectoryRoot
        {
            get { return _configurationDirectoryRoot; }
        }
        /// <summary>
        /// Get country configuration directory root
        /// </summary>
        public static string CountryConfigurationDirectoryRoot
        {
            get { return _countryConfigurationDirectoryRoot; }
        }

        /// <summary>
        /// Get Default Language
        /// </summary>
        public static int DefaultLanguage
        {
            get { return _defaultLanguage; }
        }
        /// <summary>
        /// Get Default data Language
        /// </summary>
        public static int DefaultDataLanguage
        {
            get { return _defaultDataLanguage; }
        }
        /// <summary>
        /// Get allowed languages List 
        /// </summary>
        public static Dictionary<Int64, WebLanguage> AllowedLanguages
        {
            get { return _allowedLanguages; }
        }

        /// <summary>
        /// Get Customer Types
        /// </summary>
        public static Dictionary<Int64, CustomerTypesXL.CustomerType> CustomerTypeInfo
        {
            get { return _customerType; }
        }


        /// <summary>
        /// Get themes List 
        /// </summary>
        public static Dictionary<Int64, WebTheme> Themes
        {
            get { return _themes; }
        }

        /// <summary>
        /// Get Database description
        /// </summary>
        public static DataBase DataBaseDescription
        {
            get { return _dataBase; }
        }

        /// <summary>
        /// Get country code
        /// </summary>
        public static string CountryCode
        {
            get { return (_countryCode); }
        }

       
       

        #endregion

    }
}
