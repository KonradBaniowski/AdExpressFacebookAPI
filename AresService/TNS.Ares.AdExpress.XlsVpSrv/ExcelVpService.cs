using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using TNS.Ares.Domain.LS;
using System.Threading;
using TNS.FrameWork.DB.Common;
using TNS.Ares.AdExpress.XlsVp;

namespace TNS.Ares.AdExpress.XlsVpSrv {
    public partial class ExcelVpService : ServiceBase {
        
        #region Constante
        /// <summary>
        /// Directory configuratyion Name
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion

        #region Variables
        /// <summary>
        /// Current Thread.
        /// </summary>
        private Thread _currentThread = null;
        /// <summary>
        /// Current Shell.
        /// </summary>
        private TefnoutShell _currentShell;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ExcelVpService()
        {
            InitializeComponent();
        }
        #endregion

        #region Run
        /// <summary>
        /// Run
        /// </summary>
        public void Run()
        {
            try {
                string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
                LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));


                _currentShell = new TefnoutShell(lsClientConfiguration, lsClientConfiguration.DirectoryName);
                _currentShell.StartMonitorServer(lsClientConfiguration.MonitorPort);

                Thread.Sleep(Timeout.Infinite);
                _currentShell.Dispose();
            }
            catch (Exception e) {
                if(_currentShell!=null)
                    _currentShell.Dispose();
            }
        }
        #endregion

        #region OnStart
        /// <summary>
        /// OnStart
        /// </summary>
        /// <param name="args">Argument</param>
        protected override void OnStart(string[] args)
        {
            _currentThread = new System.Threading.Thread(this.Run);
            _currentThread.Start();
        }
        #endregion

        #region OnStop
        /// <summary>
        /// OnStop
        /// </summary>
        protected override void OnStop()
        {
            if (_currentShell != null) {
                _currentShell.Dispose();
                _currentShell = null;
            }

            _currentThread.Abort();
        }
        #endregion

    }
}
