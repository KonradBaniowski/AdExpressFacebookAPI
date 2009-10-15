using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;

using WebConstantes = TNS.Ares.Constantes;
using TNS.Ares.AdExpress.MailAlert;
using TNS.Alert.Domain;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.Ares.Domain.DataBaseDescription;

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
            string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
            TNS.Ares.Domain.LS.LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + WebConstantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));

            string countryCode = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(configurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME));
            string countryConfigurationDirectoryRoot = configurationDirectoryRoot + countryCode + @"\";

            // Loading administration DataSource
            TNS.Ares.Domain.DataBase.DataBaseConfiguration.Load(new XmlReaderDataSource(countryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));
            IDataSource source = TNS.Ares.Domain.DataBase.DataBaseConfiguration.DataBase.GetDefaultConnection(DefaultConnectionIds.alert);
            TNS.Ares.Domain.LS.PluginConfiguration.Load(new XmlReaderDataSource(countryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.PLUGIN_CONFIGURATION_FILENAME));
            AlertConfiguration.Load(new XmlReaderDataSource(countryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.ALERTE_CONFIGURATION));


            // Loading Alert Shell and starting monitor server
            _shell = new AlertShell(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, source, lsClientConfiguration.ModuleDescriptionList, lsClientConfiguration.MaxAvailableSlots, countryCode);
            _shell.StartMonitorServer(lsClientConfiguration.MonitorPort);

            // Stopping program
            Console.WriteLine("Alert treatment service");
            Console.ReadLine();
            _shell.Dispose();
        }
    }
}
