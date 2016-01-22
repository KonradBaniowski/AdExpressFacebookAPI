#region Information
//Auteur G. RAGNEAU
//date de création : 02/08/2005
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

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;

using WebFunctions = TNS.AdExpress.Web.Functions;

namespace AdExpress.Private.Results.Excel {
	/// <summary>
	/// Display Insertion of a media in the APPM module
	/// </summary>
	public partial class APPMInsertion : TNS.AdExpress.Web.UI.ExcelWebPage {
		
		#region Variables
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";		
		private string _idMedia = "";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : Chargement de la session
		/// </summary>
		public APPMInsertion():base()	{			
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
            this._dataSource = _webSession.Source;
			try{
				_idMedia = Page.Request.QueryString.Get("idMed");
			}
			catch(System.Exception){
				Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
			}

			try{
				#region Resultat
				result = TNS.AdExpress.Web.BusinessFacade.Results.APPMSystem.GetInsertionHtml(this.Page,_dataSource,_webSession, int.Parse(_idMedia),true);
				#endregion	
			}	
			catch(System.Exception exc) {
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void Page_UnLoad(object sender, System.EventArgs e) {			
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
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
