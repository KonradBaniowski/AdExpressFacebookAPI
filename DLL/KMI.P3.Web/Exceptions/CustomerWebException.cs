using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using KMI.P3.Domain.Web;
using TNS.FrameWork;
using KMIMail = TNS.FrameWork.Net.Mail;
using KMI.P3.Web.Core.Sessions;
using WebConstantes = KMI.P3.Constantes.Web;
namespace KMI.P3.Web.Exceptions
{
    /// <summary>
    /// Classe gestion exceptions Avec envoie de mail lorsqu'il y a une erreur sur une page Web.
    /// Cette exception doit être lancé qui si l'utilisateur est authentifié.
    /// </summary>
    [Serializable]
    public class CustomerWebException : System.Exception
    {

        #region Variables
        /// <summary>
        /// Session du client
        /// </summary>
        protected WebSession webSession;
        /// <summary>
        /// Page Web qui lance l'erreur
        /// </summary>
        protected System.Web.UI.Page page;
        /// <summary>
        /// stackTrace
        /// </summary>
        protected string stackTrace;
        /// <summary>
        /// Nom du serveur où la page s'execute
        /// </summary>
        protected string serverName;
        /// <summary>
        /// Url demandée
        /// </summary>
        protected string url;
        /// <summary>
        /// Browser
        /// </summary>
        protected string browser;
        /// <summary>
        /// Version du Browser
        /// </summary>
        protected string versionBrowser;
        /// <summary>
        /// Sous version du browser
        /// </summary>
        protected string minorVersionBrowser;
        /// <summary>
        /// UserAgent
        /// </summary>
        protected string userAgent;
        /// <summary>
        /// Système d'exploitation
        /// </summary>
        protected string os;
        /// <summary>
        /// Adresse IP du client
        /// </summary>
        protected string userHostAddress;
        /// <summary>
        /// Platforme
        /// </summary>
        protected string platform;

        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        /// <param name="page">Page Web qui lance l'erreur</param>
        /// <param name="webSession">Session du client</param>
        public CustomerWebException(System.Web.UI.Page page, WebSession webSession)
            : base()
        {
            this.webSession = webSession;
            this.browser = page.Request.Browser.Browser;
            this.versionBrowser = page.Request.Browser.Version;
            this.minorVersionBrowser = page.Request.Browser.MinorVersion.ToString();
            this.os = page.Request.Browser.Platform;
            this.userAgent = page.Request.UserAgent;
            this.userHostAddress = page.Request.UserHostAddress;
            this.url = page.Request.Url.ToString();
            this.page = page;
            this.serverName = page.Server.MachineName;
            this.platform = page.Request.Browser.Platform;
        }

        /// <summary>
        /// Constructeur de base
        /// </summary>
        /// <param name="webSession">Session du client</param>
        public CustomerWebException(WebSession webSession)
            : base()
        {
            this.webSession = webSession;
            this.browser = webSession.Browser;
            this.versionBrowser = webSession.BrowserVersion;
            this.minorVersionBrowser = "";
            this.os = webSession.CustomerOs;
            this.userAgent = webSession.UserAgent;
            this.userHostAddress = webSession.CustomerIp;
            this.url = webSession.LastWebPage;
            this.page = null;
            this.serverName = webSession.ServerName;
            this.platform = webSession.CustomerOs;
        }


        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="page">Page Web qui lance l'erreur</param>
        /// <param name="message">Message d'erreur</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="stackTrace">stackTrace</param>
        public CustomerWebException(System.Web.UI.Page page, string message, string stackTrace, WebSession webSession)
            : base(message)
        {
            this.webSession = webSession;
            this.browser = page.Request.Browser.Browser;
            this.versionBrowser = page.Request.Browser.Version;
            this.minorVersionBrowser = page.Request.Browser.MinorVersion.ToString();
            this.os = page.Request.Browser.Platform;
            this.userAgent = page.Request.UserAgent;
            this.userHostAddress = page.Request.UserHostAddress;
            this.url = page.Request.Url.ToString();
            this.platform = page.Request.Browser.Platform;
            this.page = page;
            this.serverName = page.Server.MachineName;
            this.stackTrace = stackTrace;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="stackTrace">stackTrace</param>
        public CustomerWebException(string message, string stackTrace, WebSession webSession)
            : base(message)
        {
            this.webSession = webSession;
            this.browser = webSession.Browser;
            this.versionBrowser = webSession.BrowserVersion;
            this.minorVersionBrowser = "";
            this.os = webSession.CustomerOs;
            this.userAgent = webSession.UserAgent;
            this.userHostAddress = webSession.CustomerIp;
            this.url = webSession.LastWebPage;
            this.page = null;
            this.serverName = webSession.ServerName;
            this.platform = webSession.CustomerOs;
            this.stackTrace = stackTrace;
        }
        #endregion

        #region Méthodes internes
        /// <summary>
        /// Envoie un mail d'erreur
        /// </summary>
        public void SendMail()
        {
            string body = "";

            if (webSession == null)
            {
                body += "<html><b><u>" + serverName + ":</u></b><br>" + "<font color=#FF0000>Erreur client:<br></font>";
                body += "<hr>";
                body += "<u>Page demandée:</u><br><a href=" + url + ">" + url + "</a><br>";
                body += "<u>Navigateur:</u><br>" + browser + " " + versionBrowser + " " + minorVersionBrowser + "<br>" + userAgent + "<br>";
                body += "<u>Système d'exploitation:</u><br>" + os + "<br>" + userHostAddress + "<br>";
                body += "<u>Message d'erreur:</u><br>" + Message + "<br>";
                body += "<u>Source:</u><br>" + Source + "<br>";
                body += "<u>StackTrace:</u><br>" + stackTrace.Replace("at ", "<br>at ") + "<br>";
                body += "<hr>";
                body += "</html>";
            }
            else
            {
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);

                #region Identifiaction du client
                body += "<html><b><u>" + serverName + ":</u></b><br>" + "<font color=#FF0000>Erreur client:<br></font>";
                body += "Numéro de session: " + webSession.SessionId + "<br>";
                body += "Login: " + webSession.Login + "<br>";
                body += "Password: " + webSession.Password + "<br>";
                #endregion

                #region Message d'erreur
                body += "<hr>";
                body += "<u>Page demandée:</u><br><a href=" + url + ">" + url + "</a><br>";
                body += "<u>Navigateur:</u><br>" + browser + " " + versionBrowser + " " + minorVersionBrowser + "<br>" + userAgent + "<br>";
                body += "<u>Système d'exploitation:</u><br>" + platform + "<br>" + userHostAddress + "<br>";
                body += "<u>Message d'erreur:</u><br>" + Message + "<br>";
                body += "<u>Source:</u><br>" + Source + "<br>";
                if (stackTrace != null)
                    body += "<u>StackTrace:</u><br>" + stackTrace.Replace("at ", "<br>at ") + "<br>";
                body += "<hr>";
                #endregion

                body += "</html>";
            }
            KMIMail.SmtpUtilities errorMail = new KMIMail.SmtpUtilities(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ErrorManager.CUSTOMER_ERROR_MAIL_FILE);
            errorMail.SendWithoutThread("Erreur KantarMusic Client (" + serverName + ")", Convertion.ToHtmlString(body), true, false);

        }
        #endregion

    }
}
