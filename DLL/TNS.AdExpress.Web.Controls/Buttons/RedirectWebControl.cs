#region Informations
// Auteur: 
// Date de cr�ation: 
// Date de modification:
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace TNS.AdExpress.Web.Controls.Buttons{
	/// <summary>
	/// Description r�sum�e de RedirectWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:RedirectWebControl runat=server></{0}:RedirectWebControl>")]
	public class RedirectWebControl : System.Web.UI.WebControls.Button{
		/// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output){
			output.Write("");
		}
	}
}
