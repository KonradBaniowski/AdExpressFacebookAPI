#region Information
//Author : D. V. Mussuma, Y. Rkaina
//Creation : 29/05/2006
//Modifications:
#endregion

using System;
using System.Data;
using System.Threading;

using TNS.AdExpress.Anubis.Satet.Common;
using TNS.AdExpress.Anubis.Satet.BusinessFacade;

using TNS.AdExpress.Web.Core.Sessions;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Theme;
using TNS.AdExpress.Domain.Web;
using TNS.Ares;
using TNS.Ares.StaticNavSession.DAL;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Layers;
using System.Reflection;

namespace TNS.AdExpress.Anubis.Satet {
	/// <summary>
	/// Implementation of TNS.AdExpress.Anubis.BusinessFacade.IPlugin for Sobek plug-in
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
		private SatetConfig _satetConfig;
		/// <summary>
		/// Composant excel
		/// </summary>
		private SatetExcelSystem _excel = null;
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
			return("Satet (APPM) Excel Generator");
		
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
		public void Treatement(string confifurationFilePath,IDataSource dataSource,Int64 navSessionId){
			_navSessionId=navSessionId;

            object[] parameter = new object[1];
            parameter[0] = dataSource;
            CoreLayer cl = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.dataAccess];
            _dataAccess = (IStaticNavSessionDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameter, null, null, null);

			#region Chargement du fichier de configuration
			if(confifurationFilePath==null){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job", new ArgumentNullException("Le nom du fichier de configuration est null."));
				return;
			}
			if(confifurationFilePath.Length==0){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job", new ArgumentException("Le nom du fichier de configuration est vide."));
				return;
			}
			try{
				_satetConfig=new SatetConfig(new XmlReaderDataSource(confifurationFilePath));
			}
			catch(System.Exception err){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
            try {
                _theme = new Theme(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory + _satetConfig.ThemePath + @"\App_Themes\" + WebApplicationParameters.Themes[((WebSession)_dataAccess.LoadData(_navSessionId)).SiteLanguage].Name + @"\" + "Styles.xml"));
            }
            catch (System.Exception err) {
                OnError(_navSessionId, "File of theme not found ! (in Plugin Satet in TreatmentSystem class)",err);
            }
			#endregion

			_dataSource=dataSource;
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="Satet (APPM) Excel Generator";
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
		/// Generate the excel file for Satet plug-in
		/// </summary>
		private void ComputeTreatement(){
			
			try{
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);

				#region Request Details
                DataRow rqDetails = _dataAccess.GetRow(_navSessionId);
				#endregion

				#region excel management

                _excel = new SatetExcelSystem(_dataSource, _satetConfig, rqDetails, (WebSession)_dataAccess.LoadData(_navSessionId), _theme);
				string fileName = _excel.Init();				
				_excel.Fill();
                _dataAccess.RegisterFile(_navSessionId, fileName);
				_excel.Send();
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
				_excel = null;
			}

		}
		#endregion

	}
}
