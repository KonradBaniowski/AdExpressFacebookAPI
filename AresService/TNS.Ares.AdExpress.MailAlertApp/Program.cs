using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;

using WebConstantes = TNS.Ares.Constantes;
using TNS.Ares.AdExpress.MailAlert;
using TNS.Alert.Domain;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.Ares.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.AdExpress.MailAlertApp
{
    class Program
    {
        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion

        private static AlertShell _shell;

        static void Main(string[] args)
        {
            TNS.Ares.Domain.LS.LsClientConfiguration lsClientConfiguration = null;

            string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
            try {
                lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + WebConstantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));
            }
            catch (Exception e) {
                throw new BaseException("Impossible to Load LsClientConfiguration", e);
            }

            // Loading Alert Shell and starting monitor server
            _shell = new AlertShell(lsClientConfiguration);
            _shell.StartMonitorServer(lsClientConfiguration.MonitorPort);

            // Stopping program
            Console.WriteLine("Alert treatment service");
            Console.ReadLine();
            _shell.Dispose();
        }
    }
}
