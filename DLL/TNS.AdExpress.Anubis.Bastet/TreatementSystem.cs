#region Informations
///////////////////////////////////////////////////////////
//  TreatementSystem.cs
//  Implementation of the Class TreatementSystem
//  Generated by Enterprise Architect
//  Created on:      17-nov.-2005 16:51:12
//  Original author: D. V. Mussuma
///////////////////////////////////////////////////////////

#endregion


using System;
using System.IO;
using System.Data;
using System.Threading;

using TNS.AdExpress.Anubis.BusinessFacade;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.Common;

using TNS.AdExpress.Anubis.Bastet.Common;
using TNS.AdExpress.Anubis.Bastet.BusinessFacade;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.Common;

using Aspose.Excel;

namespace TNS.AdExpress.Anubis.Bastet {
	/// <summary>
	/// Implementation de TNS.AdExpress.Anubis.BusinessFacade.IPlugin pour Bastet plug-
	/// in
	/// </summary>
	public class TreatementSystem :TNS.AdExpress.Anubis.BusinessFacade.IPlugin{
		
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
		private BastetConfig _bastetConfig;
		/// <summary>
		/// Composant excel
		/// </summary>
		private BastetExcelSystem _excel;
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
			return " Bastet Excel Generator"; // Constante
		}
		#endregion
		
		#region D�marrage et Arr�t de l'�x�cution du thread
		/// <summary>
		///  Lance le traitement du r�sultat
		/// </summary>
		/// <param name="navSessionId"></param>
		/// <param name="confifurationFilePath"></param>
		/// <param name="dataSource">source de donn�es</param>
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
				_bastetConfig=new BastetConfig(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+confifurationFilePath));
			}
			catch(System.Exception err){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
			#endregion

			_dataSource=dataSource;
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name=" Bastet Excel Generator ";
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
		/// Genere  et envoie par mail le fichier excel pour le plug-in Bastet
		/// </summary>
		private void ComputeTreatement(){
			
			try{
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);

				#region Request Details
				DataRow rqDetails = ParameterSystem.GetRequestDetails(_dataSource,_navSessionId).Tables[0].Rows[0];
				#endregion

				#region Excel management
				
				_excel = new BastetExcelSystem(_dataSource,_bastetConfig,rqDetails,(Parameters)Parameters.Load(_navSessionId));
				string fileName = _excel.Init();
				_excel.Fill();				
				ParameterSystem.RegisterFile(_dataSource,_navSessionId,fileName);
				_excel.Send();
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.sent);	
				ParameterSystem.DeleteRequest(_dataSource,_navSessionId);			
				
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId);
			}
			catch(System.Exception err){
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.error);
				OnError(_navSessionId,"Erreur lors du traitement du r�sultat.", err);
				return;
			}
			finally{
				try{
					if(File.Exists(_excel.ExcelFilePath)){
						File.Delete(_excel.ExcelFilePath);
					}
				}
				catch(System.Exception){					
				}
			}


		}
		#endregion

	}//end TreatementSystem

}//end namespace TNS.AdExpress.Anubis.Bastet