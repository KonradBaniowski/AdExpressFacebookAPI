#region Information
//Author : G. FACON, G. RAGNEAU
//Creation : ?
//Modifications:
//		17/08/2005 : G. RAGNEAU - CompûteTreatment
#endregion

using System;
using System.Data;
using System.Threading;

//using TNS.AdExpress.Common;

using TNS.AdExpress.Anubis.Appm.Common;
using TNS.AdExpress.Anubis.Appm.BusinessFacade;

using TNS.AdExpress.Web.Core.Sessions;

using TNS.FrameWork.DB.Common;

using PDFCreatorPilotLib;
using TNS.AdExpress.Domain.Web;
using CstWeb = TNS.AdExpress.Constantes.Web;

using TNS.Ares;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Anubis.Appm.Exceptions;
using TNS.AdExpress.Domain.Layers;
using TNS.Ares.StaticNavSession.DAL;
using System.IO;
using TNS.Ares.Domain.LS;
using System.Reflection;
using System.Collections;

namespace TNS.AdExpress.Anubis.Appm{
	/// <summary>
	/// Implementation of TNS.AdExpress.Anubis.BusinessFacade.IPlugin for APPM plug-in
	/// </summary>
	public class TreatementSystem:IPlugin{

        #region Evènements
        /// <summary>
        /// Lancement du module
        /// </summary>
        public event StartWork EvtStartWork;
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
        public event StopWorkerJob EvtStopWorkerJob;
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
        public event MessageAlert EvtMessageAlert;
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
        public event Error EvtError;
        /// <summary>
        /// Message d'une alerte
        /// </summary>
        public void OnError(Int64 navSessionId, string message, System.Exception e)
        {
            if (EvtError != null) EvtError(navSessionId, message, e);
        }
        /// <summary>
        /// Envoie des rapports
        /// </summary>
        public event SendReport EvtSendReport;
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
		private AppmConfig _appmConfig;
        /// <summary>
        /// Theme
        /// </summary>
        private Theme _theme;
        /// <summary>
        /// Data Access Layer
        /// </summary>
        private IStaticNavSessionDAL _dataAccess;
        /// <summary>
        /// WebSession
        /// </summary>
        private WebSession _webSession;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public TreatementSystem(){
		}
		#endregion

		#region Nom du Plug-in
		/// <summary>
		/// Obtient le nom du plug-in
		/// </summary>
		/// <returns>Le nom du plug-in</returns>
		public string GetPluginName(){
			return("APPM PDF Generator");
		
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
        public void Treatement(string configurationFilePath, IDataSource dataSource, Int64 navSessionId) {
            try {
                _navSessionId = navSessionId;

                #region Initialization

                #region Create Instance of _webSession
                try {
                    object[] parameter = new object[1];
                    parameter[0] = dataSource;
                    CoreLayer cl = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.dataAccess];
                    _dataAccess = (IStaticNavSessionDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameter, null, null);
                }
                catch (Exception e) {
                    throw new AppmPdfException("Impossible to Create Instance Of Layer IStaticNavSessionDAL ", e);
                }
                #endregion

                #region Check Path File
                if (configurationFilePath == null) {
                    throw new AppmPdfException("Impossible de lancer le traitement d'un job", new ArgumentNullException("Le nom du fichier de configuration est null."));
                }
                if (configurationFilePath.Length == 0) {
                    throw new AppmPdfException("Impossible de lancer le traitement d'un job", new ArgumentException("Le nom du fichier de configuration est vide."));
                }
                #endregion

                #region Initialize Hotep
                try {
                    _appmConfig = new AppmConfig(new XmlReaderDataSource(configurationFilePath));
                }
                catch (System.Exception err) {
                    throw new AppmPdfException("Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration", err);
                }
                #endregion

                #region Initialize WebSession
                try {
                    _webSession = ((WebSession)_dataAccess.LoadData(_navSessionId));
                }
                catch (System.Exception err) {
                    throw new AppmPdfException("Error for load session", err);
                }
                #endregion

                #region Initialize Theme
                string pathFileTheme = string.Empty;
                try {

                    if (File.Exists(_appmConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml")) {
                        pathFileTheme = _appmConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + _appmConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml")) {
                        pathFileTheme = AppDomain.CurrentDomain.BaseDirectory + _appmConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    else {
                        pathFileTheme = _appmConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    _theme = new TNS.FrameWork.WebTheme.Theme(new XmlReaderDataSource(pathFileTheme));
                }
                catch (System.Exception err) {
                    throw new AppmPdfException(string.Format("File of theme not found ! (in Plugin Hotep in TreatmentSystem class) - Path : '{0}'", pathFileTheme), err);
                }
                #endregion

                #endregion

            }
            catch (Exception e) {
                if (_dataAccess != null)
                    _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
                OnError(_navSessionId, "Impossible to initialize process ", e);
                return;
            }

			_dataSource=dataSource;
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="APPM PDF Generator";
			_myThread.Start();
		}

		/// <summary>
		/// Arrête le traitement du résultat
		/// </summary>
		public void AbortTreatement(){
				_myThread.Abort();
		}
		#endregion

		#region Traitment du résultat
		/// <summary>
		/// Generate the PDF for APPM plug-in
		/// </summary>
		private void ComputeTreatement(){
			AppmPdfSystem pdf = null;
			try{
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);

                #region Request Details
                DataRow rqDetails = _dataAccess.GetRow(_navSessionId);
                #endregion

				#region PDF management

                pdf = new AppmPdfSystem(_dataSource, _appmConfig, rqDetails, _webSession, _theme);
				string fileName = pdf.Init();
				pdf.AutoLaunch = false;
				//TODO update Database for physical file name
				pdf.Fill();
				pdf.EndDoc();
                _dataAccess.RegisterFile(_navSessionId, fileName);
				pdf.Send(fileName);
                _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.sent.GetHashCode());

                PluginInformation pluginInformation = PluginConfiguration.GetPluginInformation(PluginType.Miysis);
                if (pluginInformation != null && pluginInformation.DeleteRowSuccess)
                    _dataAccess.DeleteRow(_navSessionId);
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId);
			}
			catch(System.Exception err){
                _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
				OnError(_navSessionId,"Erreur lors du traitement du résultat.", err);
				return;
			}
		}
		#endregion

    }
}
