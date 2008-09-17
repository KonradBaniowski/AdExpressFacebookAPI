#region Information
//Author : G. FACON, G. RAGNEAU
//Creation : ?
//Modifications:
//		17/08/2005 : G. RAGNEAU - CompûteTreatment
#endregion

using System;
using System.Data;
using System.Threading;

using TNS.AdExpress.Common;

using TNS.AdExpress.Anubis.BusinessFacade;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.Common;

using TNS.AdExpress.Anubis.Appm.Common;
using TNS.AdExpress.Anubis.Appm.BusinessFacade;

using TNS.AdExpress.Web.Core.Sessions;

using TNS.FrameWork.DB.Common;

using PDFCreatorPilot2;

namespace TNS.AdExpress.Anubis.Appm{
	/// <summary>
	/// Implementation of TNS.AdExpress.Anubis.BusinessFacade.IPlugin for APPM plug-in
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
		private AppmConfig _appmConfig;
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
				_appmConfig=new AppmConfig(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+confifurationFilePath));
			}
			catch(System.Exception err){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
			#endregion

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
				DataRow rqDetails = ParameterSystem.GetRequestDetails(_dataSource,_navSessionId).Tables[0].Rows[0];
				#endregion

				#region PDF management
				
				pdf = new AppmPdfSystem(_dataSource, _appmConfig,rqDetails,(WebSession)ParameterSystem.Load(_navSessionId));
				string fileName = pdf.Init();
				pdf.AutoLaunch = false;
				//TODO update Database for physical file name
				pdf.Fill();
				pdf.EndDoc();
				ParameterSystem.RegisterFile(_dataSource,_navSessionId,fileName);
				pdf.Send(fileName);
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
				try{
					Functions.CleanWorkDirectory(pdf.GetWorkDirectory());
				}
				catch(System.Exception e){
					int i = 0;
				}
			}

		}
		#endregion

	}
}
