#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 03/07/2006
// Date de modification:
#endregion

using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Exceptions;
using AjaxPro;

namespace TNS.AdExpress.Web.Controls{
	/// <summary>
	/// Description r�sum�e de AjaxBaseWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:AjaxBaseWebControl runat=server></{0}:AjaxBaseWebControl>")]
	public abstract class AjaxBaseWebControl : System.Web.UI.WebControls.WebControl{

		#region Variables
		/// <summary>
		/// Timeout des scripts utilis�s par AjaxPro
		/// </summary>
		protected int _ajaxProTimeOut=60;
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _customerWebSession=null;
		#endregion
	
		#region Accesseurs
		/// <summary>
		/// Obtient ou d�finit le Timeout des scripts utilis�s par AjaxPro
		/// </summary>
		[Bindable(true), 
		Category("Ajax"),
		Description("Timeout des scripts utilis�s par AjaxPro"),
		DefaultValue("60")]
		public int AjaxProTimeOut{
			get{return _ajaxProTimeOut;}
			set{_ajaxProTimeOut=value;}
		}
		/// <summary>
		/// Obtient ou d�finit la Sesion du client
		/// </summary>
		[Bindable(false)] 
		public WebSession CustomerWebSession {
			get{return(_customerWebSession);}
			set{_customerWebSession = value;}
		}

		#endregion

		#region JavaScript
		/// <summary>
		/// G�n�re le code JavaSript pour ajuster le time out d'AjaxPro
		/// </summary>
		/// <returns>Code JavaScript</returns>
		private string AjaxProTimeOutScript(){
			StringBuilder js=new StringBuilder(100);
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			js.Append("\r\nAjaxPro.timeoutPeriod="+_ajaxProTimeOut.ToString()+"*1000;"); 
			js.Append("\r\n-->\r\n</SCRIPT>");
			return(js.ToString());
		}

        /// <summary>
        /// Evenement Ajax
        /// </summary>
        /// <returns></returns>
		protected virtual string AjaxEventScript(){
			StringBuilder js=new StringBuilder(1000);
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			js.Append("\r\nfunction get_"+this.ID+"(){");
			js.Append("\r\n\t"+this.GetType().Namespace+"."+this.GetType().Name+".GetData('"+_customerWebSession.IdSession+"',get_"+this.ID+"_callback);");
			js.Append("\r\n}");

			js.Append("\r\nfunction get_"+this.ID+"_callback(res){");
			js.Append("\r\n\tvar oN=document.getElementById('res_"+this.ID+"');");
			js.Append("\r\n\toN.innerHTML=res.value;");
			js.Append("\r\n}\r\n");
			js.Append("\r\naddEvent(window, \"load\", get_"+this.ID+");");

			js.Append("\r\n-->\r\n</SCRIPT>");
			return(js.ToString());
		}
		#endregion

		#region M�thodes abstraites
        /// <summary>
        /// Obtention du code HTML � ins�rer dans le composant
        /// </summary>
        /// <param name="sessionId">Session du client</param>
        /// <returns>Code HTML</returns>
        //[AjaxPro.AjaxMethod]
        public abstract string GetData(string sessionId);

//		/// <summary>
//		/// Obtention du code HTML � ins�rer dans le composant
//		/// </summary>
//		/// <param name="sessionId">Session du client</param>
//		/// <param name="oParams">Tableaux de param�tres</param>
//		/// <returns>Code HTML</returns>
//		[AjaxPro.AjaxMethod]
//		public abstract string GetData(string sessionId,AjaxPro.JavaScriptObject oParams);
		#endregion

		#region Ev�nements

		#region Initialisation
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			base.OnInit (e);
		}
		/// <summary>
		/// Appel� sur erreur � l'ex�cution des m�thodes Ajax
		/// </summary>
		/// <param name="errorException">Exception</param>
		/// <param name="customerSession">Session utilisateur</param>
		/// <returns>Message d'erreur</returns>
		protected string OnAjaxMethodError(Exception errorException,WebSession customerSession) {
			TNS.AdExpress.Web.Exceptions.CustomerWebException cwe=null;
			try{
				BaseException err=(BaseException)errorException;
				cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException(err.Message,err.GetHtmlDetail(),customerSession);
			}
			catch(System.Exception){
				try{
					cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException(errorException.Message,errorException.StackTrace,customerSession);
				}
				catch(System.Exception es){
					throw(es);
				}
			}
			cwe.SendMail();
			return GetMessageError(customerSession,1973);
		}
		#endregion

		#region Load
		/// <summary>
		/// Chargement du composant
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnLoad(EventArgs e) {
			AjaxPro.Utility.RegisterTypeForAjax(this.GetType(), this.Page);
			base.OnLoad (e);
		}
		#endregion

		#region Pr�Render
		/// <summary>
		/// Pr�rendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
		}
		#endregion

		#region Render
		/// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output){
			StringBuilder html=new StringBuilder(1000);
			html.Append(AjaxProTimeOutScript());
			html.Append(AjaxEventScript());
			html.Append(GetLoadingHTML());
			output.Write(html.ToString());
		}
		#endregion

		#endregion

		#region M�thodes
		/// <summary>
		/// Obtient le code HTML du loading
		/// </summary>
		/// <returns></returns>
		protected string GetLoadingHTML(){
            string themeName = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;
            return ("<div align=\"center\" id=\"res_" + this.ID + "\"><img src=\"/App_Themes/" + themeName + "/Images/Common/waitAjax.gif\"></div>");
		}
		
		/// <summary>
		/// Message d'erreur
		/// </summary>
		/// <param name="customerSession">Session du client</param>
		/// <param name="code">Code message</param>
		/// <returns>Message d'erreur</returns>
		protected string GetMessageError(WebSession customerSession, int code){
			string errorMessage="<div align=\"center\" class=\"txtViolet11Bold\">";
			if(customerSession!=null)
				errorMessage += GestionWeb.GetWebWord(code,customerSession.SiteLanguage)+". "+GestionWeb.GetWebWord(2099,customerSession.SiteLanguage);			
			else
				errorMessage += GestionWeb.GetWebWord(code,33)+". "+GestionWeb.GetWebWord(2099,33);
			
			errorMessage +="</div>";
			return errorMessage;
		}
		#endregion
	}
}

