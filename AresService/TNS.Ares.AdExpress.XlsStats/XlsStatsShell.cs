using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWorks.LSConnectivity;
using TNS.LinkSystem.LinkKernel;
using TNS.AdExpress.Anubis.Bastet;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.Web;
using TNS.Ares.Domain.DataBase;
using TNS.Ares.Constantes;
using TNS.Ares.AdExpress.XlsStats.Exceptions;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.Net.Mail;
using System.Collections;
using TNS.FrameWork;
using TNS.Ares.Domain.LS;

namespace TNS.Ares.AdExpress.XlsStats
{
    public class XlsStatsShell : Shell {

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
        public XlsStatsShell(LsClientConfiguration lsClientConfiguration, string directoryName) :
            base(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, lsClientConfiguration.FamilyName, directoryName, lsClientConfiguration.ModuleDescriptionList)
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
                    TNS.AdExpress.Bastet.Units.UnitsInformation.Init(new TNS.FrameWork.DB.Common.XmlReaderDataSource(TNS.AdExpress.Bastet.Web.WebApplicationParameters.CountryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.UNITS_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UnitsInformation", e);
                }
                try {
                    TNS.AdExpress.Bastet.Periods.PeriodsInformation.Init(new TNS.FrameWork.DB.Common.XmlReaderDataSource(TNS.AdExpress.Bastet.Web.WebApplicationParameters.CountryConfigurationDirectoryRoot + "Periods.xml"));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load PeriodsInformation", e);
                }
                try {
                    this._source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(TNS.AdExpress.Domain.DataBaseDescription.DefaultConnectionIds.webAdministration);
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
                this._confFile = WebApplicationParameters.ConfigurationDirectoryRoot + pathConfiguration;
                base.InitializeShell(pathConfiguration);
            }
            catch (Exception e) {
                this.sendEmailError("Initialization Error in Shell in Initialize(string directoryName)", e);
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
            TaskExecution task = (TaskExecution)oObj;
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
                    t.OnMessageAlert += new TNS.Ares.MessageAlert(t_OnMessage);
                    t.Treatement(this._confFile, this._source, staticNavSession);
                }
                else
                    this._oLinkClient.ReleaseTaskInError(task, new LogLine("Link system failure: the given task definition is not correct"));
            }
            catch (Exception e) {
                this.sendEmailError("Build Task Error in Shell in DoTask(object oObj)", e);
                this._oLinkClient.ReleaseTaskInError(task, new LogLine("Build Task Error in Shell in DoTask(object oObj)"));
            }
        }
        #endregion

    }
}
