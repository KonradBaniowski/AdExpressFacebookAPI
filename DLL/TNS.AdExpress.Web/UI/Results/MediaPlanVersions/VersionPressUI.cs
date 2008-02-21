#region Info
/*
 * Author : G Ragneau
 * Creation : 13/07/2006
 * Modification :
 *		Author - Date - description
 *		Y. Rkaina - 18 août 2006 - Ajout de la méthode RenderImageExport(StringBuilder output)
 * */
#endregion

using System;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Common.Results;

namespace TNS.AdExpress.Web.UI.Results.MediaPlanVersions {


	///<summary>VersionPressWebControl provide methods to get html code to display a version of the vehicle Press</summary>
	///  <author>gragneau</author>
	///  <since>jeudi 13 juillet 2006</since>
	public class VersionPressUI : VersionDetailUI {

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="version">Version to display</param>
		public VersionPressUI(WebSession webSession, VersionItem version):base(webSession, version){
		}
		/// <summary>
		/// Constructor with exportVersion
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="exportVersion">Version to display</param>
		public VersionPressUI(WebSession webSession, ExportVersionItem exportVersion):base(webSession, exportVersion){
		}
		/// <summary>
		/// Constructor with exportVersion For APPM
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="exportAPPMVersion">Version to display</param>
		public VersionPressUI(WebSession webSession, ExportAPPMVersionItem exportAPPMVersion):base(webSession, exportAPPMVersion){
		}
        /// <summary>
        /// Constructor with exportVersion For MD
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="exportMDVersion">Version to display</param>
        public VersionPressUI(WebSession webSession, ExportMDVersionItem exportMDVersion): base(webSession, exportMDVersion){
        }
        /// <summary>
        /// Constructor with exportVersion For MD
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="exportOutdoorVersion">Version to display</param>
        public VersionPressUI(WebSession webSession, ExportOutdoorVersionItem exportOutdoorVersion): base(webSession, exportOutdoorVersion) {
        }
		#endregion

		#region Méthodes

		#region Render Version Visual
		///<summary>Render Version Visual</summary>
		///  <author>gragneau</author>
		///  <since>jeudi 13 juillet 2006</since>
		protected override void RenderImage(StringBuilder output) {
			string[] pathes = this._version.Path.Split(',');
			output.Append("<tr ><td align=\"left\" bgcolor=\"#E0D7EC\" >");
			output.Append("<table align=\"left\" border=0 cellpadding=0  cellspacing=0><tr >");
			foreach(string path in pathes){				
				output.Append("<td bgcolor=\"#E0D7EC\" >");
				output.Append("<a href=\"javascript:openPressCreation('" + this._version.Path.Replace("/imagette", "") + "');\">");
				output.Append("<img border=0 "
					+ ((path.Length>0)?" width=\"70px\" height=\"90px\" src=\"" + path + "\"" :"src=\"images/common/detailSpot_down.gif\"")
					+ ">");
				output.Append("</a>");
				output.Append("</td>");	
				
			}
			output.Append("</tr></table>");			
			output.Append("</td></tr>");
		} 
		#endregion

		#region Render Version Visual For Export UI
		///<summary>Render Version Visual For Export UI</summary>
		///<param name="output"></param>
		///<param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
		///  <author>rkaina</author>
		///  <since>vendredi 18 août 2006</since>
		protected override void RenderImageExport(StringBuilder output,Int64 index) {
			string[] pathes = null;
			Int64 lastIndex=index+5;

			if (_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE) { 
				pathes = this._exportAPPMVersion.Path.Split(',');
			}
			else {
                if(this._exportVersion != null)
				    pathes = this._exportVersion.Path.Split(',');
                else if (this._exportMDVersion != null)
                    pathes = this._exportMDVersion.Path.Split(',');
                else
                    pathes = this._exportOutdoorVersion.Path.Split(',');
			}

			Int64 end=0;
				
			if(pathes.Length<lastIndex)
				end=pathes.Length;
			else
				end=lastIndex;

			if((pathes.Length%5)==0) {
				if(pathes.Length>lastIndex) {
					end=lastIndex;
				} 
				else if(pathes.Length==lastIndex) {
					end=lastIndex-1;
				} 
				else {
					end=pathes.Length;
					index--;
				}
			}

//			if(pathes.Length==5){
//				if(index==0)
//					end=4;
//				else{
//					end=5;
//					index=4;
//				}
//			}

			output.Append("<table cellpadding=0 cellspacing=0 border=\"0\" bgcolor=\"#ffffff\">");
			output.Append("<tr>");
			
			//foreach(string path in pathes){
			for(Int64 i=index;i<end;i++){
				if((end-index)==1)
					output.Append("<td width=\"221px\" height=\"300px\" style=\" BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid; BORDER-RIGHT: white 1px solid; BORDER-LEFT: white 1px solid; \">&nbsp;");
				else
					output.Append("<td width=\"223px\" height=\"300px\" style=\" BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid; BORDER-RIGHT: white 1px solid; BORDER-LEFT: white 1px solid; \">&nbsp;");
				output.Append("</td>");

//				output.Append("<td width=\"100px\" height=\"250px\">");
//				output.Append("<img border=0"
//					+ ((pathes[i].Length>0)?" width=\"240px\" height=\"300px\" src=\"http://www.tnsadexpress.com/" + pathes[i].Replace("/imagette","") + "\"" :"src=\"images/common/detailSpot_down.gif\"")
//					+ ">");
//				output.Append("</td>");
			}
			output.Append("</tr>");
			output.Append("</table>");
		} 
		#endregion

		#region Render Version Visual For Export UI
		///<summary>Render Version Visual For Export UI</summary>
		///<param name="output"></param>
		///<param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
		///  <author>rkaina</author>
		///  <since>vendredi 18 août 2006</since>
		protected override void RenderAPPMImageExport(StringBuilder output,Int64 index) {
			string[] pathes = null;
			Int64 lastIndex=index+4;
			
			if (_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE) { 
				pathes = this._exportAPPMVersion.Path.Split(',');
			}
			else {
				pathes = this._exportVersion.Path.Split(',');
			}

			Int64 end=0;
				
			if(pathes.Length<index+4)
				end=pathes.Length;
			else
				end=index+4;

			if((pathes.Length%4)==0) {
				if(pathes.Length > lastIndex) {
					end=lastIndex;
				}
				else if(pathes.Length == lastIndex) {
					end=lastIndex-1;
				}
				else {
					end=pathes.Length;
					index--;
				}

			
			}
//			if(pathes.Length==4) {
//				if(index==0)
//					end=3;
//				else {
//					end=4;
//					index=3;
//				}
//			}

			output.Append("<table cellpadding=0 cellspacing=0 border=\"0\" bgcolor=\"#ffffff\">");
			output.Append("<tr>");
			
			//foreach(string path in pathes){
			for(Int64 i=index;i<end;i++) {
				//output.Append("<td width=\"100px\" height=\"250px\">");
				if((end-index)==1)
					output.Append("<td width=\"231px\" height=\"300px\" style=\" BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid; BORDER-RIGHT: white 1px solid; BORDER-LEFT: white 1px solid; \">&nbsp;");
				else
					output.Append("<td width=\"233px\" height=\"300px\" style=\" BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid; BORDER-RIGHT: white 1px solid; BORDER-LEFT: white 1px solid; \">&nbsp;");

//				output.Append("<img border=0"
//					+ ((pathes[i].Length>0)?" width=\"240px\" height=\"300px\" src=\"http://www.tnsadexpress.com/" + pathes[i].Replace("/imagette","") + "\"" :"src=\"images/common/detailSpot_down.gif\"")
//					+ ">");
				output.Append("</td>");
			}
			output.Append("</tr>");	
			output.Append("</table>");
		} 
		#endregion

		#endregion

	}
}
