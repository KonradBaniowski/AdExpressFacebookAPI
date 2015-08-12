using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Ares.Domain.LS;
using TNS.FrameWork.DB.Common;
using TNS.Ares.AdExpress.TextTvSponsorship;

namespace TNS.Ares.AdExpress.TextTvSponsorshipApp
{
    class Program
    {
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


            PachetShell shell = new PachetShell(lsClientConfiguration, lsClientConfiguration.DirectoryName);
            shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
            Console.WriteLine("Ares Service treatment - Texte export popcorn (Pachet)");
            Console.ReadLine();
            shell.Dispose();
        }
    }
}
