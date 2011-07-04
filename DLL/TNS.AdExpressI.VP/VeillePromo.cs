#define DEBUG
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

namespace TNS.AdExpressI.VP
{
    public class VeillePromo : IVeillePromo
    {
        #region Constantes
        /// <summary>
        /// Index de la colonne du niveau 1
        /// </summary>
        public const int L1_ID_COLUMN_INDEX = 0;
        /// <summary>
        /// Index of N1 label
        /// </summary>
        public const int L1_COLUMN_INDEX = 1;
        /// <summary>
        /// Index de la colonne du niveau 2
        /// </summary>
        public const int L2_ID_COLUMN_INDEX = 2;
        /// <summary>
        /// Index of N2 label
        /// </summary>
        public const int L2_COLUMN_INDEX = 3;
        /// <summary>
        /// Index de la colonne du niveau 3
        /// </summary>
        public const int L3_ID_COLUMN_INDEX = 4;
        /// <summary>
        /// Index of N3 label
        /// </summary>
        public const int L3_COLUMN_INDEX = 5;
        /// <summary>
        /// Css promo
        /// </summary>
        protected const string CSS_PROMO = "vph";
        /// <summary>
        /// Css promo
        /// </summary>
        protected const string ETC = "...";

        /// <summary>
        /// Nb minimum weeks to show period label
        /// </summary>
        public const int NB_MIN_WEEKS_TO_SHOW_PERIOD = 3;

        /// Nb max char by column
        /// </summary>
        public const int NB_CHAR_BY_COL = 4;
        #endregion

        #region Variables
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
        protected string _periodBeginningDate = "";
        /// <summary>
        /// Period end date
        /// </summary>
        protected string _periodEndDate = "";
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

                object[] param = new object[1];
                param[0] = _session;
                IVeillePromoDAL vpScheduleDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                DataSet ds = vpScheduleDAL.GetMinMaxPeriod();
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    string periodBeginningDate = ds.Tables[0].Rows[0]["DATE_BEGIN_NUM"].ToString();
                    DateTime dateBegin = new DateTime(int.Parse(periodBeginningDate.Substring(0, 4)), int.Parse(periodBeginningDate.Substring(4, 2)), 1);
                    dateBegin = dateBegin.AddMonths(-1);
                    _periodBeginningDate = dateBegin.ToString("yyyyMMdd");
                    string periodEndDate = ds.Tables[0].Rows[0]["DATE_END_NUM"].ToString();
                    DateTime dateEnd = new DateTime(int.Parse(periodEndDate.Substring(0, 4)), int.Parse(periodEndDate.Substring(4, 2)), int.Parse(periodEndDate.Substring(6, 2)));
                    dateEnd = dateEnd.AddMonths(1);
                    int days = DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month);
                    _periodEndDate = dateEnd.ToString("yyyyMM") + days.ToString();
                }
            }
            else
            {
                DateTime dateBegin = new DateTime(int.Parse(_session.PeriodBeginningDate.Substring(0, 4)), int.Parse(_session.PeriodBeginningDate.Substring(4, 2)), 1);
                dateBegin = dateBegin.AddMonths(-1);
                _periodBeginningDate = dateBegin.ToString("yyyyMMdd");
                DateTime dateEnd = new DateTime(int.Parse(_session.PeriodEndDate.Substring(0, 4)), int.Parse(_session.PeriodEndDate.Substring(4, 2)), int.Parse(_session.PeriodEndDate.Substring(6, 2)));
                dateEnd = dateEnd.AddMonths(1);
                int days = DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month);
                _periodEndDate = dateEnd.ToString("yyyyMM") + days.ToString();
            }

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
                html.AppendFormat("<td rowspan=\"{0}\" class=\"pt\"> &nbsp;Niveaux </td>", rowSpan);//TODO : Mettre libellé dans fichier ressources                 
                int curMonth = -1, curYear = -1, oldYear = -1, curWeek = -1, oldMonth = -1, start = -1;
                Int64 oldIdL1 = -1, idL1 = -1, oldIdL2 = -1, idL2 = -1, oldIdL3 = -1, idL3 = -1;
                int levelColSpan = 1, width = 31;
                string linkToPromoFile = "#", promoContent = "";
                foreach (KeyValuePair<long, long> kpv in weekList)
                {
                    long key = kpv.Key;
                    curMonth = int.Parse(key.ToString().Substring(4, 2));
                    curYear = int.Parse(key.ToString().Substring(2, 2));
                    curWeek = int.Parse(key.ToString().Substring(6, 2));
                    if (oldMonth != curMonth && start > -1)
                    {
                        string monthString = MonthString.GetHTMLCharacters(oldMonth, _session.SiteLanguage, 3);
                        html.AppendFormat(" <td colspan=\"{0}\" class=\"ptm\">{1} {2}</td>", colSpan, monthString, oldYear);
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
                    string monthString = MonthString.GetHTMLCharacters(oldMonth, _session.SiteLanguage, 3);
                    html.AppendFormat(" <td colspan=\"{0}\" class=\"ptm\">{1} {2}</td>", colSpan, monthString, oldYear);
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

                bool newLevel = false;
                string brand = "";

                #region Table content

                for (int i = 0; i < data.Count; i++)
                {
                    object[] dr = data[i];
                    if (nbLevels >= 1) idL1 = Convert.ToInt64(dr[L1_ID_COLUMN_INDEX]);
                    if (nbLevels >= 2) idL2 = Convert.ToInt64(dr[L2_ID_COLUMN_INDEX]);
                    if (nbLevels >= 3) idL3 = Convert.ToInt64(dr[L3_ID_COLUMN_INDEX]);
                    levelColSpan = weekList.Count;

                    //Start row level 1
                    if (nbLevels >= 1 && idL1 != oldIdL1)
                    {
                        html.Append("<tr><td class=\"L1\">");
                        html.Append(Convert.ToString(dr[L1_COLUMN_INDEX]));
                        html.AppendFormat("</td><td colspan=\"{0}\" class=\"p3\">&nbsp;</td></tr>", levelColSpan);
                        newLevel = true;
                    }
                    //End row level 1

                    //Start row level 2
                    if (nbLevels >= 2 && idL2 != oldIdL2)
                    {
                        html.Append("<tr><td class=\"L2\">");
                        html.AppendFormat("&nbsp; {0}", Convert.ToString(dr[L2_COLUMN_INDEX]));
                        html.AppendFormat("</td><td colspan=\"{0}\" class=\"p3\">&nbsp;</td></tr>", levelColSpan);
                        newLevel = true;

                    }
                    //End row level 2

                    //Start row level 3
                    if (nbLevels >= 3 && idL3 != oldIdL3)
                    {
                        html.Append("<tr><td class=\"L3\">");
                        html.AppendFormat("&nbsp;&nbsp; {0}", Convert.ToString(dr[L3_COLUMN_INDEX]));
                        html.AppendFormat("</td><td colspan=\"{0}\" class=\"p3\">&nbsp;</td></tr>", levelColSpan);
                        newLevel = true;
                    }
                    //End row level 3

                    //***********************Start promotions rows*********************************************************************
                    if (newLevel)
                    {
                        rowSpan = 0;

                    }
                    rowSpan++;
                    colSpan = 0;
                    html.AppendFormat("<tr><td rowspan=\"{0}\" class=\"L4\"></td>", rowSpan);

                    for (int j = L3_COLUMN_INDEX + 1; j < dr.Length; j++)
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
                                //string promoAnchor = "<a class=\"Tipspr\" href=\"" + linkToPromoFile + "\" title=\"::<table border=0 cellpadding=0 cellspacing=1 width=100% class=promoTableInfoBulle><tr><td class=titlepr>" + (Convertion.ToHtmlString(brand)) + "</td></tr><tr><td><div align=justify>" + (Convertion.ToHtmlString(vpi.PromotionContent)).Replace("'", "\'") + "</div></td></tr><tr><td height=5></td></tr></table>\">";
                                string promoAnchor = "<a class=\"tooltip\" href=\"" + linkToPromoFile + "\">";
                                promoAnchor += "<em><span></span><b>" + (Convertion.ToHtmlString(brand)) + " : " + promoPeriod + "</b><br/>" + (Convertion.ToHtmlString(vpi.PromotionContent)).Replace("'", "\'") + "</em>";


                                html.Append(promoAnchor);
                                if (startIncomplete) html.Append("<span class=\"flg\">&lsaquo;&nbsp;</span>");
                                //Add promotion period
                                if (colSpan >= NB_MIN_WEEKS_TO_SHOW_PERIOD)
                                {
                                    html.Append(promoPeriod);
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
                    newLevel = false;
                }
                #endregion

                html.Append("</table>");
            }
            else return string.Empty;

            return html.ToString();
        }

        #endregion

        #region GetData
        /// <summary>
        /// Get Data
        /// </summary>
        /// <returns>data promo</returns>
        protected virtual VeillePromoScheduleData GetData()
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
            int currentRowIndex = 0, oldRowIndex = -1;
            DetailLevelItemInformation persoLevel = DetailLevelItemsInformation.Get(64);//TODO : Get personnalized level via client session
            int cssIndex = 1; long tempId = -1;

            param = new object[3];
            param[0] = _session;
            param[1] = _periodBeginningDate;
            param[2] = _periodEndDate;
            vpScheduleDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
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
            VeillePromoScheduleData vpData = new VeillePromoScheduleData(_periodBeginningDate, _periodEndDate, nbLevels);

            //Get nummber of columns
            int nbCol = _session.GenericMediaDetailLevel.GetNbLevels * 2 + vpData.WeekList.Count;


            foreach (DataRow currentRow in dt.Rows)
            {
                promoItemType = CstResults.VeillePromo.itemType.absent;

                //Set classification data
                if (nbLevels >= 1 && oldIdL1 != GetLevelId(currentRow, 1, detailLevel))
                {
                    newL1 = true;
                    newL2 = true;
                    oldIdL1 = GetLevelId(currentRow, 1, detailLevel);

                }
                if (nbLevels >= 2 && (oldIdL2 != GetLevelId(currentRow, 2, detailLevel) || newL2))
                {
                    newL2 = true;
                    newL3 = true;
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
                                cssPromoList.Add(tempId, CSS_PROMO + cssIndex.ToString());
                                cssIndex++;
                            }
                            promoItemType = (promoStartWeekYYYYWW < beginWeekYYYYWW) ? CstResults.VeillePromo.itemType.presentStartIncomplete : CstResults.VeillePromo.itemType.presentStart;
                            veillePromoItem = new VeillePromoItem(promoItemType, Convert.ToInt64(currentRow["ID_DATA_PROMOTION"].ToString()), promoDateBegin, promoDateEnd, promocontent, cssPromoList[tempId], currentRow["BRAND"].ToString());
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
                    promoRow[L1_ID_COLUMN_INDEX] = oldIdL1;
                    promoRow[L1_COLUMN_INDEX] = GetLevelLabel(currentRow, 1, detailLevel);
                }
                if (nbLevels >= 2)
                {
                    promoRow[L2_ID_COLUMN_INDEX] = oldIdL2;
                    promoRow[L2_COLUMN_INDEX] = GetLevelLabel(currentRow, 2, detailLevel);
                }
                if (nbLevels >= 3)
                {
                    promoRow[L3_ID_COLUMN_INDEX] = oldIdL3;
                    promoRow[L3_COLUMN_INDEX] = GetLevelLabel(currentRow, 3, detailLevel);
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

                for (int i = 0; i < currentLevelPromoLineIndex.Count; i++)
                {
                    object[] promoRow = data[currentLevelPromoLineIndex[i]];

                    promoEndWeek.Increment();
                    while (!(promoStartWeek.Week == promoEndWeek.Week && promoStartWeek.Year == promoEndWeek.Year))
                    {
                        res = currentLevelPromoLineIndex[i];
                        long weekKey = promoStartWeek.Year * 100 + promoStartWeek.Week;
                        if (promoRow[weekList[weekKey]] != null)
                        {
                            res = -1; break;
                        }

                        promoStartWeek.Increment();
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
        /// <param name="promoContent"></param>
        /// <param name="nbCol"></param>
        /// <returns></returns>
        protected virtual string SplitContent(string promoContent, int nbCol)
        {
            string res = "";
            int length = promoContent.Length;
            int nbCharByCol = NB_CHAR_BY_COL;
            int nbLine = 2;

            if (!string.IsNullOrEmpty(promoContent) && (length > (nbCol * nbCharByCol * nbLine)))
            {
                int lenghtMax = (nbCol * nbCharByCol * nbLine) - ETC.Length;
                res = promoContent.Substring(0, lenghtMax) + ETC;
            }
            else return promoContent;

            return res;
        }
        #endregion


    }
}
