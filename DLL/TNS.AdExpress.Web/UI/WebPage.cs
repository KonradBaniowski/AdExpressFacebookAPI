#region Information
// Auteur : G Facon
// Cr�� le : 24/12/2004
// Modifi� le : 24/12/2004
//		29/12/2004	G.Facon		ajout �v�nements Load et Unload
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
using WebNavigation=TNS.AdExpress.Web.Core.Navigation;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.UI{
	/// <summary>
	/// Classe de base d'une page Web d'AdExpress.
	/// <seealso cref="TNS.AdExpress.Web.UI.ErrorEventArgs"/>
	/// </summary>
	/// <remarks>
	/// Cette page doit �tre utilis�e lorsque le client est connect�.
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
	///		// Ev�nement de chargement de la page
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
	/// <exception cref="TNS.AdExpress.Web.Exceptions.CustomerWebException">Exception lanc�e lors d'une erreur, s'il on est en mode debug.</exception>
	public class WebPage: System.Web.UI.Page{

		#region Variables


		///<summary>
		/// objet WebSession
		/// </summary>
		///  <supplierCardinality>1</supplierCardinality>
		///  <directed>True</directed>
		protected WebSession _webSession = null;
		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage = 33;		
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
		/// Argument de l'exception lev�e s'il y a une erreur
		/// </summary>
		private EventArgs _errorArgs;
		/// <summary>
		/// ?????
		/// </summary>
		protected WebConstantes.ErrorManager.selectedUnivers _selectionError=WebConstantes.ErrorManager.selectedUnivers.none;
		/// <summary>
		/// Type de format (html,excel)
		/// </summary>
		protected WebConstantes.ErrorManager.formatFile _formatFile=WebConstantes.ErrorManager.formatFile.html;
		/// <summary>
		/// Source de donn�es
		/// </summary>
        protected TNS.FrameWork.DB.Common.IDataSource _dataSource;
		#endregion
	

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public WebPage(){
			// Gestion des �v�nements
			base.Load +=new EventHandler(WebPage_Load);
			base.Unload += new EventHandler(WebPage_Unload);
			try{
				// Chargement de la session du client
				 _webSession=(WebSession)WebSession.Load(HttpContext.Current.Request.QueryString.Get("idSession"));
				// On obtient les informations sur le modules
				_currentModule=WebNavigation.ModulesList.GetModule(_webSession.CurrentModule);	
																									
			}
			catch(System.Exception){}
		}

		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		private void WebPage_Load(object sender, EventArgs e) {
			//On test si la session a bien �t� charg�e
			if(_webSession==null){
				this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this.Page,new WebExeptions.WebPageException("La session n'a pas �t� charg�e dans la classe de base d'un page"),_webSession));
			}

			if(_selectionError!=WebConstantes.ErrorManager.selectedUnivers.none){
				Response.Redirect("/Public/SelectionError.aspx?idSession="+_webSession.IdSession+"&siteLanguage="+_webSession.SiteLanguage+"&error="+_selectionError.GetHashCode()+"&format="+_formatFile.ToString() );
				Response.Flush();
				Response.End();
			}

			// D�finition de la langue � utiliser
			_siteLanguage = _webSession.SiteLanguage;

			#region Customer Informations
			// Use for custom error in Ajax
			_webSession.CustomerOs=Page.Request.Browser.Platform;
			_webSession.CustomerIp=Page.Request.UserHostAddress;
			_webSession.Browser=Page.Request.Browser.Browser;
			_webSession.BrowserVersion=Page.Request.Browser.Version+Page.Request.Browser.MinorVersion.ToString();
			_webSession.UserAgent=Page.Request.UserAgent;
			_webSession.LastWebPage=Page.Request.Url.ToString();
			_webSession.ServerName=Page.Server.MachineName;
			#endregion

			#region Connexion � la base de donn�es
			try{
				//Pour reste APPM
				_dataSource=new OracleDataSource(new OracleConnection(_webSession.CustomerLogin.OracleConnectionString));
			}
			catch(System.Exception err){
				this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this.Page,new WebExeptions.WebPageException("Le DataSource ne peut �tre cr�er",err),_webSession));
			}
			//Ancienne connexion client
			_webSession.CustomerLogin.Connection=(OracleConnection)_dataSource.GetSource();
			#endregion
		}
		#endregion

		#region D�chargement de la page
		
		/// <summary>
		/// D�chargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		private void WebPage_Unload(object sender, EventArgs e) {
			try{
				// Ancienne utilisation de la connexion client
				if(_webSession!=null && _webSession.CustomerLogin.Connection!=null){
					if(_webSession.CustomerLogin.Connection.State==System.Data.ConnectionState.Open)_webSession.CustomerLogin.Connection.Close();
					_webSession.CustomerLogin.Connection.Dispose();
				
					//IdataSource
					if((OracleConnection)_webSession.Source.GetSource()!=null){
						if(((OracleConnection)_webSession.Source.GetSource()).State==System.Data.ConnectionState.Open)_webSession.Source.Close();
						((OracleConnection)_webSession.Source.GetSource()).Dispose();
					
					}
				}
			}
			catch(System.Exception err){
				this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this.Page,new WebExeptions.WebPageException("Impossible de d�charger la page: "+err.Message+"  "+err.StackTrace),_webSession));
			}
		}
		
		#endregion

		#region Gestion des erreurs
		/// <summary>
		/// Ev�nement d'erreur
		/// </summary>
		/// <param name="e">Argument</param>
		protected override void OnError(EventArgs e) {
			_errorArgs=e;
			if(e.GetType()!=typeof(TNS.AdExpress.Web.UI.ErrorEventArgs)){
				base.OnError(_errorArgs);
				return;
			}
			if(e==EventArgs.Empty){
				base.OnError(_errorArgs);
				return;
			}
			TNS.AdExpress.Web.Exceptions.CustomerWebException cwe=null;
			try{
				BaseException err=((BaseException)((ErrorEventArgs)e)[ErrorEventArgs.argsName.error]);
				cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException((System.Web.UI.Page)(((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.sender]),err.Message,err.GetHtmlDetail(),((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.custormerSession]));
			}
			catch(System.Exception){
				try{
					cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException((System.Web.UI.Page)(((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.sender]),((System.Exception)((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.error]).Message,((System.Exception)((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.error]).StackTrace,((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.custormerSession]));
				}
				catch(System.Exception es){
					throw(es);
				}
			}
			cwe.SendMail();
			manageCustomerError(cwe);
			
		}

		/// <summary>
		/// Traite l'affichage d'erreur en fonction du mode compilation
		/// </summary>
		private void manageCustomerError(object source){
			#if DEBUG			
				throw((TNS.AdExpress.Web.Exceptions.CustomerWebException)source);				
			#else
				// Script
				if (!Page.ClientScript.IsClientScriptBlockRegistered("redirectError")){
					//Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"redirectError",WebFunctions.Script.redirectError(((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)e)[ErrorEventArgs.argsName.custormerSession]).SiteLanguage.ToString()));	
					Response.Write(WebFunctions.Script.RedirectError(((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)_errorArgs)[ErrorEventArgs.argsName.custormerSession]).SiteLanguage.ToString()));		
					Response.Flush();
					Response.End();
				}
				//this.Response.Redirect("/Public/Message.aspx?msgCode=5&siteLanguage="+((TNS.AdExpress.Web.Core.Sessions.WebSession)((ErrorEventArgs)e)[ErrorEventArgs.argsName.custormerSession]).SiteLanguage);
			#endif
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
