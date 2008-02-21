#region Info
/*
 * Author : G Facon / G Ragneau
 * Creation : 13/07/2006
 * Modification :
 *		Author - Date - description
 * 
 * */
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.Core.Sessions;

using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.Controls.Results.MediaPlan{

	/// <summary>
	/// VersionDetailWebControl provides process to display a version
	/// </summary>
	[ToolboxData("<{0}:VersionDetailWebControl runat=server></{0}:VersionDetailWebControl>")]
	public class VersionDetailWebControl : System.Web.UI.WebControls.WebControl{

		#region Variables
		/// <summary>
		/// Objet session
		/// </summary>
		protected WebSession _webSession=null;
		///<summary>Version to display</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		protected VersionItem _version;
		#endregion

		#region Accessors
		///<summary>Get / Set Version</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public VersionItem Version {
    		get {
        		return (_version);
    		}
    		set {
        		_version = value;
    		}
		}
		///<summary>Get / Set WebSession</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public WebSession Session {
			get {
				return (_webSession);
			}
			set {
				_webSession = value;
			}
		}
		#endregion
	
		#region Evènements

		#region Render
		/// <summary>Render Version</summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			//Table
			output.WriteLine("<table border=\"0\">");
			//Render Verion visual
			output.Write("<tr><td align=\"center\">");
			this.RenderImage(output);
			output.Write("</td></tr>");

			//Render version nb cell
			output.Write("<tr><td " + 
				((this._version.CssClass.Length>0)?"class=\"" + this._version.CssClass + "\">":"\">"));
			output.Write(this._version.Id);
			output.Write("</td></tr>");


			//Render parution date
			output.Write("<tr><td>");
			output.Write(DateString.YYYYMMDDToDD_MM_YYYY( this._version.Parution, this._webSession.SiteLanguage));
			output.Write("</td></tr>");


			//Render version synthesis
			output.Write("<tr><td>");
			this.RenderSynthesis(output);
			output.Write("</td></tr>");
			//End table
			output.Write("</table>");
		}
		#endregion

		#endregion

		#region Méthodes

		///<summary>Render Version Visual</summary>
		///  <author>gragneau</author>
		///  <since>jeudi 13 juillet 2006</since>
		protected virtual void RenderImage( HtmlTextWriter output ) {
			string[] pathes = this._version.Path.Split(',');
			foreach(string path in pathes){
				output.Write("<img src=\""
					+((path.Length>0)?path:"images/common/Picto_Radio.gif")
					+ "\">");
			}
		}
		///<summary>Render Version SYnthesis</summary>
		///  <author>gragneau</author>
		///  <since>jeudi 13 juillet 2006</since>
		protected virtual void RenderSynthesis( System.Web.UI.HtmlTextWriter output ) {
		}
		#endregion

	}
}
