﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using KMI.PromoPSA.Web.Domain;
using CsteWebSession = KMI.PromoPSA.Constantes.WebSession;
namespace KMI.PromoPSA.Web.Core.Sessions
{
  public  class WebSession
    {
      private string _idSession;
      private Right _customerLogin;
      private Page _currentPage;
      private string _browser;
      private string _browserVersion;
      private string _customerOs;
      private string _userAgent;
      private string _customerIp;
      private string _lastWebPage;
      private string _serverName;
      private int _siteLanguage = 33;
      private int _currentAdvertNumber;
      private HttpContext _currentHttpContext;

      #region Constructeur
        /// <summary>
        /// Constructeur de la classe session
        /// </summary>
        /// <param name="session">Session HTTP</param>
        /// <param name="menu">Tab menu list</param>
        /// <param name="loginId">Login ID</param>
        public WebSession(Right customerRight, HttpSessionState session, Page page)
        {
            try
            {
                //Construction de l'identifiant de session
                _idSession = session.SessionID;
                _customerLogin = customerRight;
                _currentPage = page;

                _browser = page.Request.Browser.Browser;
                _browserVersion = page.Request.Browser.Version;
                _customerOs = page.Request.Browser.Platform;
                _userAgent = page.Request.UserAgent;
                _customerIp = page.Request.UserHostAddress;
                _lastWebPage = page.Request.Url.ToString();
                _serverName = page.Server.MachineName;
            }
            catch (System.Exception e)
            {
                throw new Exception("WebSession.WebSession(...) : Paramètre \"login\" invalide : " + e.Message);
            }
        }
        /// <summary>
        /// Constructeur de la classe session
        /// </summary>
        private WebSession()
        {
        }
        #endregion

        #region Session
        /// <summary>
        /// Get/Set Identifiant de session
        /// </summary>
        public string IdSession
        {
            get { return _idSession; }
            set
            {
                _idSession = value;
            }
        }

        #endregion
        #region Right
        /// <summary>
        /// Get/Set Identifiant de session login
        /// </summary>
        public Right CustomerLogin
        {
            get { return _customerLogin; }
            set { _customerLogin = value; }
        }
        #endregion

        #region user
        public string Browser
        {
            get { return _browser; }
        }
        public string BrowserVersion
        {
            get { return _browserVersion; }
        }
        public string CustomerOs
        {
            get { return _customerOs; }
        }
        public string UserAgent
        {
            get { return _userAgent; }
        }
        public string CustomerIp
        {
            get { return _customerIp; }
        }
        public string LastWebPage
        {
            get { return _lastWebPage; }
        }
        public string ServerName
        {
            get { return _serverName; }
        }
        #endregion

        #region Langues
        /// <summary>
        /// Obtient ou définit la langue choisie par l'utilisateur
        /// </summary>
        public int SiteLanguage
        {
            get { return (_siteLanguage); }
            set
            {
                _siteLanguage = value;
            }
        }
        /// <summary>
        /// Get data language
        /// </summary>
        public int DataLanguage
        {
            get
            {
                if (WebApplicationParameters.AllowedLanguages.ContainsKey(_siteLanguage))
                    return (WebApplicationParameters.AllowedLanguages[_siteLanguage].ClassificationLanguageId);
                else
                {
                    throw (new NullReferenceException("Data language id is null"));
                }
            }
        }
        #endregion

        #region Classification
        ///// <summary>
        ///// Get Advert List
        ///// </summary>
        //public Dictionary<string, Advert> AdvertList
        //{
        //    get { return _advertList; }
        //}

       

       
        ///<summary>
        /// Get / Set Advert Number list
        /// </summary>
        public int CurrentAdvertNumber
        {
            get { return _currentAdvertNumber; }
            set { _currentAdvertNumber = value; }
        }

        /// <summary>
        /// Get / Set Current Page
        /// </summary>
        public Page CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; }
        }

        /// <summary>
        /// Get / Set Current Page
        /// </summary>
        public HttpContext CurrentHttpContext
        {
            get { return _currentHttpContext; }
            set { _currentHttpContext = value; }
        }

        ///// <summary>
        ///// Get / Set Current Page
        ///// </summary>
        //public AdvertStatut[] AdvertStatutData
        //{
        //    get { return _advertStatutData; }
        //    set { _advertStatutData = value; }
        //}

        #endregion

        #region disconnect User
        /// <summary>
        /// Disconect user
        /// </summary>
        /// <param name="Session"></param>
        public void Disconect(HttpSessionState Session)
        {
            Session.Abandon();
        }
        #endregion

        #region Save Current Session
        /// <summary>
        /// Save Current object in session
        /// </summary>
        public void Save(HttpSessionState session)
        {
            session[CsteWebSession.WEB_SESSION] = this;
            WebSessions.Modify(this.CustomerLogin.IdLogin, this);
        }
        #endregion
    }
}