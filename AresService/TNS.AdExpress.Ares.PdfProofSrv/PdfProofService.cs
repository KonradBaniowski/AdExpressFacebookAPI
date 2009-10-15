using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;

using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.DataBaseDescription;
using System.Threading;
using TNS.AdExpress.Ares.PdfProof;

using TNS.Ares.Constantes;
using TNS.Ares.Domain.Classification;

namespace TNS.AdExpress.Ares.PdfProofSrv
{
    public partial class PdfProofService : ServiceBase
    {
        private ShouShell _shell;

        public PdfProofService()
        {
            InitializeComponent();
        }

        protected void InitService()
        {
            #region Loading application parameters

            // Initialisation des listes de texte
            AdExpressWordListLoader.LoadLists();

            Product.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.BAAL_CONFIGURATION_FILENAME));
            Media.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.BAAL_CONFIGURATION_FILENAME));

            // Units
            UnitsInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNITS_CONFIGURATION_FILENAME));

            // Vehicles
            VehiclesInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.VEHICLES_CONFIGURATION_FILENAME));

            // Universes
            UniverseLevels.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_FILENAME));

            // Charge les styles personnalisés des niveaux d'univers
            UniverseLevelsCustomStyles.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CUSTOM_STYLES_CONFIGURATION_FILENAME));

            // Charge la hierachie de niveau d'univers
            UniverseBranches.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_BRANCHES_CONFIGURATION_FILENAME));

            // Charge les niveaux d'univers
            UniverseLevels.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_FILENAME));

            //Load flag list
            TNS.AdExpress.Domain.AllowedFlags.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.FLAGS_CONFIGURATION_FILENAME));

            #endregion

            IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.webAdministration);
            LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(WebApplicationParameters.ConfigurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));

            // Loading DataBase configuration
            DataBaseConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));

            string confFilePath = WebApplicationParameters.ConfigurationDirectoryRoot + lsClientConfiguration.DirectoryName;

            ShouShell _shell = new ShouShell(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, confFilePath, source);
            _shell.StartMonitorServer(lsClientConfiguration.MonitorPort);
        }

        protected override void OnStart(string[] args)
        {
            new Thread(new ThreadStart(this.InitService)).Start();
        }

        protected override void OnStop()
        {
            _shell.Dispose();
        }
    }
}
