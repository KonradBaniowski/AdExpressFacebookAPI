#region Information
/*
Author : D. Mussuma
Creation : 21/08/2006
Last modification:
*/
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
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
	/// Pop-up Synthèse d'une Version
	/// </summary>
	public partial class APPMVersionSynthesis : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variables
		/// <summary>
		/// Résultat
		/// </summary>
		public string result="";
	
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string _idSession="";
		/// <summary>
		/// Contextual Menu
		/// </summary>
		/// <summary>
		/// Version ID to study
		/// </summary>
		private string _idVersion="";
		/// <summary>
		/// Date de la 1ere insertion de la version
		/// </summary>
		private string _firstInsertionDate="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public APPMVersionSynthesis():base(){			
			_idSession=HttpContext.Current.Request.QueryString.Get("idSession");
		}
		#endregion

		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){

            _dataSource = _webSession.Source;

			#region Gestion du flash d'attente
			Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
			Page.Response.Flush();
			#endregion

			#region TEMP : Script pour détection flash pour info sur le clic droit
			if (!Page.ClientScript.IsClientScriptBlockRegistered("detectFlash")){
				string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"detectFlash",tmp);
			}
			#endregion

			#region Résultat
			try{
				_idVersion = Page.Request.QueryString.Get("idVersion");
				_firstInsertionDate = Page.Request.QueryString.Get("firstInsertionDate");
			}
			catch(System.Exception){
				
				Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
			}

			result= APPMSystem.GetVersionSynthesisHtml(_dataSource, _webSession,_idVersion,_firstInsertionDate,false);
			if (result.Length > 0 ){
				MenuWebControl2.ForcePrint = "/Private/Results/Excel/APPMVersionSynthesis.aspx?idSession=" + this._webSession.IdSession + "&idVersion=" + _idVersion+ "&firstInsertionDate=" + _firstInsertionDate;
			}
			else{
				MenuWebControl2.Visible = false;
			}
			#endregion
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
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{
          
		}
		#endregion
	}
}
