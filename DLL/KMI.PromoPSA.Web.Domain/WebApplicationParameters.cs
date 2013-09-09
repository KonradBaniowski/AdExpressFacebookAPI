using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.PromoPSA.Web.Domain.Configuration;
using KMI.PromoPSA.Web.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;

namespace KMI.PromoPSA.Web.Domain {
    /// <summary>
    /// Web Application Parameters
    /// </summary>
    public class WebApplicationParameters {

        #region Variables
        /// <summary>
        /// Web site name
        /// </summary>
        protected static string _webSiteName;
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
        /// Country Short Name
        /// </summary>
        private static string _countryShortName;
        /// <summary>
        /// Configuration Directory Root
        /// </summary>
        private static string _configurationDirectoryRoot;
        /// <summary>
        /// Country Configuration Directory Root
        /// </summary>
        private static string _countryConfigurationDirectoryRoot;
        #endregion

        #region Contructeur

        #region Contructeur

        /// <summary>
        /// Constructor
        /// </summary>
        static WebApplicationParameters() {
            //Initialization
            _configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + Constantes.Configuration.DIRECTORY_CONFIGURATION;
            _webSiteName = WebParamtersXL.LoadSiteName(new XmlReaderDataSource(_configurationDirectoryRoot + Constantes.Configuration.FILE_WEBPARAMETERS_CONFIGURATION));
            _countryShortName = WebParamtersXL.LoadCountryShortName(new XmlReaderDataSource(_configurationDirectoryRoot + Constantes.Configuration.FILE_WEBPARAMETERS_CONFIGURATION));
            _countryConfigurationDirectoryRoot = _configurationDirectoryRoot + WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(_configurationDirectoryRoot + Constantes.Configuration.FILE_WEBPARAMETERS_CONFIGURATION)) + @"\";
            _defaultDataLanguage = WebLanguagesXL.LoadDefaultDataLanguage(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Constantes.Configuration.FILE_WEBLANGUAGES_CONFIGURATION));
            _defaultLanguage = WebLanguagesXL.LoadDefaultLanguage(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Constantes.Configuration.FILE_WEBLANGUAGES_CONFIGURATION));
            _allowedLanguages = WebLanguagesXL.LoadLanguages(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Constantes.Configuration.FILE_WEBLANGUAGES_CONFIGURATION));
            _themes = WebThemesXL.LoadThemes(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Constantes.Configuration.FILE_WEBTHEMES_CONFIGURATION));
            PromoPSAWebServices.Init(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Constantes.Configuration.WEBSERVICE_CONFIGURATION));
            //StatusInformations.Load(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Cste.Configuration.STATUS_INFORMATION_CONFIGURATION));
            //MenuList.Init(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Cste.Configuration.MENU_CONFIGURATION));
            //OptionsNumber.Init(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Cste.Configuration.OPTION_ADVERT_CONFIGURATION));
            //Vehicles.Load(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Cste.Configuration.VEHICLES_CONFIGURATION));
            //MediaStatusInformations.Load(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Cste.Configuration.MEDIA_STATUS_INFORMATION_CONFIGURATION));
            //AjaxParameter.Init(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Cste.Configuration.AJAX_CONFIGURATION));
            //_rulesLayer = LayerWebServicesXL.Load(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + Cste.Configuration.WEBSERVICE_CONFIGURATION));
        }

        #endregion

        #endregion

        #region Accessors
        /// <summary>
        /// Get Web site name
        /// </summary>
        public static string WebSiteName {
            get { return _webSiteName; }
        }
        /// <summary>
        /// Get Web site name
        /// </summary>
        public static string CountryShortName {
            get { return _countryShortName; }
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
        public static Dictionary<Int64, WebLanguage> AllowedLanguages {
            get { return _allowedLanguages; }
        }

        /// <summary>
        /// Get themes List 
        /// </summary>
        public static Dictionary<Int64, WebTheme> Themes {
            get { return _themes; }
        }
        #endregion

    }
}
