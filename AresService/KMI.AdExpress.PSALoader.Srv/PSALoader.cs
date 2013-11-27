using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using TNS.FrameWork.DB.Common;
using System.Threading;
using TNS.Ares.Constantes;

namespace KMI.AdExpress.PSALoader.Srv {
    public partial class PSALoader : ServiceBase {

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
        private PSALoaderShell _CurrentShell;
        #endregion

        #region Constructor
        public PSALoader() {
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
            _CurrentShell = new PSALoaderShell(lsClientConfiguration);
            _CurrentShell.StartMonitorServer(lsClientConfiguration.MonitorPort);
        }
        #endregion

        #region OnStart
        /// <summary>
        /// On Start
        /// </summary>
        /// <param name="args">Args</param>
        protected override void OnStart(string[] args) {
            _CurrentThread = new System.Threading.Thread(this.Run);
            _CurrentThread.Start();
        }
        #endregion

        #region OnStop
        /// <summary>
        /// OnStop
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
