using System;
using System.Collections.Generic;
using System.Text;
using TNS.Ares.AdExpress.XlsStats;
using TNS.FrameWork.DB.Common;

using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.Ares.Domain.LS;
using TNS.Ares.Constantes;
using TNS.Ares.Domain.DataBase;

namespace TNS.Ares.AdExpress.XlsStatsApp
{
    class Program {

        #region Constante
        /// <summary>
        /// Directory configuratyion Name
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion

        static void Main(string[] args)
        {
            string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
            LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));


            XlsStatsShell shell = new XlsStatsShell(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, lsClientConfiguration.ModuleDescriptionList, lsClientConfiguration.DirectoryName);
            shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
            Console.WriteLine("Ares XlsStats treatment service");
            Console.ReadLine();
            shell.Dispose();
        }
    }
}
