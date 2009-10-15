using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using TNS.FrameWork.DB.Common;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TNS.Ares.Domain.DataBaseDescription;
using TNS.Ares.Domain.LS;
using TNS.Ares.Domain.DataBase;
using TNS.Ares.Domain.Layers;
using System.Reflection;
using TNS.LinkSystem.LinkKernel;
using System.Text.RegularExpressions;

using ConfigurationFile = TNS.Ares.Constantes.ConfigurationFile;
using TNS.FrameWorks.LSConnectivity;
using TNS.Alert.Domain;

namespace TNS.Ares.Requester.App
{
    class Program
    {
        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        private const string CONFIGARION_DIRECTORY_NAME = "Configuration";
        #endregion

        static void Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "-p" && args[1].Length > 0) {

                LsClientName lsClientName = (LsClientName)Enum.Parse(typeof(LsClientName), args[1]);

                string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGARION_DIRECTORY_NAME + @"\";

                // Loading Requester Configurations
                RequesterConfigurations.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.REQUESTER_CONFIGURATION_FILENAME));
                RequesterConfiguration currentResquesterConfiguration = RequesterConfigurations.GetAresConfiguration(lsClientName);

                configurationDirectoryRoot = configurationDirectoryRoot + "\\" + currentResquesterConfiguration.DirectoryName + "\\";

                // Creating datasource
                IDataSource src = null;

                if (lsClientName != LsClientName.PixPalaceRequester)
                {
                    // Loading Plugin configuration
                    PluginConfiguration.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.PLUGIN_CONFIGURATION_FILENAME));

                    //Loading Alert Configuration
                    if(File.Exists(configurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.ALERTE_CONFIGURATION))
                        AlertConfiguration.Load(new XmlReaderDataSource(configurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.ALERTE_CONFIGURATION));

                    // Loading DataBase configuration
                    DataBaseConfiguration.Load(new XmlReaderDataSource(configurationDirectoryRoot + ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));

                    // Creating datasource
                    src = (IDataSource)DataBaseConfiguration.DataBase.GetDefaultConnection(PluginConfiguration.DefaultConnectionId);
                }

                SrvShell srv = new SrvShell(lsClientName, currentResquesterConfiguration, src);

                srv.StartMonitorServer(currentResquesterConfiguration.MonitorPort);

                Console.WriteLine(currentResquesterConfiguration.ProductName);
                Console.ReadLine();
                srv.Dispose();
            }
        }
    }
}
