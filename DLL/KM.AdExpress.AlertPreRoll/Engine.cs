using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using KM.AdExpress.AlertPreRoll.DAL;
using KM.AdExpress.AlertPreRoll.Domain;
using KM.AdExpress.AlertPreRoll.Domain.XmlLoaders;
using LinkSystem.LinkClient;
using LinkSystem.LinkKernel.Core;
using LinkSystem.LinkKernel.CoreMonitor;
using LinkSystem.LinkKernel.Interfaces;
using LinkSystem.LinkMonitor;
using LinkSystem.LinkMonitor.AdvancedTraceListeners;

namespace KM.AdExpress.AlertPreRoll
{
    public partial class Engine : LinkClient, IServer_SelectRule
    {
        #region Constants
        private const string ConfigurationDirectoryName = "Configuration";
        private const int Timeout = 10000;
        private const string ErrorMailFileName = "ErrorMail.xml";
        private const string LinkClientFileName = "LinkClient.xml";
        private const string LsClientConfigurationFileName = "LsClientConfiguration.xml";
        private const string MediaInformationFileName = "MediasInformation.xml";
        readonly string _configurationDirectoryRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationDirectoryName);
        const string ModuleName = "ModuleName";
        #endregion

        #region Variable

        /// <summary>
        /// Modules
        /// </summary>
        private readonly List<ModuleDescription> _modules;

        /// <summary>
        /// Server link
        /// </summary>
        private readonly ServerLink _serverLink;


        private bool _isSelected;
        private MonitorServer _oMonitorServer;

        private List<PreRollInformation> _preRollInformations;
        private string _connectionString = string.Empty;
        private string _providerDataAccess = string.Empty;
        private string _customerMailFrom = string.Empty;
        private int _customerMailPort = 21;
        private string _customerMailServer = string.Empty;
        private ArrayList _customerMailRecipients;
        private string _ftpServer = string.Empty;
        private string _ftpUserName = string.Empty;
        private string _ftpPassword = string.Empty;

        #endregion

        #region Constructor / Destructor

        public Engine()
        {

            var productInfo = ProductInformationLoader.Load(Path.Combine(_configurationDirectoryRoot, LsClientConfigurationFileName));

            // MonitorServer
            _oMonitorServer = new MonitorServer();
            _oMonitorServer.ActivateDiskPersistence(productInfo.ProductInformation.ProductName, 7);
            _oMonitorServer.ActivateCommunication(productInfo.MonitorPort);


            // Configure the advanced trace
            AdvancedTrace.RegisterTraceListenerToAllTraceType(new ConsoleStandardOutputListener());
            AdvancedTrace.RegisterTraceListenerToAllTraceType(new MonitorServerListener(_oMonitorServer));
            AdvancedTrace.RegisterCustomTraceType(ModuleName);


            // MailerConfig
            var configEmailFileName = Path.Combine(_configurationDirectoryRoot, ErrorMailFileName);
            if (File.Exists(configEmailFileName))
            {
                var mailerConfig = MailerConfigLoader.Load(configEmailFileName);

                _oMonitorServer.ActivateMailer(mailerConfig);

                // AdvancedTrace.RegisterTraceListener(TraceTypeConstant.Warning, new MonitorServerEmailListener(_oMonitorServer));
                AdvancedTrace.RegisterTraceListener(TraceTypeConstant.Error, new MonitorServerEmailListener(_oMonitorServer));
                AdvancedTrace.RegisterTraceListener(TraceTypeConstant.Fatal, new MonitorServerEmailListener(_oMonitorServer));
            }

            // Load modules
            _modules = ModuleDescriptionLoader.Load(Path.Combine(_configurationDirectoryRoot, LsClientConfigurationFileName));

            // Load LinkClient.xml configuration and connect to the link server
            _serverLink = ServerLinkConfigurationLoader.Load(Path.Combine(_configurationDirectoryRoot, LinkClientFileName));

            CallTimeout = Timeout;
            AlwaysReconnect = true;


            _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            _providerDataAccess = ConfigurationManager.AppSettings["ProviderDataAccess"];          
            _customerMailFrom = ConfigurationManager.AppSettings["Sender"];
            _customerMailPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            _customerMailServer = ConfigurationManager.AppSettings["SmtpServer"];
            _customerMailRecipients = new ArrayList(ConfigurationManager.AppSettings["Recipients"].Split(';'));
            _connectionString =
                String.Format(
                    Provider.GetConnectionStringByProvider(_providerDataAccess),
                    ConfigurationManager.AppSettings["DbUserName"],
                    ConfigurationManager.AppSettings["DbPassword"],
                    ConfigurationManager.AppSettings["DbDataSource"]);

            _ftpServer = ConfigurationManager.AppSettings["FtpServer"];
            _ftpUserName = ConfigurationManager.AppSettings["FtpUserName"];
             _ftpPassword = ConfigurationManager.AppSettings["FtpPassword"];

            try
            {
                Connect(_serverLink.Host, _serverLink.Port, true);

                _isSelected = true;
            }
            catch (Exception exception)
            {
                AdvancedTrace.TraceFatal("The configuration file contains errors.", exception);
            }
        }

        public override void Dispose()
        {
            CancelAllTasks();
            WaitForRunningTaskCompletion();

            _oMonitorServer.Dispose();
            _oMonitorServer = null;

            base.Dispose();
        }

        #endregion

        #region Overrides of LinkClient

        protected override ProductInformation RetrieveProductInformation()
        {
            return ProductInformationLoader.Load(Path.Combine(_configurationDirectoryRoot, LsClientConfigurationFileName)).ProductInformation;
        }

        #endregion

        #region Tasks management

        protected override void OnTaskExecute(TaskExecution taskExecution)
        {
            switch (taskExecution.ModuleID)
            {
                case 42:
                    ExecuteTask(taskExecution, DoPreRoll, Guid.NewGuid().ToString());
                    break;
            }
        }

        #endregion

        #region Implement IServer_SelectRule

        public void Block(bool bln)
        {
            _isSelected = !bln;
        }

        public void BlockModule(int intModuleID, bool bln)
        {
            _modules.First(p => p.ModuleID == intModuleID).IsSelectable = bln;
        }

        public bool IsSelectable()
        {
            return _isSelected;
        }

        public List<ModuleDescription> KnownModules()
        {
            return _modules;
        }

        public int MaxAvailableSlots()
        {
            return 1;
        }

        #endregion

        #region Private Methos

        private void SendEmail(String message)
        {
            var oMessage = new MonitorMailMessage
            {
                Subject = String.Format("AdExpress Pre Roll > KM.AdExpress.AlertPreRoll.App : Preroll alert SUCCESSFUL"),
                Title = "AdExpress",
            };

            oMessage.AddHTMLLine(message);

            _oMonitorServer.SendEmail(oMessage);
        }

        public static void SendEmail(MonitorServer oMonitorServer, String message, String appName, String moduleName, Boolean isInError = false)
        {
            var oMessage = new MonitorMailMessage
            {
                Subject = String.Format("AdExpress > {0} : {1} {2}", appName, moduleName, isInError ? "ERROR" : "SUCCESSFUL"),
                Title = "AdExpress ",
            };

            oMessage.AddHTMLLine(message);

            oMonitorServer.SendEmail(oMessage);
        }
        #endregion
    }
}
