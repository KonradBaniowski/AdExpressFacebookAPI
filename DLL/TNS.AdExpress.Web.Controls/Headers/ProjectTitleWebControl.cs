using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// Description r�sum�e de ProjectTitleWebControl.
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
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output)
		{
			output.Write(Text);
		}
	}
}
