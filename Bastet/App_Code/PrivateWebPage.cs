#region Informations
// Auteur: B. Masson, G.Facon
// Date de création: 18/11/2005
// Date de modification: 
// 22/11/2005 Par B.Masson > Ajout de la gestion d'erreur et d'envoi du mail d'erreur
#endregion

using System;
using System.Text;
using TNS.FrameWork;
using TNS.FrameWork.Exceptions;
using TNSMail = TNS.FrameWork.Net.Mail;
using Localization = Bastet.Localization;
using TNS.AdExpress.Bastet.Translation;
using TNS.AdExpress.Bastet;
using System.Web;
using TNS.AdExpress.Bastet.Web;
using TNS.AdExpress.Bastet.Constantes.Web;

namespace BastetWeb{
	/// <summary>
	/// Classe de base d'une page Web de Bastet
	/// </summary>
    public class PrivateWebPage : WebPage {

		#region Variables
		/// <summary>
		/// Erreur client
		/// </summary>
		public string _email_customer_error = string.Empty;
		/// <summary>
		/// Serveur :
		/// </summary>
		public string _email_server = string.Empty;
		/// <summary>
		/// Page demandée :
		/// </summary>
		public string _email_request_page = string.Empty;
		/// <summary>
		/// Navigateur :
		/// </summary>
		public string _email_navigator = string.Empty;
		/// <summary>
		/// Système d'exploitation :
		/// </summary>
		public string _email_os = string.Empty;
		/// <summary>
		/// Message d'erreur :
		/// </summary>
		public string _email_error_message = string.Empty;
		/// <summary>
		/// StackTrace :
		/// </summary>
		public string _email_stacktrace = string.Empty;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public PrivateWebPage(){
			// Gestion des évènements
			base.Load +=new EventHandler(WebPage_Load);
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void WebPage_Load(object sender, System.EventArgs e){
			try{
				// Traite l'affichage en cas d'erreur
				//PageErrorRedirection();
			}
			catch(System.Exception err){
				//Attention thread abording
			}
		}
		#endregion

        #region On PreInit
        /// <summary>
        /// On preinit event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreInit(EventArgs e) {
            // TODO Gestion des exceptions
            if (HttpContext.Current.Request.QueryString.Get("sitelanguage") != null)
                if (Session[WebSession.LANGUAGE] == null)
                    Session.Add(WebSession.LANGUAGE, _siteLanguage);
                else
                    Session[WebSession.LANGUAGE] = _siteLanguage;
            else if (Session[WebSession.LANGUAGE] == null)
                    Session.Add(WebSession.LANGUAGE, WebApplicationParameters.DefaultLanguage);

            _siteLanguage = Int32.Parse(Session[WebSession.LANGUAGE].ToString());
            base.OnPreInit(e);
        }
        #endregion

		#region Initialisation de la page
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			InitializeComponentCustom();
			base.OnInit (e);
		}

		/// <summary>
		/// Initialisation des composants
		/// </summary>
		private void InitializeComponentCustom(){
            _email_customer_error = GestionWeb.GetWebWord(39, _siteLanguage);
            _email_server = GestionWeb.GetWebWord(40, _siteLanguage);
            _email_request_page = GestionWeb.GetWebWord(41, _siteLanguage);
            _email_navigator = GestionWeb.GetWebWord(42, _siteLanguage);
            _email_os = GestionWeb.GetWebWord(43, _siteLanguage);
            _email_error_message = GestionWeb.GetWebWord(44, _siteLanguage);
            _email_stacktrace = GestionWeb.GetWebWord(45, _siteLanguage); 
		}
		#endregion

		#region Gestion des erreurs
		/// <summary>
		/// Evènement d'erreur
		/// </summary>
		/// <param name="e">Argument</param>
		protected override void OnError(EventArgs e) {
			if(e.GetType()!=typeof(ErrorEventArgs)){
				base.OnError(e);
				return;
			}
			if(e==EventArgs.Empty){
				base.OnError(e);
				return;
			}
			try{
				BaseException err=((BaseException)((ErrorEventArgs)e)[ErrorEventArgs.argsName.error]);
				SendMail(this.Page, err.Message, err.GetHtmlDetail());
			}
			catch(System.Exception err){
				try{
					SendMail(this.Page, 
						((System.Exception)((ErrorEventArgs)e)[ErrorEventArgs.argsName.error]).Message, 
						((System.Exception)((ErrorEventArgs)e)[ErrorEventArgs.argsName.error]).StackTrace);
				}
				catch(System.Exception es){
					throw(es);
				}
			}
			finally{
				PageErrorRedirection();
			}
		}
		#endregion

		#endregion

		#region Méthodes internes
		/// <summary>
		/// Envoi un mail d'erreur
		/// </summary>
		internal void SendMail(System.Web.UI.Page page, string message, string stackTrace){
			StringBuilder body=new StringBuilder(1000);
			
			body.Append("<html>");
			// Style CSS
			body.Append("<style type=\"text/css\"><!--");
			body.Append("body {font-family: Arial, Helvetica, sans-serif;font-size: 11px;}");
			body.Append("a {color: #FF0099;}");
			body.Append("--></style>");
			
			// Corps du mail
			body.Append("<body>");
			// Erreur client
			body.Append("<p><font color=#FF0000><b>"+_email_customer_error+"</b></font></p>");
			// Serveur :
			body.Append("<p><b>"+_email_server+"</b><br>"+ page.Server.MachineName +"</p>");
			// Page demandée :
			body.Append("<p><b>"+_email_request_page+"</b><br><a href="+ page.Request.Url.ToString() +">"+ page.Request.Url.ToString() +"</a></p>");
			// Navigateur :
			body.Append("<p><b>"+_email_navigator+"</b><br>"+ page.Request.Browser.Browser +" "+ page.Request.Browser.Version +" "+ page.Request.Browser.MinorVersion.ToString() +"<br>"+ page.Request.UserAgent +"</p>");
			// Système d'exploitation :
			body.Append("<p><b>"+_email_os+"</b><br>"+ page.Request.Browser.Platform +"<br>"+ page.Request.UserHostAddress +"</p>");
			// Message d'erreur :
			body.Append("<p><b>"+_email_error_message+"</b><br><font color=#FF0000>"+ message +"</font></p>");
			// StackTrace :
			body.Append("<p><b>"+_email_stacktrace+"</b><br>"+ stackTrace.Replace("at ","<br>at ") +"</p>");
			body.Append("</body>");
			body.Append("</html>");
			
			// Envoi du mail
			TNSMail.SmtpUtilities errorMail=new TNSMail.SmtpUtilities(AppDomain.CurrentDomain.BaseDirectory+@"Configuration\ErrorMail.xml");
			errorMail.SendWithoutThread("Bastet Error ("+page.Server.MachineName+")",Convertion.ToHtmlString(body.ToString()),true,false);
		}

		/// <summary>
		/// Traite l'affichage d'erreur
		/// </summary>
		internal void PageErrorRedirection(){
			bool findError=false;
//			string currentPage = Page.Request.Url.LocalPath.ToString().Substring(1);
//			switch(currentPage){
//				case "MailSelection.aspx":
//					if(Session["Login"]==null){
//						Response.Redirect("Error.aspx?errorId=1");
//						findError=true;
//					}
//					break;
//				case "DateSelection.aspx":
//					if(Session["Login"]==null){
//						Response.Redirect("Error.aspx?errorId=1");
//						findError=true;
//					}
//					else{
//						if(Session["Mails"]==null){
//							Response.Redirect("Error.aspx?errorId=2");
//							findError=true;
//						}
//					}
//					break;
//				case "LoginSelection.aspx":
//					if(Session["Login"]==null){
//						Response.Redirect("Error.aspx?errorId=1");
//						findError=true;
//					}
//					else if(Session["Mails"]==null){
//						Response.Redirect("Error.aspx?errorId=2");
//						findError=true;
//						break;
//					}
//					else if(Session["DateBegin"]==null || Session["DateEnd"]==null){
//						Response.Redirect("Error.aspx?errorId=3");
//						findError=true;
//						break;
//					}
//					break;
//			}
//			if(!findError)Response.Redirect("Error.aspx");
//
            if (Session[WebSession.LANGUAGE] == null) {
                Response.Redirect("/Error.aspx?errorId=4");
                findError = true;
                return;
            }
			if(Session[WebSession.LOGIN]==null){
				Response.Redirect("/Error.aspx?errorId=1");
				findError=true;
				return;
			}
            if (Session[WebSession.MAILS] == null) {
				Response.Redirect("/Error.aspx?errorId=2");
				findError=true;
				return;
			}
            if (Session[WebSession.DATE_BEGIN] == null || Session[WebSession.DATE_END] == null) {
				Response.Redirect("/Error.aspx?errorId=3");
				findError=true;
				return;
			}
			if(!findError)Response.Redirect("/Error.aspx");
		}
		#endregion

	}
}
