using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress;

using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.DB.Common;
using TNS.Ares.AdExpress.XlsChrono;
using TNS.Ares.Domain.LS;
using TNS.Ares.Constantes;
using TNS.Ares.Domain.DataBase;

namespace TNS.Ares.AdExpress.XlsChronoApp
{
    class Program
    {
        #region Constante
        /// <summary>
        /// Directory configuratyion Name
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion

        static void Main(string[] args) {
            string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
            LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));


            SatetShell shell = new SatetShell(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, lsClientConfiguration.ModuleDescriptionList, lsClientConfiguration.DirectoryName);
            shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
            Console.WriteLine("Ares XlsStats treatment service");
            Console.ReadLine();
            shell.Dispose();
        }
    }
}
