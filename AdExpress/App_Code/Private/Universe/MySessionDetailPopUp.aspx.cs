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

namespace AdExpress.Private.Universe{
	/// <summary>
	/// Description résumée de MySessionDetailPopUp.
	/// </summary>
	public class MySessionDetailPopUp : System.Web.UI.Page{
		#region Variables MMI
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText detailUniversAdExpressText;
		/// <summary>
		/// Univers
		/// </summary>
		protected System.Web.UI.WebControls.Label universeLabel;
		/// <summary>
		/// Texte
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText advertiserAdexpressText;
		/// <summary>
		/// Bouton fermer
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl closeImageButtonRollOverWebControl;
		#endregion

		#region Evènement

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void Page_Load(object sender, System.EventArgs e){
			// Placer ici le code utilisateur pour initialiser la page
		}
		#endregion

		#region Initialisation
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
