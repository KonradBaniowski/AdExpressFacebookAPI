#region Information
//Author : Y. Rkaina
//Creation : 10/08/2006
#endregion

using System;
using System.Collections;
using System.Data;
using System.Threading;

using TNS.AdExpress.Anubis.Miysis.Common;
using TNS.AdExpress.Anubis.Miysis.BusinessFacade;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using CstWeb = TNS.AdExpress.Constantes.Web;

using TNS.Ares;
using TNS.Ares.StaticNavSession.DAL;
using TNS.AdExpress.Domain.Layers;
using System.Reflection;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Anubis.Miysis.Exceptions;
using System.IO;
using TNS.Ares.Domain.LS;

namespace TNS.AdExpress.Anubis.Miysis
{
	/// <summary>
	/// Description résumée de TreatementSystem.
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
        public void OnStartWork(Int64 navSessionId, string message) {
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
		private Thread _myThread;
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
		private MiysisConfig _miysisConfig;
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

		#region Nom du Plug-in
		/// <summary>
		/// Obtient le nom du plug-in
		/// </summary>
		/// <returns>Le nom du plug-in</returns>
		public string GetPluginName(){
			return("Miysis PDF Generator");
		
		}
		#endregion

		#region Server start and stop
		/// <summary>
		/// Lance le traitement du résultat
		/// </summary>
        /// <param name="configurationFilePath">Chemin de configuration du plug-in</param>
		/// <param name="dataSource">Source de données pour charger la session</param>
		/// <param name="navSessionId">Identifiant de la session à traiter</param>
		/// <remarks>
		/// Le traitement se charge de charger les données nécessaires à son execution et de lancer le résultat
		/// </remarks>
		public void Treatement(string configurationFilePath,IDataSource dataSource,Int64 navSessionId){

            try {
                _navSessionId = navSessionId;

                #region Initialization

                #region Create Instance of _webSession
                try {
                    object[] parameter = new object[1];
                    parameter[0] = dataSource;
                    CoreLayer cl = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.dataAccess];
                    _dataAccess = (IStaticNavSessionDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameter, null, null, null);
                }
                catch (Exception e) {
                    _dataAccess = null;
                    throw new MiysisPdfException("Impossible to Create Instance Of Layer IStaticNavSessionDAL ", e);
                }
                #endregion

                #region Check Path File
                if (configurationFilePath == null) {
                    throw new MiysisPdfException("Impossible de lancer le traitement d'un job", new ArgumentNullException("configurationFilePath"));
                }
                if (configurationFilePath.Length == 0) {
                    throw new MiysisPdfException("Impossible de lancer le traitement d'un job", new ArgumentException("Le nom du fichier de configuration est vide."));
                }
                #endregion

                #region Initialize Hotep
                try {
                    _miysisConfig = new MiysisConfig(new XmlReaderDataSource(configurationFilePath));
                }
                catch (Exception err) {
                    throw new MiysisPdfException("Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration", err);
                }
                #endregion

                #region Initialize WebSession
                try {
                    _webSession = ((WebSession)_dataAccess.LoadData(_navSessionId));
                }
                catch (Exception err) {
                    throw new MiysisPdfException("Error for load session", err);
                }
                #endregion

                #region Initialize Theme
                string pathFileTheme = string.Empty;
                try {

                    if (File.Exists(_miysisConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml")) {
                        pathFileTheme = _miysisConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + _miysisConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml")) {
                        pathFileTheme = AppDomain.CurrentDomain.BaseDirectory + _miysisConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    else {
                        pathFileTheme = _miysisConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    _theme = new Theme(new XmlReaderDataSource(pathFileTheme));
                }
                catch (Exception err) {
                    throw new MiysisPdfException(string.Format("File of theme not found ! (in Plugin Hotep in TreatmentSystem class) - Path : '{0}'", pathFileTheme), err);
                }
                #endregion

                #endregion

            }
            catch (Exception e) {
                if (_dataAccess != null) {
                    try {
                        _dataAccess.UpdateStatus(_navSessionId, Ares.Constantes.Constantes.Result.status.error.GetHashCode());
                    }
                    catch { }
                }
                OnError(_navSessionId, "Impossible to initialize process ", e);
                return;
            }

            _dataSource = dataSource;
			
			ThreadStart myThreadStart = ComputeTreatement;
			_myThread=new Thread(myThreadStart);
			_myThread.Name="Miysis PDF Generator";
			_myThread.Start();
		}

		/// <summary>
		/// Arrête le traitement du résultat
		/// </summary>
		public void AbortTreatement(){
            if(_myThread!= null && _myThread.ThreadState == ThreadState.Running)
			    _myThread.Abort();
		}
		#endregion

		#region Traitment du résultat
		/// <summary>
		/// Generate the PDF for Miysis plug-in
		/// </summary>
		private void ComputeTreatement(){
			MiysisPdfSystem pdf;
			try{
				OnStartWork(_navSessionId,GetPluginName()+" started for "+_navSessionId);

				#region Request Details
                DataRow rqDetails = _dataAccess.GetRow(_navSessionId);
				#endregion

				#region PDF management
				
				pdf = new MiysisPdfSystem(_dataSource, _miysisConfig,rqDetails,_webSession,_theme);
				string fileName = pdf.Init(); 
				pdf.AutoLaunch = false;
				//TODO update Database for physical file name
				pdf.Fill();
				pdf.EndDoc();
                _dataAccess.RegisterFile(_navSessionId, fileName);
				pdf.Send(fileName);
                _dataAccess.UpdateStatus(_navSessionId, Ares.Constantes.Constantes.Result.status.sent.GetHashCode());

                PluginInformation pluginInformation = PluginConfiguration.GetPluginInformation(PluginType.Miysis);
                if (pluginInformation != null && pluginInformation.DeleteRowSuccess)
                    _dataAccess.DeleteRow(_navSessionId);
				#endregion

                OnStopWorkerJob(_navSessionId,"","",GetPluginName()+" finished for '"+_navSessionId+"'");
			}
			catch(Exception err){
                if(_dataAccess != null)
                     _dataAccess.UpdateStatus(_navSessionId, Ares.Constantes.Constantes.Result.status.error.GetHashCode()); 
				OnError(_navSessionId,"Erreur lors du traitement du résultat for '"+_navSessionId+"'.", err);
				return;
			}
		}
		#endregion

    }
}

