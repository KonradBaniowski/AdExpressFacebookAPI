#region Informations
// Auteur: B. Masson
// Date de création: 16/11/2004 
// Date de modification: 19/11/2004 
//    31/12/2004 A. Obermeyer Intégration de WebPage
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

using WebCst = TNS.AdExpress.Constantes.Web;
using TradCst = TNS.AdExpress.Constantes.DB.Language;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebFunctions=TNS.AdExpress.Web.Functions;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Module infos/news
	/// </summary>
	public partial class InfoNews : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variable MMI
		/// <summary>
		/// Contrôle En tête de page
		/// </summary>
		/// <summary>
		/// Titre de la page
		/// </summary>
		#endregion
				
		#region Variables		
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idsession;		
		/// <summary>
		/// Affichage du tableau de résultat
		/// </summary>
		public string result;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public InfoNews():base(){
			// Chargement de la Session
			try{				
				idsession=HttpContext.Current.Request.QueryString.Get("idSession");	
			}
			catch(System.Exception){
				Response.Write(WebFunctions.Script.ErrorCloseScript("Your session is unavailable. Please reconnect via the Homepage"));
				Response.Flush();	
			}
		}
		#endregion
		
		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
	
				#region Textes et Langage du site
				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_webSession.SiteLanguage);
			
				HeaderWebControl1.ActiveMenu = WebCst.MenuTraductions.NEWS;
                HeaderWebControl1.Language = _webSession.SiteLanguage;
				PageTitleWebControl1.Language = _webSession.SiteLanguage;
				#endregion

				#region Résultat
				result = TNS.AdExpress.Web.BusinessFacade.Results.InfoNewsSystem.GetHtml(this.Page, _webSession);
				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
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
		protected void Page_UnLoad(object sender, System.EventArgs e){
			
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
          
            this.Unload += new System.EventHandler(this.Page_UnLoad);
		}
		#endregion

	}
}
