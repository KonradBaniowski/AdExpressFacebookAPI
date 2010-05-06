using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web;

using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.DataBaseDescription;
using System.IO;
using TNS.Ares.Domain.LS;
using TNS.Ares.AdExpress.PdfChronoPM;
using TNS.Ares.Constantes;
using TNS.Ares.Domain.DataBase;

namespace TNS.Ares.AdExpress.PdfChronoPMApp
{
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


            MnevisShell shell = new MnevisShell(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, lsClientConfiguration.ModuleDescriptionList, lsClientConfiguration.DirectoryName);
            shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
            Console.WriteLine("Ares Service treatment - PDF Plan de Roulement (Mnevis)");
            Console.ReadLine();
            shell.Dispose();
        }
    }
}
