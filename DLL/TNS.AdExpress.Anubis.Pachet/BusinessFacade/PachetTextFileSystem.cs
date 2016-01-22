using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using TNS.AdExpress.Anubis.Pachet.UI;
using System.Collections;
using TNS.AdExpress.Anubis.Pachet.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Pachet.Common;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Anubis.Pachet.BusinessFacade
{
    public class PachetTextFileSystem
    {
        #region Variables
		/// <summary>
		/// Source de données
		/// </summary>
		private IDataSource _dataSource = null;
		/// <summary>
		///Pachet Configuration 
		/// </summary>
		private PachetConfig _config = null;
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
        public PachetTextFileSystem(IDataSource dataSource, PachetConfig config, DataRow rqDetails, WebSession webSession)
        {
            try {
                this._dataSource = dataSource;
                this._config = config;
                this._rqDetails = rqDetails;
                this._webSession = webSession;
            }
            catch (Exception e) {
                throw new PachetTextFileSystemException("Error in constructor PachetTextFileSystem", e);
            }

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
                throw (new Exceptions.PachetTextFileSystemException("Erreur initialisation", e));
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
					+ TNS.Ares.Functions.GetRandomString(30,40);

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
		public bool  Fill(){
			string sepChar=" ";
            return InsertionsDetail.CreatePachetTextFile(_dataSource, _config, _webSession, _textFilePath);
		}
		#endregion

        #region Send
        /// <summary>
        /// Sned email to customer
        /// </summary>
        /// <param name="res">result</param>
        internal void Send( bool res)
        {


            try
            {
                var to = new ArrayList();
                foreach (string s in _webSession.EmailRecipient)
                {
                    to.Add(s);
                }
                string body = (res) ? GestionWeb.GetWebWord(2945, _webSession.SiteLanguage) + "\" " + _webSession.ExportedPDFFileName
                              + "\"" + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer) : GestionWeb.GetWebWord(2106, _webSession.SiteLanguage);

                var mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                    GestionWeb.GetWebWord(2944, _webSession.SiteLanguage),
                    body
                    + "<br><br>"
                    + GestionWeb.GetWebWord(1776, _webSession.SiteLanguage),
                    true, _config.CustomerMailServer, _config.CustomerMailPort);
                try
                {
                    mail.SubjectEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                    mail.BodyEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                    string charset = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset;
                    if (!string.IsNullOrEmpty(charset))
                    {
                        mail.CharsetTextHtml = charset;
                        mail.CharsetTextPlain = charset;
                    }
                }
                catch (Exception) { }
                mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
                mail.SendWithoutThread(false);


            }
            catch (System.Exception e)
            {
                throw new PachetTextFileSystemException("Error to Send mail to client in Send(string fileName)", e);
            }


        }



        #endregion

		#region Evenement Envoi mail client
		/// <summary>
		/// Rise exception when the customer mail has not been sent
		/// </summary>
		/// <param name="source">Error source></param>
		/// <param name="message">Error message</param>
		private void mail_mailKoHandler(object source, string message) {
            throw (new PachetTextFileSystemException("Echec lors de l'envoi mail client pour l'export texte des insertions  : " + message));
		}
		#endregion
    }


}
