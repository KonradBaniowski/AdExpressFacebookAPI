#region Informations
// Auteur: 
// Date de création: 
// Date de modification:
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace TNS.AdExpress.Web.Controls.Buttons{
	/// <summary>
	/// Description résumée de RedirectWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:RedirectWebControl runat=server></{0}:RedirectWebControl>")]
	public class RedirectWebControl : System.Web.UI.WebControls.Button{
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			output.Write("");
		}
	}
}
