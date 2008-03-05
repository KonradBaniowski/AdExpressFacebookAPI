#region Information
/*
Author : G. RAGNEAU
Creation : 01/08/2005
Last modification:
*/
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


using TNS.AdExpress.Web.BusinessFacade.Results;
using TNS.AdExpress.Domain.Translation;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Results{
	/// <summary>
	///  Pop-up Zoom du plan media
	/// </summary>
	public partial class APPMInsertion : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variables
		/// <summary>
		/// Résultat
		/// </summary>
		public string result="";
		/// <summary>
		/// Nombre de page de retour en arrière
		/// </summary>
		public int backPageNb = 1;
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idSession="";
		/// <summary>
		/// Contextual Menu
		/// </summary>
		/// <summary>
		/// Media ID to study
		/// </summary>
		private string _idMedia="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public APPMInsertion():base(){			
			idSession=HttpContext.Current.Request.QueryString.Get("idSession");
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e) {
			try{

				#region Variables
				System.Text.StringBuilder HtmlTxt = new System.Text.StringBuilder(3000);
				#endregion

				#region Gestion du flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
				#endregion

				#region Résultat
				try{
					_idMedia = Page.Request.QueryString.Get("idMed");
				}
				catch(System.Exception){
					Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
				}

				result= APPMSystem.GetInsertionHtml(this.Page,_dataSource, _webSession,Int64.Parse(_idMedia),false);
				if (result.Length > 0 ){
					MenuWebControl2.ForcePrint = "/Private/Results/Excel/APPMInsertion.aspx?idSession=" + this._webSession.IdSession + "&idMed=" + _idMedia;
				}
				else{
					MenuWebControl2.Visible = false;
				}
				#endregion
			
				#region MAJ Session
				//Sauvegarde de la session
				_webSession.Save();
				#endregion

				#region TEMP : Script pour détection flash pour info sur le clic droit
				if (!Page.ClientScript.IsClientScriptBlockRegistered("detectFlash")){
					string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"detectFlash",tmp);
				}
				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}

		#region Determine PostBack
		/// <summary>
		/// Determine PostBack
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection coll = base.DeterminePostBackMode ();

			MenuWebControl2.CustomerWebSession = this._webSession;
			MenuWebControl2.ForbidHelpPages = true;

			return coll;
		}

		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation de la page
		/// </summary>
		/// <param name="e">Argumznts</param>
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent() {
           
		}
		#endregion
	}
}
