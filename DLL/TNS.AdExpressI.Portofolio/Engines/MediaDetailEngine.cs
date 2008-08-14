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
			string style = "cursorHand";

			StringBuilder t = new StringBuilder(20000);
			string nbrInsertion = "";
			switch (_vehicleInformation.Id) {
				case DBClassificationConstantes.Vehicles.names.radio:
					nbrInsertion = GestionWeb.GetWebWord(939, _webSession.SiteLanguage);
					break;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					nbrInsertion = GestionWeb.GetWebWord(939, _webSession.SiteLanguage);
					break;
				default:
					break;
			}

			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

			int[,] tab = GetFormattedTableDetailMedia(portofolioDAL);

			#region aucune données
			if (tab.GetLength(0) == 0) {
				return ("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, _webSession.SiteLanguage)
					+ "</div>");
			}
			#endregion

			//Vérifie si le support appartient à la TV Nat Thématiques
			isTvNatThematiques = portofolioDAL.IsMediaBelongToCategory(_idMedia, DBCst.Category.ID_THEMATIC_TV);
			if (isTvNatThematiques) style = "";
			if (!_excel && !isTvNatThematiques) {
				//Ensemble du spot à spot sur la période intérrogée
				GetAllPeriodInsertions(t, GestionWeb.GetWebWord(1836, _webSession.SiteLanguage));
			}

			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");

			#region Première ligne
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
			for (int i = 0; i < tab.GetLength(0) && int.Parse(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN].ToString()) >= 0; i++) {

				if (color) {
					t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV2';\" class=\"violetBackGroundV2\">");
				}
				else {
					t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='greyBackGround';\" class=\"greyBackGround\">");
				}
				// code écran
				t.Append("<td class=\"p2\" rowspan=2 align=\"left\" nowrap>" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "</td>");

				if (color) {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"left\" nowrap>" + GestionWeb.GetWebWord(868, _webSession.SiteLanguage) + "</td>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"left\" nowrap>" + GestionWeb.GetWebWord(868, _webSession.SiteLanguage) + "</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Monday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}

				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Tuesday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}

				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Wednesday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Thursday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Friday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Saturday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Sunday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				t.Append("</tr>");

				if (color) {
					t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV2';\" class=\"violetBackGroundV2\">");
					t.Append("<td class=\"" + classStyleValue + "\" align=\"left\" nowrap>" + GestionWeb.GetWebWord(939, _webSession.SiteLanguage) + "</td>");
					color = !color;
				}
				else {
					t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='greyBackGround';\" class=\"greyBackGround\">");
					t.Append("<td class=\"" + classStyleValue + "\" align=\"left\" nowrap>" + GestionWeb.GetWebWord(939, _webSession.SiteLanguage) + "</td>");
					color = !color;
				}

				// Partie Nombre de spot

				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Monday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Tuesday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Wednesday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Thursday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Friday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Saturday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION].ToString() != "0") {
					if (!_excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Sunday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

					t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
					if (!_excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else {
					t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
				}
				t.Append("</tr>");

			}
			#endregion

			t.Append("</table>");
			return t.ToString();

		}
		#endregion

		#region GetFormattedTableDetailMedia
		/// <summary>
		/// Create a table with each week day the media's investment
		/// and the number of spot
		/// </summary>
		/// <returns>table with each week day the media's investment
		///  and the number of spot</returns>
		public int[,] GetFormattedTableDetailMedia(IPortofolioDAL portofolioDAL) {

			#region Variables
			int[,] tab = null;
			DataTable dt = null;
			DateTime dayDT;
			int currentLine = -1;
			int oldEcranCode = -1;
			int ecranCode;
			
			#endregion

			DataSet ds = portofolioDAL.GetData();//portofolioDAL.GetCommercialBreakForTvRadio().Tables[0];
			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
				dt = ds.Tables[0];
				
				#region Init table
				tab = new int[dt.Rows.Count, FrameWorkResultConstantes.PortofolioDetailMedia.TOTAL_INDEX];
				#endregion

				#region for each table row
				foreach (DataRow row in dt.Rows) {
					ecranCode = int.Parse(row["code_ecran"].ToString());
					dayDT = new DateTime(int.Parse(row["date_media_num"].ToString().Substring(0, 4)), int.Parse(row["date_media_num"].ToString().Substring(4, 2)), int.Parse(row["date_media_num"].ToString().Substring(6, 2)));
					if (ecranCode != oldEcranCode) {
						currentLine++;
						oldEcranCode = ecranCode;
						tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] = int.Parse(row["code_ecran"].ToString());
					}
					switch (dayDT.DayOfWeek.ToString()) {

						case "Monday":
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_VALUE] += int.Parse(row["value"].ToString());
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_INSERTION] += int.Parse(row["insertion"].ToString());
							break;
						case "Tuesday":
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_VALUE] += int.Parse(row["value"].ToString());
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION] += int.Parse(row["insertion"].ToString());
							break;
						case "Wednesday":
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_VALUE] += int.Parse(row["value"].ToString());
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION] += int.Parse(row["insertion"].ToString());
							break;
						case "Thursday":
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_VALUE] += int.Parse(row["value"].ToString());
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION] += int.Parse(row["insertion"].ToString());
							break;
						case "Friday":
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_VALUE] += int.Parse(row["value"].ToString());
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION] += int.Parse(row["insertion"].ToString());
							break;
						case "Saturday":
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_VALUE] += int.Parse(row["value"].ToString());
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION] += int.Parse(row["insertion"].ToString());
							break;
						case "Sunday":
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_VALUE] += int.Parse(row["value"].ToString());
							tab[currentLine, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION] += int.Parse(row["insertion"].ToString());
							break;
					}
				}
				//if condition added to fix the bug in detail support when we select the single date
				if (currentLine + 1 < dt.Rows.Count)
					tab[currentLine + 1, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] = FrameWorkConstantes.Results.PortofolioDetailMedia.END_ARRAY;
				#endregion
			}
			return tab;
		}
		#endregion
	}
}
