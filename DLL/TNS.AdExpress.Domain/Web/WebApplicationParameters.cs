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
using TNS.AdExpress.Domain.Classification;

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
        /// Use Comparative Media Schedule
        /// </summary>
        protected static bool _useComparativeMediaSchedule = false;
        /// <summary>
        /// Vehicles Format Information
        /// </summary>
        protected static VehiclesFormatInformation _vehiclesFormatInformation = new VehiclesFormatInformation(false, null);
        /// <summary>
        /// Use Retailer
        /// </summary>
        protected static bool _useRetailer = false;
        /// <summary>
        /// Use Retailer
        /// </summary>
        protected static Dictionary<TableIds, MatchingTable> _matchingRetailerTableList = new Dictionary<TableIds, MatchingTable>();
        /// <summary>
        /// Use comparative date In module Lost Won Module
        /// </summary>
        protected static bool _useComparativeLostWon = false;
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
        /// Number of year of data history
        /// </summary>
        protected static int _dataNumberOfYear = 0;

        /// <summary>
        /// RightMenuLinks configuration
        /// </summary>
        protected static RightMenuLinks _RightMenuLinks = null;
        /// <summary>
        /// Vp Configuration Detail
        /// </summary>
        protected static VpConfigurationDetail _vpConfigurationDetail = null;
        /// <summary>
        /// Vp Date Configuration List
        /// </summary>
        protected static VpDateConfigurations _vpDateConfigurations = null;
      
        /// <summary>
        /// Campaign type option is allowed
        /// </summary>
        protected static bool _allowCampaignTypeOption = false;
        /// <summary>
        /// Determine if keep universe selection each time user go back to refine selection page
        /// </summary>
        protected static bool _keepRefineUniverseSelection = false;
        /// <summary>
        /// Diponibility Option Period
        /// </summary>
        protected static bool _useDiponibilityOptionPeriodLostWon = true;
        /// <summary>
        /// Type Option Period 
        /// </summary>
        protected static bool _useTypeOptionPeriodLostWon = true;
        /// <summary>
        /// Is All Period Is Restrict To 4 Month In Insertion Report 
        /// </summary>
        protected static bool _isAllPeriodIsRestrictTo4MonthInInsertionReport = true;
        /// <summary>
        ///  Insertions reports options
        /// </summary>
        protected static TNS.AdExpress.Domain.Results.InsertionOptions _insertionOptions = null;
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
            _dataNumberOfYear = DataHistoryXL.LoadDataHistory(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.DATA_HISTORY_CONFIGURATION_FILENAME));

            _RightMenuLinks = new RightMenuLinks(new XmlReaderDataSource(_countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.RIGHT_MENU_LINKS_FILENAME));      
            _useRightDefaultConnection = RightOptionsXL.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.RIGHT_OPTIONS_CONFIGURATION_FILENAME));
           

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
        /// Get / Set Use Comparative Media Schedule
        /// </summary>
        public static bool UseComparativeMediaSchedule {
            get {
                return _useComparativeMediaSchedule;
            }
            set {
                _useComparativeMediaSchedule = value;
            }
        }
        /// <summary>
        /// Get / Set Vehicles Format Information
        /// </summary>
        public static VehiclesFormatInformation VehiclesFormatInformation {
            get {
                return _vehiclesFormatInformation;
            }
            set {
                _vehiclesFormatInformation = value;
            }
        }
        /// <summary>
        /// Get / Set Use Retailer
        /// </summary>
        public static bool UseRetailer {
            get {
                return _useRetailer;
            }
            set {
                _useRetailer = value;
            }
        }

        /// <summary>
        /// Set Use Retailer
        /// </summary>
        public static Dictionary<TableIds, MatchingTable> MatchingRetailerTableList {
            set {
                _matchingRetailerTableList = value;
            }
        }

        /// <summary>
        /// Get / Set Use comparative date In module Lost Won Module
		/// </summary>
        public static bool UseComparativeLostWon {
			get {
                return _useComparativeLostWon;
			}
			set {
                _useComparativeLostWon = value;
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
        /// Get Number of year of data history
        /// </summary>
        public static int DataNumberOfYear
        {
            get { return _dataNumberOfYear; }
        }

        /// <summary>
        /// Get RightMenuLinks information
        /// </summary>
        public static RightMenuLinks RightMenuLinksInformations
        {
            get { return _RightMenuLinks; }
        }

        /// <summary>
        /// Get / Set Vp Configuration Detail
        /// </summary>
        public static VpConfigurationDetail VpConfigurationDetail {
            get { return _vpConfigurationDetail; }
            set { _vpConfigurationDetail = value; }
        }
        
        /// <summary>
        /// Get / Set Vp Date Configurations
        /// </summary>
        public static VpDateConfigurations VpDateConfigurations {
            get { return _vpDateConfigurations; }
            set { _vpDateConfigurations = value; }
        }
        #endregion

        #region Get Table
        /// <summary>
        /// Get table label with schema label and prefix
        /// Schema.Table prefix
        /// </summary>
        /// <remarks>
        /// A space is put before the string
        /// </remarks>
        /// <example> adexpr03.data_press_4M wp</example>
        /// <param name="tableId">Table Id</param>
        /// <returns>SQL Table code</returns>
        public static string GetSqlDataTableLabelWithPrefix(TableIds tableId, bool isRetailerDisplay) {
            if(isRetailerDisplay && _matchingRetailerTableList!=null && _matchingRetailerTableList.ContainsKey(tableId))
                return _dataBase.GetSqlTableLabelWithPrefix(_matchingRetailerTableList[tableId].TableId);
            else 
                return _dataBase.GetSqlTableLabelWithPrefix(tableId);
        }

        /// <summary>
        /// Get table object
        /// </summary>
        /// <param name="tableId">Table Id</param>
        /// <returns>Table Object</returns>
        public static TNS.AdExpress.Domain.DataBaseDescription.Table GetDataTable(TableIds tableId, bool isRetailerDisplay) {
            if (isRetailerDisplay && _matchingRetailerTableList != null && _matchingRetailerTableList.ContainsKey(tableId))
                return _dataBase.GetTable(_matchingRetailerTableList[tableId].TableId);
            else
                return _dataBase.GetTable(tableId);
        }
      
        /// <summary>
        ///  Campaign Type option is allowed
        /// </summary>
        public static bool AllowCampaignTypeOption
        {
            get
            {
                return _allowCampaignTypeOption;
            }
            set
            {
                _allowCampaignTypeOption = value;
            }
        }
        /// <summary>
        /// Determine if keep universe selection each time user go back to refine selection page
        /// </summary>
        public static bool KeepRefineUniverseSelection
        {
            get
            {
                return _keepRefineUniverseSelection;
            }
            set
            {
                _keepRefineUniverseSelection = value;
            }
        }
        /// <summary>
        /// Get Diponibility Option Period
        /// </summary>
        public static bool UseDiponibilityOptionPeriodLostWon {
            get { return _useDiponibilityOptionPeriodLostWon; }
            set { _useDiponibilityOptionPeriodLostWon = value; }
        }
        /// <summary>
        /// Get Type Option Period 
        /// </summary>
        public static bool UseTypeOptionPeriodLostWon {
            get { return _useTypeOptionPeriodLostWon; }
            set { _useTypeOptionPeriodLostWon = value; }
        }
        ///// <summary>
        ///// Get / Set Is All Period Is Restrict To 4 Month In Insertion Report 
        ///// </summary>
        //public static bool IsAllPeriodIsRestrictTo4MonthInInsertionReport {
        //    get { return _isAllPeriodIsRestrictTo4MonthInInsertionReport; }
        //    set { _isAllPeriodIsRestrictTo4MonthInInsertionReport = value; }
        //}
        /// <summary>
        /// Get / Set Insertions reports options
        /// </summary>
        public static InsertionOptions InsertionOptions
        {
            get { return _insertionOptions; }
            set { _insertionOptions = value; }
        }
        #endregion

    }
}
