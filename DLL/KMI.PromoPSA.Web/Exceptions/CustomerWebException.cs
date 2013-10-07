using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using KMI.PromoPSA.Web.Core.Sessions;
using TNS.FrameWork;
using TNSMail = TNS.FrameWork.Net.Mail;
using Cste = KMI.PromoPSA.Constantes;
using KMI.PromoPSA.Web.Domain;

namespace KMI.PromoPSA.Web.Exceptions
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
        protected WebSession _webSession;
        /// <summary>
        /// Page Web qui lance l'erreur
        /// </summary>
        protected System.Web.UI.Page _page;
        /// <summary>
        /// stackTrace
        /// </summary>
        protected string _stackTrace;
        /// <summary>
        /// Nom du serveur où la page s'execute
        /// </summary>
        protected string _serverName;
        /// <summary>
        /// Url demandée
        /// </summary>
        protected string _url;
        /// <summary>
        /// Url demandée Ajax
        /// </summary>
        protected string _urlAjax;
        /// <summary>
        /// Browser
        /// </summary>
        protected string _browser;
        /// <summary>
        /// Version du Browser
        /// </summary>
        protected string _versionBrowser;
        /// <summary>
        /// Sous version du browser
        /// </summary>
        protected string _minorVersionBrowser;
        /// <summary>
        /// UserAgent
        /// </summary>
        protected string _userAgent;
        /// <summary>
        /// Système d'exploitation
        /// </summary>
        protected string _os;
        /// <summary>
        /// Adresse IP du client
        /// </summary>
        protected string _userHostAddress;
        /// <summary>
        /// Platforme
        /// </summary>
        protected string _platform;

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
            this._webSession = webSession;
            this._browser = page.Request.Browser.Browser;
            this._versionBrowser = page.Request.Browser.Version;
            this._minorVersionBrowser = page.Request.Browser.MinorVersion.ToString();
            this._os = page.Request.Browser.Platform;
            this._userAgent = page.Request.UserAgent;
            this._userHostAddress = page.Request.UserHostAddress;
            this._url = page.Request.Url.ToString();
            if (webSession.CurrentHttpContext.Request.Url.ToString() != System.Web.HttpContext.Current.Request.Url.ToString())
                this._urlAjax = System.Web.HttpContext.Current.Request.Url.ToString();
            else
                this._urlAjax = null;
            this._page = page;
            this._serverName = page.Server.MachineName;
            this._platform = page.Request.Browser.Platform;
        }

        /// <summary>
        /// Constructeur de base
        /// </summary>
        /// <param name="webSession">Session du client</param>
        public CustomerWebException(WebSession webSession)
            : base()
        {
            this._webSession = webSession;
            this._browser = webSession.Browser;
            this._versionBrowser = webSession.BrowserVersion;
            this._minorVersionBrowser = "";
            this._os = webSession.CustomerOs;
            this._userAgent = webSession.UserAgent;
            this._userHostAddress = webSession.CustomerIp;
            if (webSession.CurrentHttpContext != null)
                this._url = webSession.CurrentHttpContext.Request.Url.ToString();
            else
                this._url = webSession.LastWebPage;
            if (webSession.CurrentHttpContext != null && webSession.CurrentHttpContext.Request.Url.ToString() != System.Web.HttpContext.Current.Request.Url.ToString())
                this._urlAjax = System.Web.HttpContext.Current.Request.Url.ToString();
            else
                this._urlAjax = null;
            this._page = null;
            this._serverName = webSession.ServerName;
            this._platform = webSession.CustomerOs;
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
            this._webSession = webSession;
            this._browser = page.Request.Browser.Browser;
            this._versionBrowser = page.Request.Browser.Version;
            this._minorVersionBrowser = page.Request.Browser.MinorVersion.ToString();
            this._os = page.Request.Browser.Platform;
            this._userAgent = page.Request.UserAgent;
            this._userHostAddress = page.Request.UserHostAddress;
            this._url = page.Request.Url.ToString();
            if (webSession.CurrentHttpContext.Request.Url.ToString() != System.Web.HttpContext.Current.Request.Url.ToString())
                this._urlAjax = System.Web.HttpContext.Current.Request.Url.ToString();
            else
                this._urlAjax = null;
            this._platform = page.Request.Browser.Platform;
            this._page = page;
            this._serverName = page.Server.MachineName;
            this._stackTrace = stackTrace;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="page">Page Web qui lance l'erreur</param>
        /// <param name="message">Message d'erreur</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="stackTrace">stackTrace</param>
        public CustomerWebException(string message, string stackTrace)
            : base(message)
        {
            this._webSession = null;
            this._browser = System.Web.HttpContext.Current.Request.Browser.Browser;
            this._versionBrowser = System.Web.HttpContext.Current.Request.Browser.Version;
            this._minorVersionBrowser = System.Web.HttpContext.Current.Request.Browser.MinorVersion.ToString();
            this._os = System.Web.HttpContext.Current.Request.Browser.Platform;
            this._userAgent = System.Web.HttpContext.Current.Request.UserAgent;
            this._userHostAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
            this._url = System.Web.HttpContext.Current.Request.Url.ToString();
            this._urlAjax = null;
            this._platform = System.Web.HttpContext.Current.Request.Browser.Platform;
            this._page = null;
            this._serverName = System.Web.HttpContext.Current.Server.MachineName;
            this._stackTrace = stackTrace;
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
            this._webSession = webSession;
            this._browser = webSession.Browser;
            this._versionBrowser = webSession.BrowserVersion;
            this._minorVersionBrowser = "";
            this._os = webSession.CustomerOs;
            this._userAgent = webSession.UserAgent;
            this._userHostAddress = webSession.CustomerIp;
            if (webSession.CurrentHttpContext != null)
                this._url = webSession.CurrentHttpContext.Request.Url.ToString();
            else
                this._url = webSession.LastWebPage;

            if (webSession.CurrentHttpContext != null && 
                webSession.CurrentHttpContext.Request.Url.ToString() != System.Web.HttpContext.Current.Request.Url.ToString())
                this._urlAjax = System.Web.HttpContext.Current.Request.Url.ToString();
            else
                this._urlAjax = null;

            this._page = null;
            this._serverName = webSession.ServerName;
            this._platform = webSession.CustomerOs;
            this._stackTrace = stackTrace;
        }
        #endregion

        #region Méthodes internes
        /// <summary>
        /// Envoie un mail d'erreur
        /// </summary>
        public void SendMail()
        {
            string body = "";

            if (_webSession == null)
            {
                body += "<html><b><u>" + _serverName + ":</u></b><br>" + "<font color=#FF0000>Erreur client:<br></font>";
                body += "<hr>";
                body += "<u>Page demandée:</u><br><a href=" + _url + ">" + _url + "</a><br>";
                if (!string.IsNullOrEmpty(_urlAjax))
                    body += "<u>Page Ajax demandée:</u><br><a href=" + _urlAjax + ">" + _urlAjax + "</a><br>";
                body += "<u>Navigateur:</u><br>" + _browser + " " + _versionBrowser + " " + _minorVersionBrowser + "<br>" + _userAgent + "<br>";
                body += "<u>Système d'exploitation:</u><br>" + _os + "<br>" + _userHostAddress + "<br>";
                body += "<u>Message d'erreur:</u><br>" + Message + "<br>";
                body += "<u>Source:</u><br>" + Source + "<br>";
                body += "<u>StackTrace:</u><br>" + _stackTrace.Replace("at ", "<br>at ") + "<br>";
                body += "<hr>";
                body += "</html>";
            }
            else
            {
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);

                #region Identifiaction du client
                body += "<html><b><u>" + _serverName + ":</u></b><br>" + "<font color=#FF0000>Erreur client:<br></font>";
                body += "Numéro de session: " + _webSession.IdSession + "<br>";
                body += "Login: " + _webSession.CustomerLogin.Login + "<br>";
                body += "Password: " + _webSession.CustomerLogin.Password + "<br>";
                #endregion

                #region Message d'erreur
                body += "<hr>";
                body += "<u>Page demandée:</u><br><a href=" + _url + ">" + _url + "</a><br>";
                if (!string.IsNullOrEmpty(_urlAjax))
                    body += "<u>Page Ajax demandée:</u><br><a href=" + _urlAjax + ">" + _urlAjax + "</a><br>";
                body += "<u>Navigateur:</u><br>" + _browser + " " + _versionBrowser + " " + _minorVersionBrowser + "<br>" + _userAgent + "<br>";
                body += "<u>Système d'exploitation:</u><br>" + _platform + "<br>" + _userHostAddress + "<br>";
                body += "<u>Message d'erreur:</u><br>" + Message + "<br>";
                body += "<u>Source:</u><br>" + Source + "<br>";
                body += "<u>StackTrace:</u><br>" + _stackTrace.Replace("at ", "<br>at ") + "<br>";
                body += "<hr>";
                #endregion

                body += "</html>";
            }
            TNSMail.SmtpUtilities errorMail = new TNSMail.SmtpUtilities(WebApplicationParameters.ConfigurationDirectoryRoot + Cste.Configuration.FILE_ERROR_MAIL);
            errorMail.SendWithoutThread("PSA Promo Web site Error (" + _serverName + ")", Convertion.ToHtmlString(body), true, false);

        }
        #endregion

    }
}
