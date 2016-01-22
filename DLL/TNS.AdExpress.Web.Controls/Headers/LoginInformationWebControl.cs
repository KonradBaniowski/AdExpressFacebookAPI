#region Informations
// Auteur: B.Masson, G.Facon
// Date de création : 16/01/2007
// Date de modification :
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.Isis.Right.Common;

namespace TNS.AdExpress.Web.Controls.Headers {
	/// <summary>
	/// Composant d'affichage des informations sur le login (société, conseil)
	/// </summary>
	[DefaultProperty("Text"),
		ToolboxData("<{0}:LoginInformationWebControl runat=server></{0}:LoginInformationWebControl>")]
	public class LoginInformationWebControl : System.Web.UI.WebControls.WebControl {

		#region Variables
		/// <summary>
		/// WebSession
		/// </summary>
		protected WebSession _webSession = null;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Définit l'identifiant de la session
		/// </summary>
		public WebSession CustomerSession {
			set { _webSession = value; }
		}
		#endregion

		#region Evènements

		#region PréRendu
		/// <summary>
		/// PréRendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
		}
		#endregion

		#region Rendu
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
			if (_webSession != null) {
				output.Write("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td width=10>&nbsp;</td><td><font class=\"txtNoir11Bold\">" + GestionWeb.GetWebWord(2078, _webSession.SiteLanguage) + " " + _webSession.LoginCompany.Label.ToUpper() + "</font></td></tr></table>");
			}
			else {
				output.Write("Company Name");
			}
		}
		#endregion

		#endregion

	}
}
