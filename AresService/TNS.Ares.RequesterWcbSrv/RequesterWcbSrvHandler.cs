using System;
using System.ServiceProcess;
using TNS.Ares.Constantes;
using TNS.Ares.Domain.DataBase;
using TNS.Ares.RequesterSrv;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Requester;
using System.Threading;
using TNS.Ares.Domain.LS;
using System.IO;
using TNS.Alert.Domain;

namespace TNS.Ares.RequesterWcbSrv
{
    public partial class RequesterWcbSrvHandler : ServiceBase
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
        private Thread _currentThread;
        /// <summary>
        /// Current Shell.
        /// </summary>
        private SrvShell _currentShell;
        /// <summary>
        /// Requester Name.
        /// </summary>
        private RequesterSrvMain _requesterSrvMain;
        /// <summary>
        /// Ls Client Name
        /// </summary>
        private LsClientName _lsClientName = LsClientName.WcbRequester;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public RequesterWcbSrvHandler() {
            InitializeComponent();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public RequesterWcbSrvHandler(RequesterSrvMain requesterSrvMain)
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

                //Loading Alert Configuration
                if (File.Exists(configurationDirectoryRoot + AdExpress.Constantes.Web.ConfigurationFile.ALERTE_CONFIGURATION))
                    AlertConfiguration.Load(new XmlReaderDataSource(configurationDirectoryRoot + AdExpress.Constantes.Web.ConfigurationFile.ALERTE_CONFIGURATION));

                // Loading DataBase configuration
                DataBaseConfiguration.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));

                // Creating datasource
                src = DataBaseConfiguration.DataBase.GetDefaultConnection(PluginConfiguration.DefaultConnectionId);
            }

            _currentShell = new SrvShell(_lsClientName, currentResquesterConfiguration, src);
            _currentShell.StartMonitorServer(currentResquesterConfiguration.MonitorPort);
                        
            Thread.Sleep(Timeout.Infinite);
            _currentShell.Dispose();
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
            _currentThread = new Thread(Run);
            _currentThread.Start();
            
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
