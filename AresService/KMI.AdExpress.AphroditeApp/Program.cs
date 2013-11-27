using System;
using System.Collections.Generic;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Constantes;
using KMI.AdExpress.AphroditeLS;

namespace KMI.AdExpress.AphroditeApp {
    class Program {
        static void Main(string[] args) {

            string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + @"Configuration\";

            // Loading LS Client Configurations
            TNS.Ares.Domain.LS.LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));
            AphroditeShell shell = new AphroditeShell(lsClientConfiguration, null);
            shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
            Console.WriteLine("Aphrodite Service");
            Console.ReadLine();

            shell.Dispose();

        }
    }
}
