using System;
using System.Collections.Generic;
using System.Text;
using TNS.Ares.Domain.LS;
using TNS.Ares.AdExpress.XlsVp;
using TNS.FrameWork.DB.Common;

namespace TNS.Ares.AdExpress.XlsVpApp {
    class Program {

        #region Constante
        /// <summary>
        /// Directory configuratyion Name
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion

        static void Main(string[] args) {
            string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
            LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));


            TefnoutShell shell = new TefnoutShell(lsClientConfiguration, lsClientConfiguration.DirectoryName);
            shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
            Console.WriteLine("Ares Xls VP treatment service");
            Console.ReadLine();
            shell.Dispose();
        }

    }
}
