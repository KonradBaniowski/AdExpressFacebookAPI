#region Information
// Author: D. Mussuma
// Creation date: 12/08/2008
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
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;

using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;

using TNS.AdExpress.Domain.Classification;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;


namespace TNS.AdExpressI.Portofolio.Engines {
	/// <summary>
	/// Compute media detail's results
	/// </summary>
	public class MediaDetailEngine : Engine {
		
		#region Variables
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
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public MediaDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public MediaDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd,bool excel)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
			_excel = excel;
		}

		#endregion

		#region Public methods

		#region Abstract methods implementation
		/// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override ResultTable ComputeResultTable() {
			throw new PortofolioException("The method or operation is not implemented.");
		}
		/// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override string BuildHtmlResult() {
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.radio :
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others :
					return GetDetailMediaHtml();					
				case DBClassificationConstantes.Vehicles.names.press :
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return GetVehicleViewHtml(_excel);					
				default: throw new PortofolioException("The method to get data is not defined for this vehicle.");
			}
		}
		#endregion

		#endregion		

		#region GetFormattedTableDetailMedia 
		/// <summary>
		/// Create a table with each week day the media's investment
		/// and the number of spot
		/// </summary>
		/// <returns>table with each week day the media's investment
		///  and the number of spot</returns>
		public DataTable GetFormattedTableDetailMedia(IPortofolioDAL portofolioDAL) {

			#region Variables
			DataTable dt = null, dtResult = null;
			DataRow newRow = null;
			DateTime dayDT;
			CellUnit cellUnit;
			int currentLine = 0;
			int oldEcranCode = -1;
			int ecranCode;
			string dayString="";
			bool start = true;
			List<CellUnitFactory> listCellUnitFactory = null;
			string classStyleValue = "sc1";
			string cursorHand = (portofolioDAL.IsMediaBelongToCategory(_idMedia, DBCst.Category.ID_THEMATIC_TV)) ? "" : "cursorHand";
			string cssClass = classStyleValue ;
			
			#endregion

			List<UnitInformation> unitsList = _webSession.GetValidUnitForResult();

			DataSet ds = portofolioDAL.GetData();
			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && unitsList != null && unitsList.Count>0) {
				dt = ds.Tables[0];

				#region Init table

				dtResult = new DataTable();
				dtResult.Columns.Add("screenCode", System.Type.GetType("System.Int64"));
				dtResult.Columns.Add("Monday", System.Type.GetType("System.String"));
				dtResult.Columns.Add("Tuesday", System.Type.GetType("System.String"));
				dtResult.Columns.Add("Wednesday", System.Type.GetType("System.String"));
				dtResult.Columns.Add("Thursday", System.Type.GetType("System.String"));
				dtResult.Columns.Add("Friday", System.Type.GetType("System.String"));
				dtResult.Columns.Add("Saturday", System.Type.GetType("System.String"));
				dtResult.Columns.Add("Sunday", System.Type.GetType("System.String"));
				dtResult.Columns.Add("IdUnit", System.Type.GetType("System.Int32"));

				#endregion

				#region for each table row
				Assembly assembly = System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");
				listCellUnitFactory = new List<CellUnitFactory>();
				for (int i = 0; i < unitsList.Count; i++) {
					Type type = assembly.GetType(unitsList[i].CellType);
					cellUnit = (CellUnit)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);					
					listCellUnitFactory.Add(new CellUnitFactory(cellUnit));
				}
				foreach (DataRow row in dt.Rows) {
					ecranCode = int.Parse(row["code_ecran"].ToString());
					dayDT = new DateTime(int.Parse(row["date_media_num"].ToString().Substring(0, 4)), int.Parse(row["date_media_num"].ToString().Substring(4, 2)), int.Parse(row["date_media_num"].ToString().Substring(6, 2)));
					if (ecranCode != oldEcranCode) {						
						for (int i = 0; i < unitsList.Count; i++) {
							newRow = dtResult.NewRow();
							dtResult.Rows.Add(newRow);
						}
						oldEcranCode = ecranCode;
						if(!start)currentLine += unitsList.Count;
					}
					switch (dayDT.DayOfWeek) {

						case DayOfWeek.Monday :
							dayString = "Monday";					
							break; 
						case DayOfWeek.Tuesday:
							dayString = "Tuesday";
							break;
						case DayOfWeek.Wednesday:
							dayString = "Wednesday";
							break;
						case DayOfWeek.Thursday:
							dayString = "Thursday";
							break;
						case DayOfWeek.Friday:
							dayString = "Friday";
							break;
						case DayOfWeek.Saturday:
							dayString = "Saturday";
							break;
						case DayOfWeek.Sunday:
							dayString = "Sunday";
							break;
					}
					
					for (int i = 0; i < unitsList.Count; i++) {												
						dtResult.Rows[currentLine + i]["screenCode"] = long.Parse(row["code_ecran"].ToString());
						cellUnit = listCellUnitFactory[i].Get(double.Parse(row[unitsList[i].Id.ToString()].ToString()));
						dtResult.Rows[currentLine + i][dayString] = cellUnit.Render(cssClass);
						dtResult.Rows[currentLine + i]["IdUnit"] = unitsList[i].Id.GetHashCode();
					}
					start = false;
				}				
				#endregion
			}
			return dtResult;
		}
		 #endregion

		#region GetDetailMediaHtml 
		/// <summary>
		/// Get Detail Media for Tv & Radio
		/// </summary>
		/// <param name="excel">true for excel result</param>
		/// <returns>HTML Code</returns>
		protected string GetDetailMediaHtml() {

			string classStyleValue = "acl2";
			bool color = true;
			bool isTvNatThematiques = false;
			//string style = "cursorHand";
			DataTable dt = null;
			StringBuilder t = new StringBuilder(20000);
			long oldEcranCode = -1;

			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

			 dt = GetFormattedTableDetailMedia(portofolioDAL);

			#region	No data
			if (dt==null || dt.Rows.Count == 0) {
				return GetNoDataMessageHtml();
			}
			#endregion

			//Checks if media belong to TV Nat Thematics
			isTvNatThematiques = portofolioDAL.IsMediaBelongToCategory(_idMedia, DBCst.Category.ID_THEMATIC_TV);
			List<UnitInformation> unitsList = _webSession.GetValidUnitForResult();

			//if (isTvNatThematiques) style = "";
			if (!_excel && !isTvNatThematiques) {
				//Link to acccess all spot detail
				GetAllPeriodInsertions(t, GestionWeb.GetWebWord(1836, _webSession.SiteLanguage));
			}

			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");

			#region First line
			t.Append("\r\n\t<tr height=\"20px\" >");
			t.Append("<td class=\"p2 violetBorderTop\" colspan=2>&nbsp;</td>");
			//Monday
			t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(654, _webSession.SiteLanguage) + "</td>");
			// Tuesday
			t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(655, _webSession.SiteLanguage) + "</td>");
			//Wednesday
			t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(656, _webSession.SiteLanguage) + "</td>");
			// Thursday
			t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(657, _webSession.SiteLanguage) + "</td>");
			// friday
			t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(658, _webSession.SiteLanguage) + "</td>");
			// Saturday
			t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(659, _webSession.SiteLanguage) + "</td>");
			// Sunday
			t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(660, _webSession.SiteLanguage) + "</td>");
			t.Append("</tr>");
			#endregion

			#region Table
			foreach(DataRow dr in dt.Rows) {

				if (color) {
					t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV2';\" class=\"violetBackGroundV2\">");
				}
				else {
					t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='greyBackGround';\" class=\"greyBackGround\">");
				}
				if (oldEcranCode != long.Parse(dr["screenCode"].ToString())) {
					// Screen code
					t.Append("<td class=\"p2\" rowspan=" + unitsList.Count + " align=\"left\" nowrap>" + dr["screenCode"].ToString() + "</td>");
					color = !color;
				}

				if (color) {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"left\" nowrap>" + GestionWeb.GetWebWord(UnitsInformation.Get((TNS.AdExpress.Constantes.Web.CustomerSessions.Unit) long.Parse(dr["idUnit"].ToString())).WebTextId,_webSession.SiteLanguage) + "</td>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"left\" nowrap>" + GestionWeb.GetWebWord(UnitsInformation.Get((TNS.AdExpress.Constantes.Web.CustomerSessions.Unit)long.Parse(dr["idUnit"].ToString())).WebTextId, _webSession.SiteLanguage) + "</td>");
				}
				if (dr["Monday"] != null && dr["Monday"] !=System.DBNull.Value && !dr["Monday"].ToString().Equals("0")) {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Monday','" + dr["screenCode"].ToString() + "');\" title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage)+"\"> ");

					//t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + dr["Monday"].ToString() + "</td>");
					t.Append(dr["Monday"].ToString());
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}

				if (dr["Tuesday"] != null && dr["Tuesday"] != System.DBNull.Value && !dr["Tuesday"].ToString().Equals("0")) {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Tuesday','" + dr["screenCode"].ToString() + "');\" title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">");

					//t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + dr["Tuesday"].ToString() + "</td>");
					t.Append(dr["Tuesday"].ToString());
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}

				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (dr["Wednesday"] != null && dr["Wednesday"] != System.DBNull.Value && !dr["Wednesday"].ToString().Equals("0")) {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Wednesday','" + dr["screenCode"].ToString() + "');\" title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">");

					//t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + dr["Wednesday"].ToString() + "</td>");
					t.Append(dr["Wednesday"].ToString());
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}

				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (dr["Thursday"] != null && dr["Thursday"] != System.DBNull.Value && !dr["Thursday"].ToString().Equals("0")) {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Thursday','" + dr["screenCode"].ToString() + "');\" title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">");

					//t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + dr["Thursday"].ToString() + "</td>");
					t.Append(dr["Thursday"].ToString());
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}

				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (dr["Friday"] != null && dr["Friday"] != System.DBNull.Value && !dr["Friday"].ToString().Equals("0")) {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Friday','" + dr["screenCode"].ToString() + "');\" title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">");

					//t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + dr["Friday"].ToString() + "</td>");
					t.Append(dr["Friday"].ToString());
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}

				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (dr["Saturday"] != null && dr["Saturday"] != System.DBNull.Value && !dr["Saturday"].ToString().Equals("0")) {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Saturday','" + dr["screenCode"].ToString() + "');\" title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">");

					//t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + dr["Saturday"].ToString() + "</td>");
					t.Append(dr["Saturday"].ToString());
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}

				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (dr["Sunday"] != null && dr["Sunday"] != System.DBNull.Value && !dr["Sunday"].ToString().Equals("0")) {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Sunday','" + dr["screenCode"].ToString() + "');\" title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">");

					//t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + dr["Sunday"].ToString() + "</td>");
					t.Append(dr["Sunday"].ToString());
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}

				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				t.Append("</tr>");

				
				oldEcranCode = long.Parse(dr["screenCode"].ToString());
				
			}
			#endregion

			t.Append("</table>");
			return t.ToString();

		}
		#endregion
	}
}
