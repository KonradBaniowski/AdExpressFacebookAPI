using System;
using KMI.PromoPSA.Rules.Exceptions;
using KMI.PromoPSA.Web.Core.Sessions;
using TNS.FrameWork.Exceptions;
using CsteWebSession = KMI.PromoPSA.Constantes.WebSession;

namespace KMI.PromoPSA.Web.UI {
    /// <summary>
    /// Base class for public and private AdExpress page
    /// </summary>
    public class PrivateWebPage : WebPage {

        #region Variables
        /// <summary>
        /// Web Session
        /// </summary>
        public WebSession _webSession;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PrivateWebPage() : base() {}
        #endregion

        #region Events

        #region OnPreInit
        /// <summary>
        /// OnPreInit event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreInit(EventArgs e) {

            base.OnPreInit(e);

            WebSession webSession = (WebSession)Session[CsteWebSession.WEB_SESSION];

            //If session already in Use
            if (webSession != null && WebSessions.Contains(webSession.CustomerLogin.IdLogin)) {
                if (WebSessions.Get(webSession.CustomerLogin.IdLogin).IdSession != webSession.IdSession) {
                    webSession = null;
                }
            }
            else {
                webSession = null;
            }

            if (webSession == null) {
                Response.Redirect(Page.ResolveUrl("~/index.aspx"));
            }

            this.EnableViewState = true;

            if (webSession != null) {
                _webSession = webSession;
                _webSession.SiteLanguage = _siteLanguage;
                _webSession.CurrentPage = this.Page;
                _webSession.CurrentHttpContext = this.Context;

                //if (!_webSession.HasRightToCurrentPage())
                //{
                //    _webSession.Disconect(Session);
                //    Response.Redirect(Page.ResolveUrl("~/index.aspx"));
                //}
            }
        }
        #endregion

        #region OnError
        /// <summary>
        /// Evènement d'erreur
        /// </summary>
        /// <param name="e">Argument</param>
        protected override void OnError(EventArgs e) {

            Web.Exceptions.CustomerWebException cwe;
            
            if (e.GetType() != typeof(ErrorEventArgs)) {
                base.OnError(e);
                return;
            }

            ErrorEventArgs error = (ErrorEventArgs)e;
            
            if (e == EventArgs.Empty) {
                base.OnError(e);
                return;
            }

            try {
                BaseException err = ((BaseException)((ErrorEventArgs)e)[ErrorEventArgs.argsName.error]);
                cwe = new Web.Exceptions.CustomerWebException(Page, err.Message, err.GetHtmlDetail(), _webSession);
            }
            catch (Exception) {
                cwe = new Exceptions.CustomerWebException(Page,
                                                                                     ((Exception)((ErrorEventArgs)e)[ErrorEventArgs.argsName.error]).Message,
                                                                                     ((Exception)((ErrorEventArgs)e)[ErrorEventArgs.argsName.error]).StackTrace, _webSession);

            }

            cwe.SendMail();
            Exception currentException = error.Error;
            int errorId = -1;

            while (currentException != null && errorId < 0) {

                if (currentException.GetType() == typeof(PromotionException)) {
                    errorId = 96;
                }
                else if (currentException.GetType() == typeof(PromotionClassificationException)) {
                    errorId = 95;
                }
                else if (currentException.GetType() == typeof(WebServiceDispacherException)) {
                    errorId = 94;
                }
                else if (currentException.GetType() == typeof(WebServiceRightException)) {
                    errorId = 93;
                }
                currentException = currentException.InnerException;
                
            }

            if (errorId < 0)
                errorId = 98;

            Redirect(errorId);

        }
        #endregion

        #endregion

    }
}
