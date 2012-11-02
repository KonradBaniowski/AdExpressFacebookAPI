using System;
using System.ServiceProcess;
using System.Threading;
using TNS.Ares.AdExpress.PdfRolexSchedule;
using TNS.Ares.Domain.LS;
using TNS.FrameWork.DB.Common;

namespace TNS.Ares.AdExpress.PdfRolexScheduleSrv
{
    public partial class PdfRolexScheduleService : ServiceBase
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
        private AmonShell _currentShell;
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
                LsClientConfiguration lsClientConfiguration = Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot 
                    + Constantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));


                _currentShell = new AmonShell(lsClientConfiguration, lsClientConfiguration.DirectoryName);
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

        public PdfRolexScheduleService()
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
