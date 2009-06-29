#region Informations
// Auteur : B.Masson
// Date de cr�ation : 16/02/2007
// Date de modification :
#endregion

#region Namespaces
using System;
using System.IO;
using System.Data;
using System.Threading;
using System.Text;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Anubis.BusinessFacade;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.Common;
using TNS.FrameWork.DB.Common;
using FrameworkDBBusiness = TNS.FrameWork.DB.BusinessFacade;
using Hermes=TNS.AdExpress.Hermes;
using TNS.Baal.Common;
using TNS.Baal.ExtractList;
using ClassificationDA=TNS.AdExpress.DataAccess.Classification;
#endregion

namespace TNS.AdExpress.Anubis.HermesPress{
	/// <summary>
	/// Implementation de TNS.AdExpress.Anubis.BusinessFacade.IPlugin pour HermesPress plugin
	/// </summary>
	public class TreatementSystem : TNS.AdExpress.Anubis.BusinessFacade.ReportingPluginBase,TNS.AdExpress.Anubis.BusinessFacade.IPlugin{

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
		/// Source de donn�es
		/// </summary>
		private IDataSource _dataSource;
		/// <summary>
		/// Source de donn�es pour la base pitagor (interrogation de la vue)
		/// </summary>
		private IDataSource _dataSourcePitagor;
		/// <summary>
		/// Configuration du plugin Hermes Radio TV
		/// </summary>
		private TNS.AdExpress.Anubis.HermesPress.Common.Configuration _hermesConfig;
		/// <summary>
		/// Param�tres d'une r�gle
		/// </summary>
		private Hermes.RuleParameters _ruleParameters = null;
		/// <summary>
		/// Nombre de support trait�
		/// </summary>
		private int _nbMediaTreated=0;
		/// <summary>
		/// Nombre de support trait� dont le r�sultat est 0 dans DATA_PRESS
		/// </summary>
		private int _nbMediaTreatedKO=0;
		/// <summary>
		/// Liste des supports trait�s dont le r�sultat est 0 dans DATA_PRESS
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
			return "Hermes Press Indicator";
		}
		#endregion

		#region D�marrage et Arr�t de l'�x�cution du thread
		/// <summary>
		///  Lance le traitement du r�sultat
		/// </summary>
		/// <param name="navSessionId">Identifiant Id_static_nav_session</param>
		/// <param name="confifurationFilePath">Chemin du fihier de configuration</param>
		/// <param name="dataSource">Source de donn�es</param>
		public void Treatement(string confifurationFilePath,IDataSource dataSource,Int64 navSessionId){
			_navSessionId=navSessionId;

			// A. Chargement de la connexion (user pour Hermes) pour WEBNAV et ADEXPR03 :
			// B. Chargement de la connexion (user pour Hermes) pour interroger la vue dans PITAGOR :
	
			#region A. Chargement du fichier de configuration
			if(confifurationFilePath==null){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job", new ArgumentNullException("Le nom du fichier de configuration est null."));
				return;
			}
			if(confifurationFilePath.Length==0){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job", new ArgumentException("Le nom du fichier de configuration est vide."));
				return;
			}
			try{
				// Chargement de la connexion : utilisateur particulier pour Hermes
				TNS.FrameWork.DB.Common.Oracle.DataBaseConfiguration connection = FrameworkDBBusiness.Oracle.DataBaseConfigurationBussinessFacade.GetOne(AppDomain.CurrentDomain.BaseDirectory+confifurationFilePath);
				_dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(connection.ConnectionString);

				// Chargement des param�tres d'email pour l'envoi du rapport (server, port, from...)
				_hermesConfig=new TNS.AdExpress.Anubis.HermesPress.Common.Configuration(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+confifurationFilePath));
				
				// Chargement de la configuration du rapport (title, mails...)
				base.LoadReportingConfig(_hermesConfig.ConfigurationReportFilePath);
			}
			catch(System.Exception err){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
			#endregion

			#region B. Chargement du fichier de configuration
			try{
				// Chargement de la connexion : utilisateur particulier pour Hermes
				TNS.FrameWork.DB.Common.Oracle.DataBaseConfiguration connectionPitagor = FrameworkDBBusiness.Oracle.DataBaseConfigurationBussinessFacade.GetOne(AppDomain.CurrentDomain.BaseDirectory+@"Config\Hermes\HermesPita.xml");
				_dataSourcePitagor = new TNS.FrameWork.DB.Common.OracleDataSource(connectionPitagor.ConnectionString);
			}
			catch(System.Exception err){
				OnError(_navSessionId,"Impossible de lancer le traitement d'un job <== impossible de charger le fichier de configuration",err);
				return;
			}
			#endregion
			
			ThreadStart myThreadStart = new ThreadStart(ComputeTreatement);
			_myThread=new Thread(myThreadStart);
			_myThread.Name="Hermes Press Indicator";
			_myThread.Start();
		}

		/// <summary>
		/// Arr�te le traitement du r�sultat
		/// </summary>
		public void AbortTreatement(){
			_myThread.Abort();
		}
		#endregion

		#region Traitement du r�sultat
		/// <summary>
		/// Traitement des v�rifications des mises � jour des transferts de la presse sur la base AdExpress
		/// </summary>
		private void ComputeTreatement(){
			
			#region Variables
			Liste list = null;
			string[] mediaList = null;
            Int64 nbLine = 0;
			DataSet dsParutionList = null;
			bool error = false;
			#endregion

			try{
				OnStartWork(_navSessionId,this.GetPluginName()+" started for "+_navSessionId);
				_endExecutionDateTime = DateTime.Now;

				#region Request Details
				DataRow rqDetails = ParameterSystem.GetRequestDetails(_dataSource,_navSessionId).Tables[0].Rows[0];
				#endregion

				#region Chargement des param�tres de la r�gle (blob)
				_ruleParameters = (Hermes.RuleParameters)Hermes.RuleParameters.Load(_navSessionId);
				#endregion

				#region Chargement de la liste des supports (Baal)
				list = Baal.ExtractList.BusinessFacade.ListesSystem.GetFromId((int)_ruleParameters.MediaListId);
				mediaList = list.GetLevelIds(Baal.ExtractList.Constantes.Levels.media).Split(',');
				#endregion

				#region V�rification du transfert des donn�es de chaque support de la Presse
				// Statut : En cours de traitement
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.processing);

				foreach(string mediaId in mediaList){
					try{
						// Chargement des parutions du support
						dsParutionList = Hermes.DataAccess.RuleDataAccess.GetMediaPressList(_dataSourcePitagor,mediaId,Int64.Parse(_ruleParameters.Day.ToString("yyyyMMdd")));

						// Parcours des parutions
						foreach(DataRow currentRow in dsParutionList.Tables[0].Rows){
							// Compteur du nombre de parution
							_nbMediaTreated++;
							try {

                                #region Calcul du nombre de lignes de DATA_PRESS (Modifi�e le 26/05/2008)
                                // Calcul du nombre de lignes de DATA_PRESS pour chaque parution (date_media_num)
                                nbLine = Hermes.DataAccess.RuleDataAccess.GetDataControl(_dataSource, _ruleParameters.TableName, Int64.Parse(currentRow["id_media"].ToString()), Int64.Parse(currentRow["date_media_num"].ToString()));

                                // Compteur du nombre de parution � 0
                                if(nbLine == 0) {
                                    _nbMediaTreatedKO++;
                                    _mediaTreatedKOList += currentRow["id_media"].ToString() + ",";
                                }

                                // Enregistrement du nombre de lignes dans la table DATA_CONTROL (avec date_cover_num)
                                Hermes.DataAccess.RuleDataAccess.SetDataControl(_dataSource, Int64.Parse(currentRow["id_media"].ToString()), Int64.Parse(currentRow["date_cover_num"].ToString()), nbLine, 0);
                                #endregion

                                // R�initialisation :
                                nbLine = 0;
							}
							catch(TNS.AdExpress.Hermes.Exceptions.RuleDataAccessException ex){
								error = true;
								OnError(_navSessionId,"Erreur dans la v�rification de la mise � jour du Support ("+currentRow["id_media"].ToString()+") / Parution ("+currentRow["date_media_num"].ToString()+") > Rule ("+_ruleParameters.PluginId+") - Date : "+_ruleParameters.Day.ToString(), ex);
								base.AddErrorMessage(ex.Message);
							}
						}

						// R�initialisation :
						dsParutionList = null;
					}
					catch(System.Exception exc){
						error = true;
						OnError(_navSessionId,"Erreur dans le chargement des parutions du support ("+mediaId+") > Rule ("+_ruleParameters.PluginId+") - Date : "+_ruleParameters.Day.ToString(), exc);
						base.AddErrorMessage(exc.Message);
					}
				}

				// Envoi du rapport de fin de traitement
				_duration = DateTime.Now.Subtract(_endExecutionDateTime);
				OnSendReport(_reportTitle,_duration,DateTime.Now,GetHtml(),_mailList,_errorList,_hermesConfig.CustomerMailFrom,_hermesConfig.CustomerMailServer,_hermesConfig.CustomerMailPort,_navSessionId);

				// Statut : Trait�
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.done);
				if(!error) ParameterSystem.DeleteRequest(_dataSource,_navSessionId);
				#endregion

				OnStopWorkerJob(_navSessionId,"","",this.GetPluginName()+" finished for "+_navSessionId+" > Rule ("+_ruleParameters.Id.ToString()+") - Day : "+_ruleParameters.Day.ToString()+" - "+_ruleParameters.TableName.ToUpper());
			}
			catch(System.Exception err){
				// Erreur dans le traitement, on change l'id_statut dans static_nav_session
				ParameterSystem.ChangeStatus(_dataSource,_navSessionId,TNS.AdExpress.Anubis.Constantes.Result.status.error);
				OnError(_navSessionId,"Erreur lors du traitement du r�sultat.", err);
			}
		}
		#endregion

		#region M�thode priv�e
		/// <summary>
		/// Construction du code html du rapport
		/// </summary>
		/// <returns>HTML</returns>
		private string GetHtml(){
			StringBuilder t = new StringBuilder();

			t.Append("<table width=800 cellspacing=0 cellpadding=0 border=0");
			t.Append("<tr>");
			t.Append("<td colspan=2>&nbsp;</td>");
			t.Append("</tr>");

			// V�rification sur la table
			t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
			t.Append("<td width=230>&nbsp;&raquo;&nbsp;V�rification sur</td>");
			t.Append("<td><strong>"+_ruleParameters.TableName.ToUpper()+"</strong></td>");
			t.Append("</tr>");
			// Journ�e du
			t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
			t.Append("<td>&nbsp;&raquo;&nbsp;Journ�e du</td>");
			t.Append("<td><strong>"+_ruleParameters.Day.ToString("dd/MM/yyyy")+"</strong></td>");
			t.Append("</tr>");
			// Nombre total de parution v�rifi�e
			t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
			t.Append("<td>&nbsp;&raquo;&nbsp;Nombre total de parution v�rifi�e</td>");
			t.Append("<td><strong>"+_nbMediaTreated.ToString()+"</strong></td>");
			t.Append("</tr>");
			// Nombre de parution non transf�r�e
			t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
			t.Append("<td>&nbsp;&raquo;&nbsp;Nombre de parution non transf�r�e</td>");
			t.Append("<td><strong>"+_nbMediaTreatedKO.ToString()+"</strong></td>");
			t.Append("</tr>");
			
			// Liste des supports qui n'ont pas �t� transf�r� (0 dans DATA_PRESS)
			if(_mediaTreatedKOList.Length > 0){
				t.Append("<tr style=\"font-family: Arial, Helvetica, sans-serif;font-size: 13px;color: #4b3e5a;\" nowrap>");
				t.Append("<td valign=\"top\">&nbsp;&raquo;&nbsp;Liste des supports non transf�r�s</td>");
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
			t.Append("<td colspan=2>&nbsp;</td>");
			t.Append("</tr>");
			t.Append("</table>");

			return(t.ToString());
		}
		#endregion

	}
}
