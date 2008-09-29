using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;

using TNS.FrameWork;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.Net.Mail;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Anubis.Sobek.UI;
using TNS.AdExpress.Anubis.Sobek.Common;

namespace TNS.AdExpress.Anubis.Sobek.BusinessFacade
{
	/// <summary>
	/// Description résumée de SobekTextFileSystem.
	/// </summary>
	public class SobekTextFileSystem
	{
		#region Variables
		/// <summary>
		/// Source de données
		/// </summary>
		private IDataSource _dataSource = null;
		/// <summary>
		///Bastet Configuration 
		/// </summary>
		private SobekConfig _config = null;
		/// <summary>
		/// Customer Client request
		/// </summary>
		private DataRow _rqDetails = null;
		/// <summary>
		/// WebSession to process
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Chemin du fichier texte généré
		/// </summary>
		private string _textFilePath;
		#endregion
		
		#region Accesseurs
		/// <summary>
		/// Chemin du fichier texte généré
		/// </summary>
		public string TextFilePath{
			get{return _textFilePath;}
		}
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public SobekTextFileSystem(IDataSource dataSource, SobekConfig config,DataRow rqDetails, WebSession webSession){
		
			this._dataSource = dataSource;		
			this._config = config;
			this._rqDetails = rqDetails;
			this._webSession = webSession;

		}
		#endregion


		#region Init
		/// <summary>
		/// Initialisation des paramètres du fichier
		/// </summary>
		internal string Init(){
			try{
				string shortFName = "";
				string fName =  GetFileName(_rqDetails, ref shortFName);	
//				_textFilePath = AppDomain.CurrentDomain.BaseDirectory+fName;
				_textFilePath = fName;
		
				return shortFName;
				
			}
			catch(Exception e){
				throw(new Exceptions.SobekTextFileSystemException("Erreur initialisation",e));
			}
		}
		#endregion

		#region Méthodes privées
		
		#region Nom du fichier texte
		/// <summary>
		/// Generate a valid file name from customer request
		/// </summary>
		/// <param name="rqDetails">Details of the customer request</param>
		/// <param name="shortName">Return value : short name of the File (the method return the complet path)</param>
		/// <returns>Complet File Name String (path + short name)</returns>
		private string GetFileName(DataRow rqDetails,ref string shortName){
			string txtFileName;

			try{

				txtFileName = this._config.TextFilePath;
				txtFileName += @"\" + rqDetails["ID_LOGIN"].ToString() ;

				if(!Directory.Exists(txtFileName)){
					Directory.CreateDirectory(txtFileName);
				}
				shortName = DateTime.Now.ToString("yyyyMMddHHmmss_")
					+ rqDetails["id_static_nav_session"].ToString()
					+ "_"
					+ TNS.AdExpress.Anubis.Common.Functions.GetRandomString(30,40);

				txtFileName += @"\" + shortName + ".txt";

				string checkPath = 	Regex.Replace(txtFileName, @"(\.txt)+", ".txt", RegexOptions.IgnoreCase | RegexOptions.Multiline);


				int i = 0;
				while(System.IO.File.Exists(checkPath)){
					if(i<=1){
						checkPath = Regex.Replace(txtFileName, @"(\.txt)+", "_"+(i+1)+".txt", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					else{
						checkPath = Regex.Replace(txtFileName, "(_"+i+@"\.txt)+", "_"+(i+1)+".txt", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					i++;
				}
				return checkPath;
			}
			catch(System.Exception e){
				throw(new Exception("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() +".",e));

			}
		}
		#endregion	

		#endregion

		#region Fill
		/// <summary>
		/// Génère les insertions dans un fichier texte 
		/// </summary>
		public void Fill(){
			string sepChar=";";
			InsertionsDetail.CreateSobekTextFile(_dataSource,_config,_webSession,_textFilePath,sepChar);
		}
		#endregion

		
		#region Send
		/// <summary>
		/// Envoie le mail à l'utilisateur avec le fichier excel attaché
		/// </summary>
		/// <param name="fileName"></param>
		internal void Send(){
			ArrayList to = new ArrayList();
			foreach(string s in _webSession.EmailRecipient){
				to.Add(s);
			}
//			to.Add("dede.mussuma@tnsmi.fr");//test
			SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
				Text.SuppressAccent(GestionWeb.GetWebWord(1917,_webSession.SiteLanguage)),
				Text.SuppressAccent(GestionWeb.GetWebWord(1918,_webSession.SiteLanguage)+"\" "+_webSession.ExportedPDFFileName
				+ "\""+String.Format(GestionWeb.GetWebWord(1751,_webSession.SiteLanguage),_config.WebServer)				
				+ "<br><br>"
				+ GestionWeb.GetWebWord(1776,_webSession.SiteLanguage)),
				true, _config.CustomerMailServer, _config.CustomerMailPort);
			
			mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
//			mail.Attach(_textFilePath,SmtpUtilities.AttachmentType.ATTACH_TEXT);// Attache le fichier texte
			mail.SendWithoutThread(false);
		}
		#endregion

		#region Evenement Envoi mail client
		/// <summary>
		/// Rise exception when the customer mail has not been sent
		/// </summary>
		/// <param name="source">Error source></param>
		/// <param name="message">Error message</param>
		private void mail_mailKoHandler(object source, string message) {
			throw(new Exceptions.SobekTextFileSystemException("Echec lors de l'envoi mail client pour l'export texte des insertions  : " + message));
		}
		#endregion

	}
}
