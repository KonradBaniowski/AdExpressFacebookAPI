#region Information
// Author: G. Facon
// Creation date: 17/03/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web.Navigation;
using WebCst=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Result;
using WebFunctions = TNS.AdExpress.Web.Functions;

using TNS.AdExpress.Domain.Level;
using DBCst=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Web.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using System.Reflection;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpressI.Portofolio {
    /// <summary>
    /// Portofolio Results
    /// </summary>
    public abstract class PortofolioResults:IPortofolioResults {

        #region Constantes
        protected const long TOTAL_LINE_INDEX=0;
        protected const long DETAILED_PORTOFOLIO_EURO_COLUMN_INDEX=2;
        protected const long DETAILED_PORTOFOLIO_INSERTION_COLUMN_INDEX=3;
        protected const long DETAILED_PORTOFOLIO_DURATION_COLUMN_INDEX=4;
        protected const long DETAILED_PORTOFOLIO_MMC_COLUMN_INDEX=4;
        protected const long DETAILED_PORTOFOLIO_PAGE_COLUMN_INDEX=5;
        protected const int PROD_COL = 1164;
        protected const int INSERTIONS_LIST_COL = 2245;
        protected const int CREATIVES_COL = 1994;
        protected const int PM_COL = 751;
        protected const int EUROS_COL = 1423;
        protected const int MM_COL = 1424;
        protected const int SPOTS_COL = 939;
        protected const int INSERTIONS_COL = 940;
        protected const int PAGE_COL =943;
        protected const int PAN_COL = 1604;
        protected const int DURATION_COL = 1435;
        protected const int VOLUME = 2216;
        protected const int TOTAL_COL = 1401;
        protected const int POURCENTAGE_COL = 1236;
        #endregion

        #region Variables
        /// <summary>
        /// Customer session
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Vehicle
        /// </summary>
        protected DBClassificationConstantes.Vehicles.names _vehicle;
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
        /// <summary>
        /// Show creations in the result
        /// </summary>
        protected bool _showCreatives;
        /// <summary>
        /// Show insertions in the result
        /// </summary>
        protected bool _showInsertions;		
		/// <summary>
		/// Screen code
		/// </summary>
		protected string _adBreak;
		/// <summary>
		/// Day of Week
		/// </summary>
		protected string _dayOfWeek;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        protected PortofolioResults(WebSession webSession) {
            if(webSession==null) throw (new ArgumentNullException("Customer session is null"));
            _webSession=webSession;
            try {
                // Set Vehicle
                _vehicle=GetVehicleName();
                //Set Media Id
                _idMedia = GetMediaId();
                // Period
                _periodBeginning = GetDateBegin();
                _periodEnd = GetDateEnd();
                // Module
                _module=ModulesList.GetModule(webSession.CurrentModule);
                _showCreatives=ShowCreatives();
                _showInsertions=ShowInsertions();
            }
            catch(System.Exception err) {
                throw (new PortofolioException("Impossible to set parameters",err));
            }
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="adBreak">Screen code</param>
		/// <param name="dayOfWeek">Day of week</param>
		protected PortofolioResults(WebSession webSession, string adBreak, string dayOfWeek) {
			if (webSession == null) throw (new ArgumentNullException("Customer session is null"));
			_webSession = webSession;
			try {
				// Set Vehicle
				_vehicle = GetVehicleName();
				//Set Media Id
				_idMedia = GetMediaId();
				// Period
				_periodBeginning = GetDateBegin();
				_periodEnd = GetDateEnd();
				// Module
				_module = ModulesList.GetModule(webSession.CurrentModule);
				_showCreatives = ShowCreatives();
				_showInsertions = ShowInsertions();
				_adBreak = adBreak;
				_dayOfWeek = dayOfWeek;
			}
			catch (System.Exception err) {
				throw (new PortofolioException("Impossible to set parameters", err));
			}
		}
        #endregion

        #region IResult Membres

        #region HTML for:SYNTHESIS, NOVELTY, DETAIL MEDIA, PERFORMANCES
        /// <summary>
        /// Get HTML code for some portofolio result
        ///  - SYNTHESIS
        ///  - NOVELTY
        ///  - DETAIL_MEDIA
        ///  - PERFORMANCES
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="webSession">Customer session</param>
        /// <returns>HTML Code</returns>
        public string GetHtml(Page page) {
            throw new Exception("The method or operation is not implemented.");
        }
		/// <summary>
		/// Get Structure html result
		/// </summary>
		/// <param name="excel">True if export excel</param>
		/// <returns>html code</returns>
		public string GetStructureHtml(bool excel) {
			#region  Variables
			string L2 = "acl2";			
			if (excel) L2 = "acl21";			
			string classCss = L2;			
			DataTable dtFormat;
			DataTable dtColor;
			DataTable dtInsert;
			DataTable dtLocation;
			StringBuilder t = new StringBuilder(2000);
			object[,] tab = null;
			#endregion

			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicle;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
		
			switch (_vehicle) {
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:

					GetStructPressResult(portofolioDAL,out dtFormat, out dtColor, out dtLocation, out dtInsert);
					if (dtFormat == null && dtColor == null && dtInsert == null && dtLocation == null) {
						#region No data
						return ("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, _webSession.SiteLanguage)
							+ "</div>");
						#endregion
					}
					t.Append(GetPressStructureHtml(dtFormat, dtColor, dtInsert, dtLocation, classCss));
					if (excel) t.Append(TNS.AdExpress.Web.UI.ExcelWebPage.GetFooter(_webSession));
					return t.ToString();
				case DBClassificationConstantes.Vehicles.names.radio:
					tab = GetStructRadio(portofolioDAL);
					if (tab == null || tab.GetLength(0) == 0) {
						#region No data
						return ("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, _webSession.SiteLanguage)
							+ "</div>");
						#endregion
					}
					t.Append(GetStructureHtml(tab, classCss));
					if (excel) t.Append(TNS.AdExpress.Web.UI.ExcelWebPage.GetFooter(_webSession));
					return t.ToString();
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					tab = GetStructTV(portofolioDAL);
					if (tab == null || tab.GetLength(0) == 0) {
						#region No data
						return ("<div align=\"center\" class=\"txtViolet11Bold\">" + GestionWeb.GetWebWord(177, _webSession.SiteLanguage)
							+ "</div>");
						#endregion
					}
					t.Append(GetStructureHtml(tab, classCss));
					if (excel) t.Append(TNS.AdExpress.Web.UI.ExcelWebPage.GetFooter(_webSession));
					return t.ToString();
				default:
					throw new PortofolioException("Vehicle unknown.");
			}
		}
        #endregion

        #region HTML for vehicle view
        /// <summary>
        /// Get view of the vehicle (HTML)
        /// </summary>
        /// <param name="excel">True for excel result</param>
        /// <returns>HTML code</returns>
        public string GetVehicleViewHtml(bool excel) {

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
            parameters[1] = _vehicle;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

            dsVisuel = portofolioDAL.GetListDate(true);
            DataTable dtVisuel = dsVisuel.Tables[0];
            #endregion

            // Vérifie si le client a le droit aux créations
            if (_webSession.CustomerLogin.ShowCreatives(_vehicle)) {
                if (!excel) {
                    if (_vehicle == DBClassificationConstantes.Vehicles.names.press
                        || _vehicle == DBClassificationConstantes.Vehicles.names.internationalPress) {

                        Hashtable htValue = portofolioDAL.GetInvestmentByMedia();

                        //t.Append("</table>");

                        int compteur = 0;
                        string endBalise = "";
                        string day = "";
                        t.Append("<table  border=1 cellpadding=0 cellspacing=0 width=600 align=center class=\"paleVioletBackGroundV2 violetBorder\">");
                        //Vehicle view
                        t.Append("\r\n\t<tr height=\"25px\" ><td colspan=3 class=\"txtBlanc12Bold violetBackGround portofolioSynthesisBorder\" align=\"center\">" + GestionWeb.GetWebWord(1397, _webSession.SiteLanguage) + "</td></tr>");
                        for (int i = 0; i < dtVisuel.Rows.Count; i++) {
                            //date_media_num

                            if (dtVisuel.Rows[i]["disponibility_visual"] != System.DBNull.Value && int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10) {
                                pathWeb = WebCst.CreationServerPathes.IMAGES + "/" + _idMedia.ToString() + "/" + dtVisuel.Rows[i]["date_cover_num"].ToString() + "/Imagette/" + WebCst.CreationServerPathes.COUVERTURE + "";
                            }
                            else {
                                pathWeb = "/App_Themes/"+themeName+"/Images/Culture/Others/no_visuel.gif";
                            }
                            DateTime dayDT = new DateTime(int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(0, 4)), int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(4, 2)), int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(6, 2)));
                            day = GetDayOfWeek(dayDT.DayOfWeek.ToString()) + " " + dayDT.ToString("dd/MM/yyyy");

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

        #region HTML detail media
        /// <summary>
        /// Get media detail html
        /// </summary>
        /// <param name="excel">true for excel result</param>
        /// <returns>HTML Code</returns>
        public string GetDetailMediaHtml(bool excel) {
            switch (_vehicle) {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return GetDetailMediaPressHtml(excel);
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    return GetDetailMediaTvRadioHtml(excel);
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
        }
        #endregion

        #region Get Detail Media for Press
        /// <summary>
        ///  Get Detail Media for Press
        /// </summary>
        /// <param name="excel">True for excel result</param>
        /// <returns>Code Html</returns>
        public string GetDetailMediaPressHtml(bool excel) {

            StringBuilder t = new StringBuilder(5000);

            GetAllPeriodInsertions(t, GestionWeb.GetWebWord(1837, _webSession.SiteLanguage));
            t.Append(GetVehicleViewHtml(excel));

            return t.ToString();
        }

        #endregion

        #region Get Detail Media for Tv & Radio
        /// <summary>
        /// Get Detail Media for Tv & Radio
        /// </summary>
        /// <param name="excel">true for excel result</param>
        /// <returns>HTML Code</returns>
        public string GetDetailMediaTvRadioHtml(bool excel) {

            string classStyleValue = "acl2";
            bool color = true;
            bool isTvNatThematiques = false;
            string style = "cursorHand";

            StringBuilder t = new StringBuilder(20000);
            string nbrInsertion = "";
            switch (_vehicle) {
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
            parameters[1] = _vehicle;
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
            if (!excel && !isTvNatThematiques) {
                //Ensemble du spot à spot sur la période intérrogée
                GetAllPeriodInsertions(t, GestionWeb.GetWebWord(1836, _webSession.SiteLanguage));
            }

            t.Append("<table border=0 cellpadding=0 cellspacing=0 >");

            #region Première ligne
            t.Append("\r\n\t<tr height=\"20px\" >");
            t.Append("<td class=\"p2 violetBorderTop\" colspan=2>&nbsp;</td>");
            // Lundi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(654, _webSession.SiteLanguage) + "</td>");
            // Mardi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(655, _webSession.SiteLanguage) + "</td>");
            // Mercredi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(656, _webSession.SiteLanguage) + "</td>");
            // Jeudi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(657, _webSession.SiteLanguage) + "</td>");
            // Vendredi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(658, _webSession.SiteLanguage) + "</td>");
            // Samedi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(659, _webSession.SiteLanguage) + "</td>");
            // Dimanche
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
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Monday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }

                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Tuesday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }

                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Wednesday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");
                    
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Thursday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");
                    
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Friday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Saturday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");
                    
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Sunday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");
                    
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_VALUE].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
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
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Monday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Tuesday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Wednesday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Thursday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Friday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Saturday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
                        t.Append("</a>");
                }
                else {
                    t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                }
                if (tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION].ToString() != "0") {
                    if (!excel && !isTvNatThematiques)
                        t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','Sunday','" + tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN] + "');\" >");

                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">" + Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
                    if (!excel && !isTvNatThematiques)
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

        #region Result table for: DETAIL PORTOFOLIO, CALENDAR
        /// <summary>
        /// Get ResultTable for some portofolio result
        ///  - DETAIL_PORTOFOLIO
        ///  - CALENDAR
        /// </summary>
        /// <param name="webSession">Customer session</param>
        /// <returns>Result Table</returns>
        public ResultTable GetResultTable() {
            try {
                switch(_webSession.CurrentTab) {
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                        return GetPortofolioResultTable();
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                        return GetCalendar();
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
                        return GetSynthesisResultTable();
                    default:
                        return null;
                }
            }
            catch(System.Exception err) {
                throw (new PortofolioException("Impossible de calculer le résultat d'une analyse de portefeuille",err));
            }
        }
        #endregion

        #endregion

        #region Rules

        #region Synthesis
        /// <summary>
        /// Get Result Table for portofolio synthesis
        /// </summary>
        /// <returns>ResultTable</returns>
        public ResultTable GetSynthesisResultTable() {

            #region Constantes
            /// <summary>
            /// Header column index
            /// </summary>
            const int HEADER_COLUMN_INDEX = 0;
            /// <summary>
		    /// First column index
		    /// </summary>
		    const int FIRST_COLUMN_INDEX = 1;
		    /// <summary>
		    /// Second column index
		    /// </summary>
		    const int SECOND_COLUMN_INDEX =2;
            #endregion

            #region Variables
            string investment = "";
            string firstDate = "";
            string lastDate = "";
            string support = "";
            string periodicity = "";
            string category = "";
            string regie = "";
            string interestCenter = "";
            string pageNumber = "";
            string ojd = "";
            string nbrSpot = "";
            string nbrEcran = "";
            decimal averageDurationEcran = 0;
            decimal nbrSpotByEcran = 0;
            string totalDuration = "";
            string numberBoard = "";
            ResultTable resultTable = null;
            LineType lineType = LineType.level1;
            string typeReseauStr = string.Empty;
            #endregion

            #region Accès aux tables
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicle;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

            DataSet dsInvestment = portofolioDAL.GetInvestment();
            DataTable dtInvestment = dsInvestment.Tables[0];

            DataSet dsInsertionNumber = portofolioDAL.GetInsertionNumber();
            DataTable dtInsertionNumber = dsInsertionNumber.Tables[0];

            DataSet dsCategory = portofolioDAL.GetCategoryMediaSellerData();
            DataTable dtCategory = dsCategory.Tables[0];

            DataSet dsPage = portofolioDAL.GetPage();
            DataTable dtPage = dsPage.Tables[0];
            
            DataTable dtTypeSale = null;
            if (_vehicle == DBClassificationConstantes.Vehicles.names.outdoor) {
                DataSet dsTypeSale = portofolioDAL.GetTypeSale();
                dtTypeSale = dsTypeSale.Tables[0];
            }

            object[] tab = portofolioDAL.NumberProductAdvertiser();
            object[] tabEncart = null;

            if (_vehicle == DBClassificationConstantes.Vehicles.names.press || _vehicle == DBClassificationConstantes.Vehicles.names.internationalPress) {
                tabEncart = portofolioDAL.NumberPageEncart();
            }
            #endregion

            #region Parcours des tableaux
            foreach (DataRow row in dtInvestment.Rows) {
                
                investment = row["investment"].ToString();
                firstDate = row["first_date"].ToString();
                lastDate = row["last_date"].ToString();
                
                if (dtInvestment.Columns.Contains("duration")) {
                    totalDuration = row["duration"].ToString();
                }
                if (dtInvestment.Columns.Contains("number_board")) {
                    numberBoard = row["number_board"].ToString();
                }

            }
            //nombre d'insertions
            if (!dtInsertionNumber.Equals(System.DBNull.Value) && dtInsertionNumber.Rows.Count > 0) {
                nbrSpot = dtInsertionNumber.Rows[0]["insertion"].ToString();
            }
            foreach (DataRow row in dtCategory.Rows) {
                support = row["support"].ToString();
                category = row["category"].ToString();
                regie = row["media_seller"].ToString();
                interestCenter = row["interest_center"].ToString();
                if (dtCategory.Columns.Contains("periodicity"))
                    periodicity = row["periodicity"].ToString();
                if (dtCategory.Columns.Contains("ojd"))
                    ojd = row["ojd"].ToString();
            }

            if (_vehicle== DBClassificationConstantes.Vehicles.names.press
                || _vehicle == DBClassificationConstantes.Vehicles.names.internationalPress) {
                foreach (DataRow row in dtPage.Rows) {
                    pageNumber = row["page"].ToString();
                }
                if (pageNumber.Length == 0)
                    pageNumber = "0";
            }

            if (_vehicle == DBClassificationConstantes.Vehicles.names.radio
                || _vehicle == DBClassificationConstantes.Vehicles.names.tv
                || _vehicle == DBClassificationConstantes.Vehicles.names.others) {
                DataSet dsEcran = portofolioDAL.GetEcranData();
                DataTable dtEcran = dsEcran.Tables[0];

                foreach (DataRow row in dtEcran.Rows) {
                    nbrEcran = row["nbre_ecran"].ToString();
                    if (row["nbre_ecran"] != System.DBNull.Value) {
                        averageDurationEcran = decimal.Parse(row["ecran_duration"].ToString()) / decimal.Parse(row["nbre_ecran"].ToString());
                        nbrSpotByEcran = decimal.Parse(row["nbre_spot"].ToString()) / decimal.Parse(row["nbre_ecran"].ToString());
                    }
                }
            }
            #endregion

            #region Période
            DateTime dtFirstDate = DateTime.Today;
            DateTime dtLastDate = DateTime.Today;
            if (_vehicle == DBClassificationConstantes.Vehicles.names.outdoor) {
                if (firstDate.Length > 0) {
                    dtFirstDate = Convert.ToDateTime(firstDate);
                    dtFirstDate = dtFirstDate.Date;
                }
                if (lastDate.Length > 0) {
                    dtLastDate = Convert.ToDateTime(lastDate);
                    dtLastDate = dtLastDate.Date;
                }
            }
            else {
                if (firstDate.Length > 0) {
                    dtFirstDate = new DateTime(int.Parse(firstDate.Substring(0, 4)), int.Parse(firstDate.Substring(4, 2)), int.Parse(firstDate.Substring(6, 2)));
                }

                if (lastDate.Length > 0) {
                    dtLastDate = new DateTime(int.Parse(lastDate.Substring(0, 4)), int.Parse(lastDate.Substring(4, 2)), int.Parse(lastDate.Substring(6, 2)));
                }
            }
            #endregion

            #region nbLines init
            long nbLines = 0;
            long lineIndex = 0;
            switch (_vehicle) {
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress:
                    nbLines = 16;
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                    nbLines = 14;
                    break;
                default:
                    throw (new PortofolioException("Vehicle unknown"));
            }
            #endregion

            #region headers
            Headers headers = new Headers();

            TNS.FrameWork.WebResultUI.Header header = new TNS.FrameWork.WebResultUI.Header(support.ToString(), HEADER_COLUMN_INDEX, "SynthesisH1");
            header.Add(new TNS.FrameWork.WebResultUI.Header("", FIRST_COLUMN_INDEX, "SynthesisH2"));
            header.Add(new TNS.FrameWork.WebResultUI.Header("", SECOND_COLUMN_INDEX, "SynthesisH2"));
            headers.Root.Add(header);
            resultTable = new ResultTable(nbLines, headers);
            #endregion

            #region Construction de resultTable
            // Date de début et fin de vague pour affichage
            if (_vehicle== DBClassificationConstantes.Vehicles.names.outdoor){
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1607, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(dtFirstDate.Date.ToString("dd/MM/yyyy"));

                ChangeLineType(ref lineType);

                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1608, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(dtLastDate.Date.ToString("dd/MM/yyyy"));
            }
            // Date de parution ou diffusion
            else {
                if (firstDate.Length > 0) {
                    if (_vehicle == DBClassificationConstantes.Vehicles.names.press
                        || _vehicle == DBClassificationConstantes.Vehicles.names.internationalPress) {
                        //Cas Presse : Date de parution 
                        lineIndex = resultTable.AddNewLine(lineType);
                        resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1381, _webSession.SiteLanguage));
                    }
                    else {
                        // Cas TV-Radio : Date de diffusion
                        lineIndex = resultTable.AddNewLine(lineType);
                        resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1380, _webSession.SiteLanguage));
                    }
                    if (firstDate == lastDate) {
                        resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(dtFirstDate.Date.ToString("dd/MM/yyyy"));
                    }
                    else {
                        resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel("Du " + dtFirstDate.Date.ToString("dd/MM/yyyy") + " au " + dtLastDate.Date.ToString("dd/MM/yyyy"));
                    }
                }
            }
            
            ChangeLineType(ref lineType);

            // Périodicité
            if (dtCategory.Columns.Contains("periodicity")) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1450, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(periodicity);
                ChangeLineType(ref lineType);
            }
            // Categorie
            lineIndex = resultTable.AddNewLine(lineType);
            resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1416, _webSession.SiteLanguage));
            resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(category);
            ChangeLineType(ref lineType);

            // Régie
            lineIndex = resultTable.AddNewLine(lineType);
            resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1417, _webSession.SiteLanguage));
            resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(regie);
            ChangeLineType(ref lineType);

            // Centre d'intérêt
            lineIndex = resultTable.AddNewLine(lineType);
            resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1411, _webSession.SiteLanguage));
            resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(interestCenter);
            ChangeLineType(ref lineType);

            //number board et type de reseau ,cas de l'Affichage
            if (_vehicle == DBClassificationConstantes.Vehicles.names.outdoor && dtTypeSale.Rows.Count > 0) {

                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1604, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(numberBoard));
                ChangeLineType(ref lineType);

                int count = 0;
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1609, _webSession.SiteLanguage));
                if (dtTypeSale.Rows.Count == 0) typeReseauStr="&nbsp;";
                else {
                    foreach (DataRow row in dtTypeSale.Rows) {
                        if (count > 0) {
                            typeReseauStr += "<BR>";
                        }
                        typeReseauStr += SQLGenerator.SaleTypeOutdoor(row["type_sale"].ToString(), _webSession.SiteLanguage);
                        count++;
                    }
                }
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(typeReseauStr);
                ChangeLineType(ref lineType);

            }
            // Cas de la presse
            if (_vehicle == DBClassificationConstantes.Vehicles.names.press
                || _vehicle == DBClassificationConstantes.Vehicles.names.internationalPress) {
                // Nombre de page
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1385, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(pageNumber));
                ChangeLineType(ref lineType);

                if (((string)tabEncart[0]).Length > 0) {
                    // Nombre de page pub		
                    lineIndex = resultTable.AddNewLine(lineType);
                    resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1386, _webSession.SiteLanguage));
                    //resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPage(double.Parse(Units.ConvertUnitValueAndPdmToString((string)tabEncart[0], WebCst.CustomerSessions.Unit.pages, false)));
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPage(double.Parse(tabEncart[0].ToString()));

                    ChangeLineType(ref lineType);

                    // Ratio
                    if (pageNumber.Length > 0) {
                        lineIndex = resultTable.AddNewLine(lineType);
                        resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1387, _webSession.SiteLanguage));
                        //resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPercent(((decimal)(decimal.Parse(Units.ConvertUnitValueAndPdmToString((string)tabEncart[0], WebCst.CustomerSessions.Unit.pages, false)) / decimal.Parse(pageNumber) * 100)).ToString("0.###") + " %");
                        resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(((decimal.Parse(tabEncart[0].ToString()) / decimal.Parse(pageNumber) * 100)/1000).ToString("0.###")+" %");
                    }
                    else {
                        lineIndex = resultTable.AddNewLine(lineType);
                        resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1387, _webSession.SiteLanguage));
                        resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel("&nbsp;&nbsp;&nbsp;&nbsp;");
                    }
                    ChangeLineType(ref lineType);
                }

                if (((string)tabEncart[1]).Length > 0) {
                    // Nombre de page de pub hors encarts
                    if (((string)tabEncart[1]).Length == 0)
                        tabEncart[1] = "0";
                    lineIndex = resultTable.AddNewLine(lineType);
                    resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1388, _webSession.SiteLanguage));
                    //resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPage(double.Parse(Units.ConvertUnitValueAndPdmToString((string)tabEncart[1], WebCst.CustomerSessions.Unit.pages, false)));
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPage(double.Parse(tabEncart[1].ToString()));
                    ChangeLineType(ref lineType);
                }
                if (((string)tabEncart[2]).Length > 0) {
                    // Nombre de page de pub encarts
                    if (((string)tabEncart[2]).Length == 0) {
                        tabEncart[2] = "0";
                    }
                    lineIndex = resultTable.AddNewLine(lineType);
                    resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1389, _webSession.SiteLanguage));
                    //resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPage(double.Parse(Units.ConvertUnitValueAndPdmToString((string)tabEncart[2], WebCst.CustomerSessions.Unit.pages, false)));
                    resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPage(double.Parse(tabEncart[2].ToString()));
                    ChangeLineType(ref lineType);
                }
            }

            // Cas tv, radio
            if (_vehicle == DBClassificationConstantes.Vehicles.names.radio
                || _vehicle == DBClassificationConstantes.Vehicles.names.tv
                || _vehicle == DBClassificationConstantes.Vehicles.names.others) {

                //Nombre de spot
                if (nbrSpot.Length == 0) {
                    nbrSpot = "0";
                }
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1404, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(nbrSpot));
                ChangeLineType(ref lineType);

                // Nombre d'ecran
                if (nbrEcran.Length == 0) {
                    nbrEcran = "0";
                }
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1412, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(nbrEcran));
                ChangeLineType(ref lineType);

                if (totalDuration.Length == 0) {
                    totalDuration = "0";
                    //totalDuration = Units.ConvertUnitValueAndPdmToString(totalDuration, WebCst.CustomerSessions.Unit.duration, false);
                }
                // total durée
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1413, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellDuration(double.Parse(totalDuration));
                ChangeLineType(ref lineType);
            }

            // Total investissements
            if (investment.Length > 0) {
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1390, _webSession.SiteLanguage));
                //resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellEuro(double.Parse(Units.ConvertUnitValueAndPdmToString(investment, WebCst.CustomerSessions.Unit.euro, false)));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellEuro(double.Parse(investment));
                ChangeLineType(ref lineType);
            }

            //Nombre de produits
            lineIndex = resultTable.AddNewLine(lineType);
            resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1393, _webSession.SiteLanguage));
            resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(tab[0].ToString()));
            ChangeLineType(ref lineType);

            if (_vehicle != DBClassificationConstantes.Vehicles.names.outdoor) {
                //Nombre de nouveaux produits dans la pige
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1394, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(tab[2].ToString()));
                ChangeLineType(ref lineType);

                //Nombre de nouveaux produits dans le support
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1395, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(tab[1].ToString()));
                ChangeLineType(ref lineType);
            }
            //Nombre d'annonceurs
            lineIndex = resultTable.AddNewLine(lineType);
            resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1396, _webSession.SiteLanguage));
            resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(tab[3].ToString()));
            ChangeLineType(ref lineType);

            // Cas tv, presse
            if (_vehicle == DBClassificationConstantes.Vehicles.names.radio
                || _vehicle == DBClassificationConstantes.Vehicles.names.tv
                || _vehicle == DBClassificationConstantes.Vehicles.names.others) {

                // Durée moyenne d'un écran
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1414, _webSession.SiteLanguage));
                //resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellDuration(double.Parse(Units.ConvertUnitValueAndPdmToString(((long)averageDurationEcran).ToString(), WebCst.CustomerSessions.Unit.duration, false)));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellDuration(Convert.ToDouble(((long)averageDurationEcran).ToString()));
                ChangeLineType(ref lineType);

                // Nombre moyen de spots par écran
                lineIndex = resultTable.AddNewLine(lineType);
                resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1415, _webSession.SiteLanguage));
                resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(nbrSpotByEcran.ToString("0.00"));

            }
            #endregion

            return resultTable;

        }
        #endregion

        #region Détail du portefeuille
        /// <summary>
        /// Obtient le tableau contenant l'ensemble des résultats
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Tableau de résultats</returns>
        public ResultTable GetPortofolioResultTable() {

            #region Variables
            ResultTable tab=null;
            DataTable dt =null;
            Headers headers=null;
            CellUnitFactory[] cellFactories=null;
            AdExpressCellLevel[] cellLevels;
            LineType[] lineTypes = new LineType[5] { LineType.total,LineType.level1,LineType.level2,LineType.level3,LineType.level4 };
            string[] columnsName=null;
            int iCurLine=0;
            int iNbLine=0;
            int iNbLevels=0;
            int insertions=0,creatives=0;
            
            #endregion

            // Get Data
            dt=GetDataForResultTable().Tables[0];
            if (dt == null || dt.Rows.Count == 0) {
                return null;
            }
            // Table nb lines
            iNbLine=GetPortofolioSize(dt);

            #region Initialisation du tableau de résultats
            if(_showInsertions)insertions = 1;
            if(_showCreatives) creatives=1;
            GetPortofolioHeaders(out headers,out cellFactories,out columnsName);
            tab = new ResultTable(iNbLine,headers);
            #endregion

            #region Traitement du tableau de résultats

            #region Intialisation des totaux
            iNbLevels = _webSession.GenericProductDetailLevel.GetNbLevels;
            cellLevels = new AdExpressCellLevel[iNbLevels+1];
            tab.AddNewLine(LineType.total);
            tab[iCurLine,1] = cellLevels[0] = new AdExpressCellLevel(0,GestionWeb.GetWebWord(805,_webSession.SiteLanguage),0,iCurLine,_webSession);
            //Creatives
            if(_showCreatives) tab[iCurLine,1+creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[iCurLine,1],_webSession,_webSession.GenericProductDetailLevel);
            if(_showInsertions) tab[iCurLine,1+creatives+insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[iCurLine,1],_webSession,_webSession.GenericProductDetailLevel);

            tab[iCurLine,2+creatives+insertions] = new CellMediaScheduleLink(cellLevels[0],_webSession);
            AffectPortefolioLine(cellFactories,columnsName,null,tab,iCurLine,false);
            #endregion

            int i = 1;
            long dCurLevel=0;
            foreach(DataRow row in dt.Rows) {
                //pour chaque niveau
                for(i=1;i <= iNbLevels;i++) {
                    //nouveau niveau i
                    dCurLevel = _webSession.GenericProductDetailLevel.GetIdValue(row,i);
                    if(dCurLevel > 0 && (cellLevels[i]==null || dCurLevel!=cellLevels[i].Id)) {
                        for(int j = i+1;j < cellLevels.Length;j++) {
                            cellLevels[j] = null;
                        }
                        iCurLine++;
                        tab.AddNewLine(lineTypes[i]);
                        tab[iCurLine,1] = cellLevels[i] = new AdExpressCellLevel(dCurLevel,_webSession.GenericProductDetailLevel.GetLabelValue(row,i),cellLevels[i-1],i,iCurLine,_webSession);
                        if(row.Table.Columns.Contains("id_address") && row["id_address"]!=System.DBNull.Value) {
                            cellLevels[i].AddressId = Convert.ToInt64(row["id_address"]);
                        }
                        //Creatives
                        if(creatives>0) tab[iCurLine,1+creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[iCurLine,1],_webSession,_webSession.GenericProductDetailLevel);
                        if(insertions > 0) tab[iCurLine,1+creatives+insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[iCurLine,1],_webSession,_webSession.GenericProductDetailLevel);
                        tab[iCurLine,2+creatives+insertions] = new CellMediaScheduleLink((AdExpressCellLevel)tab[iCurLine,1],_webSession);
                        //feuille ou niveau parent?
                        if(i != iNbLevels) {
                            AffectPortefolioLine(cellFactories,columnsName,null,tab,iCurLine,false);
                        }
                        else {
                            AffectPortefolioLine(cellFactories,columnsName,row,tab,iCurLine,true);
                        }
                    }
                }
            }
            #endregion

            return tab;
        }

        #endregion

        #region Calendrier d'actions
        /// <summary>
        /// Obtient le tableau contenant l'ensemble des résultats
        /// </summary>
        /// <returns>Tableau de résultats</returns>
        public ResultTable GetCalendar() {

            #region Variables
            ResultTable tab=null;
            DataSet ds  =null;
            DataTable dt =null;
            CellUnitFactory[] cellFactories=null;
            CellUnitFactory cellFactory = null;
            AdExpressCellLevel[] cellLevels;
            LineType[] lineTypes = new LineType[5] { LineType.total,LineType.level1,LineType.level2,LineType.level3,LineType.level4 };
            Headers headers = null;
            int iCurLine=0;
            int iNbLine=0;
            int iNbLevels=0;
            ArrayList parutions = new ArrayList();
            #endregion

            #region Chargement des données
            if(_module.CountryDataAccessLayer==null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters=new object[5];
            parameters[0]=_webSession;
            parameters[1]=_vehicle;
            parameters[2] = _idMedia;
            parameters[3]=_periodBeginning;
            parameters[4]=_periodEnd;
            IPortofolioDAL portofolioDAL=(IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory+@"Bin\"+_module.CountryDataAccessLayer.AssemblyName,_module.CountryDataAccessLayer.Class,false,BindingFlags.CreateInstance|BindingFlags.Instance|BindingFlags.Public,null,parameters,null,null,null);
            ds=portofolioDAL.GetDataCalendar();

            if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0) dt=ds.Tables[0];
            else return (tab);
            #endregion

            #region Nombre de lignes du tableau du tableau
            iNbLine=GetCalendarSize(dt,parutions);
            #endregion

            #region Headers
            GetCalendarHeaders(out headers, out cellFactory, parutions);
            #endregion

            #region Initialisation du tableau de résultats
            tab = new ResultTable(iNbLine,headers);
            #endregion

            #region Traitement du tableau de résultats
            int i = 1;

                #region Intialisation des totaux
                iNbLevels = _webSession.GenericProductDetailLevel.GetNbLevels;
                cellLevels = new AdExpressCellLevel[iNbLevels+1];
                tab.AddNewLine(LineType.total);
                tab[iCurLine,1] = cellLevels[0] = new AdExpressCellLevel(0,GestionWeb.GetWebWord(805,_webSession.SiteLanguage),0,iCurLine,_webSession);
                tab[iCurLine,2] = new CellMediaScheduleLink(cellLevels[0],_webSession);
                if(!_webSession.Percentage)tab[iCurLine,3] = cellFactory.Get(0.0);
                else tab[iCurLine,3] = new CellPDM(0.0,null);
                tab[iCurLine,4] = new CellPercent(0.0,null);
                for(i = 5;i < 5+parutions.Count;i++) {
                    if(!_webSession.Percentage)tab[iCurLine,i] = cellFactory.Get(0.0);
                    else tab[iCurLine,i] = new CellPDM(0.0,(CellUnit)tab[iCurLine,3]);
                }
                #endregion

                i = 1;
                long dCurLevel=0;
                DetailLevelItemInformation.Levels level;
                long lCol = -1;
                double valu = 0.0;
                foreach(DataRow row in dt.Rows) {
                    //pour chaque niveau
                    for(i=1;i <= iNbLevels;i++) {
                        //nouveau niveau i
                        dCurLevel = _webSession.GenericProductDetailLevel.GetIdValue(row,i);
                        if(dCurLevel > 0 && (cellLevels[i]==null || dCurLevel!=cellLevels[i].Id)) {
                            for (int j = i + 1; j < cellLevels.Length; j++) {
                                cellLevels[j] = null;
                            }
                            iCurLine++;
                            tab.AddNewLine(lineTypes[i]);
                            tab[iCurLine,1] = cellLevels[i] = new AdExpressCellLevel(dCurLevel,_webSession.GenericProductDetailLevel.GetLabelValue(row,i),cellLevels[i-1],i,iCurLine,_webSession);
                            if(row.Table.Columns.Contains("id_address") && row["id_address"]!=System.DBNull.Value) {
                                cellLevels[i].AddressId = Convert.ToInt64(row["id_address"]);
                            }
                            level = _webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(i);
                            //PM
                            if(level!=DetailLevelItemInformation.Levels.agency && level!=DetailLevelItemInformation.Levels.groupMediaAgency) {
                                tab[iCurLine,2] = new CellMediaScheduleLink((AdExpressCellLevel)tab[iCurLine,1],_webSession);
                            }
                            else {
                                tab[iCurLine,2] = new CellEmpty();
                            }
                            //total
                            if(!_webSession.Percentage)tab[iCurLine,3] = cellFactory.Get(0.0);
                            else tab[iCurLine,3] = new CellPDM(0.0,null); ;
                            //pourcentage
                            tab[iCurLine,4] = new CellPercent(0.0,(CellUnit)tab[cellLevels[i-1].LineIndexInResultTable,4]);
                            //initialisation des autres colonnes
                            for(int j =5;j < 5+parutions.Count;j++) {
                                if(!_webSession.Percentage)tab[iCurLine,j] = cellFactory.Get(0.0);
                                else tab[iCurLine,j] = new CellPDM(0.0,(CellUnit)tab[iCurLine,3]);
                            }
                        }
                        //feuille ou niveau parent?
                        if(i == iNbLevels) {
                            lCol = tab.GetHeadersIndexInResultTable(row["date_media_num"].ToString());
                            valu = Convert.ToDouble(row["unit"]);
                            tab.AffectValueAndAddToHierarchy(1,iCurLine,lCol,valu);
                            tab.AffectValueAndAddToHierarchy(1,iCurLine,4,valu);
                            tab.AffectValueAndAddToHierarchy(1,iCurLine,3,valu);
                        }
                    }
                }

            #endregion

            return tab;
        }
        #endregion 

        #region Détail support
        /// <summary>
        /// Créer un tableau avec pour chaque jour de la semaine
        /// l'investissement du support et le nombre de spot
        /// </summary>
        /// <returns>tableau avec pour chaque jour de la semaine
        /// l'investissement du support et le nombre de spot</returns>
        public int[,] GetFormattedTableDetailMedia(IPortofolioDAL portofolioDAL) {

            #region Variables
            int[,] tab = null;
            DataTable dt = null;
            DateTime dayDT;
            int currentLine = -1;
            int oldEcranCode = -1;
            int ecranCode;
            #endregion

            dt = portofolioDAL.GetCommercialBreakForTvRadio().Tables[0];

            #region Initialisation du tableau
            tab = new int[dt.Rows.Count, FrameWorkResultConstantes.PortofolioDetailMedia.TOTAL_INDEX];
            #endregion

            #region Parcours du tableau
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

            return tab;
        }
        #endregion		

		#region Media Insertion detail
		/// <summary>
		/// Get portofolio media detail insertions results
		/// </summary>
		/// <returns>Result table</returns>
		public virtual ResultTable GetPortofolioDetailMediaResultTable() {
			
			#region Variables
			ResultTable tab = null;
			DataSet ds;
			DataTable dt = null;
			Headers headers;
			ArrayList columnItemList;
			int iCurLine = 0;
			int iNbLine = 0;
			Assembly assembly;
			Type type;
			bool allPeriod = true;
			bool isDigitalTV = false; 
			#endregion

			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[6];
			parameters[0] = _webSession;
			parameters[1] = _vehicle;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			parameters[5] = _adBreak;
			if (_adBreak != null && _adBreak.Length > 0) allPeriod = false;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
			isDigitalTV = portofolioDAL.IsMediaBelongToCategory(_idMedia, DBCst.Category.ID_DIGITAL_TV);

			#region Product detail level (Generic)
			// Initialisation to product
			ArrayList levels = new ArrayList();
			levels.Add(10);
			_webSession.GenericProductDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
			#endregion

			#region Columns levels (Generic)
			columnItemList = TNS.AdExpress.Web.Core.PortofolioDetailMediaColumnsInformation.GetDefaultMediaDetailColumns(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);

			ArrayList columnIdList = new ArrayList();
			foreach (GenericColumnItemInformation Column in columnItemList)
				columnIdList.Add((int)Column.Id);

			_webSession.GenericInsertionColumns = new GenericColumns(columnIdList);
			_webSession.Save();
			#endregion

			#region Data loading
			try {				
				ds = portofolioDAL.GetGenericDetailMedia();				
				if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
					dt = ds.Tables[0];
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioException("Error while extracting portofolio media detail data", err));
			}
			#endregion

			#region Press and Internatioanl Press cases
			try {
				if (_vehicle == DBClassificationConstantes.Vehicles.names.press || _vehicle == DBClassificationConstantes.Vehicles.names.internationalPress)
					SetDataTable(dt, _dayOfWeek, allPeriod);
			}
			catch (System.Exception err) {
				throw (new PortofolioException("Error while deleting rows (case of press) fro portofolio media detail", err));
			}
			#endregion

			if (dt != null && dt.Rows != null && dt.Rows.Count > 0) {

				#region Rigths management des droits
				// Show creatives
				bool showCreative = _webSession.CustomerLogin.ShowCreatives(_vehicle);
				// Show media agency
				bool showMediaAgency = false;
				if (_webSession.CustomerLogin.GetFlag((long)DBCst.Flags.ID_MEDIA_AGENCY) != null && dt.Columns.Contains("advertising_agency")) {
					showMediaAgency = true;
				}
				//Show diffusion date
				bool showDate = true;
				if (!allPeriod && (_vehicle == DBClassificationConstantes.Vehicles.names.press || _vehicle == DBClassificationConstantes.Vehicles.names.internationalPress))
					showDate = false;
				#endregion

				#region Table nb rows
				iNbLine = dt.Rows.Count;
				#endregion

				#region Initialisation of result table
				try {
					headers = new Headers();
					columnItemList = TNS.AdExpress.Web.Core.PortofolioDetailMediaColumnsInformation.GetDefaultMediaDetailColumns(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);

					foreach (GenericColumnItemInformation Column in columnItemList) {

						switch (Column.Id) {
							case GenericColumnItemInformation.Columns.associatedFile://Visual radio/tv
							case GenericColumnItemInformation.Columns.visual://Visual press
								if (showCreative)
									headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
								break;
							case GenericColumnItemInformation.Columns.agenceMedia://media agency
								if (showMediaAgency)
									headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
								break;
							case GenericColumnItemInformation.Columns.planMedia://Plan media
								headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
								break;
							case GenericColumnItemInformation.Columns.dateDiffusion:
							case GenericColumnItemInformation.Columns.dateParution:
								if (showDate)
									headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
								break;
							case GenericColumnItemInformation.Columns.topDiffusion:
								if (!isDigitalTV)
									headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
								break;
							default:
								if (Column.Visible)
									headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
								break;
						}

					}

					tab = new ResultTable(iNbLine, headers);
				}
				catch (System.Exception err) {
					throw (new PortofolioException("Error while initiating headers of portofolio media detail", err));
				}
				#endregion

				#region Generation of table
				string[] files;
				string listVisual = "";
				long iCurColumn = 0;
				Cell curCell = null;
				string date = "";
				string dateMediaNum = "";
				DateTime dateMedia;

				try {

					// assembly loading
					assembly = Assembly.Load(@"TNS.FrameWork.WebResultUI");
					 
					foreach (DataRow row in dt.Rows) {

						#region Initialisation of dateMediaNum
						switch (_vehicle) {
							case DBClassificationConstantes.Vehicles.names.press:
							case DBClassificationConstantes.Vehicles.names.internationalPress:
								dateMediaNum = row["date_media_num"].ToString();
								break;
							case DBClassificationConstantes.Vehicles.names.tv:
							case DBClassificationConstantes.Vehicles.names.radio:
							case DBClassificationConstantes.Vehicles.names.others:
								dateMedia = new DateTime(int.Parse(row["date_media_num"].ToString().Substring(0, 4)), int.Parse(row["date_media_num"].ToString().Substring(4, 2)), int.Parse(row["date_media_num"].ToString().Substring(6, 2)));
								dateMediaNum = dateMedia.DayOfWeek.ToString();
								break;
						}
						#endregion

						if (_dayOfWeek == dateMediaNum || allPeriod) {

							tab.AddNewLine(LineType.level1);
							iCurColumn = 1;

							foreach (GenericColumnItemInformation Column in columnItemList) {
								switch (Column.Id) {
									case GenericColumnItemInformation.Columns.visual://Visual press
										if (showCreative) {
											if (row[Column.DataBaseField].ToString().Length > 0) {
												// Creation
												files = row[Column.DataBaseField].ToString().Split(',');
												foreach (string str in files) {
													listVisual += "/ImagesPresse/" + _idMedia + "/" + row["date_cover_num"] + "/" + str + ",";
												}
												if (listVisual.Length > 0) {
													listVisual = listVisual.Substring(0, listVisual.Length - 1);
												}
												tab[iCurLine, iCurColumn++] = new CellPressCreativeLink(listVisual, _webSession);
												listVisual = "";
											}
											else
												tab[iCurLine, iCurColumn++] = new CellPressCreativeLink("", _webSession);
										}
										break;
									case GenericColumnItemInformation.Columns.associatedFile://Visual radio/tv
										if (showCreative) {
											switch (_vehicle) {
												case DBClassificationConstantes.Vehicles.names.radio:
													tab[iCurLine, iCurColumn++] = new CellRadioCreativeLink(row[Column.DataBaseField].ToString(), _webSession);
													break;
												case DBClassificationConstantes.Vehicles.names.tv:
												case DBClassificationConstantes.Vehicles.names.others:
													if (row[Column.DataBaseField].ToString().Length > 0)
														tab[iCurLine, iCurColumn++] = new CellTvCreativeLink(Convert.ToInt64(row[Column.DataBaseField]), _webSession, _vehicle.GetHashCode());
													else
														tab[iCurLine, iCurColumn++] = new CellTvCreativeLink(-1, _webSession, _vehicle.GetHashCode());

													break;
											}
										}
										break;
									case GenericColumnItemInformation.Columns.agenceMedia://Media agncy
										if (showMediaAgency) {
											tab[iCurLine, iCurColumn++] = new CellLabel(row["advertising_agency"].ToString());
										}
										break;
									case GenericColumnItemInformation.Columns.planMedia://Plan media
										tab[iCurLine, iCurColumn++] = new CellInsertionMediaScheduleLink(_webSession, Convert.ToInt64(row["id_product"]), 1);
										break;
									case GenericColumnItemInformation.Columns.dateParution:// Parution Date and  diffusion Date
									case GenericColumnItemInformation.Columns.dateDiffusion:
										if (showDate) {
											type = assembly.GetType(Column.CellType);
											curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
											date = row[Column.DataBaseField].ToString();
											if (date.Length > 0)
												curCell.SetCellValue((object)new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2))));
											else
												curCell.SetCellValue(null);
											tab[iCurLine, iCurColumn++] = curCell;
										}
										break;
									case GenericColumnItemInformation.Columns.topDiffusion:
									case GenericColumnItemInformation.Columns.idTopDiffusion:
										if (!isDigitalTV) {
											if (row[Column.DataBaseField].ToString().Length > 0)
												tab[iCurLine, iCurColumn++] = new TNS.FrameWork.WebResultUI.CellAiredTime(Convert.ToDouble(row[Column.DataBaseField]));
											else
												tab[iCurLine, iCurColumn++] = new TNS.FrameWork.WebResultUI.CellAiredTime(0);
										}
										break;
									default:
										if (Column.Visible) {
											type = assembly.GetType(Column.CellType);
											curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
											curCell.SetCellValue(row[Column.DataBaseField]);
											tab[iCurLine, iCurColumn++] = curCell;
										}
										break;
								}
							}
							iCurLine++;
						}

					}
				}
				catch (System.Exception err) {
					throw (new PortofolioException("Error while generating result table of portofolio media detail", err));
				}
				#endregion

			}

			return tab;
		}

		#endregion

		#endregion

		#region Get Data
		/// <summary>
        /// Get data for ResultTable result
        /// </summary>
        /// <returns></returns>
        protected DataSet GetDataForResultTable() {
            DataSet ds=null;

            switch(_webSession.CurrentModule) {
                case WebCst.Module.Name.ALERTE_PORTEFEUILLE:
                    if(_module.CountryDataAccessLayer==null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
                    object[] parameters=new object[5];
                    parameters[0]=_webSession;
                    parameters[1]=_vehicle;
                    parameters[2] = _idMedia;
                    parameters[3]=_periodBeginning;
                    parameters[4]=_periodEnd;
                    IPortofolioDAL portofolioDAL=(IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory+@"Bin\"+_module.CountryDataAccessLayer.AssemblyName,_module.CountryDataAccessLayer.Class,false,BindingFlags.CreateInstance|BindingFlags.Instance|BindingFlags.Public,null,parameters,null,null,null);
                    //Portofolio.IResults result=(Portofolio.IResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory+@"Bin\"+module.CountryRulesLayer.AssemblyName,module.CountryRulesLayer.Class);
                    ds=portofolioDAL.GetMediaPortofolio();
                    //ds=PortofolioDataAccess.GetData(_webSession,vehicle,_module.ModuleType,periodBeginning,_periodEnd);
                    break;
                case WebCst.Module.Name.ANALYSE_PORTEFEUILLE:
                    //ds = PortofolioAnalysisDataAccess.GetGenericData(_webSession,vehicleName);
                    break;
                default:
                    throw (new PortofolioException("Invalid module"));
            }
            
            return (ds);
        }
        #endregion

        #region Methods

        #region Affect Portefolio Line
        /// <summary>
        /// Affect Portefolio Line
        /// </summary>
        /// <param name="tCellFactories">Cell Factory</param>
        /// <param name="columnsName">Column names table</param>
        /// <param name="dr">DataRow</param>
        /// <param name="oTab">Result table</param>
        /// <param name="iLineIndex">Line Index</param>
        /// <param name="isLeaf">Is Leaf</param>
        protected void AffectPortefolioLine(CellUnitFactory[] tCellFactories,string[] columnsName,DataRow dr,ResultTable oTab,int iLineIndex,bool isLeaf) {

            for(int i = 0;i < tCellFactories.Length;i++) {
                if(tCellFactories[i] != null) {
                    oTab[iLineIndex,i+1] = tCellFactories[i].Get(0.0);
                    if(dr != null) {
                        if(isLeaf) {
                            oTab.AffectValueAndAddToHierarchy(1,iLineIndex,i+1,Convert.ToDouble(dr[columnsName[i]]));
                        }
                    }
                }
            }
        } 
        #endregion

        #region Headers
        
        #region Portofolio headers
        /// <summary>
        /// Portofolio Headers and Cell factory
        /// </summary>
        /// <returns></returns>
        protected void GetPortofolioHeaders(out Headers headers,out CellUnitFactory[] cellFactories,out string[] columnsName) {
            int insertions = 0;
            int creatives =0;
            int iNbCol=0;

            headers = new TNS.FrameWork.WebResultUI.Headers();
            // Product column
            headers.Root.Add(new Header(true,GestionWeb.GetWebWord(PROD_COL,_webSession.SiteLanguage),PROD_COL));
            // Creatives column     
            if (_showCreatives) {
                headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(CREATIVES_COL, _webSession.SiteLanguage), CREATIVES_COL));
                creatives = 1;
            }
            // Insertions column
            if(_showInsertions) {
                headers.Root.Add(new HeaderCreative(false,GestionWeb.GetWebWord(INSERTIONS_LIST_COL,_webSession.SiteLanguage),INSERTIONS_LIST_COL));
                insertions = 1;
            }
            
            // Media schedule column
            headers.Root.Add(new HeaderMediaSchedule(false,GestionWeb.GetWebWord(PM_COL,_webSession.SiteLanguage),PM_COL));

            headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(EUROS_COL,_webSession.SiteLanguage),EUROS_COL));
            switch(_vehicle) {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(MM_COL,_webSession.SiteLanguage),MM_COL));
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(PAGE_COL,_webSession.SiteLanguage),PAGE_COL));
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(INSERTIONS_COL,_webSession.SiteLanguage),INSERTIONS_COL));
                    iNbCol = 6+creatives+insertions;
                    cellFactories = new CellUnitFactory[iNbCol];
                    columnsName = new string[iNbCol];
                    columnsName[3+creatives+insertions] = "mmpercol";
                    columnsName[4+creatives+insertions] = "pages";
                    columnsName[5+creatives+insertions] = "insertion";
                    cellFactories[3+creatives+insertions] = new CellUnitFactory(new CellMMC(0.0));
                    cellFactories[4+creatives+insertions] = new CellUnitFactory(new CellPage(0.0));
                    cellFactories[5+creatives+insertions] = new CellUnitFactory(new CellInsertion(0.0));
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing:
                    if(_webSession.CustomerLogin.GetFlag(TNS.AdExpress.Constantes.DB.Flags.ID_VOLUME_MARKETING_DIRECT) != null) {
                        headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(VOLUME,_webSession.SiteLanguage),VOLUME));
                        iNbCol = 4 + creatives+insertions;
                        cellFactories = new CellUnitFactory[iNbCol];
                        columnsName = new string[iNbCol];
                        columnsName[3 + creatives+insertions] = "volume";
                        cellFactories[3 + creatives+insertions] = new CellUnitFactory(new CellVolume(0.0));
                    }
                    else {
                        iNbCol = 3 + creatives+insertions;
                        cellFactories = new CellUnitFactory[iNbCol];
                        columnsName = new string[iNbCol];
                    }
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(DURATION_COL,_webSession.SiteLanguage),DURATION_COL));
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(SPOTS_COL,_webSession.SiteLanguage),SPOTS_COL));
                    iNbCol = 5+creatives+insertions;
                    columnsName = new string[iNbCol];
                    columnsName[3+creatives+insertions] = "duration";
                    columnsName[4+creatives+insertions] = "insertion";
                    cellFactories = new CellUnitFactory[iNbCol];
                    cellFactories[3+creatives+insertions] = new CellUnitFactory(new CellDuration(0.0));
                    cellFactories[4+creatives+insertions] = new CellUnitFactory(new CellNumber(0.0));
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(PAN_COL,_webSession.SiteLanguage),PAN_COL));
                    iNbCol = 4+creatives+insertions;
                    columnsName = new string[iNbCol];
                    columnsName[3+creatives+insertions] = "insertion";
                    cellFactories = new CellUnitFactory[iNbCol];
                    cellFactories[3+creatives+insertions] = new CellUnitFactory(new CellNumber(0.0));
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
                    iNbCol = 3+creatives+insertions;
                    columnsName = new string[iNbCol];
                    cellFactories = new CellUnitFactory[iNbCol];
                    break;
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
            cellFactories[0] = null;
            cellFactories[1] = null;
            if(_showCreatives) columnsName[1+creatives] = null;
            if(_showInsertions) columnsName[1+creatives+insertions] = null;
            columnsName[2+creatives+insertions] = "euro";
            cellFactories[2+creatives+insertions] = new CellUnitFactory(new CellEuro(0.0));
        }
        #endregion

        #region Calendar headers
        /// <summary>
        /// Calendar Headers and Cell factory
        /// </summary>
        /// <returns></returns>
        protected void GetCalendarHeaders(out Headers headers, out CellUnitFactory cellFactory, ArrayList parutions) {

            headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(PROD_COL, _webSession.SiteLanguage), PROD_COL));
            headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL, _webSession.SiteLanguage), PM_COL));
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(TOTAL_COL, _webSession.SiteLanguage), TOTAL_COL));
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(POURCENTAGE_COL, _webSession.SiteLanguage), POURCENTAGE_COL));

            //une colonne par date de parution
            parutions.Sort();
            foreach (Int32 parution in parutions) {
                headers.Root.Add(new Header(true, DateString.YYYYMMDDToDD_MM_YYYY(parution.ToString(), _webSession.SiteLanguage), (long)parution));
            }
            if (!_webSession.Percentage) {
                switch (_webSession.Unit) {
                    case WebCst.CustomerSessions.Unit.duration:
                        cellFactory = new CellUnitFactory(new CellDuration(0.0));
                        break;
                    case WebCst.CustomerSessions.Unit.euro:
                        cellFactory = new CellUnitFactory(new CellEuro(0.0));
                        break;
                    case WebCst.CustomerSessions.Unit.kEuro:
                        cellFactory = new CellUnitFactory(new CellKEuro(0.0));
                        break;
                    case WebCst.CustomerSessions.Unit.insertion:
                        cellFactory = new CellUnitFactory(new CellInsertion(0.0));
                        break;
                    case WebCst.CustomerSessions.Unit.pages:
                        cellFactory = new CellUnitFactory(new CellPage(0.0));
                        break;
                    case WebCst.CustomerSessions.Unit.mmPerCol:
                        cellFactory = new CellUnitFactory(new CellMMC(0.0));
                        break;
                    default:
                        cellFactory = new CellUnitFactory(new CellNumber(0.0));
                        break;
                }
            }
            else {
                cellFactory = new CellUnitFactory(new CellPDM(0.0));
            }
        }
        #endregion

        #endregion

        #region Dates
        /// <summary>
        /// Get begin date for the 2 module types
        /// - Portofolio Alert
        /// - Portofolio analysis
        /// </summary>
        /// <returns>Begin date</returns>
        protected string GetDateBegin() {
            switch(_webSession.CurrentModule) {
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE:
                    return (Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate,_webSession.PeriodType).ToString("yyyyMMdd"));
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                    return (_webSession.PeriodBeginningDate);
            }
            return (null);
        }

        /// <summary>
        /// Get ending date for the 2 module types
        /// </summary>
        /// - Portofolio Alert
        /// - Portofolio analysis
        /// <returns>Ending date</returns>
        protected string GetDateEnd() {
            switch(_webSession.CurrentModule) {
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE:
                    return (Dates.getPeriodEndDate(_webSession.PeriodEndDate,_webSession.PeriodType).ToString("yyyyMMdd"));
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                    return (_webSession.PeriodEndDate);
            }
            return (null);
        }
        #endregion

        #region Vehicle Selection
        /// <summary>
        /// Get Vehicle Selection
        /// </summary>
        /// <returns>Vehicle label</returns>
        protected string GetVehicle() {
            string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
            if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw (new PortofolioException("The media selection is invalid"));
            return (vehicleSelection);
        }
        /// <summary>
        /// Get vehicle selection
        /// </summary>
        /// <returns>Vehicle</returns>
        protected DBClassificationConstantes.Vehicles.names GetVehicleName() {
            try {
                return ((DBClassificationConstantes.Vehicles.names)int.Parse(GetVehicle()));
            }
            catch(System.Exception err) {
                throw (new PortofolioException("Impossible to retreive vehicle selection"));
            }
        }
        #endregion

        #region Media Selection
        /// <summary>
        /// Get Media Id
        /// </summary>
        /// <returns>Media Id</returns>
        protected Int64 GetMediaId() {
            try {
                return (((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID);
            }
            catch (System.Exception err) {
                throw (new PortofolioException("Impossible to retrieve media id"));
            }
        }
        #endregion

        #region Get lines number

        #region Media Portofolio
        /// <summary>
        /// Get lines number for the portofolio result
        /// </summary>
        /// <param name="dt">Data table</param>
        /// <returns>Lines number</returns>
        protected virtual int GetPortofolioSize(DataTable dt) {

            #region Variables
            Int64 OldL1Id=0;
            Int64 cL1Id=0;
            Int64 nbL1Id=0;
            Int64 OldL2Id=0;
            Int64 cL2Id=0;
            Int64 nbL2Id=0;
            Int64 OldL3Id=0;
            Int64 cL3Id=0;
            Int64 nbL3Id=0;
            Int64 nbLine=0;
            #endregion

            if(dt!=null && dt.Rows.Count>0) {
                foreach(DataRow dr in dt.Rows) {
                    cL1Id = _webSession.GenericProductDetailLevel.GetIdValue(dr,1);
                    if(cL1Id > 0 && cL1Id!=OldL1Id) {
                        nbL1Id++;
                        OldL1Id=cL1Id;
                        OldL2Id=OldL3Id=-1;
                    }
                    cL2Id = _webSession.GenericProductDetailLevel.GetIdValue(dr,2);
                    if(cL2Id>0 && OldL2Id!=cL2Id) {
                        nbL2Id++;
                        OldL2Id=cL2Id;
                        OldL3Id=-1;
                    }
                    cL3Id = _webSession.GenericProductDetailLevel.GetIdValue(dr,3);
                    if(cL3Id>0 && OldL3Id!=cL3Id) {
                        nbL3Id++;
                        OldL3Id=cL3Id;
                    }
                }
            }
            if((nbL1Id>0) || (nbL2Id>0) || (nbL3Id>0)) {
                nbLine=nbL1Id+nbL2Id+nbL3Id+1;
            }
            return (int)nbLine;
        }
        #endregion

        #region GetCalendarSize
        /// <summary>
        /// Calcul la taille du tableau de résultats d'un calendrier d'action
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="dt">table de données</param>
        /// <returns>nombre de ligne du tableau de résultats</returns>
        /// <param name="parutions">Parutions</param>
        protected virtual int GetCalendarSize(DataTable dt,ArrayList parutions) {

            #region Variable
            Int64 OldL1Id=0;
            Int64 cL1Id=0;
            Int64 nbL1Id=0;
            Int64 OldL2Id=0;
            Int64 cL2Id=0;
            Int64 nbL2Id=0;
            Int64 OldL3Id=0;
            Int64 cL3Id=0;
            Int64 nbL3Id=0;
            Int64 nbLine=0;
            #endregion

            foreach(DataRow dr in dt.Rows) {
                cL1Id = _webSession.GenericProductDetailLevel.GetIdValue(dr,1);
                if(cL1Id > 0 && cL1Id!=OldL1Id) {
                    nbL1Id++;
                    OldL1Id=cL1Id;
                    OldL2Id=OldL3Id=-1;
                }
                cL2Id = _webSession.GenericProductDetailLevel.GetIdValue(dr,2);
                if(cL2Id>0 && OldL2Id!=cL2Id) {
                    nbL2Id++;
                    OldL2Id=cL2Id;
                    OldL3Id=-1;
                }
                cL3Id = _webSession.GenericProductDetailLevel.GetIdValue(dr,3);
                if(cL3Id>0 && OldL3Id!=cL3Id) {
                    nbL3Id++;
                    OldL3Id=cL3Id;
                }
                if(!parutions.Contains(dr["date_media_num"])) {
                    parutions.Add(dr["date_media_num"]);
                }
            }

            if((nbL1Id>0) || (nbL2Id>0) || (nbL3Id>0)) {
                nbLine=nbL1Id+nbL2Id+nbL3Id+1;
            }
            return (int)nbLine;
        }
        #endregion

        #endregion

        #region Insertion and Creations
        /// <summary>
        /// Determine if the result shows the insertion column
        /// </summary>
        /// <returns>True if the Insertion column is shown</returns>
        protected virtual bool ShowInsertions() {
            foreach(DetailLevelItemInformation item in _webSession.GenericProductDetailLevel.Levels) {
                if(_module.ModuleType==WebCst.Module.Type.alert &&
					(item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
					|| item.Id.Equals(DetailLevelItemInformation.Levels.product))) {
                    return (true);
                }
            }
            return (false);
        }
        /// <summary>
        /// Determine if the result shows the creation column
        /// </summary>
        /// <returns>True if the creation column is shown</returns>
        protected virtual bool ShowCreatives() {
            foreach(DetailLevelItemInformation item in _webSession.GenericProductDetailLevel.Levels) {
                if(_module.ModuleType==WebCst.Module.Type.alert &&
                    _webSession.CustomerLogin.GetFlag(DBCst.Flags.ID_SLOGAN_ACCESS_FLAG)!=null &&
					(item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
					|| item.Id.Equals(DetailLevelItemInformation.Levels.product))) {
                    return (true);
                }
            }
            return (false);
        }
        #endregion

        #region Change line type
        /// <summary>
        /// Change line type
        /// </summary>
        /// <param name="lineType">Line Type</param>
        /// <returns>Line type</returns>
        protected virtual void ChangeLineType(ref LineType lineType) {

            if (lineType == LineType.level1)
                lineType = LineType.level2;
            else 
                lineType = LineType.level1;

        }
        #endregion

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

        #region Get All Period Insertions
        /// <summary>
        /// Affiche le lien vers le détail des insertions sur la période sélectionnée.
        /// </summary>
        /// <param name="t">Constructeur du lien</param>
        /// <param name="linkText">texte du lien</param>
        /// <param name="webSession">session client</param>
        /// <param name="idMedia">identifiant support</param>
        protected virtual void GetAllPeriodInsertions(StringBuilder t, string linkText) {
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

		#region Compute DataTable for Press and   interntional Press
		/// <summary>
		/// Adapte data for vehcilce Press and interntional press
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <returns>DataTable</returns>
		protected virtual void SetDataTable(DataTable dt, string dayOfWeek, bool allPeriod) {

			#region Variables
			Int64 idOldLine = -1;
			Int64 idLine = -1;
			DataRow oldRow = null;
			int iLine = 0;
			ArrayList indexLines = new ArrayList();
			#endregion

			if (dt != null && dt.Rows != null && dt.Rows.Count > 0) {

				#region Parcours du tableau
				foreach (DataRow row in dt.Rows) {

					if (dayOfWeek == row["date_media_num"].ToString() || allPeriod) {

						idLine = (long)row["id_advertisement"];

						if (idLine != idOldLine) {
							idOldLine = idLine;
							oldRow = row;
						}
						else {
							if (oldRow["location"].ToString().Length > 0 && row["location"].ToString().Length > 0)
								oldRow["location"] = oldRow["location"].ToString() + "-" + row["location"].ToString();
							else if (oldRow["location"].ToString().Length == 0 && row["location"].ToString().Length > 0)
								oldRow["location"] = row["location"].ToString();
							indexLines.Add(iLine);
						}
					}

					iLine++;
				}
				#endregion

				indexLines.Reverse();
				//suppress rows
				foreach (int index in indexLines)
					dt.Rows.Remove(dt.Rows[index]);

			}
		}
		#endregion

		#region Compute Press structure results

		
		/// <summary>
		///Compute Press structure results	
		/// </summary>		
		/// <param name="dtFormat">Format data table</param>
		/// <param name="dtColor">Color data table</param>
		/// <param name="dtLocation">Location data table</param>
		/// <param name="dtInsert">Inset data table</param>
		public virtual void GetStructPressResult(out DataTable dtFormat, out DataTable dtColor, out DataTable dtLocation, out DataTable dtInsert) {

			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicle;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

			GetStructPressResult(portofolioDAL, out dtFormat, out dtColor, out dtLocation, out dtInsert);
		}
		/// <summary>
		///Compute Press structure results	
		/// </summary>		
		/// <param name="dtFormat">Format data table</param>
		/// <param name="dtColor">Color data table</param>
		/// <param name="dtLocation">Location data table</param>
		/// <param name="dtInsert">Inset data table</param>
		protected virtual void GetStructPressResult(IPortofolioDAL portofolioDAL,out DataTable dtFormat, out DataTable dtColor, out DataTable dtLocation, out DataTable dtInsert) {		

			#region Variables
			DataSet ds = null;
			dtFormat = null;
			dtColor = null;
			dtInsert = null;
			dtLocation = null;
			#endregion

			#region Build results tables

				//Format
			ds = portofolioDAL.GetPressStructData(FrameWorkResultConstantes.PortofolioStructure.Ventilation.format);
			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
				dtFormat = ds.Tables[0];
			//Color
			ds = portofolioDAL.GetPressStructData(FrameWorkResultConstantes.PortofolioStructure.Ventilation.color);
			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
				dtColor = ds.Tables[0];
			//Location
			ds = portofolioDAL.GetPressStructData(FrameWorkResultConstantes.PortofolioStructure.Ventilation.location);
			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
				dtLocation = ds.Tables[0];
			//Inset
			ds = portofolioDAL.GetPressStructData(FrameWorkResultConstantes.PortofolioStructure.Ventilation.insert);
			if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
				dtInsert = ds.Tables[0];
			#endregion

		}		
		#endregion

		#region Get Press structure Html
		/// <summary>
		///  Get html code for press structure result table
		/// </summary>
		/// <param name="dtFormat">Data table for Format</param>
		/// <param name="dtColor">Data table for Color</param>
		/// <param name="dtInsert">Data table for Inset</param>
		/// <param name="dtLocation">Data table for Location</param>
		/// <param name="classCss">Css class</param>
		/// <returns> Html code</returns>
		protected virtual string GetPressStructureHtml(DataTable dtFormat, DataTable dtColor, DataTable dtInsert, DataTable dtLocation, string classCss) {

			StringBuilder t = new StringBuilder(3000);

			t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 >");
			//Format
			if (dtFormat != null && dtFormat.Rows.Count > 0)
				t.Append(GetVentilationLines(dtFormat, 1420, classCss, FrameWorkResultConstantes.PortofolioStructure.Ventilation.format));
			//Color
			if (dtColor != null && dtColor.Rows.Count > 0)
				t.Append(GetVentilationLines(dtColor, 1438, classCss, FrameWorkResultConstantes.PortofolioStructure.Ventilation.color));
			//Location
			if (dtLocation != null && dtLocation.Rows.Count > 0) {
				t.Append(GetVentilationLines(dtLocation, 1439, classCss, FrameWorkResultConstantes.PortofolioStructure.Ventilation.location));
			}
			//Inset
			if (dtInsert != null && dtInsert.Rows.Count > 0)
				t.Append(GetVentilationLines(dtInsert, 1440, classCss, FrameWorkResultConstantes.PortofolioStructure.Ventilation.insert));
			t.Append("</table>");

			return t.ToString();
		}

		/// <summary>
		/// Get html lines for Format, Color, Location,Inset
		/// </summary>
		/// <param name="dt">Data table</param>
		/// <param name="labelcode">Ventilation label code</param>
		/// <param name="classCss">Css Class</param>
		/// <param name="ventilation">Format, Color, Location,Inset</param>
		/// <returns>Html Code lines</returns>
		protected virtual string GetVentilationLines(DataTable dt, int labelcode, string classCss, FrameWorkResultConstantes.PortofolioStructure.Ventilation ventilation) {
			StringBuilder t = new StringBuilder(5000);
			string ventilationType = "";
			switch (ventilation) {
				case FrameWorkResultConstantes.PortofolioStructure.Ventilation.color:
					ventilationType = "color";
					break;
				case FrameWorkResultConstantes.PortofolioStructure.Ventilation.format:
					ventilationType = "format";
					break;
				case FrameWorkResultConstantes.PortofolioStructure.Ventilation.insert:
					ventilationType = "inset";
					break;
				case FrameWorkResultConstantes.PortofolioStructure.Ventilation.location:
					ventilationType = "location";
					break;
				default:
					throw new PortofolioException("GetVentilationLines: Ventilation type unknown.");
			}
			//labels
			t.Append("\r\n\t<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV3';\"  class=\"violetBackGroundV3\" height=\"20px\" >");
			t.Append("\r\n\t<td align=\"left\" class=\"p2\" nowrap><b>" + GestionWeb.GetWebWord(labelcode, _webSession.SiteLanguage) + "</b></td>");
			t.Append("\r\n\t<td  class=\"p2\"  nowrap>" + GestionWeb.GetWebWord(1398, _webSession.SiteLanguage) + "</td>");
			t.Append("</tr>");
			//Nb insertion
			foreach (DataRow dr in dt.Rows) {
				t.Append("\r\n\t<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV3';\"  class=\"violetBackGroundV3\" height=\"20px\" >");
				if (dr[ventilationType] != null)
					t.Append("\r\n\t<td align=\"left\" class=\"" + classCss + "\" nowrap>&nbsp;&nbsp;&nbsp;" + dr[ventilationType].ToString() + "</td>");
				else t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>&nbsp;</td>");
				if (dr["insertion"] != null) {
					t.Append("\r\n\t<td align=\"right\" class=\"" + classCss + "\" nowrap>" + WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["insertion"].ToString(), WebCst.CustomerSessions.Unit.insertion, false) + "</td>");
				}
				else t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>&nbsp;</td>");
				t.Append("</tr>");
			}
			return t.ToString();
		}
		#endregion

		#region Tableau de résultats pour radio
		/// <summary>
		/// Get structure radio result
		/// </summary>
		/// <returns>result table</returns>
		public virtual object[,] GetStructRadio() {
			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicle;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

			return GetStructRadio(portofolioDAL);
	
		}
		/// <summary>
		/// Obtient le tableau de donnée des informations synthétiques (structure)
		/// pour la radio
		/// </summary>
		/// <param name="portofolioDAL">Portofolio data layer</param>
		protected virtual object[,] GetStructRadio(IPortofolioDAL portofolioDAL) {

			#region Variables
			object[,] tab = null;
			double totalEuros = 0;
			double totalSpot = 0;
			double totalDuration = 0;
			DataSet ds5_7 = null, ds7_9 = null, ds9_13 = null, ds13_19 = null, ds19_24 = null;
			int nbLine = 1, lineIndex = 1;
			#endregion

			#region Construction du tab
			//Radio	
			//5h-7h
			ds5_7 = portofolioDAL.GetStructData( 50000, 70000);
			if (ds5_7 != null && ds5_7.Tables[0].Rows.Count == 1 && IsRowNull(ds5_7)) nbLine++;
			//7h-9h
			ds7_9 = portofolioDAL.GetStructData( 70000, 90000);
			if (ds7_9 != null && ds7_9.Tables[0].Rows.Count == 1 && IsRowNull(ds7_9)) nbLine++;

			//9h-13h
			ds9_13 = portofolioDAL.GetStructData(90000, 130000);
			if (ds9_13 != null && ds9_13.Tables[0].Rows.Count == 1 && IsRowNull(ds9_13)) nbLine++;

			//13h-19h			
			ds13_19 = portofolioDAL.GetStructData(130000, 190000);
			if (ds13_19 != null && ds13_19.Tables[0].Rows.Count == 1 && IsRowNull(ds13_19)) nbLine++;
			//19h-24h
			ds19_24 = portofolioDAL.GetStructData(190000, 240000);
			if (ds19_24 != null && ds9_13.Tables[0].Rows.Count == 1 && IsRowNull(ds19_24)) nbLine++;

			if (nbLine > 1) {
				tab = new object[nbLine, FrameWorkResultConstantes.PortofolioStructure.RADIO_TV_NB_MAX_COLUMNS];

				if (ds5_7 != null && ds5_7.Tables[0].Rows.Count == 1 && IsRowNull(ds5_7)) {
					FillTab(ref tab, ds5_7, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, "5H - 7H");
					lineIndex++;
				}
				if (ds7_9 != null && ds7_9.Tables[0].Rows.Count == 1 && IsRowNull(ds7_9)) {
					FillTab(ref tab, ds7_9, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, "7H - 9H");
					lineIndex++;
				}
				if (ds9_13 != null && ds9_13.Tables[0].Rows.Count == 1 && IsRowNull(ds9_13)) {
					FillTab(ref tab, ds9_13, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, "9H - 13H");
					lineIndex++;
				}
				if (ds13_19 != null && ds13_19.Tables[0].Rows.Count == 1) {
					FillTab(ref tab, ds13_19, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, "13H - 19H");
					lineIndex++;
				}
				if (ds19_24 != null && ds9_13.Tables[0].Rows.Count == 1 && IsRowNull(ds19_24)) {
					FillTab(ref tab, ds19_24, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, "19H - 24H");
					lineIndex++;
				}

				//total
				if (tab != null && tab.GetLength(0) != 0) {
					tab[0, FrameWorkResultConstantes.PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX] = GestionWeb.GetWebWord(1401, _webSession.SiteLanguage).ToString();
					tab[0, FrameWorkResultConstantes.PortofolioStructure.EUROS_COLUMN_INDEX] = totalEuros.ToString();
					tab[0, FrameWorkResultConstantes.PortofolioStructure.SPOT_COLUMN_INDEX] = totalSpot.ToString();
					tab[0, FrameWorkResultConstantes.PortofolioStructure.DURATION_COLUMN_INDEX] = totalDuration.ToString();
				}
			}


			#endregion

			return tab;
		}
		#endregion

		#region Tableau de résultats pour télé
		/// <summary>
		/// Obtient le tableau de donnée des informations synthétiques (structure)
		/// pour la TV
		/// </summary>	
		/// <param name="portofolioDAL">Portofolio data layer</param>
		public virtual object[,] GetStructTV() {
			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicle;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

			return GetStructTV(portofolioDAL);
		}
		/// <summary>
		/// Obtient le tableau de donnée des informations synthétiques (structure)
		/// pour la TV
		/// </summary>	
		/// <param name="portofolioDAL">Portofolio data layer</param>
		protected virtual object[,] GetStructTV(IPortofolioDAL portofolioDAL) {

			#region Variables
			object[,] tab = null;
			double totalEuros = 0;
			double totalSpot = 0;
			double totalDuration = 0;
			DataSet ds7_9 = null, ds12_14 = null, ds14_17 = null, ds17_19 = null, ds19_22 = null, ds22_24 = null, ds0_7 = null;		
			int nbLine = 1,lineIndex = 1;			
			#endregion

			#region Construction du tab
			//Radio	
			//7h-12h
			ds7_9 = portofolioDAL.GetStructData(70000, 120000);
			if (ds7_9 != null && ds7_9.Tables[0].Rows.Count == 1 && IsRowNull(ds7_9)) nbLine++;
			//12h-14h
			ds12_14 = portofolioDAL.GetStructData(120000, 140000);
			if (ds12_14 != null && ds12_14.Tables[0].Rows.Count == 1 && IsRowNull(ds12_14)) nbLine++;
			//14h-17h
			ds14_17 = portofolioDAL.GetStructData(140000, 170000);
			if (ds14_17 != null && ds14_17.Tables[0].Rows.Count == 1 && IsRowNull(ds14_17)) nbLine++;
			//17h-19h
			ds17_19 = portofolioDAL.GetStructData( 170000, 190000);
			if (ds17_19 != null && ds17_19.Tables[0].Rows.Count == 1 && IsRowNull(ds17_19)) nbLine++;
			//19h-22h
			ds19_22 = portofolioDAL.GetStructData(190000, 220000);
			if (ds19_22 != null && ds19_22.Tables[0].Rows.Count == 1 && IsRowNull(ds19_22)) nbLine++;
			//22h-24h
			ds22_24 = portofolioDAL.GetStructData( 220000, 240000);
			if (ds22_24 != null && ds22_24.Tables[0].Rows.Count == 1 && IsRowNull(ds22_24)) nbLine++;
			//0h-7h
			ds0_7 = portofolioDAL.GetStructData( 0, 70000);
			if (ds0_7 != null && ds0_7.Tables[0].Rows.Count == 1 && IsRowNull(ds0_7)) nbLine++;

			if (nbLine > 1) {
				tab = new object[nbLine, FrameWorkResultConstantes.PortofolioStructure.RADIO_TV_NB_MAX_COLUMNS];

				if (ds7_9 != null && ds7_9.Tables[0].Rows.Count == 1 && IsRowNull(ds7_9)) {
					FillTab(ref tab, ds7_9, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, " 7 H - 12 H ");
					lineIndex++;
				}
				if (ds12_14 != null && ds12_14.Tables[0].Rows.Count == 1 && IsRowNull(ds12_14)) {
					FillTab(ref tab, ds12_14, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, " 12 H - 14 H");
					lineIndex++;
				}
				if (ds14_17 != null && ds14_17.Tables[0].Rows.Count == 1 && IsRowNull(ds14_17)) {
					FillTab(ref tab, ds14_17, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, " 14 H - 17 H");
					lineIndex++;
				}
				if (ds17_19 != null && ds17_19.Tables[0].Rows.Count == 1 && IsRowNull(ds17_19)) {
					FillTab(ref tab, ds17_19, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, " 17 H - 19 H");
					lineIndex++;
				}
				if (ds19_22 != null && ds19_22.Tables[0].Rows.Count == 1 && IsRowNull(ds19_22)) {
					FillTab(ref tab, ds19_22, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, " 19 H - 22 H ");
					lineIndex++;
				}
				if (ds22_24 != null && ds22_24.Tables[0].Rows.Count == 1 && IsRowNull(ds22_24)) {
					FillTab(ref tab, ds22_24, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, " 22 H - 24 H ");
					lineIndex++;
				}
				if (ds0_7 != null && ds0_7.Tables[0].Rows.Count == 1 && IsRowNull(ds0_7)) {
					FillTab(ref tab, ds0_7, lineIndex, ref totalEuros, ref totalSpot, ref totalDuration, " 0 H - 7 H ");
					lineIndex++;
				}

				//total
				if (tab != null && tab.GetLength(0) != 0) {
					tab[0, FrameWorkResultConstantes.PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX] = GestionWeb.GetWebWord(1401, _webSession.SiteLanguage).ToString();
					tab[0, FrameWorkResultConstantes.PortofolioStructure.EUROS_COLUMN_INDEX] = totalEuros.ToString();
					tab[0, FrameWorkResultConstantes.PortofolioStructure.SPOT_COLUMN_INDEX] = totalSpot.ToString();
					tab[0, FrameWorkResultConstantes.PortofolioStructure.DURATION_COLUMN_INDEX] = totalDuration.ToString();
				}
			}

			#endregion

			return tab;
		}
		#endregion

		#region Euros, spot et durée pour structure
		/// <summary>
		/// Remplit le tableau de résultats avec euros,spot et durée.
		/// </summary>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="ds">groupe de données</param>
		/// <param name="lineIndex">line courante du tableau de résultat</param>
		/// <param name="totalEuros">total euros</param>
		/// <param name="totalSpot">total spot</param>
		/// <param name="totalDuration">total durée</param>
		/// <param name="timeSpan">timeSpan</param>
		protected virtual void FillTab(ref object[,] tab, DataSet ds, int lineIndex, ref double totalEuros, ref double totalSpot, ref double totalDuration, string timeSpan) {
			DataRow dr = null;
			if (tab != null && tab.GetLength(0) > 0 && ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 1) {
				dr = ds.Tables[0].Rows[0];
				//euros
				if (dr["euros"] != System.DBNull.Value) {
					tab[lineIndex, FrameWorkResultConstantes.PortofolioStructure.EUROS_COLUMN_INDEX] = dr["euros"].ToString();
					totalEuros += double.Parse(dr["euros"].ToString());
				}
				//spot
				if (dr["spot"] != System.DBNull.Value) {
					tab[lineIndex, FrameWorkResultConstantes.PortofolioStructure.SPOT_COLUMN_INDEX] = dr["spot"].ToString();
					totalSpot += double.Parse(dr["spot"].ToString());
				}
				//durée
				if (dr["duration"] != System.DBNull.Value) {
					tab[lineIndex, FrameWorkResultConstantes.PortofolioStructure.DURATION_COLUMN_INDEX] = dr["duration"].ToString();
					totalDuration += double.Parse(dr["duration"].ToString());
				}
				//intervalle horaire
				if (((dr["euros"] != System.DBNull.Value) || (dr["spot"] != System.DBNull.Value) || (dr["duration"] != System.DBNull.Value)) && (timeSpan != null && timeSpan.Length>0))
					tab[lineIndex, FrameWorkResultConstantes.PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX] = timeSpan.ToString();
			}
		}
		#endregion

		/// <summary>
		/// Génère le code HTML du tableau de structure pour la télé et la radio
		/// </summary>		
		/// <param name="tab">tableau à afficher</param>		
		/// <param name="classCss">classe de style</param>
		/// <returns>code html du tableau à afficher</returns>
		protected virtual string GetStructureHtml( object[,] tab, string classCss) {

			StringBuilder t = new StringBuilder(5000);
			string P2 = "p2";
			string backGround = "whiteBackGround";

			t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 >");

			#region libellés colonnes
			// Première ligne
			t.Append("\r\n\t<tr height=\"20px\" >");
			if (_vehicle == DBClassificationConstantes.Vehicles.names.radio)
				t.Append("<td class=\"" + P2 + " whiteBackGround\" nowrap>" + GestionWeb.GetWebWord(1299, _webSession.SiteLanguage) + "</td>");
			else if (_vehicle == DBClassificationConstantes.Vehicles.names.tv || _vehicle == DBClassificationConstantes.Vehicles.names.others)
				t.Append("<td class=\"" + P2 + " whiteBackGround\" nowrap>" + GestionWeb.GetWebWord(1451, _webSession.SiteLanguage) + "</td>");
			t.Append("<td class=\"" + P2 + " whiteBackGround\" nowrap>" + GestionWeb.GetWebWord(1423, _webSession.SiteLanguage) + "</td>");
			t.Append("<td class=\"" + P2 + " whiteBackGround\" nowrap>" + GestionWeb.GetWebWord(869,_webSession.SiteLanguage) + "</td>");
			t.Append("<td class=\"" + P2 + " whiteBackGround\" nowrap>" + GestionWeb.GetWebWord(1435, _webSession.SiteLanguage) + "</td>");
			t.Append("</tr>");
			#endregion

			//1 ligne par tranche horaire
			for (int i = 0; i < tab.GetLength(0); i++) {
				classCss = "acl1";
				if (tab[i, FrameWorkResultConstantes.PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX] != null && tab[i, FrameWorkResultConstantes.PortofolioStructure.EUROS_COLUMN_INDEX] != null &&
					tab[i, FrameWorkResultConstantes.PortofolioStructure.EUROS_COLUMN_INDEX] != null && tab[i, FrameWorkResultConstantes.PortofolioStructure.SPOT_COLUMN_INDEX] != null &&
					tab[i, FrameWorkResultConstantes.PortofolioStructure.DURATION_COLUMN_INDEX] != null) {
					t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='" + backGround + "';\"  class=\"" + backGround + "\" height=\"20px\" >");

					//tranche horaire										
					t.Append("\r\n\t<td align=\"left\" class=\"" + classCss + "\" nowrap>" + tab[i, FrameWorkResultConstantes.PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX].ToString() + "</td>");
					//Euros						
					t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkResultConstantes.PortofolioStructure.EUROS_COLUMN_INDEX].ToString(), WebCst.CustomerSessions.Unit.euro, false) + "</td>");
					//Spot						
					t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkResultConstantes.PortofolioStructure.SPOT_COLUMN_INDEX].ToString(), WebCst.CustomerSessions.Unit.spot, false) + "</td>");
					//Durée
					t.Append("\r\n\t<td class=\"" + classCss + "\" nowrap>" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkResultConstantes.PortofolioStructure.DURATION_COLUMN_INDEX].ToString(), WebCst.CustomerSessions.Unit.duration, false) + "</td>");
					t.Append("</tr>");
				}
				classCss = "acl2";
				backGround = "violetBackGroundV3";
			}
			t.Append("</table>");

			return t.ToString();

		}

		/// <summary>
		/// Vérifie qu'une datarow est vide
		/// </summary>
		/// <param name="ds">dataset</param>
		/// <returns>vrai si non vide</returns>
		protected virtual bool IsRowNull(DataSet ds) {
			if (ds != null && ds.Tables[0].Rows.Count > 0) {
				foreach (DataRow dr in ds.Tables[0].Rows) {
					return (dr["euros"] != System.DBNull.Value && dr["spot"] != System.DBNull.Value && dr["duration"] != System.DBNull.Value);
				}
			}
			return false;
		}
		#endregion
	}
}
