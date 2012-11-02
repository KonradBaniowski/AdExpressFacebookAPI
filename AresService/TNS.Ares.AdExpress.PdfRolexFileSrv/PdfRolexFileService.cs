using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using TNS.Ares.AdExpress.PdfRolexFile;
using TNS.Ares.Domain.LS;
using TNS.FrameWork.DB.Common;

namespace TNS.Ares.AdExpress.PdfRolexFileSrv
{
    public partial class PdfRolexFileService : ServiceBase
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
        private PtahShell _currentShell;
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


                _currentShell = new PtahShell(lsClientConfiguration, lsClientConfiguration.DirectoryName);
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

        public PdfRolexFileService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _currentThread = new Thread(Run);
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
