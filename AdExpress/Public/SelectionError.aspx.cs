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
using TNS.AdExpress.Web.Controls.Headers;
using TNS.AdExpress.Domain.Translation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

namespace AdExpress.Public{
	/// <summary>
	/// Description résumée de SelectionError.
	/// </summary>
	public partial class SelectionError : WebPage{

		#region Variables
		/// <summary>
		/// ?
		/// </summary>
		//protected System.Web.UI.HtmlControls.HtmlForm Form2;
		/// <summary>
		/// Message a afficher
		/// </summary>
		//protected System.Web.UI.WebControls.Label msg;
		/// <summary>
		/// Nombre de page de retour en arrière
		/// </summary>
		public int backPageNb = 2;
		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage;
		/// <summary>
		/// Titre de la page
		/// </summary>
		public string title="";
		/// <summary>
		/// Texte Explicatif
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText helpAdExpressText;
		/// <summary>
		/// Titre : aide
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText titleHelpAdExpressText;
		/// <summary>
		/// Identifiant de la session
		/// </summary>
		public string idSession;
		/// <summary>
		/// Format excel
		/// </summary>
		public bool excel=false;
		#endregion

		#region Evènements

		#region Chargement de la page

		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		private void Page_Load(object sender, System.EventArgs e){
			idSession=HttpContext.Current.Request.QueryString.Get("idSession");
            _siteLanguage = int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());
			WebConstantes.ErrorManager.selectedUnivers error=(WebConstantes.ErrorManager.selectedUnivers) int.Parse(Page.Request.QueryString.Get("error").ToString());
			
			if(WebConstantes.ErrorManager.formatFile.excel.ToString()==Page.Request.QueryString.Get("format").ToString()){
				excel=true;
			}

			helpAdExpressText.Language=_siteLanguage;
			helpAdExpressText.Code=1484;
			
			titleHelpAdExpressText.Code=992;
			titleHelpAdExpressText.Language=_siteLanguage;

			
			switch(error){
				case WebConstantes.ErrorManager.selectedUnivers.media:
					msg.Text=GestionWeb.GetWebWord(1479,_siteLanguage)+"<br>";
					break;
				case WebConstantes.ErrorManager.selectedUnivers.mediaNumber:
					msg.Text=GestionWeb.GetWebWord(1480,_siteLanguage)+"<br>";
					break;
				case WebConstantes.ErrorManager.selectedUnivers.period:
					msg.Text=GestionWeb.GetWebWord(1481,_siteLanguage)+"<br>";
					break;
				case WebConstantes.ErrorManager.selectedUnivers.product:
					msg.Text=GestionWeb.GetWebWord(1482,_siteLanguage)+"<br>";
					break;
				case WebConstantes.ErrorManager.selectedUnivers.vehicle:
					msg.Text=GestionWeb.GetWebWord(1483,_siteLanguage)+"<br>";
					break;
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#endregion
	}
}
