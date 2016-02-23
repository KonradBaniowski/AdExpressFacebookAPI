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
using TNS.AdExpressI.MediaSchedule.DAL.Exceptions;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web.Navigation;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Utilities;
#endregion

namespace TNS.AdExpressI.MediaSchedule.DAL
{
    /// <summary>
    /// Abstract Implementation of IMediaScheduleResultDAL
    /// </summary>
    public abstract class MediaScheduleResultDAL : IMediaScheduleResultDAL
    {

        #region Properties

        protected bool _isAdNetTrackMediaSchedule = false;

        #region DataBase description
        protected static Schema _schAdexpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);


        #endregion

        #region Module
        /// <summary>
        /// Current module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(CstWeb.Module.Name.ANALYSE_PLAN_MEDIA);
        /// <summary>
        /// User Session
        /// </summary>
        public TNS.AdExpress.Domain.Web.Navigation.Module Module
        {
            get { return _module; }
            set { _module = value; }
        }
        #endregion

        #region Session
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// User Session
        /// </summary>
        public WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        #endregion

        #region Period
        /// <summary>
        /// Report period filter
        /// </summary>
        protected TNS.AdExpress.Web.Core.Selection.MediaSchedulePeriod _period;
        /// <summary>
        /// Report period
        /// </summary>
        public MediaSchedulePeriod Period
        {
            get { return _period; }
            set { _period = value; }
        }
        #endregion

        #region Period
        /// <summary>
        /// Report period filter Comparative
        /// </summary>
        protected TNS.AdExpress.Web.Core.Selection.MediaSchedulePeriod _periodComparative;
        /// <summary>
        /// Report period Comparative
        /// </summary>
        public MediaSchedulePeriod PeriodComparative
        {
            get { return _periodComparative; }
            set { _periodComparative = value; }
        }
        #endregion

        #region Vehicle Id
        /// <summary>
        /// Vehicle Id filter
        /// </summary>
        protected Int64 _vehicleId = -1;
        /// <summary>
        /// Get / Set Vehicle Id filter
        /// </summary>
        public Int64 VehicleId
        {
            get { return _vehicleId; }
            set { value = _vehicleId; }
        }
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period)
        {
            _session = session;
            _period = period;


        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        /// <param name="idVehicle">Id of the vehicle</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period, Int64 idVehicle)
            : this(session, period)
        {
            _vehicleId = idVehicle;

        }
        #endregion

        #region IMediaScheduleResultDAL Membres

        #region GetMediaScheduleData
        /// <summary>
        /// Get Data to build Media Schedule
        /// </summary>
        /// <returns>DataSet containing Data</returns>
        public virtual DataSet GetMediaScheduleData()
        {
            return GetData(string.Empty, _session.GenericMediaDetailLevel);
        }
        #endregion

        #region GetMediaScheduleData (for comparative study)
        /// <summary>
        /// Get Data to build Media Schedule
        /// </summary>
        /// <param name="isComparative">True if we need data for comparative period</param>
        /// <returns>DataSet containing Data</returns>
        public virtual DataSet GetMediaScheduleData(bool isComparative)
        {
            return GetData(string.Empty, _session.GenericMediaDetailLevel, isComparative);
        }
        #endregion

        #region GetMediaScheduleDataLevels
        /// <summary>
        /// Get data to build a Media Schedule Report
        /// </summary>
        /// <returns>data to build a Media Schedule Report</returns>
        public virtual DataSet GetMediaScheduleDataLevels()
        {
            return GetDataLevels(string.Empty, _session.GenericMediaDetailLevel);
        }
        #endregion

        #region GetMediaScheduleAdNetTrackData
        /// <summary>
        /// Get Data for AdNetTrack Media Schedule
        /// </summary>
        /// <returns>Data</returns>
        public virtual DataSet GetMediaScheduleAdNetTrackData()
        {
            string additionalConditions = "";
            _isAdNetTrackMediaSchedule = true;
            switch (_session.AdNetTrackSelection.SelectionType)
            {
                case CstFmk.Results.AdNetTrackMediaSchedule.Type.advertiser:
                    additionalConditions = string.Format(" AND {0}.id_advertiser={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.AdNetTrackSelection.Id + " ");
                    break;
                case CstFmk.Results.AdNetTrackMediaSchedule.Type.product:
                    additionalConditions = string.Format(" AND {0}.id_product={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.AdNetTrackSelection.Id + " ");
                    break;
                case CstFmk.Results.AdNetTrackMediaSchedule.Type.visual:
                    additionalConditions = string.Format(" AND {0}.hashcode={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.AdNetTrackSelection.Id + " ");
                    break;
                default:
                    throw (new MediaScheduleDALException("AdNetTrack Selection Type not supported."));
            }

            return GetData(additionalConditions, _session.GenericAdNetTrackDetailLevel);

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
        protected virtual DataSet GetData(string additionalWhereClause, GenericDetailLevel detailLevel)
        {
            return GetData(additionalWhereClause, detailLevel, false);
        }
        /// <summary>
        /// Get Data to build Media Schedule
        /// </summary>
        /// <param name="additionalWhereClause">Additional conditions if required</param>
        /// <param name="detailLevel">Clasification details</param>
        /// <param name="isComparative">True if we need data for comparative period</param>
        /// <returns>DataSet containing Data</returns>
        protected virtual DataSet GetData(string additionalWhereClause, GenericDetailLevel detailLevel, bool isComparative)
        {

            #region Variables
            DataSet ds = new DataSet();
            StringBuilder sql = new StringBuilder();
            string groupOptional = string.Empty;

            #endregion

            #region Query Building
            if (isComparative)
            {
                sql.Append(GetMasterQuery(additionalWhereClause, detailLevel, _periodComparative, isComparative, string.Empty, string.Empty));
                sql.AppendFormat(" order by {0}{1} ", detailLevel.GetSqlOrderFieldsWithoutTablePrefix(), string.Empty);
            }
            else
            {
                sql.Append(GetMasterQuery(additionalWhereClause, detailLevel, _period, isComparative, "date_num, max(period_count) as period_count,", ", date_num"));
                sql.AppendFormat(" order by {0}{1} ", detailLevel.GetSqlOrderFieldsWithoutTablePrefix(), ", date_num");
            }
            #endregion

            #region Execution de la requête
            try
            {

                return _session.Source.Fill(sql.ToString());

            }
            catch (System.Exception err)
            {
                throw (new MediaScheduleDALException("Unable to load Media Schedule Data : " + sql, err));
            }
            #endregion

        }
        /// <summary>
        /// Get Data to build Media Schedule
        /// </summary>
        /// <param name="additionalWhereClause">Additional conditions if required</param>
        /// <param name="detailLevel">Clasification details</param>
        /// <param name="isComparative">True if we need data for comparative period</param>
        /// <returns>DataSet containing Data</returns>
        protected virtual DataSet GetDataLevels(string additionalWhereClause, GenericDetailLevel detailLevel)
        {

            #region Variables
            DataSet ds = new DataSet();
            StringBuilder sql = new StringBuilder();
            string groupOptional = string.Empty;
            #endregion

            #region Query Building
            sql.AppendFormat("({0}) UNION ALL ({1})"
                , GetMasterQuery(additionalWhereClause, detailLevel, _period, false, string.Empty, string.Empty)
                , GetMasterQuery(additionalWhereClause, detailLevel, _periodComparative, false, string.Empty, string.Empty));
            sql.AppendFormat(" order by {0}{1} ", detailLevel.GetSqlOrderFieldsWithoutTablePrefix(), string.Empty);
            #endregion

            #region Execution de la requête
            try
            {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new MediaScheduleDALException("Unable to load Media Schedule Data : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region GetMasterQuery
        /// <summary>
        /// Get Master Query
        /// </summary>
        /// <param name="additionalWhereClause">Additional conditions if required</param>
        /// <param name="detailLevel">Clasification details</param>
        /// <param name="period">Period to build</param>
        /// <param name="isComparative">True if we need data for comparative period</param>
        /// <param name="additionalSelect">Additional Select field if required</param>
        /// <param name="additionalGroup">Additional Group field if required</param>
        /// <returns>master Query</returns>
        protected virtual string GetMasterQuery(string additionalWhereClause, GenericDetailLevel detailLevel, TNS.AdExpress.Web.Core.Selection.MediaSchedulePeriod period, bool isComparative, string additionalSelect, string additionalGroup)
        {

            #region Variables
            bool first = true;
            DataSet ds = new DataSet();
            string[] listVehicles = null;
            StringBuilder sql = new StringBuilder();
            string groupOptional = string.Empty;
            #endregion

            #region Query Building
            if (_vehicleId < 0)
                listVehicles = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess).Split(new char[] { ',' });
            else
                listVehicles = new string[1] { _vehicleId.ToString() };

            // Select
            sql.AppendFormat("select {0}, {1}{2} from (", detailLevel.GetSqlFieldsWithoutTablePrefix(), additionalSelect, GetUnitFieldNameSumUnionWithAlias(_session));

            // SubPeriod Management
            List<MediaScheduleSubPeriod> subPeriodsSet = period.SubPeriods;
            foreach (MediaScheduleSubPeriod subPeriods in subPeriodsSet)
            {
                switch (subPeriods.SubPeriodType)
                {
                    case CstPeriod.PeriodBreakdownType.data:
                    case CstPeriod.PeriodBreakdownType.data_4m:
                        for (int i = 0; i < listVehicles.Length; i++)
                        {
                            try
                            {
                                if (!first) sql.Append(" union all ");
                                else first = false;
                                sql.AppendFormat("({0})", GetQuery(detailLevel, period.PeriodDetailLEvel, subPeriods.SubPeriodType, Int64.Parse(listVehicles[i]), subPeriods.Items, additionalWhereClause, isComparative));
                            }
                            catch (System.Exception err)
                            {
                                throw new MediaScheduleDALException("GenericMediaScheduleDataAccess.GetData(WebSession _session, MediaSchedulePeriod period)", err);
                            }
                        }
                        break;
                    case CstPeriod.PeriodBreakdownType.week:
                    case CstPeriod.PeriodBreakdownType.month:
                        if (!first) sql.Append(" union all ");
                        else first = false;

                        //Int64 vehicleIdTmp = -1;
                        List<Int64> vehicleIdListTmp = new List<long>();
                        for (int i = 0; i < listVehicles.Length; i++)
                        {
                            if (VehiclesInformation.Contains(Int64.Parse(listVehicles[i]))
                                && (VehiclesInformation.DatabaseIdToEnum(Int64.Parse(listVehicles[i])) == CstDBClassif.Vehicles.names.adnettrack
                                || VehiclesInformation.DatabaseIdToEnum(Int64.Parse(listVehicles[i])) == CstDBClassif.Vehicles.names.evaliantMobile)
                                && _session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
                            {
                                //vehicleIdTmp = Int64.Parse(listVehicles[i]);
                                vehicleIdListTmp.Add(Int64.Parse(listVehicles[i]));
                            }
                        }
                        if (vehicleIdListTmp.Count > 0)
                            sql.AppendFormat("({0})", GetQueryForWebPlanEvaliant(detailLevel, period.PeriodDetailLEvel, subPeriods.SubPeriodType, subPeriods.Items, string.Empty, isComparative));
                        else
                            sql.AppendFormat("({0})", GetQuery(detailLevel, period.PeriodDetailLEvel, subPeriods.SubPeriodType, -1, subPeriods.Items, string.Empty, isComparative));
                        break;
                    default:
                        throw new MediaScheduleDALException("Unable to determine type of subPeriod.");
                }
            }

            sql.Append(") ");
            sql.AppendFormat(" group by {0}{1} ", detailLevel.GetSqlGroupByFieldsWithoutTablePrefix(), additionalGroup);
            UnitInformation u = _session.GetSelectedUnit();
            if (u.Id == CstWeb.CustomerSessions.Unit.versionNb)
            {
                sql.AppendFormat(", {0} ", u.Id.ToString());
            }
            #endregion

            return sql.ToString();
        }
        #endregion

        #region GetQuery
        /// <summary>
        /// Build query
        /// </summary>
        /// <param name="detailLevel">Detail levels selection</param>
        /// <param name="periodDisplay">Period detail display</param>
        /// <param name="periodBreakDown">Type of period (days, weeks, monthes)</param>
        /// <param name="vehicleId">Vehicle filter for desagregated data</param>
        /// <param name="beginningDate">Period Beginning</param>
        /// <param name="endDate">Period End</param>
        /// <param name="additionalConditions">Addtional conditions such as AdNetTrack Baners...</param>
        /// <param name="isComparative">True if we need data for comparative period</param>
        /// <returns>Sql query as a string</returns>
        protected virtual string GetQuery(GenericDetailLevel detailLevel, CstPeriod.DisplayLevel periodDisplay, CstPeriod.PeriodBreakdownType periodBreakDown, Int64 vehicleId, List<PeriodItem> periodItems, string additionalConditions, bool isComparative)
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
            string unitAlias = null;
            string mediaJoinCondition = null;
            string groupByFieldName = null;
            string groupByOptional = null;
            VehicleInformation vehicleInfo = null;
            #endregion

            if (VehiclesInformation.Contains(vehicleId))
                vehicleInfo = VehiclesInformation.Get(vehicleId);

            #region Construction de la requête
            try
            {
                // Get the name of the data table
                if (_isAdNetTrackMediaSchedule && VehiclesInformation.Contains(CstDBClassif.Vehicles.names.internet) && vehicleId == VehiclesInformation.Get(CstDBClassif.Vehicles.names.internet).DatabaseId)
                {
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataInternetVersion, _session.IsSelectRetailerDisplay).SqlWithPrefix;
                    unitFieldName = string.Format("OCCURRENCE");
                }
                else
                {
                    tableName = SQLGenerator.GetDataTableName(periodBreakDown, vehicleId, _session.IsSelectRetailerDisplay);
                    unitFieldName = SQLGenerator.GetUnitFieldName(_session, vehicleId, periodBreakDown);
                }
                // Get the classification table
                mediaTableName = detailLevel.GetSqlTables(_schAdexpr03.Label);
                if (mediaTableName.Length > 0) mediaTableName += ",";
                // Get unit field
                dateFieldName = SQLGenerator.GetDateFieldName(periodBreakDown);
                unitAlias = SQLGenerator.GetUnitAlias(_session);
                // isComparative
                if (!isComparative)
                    mediaPeriodicity = GetPeriodicity(periodBreakDown, vehicleId, periodDisplay);

                // Get classification fields
                if (_isAdNetTrackMediaSchedule || (VehiclesInformation.Contains(vehicleId) && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.adnettrack
                    || VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.evaliantMobile)))
                {
                    mediaFieldName = GetAdnettrackSqlFields(detailLevel);
                }
                else
                {
                    mediaFieldName = detailLevel.GetSqlFields();
                }

                // Get field order
                orderFieldName = detailLevel.GetSqlOrderFields();

                // Get group by clause
                if (_isAdNetTrackMediaSchedule || (VehiclesInformation.Contains(vehicleId) && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.adnettrack
                    || VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.evaliantMobile)))
                {
                    groupByFieldName = GetAdnettrackSqlGroupByFields(detailLevel);
                }
                else
                {
                    groupByFieldName = detailLevel.GetSqlGroupByFields();
                }

                // Get joins for classification
                mediaJoinCondition = detailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            }
            catch (System.Exception err)
            {
                throw (new MediaScheduleDALException("Unable to build the query.", err));
            }

            // Select : Media classificaion selection
            sql.AppendFormat("select {0} ", mediaFieldName);

            // Date selection
            if (periodDisplay != CstPeriod.DisplayLevel.dayly
                && (periodBreakDown == CstPeriod.PeriodBreakdownType.data
                || periodBreakDown == CstPeriod.PeriodBreakdownType.data_4m))
            {
                if (periodDisplay != CstPeriod.DisplayLevel.weekly)
                    sql.AppendFormat(", to_number(to_char(to_date({0}, 'YYYYMMDD'), 'YYYYMM')) as date_num,", dateFieldName);
                else
                    sql.AppendFormat(", to_number(to_char(to_date({0}, 'YYYYMMDD'), 'IYYYIW')) as date_num,", dateFieldName);
            }
            else
            {
                sql.AppendFormat(", {0} as date_num,", dateFieldName);
            }

            // isComparative
            if (!isComparative)
                sql.AppendFormat("{0}, ", mediaPeriodicity);

            // Unit selection expect for AdNetTrack           
            if (VehiclesInformation.Contains(vehicleId)
                && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.adnettrack
                || VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.evaliantMobile)
                && _session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
            {
                switch (periodBreakDown)
                {
                    case CstPeriod.PeriodBreakdownType.data:
                    case CstPeriod.PeriodBreakdownType.data_4m:
                        unitFieldName = string.Format("  to_char({0}.{1}) as {2} ",
                            WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.GetSelectedUnit().DatabaseField, _session.GetSelectedUnit().Id.ToString());
                        groupByOptional = string.Format(", {0}.{1} ",
                            WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.GetSelectedUnit().DatabaseField);
                        break;
                    default:
                        unitFieldName = string.Format(" {0}.{1} as {2} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.GetSelectedUnit().DatabaseMultimediaField, _session.GetSelectedUnit().Id.ToString());
                        groupByOptional = string.Format(", {0}.{1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.GetSelectedUnit().DatabaseMultimediaField);
                        break;
                }
                sql.AppendFormat("{0}", unitFieldName);
            }
            else
                sql.AppendFormat("sum({0}) as {1}", unitFieldName, unitAlias);

            // From : Tables
            sql.AppendFormat(" from {0}{1} ", mediaTableName, tableName);

            // Where : Conditions media
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
                sql.AppendFormat(" and {0}<={1}", dateFieldName, periodItem.End);
            }
            if (!first)
            {
                sql.Append(")) ");
            }

            //INset option
            if ((vehicleInfo != null && (vehicleInfo.Id == CstDBClassif.Vehicles.names.press
                || vehicleInfo.Id == CstDBClassif.Vehicles.names.internationalPress
                || CstDBClassif.Vehicles.names.newspaper == vehicleInfo.Id
                || CstDBClassif.Vehicles.names.magazine == vehicleInfo.Id
                ))
                || (periodBreakDown != CstWeb.CustomerSessions.Period.PeriodBreakdownType.data
                && periodBreakDown != CstWeb.CustomerSessions.Period.PeriodBreakdownType.data_4m)
                )
            {
                sql.Append(SQLGenerator.GetJointForInsertDetail(_session,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            }

            // Autopromo
            if (VehiclesInformation.Contains(vehicleId))
            {

                string idMediaLabel = string.Empty;

                if ((VehiclesInformation.Contains(CstDBClassif.Vehicles.names.adnettrack) && VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.adnettrack)
                    || (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.evaliantMobile) && VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.evaliantMobile))
                    idMediaLabel = "id_media_evaliant";
                else if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.mms) && VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.mms)
                    idMediaLabel = "id_media_mms";

                if (VehiclesInformation.Get(vehicleId).Autopromo
                    && ((VehiclesInformation.Contains(CstDBClassif.Vehicles.names.adnettrack) && VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.adnettrack)
                    || (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.evaliantMobile) && VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.evaliantMobile)
                    || (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.mms) && VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.mms)))
                {

                    Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                    if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                        sql.AppendFormat(" and {0}.auto_promotion = 0 ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    else if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany)
                    {
                        sql.AppendFormat(" and ({0}.id_media, {0}.id_holding_company) not in ( ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                        sql.AppendFormat(" select distinct {0}, id_holding_company ", idMediaLabel);
                        sql.AppendFormat(" from {0} ", tblAutoPromo.Sql);
                        sql.AppendFormat(" where {0} is not null ", idMediaLabel);
                        sql.AppendFormat(" ) ");
                    }
                }
            }
            else if (periodBreakDown != CstWeb.CustomerSessions.Period.PeriodBreakdownType.data
                && periodBreakDown != CstWeb.CustomerSessions.Period.PeriodBreakdownType.data_4m)
            {

                string listVehicles = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess);
                string autoPromoVehicles = string.Empty;
                bool comma = false;

                if ((
                    VehiclesInformation.Contains(CstDBClassif.Vehicles.names.adnettrack)
                    && listVehicles.Contains(VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.adnettrack).ToString())
                    && VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack).Autopromo
                    )
                    ||
                    (
                    VehiclesInformation.Contains(CstDBClassif.Vehicles.names.evaliantMobile)
                    && listVehicles.Contains(VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.evaliantMobile).ToString())
                    && VehiclesInformation.Get(CstDBClassif.Vehicles.names.evaliantMobile).Autopromo
                    ))
                {

                    Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                    if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.adnettrack))
                    {
                        autoPromoVehicles = VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.adnettrack).ToString();
                        comma = true;
                    }

                    if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.evaliantMobile))
                    {
                        if (comma)
                            autoPromoVehicles += ",";

                        autoPromoVehicles += VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.evaliantMobile).ToString();
                    }

                    if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                        sql.AppendFormat(" and (({0}.id_vehicle not in ({1})) or ({0}.id_vehicle in ({1}) and {0}.auto_promotion = 0 )) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, autoPromoVehicles);
                    else if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany)
                    {
                        sql.AppendFormat(" and (({0}.id_vehicle not in ({1})) or ({0}.id_vehicle in ({1}) and (({0}.id_media, {0}.id_holding_company) not in ( ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, autoPromoVehicles);
                        sql.AppendFormat(" select distinct id_media_evaliant, id_holding_company ");
                        sql.AppendFormat(" from {0} ", tblAutoPromo.Sql);
                        sql.AppendFormat(" where id_media_evaliant is not null ");
                        sql.AppendFormat(" )))) ");
                    }

                }
                else if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.mms) && listVehicles.Contains(VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.mms).ToString()))
                {

                    Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);
                    autoPromoVehicles = VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.mms).ToString();

                    if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                        sql.AppendFormat(" and (({0}.id_vehicle not in ({1})) or ({0}.id_vehicle in ({1}) and {0}.auto_promotion = 0 )) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, autoPromoVehicles);
                    else if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany)
                    {
                        sql.AppendFormat(" and (({0}.id_vehicle not in ({1})) or ({0}.id_vehicle in ({1}) and (({0}.id_media, {0}.id_holding_company) not in ( ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, autoPromoVehicles);
                        sql.AppendFormat(" select distinct id_media_mms, id_holding_company ");
                        sql.AppendFormat(" from {0} ", tblAutoPromo.Sql);
                        sql.AppendFormat(" where id_media_mms is not null ");
                        sql.AppendFormat(" )))) ");
                    }

                }

            }

            // Additional conditions            
            if (additionalConditions.Length > 0)
            {
                sql.AppendFormat(" {0} ", additionalConditions);
            }

            // Version selection
            string slogans = _session.SloganIdList;


            // Zoom on a specific version
            if (vehicleInfo != null &&
                vehicleInfo.Id != CstDBClassif.Vehicles.names.mailValo)
            {
                var creativesUtilities = new TNS.AdExpress.Web.Core.Utilities.Creatives();
                if (creativesUtilities.IsSloganZoom(_session.SloganIdZoom) && periodDisplay == CstPeriod.DisplayLevel.dayly)
                {
                    sql.AppendFormat(" and {0}.{2} = {1} ",
                                          WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                                          _session.SloganIdZoom
                                          ,
                                          (vehicleInfo != null &&
                                           (vehicleInfo.Id != CstDBClassif.Vehicles.names.adnettrack &&
                                            vehicleInfo.Id != CstDBClassif.Vehicles.names.evaliantMobile))
                                              ? "id_slogan"
                                              : "hashcode");
                }
                else
                {
                    // Refine vesions
                    if (slogans.Length > 0 && periodDisplay == CstPeriod.DisplayLevel.dayly)
                    {
                        sql.AppendFormat(" and {0}.{2} in({1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, slogans
                            , (vehicleInfo != null && (vehicleInfo.Id != CstDBClassif.Vehicles.names.adnettrack &&
                            vehicleInfo.Id != CstDBClassif.Vehicles.names.evaliantMobile)) ? "id_slogan" : "hashcode");
                    }
                }
            }




            // Selection and right managment

            #region Nomenclature Produit (droits)
            //Access rgithDroits en accès
            if (_module == null) throw (new MediaScheduleDALException("_module cannot be NULL"));
            sql.Append(SQLGenerator.GetClassificationCustomerProductRight(_session,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches));

            GetExcludeProudcts(sql);
            #endregion

            #region Nomenclature Produit (Niveau de détail)
            // Product level
            sql.Append(SQLGenerator.getLevelProduct(_session,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            #endregion

            #region Sélection produit
            // product
            if (!_isAdNetTrackMediaSchedule)
            {
                sql.Append(GetProductSelection(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            }
            #endregion

            #region Sélection support
            // media
            sql.Append(GetMediaSelection(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            #endregion

            #region Media classification

            #region Rights
            // No media right if AdNetTrack media schedule
            if (_isAdNetTrackMediaSchedule)
            {
                sql.Append(SQLGenerator.GetAdNetTrackMediaRight(_session,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            }
            else
                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            #endregion

            #region Selection
            list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess);
            if (_isAdNetTrackMediaSchedule)
            {
                sql.AppendFormat(" and ({0}.id_vehicle={1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                    VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack).DatabaseId);
            }
            else if (vehicleId > -1)
            {
                sql.AppendFormat(" and ({0}.id_vehicle={1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, vehicleId);
            }
            else
            {
                sql.AppendFormat(" and ({0}.id_vehicle in ({1})) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, list);
            }
            #endregion

            //Universe media
            sql.Append(GetMediaUniverse(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

            #endregion

            #region Banners Format Filter
            if (vehicleInfo != null)
            {
                VehicleInformation cVehicleInfo;
                if (_isAdNetTrackMediaSchedule && VehiclesInformation.Contains(CstDBClassif.Vehicles.names.internet) && vehicleId == VehiclesInformation.Get(CstDBClassif.Vehicles.names.internet).DatabaseId)
                    cVehicleInfo = VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack);
                else
                    cVehicleInfo = vehicleInfo;
             
                var formatIdList = _session.GetValidFormatSelectedList(new List<VehicleInformation>(new[] { cVehicleInfo }));
              

                if (formatIdList != null && formatIdList.Count > 0)
                    sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".ID_" +
                             WebApplicationParameters.DataBaseDescription.GetTable(
                                 WebApplicationParameters.VehiclesFormatInformation.VehicleFormatInformationList[
                                     cVehicleInfo.DatabaseId].FormatTableName).Label + " in (" +
                             string.Join(",", formatIdList.ConvertAll(p => p.ToString()).ToArray()) + ") ");
            }
            else
            {
                if (list.Length > 0)
                {
                    var sqlFormatSelectedClause = new StringBuilder();
                    sqlFormatSelectedClause.Append(" and (");
                    var vehicleInfoList = _session.GetVehiclesSelected();
                    bool firstVehicle = true;
                    bool hasValidFormat = false;
                    foreach (var cVehicleInformation in vehicleInfoList.Values)
                    {
                        VehicleInformation cVehicleInfo;
                        if (_isAdNetTrackMediaSchedule && VehiclesInformation.Contains(CstDBClassif.Vehicles.names.internet)
                            && cVehicleInformation.DatabaseId == VehiclesInformation.Get(CstDBClassif.Vehicles.names.internet).DatabaseId)
                            cVehicleInfo = VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack);
                        else
                            cVehicleInfo = cVehicleInformation;

                        if (firstVehicle) firstVehicle = false;
                        else sqlFormatSelectedClause.Append(" OR ");
                        sqlFormatSelectedClause.Append(" (");

                        var formatIdList = _session.GetValidFormatSelectedList(new List<VehicleInformation>(new[] { cVehicleInfo }));
                        if (formatIdList.Count > 0)
                        {
                            sqlFormatSelectedClause.AppendFormat(" {0}.id_vehicle = {1} ",
                                             WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                                             cVehicleInfo.DatabaseId);
                            sqlFormatSelectedClause.Append("and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix
                                + ".ID_" + WebApplicationParameters.DataBaseDescription.GetTable(WebApplicationParameters.
                                VehiclesFormatInformation.VehicleFormatInformationList[cVehicleInfo.DatabaseId].FormatTableName).Label
                                + " in (" + string.Join(",", formatIdList.ConvertAll(p => p.ToString()).ToArray()) + ") ");
                            hasValidFormat = true;
                        }
                        else
                        {
                            sqlFormatSelectedClause.AppendFormat(" {0}.id_vehicle = {1} ",
                                             WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                                             cVehicleInfo.DatabaseId);
                        }
                        sqlFormatSelectedClause.Append(") ");
                    }
                    sqlFormatSelectedClause.Append(" ) ");
                    if (hasValidFormat)
                        sql.Append(sqlFormatSelectedClause.ToString());
                }
            }

            #endregion

            #region Purchase Mode Filter
            if (WebApplicationParameters.UsePurchaseMode && _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PURCHASE_MODE_DISPLAY_FLAG))
            {
                if (vehicleInfo != null)
                {
                    if (vehicleId == VehiclesInformation.Get(CstDBClassif.Vehicles.names.mms).DatabaseId && WebApplicationParameters.UsePurchaseMode && _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PURCHASE_MODE_DISPLAY_FLAG))
                    {
                        string purchaseModeIdList = _session.SelectedPurchaseModeList;
                        if (purchaseModeIdList.Length > 0)
                            sql.AppendFormat(" and {0}ID_{1} in ({2}) "
                                       , WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "."
                                       , WebApplicationParameters.DataBaseDescription.GetTable(TableIds.purchaseModeMMS).Label
                                       , purchaseModeIdList);
                    }
                }
                else
                {
                    if (list.Length > 0)
                    {
                        var sqlPurchaseModeSelectedClause = new StringBuilder();
                        var vehicleInfoList = _session.GetVehiclesSelected();
                        if (vehicleInfoList.ContainsKey(VehiclesInformation.Get(CstDBClassif.Vehicles.names.mms).DatabaseId))
                        {
                            foreach (var cVehicleInformation in vehicleInfoList.Values)
                            {

                                if (cVehicleInformation.Id == CstDBClassif.Vehicles.names.mms && WebApplicationParameters.UsePurchaseMode && _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PURCHASE_MODE_DISPLAY_FLAG))
                                {
                                    string purchaseModeIdList = _session.SelectedPurchaseModeList;
                                    string mmsId = VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.mms).ToString();
                                    if (purchaseModeIdList.Length > 0)
                                        sqlPurchaseModeSelectedClause.AppendFormat(" and (({0}.id_vehicle not in ({1})) or ({0}.id_vehicle in ({1}) and {0}.id_purchase_mode_mms in ({2}) )) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, mmsId, purchaseModeIdList);
                                }
                            }
                            sql.Append(sqlPurchaseModeSelectedClause.ToString());
                        }
                    }
                }
            }
            #endregion

            // Order
            sql.AppendFormat("Group by {0} {1}", groupByFieldName, groupByOptional);
            // And date
            sql.AppendFormat(", {0} ", dateFieldName);
            #endregion

            return (sql.ToString());
        }
        #endregion

        #region GetQuery web plan evaliant and evaliant mobile
        /// <summary>
        /// Build query
        /// </summary>
        /// <param name="detailLevel">Detail levels selection</param>
        /// <param name="periodDisplay">Period detail display</param>
        /// <param name="periodBreakDown">Type of period (days, weeks, monthes)</param>
        /// <param name="vehicleId">Vehicle filter for desagregated data</param>
        /// <param name="beginningDate">Period Beginning</param>
        /// <param name="endDate">Period End</param>
        /// <param name="additionalConditions">Addtional conditions such as AdNetTrack Baners...</param>
        /// <param name="isComparative">True if we need data for comparative period</param>
        /// <returns>Sql query as a string</returns>
        protected virtual string GetQueryForWebPlanEvaliant(GenericDetailLevel detailLevel, CstPeriod.DisplayLevel periodDisplay,
            CstPeriod.PeriodBreakdownType periodBreakDown, List<PeriodItem> periodItems, string additionalConditions, bool isComparative)
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
            string unitAlias = null;
            string mediaJoinCondition = null;
            string groupByFieldName = null;
            string groupByOptional = null;
            //VehicleInformation vehicleInfo = null;
            #endregion

            //if (VehiclesInformation.Contains(vehicleId))
            //    vehicleInfo = VehiclesInformation.Get(vehicleId);

            #region Construction de la requête
            try
            {
                // Get the name of the data table	
                switch (periodBreakDown)
                {
                    case CstPeriod.PeriodBreakdownType.month:
                        tableName = WebApplicationParameters.GetDataTable(TableIds.monthData, _session.IsSelectRetailerDisplay).SqlWithPrefix;
                        break;
                    case CstPeriod.PeriodBreakdownType.week:
                        tableName = WebApplicationParameters.GetDataTable(TableIds.weekData, _session.IsSelectRetailerDisplay).SqlWithPrefix;
                        break;
                    default:
                        throw (new MediaScheduleDALException("Unable to determine table to use."));
                }
                //Get unit field
                unitFieldName = _session.GetSelectedUnit().DatabaseMultimediaField;

                // Get the classification table
                mediaTableName = detailLevel.GetSqlTables(_schAdexpr03.Label);
                if (mediaTableName.Length > 0) mediaTableName += ",";
                // Get unit field
                dateFieldName = SQLGenerator.GetDateFieldName(periodBreakDown);
                unitAlias = SQLGenerator.GetUnitAlias(_session);
                // isComparative
                if (!isComparative)
                    mediaPeriodicity = GetPeriodicity(periodBreakDown, -1, periodDisplay);

                // Get classification fields				
                mediaFieldName = GetAdnettrackSqlFields(detailLevel);

                // Get field order
                orderFieldName = detailLevel.GetSqlOrderFields();

                // Get group by clause				
                groupByFieldName = GetAdnettrackSqlGroupByFields(detailLevel);

                // Get joins for classification
                mediaJoinCondition = detailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            }
            catch (System.Exception err)
            {
                throw (new MediaScheduleDALException("Unable to build the query.", err));
            }

            // Select : Media classificaion selection
            sql.AppendFormat("select {0} ", mediaFieldName);

            // Date selection
            if (periodDisplay != CstPeriod.DisplayLevel.dayly
                && (periodBreakDown == CstPeriod.PeriodBreakdownType.data
                || periodBreakDown == CstPeriod.PeriodBreakdownType.data_4m))
            {
                if (periodDisplay != CstPeriod.DisplayLevel.weekly)
                    sql.AppendFormat(", to_number(to_char(to_date({0}, 'YYYYMMDD'), 'YYYYMM')) as date_num,", dateFieldName);
                else
                    sql.AppendFormat(", to_number(to_char(to_date({0}, 'YYYYMMDD'), 'IYYYIW')) as date_num,", dateFieldName);
            }
            else
            {
                sql.AppendFormat(", {0} as date_num,", dateFieldName);
            }

            // isComparative
            if (!isComparative)
                sql.AppendFormat("{0}, ", mediaPeriodicity);

            switch (periodBreakDown)
            {
                case CstPeriod.PeriodBreakdownType.data:
                case CstPeriod.PeriodBreakdownType.data_4m:
                    unitFieldName = string.Format(" to_char({0}.{1}) as {2} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                        _session.GetSelectedUnit().DatabaseField, _session.GetSelectedUnit().Id.ToString());
                    groupByOptional = string.Format(", {0}.{1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                        _session.GetSelectedUnit().DatabaseField);
                    break;
                default:
                    unitFieldName = string.Format(" {0}.{1} as {2} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                        _session.GetSelectedUnit().DatabaseMultimediaField, _session.GetSelectedUnit().Id.ToString());
                    groupByOptional = string.Format(", {0}.{1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                        _session.GetSelectedUnit().DatabaseMultimediaField);
                    break;
            }
            sql.AppendFormat("{0}", unitFieldName);

            // From : Tables
            sql.AppendFormat(" from {0}{1} ", mediaTableName, tableName);

            // Where : Conditions media
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
                sql.AppendFormat(" and {0}<={1}", dateFieldName, periodItem.End);
            }
            if (!first)
            {
                sql.Append(")) ");
            }

            // Autopromo			
            if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser) // Hors autopromo (checkbox = checked)
                sql.AppendFormat(" and {0}.auto_promotion = 0 ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            else if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany)
            {

                Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                sql.AppendFormat(" and ({0}.id_media, {0}.id_holding_company) not in ( ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                sql.AppendFormat(" select distinct id_media_evaliant, id_holding_company ");
                sql.AppendFormat(" from {0} ", tblAutoPromo.Sql);
                sql.AppendFormat(" where id_media_evaliant is not null ");
                sql.AppendFormat(" ) ");
            }

            // Additional conditions            
            if (additionalConditions.Length > 0)
            {
                sql.AppendFormat(" {0} ", additionalConditions);
            }

            // Version selection
            string slogans = _session.SloganIdList;

            // Zoom on a specific version
            TNS.AdExpress.Web.Core.Utilities.Creatives creativesUtilities = new TNS.AdExpress.Web.Core.Utilities.Creatives();
            if (creativesUtilities.IsSloganZoom(_session.SloganIdZoom) && periodDisplay == CstPeriod.DisplayLevel.dayly)
            {
                sql.AppendFormat(" and {0}.{2} = {1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.SloganIdZoom
                    , "hashcode");
            }
            else
            {
                // Refine vesions
                if (slogans.Length > 0 && periodDisplay == CstPeriod.DisplayLevel.dayly)
                {
                    sql.AppendFormat(" and {0}.{2} in({1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, slogans
                        , "hashcode");
                }
            }

            // Selection and right managment

            #region Nomenclature Produit (droits)
            //Access rgithDroits en accès
            if (_module == null) throw (new MediaScheduleDALException("_module cannot be NULL"));
            sql.Append(SQLGenerator.GetClassificationCustomerProductRight(_session,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches));

            // Exclude product if radio selected)
            GetExcludeProudcts(sql);
            #endregion

            #region Nomenclature Produit (Niveau de détail)
            // Product level
            sql.Append(SQLGenerator.getLevelProduct(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            #endregion

            #region Sélection produit
            // product
            sql.Append(GetProductSelection(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            #endregion

            #region Sélection support
            // media
            sql.Append(GetMediaSelection(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            #endregion

            #region Media classification

            #region Rights
            sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            #endregion

            #region Selection
            list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess);
            if (list.Length > 0)
            {
                sql.AppendFormat(" and ({0}.id_vehicle in ({1})) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, list);
            }
            #endregion

            //Universe media
            sql.Append(GetMediaUniverse(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

            #endregion

            #region Banners Format Filter
            if (list.Length > 0)
            {
                var sqlFormatSelectedClause = new StringBuilder();
                sqlFormatSelectedClause.Append(" and (");
                var vehicleInfoList = _session.GetVehiclesSelected();
                bool firstVehicle = true;
                bool hasValidFormat = false;
                foreach (var cVehicleInformation in vehicleInfoList.Values)
                {
                    VehicleInformation cVehicleInfo;
                    if (_isAdNetTrackMediaSchedule && VehiclesInformation.Contains(CstDBClassif.Vehicles.names.internet)
                        && cVehicleInformation.DatabaseId == VehiclesInformation.Get(CstDBClassif.Vehicles.names.internet).DatabaseId)
                        cVehicleInfo = VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack);
                    else
                        cVehicleInfo = cVehicleInformation;
                    if (firstVehicle) firstVehicle = false;
                    else sqlFormatSelectedClause.Append(" OR ");
                    sqlFormatSelectedClause.Append(" (");
                   
                    var formatIdList = _session.GetValidFormatSelectedList(new List<VehicleInformation>(new[] { cVehicleInfo }));

                    if (formatIdList != null && formatIdList.Count > 0)
                    {
                        sqlFormatSelectedClause.AppendFormat(" {0}.id_vehicle = {1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, cVehicleInfo.DatabaseId);
                        sqlFormatSelectedClause.AppendFormat("and {0}.ID_{1} in ({2}) "
                            , WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                            WebApplicationParameters.DataBaseDescription.GetTable(WebApplicationParameters.
                                                                                                                                                                                                                                                                                                                                                                                                                                                                          VehiclesFormatInformation.VehicleFormatInformationList[cVehicleInfo.DatabaseId].FormatTableName).Label, string.Join(",", formatIdList.ConvertAll(p => p.ToString()).ToArray()));
                        hasValidFormat = true;
                    }
                    else
                    {
                        sqlFormatSelectedClause.AppendFormat(" {0}.id_vehicle = {1} ",
                            WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, cVehicleInfo.DatabaseId);
                    }
                    sqlFormatSelectedClause.Append(") ");
                }
                sqlFormatSelectedClause.Append(" ) ");
                if (hasValidFormat)
                    sql.Append(sqlFormatSelectedClause.ToString());
            }
            #endregion

            // Order
            sql.AppendFormat("Group by {0} {1}", groupByFieldName, groupByOptional);
            // And date
            sql.AppendFormat(", {0} ", dateFieldName);
            #endregion

            return (sql.ToString());
        }
        #endregion

        #region Queries building stuff

        #region Périodicité
        /// <summary>
        /// Get field for periodicity data
        /// </summary>
        /// <remarks>Finish implementation exists for Finland</remarks>
        ///<param name="detailPeriod">Detail period type</param>
        ///<param name="vehicleId">Vehicle Id</param>
        /// <param name="displayPeriod">Result display level</param>
        /// <returns>Field matching period type</returns>
        protected virtual string GetPeriodicity(CstPeriod.PeriodBreakdownType detailPeriod, Int64 vehicleId, CstPeriod.DisplayLevel displayPeriod)
        {
            switch (detailPeriod)
            {
                case CstPeriod.PeriodBreakdownType.month:
                    return (" max(duration) as period_count ");
                case CstPeriod.PeriodBreakdownType.week:
                    return (" max(duration) as period_count ");
                case CstPeriod.PeriodBreakdownType.data:
                case CstPeriod.PeriodBreakdownType.data_4m:
                    switch (VehiclesInformation.DatabaseIdToEnum(vehicleId))
                    {
                        case CstDBClassif.Vehicles.names.press:
                        case CstDBClassif.Vehicles.names.newspaper:
                        case CstDBClassif.Vehicles.names.magazine:
                        case CstDBClassif.Vehicles.names.internationalPress:
                        case CstDBClassif.Vehicles.names.outdoor:
                        case CstDBClassif.Vehicles.names.instore:
                        case CstDBClassif.Vehicles.names.indoor:
                            switch (displayPeriod)
                            {
                                case CstPeriod.DisplayLevel.monthly:
                                    return string.Format(" max({0}DURATION_MONTH(date_media_num,duration)) as period_count ", _schAdexpr03.Sql);
                                case CstPeriod.DisplayLevel.weekly:
                                    return string.Format(" max({0}DURATION_WEEK(date_media_num,duration)) as period_count ", _schAdexpr03.Sql);
                                default:
                                    if (VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.instore
                                        || VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.indoor
                                        || VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.outdoor)
                                    {
                                        return (" trunc(CEIL(max(duration)/86400)) as period_count ");
                                    }
                                    else
                                    {
                                        return (" trunc((max(duration)/86400)) as period_count ");
                                    }
                            }
                        case CstDBClassif.Vehicles.names.radio:
                        case CstDBClassif.Vehicles.names.radioGeneral:
                        case CstDBClassif.Vehicles.names.radioSponsorship:
                        case CstDBClassif.Vehicles.names.radioMusic:
                        case CstDBClassif.Vehicles.names.tv:
                        case CstDBClassif.Vehicles.names.tvGeneral:
                        case CstDBClassif.Vehicles.names.tvSponsorship:
                        case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                        case CstDBClassif.Vehicles.names.tvAnnounces:
                        case CstDBClassif.Vehicles.names.others:
                        case CstDBClassif.Vehicles.names.internet:
                        case CstDBClassif.Vehicles.names.czinternet:
                        case CstDBClassif.Vehicles.names.adnettrack:
                        case CstDBClassif.Vehicles.names.evaliantMobile:
                        case CstDBClassif.Vehicles.names.directMarketing:
                        case CstDBClassif.Vehicles.names.mailValo:
                        case CstDBClassif.Vehicles.names.cinema: // A Changer quand les durées seront bonnes
                        case CstDBClassif.Vehicles.names.mms:
                        case CstDBClassif.Vehicles.names.search:
                        case CstDBClassif.Vehicles.names.social:
                            return (" 1 as period_count ");
                        default:
                            throw (new MediaScheduleDALException("Unable to determine the media periodicity"));
                    }
                default:
                    throw (new MediaScheduleDALException("Selected period detail unvalid. Unable to determine periodicity field."));
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
        /// <param name="dataTablePrefixe">data table prefixe</param>
        /// <returns>product selection to add as condition into a sql query</returns>
        protected virtual string GetProductSelection(string dataTablePrefixe)
        {
            string sql = "";
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql = _session.PrincipalProductUniverses[0].GetSqlConditions(dataTablePrefixe, true);
            return sql;
        }
        #endregion

        #region GetMediatSelection
        /// <summary>
        /// Get media selection
        /// </summary>
        /// <remarks>
        /// Must beginning by AND
        /// </remarks>
        /// <param name="dataTablePrefixe">data table prefixe</param>
        /// <returns>media selection to add as condition into a sql query</returns>
        protected virtual string GetMediaSelection(string dataTablePrefixe)
        {
            string sql = "";
            if (_session.SecondaryMediaUniverses != null && _session.SecondaryMediaUniverses.Count > 0)
                sql = _session.SecondaryMediaUniverses[0].GetSqlConditions(dataTablePrefixe, true);
            return sql;
        }
        #endregion

        /// <summary>
        /// Get excluded products
        /// </summary>
        /// <param name="sql">String builder</param>
        /// <returns></returns>
        protected virtual void GetExcludeProudcts(StringBuilder sql)
        {
            // Exclude product 
            ProductItemsList prList = null;
            if (Product.Contains(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID) && (prList = Product.GetItemsList(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID)) != null)
                sql.Append(prList.GetExcludeItemsSql(true, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
        }
        /// <summary>
        /// Get media Universe
        /// </summary>
        /// <param name="webSession">Web Session</param>
        /// <returns>string sql</returns>
        protected virtual string GetMediaUniverse(WebSession webSession, string prefix)
        {
            string sql = "";
            WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
            ResultPageInformation resPageInfo = module.GetResultPageInformation(webSession.CurrentTab);
            if (resPageInfo != null)
                sql = resPageInfo.GetAllowedMediaUniverseSql(prefix, true);
            return sql;
        }

        /// <summary>
        /// Détermine le nom du champ à utiliser pour l'unité
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Nom du champ à utiliser pour l'unité</returns>
        protected virtual string GetUnitFieldNameSumUnionWithAlias(WebSession webSession)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                if (webSession.GetSelectedUnit().Id != CstWeb.CustomerSessions.Unit.versionNb)
                    sql.AppendFormat("sum({0}) as {0}", webSession.GetSelectedUnit().Id.ToString());
                else
                    sql.AppendFormat("{0} as {0}", webSession.GetSelectedUnit().Id.ToString());

                return sql.ToString();
            }
            catch
            {
                throw new MediaScheduleDALException("Not managed unit (Alert Module)");
            }
        }

        /// <summary>
        /// Obtient le code SQL des champs correspondant aux éléments du niveau de détail
        /// </summary>
        /// <remarks>Ne termine pas par une virgule</remarks>
        /// <returns>Code SQL</returns>
        protected virtual string GetAdnettrackSqlFields(GenericDetailLevel levelsList)
        {
            string sql = "";
            string prefix = "";
            foreach (DetailLevelItemInformation currentLevel in levelsList.Levels)
            {
                if (currentLevel.Id != DetailLevelItemInformation.Levels.slogan)
                    sql += currentLevel.GetSqlFieldId() + "," + currentLevel.GetSqlField() + ",";
                else
                {
                    if (!string.IsNullOrEmpty(currentLevel.DataBaseTableNamePrefix))
                        prefix = currentLevel.DataBaseTableNamePrefix + ".";
                    sql += "nvl(" + prefix + "hashcode,0) as " + currentLevel.DataBaseAliasIdField + ",nvl(" + prefix + "hashcode,0) as " + currentLevel.DataBaseAliasField + ",";
                }
            }
            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);
            return (sql);
        }

        /// <summary>
        /// Obtient le code SQL de la clause group by correspondant aux éléments du niveau de détail
        /// </summary>
        /// <remarks>Ne termine pas par une virgule</remarks>
        /// <returns>Code SQL</returns>
        protected virtual string GetAdnettrackSqlGroupByFields(GenericDetailLevel levelsList)
        {
            string sql = "";
            foreach (DetailLevelItemInformation currentLevel in levelsList.Levels)
            {
                if (currentLevel.Id != DetailLevelItemInformation.Levels.slogan)
                    sql += currentLevel.GetSqlIdFieldForGroupBy() + "," + currentLevel.GetSqlFieldForGroupBy() + ",";
                else
                {
                    sql += "hashcode,hashcode,";
                }
            }
            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);
            return (sql);
        }
        #endregion

    }
}
