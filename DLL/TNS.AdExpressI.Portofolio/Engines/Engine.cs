#region Information
// Author: D. Mussuma
// Creation date: 08/08/2008
// Modification date:
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Globalization;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Web.Core.Utilities;

using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;


namespace TNS.AdExpressI.Portofolio.Engines {
	/// <summary>
	/// Define default behaviours of portofolio result like global process, lengendaries, properties...
	/// </summary>
	public abstract class Engine {

		#region Variables
		/// <summary>
		/// Customer session
		/// </summary>
		protected WebSession _webSession;
		/// <summary>
		/// Vehicle
		/// </summary>
		protected VehicleInformation _vehicleInformation;
		/// <summary>
		/// Media Id
		/// </summary>
		protected Int64 _idMedia;
		/// <summary>
		/// Date begin
		/// </summary>
		protected string _periodBeginning;
		/// <summary>
		/// Date end
		/// </summary>
		protected string _periodEnd;
		/// <summary>
		/// Current Module
		/// </summary>
		protected TNS.AdExpress.Domain.Web.Navigation.Module _module;
		#endregion

		#region Constructor
		/// <summary>
        /// Constructor
        /// </summary>
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		protected Engine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd) {
            if(webSession==null) throw (new ArgumentNullException("Customer session is null"));
            _webSession=webSession;
            try {
                // Set Vehicle
				_vehicleInformation = vehicleInformation;
                //Set Media Id
                _idMedia = idMedia;
                // Period
				_periodBeginning = periodBeginning;
				_periodEnd = periodEnd;
                // Module
                _module=ModulesList.GetModule(webSession.CurrentModule);                
            }
            catch(System.Exception err) {
				throw (new PortofolioException("Impossible to set parameters", err));
            }
        }

		#endregion

		#region Virtual Public methods
		/// <summary>
		/// Get result table
		/// </summary>
		/// <returns></returns>
		public virtual ResultTable GetResultTable(){
			return ComputeResultTable();
		}
		/// <summary>
		/// Get Html result
		/// </summary>
		/// <returns></returns>
		public virtual string GetHtmlResult(){
			return BuildHtmlResult();
		}

		#region HTML for vehicle view
		/// <summary>
		/// Get view of the vehicle (HTML)
		/// </summary>
		/// <param name="excel">True for excel result</param>
		/// <returns>HTML code</returns>
		public virtual string GetVehicleViewHtml(bool excel) {

			#region Variables
			string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
			StringBuilder t = new StringBuilder(5000);
			DataSet dsVisuel = null;
			string pathWeb = "";
			#endregion

			#region Accès aux tables
			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

			dsVisuel = portofolioDAL.GetListDate(true, DBCst.TableType.Type.dataVehicle4M);
			DataTable dtVisuel = dsVisuel.Tables[0];
			#endregion

			// Vérifie si le client a le droit aux créations
			if (_webSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id)) {
				if (!excel) {
					if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
						|| _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {

						Hashtable htValue = portofolioDAL.GetInvestmentByMedia();

						//t.Append("</table>");

						int compteur = 0;
						string endBalise = "";
						string day = "";
						t.Append("<table  border=1 cellpadding=0 cellspacing=0 width=600 align=center class=\"paleVioletBackGroundV2 violetBorder\">");
						//Vehicle view
						t.Append("\r\n\t<tr height=\"25px\" ><td colspan=3 class=\"txtBlanc12Bold violetBackGround portofolioSynthesisBorder\" align=\"center\">" + GestionWeb.GetWebWord(1397, _webSession.SiteLanguage) + "</td></tr>");
                        CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);
						for (int i = 0; i < dtVisuel.Rows.Count; i++) {
							//date_media_num

							if (dtVisuel.Rows[i]["disponibility_visual"] != System.DBNull.Value && int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10) {
								pathWeb = WebCst.CreationServerPathes.IMAGES + "/" + _idMedia.ToString() + "/" + dtVisuel.Rows[i]["date_cover_num"].ToString() + "/Imagette/" + WebCst.CreationServerPathes.COUVERTURE + "";
							}
							else {
								pathWeb = "/App_Themes/" + themeName + "/Images/Culture/Others/no_visuel.gif";
							}
							DateTime dayDT = new DateTime(int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(0, 4)), int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(4, 2)), int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(6, 2)));
                            day = DayString.GetCharacters(dayDT, cultureInfo) + " " + dayDT.ToString("dd/MM/yyyy"); ;
							if (compteur == 0) {
								t.Append("<tr>");
								compteur = 1;
								endBalise = "";
							}
							else if (compteur == 1) {
								compteur = 2;
								endBalise = "";
							}
							else {
								compteur = 0;
								endBalise = "</td></tr>";

							}
							t.Append("<td class=\"portofolioSynthesisBorder\"><table  border=0 cellpadding=0 cellspacing=0 width=100% >");
							t.Append("<tr><td class=\"portofolioSynthesis\" align=center >" + day + "</td><tr>");
							t.Append("<tr><td align=\"center\" class=\"portofolioSynthesis\" >");
							if (dtVisuel.Rows[i]["disponibility_visual"] != System.DBNull.Value && int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10) {
								t.Append("<a href=\"javascript:portofolioCreation('" + _webSession.IdSession + "','" + _idMedia + "','" + dtVisuel.Rows[i]["date_media_num"].ToString() + "','" + dtVisuel.Rows[i]["date_cover_num"].ToString() + "','" + dtVisuel.Rows[i]["media"] + "','" + dtVisuel.Rows[i]["number_page_media"].ToString() + "');\" >");
							}
							t.Append(" <img alt=\"" + GestionWeb.GetWebWord(1409, _webSession.SiteLanguage) + "\" src='" + pathWeb + "' border=\"0\" width=180 height=220>");
							if (dtVisuel.Rows[i]["disponibility_visual"] != System.DBNull.Value && int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10) {
								t.Append("</a>");
							}
							t.Append("</td></tr>");
							if (htValue.Count > 0) {
								if (htValue.ContainsKey(dtVisuel.Rows[i]["date_cover_num"])) {
									t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1398, _webSession.SiteLanguage) + " : " + ((string[])htValue[dtVisuel.Rows[i]["date_cover_num"]])[1] + "</td><tr>");
									t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1399, _webSession.SiteLanguage) + " :" + int.Parse(((string[])htValue[dtVisuel.Rows[i]["date_cover_num"]])[0]).ToString("### ### ### ###") + "</td><tr>");
								}
								else {
									t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1398, _webSession.SiteLanguage) + " : 0</td><tr>");
									t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1399, _webSession.SiteLanguage) + " : 0</td><tr>");

								}
							}
							t.Append("</table></td>");
							t.Append(endBalise);
						}
						if (compteur != 0)
							t.Append("</tr>");

						t.Append("</table>");
					}
				}
			}

			return t.ToString();
		}
		#endregion

		#region Get All Period Insertions
		/// <summary>
		/// Show the link for access to all insetion detail.
		/// </summary>
		/// <param name="t">String builder</param>
		/// <param name="linkText">Link text</param>
		/// <param name="webSession">session client</param>
		/// <param name="idMedia">Id media</param>
		public virtual void GetAllPeriodInsertions(StringBuilder t, string linkText) {
			string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");
			t.Append("<TR height=10><TD ><a class=\"roll03\" href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','','');\" ");
			t.Append(" onmouseover=\"detailSpotButton.src='/App_Themes/" + themeName + "/Images/Common/detailSpot_down.gif';\" onmouseout=\"detailSpotButton.src='/App_Themes/" + themeName + "/Images/Common/detailSpot_up.gif';\" ");
			t.Append("><IMG NAME=\"detailSpotButton\" src=\"/App_Themes/" + themeName + "/Images/Common/detailSpot_up.gif\" BORDER=0 align=absmiddle alt=\"" + linkText + "\">");
			t.Append("&nbsp;" + linkText);
			t.Append("</a></TD></TR>");
			t.Append("</table>");
			t.Append("<br><br>");
		}
		#endregion

		#endregion

		#region Internal methods
		/// <summary>
		/// Get result table
		/// </summary>
		/// <returns></returns>
		protected abstract ResultTable ComputeResultTable();

		/// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected abstract string BuildHtmlResult();
		#endregion

		#region Protected methods
        /*
		#region Get day of week
		/// <summary>
		/// Get day of week
		/// </summary>
		/// <param name="dayOfWeek">Day Of Week</param>
		/// <returns>Day Of Week</returns>
		protected virtual string GetDayOfWeek(string dayOfWeek) {
			string txt = "";
			switch (dayOfWeek) {
				case "Monday":
					txt = GestionWeb.GetWebWord(654, _webSession.SiteLanguage);
					break;
				case "Tuesday":
					txt = GestionWeb.GetWebWord(655, _webSession.SiteLanguage);
					break;
				case "Wednesday":
					txt = GestionWeb.GetWebWord(656, _webSession.SiteLanguage);
					break;
				case "Thursday":
					txt = GestionWeb.GetWebWord(657, _webSession.SiteLanguage);
					break;
				case "Friday":
					txt = GestionWeb.GetWebWord(658, _webSession.SiteLanguage);
					break;
				case "Saturday":
					txt = GestionWeb.GetWebWord(659, _webSession.SiteLanguage);
					break;
				case "Sunday":
					txt = GestionWeb.GetWebWord(660, _webSession.SiteLanguage);
					break;
			}
			return txt;
		}
		#endregion		
        */
		/// <summary>
		/// No data message
		/// </summary>
		/// <returns></returns>
		protected virtual string GetNoDataMessageHtml() {
			#region No data
			return ("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, _webSession.SiteLanguage)
				+ "</div>");
			#endregion
		}

		#endregion
	}
}
