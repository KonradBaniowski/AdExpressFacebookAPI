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
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

namespace AdExpress.Private.Helps{
	/// <summary>
	/// Page d'aide de AEPMTargetSelectionHelp.aspx
	/// </summary>
	public partial class AEPMTargetSelectionHelp : WebPage{

		#region Variables
        /// <summary>
        /// Theme name
        /// </summary>
        public string _themeName = string.Empty;
		#endregion

		#region Variables MMI
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
		/// </summary>
		/// <summary>
		/// Texte
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
            _themeName = WebApplicationParameters.Themes[_siteLanguage].Name;
			
			#region Textes et langage du site
			//Modification de la langue pour les Textes AdExpress
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[3].Controls,_siteLanguage);
			#endregion

		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{    

		}
		#endregion

		#endregion

	}
}
