#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Result;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Constantes.FrameWork.Results;
using ExcelFunction = TNS.AdExpress.Web.UI.ExcelWebPage;

using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Portofolio.Engines {
	/// <summary>
	/// Compute portofolio structure's results
	/// </summary>
	public class StructureEngine : Engine{

		#region Variables
		/// <summary>
		/// Beginning hour interval list
		/// </summary>
		protected Dictionary<string, double> _hourBeginningList = null;
		/// <summary>
		/// End hour interval list
		/// </summary>
		protected Dictionary<string, double> _hourEndList = null;
		/// <summary>
		/// Ventilation type list for press result
		/// </summary>
		protected List<PortofolioStructure.Ventilation> _ventilationTypeList = null;
		/// <summary>
		/// Determine if render will be into excel file
		/// </summary>
		protected bool _excel = false;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public StructureEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList, bool excel)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
			if (hourBeginningList == null || hourBeginningList.Count == 0) throw (new ArgumentException("hourBeginningList parameter is invalid"));
			if (hourEndList == null || hourEndList.Count == 0) throw (new ArgumentException("hourEndList parameter is invalid"));
			if (hourBeginningList.Count != hourEndList.Count) throw (new ArgumentException("hourEndList and hourBeginningList parameter don't have the same number of elements"));
			_hourBeginningList = hourBeginningList;
			_hourEndList = hourEndList;
			_excel = excel;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		/// <param name="ventilationTypeList">Ventilation type List</param>
		public StructureEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, List<PortofolioStructure.Ventilation> ventilationTypeList,bool excel)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
			if (ventilationTypeList == null || ventilationTypeList.Count == 0) throw (new ArgumentException("ventilationTypeList parameter is invalid"));
			_ventilationTypeList = ventilationTypeList;
			_excel = excel;
		}
		#endregion

		#region Abstract methods implementation
		/// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override string BuildHtmlResult() {
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.radio:
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return GetStructureHtml();
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return GetPressStructureHtml();					
				default: throw new PortofolioException("The method to get data is not defined for this vehicle.");
			}
		}

		/// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override ResultTable ComputeResultTable() {
			throw new PortofolioException("The method or operation is not implemented.");
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Compute structure chart data
		/// </summary>
		/// <returns></returns>
		public virtual DataTable GetChartData() {

			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.radio:
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return ComputeChartData();
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return ComputePressChartData();
				default: throw new PortofolioException("GetChartData() : The method to get data is not defined for this vehicle.");
			}
		}

		#endregion

		#region GetPressStructureHtml
		/// <summary>
		/// Get press structure html
		/// </summary>
		/// <returns></returns>
		protected virtual string GetPressStructureHtml() {
			StringBuilder t = new StringBuilder(5000);			
			string classCss = (_excel) ? "acl21" : "acl2";
			DataSet ds = null;
			DataTable dt = null;
			int oldIdVentilationType = -1;
			int labelCode;

			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[6];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			parameters[5] = _ventilationTypeList;
			
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
			ds = portofolioDAL.GetData();

			// Selection recall
			if (_excel) {
				t.Append(ExcelFunction.GetLogo(_webSession));
				t.Append(ExcelFunction.GetExcelHeader(_webSession, GestionWeb.GetWebWord(1379, _webSession.SiteLanguage)));
			}

			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
				
				dt = ds.Tables[0];
				t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 >");
				foreach (DataRow dr in dt.Rows) {
					if (oldIdVentilationType != int.Parse(dr["ventilationType"].ToString())) {
						switch ((FrameWorkResultConstantes.PortofolioStructure.Ventilation)long.Parse(dr["ventilationType"].ToString())) {
							case FrameWorkResultConstantes.PortofolioStructure.Ventilation.color:
								labelCode = 1438;
								break;
							case FrameWorkResultConstantes.PortofolioStructure.Ventilation.format:
								labelCode = 1420;
								break;
							case FrameWorkResultConstantes.PortofolioStructure.Ventilation.insert:
								labelCode = 1440;
								break;
							case FrameWorkResultConstantes.PortofolioStructure.Ventilation.location:
								labelCode = 1439;
								break;
							default:
								throw new PortofolioException("GetVentilationLines: Ventilation type unknown.");
						}
						//labels
						t.Append("\r\n\t<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV3';\"  class=\"violetBackGroundV3\" height=\"20px\" >");
						t.Append("\r\n\t<td align=\"left\" class=\"p2\" nowrap><b>" + GestionWeb.GetWebWord(labelCode, _webSession.SiteLanguage) + "</b></td>");
						t.Append("\r\n\t<td  class=\"p2\"  nowrap>" + GestionWeb.GetWebWord(1398, _webSession.SiteLanguage) + "</td>");
						t.Append("</tr>");
					}
					//Nb insertion			
					t.Append("\r\n\t<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV3';\"  class=\"violetBackGroundV3\" height=\"20px\" >");
					if (dr["ventilation"] != null)
						t.Append("\r\n\t<td align=\"left\" class=\"" + classCss + "\" nowrap>&nbsp;&nbsp;&nbsp;" + dr["ventilation"].ToString() + "</td>");
					else t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>&nbsp;</td>");
                    UnitInformation unitInformation = UnitsInformation.Get(WebCst.CustomerSessions.Unit.insertion);
                    if (dr[unitInformation.Id.ToString()] != null) {
                        t.Append("\r\n\t<td align=\"right\" class=\"" + classCss + "\" nowrap>" + WebFunctions.Units.ConvertUnitValueAndPdmToString(dr[unitInformation.Id.ToString()].ToString(), unitInformation.Id, false) + "</td>");
					}
					else t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>&nbsp;</td>");
					t.Append("</tr>");

					oldIdVentilationType = int.Parse(dr["ventilationType"].ToString());
				}
				t.Append("</table>");
			}
			else t.Append(GetNoDataMessageHtml());
			if (_excel) {
				t.Append(ExcelFunction.GetFooter(_webSession));
			}
			return t.ToString();
		}
		#endregion

		#region GetStructureHtml
		/// <summary>
		/// Get structure html
		/// <remarks>Used currently for vehicle , tv,others an radio</remarks>
		/// </summary>
		/// <returns></returns>
		protected virtual string GetStructureHtml() {
			StringBuilder t = new StringBuilder(5000);
			DataSet ds = null;
			DataTable dt = null;
            List<UnitInformation> unitInformationList = new List<UnitInformation>();
			string P2 = "p2";
			string backGround = "whiteBackGround";
			string classCss = "acl1";
			string hourIntervallLabel = "";
			double totalUnit = 0;

			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[7];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			parameters[5] = _hourBeginningList;
			parameters[6] = _hourEndList;

			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
			ds = portofolioDAL.GetData();

			// Selection recall
			if (_excel) {
				t.Append(ExcelFunction.GetLogo(_webSession));
				t.Append(ExcelFunction.GetExcelHeader(_webSession, GestionWeb.GetWebWord(1379, _webSession.SiteLanguage)));
			}
			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
				dt = ds.Tables[0];


                unitInformationList = _webSession.GetValidUnitForResult();   
                    
				t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 >");

				#region libellés colonnes
				// Première ligne
				t.Append("\r\n\t<tr height=\"20px\" >");
				if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio)
					t.Append("<td class=\"" + P2 + "\" nowrap>" + GestionWeb.GetWebWord(1299, _webSession.SiteLanguage) + "</td>");
				else if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)
					t.Append("<td class=\"" + P2 + "\" nowrap>" + GestionWeb.GetWebWord(1451, _webSession.SiteLanguage) + "</td>");

                for (int i = 0; i < unitInformationList.Count; i++) {
                    t.Append("<td class=\"" + P2 + "\" nowrap>" + GestionWeb.GetWebWord(unitInformationList[i].WebTextId, _webSession.SiteLanguage) + "</td>");
                }

				t.Append("</tr>");
				#endregion
                
				t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='" + backGround + "';\"  class=\"" + backGround + "\" height=\"20px\" >");
				//time interval										
				t.Append("\r\n\t<td align=\"left\" class=\"" + classCss + "\" nowrap>" + GestionWeb.GetWebWord(1401,_webSession.SiteLanguage) + "</td>");
                ////line total units
                for (int i = 0; i < unitInformationList.Count; i++) {
                    totalUnit = 0;
                    foreach (DataRow dr in dt.Rows) {
                        totalUnit += (dr[unitInformationList[i].Id.ToString()] != System.DBNull.Value) ? double.Parse(dr[unitInformationList[i].Id.ToString()].ToString()) : 0;
                    }
                    t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + WebFunctions.Units.ConvertUnitValueAndPdmToString(totalUnit.ToString(), unitInformationList[i].Id, false) + "</td>");
                }

				t.Append("</tr>");
				backGround = "violetBackGroundV3";

				//One line by time interval
				foreach (DataRow dr in dt.Rows) {
					classCss = "acl1";
					hourIntervallLabel = GestionWeb.GetWebWord(GetHourIntervalWebWordCode()[dr["HourInterval"].ToString()], _webSession.SiteLanguage);

					t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='" + backGround + "';\"  class=\"" + backGround + "\" height=\"20px\" >");
					//time interval										
					t.Append("\r\n\t<td align=\"left\" class=\"" + classCss + "\" nowrap>" + hourIntervallLabel + "</td>");
                    //Unit Value
                    for (int i = 0; i < unitInformationList.Count; i++) {
                        t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + WebFunctions.Units.ConvertUnitValueAndPdmToString(dr[unitInformationList[i].Id.ToString()].ToString(), unitInformationList[i].Id, false) + "</td>");
                    }
					t.Append("</tr>");

					classCss = "acl2";
					//backGround = "violetBackGroundV3";
				}
				t.Append("</table>");
			}
			if (_excel) {
				t.Append(ExcelFunction.GetFooter(_webSession));
			}
			return t.ToString();
		}

		/// <summary>
		///Get hour interval web word code 
		/// </summary>
		/// <returns></returns>
		protected virtual Dictionary<string, int> GetHourIntervalWebWordCode() {
			Dictionary<string, int> dic = new Dictionary<string, int>();
			dic.Add("0007",2462);
			dic.Add("0712", 2463);
			dic.Add("1214", 2464);
			dic.Add("1417", 2465);
			dic.Add("1719", 2466);
			dic.Add("1922", 2467);
			dic.Add("2224", 2468);
			dic.Add("0507", 2469);
			dic.Add("0709", 2470);
			dic.Add("0913", 2471);
			dic.Add("1319", 2472);
			dic.Add("1924", 2473);
			return dic;

		}
		#endregion

		#region Compute data for chart for press
		/// <summary>
		/// Compute data for preess chart results 
		/// </summary>
		/// <returns>Data set for preess chart</returns>
		protected virtual DataTable ComputePressChartData() {
			DataSet ds = null;
			DataTable dt = null, dtResult = null;
			DataRow newRow = null;
			int labelCode = 0;
			long oldIdVentilationType = -1;

			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[6];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			parameters[5] = _ventilationTypeList;

			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
			ds = portofolioDAL.GetData();

			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
				dt = ds.Tables[0];

				dtResult = new DataTable();
				dtResult.Columns.Add("idUnit", System.Type.GetType("System.Int64"));
				dtResult.Columns.Add("unitLabel", System.Type.GetType("System.String"));
				dtResult.Columns.Add("chartDataLabel", System.Type.GetType("System.String"));
				dtResult.Columns.Add("chartDataValue", System.Type.GetType("System.Double"));
                UnitInformation unitInformation = UnitsInformation.Get(WebCst.CustomerSessions.Unit.insertion);
				foreach (DataRow dr in dt.Rows) {
					if (oldIdVentilationType != int.Parse(dr["ventilationType"].ToString())) {
						switch ((FrameWorkResultConstantes.PortofolioStructure.Ventilation)long.Parse(dr["ventilationType"].ToString())) {
							case FrameWorkResultConstantes.PortofolioStructure.Ventilation.color:
								labelCode = 1438;
								break;
							case FrameWorkResultConstantes.PortofolioStructure.Ventilation.format:
								labelCode = 1420;
								break;
							case FrameWorkResultConstantes.PortofolioStructure.Ventilation.insert:
								labelCode = 1440;
								break;
							case FrameWorkResultConstantes.PortofolioStructure.Ventilation.location:
								labelCode = 1439;
								break;
							default:
								throw new PortofolioException("GetVentilationLines: Ventilation type unknown.");
						}						
					}
					newRow = dtResult.NewRow();
					dtResult.Rows.Add(newRow);

					newRow["idUnit"] = long.Parse(dr["ventilationType"].ToString());
					newRow["unitLabel"] = GestionWeb.GetWebWord(labelCode, _webSession.SiteLanguage);
					newRow["chartDataLabel"] = dr["ventilation"].ToString();
                    newRow["chartDataValue"] = (dr[unitInformation.Id.ToString()] != System.DBNull.Value) ? double.Parse(dr[unitInformation.Id.ToString()].ToString()) : 0;

					oldIdVentilationType = long.Parse(dr["ventilationType"].ToString());

				}
			}

				return dtResult;

		}
		#endregion

		#region Compute data for chart for tv,others,radio
		/// <summary>
		/// Compute data for preess chart results 
		/// </summary>
		/// <returns>Data set for preess chart</returns>
		protected virtual DataTable ComputeChartData() {
			DataSet ds = null;
			DataTable dt = null, dtResult = null;
			DataRow newRow = null;
			string hourIntervallLabel = "";
            List<UnitInformation> unitInformationList = new List<UnitInformation>();

			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[7];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			parameters[5] = _hourBeginningList;
			parameters[6] = _hourEndList;

			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
			ds = portofolioDAL.GetData();

			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
				dt = ds.Tables[0];

                unitInformationList = _webSession.GetValidUnitForResult();  

				dtResult = new DataTable();
				dtResult.Columns.Add("idUnit", System.Type.GetType("System.Int64"));
				dtResult.Columns.Add("unitLabel", System.Type.GetType("System.String"));
				dtResult.Columns.Add("chartDataLabel", System.Type.GetType("System.String"));
				dtResult.Columns.Add("chartDataValue", System.Type.GetType("System.Double"));


                for (int i = 0; i < unitInformationList.Count; i++) {
                    //One line by time interval and unit
                    foreach (DataRow dr in dt.Rows) {
                        newRow = dtResult.NewRow();
                        dtResult.Rows.Add(newRow);
                        hourIntervallLabel = GestionWeb.GetWebWord(GetHourIntervalWebWordCode()[dr["HourInterval"].ToString()], _webSession.SiteLanguage);

                        newRow["idUnit"] = unitInformationList[i].Id.GetHashCode();
                        newRow["unitLabel"] = GestionWeb.GetWebWord(unitInformationList[i].WebTextId, _webSession.SiteLanguage);
                        newRow["chartDataLabel"] = hourIntervallLabel;
                        newRow["chartDataValue"] = (dr[unitInformationList[i].Id.ToString()] != System.DBNull.Value) ? double.Parse(dr[unitInformationList[i].Id.ToString()].ToString()) : 0;
                    }
                }
			}

			return dtResult;
		}
		#endregion

	}
}
