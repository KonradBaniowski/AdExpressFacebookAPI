using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using TNS.FrameWork.DB.Common;

using WebConstantes = TNS.Ares.Constantes;
using System.Threading;
using TNS.Ares.AdExpress.MailAlert;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.Exceptions;
using TNS.Ares.Domain.DataBaseDescription;
using TNS.Alert.Domain;
using TNS.AdExpress.Domain.Web;

namespace TNS.Ares.AdExpress.MailAlertSrv
{
    public partial class AresMailAlertSrv : ServiceBase
    {
        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion

        private AlertShell _shell;

        public AresMailAlertSrv()
        {
            InitializeComponent();
        }

        private void InitService()
        {
            try {
                EventLog.WriteEntry("Ares Alert Treatment service is starting", EventLogEntryType.Information);
                TNS.Ares.Domain.LS.LsClientConfiguration lsClientConfiguration = null;

                string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
                
                try {
                    lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + WebConstantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));
                }
                catch {
                    throw new BaseException("Impossible to Load LsClientConfiguration");
                }
                
                // Loading Alert Shell and starting monitor server
                _shell = new AlertShell(lsClientConfiguration);
                _shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
            }
            catch (Exception e) {
                try { EventLog.WriteEntry("Error in InitService : " + e.Message, EventLogEntryType.Error); }
                catch { }
            }
        }

        protected override void OnStart(string[] args)
        {
            new Thread(new ThreadStart(this.InitService)).Start();
        }

        protected override void OnStop()
        {
            _shell.Dispose();
            EventLog.WriteEntry("Ares Alert Treatment service is stopping", EventLogEntryType.Information);
        }
    }
}
