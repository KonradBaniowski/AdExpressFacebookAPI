using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// Description résumée de ProjectTitleWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:ProjectTitleWebControl runat=server></{0}:ProjectTitleWebControl>")]
	public class ProjectTitleWebControl : System.Web.UI.WebControls.WebControl
	{
		private string text;
	
		/// <summary>
		/// Titre du projet
		/// </summary>
		[Bindable(true), 
			Category("Appearance"), 
			DefaultValue("")] 
		public string Text 
		{
			get
			{
				return text;
			}

			set
			{
				text = value;
			}
		}

		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output)
		{
			output.Write(Text);
		}
	}
}
