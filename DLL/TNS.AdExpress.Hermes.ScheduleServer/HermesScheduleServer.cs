#region Informations
// Auteur : B.Masson
// Date de création : 12/02/2007
// Date de modification :
#endregion

#region Namespaces
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Threading;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Hermes.ScheduleServer.DataAccess;
using HermesConfiguration=TNS.AdExpress.Hermes.ScheduleServer.Configuration;
using HermesException=TNS.AdExpress.Hermes.ScheduleServer.Exceptions;
#endregion

namespace TNS.AdExpress.Hermes.ScheduleServer{
	/// <summary>
	/// Classe Hermes Schedule Server
	/// </summary>
	public class HermesScheduleServer{

		#region Constantes
		/// <summary>
		/// Chemin du répertoire du fichier contentant la dernière règle ajoutée dans la table static_nav_session
		/// </summary>
		private const string RULES_PATH=@"Rules\";
		/// <summary>
		/// Nom du fichier stockant la dernière règle ajoutée à static_nav_session
		/// </summary>
		private const string FILE_NAME="RulesList.txt";
		#endregion

		#region Déclaration des évènements
		/// <summary>
		/// Lancement du serveur
		/// </summary>
		public delegate void StartServer(string message);
		/// <summary>
		/// Arrêt du serveur Geb
		/// </summary>
		public delegate void StopServer(string message);
		/// <summary>
		/// Nouvelle règle
		/// </summary>
		public delegate void NewRule(string message);
		/// <summary>
		/// Message
		/// </summary>
		public delegate void Message(string message);
		/// <summary>
		/// Message d'erreur
		/// </summary>
		public delegate void ErrorMessage(string message);
		/// <summary>
		/// Message d'erreur avec Exception
		/// </summary>
		public delegate void Error(string message,System.Exception err);
		#endregion

		#region Evènements
		/// <summary>
		/// Lancement du serveur
		/// </summary>
		public event StartServer OnStartServer;
		/// <summary>
		/// Arrêt du serveur
		/// </summary>
		public event StopServer OnStopServer;
		/// <summary>
		/// Nouvelle règle
		/// </summary>
		public event NewRule OnNewRule;
		/// <summary>
		/// Message
		/// </summary>
		public event Message OnMessage;
		/// <summary>
		/// Message d'erreur
		/// </summary>
		public event ErrorMessage OnErrorMessage;
		/// <summary>
		/// Message d'erreur
		/// </summary>
		public event Error OnError;
		#endregion

		#region Variables
		/// <summary>
		/// Répertoire de l'application
		/// </summary>
		protected string _appPath = AppDomain.CurrentDomain.BaseDirectory;
		/// <summary>
		/// Configuration de l'application
		/// </summary>
		protected HermesConfiguration.Application _applicationConfiguration = null;
		/// <summary>
		/// Thread
		/// </summary>
		private System.Threading.Thread _myThread;
		/// <summary>
		/// Indique que Geb Server doit être stopper
		/// </summary>
		private bool _stop = false;
		/// <summary>
		/// Source de données
		/// </summary>
		protected TNS.FrameWork.DB.Common.IDataSource _source = null;
		/// <summary>
		/// Nombre d'ouvrier
		/// </summary>
		private Hashtable _jobsList;
		/// <summary>
		/// List of rules
		/// </summary>
		private Hashtable _rulesList;
		/// <summary>
		/// Schedule
		/// </summary>
		private TNS.AdExpress.Hermes.Schedule _schedule;
		/// <summary>
		/// Hashtable contenant les règles déjà traitées
		/// </summary>
		private LastTaskTreated _treatedRulesOFDay;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="source">DataSource</param>
		/// <param name="applicationConfiguration">Application Configuration</param>
		public HermesScheduleServer(IDataSource source, HermesConfiguration.Application applicationConfiguration){
			_source = source;
			_applicationConfiguration = applicationConfiguration;
			_jobsList=new Hashtable(1);
			if(!Directory.Exists(_appPath+RULES_PATH))
				Directory.CreateDirectory(_appPath+RULES_PATH);
		}
		#endregion

		#region Lancement de la thread
		/// <summary>
		/// Traitement pour le lancement de la thread
		/// </summary>
		public void Treatement(){
			_myThread=new Thread(new ThreadStart(Compute));
			_myThread.Name="Hermes Schedule Server";
			_myThread.Start();
		}
		#endregion

		#region Fermeture de la thread
		/// <summary>
		/// Arrêt brutal de Hermes Schedule Server
		/// </summary>
		public void AbortHermesServer(){
			_myThread.Abort();
		}

		/// <summary>
		/// Signal to server to finish pending jobs and to stop
		/// </summary>
		public void StopHermesServer(){
			_stop = true;
			if(_myThread.ThreadState == ThreadState.WaitSleepJoin)
				_myThread.Interrupt();
		}
		#endregion

		#region Accesseur
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
		#endregion

		#region Execution du traitement
		/// <summary>
		/// Execution du traitement
		/// </summary>
		public void Compute(){

			#region Variables
			DateTime dtStartTreatement;
			DateTime dtEndTreatement;
			ArrayList rulesListOfCurrentDay;
			TNS.AdExpress.Hermes.ScheduledRule scheduledRule;
			bool error=false;
			#endregion

			#region Chargement des rules et du Schedule
			// Rules
			_rulesList=RulesDataAccess.Load(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+@"Configuration\RulesConfiguration.xml"));

			// Schedule
			_schedule = ScheduleDataAccess.Load(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+@"Configuration\ScheduleConfiguration.xml"),_rulesList);
			#endregion

			#region Chargement des règles déjà traitées
			_treatedRulesOFDay = new LastTaskTreated(_appPath+RULES_PATH+FILE_NAME);
			#endregion

			#region Traitement
			OnStartServer(DateTime.Now.ToString());
			while(_myThread.IsAlive && !_stop){
				dtStartTreatement = DateTime.Now; // Date de début de traitement
				try{

					#region Règles de la journée
					rulesListOfCurrentDay = _schedule[dtStartTreatement.DayOfWeek];
					#endregion

					if(rulesListOfCurrentDay!=null){

						// Parcours des règles de la journée
						for(int i=0; i<rulesListOfCurrentDay.Count;i++){
							scheduledRule = (TNS.AdExpress.Hermes.ScheduledRule)rulesListOfCurrentDay[i];
						
							// Comparaison heure courante et heure lancement d'une règle
							if(scheduledRule.LaunchHour.Hour <= DateTime.Now.Hour && scheduledRule.LaunchHour.Minute <= DateTime.Now.Minute){

								// Conditions pour enregistrer une règle dans STATIC_NAV_SESSION : Si la key(ruleId) n'est pas dans hashtable OU bien la key(ruleId) est dans hashtable ET QUE la date enregistrée est différente de la date du jour
								if(!_treatedRulesOFDay.ContainsKey(scheduledRule.Id) || 
									( _treatedRulesOFDay.ContainsKey(scheduledRule.Id) && (_treatedRulesOFDay[scheduledRule.Id]).ToString()!=DateTime.Now.ToString("yyyyMMdd") ) ){

									#region Enregistrement des règles
									// Parcours des jours pour une règle (Exemple day:-4,-5,etc.)
									for(int j=0;j<scheduledRule.DaysOfWeek.Count;j++){
										_jobsList.Add(scheduledRule.Id,scheduledRule.DaysOfWeek[j]);
										try{
											// Création de la règle dans la table "static_nav_session" (BLOB)
											RuleParameters parameters = new RuleParameters(scheduledRule,DateTime.Now.AddDays(int.Parse(scheduledRule.DaysOfWeek[j].ToString())));
											parameters.Save(_source);
											error=false;
											OnNewRule("Created Rule ("+scheduledRule.Id+") - Day : "+scheduledRule.DaysOfWeek[j].ToString()+" - "+scheduledRule.TableName.ToUpper()+" -> "+DateTime.Now.ToString());
										}
										catch(System.Exception ex){
											error=true;
											OnError("Error in the creation of the request for rule ("+scheduledRule.Id+") in the table",ex);
										}
										_jobsList.Remove(scheduledRule.Id);
									}
									#endregion
				
									#region Stockage de la dernière règle traitée
									if(!error)
										_treatedRulesOFDay[scheduledRule.Id]=DateTime.Now.ToString("yyyyMMdd");
									#endregion

								}
							}
						}

					}
				}
				catch(HermesException.HermesScheduleServerException erHss){
					OnError("Error",erHss);
				}
				catch(System.Exception err){
					OnErrorMessage("General error : "+err.Message);
				}

				#region Mise en suspend de la thread jusqu'au prochain refresh
				dtEndTreatement = DateTime.Now; // Date de fin de traitement

				// Mise en suspend de la thread jusqu'au prochain refresh
				System.Threading.Thread.Sleep(GetNextRefresh(dtStartTreatement,dtEndTreatement));
				#endregion
			}
			OnStopServer(DateTime.Now.ToString());
			#endregion

		}
		#endregion

		#region Méthodes privées
		/// <summary>
		/// Calcule le temps restant pour le prochain refresh
		/// </summary>
		/// <param name="dtStartTreatement">DateTime du début du traitement</param>
		/// <param name="dtEndTreatement">DateTime de la fin du traitement</param>
		/// <returns>Temps pour le prochain refresh (en milliseconde)</returns>
		private int GetNextRefresh(DateTime dtStartTreatement,DateTime dtEndTreatement){
			int newRefresh = 0;
			int intervalTime = 0;
			DateTime dtNow = DateTime.Now;

			// Calcul de l'interval de temps entre le refresh initial et le temps du traitement
			intervalTime = _applicationConfiguration.Refresh - (GetMilliseconds(dtEndTreatement) - GetMilliseconds(dtStartTreatement));

			if(intervalTime > 0){
				// Ex : intervalTime = 15 - (10) > prochain refresh dans 5 min
				newRefresh = intervalTime;
			}
			else if(intervalTime < 0){
				// Ex : intervalTime = 15 - (20) > on dépasse, on retire la difference au prochain refresh :
				// refresh initial (15 min) - intervalTime (5 min) > prochain refresh dans 10 min
				newRefresh = _applicationConfiguration.Refresh + intervalTime;
			}
			else{
				newRefresh = _applicationConfiguration.Refresh;
			}

//			if(dtNow.Hour >= 7 && dtNow.Hour <= 20){
//				// Calcul de l'interval de temps entre le refresh initial et le temps du traitement
//				intervalTime = _applicationConfiguration.Refresh - (GetMilliseconds(dtEndTreatement) - GetMilliseconds(dtStartTreatement));
//
//				if(intervalTime > 0){
//					// Ex : intervalTime = 15 - (10) > prochain refresh dans 5 min
//					newRefresh = intervalTime;
//				}
//				else if(intervalTime < 0){
//					// Ex : intervalTime = 15 - (20) > on dépasse, on retire la difference au prochain refresh :
//					// refresh initial (15 min) - intervalTime (5 min) > prochain refresh dans 10 min
//					newRefresh = _applicationConfiguration.Refresh + intervalTime;
//				}
//				else{
//					newRefresh = _applicationConfiguration.Refresh;
//				}
//			}
//			else{
//				// Calcul du DateTime.Now en milliseconde
//				int dateNowInMMS = GetMilliseconds(dtNow);
//				// Calcul de la différence de temps entre 24H (86400000 ms) et le DateTime.Now
//				int diffrenceUntilMidnight = 86400000 - dateNowInMMS;
//				// Prochain refresh fixé à 7h00
//				newRefresh = diffrenceUntilMidnight + 25200000;
//			}
			return(newRefresh);
		}

		/// <summary>
		/// Converti un DateTime en milliseconde
		/// </summary>
		/// <param name="dt">DateTime</param>
		/// <returns>Conversion du DateTime en milliseconde</returns>
		private int GetMilliseconds(DateTime dt){
			int valueConverted=0;
			valueConverted += dt.Hour * 3600000;
			valueConverted += dt.Minute * 60000;
			valueConverted += dt.Second * 1000;
			return(valueConverted);
		}
		#endregion

	}
}
