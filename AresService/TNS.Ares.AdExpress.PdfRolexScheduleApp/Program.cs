using System;
using TNS.Ares.AdExpress.PdfRolexSchedule;
using TNS.FrameWork.DB.Common;

namespace TNS.Ares.AdExpress.PdfRolexScheduleApp
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
            var lsClientConfiguration = Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));


            var shell = new AmonShell(lsClientConfiguration, lsClientConfiguration.DirectoryName);
            shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
            Console.WriteLine("Ares Service treatment - PDF Rolex Schedule (Amon)");
            Console.ReadLine();
            shell.Dispose();
        }
    }
}
