using System;
using System.Collections.Generic;
using KMI.AdExpress.Aphrodite.DAL;
using KMI.AdExpress.Aphrodite.Domain;
using TNS.FrameWork.DB.Common;
using KMI.AdExpress.Aphrodite.Constantes;

namespace KMI.AdExpress.Aphrodite.DAL.Finland {
    public class TrendsDAL : DAL.TrendsDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public TrendsDAL() : base() {
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
        /// <param name="comparativeType">Comparative Type</param>
        /// <param name="treatmentType">Treatment type</param>
        /// <param name="mediaTypeInformation"></param>
        /// <param name="cumul"></param>
        /// <param name="source"></param>
        public override void InsertData(string periodBeginning, string periodEnding, string periodBeginningPrev, string periodEndingPrev, string periodId, string period, string year, string comparativeType, MediaTypeInformation mediaTypeInformation, string cumul, Application.TreatmentType treatmentType, IDataSource source) {

            #region Variables

            string sql = "";
            string getUnitsForTotal = mediaTypeInformation.GetTotalUnitsInsertSQLFields;
            string getUnitsForSelectCur = mediaTypeInformation.GetUnitsSelectSQLForCurrentYear;
            string getUnitsForSelectPrev = mediaTypeInformation.GetUnitsSelectSQLForPreviousYear;
            string getUnitForInset = mediaTypeInformation.GetUnitsInsertSQLFields;
            //string listMedia=SqlGenerator.GetListMedia(vehicle);
            //string listMedia=vehicle.MediaList;
            //string listExcludeProduct=TNS.AdExpress.Hathor.Constantes.Constantes.LIST_EXCLUDE_PRODUCT;

            //string tmp1="";
            string tmp2 = "";
            string tmp3 = "";
            string tmp4 = "";
            #endregion

            #region Construction de la requête
            sql += "insert into " + mediaTypeInformation.MonthTrendsTable + " (id_media ";
            sql += ",id_cumulative ";
            sql += ",date_period ";
            sql += ",id_pdm ";
            sql += ",id_period";
            sql += ",id_vehicle ";
            sql += ",id_category ";
            sql += ",category ";
            sql += ",media ";
            sql += " ,year ";
            sql += getUnitForInset;
            sql += " ) ";

            #region tmp
            tmp2 += " date_media_num between " + periodBeginning + " and " + periodEnding + " ";
            tmp3 += " date_media_num between " + periodBeginningPrev + " and " + periodEndingPrev + " ";

            //tmp4+=" and wp.id_media in("+listMedia+")";
            //tmp4+=" and wp.id_product not in("+listExcludeProduct+")";
            tmp4 += " group by id_media,id_category ";

            #endregion


            sql += "select id_media," + cumul + "," + period + ",";
            sql += " " + TNS.AdExpress.Constantes.DB.Hathor.PDM_FALSE + "," + periodId + "," + mediaTypeInformation.DatabaseId + "";
            sql += ",id_category,category,media," + year + ",";
            sql += getUnitsForTotal;
            sql += " from((select id_media,'USE DB CLASSIFICATION' as media,id_category,'USE DB CLASSIFICATION' as category,";
            sql += getUnitsForSelectCur;
            sql += " from " + mediaTypeInformation.DataTable + " where ";
            //sql+=tmp1+tmp2+tmp4;
            sql += tmp2 + tmp4;
            // union
            sql += " )union (select  id_media,'USE DB CLASSIFICATION'as media, id_category,'USE DB CLASSIFICATION' as category,";
            sql += getUnitsForSelectPrev;
            sql += " from " + mediaTypeInformation.DataTable + " where ";
            sql += tmp3 + tmp4;
            sql += ") )group by id_media,media,id_category,category";
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
        public override void InsertSubTotal(string datePeriode, IDataSource source, MediaTypeInformation mediaTypeInformation, bool total, string cumul, string type_tendency, Application.TreatmentType treatmentType) {

            #region Variables


            string sql = "";
            string getUnitsForTotal = mediaTypeInformation.GetTotalUnitsInsertSQLFields;
            string getUnit = mediaTypeInformation.GetSubTotalUnitsSQLFields;
            string getUnitInsert = mediaTypeInformation.GetUnitsInsertSQLFields;
            string totalTrendsTableLabel = mediaTypeInformation.TotalMonthTrendsTable.Substring(mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.') + 1, mediaTypeInformation.TotalMonthTrendsTable.Length - mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.') - 1);
            string totalTrendsTableLabelWithSchema = mediaTypeInformation.TotalMonthTrendsTable;
            string trendsTableLabelWithSchema = mediaTypeInformation.MonthTrendsTable;
            #endregion

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
            sql += " ,year,date_period";
            sql += getUnitInsert;
            sql += ")";
            if (!total) {
                sql += "select date_period||id_pdm||id_cumulative||" + type_tendency + "||id_category||id_vehicle";
            }
            else {
                sql += "select date_period||id_pdm||id_vehicle||id_cumulative||" + type_tendency + "";
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
            sql += " ,year,date_period,";
            sql += getUnitsForTotal;
            sql += " from(select ";
            if (!total) {
                sql += " id_category,category, ";
            }
            sql += " id_pdm,id_period, ";
            sql += "id_cumulative,date_period,id_vehicle,year";
            sql += getUnit;
            sql += " from " + trendsTableLabelWithSchema;
            sql += " where id_vehicle=" + mediaTypeInformation.DatabaseId;

            if (datePeriode != TNS.AdExpress.Constantes.DB.Hathor.DATE_PERIOD_CUMULATIVE) {
                sql += " and date_period=" + datePeriode;
            }
            sql += " and id_pdm=" + TNS.AdExpress.Constantes.DB.Hathor.PDM_FALSE;
            sql += " and id_cumulative=" + cumul;
            sql += " group by ";
            if (!total) {
                sql += "	id_category,category, ";
            }
            sql += "id_pdm,id_cumulative,date_period,id_vehicle,year,id_period";
            sql += " ) ";
            sql += " group by ";
            if (!total) {
                sql += " id_category,category,";
            }
            sql += " id_pdm,id_cumulative,date_period,id_vehicle,year,id_period";

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
        /// Compute periods and insert cumul data in table month
        /// </summary>
        /// <param name="currentDay">Current Day</param>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="treatmentType">Treatment Type</param>
        /// <param name="source">Data source</param>
        public override void InsertCumul(MediaTypeInformation mediaTypeInformation, DateTime currentDay, Application.TreatmentType treatmentType, IDataSource source) {

            string periodBeginning = "";
            string periodEnding = "";
            string periodBeginningPrev = "";
            string periodEndingPrev = "";
            string cumul = TNS.AdExpress.Constantes.DB.Hathor.CUMULATIVE_TRUE;
            string periodId = TNS.AdExpress.Constantes.DB.Hathor.ID_PERIOD_CUMULATIVE;
            string year = TNS.AdExpress.Constantes.DB.Hathor.YEAR_PERIOD_CUMULATIVE;
            string comparativeType = string.Empty;

            switch (treatmentType) {
                case Application.TreatmentType.month:
                case Application.TreatmentType.comparativeDateWeek:
                    comparativeType = TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DEFAULT;
                    break;
                case Application.TreatmentType.dateToDateWeek:
                    comparativeType = TNS.AdExpress.Constantes.DB.Hathor.COMPARATIVE_TYPE_DATE_TO_DATE;
                    break;
                default:
                    throw new AphroditeDALException("Treatment type not defined !!!");
            }

            DateTime dT = currentDay.AddMonths(-1);
            string studyYear = dT.Year.ToString();

            DateTime LastDate = new DateTime(currentDay.Year, currentDay.Month, 1).AddDays(-1);
            periodEnding = LastDate.Year.ToString();
            if (LastDate.Month.ToString().Length < 2) periodEnding += "0";
            periodEnding += LastDate.Month.ToString() + LastDate.Day.ToString();

            DateTime LastDatePreviousPeriod = new DateTime(currentDay.AddYears(-1).Year, currentDay.Month, 1).AddDays(-1);
            periodEndingPrev = LastDatePreviousPeriod.Year.ToString();
            if (LastDatePreviousPeriod.Month.ToString().Length < 2) periodEndingPrev += "0";
            periodEndingPrev += LastDatePreviousPeriod.Month.ToString() + LastDatePreviousPeriod.Day.ToString();

            periodBeginning = studyYear + "0101";
            string previousStudyYear = dT.AddYears(-1).Year.ToString();
            periodBeginningPrev = previousStudyYear + "0101";

            string period = studyYear;
            if (dT.Month.ToString().Length < 2) period += "0";
            period += dT.Month.ToString();

            InsertData(periodBeginning, periodEnding, periodBeginningPrev, periodEndingPrev, periodId, period, year, comparativeType, mediaTypeInformation, cumul, treatmentType, source);
        }
        #endregion

        #region InsertPDM
        public override void InsertPDM(MediaTypeInformation mediaTypeInformation, IDataSource source, bool total, Application.TreatmentType treatmentType) {

            #region Variables

            string sql = "";
            string getFirstSelect = GetFirstSelectPdm(mediaTypeInformation);
            string getSecondSelect = GetSecondSelectPdm(mediaTypeInformation);
            string getThirdSelect = GetThirdSelectPdm(mediaTypeInformation);
            string getUnitForInset = GetUnitsForInsertPdm(mediaTypeInformation);
            string trendsTableLabelWithSchema = mediaTypeInformation.MonthTrendsTable;
            string totalTrendsTableLabelWithSchema = mediaTypeInformation.TotalMonthTrendsTable;
            string totalTrendsTableLabel = mediaTypeInformation.TotalMonthTrendsTable.Substring(mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.') + 1, mediaTypeInformation.TotalMonthTrendsTable.Length - mediaTypeInformation.TotalMonthTrendsTable.IndexOf('.') - 1);

            #endregion

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
                sql += getUnitForInset;
                sql += " ) ";
            }
            #endregion

            sql += " select ";
            if (total) {
                sql += " date_period||0||id_cumulative||id_type_tendency||id_category||id_vehicle ";
                sql += " ,id_type_tendency,";
            }
            if (!total) {
                sql += " id_media, ";
            }
            sql += " id_cumulative ";
            sql += " ,date_period," + TNS.AdExpress.Constantes.DB.Hathor.PDM_TRUE + " ";
            sql += " ,id_period ";
            sql += " ,id_vehicle, id_category";
            sql += " ,'USE DB CLASSIFICATION' as category";
            if (!total) {
                sql += " ,'USE DB CLASSIFICATION' as media ";
            }
            sql += " ,year";
            sql += getFirstSelect;
            sql += " from( ";
            sql += " select ";
            if (!total) {
                sql += " id_media ,";
            }

            sql += "id_type_tendency ";
            sql += ",id_cumulative ";
            sql += " ,date_period," + TNS.AdExpress.Constantes.DB.Hathor.PDM_TRUE + " ";
            sql += " ,id_period ";
            sql += " ,id_vehicle, id_category";
            sql += " ,category";
            if (!total) {
                sql += " ,media ";
            }
            sql += " ,year";
            sql += getSecondSelect;
            sql += " from( ";

            sql += " select ";
            if (!total) {
                sql += "" + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_media,";
            }

            sql += " " + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_cumulative ";

            if (total) {
                sql += " ," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_type_tendency ";
            }
            else {
                sql += " ," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".id_type_tendency ";
            }
            sql += " ," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_vehicle," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_category";
            sql += " ," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_period ";
            sql += " ," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".category ";
            sql += " ," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".year";
            sql += " ," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".date_period ";
            if (!total) {
                sql += " ," + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".media ";
            }
            sql += getThirdSelect;

            sql += " from ";

            sql += " " + totalTrendsTableLabelWithSchema + " " + TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + " ";
            if (!total) {
                sql += "," + trendsTableLabelWithSchema + " " + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + " ";
            }
            else {
                sql += "," + totalTrendsTableLabelWithSchema + " " + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + " ";
            }
            sql += " where " + TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".id_type_tendency=0 ";
            sql += " and " + TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".id_pdm=10 ";
            sql += " and " + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_vehicle=" + mediaTypeInformation.DatabaseId;
            sql += " and " + TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".id_vehicle=" + mediaTypeInformation.DatabaseId;
            sql += " and " + TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".date_period=" + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".date_period ";
            sql += " and " + TNS.AdExpress.Constantes.DB.Hathor.Tables.TOTAL_TENDENCY_MONTH_PREFIXE + ".id_cumulative=" + TNS.AdExpress.Constantes.DB.Hathor.Tables.TENDENCY_MONTH_PREFIXE + ".id_cumulative ";
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

    }
}
