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
using TNS.AdExpress.Domain.Level;
using DBCst=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Web.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Portofolio.DAL;
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
            isTvNatThematiques = portofolioDAL.IsBelongToTvNatThematiques();
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
            if(ds==null || ds.Tables[0]==null || ds.Tables[0].Rows.Count==0) {
                throw(new PortofolioException("DataSet for ResultTable is null"));
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
            // Insertions column
            if(_showInsertions) {
                headers.Root.Add(new HeaderCreative(false,GestionWeb.GetWebWord(INSERTIONS_LIST_COL,_webSession.SiteLanguage),INSERTIONS_LIST_COL));
                insertions = 1;
            }
            // Creatives column     
            if(_showCreatives) {
                headers.Root.Add(new HeaderCreative(false,GestionWeb.GetWebWord(CREATIVES_COL,_webSession.SiteLanguage),CREATIVES_COL));
                creatives=1;
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
                    throw new PortofolioException("Média non traité.");
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

        #region Cell Factory
        //protected CellFac
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
        protected int GetPortofolioSize(DataTable dt) {

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
        private int GetCalendarSize(DataTable dt,ArrayList parutions) {

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
        protected bool ShowInsertions() {
            foreach(DetailLevelItemInformation item in _webSession.GenericProductDetailLevel.Levels) {
                if(_module.ModuleType==WebCst.Module.Type.alert &&
					(item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
					|| item.Id.Equals(DetailLevelItemInformation.Levels.product))) {
                    return (true);
                    break;
                }
            }
            return (false);
        }
        /// <summary>
        /// Determine if the result shows the creation column
        /// </summary>
        /// <returns>True if the creation column is shown</returns>
        protected bool ShowCreatives() {
            foreach(DetailLevelItemInformation item in _webSession.GenericProductDetailLevel.Levels) {
                if(_module.ModuleType==WebCst.Module.Type.alert &&
                    _webSession.CustomerLogin.GetFlag(DBCst.Flags.ID_SLOGAN_ACCESS_FLAG)!=null &&
					(item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
					|| item.Id.Equals(DetailLevelItemInformation.Levels.product))) {
                    return (true);
                    break;
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
        private void ChangeLineType(ref LineType lineType) {

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
        private string GetDayOfWeek(string dayOfWeek) {
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
        private void GetAllPeriodInsertions(StringBuilder t, string linkText) {
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
    }
}
