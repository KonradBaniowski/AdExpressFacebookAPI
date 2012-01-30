#region Info
/*
 *  Author : D. Mussuma
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
using System.Reflection;

namespace TNS.AdExpress.Web.UI.Results.MediaPlanVersions
{
	///<summary>VersionRadioUI provide methods to get html code to display a version of the vehicle Radio</summary>
	///  <author>dmussuma</author>
	///  <since>mardi 5 septembre 2006</since>
	public class VersionRadioUI  : VersionDetailUI {
	
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="version">Version to display</param>
        public VersionRadioUI(WebSession webSession, VersionItem version, long vehicleId)
            : base(webSession, version)
        {
            _vehicleId = vehicleId;
		} 
		/// <summary>
		/// Constructor with exportVersion
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="exportVersion">Version to display</param>
        public VersionRadioUI(WebSession webSession, ExportVersionItem exportVersion, long vehicleId)
            : base(webSession, exportVersion)
        {
            _vehicleId = vehicleId;
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
            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.creativesUtilities];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the creatives utilities class"));
            TNS.AdExpress.Web.Core.Utilities.Creatives creativesUtilities = (TNS.AdExpress.Web.Core.Utilities.Creatives)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null);

            if (!creativesUtilities.IsSloganZoom(_webSession.SloganIdZoom))
            {
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
		///<summary>Render Version radio</summary>
		///  <author>dmussuma</author>
		///  <since>mardi 5 septembre 2006</since>
		protected override void RenderImage(StringBuilder output) {
			
			output.Append("<a href=\"javascript:openDownload('"+this._version.Path+","+this._version.Id+"','"+this._webSession.IdSession+"','"+_vehicleId.ToString()+"');\">");		
			output.Append("<img border=0 src=\"/Images/common/Picto_Radio.gif\">");			
			output.Append("</a>");
			
		} 
		#endregion
	}
}
