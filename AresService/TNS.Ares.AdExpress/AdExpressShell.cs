using System;
using TNS.AdExpress.Web.Core;
using TNS.LinkSystem.LinkKernel;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.FrameWorks.LSConnectivity;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.Ares.Domain.DataBase;
using TNS.Ares.Domain.LS;
using TNS.Ares.Constantes;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.Ares.AdExpress.Exceptions;
using TNS.Ares.Domain.Mail;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.AdExpress.Domain.CampaignTypes;

namespace TNS.Ares.AdExpress
{
    public class AdExpressShell : Shell
    {
        #region Variables
        /// <summary>
        /// DataSource
        /// </summary>
        protected IDataSource _source;
        /// <summary>
        /// Path File Of Configuration
        /// </summary>
        protected string _confFile;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lsClientConfiguration">LS ClientConfiguration</param>
        /// <param name="directoryName">Directory Name</param>
        public AdExpressShell(LsClientConfiguration lsClientConfiguration, string directoryName) :
            base(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, lsClientConfiguration.FamilyName, directoryName, lsClientConfiguration.ModuleDescriptionList)
        {
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize
        /// </summary>
        protected override void InitializeShell(string pathConfiguration)
        {
            try
            {

                #region WebApplicationParameters
                try
                {
                    new WebApplicationParameters();
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load WebApplicationParameters", e);
                }
                #endregion

                //Initialisation des chemins d'accès aux créations
                CreativeConfigDataAccess.LoadPathes(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.CREATIVES_PATH_CONFIGURATION));

                //Lists
                try
                {
                    TNS.AdExpress.Domain.Lists.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.LISTS_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to  Baal Lists", e);
                }
                #region Product Baal List
                try
                {
                    Product.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.BAAL_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load Product Baal List", e);
                }
                #endregion

                #region Media Baal List
                try
                {
                    Media.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.BAAL_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load Media Baal List", e);
                }
                #endregion

                #region UnitsInformation
                try
                {
                    // Units
                    UnitsInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNITS_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load UnitsInformation", e);
                }
                #endregion

                #region VehiclesInformation
                try
                {
                    // Vehicles
                    VehiclesInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.VEHICLES_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load VehiclesInformation", e);
                }
                #endregion

                #region UniverseLevels
                try
                {
                    // Universes
                    UniverseLevels.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load UniverseLevels", e);
                }
                #endregion

                #region UniverseLevelsCustomStyles
                try
                {
                    // Charge les styles personnalisés des niveaux d'univers
                    UniverseLevelsCustomStyles.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CUSTOM_STYLES_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load UniverseLevelsCustomStyles", e);
                }
                #endregion

                #region UniverseBranches
                try
                {
                    // Charge la hierachie de niveau d'univers
                    UniverseBranches.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_BRANCHES_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load UniverseBranches", e);
                }
                #endregion

                #region UniverseLevels
                try
                {
                    // Charge les niveaux d'univers
                    UniverseLevels.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load UniverseLevels", e);
                }
                #endregion

                #region AllowedFlags
                try
                {
                    //Load flag list
                    TNS.AdExpress.Domain.AllowedFlags.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.FLAGS_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load AllowedFlags", e);
                }
                #endregion

                #region ModulesList
                try
                {
                    new ModulesList();
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to load ModulesList", e);
                }
                #endregion

                ResultOptionsXL.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.RESULT_OPTIONS_CONFIGURATION_FILENAME));

                if (WebApplicationParameters.VehiclesFormatInformation != null && WebApplicationParameters.VehiclesFormatInformation.Use)
                    VehiclesFormatList.Init(WebApplicationParameters.VehiclesFormatInformation.VehicleFormatInformationList, WebApplicationParameters.DefaultDataLanguage);


                #region Get Source
                try
                {
                    _source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(TNS.AdExpress.Domain.DataBaseDescription.DefaultConnectionIds.webAdministration);
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to Get dataSource", e);
                }
                #endregion

                #region Ares DataBaseConfiguration
                try
                {
                    // Loading DataBase configuration
                    DataBaseConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to Load DataBaseConfiguration", e);
                }
                #endregion

                #region Ares PluginConfiguration
                try
                {
                    PluginConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.PLUGIN_CONFIGURATION_FILENAME));
                }
                catch (Exception e)
                {
                    throw new ShellInitializationException("Impossible to Load Ares PluginConfiguration", e);
                }
                #endregion

                _confFile = WebApplicationParameters.ConfigurationDirectoryRoot + pathConfiguration;

                //Campaign  types
                CampaignTypesInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.CAMPAIGN_TYPES_CONFIGURATION_FILENAME));

                base.InitializeShell(pathConfiguration);
            }
            catch (Exception e)
            {
                sendEmailError("Initialization Error in Shell in Initialize(string directoryName)", e);
                throw new ShellException("Initialization Error in Shell in Initialize(string directoryName)", e);
            }
        }
        #endregion

        #region Plugin event callbacks
        /// <summary>
        /// On send Report
        /// </summary>
        /// <param name="reportTitle">Report Title</param>
        /// <param name="duration">Duration Traitment</param>
        /// <param name="endExecutionDateTime">End Execution Traitment</param>
        /// <param name="reportCore">Report Core</param>
        /// <param name="mailList">Mail List</param>
        /// <param name="errorList">Error List</param>
        /// <param name="from">From</param>
        /// <param name="mailServer">Mail Server</param>
        /// <param name="mailPort">%Mail Port</param>
        /// <param name="navSessionId">Task Id</param>
        protected void t_OnSendReport(string reportTitle, TimeSpan duration, DateTime endExecutionDateTime, string reportCore, ArrayList mailList, ArrayList errorList, string from, string mailServer, int mailPort, long navSessionId)
        {
            ReportingSystem reportingSystem = new ReportingSystem(reportTitle, duration, endExecutionDateTime, reportCore, mailList, errorList, from, mailServer, mailPort, navSessionId);
            try
            {
                reportingSystem.SendReport(reportingSystem.SetReport());
            }
            catch (Exception ex)
            {
                _oLinkClient.ReleaseTaskInError(_oListRunningTasks[navSessionId], new LogLine(ex.Message, ex, eLogCategories.Warning));
            }
        }

        #endregion

    }
}
