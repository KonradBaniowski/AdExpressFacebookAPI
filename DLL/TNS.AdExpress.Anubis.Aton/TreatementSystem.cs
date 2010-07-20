#region Information
//Author : Y. R'kaina
//Creation : 09/02/2007
//Modifications:
#endregion

using System;
using System.Data;
using System.Threading;

using TNS.Ares;
using TNS.Ares.StaticNavSession.DAL;
using TNS.AdExpress.Anubis.Aton.Common;
using TNS.AdExpress.Anubis.Aton.BusinessFacade;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using PDFCreatorPilotLib;
using TNS.AdExpress.Domain.Web;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Layers;
using System.Reflection;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Anubis.Aton.Exceptions;
using System.IO;
using TNS.Ares.Domain.LS;

namespace TNS.AdExpress.Anubis.Aton{
	/// <summary>
	/// Implementation of TNS.AdExpress.Anubis.BusinessFacade.IPlugin for Aton plug-in
	/// </summary>
	public class TreatementSystem : IPlugin{

		#region Ev�nements
		/// <summary>
		/// Lancement du module
		/// </summary>
		public event StartWork OnStartWork;
		/// <summary>
		/// Arr�t du module
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
		/// Identifiant du r�sultat � traiter
		/// </summary>
		private Int64 _navSessionId;
		/// <summary>
		/// Source de donn�es pour charger la session du r�sultat
		/// </summary>
		private IDataSource _dataSource;
		/// <summary>
		/// Configuration du plug-in
		/// </summary>
		private AtonConfig _atonConfig;
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
			return("Aton PDF Generator");
		}
		#endregion

		#region Server start and stop
		/// <summary>
		/// Lance le traitement du r�sultat
		/// </summary>
		/// <param name="confifurationFilePath">Chemin de configuration du plug-in</param>
		/// <param name="dataSource">Source de donn�es pour charger la session</param>
		/// <param name="navSessionId">Identifiant de la session � traiter</param>
		/// <remarks>
		/// Le traitement se charge de charger les donn�es n�cessaires � son execution et de lancer le r�sultat
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
                    throw new AtonPdfException("Impossible to Create Instance Of Layer IStaticNavSessionDAL ", e);
                }
                #endregion

                #region Check Path File
                if (configurationFilePath == null) {
                    throw new AtonPdfException("Impossible de lancer le traitement d'un job", new ArgumentNullException("Le nom du fichier de configuration est null."));
                }
                if (configurationFilePath.Length == 0) {
                    throw new AtonPdfException("Impossible de lancer le traitement d'un job", new ArgumentException("Le nom du fichier de configuration est vide."));
                }
                #endregion

                #region Initialize Hotep
                try {
                    _atonConfig = new AtonConfig(new XmlReaderDataSource(configurationFilePath));
                }
                catch (System.Exception err) {
                    throw new AtonPdfException("Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration", err);
                }
                #endregion

                #region Initialize WebSession
                try {
                    _webSession = ((WebSession)_dataAccess.LoadData(_navSessionId));
                }
                catch (System.Exception err) {
                    throw new AtonPdfException("Error for load session", err);
                }
                #endregion

                #region Initialize Theme
                string pathFileTheme = string.Empty;
                try {

                    if (File.Exists(_atonConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml")) {
                        pathFileTheme = _atonConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + _atonConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml")) {
                        pathFileTheme = AppDomain.CurrentDomain.BaseDirectory + _atonConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    else {
                        pathFileTheme = _atonConfig.ThemePath + @"\" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + @"\" + "Styles.xml";
                    }
                    _theme = new TNS.FrameWork.WebTheme.Theme(new XmlReaderDataSource(pathFileTheme));
                }
                catch (System.Exception err) {
                    throw new AtonPdfException(string.Format("File of theme not found ! (in Plugin Hotep in TreatmentSystem class) - Path : '{0}'", pathFileTheme), err);
                }
                #endregion

                #endregion

            }
            catch (Exception e) {
                if (_dataAccess!=null) _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
                OnError(_navSessionId, "Impossible to initialize process ", e);
                return;
            }

			_dataSource=dataSource;
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="Aton PDF Generator";
			_myThread.Start();
		}

		/// <summary>
		/// Arr�te le traitement du r�sultat
		/// </summary>
		public void AbortTreatement(){
			_myThread.Abort();
		}
		#endregion

		#region Traitment du r�sultat
		/// <summary>
		/// Generate the PDF for Aton plug-in
		/// </summary>
		private void ComputeTreatement(){
			AtonPdfSystem pdf = null;
			try{
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);

				#region Request Details
                DataRow rqDetails = _dataAccess.GetRow(_navSessionId);
				#endregion

				#region PDF management
                pdf = new AtonPdfSystem(_dataSource, _atonConfig, rqDetails, _webSession, _theme);
				string fileName = pdf.Init();
				pdf.AutoLaunch = false;
				//TODO update Database for physical file name
				pdf.Fill();
				pdf.EndDoc();
                _dataAccess.RegisterFile(_navSessionId, fileName);
				pdf.Send(fileName);
                _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.sent.GetHashCode());

                PluginInformation pluginInformation = PluginConfiguration.GetPluginInformation(PluginType.Aton);
                if (pluginInformation != null && pluginInformation.DeleteRowSuccess)
                    _dataAccess.DeleteRow(_navSessionId);
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId);
			}
			catch(System.Exception err){
                if (_dataAccess != null)
                    _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
				OnError(_navSessionId,"Erreur lors du traitement du r�sultat.", err);
				return;
			}
		}
		#endregion

	}
}
