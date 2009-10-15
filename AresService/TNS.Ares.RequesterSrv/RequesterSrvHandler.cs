using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using TNS.Ares.Domain.DataBase;
using TNS.FrameWork.DB.Common;

using ConfigurationFile = TNS.Ares.Constantes.ConfigurationFile;
using TNS.Ares.Requester;
using System.Threading;
using System.Configuration;
using TNS.Ares.Domain.LS;

namespace TNS.Ares.RequesterSrv
{
    public partial class RequesterSrvHandler : ServiceBase
    {

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
        /// Requester Name.
        /// </summary>
        private RequesterSrvMain _requesterSrvMain;
        /// <summary>
        /// Ls Client Name
        /// </summary>
        private LsClientName _lsClientName = LsClientName.AdExpress;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public RequesterSrvHandler() {
            InitializeComponent();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public RequesterSrvHandler(RequesterSrvMain requesterSrvMain)
            : this() {
            _requesterSrvMain = requesterSrvMain;
        }
        #endregion

        #region Run
        /// <summary>
        /// Run
        /// </summary>
        protected void Run()
        {
            string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";

            // Loading Requester Configurations
            RequesterConfigurations.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.REQUESTER_CONFIGURATION_FILENAME));
            RequesterConfiguration currentResquesterConfiguration = RequesterConfigurations.GetAresConfiguration(_lsClientName);

            configurationDirectoryRoot = configurationDirectoryRoot + "\\" + currentResquesterConfiguration.DirectoryName + "\\";

            // Creating datasource
            IDataSource src = null;

            if (_lsClientName != LsClientName.PixPalaceRequester)
            {
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

        #region OnStart
        /// <summary>
        /// Onstart
        /// </summary>
        /// <param name="args">Arguments</param>
        protected override void OnStart(string[] args)
        {
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
            
            /*
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["RequesterName"]))
            {
                _lsClientName = (LsClientName)Enum.Parse(typeof(LsClientName), ConfigurationManager.AppSettings["RequesterName"]);
                _CurrentThread = new Thread(new ThreadStart(this.Run));
                _CurrentThread.Name = ConfigurationManager.AppSettings["RequesterName"];
                _CurrentThread.Start();
            }*/
        }
        #endregion

        #region OnStop
        /// <summary>
        /// OnStop
        /// </summary>
        protected override void OnStop()
        {            
            if (_CurrentShell != null)
            {
                _CurrentShell.Dispose();
                _CurrentShell = null;
            }

            _CurrentThread.Abort();
        }
        #endregion

    }
}
