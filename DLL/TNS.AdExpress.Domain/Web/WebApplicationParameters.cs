#region Informations
// Author: G. Facon
// Creation date: 21/02/2008 
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web.Core;

namespace TNS.AdExpress.Domain.Web {

    /// <summary>
    /// Web Application Parameters
    /// </summary>
    public class WebApplicationParameters {

        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        private const string CONFIGARION_DIRECTORY_NAME="Configuration";
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
        protected static Dictionary<Int64,WebTheme> _themes;


        #endregion
        
        #region Contructeur
        /// <summary>
        /// Constructor
        /// </summary>
        static WebApplicationParameters() {
            //Initialization
            _configurationDirectoryRoot=AppDomain.CurrentDomain.BaseDirectory+CONFIGARION_DIRECTORY_NAME+@"\";
            _webSiteName=WebParamtersXL.LoadSiteName(new XmlReaderDataSource(_configurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME));
            _countryConfigurationDirectoryRoot=_configurationDirectoryRoot+WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(_configurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME))+@"\";
            _defaultDataLanguage=WebLanguagesXL.LoadDefaultDataLanguage(new XmlReaderDataSource(_countryConfigurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
			_defaultLanguage = WebLanguagesXL.LoadDefaultLanguage(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
            _allowedLanguages=WebLanguagesXL.LoadLanguages(new XmlReaderDataSource(_countryConfigurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
            _themes=WebThemesXL.LoadThemes(new XmlReaderDataSource(_countryConfigurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBTHEMES_CONFIGURATION_FILENAME));
            _dataBase=new DataBase(new XmlReaderDataSource(_countryConfigurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));
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
        public static Dictionary<Int64,WebTheme> Themes {
            get { return _themes; }
        }

        /// <summary>
        /// Get Database description
        /// </summary>
        public static DataBase DataBaseDescription{
            get { return _dataBase; }
        }
		
        #endregion

    }
}
