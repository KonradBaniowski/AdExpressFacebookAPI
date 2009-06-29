#region Informations
// Auteur : B.Masson
// Date de création : 15/02/2007
// Date de modification :
#endregion

#region Namespaces
using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Text;
using System.Threading;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Anubis.BusinessFacade;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.Common;
using TNS.FrameWork.DB.Common;
using FrameworkDBBusiness = TNS.FrameWork.DB.BusinessFacade;
using Hermes=TNS.AdExpress.Hermes;
using ClassificationDA=TNS.AdExpress.DataAccess.Classification;
#endregion

namespace TNS.AdExpress.Anubis.HermesRadioTv{
	/// <summary>
	/// Implementation de TNS.AdExpress.Anubis.BusinessFacade.IPlugin pour HermesRadioTv plugin
	/// </summary>
	public class TreatementSystem : TNS.AdExpress.Anubis.BusinessFacade.ReportingPluginBase,TNS.AdExpress.Anubis.BusinessFacade.IPlugin{

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
		/// Configuration du plugin Hermes Radio TV
		/// </summary>
		private TNS.AdExpress.Anubis.HermesRadioTv.Common.Configuration _hermesConfig;
		/// <summary>
		/// Paramètres d'une règle
		/// </summary>
		private Hermes.RuleParameters _ruleParameters = null;
		/// <summary>
		/// Nombre de support traité
		/// </summary>
		private int _nbMediaTreated=0;
		/// <summary>
		/// Nombre de support traité dont le résultat est 0 dans DATA_RADIO ou DATA_TV
		/// </summary>
		private int _nbMediaTreatedKO=0;
		/// <summary>
		/// Liste des supports traités dont le résultat est 0 dans DATA_RADIO ou DATA_TV
		/// </summary>
		private string _mediaTreatedKOList="";
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
			return "Hermes RadioTv Indicator";
		}
		#endregion

		#region Démarrage et Arrêt de l'éxécution du thread
		/// <summary>
		///  Lance le traitement du résultat
		/// </summary>
		/// <param name="navSessionId">Identifiant Id_static_nav_session</param>
		/// <param name="confifurationFilePath">Chemin du fihier de configuration</param>
		/// <param name="dataSource">Source de données</param>
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
				// Chargement de la connexion (utilisateur particulier pour Hermes)
				TNS.FrameWork.DB.Common.Oracle.DataBaseConfiguration connection = FrameworkDBBusiness.Oracle.DataBaseConfigurationBussinessFacade.GetOne(AppDomain.CurrentDomain.BaseDirectory+confifurationFilePath);
				_dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(connection.ConnectionString);

				// Chargement des paramètres d'email pour l'envoi du rapport (server, port, from...)
				_hermesConfig=new TNS.AdExpress.Anubis.HermesRadioTv.Common.Configuration(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+confifurationFilePath));
				
				// Chargement de la configuration du rapport (title, mails...)
				base.LoadReportingConfig(_hermesConfig.ConfigurationReportFilePath);
			}
			catch(System.Exception err){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
			#endregion

			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="Hermes RadioTv Indicator";
			_myThread.Start();
		}

		/// <summary>
		/// Arrête le traitement du résultat
		/// </summary>
		public void AbortTreatement(){
			_myThread.Abort();
		}
		#endregion

		#region Traitement du résultat
		/// <summary>
		/// Traitement des vérifications des mises à jour des transferts de la radio tv sur la base AdExpress
		/// </summary>
		private void ComputeTreatement(){
			
			#region Variables
			bool error = false;
			#endregion

			try{
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);
				_endExecutionDateTime = DateTime.Now;

				#region Request Details
				DataRow rqDetails = ParameterSystem.GetRequestDetails(_dataSource,_navSessionId).Tables[0].Rows[0];
				#endregion

				#region Chargement des paramètres de la règle (blob)
				_ruleParameters = (Hermes.RuleParameters)Hermes.RuleParameters.Load(_navSessionId);
				#endregion

				#region Vérification du transfert des données de chaque support de la Radio TV
				// Statut : En cours de traitement
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.processing);

				// Calcul du nombre de lignes de DATA_RADIO ou DATA_TV (+ Chargement des liste des supports de BAAL)
				DataSet ds = Hermes.DataAccess.RuleDataAccess.GetDataControl(_dataSource,_ruleParameters);
				
				// Enregistrement du nombre de lignes dans la table DATA_CONTROL
				foreach(DataRow currentRow in ds.Tables[0].Rows){
					// Compteurs du nombre de support et du nombre de support à 0
					_nbMediaTreated++;
					if(Int64.Parse(currentRow["nbLine"].ToString())==0){
						_nbMediaTreatedKO++;
						_mediaTreatedKOList+=currentRow["id_media"].ToString()+",";
					}

					try{
						Hermes.DataAccess.RuleDataAccess.SetDataControl(_dataSource,Int64.Parse(currentRow["id_media"].ToString()),Int64.Parse(currentRow["date_media_num"].ToString()),Int64.Parse(currentRow["nbLine"].ToString()),_ruleParameters.DiffusionId);
					}
					catch(TNS.AdExpress.Hermes.Exceptions.RuleDataAccessException exc){
						error = true;
						OnError(_navSessionId,"Erreur dans la vérification de la mise à jour du Support ("+currentRow["id_media"].ToString()+") > Rule ("+_ruleParameters.PluginId+") - Date : "+currentRow["date_media_num"].ToString()+" - "+_ruleParameters.TableName, exc);
						base.AddErrorMessage(exc.Message);
					}
				}

				// Envoi du rapport de fin de traitement
				_duration = DateTime.Now.Subtract(_endExecutionDateTime);
				OnSendReport(_reportTitle,_duration,DateTime.Now,GetHtml(),_mailList,_errorList,_hermesConfig.CustomerMailFrom,_hermesConfig.CustomerMailServer,_hermesConfig.CustomerMailPort,_navSessionId);

				// Statut : Traité
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.done);
				if(!error) ParameterSystem.DeleteRequest(_dataSource,_navSessionId);
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId+" > Rule ("+_ruleParameters.Id.ToString()+") - Day : "+_ruleParameters.Day.ToString()+" - "+_ruleParameters.TableName.ToUpper());
			}
			catch(System.Exception err){
				// Erreur dans le traitement, on change l'id_statut dans static_nav_session
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.error);
				OnError(_navSessionId,"Erreur lors du traitement du résultat.", err);
			}
		}
		#endregion

		#region Méthode privée
		/// <summary>
		/// Construction du code html du rapport
		/// </summary>
		/// <returns>HTML</returns>
		private string GetHtml(){
			StringBuilder t = new StringBuilder();

			t.Append("<table width=800 cellspacing=0 cellpadding=0 border=0");
			t.Append("<tr>");
			t.Append("<td colpsan=2>&nbsp;</td>");
			t.Append("</tr>");

			// Media
			t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
			t.Append("<td width=230>&nbsp;&raquo;&nbsp;Vérification sur</td>");
			t.Append("<td><strong>"+_ruleParameters.TableName.ToUpper()+"</strong></td>");
			t.Append("</tr>");
			// Jour vérifié
			t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
			t.Append("<td>&nbsp;&raquo;&nbsp;Journée du</td>");
			t.Append("<td><strong>"+_ruleParameters.Day.ToString("dd/MM/yyyy")+"</strong></td>");
			t.Append("</tr>");
			// Tranche horaire
			if(_ruleParameters.TableName == "data_radio"){
				t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
				t.Append("<td>&nbsp;&raquo;&nbsp;Tranche horaire</td>");
				t.Append("<td><strong>"+_ruleParameters.HourBegin.ToString("HH:mm")+" - "+_ruleParameters.HourEnd.ToString("HH:mm")+"</strong></td>");
				t.Append("</tr>");
			}
			// Nombre total de support vérifié
			t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
			t.Append("<td>&nbsp;&raquo;&nbsp;Nombre total de support vérifié</td>");
			t.Append("<td><strong>"+_nbMediaTreated.ToString()+"</strong></td>");
			t.Append("</tr>");
			// Nombre de support non transféré
			t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
			t.Append("<td>&nbsp;&raquo;&nbsp;Nombre de support non transféré</td>");
			t.Append("<td><strong>"+_nbMediaTreatedKO.ToString()+"</strong></td>");
			t.Append("</tr>");

			// Liste des supports qui n'ont pas été transféré (0 dans DATA_PRESS)
			if(_mediaTreatedKOList.Length > 0){
				t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
				t.Append("<td valign=\"top\">&nbsp;&raquo;&nbsp;Liste des supports non transférés</td>");
				t.Append("<td><table>");
				ClassificationDA.MediaBranch.PartialMediaListDataAccess media = new TNS.AdExpress.DataAccess.Classification.MediaBranch.PartialMediaListDataAccess(_mediaTreatedKOList.Substring(0,_mediaTreatedKOList.Length-1),33,_dataSource);
				string[] mediaIdList = _mediaTreatedKOList.Substring(0,_mediaTreatedKOList.Length-1).Split(',');
				foreach(string mediaId in mediaIdList){
					t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-weight: bold;font-size: 13px;color: #4b3e5a;\" nowrap><td width=50%>"+media[Int64.Parse(mediaId)]+"</td><td>"+mediaId+"</td></tr>");
				}
				t.Append("</table></td>");
				t.Append("</tr>");
			}

			t.Append("<tr>");
			t.Append("<td colpsan=2>&nbsp;</td>");
			t.Append("</tr>");
			t.Append("</table>");

			return(t.ToString());
		}
		#endregion

	}
}
