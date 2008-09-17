#region Informations
// Auteur : B.Masson
// Date de cr�ation : 13/04/2006
// Date de modification :
#endregion

using System;
using System.Collections;
using System.Data;
using System.Threading;
using GebConfiguration=TNS.AdExpress.Geb.Configuration;
using GebDA=TNS.AdExpress.Geb.DataAccess;
using GebExceptions=TNS.AdExpress.Geb.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Geb{
	/// <summary>
	/// Serveur Geb
	/// </summary>
	public class GebServer{

		#region D�claration des �v�nements
		/// <summary>
		/// Lancement du serveur
		/// </summary>
		public delegate void StartServer(string message);
		/// <summary>
		/// Arr�t du serveur Geb
		/// </summary>
		public delegate void StopServer(string message);
		/// <summary>
		/// Nouveau support
		/// </summary>
		public delegate void NewMedia(string message);
		/// <summary>
		/// Nouvelle alerte
		/// </summary>
		public delegate void NewAlert(string message);
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

		#region Ev�nements
		/// <summary>
		/// Lancement du serveur
		/// </summary>
		public event StartServer OnStartServer;
		/// <summary>
		/// Arr�t du serveur
		/// </summary>
		public event StopServer OnStopServer;
		/// <summary>
		/// Nouveau support
		/// </summary>
		public event NewMedia OnNewMedia;
		/// <summary>
		/// Nouvelle alerte
		/// </summary>
		public event NewAlert OnNewAlert;
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
		/// Thread
		/// </summary>
		private System.Threading.Thread _myThread;
		/// <summary>
		/// Indique que Geb Server doit �tre stopper
		/// </summary>
		private bool _stop = false;
		/// <summary>
		/// Configuration de l'application
		/// </summary>
		protected GebConfiguration.Application _applicationConfiguration = null;
		/// <summary>
		/// Source de donn�es
		/// </summary>
		protected TNS.FrameWork.DB.Common.IDataSource _source = null;
		/// <summary>
		/// Nombre d'ouvrier
		/// </summary>
		private Hashtable _jobsList;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="applicationConfiguration">Configuration de l'application</param>
		public GebServer(IDataSource source, GebConfiguration.Application applicationConfiguration){
			_applicationConfiguration = applicationConfiguration;
			_source = source;
			_jobsList=new Hashtable(1);
		}
		#endregion

		#region  Lancement de la thread
		/// <summary>
		/// Traitement pour le lancement de la thread
		/// </summary>
		public void Treatement(){
			_myThread=new Thread(new ThreadStart(Compute));
			_myThread.Name="GebServer : Push Mail Alerts Update";
			_myThread.Start();
		}
		#endregion

		#region Fermeture de la thread
		/// <summary>
		/// Arr�t brutal du serveur
		/// </summary>
		public void AbortGebServer(){
			_myThread.Abort();
		}

		/// <summary>
		/// Signal to server to finish pending jobs and to stop
		/// </summary>
		public void StopGebServer(){
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
		/// Execution du traitement de mise � jour des alertes
		/// </summary>
		public void Compute(){
			
			#region Variables
			DataSet dsNewMedia = null;
			AlertByMedia alertByMedia = null;
			DateTime dtStartTreatement;
			DateTime dtEndTreatement;
			bool error=false;
			#endregion

			OnStartServer(DateTime.Now.ToString());
			while(_myThread.IsAlive && !_stop){

				#region Chargement des nouveaux supports
				dtStartTreatement = DateTime.Now; // Date de d�but de traitement
				try{
					dsNewMedia = TNS.AdExpress.Geb.DataAccess.NewPublicationDataAccess.GetData(_source);
					// Pas de nouveaux supports
					if(dsNewMedia==null || dsNewMedia.Tables.Count==0 || dsNewMedia.Tables[0]==null || dsNewMedia.Tables[0].Rows.Count==0){
						OnNewMedia("Pas de nouveaux supports charg�s le "+DateTime.Now.ToString());
					}
					// Il existe des nouveaux supports
					else{
						OnNewMedia(dsNewMedia.Tables[0].Rows.Count.ToString() + " nouveaux supports charg�s le "+DateTime.Now.ToString());

						#region Chargement des alertes
						try{
							alertByMedia = new AlertByMedia(_source,dsNewMedia);
							// Il existe au moins un support impact� par les alertes
							if(alertByMedia.Count>0){

								#region Cr�ation des alertes de chaque support
								foreach(DataRow currentNewMedia in dsNewMedia.Tables[0].Rows){
									if(alertByMedia.Contains((Int64)currentNewMedia["id_media"])){
										foreach(Int64 currentAlert in alertByMedia[(Int64)currentNewMedia["id_media"]]){

											_jobsList.Add(currentAlert,(Int64)currentNewMedia["id_media"]);

											try{
												// Cr�ation de l'alerte dans la table "static_nav_session"
												AlertRequest alertRequest = new AlertRequest(currentAlert, currentNewMedia["date_media_num"].ToString());
												alertRequest.Save(_source);
												OnNewAlert(((Int64)currentNewMedia["id_media"]).ToString()+"("+currentNewMedia["date_media_num"].ToString()+") -> "+currentAlert.ToString());
											}
											catch(System.Exception ex){

												#region Mise en statut Erreur en cas d'erreur
												// En cas d'erreur lors de la cr�ation d'une alerte, changer statut du support
												try{
													error = true;
													GebDA.NewPublicationDataAccess.UpdateMediaStatusType(_source,(Int64)currentNewMedia["id_alert_push_mail"]);
													OnMessage("Le support "+currentNewMedia["id_media"].ToString()+" (alerte : "+currentNewMedia["id_alert_push_mail"].ToString()+") a �t� chang� en statut Erreur");
												}
												catch(GebExceptions.NewPublicationDataAccessException ge){
													OnError("Erreur dans la mise en statut erreur du support "+currentNewMedia["id_media"].ToString()+" (alerte : "+currentNewMedia["id_alert_push_mail"].ToString()+")",ge);
												}
												#endregion

												OnError("Erreur dans la cr�ation de l'alerte : "+currentAlert.ToString(),ex);
											}

											_jobsList.Remove(currentAlert);
										}
									}
									if(!error){
										error=false;
										try{
											// Suppression de la ligne des nouveaux supports de la table "alert_push_mail"
											GebDA.NewPublicationDataAccess.DeleteMedia(_source, (Int64)currentNewMedia["id_alert_push_mail"]);
										}
										catch(System.Exception ex){
											OnError("Erreur dans la suppression de l'alerte : "+currentNewMedia["id_alert_push_mail"].ToString(),ex);
										}
									}
								}
								#endregion
								
							}
							else OnNewAlert("Aucune alerte � cr�er");
						}
						catch(GebExceptions.AlertByMediaException ge){
							OnError("Erreur dans le chargement des alertes",ge);
						}
						catch(System.Exception ex){
							OnErrorMessage("Erreur g�n�rale dans le chargement des alertes");
						}
						#endregion
					
					}
					// Fin else
				}
				catch(GebExceptions.NewPublicationDataAccessException ge){
					OnError("Erreur dans le chargement des nouveaux supports",ge);
				}
				catch(System.Exception ex){
					OnErrorMessage("Erreur g�n�rale dans le chargement des nouveaux supports");
				}
				#endregion

				#region Mise en suspend de la thread jusqu'au prochain refresh
				dtEndTreatement = DateTime.Now; // Date de fin de traitement		

				// Mise en suspend de la thread jusqu'au prochain refresh
				System.Threading.Thread.Sleep(GetNextRefresh(dtStartTreatement,dtEndTreatement));
				#endregion

			}
			OnStopServer(DateTime.Now.ToString());
		}
		#endregion

		#region M�thodes priv�es
		/// <summary>
		/// Calcule le temps restant pour le prochain refresh
		/// </summary>
		/// <param name="dtStartTreatement">DateTime du d�but du traitement</param>
		/// <param name="dtEndTreatement">DateTime de la fin du traitement</param>
		/// <returns>Temps pour le prochain refresh (en milliseconde)</returns>
		private int GetNextRefresh(DateTime dtStartTreatement,DateTime dtEndTreatement){
			int newRefresh = 0;
			int intervalTime = 0;
			DateTime dtNow = DateTime.Now;

			if(dtNow.Hour >= 7 && dtNow.Hour <= 20){
				// Calcul de l'interval de temps entre le refresh initial et le temps du traitement
				intervalTime = _applicationConfiguration.Refresh - (GetMilliseconds(dtEndTreatement) - GetMilliseconds(dtStartTreatement));

				if(intervalTime > 0){
					// Ex : intervalTime = 15 - (10) > prochain refresh dans 5 min
					newRefresh = intervalTime;
				}
				else if(intervalTime < 0){
					// Ex : intervalTime = 15 - (20) > on d�passe, on retire la difference au prochain refresh :
					// refresh initial (15 min) - intervalTime (5 min) > prochain refresh dans 10 min
					newRefresh = _applicationConfiguration.Refresh + intervalTime;
				}
				else{
					newRefresh = _applicationConfiguration.Refresh;
				}
			}
			else{
				// Calcul du DateTime.Now en milliseconde
				int dateNowInMMS = GetMilliseconds(dtNow);
				// Calcul de la diff�rence de temps entre 24H (86400000 ms) et le DateTime.Now
				int diffrenceUntilMidnight = 86400000 - dateNowInMMS;
				// Prochain refresh fix� � 7h00
				newRefresh = diffrenceUntilMidnight + 25200000;
			}
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
