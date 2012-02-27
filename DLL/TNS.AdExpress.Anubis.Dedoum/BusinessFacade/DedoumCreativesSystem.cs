using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Anubis.Dedoum.Exceptions;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Dedoum.Common;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using System.IO;
using System.Text.RegularExpressions;
using TNS.FrameWork.Net.Mail;
using TNS.AdExpress.Domain.Translation;
using System.Net.Mail;

namespace TNS.AdExpress.Anubis.Dedoum.BusinessFacade
{
    /// <summary>
    /// Generate zuip for creatives of Internet Evaliant
    /// </summary>
    public class DedoumCreativesSystem
    {
        #region Variables
        /// <summary>
        /// Source de données
        /// </summary>
        private IDataSource _dataSource = null;
        /// <summary>
        ///Bastet Configuration 
        /// </summary>
        private DedoumConfig _config = null;
        /// <summary>
        /// Customer Client request
        /// </summary>
        private DataRow _rqDetails = null;
        /// <summary>
        /// WebSession to process
        /// </summary>
        private WebSession _webSession = null;
        /// <summary>
        /// zip file path
        /// </summary>
        private string _zipFilePath;
        #endregion

        #region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
        public DedoumCreativesSystem(IDataSource dataSource, DedoumConfig config, DataRow rqDetails, WebSession webSession)
        {
            try {
                _dataSource = dataSource;
                _config = config;
                _rqDetails = rqDetails;
                _webSession = webSession;
            }
            catch (Exception e) {
                throw new DedoumException("Error in constructor DedoumCreativesSystem", e);
            }

		}
		#endregion

        #region Fill
        /// <summary>
        /// Generate zip file
        /// </summary>
        public bool Fill()
        {       
            return Rules.CreativesResult.ExportCreatives(_dataSource, _webSession, _rqDetails, _config, _zipFilePath);
        }
        #endregion


        #region Init
        /// <summary>
        /// Initialisation des paramètres du fichier
        /// </summary>
        internal string Init()
        {
            try
            {
                //string fName = GetFileName(_rqDetails, ref shortFName);
                 _zipFilePath = DateTime.Now.ToString("yyyyMMddHHmmss_") + _rqDetails["id_static_nav_session"].ToString()
                       + "_" + Guid.NewGuid();

                return _zipFilePath;

            }
            catch (Exception e)
            {
                throw (new DedoumException("Error initialization", e));
            }
        }
        #endregion

        #region Nom du fichier zip
        /// <summary>
        /// Generate a valid file name from customer request
        /// </summary>
        /// <param name="rqDetails">Details of the customer request</param>
        /// <param name="shortName">Return value : short name of the File (the method return the complet path)</param>
        /// <returns>Complet File Name String (path + short name)</returns>
        private string GetFileName(DataRow rqDetails, ref string shortName)
        {
            string zipFileName;

            try
            {

                zipFileName = this._config.CreativesPath;
                zipFileName += @"\" + rqDetails["ID_LOGIN"].ToString();

                if (!Directory.Exists(zipFileName))
                {
                    Directory.CreateDirectory(zipFileName);
                }
                shortName = DateTime.Now.ToString("yyyyMMddHHmmss_")
                    + rqDetails["id_static_nav_session"].ToString()
                    + "_"
                    + TNS.Ares.Functions.GetRandomString(30, 40);

                zipFileName += @"\" + shortName + ".zip";

                string checkPath = Regex.Replace(zipFileName, @"(\.zip)+", ".zip", RegexOptions.IgnoreCase | RegexOptions.Multiline);


                int i = 0;
                while (File.Exists(checkPath))
                {
                    if (i <= 1)
                    {
                        checkPath = Regex.Replace(zipFileName, @"(\.zip)+", "_" + (i + 1) + ".zip", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    }
                    else
                    {
                        checkPath = Regex.Replace(zipFileName, "(_" + i + @"\.zip)+", "_" + (i + 1) + ".zip", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    }
                    i++;
                }
                return checkPath;
            }
            catch (System.Exception e)
            {
                throw (new Exception("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));

            }
        }
        #endregion	

        #region Send
        /// <summary>
        /// Sned email to customer
        /// </summary>
        /// <param name="fileName">file name</param>
        internal void Send(string fileName, bool res)
        {
                MailMessage message = new MailMessage();

                try
                {
                    var to = new ArrayList();
                    foreach (string s in _webSession.EmailRecipient)
                    {
                        to.Add(s);
                    }
                    string body = (res) ? GestionWeb.GetWebWord(2936, _webSession.SiteLanguage) + "\" " +_webSession.ExportedPDFFileName
                                  + "\"" + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer) : GestionWeb.GetWebWord(2938, _webSession.SiteLanguage);

                    var mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                        GestionWeb.GetWebWord(2935, _webSession.SiteLanguage),
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
                    throw new DedoumException("Error to Send mail to client in Send(string fileName)", e);
                }
           

        }

    
        /// <summary>
        /// Rise exception when the customer mail has not been sent
        /// </summary>
        /// <param name="source">Error source></param>
        /// <param name="message">Error message</param>
        private void mail_mailKoHandler(object source, string message)
        {
            throw new DedoumException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
        }

        #endregion
    }
}
