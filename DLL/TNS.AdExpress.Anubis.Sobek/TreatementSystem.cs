#region Information
//Author : D. V. Mussuma, Y. Rkaina
//Creation : 24/05/2006
//Modifications:
#endregion

using System;
using System.Data;
using System.Threading;

using TNS.AdExpress.Anubis.Sobek.Common;
using TNS.AdExpress.Anubis.Sobek.BusinessFacade;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.Ares;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;
using TNS.Ares.StaticNavSession.DAL;
using System.Reflection;


namespace TNS.AdExpress.Anubis.Sobek
{
	/// <summary>
	/// Implementation of TNS.AdExpress.Anubis.BusinessFacade.IPlugin for Sobek plug-in
	/// </summary>
	public class TreatementSystem:IPlugin{
		
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
		private SobekConfig _sobekConfig;
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
			return("SOBEK CSV Generator");
		
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
				_sobekConfig=new SobekConfig(new XmlReaderDataSource(confifurationFilePath));
			}
			catch(System.Exception err){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
			#endregion

			_dataSource=dataSource;
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="SOBEK CSV Generator";
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
		/// Generate the TXT for SOBEK plug-in
		/// </summary>
		private void ComputeTreatement(){
			SobekTextFileSystem csv = null;
			try{
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);

				#region Request Details
				DataRow rqDetails = _dataAccess.GetRow(_navSessionId);
				#endregion

				#region csv management
				
				csv = new SobekTextFileSystem(_dataSource,_sobekConfig,rqDetails,(WebSession)_dataAccess.LoadData(_navSessionId));
				string fileName = csv.Init();
				//TODO update Database for physical file name
				csv.Fill();
				_dataAccess.RegisterFile(_navSessionId,fileName);
				csv.Send();
				_dataAccess.UpdateStatus(_navSessionId,TNS.Ares.Constantes.Constantes.Result.status.sent.GetHashCode());
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId);
			}
			catch(System.Exception err){
				_dataAccess.UpdateStatus(_navSessionId,TNS.Ares.Constantes.Constantes.Result.status.error.GetHashCode());
				OnError(_navSessionId,"Erreur lors du traitement du r�sultat.", err);
				return;
			}
			finally{
//				try{
//					//Functions.CleanWorkDirectory(txt.GetWorkDirectory());
//				}
//				catch(System.Exception e){
//					int i = 0;
//				}
			}

		}
		#endregion

	}
}
