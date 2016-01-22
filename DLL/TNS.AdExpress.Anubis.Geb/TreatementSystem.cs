#region Informations
// Auteur : B.Masson
// Date de création : 20/04/2006
// Date de modification :
#endregion

using System;
using System.IO;
using System.Data;
using System.Threading;
using TNS.AdExpress.Anubis.BusinessFacade;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.Common;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Anubis.Geb.Common;
using TNS.AdExpress.Anubis.Geb.BusinessFacade;
using TNS.AdExpress.Geb.Configuration;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Geb;
using Aspose.Cells;

namespace TNS.AdExpress.Anubis.Geb{
	/// <summary>
	/// Implementation de TNS.AdExpress.Anubis.BusinessFacade.IPlugin pour Geb plugin
	/// </summary>
	public class TreatementSystem : TNS.AdExpress.Anubis.BusinessFacade.IPlugin{
		
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
		private GebConfig _gebConfig;
		/// <summary>
		/// Composant excel
		/// </summary>
		private GebExcelSystem _excel;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public TreatementSystem(){

		}
		#endregion

		#region Destructeur
		/// <summary>
		/// Destructeur
		/// </summary>
		~TreatementSystem(){

		}
		#endregion

		#region Dispose
		/// <summary>
		/// Nettoyage
		/// </summary>
		public virtual void Dispose(){

		}		
		#endregion

		#region Nom du Plug-in
		/// <summary>
		/// Obtient le nom du plug-in
		/// </summary>
		/// <returns>Le nom du plug-in</returns>
		public string GetPluginName(){
			return "Geb Excel Generator";
		}
		#endregion

		#region Démarrage et Arrêt de l'éxécution du thread
		/// <summary>
		///  Lance le traitement du résultat
		/// </summary>
		/// <param name="navSessionId"></param>
		/// <param name="confifurationFilePath"></param>
		/// <param name="dataSource">source de données</param>
		public void Treatement(string confifurationFilePath,IDataSource dataSource,Int64 navSessionId){
			_navSessionId=navSessionId;
	
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
				_gebConfig=new GebConfig(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+confifurationFilePath));
			}
			catch(System.Exception err){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
			#endregion

			_dataSource=dataSource;
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="Geb Excel Generator";
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
		/// Genere  et envoie par mail le fichier excel pour le plug-in Geb
		/// </summary>
		private void ComputeTreatement(){
			AlertRequest currentAlertParameters = null;
			string fileName = null;
			try{
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);

				#region Request Details
				DataRow rqDetails = ParameterSystem.GetRequestDetails(_dataSource,_navSessionId).Tables[0].Rows[0];
				#endregion

				#region Chargement des paramètres du blob de l'alerte
				currentAlertParameters = (AlertRequest)AlertRequest.Load(_navSessionId);

//				// Controle des paramètres
//				long a		= currentAlertParameters.AlertId;
//				long b		= currentAlertParameters.AlertTypeId;
//				string c	= currentAlertParameters.DateMediaNum;
				#endregion

				#region Chargement des paramètres de l'alerte
                Alert alertConfiguration=null;
                alertConfiguration = new Alert(_dataSource,currentAlertParameters.AlertId);
                
				#endregion

				#region Excel management
				_excel = new GebExcelSystem(_dataSource, _gebConfig, rqDetails, currentAlertParameters, alertConfiguration);
				fileName = _excel.Init();
				_excel.Fill();				
				ParameterSystem.RegisterFile(_dataSource,_navSessionId,fileName);
				_excel.Send();
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.sent);	
				ParameterSystem.DeleteRequest(_dataSource,_navSessionId);
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId+" > Alert : "+alertConfiguration.AlertName.ToString()+" ("+alertConfiguration.AlertId.ToString()+") / Media : "+alertConfiguration.MediaName.ToString()+" ("+alertConfiguration.MediaId.ToString()+")");
			}
			catch(System.Exception err){
				// Erreur dans le traitement, on change l'id_statut dans static_nav_session
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.error);
				OnError(_navSessionId,"Erreur lors du traitement du résultat.", err);
				return;
			}
			finally{
				try{
					if(File.Exists(_excel.ExcelFilePath))
						File.Delete(_excel.ExcelFilePath);
				}
				catch(System.Exception){}
			}
		}
		#endregion

	}
}
