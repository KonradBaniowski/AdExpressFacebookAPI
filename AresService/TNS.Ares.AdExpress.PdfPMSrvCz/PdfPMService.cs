using System;
using System.ServiceProcess;
using System.Threading;
using TNS.Ares.AdExpress.PdfPM;
using TNS.Ares.Domain.LS;
using TNS.FrameWork.DB.Common;

namespace TNS.Ares.AdExpress.PdfPMSrvCz
{
    public partial class PdfPMService : ServiceBase
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
        private Thread _currentThread;
        /// <summary>
        /// Current Shell.
        /// </summary>
        private MiysisShell _currentShell;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PdfPMService()
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
            try
            {
                string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
                LsClientConfiguration lsClientConfiguration = Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + Constantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));


                _currentShell = new MiysisShell(lsClientConfiguration, lsClientConfiguration.DirectoryName);
                _currentShell.StartMonitorServer(lsClientConfiguration.MonitorPort);

                Thread.Sleep(Timeout.Infinite);
                _currentShell.Dispose();
            }
            catch (Exception)
            {
                if (_currentShell != null)
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
            _currentThread = new Thread(Run);
            _currentThread.Start();
        }
        #endregion

        #region OnStop
        /// <summary>
        /// OnStop
        /// </summary>
        protected override void OnStop()
        {
            if (_currentShell != null)
            {
                _currentShell.Dispose();
                _currentShell = null;
            }

            _currentThread.Abort();
        }
        #endregion
    }
}
