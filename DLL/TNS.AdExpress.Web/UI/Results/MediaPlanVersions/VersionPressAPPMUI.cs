#region Info
/*
 * Author : G Ragneau
 * Creation : 13/07/2006
 * Modification :
 *		Author - Date - description
 * 
 * */
#endregion

using System.Text;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Common.Results;
using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.UI.Results.MediaPlanVersions {


	///<summary>VersionPressAPPMUI provide a control to render a version in the APPM module</summary>
	///  <author>gragneau</author>
	///  <since>jeudi 13 juillet 2006</since>
	public class VersionPressAPPMUI : VersionPressUI {

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="version">Version to display</param>
		public VersionPressAPPMUI(WebSession webSession, VersionItem version):base(webSession, version){
		}
		/// <summary>
		/// Constructor with exportVersion
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="exportAPPMVersion">Version to display</param>
		public VersionPressAPPMUI(WebSession webSession, ExportAPPMVersionItem exportAPPMVersion):base(webSession, exportAPPMVersion)
		{
		}
		#endregion

	    #region Méthodes
		///<summary>Render Version SYnthesis</summary>
		///  <author>gragneau</author>
		///  <since>jeudi 13 juillet 2006</since>
		protected override void RenderSynthesis( StringBuilder output ) {
			//Beginning synthessis link and parution date tables
            output.Append("<tr height=100%><td align=\"left\"   height=100% width=100% " +
                ((this._version.CssClass.Length > 0) ? "class=\"violetBackGroundV3 " + this._version.CssClass + "\">" : "\">"));
			output.Append("<table height=100%  cellpadding=1 cellspacing=1  width=100% >");
				

			//Render parution date
			output.Append("<tr width=100% style=\"\">");
            output.Append("<td width=100%  nowrap    class=\"sloganVioletBackGround\" ><p>"); 
			if(this._version.Parution!=null && this._version.Parution.Length>0){				
				output.Append("<font size=1 class=\"txtNoir11\">");
				output.Append("&nbsp;"+DateString.YYYYMMDDToDD_MM_YYYY( this._version.Parution, this._webSession.SiteLanguage));
				output.Append("</font>");
			}

			//Render version synthesis
			output.Append("<br>");
			//Link and javascript call to open synthesis
			output.Append("<a class=\"roll02\" href=\"javascript:popupOpen('/Private/Results/APPMVersionSynthesis.aspx?idSession="+ _webSession.IdSession +"&idVersion=" + this._version.Id+"&firstInsertionDate=" +this._version.Parution+"','800','600');\">&nbsp;" + GestionWeb.GetWebWord(1664, _webSession.SiteLanguage) + "</a>");			
			output.Append("</p></td></tr>");
			output.Append("</table>");
			output.Append("</td></tr>");
			//Ending synthsesis link and parution date tables
						
		}
		#endregion

	}
}
