#region Information
//Author : Y. Rkaina 
//Creation : 11/07/2006
#endregion

using System;
using System.Data;
using System.Threading;

using TNS.AdExpress.Anubis.Hotep.Common;
using TNS.AdExpress.Anubis.Hotep.BusinessFacade;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using PDFCreatorPilotLib;
using TNS.AdExpress.Domain.Web;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.Ares;
using TNS.AdExpress.Domain.Layers;
using TNS.Ares.StaticNavSession.DAL;
using System.Reflection;
using TNS.AdExpress.Anubis.Hotep.Exceptions;
using System.IO;
using TNS.Ares.Domain.LS;

namespace TNS.AdExpress.Anubis.Hotep{
	/// <summary>
	/// Implementation of TNS.AdExpress.Anubis.BusinessFacade.IPlugin for Hotep plug-in
	/// </summary>
	public class TreatementSystem:IPlugin{

		#region Evènements
		/// <summary>
		/// Lancement du module
		/// </summary>
		public event StartWork OnStartWork;
		/// <summary>
		/// Arrêt du module
		/// </summary>
		public event StopWorkerJob OnStopWorkerJob;
		/// <summary>
		/// Message d'une alerte
		/// </summary>
		public event MessageAlert OnMessageAlert;
		/// <summary>
		/// Message d'une alerte
		/// </summary>
		public event Error OnError;
		/// <summary>
		/// Envoie des rapports
		/// </summary>
		public event SendReport OnSendReport;
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
		private HotepConfig _hotepConfig;
        /// <summary>
        /// Theme
        /// </summary>
        private TNS.FrameWork.WebTheme.Theme _theme;
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
		public TreatementSystem(){
		}
		#endregion

		#region Nom du Plug-in
		/// <summary>
		/// Obtient le nom du plug-in
		/// </summary>
		/// <returns>Le nom du plug-in</returns>
		public string GetPluginName(){
			return("Hotep (Indicateurs) PDF Generator");
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
                    throw new HotepPdfInitializationException("Impossible to Create Instance Of Layer IStaticNavSessionDAL ", e);
                }
                #endregion

                #region Check Path File
                if (configurationFilePath == null) {
                    throw new HotepPdfInitializationException("Impossible de lancer le traitement d'un job", new ArgumentNullException("Le nom du fichier de configuration est null."));
                }
                if (configurationFilePath.Length == 0) {
                    throw new HotepPdfInitializationException("Impossible de lancer le traitement d'un job", new ArgumentException("Le nom du fichier de configuration est vide."));
                }
                #endregion

                #region Initialize Hotep
                try {
                    _hotepConfig = new HotepConfig(new XmlReaderDataSource(configurationFilePath));
                }
                catch (System.Exception err) {
                    throw new HotepPdfInitializationException("Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration", err);
                }
                #endregion

                #region Initialize WebSession
                try {
                    _webSession = ((WebSession)_dataAccess.LoadData(_navSessionId));
                }
                catch (System.Exception err) {
                    throw new HotepPdfInitializationException("Error for load session", err);
                }
                #endregion

                #region Initialize Theme
                string pathFileTheme = string.Empty;
                try {

                    if (File.Exists(_hotepConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml")) {
                        pathFileTheme = _hotepConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + _hotepConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml")) {
                        pathFileTheme = AppDomain.CurrentDomain.BaseDirectory + _hotepConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    else {
                        pathFileTheme = _hotepConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    _theme = new TNS.FrameWork.WebTheme.Theme(new XmlReaderDataSource(pathFileTheme));
                }
                catch (System.Exception err) {
                    throw new HotepPdfInitializationException(string.Format("File of theme not found ! (in Plugin Hotep in TreatmentSystem class) - Path : '{0}'", pathFileTheme), err);
                }
                #endregion

                #endregion

            }
            catch (Exception e) {
                if(_dataAccess!=null)
                    _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
                OnError(_navSessionId, "Impossible to initialize process ", e);
                return;
            }
			_dataSource=dataSource;
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="Hotep (Indicateurs) PDF Generator";
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
		/// Generate the PDF for Hotep plug-in
		/// </summary>
		private void ComputeTreatement(){
			HotepPdfSystem pdf = null;
			try{
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);

				#region Request Details
                DataRow rqDetails = _dataAccess.GetRow(_navSessionId);
				#endregion

				#region PDF management

                pdf = new HotepPdfSystem(_dataSource, _hotepConfig, rqDetails, _webSession, _theme);
				string fileName = pdf.Init();
				pdf.AutoLaunch = false;
				//TODO update Database for physical file name
				pdf.Fill();
				pdf.EndDoc();
                _dataAccess.RegisterFile(_navSessionId, fileName);
				pdf.Send(fileName);
                _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.sent.GetHashCode());

                PluginInformation pluginInformation = PluginConfiguration.GetPluginInformation(PluginType.Hotep);
                if (pluginInformation != null && pluginInformation.DeleteRowSuccess)
                    _dataAccess.DeleteRow(_navSessionId);
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId);
			}
			catch(System.Exception err){
                if (_dataAccess != null)
                    _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
				OnError(_navSessionId,"Erreur lors du traitement du résultat.", err);
				return;
			}
		}
		#endregion
	}
}
