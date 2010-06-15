#region Informations
// Auteur : B.Masson
// Date de création : 12/02/2007
// Date de modification :
#endregion

#region Namespaces
using System;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using Oracle.DataAccess.Client;

using TNS.FrameWork.DB.Common;
using Framework=TNS.FrameWork;
using FrameworkDBBusiness = TNS.FrameWork.DB.BusinessFacade;
using FrameworkExceptions=TNS.FrameWork.Exceptions;
using FrameworkNet=TNS.FrameWork.Net;
using FrameworkError=TNS.FrameWork.Error;
#endregion

namespace HermesScheduleServer{
	/// <summary>
	/// Fenêtre principale de l'application (Hermes Schedule Server)
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
		private const string CONFIGURATION_PATH=@"Configuration\";
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
		/// Source de données
		/// </summary>
		protected TNS.FrameWork.DB.Common.IDataSource _source = null;
		/// <summary>
		/// Hermes Schedule Server
		/// </summary>
		protected TNS.AdExpress.Hermes.ScheduleServer.HermesScheduleServer _hermesScheduleServer=null;
		/// <summary>
		/// Configuration de l'application
		/// </summary>
		protected TNS.AdExpress.Hermes.ScheduleServer.Configuration.Application  _applicationConfiguration = null;
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

		#region Variables MMI
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ListView _listView;
		private System.Windows.Forms.MainMenu _mainMenu;
		private System.Windows.Forms.MenuItem _fileMenuItem;
		private System.Windows.Forms.MenuItem _quitMenuItem;
		private System.Windows.Forms.ColumnHeader taskColumnHeader;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MainForm(){

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
				_applicationConfiguration = TNS.AdExpress.Hermes.ScheduleServer.DataAccess.ApplicationDataAccess.Load(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+@"Configuration\Application.xml"));
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
			catch(System.Exception){
				Application.Exit();
			}
			#endregion

			#region Création du log d'erreur
			try{
				if (!Directory.Exists(_appPath+LOG_PATH)) Directory.CreateDirectory(_appPath+LOG_PATH);
				
//				DateTime dt = DateTime.Now;
//				string dateFormat = dt.Year.ToString();
//				if (dt.Month < 10) dateFormat += "0"+dt.Month.ToString();
//				else dateFormat += dt.Month.ToString();
//				if (dt.Day < 10) dateFormat += "0"+dt.Day.ToString();
//				else dateFormat += dt.Day.ToString();
//				if (dt.Hour < 10) dateFormat += "0"+dt.Hour.ToString();
//				else dateFormat += dt.Hour.ToString();
//				if (dt.Minute < 10) dateFormat += "0"+dt.Minute.ToString();
//				else dateFormat += dt.Minute.ToString();
//				if (dt.Second < 10) dateFormat += "0"+dt.Second.ToString();
//				else dateFormat += dt.Second.ToString();

				string dateFormat = DateTime.Now.ToString("yyyyMMddHHmmss");

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

			//this.Text="Hermes Schedule Server ("+Application.ProductVersion.ToString()+")";
		}
		#endregion

		#region Destructeur
		/// <summary>
		/// Nettoyage des ressources utilisées.
		/// </summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if (components != null) {
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
		private void InitializeComponent()
		{
			this._listView = new System.Windows.Forms.ListView();
			this.taskColumnHeader = new System.Windows.Forms.ColumnHeader();
			this._mainMenu = new System.Windows.Forms.MainMenu();
			this._fileMenuItem = new System.Windows.Forms.MenuItem();
			this._quitMenuItem = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// _listView
			// 
			this._listView.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this._listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.taskColumnHeader});
			this._listView.FullRowSelect = true;
			this._listView.GridLines = true;
			this._listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._listView.Location = new System.Drawing.Point(8, 8);
			this._listView.MultiSelect = false;
			this._listView.Name = "_listView";
			this._listView.Size = new System.Drawing.Size(480, 424);
			this._listView.TabIndex = 0;
			this._listView.View = System.Windows.Forms.View.Details;
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
			this.Controls.Add(this._listView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Menu = this._mainMenu;
			this.Name = "MainForm";
			this.Text = "Hermes Schedule Server";
			this.ResumeLayout(false);

		}
		#endregion

		#region Lancement de l'application
		/// <summary>
		/// Point d'entrée principal de l'application.
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
			// Instancie Hermes Schedule Server
			_hermesScheduleServer = new TNS.AdExpress.Hermes.ScheduleServer.HermesScheduleServer(_source,_applicationConfiguration);

			// Instancie les évènement du serveur
			_hermesScheduleServer.OnStartServer	+= new TNS.AdExpress.Hermes.ScheduleServer.HermesScheduleServer.StartServer(server_OnStartServer);
			_hermesScheduleServer.OnStopServer	+= new TNS.AdExpress.Hermes.ScheduleServer.HermesScheduleServer.StopServer(server_OnStopServer);

			// Instancie les évènement des messages
			_hermesScheduleServer.OnNewRule		+= new TNS.AdExpress.Hermes.ScheduleServer.HermesScheduleServer.NewRule(server_OnNewRule);
			_hermesScheduleServer.OnMessage		+= new TNS.AdExpress.Hermes.ScheduleServer.HermesScheduleServer.Message(server_OnMessage);
			_hermesScheduleServer.OnErrorMessage+= new TNS.AdExpress.Hermes.ScheduleServer.HermesScheduleServer.ErrorMessage(server_OnErrorMessage);
			_hermesScheduleServer.OnError		+= new TNS.AdExpress.Hermes.ScheduleServer.HermesScheduleServer.Error(server_OnError);

			// Appel de la méthode de traitement
			_hermesScheduleServer.Treatement();
		}
		#endregion

		#region Evènements

		#region Hermes Schedule Server
		/// <summary>
		/// Démarrage du serveur
		/// </summary>
		/// <param name="message">Message</param>
		private void server_OnStartServer(string message) {
			//WriteMessage("(StartServer) : "+message);
			MT_WriteMessage("(StartServer) : "+message);			
		}

		/// <summary>
		/// Arrêt du serveur
		/// </summary>
		/// <param name="message">Message</param>
		private void server_OnStopServer(string message) {
			//WriteMessage("(StopServer) : "+message);
			MT_WriteMessage("(StopServer) : "+message);
			SendLogErrorFile(); // Envoi du mail avec le fichier de log d'erreur
		}

		/// <summary>
		/// Nouvelle règle
		/// </summary>
		/// <param name="message">Message</param>
		private void server_OnNewRule(string message) {
			//WriteMessage("(NewRule) : "+message);
			MT_WriteMessage("(NewRule) : "+message); 
		}

		/// <summary>
		/// Message
		/// </summary>
		/// <param name="message">Message</param>
		private void server_OnMessage(string message) {
			MT_WriteMessage("(Message) : "+message);
		}

		/// <summary>
		/// Message d'erreur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		private void server_OnErrorMessage(string message) {
			MT_WriteErrorMessage(message);	
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
//			WriteErrorMessage(msg);
			MT_WriteErrorMessage(msg);
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
			_hermesScheduleServer.StopHermesServer();

			if(_hermesScheduleServer.RunningJobCount > 0)
				new Exit(_hermesScheduleServer).ShowDialog();

			// Fin brutal de la thread en cours
			_hermesScheduleServer.AbortHermesServer();

			// Log writing
			Console.Error.WriteLine("Stop Hermes Schedule Server by user");
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
		private void MT_WriteMessage(string message){
			#region Old version
			//lock(_listView){
			//			ListViewItem Entry = new ListViewItem(message,0);
			//			Entry.SubItems.Add(message);
			//			_listView.Items.Insert(0,Entry);
			//			}
			//			Console.Error.WriteLine(message);
			#endregion

														
			if(this.InvokeRequired){
				MessageCallback d = new MessageCallback(MT_WriteMessage);											
				this.Invoke(d,new object[] { message });									
			}else{		
				lock(_listView){
					ListViewItem Entry=new ListViewItem(message,0);
					Entry.SubItems.Add(message);
					_listView.Items.Insert(0,Entry);
				}
				Console.Error.WriteLine(message);
			}
			
			
		}

		/// <summary>
		/// Ecrit un message dans la liste et le ficher de log
		/// </summary>
		/// <param name="message">Message</param>
		private void MT_WriteErrorMessage(string message){
			#region Old version
			//lock(_listView){
			//			string[] msg = new string[1];
			//			msg[0] = "(Error) : "+message;
			//			ListViewItem Entry=new ListViewItem(msg,0,System.Drawing.Color.White,System.Drawing.Color.Red,null);
			//			Entry.SubItems.Add(message);
			//			_listView.Items.Insert(0,Entry);
			//			}
			//			Console.Error.WriteLine("Message d'erreur : "+message);
			//			ErrorManagement(message);
			#endregion

	
			if(this.InvokeRequired){				
				ErrorMessageCallback d = new ErrorMessageCallback(MT_WriteErrorMessage);
				this.Invoke(d,new object[] { message });				
			}else{
				lock(_listView){
					string[] msg  = new string[1];
					msg[0] = "(Error) : "+message;
					ListViewItem Entry =new ListViewItem(msg,0,System.Drawing.Color.White,System.Drawing.Color.Red,null);
					Entry.SubItems.Add(message);
					_listView.Items.Insert(0,Entry);
				}
				Console.Error.WriteLine("Message d'erreur : "+message);
				ErrorManagement(message);
			}
						
		}

		/// <summary>
		/// Envoie le mail contenant le fichier de log d'erreur
		/// </summary>
		private void SendLogErrorFile(){
			_errorXML.Close();
			_errMail.Attach(_errorXML.FilePath,FrameworkNet.Mail.SmtpUtilities.AttachmentType.ATTACH_OTHER);
			_errMail.SendWithoutThread("Hermes Schedule Server - STOP SERVER (" + Environment.MachineName + ")",Framework.Convertion.ToHtmlString("Arrêt manuel de Hermes Schedule Server"),true,false);
		}

		/// <summary>
		/// Envoi le mail d'erreur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		private void ErrorManagement(string message){
			// Mail sending
			try{
				(new FrameworkNet.Mail.SmtpUtilities(_appPath+CONFIGURATION_PATH+MAIL_CONFIGURATION_FILE_NAME))
					.SendWithoutThread("Hermes Schedule Server (" + Environment.MachineName + ")",Framework.Convertion.ToHtmlString(message).Replace("at ","<br>at "),true,false) ;
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
						.SendWithoutThread("Hermes Schedule Server Error (" + Environment.MachineName + ")", Framework.Convertion.ToHtmlString("Unable to write log"+ "<br>Mail sending error :<br>" + err.Message + " - " + err.StackTrace + "<br><br>Message to send:<br>" + message).Replace("at ","<br>at "),true,false) ;
				}
				catch(System.Exception){}
			}
		}
		#endregion

	}
}
