using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace TNS.AdExpress.Web.UI
{
	/// <summary>
	/// Description résumée de HtmlHeader.
	/// </summary>
	[ToolboxData("<{0}:HtmlHeader runat=server></{0}:HtmlHeader>")]
	public class HtmlHeader : System.Web.UI.HtmlControls.HtmlGenericControl
	{
		#region variables
		/// <summary>
		/// Liste des clés des styles css
		/// </summary>
		protected  ArrayList _cssKeys = null;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public HtmlHeader(ArrayList cssKeys ) : base("head") {
			_cssKeys = cssKeys;
		}
		#endregion

	
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="writer"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter writer) {
			writer.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >");
			writer.RenderBeginTag("html");					
			writer.RenderBeginTag("title");
			writer.WriteLine("AdExpress");
			//writer.WriteLine(this.Page.Title);
			writer.RenderEndTag();
			this.RenderBeginTag(writer);	
			writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"application/vnd.ms-excel; charset=iso-8859-1, windows-1252\">");
			writer.WriteLine("<meta name=\"vs_defaultClientScript\" content=\"JavaScript\">");
			writer.WriteLine("<meta name=\"vs_targetSchema\" content=\"http://schemas.microsoft.com/intellisense/ie5\">");
			writer.WriteLine("<meta http-equiv=\"expires\" content=\"Wed, 23 Feb 1999 10:49:02 GMT\">");
			writer.WriteLine("<meta http-equiv=\"expires\" content=\"0\">");
			writer.WriteLine("<meta http-equiv=\"pragma\" content=\"no-cache\">");
			writer.WriteLine("<meta name=\"Cache-control\" content=\"no-cache\">");
			if(_cssKeys !=null && _cssKeys.Count>0){
				for(int i=0;i<_cssKeys.Count;i++)	
					writer.WriteLine(TNS.AdExpress.Web.Core.CssStylesList.GetCssStyles(_cssKeys[i].ToString()));
			}
			this.RenderEndTag(writer);
		}

	}
}
