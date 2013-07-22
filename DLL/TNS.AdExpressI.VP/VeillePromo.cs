﻿#define DEBUG
using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.VP.DAL;
using System.Data;
using TNS.AdExpress.Domain.Web.Navigation;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstResults = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.FrameWork.Date;
using System.Reflection;
using TNS.AdExpress.Domain.Level;
using CoreUtils = TNS.AdExpress.Web.Core.Utilities;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpressI.VP
{
    public class VeillePromo : IVeillePromo
    {        

        #region Variables
        /// <summary>
        /// Is Initialize Visual List
        /// </summary>
        private bool _isInitializeVisualList = false;
        /// <summary>
        /// Client's session
        /// </summary>
        protected Dictionary<string, List<string>> _visualList = null;
        /// <summary>
        /// Client's session
        /// </summary>
        protected WebSession _session = null;
        /// <summary>
        /// Current module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(CstWeb.Module.Name.VP);

        /// <summary>
        /// Period beginning date
        /// </summary>
        protected string _periodBeginningDate = string.Empty;

       
        /// <summary>
        /// Period end date
        /// </summary>
        private string _periodEndDate = string.Empty;

   
        /// <summary>
        /// ID data promotion
        /// </summary>
        protected long _idDataPromotion;

        protected string _resultControlId = string.Empty;
        /// <summary>
        /// Theme
        /// </summary>
        protected string _theme = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public VeillePromo(WebSession session)
        {
            if (session == null) throw new NullReferenceException(" parameter session cannot be null ");
            _session = session;
            if (_session.PeriodType == CstWeb.CustomerSessions.Period.Type.allHistoric)
            {

                var param = new object[1];
                param[0] = _session;
                var vpScheduleDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.
                    CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory,
                    _module.CountryDataAccessLayer.AssemblyName), _module.CountryDataAccessLayer.Class,
                    false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

                DataSet ds = vpScheduleDAL.GetMinMaxPeriod();
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    string periodBeginningDate = ds.Tables[0].Rows[0]["DATE_BEGIN_NUM"].ToString();
                    var dateBegin = new DateTime(int.Parse(periodBeginningDate.Substring(0, 4)), int.Parse(periodBeginningDate.Substring(4, 2)), 1);
                    dateBegin = dateBegin.AddMonths(-1);
                    _periodBeginningDate = dateBegin.ToString("yyyyMMdd");
                    string periodEndDate = ds.Tables[0].Rows[0]["DATE_END_NUM"].ToString();
                    var dateEnd = new DateTime(int.Parse(periodEndDate.Substring(0, 4)),
                        int.Parse(periodEndDate.Substring(4, 2)), int.Parse(periodEndDate.Substring(6, 2)));
                    dateEnd = dateEnd.AddMonths(1);
                    int days = DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month);
                    _periodEndDate = dateEnd.ToString("yyyyMM") + days.ToString();
                }
            }
            else
            {
                var dateBegin = new DateTime(int.Parse(_session.PeriodBeginningDate.Substring(0, 4)),
                    int.Parse(_session.PeriodBeginningDate.Substring(4, 2)), 1);
                dateBegin = dateBegin.AddMonths(-1);
                _periodBeginningDate = dateBegin.ToString("yyyyMMdd");

                var dateEnd = new DateTime(int.Parse(_session.PeriodEndDate.Substring(0, 4)),
                    int.Parse(_session.PeriodEndDate.Substring(4, 2)), int.Parse(_session.PeriodEndDate.Substring(6, 2)));
                dateEnd = dateEnd.AddMonths(1);

                int days = DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month);
                _periodEndDate = dateEnd.ToString("yyyyMM") + days.ToString();
            }

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="idDataPromotion">Id data promotion</param>
        public VeillePromo(WebSession session, long idDataPromotion)
        {
            if (session == null) throw new NullReferenceException(" parameter session cannot be null ");
            _session = session;
            _idDataPromotion = idDataPromotion;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Define Result Control Id
        /// </summary>
        public string ResultControlId
        {
            set { _resultControlId = value; }
        }
        /// <summary>
        /// Define Theme
        /// </summary>
        public string Theme
        {
            set { _theme = value; }
        }
        /// <summary>
        /// Get Period Beginning Date 
        /// </summary>
        public string PeriodBeginningDate
        {
            get { return _periodBeginningDate; }
        }

        /// <summary>
        /// Get Period EndDate
        /// </summary>
        public string PeriodEndDate
        {
            get { return _periodEndDate; }
        }

        #endregion

        #region GetHtml
        /// <summary>
        /// Get HTML code for the promotion schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        public virtual string GetHtml()
        {
            StringBuilder html = new StringBuilder(5000), tempHtml = new StringBuilder(1000);
            int rowSpan = 1, colSpan = 0;
            bool newL1 = false, newL2 = false, newL3 = false;

            VeillePromoScheduleData veillePromoScheduleData = GetData();

            if (veillePromoScheduleData != null && veillePromoScheduleData.Data != null && veillePromoScheduleData.Data.Count > 0)
            {
                //Generic Media Detail Level
                GenericDetailLevel detailLevel = _session.GenericMediaDetailLevel;

                //Number of levels
                int nbLevels = _session.GenericMediaDetailLevel.GetNbLevels;

                //weeks
                Dictionary<long, long> weekList = veillePromoScheduleData.WeekList;

                //data
                List<object[]> data = veillePromoScheduleData.Data;

                html.Append("<table cellspacing=\"0\" cellpadding=\"0\" >");

                #region Start list of monthes
                //Start list of monthes
                rowSpan = 2;
                html.Append("<tr>");
                html.AppendFormat("<td rowspan=\"{0}\" class=\"ptVp\"> &nbsp;{1}</td>", rowSpan, GestionWeb.GetWebWord(2872, _session.SiteLanguage));
                int curMonth = -1, curYear = -1, oldYear = -1, curWeek = -1, oldMonth = -1, start = -1;
                Int64 oldIdL1 = -1, idL1 = -1, oldIdL2 = -1, idL2 = -1, oldIdL3 = -1, idL3 = -1;
                int levelColSpan = 1, width = 31;
                string promoContent = "";
                foreach (KeyValuePair<long, long> kpv in weekList)
                {
                    long key = kpv.Key;
                    curMonth = int.Parse(key.ToString().Substring(4, 2));
                    curYear = int.Parse(key.ToString().Substring(2, 2));
                    curWeek = int.Parse(key.ToString().Substring(6, 2));
                    if (oldMonth != curMonth && start > -1)
                    {
                        string monthString = "&nbsp;", yearString = "";
                        if (colSpan > 1)
                        {
                            monthString = MonthString.GetHTMLCharacters(oldMonth, _session.SiteLanguage, 3);
                            yearString = (oldYear.ToString().Length == 1) ? "0" + oldYear.ToString() : oldYear.ToString();
                        }
                        html.AppendFormat(" <td colspan=\"{0}\" class=\"ptm\">{1} {2}</td>", colSpan, monthString, yearString);
                        colSpan = 0;
                    }

                    tempHtml.AppendFormat(" <td class=\"vpw\">{0}</td>", (curWeek.ToString().Length == 1 ? "0" + curWeek.ToString() : curWeek.ToString()));
                    start = 0;
                    oldMonth = curMonth;
                    oldYear = curYear;
                    colSpan++;
                }
                if (start > -1)
                {
                    string monthString = "&nbsp;", yearString = "";
                    if (colSpan > 1)
                    {
                        monthString = MonthString.GetHTMLCharacters(oldMonth, _session.SiteLanguage, 3);
                        yearString = (oldYear.ToString().Length == 1) ? "0" + oldYear.ToString() : oldYear.ToString();
                    }
                    html.AppendFormat(" <td colspan=\"{0}\" class=\"ptm\">{1} {2}</td>", colSpan, monthString, yearString);
                    colSpan = 0;
                }
                html.Append("</tr>");
                //End list of monthes
                #endregion

                #region Start list of weeks
                //Start list of weeks
                if (tempHtml.Length > 0)
                {
                    html.Append("<tr>");
                    html.Append(tempHtml.ToString());
                    html.Append("</tr>");
                }
                //End list of weeks
                #endregion

                //bool newLevel = false;
                string brand = "";
                rowSpan = 0;
                int firstVpItemColIndex = -1;
                if (nbLevels == 1) firstVpItemColIndex = CstResults.VeillePromo.L1_COLUMN_INDEX + 1;
                if (nbLevels == 2) firstVpItemColIndex = CstResults.VeillePromo.L2_COLUMN_INDEX + 1;
                if (nbLevels == 3) firstVpItemColIndex = CstResults.VeillePromo.L3_COLUMN_INDEX + 1;

                #region Table content

                for (int i = 0; i < data.Count; i++)
                {
                    object[] dr = data[i];
                    if (nbLevels >= 1) idL1 = Convert.ToInt64(dr[CstResults.VeillePromo.L1_ID_COLUMN_INDEX]);
                    if (nbLevels >= 2) idL2 = Convert.ToInt64(dr[CstResults.VeillePromo.L2_ID_COLUMN_INDEX]);
                    if (nbLevels >= 3) idL3 = Convert.ToInt64(dr[CstResults.VeillePromo.L3_ID_COLUMN_INDEX]);
                    levelColSpan = weekList.Count;

                    //Start row level 1
                    if (nbLevels >= 1 && idL1 != oldIdL1)
                    {
                        html.AppendFormat("<tr><td class=\"{0}\">", ((i == 0) ? "L1Vp" : "L1VpbBis"));
                        html.Append(Convert.ToString(dr[CstResults.VeillePromo.L1_COLUMN_INDEX]));
                        html.AppendFormat("</td><td colspan=\"{0}\" class=\"p3\">&nbsp;</td></tr>", levelColSpan);
                        newL1 = true;
                        if (nbLevels > 1) newL2 = true;
                    }
                    //End row level 1

                    //Start row level 2
                    if (nbLevels >= 2 && (idL2 != oldIdL2 || newL2))
                    {
                        html.AppendFormat("<tr><td class=\"{0}\">", ((newL1) ? "L2Vp" : "L2VpBis"));
                        html.AppendFormat("&nbsp; {0}", Convert.ToString(dr[CstResults.VeillePromo.L2_COLUMN_INDEX]));
                        html.AppendFormat("</td><td colspan=\"{0}\" class=\"p3\">&nbsp;</td></tr>", levelColSpan);
                        newL2 = true;
                        if (nbLevels > 2) newL3 = true;
                    }
                    //End row level 2

                    //Start row level 3
                    if (nbLevels >= 3 && (idL3 != oldIdL3 || newL3))
                    {
                        html.AppendFormat("<tr><td class=\"{0}\">", ((newL2) ? "L3Vp" : "L3VpBis"));
                        html.AppendFormat("&nbsp;&nbsp; {0}", Convert.ToString(dr[CstResults.VeillePromo.L3_COLUMN_INDEX]));
                        html.AppendFormat("</td><td colspan=\"{0}\" class=\"p3\">&nbsp;</td></tr>", levelColSpan);
                        newL3 = true;
                    }
                    //End row level 3

                    //***********************Start promotions rows*********************************************************************
                    //if (newLevel)
                    //{
                    //    rowSpan = 0;

                    //}
                    //rowSpan++;
                    colSpan = 0;
                    rowSpan = 1;//DEBUG
                    html.AppendFormat("<tr><td  rowspan=\"{0}\" class=\"L4Vp\"></td>", rowSpan);

                    for (int j = firstVpItemColIndex; j < dr.Length; j++)
                    {
                        bool endIncomplete = false, startIncomplete = false;
                        if (dr[j] != null)
                        {
                            VeillePromoItem vpi = (VeillePromoItem)dr[j];
                            if (vpi.ItemType == CstResults.VeillePromo.itemType.presentStart
                                || vpi.ItemType == CstResults.VeillePromo.itemType.presentStartIncomplete)
                            {
                                if (vpi.ItemType == CstResults.VeillePromo.itemType.presentStartIncomplete) startIncomplete = true;
                                brand = vpi.Brand;
                                for (int k = j; k < dr.Length; k++)
                                {
                                    VeillePromoItem vpi2 = (VeillePromoItem)dr[k];
                                    if ((j == k) || (vpi2.ItemType == CstResults.VeillePromo.itemType.extended
                                    || vpi2.ItemType == CstResults.VeillePromo.itemType.endIncomplete))
                                    {
                                        if (vpi2.ItemType == CstResults.VeillePromo.itemType.endIncomplete) endIncomplete = true;
                                        colSpan++;
                                        j = k;
                                    }
                                    else
                                    {
                                        j = k - 1;
                                        break;
                                    }

                                }

                                html.AppendFormat("<td colspan=\"{0}\" class=\"vp2\" width=\"{1}\">", colSpan, width * colSpan);
                                promoContent = SplitContent(vpi.PromotionContent, colSpan);
                                string promoPeriod = (vpi.DateBegin.Day.ToString().Length == 1) ? "0" + vpi.DateBegin.Day.ToString() : vpi.DateBegin.Day.ToString();
                                promoPeriod += "/" + ((vpi.DateBegin.Month.ToString().Length == 1) ? "0" + vpi.DateBegin.Month.ToString() : vpi.DateBegin.Month.ToString());
                                promoPeriod += " - " + ((vpi.DateEnd.Day.ToString().Length == 1) ? "0" + vpi.DateEnd.Day.ToString() : vpi.DateEnd.Day.ToString());
                                promoPeriod += "/" + ((vpi.DateEnd.Month.ToString().Length == 1) ? "0" + vpi.DateEnd.Month.ToString() : vpi.DateEnd.Month.ToString());

                                html.Append("<table class=\"vp\">");
                                html.AppendFormat("<tr><td class=\"{0}\">", vpi.CssClass);
                                string promoAnchor = "<a class=\"tooltip\"  href=\"javascript:displayPromoFile_" + _resultControlId + "(" + vpi.IdDataPromotion + ", true);\">";
                                string excluWeb = vpi.ExcluWeb == 1
                                                      ? GestionWeb.GetWebWord(3000, _session.DataLanguage)
                                                      : string.Empty;
                                promoAnchor += string.Format("<em><span></span><b>{0} : {1} {2}</b><br/>{3}</em>"
                                    , (Convertion.ToHtmlString(brand)), promoPeriod, excluWeb, 
                                    (Convertion.ToHtmlString(vpi.PromotionContent)).Replace("'", "\'"));


                                html.Append(promoAnchor);
                                if (startIncomplete) html.Append("<span class=\"flg\">&lsaquo;&nbsp;</span>");
                                //Add promotion period
                                if (colSpan >= CstResults.VeillePromo.NB_MIN_WEEKS_TO_SHOW_PERIOD)
                                {
                                    if (!string.IsNullOrEmpty(excluWeb)) html.AppendFormat("{0} {1}",promoPeriod,excluWeb);
                                    else html.AppendFormat(promoPeriod);
                                }
                                if (endIncomplete) html.Append("<span class=\"fld\">&nbsp;&rsaquo;</span>");
                                html.Append("</a>");
                                html.Append("</td></tr>");
                                //Add promotion Content
                                html.Append("<tr><td class=\"vpb\">");
                                html.Append(promoAnchor);
                                html.Append(promoContent);
                                html.Append("</a>");
                                html.Append("</td></tr>");
                                html.Append("</table>");
                                html.Append("</td>");
                            }
                            else
                            {
                                html.AppendFormat(" <td class=\"p3\">&nbsp;</td>");
                            }
                        }
                        else
                        {
                            html.AppendFormat(" <td class=\"p3\">&nbsp;</td>");
                        }
                        colSpan = 0;
                        endIncomplete = false;
                        startIncomplete = false;
                    }

                    html.Append("</tr>");
                    //***********************End promotions rows*********************************************************************


                    oldIdL1 = idL1;
                    oldIdL2 = idL2;
                    oldIdL3 = idL3;
                    newL1 = newL2 = newL3 = false;
                }
                #endregion

                html.Append("</table>");
            }
            else
            {

                html.Append("<div align=\"center\" class=\"vpResNoData\">" + GestionWeb.GetWebWord(177, _session.SiteLanguage) + "</div>");

            }

            return html.ToString();
        }

        #endregion

        #region GetPromoFileHtml
        /// <summary>
        /// Get HTML code for the promotion file
        /// </summary>
        /// <param name="idDataPromotion">Promotion Id</param>
        /// <returns>HTML Code</returns>
        public virtual string GetPromoFileHtml()
        {
            object[] param = null;
            IVeillePromoDAL vpScheduleDAL = null;
            param = new object[3];
            param[0] = _session;
            param[1] = _periodBeginningDate;
            param[2] = _periodEndDate;
            vpScheduleDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}",
                AppDomain.CurrentDomain.BaseDirectory, _module.CountryDataAccessLayer.AssemblyName), _module.CountryDataAccessLayer.Class, false,
                BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            DataSet ds = vpScheduleDAL.GetData(_idDataPromotion);
            var html = new StringBuilder(1000);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                string visibility = "hidden";
                html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"100%\" height=\"100%\">");

                #region Header
                //Header
                html.Append("<tr><td class=\"VpFilePopUpHeader\">");
                html.AppendFormat("&nbsp;&nbsp;{0} ", GestionWeb.GetWebWord(2883, _session.SiteLanguage));

                html.Append("</td></tr>");
                #endregion

                #region Content
                html.Append("<tr>");
                html.Append("<td class=\"vpfContent\">");
                html.Append("<div class=\"vpfContent\">");
                html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"100%\" height=\"100%\">");

                //Content
                html.Append("<tr><td class=\"vpfDescr\">");
                html.Append("<ul class=\"prf\">");
              

                //VP Brand
                html.AppendFormat(" <li><span><b>{0}:</b></span><span class=\"prfSp\"\"> {1}</span>", GestionWeb.GetWebWord(2876, _session.SiteLanguage), dr["BRAND"].ToString());

              

                ////Image export pdf
                html.Append("<img align=\"right\" alt=\""+ GestionWeb.GetWebWord(2865, _session.SiteLanguage)+"\" ");
                html.Append("id=\"promofile" + _resultControlId + "\" ");
                html.AppendFormat("src=\"/App_Themes/{0}/Images/Common/export_pdf.gif\" ", _theme);
                html.AppendFormat("onmouseover=\"javascript:this.src='/App_Themes/{0}/Images/Common/export_pdf_over.gif';\" ", _theme);
                html.AppendFormat("onmouseout=\"javascript:this.src='/App_Themes/{0}/Images/Common/export_pdf.gif';\" ", _theme);
                html.AppendFormat("onclick=\"javascript:popupOpenBis('/Private/MyAdExpress/PdfSavePopUp.aspx?idSession={0}&idDataPromotion={1}&resultType={2}','470','210','yes');\" "
                    , _session.IdSession, _idDataPromotion, TNS.AdExpress.Anubis.Constantes.Result.type.selket.GetHashCode());
                html.Append(" />");
                html.Append("</li>");

                string dateBegin = TNS.FrameWork.Date.DateString.YYYYMMDDToDD_MM_YYYY(Convert.ToInt32(dr["DATE_BEGIN_NUM"].ToString()), _session.SiteLanguage);
                string dateEnd = TNS.FrameWork.Date.DateString.YYYYMMDDToDD_MM_YYYY(Convert.ToInt32(dr["DATE_END_NUM"].ToString()), _session.SiteLanguage);
                //Date
                html.AppendFormat(" <li><span><b>{0}:</b></span><span class=\"prfSp\"> {1} {2} {3} {4}</span></li>"
                    , GestionWeb.GetWebWord(895, _session.SiteLanguage), GestionWeb.GetWebWord(896, _session.SiteLanguage)
                    , dateBegin, GestionWeb.GetWebWord(897, _session.SiteLanguage), dateEnd);

                //Product classification
                html.AppendFormat("  <li><span><b>{0}:</b></span><span class=\"prfSp\"> {1} /{2} / {3}</span></li>"
                    , GestionWeb.GetWebWord(2884, _session.SiteLanguage), dr["SEGMENT"].ToString(), dr["CATEGORY"].ToString(), dr["PRODUCT"].ToString());

                //Promotion Content
                if (dr["PROMOTION_CONTENT"] != DBNull.Value && dr["PROMOTION_CONTENT"].ToString().Length > 0)
                {
                    string promoContent = dr["PROMOTION_CONTENT"].ToString();
                    html.AppendFormat("  <li><span><b>{0}:</b></span><span class=\"prfSp\"> {1}</span></li>"
                        , GestionWeb.GetWebWord(2885, _session.SiteLanguage), promoContent);
                }

                //Exclu Web
                html.AppendFormat(" <li><span><b>{0}:</b></span><span class=\"prfSp\"\"> {1}</span></li>"
                    , GestionWeb.GetWebWord(2997, _session.SiteLanguage),
                    (Convert.ToInt64(dr["EXCLU_WEB"].ToString()) == 1) ? GestionWeb.GetWebWord(2998, _session.SiteLanguage)
                    : GestionWeb.GetWebWord(2999, _session.SiteLanguage));

                //Brands
                if (dr["PROMOTION_BRAND"] != System.DBNull.Value && dr["PROMOTION_BRAND"].ToString().Length > 0)
                {
                    string[] brands = dr["PROMOTION_BRAND"].ToString().Split(',');
                    html.AppendFormat(" <li><span><b>{0}:</b></span>", GestionWeb.GetWebWord(1149, _session.SiteLanguage));

                    if (brands.Length == 1)
                    {
                        html.AppendFormat("<span class=\"prfSp\"> {0}</span>", brands[0]);
                    }
                    else
                    {
                        html.Append(" <ul class=\"prfbd\">");
                        for (int i = 0; i < brands.Length; i++)
                        {
                            html.AppendFormat("<li><span class=\"prfSp\">{0}</span> </li>", brands[i]);
                        }
                        html.Append(" </ul>");
                    }
                    html.Append(" </li>");

                }
                html.Append("</ul></td></tr>");

                #region Image(s) promotion
                if (dr["PROMOTION_VISUAL"] != System.DBNull.Value && dr["PROMOTION_VISUAL"].ToString().Length > 0)
                {
                    html.Append("<tr><td>");
                    html.Append("<hr class=\"hrSpacer\"/>");
                    html.Append("</td></tr>");

                    //Button arrow left
                    html.Append("<tr><td>");
                    html.Append("  <table cellpadding=\"0\" border=\"0\" align=\"center\">");
                    html.Append(" <tr><td id=\"previous\">");
                    html.AppendFormat(" <img id=\"imgPrevious_" + _resultControlId + "\" style=\"visibility: hidden; margin-left: 5px; cursor: pointer; margin-right: 5px; vertical-align: middle;\" onmouseover=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_left_down.gif'\" onmouseout=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_left.gif'\" onclick=\"javascript:DisplayPics_" + _resultControlId + "(-1,false,this,document.getElementById('imgNext_" + _resultControlId + "'),document.getElementById('img_pr_" + _resultControlId + "'));\" src=\"/App_Themes/{0}/Images/Common/Button/arrow_left.gif\" />", _theme);
                    html.Append("</td>");

                    //Promoption visual
                    string currentPromovisual = "";
                    visibility = "hidden";
                    if (_visualList != null && _visualList.ContainsKey("p"))
                    {
                        currentPromovisual = _visualList["p"][0];
                        if (_visualList["p"].Count > 1) visibility = "visible";
                    }
                    html.AppendFormat(" <td id=\"img_pr_div\" class=\"pr_visu\"><a onclick=\"javascript:ZoomPromotionImage_" + _resultControlId + "(img_pr_" + _resultControlId + ".src);\"><img class=\"pr_visu\" id=\"img_pr_" + _resultControlId + "\" src=\"{0}\" /> </a> </td>", currentPromovisual);

                    //Button arrow right
                    html.Append(" <td id=\"next\">");
                    html.AppendFormat(" <img id=\"imgNext_" + _resultControlId + "\" style=\"visibility: {1}; margin-left: 5px; cursor: pointer; margin-right: 5px; vertical-align: middle;\" onmouseover=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_right_down.gif'\" onmouseout=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_right.gif'\" onclick=\"javascript:DisplayPics_" + _resultControlId + "(1,false,document.getElementById('imgPrevious_" + _resultControlId + "'),this,document.getElementById('img_pr_" + _resultControlId + "'));\" src=\"/App_Themes/{0}/Images/Common/Button/arrow_right.gif\" />", _theme, visibility);
                    html.Append("</td>");

                    html.Append("</table>");
                    html.Append("</td></tr>");
                }
                #endregion

                #region Promotion's Conditions Text
                //Promotion's Conditions
                if (dr["CONDITION_TEXT"] != System.DBNull.Value && dr["CONDITION_TEXT"].ToString().Length > 0)
                {
                    html.Append("<tr><td>");
                    html.Append("<hr class=\"hrSpacer\"/>");
                    html.Append("</td></tr>");

                    string conditionText = dr["CONDITION_TEXT"].ToString();
                    html.Append("<tr><td  class=\"vpfDescrCond\">");
                    html.Append("<ul class=\"prf\">");
                    html.AppendFormat(" <li><span><b>{0}:</b></span><span class=\"prfSp\">", GestionWeb.GetWebWord(2886, _session.SiteLanguage));
                    html.Append("  <br />");
                    html.Append(conditionText);
                    html.Append("</span></li>");
                    html.Append(" </ul>");
                    html.Append("</td></tr>");
                }
                #endregion

                #region Promotion's Conditions Images
                if (dr["CONDITION_VISUAL"] != System.DBNull.Value && dr["CONDITION_VISUAL"].ToString().Length > 0)
                {
                    html.Append("<tr><td>");
                    html.Append("<hr class=\"hrSpacer\"/>");
                    html.Append("</td></tr>");

                    //Button arrow left
                    html.Append("<tr><td>");
                    html.Append("  <table cellpadding=\"0\" border=\"0\" align=\"center\">");
                    html.Append(" <tr><td id=\"previous\">");
                    html.AppendFormat(" <img id=\"imgPreviousCd_{1}\" style=\"visibility: hidden; margin-left: 5px; cursor: pointer; margin-right: 5px; vertical-align: middle;\" onmouseover=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_left_down.gif'\" onmouseout=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_left.gif'\" onclick=\"javascript:DisplayPics_{1}(-1,true,this,document.getElementById('imgNextCd_{1}'),document.getElementById('img_prCd_{1}'));\" src=\"/App_Themes/{0}/Images/Common/Button/arrow_left.gif\" />"
                        , _theme, _resultControlId);
                    html.Append("</td>");

                    //Promoption visual
                    visibility = "hidden";
                    string currentPromoCondivisual = "";
                    if (_visualList != null && _visualList.ContainsKey("pc"))
                    {
                        currentPromoCondivisual = _visualList["pc"][0];
                        if (_visualList["pc"].Count > 1) visibility = "visible";
                    }
                    html.AppendFormat(" <td id=\"img_pr_Cd_div\" class=\"pr_visu\"><a onclick=\"javascript:ZoomPromotionImage_{1}(img_prCd_{1}.src);\"><img class=\"pr_visu\" id=\"img_prCd_{1}\" src=\"{0}\" /> </a> </td>"
                        , currentPromoCondivisual, _resultControlId);

                    //Button arrow right
                    html.Append(" <td id=\"next\">");
                    html.AppendFormat(" <img id=\"imgNextCd_{2}\" style=\"visibility: {1}; margin-left: 5px; cursor: pointer; margin-right: 5px; vertical-align: middle;\" onmouseover=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_right_down.gif'\" onmouseout=\"this.src='/App_Themes/{0}/Images/Common/Button/arrow_right.gif'\" onclick=\"javascript:DisplayPics_{2}(1,true,document.getElementById('imgPreviousCd_{2}'),this,document.getElementById('img_prCd_{2}'));\" src=\"/App_Themes/{0}/Images/Common/Button/arrow_right.gif\" />"
                        , _theme, visibility, _resultControlId);
                    html.Append("</td>");

                    html.Append("</table>");
                    html.Append("</td></tr>");
                }
                #endregion


                html.Append("</table>");
                html.Append("</div>");
                html.Append("</td>");
                html.Append("</tr>");
                #endregion

                #region Button close
                html.Append("<tr>");//
                html.Append("<td class=\"vpScheduleResultWebControlButtons\">");
                html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" class=\"vpScheduleResultWebControlButtons\">");
                html.Append("<tr>");
                html.Append("<td class=\"vpScheduleResultWebControlButtonCancel\">");
                html.AppendFormat("<img src=\"/App_Themes/{0}/Images/Common/Button/initialize_all_up.gif\" onmouseover=\"javascript:this.src='/App_Themes/{0}/Images/Common/Button/initialize_all_down.gif';\" onmouseout=\"javascript:this.src='/App_Themes/{0}/Images/Common/Button/initialize_all_up.gif';\" onclick=\"javascript:displayPromoFile_{1}({2},false);\"/>"
                    , _theme, _resultControlId, _idDataPromotion);
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("</table>");
                html.Append("</td>");
                html.Append("</tr>");

                #endregion

                html.Append("</table>");

            }
            else
            {

                html.AppendFormat("<div align=\"center\" class=\"vpResNoData\">{0}</div>"
                    , GestionWeb.GetWebWord(177, _session.SiteLanguage));

            }

            return html.ToString();
        }
        #endregion

        #region GetPromoFileList
        /// <summary>
        /// Get HTML code for the promotion file
        /// </summary>
        /// <param name="idDataPromotion">Promotion Id</param>
        /// <returns>HTML Code</returns>
        public virtual Dictionary<string, List<string>> GetPromoFileList()
        {
            if (!_isInitializeVisualList)
            {
                _isInitializeVisualList = true;
                _visualList = new Dictionary<string, List<string>>();
                var param = new object[3];
                param[0] = _session;
                param[1] = _periodBeginningDate;
                param[2] = _periodEndDate;
                var vpScheduleDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                    , AppDomain.CurrentDomain.BaseDirectory, _module.CountryDataAccessLayer.AssemblyName),
                    _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                DataSet ds = vpScheduleDAL.GetData(_idDataPromotion);


                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string[] promoVisuals = null;
                    if (dr["PROMOTION_VISUAL"] != System.DBNull.Value && dr["PROMOTION_VISUAL"].ToString().Length > 0)
                    {
                        //Add promotions visuals
                        promoVisuals = dr["PROMOTION_VISUAL"].ToString().Split(',');
                        var promoList = new List<string>();
                        for (int i = 0; i < promoVisuals.Length; i++)
                        {
                            promoList.Add(string.Format("{0}/{1}", CstWeb.CreationServerPathes.IMAGES_VP, promoVisuals[i].Trim()));

                        }
                        if (promoList.Count > 0) _visualList.Add("p", promoList);


                    }
                    if (dr["CONDITION_VISUAL"] != System.DBNull.Value && dr["CONDITION_VISUAL"].ToString().Length > 0)
                    {
                        //Add promotions visuals
                        promoVisuals = dr["CONDITION_VISUAL"].ToString().Split(',');
                        var promoCdList = new List<string>();
                        for (int i = 0; i < promoVisuals.Length; i++)
                        {
                            promoCdList.Add(string.Format("{0}/{1}", CstWeb.CreationServerPathes.IMAGES_VP, promoVisuals[i].Trim()));

                        }
                        if (promoCdList.Count > 0) _visualList.Add("pc", promoCdList);


                    }
                }
            }
            return _visualList;
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get Data
        /// </summary>
        /// <returns>data promo</returns>
        public virtual VeillePromoScheduleData GetData()
        {
            object[] param = null, promoRow = null;
            IVeillePromoDAL vpScheduleDAL = null;
            DataSet ds = null;
            DataTable dt = null;
            Dictionary<long, long> weekList = new Dictionary<long, long>();
            Dictionary<long, string> cssPromoList = new Dictionary<long, string>();
            Int64 oldIdL1 = -1;
            Int64 oldIdL2 = -1;
            Int64 oldIdL3 = -1;
            bool newL1 = false, newL2 = false, newL3 = false, withPromoOverlap = false;
            List<int> currentLevelPromoLineIndex = null;
            VeillePromoItem veillePromoItem = null;
            CstResults.VeillePromo.itemType promoItemType = CstResults.VeillePromo.itemType.absent;
            int currentRowIndex = -1, oldRowIndex = -1;
            DetailLevelItemInformation persoLevel = DetailLevelItemsInformation.Get(_session.PersonnalizedLevel);
            int cssIndex = 1; long tempId = -1;

            param = new object[3];
            param[0] = _session;
            param[1] = _periodBeginningDate;
            param[2] = _periodEndDate;
            vpScheduleDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, _module.CountryDataAccessLayer.AssemblyName), _module.CountryDataAccessLayer.Class
                , false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            ds = vpScheduleDAL.GetData();

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            dt = ds.Tables[0];

            //Generic Media Detail Level
            GenericDetailLevel detailLevel = _session.GenericMediaDetailLevel;

            //Number of levels
            int nbLevels = _session.GenericMediaDetailLevel.GetNbLevels;

            //Get week list 
            DateTime periodBegin = DateString.YYYYMMDDToDateTime(_periodBeginningDate);
            DateTime periodEnd = DateString.YYYYMMDDToDateTime(_periodEndDate);
            AtomicPeriodWeek periodBeginWeek = new AtomicPeriodWeek(periodBegin);
            AtomicPeriodWeek periodEndWeek = new AtomicPeriodWeek(periodEnd);
            long beginWeekYYYYWW = Convert.ToInt64(DateString.AtomicPeriodWeekToYYYYWW(periodBeginWeek));
            long endWeekYYYYWW = Convert.ToInt64(DateString.AtomicPeriodWeekToYYYYWW(periodEndWeek));

            //Init Promo Schedule Data
            var vpData = new VeillePromoScheduleData(_periodBeginningDate, _periodEndDate, nbLevels);

            //Get nummber of columns
            int nbCol = _session.GenericMediaDetailLevel.GetNbLevels * 2 + vpData.WeekList.Count;


            foreach (DataRow currentRow in dt.Rows)
            {
                promoItemType = CstResults.VeillePromo.itemType.absent;

                //Set classification data
                if (nbLevels >= 1 && oldIdL1 != GetLevelId(currentRow, 1, detailLevel))
                {
                    newL1 = true;
                    if (nbLevels > 1) newL2 = true;
                    oldIdL1 = GetLevelId(currentRow, 1, detailLevel);

                }
                if (nbLevels >= 2 && (oldIdL2 != GetLevelId(currentRow, 2, detailLevel) || newL2))
                {
                    newL2 = true;
                    if (nbLevels > 2) newL3 = true;
                    oldIdL2 = GetLevelId(currentRow, 2, detailLevel);
                }
                if (nbLevels >= 3 && (oldIdL3 != GetLevelId(currentRow, 3, detailLevel) || newL3))
                {
                    newL3 = true;
                    oldIdL3 = GetLevelId(currentRow, 3, detailLevel);
                }

                //Set data promo 
                DateTime promoDateBegin = DateString.YYYYMMDDToDateTime(currentRow["DATE_BEGIN_NUM"].ToString());
                DateTime promoDateEnd = DateString.YYYYMMDDToDateTime(currentRow["DATE_END_NUM"].ToString());
                AtomicPeriodWeek promoStartWeek = new AtomicPeriodWeek(promoDateBegin);
                AtomicPeriodWeek promoEndWeek = new AtomicPeriodWeek(promoDateEnd);
                long promoStartWeekYYYYWW = Convert.ToInt64(DateString.AtomicPeriodWeekToYYYYWW(promoStartWeek));
                long promoEndWeekYYYYWW = Convert.ToInt64(DateString.AtomicPeriodWeekToYYYYWW(promoEndWeek));

                string promocontent = (currentRow["PROMOTION_CONTENT"] != System.DBNull.Value) ? currentRow["PROMOTION_CONTENT"].ToString() : "";

                if (newL3 || newL2 || newL1)
                {
                    promoRow = new object[nbCol];
                    foreach (KeyValuePair<long, long> kpv in vpData.WeekList)
                    {
                        promoRow[kpv.Value] = new VeillePromoItem(CstResults.VeillePromo.itemType.absent);
                    }
                    currentRowIndex++;
                    currentLevelPromoLineIndex = new List<int>();
                }
                else
                {
                    oldRowIndex = GetLineWithAvailableRange(vpData.Data, vpData.WeekList, currentLevelPromoLineIndex, promoStartWeek, promoEndWeek);
                    if (oldRowIndex > -1)
                    {
                        promoRow = vpData.Data[oldRowIndex];
                    }
                    else
                    {
                        promoRow = new object[nbCol];
                        foreach (KeyValuePair<long, long> kpv in vpData.WeekList)
                        {
                            promoRow[kpv.Value] = new VeillePromoItem(CstResults.VeillePromo.itemType.absent);
                        }
                        currentRowIndex++;
                        withPromoOverlap = true;
                    }
                }

                promoEndWeek.Increment();
                AtomicPeriodWeek promoCurrentWeek = promoStartWeek;
                long promoCurrentWeekYYYYWW;
                int start = 0;
                while (!(promoCurrentWeek.Week == promoEndWeek.Week && promoCurrentWeek.Year == promoEndWeek.Year))
                {
                    promoCurrentWeekYYYYWW = Convert.ToInt64(DateString.AtomicPeriodWeekToYYYYWW(promoCurrentWeek));

                    if (beginWeekYYYYWW <= promoCurrentWeekYYYYWW && promoCurrentWeekYYYYWW <= endWeekYYYYWW)
                    {
                        if (start == 0)
                        {
                            tempId = Convert.ToInt64(currentRow[persoLevel.DataBaseIdField].ToString());
                            if (!cssPromoList.ContainsKey(tempId))
                            {
                                cssPromoList.Add(tempId, CstResults.VeillePromo.CSS_PROMO + cssIndex.ToString());
                                cssIndex++;
                            }
                            promoItemType = (promoStartWeekYYYYWW < beginWeekYYYYWW) ? 
                                CstResults.VeillePromo.itemType.presentStartIncomplete : CstResults.VeillePromo.itemType.presentStart;
                            veillePromoItem = new VeillePromoItem(promoItemType, Convert.ToInt64(currentRow["ID_DATA_PROMOTION"].ToString()),
                                promoDateBegin, promoDateEnd, promocontent, cssPromoList[tempId], currentRow["BRAND"].ToString()
                                , Convert.ToInt64(currentRow["EXCLU_WEB"].ToString()));
                        }
                        else if (promoCurrentWeekYYYYWW == endWeekYYYYWW && promoEndWeekYYYYWW > endWeekYYYYWW)
                        {
                            promoItemType = CstResults.VeillePromo.itemType.endIncomplete;
                            veillePromoItem = new VeillePromoItem(promoItemType);
                        }
                        else
                        {
                            promoItemType = CstResults.VeillePromo.itemType.extended;
                            veillePromoItem = new VeillePromoItem(promoItemType);
                        }
                        int month = CoreUtils.Dates.GetMonthFromWeek(promoCurrentWeek.Year, promoCurrentWeek.Week);
                        promoRow[vpData.WeekList[(promoCurrentWeek.Year * 100 + month) * 100 + promoCurrentWeek.Week]] = veillePromoItem;
                        start++;
                    }
                    promoCurrentWeek.Increment();

                }

                if (nbLevels >= 1)
                {
                    promoRow[CstResults.VeillePromo.L1_ID_COLUMN_INDEX] = oldIdL1;
                    promoRow[CstResults.VeillePromo.L1_COLUMN_INDEX] = GetLevelLabel(currentRow, 1, detailLevel);
                }
                if (nbLevels >= 2)
                {
                    promoRow[CstResults.VeillePromo.L2_ID_COLUMN_INDEX] = oldIdL2;
                    promoRow[CstResults.VeillePromo.L2_COLUMN_INDEX] = GetLevelLabel(currentRow, 2, detailLevel);
                }
                if (nbLevels >= 3)
                {
                    promoRow[CstResults.VeillePromo.L3_ID_COLUMN_INDEX] = oldIdL3;
                    promoRow[CstResults.VeillePromo.L3_COLUMN_INDEX] = GetLevelLabel(currentRow, 3, detailLevel);
                }

                if (newL3 || newL2 || newL1 || withPromoOverlap)
                    vpData.Data.Add(promoRow);


                if (!currentLevelPromoLineIndex.Contains(currentRowIndex))
                    currentLevelPromoLineIndex.Add(currentRowIndex);
                newL1 = newL2 = newL3 = false;
                withPromoOverlap = false;
            }


            return vpData;
        }

        #endregion

        #region Get Level Id / Label
        /// <summary>
        /// Get Level Id
        /// </summary>
        /// <param name="dr">Data Source</param>
        /// <param name="level">Requested level</param>
        /// <param name="detailLevel">Levels breakdown</param>
        /// <returns>Level Id</returns>
        protected virtual Int64 GetLevelId(DataRow dr, int level, GenericDetailLevel detailLevel)
        {
            return (Int64.Parse(dr[detailLevel.GetColumnNameLevelId(level)].ToString()));
        }
        /// <summary>
        /// Get Level Label
        /// </summary>
        /// <param name="dr">Data Source</param>
        /// <param name="level">Requested level</param>
        /// <param name="detailLevel">Levels breakdown</param>
        /// <returns>Level Label</returns>
        protected virtual string GetLevelLabel(DataRow dr, int level, GenericDetailLevel detailLevel)
        {
            return (dr[detailLevel.GetColumnNameLevelLabel(level)].ToString());
        }

        #endregion


        #region GetLineWithAvailableRange
        /// <summary>
        /// Get Line With Available Range
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="weekList">week List</param>
        /// <param name="currentLevelPromoLineIndex">current Level Promo Line Index</param>
        /// <param name="promoStartWeek">promo Start Week</param>
        /// <param name="promoEndWeek">promo End Week</param>
        /// <returns>Index of availble row with Available range</returns>
        protected virtual int GetLineWithAvailableRange(List<object[]> data, Dictionary<long, long> weekList, List<int> currentLevelPromoLineIndex, AtomicPeriodWeek promoStartWeek, AtomicPeriodWeek promoEndWeek)
        {
            int res = -1;

            if (currentLevelPromoLineIndex != null && currentLevelPromoLineIndex.Count > 0 && data != null && weekList != null)
            {
                AtomicPeriodWeek pStartWeek = (AtomicPeriodWeek)promoStartWeek.Clone();
                AtomicPeriodWeek pEndWeek = (AtomicPeriodWeek)promoEndWeek.Clone();
                for (int i = 0; i < currentLevelPromoLineIndex.Count; i++)
                {
                    object[] promoRow = data[currentLevelPromoLineIndex[i]];

                    pEndWeek.Increment();
                    while (!(pStartWeek.Week == pEndWeek.Week && pStartWeek.Year == pEndWeek.Year))
                    {
                        res = currentLevelPromoLineIndex[i];
                        int month = Dates.GetMonthFromWeek(pStartWeek.Year, pStartWeek.Week);
                        long weekKey = (pStartWeek.Year * 100 + month) * 100 + pStartWeek.Week;
                        if (weekList.ContainsKey(weekKey))
                        {
                            VeillePromoItem vItem = (VeillePromoItem)promoRow[weekList[weekKey]];
                            if (vItem.ItemType != CstResults.VeillePromo.itemType.absent)
                            {
                                res = -1; break;
                            }
                        }
                        pStartWeek.Increment();
                    }
                    if (res > 0) break;
                }
            }
            return res;
        }
        #endregion

        #region SplitContent
        /// <summary>
        /// Split text Content
        /// </summary>
        /// <param name="promoContent">promo Content</param>
        /// <param name="nbCol">nb Col </param>
        /// <returns>Split text </returns>
        public virtual string SplitContent(string promoContent, int nbCol)
        {
            string res = "";
            int length = promoContent.Length;
            int nbCharByCol = CstResults.VeillePromo.NB_CHAR_BY_COL;
            int nbLine = 2;

            if (!string.IsNullOrEmpty(promoContent) && (length > (nbCol * nbCharByCol * nbLine)))
            {
                int lenghtMax = (nbCol * nbCharByCol * nbLine) - CstResults.VeillePromo.ETC.Length;
                res = promoContent.Substring(0, lenghtMax) + CstResults.VeillePromo.ETC;
            }
            else return promoContent;

            return res;
        }
        #endregion


    }
}
