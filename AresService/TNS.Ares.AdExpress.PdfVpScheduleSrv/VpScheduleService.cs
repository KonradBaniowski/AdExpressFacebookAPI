using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Domain.LS;
using TNS.Ares.AdExpress.PdfVpSchedule;
using System.Threading;

namespace TNS.Ares.AdExpress.PdfVpScheduleSrv
{
    public partial class VpScheduleService : ServiceBase
    {
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
        private ThouerisShell _currentShell;
        #endregion

        #region Run
        /// <summary>
        /// Run
        /// </summary>
        public void Run()
        {
            try
            {
                string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
                LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));


                _currentShell = new ThouerisShell(lsClientConfiguration, lsClientConfiguration.DirectoryName);
                _currentShell.StartMonitorServer(lsClientConfiguration.MonitorPort);

                Thread.Sleep(Timeout.Infinite);
                _currentShell.Dispose();
            }
            catch (Exception e)
            {
                if (_currentShell != null)
                    _currentShell.Dispose();
            }
        }
        #endregion

        public VpScheduleService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _currentThread = new System.Threading.Thread(this.Run);
            _currentThread.Start();
        }

        protected override void OnStop()
        {
            if (_currentShell != null)
            {
                _currentShell.Dispose();
                _currentShell = null;
            }

            _currentThread.Abort();
        }
    }
}
