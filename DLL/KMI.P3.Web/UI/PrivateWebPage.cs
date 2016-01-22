using System;
using System.Collections.Generic;
using System.Text;
using KMI.P3.Web.Core.Sessions;
using System.Web;
using WebExeptions = KMI.P3.Web.Exceptions;
using TNS.FrameWork.Exceptions;
using KMI.P3.Domain.Web;
namespace KMI.P3.Web.UI
{
    /// <summary>
    /// Classe de base d'une page Web d'EasyMusic.
    /// <seealso cref="TNS.EasyMusic.Web.UI.ErrorEventArgs"/>
    /// </summary>
    /// <remarks>
    /// Cette page doit être utilisée lorsque le client est connecté.
    /// En effet, dans le constructeur elle charge la session du client via la QueryString "idSession".
    /// </remarks>
    /// <example>
    /// <code>
    /// public class DateSelection : TNS.EasyMusic.Web.UI.WebPage{
    ///	
    ///		// Constructeur
    ///		public DateSelection():base(){
    ///		}
    ///		
    ///		// Evènement de chargement de la page
    ///		private void Page_Load(object sender, System.EventArgs e){
    ///			try{
    ///			...
    ///			}
    ///			catch(System.Exception exc){
    ///				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
    ///					this.OnError(new TNS.EasyMusic.Web.UI.ErrorEventArgs(this,exc,webSession));
    ///				}
    ///			}
    ///		}
    ///		
    /// }
    /// </code>
    /// </example>
    /// <exception cref="TNS.AdExpress.Web.Exceptions.CustomerWebException">Exception lancée lors d'une erreur, s'il on est en mode debug.</exception>
    public class PrivateWebPage : WebPage
    {

        #region Variables
        ///<summary>
        /// objet WebSession
        /// </summary>
        ///  <supplierCardinality>1</supplierCardinality>
        ///  <directed>True</directed>
        protected WebSession _webSession = null;
      
        /// <summary>
        /// Argument de l'exception levée s'il y a une erreur
        /// </summary>
        private EventArgs _errorArgs;
        ///// <summary>
        ///// Source de données
        ///// </summary>
        //protected TNS.FrameWork.DB.Common.IDataSource _dataSource;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public PrivateWebPage()
        {
            PageTypeInfo = PageType.connect;
            // Gestion des évènements
            base.Load += new EventHandler(WebPage_Load);
            base.Unload += new EventHandler(WebPage_Unload);

            try
            {
                // Chargement de la session du client
                _webSession = (WebSession)WebSession.Load(HttpContext.Current.Request.QueryString.Get("idSession"));
                if (_webSession != null)
                {
                  
                    if (HttpContext.Current.Request.QueryString.Get("sitelanguage") != null) _webSession.SiteLanguage = _siteLanguage;
                    // Définition de la langue à utiliser
                    else _siteLanguage = _webSession.SiteLanguage;
                }
            }
            catch (System.Exception) { }
        }
        #endregion

        #region Evènements

        #region Chargement de la page
        /// <summary>
        /// Chargement de la page
        /// </summary>
        /// <param name="sender">Objet source</param>
        /// <param name="e">Arguments</param>
        private void WebPage_Load(object sender, EventArgs e)
        {
            //On test si la session a bien été chargée
            if (_webSession == null)
            {
                this.OnError(new KMI.P3.Web.UI.ErrorEventArgs(this.Page, new WebExeptions.WebPageException("La session n'a pas été chargée dans la classe de base d'un page"), _webSession));
            }          
        }
        #endregion

        #region Déchargement de la page
        /// <summary>
        /// Déchargement de la page
        /// </summary>
        /// <param name="sender">Objet source</param>
        /// <param name="e">Arguments</param>
        private void WebPage_Unload(object sender, EventArgs e)
        {
        }
        #endregion

        #region Gestion des erreurs
        /// <summary>
        /// Evènement d'erreur
        /// </summary>
        /// <param name="e">Argument</param>
        protected override void OnError(EventArgs e)
        {
            _errorArgs = e;
            if (e.GetType() != typeof(KMI.P3.Web.UI.ErrorEventArgs))
            {
                base.OnError(_errorArgs);
                return;
            }
            if (e == EventArgs.Empty)
            {
                base.OnError(_errorArgs);
                return;
            }
            KMI.P3.Web.Exceptions.CustomerWebException cwe = null;
            try
            {
                BaseException err = ((BaseException)((ErrorEventArgs)e)[ErrorEventArgs.argsName.error]);
                cwe = new KMI.P3.Web.Exceptions.CustomerWebException((System.Web.UI.Page)(((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.sender]), err.Message, err.GetHtmlDetail(), ((KMI.P3.Web.Core.Sessions.WebSession)((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.custormerSession]));
            }
            catch (System.Exception)
            {
                try
                {
                    cwe = new KMI.P3.Web.Exceptions.CustomerWebException((System.Web.UI.Page)(((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.sender]), ((System.Exception)((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.error]).Message, ((System.Exception)((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.error]).StackTrace, ((KMI.P3.Web.Core.Sessions.WebSession)((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.custormerSession]));
                }
                catch (System.Exception es)
                {
                    throw (es);
                }
            }
            cwe.SendMail();
            manageCustomerError(cwe);

        }

        /// <summary>
        /// Traite l'affichage d'erreur en fonction du mode compilation
        /// </summary>
        private void manageCustomerError(object source)
        {
#if DEBUG
            throw ((KMI.P3.Web.Exceptions.CustomerWebException)source);
#else
				// Script
                //if (!Page.ClientScript.IsClientScriptBlockRegistered("redirectError")){
                //    Response.Write(WebFunctions.Script.RedirectError((( KMI.P3.Web.Core.Sessions.WebSession)((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.custormerSession]).SiteLanguage.ToString()));		
                //    Response.Flush();
                //    Response.End();
                //}
#endif
        }
        #endregion

        #region Initialisation de la page
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e)
        {



            base.OnInit(e);
        }
        #endregion

        #region PreInitialisation de la page
        /// <summary>
        /// PreInitialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreInit(EventArgs e)
        {

            #region Customer Informations
            // Use for custom error in Ajax
            _webSession.CustomerOs = Page.Request.Browser.Platform;
            _webSession.CustomerIp = Page.Request.UserHostAddress;
            _webSession.Browser = Page.Request.Browser.Browser;
            _webSession.BrowserVersion = Page.Request.Browser.Version + Page.Request.Browser.MinorVersion.ToString();
            _webSession.UserAgent = Page.Request.UserAgent;
            _webSession.LastWebPage = Page.Request.Url.ToString();
            _webSession.ServerName = Page.Server.MachineName;
            _webSession.Save();
            #endregion

            base.OnPreInit(e);

            #region Init Theme
            if (_useThemes)
            {
               
                    this.Page.Theme = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
               
            }
            #endregion
        }
        #endregion

        #endregion
    }
}
