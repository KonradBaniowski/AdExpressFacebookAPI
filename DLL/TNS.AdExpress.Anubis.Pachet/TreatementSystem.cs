using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Anubis.Pachet.Common;
using TNS.AdExpress.Anubis.Pachet.Exceptions;
using TNS.FrameWork.DB.Common;
using System.Threading;
using TNS.AdExpress.Anubis.Pachet.BusinessFacade;
using System.Data;
using TNS.Ares;
using System.Collections;
using System.Reflection;
using TNS.Ares.StaticNavSession.DAL;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Anubis.Pachet
{
    /// <summary>
	/// Implementation of TNS.AdExpress.Anubis.BusinessFacade.IPlugin for Pachet plug-in
	/// </summary>
	public class TreatementSystem:IPlugin{

        #region IPlugin Membres
        /// <summary>
        /// Lancement du module
        /// </summary>
        public event StartWork EvtStartWork;
        /// <summary>
        /// Arrêt du module
        /// </summary>
        public event StopWorkerJob EvtStopWorkerJob;
        /// <summary>
        /// Arrêt du module
        /// </summary>
        public event MessageAlert EvtMessageAlert;
        /// <summary>
        /// Message d'une alerte
        /// </summary>
        public event Error EvtError;
        /// <summary>
        /// Envoie des rapports
        /// </summary>
        public event SendReport EvtSendReport;

        #endregion

        #region Evènements
        /// <summary>
        /// Lancement du module
        /// </summary>
        public void OnStartWork(Int64 navSessionId, string message)
        {
            if (EvtStartWork != null) EvtStartWork(navSessionId, message);
        }
        /// <summary>
        /// Arrêt du module
        /// </summary>
        public void OnStopWorkerJob(Int64 navSessionId, string resultFilePath, string mail, string evtMessage)
        {
            if (EvtStopWorkerJob != null) EvtStopWorkerJob(navSessionId, resultFilePath, mail, evtMessage);
        }
        /// <summary>
        /// Message d'une alerte
        /// </summary>
        public void OnMessageAlert(Int64 navSessionId, string message)
        {
            if (EvtMessageAlert != null) EvtMessageAlert(navSessionId, message);
        }

        /// <summary>
        /// Message d'une alerte
        /// </summary>
        public void OnError(Int64 navSessionId, string message, Exception e)
        {
            if (EvtError != null) EvtError(navSessionId, message, e);
        }

        /// <summary>
        /// Envoie des rapports
        /// </summary>
        public void OnSendReport(string reportTitle, TimeSpan duration, DateTime endExecutionDateTime, string reportCore, ArrayList mailList, ArrayList errorList, string from, string mailServer, int mailPort, Int64 navSessionId)
        {
            if (EvtSendReport != null) EvtSendReport(reportTitle, duration, endExecutionDateTime, reportCore, mailList, errorList, from, mailServer, mailPort, navSessionId);
        }

        #endregion


        #region Variables
        /// <summary>
        /// Thread qui traite l'alerte
        /// </summary>
        private System.Threading.Thread _myThread;
        /// <summary>
        /// Identifiant du résultat à traiter
        /// </summary>
        private Int64 _navSessionId;
        /// <summary>
        /// Source de données pour charger la session du résultat
        /// </summary>
        private IDataSource _dataSource;
        /// <summary>
        /// Configuration du plug-in
        /// </summary>
        private PachetConfig _pachetConfig;
        /// <summary>
        /// Data Access Layer
        /// </summary>
        private IStaticNavSessionDAL _dataAccess;
        /// <summary>
        /// WebSession
        /// </summary>
        private WebSession _webSession = null;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public TreatementSystem()
        {
        }
        #endregion

        #region Nom du Plug-in
        /// <summary>
        /// Obtient le nom du plug-in
        /// </summary>
        /// <returns>Le nom du plug-in</returns>
        public string GetPluginName()
        {
            return ("PACHET FILE TEXT Generator");

        }
        #endregion

        #region Server start and stop
        /// <summary>
        /// Lance le traitement du résultat
        /// </summary>
        /// <param name="confifurationFilePath">Chemin de configuration du plug-in</param>
        /// <param name="dataSource">Source de données pour charger la session</param>
        /// <param name="navSessionId">Identifiant de la session à traiter</param>
        /// <remarks>
        /// Le traitement se charge de charger les données nécessaires à son execution et de lancer le résultat
        /// </remarks>
        public void Treatement(string configurationFilePath, IDataSource dataSource, Int64 navSessionId)
        {

            try
            {
                _navSessionId = navSessionId;

                #region Initialization

                #region Create Instance of IStaticNavSessionDAL
                try
                {
                    object[] parameter = new object[1];
                    parameter[0] = dataSource;
                    CoreLayer cl = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.dataAccess];
                    _dataAccess = (IStaticNavSessionDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameter, null, null, null);
                }
                catch (Exception e)
                {
                    throw new PachetTextFileSystemException("Impossible to Create Instance Of Layer IStaticNavSessionDAL ", e);
                }
                #endregion

                #region Check Path File
                if (configurationFilePath == null)
                {
                    throw new PachetTextFileSystemException("Impossible de lancer le traitement d'un job", new ArgumentNullException("Le nom du fichier de configuration est null."));
                }
                if (configurationFilePath.Length == 0)
                {
                    throw new PachetTextFileSystemException("Impossible de lancer le traitement d'un job", new ArgumentException("Le nom du fichier de configuration est vide."));
                }
                #endregion

                #region Initialize Pachet
                try
                {
                    _pachetConfig = new PachetConfig(new XmlReaderDataSource(configurationFilePath));
                }
                catch (System.Exception err)
                {
                    throw new PachetTextFileSystemException("Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration", err);
                }
                #endregion

                #region Initialize WebSession
                try
                {
                    _webSession = ((WebSession)_dataAccess.LoadData(_navSessionId));
                }
                catch (System.Exception err)
                {
                    throw new PachetTextFileSystemException("Error for load session", err);
                }
                #endregion

                #endregion

            }
            catch (Exception e)
            {
                if (_dataAccess != null) _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
                OnError(_navSessionId, "Impossible to initialize process ", e);
                return;
            }
            _dataSource = dataSource;

            ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
            _myThread = new Thread(myThreadStart);
            _myThread.Name = "Pachet file text Generator";
            _myThread.Start();
        }

        /// <summary>
        /// Arrête le traitement du résultat
        /// </summary>
        public void AbortTreatement()
        {
            _myThread.Abort();
        }
        #endregion

        #region Traitment du résultat
        /// <summary>
        /// Generate the TXT for SOBEK plug-in
        /// </summary>
        private void ComputeTreatement()
        {
            PachetTextFileSystem csv = null;
            try
            {
                OnStartWork(_navSessionId, this.GetPluginName() + " started for " + _navSessionId);

                #region Request Details
                DataRow rqDetails = _dataAccess.GetRow(_navSessionId);
                #endregion

                #region csv management

                csv = new PachetTextFileSystem(_dataSource, _pachetConfig, rqDetails, _webSession);
                string fileName = csv.Init();
                //TODO update Database for physical file name
                bool res = csv.Fill();
                _dataAccess.RegisterFile(_navSessionId, fileName);
                csv.Send();
                _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.sent.GetHashCode());

                PluginInformation pluginInformation = PluginConfiguration.GetPluginInformation(PluginType.Pachet);
                if (pluginInformation != null && pluginInformation.DeleteRowSuccess)
                    _dataAccess.DeleteRow(_navSessionId);
                #endregion

                OnStopWorkerJob(_navSessionId, "", "", this.GetPluginName() + " finished for " + _navSessionId);
            }
            catch (System.Exception err)
            {
                if (_dataAccess != null) _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
                OnError(_navSessionId, "Erreur lors du traitement du résultat.", err);
                return;
            }
            finally
            {
                //				try{
                //					//Functions.CleanWorkDirectory(txt.GetWorkDirectory());
                //				}
                //				catch(System.Exception e){
                //					int i = 0;
                //				}
            }

        }
        #endregion
    }
}
