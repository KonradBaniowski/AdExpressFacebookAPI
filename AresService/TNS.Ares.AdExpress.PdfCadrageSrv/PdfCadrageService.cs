using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.Ares.Domain.LS;
using TNS.Classification.Universe;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Constantes.Web;
using TNS.Ares.AdExpress.PdfCadrage;
using TNS.AdExpress;
using TNS.Ares.Domain.DataBase;

namespace TNS.Ares.AdExpress.PdfCadrageSrv
{
    public partial class PdfCadrageService : ServiceBase {
        
        #region Constante
        /// <summary>
        /// Directory configuratyion Name
        /// </summary>
        private const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion

        #region Variables
        /// <summary>
        /// Current Thread.
        /// </summary>
        private Thread _currentThread = null;
        /// <summary>
        /// Current Shell.
        /// </summary>
        private AtonShell _currentShell;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PdfCadrageService()
        {
            InitializeComponent();
        }
        #endregion

        #region Run
        /// <summary>
        /// Run
        /// </summary>
        public void Run()
        {
            try {
                string configurationDirectoryRoot = AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_DIRECTORY_NAME + @"\";
                LsClientConfiguration lsClientConfiguration = TNS.Ares.Domain.XmlLoader.LsClientConfigurationXL.Load(new XmlReaderDataSource(configurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.LS_CLIENT_CONFIGURATION_FILENAME));


                _currentShell = new AtonShell(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, lsClientConfiguration.ModuleDescriptionList, lsClientConfiguration.DirectoryName);
                _currentShell.StartMonitorServer(lsClientConfiguration.MonitorPort);

                Thread.Sleep(Timeout.Infinite);
                _currentShell.Dispose();
            }
            catch (Exception e) {
                if(_currentShell!=null)
                    _currentShell.Dispose();
            }
        }
        #endregion

        #region OnStart
        /// <summary>
        /// OnStart
        /// </summary>
        /// <param name="args">Argument</param>
        protected override void OnStart(string[] args)
        {
            _currentThread = new System.Threading.Thread(this.Run);
            _currentThread.Start();
        }
        #endregion

        #region OnStop
        /// <summary>
        /// OnStop
        /// </summary>
        protected override void OnStop()
        {
            if (_currentShell != null) {
                _currentShell.Dispose();
                _currentShell = null;
            }

            _currentThread.Abort();
        }
        #endregion
    }
}
