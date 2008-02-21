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

namespace AdExpress.Private.Helps{
	/// <summary>
	/// Page d'aide de VehicleSelection.aspx
	/// </summary>
	public partial class VehicleSelectionHelp : System.Web.UI.Page{

		#region Variables
		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage=33;
		#endregion

		#region Variables MMI
		/// <summary>
		/// Titre de la page
		/// </summary>
		/// <summary>
		/// Titre de Paragraphe
		/// </summary>
		/// <summary>
		/// Texte Explicatif
		/// </summary>
		/// <summary>
		/// Texte Explicatif
		/// </summary>
		/// <summary>
		/// Texte Explicatif
		/// </summary>
		/// <summary>
		/// Texte Explicatif
		/// </summary>
		/// <summary>
		/// Texte Explicatif
		/// </summary>
		/// <summary>
		/// Texte Explicatif
		/// </summary>
		/// <summary>
		/// Texte Explicatif
		/// </summary>
		/// <summary>
		/// Texte Explicatif
		/// </summary>
		/// <summary>
		/// Titre de Paragraphe
		/// </summary>
		/// <summary>
		/// Texte Explicatif
		/// </summary>
		#endregion

		#region Ev�nements

		#region Chargement
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Aguments</param>
		protected void Page_Load(object sender, System.EventArgs e){

			#region Textes et langage du site
			if(Page.Request.QueryString.Get("siteLanguage")!=null)
				_siteLanguage=int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());
			//Modification de la langue pour les Textes AdExpress
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls,_siteLanguage);
			#endregion

		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation de la page
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent(){    

		}
		#endregion
		#endregion
	}
}
