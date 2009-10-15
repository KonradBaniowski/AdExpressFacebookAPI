#region Information
//Author : Y. Rkaina
//Creation : 10/08/2006
#endregion

using System;
using System.Data;
using System.Threading;

using TNS.AdExpress.Anubis.Miysis.Common;
using TNS.AdExpress.Anubis.Miysis.BusinessFacade;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core;

using TNS.FrameWork.DB.Common;

using PDFCreatorPilotLib;
using TNS.AdExpress.Domain.Theme;
using TNS.AdExpress.Domain.Web;
using CstWeb = TNS.AdExpress.Constantes.Web;

using TNS.Ares;
using TNS.Ares.StaticNavSession.DAL;
using TNS.AdExpress.Domain.Layers;
using System.Reflection;

namespace TNS.AdExpress.Anubis.Miysis
{
	/// <summary>
	/// Description résumée de TreatementSystem.
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
		private MiysisConfig _miysisConfig;
        /// <summary>
        /// Theme
        /// </summary>
        private Theme _theme;
        /// <summary>
        /// Data Access Layer
        /// </summary>
        private IStaticNavSessionDAL _dataAccess;
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
			return("Miysis PDF Generator");
		
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

			_navSessionId=navSessionId;

            object[] parameter = new object[1];
            parameter[0] = dataSource;
            CoreLayer cl = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.dataAccess];
            _dataAccess = (IStaticNavSessionDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameter, null, null, null);

			#region Chargement du fichier de configuration
			if(configurationFilePath==null){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job", new ArgumentNullException("Le nom du fichier de configuration est null."));
				return;
			}
			if(configurationFilePath.Length==0){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job", new ArgumentException("Le nom du fichier de configuration est vide."));
				return;
			}
			try{
				_miysisConfig=new MiysisConfig(new XmlReaderDataSource(configurationFilePath));
			}
			catch(System.Exception err){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
            try {
                _theme = new Theme(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory + _miysisConfig.ThemePath + @"\App_Themes\" + WebApplicationParameters.Themes[((WebSession)_dataAccess.LoadData(_navSessionId)).SiteLanguage].Name + @"\" + "Styles.xml"));
            }
            catch (System.Exception err) {
                OnError(_navSessionId, "File of theme not found ! (in Plugin APPM in TreatmentSystem class)",err);
            }
			#endregion

			#region
//			// Initialisation des descriptions des éléments de niveaux de détail
//			DetailLevelItemsInformation.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.GENERIC_DETAIL_LEVEL_ITEMS_CONFIGURATION_PATH)); 
//			// Initialisation des descriptions des niveaux de détail
//			DetailLevelsInformation.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.GENERIC_DETAIL_LEVEL_CONFIGURATION_PATH)); 				
//			// Chargement des noms de modules
//			ModulesList.Init(TNS.AdExpress.Constantes.Web.ConfigurationFile.MODULE_CONFIGURATION_PATH);
			#endregion

			_dataSource=dataSource;
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="Miysis PDF Generator";
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
		/// Generate the PDF for Miysis plug-in
		/// </summary>
		private void ComputeTreatement(){
			MiysisPdfSystem pdf = null;
			try{
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);

				#region Request Details
                WebSession webSession = (WebSession)_dataAccess.LoadData(_navSessionId);
                //webSession.CustomerLogin.Connection = new Oracle.DataAccess.Client.OracleConnection(webSession.CustomerLogin.OracleConnectionString);
                DataRow rqDetails = _dataAccess.GetRow(_navSessionId);
				#endregion

				#region PDF management
				
				pdf = new MiysisPdfSystem(_dataSource, _miysisConfig,rqDetails,webSession,_theme);
				string fileName = pdf.Init(); 
				pdf.AutoLaunch = false;
				//TODO update Database for physical file name
				pdf.Fill();
				pdf.EndDoc();
                _dataAccess.RegisterFile(_navSessionId, fileName);
				pdf.Send(fileName);
                _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.sent.GetHashCode());
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId);
			}
			catch(System.Exception err){
                _dataAccess.UpdateStatus(_navSessionId, TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
				OnError(_navSessionId,"Erreur lors du traitement du résultat.", err);
				return;
			}
			finally{
				try{
                    TNS.Ares.Functions.CleanWorkDirectory(pdf.GetWorkDirectory());
				}
				catch(System.Exception e){
					int i = 0;
				}
			}
		}
		#endregion


	}
}

