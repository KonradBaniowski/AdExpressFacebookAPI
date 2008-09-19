#region Information
/*
 * Author : G. Facon, G. Ragneau
 * Creation : 07/2005
 * Modifications:
 *		31/08/2005 : G. Ragneau : load media publications
 *		31/08/2005 : G. Ragneau : add treeatement time information
 *		06/03/2007 : Y. R'kaina : add treatment Send Report
 *		21/03/2007 : Y. R'kaina : add WarinigJob event
 * 
 * */
#endregion

using System;
using System.Collections;
using System.Data;
using System.Threading;

using TNS.AdExpress.Web.BusinessFacade.Selections.Medias;

using AnubisConfiguration=TNS.AdExpress.Anubis.Common.Configuration;
using AnubisConstantes=TNS.AdExpress.Anubis.Constantes;
using TNS.AdExpress.Anubis.Exceptions;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.BusinessFacade;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web.Navigation;

using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Constantes.Web;


namespace TNS.AdExpress.Anubis.BusinessFacade.Core{
	/// <summary>
	/// Distribution des demandes
	/// </summary>
	public class DistributionSystem{
		
		#region Evènement
		/// <summary>
		/// Un job a été Distribué
		/// </summary>
		public delegate void StartJob(Int64 navSessionId,string message);
		/// <summary>
		/// Lancement d'un traitement d'un job
		/// </summary>
		public event StartJob OnStartJob;
		/// <summary>
		/// Un job a été Traité
		/// </summary>
		public delegate void StopJob(Int64 navSessionId, string message);
		/// <summary>
		/// Lancement fin d'un traitement d'un job
		/// </summary>
		public event StopJob OnStopJob;
		/// <summary>
		/// Un job a été Traité
		/// </summary>
		public delegate void ErrorJob(Int64 navSessionId,string message,System.Exception err);
		/// <summary>
		/// Lancement fin d'un traitement d'un job
		/// </summary>
		public event ErrorJob OnErrorJob;
		/// <summary>
		/// Avertissement
		/// </summary>
		public delegate void WarningJob(Int64 navSessionId,string message);
		/// <summary>
		/// Lancement d'un avertissement
		/// </summary>
		public event WarningJob OnWarningJob;
		#endregion

		#region Variables
		/// <summary>
		/// Thread qui traite l'alerte
		/// </summary>
		private Thread _myThread;
		/// <summary>
		/// Indicate if the thread must stop
		/// </summary>
		private bool _stop = false;
		/// <summary>
		/// Data Source relative to configuration
		/// </summary>
		private AnubisConfiguration.AnubisConfig _config;
		/// <summary>
		/// Liste des demandes
		/// </summary>
		private DataTable _requestList;
		/// <summary>
		/// Source de données
		/// </summary>
		private IDataSource _datasource;
		/// <summary>
		/// Liste des plug-ins qui peuvent traiter des résultats
		/// </summary>
		private Hashtable _plugins;
		/// <summary>
		/// Nombre d'ouvrier
		/// </summary>
		private Hashtable _jobsList;
		/// <summary>
		/// Start time for each job
		/// </summary>
		private Hashtable _jobsTime;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="config">Configuration</param>
		/// <param name="plugins">Liste des plug-ins</param>
		/// <param name="requestList">Liste des demandes</param>
		/// <param name="dataSource">Source de données</param>
		public DistributionSystem(AnubisConfiguration.AnubisConfig config,Hashtable plugins,DataTable requestList,IDataSource dataSource){
			if(config==null)throw(new ArgumentNullException("La configuration est null"));
			if(plugins==null)throw(new ArgumentNullException("La liste des plug-ins est null"));
			if(plugins.Count==0)throw(new ArgumentException("La liste des plug-ins est vide"));
			if(requestList==null)throw(new ArgumentNullException("La liste des demandes est null"));
			_requestList=requestList;
			_datasource=dataSource;
			_config=config;
			_plugins=plugins;
			_jobsList=new Hashtable(_config.JobsNumber);
			_jobsTime=new Hashtable(_config.JobsNumber);
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient le nombre de process qui traite une demande
		/// </summary>
		public int RunningJobCount{
			get{
				int number=0;
				lock(_jobsList){
					number=_jobsList.Count;
				}
				return(number);
			}
		}

		/// <summary>
		/// Get the list of running jobs
		/// </summary>
		public string JobList{
			get{
				string jbLst="";
				lock(_jobsList){
					foreach(object obj in _jobsList.Keys){
						jbLst+=obj.ToString()+",";
					}
				}
				return(jbLst);
			}
		}
		#endregion

		#region Lancement du thread
		/// <summary>
		/// Lance le traitement de l'alerte
		/// </summary>
		/// <remarks>
		/// Le traitement se charge de charger les données nécessaires à son execution et de lancer les alertes
		/// </remarks>
		public void Treatement(){
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="RequestsUpdateSystem";
			_myThread.Start();				
		}
		#endregion

		#region Fermeture du thread
		/// <summary>
		/// Arrêt brutal du serveur
		/// </summary>
		public void AbortServer(){
			foreach(IPlugin currentPlugin in _jobsList.Values){
				currentPlugin.AbortTreatement();
			}
			_myThread.Abort();
		}

		/// <summary>
		/// Signal to server to finish pending jobs and to stop
		/// </summary>
		public void StopServer(){
			_config.JobsNumber = 0;
			_stop = true;
			if(_myThread.ThreadState == ThreadState.WaitSleepJoin)
				_myThread.Interrupt();
		}
		#endregion

		#region Traitment de la distribution
		/// <summary>
		/// Traitement
		/// </summary>
		private void ComputeTreatement(){
			Int64 navSessionId=0;
			IPlugin plugin;

			MediaPublicationDatesSystem.Init();
 
			#region Initialisations des Listes 
			// Initialisation des descriptions des éléments de niveaux de détail
			DetailLevelItemsInformation.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+@"Configuration\"+TNS.AdExpress.Constantes.Web.ConfigurationFile.GENERIC_DETAIL_LEVEL_ITEMS_CONFIGURATION_FILENAME)); 
			// Initialisation des descriptions des niveaux de détail
            DetailLevelsInformation.Init(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+@"Configuration\"+TNS.AdExpress.Constantes.Web.ConfigurationFile.GENERIC_DETAIL_LEVEL_CONFIGURATION_FILENAME)); 				
			// Chargement des noms de modules
            //ModulesList.Init(AppDomain.CurrentDomain.BaseDirectory+@"Configuration\"+TNS.AdExpress.Constantes.Web.ConfigurationFile.MODULE_CONFIGURATION_FILENAME,AppDomain.CurrentDomain.BaseDirectory+@"Configuration\"+TNS.AdExpress.Constantes.Web.ConfigurationFile.MODULE_CATEGORY_CONFIGURATION_FILENAME);
            //Charge les niveaux d'univers
            UniverseLevels.getInstance(new TNS.FrameWork.DB.Common.XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory +@"Configuration\"+ TNS.AdExpress.Constantes.Web.ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_FILENAME));

            //Charge les styles personnalisés des niveaux d'univers
            UniverseLevelsCustomStyles.getInstance(new TNS.FrameWork.DB.Common.XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory +@"Configuration\"+ TNS.AdExpress.Constantes.Web.ConfigurationFile.UNIVERSE_LEVELS_CUSTOM_STYLES_CONFIGURATION_FILENAME));

            //Charge la hierachie de niveau d'univers
            UniverseBranches.getInstance(new TNS.FrameWork.DB.Common.XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory +@"Configuration\"+ TNS.AdExpress.Constantes.Web.ConfigurationFile.UNIVERSE_BRANCHES_CONFIGURATION_FILENAME));

            // Initialisation des listes de texte
            TNS.AdExpress.AdExpressWordListLoader.LoadLists();

            Product.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.BAAL_CONFIGURATION_FILENAME));
            Media.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.BAAL_CONFIGURATION_FILENAME));

            //Units
            UnitsInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.UNITS_CONFIGURATION_FILENAME));

            //Vehicles
            VehiclesInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.VEHICLES_CONFIGURATION_FILENAME));

            //Load flag list
            TNS.AdExpress.Domain.AllowedFlags.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.FLAGS_CONFIGURATION_FILENAME));
            #endregion
  
			while(_myThread.IsAlive && !(_stop && this.RunningJobCount<=0))
			{
				lock(_requestList){
					if(_requestList.Rows.Count>0 && _jobsList.Count<_config.JobsNumber){
						navSessionId=(Int64)_requestList.Rows[0]["ID_STATIC_NAV_SESSION"];
						int currentType=int.Parse(_requestList.Rows[0]["ID_PDF_RESULT_TYPE"].ToString());
						plugin = (IPlugin)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap( ((AnubisConfiguration.Plugin)_plugins[currentType]).AssemblyName ,((AnubisConfiguration.Plugin)_plugins[currentType]).Class);
						plugin.OnStartWork+=new StartWork(plugin_OnStartWork);
						plugin.OnStopWorkerJob+=new StopWorkerJob(plugin_OnStopWorkerJob);
						plugin.OnMessageAlert+=new MessageAlert(plugin_OnMessageAlert);
						plugin.OnError+=new Error(plugin_OnError);
						plugin.OnSendReport+=new SendReport(plugin_OnSendReport);
						plugin.Treatement(((AnubisConfiguration.Plugin)_plugins[currentType]).ConfigurationFilePath,_datasource,navSessionId);
						_jobsList.Add(navSessionId,plugin);
						_jobsTime.Add(navSessionId,DateTime.Now);
						ParameterSystem.ChangeStatus(_datasource,navSessionId,AnubisConstantes.Result.status.processing);
						_requestList.Rows.RemoveAt(0);
					}

				}
				try{
					if(!(_stop && this.RunningJobCount<=0))
						_myThread.Join(_config.DistributionInterval);
				}
				catch(ThreadInterruptedException){}
			}
		}
		#endregion

		#region Evènements des plug-ins
		/// <summary>
		/// Un traitement de résultat c'est lancé
		/// </summary>
		/// <param name="navSessionId">Session statique</param>
		/// <param name="message">Message de lancement</param>
		private void plugin_OnStartWork(Int64 navSessionId,string message) {
			OnStartJob(navSessionId,message);

		}

		/// <summary>
		/// Un traitement de résultat c'est terminé
		/// </summary>
		/// <param name="navSessionId">Résultat traité</param>
		/// <param name="resultFilePath">Chemin du gichier de résultat</param>
		/// <param name="mail">Mail d'envoi</param>
		/// <param name="evtMessage">Message de l'evènement</param>
		private void plugin_OnStopWorkerJob(Int64 navSessionId, string resultFilePath, string mail, string evtMessage) {
			//ParameterSystem.ChangeStatus(_datasource,navSessionId,AnubisConstantes.Result.status.done);
			_jobsList.Remove(navSessionId);
			TimeSpan duration = DateTime.Now.Subtract((DateTime)_jobsTime[navSessionId]);
			_jobsTime.Remove(navSessionId);
			OnStopJob(navSessionId,evtMessage + " ( + " + duration.Minutes + "m " + duration.Seconds + "s)");
			if(_myThread.ThreadState == ThreadState.WaitSleepJoin)
				_myThread.Interrupt();
		}

		/// <summary>
		/// Erreur pendant l'execution d'un job
		/// </summary>
		/// <param name="navSessionId">Identifiant du résultat</param>
		/// <param name="message">Message</param>
		/// <param name="e">Exception</param>
		private void plugin_OnError(Int64 navSessionId, string message, System.Exception e) {
			_jobsList.Remove(navSessionId);
			OnErrorJob(navSessionId,message,e);
		}
		
		/// <summary>
		/// Message provenant d'un plug-in
		/// </summary>
		/// <param name="navSessionId">Identifiant du résultat</param>
		/// <param name="message">Message</param>
		private void plugin_OnMessageAlert(Int64 navSessionId,string message) {
		}

		/// <summary>
		/// Envoie d'un rapport
		/// </summary>
		/// <param name="reportTitle">Titre du rapport</param>
		/// <param name="duration">Temps d'exécution</param>
		/// <param name="endExecutionDateTime">Heure de fin</param>
		/// <param name="reportCore">Le corps du rapport</param>
		/// <param name="mailList">La liste des mails</param>
		/// <param name="errorList">la liste des erreurs</param>
		/// <param name="from">l'expediteur du mail</param>
		/// <param name="mailServer">le serveur du mail</param>
		/// <param name="mailPort">le port utilisé pour l'envoie de mail</param>
		/// <param name="navSessionId">Id session</param>
		private void plugin_OnSendReport(string reportTitle, TimeSpan duration, DateTime endExecutionDateTime, string reportCore, ArrayList mailList, ArrayList errorList,string from, string mailServer, int mailPort, Int64 navSessionId){

			ReportingSystem reportingSystem = new ReportingSystem(reportTitle,duration,endExecutionDateTime,reportCore,mailList,errorList,from,mailServer,mailPort,navSessionId);
			try{
				reportingSystem.SendReport(reportingSystem.SetReport());
			}
			catch(System.Exception ex){
				OnWarningJob(navSessionId,ex.Message);
			}
		
		}
		#endregion

	}
}
