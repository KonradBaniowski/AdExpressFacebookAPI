using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDB = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.MediaSchedule.DAL.Exceptions;

namespace TNS.AdExpressI.MediaSchedule.DAL.Turkey
{
    public class MediaScheduleResultDAL : DAL.MediaScheduleResultDAL
    {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period) : base(session, period) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        /// <param name="idVehicle">Id of the vehicle</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period, Int64 idVehicle) : base(session, period, idVehicle) { }
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
        protected override string GetMasterQuery(string additionalWhereClause, GenericDetailLevel detailLevel, TNS.AdExpress.Web.Core.Selection.MediaSchedulePeriod period, bool isComparative, string additionalSelect, string additionalGroup)
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
            sql.AppendFormat("select {0}, {1}{2} from (", detailLevel.GetSqlFieldsWithoutTablePrefix(), additionalSelect, GetUnitsFieldNameSumUnionWithAlias(_session));

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
        protected override string GetQuery(GenericDetailLevel detailLevel, CstPeriod.DisplayLevel periodDisplay, CstPeriod.PeriodBreakdownType periodBreakDown, Int64 vehicleId, List<PeriodItem> periodItems, string additionalConditions, bool isComparative)
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
            string unitsFieldNameWithAlias = string.Empty;
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
                tableName = SQLGenerator.GetDataTableName(periodBreakDown, vehicleId, _session.IsSelectRetailerDisplay);
                unitsFieldNameWithAlias = SQLGenerator.GetUnitsFieldNameWithAlias(_session, vehicleId, periodBreakDown);
                // Get the classification table
                mediaTableName = detailLevel.GetSqlTables(_schAdexpr03.Label);
                if (mediaTableName.Length > 0) mediaTableName += ",";
                // Get unit field
                dateFieldName = SQLGenerator.GetDateFieldName(periodBreakDown);
                // isComparative
                if (!isComparative)
                    mediaPeriodicity = GetPeriodicity(periodBreakDown, vehicleId, periodDisplay);

                // Get classification fields
                mediaFieldName = detailLevel.GetSqlFields();

                // Get field order
                orderFieldName = detailLevel.GetSqlOrderFields();

                // Get group by clause
                groupByFieldName = detailLevel.GetSqlGroupByFields();

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

            sql.AppendFormat(unitsFieldNameWithAlias);
            sql.Append(GetGrpFieldName(periodBreakDown));

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
                                          _session.SloganIdZoom, 
                                          "id_slogan");
                }
                else
                {
                    // Refine vesions
                    if (slogans.Length > 0 && periodDisplay == CstPeriod.DisplayLevel.dayly)
                    {
                        sql.AppendFormat(" and {0}.{2} in({1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, slogans
                            , "id_slogan");
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
            sql.Append(GetProductSelection(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            #endregion

            #region Sélection support
            // media
            if (_session.CurrentModule == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA)
                sql.Append(GetMediaSelection(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            #endregion

            #region Media classification

            #region Rights
            sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            #endregion

            #region Selection
            list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess);
            if (vehicleId > -1)
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

            // Order
            sql.AppendFormat("Group by {0} {1}", groupByFieldName, groupByOptional);
            // And date
            sql.AppendFormat(", {0} ", dateFieldName);
            #endregion

            return (sql.ToString());
        }
        #endregion

        private string GetGrpFieldName(CstPeriod.PeriodBreakdownType periodType)
        {
            string grpFieldName = string.Empty;

            switch (periodType)
            {
                case CstPeriod.PeriodBreakdownType.week:
                case CstPeriod.PeriodBreakdownType.month:

                    try
                    {
                        if (_session.Grp)
                            grpFieldName = ", sum(TOTAL_GRP_SPOT) as GRP";

                        if(_session.Grp30S)
                            grpFieldName = ", sum(TOTAL_GRP_SPOT_30S) as GRP";

                        return grpFieldName;
                    }
                    catch
                    {
                        throw (new MediaScheduleDALException("Selected unit detail is uncorrect. Unable to determine unit field."));
                    }

                case CstPeriod.PeriodBreakdownType.data:
                case CstPeriod.PeriodBreakdownType.data_4m:

                    try
                    {
                        if (_session.Grp)
                            grpFieldName = ", sum(GRP_TV_SPOT) as GRP";

                        if (_session.Grp30S)
                            grpFieldName = ", sum(GRP_TV_SPOT_30S) as GRP";

                        return grpFieldName;
                    }
                    catch
                    {
                        throw (new MediaScheduleDALException("Selected unit detail is uncorrect. Unable to determine unit field."));
                    }

                default:
                    throw (new MediaScheduleDALException("Selected period detail is uncorrect. Unable to determine unit field."));

            }
        }

        protected override string GetUnitsFieldNameSumUnionWithAlias(WebSession webSession)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                List<UnitInformation> unitsInformation = webSession.GetSelectedUnits();

                foreach (var unit in unitsInformation)
                    sql.AppendFormat("sum({0}) as {0}, ", unit.Id.ToString());

                if (_session.Grp || _session.Grp30S)
                    sql.AppendFormat("sum(GRP) as GRP, ");

                return sql.ToString().Substring(0, sql.Length - 2);
            }
            catch
            {
                throw new MediaScheduleDALException("Not managed unit (Alert Module)");
            }
        }
    }
}
