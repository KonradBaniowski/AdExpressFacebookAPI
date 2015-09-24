using KM.AdExpress.AudioVideoConverter.DAL;
using KM.AdExpress.AudioVideoConverter.Domain;
using KM.AdExpress.AudioVideoConverter.Domain.XmlLoaders;
using KM.AdExpress.AudioVideoConverter.Tools;
using LinkSystem.LinkClient;
using LinkSystem.LinkKernel.Core;
using LinkSystem.LinkKernel.CoreMonitor;
using LinkSystem.LinkKernel.Interfaces;
using LinkSystem.LinkMonitor;
using LinkSystem.LinkMonitor.AdvancedTraceListeners;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AudioVideoConverter
{
    public partial class Engine : LinkClient, IServer_SelectRule
    {
        #region Constants
        private const string ConfigurationDirectoryName = "Configuration";
        private const int Timeout = 10000;
        private const string ErrorMailFileName = "ErrorMail.xml";
        private const string LinkClientFileName = "LinkClient.xml";
        private const string LsClientConfigurationFileName = "LsClientConfiguration.xml";     
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

        /// <summary>
        /// Impersonate Information
        /// </summary>
        private ImpersonateInformation _impersonateInformation = null;
        /// <summary>
        /// Impersonation
        /// </summary>
        private Impersonation _oImp = null;

        private bool _isSelected;
        private MonitorServer _oMonitorServer;

       // private List<PreRollInformation> _preRollInformations;
        private string _connectionString = string.Empty;
        private string _providerDataAccess = string.Empty;
        private string _customerMailFrom = string.Empty;      
        private string _customerMailServer = string.Empty;
        public string RadioDestinationPath { get; set; }
        public string TvDestinationPath { get; set; }
        public string VideoBrQuality { get; set; }
        public string VideoMrQuality { get; set; }
        public string VideoHrQuality { get; set; }

        public bool UseImpersonate { get; set; }
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


            _providerDataAccess = ConfigurationManager.AppSettings["ProviderDataAccess"];          
            _connectionString =
                String.Format(
                    Provider.GetConnectionStringByProvider(_providerDataAccess),
                    ConfigurationManager.AppSettings["DbUserName"],
                    ConfigurationManager.AppSettings["DbPassword"],
                    ConfigurationManager.AppSettings["DbDataSource"]);

            RadioDestinationPath = ConfigurationManager.AppSettings["RadioDestinationPath"];
            TvDestinationPath = ConfigurationManager.AppSettings["TvDestinationPath"];

            VideoBrQuality = ConfigurationManager.AppSettings["VideoBrQuality"];
            VideoMrQuality = ConfigurationManager.AppSettings["VideoMrQuality"];
            VideoHrQuality = ConfigurationManager.AppSettings["VideoHrQuality"];

            UseImpersonate = Convert.ToBoolean(ConfigurationManager.AppSettings["UseImpersonate"]);
            _impersonateInformation = new ImpersonateInformation(ConfigurationManager.AppSettings["User"],
                ConfigurationManager.AppSettings["Password"], ConfigurationManager.AppSettings["Domain"]);
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
                case 43:
                    ExecuteTask(taskExecution, DoAudioConversion, Guid.NewGuid().ToString());
                    break;
                case 44:
                    ExecuteTask(taskExecution, DoVideoConversion, Guid.NewGuid().ToString());
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
                Subject = String.Format("AdExpress Audio Video Converter > KM.AdExpress.AudioVideoConverter.App :  Audio Video Converter SUCCESSFUL"),
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

        #region Impersonation Methods
        /// <summary>
        /// Open Impersonation
        /// </summary>
        /// <returns></returns>
        public void OpenImpersonation()
        {

            if (_impersonateInformation != null)
            {
                CloseImpersonation();
                _oImp = new Impersonation();
                _oImp.ImpersonateValidUser(_impersonateInformation.UserName, _impersonateInformation.Domain, _impersonateInformation.Password, Impersonation.LogonType.LOGON32_LOGON_NEW_CREDENTIALS);
            }
        }
        /// <summary>
        /// Close Impersonation
        /// </summary>
        public void CloseImpersonation()
        {
            if (_oImp != null)
                _oImp.UndoImpersonation();
            _oImp = null;
        }
        #endregion

       
    }
}
