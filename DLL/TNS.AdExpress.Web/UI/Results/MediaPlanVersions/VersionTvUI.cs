
#region Info
/*  Author : D. Mussuma
 * Creation : 05/09/2006
 * Modification :
 *		Author - Date - description
 * */
#endregion

using System;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Results;


namespace TNS.AdExpress.Web.UI.Results.MediaPlanVersions
{
	///<summary>VersionRadioUI provide methods to get html code to display a version of the vehicle Tv</summary>
	///  <author>dmussuma</author>
	///  <since>mardi 5 septembre 2006</since>
	public class VersionTvUI : VersionDetailUI {
	
		
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="version">Version to display</param>
		public VersionTvUI(WebSession webSession, VersionItem version):base(webSession, version){
		} 
		/// <summary>
		/// Constructor with exportVersion
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="exportVersion">Version to display</param>
		public VersionTvUI(WebSession webSession, ExportVersionItem exportVersion):base(webSession, exportVersion){
		}
		#endregion

		#region Render Version
		/// <summary>Render Version</summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		public override void GetHtml(StringBuilder output) {
			//Table
			output.Append("<table align=\"left\"  width=\"100%\" cellpadding=0 cellspacing=0  border=\"0\" bgcolor=\"#B1A3C1\">");
			
			//Render Verion visual
			output.Append("<tr ><td height=\"40px\" width=\"100%\" align=\"center\" bgcolor=\"#E0D7EC\">");
			this.RenderImage(output);
			output.Append("</td></tr>");

			//Render version nb cell
			output.Append("<tr ><td  bgcolor=\"#E0D7EC\" nowrap " + 
				((this._version.CssClass.Length>0)?"class=\"" + this._version.CssClass + "\">":"\">"));
			if(_webSession.SloganIdZoom<0){
				output.Append("<a href=\"javascript:get_version('"+this._version.Id+"');\" onmouseover=\"res_"+this._version.Id+".src='/Images/Common/button/result2_down.gif';\" onmouseout=\"res_"+this._version.Id+".src ='/Images/Common/button/result2_up.gif';\">");
				output.Append("<img name=\"res_"+this._version.Id+"\" border=0 align=\"absmiddle\" src=\"/Images/Common/button/result2_up.gif\">");
				output.Append("</a>");
			}
			output.Append("<font size=1>");
			output.Append("&nbsp;"+this._version.Id);
			output.Append("</font>");		
			output.Append("</td></tr>");
				
			//End table
			output.Append("</table>");
		}
		#endregion

		#region Méthodes
		///<summary>Render Version tv</summary>
		///  <author>dmussuma</author>
		///  <since>mardi 5 septembre 2006</since>
		protected override void RenderImage(StringBuilder output) {
		
			output.Append("<a href=\"javascript:openDownload('"+this._version.Path+"','"+this._webSession.IdSession+"','"+VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.tv).ToString()+"');\">");			
			output.Append("<img border=0 src=\"/Images/common/Picto_pellicule.gif\">");			
			output.Append("</a>");
		
		} 
		///<summary>Render Version Visual For Tv Export UI</summary>
		///<param name="output"></param>
		///  <author>rkaina</author>
		///  <since>vendredi 06 septembre 2006</since>
		protected override void RenderImageExport(StringBuilder output) {
		
			output.Append("<a href=\"javascript:openDownload('"+this._exportVersion.Path+"','"+this._webSession.IdSession+"','"+VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.tv).ToString()+"');\">");			
			output.Append("<img border=0 src=\"http://www.tnsadexpress.com/Images/common/Picto_pellicule.gif\">");			
			output.Append("</a>");
		
		} 
		#endregion
	}
}
