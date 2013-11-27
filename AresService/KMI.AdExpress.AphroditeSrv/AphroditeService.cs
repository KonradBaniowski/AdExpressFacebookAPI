using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using TNS.Ares.Constantes;
using TNS.FrameWork.DB.Common;

namespace KMI.AdExpress.AphroditeSrv {
    public partial class AphroditeService : ServiceBase {

        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion

        #region Variables
        /// <summary>
        /// Current Thread.
        /// </summary>
        private Thread _CurrentThread = null;
        /// <summary>
        /// Current Shell.
        /// </summary>
        private AphroditeLS.AphroditeShell _CurrentShell;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AphroditeService() {
            InitializeComponent();
        }
        #endregion

        #region Run
        /// <summary>
        /// Run
        /// </summary>
        protected void Run() {

            string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";

            // Loading LS Client Configurations
            TNS.Ares.Domain.LS.LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));
            _CurrentShell = new AphroditeLS.AphroditeShell(lsClientConfiguration, eventLogAphrodite);
            _CurrentShell.StartMonitorServer(lsClientConfiguration.MonitorPort);
        }
        #endregion

        #region Start
        /// <summary>
        /// On Start
        /// </summary>
        /// <param name="args">Args</param>
        protected override void OnStart(string[] args) {
            _CurrentThread = new System.Threading.Thread(this.Run);
            _CurrentThread.Start();
        }
        #endregion

        #region Stop
        /// <summary>
        /// On Stop
        /// </summary>
        protected override void OnStop() {
            if (_CurrentShell != null) {
                _CurrentShell.Dispose();
                _CurrentShell = null;
            }

            _CurrentThread.Abort();
        }
        #endregion
    }
}
