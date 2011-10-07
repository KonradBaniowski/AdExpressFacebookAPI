using System;
using TNS.FrameWorks.LSConnectivity;
using TNS.LinkSystem.LinkKernel;
using TNS.AdExpress.Anubis.Bastet;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.Web;
using TNS.Ares.Domain.DataBase;
using TNS.Ares.Constantes;
using TNS.Ares.AdExpress.XlsStats.Exceptions;
using TNS.Ares.Domain.LS;

namespace TNS.Ares.AdExpress.XlsStats
{
    public class XlsStatsShell : Shell {

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
        /// <param name="lsClientConfiguration">LS Client Configuration</param>
        /// <param name="directoryName">Directory Name</param>
        public XlsStatsShell(LsClientConfiguration lsClientConfiguration, string directoryName) :
            base(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, lsClientConfiguration.FamilyName, directoryName, lsClientConfiguration.ModuleDescriptionList, true, false)
        {
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize
        /// </summary>
        protected override void InitializeShell(string pathConfiguration) {
            try {
                try {
                    new WebApplicationParameters();
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load WebApplicationParameters", e);
                }
                try {
                    TNS.AdExpress.Bastet.Units.UnitsInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.UNITS_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UnitsInformation", e);
                }
                try {
                    TNS.AdExpress.Bastet.Periods.PeriodsInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + "Periods.xml"));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load PeriodsInformation", e);
                }
                try {
                    _source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(TNS.AdExpress.Domain.DataBaseDescription.DefaultConnectionIds.webAdministration);
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to Get dataSource", e);
                }
                try {
                    // Loading DataBase configuration
                    DataBaseConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load Ares DataBaseConfiguration", e);
                }
                try {
                    PluginConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.PLUGIN_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load Ares PluginConfiguration", e);
                }
                _confFile = WebApplicationParameters.ConfigurationDirectoryRoot + pathConfiguration;
                base.InitializeShell(pathConfiguration);
                Connect();
            }
            catch (Exception e) {
                sendEmailError("Initialization Error in Shell in Initialize(string directoryName)", e);
                throw new Exception(e.Message, e);
            }
        }
        #endregion

        #region Do task
        /// <summary>
        /// Do Task
        /// </summary>
        /// <param name="oObj">Task</param>
        protected override void DoTask(object oObj)
        {
            var task = (TaskExecution)oObj;
            try {
                    // Extracting parameter id from task
                    long staticNavSession = extractParameterId(task);
                    if (_oListRunningTasks.ContainsKey(staticNavSession) == false)
                        _oListRunningTasks.Add(staticNavSession, task);

                    // Preparing treatment
                    var t = new TreatementSystem();
                    t.EvtStartWork += t_OnStartWork;
                    t.EvtError += t_OnError;
                    t.EvtStopWorkerJob += t_OnStopWorkerJob;
                    t.EvtMessageAlert += t_OnMessage;
                    t.Treatement(_confFile, _source, staticNavSession);
            }
            catch (Exception e) {
                sendEmailError("Build Task Error in Shell in DoTask(object oObj)", e);
                _oLinkClient.ReleaseTaskInError(task, new LogLine("Build Task Error in Shell in DoTask(object oObj)"));
            }
        }
        #endregion

    }
}
