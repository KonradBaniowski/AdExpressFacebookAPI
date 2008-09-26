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
using System.Text;


using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Exceptions;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using WeBCst = TNS.AdExpress.Constantes.Web;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.UI.Results.MediaPlanVersions
{
	/// <summary>
	/// VersionsVehicleUI provide methods to get html code to display a set of version all from the same vehicle
	/// </summary>
	public class VersionsVehicleUI
	{

		#region Variables
		/// <summary>
		/// WebControl title
		/// </summary>
		private string _title = string.Empty;
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
		private int _nb_column = 10;
		/// <summary>
		/// Object genberating html code
		/// </summary>
		private	ArrayList _versionsUIs;
        /// <summary>
        /// Période utilisée
        /// </summary>
        private MediaSchedulePeriod _period = null;
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
		/// <summary>
		/// Get / Set Columns number
		/// </summary>
		public int Nb_Columns {
			get{return _nb_column;}
			set{_nb_column = value;}
		}
		///<summary>Get / Set versions</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public Hashtable versions {
    		get {return (_versions);}
    		set {_versions = value;}
		}
        ///<summary>Get / Set La période sélectionnée</summary>
        /// <author>yrkaina</author>
        /// <since>jeudi 24 janvier 2008</since>
        public MediaSchedulePeriod Period {
            get { return (_period); }
            set { _period = value; }
        }
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="versions">List of verions details indexed by their Id</param>
		/// <param name="vehicle">Vehicle considered</param>
		public VersionsVehicleUI(WebSession webSession, Hashtable versions, DBCst.Vehicles.names vehicle){
			this._webSession = webSession;
			this._versions = versions;
			this._vehicle = vehicle;
		}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="versions">List of verions details indexed by their Id</param>
        /// <param name="vehicle">Vehicle considered</param>
        /// <param name="period">Période utilisée</param>
        public VersionsVehicleUI(WebSession webSession, Hashtable versions, DBCst.Vehicles.names vehicle, MediaSchedulePeriod period) {
            this._webSession = webSession;
            this._versions = versions;
            this._vehicle = vehicle;
            this._period = period;
        }
		#endregion

		#region Public Methods
		/// <summary>
		/// Build Html code to display the set of version
		/// </summary>
		/// <returns>Html Code</returns>
		public string GetHtml(){
			SetUp();
			StringBuilder htmlBld = new StringBuilder(10000);
			BuildHtml(htmlBld);
			return htmlBld.ToString();
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Initialise all webcontrols
		/// </summary>
		protected void SetUp() {

			#region Get Data from persistent layer
			//TODO Get Data from database
			DataSet dtSet = null;
			switch(this._vehicle){
				case DBCst.Vehicles.names.press:
					if (_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE){
						dtSet = VersionDataAccess.GetAPPMVersions(_versions.Keys, _webSession);
					}
					else{
//						dtSet = VersionDataAccess.GetPressVersions(_versions.Keys, _webSession);
						dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession,DBCst.Vehicles.names.press,_period);
					}
					break;

				case DBCst.Vehicles.names.internationalPress:
					dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession,DBCst.Vehicles.names.internationalPress,_period);
					break;

				case DBCst.Vehicles.names.radio:
					dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession,DBCst.Vehicles.names.radio,_period);
					break;

				case DBCst.Vehicles.names.tv:										
					dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession,DBCst.Vehicles.names.tv,_period);					
					break;

                case DBCst.Vehicles.names.directMarketing:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.directMarketing,_period);
                    break;
                
                case DBCst.Vehicles.names.outdoor:
                    dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.outdoor,_period);
                    break;

				default:
					throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
			}
			#endregion
			
			#region Build Set of VersionControl
			//Create each webcontrol
			string path = string.Empty;
			string[] pathes = null;
			string dirPath = string.Empty;
			VersionItem item = null;
			VersionDetailUI versionUi = null;

			if(dtSet != null && dtSet.Tables[0].Rows != null && dtSet.Tables[0].Rows.Count > 0){
				this._versionsUIs = new ArrayList();
				foreach(DataRow row in dtSet.Tables[0].Rows){
					if (row["visual"] != DBNull.Value){
						
						//build different pathes
						switch(this._vehicle){

							case DBCst.Vehicles.names.press:
							case DBCst.Vehicles.names.internationalPress:
								pathes = row["visual"].ToString().Split(',');
								path = string.Empty;
                                foreach (string str in pathes) {
                                    path += Functions.Creatives.GetCreativePath(Int64.Parse(row["idMedia"].ToString()), Int64.Parse(row["dateKiosque"].ToString()), Int64.Parse(row["dateCover"].ToString()), str, true, true) + ",";
                                }
								break;

                            case DBCst.Vehicles.names.directMarketing:
                                pathes = row["visual"].ToString().Split(',');
                                path = string.Empty;
                                dirPath = this.BuildVersionPath(row["id"].ToString(), WeBCst.CreationServerPathes.IMAGES_MD);
                                foreach (string str in pathes){
                                    path += dirPath + "/" + str + ",";
                                }
                                break;

                            case DBCst.Vehicles.names.outdoor:
                                pathes = row["visual"].ToString().Split(',');
                                path = string.Empty;
                                dirPath = this.BuildVersionPath(row["id"].ToString(), WeBCst.CreationServerPathes.IMAGES_OUTDOOR);
                                foreach (string str in pathes) {
                                    path += dirPath + "/" + str + ",";
                                }
                                break;
                                
							case DBCst.Vehicles.names.radio:
							case DBCst.Vehicles.names.tv:
								path = row["visual"].ToString();
								break;

							default :
								break;
						}

						//fill version path
						item = ((VersionItem) this._versions[(Int64)row["id"]]);
						if (item == null){
							continue;
						}
						

						//build control
						switch(this._vehicle){
							case DBCst.Vehicles.names.press:
								if (path.Length>0){
									item.Path = path.Substring(0, path.Length - 1);
								}
								if (_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE){
                                    if (row["dateKiosque"] != System.DBNull.Value) item.Parution = row["dateKiosque"].ToString();
									else item.Parution=""; 
									versionUi = new VersionPressAPPMUI(this._webSession, item);
								}
								else{
									versionUi = new VersionPressUI(this._webSession, item);
								}
								break;

							case DBCst.Vehicles.names.internationalPress:
								if (path.Length>0){
									item.Path = path.Substring(0, path.Length - 1);
								}
								versionUi = new VersionPressUI(this._webSession, item);
								break;

                            case DBCst.Vehicles.names.directMarketing:
                            case DBCst.Vehicles.names.outdoor:
                                if (path.Length > 0){
                                    item.Path = path.Substring(0, path.Length - 1);
                                }
                                versionUi = new VersionPressUI(this._webSession, item);
                                break;

							case DBCst.Vehicles.names.radio:
								if (path.Length>0){
									item.Path = path;
								}
								versionUi = new VersionRadioUI(this._webSession, item);
								break;

							case DBCst.Vehicles.names.tv:
								if (path.Length>0){
									item.Path = path;
								}
								versionUi = new VersionTvUI(this._webSession, item);
								break;

							default:
								throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
						}
						this._versionsUIs.Add(versionUi);
					}
				}
			}
			#endregion

		}
		/// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns>
		protected void BuildHtml(StringBuilder output){

			if (this._versionsUIs != null){
                output.Append("<table align=\"left\" border=\"0\" class=\"violetBackGroundV3 txtBlanc12Bold\">");
				output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
				if (_title == string.Empty){
					switch(this._vehicle){
						case DBCst.Vehicles.names.press:
							_title = GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage);
							break;

						case DBCst.Vehicles.names.internationalPress:
							_title = GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage);
							break;

						case DBCst.Vehicles.names.tv:
							_title = GestionWeb.GetWebWord(2012, this._webSession.SiteLanguage);
							break;

						case DBCst.Vehicles.names.radio:
							_title = GestionWeb.GetWebWord(2011, this._webSession.SiteLanguage);
							break;

                        case DBCst.Vehicles.names.directMarketing:
                            _title = GestionWeb.GetWebWord(2217, this._webSession.SiteLanguage);
							break;

                        case DBCst.Vehicles.names.outdoor:
                            _title = GestionWeb.GetWebWord(2255, this._webSession.SiteLanguage);
                            break;

						default:
							_title = "?";
							break;
					}
				}
				output.Append(_title);
				output.Append("</td></tr>");

				int columnIndex = 0;
				foreach(VersionDetailUI item in  this._versionsUIs){

					if ((columnIndex % Nb_Columns) == 0){
						if (columnIndex > 0){
							output.Append("</tr>");
						}
						output.Append("<tr>");
					
					}
					output.Append("<td>");
					item.GetHtml(output);
					output.Append("</td>");
					columnIndex++;
				}			
				output.Append("</tr>");
				output.Append("</table>");
			}
		}
		#endregion

		#region Internal Methods
		/// <summary>
		/// Build visual access path depending on the vehicle
		/// </summary>
		/// <param name="date">date to format YYYYMMDD</param>
		/// <param name="idMedia">Media Id</param>
		/// <returns>Full path to access an image</returns>
		private string BuildVersionPath(string idMedia, string date, string folderPath){
			string path = string.Empty;
			switch(this._vehicle){
				case DBCst.Vehicles.names.press:
				case DBCst.Vehicles.names.internationalPress:
                    path = folderPath + "/" + idMedia + "/" + date + "/imagette"; 
					break;
				default:
					throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
			}			
			return path;
		}
        /// <summary>
        /// Build visual access path for Marketing Direct & Outdoor
        /// </summary>
        /// <param name="idSlogan">Slogan ID</param>
        /// <returns>Full path to access an image</returns>
        private string BuildVersionPath(string idSlogan, string folderPath) {
            string path = string.Empty;
            path = folderPath;
            string dir1 = idSlogan.Substring(idSlogan.Length - 1, 1);
            path = string.Format(@"{0}/{1}", path, dir1);
            string dir2 = idSlogan.Substring(idSlogan.Length - 2, 1);
            path = string.Format(@"{0}/{1}", path, dir2);
            string dir3 = idSlogan.Substring(idSlogan.Length - 3, 1);
            path = string.Format(@"{0}/{1}/imagette", path, dir3);
            return path;
        }
		#endregion

	}
}
