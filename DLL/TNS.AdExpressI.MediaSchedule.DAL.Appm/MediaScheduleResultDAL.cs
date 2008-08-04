#region Information
/*
 * Author : G. Facon & G. Ragneau
 * Created : 21/11/2007
 * Modifications :
 *      Author - Date - Descriptopn
 *      G Ragneau - 30/04/2008 - Mise en couches
 *      
*/
#endregion

#region using

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

using CstFmk = TNS.AdExpress.Constantes.FrameWork;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstWeb = TNS.AdExpress.Constantes.Web;

using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpressI.MediaSchedule.DAL;
using TNS.AdExpressI.MediaSchedule.DAL.Exceptions;
#endregion

namespace TNS.AdExpressI.MediaSchedule.DAL.Appm
{
    /// <summary>
    /// Implementation of IMediaScheduleResultDAL for APPM Module
    /// </summary>
    public class MediaScheduleResultDAL : TNS.AdExpressI.MediaSchedule.DAL.MediaScheduleResultDAL
    {

        #region Properties

        #region DataBase description
        protected static Schema _schAppm01 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.appm01);
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period):base(session, period){}
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        /// <param name="idVehicle">Id of the vehicle</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period, Int64 idVehicle):base(session, period,idVehicle){}
        #endregion

        #region IMediaScheduleResultDAL Membres

        #region GetMediaScheduleAdNetTrackData
        /// <summary>
        /// No Implemented
        /// </summary>
        /// <returns>null</returns>
        public override DataSet GetMediaScheduleAdNetTrackData()
        {
            throw new NotImplementedException("AdNetTrack media is not availabkle in APPM Module.");
            return null;

        }
        #endregion

        #endregion

        #region protected GetData
        /// <summary>
        /// Get Data to build Media Schedule
        /// </summary>
        /// <param name="additionalWhereClause">Additional conditions if required</param>
        /// <param name="detailLevel">Clasification details</param>
        /// <returns>DataSet containing Data</returns>
        protected override DataSet GetData(string additionalWhereClause, GenericDetailLevel detailLevel)
        {

            #region Query Building
            bool first = true;

            DataSet ds = new DataSet();

            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("select {0},date_num, max(period_count) as period_count,sum(unit) as unit from (",
                detailLevel.GetSqlFieldsWithoutTablePrefix());

            //SubPeriod Management
            List<MediaScheduleSubPeriod> subPeriodsSet = _period.SubPeriods;

            foreach (MediaScheduleSubPeriod subPeriods in subPeriodsSet)
            {
                switch (subPeriods.SubPeriodType)
                {
                    case CstPeriod.PeriodBreakdownType.data:
                    case CstPeriod.PeriodBreakdownType.data_4m:
                        try
                        {
                            if (!first) sql.Append(" union all ");
                            else first = false;
                            sql.AppendFormat("({0})", GetQuery(detailLevel, _period.PeriodDetailLEvel, subPeriods.SubPeriodType, subPeriods.Items));
                        }
                        catch (System.Exception err)
                        {
                            throw new MediaScheduleDALException("Appm.MediaScheduleResultDAL.GetData(WebSession _session, MediaSchedulePeriod _period)", err);
                        }
                        break;
                    case CstPeriod.PeriodBreakdownType.week:
                    case CstPeriod.PeriodBreakdownType.month:
                        if (!first) sql.Append(" union all ");
                        else first = false;
                        sql.AppendFormat("({0})",
                            GetQuery(detailLevel, _period.PeriodDetailLEvel, subPeriods.SubPeriodType, subPeriods.Items));
                        break;
                    default:
                        throw new MediaScheduleDALException("Unable to determine type of subPeriod.");

                }
            }

            sql.Append(") ");
            sql.AppendFormat(" group by {0},date_num ", detailLevel.GetSqlGroupByFieldsWithoutTablePrefix());
            sql.AppendFormat(" order by {0}, date_num ", detailLevel.GetSqlOrderFieldsWithoutTablePrefix());

            #endregion

            #region Execution de la requête
            try
            {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new MediaScheduleDALException("Unable to load Appm Media Schedule Data : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region GetQuery
        /// <summary>
        /// Build query
        /// </summary>
        /// <param name="detailLevel">Detail levels selection</param>
        /// <param name="periodDisplay">Period detail display</param>
        /// <param name="periodBreakDown">Type of period (days, weeks, monthes)</param>
        /// <param name="beginningDate">Period Beginning</param>
        /// <param name="endDate">Period End</param>
        /// <returns>Sql query as a string</returns>
        protected string GetQuery(GenericDetailLevel detailLevel, CstPeriod.DisplayLevel periodDisplay, CstPeriod.PeriodBreakdownType periodBreakDown, List<PeriodItem> periodItems)
        {

            #region Variables
            StringBuilder sql = new StringBuilder();
            string list = "";
            string tableName = null;
            string mediaTableName = null;
            string dateFieldName = null;
            string mediaFieldName = null;
            string mediaPeriodicity = null;
            string orderFieldName = null;
            string unitFieldName = null;
            string mediaJoinCondition = null;
            string groupByFieldName = null;
            Table tblTargetMediaAssignment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.appmTargetMediaAssignment);
            #endregion

            #region Construction de la requête
            try
            {

                // Get the name of the data table
                tableName = GetDataTableName(periodBreakDown);
                // Get the classification table
                mediaTableName = string.Format("{0}, {1}, ", detailLevel.GetSqlTables(_schAdexpr03.Label), tblTargetMediaAssignment.SqlWithPrefix);
                // Get unit field
                dateFieldName = GetDateFieldName(periodBreakDown);
                unitFieldName = GetUnitFieldName(periodBreakDown);
                // Periodicity
                mediaPeriodicity = GetPeriodicity(periodBreakDown, CstDBClassif.Vehicles.names.press.GetHashCode(), periodDisplay);
                // Get classification fields
                mediaFieldName = detailLevel.GetSqlFields();
                // Get field order
                orderFieldName = detailLevel.GetSqlOrderFields();
                // Get group by clause
                groupByFieldName = detailLevel.GetSqlGroupByFields();
                // Get joins for classification
                mediaJoinCondition = string.Format("{0} and {1}.id_media_secodip = {2}.id_media "
                    , detailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix)
                    , tblTargetMediaAssignment.Prefix
                    , WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            }
            catch (System.Exception err)
            {
                throw (new MediaScheduleDALException("Unable to build the query.", err));
            }

            // Media classificaion selection
            sql.AppendFormat("select {0} ",mediaFieldName);
            // Date selection
            if (periodDisplay != CstPeriod.DisplayLevel.dayly
                && (periodBreakDown == CstPeriod.PeriodBreakdownType.data
                || periodBreakDown == CstPeriod.PeriodBreakdownType.data_4m))
            {
                if (periodDisplay != CstPeriod.DisplayLevel.weekly)
                {
                    sql.AppendFormat(", to_number(to_char(to_date({0}, 'YYYYMMDD'), 'YYYYMM')) as date_num,", dateFieldName);
                }
                else
                {
                    sql.AppendFormat(", to_number(to_char(to_date({0}, 'YYYYMMDD'), 'IYYYIW')) as date_num,",dateFieldName);
                }
            }
            else
            {
                sql.AppendFormat(", {0} as date_num,",dateFieldName);
            }
            //Periodicity selection
            sql.AppendFormat("{0}, ", mediaPeriodicity);
            sql.AppendFormat("sum({0}) as unit", unitFieldName);
            // Tables
            sql.AppendFormat(" from {0}{1} ", mediaTableName, tableName);
            //Conditions media
            sql.AppendFormat("where 0=0 {0}", mediaJoinCondition);

            // Period
            bool first = true;
            foreach (PeriodItem periodItem in periodItems)
            {
                if (first)
                {
                    first = false;
                    sql.Append(" and ((");
                }
                else
                {
                    sql.Append(") or (");
                }
                sql.AppendFormat("{0}>={1}", dateFieldName, periodItem.Begin);
                sql.AppendFormat(" and {0}<={1}",dateFieldName, periodItem.End);
            }
            if (!first)
            {
                sql.Append(")) ");
            }

            // Additional conditions
            //on one target
            sql.AppendFormat(" and {0}.id_target = {1}"
                , tblTargetMediaAssignment.Prefix
                , _session.GetSelection(_session.SelectionUniversAEPMTarget, CstRight.type.aepmTargetAccess));

            //outside encart
            if (periodBreakDown != CstPeriod.PeriodBreakdownType.data && periodBreakDown != CstPeriod.PeriodBreakdownType.data_4m)
            {
                sql.AppendFormat(" and {0}.id_inset = 0 ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            }
            else
            {
                sql.AppendFormat(" and {0}.id_inset is null ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            }


            //Access rgith
            sql.Append(SQLGenerator.getAnalyseCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

            //Advertiser classification rights
            sql.Append(GetProductSelection(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));


            //Media Rights
            sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

            // Order
            sql.AppendFormat("Group by {0} ", groupByFieldName);
            // And date
            sql.AppendFormat(", {0} ", dateFieldName);

            #endregion

            return (sql.ToString());

        }
        #endregion

        #region Queries building stuff

        #region GetDataTableName
        /// <summary>
        /// Get data table to use in queries
        /// </summary>
        /// <param name="period">Type of period</param>
        /// <returns>Table matching the type of period</returns>
        protected string GetDataTableName(CstPeriod.PeriodBreakdownType period)
        {
            switch (period)
            {
                case CstPeriod.PeriodBreakdownType.month:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthAppmData).SqlWithPrefix;
                case CstPeriod.PeriodBreakdownType.week:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.weekData).SqlWithPrefix;
                case CstPeriod.PeriodBreakdownType.data_4m:
                case CstPeriod.PeriodBreakdownType.data:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataPressAPPM).SqlWithPrefix;
                default:
                    throw (new MediaScheduleDALException("The detail selected is not a correct one to choose of the table"));
            }
        }
        #endregion

        #endregion


    }
}
