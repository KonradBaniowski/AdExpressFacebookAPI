#region Informations
/*
 * Author : G. Facon & G. Ragneau
 * Created : 21/11/2007
 * Modifications :
 *      Author - Date - Descriptopn
 *      
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;
using ConstantesDB = TNS.AdExpress.Constantes.DB;
using ConstantesDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using ConstantesWeb = TNS.AdExpress.Constantes.Web;
using ConstanteCustomerRight = TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Exceptions;


namespace TNS.AdExpress.Web.DataAccess.Results
{
    /// <summary>
    /// DataAccess management for Media Schedule
    /// </summary>
    public class GenericMediaScheduleDataAccess
    {

        #region Class Access Points

        #region GetData
        /// <summary>
        /// Get Data to build Media Schedule
        /// </summary>
        /// <param name="session">Client Session</param>
        /// <param name="period">Period</param>
        /// <returns>DataSet containing Data</returns>
        public static DataSet GetData(WebSession session, MediaSchedulePeriod period)
        {
            return GetData(session, period, -1, string.Empty, session.GenericMediaDetailLevel);
        }

        #endregion

        #region GetAdNetTrackData
        /// <summary>
        /// Get Data for AdNetTrack Media Schedule
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="period">Media Schedule Period</param>
        /// <returns>Data</returns>
        public static DataSet GetAdNetTrackData(WebSession session, MediaSchedulePeriod period)
        {
            string additionalConditions = "";
            switch (session.AdNetTrackSelection.SelectionType)
            {
                case TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type.advertiser:
                    additionalConditions = " AND " + ConstantesDB.Tables.WEB_PLAN_PREFIXE + ".id_advertiser=" + session.AdNetTrackSelection.Id.ToString() + " ";
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type.product:
                    additionalConditions = " AND " + ConstantesDB.Tables.WEB_PLAN_PREFIXE + ".id_product=" + session.AdNetTrackSelection.Id.ToString() + " ";
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type.visual:
                    additionalConditions = " AND " + ConstantesDB.Tables.WEB_PLAN_PREFIXE + ".hashcode=" + session.AdNetTrackSelection.Id.ToString() + " ";
                    break;
                default:
                    throw (new NotSupportedException("AdNetTrack Selection Type not supported"));
            }

            return GetData(session, period, ConstantesDBClassif.Vehicles.names.adnettrack.GetHashCode(), additionalConditions, session.GenericAdNetTrackDetailLevel);

        }
        #endregion

        #endregion

        #region private GetData
        /// <summary>
        /// Get Data to build Media Schedule
        /// </summary>
        /// <param name="session">Client Session</param>
        /// <param name="period">Period</param>
        /// <param name="vehicleId">Vehicle Filter if required</param>
        /// <param name="additionalWhereClause">Additional conditions if required</param>
        /// <returns>DataSet containing Data</returns>
        private static DataSet GetData(WebSession session, MediaSchedulePeriod period, Int64 vehicleId, string additionalWhereClause, GenericDetailLevel detailLevel)
        {

            #region Query Building
            bool first = true;

            string[] listVehicles = null;
            if (vehicleId < 0)
            {
                listVehicles = session.GetSelection(session.SelectionUniversMedia, ConstanteCustomerRight.type.vehicleAccess).Split(new char[] { ',' });
            }
            else
            {
                listVehicles = new string[1] { vehicleId.ToString() };
            }

            DataSet ds = new DataSet();

            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("select {0},date_num, max(period_count) as period_count,sum({1}) as {1} from (",
                detailLevel.GetSqlFieldsWithoutTablePrefix(),
                SQLGenerator.GetUnitAlias(session),
                ConstantesDB.Schema.ADEXPRESS_SCHEMA);

            //SubPeriod Management
            List<MediaScheduleSubPeriod> subPeriodsSet = period.SubPeriods;

            foreach (MediaScheduleSubPeriod subPeriods in subPeriodsSet)
            {
                switch (subPeriods.SubPeriodType)
                {
                    case ConstantesPeriod.PeriodBreakdownType.data:
                    case ConstantesPeriod.PeriodBreakdownType.data_4m:
                        for (int i = 0; i < listVehicles.Length; i++)
                        {
                            try
                            {
                                if (!first) sql.Append(" union all ");
                                else first = false;
                                sql.AppendFormat("({0})", GetQuery(session, detailLevel, period.PeriodDetailLEvel, subPeriods.SubPeriodType, Int64.Parse(listVehicles[i]), subPeriods.Items, additionalWhereClause));
                            }
                            catch (System.Exception err)
                            {
                                throw new MediaPlanDataAccessException("GenericMediaScheduleDataAccess.GetData(WebSession session, MediaSchedulePeriod period)", err);
                            }
                        }

                        break;
                    case ConstantesPeriod.PeriodBreakdownType.week:
                    case ConstantesPeriod.PeriodBreakdownType.month:
                        if (!first) sql.Append(" union all ");
                        else first = false;
                        sql.AppendFormat("({0})",
                            GetQuery(session, detailLevel, period.PeriodDetailLEvel, subPeriods.SubPeriodType, -1, subPeriods.Items, string.Empty));
                        break;
                    default:
                        throw new MediaPlanDataAccessException("Unable to determine type of subPeriod.");

                }
            }

            sql.Append(") ");
            sql.AppendFormat(" group by {0},date_num ", detailLevel.GetSqlGroupByFieldsWithoutTablePrefix());
            sql.AppendFormat(" order by {0}, date_num ", detailLevel.GetSqlOrderFieldsWithoutTablePrefix());

            #endregion

            #region Execution de la requête
            try
            {
                return session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new MediaPlanDataAccessException("Unable to load Media Schedule Data : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region GetQuery
        /// <summary>
        /// Build query
        /// </summary>
        /// <param name="session">Client sesseion</param>
        /// <param name="detailLevel">Detail levels selection</param>
        /// <param name="periodDisplay">Period detail display</param>
        /// <param name="periodBreakDown">Type of period (days, weeks, monthes)</param>
        /// <param name="vehicleId">Vehicle filter for desagregated data</param>
        /// <param name="beginningDate">Period Beginning</param>
        /// <param name="endDate">Period End</param>
        /// <param name="additionalConditions">Addtional conditions such as AdNetTrack Baners...</param>
        /// <returns>Sql query as a string</returns>
        private static string GetQuery(WebSession session,GenericDetailLevel detailLevel, ConstantesPeriod.DisplayLevel periodDisplay,ConstantesPeriod.PeriodBreakdownType periodBreakDown, Int64 vehicleId, List<PeriodItem> periodItems, string additionalConditions)
        {

            #region Variables
            string sql = "";
            string list = "";
            string tableName = null;
            string mediaTableName = null;
            string dateFieldName = null;
            string mediaFieldName = null;
            string mediaPeriodicity = null;
            string orderFieldName = null;
            string unitFieldName = null;
            string unitAlias = null;
            string mediaJoinCondition = null;
            string groupByFieldName = null;
            #endregion

            #region Construction de la requête
            try
            {

                // Obtient le nom de la table de données
                tableName = GetDataTableName(periodBreakDown, vehicleId);
                // Obtient les tables de la nomenclature
                mediaTableName = detailLevel.GetSqlTables(ConstantesDB.Schema.ADEXPRESS_SCHEMA);
                if (mediaTableName.Length > 0) mediaTableName += ",";
                // Obtient le champs unité
                dateFieldName = GetDateFieldName(periodBreakDown);
                unitFieldName = SQLGenerator.GetUnitFieldName(session, vehicleId, periodBreakDown);
                unitAlias = SQLGenerator.GetUnitAlias(session);
                //SQL Pour la périodicité
                mediaPeriodicity = GetPeriodicity(periodBreakDown, vehicleId, periodDisplay);
                // Obtient les champs de la nomenclature
                mediaFieldName = detailLevel.GetSqlFields();
                // Obtient l'ordre des champs
                orderFieldName = detailLevel.GetSqlOrderFields();
                // obtient la clause group by
                groupByFieldName = detailLevel.GetSqlGroupByFields();
                // Obtient les jointures pour la nomenclature
                mediaJoinCondition = detailLevel.GetSqlJoins(session.SiteLanguage, ConstantesDB.Tables.WEB_PLAN_PREFIXE);
            }
            catch (System.Exception err)
            {
                throw (new MediaPlanDataAccessException("Unable to build the query.", err));
            }

            // Sélection de la nomenclature Support
            sql += "select " + mediaFieldName + " ";
            // Sélection de la date
            if (periodDisplay != ConstantesPeriod.DisplayLevel.dayly
                && (periodBreakDown == ConstantesPeriod.PeriodBreakdownType.data
                || periodBreakDown == ConstantesPeriod.PeriodBreakdownType.data_4m))
            {
                if (periodDisplay != ConstantesPeriod.DisplayLevel.weekly)
                {
                    sql += ", to_number(to_char(to_date(" + dateFieldName + ", 'YYYYMMDD'), 'YYYYMM')) as date_num,";
                }
                else
                {
                    sql += ", to_number(to_char(to_date(" + dateFieldName + ", 'YYYYMMDD'), 'IYYYIW')) as date_num,";
                }
            }
            else
            {
                sql += ", " + dateFieldName + " as date_num,";
            }
            //Periodicity selection
            sql += mediaPeriodicity + ",";
            // Sélection de l'unité sauf pour AdNetTrack
            if ((ConstantesDBClassif.Vehicles.names)vehicleId == ConstantesDBClassif.Vehicles.names.adnettrack)
                sql += "sum(OCCURRENCE) as " + unitAlias + "";
            else
                sql += "sum(" + unitFieldName + ") as " + unitAlias + "";
            // Tables
            sql += " from " + mediaTableName + tableName + " wp ";
            //Conditions media
            sql += "where 0=0 " + mediaJoinCondition + "";
            // Période
            bool first = true;
            foreach (PeriodItem periodItem in periodItems)
            {
                if (first)
                {
                    first = false;
                    sql += " and ((";
                }
                else
                {
                    sql += ") or (";
                }
                sql += dateFieldName + ">=" + periodItem.Begin;
                sql += " and " + dateFieldName + "<=" + periodItem.End;
            }
            if (!first)
            {
                sql += ")) ";
            }

            // Conditions additionnelles
            if (additionalConditions.Length > 0)
            {
                sql += " " + additionalConditions + " ";
            }

            // Sous sélection de version
            string slogans = session.SloganIdList;
            // Zoom sur une version
            if (session.SloganIdZoom > 0 && periodDisplay == ConstantesPeriod.DisplayLevel.dayly)
            {
                sql += " and wp.id_slogan =" + session.SloganIdZoom + " ";
            }
            else
            {
                // affiner les version
                if (slogans.Length > 0 && periodDisplay == ConstantesPeriod.DisplayLevel.dayly)
                {
                    sql += " and wp.id_slogan in(" + slogans + ") ";
                }
            }

            // Gestion des sélections et des droits

            #region Nomenclature Produit (droits)
            //Droits en accès
            sql += SQLGenerator.getAnalyseCustomerProductRight(session, ConstantesDB.Tables.WEB_PLAN_PREFIXE, true);
            // Produit à exclure en radio
            sql += SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, ConstantesDB.Tables.WEB_PLAN_PREFIXE, true, false);
            #endregion

            #region Nomenclature Produit (Niveau de détail)
            // Niveau de produit
            sql += SQLGenerator.getLevelProduct(session, ConstantesDB.Tables.WEB_PLAN_PREFIXE, true);
            #endregion

            #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection)

            #region Sélection
            sql += GetProductSelection(session, ConstantesDB.Tables.WEB_PLAN_PREFIXE);
            #endregion

            #endregion

            #region Nomenclature Media (droits et sélection)

            #region Droits
            // On ne tient pas compte des droits vehicle pour les plans media AdNetTrack
            if ((ConstantesDBClassif.Vehicles.names)vehicleId == ConstantesDBClassif.Vehicles.names.adnettrack)
                sql += SQLGenerator.GetAdNetTrackMediaRight(session, ConstantesDB.Tables.WEB_PLAN_PREFIXE, true);
            else
                sql += SQLGenerator.getAnalyseCustomerMediaRight(session, ConstantesDB.Tables.WEB_PLAN_PREFIXE, true);
            #endregion

            #region Sélection
            if (periodDisplay != ConstantesPeriod.DisplayLevel.dayly && (ConstantesDBClassif.Vehicles.names)vehicleId != ConstantesDBClassif.Vehicles.names.adnettrack)
            {
                list = session.GetSelection(session.SelectionUniversMedia, ConstanteCustomerRight.type.vehicleAccess);
                if (list.Length > 0) sql += " and ((" + ConstantesDB.Tables.WEB_PLAN_PREFIXE + ".id_vehicle in (" + list + "))) ";
            }
            else
                sql += " and ((wp.id_vehicle= " + vehicleId + ")) ";
            #endregion

            #endregion

            // Ordre
            sql += "Group by " + groupByFieldName + " ";
            // et la date
            sql += ", " + dateFieldName + " ";

            #endregion

            return (sql);

        }
        #endregion

        #region Queries building stuff

        #region GetDataTableName
        /// <summary>
        /// Get data table to use in queries
        /// </summary>
        /// <param name="period">Type of period</param>
        /// <param name="vehicleId">Vehicle Id</param>
        /// <returns>Table matching the vehicle and the type of period</returns>
        private static string GetDataTableName(ConstantesPeriod.PeriodBreakdownType period, Int64 vehicleId)
        {
            switch (period)
            {
                case ConstantesPeriod.PeriodBreakdownType.month:
                    return (ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.WEB_PLAN_MEDIA_MONTH);
                case ConstantesPeriod.PeriodBreakdownType.week:
                    return (ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.WEB_PLAN_MEDIA_WEEK);
                case ConstantesPeriod.PeriodBreakdownType.data_4m:
                    switch ((ConstantesDBClassif.Vehicles.names)Convert.ToInt32(vehicleId))
                    {
                        case ConstantesDBClassif.Vehicles.names.press:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.ALERT_DATA_PRESS;
                        case ConstantesDBClassif.Vehicles.names.internationalPress:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.ALERT_DATA_PRESS_INTER;
                        case ConstantesDBClassif.Vehicles.names.radio:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.ALERT_DATA_RADIO;
                        case ConstantesDBClassif.Vehicles.names.tv:
                        case ConstantesDBClassif.Vehicles.names.others:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.ALERT_DATA_TV;
                        case ConstantesDBClassif.Vehicles.names.outdoor:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.ALERT_DATA_OUTDOOR;
                        case ConstantesDBClassif.Vehicles.names.adnettrack:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.ALERT_DATA_ADNETTRACK;
                        case ConstantesDBClassif.Vehicles.names.internet:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.ALERT_DATA_INTERNET;
                        case ConstantesDBClassif.Vehicles.names.directMarketing:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.ALERT_DATA_MARKETING_DIRECT;
                        default:
                            throw (new SQLGeneratorException("Impossible de déterminer la table media à utiliser"));
                    }
                case ConstantesPeriod.PeriodBreakdownType.data:
                    switch ((ConstantesDBClassif.Vehicles.names)Convert.ToInt32(vehicleId.ToString()))
                    {
                        case ConstantesDBClassif.Vehicles.names.press:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.DATA_PRESS;
                        case ConstantesDBClassif.Vehicles.names.internationalPress:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.DATA_PRESS_INTER;
                        case ConstantesDBClassif.Vehicles.names.radio:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.DATA_RADIO;
                        case ConstantesDBClassif.Vehicles.names.tv:
                        case ConstantesDBClassif.Vehicles.names.others:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.DATA_TV;
                        case ConstantesDBClassif.Vehicles.names.outdoor:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.DATA_OUTDOOR;
                        case ConstantesDBClassif.Vehicles.names.adnettrack:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.DATA_ADNETTRACK;
                        case ConstantesDBClassif.Vehicles.names.internet:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.DATA_INTERNET;
                        case ConstantesDBClassif.Vehicles.names.directMarketing:
                            return ConstantesDB.Schema.ADEXPRESS_SCHEMA + "." + ConstantesDB.Tables.DATA_MARKETING_DIRECT;
                        default:
                            throw (new SQLGeneratorException("Impossible de déterminer la table media à utiliser"));
                    }
                default:
                    throw (new SQLGeneratorException("Le détails période sélectionné est incorrect pour le choix de la table"));
            }
        }
        #endregion

        #region GetDateFieldName
        /// <summary>
        /// Get Field to use for date
        /// </summary>
        /// <param name="period">Type of period</param>
        /// <returns>Date Filed Name matchnig the type of period</returns>
        private static string GetDateFieldName(ConstantesPeriod.PeriodBreakdownType period)
        {
            switch (period)
            {
                case ConstantesPeriod.PeriodBreakdownType.month:
                    return (ConstantesDB.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD);
                case ConstantesPeriod.PeriodBreakdownType.week:
                    return (ConstantesDB.Fields.WEB_PLAN_MEDIA_WEEK_DATE_FIELD);
                case ConstantesPeriod.PeriodBreakdownType.data:
                case ConstantesPeriod.PeriodBreakdownType.data_4m:
                    return ("date_media_num");
                default:
                    throw (new MediaPlanDataAccessException("Selected detail period is uncorrect. Unable to determine date field to use."));
            }
        }
        #endregion

        #region Périodicité
        /// <summary>
        /// Get field for periodicity data
        /// </summary>
        ///<param name="detailPeriod">Detail period type</param>
        ///<param name="vehicleId">Vehicle Id</param>
        /// <param name="displayPeriod">Result display level</param>
        /// <returns>Field matching period type</returns>
        private static string GetPeriodicity(ConstantesPeriod.PeriodBreakdownType detailPeriod, Int64 vehicleId, ConstantesPeriod.DisplayLevel displayPeriod)
        {


            switch (detailPeriod)
            {
                case ConstantesPeriod.PeriodBreakdownType.month:
                    return (" max(duration) as period_count ");
                case ConstantesPeriod.PeriodBreakdownType.week:
                    return (" max(duration) as period_count ");
                case ConstantesPeriod.PeriodBreakdownType.data:
                case ConstantesPeriod.PeriodBreakdownType.data_4m:
                    switch ((ConstantesDBClassif.Vehicles.names)Convert.ToInt32(vehicleId.ToString()))
                    {
                        case ConstantesDBClassif.Vehicles.names.press:
                        case ConstantesDBClassif.Vehicles.names.internationalPress:
                        case ConstantesDBClassif.Vehicles.names.outdoor:
                            switch (displayPeriod)
                            {
                                case ConstantesPeriod.DisplayLevel.monthly:
                                    return (" max(" + ConstantesDB.Schema.ADEXPRESS_SCHEMA + ".DURATION_MONTH(date_media_num,duration)) as period_count ");
                                case ConstantesPeriod.DisplayLevel.weekly:
                                    return (" max(" + ConstantesDB.Schema.ADEXPRESS_SCHEMA + ".DURATION_WEEK(date_media_num,duration)) as period_count ");
                                default:
                                    return (" trunc((max(duration)/86400)) as period_count ");
                            }
                        //case ConstantesDBClassif.Vehicles.names.outdoor:
                        //    switch (displayPeriod)
                        //    {
                        //        case ConstantesPeriod.DisplayLevel.monthly:
                        //            return (" max(" + ConstantesDB.Schema.ADEXPRESS_SCHEMA + ".DURATION_MONTH_OD(to_number(to_char(DATE_CAMPAIGN_BEGINNING,'YYYYMMDD')),to_number(to_char(DATE_CAMPAIGN_END,'YYYYMMDD')))) as period_count ");
                        //        case ConstantesPeriod.DisplayLevel.weekly:
                        //            return (" max(" + ConstantesDB.Schema.ADEXPRESS_SCHEMA + ".DURATION_WEEK_OD(to_number(to_char(DATE_CAMPAIGN_BEGINNING,'YYYYMMDD')),to_number(to_char(DATE_CAMPAIGN_END,'YYYYMMDD')))) as period_count ");
                        //        default:
                        //            return (" (max(duration)/86400) as period_count ");
                        //    }
                        case ConstantesDBClassif.Vehicles.names.radio:
                        case ConstantesDBClassif.Vehicles.names.tv:
                        case ConstantesDBClassif.Vehicles.names.others:
                        case ConstantesDBClassif.Vehicles.names.internet:
                        case ConstantesDBClassif.Vehicles.names.adnettrack:
                            return (" 1 as period_count ");
                        case ConstantesDBClassif.Vehicles.names.directMarketing:
                            return (" 7 as period_count ");
                        default:
                            throw (new SQLGeneratorException("Impossible de déterminer la périodicité du media"));
                    }
                default:
                    throw (new MediaPlanDataAccessException("Selected period detail unvalid. Unable to determine periodicity field."));
            }
        }
        #endregion

        #region GetProductSelection
        /// <summary>
        /// Get product selection
        /// </summary>
        /// <remarks>
        /// Must beginning by AND
        /// </remarks>
        /// <param name="session">Client session</param>
        /// <param name="dataTablePrefixe">data table prefixe</param>
        /// <returns>product selection to add as condition into a sql query</returns>
        private static string GetProductSelection(WebSession session, string dataTablePrefixe)
        {
            string sql = "";
            if (session.PrincipalProductUniverses != null && session.PrincipalProductUniverses.Count > 0)
                sql = session.PrincipalProductUniverses[0].GetSqlConditions(dataTablePrefixe, true);
            return sql;
        }
        #endregion

        #endregion

    }
}
