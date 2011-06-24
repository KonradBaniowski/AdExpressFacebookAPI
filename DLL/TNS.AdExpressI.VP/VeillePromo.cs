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
        #endregion

        #region Variables
        /// <summary>
        /// Client's session
        /// </summary>
        protected WebSession _session = null;
        /// <summary>
        /// Current module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(CstWeb.Module.Name.ANALYSE_PLAN_MEDIA);

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public VeillePromo(WebSession session)
        {
            if (session == null) throw new NullReferenceException(" parameter session cannot be null ");
            _session = session;
        }
        #endregion

        #region GetHtml
        /// <summary>
        /// Get HTML code for the promotion schedule
        /// </summary>
        /// <returns>HTML Code</returns>
        public virtual string GetHtml()
        {
            StringBuilder html = new StringBuilder(5000);
            int rowSpan =1,  colSpan=1; 
            VeillePromoScheduleData veillePromoScheduleData = GetData();


            if (veillePromoScheduleData != null && veillePromoScheduleData.Data != null && veillePromoScheduleData.Data.Count > 0)
            {
                //Generic Media Detail Level
                GenericDetailLevel detailLevel = _session.GenericMediaDetailLevel;

                //Number of levels
                int nbLevels = _session.GenericMediaDetailLevel.GetNbLevels;

                html.Append("<table cellspacing=\"0\" cellpadding=\"0\" >");

                //Start list of monthes
                TNS.FrameWork.Date.YearMonthWeekCalendar ymc = new YearMonthWeekCalendar(2010);


                
                html.Append("<tr>");
                html.AppendFormat("<td rowspan=\"{0}\" class=\"pt\"> &nbsp;Niveaux </td>", rowSpan);//TODO : Mettre libellé dans fichier ressources
                html.Append("</tr>");
                //End list of months

                //Start list of weeks

                //End list of weeks

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
            Int64 oldIdL1 = -1;
            Int64 oldIdL2 = -1;
            Int64 oldIdL3 = -1;
            bool newL1 = false, newL2 = false, newL3 = false, withPromoOverlap = false;
            List<int> currentLevelPromoLineIndex = null;
            VeillePromoItem veillePromoItem = null;
            CstResults.VeillePromo.itemType promoItemType = CstResults.VeillePromo.itemType.absent;
            int currentRowIndex = 0, oldRowIndex = -1;

            param = new object[1];
            param[0] = _session;
            vpScheduleDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            ds = vpScheduleDAL.GetData();

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null || ds.Tables[0].Rows.Count==0)
            {
                return null;
            }
            dt = ds.Tables[0];

            //Generic Media Detail Level
            GenericDetailLevel detailLevel = _session.GenericMediaDetailLevel;

            //Number of levels
            int nbLevels = _session.GenericMediaDetailLevel.GetNbLevels;

            //Get week list 
            DateTime periodBegin = DateString.YYYYMMDDToDateTime(_session.CustomerPeriodSelected.StartDate);
            DateTime periodEnd = DateString.YYYYMMDDToDateTime(_session.CustomerPeriodSelected.EndDate);
            AtomicPeriodWeek periodBeginWeek = new AtomicPeriodWeek(periodBegin);
            AtomicPeriodWeek periodEndWeek = new AtomicPeriodWeek(periodEnd);
            long beginWeekYYYYWW = Convert.ToInt64(DateString.AtomicPeriodWeekToYYYYWW(periodBeginWeek));
            long endWeekYYYYWW = Convert.ToInt64(DateString.AtomicPeriodWeekToYYYYWW(periodEndWeek));

            //Init Promo Schedule Data
            VeillePromoScheduleData vpData = new VeillePromoScheduleData(_session.CustomerPeriodSelected.StartDate, _session.CustomerPeriodSelected.EndDate, nbLevels);

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
                string promocontent = (currentRow["PROMOTION_CONTENT"] != System.DBNull.Value) ? currentRow["DATE_END_NUM"].ToString() : "";

                if (newL3 || newL2 || newL1)
                {
                    promoRow = new object[nbCol];
                    foreach (KeyValuePair<long, long> kpv in vpData.WeekList)
                    {
                        promoRow[kpv.Key] = new VeillePromoItem(CstResults.VeillePromo.itemType.absent);
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
                        foreach (KeyValuePair<long,long> kpv in vpData.WeekList)
                        {
                            promoRow[kpv.Key] =  new VeillePromoItem(CstResults.VeillePromo.itemType.absent);
                        }
                        currentRowIndex++;
                        withPromoOverlap = true;
                    }
                }
               
                promoEndWeek.Increment();
                int incr = 0;
                while (!(promoStartWeek.Week == promoEndWeek.Week && promoStartWeek.Year == promoEndWeek.Year))
                {
                    promoStartWeekYYYYWW = Convert.ToInt64(DateString.AtomicPeriodWeekToYYYYWW(promoStartWeek));


                    if (beginWeekYYYYWW <= promoStartWeekYYYYWW || promoStartWeekYYYYWW <= endWeekYYYYWW)
                    {
                        if (beginWeekYYYYWW == promoStartWeekYYYYWW && incr == 0)
                        {
                            promoItemType = CstResults.VeillePromo.itemType.presentStart;
                            veillePromoItem = new VeillePromoItem(promoItemType, Convert.ToInt64(currentRow["ID_DATA_PROMOTION"].ToString()), promoDateBegin, promoDateEnd, promocontent);
                        }
                        else if (beginWeekYYYYWW == promoStartWeekYYYYWW && incr > 0)
                        {
                            promoItemType = CstResults.VeillePromo.itemType.presentStartIncomplete;
                            veillePromoItem = new VeillePromoItem(promoItemType, Convert.ToInt64(currentRow["ID_DATA_PROMOTION"].ToString()), promoDateBegin, promoDateEnd, promocontent);
                        }
                        else if (endWeekYYYYWW == promoStartWeekYYYYWW && promoEndWeekYYYYWW > endWeekYYYYWW)
                        {
                            promoItemType = CstResults.VeillePromo.itemType.endIncomplete;
                            veillePromoItem = new VeillePromoItem(promoItemType);
                        }
                        else
                        {
                            promoItemType = CstResults.VeillePromo.itemType.extended;
                            veillePromoItem = new VeillePromoItem(promoItemType);
                        }
                        promoRow[vpData.WeekList[promoStartWeek.Year * 100 + promoStartWeek.Week]] = veillePromoItem;

                    }
                    promoStartWeek.Increment();
                    incr++;
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


    }
}
