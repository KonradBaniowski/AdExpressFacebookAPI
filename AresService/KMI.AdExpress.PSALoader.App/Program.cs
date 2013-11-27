using System;
using System.Collections.Generic;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Constantes;

namespace KMI.AdExpress.PSALoader.App {
    class Program {
        static void Main(string[] args) {

            string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + @"Configuration\";

            // Loading LS Client Configurations
            TNS.Ares.Domain.LS.LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));
            PSALoaderShell shell = new PSALoaderShell(lsClientConfiguration);
            shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
            Console.WriteLine("PSA Service");
            Console.ReadLine();

            shell.Dispose();

        }
    }
}
