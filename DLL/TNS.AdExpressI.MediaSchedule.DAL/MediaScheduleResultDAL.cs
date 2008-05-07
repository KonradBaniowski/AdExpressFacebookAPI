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
using TNS.AdExpressI.MediaSchedule.DAL.Exceptions;
#endregion

namespace TNS.AdExpressI.MediaSchedule.DAL
{
    /// <summary>
    /// Abstract Implementation of IMediaScheduleResultDAL
    /// </summary>
    public abstract class MediaScheduleResultDAL : IMediaScheduleResultDAL
    {

        #region Properties

        #region DataBase description
        protected static Schema _schAdexpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
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
            set { value = _session; }
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
            set { value = _period; }
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
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period, Int64 idVehicle):this(session, period)
        {
            _vehicleId = idVehicle;

        }
        #endregion

        #region IMediaScheduleResultDAL Membres

        #region GetData
        /// <summary>
        /// Get Data to build Media Schedule
        /// </summary>
        /// <returns>DataSet containing Data</returns>
        public DataSet GetData()
        {
            return GetData(string.Empty, _session.GenericMediaDetailLevel);
        }
        #endregion

        #region GetAdNetTrackData
        /// <summary>
        /// Get Data for AdNetTrack Media Schedule
        /// </summary>
        /// <returns>Data</returns>
        public DataSet GetAdNetTrackData()
        {
            string additionalConditions = "";
            this._vehicleId = CstDBClassif.Vehicles.names.adnettrack.GetHashCode();
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
        protected DataSet GetData(string additionalWhereClause, GenericDetailLevel detailLevel)
        {

            #region Query Building
            bool first = true;

            string[] listVehicles = null;
            if (_vehicleId < 0)
            {
                listVehicles = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess).Split(new char[] { ',' });
            }
            else
            {
                listVehicles = new string[1] { _vehicleId.ToString() };
            }

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
                        for (int i = 0; i < listVehicles.Length; i++)
                        {
                            try
                            {
                                if (!first) sql.Append(" union all ");
                                else first = false;
                                sql.AppendFormat("({0})", GetQuery(detailLevel, _period.PeriodDetailLEvel, subPeriods.SubPeriodType, Int64.Parse(listVehicles[i]), subPeriods.Items, additionalWhereClause));
                            }
                            catch (System.Exception err)
                            {
                                throw new MediaScheduleDALException("GenericMediaScheduleDataAccess.GetData(WebSession _session, MediaSchedulePeriod _period)", err);
                            }
                        }

                        break;
                    case CstPeriod.PeriodBreakdownType.week:
                    case CstPeriod.PeriodBreakdownType.month:
                        if (!first) sql.Append(" union all ");
                        else first = false;
                        sql.AppendFormat("({0})",
                            GetQuery(detailLevel, _period.PeriodDetailLEvel, subPeriods.SubPeriodType, -1, subPeriods.Items, string.Empty));
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
                throw (new MediaScheduleDALException("Unable to load Media Schedule Data : " + sql, err));
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
        /// <param name="vehicleId">Vehicle filter for desagregated data</param>
        /// <param name="beginningDate">Period Beginning</param>
        /// <param name="endDate">Period End</param>
        /// <param name="additionalConditions">Addtional conditions such as AdNetTrack Baners...</param>
        /// <returns>Sql query as a string</returns>
        protected string GetQuery(GenericDetailLevel detailLevel, CstPeriod.DisplayLevel periodDisplay, CstPeriod.PeriodBreakdownType periodBreakDown, Int64 vehicleId, List<PeriodItem> periodItems, string additionalConditions)
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
            #endregion

            #region Construction de la requête
            try
            {

                // Get the name of the data table
                tableName = GetDataTableName(periodBreakDown, vehicleId);
                // Get the classification table
                mediaTableName = detailLevel.GetSqlTables(_schAdexpr03.Label);
                if (mediaTableName.Length > 0) mediaTableName += ",";
                // Get unit field
                dateFieldName = GetDateFieldName(periodBreakDown);
                // Outdoor alert case
                if ((CstDBClassif.Vehicles.names)vehicleId == CstDBClassif.Vehicles.names.outdoor && _session.Unit == CstWeb.CustomerSessions.Unit.insertion
                    && (periodBreakDown == CstPeriod.PeriodBreakdownType.data_4m || periodBreakDown == CstPeriod.PeriodBreakdownType.data))
                    unitFieldName = CstDB.Fields.NUMBER_BOARD;
                else
                    unitFieldName = GetUnitFieldName(periodBreakDown);
                // Periodicity
                mediaPeriodicity = GetPeriodicity(periodBreakDown, vehicleId, periodDisplay);
                // Get classification fields
                mediaFieldName = detailLevel.GetSqlFields();
                // Get field order
                orderFieldName = detailLevel.GetSqlOrderFields();
                // Get group by clause
                groupByFieldName = detailLevel.GetSqlGroupByFields();
                // Get joins for classification
                mediaJoinCondition = detailLevel.GetSqlJoins(_session.SiteLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
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
            // Unit selection expect for AdNetTrack
            if ((CstDBClassif.Vehicles.names)vehicleId == CstDBClassif.Vehicles.names.adnettrack)
                sql.Append("sum(OCCURRENCE) as unit");
            else
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
            if (additionalConditions.Length > 0)
            {
                sql.AppendFormat(" {0} ", additionalConditions);
            }

            // Version selection
            string slogans = _session.SloganIdList;
            // Zoom on a specific version
            if (_session.SloganIdZoom > 0 && periodDisplay == CstPeriod.DisplayLevel.dayly)
            {
                sql.AppendFormat(" and {0}.id_slogan = {1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.SloganIdZoom );
            }
            else
            {
                // Refine vesions
                if (slogans.Length > 0 && periodDisplay == CstPeriod.DisplayLevel.dayly)
                {
                    sql.AppendFormat(" and {0}.id_slogan in({1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, slogans);
                }
            }

            // Selection and right managment

            #region Nomenclature Produit (droits)
            //Access rgithDroits en accès
            sql.Append(SQLGenerator.getAnalyseCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            // Exclude product if radio selected)
            sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false));
            #endregion

            #region Nomenclature Produit (Niveau de détail)
            // Product level
            sql.Append(SQLGenerator.getLevelProduct(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            #endregion

            #region Advertiser classification rights

            #region Sélection
            sql.Append(GetProductSelection(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            #endregion

            #endregion

            #region Media classification

            #region Rights
            // No media right if AdNetTrack media schedule
            if ((CstDBClassif.Vehicles.names)vehicleId == CstDBClassif.Vehicles.names.adnettrack)
                sql.Append(SQLGenerator.GetAdNetTrackMediaRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            else
                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            #endregion

            #region Selection
            if (periodDisplay != CstPeriod.DisplayLevel.dayly && (CstDBClassif.Vehicles.names)vehicleId != CstDBClassif.Vehicles.names.adnettrack)
            {
                list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess);
                if (list.Length > 0) sql.AppendFormat(" and ({0}.id_vehicle in ({1})) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, list);
            }
            else
                sql.AppendFormat(" and ({0}.id_vehicle={1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, vehicleId);
            #endregion

            #endregion

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
        /// <param name="vehicleId">Vehicle Id</param>
        /// <returns>Table matching the vehicle and the type of period</returns>
        protected string GetDataTableName(CstPeriod.PeriodBreakdownType period, Int64 vehicleId)
        {
            switch (period)
            {
                case CstPeriod.PeriodBreakdownType.month:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData).SqlWithPrefix;
                case CstPeriod.PeriodBreakdownType.week:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.weekData).SqlWithPrefix;
                case CstPeriod.PeriodBreakdownType.data_4m:
                    switch ((CstDBClassif.Vehicles.names)Convert.ToInt32(vehicleId))
                    {
                        case CstDBClassif.Vehicles.names.press:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataPressAlert).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.internationalPress:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataPressInterAlert).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.radio:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataRadioAlert).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.tv:
                        case CstDBClassif.Vehicles.names.others:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataTvAlert).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.outdoor:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataOutDoorAlert).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.adnettrack:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataAdNetTrackAlert).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.internet:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataInternetAlert).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.directMarketing:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataMarketingDirectAlert).SqlWithPrefix;
                        default:
                            throw (new MediaScheduleDALException("Unable to determine table to use."));
                    }
                case CstPeriod.PeriodBreakdownType.data:
                    switch ((CstDBClassif.Vehicles.names)Convert.ToInt32(vehicleId.ToString()))
                    {
                        case CstDBClassif.Vehicles.names.press:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataPress).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.internationalPress:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataPressInter).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.radio:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataRadio).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.tv:
                        case CstDBClassif.Vehicles.names.others:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataTv).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.outdoor:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataOutDoor).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.adnettrack:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataAdNetTrack).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.internet:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataInternet).SqlWithPrefix;
                        case CstDBClassif.Vehicles.names.directMarketing:
                            return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataMarketingDirect).SqlWithPrefix;
                        default:
                            throw (new MediaScheduleDALException("Unable to determine the table to use"));
                    }
                default:
                    throw (new MediaScheduleDALException("The detail selected is not a correct one to to choos of the tablme"));
            }
        }
        #endregion

        #region GetDateFieldName
        /// <summary>
        /// Get Field to use for date
        /// </summary>
        /// <param name="period">Type of period</param>
        /// <returns>Date Filed Name matchnig the type of period</returns>
        protected string GetDateFieldName(CstPeriod.PeriodBreakdownType period)
        {
            switch (period)
            {
                case CstPeriod.PeriodBreakdownType.month:
                    return (CstDB.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD);
                case CstPeriod.PeriodBreakdownType.week:
                    return (CstDB.Fields.WEB_PLAN_MEDIA_WEEK_DATE_FIELD);
                case CstPeriod.PeriodBreakdownType.data:
                case CstPeriod.PeriodBreakdownType.data_4m:
                    return ("date_media_num");
                default:
                    throw (new MediaScheduleDALException("Selected detail period is uncorrect. Unable to determine date field to use."));
            }
        }
        #endregion

        #region GetUnitFieldName
        /// <summary>
        /// Get unit field to use in query
        /// </summary>
        /// <returns>Unit field name</returns>
        protected string GetUnitFieldName(CstPeriod.PeriodBreakdownType periodType)
        {
            switch (periodType)
            {
                case CstPeriod.PeriodBreakdownType.week:
                case CstPeriod.PeriodBreakdownType.month:
                    switch (_session.Unit)
                    {
                        case CstWeb.CustomerSessions.Unit.euro:
                        case CstWeb.CustomerSessions.Unit.kEuro:
                            return (CstDB.Fields.WEB_PLAN_MEDIA_MONTH_EURO_FIELD);
                        case CstWeb.CustomerSessions.Unit.mmPerCol:
                            return (CstDB.Fields.WEB_PLAN_MEDIA_MONTH_MMC_FIELD);
                        case CstWeb.CustomerSessions.Unit.pages:
                            return (CstDB.Fields.WEB_PLAN_MEDIA_MONTH_PAGES_FIELD);
                        case CstWeb.CustomerSessions.Unit.insertion:
                        case CstWeb.CustomerSessions.Unit.numberBoard:
                            return (CstDB.Fields.WEB_PLAN_MEDIA_MONTH_INSERT_FIELD);
                        case CstWeb.CustomerSessions.Unit.spot:
                            return (CstDB.Fields.WEB_PLAN_MEDIA_MONTH_INSERT_FIELD);
                        case CstWeb.CustomerSessions.Unit.duration:
                            return (CstDB.Fields.WEB_PLAN_MEDIA_MONTH_DUREE_FIELD);
                        case CstWeb.CustomerSessions.Unit.volume:
                            if (_session.CustomerLogin.GetFlag(CstDB.Flags.ID_VOLUME_MARKETING_DIRECT) != null)
                                return (CstDB.Fields.WEB_PLAN_MEDIA_MONTH_VOLUME_FIELD);
                            else
                                return (CstDB.Fields.WEB_PLAN_MEDIA_MONTH_EURO_FIELD);
                        default:
                            throw (new MediaScheduleDALException("Selected unit detail is uncorrect. Unable to determine unit field."));
                    }
                case CstPeriod.PeriodBreakdownType.data:
                case CstPeriod.PeriodBreakdownType.data_4m:
                    switch (_session.Unit)
                    {
                        case CstWeb.CustomerSessions.Unit.euro:
                        case CstWeb.CustomerSessions.Unit.kEuro:
                            return (CstDB.Fields.EXPENDITURE_EURO);
                        case CstWeb.CustomerSessions.Unit.mmPerCol:
                            return (CstDB.Fields.AREA_MMC);
                        case CstWeb.CustomerSessions.Unit.pages:
                            return (CstDB.Fields.AREA_PAGE);
                        case CstWeb.CustomerSessions.Unit.spot:
                        case CstWeb.CustomerSessions.Unit.insertion:
                            return (CstDB.Fields.INSERTION);
                        case CstWeb.CustomerSessions.Unit.numberBoard:
                            return (CstDB.Fields.NUMBER_BOARD);
                        case CstWeb.CustomerSessions.Unit.duration:
                            return (CstDB.Fields.DURATION);
                        case CstWeb.CustomerSessions.Unit.volume:
                            if (_session.CustomerLogin.GetFlag(CstDB.Flags.ID_VOLUME_MARKETING_DIRECT) != null)
                                return (CstDB.Fields.VOLUME);
                            else
                                return (CstDB.Fields.EXPENDITURE_EURO);
                        default:
                            throw (new MediaScheduleDALException("Selected unit detail is uncorrect. Unable to determine unit field."));
                    }
                default:
                    throw (new MediaScheduleDALException("Selected period detail is uncorrect. Unable to determine unit field."));

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
        protected string GetPeriodicity(CstPeriod.PeriodBreakdownType detailPeriod, Int64 vehicleId, CstPeriod.DisplayLevel displayPeriod)
        {


            switch (detailPeriod)
            {
                case CstPeriod.PeriodBreakdownType.month:
                    return (" max(duration) as period_count ");
                case CstPeriod.PeriodBreakdownType.week:
                    return (" max(duration) as period_count ");
                case CstPeriod.PeriodBreakdownType.data:
                case CstPeriod.PeriodBreakdownType.data_4m:
                    switch ((CstDBClassif.Vehicles.names)Convert.ToInt32(vehicleId.ToString()))
                    {
                        case CstDBClassif.Vehicles.names.press:
                        case CstDBClassif.Vehicles.names.internationalPress:
                        case CstDBClassif.Vehicles.names.outdoor:
                            switch (displayPeriod)
                            {
                                case CstPeriod.DisplayLevel.monthly:
                                    return string.Format(" max({0}DURATION_MONTH(date_media_num,duration)) as period_count ", _schAdexpr03.Sql);
                                case CstPeriod.DisplayLevel.weekly:
                                    return string.Format(" max({0}DURATION_WEEK(date_media_num,duration)) as period_count ", _schAdexpr03.Sql);
                                default:
                                    return (" (max(duration)/86400) as period_count ");
                            }
                        case CstDBClassif.Vehicles.names.radio:
                        case CstDBClassif.Vehicles.names.tv:
                        case CstDBClassif.Vehicles.names.others:
                        case CstDBClassif.Vehicles.names.internet:
                        case CstDBClassif.Vehicles.names.adnettrack:
                            return (" 1 as period_count ");
                        case CstDBClassif.Vehicles.names.directMarketing:
                            return (" 7 as period_count ");
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
        protected string GetProductSelection(string dataTablePrefixe)
        {
            string sql = "";
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql = _session.PrincipalProductUniverses[0].GetSqlConditions(dataTablePrefixe, true);
            return sql;
        }
        #endregion

        #endregion


    }
}
