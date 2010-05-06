using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using TNS.LinkSystem.LinkKernel;
using TNS.AdExpress.Anubis.Mnevis;
using TNS.Ares;
using TNS.FrameWork.DB.Common;
using TNS.FrameWorks.LSConnectivity;
using TNS.AdExpress.Domain.Web;
using TNS.Ares.AdExpress.PdfChronoPM.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.Ares.Domain.DataBase;
using TNS.Ares.Domain.LS;
using TNS.Ares.Constantes;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.AdExpress.PdfChronoPM
{
    public class MnevisShell : Shell
    {
        #region Variables
        /// <summary>
        /// DataSource
        /// </summary>
        private IDataSource _source;
        /// <summary>
        /// Path File Of Configuration
        /// </summary>
        private string _confFile;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productName">Product Name</param>
        /// <param name="familyId">Family Id</param>
        /// <param name="source">DataSource</param>
        /// <param name="confFile">Path Configuration File</param>
        public MnevisShell(string productName, int familyId, List<ModuleDescription> moduleDescriptionList, string directoryName) :
            base(productName, familyId, moduleDescriptionList)
        {
            this.Initialize(directoryName);
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize
        /// </summary>
        protected void Initialize(string directoryName) {
            try {

                #region WebApplicationParameters
                try {
                    new WebApplicationParameters();
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load WebApplicationParameters", e);
                }
                #endregion

                #region Product Baal List
                try {
                    Product.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.BAAL_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load Product Baal List", e);
                }
                #endregion

                #region Media Baal List
                try {
                    Media.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.BAAL_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load Media Baal List", e);
                }
                #endregion

                #region UnitsInformation
                try {
                    // Units
                    UnitsInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNITS_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UnitsInformation", e);
                }
                #endregion

                #region VehiclesInformation
                try {
                    // Vehicles
                    VehiclesInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.VEHICLES_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load VehiclesInformation", e);
                }
                #endregion

                #region UniverseLevels
                try {
                    // Universes
                    UniverseLevels.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UniverseLevels", e);
                }
                #endregion

                #region UniverseLevelsCustomStyles
                try {
                    // Charge les styles personnalisés des niveaux d'univers
                    UniverseLevelsCustomStyles.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CUSTOM_STYLES_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UniverseLevelsCustomStyles", e);
                }
                #endregion

                #region UniverseBranches
                try {
                    // Charge la hierachie de niveau d'univers
                    UniverseBranches.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_BRANCHES_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UniverseBranches", e);
                }
                #endregion

                #region UniverseLevels
                try {
                    // Charge les niveaux d'univers
                    UniverseLevels.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UniverseLevels", e);
                }
                #endregion

                #region AllowedFlags
                try {
                    //Load flag list
                    TNS.AdExpress.Domain.AllowedFlags.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.FLAGS_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load AllowedFlags", e);
                }
                #endregion

                #region ModulesList
                try {
                    new ModulesList();
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load ModulesList", e);
                }
                #endregion

                #region Get Source
                try {
                    this._source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(TNS.AdExpress.Domain.DataBaseDescription.DefaultConnectionIds.webAdministration);
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to Get dataSource", e);
                }
                #endregion

                #region Ares DataBaseConfiguration
                try {
                    // Loading DataBase configuration
                    DataBaseConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to Load DataBaseConfiguration", e);
                }
                #endregion

                #region Ares PluginConfiguration
                try {
                    PluginConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.PLUGIN_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to Load Ares PluginConfiguration", e);
                }
                #endregion

                this._confFile = WebApplicationParameters.ConfigurationDirectoryRoot + directoryName;
            }
            catch (Exception e) {
                this.sendEmailError("Initialization Error in Shell in Initialize(string directoryName)", e);
            }
        }
        #endregion

        #region DoTask
        /// <summary>
        /// Do task
        /// </summary>
        /// <param name="oObj">task Execution</param>
        protected override void DoTask(object oObj)
        {
            TaskExecution task = (TaskExecution)oObj;
            long staticNavSessionId = -1;
            try {
                if (task != null) {
                    // Extracting parameter id from task
                    long staticNavSession = extractParameterId(task);
                    if (_oListRunningTasks.ContainsKey(staticNavSession) == false)
                        _oListRunningTasks.Add(staticNavSession, task);
                        // Preparing treatment
                        TreatementSystem t = new TreatementSystem();
                        t.OnStartWork += new TNS.Ares.StartWork(t_OnStartWork);
                        t.OnError += new TNS.Ares.Error(t_OnError);
                        t.OnStopWorkerJob += new TNS.Ares.StopWorkerJob(t_OnStopWorkerJob);
                        t.OnSendReport += new TNS.Ares.SendReport(t_OnSendReport);
                        t.OnMessageAlert += new TNS.Ares.MessageAlert(t_OnMessageAlert);
                        t.Treatement(this._confFile, this._source, staticNavSession);
                }
                else {
                    this._oLinkClient.ReleaseTaskInError(task, new LogLine("TaskExecution object is null", eLogCategories.Problem));
                }
            }
            catch (Exception e) {
                this.sendEmailError("Build Task Error in Shell in DoTask(object oObj)", e);
                this._oLinkClient.ReleaseTaskInError(task, new LogLine("Build Task Error in Shell in DoTask(object oObj)"));
            }

        }
        #endregion

        #region Plugin event callbacks

        protected void t_OnSendReport(string reportTitle, TimeSpan duration, DateTime endExecutionDateTime, string reportCore, ArrayList mailList, ArrayList errorList, string from, string mailServer, int mailPort, long navSessionId)
        {
            ReportingSystem reportingSystem = new ReportingSystem(reportTitle, duration, endExecutionDateTime, reportCore, mailList, errorList, from, mailServer, mailPort, navSessionId);
            try
            {
                reportingSystem.SendReport(reportingSystem.SetReport());
            }
            catch (System.Exception ex)
            {
                this._oLinkClient.ReleaseTaskInError(_oListRunningTasks[navSessionId], new LogLine(ex.Message, ex, eLogCategories.Warning));
            }
        }

        #endregion

    }
}
