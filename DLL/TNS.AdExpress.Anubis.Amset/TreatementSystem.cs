#region Information
//Author : Y. Rkaina
//Creation : 05/02/2007
//Modifications:
#endregion

using System;
using System.Data;
using System.Threading;

//using TNS.AdExpress.Common;

using TNS.AdExpress.Anubis.BusinessFacade;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.Common;

using TNS.AdExpress.Anubis.Amset.Common;
using TNS.AdExpress.Anubis.Amset.BusinessFacade;

using TNS.AdExpress.Web.Core.Sessions;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Theme;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Amset{
	/// <summary>
	/// Implementation of TNS.AdExpress.Anubis.BusinessFacade.IPlugin for Amset plug-in
	/// </summary>
	public class TreatementSystem:TNS.AdExpress.Anubis.BusinessFacade.IPlugin{
	
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
		private AmsetConfig _amsetConfig;
		/// <summary>
		/// Composant excel
		/// </summary>
		private AmsetExcelSystem _excel = null;
        /// <summary>
        /// Theme
        /// </summary>
        private Theme _theme;
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
			return("Amset (Données de cadrage) Excel Generator");
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
				_amsetConfig=new AmsetConfig(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+configurationFilePath));
			}
			catch(System.Exception err){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
            try {
                _theme = new Theme(new XmlReaderDataSource(_amsetConfig.ThemePath + @"\App_Themes\" + WebApplicationParameters.Themes[((WebSession)ParameterSystem.Load(_navSessionId)).SiteLanguage].Name + @"\" + "Styles.xml"));
            }
            catch (System.Exception err) {
                throw new Exception("File of theme not found ! (in Plugin Satet in TreatmentSystem class)");
            }
			#endregion

			_dataSource=dataSource;
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="Amset (Données de cadrage) Excel Generator";
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
		/// Generate the excel file for Amset plug-in
		/// </summary>
		private void ComputeTreatement(){
			
			try{

				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);

				#region Request Details
				DataRow rqDetails = ParameterSystem.GetRequestDetails(_dataSource,_navSessionId).Tables[0].Rows[0];
				#endregion

				#region excel management
				_excel = new AmsetExcelSystem(_dataSource,_amsetConfig,rqDetails,(WebSession)ParameterSystem.Load(_navSessionId),_theme);
				string fileName = _excel.Init();				
				_excel.Fill();
				ParameterSystem.RegisterFile(_dataSource,_navSessionId,fileName);
				_excel.Send();
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.sent);
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId);
			}
			catch(System.Exception err){
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.error);
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
