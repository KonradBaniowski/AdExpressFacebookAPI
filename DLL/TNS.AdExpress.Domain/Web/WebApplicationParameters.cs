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
using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web.Core;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Constantes.Web;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.ModulesDescritpion;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web.Navigation;

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
        protected static Dictionary<Int64,WebTheme> _themes;
        /// <summary>
        /// List of Column Item Information
        /// </summary>
        protected static GenericColumnItemsInformation _genericColumnItemsInformation;
        /// <summary>
        /// List of Column Items by vehicle
        /// </summary>
        protected static GenericColumnsInformation _genericColumnsInformation;
        /// <summary>
        /// Insertion details description
        /// </summary>
        protected static InsertionDetails _insertionsDetails;
        /// <summary>
        /// Creatives details description
        /// </summary>
        protected static InsertionDetails _creativesDetails;
        /// <summary>
        /// Creatives details description for media schedule result and PDF export
        /// </summary>
        protected static MSCreativesDetails _msCreativesDetails;
        /// <summary>
        /// Columns displayed in a result of type "Media Detail" in the Portefolio module
        /// </summary>
        protected static PortofolioDetailMediaColumns _portofolioDetailMediaColumns;
        /// <summary>
        /// Inset option is allowed
        /// </summary>
        protected static bool _allowInsetOption = true;
		/// <summary>
		/// Collections of inset type
		/// </summary>
		protected static Dictionary<CustomerSessions.InsertType, long> _insetTypeCollection = null;
        /// <summary>
        /// Dundas Configuration
        /// </summary>
        protected static DundasConfiguration _dundas = null;
		/// <summary>
		/// Info news information
		/// </summary>
		protected static TNS.AdExpress.Domain.Results.InfoNews _infoNewsInformations = null;
		/// <summary>
		/// Collections of layers which can be called in all web site
		/// <example></example>
		/// </summary>
		protected static Dictionary<WebConstantes.Layers.Id, Domain.Layers.CoreLayer> _coreLayers = null;
		/// <summary>
		/// True if must use right defaut connection
		/// </summary>
		protected static bool _useRightDefaultConnection = false;
        /// <summary>
        /// Trends
        /// </summary>
        protected static Trends _trends = null;

        /// <summary>
        /// RightMenuLinks configuration
        /// </summary>
        protected static RightMenuLinks _RightMenuLinks = null;
        #endregion
        
        #region Contructeur
        /// <summary>
        /// Constructor
        /// </summary>
        static WebApplicationParameters() {
            //Initialization
            _configurationDirectoryRoot=AppDomain.CurrentDomain.BaseDirectory+CONFIGARION_DIRECTORY_NAME+@"\";

            _webSiteName=WebParamtersXL.LoadSiteName(new XmlReaderDataSource(_configurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME));
            _countryCode = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(_configurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME));
            _countryConfigurationDirectoryRoot = _configurationDirectoryRoot + _countryCode + @"\";
            
            // Initialisation des descriptions des éléments de niveaux de détail
            DetailLevelItemsInformation.Init(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + ConfigurationFile.GENERIC_DETAIL_LEVEL_ITEMS_CONFIGURATION_FILENAME));
            
            // Initialisation des descriptions des niveaux de détail
            DetailLevelsInformation.Init(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + ConfigurationFile.GENERIC_DETAIL_LEVEL_CONFIGURATION_FILENAME)); 				
            
            _defaultDataLanguage=WebLanguagesXL.LoadDefaultDataLanguage(new XmlReaderDataSource(_countryConfigurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
			_defaultLanguage = WebLanguagesXL.LoadDefaultLanguage(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
            _allowedLanguages=WebLanguagesXL.LoadLanguages(new XmlReaderDataSource(_countryConfigurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_FILENAME));
            _themes=WebThemesXL.LoadThemes(new XmlReaderDataSource(_countryConfigurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBTHEMES_CONFIGURATION_FILENAME));
            _dataBase=new DataBase(new XmlReaderDataSource(_countryConfigurationDirectoryRoot+TNS.AdExpress.Constantes.Web.ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));
            _genericColumnItemsInformation = new GenericColumnItemsInformation(GenericColumnItemsInformationXL.Load(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.GENERIC_COLUMNS_ITEMS_CONFIGURATION_FILENAME)));
            _genericColumnsInformation = new GenericColumnsInformation(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.GENERIC_COLUMNS_ITEMS_CONFIGURATION_FILENAME));
            _insertionsDetails = new InsertionDetails(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.MEDIA_PLANS_INSERTION_CONFIGURATION_COLUMNS_ITEMS_CONFIGURATION_FILENAME));
            _creativesDetails = new InsertionDetails(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.CREATIVES_CONFIGURATION_COLUMNS_ITEMS_CONFIGURATION_FILENAME));
            _msCreativesDetails = new MSCreativesDetails(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.MS_CREATIVES_CONFIGURATION_COLUMNS_ITEMS_CONFIGURATION_FILENAME));
            _portofolioDetailMediaColumns = new PortofolioDetailMediaColumns(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.PORTOFOLIO_DETAIL_MEDIA_CONFIGURATION_FILENAME));
            _dundas = new DundasConfiguration(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.DUNDAS_CONFIGURATION_FILENAME));
			_infoNewsInformations = new InfoNews(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.INFO_NEWS_FILENAME));
			_coreLayers = CoreLayersXL.Load(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.CORE_LAYERS_CONFIGURATION_FILENAME));
            _trends = new Trends(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.TRENDS_FILENAME));

            _RightMenuLinks = new RightMenuLinks(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.RIGHT_MENU_LINKS_FILENAME));
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
        public static Dictionary<Int64,WebTheme> Themes {
            get { return _themes; }
        }
        /// <summary>
        /// Get Database description
        /// </summary>
        public static DataBase DataBaseDescription{
            get { return _dataBase; }
        }
        /// <summary>
        /// Get List of Column Item Information
        /// </summary>
        public static GenericColumnItemsInformation GenericColumnItemsInformation
        {
            get { return _genericColumnItemsInformation; }
        }
        /// <summary>
        /// Get List of Column Items by vehicle
        /// </summary>
        public static GenericColumnsInformation GenericColumnsInformation
        {
            get { return _genericColumnsInformation; }
        }
        /// <summary>
        /// Insertion details description
        /// </summary>
        public static InsertionDetails InsertionsDetail
        {
            get { return _insertionsDetails; }
        }
        /// <summary>
        /// Creatives details description
        /// </summary>
        public static InsertionDetails CreativesDetail
        {
            get { return _creativesDetails; }
        }
        /// <summary>
        /// Creatives details description for media schedule result and PDF export
        /// </summary>
        public static MSCreativesDetails MsCreativesDetail {
            get { return _msCreativesDetails; }
        }
        /// <summary>
        /// Get Columns displayed in a result of type "Media Detail" in the Portefolio module
        /// </summary>
        public static PortofolioDetailMediaColumns PortofolioDetailMediaColumns
        {
            get { return _portofolioDetailMediaColumns; }
        }
        /// <summary>
        /// Inset option is allowed
        /// </summary>
        public static bool AllowInsetOption{
            get
            {
                return _allowInsetOption;
            }
            set
            {
                _allowInsetOption = value;
            }
        }
		/// <summary>
		/// Get inset type collection
		/// </summary>
		public static Dictionary<CustomerSessions.InsertType, long> InsetTypeCollection {
			get {
				return _insetTypeCollection;
			}
			set {
				_insetTypeCollection = value;
			}
		}
        /// <summary>
        /// Get Dundas Virtual Path File Temporary
        /// </summary>
        public static DundasConfiguration DundasConfiguration {
            get { return _dundas; }
        }

		/// <summary>
		/// Get Infos news items information
		/// </summary>
		public static TNS.AdExpress.Domain.Results.InfoNews InfoNewsInformations {
			get { return _infoNewsInformations; }
		}
		/// <summary>
		/// Get Collections of layers which can be called in all web site
		/// </summary>
		public static Dictionary<WebConstantes.Layers.Id, Domain.Layers.CoreLayer> CoreLayers {
			get { return _coreLayers; }
		}
		/// <summary>
		/// True if must use right defaut connection
		/// </summary>
		public static bool UseRightDefaultConnection {
			get {
				return _useRightDefaultConnection;
			}
			set {
				_useRightDefaultConnection = value;
			}
		}
        /// <summary>
        /// Get trends information
        /// </summary>
        public static Trends TrendsInformations
        {
            get { return _trends; }
        }

        /// <summary>
        /// Get RightMenuLinks information
        /// </summary>
        public static RightMenuLinks RightMenuLinksInformations
        {
            get { return _RightMenuLinks; }
        }
        #endregion

    }
}
