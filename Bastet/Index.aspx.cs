#region Informations
// Auteur: B. Masson, G.Facon
// Date de création: 14/11/2005
// Date de modification: 
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
using Localization = Bastet.Localization;
using TNS.AdExpress.Bastet.Translation;
using TNS.AdExpress.Bastet.Web;
using TNS.AdExpress.Bastet;

namespace BastetWeb{
	/// <summary>
	/// Page d'accueil de Bastet, étape d'authentification
	/// </summary>
	public partial class Index : WebPage{

		#region Variables
		/// <summary>
		/// Connexion impossible
		/// </summary>
		public string _msg_err_connection = string.Empty;
		#endregion

		#region Variables MMI
		#endregion

		#region Evènements
	
		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
			}
			catch(System.Exception err){
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
            base.OnInit(e);
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
            HeaderWebControl1.Type_de_page = TNS.AdExpress.Bastet.WebControls.PageType.homepage;
            HeaderWebControl1.LanguageId = _siteLanguage;
            loginWebControl.WebServiceURL = WebApplicationParameters.WebServiceRightConfiguration.URL;
            loginWebControl.TNSName = WebApplicationParameters.WebServiceRightConfiguration.TnsName;
           
			InitializeComponent();
			InitializeComponentCustom();
		}

		/// <summary>
		/// Initialisation des composants
		/// </summary>
		private void InitializeComponentCustom(){
            _msg_err_connection = GestionWeb.GetWebWord(25, _siteLanguage);
            this.loginWebControl.CultureInfo = TNS.AdExpress.Bastet.Web.WebApplicationParameters.AllowedLanguages[_siteLanguage].CultureInfo;
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{    
			this.loginWebControl.OnCanAccess += new TNS.Isis.WebControls.Authentification.LoginWebControl.CanAccess(this.CanAccess_click);
		}
		#endregion

		#region Authentification
		/// <summary>
		/// Evénement CanAccess
		/// </summary>
		/// <param name="ok">Booléen</param>
		private void CanAccess_click(bool ok) {
            try {
                if(ok) {
                    Session.Add("Login",loginWebControl.Login);
                    Response.Redirect("ModuleSelection.aspx");
                }
                else {

                    // Javascript Erreur : Connexion impossible
                    Response.Write("<script language=Javascript>");
                    Response.Write("alert('"+_msg_err_connection+"');");
                    Response.Write("</script>");

                }
            }
            catch(System.Exception err) {
            }
		}
		#endregion

		#endregion

	}
}
