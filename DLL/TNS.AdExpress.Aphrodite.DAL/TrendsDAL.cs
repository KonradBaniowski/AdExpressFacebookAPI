using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.Aphrodite.Domain;
using TNS.FrameWork.DB.Common;
using KMI.AdExpress.Aphrodite.Constantes;
using AdExpressConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.Date;

namespace KMI.AdExpress.Aphrodite.DAL {
    /// <summary>
    /// Trends DAL
    /// </summary>
    public class TrendsDAL : ITrendsDAL {

        #region BuildRemoveQuery
        /// <summary>
        /// Build Remove Query for a table;
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="queries"></param>
        protected void BuildRemoveQuery(string tableName,Int64 mediaTypeId,Queue<string> queries) {
            if(tableName.Length>0) {
                queries.Enqueue("Delete from "+tableName+" where id_vehicle="+mediaTypeId.ToString());
            }
        }
        /// <summary>
        /// Build Remove Query for a table; taking into account comparative type column
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="queries"></param>
        protected void BuildRemoveWeekQuery(string tableName, Int64 mediaTypeId, Application.TreatmentType treatmentType, Queue<string> queries) {
            if (tableName.Length > 0) {
                switch (treatmentType) {
                    case Application.TreatmentType.comparativeDateWeek:
                        queries.Enqueue("Delete from " + tableName + " where id_vehicle=" + mediaTypeId.ToString() + " and comparative_type = " + TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DEFAULT);
                        break;
                    case Application.TreatmentType.dateToDateWeek:
                        queries.Enqueue("Delete from " + tableName + " where id_vehicle=" + mediaTypeId.ToString() + " and comparative_type = " + TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DATE_TO_DATE);
                        break;
                    default:
                        throw new AphroditeDALException("Treatment type not defined");
                }
            }
        }
        #endregion

        #region Remove
        /// <summary>
        /// Remove trends data for a media
        /// </summary>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="source">Data source</param>
        public void Remove(MediaTypeInformation mediaTypeInformation, IDataSource source) {
            Remove(mediaTypeInformation, Application.TreatmentType.month, source);
        }
        /// <summary>
        /// Remove trends data for a media
        /// </summary>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="source">Data source</param>
        public void Remove(MediaTypeInformation mediaTypeInformation, Application.TreatmentType treatmentType, IDataSource source) {

            Queue<string> queries=new Queue<string>(4);

            #region Buid queries
            switch (treatmentType) {
                case Application.TreatmentType.month:
                    BuildRemoveQuery(mediaTypeInformation.MonthTrendsTable, mediaTypeInformation.DatabaseId, queries);
                    BuildRemoveQuery(mediaTypeInformation.TotalMonthTrendsTable, mediaTypeInformation.DatabaseId, queries);
                    break;
                case Application.TreatmentType.comparativeDateWeek:
                    BuildRemoveWeekQuery(mediaTypeInformation.WeekTrendsTable, mediaTypeInformation.DatabaseId, treatmentType, queries);
                    BuildRemoveWeekQuery(mediaTypeInformation.TotalWeekTrendsTable, mediaTypeInformation.DatabaseId, treatmentType, queries);
                    break;
                case Application.TreatmentType.dateToDateWeek:
                    BuildRemoveWeekQuery(mediaTypeInformation.WeekTrendsTable, mediaTypeInformation.DatabaseId, treatmentType, queries);
                    BuildRemoveWeekQuery(mediaTypeInformation.TotalWeekTrendsTable, mediaTypeInformation.DatabaseId, treatmentType, queries);
                    break;
                default:
                    break;
            }
            
            if(queries.Count==0) throw(new AphroditeDALException("Impossible to build data remove queries: Maybe bad Media type xml file"));
            #endregion

            #region Remove data
            string query=string.Empty;
            try {
                source.Open();
                lock(queries) {
                    while(queries.Count>0) {
                        query=queries.Dequeue();
                        source.Delete(query);
                    }
                }
            }
            catch(System.Exception err) {
                throw (new AphroditeDALException("Impossible to remove trends data: "+query,err));
            }
            finally {
                source.Close();
            }
            #endregion

        }
        #endregion

        #region Insert Data
        /// <summary>
        /// Insert Data
        /// </summary>
        /// <param name="periodBeginning"></param>
        /// <param name="periodEnding"></param>
        /// <param name="periodBeginningPrev"></param>
        /// <param name="periodEndingPrev"></param>
        /// <param name="periodId"></param>
        /// <param name="period"></param>
        /// <param name="year"></param>
        /// <param name="mediaTypeInformation"></param>
        /// <param name="cumul"></param>
        /// <param name="source"></param>
        public virtual void InsertData(string periodBeginning, string periodEnding, string periodBeginningPrev, string periodEndingPrev, string periodId, string period, string year, string comparativeType, MediaTypeInformation mediaTypeInformation, string cumul, Application.TreatmentType treatmentType, IDataSource source) {

            #region Variables
            string sql = "";
            string getUnitsForTotal = mediaTypeInformation.GetTotalUnitsInsertSQLFields;
            string getUnitsForSelectCur = mediaTypeInformation.GetUnitsSelectSQLForCurrentYear;
            string getUnitsForSelectPrev = mediaTypeInformation.GetUnitsSelectSQLForPreviousYear;
            string getUnitForInset = mediaTypeInformation.GetUnitsInsertSQLFields;
            string listMedia = mediaTypeInformation.MediaList;
            string listExcludeProduct = TNS.AdExpress.Constantes.DB.Hathor.LIST_EXCLUDE_PRODUCT;

            string tmp1 = "";
            string tmp2 = "";
            string tmp3 = "";
            string tmp4 = "";
            #endregion

            #region Construction de la requête
            switch (treatmentType) { 
                case Application.TreatmentType.month:
                    sql += "insert into " + mediaTypeInformation.MonthTrendsTable + " (id_media ";
                    break;
                case Application.TreatmentType.comparativeDateWeek:
                case Application.TreatmentType.dateToDateWeek:
                    sql += "insert into " + mediaTypeInformation.WeekTrendsTable + " (id_media ";
                    break;
                default:
                    throw new Exception("Treatment type not defined !!!");
            }
            sql += ",id_cumulative ";
            sql += ",date_period ";
            sql += ",id_pdm ";
            sql += ",id_period";
            sql += ",id_vehicle ";
            sql += ",id_category ";
            sql += ",category ";
            sql += ",media ";
            sql += " ,year ";
            sql += " ,comparative_type ";
            sql += getUnitForInset;
            sql += " ) ";

            #region tmp
            tmp2 += " and wp.date_media_num between " + periodBeginning + " and " + periodEnding + " ";
            tmp3 += " and wp.date_media_num between " + periodBeginningPrev + " and " + periodEndingPrev + " ";

            tmp1 += " from  " + AdExpressConstantes.Schema.ADEXPRESS_SCHEMA + ".MEDIA " + AdExpressConstantes.Tables.MEDIA_PREFIXE + ", " + mediaTypeInformation.DataTable + " wp,  " + AdExpressConstantes.Schema.ADEXPRESS_SCHEMA + ".basic_media " + AdExpressConstantes.Tables.BASIC_MEDIA_PREFIXE + ",  " + AdExpressConstantes.Schema.ADEXPRESS_SCHEMA + ".category " + AdExpressConstantes.Tables.CATEGORY_PREFIXE + "";
            tmp1 += " where   " + AdExpressConstantes.Tables.MEDIA_PREFIXE + ".id_media=wp.id_media ";
            tmp1 += " and " + AdExpressConstantes.Tables.MEDIA_PREFIXE + ".id_basic_media=" + AdExpressConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_basic_media ";
            tmp1 += " and " + AdExpressConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_category=" + AdExpressConstantes.Tables.CATEGORY_PREFIXE + ".id_category ";

            tmp4 += " and " + AdExpressConstantes.Tables.MEDIA_PREFIXE + ".id_language=33 ";
            tmp4 += " and " + AdExpressConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_language=33 ";
            tmp4 += " and " + AdExpressConstantes.Tables.CATEGORY_PREFIXE + ".id_language=33 ";
            tmp4 += " and wp.id_media in(" + listMedia + ")";
            tmp4 += " and wp.id_product not in(" + listExcludeProduct + ")";
            tmp4 += " group by " + AdExpressConstantes.Tables.MEDIA_PREFIXE + ".id_media,";
            tmp4 += " media," + AdExpressConstantes.Tables.CATEGORY_PREFIXE + ".id_category,category)";
            #endregion


            sql += "select id_media," + cumul + "," + period + ",";
            sql += " " + AdExpressConstantes.Hathor.PDM_FALSE + "," + periodId + "," + mediaTypeInformation.DatabaseId + "";
            sql += ",id_category,category,media," + year + "," + comparativeType + ",";
            sql += getUnitsForTotal;
            sql += " from((select  md.id_media , media ,ct.id_category,ct.category ,";
            sql += getUnitsForSelectCur;
            sql += tmp1 + tmp2 + tmp4;
            // union
            sql += " union (select  md.id_media , media ,ct.id_category,ct.category ,";
            sql += getUnitsForSelectPrev;
            sql += tmp1 + tmp3 + tmp4;
            sql += " )group by id_media, media,id_category,category";
            #endregion

            try {
                source.Insert(sql);
            }
            catch (System.Exception err) {
                throw (new AphroditeDALException("Imposible to insert data :" + sql, err));
            }

        }
        #endregion

        #region Insertion des sous-totaux
        /// <summary>
		/// Insertion des Sous totaux dans la table
		/// </summary>
		/// <param name="datePeriode">periode YYYYMM</param>
		/// <param name="connection">Connection Oracle</param>
		/// <param name="vehicle">Vehicle</param>
        /// <param name="treatmentType">Treatment Type</param>
        public virtual void InsertSubTotal(string datePeriode, IDataSource source, MediaTypeInformation mediaTypeInformation, bool total, string cumul, string type_tendency, Application.TreatmentType treatmentType) {

            #region Variables
            string sql = "";
            string getUnitsForTotal = mediaTypeInformation.GetTotalUnitsInsertSQLFields;
            string getUnit = mediaTypeInformation.GetSubTotalUnitsSQLFields;
            string getUnitInsert = mediaTypeInformation.GetUnitsInsertSQLFields;
            string totalTrendsTableLabel = string.Empty;
            string totalTrendsTableLabelWithSchema = string.Empty;
            string trendsTableLabelWithSchema = string.Empty;
            string comparativeType = string.Empty;
            #endregion

            switch (treatmentType) {
                case Application.TreatmentType.month:
                    totalTrendsTableLabel = mediaTypeInformation.TotalMonthTrendsTable.Substring(mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.') + 1, mediaTypeInformation.TotalMonthTrendsTable.Length - mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.') - 1);
                    totalTrendsTableLabelWithSchema = mediaTypeInformation.TotalMonthTrendsTable;
                    trendsTableLabelWithSchema = mediaTypeInformation.MonthTrendsTable;
                    break;
                case Application.TreatmentType.comparativeDateWeek:
                    totalTrendsTableLabel = mediaTypeInformation.TotalWeekTrendsTable.Substring(mediaTypeInformation.TotalWeekTrendsTable.IndexOf('.') + 1, mediaTypeInformation.TotalWeekTrendsTable.Length - mediaTypeInformation.TotalWeekTrendsTable.IndexOf('.') - 1);
                    totalTrendsTableLabelWithSchema = mediaTypeInformation.TotalWeekTrendsTable;
                    trendsTableLabelWithSchema = mediaTypeInformation.WeekTrendsTable;
                    comparativeType = TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DEFAULT;
                    break;
                case Application.TreatmentType.dateToDateWeek:
                    totalTrendsTableLabel = mediaTypeInformation.TotalWeekTrendsTable.Substring(mediaTypeInformation.TotalWeekTrendsTable.IndexOf('.') + 1, mediaTypeInformation.TotalWeekTrendsTable.Length - mediaTypeInformation.TotalWeekTrendsTable.IndexOf('.') - 1);
                    totalTrendsTableLabelWithSchema = mediaTypeInformation.TotalWeekTrendsTable;
                    trendsTableLabelWithSchema = mediaTypeInformation.WeekTrendsTable;
                    comparativeType = TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DATE_TO_DATE;
                    break;
                default:
                    throw new Exception("Treatment type not defined !!!");
            }

            #region Construction de la requête

            sql += " insert into " + totalTrendsTableLabelWithSchema + " ";
            sql += " ( id_" + totalTrendsTableLabel + " ,id_pdm";
            sql += " ,id_cumulative,id_period ";
            sql += " ,id_type_tendency";
            if (!total) {
                sql += " ,id_category";
            }
            sql += " ,id_vehicle";
            if (!total) {
                sql += " ,category";
            }
            sql += " , year, comparative_type, date_period";
            sql += getUnitInsert;
            sql += ")";
            if (!total) {
                sql += "select date_period||id_pdm||id_cumulative||" + type_tendency + "||id_category||id_vehicle||comparative_type";
            }
            else {
                sql += "select date_period||id_pdm||id_vehicle||id_cumulative||" + type_tendency + "||comparative_type";
            }

            sql += " ,id_pdm,id_cumulative,id_period";
            sql += "," + type_tendency + "";
            if (!total) {
                sql += " ,id_category";
            }
            sql += " ,id_vehicle";
            if (!total) {
                sql += ",category";
            }
            sql += " , year, comparative_type, date_period,";
            sql += getUnitsForTotal;
            sql += " from(select ";
            if (!total) {
                sql += " id_category,category, ";
            }
            sql += " id_pdm,id_period, ";
            sql += "id_cumulative,date_period,id_vehicle,year,comparative_type";
            sql += getUnit;
            sql += " from " + trendsTableLabelWithSchema;
            sql += " where id_vehicle=" + mediaTypeInformation.DatabaseId;

            if (datePeriode != AdExpressConstantes.Hathor.DATE_PERIOD_CUMULATIVE) {
                sql += " and date_period=" + datePeriode;
            }
            sql += " and id_pdm=" + AdExpressConstantes.Hathor.PDM_FALSE;
            sql += " and id_cumulative=" + cumul;
            if (treatmentType != Application.TreatmentType.month)
                sql += " and comparative_type=" + comparativeType;
            sql += " group by ";
            if (!total) {
                sql += "	id_category,category, ";
            }
            sql += "id_pdm,id_cumulative,date_period,id_vehicle,year,comparative_type,id_period";
            sql += " ) ";
            sql += " group by ";
            if (!total) {
                sql += " id_category,category,";
            }
            sql += " id_pdm,id_cumulative,date_period,id_vehicle,year,comparative_type,id_period";

            #endregion

            try {
                source.Insert(sql);
            }
            catch (System.Exception err) {
                throw (new AphroditeDALException("Imposible to insert subtotal data :" + sql, err));
            }

		}
		#endregion

        #region Insert Cumul
        /// <summary>
        /// Compute periods and insert cumul data in table month or week
        /// </summary>
        /// <param name="currentDay">Current Day</param>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="treatmentType">Treatment Type</param>
        /// <param name="source">Data source</param>
        public virtual void InsertCumul(MediaTypeInformation mediaTypeInformation, DateTime currentDay, Application.TreatmentType treatmentType, IDataSource source) {

            string period = "";
            string year = "";
            // Format MM
            string periodId = "";
            // Format YYYYMMDD
            string periodBeginning = "";
            string periodEnding = "";
            string periodBeginningPrev = "";
            string periodEndingPrev = "";
            string comparativeType = string.Empty;
            DateTime previousWeekFirstDay;

            string cumul = TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_TRUE;

            periodId = TNS.AdExpress.Constantes.DB.Hathor.ID_PERIOD_CUMULATIVE;
            year = TNS.AdExpress.Constantes.DB.Hathor.YEAR_PERIOD_CUMULATIVE;

            TNS.FrameWork.Date.AtomicPeriodWeek weekCurrent = new AtomicPeriodWeek(DateTime.Now);
            TNS.FrameWork.Date.AtomicPeriodWeek weekCurrentPrec = new AtomicPeriodWeek(DateTime.Now.AddYears(-1));

            weekCurrent.SubWeek(1);
            weekCurrentPrec.SubWeek(1);

            string weekString = weekCurrent.Week.ToString();
            if (weekString.Length == 1) weekString = "0" + weekString;
            period = weekCurrent.Year.ToString() + weekString;

            //periodBeginning = weekCurrent.FirstDay.Year.ToString() + "0101";
            periodBeginning = weekCurrent.Year.ToString() + "0101";
            periodEnding = weekCurrent.LastDay.ToString("yyyyMMdd");

            //periodBeginningPrev = weekCurrentPrec.FirstDay.Year.ToString() + "0101";
            periodBeginningPrev = weekCurrentPrec.Year.ToString() + "0101";

            switch (treatmentType) {
                case Application.TreatmentType.month:
                    periodEndingPrev = weekCurrentPrec.LastDay.ToString("yyyyMMdd");
                    comparativeType = TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DEFAULT;
                    InsertData(periodBeginning, periodEnding, periodBeginningPrev, periodEndingPrev, periodId, period, year, comparativeType, mediaTypeInformation, cumul, treatmentType, source);

                    if (weekCurrent.FirstDay.Month == 2 && weekCurrent.FirstDay.Day == 29)
                        previousWeekFirstDay = new DateTime(weekCurrent.FirstDay.Year - 1, 3, 1);
                    else
                        previousWeekFirstDay = new DateTime(weekCurrent.FirstDay.Year - 1, weekCurrent.FirstDay.Month, weekCurrent.FirstDay.Day);
                    periodEndingPrev = SetPreviousDateToDateWeek(previousWeekFirstDay).ToString("yyyyMMdd");
                    comparativeType = TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DATE_TO_DATE;
                    InsertData(periodBeginning, periodEnding, periodBeginningPrev, periodEndingPrev, periodId, period, year, comparativeType, mediaTypeInformation, cumul, treatmentType, source);
                    break;
                case Application.TreatmentType.comparativeDateWeek:
                    periodEndingPrev = weekCurrentPrec.LastDay.ToString("yyyyMMdd");
                    comparativeType = TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DEFAULT;
                    InsertData(periodBeginning, periodEnding, periodBeginningPrev, periodEndingPrev, periodId, period, year, comparativeType, mediaTypeInformation, cumul, treatmentType, source);
                    break;
                case Application.TreatmentType.dateToDateWeek:
                    if (weekCurrent.FirstDay.Month == 2 && weekCurrent.FirstDay.Day == 29)
                        previousWeekFirstDay = new DateTime(weekCurrent.FirstDay.Year - 1, 3, 1);
                    else
                        previousWeekFirstDay = new DateTime(weekCurrent.FirstDay.Year - 1, weekCurrent.FirstDay.Month, weekCurrent.FirstDay.Day);
                    periodEndingPrev = SetPreviousDateToDateWeek(previousWeekFirstDay).ToString("yyyyMMdd");
                    comparativeType = TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DATE_TO_DATE;
                    InsertData(periodBeginning, periodEnding, periodBeginningPrev, periodEndingPrev, periodId, period, year, comparativeType, mediaTypeInformation, cumul, treatmentType, source);
                    break;
                default:
                    throw new AphroditeDALException("Treatment type not defined !!!");
            }

        }
        #endregion

        #region InsertPDM
        public virtual void InsertPDM(MediaTypeInformation mediaTypeInformation, IDataSource source, bool total, Application.TreatmentType treatmentType) {

            #region Variables
            string sql = "";
            string getFirstSelect = GetFirstSelectPdm(mediaTypeInformation);
            string getSecondSelect = GetSecondSelectPdm(mediaTypeInformation);
            string getThirdSelect = GetThirdSelectPdm(mediaTypeInformation);
            string getUnitForInset = GetUnitsForInsertPdm(mediaTypeInformation);
            string trendsTableLabelWithSchema = string.Empty;
            string totalTrendsTableLabelWithSchema = string.Empty;
            string totalTrendsTableLabel = string.Empty;
            string comparativeType = string.Empty;
            #endregion

            switch (treatmentType) {
                case Application.TreatmentType.month:
                    trendsTableLabelWithSchema = mediaTypeInformation.MonthTrendsTable;
                    totalTrendsTableLabelWithSchema = mediaTypeInformation.TotalMonthTrendsTable;
                    totalTrendsTableLabel = mediaTypeInformation.TotalMonthTrendsTable.Substring(mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.') + 1, mediaTypeInformation.TotalMonthTrendsTable.Length - mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.') - 1);
                    break;
                case Application.TreatmentType.comparativeDateWeek:
                    trendsTableLabelWithSchema = mediaTypeInformation.WeekTrendsTable;
                    totalTrendsTableLabelWithSchema = mediaTypeInformation.TotalWeekTrendsTable;
                    totalTrendsTableLabel = mediaTypeInformation.TotalWeekTrendsTable.Substring(mediaTypeInformation.TotalWeekTrendsTable.IndexOf('.') + 1, mediaTypeInformation.TotalWeekTrendsTable.Length - mediaTypeInformation.TotalWeekTrendsTable.IndexOf('.') - 1);
                    comparativeType = TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DEFAULT;
                    break;
                case Application.TreatmentType.dateToDateWeek:
                    trendsTableLabelWithSchema = mediaTypeInformation.WeekTrendsTable;
                    totalTrendsTableLabelWithSchema = mediaTypeInformation.TotalWeekTrendsTable;
                    totalTrendsTableLabel = mediaTypeInformation.TotalWeekTrendsTable.Substring(mediaTypeInformation.TotalWeekTrendsTable.IndexOf('.') + 1, mediaTypeInformation.TotalWeekTrendsTable.Length - mediaTypeInformation.TotalWeekTrendsTable.IndexOf('.') - 1);
                    comparativeType = TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DATE_TO_DATE;
                    break;
                default:
                    throw new AphroditeDALException("Treatment type not defined !!!");
            }

            #region Construction de la requête

            #region Insert tendency
            if (!total) {
                sql += "insert into " + trendsTableLabelWithSchema + " ( ";
                sql += " id_media ,";
                sql += "id_cumulative ";
                sql += ",date_period ";
                sql += ",id_pdm ";
                sql += ",id_period";
                sql += ",id_vehicle ";
                sql += ",id_category ";
                sql += ",category ";
                sql += ",media ";
                sql += " ,year ";
                sql += " ,comparative_type ";
                sql += getUnitForInset;
                sql += " ) ";
            }
            #endregion

            #region Insert Total
            if (total) {
                sql += "insert into " + totalTrendsTableLabelWithSchema + " ( ";
                sql += " id_" + totalTrendsTableLabel + " ";
                sql += " ,id_type_tendency";
                sql += " ,id_cumulative ";
                sql += " ,date_period ";
                sql += " ,id_pdm ";
                sql += " ,id_period";
                sql += " ,id_vehicle ";
                sql += " ,id_category ";
                sql += " ,category ";
                sql += " ,year ";
                sql += " ,comparative_type ";
                sql += getUnitForInset;
                sql += " ) ";
            }
            #endregion

            sql += " select ";
            if (total) {
                sql += " date_period||0||id_cumulative||id_type_tendency||id_category||id_vehicle||comparative_type ";
                sql += " ,id_type_tendency,";
            }
            if (!total) {
                sql += " id_media, ";
            }
            sql += " id_cumulative ";
            sql += " ,date_period," + AdExpressConstantes.Hathor.PDM_TRUE + " ";
            sql += " ,id_period ";
            sql += " ,id_vehicle, id_category";
            sql += " ,category";
            if (!total) {
                sql += " ,media ";
            }
            sql += " ,year ";
            sql += " ,comparative_type ";
            sql += getFirstSelect;
            sql += " from( ";
            sql += " select ";
            if (!total) {
                sql += " id_media ,";
            }

            sql += "id_type_tendency ";
            sql += ",id_cumulative ";
            sql += " ,date_period," + AdExpressConstantes.Hathor.PDM_TRUE + " ";
            sql += " ,id_period ";
            sql += " ,id_vehicle, id_category";
            sql += " ,category";
            if (!total) {
                sql += " ,media ";
            }
            sql += " ,year ";
            sql += " ,comparative_type ";
            sql += getSecondSelect;
            sql += " from( ";

            sql += " select ";
            if (!total) {
                sql += "" + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_media,";
            }

            sql += " " + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_cumulative ";

            if (total) {
                sql += " ," + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_type_tendency ";
            }
            else {
                sql += " ," + AdExpressConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".id_type_tendency ";
            }
            sql += " ," + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_vehicle," + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_category";
            sql += " ," + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_period ";
            sql += " ," + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".category ";
            sql += " ," + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".year";
            sql += " ," + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".comparative_type";
            sql += " ," + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".date_period ";
            if (!total) {
                sql += " ," + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".media ";
            }
            sql += getThirdSelect;

            sql += " from ";

            sql += " " + totalTrendsTableLabelWithSchema + " " + AdExpressConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + " ";
            if (!total) {
                sql += "," + trendsTableLabelWithSchema + " " + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + " ";
            }
            else {
                sql += "," + totalTrendsTableLabelWithSchema + " " + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + " ";
            }
            sql += " where " + AdExpressConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".id_type_tendency=0 ";
            sql += " and " + AdExpressConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".id_pdm=10 ";
            if (treatmentType != Application.TreatmentType.month) {
                sql += " and " + AdExpressConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".comparative_type= " + comparativeType + " ";
                sql += " and " + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".comparative_type= " + comparativeType + " ";
            }
            sql += " and " + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_vehicle=" + mediaTypeInformation.DatabaseId;
            sql += " and " + AdExpressConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".id_vehicle=" + mediaTypeInformation.DatabaseId;
            sql += " and " + AdExpressConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".date_period=" + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".date_period ";
            sql += " and " + AdExpressConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".id_cumulative=" + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_cumulative ";
            sql += " and " + AdExpressConstantes.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".comparative_type=" + AdExpressConstantes.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".comparative_type ";
            sql += " ) ";
            sql += " ) ";

            #endregion

            try {
                source.Insert(sql);
            }
            catch (System.Exception err) {
                throw (new AphroditeDALException("Imposible to insert PDM data :" + sql, err));
            }

        }
        #endregion

        #region Pdm
        /// <summary>
        /// Génére le code sql pour le select pour le calcul du pdm
        /// </summary>
        /// <param name="vehicle">vehicle</param>
        /// <returns></returns>
        protected string GetFirstSelectPdm(MediaTypeInformation mediaTypeInformation) {

            #region Variables
            string sql = "";
            int lenghtUnit = 0;
            #endregion

            for (int i = 0; i < mediaTypeInformation.ListCurrentUnit.Count; i++) {
                lenghtUnit = mediaTypeInformation.ListCurrentUnit[i].Length - 4;
                sql += "," + mediaTypeInformation.ListCurrentUnit[i];
                sql += "," + mediaTypeInformation.ListPreviousUnit[i];
                sql += "," + " decode(" + mediaTypeInformation.ListPreviousUnit[i] + ",0,100,((" + mediaTypeInformation.ListCurrentUnit[i] + "-" + mediaTypeInformation.ListPreviousUnit[i] + ")/" + mediaTypeInformation.ListPreviousUnit[i] + ")*100) as " + mediaTypeInformation.ListCurrentUnit[i].Substring(0, lenghtUnit) + "_evol";
            }

            return sql;

        }

        /// <summary>
        /// Génère le code sql pour le deuxième select pour le pdm
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <returns></returns>
        protected string GetSecondSelectPdm(MediaTypeInformation mediaTypeInformation) {

            #region Variables
            string sql = "";
            #endregion

            for (int i = 0; i < mediaTypeInformation.ListCurrentUnit.Count; i++) {
                sql += ", decode(total_" + mediaTypeInformation.ListCurrentUnit[i] + ",0,100," + mediaTypeInformation.ListCurrentUnit[i] + "/total_" + mediaTypeInformation.ListCurrentUnit[i] + "*100) as " + mediaTypeInformation.ListCurrentUnit[i] + "";
                sql += ", decode(total_" + mediaTypeInformation.ListPreviousUnit[i] + ",0,100," + mediaTypeInformation.ListPreviousUnit[i] + "/total_" + mediaTypeInformation.ListPreviousUnit[i] + "*100) as " + mediaTypeInformation.ListPreviousUnit[i] + "";
            }

            return sql;
        }

        /// <summary>
        /// Génère le code sql 
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <returns></returns>
        protected string GetThirdSelectPdm(MediaTypeInformation mediaTypeInformation) {

            #region Variables
            string sql = "";
            #endregion


            for (int i = 0; i < mediaTypeInformation.ListCurrentUnit.Count; i++) {
                sql += " ," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + "." + mediaTypeInformation.ListCurrentUnit[i] + " as total_" + mediaTypeInformation.ListCurrentUnit[i] + " ";
                sql += " ," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + "." + mediaTypeInformation.ListCurrentUnit[i] + " as " + mediaTypeInformation.ListCurrentUnit[i] + " ";
                sql += "," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + "." + mediaTypeInformation.ListPreviousUnit[i] + " as total_" + mediaTypeInformation.ListPreviousUnit[i] + " ";
                sql += "," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + "." + mediaTypeInformation.ListPreviousUnit[i] + " as " + mediaTypeInformation.ListPreviousUnit[i] + " ";
            }

            return sql;

        }

        /// <summary>
        /// Génère le code sql pour l'insert
        /// </summary>
        /// <param name="vehicle">vehicle</param>
        /// <returns></returns>
        protected string GetUnitsForInsertPdm(MediaTypeInformation mediaTypeInformation) {

            #region Variables
            string sql = "";
            int lenghtUnit = 0;
            #endregion

            for (int i = 0; i < mediaTypeInformation.ListCurrentUnit.Count; i++) {
                lenghtUnit = mediaTypeInformation.ListCurrentUnit[i].Length - 4;
                sql += "," + mediaTypeInformation.ListCurrentUnit[i];
                sql += "," + mediaTypeInformation.ListPreviousUnit[i];
                sql += "," + mediaTypeInformation.ListCurrentUnit[i].Substring(0, lenghtUnit) + "_evol";
            }

            return sql;
        }
        #endregion

        #region Set previous date to date week
        /// <summary>
        /// Set previous date to date week
        /// </summary>
        /// <returns>Last day</returns>
        private DateTime SetPreviousDateToDateWeek(DateTime firstDay) {

            DateTime lastDate = firstDay;

            for (int i = 1; i <= 6; i++)
                lastDate = lastDate.AddDays(1);

            return lastDate;
        }
        #endregion

    }
}
