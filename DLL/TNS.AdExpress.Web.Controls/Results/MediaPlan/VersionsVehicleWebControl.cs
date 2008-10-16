#region Info
/*
 * Author : G Ragneau
 * Creation : 13/07/2006
 * Modification :
 *		Author - Date - description
 * 
 * */
#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;


using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.DataAccess.Results;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using WeBCst = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Controls.Results.MediaPlan
{
	/// <summary>
	/// VersionsVehicleWebControl provide a web control to display a set of version all from the same vehicle
	/// </summary>
	[ToolboxData("<{0}:VersionsVehicleWebControl runat=server></{0}:VersionsVehicleWebControl>")]
	public class VersionsVehicleWebControl : System.Web.UI.WebControls.WebControl
	{

		#region Variables
		/// <summary>
		/// WebControl title
		/// </summary>
		private string _title;
		///<summary>List of Versions</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		private Hashtable _versions;
		/// <summary>
		/// Media Classification considered in the web control
		/// </summary>
		///<directed>True</directed>
		private DBCst.Vehicles.names _vehicle;
		/// <summary>
		/// Customer web session
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Number of versions on a line
		/// </summary>
		public const int NB_COLUMN = 5;
		#endregion

		#region Accessors
		/// <summary>
		/// Get / Set Customer web session
		/// </summary>
		public WebSession Session{
			get{return _webSession;}
			set{_webSession = value;}
		}
		/// <summary>
		/// Get / Set Considered Vehicle
		/// </summary>
		public DBCst.Vehicles.names Vehicle {
			get{return _vehicle;}
			set{_vehicle = value;}
		}
		/// <summary>
		/// Get / Set Web Control title
		/// </summary>
		public string Title 
		{
			get{return _title;}
			set{_title = value;}
		}
		///<summary>Get / Set versions</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public Hashtable versions {
    		get {return (_versions);}
    		set {_versions = value;}
		}
		#endregion

		#region Events
		/// <summary>
		/// Initialise all webcontrols
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);

			#region Get Data from persistent layer
			//TODO Get Data from database
			DataTable dtTbl = null;
			switch(this._vehicle){
				case DBCst.Vehicles.names.press:
					if (_webSession.CurrentModule.Equals(WeBCst.Module.Type.chronoPress)){
						dtTbl = VersionDataAccess.GetAPPMVersions(_versions.Keys, _webSession).Tables[0];
					}
					else{
//						dtTbl = VersionDataAccess.GetPressVersions(_versions.Keys, _webSession).Tables[0];
						dtTbl = VersionDataAccess.GetVersions(_versions.Keys, _webSession,DBCst.Vehicles.names.press).Tables[0];
					}
					break;
				case DBCst.Vehicles.names.internationalPress:
					dtTbl = VersionDataAccess.GetVersions(_versions.Keys, _webSession,DBCst.Vehicles.names.internationalPress).Tables[0];
					break;
				default:
					throw new VersionsVehicleWebControlException("Non authorized vehicle level : " + this._vehicle.ToString());
			}
			#endregion
			
			#region Build Set of VersionControl
			//Create each webcontrol
			string path = string.Empty;
			string[] pathes = null;
			string dirPath = string.Empty;
			VersionItem item = null;
			VersionDetailWebControl versionWebCtrl = null;

			if(dtTbl.Rows != null && dtTbl.Rows.Count > 0){
				foreach(DataRow row in dtTbl.Rows){
					if (row["visual"] != DBNull.Value){
						
						//build different pathes
						pathes = row["visual"].ToString().Split(',');
						path = string.Empty;
						dirPath = this.BuildVersionPath(row["idMedia"].ToString(), row["datenum"].ToString());
						foreach(string str in pathes){
							path += Path.Combine(dirPath, str) + ",";
						}

						//fill version path
						item = ((VersionItem) this._versions[row["id"].ToString()]);
						if (path.Length>0){
							item.Path = path.Substring(0, path.Length - 1);
						}

						//build control
						switch(this._vehicle){
							case DBCst.Vehicles.names.press:
								if (_webSession.CurrentModule.Equals(WeBCst.Module.Type.chronoPress)){
									versionWebCtrl = new VersionPressAPPMWebControl();
								}
								else{
									versionWebCtrl = new VersionPressWebControl();
								}
								break;
							case DBCst.Vehicles.names.internationalPress:
								versionWebCtrl = new VersionPressWebControl();
								break;
							default:
								throw new VersionsVehicleWebControlException("Non authorized vehicle level : " + this._vehicle.ToString());
						}
						versionWebCtrl.Session = this._webSession;
						versionWebCtrl.Version = item;
						this.Controls.Add(versionWebCtrl);
					}
				}
			}
			#endregion

		}
		/// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output)
		{
			output.WriteLine("<table border=\"0\" class=\"txtViolet12Bold\"");
			output.Write("<tr><td colSpan=\"" + NB_COLUMN + "\">");
			switch(this._vehicle){
				case DBCst.Vehicles.names.press:
					output.Write(GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage));
					break;
				case DBCst.Vehicles.names.internationalPress:
					output.Write(GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage));
					break;
				default:
					output.Write("?");
					break;
			}
			output.WriteLine("</td></tr>");

			int columnIndex = 0;

			foreach(WebControl control in this.Controls){

				if ((columnIndex % NB_COLUMN) == 0){
					if (columnIndex > 0){
						output.WriteLine("</tr>");
					}
					output.WriteLine("<tr>");
				}
				output.Write("<td>");
				control.RenderControl(output);
				output.Write("</td>");
			}			
			output.WriteLine("</tr>");
			output.WriteLine("</table>");
		}
		#endregion

		#region Internal Methods
		/// <summary>
		/// Build visual access path depending on the vehicle
		/// </summary>
		/// <param name="date">date to format YYYYMMDD</param>
		/// <param name="idMedia">Media Id</param>
		/// <returns>Full path to access an image</returns>
		private string BuildVersionPath(string idMedia, string date){
			string path = string.Empty;
			switch(this._vehicle){
				case DBCst.Vehicles.names.press:
				case DBCst.Vehicles.names.internationalPress:
					path = Path.Combine(WeBCst.CreationServerPathes.IMAGES, idMedia + "/" + date + "/imagettes"); 
					break;
				default:
					throw new VersionsVehicleWebControlException("Non authorized vehicle level : " + this._vehicle.ToString());
			}			
			return path;
		}
		#endregion


	}
}
