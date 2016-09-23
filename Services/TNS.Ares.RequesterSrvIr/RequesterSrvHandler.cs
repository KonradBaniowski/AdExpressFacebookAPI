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
using TNS.Ares.Domain.DataBase;
using TNS.Ares.Domain.LS;
using TNS.Ares.Requester;
using TNS.FrameWork.DB.Common;

namespace TNS.Ares.RequesterSrvIr {
    /// <summary>
    /// RequesterSrvHandler
    /// </summary>
    public partial class RequesterSrvHandler : ServiceBase {

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
        private SrvShell _CurrentShell;
        /// <summary>
        /// Ls Client Name
        /// </summary>
        private LsClientName _lsClientName = LsClientName.AdExpress;
        #endregion

        public RequesterSrvHandler() {
            InitializeComponent();
        }

        #region Run
        /// <summary>
        /// Run
        /// </summary>
        protected void Run() {

            string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";

            // Loading Requester Configurations
            RequesterConfigurations.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.REQUESTER_CONFIGURATION_FILENAME));
            RequesterConfiguration currentResquesterConfiguration = RequesterConfigurations.GetAresConfiguration(_lsClientName);

            configurationDirectoryRoot = configurationDirectoryRoot + "\\" + currentResquesterConfiguration.DirectoryName + "\\";

            // Creating datasource
            IDataSource src = null;

            if (_lsClientName != LsClientName.PixPalaceRequester) {
                // Loading Plugin configuration
                PluginConfiguration.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.PLUGIN_CONFIGURATION_FILENAME));

                // Loading DataBase configuration
                DataBaseConfiguration.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));

                // Creating datasource
                src = (IDataSource)DataBaseConfiguration.DataBase.GetDefaultConnection(PluginConfiguration.DefaultConnectionId);
            }

            _CurrentShell = new SrvShell(_lsClientName, currentResquesterConfiguration, src);
            _CurrentShell.StartMonitorServer(currentResquesterConfiguration.MonitorPort);

            Thread.Sleep(Timeout.Infinite);
            _CurrentShell.Dispose();
        }
        #endregion

        #region On Start
        /// <summary>
        /// On Start
        /// </summary>
        /// <param name="args">args</param>
        protected override void OnStart(string[] args) {

            for (int i = 0; i < args.Length; i++) {
                switch (args[i]) {
                    case "-p":
                        if (i < args.Length - 1) {
                            i++;
                            _lsClientName = (LsClientName)Enum.Parse(typeof(LsClientName), args[i]);
                        }
                        break;
                }

            }
            _CurrentThread = new System.Threading.Thread(this.Run);
            _CurrentThread.Start();

        }
        #endregion

        #region On Stop
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
