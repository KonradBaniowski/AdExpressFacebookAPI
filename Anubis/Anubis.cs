#region Informations
// Auteur: G. Facon
// Date de création: 19/05/2005
// Date de modification: 19/05/2005
#endregion

#region Using
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Data;

using TNS.AdExpress.Anubis.BusinessFacade.Core;
using TNS.AdExpress.Anubis.BusinessFacade;
using TNSAnubisCommon = TNS.AdExpress.Anubis.Common;
using TNSAnubisExceptions = TNS.AdExpress.Anubis.Exceptions;
using TNSAnubisConstantes = TNS.AdExpress.Anubis.Constantes;
using TNS.AdExpress.Anubis.BusinessFacade.Configuration;
using TNS.AdExpress.Anubis.Common.Configuration;

using TNS.FrameWork;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.BusinessFacade.Oracle;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Error.Log;
using TNS.FrameWork.Net.Mail;

#endregion



namespace Anubis {
	public partial class Anubis : Form {

		#region Variables
		/// <summary>
		/// Liste des plug-ins
		/// </summary>
		private Hashtable _plugInList;
		/// <summary>
		/// Liste des demandes
		/// </summary>
		private DataTable _requestList = new DataTable();
		/// <summary>
		/// Source de données
		/// </summary>
		private IDataSource _dataSource;
		/// <summary>
		/// Configuration de l'application
		/// </summary>
		private AnubisConfig _anubisConfiguration;
		/// <summary>
		/// Serveur de mise à jour des demandes
		/// </summary>
		private RequestsUpdateSystem _requestsUpdateServer;
		/// <summary>
		/// Serveur de distribution des jobs
		/// </summary>
		DistributionSystem _distributionServer;
		/// <summary>
		/// Message logging
		/// </summary>
		private XmlWriter _trace;
		/// <summary>
		/// Initialization date of the log
		/// </summary>
		private DateTime _traceDate = DateTime.Now;
		/// <summary>
		/// Timer de tâches journalière récurrentes (comme trace ou nettoyage des résultats)
		/// </summary>
		private Timer _timer = null;
		#endregion

		#region Variables MMI
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ColumnHeader listenerColumnHeader;
		private System.Windows.Forms.ListView listViewPlugins;
		private System.Windows.Forms.ListView listViewJobs;
		private System.Windows.Forms.ListView listViewJobsDone;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView listViewJobsError;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.GroupBox groupBoxJobs;
		private System.Windows.Forms.ListView listViewMessage;
		private System.Windows.Forms.ColumnHeader messageColumnHeader;
		private System.Windows.Forms.ImageList IconsList;
		private System.Windows.Forms.ColumnHeader iconeColumnHeader;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label nbJobslabel;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemFileExit;
		private System.Windows.Forms.PictureBox anubisPictureBox;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		#endregion

		#region Délégués

		/// <summary>
		/// Délégué pour rappel de la méthode de notification d'ajout d'une nouvelle ligne
		/// </summary>
		public delegate void NewLineCallBack();

		/// <summary>
		/// Délégué pour rappel de la méthode notification du debut d'un job 
		/// </summary>
		public delegate void StartJobCallBack(Int64 navSessionId, string message);

		/// <summary>
		/// Délégué pour rappel de la méthode de notification d'un message
		/// </summary>
		public delegate void MessageCallBack(string message, int imageIndex);	

		/// <summary>
		/// Délégué pour rappel de la méthode de gestion d'un job 
		/// </summary>
		public delegate void ManageJobCallBack(Int64 navSessionId, string message, int imageIndex);

		#endregion


		#region Constructeur
		public Anubis() {
			InitializeComponent();


			#region Chargement des paramètres de l'application
			_anubisConfiguration = new AnubisConfig(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory + TNSAnubisConstantes.Application.Configuration.CONFIGURATION_DIRECTORY + @"\" + TNSAnubisConstantes.Application.Configuration.ANUBIS_CONFIGURATION_FILE));
			#endregion

			#region Log Management
			SetupTrace();
			try { _trace.WriteLine("Anubis started"); }
			catch (System.Exception) { }
			#endregion

			#region Chargement des plug-ins
			_plugInList = PluginsSystem.GetPluginsToLoad(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory + TNSAnubisConstantes.Application.Configuration.CONFIGURATION_DIRECTORY + @"\" + TNSAnubisConstantes.Application.Configuration.PLUGINS_FILE));
			LoadPluginsList();
			#endregion

			#region Construction de la connexion à la base de données
			_dataSource = new OracleDataSource(DataBaseConfigurationBussinessFacade.GetOne(AppDomain.CurrentDomain.BaseDirectory + TNSAnubisConstantes.Application.Configuration.CONFIGURATION_DIRECTORY + @"\" + TNSAnubisConstantes.Application.Configuration.DATABASE_FILE).ConnectionString);
			try {
				_dataSource.Open();
			}
			catch (TNS.FrameWork.Exceptions.BaseException err) {
				string t = err.GetHtmlDetail();
				ErrorManagement(t);
				throw new BaseException("Anubis failed to run", err);
			}
			#endregion

			#region Action journalière
			CleanResults();
			_timer = new Timer();
			_timer.Interval = Convert.ToInt32(GetMillisecondsToTommorow());
			_timer.Tick += new EventHandler(timer_Tick);
            _timer.Enabled = true;
            _timer.Start();
			#endregion

			#region Job List
			try {
				_requestList = RequestsSystem.Get(_dataSource, "", _plugInList);
			}
			catch (System.Exception exc) {
				string m = "Unable to launch request update server <br>";
				try {
					m += ((BaseException)exc).GetHtmlDetail();
				}
				catch (System.Exception e) {
					m += e.Message + " - " + e.StackTrace;
				}
				ErrorManagement(m);
			}
			_requestsUpdateServer = new RequestsUpdateSystem(_anubisConfiguration, _requestList, _dataSource, _plugInList);
			_requestsUpdateServer.OnNewLines += new TNS.AdExpress.Anubis.BusinessFacade.Core.RequestsUpdateSystem.NewLines(requestsUpdateSystem_OnNewLines);
			_requestsUpdateServer.OnError += new TNS.AdExpress.Anubis.BusinessFacade.Core.RequestsUpdateSystem.Error(requestsUpdateServer_OnError);
			_requestsUpdateServer.Treatement();
			#endregion

			#region Distribution des Jobs
			_distributionServer = new DistributionSystem(_anubisConfiguration, _plugInList, _requestList, _dataSource);
			_distributionServer.OnStartJob += new TNS.AdExpress.Anubis.BusinessFacade.Core.DistributionSystem.StartJob(distributionServer_OnStartJob);
			_distributionServer.OnStopJob += new TNS.AdExpress.Anubis.BusinessFacade.Core.DistributionSystem.StopJob(distributionServer_OnStopJob);
			_distributionServer.OnErrorJob += new TNS.AdExpress.Anubis.BusinessFacade.Core.DistributionSystem.ErrorJob(distributionServer_OnErrorJob);
			_distributionServer.OnWarningJob += new TNS.AdExpress.Anubis.BusinessFacade.Core.DistributionSystem.WarningJob(distributionServer_OnWarningJob);
			_distributionServer.Treatement();
			#endregion
		}
		#endregion


		#region Evènements

		#region RequestsUpdateSystem
		/// <summary>
		/// Met à jour la liste des requêtes à traiter
		/// </summary>
		private void requestsUpdateSystem_OnNewLines() {
			if (this.InvokeRequired) {
				NewLineCallBack callBack = new NewLineCallBack(MT_NewLines);
				this.Invoke(callBack);
			}
			else {
				lock (listViewJobs) {
					listViewJobs.Items.Clear();
					lock (_requestList) {
						foreach (DataRow currentRow in _requestList.Rows) {
							listViewJobs.Items.Add(currentRow[0].ToString());
						}
					}
				}
			}
		}

		/// <summary>
		/// Request Update launch an error
		/// </summary>
		/// <param name="message"></param>
		private void requestsUpdateServer_OnError(string message, System.Exception exc) {
			#region Old version
			//string m = message;
			//try {
			//    lock (listViewMessage) {
			//        ListViewItem Entry = new ListViewItem("", 0);
			//        Entry.SubItems.Add(m);
			//        listViewMessage.Items.Insert(0, Entry);
			//    }
			//    m += ((BaseException)exc).GetHtmlDetail();
			//}
			//catch (System.Exception e) {
			//    m += "<br>" + e.Message + " - " + e.StackTrace;
			//}
			//ErrorManagement(m);
			//try {
			//    _requestsUpdateServer.Treatement();
			//}
			//catch (System.Exception e2) {
			//    ErrorManagement("Unable to reinitialize request update server : <br> "
			//        + e2.Message + " - " + e2.StackTrace
			//        + "<br><br>Original error : <br>"
			//        + m);
			//}
			#endregion
						
				string m = message;
				try {
					if (this.InvokeRequired) {
						MessageCallBack callBack = new MessageCallBack(AddListViewMessage);
						this.Invoke(callBack, new object[] { message, 0 });											
					}
					else {
						AddListViewMessage(m, 0);
					}
					m += ((BaseException)exc).GetHtmlDetail();
				}
				catch (System.Exception e) {
					m += "<br>" + e.Message + " - " + e.StackTrace;
				}
				ErrorManagement(m);
				try {
					_requestsUpdateServer.Treatement();
				}
				catch (System.Exception e2) {
					ErrorManagement("Unable to reinitialize request update server : <br> "
						+ e2.Message + " - " + e2.StackTrace
						+ "<br><br>Original error : <br>"
						+ m);
				}
			

		}
		#endregion

		#region distributionServer
		/// <summary>
		/// Un Job a débuter
		/// </summary>
		/// <param name="navSessionId">Identifiant du résultat</param>
		/// <param name="message">Message</param>
		private void distributionServer_OnStartJob(Int64 navSessionId, string message) {
			#region Old version
			//requestsUpdateSystem_OnNewLines();
			//lock (listViewMessage) {
			//    ListViewItem Entry = new ListViewItem("", 0);
			//    Entry.SubItems.Add(message);
			//    listViewMessage.Items.Insert(0, Entry);
			//}
			//nbJobslabel.Text = _distributionServer.RunningJobCount + " / " + _anubisConfiguration.JobsNumber;
			//try {
			//    _trace.WriteLine(message + " ==> " + nbJobslabel.Text);
			//}
			//catch (System.Exception) { }
			#endregion
			
			if (this.InvokeRequired) {
				MessageCallBack callBack = new MessageCallBack(MT_StartJob);
				this.Invoke(callBack, new object[]{message,0});
			}
			else MT_StartJob(message, 0);
						
		}

		/// <summary>
		/// Un Job s'est arrêté
		/// </summary>
		/// <param name="navSessionId">Identifiant du résultat</param>
		/// <param name="message">Message</param>
		private void distributionServer_OnStopJob(Int64 navSessionId, string message) {
			#region Old version
			//listViewJobsDone.Items.Add(navSessionId.ToString());
			//lock (listViewMessage) {
			//    ListViewItem Entry = new ListViewItem("", 1);
			//    Entry.SubItems.Add(message);
			//    listViewMessage.Items.Insert(0, Entry);
			//}
			//nbJobslabel.Text = _distributionServer.RunningJobCount + " / " + _anubisConfiguration.JobsNumber;
			//try {
			//    _trace.WriteLine(message + " ==> " + nbJobslabel.Text);
			//}
			//catch (System.Exception) { }
			#endregion

			if (this.InvokeRequired) {
				ManageJobCallBack callBack = new ManageJobCallBack(MT_StopJob);
				this.Invoke(callBack, new object[] {navSessionId, message, 1 });
			}
			else MT_StopJob(navSessionId,message, 1);
		}

		/// <summary>
		/// Une erreur est survenue
		/// </summary>
		/// <param name="navSessionId"></param>
		/// <param name="message"></param>
		/// <param name="e">Exception</param>
		private void distributionServer_OnErrorJob(Int64 navSessionId, string message, System.Exception e) {
			#region Old version
			//listViewJobsError.Items.Add(navSessionId.ToString());
			//lock (listViewMessage) {
			//    ListViewItem Entry = new ListViewItem("", 2);
			//    Entry.SubItems.Add(message);
			//    listViewMessage.Items.Insert(0, Entry);
			//}
			//ErrorManagement(navSessionId, message, e);
			#endregion

			if (this.InvokeRequired) {
				ManageJobCallBack callBack = new ManageJobCallBack(MT_ErrorJob);
				this.Invoke(callBack, new object[] { navSessionId, message, 2 });
			}
			else MT_StopJob(navSessionId, message, 2);

			ErrorManagement(navSessionId, message, e);
		}

		/// <summary>
		/// Un avertissement
		/// </summary>
		/// <param name="navSessionId"></param>
		/// <param name="message"></param>
		private void distributionServer_OnWarningJob(Int64 navSessionId, string message) {
			#region Old version
			//lock (listViewMessage) {
			//    ListViewItem Entry = new ListViewItem("", 3);
			//    Entry.SubItems.Add(message);
			//    listViewMessage.Items.Insert(0, Entry);
			//}
			#endregion
			if (this.InvokeRequired) {
				ManageJobCallBack callBack = new ManageJobCallBack(MT_WarningJob);
				this.Invoke(callBack, new object[] { navSessionId, message, 3 });
			}
			else AddListViewMessage(message, 3);
		}
		#endregion

		#region Menu
		/// <summary>
		/// Quitte l'application
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		private void menuItemFileExit_Click(object sender, System.EventArgs e) {

			_distributionServer.StopServer();

			if (_distributionServer.RunningJobCount > 0) {

				try { _trace.WriteLine("Attempt to stop Anubis while " + _distributionServer.RunningJobCount + " job(s) still running : " + _distributionServer.JobList); }
				catch (System.Exception) { }

				(new Exit(_distributionServer)).ShowDialog();

				try { _trace.WriteLine("Anubis stopping while " + _distributionServer.RunningJobCount + " job(s) still running : " + _distributionServer.JobList); }
				catch (System.Exception) { }
			}
			_distributionServer.AbortServer();

			try { _trace.WriteLine("Distribution server stopped"); }
			catch (System.Exception) { }

			_requestsUpdateServer.StopServer();

			try { _trace.WriteLine("Request update server stopped"); }
			catch (System.Exception) { }

			this.Close();
		}

		/// <summary>
		/// Anubis stopped
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Anubis_Closed(object sender, System.EventArgs e) {
			try {
				_trace.WriteLine("Anubis stopped");
				_trace.Close();
			}
			catch (System.Exception) { }

		}

		#endregion

		#endregion

		#region méthodes internes
		/// <summary>
		/// Chargement de la liste des plugins
		/// </summary>
		private void LoadPluginsList() {
			foreach (Plugin currentPlugin in _plugInList.Values) {
				listViewPlugins.Items.Add(currentPlugin.Name);
			}

		}

		/// <summary>
		/// Supprime les résultats obsolètes. La longévité d'un résultat est définie par son plugin
		/// </summary>
		private void CleanResults() {
			try {
				RequestsSystem.DeleteOldRequests(_dataSource, _plugInList);
			}
			catch (System.Exception exc) {
				try {
					ErrorManagement(((BaseException)exc).GetHtmlDetail());
				}
				catch (System.Exception e) {
					ErrorManagement(e.Message + " - " + e.StackTrace);
				}
			}
		}

		#region Error management
		/// <summary>
		/// Error treatement
		/// </summary>
		/// <param name="navSessionId">User session identifiant</param>
		/// <param name="message">Error message</param>
		private void ErrorManagement(Int64 navSessionId, string message, System.Exception e) {
			//Exception
			string exc = string.Empty;
			try {
				exc = ((BaseException)e).GetHtmlDetail();
			}
			catch (System.Exception) {
				exc = e.Message + " - " + e.StackTrace;
			}
			//envoi Mail sending
			try {
				(new SmtpUtilities(AppDomain.CurrentDomain.BaseDirectory + TNSAnubisConstantes.Application.Configuration.CONFIGURATION_DIRECTORY + @"\" + TNSAnubisConstantes.Application.Configuration.ANUBIS_CONFIGURATION_FILE))
					.SendWithoutThread("Anubis Error on " + navSessionId + " (" + Environment.MachineName + ")", Convertion.ToHtmlString(message + "<br><br>" + exc).Replace("at ", "<br>at "), true, false);
			}
			catch (System.Exception err) {
				try {
					_trace.WriteLine("Unable to send mail for " + navSessionId + ":" + message + "<br><br>" + exc);
				}
				catch (System.Exception) { }
			}
			//log writing
			try {
				_trace.WriteLine(navSessionId + ":" + message);
			}
			catch (System.Exception err) {
				try {
					(new SmtpUtilities(AppDomain.CurrentDomain.BaseDirectory + TNSAnubisConstantes.Application.Configuration.CONFIGURATION_DIRECTORY + @"\" + TNSAnubisConstantes.Application.Configuration.ANUBIS_CONFIGURATION_FILE))
						.SendWithoutThread("Anubis Error (" + Environment.MachineName + ")", Convertion.ToHtmlString("Unable to write log for " + navSessionId + "<br>Mail sending error :<br>" + err.Message + " - " + err.StackTrace + "<br><br>Message to send:<br>" + message + "<br><br>" + exc).Replace("at ", "<br>at "), true, false);
				}
				catch (System.Exception) { }
			}
		}

		/// <summary>
		/// Error treatement
		/// </summary>
		/// <param name="message">Error message</param>
		private void ErrorManagement(string message) {
			//envoi Mail sending
			try {
				(new SmtpUtilities(AppDomain.CurrentDomain.BaseDirectory + TNSAnubisConstantes.Application.Configuration.CONFIGURATION_DIRECTORY + @"\" + TNSAnubisConstantes.Application.Configuration.ANUBIS_CONFIGURATION_FILE))
					.SendWithoutThread("Anubis Error  (" + Environment.MachineName + ")", Convertion.ToHtmlString(message).Replace("at ", "<br>at "), true, false);
			}
			catch (System.Exception err) {
				try {
					_trace.WriteLine("Unable to send mail for " + message);
				}
				catch (System.Exception) { }
			}
			//log writing
			try {
				_trace.WriteLine(message);
			}
			catch (System.Exception err) {
				try {
					(new SmtpUtilities(AppDomain.CurrentDomain.BaseDirectory + TNSAnubisConstantes.Application.Configuration.CONFIGURATION_DIRECTORY + @"\" + TNSAnubisConstantes.Application.Configuration.ANUBIS_CONFIGURATION_FILE))
						.SendWithoutThread("Anubis Error (" + Environment.MachineName + ")", Convertion.ToHtmlString("Unable to write log" + "<br>Mail sending error :<br>" + err.Message + " - " + err.StackTrace + "<br><br>Message to send:<br>" + message).Replace("at ", "<br>at "), true, false);
				}
				catch (System.Exception) { }
			}
		}
		#endregion

		#region Trace management
		/// <summary>
		/// Initialize the trace system
		/// </summary>
		private void SetupTrace() {
			string trPath = "";
			try {
				if (_trace != null) _trace.Close();
				if (!Directory.Exists(_anubisConfiguration.TracePath)) {
					Directory.CreateDirectory(_anubisConfiguration.TracePath);
				}
				trPath = _anubisConfiguration.TracePath + @"\log_" + Environment.MachineName + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xml";
				Console.SetError(_trace = new XmlWriter(trPath));
			}
			catch (System.Exception err) {
				(new SmtpUtilities(AppDomain.CurrentDomain.BaseDirectory + TNSAnubisConstantes.Application.Configuration.CONFIGURATION_DIRECTORY + @"\" + TNSAnubisConstantes.Application.Configuration.ANUBIS_CONFIGURATION_FILE))
					.Send("Anubis Error", "Unable to initialize log file " + trPath + "<br>" + err.Source + "<br>" + err.Message, true, false);
			}
		}
		#endregion

		/// <summary>
		/// Retourne le nombre de millisecond avant le changement de date
		/// </summary>
		/// <returns></returns>
		private long GetMillisecondsToTommorow() {
			//DateTime t =new DateTime(DateTime.Now.Ticks + ((DateTime.Today.AddDays(1).Ticks - DateTime.Now.Ticks)));
			return (86400000 - (DateTime.Now.Hour * 3600000 + DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond));
		}


		/// <summary>
		/// Déclenchement du timer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer_Tick(object sender, EventArgs e) {
			SetupTrace();
			CleanResults();
		}

		#region  Méthodes pour appels asynchrones

		/// <summary>
		/// Add  new line
		/// </summary>	
		private void MT_NewLines() {
			
			lock (listViewJobs) {
				listViewJobs.Items.Clear();
				lock (_requestList) {
					foreach (DataRow currentRow in _requestList.Rows) {
						listViewJobs.Items.Add(currentRow[0].ToString());
					}
				}
			}
		}

		/// <summary>
		/// Add  error message in list view 
		/// </summary>
		/// <param name="navSessionId">Session ID</param>
		/// <param name="message">Message</param>
		/// <param name="imageIndex">Image index</param>
		private void MT_ErrorJob(Int64 navSessionId, string message, int imageIndex) {
			listViewJobsError.Items.Add(navSessionId.ToString());
			AddListViewMessage(message, imageIndex);		
		}

		/// <summary>
		/// Add  stop message in list view 
		/// </summary>
		/// <param name="navSessionId">Session ID</param>
		/// <param name="message">Message</param>
		/// <param name="imageIndex">Image index</param>
		private void MT_StopJob(Int64 navSessionId, string message, int imageIndex) {
			listViewJobsDone.Items.Add(navSessionId.ToString());
			AddListViewMessage(message, imageIndex);
			nbJobslabel.Text = _distributionServer.RunningJobCount + " / " + _anubisConfiguration.JobsNumber;
			try {
				_trace.WriteLine(message + " ==> " + nbJobslabel.Text);
			}
			catch (System.Exception) { }
		}

		/// <summary>
		/// Add start job message in list view 
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="imageIndex">Image index</param>
		private void MT_StartJob(string message, int imageIndex) {
			MT_NewLines();
			AddListViewMessage(message,imageIndex);
			nbJobslabel.Text = _distributionServer.RunningJobCount + " / " + _anubisConfiguration.JobsNumber;
			try {
				_trace.WriteLine(message + " ==> " + nbJobslabel.Text);
			}
			catch (System.Exception) { }
		}

		/// <summary>
		/// Add warning message in list view 
		/// </summary>
		/// <param name="navSessionId">Session ID</param>
		/// <param name="message">Message</param>
		/// <param name="imageIndex">Image index</param>
		private void MT_WarningJob(Int64 navSessionId, string message, int imageIndex) {			
			AddListViewMessage(message, imageIndex);
		}

		/// <summary>
		/// Add list view message
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="imageIndex">Image index</param>
		private void AddListViewMessage(string message, int imageIndex) {
			lock (listViewMessage) {
				ListViewItem Entry = new ListViewItem("", imageIndex);
				Entry.SubItems.Add(message);
				listViewMessage.Items.Insert(0, Entry);
			}
		}
		#endregion

		#endregion
	}
}