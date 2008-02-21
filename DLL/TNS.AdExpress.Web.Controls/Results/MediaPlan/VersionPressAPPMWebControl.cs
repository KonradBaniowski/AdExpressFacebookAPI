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


	///<summary>VersionPressAPPMWebControlprovide a control to render a version in the APPM module</summary>
	///  <author>gragneau</author>
	///  <since>jeudi 13 juillet 2006</since>
	public class VersionPressAPPMWebControl : VersionPressWebControl {

		#region Events
		/// <summary>
		/// Register javascripts
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(System.EventArgs e) {
			base.OnPreRender (e);
			//TODO Script Registration
//			if(!this.Page.ClientScript.IsClientScriptBlockRegistered("")){
//				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"",);
//			}
		}
		#endregion

	    #region Méthodes
		///<summary>Render Version SYnthesis</summary>
		///  <author>gragneau</author>
		///  <since>jeudi 13 juillet 2006</since>
		protected override void RenderSynthesis( System.Web.UI.HtmlTextWriter output ) {
			output.Write("Synthèse");
		}
		#endregion

	}
}
