#region Info
/*
 * Author : Y. Rkaina
 * Creation : 17/08/2006
 * Modification :
 *		Author - Date - description
 * 
 * */
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Collections.Specialized;


using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Exceptions;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using WeBCst = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;
using CustomerCst=TNS.AdExpress.Constantes.Customer;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Selection;

using TNS.FrameWork;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpressI.Insertions;
using TNS.AdExpressI.Insertions.Cells;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.WebTheme;

namespace TNS.AdExpress.Web.UI.Results.MediaPlanVersions
{
	/// <summary>
	/// DetailVersionsVehicleUI provide methods to get html code to display a set of version all from the same vehicle
	/// </summary>
	public class ExportVersionsVehicleUI{

		#region Variables
		/// <summary>
		/// WebControl title
		/// </summary>
		private string _title = string.Empty;
		///<summary>List of Versions</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
        private Dictionary<Int64, VersionItem> _versions = new Dictionary<long, VersionItem>();
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
		private int _nb_column = 1;
		/// <summary>
		/// Object genberating html code
		/// </summary>
		private	ArrayList _versionsUIs;
        /// <summary>
        /// Show product
        /// </summary>
		private bool _showProduct = true;
        /// <summary>
        /// Période utilisée
        /// </summary>
        private MediaSchedulePeriod _period = null;
        /// <summary>
        /// Zoom date
        /// </summary>
        private string _zoomDate = string.Empty;
		#endregion

		#region Accessors
		/// <summary>
		/// Get / Set Customer web session
		/// </summary>
		public WebSession Session {
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
		public string Title {
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
        public Dictionary<Int64, VersionItem> versions {
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
        ///<summary>Get / Set Zoom date</summary>
        public string ZoomDate {
            get { return (_zoomDate); }
            set { _zoomDate = value; }
        }
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="versions">List of verions details indexed by their Id</param>
		/// <param name="vehicle">Vehicle considered</param>
        public ExportVersionsVehicleUI(WebSession webSession, Dictionary<Int64, VersionItem> versions, DBCst.Vehicles.names vehicle) {
			this._webSession = webSession;
			this._versions = versions;
			this._vehicle = vehicle;
			_showProduct = _webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

		}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicle">Vehicle considered</param>
        /// <param name="period">Period</param>
        /// <param name="zoomDate">Zoom date</param>
        public ExportVersionsVehicleUI(WebSession webSession, DBCst.Vehicles.names vehicle, MediaSchedulePeriod period, string zoomDate) {
            this._webSession = webSession;
            this._vehicle = vehicle;
            this._period = period;
            this._zoomDate = zoomDate;
            _showProduct = _webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
        }	
		#endregion

		#region Public Methods

        #region GetHtml
        /// <summary>
		/// Build Html code to display the set of version
		/// </summary>
		/// <returns>Html Code</returns>
		public string GetHtml(ref ArrayList versionsUIs) {
			StringBuilder htmlBld = new StringBuilder(10000);

			switch(this._vehicle) {
				case DBCst.Vehicles.names.press: 
				case DBCst.Vehicles.names.internationalPress:
					PressSetUp();
					versionsUIs = this._versionsUIs;
					PressBuildHtml(htmlBld);
					break;
				case DBCst.Vehicles.names.tv: {
					TvSetUp();
					versionsUIs = this._versionsUIs;
					TvBuildHtml(htmlBld);
					break;
				}
				case DBCst.Vehicles.names.radio: {
					RadioSetUp();
					versionsUIs = this._versionsUIs;
					RadioBuildHtml(htmlBld);
					break;
				}
                case DBCst.Vehicles.names.directMarketing: {
                    MDSetUp();
                    versionsUIs = this._versionsUIs;
					MDBuildHtml(htmlBld);
					break;
                }
                case DBCst.Vehicles.names.outdoor: {
                    OutdoorSetUp();
                    versionsUIs = this._versionsUIs;
                    OutdoorBuildHtml(htmlBld);
                    break;
                }
                case DBCst.Vehicles.names.instore: {
                        InStoreSetUp();
                        versionsUIs = this._versionsUIs;
                        InStoreBuildHtml(htmlBld);
                        break;
                    }
			}
	
			return htmlBld.ToString();
        }
        #endregion

        #region GetAPPMHtml
        /// <summary>
		/// Build Html code to display the set of version
		/// </summary>
		/// <returns>Html Code</returns>
		public ArrayList GetAPPMHtml(IDataSource dataSource, String title, ref ArrayList versionsUIs) {
			ArrayList partitHTMLVersion = new ArrayList();
			APPMSetUp(dataSource);
			versionsUIs = this._versionsUIs;
			partitHTMLVersion = APPMBuildHtml(title);
			return partitHTMLVersion;
        }
        #endregion

        #region GetMSCreativesHtml
        /// <summary>
		/// Build Html code to display the set of version
		/// </summary>
        /// <param name="creativeCells">Images path</param>
        /// <param name="style">Style</param>
		/// <returns>Html Code</returns>
        public string GetMSCreativesHtml(ref SortedDictionary<Int64, List<CellCreativesInformation>> creativeCells, Style style) {

            StringBuilder htmlBld = new StringBuilder(10000);

            #region MSCreatives
            object[] paramMSCraetives = new object[2];
            paramMSCraetives[0] = _webSession;
            paramMSCraetives[1] = _webSession.CurrentModule;
            ResultTable resultTable = null;
            IInsertionsResult resultMSCreatives = (IInsertionsResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + "TNS.AdExpressI.Insertions.Default.dll", "TNS.AdExpressI.Insertions.Default.InsertionsResult", false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, paramMSCraetives, null, null, null);
            string[] vehicles = _webSession.GetSelection(_webSession.SelectionUniversMedia, CustomerCst.Right.type.vehicleAccess).Split(',');
            string filters = string.Empty;
            int fromDate = Convert.ToInt32(_period.Begin.ToString("yyyyMMdd"));
            int toDate = Convert.ToInt32(_period.End.ToString("yyyyMMdd"));
            VehicleInformation currentVehicle = VehiclesInformation.Get(Int64.Parse(vehicles[0]));
            #endregion

            resultTable = resultMSCreatives.GetMSCreatives(VehiclesInformation.Get(Int64.Parse(vehicles[0])), fromDate, toDate, filters, -1, _zoomDate);
            if (resultTable != null)
                resultTable.CultureInfo = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;

            if (currentVehicle.Id == DBCst.Vehicles.names.tv || currentVehicle.Id == DBCst.Vehicles.names.radio) {
                List<Color> colorList = ((Colors)style.GetTag("msCreativesColor")).ColorList;
                BuildMSCreativesHtml(htmlBld, resultTable, colorList);
            }
            else {
                creativeCells = InitCreativeCells(resultTable);
                BuildMSCreativesWithImgHtml(htmlBld, creativeCells);
            }
            return htmlBld.ToString();

        }
        #endregion

        #endregion

        #region Protected Methods

        #region BuildMSCreativesWithImgHtml
        /// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns>
        protected void BuildMSCreativesWithImgHtml(StringBuilder output, SortedDictionary<Int64, List<CellCreativesInformation>> creativeCells) {

            int startToMedium = 0, mediumToEnd = 0, end = 0, indexTable = 0;

            output.Append("<table align=\"left\" border=\"0\" class=\"txtViolet14BoldWhiteBG\">");
            output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
            if (_title == string.Empty) {
                switch (this._vehicle) {
                    case DBCst.Vehicles.names.press:
                        _title = Convertion.ToHtmlString(GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage));
                        break;
                    case DBCst.Vehicles.names.directMarketing:
                        _title = Convertion.ToHtmlString(GestionWeb.GetWebWord(2219, this._webSession.SiteLanguage));
                        break;
                    case DBCst.Vehicles.names.outdoor:
                        _title = Convertion.ToHtmlString(GestionWeb.GetWebWord(2255, this._webSession.SiteLanguage));
                        break;
                    case DBCst.Vehicles.names.instore:
                        _title = Convertion.ToHtmlString(GestionWeb.GetWebWord(2667, this._webSession.SiteLanguage));
                        break;
                    default:
                        _title = "?";
                        break;
                }
            }
            output.Append(_title);
            output.Append("</td></tr>");

            int columnIndex = 0;
            Int64 nbVisuals = 0;
            foreach (List<CellCreativesInformation> currentList in creativeCells.Values) {
                foreach (CellCreativesInformation cell in currentList) {

                    nbVisuals = cell.NbVisuals;

                    if (nbVisuals == 1) {
                        startToMedium = 1;
                        if ((columnIndex % 2) == 0) {

                            if (columnIndex > 0) {
                                output.Append("</tr>");
                                if ((columnIndex % 4) == 0) {
                                    output.Append("<br>");
                                }
                            }
                            output.Append("<tr>");

                        }
                        output.Append("<td>");
                        if (indexTable == 0) {
                            output.Append("<table>");
                            output.Append("<tr>");
                            output.Append("<td>");
                        }

                        output.Append(cell.RenderPDF(true, 0));

                        indexTable++;

                        if (indexTable == 2) {
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }
                        output.Append("</td>");
                        columnIndex++;
                    }
                    else if (nbVisuals < 5) {

                        if (indexTable == 1) {
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }

                        mediumToEnd = 1;
                        if ((columnIndex % Nb_Columns) == 0) {

                            if (columnIndex > 0) {
                                output.Append("</tr>");
                                if (startToMedium == 1) {
                                    if (((columnIndex % 4) == 0) || ((columnIndex + 1) % 4 == 0)) {
                                        output.Append("<br>");
                                    }
                                    if (((columnIndex % 4) == 0) || ((columnIndex + 1) % 4 == 0))
                                        columnIndex = 0;
                                    else
                                        columnIndex = 1;
                                    startToMedium = 0;
                                }
                                else {
                                    if ((columnIndex % 2) == 0)
                                        output.Append("<br>");
                                }
                            }
                            output.Append("<tr>");

                        }

                        output.Append("<td>");

                        output.Append(cell.RenderPDF(true, 0));


                        output.Append("</td>");
                        columnIndex++;

                    }
                    else if (nbVisuals >= 5) {

                        if (indexTable == 1) {
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }

                        end = (int)Math.Ceiling((double)nbVisuals / 4);

                        for (int j = 0; j < end; j++) {

                            if ((columnIndex % Nb_Columns) == 0) {

                                if (columnIndex > 0) {
                                    output.Append("</tr>");
                                    if (mediumToEnd == 1) {
                                        if ((columnIndex % 2) == 0) {
                                            output.Append("<br>");
                                        }
                                        if (columnIndex % 2 == 0)
                                            columnIndex = 0;
                                        else
                                            columnIndex = 1;
                                        mediumToEnd = 0;
                                    }
                                    else {
                                        if ((columnIndex % 2) == 0)
                                            output.Append("<br>");
                                    }
                                }
                                output.Append("<tr>");

                            }
                            output.Append("<td>");

                            if (j == 0) {
                                output.Append(cell.RenderPDF(false, 0));
                            }
                            else {
                                if (j == end - 1) {
                                    output.Append(cell.RenderPDF(true, j + (4 * j)));
                                }
                                else {
                                    output.Append(cell.RenderPDF(false, j + (4 * j)));
                                }
                            }
                            output.Append("</td>");
                            columnIndex++;
                        }

                    }
                }
            }
                output.Append("</tr>");
                output.Append("</table>");
                output.Append("<br>");

        }
        #endregion

        #region BuildMSCreativesHtml
        /// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns>
        protected void BuildMSCreativesHtml(StringBuilder output, ResultTable resultTable, List<Color> colorList) {

            output.Append("<table align=\"left\" border=\"0\" class=\"txtViolet14BoldWhiteBG\">");
            output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
            if (_title == string.Empty) {
                switch (this._vehicle) {
                    case DBCst.Vehicles.names.tv:
                        _title = Convertion.ToHtmlString(GestionWeb.GetWebWord(2012, this._webSession.SiteLanguage));
                        break;
                    case DBCst.Vehicles.names.radio:
                        _title = Convertion.ToHtmlString(GestionWeb.GetWebWord(2011, this._webSession.SiteLanguage));
                        break;
                    default:
                        _title = "?";
                        break;
                }
            }
            output.Append(_title);
            output.Append("</td></tr>");

            output.Append("<tr><td class=\"whiteBackGround\" style=\"HEIGHT: 40px; BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\"></td></tr>");

            int columnIndex = 0;

            CellCreativesInformation cell;

            for (int i = 0; i < resultTable.LinesNumber; i++) {

                cell = (CellCreativesInformation)resultTable[i, 1];
                if ((columnIndex % 3) == 0) {
                    if (columnIndex > 0) {
                        output.Append("</tr>");
                        if ((columnIndex % 9) == 0) {
                            output.Append("<br>");
                        }
                    }
                    output.Append("<tr>");

                }
                output.Append("<td>");

                output.Append(cell.RenderPDF(true, columnIndex, colorList));

                output.Append("</td>");
                columnIndex++;
            }

            output.Append("</tr>");
            output.Append("</table>");
            output.Append("<br>");
        
        }
        #endregion

        #region PressSetUp()
        /// <summary>
		/// Initialise all webcontrols For Press
		/// </summary>
		protected void PressSetUp() {

			#region Get Data from persistent layer
			//TODO Get Data from database
			DataSet dtSet = null,dtSetDetails=null;
			switch(this._vehicle) {
				case DBCst.Vehicles.names.press: {
					dtSet = VersionDataAccess.GetVersions(_versions.Keys,_webSession,DBCst.Vehicles.names.press);
					dtSetDetails = VersionDataAccess.GetPressVersionsDetails(_versions.Keys, _webSession, DBCst.Vehicles.names.press);
				}
					break;
				case DBCst.Vehicles.names.internationalPress: {
					dtSet = VersionDataAccess.GetVersions(_versions.Keys,_webSession,DBCst.Vehicles.names.internationalPress);
					dtSetDetails = VersionDataAccess.GetPressVersionsDetails(_versions.Keys, _webSession, DBCst.Vehicles.names.internationalPress);
				}
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
			ExportVersionItem item = null;
			VersionPressUI versionUi = null;
			ArrayList[] versionsUi = new ArrayList[30];
			
			ArrayList indexList = new ArrayList();
			int i=0;

			if(dtSet != null && dtSet.Tables[0].Rows != null && dtSet.Tables[0].Rows.Count > 0) {
				this._versionsUIs = new ArrayList();
				foreach(DataRow row in dtSet.Tables[0].Rows) {
					if (row["visual"] != DBNull.Value) {
						
						//build different pathes
						pathes = row["visual"].ToString().Split(',');
						i=pathes.Length;
						path = string.Empty;
                        foreach (string str in pathes) {
                            path += Functions.Creatives.GetCreativePath(Int64.Parse(row["idMedia"].ToString()), Int64.Parse(row["dateKiosque"].ToString()), Int64.Parse(row["dateCover"].ToString()), str, true, true) + ",";
                        }

						//fill version path
						item = ((ExportVersionItem) this._versions[(Int64)row["id"]]);
						if (item == null) {
							continue;
						}
						if (path.Length>0) {
							item.Path = path.Substring(0, path.Length - 1);
						}

						//Nombre de visuels
						item.NbVisuel=i;

						item.Advertiser=row["annonceur"].ToString();
						item.Group=row["groupe"].ToString();
						if(_showProduct)
						item.Product=row["produit"].ToString();
					
						foreach(DataRow rowdetail in dtSetDetails.Tables[0].Rows) {
							if(item.Id.ToString()==rowdetail["id"].ToString()) {
								item.FirstInsertionDate=rowdetail["datenum"].ToString();
								item.NbMedia=Int64.Parse(rowdetail["nbsupports"].ToString());
                                item.NbInsertion = Int64.Parse(rowdetail["insertions"].ToString());
                                item.ExpenditureEuro = Double.Parse(rowdetail["budget"].ToString());
							}
						}
						
						//build control
						switch(this._vehicle) {
							case DBCst.Vehicles.names.press:
							case DBCst.Vehicles.names.internationalPress:
									versionUi = new VersionPressUI(this._webSession, item);
								break;
							default:
								throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
						}

						if(!indexList.Contains(i)) { 
							versionsUi[i]=new ArrayList();
							indexList.Add(i);
						}

						versionsUi[i].Add(versionUi);
						i=0;
					}
				}
				indexList.Sort();
				foreach(int j in indexList) {
					for(int k=0;k<versionsUi[j].Count;k++)
						this._versionsUIs.Add(versionsUi[j][k]);
				}
			}
			#endregion

		}
		#endregion
		
		#region APPMSetUp()
		/// <summary>
		/// Initialise all webcontrols
		/// </summary>
		protected void APPMSetUp(IDataSource dataSource) {

			#region Get Data from persistent layer
			//TODO Get Data from database
			DataSet dtSet = null;
			Hashtable[] synthesisData = new Hashtable[_versions.Count];
			bool mediaAgencyAccess=false;

			#region Paramétrage des dates
			//Formatting date to be used in the tabs which use APPM Press table
			int dateBegin = int.Parse(TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType).ToString("yyyyMMdd"));
			int dateEnd = int.Parse(TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType).ToString("yyyyMMdd"));
			#endregion

			#region targets
			//base target
            Int64 idBaseTarget=Int64.Parse(_webSession.GetSelection(_webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmBaseTargetAccess));
			//additional target
            Int64 idAdditionalTarget=Int64.Parse(_webSession.GetSelection(_webSession.SelectionUniversAEPMTarget,CustomerCst.Right.type.aepmTargetAccess));									
			#endregion

			#region Wave
            Int64 idWave=Int64.Parse(_webSession.GetSelection(_webSession.SelectionUniversAEPMWave,CustomerCst.Right.type.aepmWaveAccess));									
			#endregion

			#region Media Agency rights
			//To check if the user has a right to view the media agency or not
			//mediaAgencyAccess flag is used in the rest of the classes which indicates whethere the user has access 
			//to media agency or not
			if(_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_AGENCY))
				mediaAgencyAccess=true;

			#endregion

			switch(this._vehicle) {
				case DBCst.Vehicles.names.press: { 
						dtSet = VersionDataAccess.GetAPPMVersions(_versions.Keys, _webSession);
						synthesisData=TNS.AdExpress.Web.Rules.Results.APPM.SynthesisRules.GetData(_webSession,dataSource,int.Parse(_webSession.PeriodBeginningDate),int.Parse(_webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,mediaAgencyAccess,_versions.Keys);
					}
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
			ExportAPPMVersionItem item = null;
			VersionPressUI versionUi = null;
			ArrayList[] versionsUi = new ArrayList[30];
			
			ArrayList indexList = new ArrayList();
			int i=0;

			#region IdProduct
			//this is the id of the product selected from the products dropdownlist. 0 id refers to the whole univers i.e. if no prodcut is
			//selected its by default the whole univers and is represeted by product id 0.
			Int64 idProduct=0;
			if (_showProduct) {
				string idProductString = _webSession.GetSelection(_webSession.CurrentUniversProduct, CustomerRightConstante.type.productAccess);
				if (WebFunctions.CheckedText.IsStringEmpty(idProductString)) {
					idProduct = Int64.Parse(idProductString);
				}
			}
			#endregion

			if(dtSet != null && dtSet.Tables[0].Rows != null && dtSet.Tables[0].Rows.Count > 0) {
				
				this._versionsUIs = new ArrayList();
				foreach(DataRow row in dtSet.Tables[0].Rows) {
					if (row["visual"] != DBNull.Value) {
						
						//build different pathes
						pathes = row["visual"].ToString().Split(',');
						i=pathes.Length;
						path = string.Empty;
                        foreach (string str in pathes) {
                            path += Functions.Creatives.GetCreativePath(Int64.Parse(row["idMedia"].ToString()), Int64.Parse(row["dateKiosque"].ToString()), Int64.Parse(row["dateCover"].ToString()), str, true, true) + ",";
                        }

						//fill version path
						item = ((ExportAPPMVersionItem) this._versions[(Int64)row["id"]]);
						if (item == null) {
							continue;
						}
						if (path.Length>0) {
							item.Path = path.Substring(0, path.Length - 1);
						}

						//Nombre de visuels
						item.NbVisuel=i;
						
						//Date media Num
                        if (row["dateKiosque"] != System.DBNull.Value) item.Parution = row["dateKiosque"].ToString();
						else item.Parution=""; 
						

						//Synthèse
						foreach(Hashtable hTable in synthesisData){
						
							if(hTable["version"].ToString()==row["id"].ToString()){
							
								//Nom du Produit
								if (_showProduct) 
								item.Product = hTable["product"].ToString();
								//Nom de l'announceur
								item.Advertiser = hTable["advertiser"].ToString();
								//Marque
								if(_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
									item.Brand = hTable["brand"].ToString();
								if(mediaAgencyAccess && hTable["agency"].ToString().Length>0) {
									//Nom de l'agence Media
									item.Agency = hTable["agency"].ToString();	
								}
								
								//Périod d'analyse
								item.DateBegin = hTable["dateBegin"].ToString();
								item.DateEnd = hTable["dateEnd"].ToString();	
								//Budget brut (euros)
                                item.Budget = hTable["budget"].ToString();	
								//Nombre d'insertions
                                item.Insertions = hTable["insertions"].ToString();
								//Nombre des pages
                                item.Pages = hTable["pages"].ToString();
								//Nombre de supports utilisés
								item.Supports = hTable["supports"].ToString();	
								//Secteur de référence
								//if the competitor univers is not selected we print the groups of the products selected
								if(_webSession.CompetitorUniversAdvertiser.Count<2) {
									string[] groups=hTable["group"].ToString().Split(',');
									item.Group=hTable["group"].ToString();
//									Array.Sort(groups);
//									foreach(string gr in groups)
//									{
//										t.Append("&nbsp;&nbsp;&nbsp;&nbsp;"+gr+"<br>");	
//									}
//									t.Append("</td></tr>");
								}
								if(hTable["PDV"].ToString()!="") {
									//Part de voix de la campagne
									item.PDV = hTable["PDV"].ToString();	
								}
								//cible selectionnée
								item.TargetSelected = hTable["targetSelected"].ToString();	
								// nombre de GRP
								item.GRPNumber = hTable["GRPNumber"].ToString();	
								// nombre de GRP 15 et +
								item.BaseTarget = hTable["baseTarget"].ToString();
								item.GRPNumberBase = hTable["GRPNumberBase"].ToString();
								//Indice GRP vs cible 15 ans à +																				   
								item.IndiceGRP = hTable["IndiceGRP"].ToString();	
								// Coût GRP(cible selectionnée)					
 								item.GRPCost = hTable["GRPCost"].ToString();	
								// Coût GRP(cible 15 ans et +)					
								item.GRPCostBase = hTable["GRPCostBase"].ToString();	
								//Indice coût GRP vs cible 15 ans à +
								item.IndiceGRPCost = hTable["IndiceGRPCost"].ToString();		
								//Poids de la version vs produit correspondant
								if(hTable["versionWeight"].ToString().Length>0) {
								item.VersionWeight = hTable["versionWeight"].ToString();
								}
							}
						}

						//build control
						switch(this._vehicle) {
							case DBCst.Vehicles.names.press: {
									versionUi = new VersionPressAPPMUI(this._webSession, item);
								}
								break;
							default:
								throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
						}

						if(!indexList.Contains(i)) { 
							versionsUi[i]=new ArrayList();
							indexList.Add(i);
						}

						versionsUi[i].Add(versionUi);
						i=0;
					}
				}
				indexList.Sort();
				foreach(int j in indexList) {
					for(int k=0;k<versionsUi[j].Count;k++)
						this._versionsUIs.Add(versionsUi[j][k]);
				}
			}
			#endregion

		}
		#endregion

		#region TvSetUp()
		/// <summary>
		/// Initialise all webcontrols For TV
		/// </summary>
		protected void TvSetUp() {

			#region Get Data from persistent layer
			//TODO Get Data from database
			DataSet dtSet = null,dtSetDetails=null;
			switch(this._vehicle) {
				case DBCst.Vehicles.names.tv: {
					//					dtSet = VersionDataAccess.GetPressVersions(_versions.Keys, _webSession);
					dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession,DBCst.Vehicles.names.tv);	
					dtSetDetails = VersionDataAccess.GetPressVersionsDetails(_versions.Keys, _webSession, DBCst.Vehicles.names.tv);
				}
					break;
				default:
					throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
			}
			#endregion
			
			#region Build Set of VersionControl
			//Create each webcontrol
			string path = string.Empty;
            //string[] pathes = null;
			string dirPath = string.Empty;
			ExportVersionItem item = null;
			VersionTvUI versionUi = null;
			ArrayList[] versionsUi = new ArrayList[30];
			
			ArrayList indexList = new ArrayList();
            //int i=0;

			if(dtSet != null && dtSet.Tables[0].Rows != null && dtSet.Tables[0].Rows.Count > 0) {
				this._versionsUIs = new ArrayList();
				foreach(DataRow row in dtSet.Tables[0].Rows) {
					if (row["visual"] != DBNull.Value) {
						
						//build different pathes
						path = row["visual"].ToString();
				
						//fill version path
						item = ((ExportVersionItem) this._versions[(Int64)row["id"]]);
						if (item == null) {
							continue;
						}
	
						item.Advertiser=row["annonceur"].ToString();
						item.Group=row["groupe"].ToString();
						if(_showProduct)
						item.Product=row["produit"].ToString();

						foreach(DataRow rowdetail in dtSetDetails.Tables[0].Rows) {
							if(item.Id.ToString()==rowdetail["id"].ToString()) {
								item.FirstInsertionDate=rowdetail["datenum"].ToString();
								item.NbMedia=Int64.Parse(rowdetail["nbsupports"].ToString());
                                item.NbInsertion = Int64.Parse(rowdetail["insertions"].ToString());
                                item.ExpenditureEuro = Double.Parse(rowdetail["budget"].ToString());
							}
						}

						//build control
						if (path.Length>0) {
							item.Path = path;
						}
						versionUi = new VersionTvUI(this._webSession, item);
						this._versionsUIs.Add(versionUi);
					}
				}
			}
			#endregion

		}
		#endregion

		#region RadioSetUp()
		/// <summary>
		/// Initialise all webcontrols For RADIO
		/// </summary>
		protected void RadioSetUp() {

			#region Get Data from persistent layer
			//TODO Get Data from database
			DataSet dtSet = null,dtSetDetails=null;
			switch(this._vehicle) {
				case DBCst.Vehicles.names.radio: {
					dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession,DBCst.Vehicles.names.radio);	
					dtSetDetails = VersionDataAccess.GetPressVersionsDetails(_versions.Keys, _webSession, DBCst.Vehicles.names.radio);
				}
					break;
				default:
					throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
			}
			#endregion
			
			#region Build Set of VersionControl
			//Create each webcontrol
			string path = string.Empty;
            //string[] pathes = null;
			string dirPath = string.Empty;
			ExportVersionItem item = null;
			VersionRadioUI versionUi = null;
			ArrayList[] versionsUi = new ArrayList[30];
			
			ArrayList indexList = new ArrayList();
            //int i=0;

			if(dtSet != null && dtSet.Tables[0].Rows != null && dtSet.Tables[0].Rows.Count > 0) {
				this._versionsUIs = new ArrayList();
				foreach(DataRow row in dtSet.Tables[0].Rows) {
					if (row["visual"] != DBNull.Value) {
						
						//build different pathes
						path = row["visual"].ToString();
				
						//fill version path
						item = ((ExportVersionItem) this._versions[(Int64)row["id"]]);
						if (item == null) {
							continue;
						}
	
						item.Advertiser=row["annonceur"].ToString();
						item.Group=row["groupe"].ToString();
						if (_showProduct)
						item.Product=row["produit"].ToString();

						foreach(DataRow rowdetail in dtSetDetails.Tables[0].Rows) {
							if(item.Id.ToString()==rowdetail["id"].ToString()) {
								item.FirstInsertionDate=rowdetail["datenum"].ToString();
								item.NbMedia=Int64.Parse(rowdetail["nbsupports"].ToString());
                                item.NbInsertion = Int64.Parse(rowdetail["insertions"].ToString());
                                item.ExpenditureEuro = Double.Parse(rowdetail["budget"].ToString());
							}
						}

						//build control
						if (path.Length>0) {
							item.Path = path;
						}
						versionUi = new VersionRadioUI(this._webSession, item);
						this._versionsUIs.Add(versionUi);
					}
				}
			}
			#endregion

		}
		#endregion

        #region MDSetUp()
        /// <summary>
        /// Initialise all webcontrols For Direct Marketing
        /// </summary>
        protected void MDSetUp() {

            #region Get Data from persistent layer
            //TODO Get Data from database
            DataSet dtSetDetails = null;

            #region Dates Parameters
			//Formatting date to be used in the query
			string dateBegin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType).ToString("yyyyMMdd");
			string dateEnd = WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType).ToString("yyyyMMdd");
			#endregion

            ListDictionary  mediaImpactedList = Functions.MediaDetailLevel.GetImpactedMedia(_webSession, -1, -1, -1, -1);	

            switch (this._vehicle) {

                case DBCst.Vehicles.names.directMarketing:
                    dtSetDetails = MediaCreationDataAccess.GetMDData(_webSession, mediaImpactedList, int.Parse(dateBegin), int.Parse(dateEnd), VehiclesInformation.EnumToDatabaseId(DBCst.Vehicles.names.directMarketing), true); 
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
            ExportMDVersionItem item = null;
            VersionPressUI versionUi = null;
            ArrayList[] versionsUi = new ArrayList[30];

            ArrayList indexList = new ArrayList();
            int i = 0;

            if (dtSetDetails != null && dtSetDetails.Tables[0].Rows != null && dtSetDetails.Tables[0].Rows.Count > 0) {
                this._versionsUIs = new ArrayList();
                foreach (DataRow row in dtSetDetails.Tables[0].Rows) {
                    
                    if (row["associated_file"] != DBNull.Value) {

                        //build different pathes
                        
                        pathes = row["associated_file"].ToString().Split(',');
                        string pathWeb = string.Empty;
                        string idSlogan = row["associated_file"].ToString();
                        pathWeb = "";
                        string dir1 = idSlogan.Substring(idSlogan.Length - 8, 1);
                        pathWeb = string.Format(@"{0}/{1}", pathWeb, dir1);
                        string dir2 = idSlogan.Substring(idSlogan.Length - 9, 1);
                        pathWeb = string.Format(@"{0}/{1}", pathWeb, dir2);
                        string dir3 = idSlogan.Substring(idSlogan.Length - 10, 1);
                        pathWeb = string.Format(@"{0}/{1}/imagette/", pathWeb, dir3);

                        i = pathes.Length;
                        path = string.Empty;
                        foreach (string str in pathes) {
                            path += pathWeb + str + ",";
                        }

                        //fill version path
                        item = ((ExportMDVersionItem)this._versions[(Int64)row["id_slogan"]]);
                        if (item == null) {
                            continue;
                        }
                        if (path.Length > 0) {
                            item.Path = path.Substring(0, path.Length - 1);
                        }

                        //Nombre de visuels
                        item.NbVisuel = i;

                        item.Advertiser = row["advertiser"].ToString();
                        item.Group = row["group_"].ToString();
						if (_showProduct)
                        item.Product = row["product"].ToString();
                        item.Weight = long.Parse(row["weight"].ToString());
                        item.ExpenditureEuro = long.Parse(row["budget"].ToString());
                        item.Volume = double.Parse(row["volume"].ToString());
                        item.IdMedia = Int64.Parse(row["id_media"].ToString());

                        switch (row["id_media"].ToString()) {

                            case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                                item.Format = row["format"].ToString();
                                item.MailFormat = row["mail_format"].ToString();
                                item.MailType = row["mail_type"].ToString();
                                break;
                            case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                                item.WpMailFormat = row["wp_mail_format"].ToString();
                                item.ObjectCount = row["object_count"].ToString();
                                item.MailContent = GetItemContentList(row);
                                break;
                            case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                                item.ObjectCount = row["object_count"].ToString();
                                item.MailContent = GetItemContentList(row);
                                break;
                            case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                                item.ObjectCount = row["object_count"].ToString();
                                item.MailContent = GetItemContentList(row);
                                item.MailingRapidity = row["mailing_rapidity"].ToString();
                                break;
                        }


                        //build control
                        switch (this._vehicle) {
                            case DBCst.Vehicles.names.directMarketing:
                                versionUi = new VersionPressUI(this._webSession, item);
                                break;
                            default:
                                throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
                        }

                        if (!indexList.Contains(i)) {
                            versionsUi[i] = new ArrayList();
                            indexList.Add(i);
                        }

                        versionsUi[i].Add(versionUi);
                        i = 0;
                    }
                }
                indexList.Sort();
                foreach (int j in indexList){
                    for (int k = 0; k < versionsUi[j].Count; k++)
                        this._versionsUIs.Add(versionsUi[j][k]);
                }
            }
            #endregion

        }
        #endregion

        #region OutdoorSetUp()
        /// <summary>
        /// Initialise all webcontrols For Outdoor
        /// </summary>
        protected void OutdoorSetUp() {

            #region Get Data from persistent layer
            //TODO Get Data from database
            DataSet dtSet = null, dtSetDetails = null;
            dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.outdoor);
            dtSetDetails = VersionDataAccess.GetPressVersionsDetails(_versions.Keys, _webSession, DBCst.Vehicles.names.outdoor);
            #endregion

            #region Build Set of VersionControl
            //Create each webcontrol
            string path = string.Empty;
            string[] pathes = null;
            string dirPath = string.Empty;
            ExportOutdoorVersionItem item = null;
            VersionPressUI versionUi = null;
            ArrayList[] versionsUi = new ArrayList[30];

            ArrayList indexList = new ArrayList();
            int i = 0;

            if (dtSet != null && dtSet.Tables[0].Rows != null && dtSet.Tables[0].Rows.Count > 0) {
                this._versionsUIs = new ArrayList();
                foreach (DataRow row in dtSet.Tables[0].Rows) {
                    if (row["visual"] != DBNull.Value) {

                        //build different pathes
                        pathes = row["visual"].ToString().Split(',');
                        i = pathes.Length;
                        path = string.Empty;
                        dirPath = this.BuildVersionPath(row["id"].ToString(), "");
                        foreach (string str in pathes) {
                            path += dirPath + "/" + str + ",";
                        }

                        //fill version path
                        item = ((ExportOutdoorVersionItem)this._versions[(Int64)row["id"]]);
                        if (item == null) {
                            continue;
                        }
                        if (path.Length > 0) {
                            item.Path = path.Substring(0, path.Length - 1);
                        }

                        //Nombre de visuels
                        item.NbVisuel = i;

                        item.Advertiser = row["annonceur"].ToString();
                        item.Group = row["groupe"].ToString();
						if (_showProduct)
                        item.Product = row["produit"].ToString();

                        foreach (DataRow rowdetail in dtSetDetails.Tables[0].Rows) {
                            if (item.Id.ToString() == rowdetail["id"].ToString()) {
                                item.NbMedia = Int64.Parse(rowdetail["nbsupports"].ToString());
                                item.NbBoards = Int64.Parse(rowdetail["nbpanneau"].ToString());
                                item.ExpenditureEuro = Double.Parse(rowdetail["budget"].ToString());
                            }
                        }

                        //build control
                        switch (this._vehicle) {
                            case DBCst.Vehicles.names.outdoor:
                                versionUi = new VersionPressUI(this._webSession, item);
                                break;
                            default:
                                throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
                        }

                        if (!indexList.Contains(i)) {
                            versionsUi[i] = new ArrayList();
                            indexList.Add(i);
                        }

                        versionsUi[i].Add(versionUi);
                        i = 0;
                    }
                }
                indexList.Sort();
                foreach (int j in indexList) {
                    for (int k = 0; k < versionsUi[j].Count; k++)
                        this._versionsUIs.Add(versionsUi[j][k]);
                }
            }
            #endregion

        }
        #endregion

        #region InStoreSetUp()
        /// <summary>
        /// Initialise all webcontrols For InStore
        /// </summary>
        protected void InStoreSetUp() {

            #region Get Data from persistent layer
            //TODO Get Data from database
            DataSet dtSet = null, dtSetDetails = null;
            dtSet = VersionDataAccess.GetVersions(_versions.Keys, _webSession, DBCst.Vehicles.names.instore);
            dtSetDetails = VersionDataAccess.GetPressVersionsDetails(_versions.Keys, _webSession, DBCst.Vehicles.names.instore);
            #endregion

            #region Build Set of VersionControl
            //Create each webcontrol
            string path = string.Empty;
            string[] pathes = null;
            string dirPath = string.Empty;
            ExportInstoreVersionItem item = null;
            VersionPressUI versionUi = null;
            ArrayList[] versionsUi = new ArrayList[30];

            ArrayList indexList = new ArrayList();
            int i = 0;

            if (dtSet != null && dtSet.Tables[0].Rows != null && dtSet.Tables[0].Rows.Count > 0) {
                this._versionsUIs = new ArrayList();
                foreach (DataRow row in dtSet.Tables[0].Rows) {
                    if (row["visual"] != DBNull.Value) {

                        //build different pathes
                        pathes = row["visual"].ToString().Split(',');
                        i = pathes.Length;
                        path = string.Empty;
                        dirPath = this.BuildVersionPath(row["id"].ToString(), "");
                        foreach (string str in pathes) {
                            path += dirPath + "/" + str + ",";
                        }

                        //fill version path
                        item = ((ExportInstoreVersionItem)this._versions[(Int64)row["id"]]);
                        if (item == null) {
                            continue;
                        }
                        if (path.Length > 0) {
                            item.Path = path.Substring(0, path.Length - 1);
                        }

                        //Nombre de visuels
                        item.NbVisuel = i;

                        item.Advertiser = row["annonceur"].ToString();
                        item.Group = row["groupe"].ToString();
                        if (_showProduct)
                            item.Product = row["produit"].ToString();

                        foreach (DataRow rowdetail in dtSetDetails.Tables[0].Rows) {
                            if (item.Id.ToString() == rowdetail["id"].ToString()) {
                                item.NbMedia = Int64.Parse(rowdetail["nbsupports"].ToString());
                                item.NbBoards = Int64.Parse(rowdetail["nbpanneau"].ToString());
                                item.ExpenditureEuro = Double.Parse(rowdetail["budget"].ToString());
                            }
                        }

                        //build control
                        switch (this._vehicle) {
                            case DBCst.Vehicles.names.instore:
                                versionUi = new VersionPressUI(this._webSession, item);
                                break;
                            default:
                                throw new VersionUIException("Non authorized vehicle level : " + this._vehicle.ToString());
                        }

                        if (!indexList.Contains(i)) {
                            versionsUi[i] = new ArrayList();
                            indexList.Add(i);
                        }

                        versionsUi[i].Add(versionUi);
                        i = 0;
                    }
                }
                indexList.Sort();
                foreach (int j in indexList) {
                    for (int k = 0; k < versionsUi[j].Count; k++)
                        this._versionsUIs.Add(versionsUi[j][k]);
                }
            }
            #endregion

        }
        #endregion
			
		#region PressBuildHtml(StringBuilder output) 
		/// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns>
		protected void PressBuildHtml(StringBuilder output) 
		{
			int startToMedium=0,mediumToEnd=0,end=0,indexTable=0;

			if (this._versionsUIs != null) {
				output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
				output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
				if (_title == string.Empty)	{
					switch(this._vehicle) {
						case DBCst.Vehicles.names.press:
							_title = Convertion.ToHtmlString(GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage));
							break;
						case DBCst.Vehicles.names.internationalPress:
							_title = Convertion.ToHtmlString(GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage));
							break;
						default:
							_title = "?";
							break;
					}
				}
				output.Append(_title);
				output.Append("</td></tr>");


				int columnIndex = 0;
				foreach(VersionDetailUI item in  this._versionsUIs) {

					if (item.ExportVersion.NbVisuel==1)	{
						startToMedium=1;
						if ((columnIndex % 2) == 0) {

							if (columnIndex > 0) {
								output.Append("</tr>");
								if ((columnIndex % 4) == 0){
									output.Append("<br>");
								}
							}
							output.Append("<tr>");
					
						}
						output.Append("<td>");
						if(indexTable==0) {
							output.Append("<table>");
							output.Append("<tr>");
							output.Append("<td>");
						}

						item.GetHtmlPressExport(output,0,true);

						indexTable++;

						if(indexTable==2) {
							output.Append("</td>");
							output.Append("</tr>");
							output.Append("</table>");
							indexTable=0;
						}
						output.Append("</td>");
						columnIndex++;
					}
					else if (item.ExportVersion.NbVisuel<5){
						
						if(indexTable==1) {
							output.Append("</td>");
							output.Append("</tr>");
							output.Append("</table>");
							indexTable=0;
						}

						mediumToEnd=1;					
						if ((columnIndex % Nb_Columns) == 0) {

							if (columnIndex > 0) {
								output.Append("</tr>");
								if(startToMedium==1) {
									if (((columnIndex % 4) == 0)||((columnIndex+1) % 4 == 0)){
										output.Append("<br>");	
									}
									if(((columnIndex % 4) == 0)||((columnIndex+1) % 4 == 0))
										columnIndex=0;
									else
										columnIndex=1;
									startToMedium=0;
								}
								else {
									if ((columnIndex % 2) == 0)
										output.Append("<br>");	
								}
							}
							output.Append("<tr>");
					
						}
						
						output.Append("<td>");
					
						item.GetHtmlPressExport(output,0,true);
						
	
						output.Append("</td>");
						columnIndex++;
					
					}
					else if (item.ExportVersion.NbVisuel>=5) {
						
						if(indexTable==1) {
							output.Append("</td>");
							output.Append("</tr>");
							output.Append("</table>");
							indexTable=0;
						}

						end=(int)Math.Ceiling((double)item.ExportVersion.NbVisuel/4);
						
						for(int i=0;i<end;i++) {

							if ((columnIndex % Nb_Columns) == 0) {

								if (columnIndex > 0) {
									output.Append("</tr>");
									if(mediumToEnd==1) {
										if ((columnIndex % 2) == 0) {
											output.Append("<br>");	
										}
										if(columnIndex%2==0)
											columnIndex=0;
										else
											columnIndex=1;
										mediumToEnd=0;
									}
									else {
									if ((columnIndex % 2) == 0)
										output.Append("<br>");
									}
								}
								output.Append("<tr>");
					
							}
							output.Append("<td>");
					
							if(i==0){
									item.GetHtmlPressExport(output,0,false);
							}
							else{
								if(i==end-1) {
										item.GetHtmlPressExport(output,i+(4*i),true);
								}
								else{
										item.GetHtmlPressExport(output,i+(4*i),false);
								}
							}
							output.Append("</td>");
							columnIndex++;
						}
					
					}
				}			
				output.Append("</tr>");
				output.Append("</table>");
				output.Append("<br>");
				

			}
		}
		#endregion

		#region APPMBuildHtml(String title) 
		/// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns>
		protected ArrayList APPMBuildHtml(String title) 
		{
			ArrayList partitHTMLVersion = new ArrayList();
			StringBuilder output = new StringBuilder();
			int mediumToEnd=0,end=0;

			if (this._versionsUIs != null) {
				output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
				output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
				if (_title == string.Empty)	{
					switch(this._vehicle) 
					{
						case DBCst.Vehicles.names.press:
							_title = Convertion.ToHtmlString(title);
							break;
						default:
							_title = "?";
							break;
					}
				}
				output.Append(_title);
				output.Append("</td></tr>");


				int columnIndex = 0;
				foreach(VersionDetailUI item in  this._versionsUIs) {

					if (item.ExportAPPMVersion.NbVisuel<4)	{
						mediumToEnd=1;					
						if ((columnIndex % Nb_Columns) == 0) {

							if (columnIndex > 0) {
								output.Append("</tr>");
								if ((columnIndex % 2) == 0){
									partitHTMLVersion.Add(output.ToString());
									output = new StringBuilder();
									output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
									//output.Append("<br>");	
								}
							}
							output.Append("<tr>");
						}
						
						output.Append("<td>");
					
						item.GetHtmlAPPMExport(output,0,true);
	
						output.Append("</td>");
						columnIndex++;
					
					}
					else if (item.ExportAPPMVersion.NbVisuel>=4) {
						
						end=(int)Math.Ceiling((double)item.ExportAPPMVersion.NbVisuel/4);
						if(item.ExportAPPMVersion.NbVisuel%4==0)
							end=(int)(item.ExportAPPMVersion.NbVisuel/4)+1;
						
						for(int i=0;i<end;i++) {

							if ((columnIndex % Nb_Columns) == 0) {

								if (columnIndex > 0) {
									output.Append("</tr>");
									if(mediumToEnd==1) 	{
										if ((columnIndex % 2) == 0) {
											partitHTMLVersion.Add(output.ToString());
											output = new StringBuilder();
											output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
											//output.Append("<br>");	
										}
										if(columnIndex%2==0)
											columnIndex=0;
										else
											columnIndex=1;
										mediumToEnd=0;
									}
									else {
										if ((columnIndex % 2) == 0){ 
											partitHTMLVersion.Add(output.ToString());
											output = new StringBuilder();
											output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
										}
											//output.Append("<br>");
									}
								}
								output.Append("<tr>");
					
							}
							output.Append("<td>");
					
							if(i==0) {
								item.GetHtmlAPPMExport(output,0,false);
							}
							else {
								if(i==end-1) {
									item.GetHtmlAPPMExport(output,i+(3*i),true);
								}
								else {
									item.GetHtmlAPPMExport(output,i+(3*i),false);
								}
							}
							output.Append("</td>");
							columnIndex++;
						}
					
					}
				}			
				output.Append("</tr>");
				output.Append("</table>");
				partitHTMLVersion.Add(output.ToString());
				output = new StringBuilder();
				output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
				//output.Append("<br>");
				
			}
			return partitHTMLVersion;
		}
		#endregion

		#region TvBuildHtml(StringBuilder output) 
		/// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns> 
		protected void TvBuildHtml(StringBuilder output)  {
			
			if (this._versionsUIs != null) {
				output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
				output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
				if (_title == string.Empty) {
					switch(this._vehicle) {
						case DBCst.Vehicles.names.tv:
							_title = Convertion.ToHtmlString(GestionWeb.GetWebWord(2012, this._webSession.SiteLanguage));
							break;
						default:
							_title = "?";
							break;
					}
				}
				output.Append(_title);
				output.Append("</td></tr>");

				output.Append("<tr><td bgcolor=\"#ffffff\" style=\"HEIGHT: 40px; BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\"></td></tr>");

				int columnIndex = 0;
				foreach(VersionDetailUI item in  this._versionsUIs) {
					if ((columnIndex % 3) == 0) {
						if (columnIndex > 0) {
							output.Append("</tr>");
							if ((columnIndex % 9) == 0) {
									output.Append("<br>");
							}
						}
						output.Append("<tr>");
					
					}
					output.Append("<td>");
					
					item.GetHtmlTvExport(output,columnIndex,true);
	
					output.Append("</td>");
					columnIndex++;
				}			
				
				output.Append("</tr>");
				output.Append("</table>");
				output.Append("<br>");
			}
		}
		#endregion

		#region RadioBuildHtml(StringBuilder output) 
		/// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns> 
		protected void RadioBuildHtml(StringBuilder output) {
			
			if (this._versionsUIs != null) {
				output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
				output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
				if (_title == string.Empty) {
					switch(this._vehicle) {
						case DBCst.Vehicles.names.radio:
							_title = Convertion.ToHtmlString(GestionWeb.GetWebWord(2011, this._webSession.SiteLanguage));
							break;
						default:
							_title = "?";
							break;
					}
				}
				output.Append(_title);
				output.Append("</td></tr>");

								output.Append("<tr><td bgcolor=\"#ffffff\" style=\"HEIGHT: 40px; BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\"></td></tr>");

				int columnIndex = 0;
				foreach(VersionDetailUI item in  this._versionsUIs) {
					
					if ((columnIndex % 3) == 0) {
						if (columnIndex > 0) {
							output.Append("</tr>");
							if ((columnIndex % 9) == 0) {
								output.Append("<br>");
							}
						}
						output.Append("<tr>");
					
					}
					output.Append("<td>");
					
					item.GetHtmlRadioExport(output,columnIndex,true);
	
					output.Append("</td>");
					columnIndex++;
				}			
				
				output.Append("</tr>");
				output.Append("</table>");
				output.Append("<br>");
			}
		}
		#endregion

        #region MDBuildHtml(StringBuilder output)
        /// <summary> 
        /// Render all versions controls
        /// </summary>
        /// <returns>Html code</returns>
        protected void MDBuildHtml(StringBuilder output) {

            int startToMedium = 0, mediumToEnd = 0, end = 0, indexTable = 0;

            if (this._versionsUIs != null) {
                output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
                output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
                if (_title == string.Empty){

                    switch (this._vehicle) {
                        case DBCst.Vehicles.names.directMarketing:
                            _title = Convertion.ToHtmlString(GestionWeb.GetWebWord(2219, this._webSession.SiteLanguage));
                            break;
                        default:
                            _title = "?";
                            break;
                    }
                }
                output.Append(_title);
                output.Append("</td></tr>");


                int columnIndex = 0;
                foreach (VersionDetailUI item in this._versionsUIs){

                    if (item.ExportMDVersion.NbVisuel == 1){
                        startToMedium = 1;
                        if ((columnIndex % 2) == 0){

                            if (columnIndex > 0){
                                output.Append("</tr>");
                                if ((columnIndex % 4) == 0){
                                    output.Append("<br>");
                                }
                            }
                            output.Append("<tr>");

                        }
                        output.Append("<td>");
                        if (indexTable == 0){
                            output.Append("<table>");
                            output.Append("<tr>");
                            output.Append("<td>");
                        }

                        item.GetHtmlMDExport(output, 0, true);

                        indexTable++;

                        if (indexTable == 2){
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }
                        output.Append("</td>");
                        columnIndex++;
                    }
                    else if (item.ExportMDVersion.NbVisuel < 5){

                        if (indexTable == 1){
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }

                        mediumToEnd = 1;
                        if ((columnIndex % Nb_Columns) == 0){

                            if (columnIndex > 0){
                                output.Append("</tr>");
                                if (startToMedium == 1){
                                    if (((columnIndex % 4) == 0) || ((columnIndex + 1) % 4 == 0)){
                                        output.Append("<br>");
                                    }
                                    if (((columnIndex % 4) == 0) || ((columnIndex + 1) % 4 == 0))
                                        columnIndex = 0;
                                    else
                                        columnIndex = 1;
                                    startToMedium = 0;
                                }
                                else{
                                    if ((columnIndex % 2) == 0)
                                        output.Append("<br>");
                                }
                            }
                            output.Append("<tr>");

                        }

                        output.Append("<td>");

                        item.GetHtmlMDExport(output, 0, true);


                        output.Append("</td>");
                        columnIndex++;

                    }
                    else if (item.ExportMDVersion.NbVisuel >= 5){

                        if (indexTable == 1){
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }

                        end = (int)Math.Ceiling((double)item.ExportMDVersion.NbVisuel / 4);

                        for (int i = 0; i < end; i++){

                            if ((columnIndex % Nb_Columns) == 0){

                                if (columnIndex > 0){
                                    output.Append("</tr>");
                                    if (mediumToEnd == 1){
                                        if ((columnIndex % 2) == 0){
                                            output.Append("<br>");
                                        }
                                        if (columnIndex % 2 == 0)
                                            columnIndex = 0;
                                        else
                                            columnIndex = 1;
                                        mediumToEnd = 0;
                                    }
                                    else{
                                        if ((columnIndex % 2) == 0)
                                            output.Append("<br>");
                                    }
                                }
                                output.Append("<tr>");

                            }
                            output.Append("<td>");

                            if (i == 0){
                                item.GetHtmlMDExport(output, 0, false);
                            }
                            else{
                                if (i == end - 1){
                                    item.GetHtmlMDExport(output, i + (4 * i), true);
                                }
                                else{
                                    item.GetHtmlMDExport(output, i + (4 * i), false);
                                }
                            }
                            output.Append("</td>");
                            columnIndex++;
                        }

                    }
                }
                output.Append("</tr>");
                output.Append("</table>");
                output.Append("<br>");

            }
        }
        #endregion

        #region OutdoorBuildHtml(StringBuilder output)
        /// <summary> 
        /// Render all versions controls
        /// </summary>
        /// <returns>Html code</returns>
        protected void OutdoorBuildHtml(StringBuilder output) {
            int startToMedium = 0, mediumToEnd = 0, end = 0, indexTable = 0;

            if (this._versionsUIs != null) {
                output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
                output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
                if (_title == string.Empty) {
                    switch (this._vehicle) {
                        case DBCst.Vehicles.names.outdoor:
                            _title = Convertion.ToHtmlString(GestionWeb.GetWebWord(2255, this._webSession.SiteLanguage));
                            break;
                        default:
                            _title = "?";
                            break;
                    }
                }
                output.Append(_title);
                output.Append("</td></tr>");


                int columnIndex = 0;
                foreach (VersionDetailUI item in this._versionsUIs) {

                    if (item.ExportOutdoorVersion.NbVisuel == 1) {
                        startToMedium = 1;
                        if ((columnIndex % 2) == 0) {

                            if (columnIndex > 0) {
                                output.Append("</tr>");
                                if ((columnIndex % 4) == 0) {
                                    output.Append("<br>");
                                }
                            }
                            output.Append("<tr>");

                        }
                        output.Append("<td>");
                        if (indexTable == 0) {
                            output.Append("<table>");
                            output.Append("<tr>");
                            output.Append("<td>");
                        }

                        item.GetHtmlOutdoorExport(output, 0, true);

                        indexTable++;

                        if (indexTable == 2) {
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }
                        output.Append("</td>");
                        columnIndex++;
                    }
                    else if (item.ExportOutdoorVersion.NbVisuel < 5) {

                        if (indexTable == 1) {
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }

                        mediumToEnd = 1;
                        if ((columnIndex % Nb_Columns) == 0) {

                            if (columnIndex > 0) {
                                output.Append("</tr>");
                                if (startToMedium == 1) {
                                    if (((columnIndex % 4) == 0) || ((columnIndex + 1) % 4 == 0)) {
                                        output.Append("<br>");
                                    }
                                    if (((columnIndex % 4) == 0) || ((columnIndex + 1) % 4 == 0))
                                        columnIndex = 0;
                                    else
                                        columnIndex = 1;
                                    startToMedium = 0;
                                }
                                else {
                                    if ((columnIndex % 2) == 0)
                                        output.Append("<br>");
                                }
                            }
                            output.Append("<tr>");

                        }

                        output.Append("<td>");

                        item.GetHtmlOutdoorExport(output, 0, true);


                        output.Append("</td>");
                        columnIndex++;

                    }
                    else if (item.ExportOutdoorVersion.NbVisuel >= 5) {

                        if (indexTable == 1) {
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }

                        end = (int)Math.Ceiling((double)item.ExportOutdoorVersion.NbVisuel / 4);

                        for (int i = 0; i < end; i++) {

                            if ((columnIndex % Nb_Columns) == 0) {

                                if (columnIndex > 0) {
                                    output.Append("</tr>");
                                    if (mediumToEnd == 1) {
                                        if ((columnIndex % 2) == 0) {
                                            output.Append("<br>");
                                        }
                                        if (columnIndex % 2 == 0)
                                            columnIndex = 0;
                                        else
                                            columnIndex = 1;
                                        mediumToEnd = 0;
                                    }
                                    else {
                                        if ((columnIndex % 2) == 0)
                                            output.Append("<br>");
                                    }
                                }
                                output.Append("<tr>");

                            }
                            output.Append("<td>");

                            if (i == 0) {
                                item.GetHtmlOutdoorExport(output, 0, false);
                            }
                            else {
                                if (i == end - 1) {
                                    item.GetHtmlOutdoorExport(output, i + (4 * i), true);
                                }
                                else {
                                    item.GetHtmlOutdoorExport(output, i + (4 * i), false);
                                }
                            }
                            output.Append("</td>");
                            columnIndex++;
                        }

                    }
                }
                output.Append("</tr>");
                output.Append("</table>");
                output.Append("<br>");


            }
        }
        #endregion

        #region InStoreBuildHtml(StringBuilder output)
        /// <summary> 
        /// Render all versions controls
        /// </summary>
        /// <returns>Html code</returns>
        protected void InStoreBuildHtml(StringBuilder output) {
            int startToMedium = 0, mediumToEnd = 0, end = 0, indexTable = 0;

            if (this._versionsUIs != null) {
                output.Append("<table bgcolor=\"#ffffff\" align=\"left\" border=\"0\" class=\"txtViolet14Bold\">");
                output.Append("<tr><td colSpan=\"" + _nb_column + "\">");
                if (_title == string.Empty) {
                    switch (this._vehicle) {
                        case DBCst.Vehicles.names.instore:
                            _title = Convertion.ToHtmlString(GestionWeb.GetWebWord(2667, this._webSession.SiteLanguage));
                            break;
                        default:
                            _title = "?";
                            break;
                    }
                }
                output.Append(_title);
                output.Append("</td></tr>");


                int columnIndex = 0;
                foreach (VersionDetailUI item in this._versionsUIs) {

                    if (item.ExportInStoreVersion.NbVisuel == 1) {
                        startToMedium = 1;
                        if ((columnIndex % 2) == 0) {

                            if (columnIndex > 0) {
                                output.Append("</tr>");
                                if ((columnIndex % 4) == 0) {
                                    output.Append("<br>");
                                }
                            }
                            output.Append("<tr>");

                        }
                        output.Append("<td>");
                        if (indexTable == 0) {
                            output.Append("<table>");
                            output.Append("<tr>");
                            output.Append("<td>");
                        }

                        item.GetHtmlInStoreExport(output, 0, true);

                        indexTable++;

                        if (indexTable == 2) {
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }
                        output.Append("</td>");
                        columnIndex++;
                    }
                    else if (item.ExportInStoreVersion.NbVisuel < 5) {

                        if (indexTable == 1) {
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }

                        mediumToEnd = 1;
                        if ((columnIndex % Nb_Columns) == 0) {

                            if (columnIndex > 0) {
                                output.Append("</tr>");
                                if (startToMedium == 1) {
                                    if (((columnIndex % 4) == 0) || ((columnIndex + 1) % 4 == 0)) {
                                        output.Append("<br>");
                                    }
                                    if (((columnIndex % 4) == 0) || ((columnIndex + 1) % 4 == 0))
                                        columnIndex = 0;
                                    else
                                        columnIndex = 1;
                                    startToMedium = 0;
                                }
                                else {
                                    if ((columnIndex % 2) == 0)
                                        output.Append("<br>");
                                }
                            }
                            output.Append("<tr>");

                        }

                        output.Append("<td>");

                        item.GetHtmlInStoreExport(output, 0, true);


                        output.Append("</td>");
                        columnIndex++;

                    }
                    else if (item.ExportInStoreVersion.NbVisuel >= 5) {

                        if (indexTable == 1) {
                            output.Append("</td>");
                            output.Append("</tr>");
                            output.Append("</table>");
                            indexTable = 0;
                        }

                        end = (int)Math.Ceiling((double)item.ExportInStoreVersion.NbVisuel / 4);

                        for (int i = 0; i < end; i++) {

                            if ((columnIndex % Nb_Columns) == 0) {

                                if (columnIndex > 0) {
                                    output.Append("</tr>");
                                    if (mediumToEnd == 1) {
                                        if ((columnIndex % 2) == 0) {
                                            output.Append("<br>");
                                        }
                                        if (columnIndex % 2 == 0)
                                            columnIndex = 0;
                                        else
                                            columnIndex = 1;
                                        mediumToEnd = 0;
                                    }
                                    else {
                                        if ((columnIndex % 2) == 0)
                                            output.Append("<br>");
                                    }
                                }
                                output.Append("<tr>");

                            }
                            output.Append("<td>");

                            if (i == 0) {
                                item.GetHtmlInStoreExport(output, 0, false);
                            }
                            else {
                                if (i == end - 1) {
                                    item.GetHtmlInStoreExport(output, i + (4 * i), true);
                                }
                                else {
                                    item.GetHtmlInStoreExport(output, i + (4 * i), false);
                                }
                            }
                            output.Append("</td>");
                            columnIndex++;
                        }

                    }
                }
                output.Append("</tr>");
                output.Append("</table>");
                output.Append("<br>");


            }
        }
        #endregion

		#endregion

		#region Internal Methods
		/// <summary>
		/// Build visual access path depending on the vehicle
		/// </summary>
		/// <param name="date">date to format YYYYMMDD</param>
		/// <param name="idMedia">Media Id</param>
		/// <returns>Full path to access an image</returns>
        private string BuildVersionPath(string idMedia, string date, string folderPath) {
			string path = string.Empty;
			switch(this._vehicle) {
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

        #region Renvoie la liste des item content
        /// <summary>
        /// Renvoie la liste des item content
        /// </summary>
        /// <returns></returns>
        private static string GetItemContentList(DataRow row){

            string mailContent = "";
            int k = 0;

            for (k = 1; k <= 10; k++)
            {
                if (row["item" + k].ToString() != "")
                    mailContent += row["item" + k].ToString() + ",";
            }

            if (mailContent != "")
                mailContent = mailContent.Substring(0, mailContent.Length - 1);

            return mailContent;
        }
        #endregion

        #region Init image path
        /// <summary>
        /// Init images path
        /// </summary>
        /// <returns>Full path to access an image</returns>
        private SortedDictionary<Int64, List<CellCreativesInformation>> InitCreativeCells(ResultTable resultTable) {

            SortedDictionary<Int64, List<CellCreativesInformation>> dictionary = new SortedDictionary<Int64, List<CellCreativesInformation>>();
            CellCreativesInformation cell;
            List<CellCreativesInformation> list;

            for (int i = 0; i < resultTable.LinesNumber; i++) {

                cell = (CellCreativesInformation)resultTable[i, 1];
                if (dictionary.ContainsKey(cell.NbVisuals))
                    dictionary[cell.NbVisuals].Add(cell);
                else {
                    list = new List<CellCreativesInformation>();
                    list.Add(cell);
                    dictionary.Add(cell.NbVisuals, list);
                }
            }

            return dictionary;
        }
        #endregion

        #endregion


    }
}
