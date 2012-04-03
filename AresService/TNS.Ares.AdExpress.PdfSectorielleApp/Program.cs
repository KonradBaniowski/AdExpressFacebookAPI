using System;
using TNS.FrameWork.DB.Common;
using TNS.Ares.AdExpress.PdfSectorielle;
using TNS.Ares.Domain.LS;
using TNS.Ares.Constantes;

namespace TNS.Ares.AdExpress.PdfSectorielleApp
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
            try
            {
                string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
                LsClientConfiguration lsClientConfiguration =
                    Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));


                HotepShell shell = new HotepShell(lsClientConfiguration, lsClientConfiguration.DirectoryName);
                shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
                Console.WriteLine("Ares Service treatment - PDF Sectorielle");
                Console.ReadLine();
                shell.Dispose();
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
