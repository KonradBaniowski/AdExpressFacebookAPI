#region Information
// Auteur : G Facon
// Créé le : 24/12/2004
// Modifié le : 24/12/2004
//		29/12/2004	G.Facon		ajout évènements Load et Unload
//		12/08/2005	G. Facon	Nom de varaibles
//		16/08/2005	G. Facon	Gestion des erreurs
//		
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExeptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebNavigation=TNS.AdExpress.Domain.Web.Navigation;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.UI{
	/// <summary>
	/// Classe de base d'une page Web d'AdExpress.
	/// <seealso cref="TNS.AdExpress.Web.UI.ErrorEventArgs"/>
	/// </summary>
	/// <remarks>
	/// Cette page doit être utilisée lorsque le client est connecté.
	/// En effet, dans le constructeur elle charge la session du client via la QueryString "idSession".
	/// </remarks>
	/// <example>
	/// <code>
	/// public class AlertDateSelection : TNS.AdExpress.Web.UI.WebPage{
	///	
	///		// Constructeur
	///		public AlertDateSelection():base(){
	///		}
	///		
	///		// Evènement de chargement de la page
	///		private void Page_Load(object sender, System.EventArgs e){
	///			try{
	///			...
	///			}
	///			catch(System.Exception exc){
	///				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
	///					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,webSession));
	///				}
	///			}
	///		}
	///		
	/// }
	/// </code>
	/// </example>
	/// <exception cref="TNS.AdExpress.Web.Exceptions.CustomerWebException">Exception lancée lors d'une erreur, s'il on est en mode debug.</exception>
	public class PrivateWebPage: WebPage{

		#region Variables


		///<summary>
		/// objet WebSession
		/// </summary>
		///  <supplierCardinality>1</supplierCardinality>
		///  <directed>True</directed>
		protected WebSession _webSession = null;		
		/// <summary>
		/// Url suivante
		/// </summary>
		protected string _nextUrl="";
		/// <summary>
		/// Indique si l'url de la prochaine page est disponible ou non
		/// </summary>
		protected bool _nextUrlOk=false;
		/// <summary>
		/// Information sur le module en cours
		/// </summary>
		protected WebNavigation.Module _currentModule; 
		/// <summary>
		/// ?????
		/// </summary>
		protected WebConstantes.ErrorManager.selectedUnivers _selectionError=WebConstantes.ErrorManager.selectedUnivers.none;
		/// <summary>
		/// Type de format (html,excel)
		/// </summary>
		protected WebConstantes.ErrorManager.formatFile _formatFile=WebConstantes.ErrorManager.formatFile.html;
		/// <summary>
		/// Source de données
		/// </summary>
        protected TNS.FrameWork.DB.Common.IDataSource _dataSource;
		#endregion
	

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public PrivateWebPage(){

            HttpCookie autoConnectCookie = HttpContext.Current.Request.Cookies[Cookies.AlertAutoConnectCookie];
            string idAlert = HttpContext.Current.Request.QueryString["idAlert"];
            string idOcc = HttpContext.Current.Request.QueryString["idOcc"];
            string idSession = HttpContext.Current.Request.QueryString["idSession"];
            string redirectUrl = "";
            if (idSession != null || (autoConnectCookie != null && idAlert != null && idOcc != null))
            {

                // Gestion des évènements
                base.Load += new EventHandler(WebPage_Load);
                base.Unload += new EventHandler(WebPage_Unload);

                try
                {
                    // Chargement de la session du client via QuerySring
                    // ou Cookie
                    if (idSession != null)
                        _webSession = (WebSession)WebSession.Load(idSession);

                    // Loading session information
                    if (_webSession != null)
                    {
                        // On obtient les informations sur le modules
                        _currentModule = WebNavigation.ModulesList.GetModule(_webSession.CurrentModule);

                        // Définition de la langue à utiliser
                        if (HttpContext.Current.Request.QueryString.Get("sitelanguage") != null)
                            _webSession.SiteLanguage = _siteLanguage;
                        else
                            _siteLanguage = _webSession.SiteLanguage;
                    }
                }
                catch (System.Exception) {
                    redirectUrl = "/index.aspx";
                    if (HttpContext.Current.Request.QueryString.Get("sitelanguage") != null)                        
                    {
                        try
                        {
                            redirectUrl += "?siteLanguage=" + int.Parse(HttpContext.Current.Request.QueryString.Get("sitelanguage"));
                        }
                        catch (System.Exception)
                        {
                            redirectUrl += "?siteLanguage=" + _siteLanguage;
                        }
                    }
                    HttpContext.Current.Response.Redirect(redirectUrl);
                }
            }
            else
            {
                redirectUrl = "/index.aspx?";
                if (idAlert != null)
                    redirectUrl += "idAlert=" + idAlert;
                if (idOcc != null)
                    redirectUrl += "&idOcc=" + idOcc;
                HttpContext.Current.Response.Redirect(redirectUrl);
            }
		}

		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		private void WebPage_Load(object sender, EventArgs e) {
			//On test si la session a bien été chargée
			if(_webSession==null){
				this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this.Page,new WebExeptions.WebPageException("La session n'a pas été chargée dans la classe de base d'un page"),_webSession));
			}

			if(_selectionError!=WebConstantes.ErrorManager.selectedUnivers.none){
				Response.Redirect("/Public/SelectionError.aspx?idSession="+_webSession.IdSession+"&siteLanguage="+_webSession.SiteLanguage+"&error="+_selectionError.GetHashCode()+"&format="+_formatFile.ToString() );
				Response.Flush();
				Response.End();
			}

			#region Customer Informations
			// Use for custom error in Ajax
			_webSession.CustomerOs=Page.Request.Browser.Platform;
			_webSession.CustomerIp=Page.Request.UserHostAddress;
			_webSession.Browser=Page.Request.Browser.Browser;
			_webSession.BrowserVersion=Page.Request.Browser.Version+Page.Request.Browser.MinorVersion.ToString();
			_webSession.UserAgent=Page.Request.UserAgent;
			_webSession.LastWebPage=Page.Request.Url.ToString();
			_webSession.ServerName=Page.Server.MachineName;
            _webSession.DomainName = Page.Request.Url.Host;
			#endregion

			#region Connexion à la base de données
            try
            {
             
                    //Pour reste APPM
                    OracleConnection connection = (OracleConnection)_webSession.Source.GetSource();
                
            }
            catch (System.Exception err)
            {
                this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this.Page, new WebExeptions.WebPageException("Le DataSource ne peut être créer", err), _webSession));
            }
			//Ancienne connexion client
			//_webSession.CustomerLogin.Connection=(OracleConnection)_dataSource.GetSource();
			#endregion
		}
		#endregion

		#region Déchargement de la page
		
		/// <summary>
		/// Déchargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		private void WebPage_Unload(object sender, EventArgs e) {
			try{
                //TODO
				// Ancienne utilisation de la connexion client
                //if(_webSession!=null && _webSession.CustomerLogin.Connection!=null){
                //    if(_webSession.CustomerLogin.Connection.State==System.Data.ConnectionState.Open)_webSession.CustomerLogin.Connection.Close();
                //    _webSession.CustomerLogin.Connection.Dispose();
				
                //    //IdataSource
                //    if((OracleConnection)_webSession.Source.GetSource()!=null){
                //        if(((OracleConnection)_webSession.Source.GetSource()).State==System.Data.ConnectionState.Open)_webSession.Source.Close();
                //        ((OracleConnection)_webSession.Source.GetSource()).Dispose();
					
                //    }
                //}
			}
			catch(System.Exception err){
				this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this.Page,new WebExeptions.WebPageException("Impossible de décharger la page: "+err.Message+"  "+err.StackTrace),_webSession));
			}
		}
		
		#endregion

		#region Initialisation de la page
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			base.OnInit (e);
		}
		#endregion

		#endregion
	}
}
