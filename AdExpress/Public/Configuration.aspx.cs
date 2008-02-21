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

namespace AdExpress.Public{
	/// <summary>
	/// Page Web Expliquant la configuration nécessaire au site AdExpress
	/// </summary>
	public partial class Configuration : System.Web.UI.Page{

		#region Variable MMI
		/// <summary>
		/// Texte du titre
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText titreAdExpressText;
		/// <summary>
		/// Texte du hardware
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText computerAdExpressText;
		/// <summary>
		/// Texte du navigateur
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText iexplorerAdexpresstext;
		/// <summary>
		/// Texte de la configuration vidéo
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText screenAdexpresstext;
		/// <summary>
		/// Texte d'Excel
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText excelAdexpresstext;
		/// <summary>
		/// Texte d'adobe reader
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText adobeAdexpresstext;
		/// <summary>
		/// Texte du player flash
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText flashAdexpresstext;
		/// <summary>
		/// Texte des players vidéo
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText videoAdexpresstext;
		/// <summary>
		/// Texte de la version
		/// </summary>
		//protected TNS.AdExpress.Web.Controls.Translation.AdExpressText versionAdExpressText;
		#endregion

		#region Variables
		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage=33;
		#endregion
	
		#region Evènements
		#region Chargement de la page
		/// <summary>
		/// Chargement de la pages
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void Page_Load(object sender, System.EventArgs e){

			#region Langue de la page
			if(Page.Request.QueryString.Get("siteLanguage") == null){
				_siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
			}
			else{
				_siteLanguage = int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());
			}
			
			//Modification de la langue pour les Textes AdExpress
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_siteLanguage);
			#endregion
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#endregion

	}
}
