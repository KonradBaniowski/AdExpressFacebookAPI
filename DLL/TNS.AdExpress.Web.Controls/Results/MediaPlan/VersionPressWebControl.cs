#region Info
/*
 * Author : G Ragneau
 * Creation : 13/07/2006
 * Modification :
 *		Author - Date - description
 * 
 * */
#endregion

namespace TNS.AdExpress.Web.Controls.Results.MediaPlan {


	///<summary>VersionPressWebControl provide control to display a version of the vehicle Press</summary>
	///  <author>gragneau</author>
	///  <since>jeudi 13 juillet 2006</since>
	public class VersionPressWebControl : VersionDetailWebControl {

		#region Méthodes
		///<summary>Render Version Visual</summary>
		///  <author>gragneau</author>
		///  <since>jeudi 13 juillet 2006</since>
		protected override void RenderImage(System.Web.UI.HtmlTextWriter output) {
			string[] pathes = this._version.Path.Split(',');
			foreach(string path in pathes){
				output.Write("<img "
					+ ((path.Length>0)?"width=\"33%\" height=\"33%\" src=\"" + path + "\"" :"src=\"images/common/detailSpot_down.gif\"")
					+ "\">");
			}
		}
		#endregion

	}
}
