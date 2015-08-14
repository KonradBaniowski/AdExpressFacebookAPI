#region Informations
// Auteur : B.Masson
// Date de création : 12/04/2006
// Date de modification :
#endregion

#region Namespaces
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Data;
using System.IO;
using Oracle.DataAccess.Client;
using Framework=TNS.FrameWork;
using FrameworkExceptions=TNS.FrameWork.Exceptions;
using FrameworkNet=TNS.FrameWork.Net;
using FrameworkError=TNS.FrameWork.Error;
using FrameworkDBBusiness = TNS.FrameWork.DB.BusinessFacade;
using GebConfiguration=TNS.AdExpress.Geb.Configuration;
#endregion

namespace Geb{
	/// <summary>
	/// Fenêtre principale de l'application
	/// </summary>
	public class MainForm : System.Windows.Forms.Form{

		#region Constantes
		/// <summary>
		/// Chemin du répertoire contenant les fichiers de logs
		/// </summary>
		private const string LOG_PATH=@"Logs\";
		/// <summary>
		/// Chemin du répertoire contenant les fichiers de configuration
		/// </summary>
		private const string CONFIGURATION_PATH=@"Config\";
		/// <summary>
		/// Nom du fichier de configuration du mail pour l'envoi du log
		/// </summary>
		private const string MAIL_CONFIGURATION_FILE_NAME="Mail.xml";
		/// <summary>
		/// Nom du fichier de configuration
		/// </summary>
		private const string APPLICATION_CONFIGURATION_FILE_NAME="Application.xml";
		#endregion

		#region Variables
		/// <summary>
		/// Répertoire de l'application
		/// </summary>
		protected string _appPath = AppDomain.CurrentDomain.BaseDirectory;
		/// <summary>
		/// Mail qui envoie le log
		/// </summary>
		protected FrameworkNet.Mail.SmtpUtilities _errMail;
		/// <summary>
		/// Fichier d'erreur
		/// </summary>
		protected FrameworkError.Log.XmlWriter _errorXML;
		/// <summary>
		/// Une erreur est survenue
		/// </summary>
		protected bool _error = false;
		/// <summary>
		/// Configuration de l'application
		/// </summary>
		protected GebConfiguration.Application _applicationConfiguration = null;
		/// <summary>
		/// Source de données
		/// </summary>
		protected TNS.FrameWork.DB.Common.IDataSource _source = null;
		/// <summary>
		/// Geb server
		/// </summary>
		protected TNS.AdExpress.Geb.GebServer _server=null;
		#endregion

		#region Variables MMI
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListView _alertsListView;
		private System.Windows.Forms.MenuItem _fileMenuItem;
		private System.Windows.Forms.MainMenu _mainMenu;
		private System.Windows.Forms.MenuItem _quitMenuItem;
		private System.Windows.Forms.ColumnHeader taskColumnHeader;
		#endregion

        #region Délégués
        /// <summary>
        ///  Message 
        /// </summary>
        public delegate void MessageCallback(string message);
        /// <summary>
        /// Message d'erreur
        /// </summary>
        public delegate void ErrorMessageCallback(string message);
        #endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MainForm(){
			// Traduction
			//TNS.Geb.Localization.GebServer.CurrentCultureInfo = new System.Globalization.CultureInfo("fr");

			#region Initialisation du mail
			try{
				_errMail=new FrameworkNet.Mail.SmtpUtilities(_appPath+CONFIGURATION_PATH+MAIL_CONFIGURATION_FILE_NAME);
			}
			catch(System.Exception){
				Application.Exit();
			}
			#endregion

			#region Chargement de la configuration de l'application
			try{
				_applicationConfiguration = TNS.AdExpress.Geb.DataAccess.Configuration.ApplicationDataAccess.Get(_appPath+CONFIGURATION_PATH+APPLICATION_CONFIGURATION_FILE_NAME);
			}
			catch(System.Exception){
				Application.Exit();
			}
			#endregion

			#region Chargement de la connexion
			try{
				TNS.FrameWork.DB.Common.Oracle.DataBaseConfiguration connection = FrameworkDBBusiness.Oracle.DataBaseConfigurationBussinessFacade.GetOne(_appPath+CONFIGURATION_PATH+APPLICATION_CONFIGURATION_FILE_NAME);
				_source = new TNS.FrameWork.DB.Common.OracleDataSource(connection.ConnectionString);
			}
			catch(System.Exception ex){
				Application.Exit();
			}
			#endregion

			#region Création du log d'erreur
			try{
				if (!Directory.Exists(_appPath+LOG_PATH)) Directory.CreateDirectory(_appPath+LOG_PATH);
				
				DateTime dt = DateTime.Now;
				string dateFormat = dt.Year.ToString();
				if (dt.Month < 10) dateFormat += "0"+dt.Month.ToString();
				else dateFormat += dt.Month.ToString();
				if (dt.Day < 10) dateFormat += "0"+dt.Day.ToString();
				else dateFormat += dt.Day.ToString();
				if (dt.Hour < 10) dateFormat += "0"+dt.Hour.ToString();
				else dateFormat += dt.Hour.ToString();
				if (dt.Minute < 10) dateFormat += "0"+dt.Minute.ToString();
				else dateFormat += dt.Minute.ToString();
				if (dt.Second < 10) dateFormat += "0"+dt.Second.ToString();
				else dateFormat += dt.Second.ToString();

				_errorXML=new FrameworkError.Log.XmlWriter(_appPath+LOG_PATH+@"Log_"+Environment.MachineName+"_"+dateFormat+".xml");
				Console.SetError(_errorXML);
			}
			catch(System.Exception err){
				_errMail.SendWithoutThread(AppDomain.CurrentDomain.FriendlyName,"<font color=#FF0000>Impossible de créer le fichier de log</font><br>"+err.Message,true,false);
				Application.Exit();
			}
			#endregion

			InitializeComponent();
			Console.Error.WriteLine("Initialisation terminée");

			this.Text = TNS.Geb.Localization.GebServer._mainFormTitle();
			_fileMenuItem.Text = TNS.Geb.Localization.GebServer._fileMenuItem();
			_quitMenuItem.Text = TNS.Geb.Localization.GebServer._quitMenuItem();
		}
		#endregion

		#region Destructeur
		/// <summary>
		/// Nettoyage des ressources utilisées.
		/// </summary>
		protected override void Dispose( bool disposing ){
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Code généré par le Concepteur Windows Form
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){
			this._alertsListView = new System.Windows.Forms.ListView();
			this.taskColumnHeader = new System.Windows.Forms.ColumnHeader();
			this._mainMenu = new System.Windows.Forms.MainMenu();
			this._fileMenuItem = new System.Windows.Forms.MenuItem();
			this._quitMenuItem = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// _alertsListView
			// 
			this._alertsListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this._alertsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							  this.taskColumnHeader});
			this._alertsListView.FullRowSelect = true;
			this._alertsListView.GridLines = true;
			this._alertsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._alertsListView.Location = new System.Drawing.Point(8, 8);
			this._alertsListView.MultiSelect = false;
			this._alertsListView.Name = "_alertsListView";
			this._alertsListView.Size = new System.Drawing.Size(480, 424);
			this._alertsListView.TabIndex = 1;
			this._alertsListView.View = System.Windows.Forms.View.Details;
			// 
			// taskColumnHeader
			// 
			this.taskColumnHeader.Text = "Task";
			this.taskColumnHeader.Width = 490;
			// 
			// _mainMenu
			// 
			this._mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this._fileMenuItem});
			// 
			// _fileMenuItem
			// 
			this._fileMenuItem.Index = 0;
			this._fileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this._quitMenuItem});
			this._fileMenuItem.Text = "File";
			// 
			// _quitMenuItem
			// 
			this._quitMenuItem.Index = 0;
			this._quitMenuItem.Text = "Quit";
			this._quitMenuItem.Click += new System.EventHandler(this._quitMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(494, 440);
			this.ControlBox = false;
			this.Controls.Add(this._alertsListView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Menu = this._mainMenu;
			this.Name = "MainForm";
			this.Text = "Push Mail Alerts Update";
			this.ResumeLayout(false);

		}
		#endregion

		#region Lancement de l'application
		/// <summary>
		/// Point d'entrée principal de l'application
		/// </summary>
		[STAThread]
		static void Main() {
			MainForm program = new MainForm();
			program.Compute();
			Application.Run(program);
		}
		#endregion

		#region Traitement de l'application
		/// <summary>
		/// Traitement
		/// </summary>
		private void Compute(){
			_server = new TNS.AdExpress.Geb.GebServer(_source, _applicationConfiguration);
			// Instancie les évènement du serveur
			_server.OnStartServer+=new TNS.AdExpress.Geb.GebServer.StartServer(server_OnStartServer);
			_server.OnStopServer+=new TNS.AdExpress.Geb.GebServer.StopServer(server_OnStopServer);
			// Instancie les évènement des messages
			_server.OnNewMedia+=new TNS.AdExpress.Geb.GebServer.NewMedia(server_OnNewMedia);
			_server.OnNewAlert+=new TNS.AdExpress.Geb.GebServer.NewAlert(server_OnNewAlert);
			_server.OnMessage+=new TNS.AdExpress.Geb.GebServer.Message(server_OnMessage);
			_server.OnErrorMessage+=new TNS.AdExpress.Geb.GebServer.ErrorMessage(server_OnErrorMessage);
			_server.OnError+=new TNS.AdExpress.Geb.GebServer.Error(server_OnError);
			// Appel de la méthode de traitement
			_server.Treatement();
		}
		#endregion

		#region Evènements

		#region Server
		/// <summary>
		/// Démarrage du serveur
		/// </summary>
		/// <param name="message">Message</param>
		private void server_OnStartServer(string message) {
			WriteMessage("(StartServer) : "+message);
		}

		/// <summary>
		/// Arrêt du serveur
		/// </summary>
		/// <param name="message">Message</param>
		private void server_OnStopServer(string message) {
			WriteMessage("(StopServer) : "+message);
			SendLogErrorFile(); // Envoi du mail avec le fichier de log d'erreur
		}

		/// <summary>
		/// Nouveau support
		/// </summary>
		/// <param name="message">Message</param>
		private void server_OnNewMedia(string message) {
			WriteMessage("(NewMedia) : "+message);
		}

		/// <summary>
		/// Nouvelle alerte
		/// </summary>
		/// <param name="message">Message</param>
		private void server_OnNewAlert(string message) {
			WriteMessage("(NewAlert) : "+message);
		}

		/// <summary>
		/// Message
		/// </summary>
		/// <param name="message">Message</param>
		private void server_OnMessage(string message) {
			WriteMessage("(Message) : "+message);
		}

		/// <summary>
		/// Message d'erreur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		private void server_OnErrorMessage(string message) {
			WriteErrorMessage(message);			
		}

		/// <summary>
		/// Message d'erreur avec exception
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="err">Exception</param>
		private void server_OnError(string message, Exception err) {
			string msg=message+"<br>";
			try{
				TNS.FrameWork.Exceptions.BaseException baseErr=(TNS.FrameWork.Exceptions.BaseException)err;
				msg+=baseErr.GetHtmlDetail();
			}
			catch(System.Exception){
				msg+=err.Message;
			}
			WriteErrorMessage(msg);
		}
		#endregion

		#region Menu
		/// <summary>
		/// Quitte l'application
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Argument</param>
		private void _quitMenuItem_Click(object sender, System.EventArgs e) {
			// Arrêt du server avec attente de la thread en cours
			_server.StopGebServer();

			if(_server.RunningJobCount > 0)
				new Exit(_server).ShowDialog();

			// Fin brutal de la thread en cours
			_server.AbortGebServer();

			// Log writing
			Console.Error.WriteLine("Stop geb server");
			_errorXML.Close();
			
			// Fermeture de l'application
			this.Close();
		}
		#endregion

		#endregion

		#region Méthodes privées
		/// <summary>
		/// Ecrit un message dans la liste et le ficher de log
		/// </summary>
		/// <param name="message">Message</param>
		private void WriteMessage(string message) {

            #region Old version
            //lock(_alertsListView){
            //    ListViewItem Entry=new ListViewItem(message,0);
            //    Entry.SubItems.Add(message);
            //    _alertsListView.Items.Insert(0,Entry);
            //}
            //Console.Error.WriteLine(message);
            #endregion

            if(this.InvokeRequired) {
                MessageCallback d = new MessageCallback(WriteMessage);
                this.Invoke(d, new object[] { message });
            }
            else {
                lock(_alertsListView) {
                    ListViewItem Entry = new ListViewItem(message, 0);
                    Entry.SubItems.Add(message);
                    _alertsListView.Items.Insert(0, Entry);
                }
                Console.Error.WriteLine(message);
            }

        }

		/// <summary>
		/// Ecrit un message dans la liste et le ficher de log
		/// </summary>
		/// <param name="message">Message</param>
		private void WriteErrorMessage(string message) {

            #region Old version
            //lock(_alertsListView){
            //    string[] msg = new string[1];
            //    msg[0] = "(Error) : "+message;
            //    ListViewItem Entry=new ListViewItem(msg,0,System.Drawing.Color.White,System.Drawing.Color.Red,null);
            //    Entry.SubItems.Add(message);
            //    _alertsListView.Items.Insert(0,Entry);
            //}
            //Console.Error.WriteLine("Message d'erreur : "+message);
            //ErrorManagement(message);
            #endregion

            if(this.InvokeRequired) {
                ErrorMessageCallback d = new ErrorMessageCallback(WriteErrorMessage);
                this.Invoke(d, new object[] { message });
            }
            else {
                lock(_alertsListView) {
                    string[] msg = new string[1];
                    msg[0] = "(Error) : " + message;
                    ListViewItem Entry = new ListViewItem(msg, 0, System.Drawing.Color.White, System.Drawing.Color.Red, null);
                    Entry.SubItems.Add(message);
                    _alertsListView.Items.Insert(0, Entry);
                }
                Console.Error.WriteLine("Message d'erreur : " + message);
                ErrorManagement(message);
            }

        }

		/// <summary>
		/// Envoie le mail contenant le fichier de log d'erreur
		/// </summary>
		private void SendLogErrorFile(){
			_errorXML.Close();
			_errMail.Attach(_errorXML.FilePath,FrameworkNet.Mail.SmtpUtilities.AttachmentType.ATTACH_OTHER);
			_errMail.SendWithoutThread("Geb Server - STOP SERVER (" + Environment.MachineName + ")",Framework.Convertion.ToHtmlString("Arrêt manuel du server Geb"),true,false);
		}

		/// <summary>
		/// Envoi le mail d'erreur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		private void ErrorManagement(string message){
			// Mail sending
			try{
				(new FrameworkNet.Mail.SmtpUtilities(_appPath+CONFIGURATION_PATH+MAIL_CONFIGURATION_FILE_NAME))
					.SendWithoutThread("Geb Server Error  (" + Environment.MachineName + ")",Framework.Convertion.ToHtmlString(message).Replace("at ","<br>at "),true,false) ;
			}
			catch(System.Exception err){
				try{
					_errorXML.WriteLine("Unable to send mail for " + message);
				}
				catch(System.Exception){}
			}
			// Log writing
			try{
				_errorXML.WriteLine(message);
			}
			catch(System.Exception err){
				try{
					(new FrameworkNet.Mail.SmtpUtilities(_appPath+CONFIGURATION_PATH+MAIL_CONFIGURATION_FILE_NAME))
						.SendWithoutThread("Geb Server Error (" + Environment.MachineName + ")", Framework.Convertion.ToHtmlString("Unable to write log"+ "<br>Mail sending error :<br>" + err.Message + " - " + err.StackTrace + "<br><br>Message to send:<br>" + message).Replace("at ","<br>at "),true,false) ;
				}
				catch(System.Exception){}
			}
		}
		#endregion

	}
}
