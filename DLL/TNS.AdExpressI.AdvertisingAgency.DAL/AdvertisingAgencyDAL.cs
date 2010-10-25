﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using TNS.AdExpress.Web.Core.Sessions;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpressI.AdvertisingAgency.DAL.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using FctWeb = TNS.AdExpress.Web.Functions;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web.Navigation;
using System.Collections;

namespace TNS.AdExpressI.AdvertisingAgency.DAL
{
    /// <summary>
    /// Extract data for different type of results of the module Advertising Agency Report.
    /// </summary>
    public abstract class AdvertisingAgencyDAL : IAdvertisingAgencyDAL
    {

        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Media Schedule Period Management regarding to global date selection
        /// </summary>
        protected MediaSchedulePeriod _period = null;
        /// <summary>
        /// DataBase Schema
        /// </summary>
        protected static Schema _schAdexpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Media Schedule Period</param>
        public AdvertisingAgencyDAL(WebSession session, MediaSchedulePeriod period)
        {
            _session = session;
            _period = period;
        }
        #endregion

        #region GetData
        /// <summary>
        /// Load data for the module Advertising Agency report.
        /// </summary>
        /// <returns>DataSet</returns>
        public virtual DataSet GetData() {

            #region Variables
            bool first = true;
            DataSet ds = new DataSet();
            string[] listVehicles = null;
            StringBuilder sql = new StringBuilder();
            string groupOptional = string.Empty;
            GenericDetailLevel detailLevel = _session.GenericProductDetailLevel;
            GenericDetailLevel mediaDetailLevel = SetGenericMediaDetailLevel();
            MediaSchedulePeriod comparativePeriod = null;
            int comparativeIndex = 1;
            string N = string.Empty, N1 = string.Empty;
            #endregion

            #region Query Building
            listVehicles = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess).Split(new char[] { ',' });
            
            if (_session.ComparativeStudy) {
                comparativePeriod = new MediaSchedulePeriod(_period.Begin.AddMonths(-12), _period.End.AddMonths(-12), _session.DetailPeriod);
                comparativeIndex = 0;
            }

            // Select
            //sql.AppendFormat("select {0}, {1}, date_num, {2} from (", mediaDetailLevel.GetSqlFieldsWithoutTablePrefix(), detailLevel.GetSqlFieldsWithoutTablePrefix(), GetUnitFieldNameSumUnionWithAlias(_session));
            N = "sum(case when ((date_num >= " + _period.Begin.ToString("yyyyMMdd") + " and date_num <= " + _period.End.ToString("yyyyMMdd") + ") or (date_num >= " + _period.Begin.ToString("yyyyMM") + " and date_num <= " + _period.End.ToString("yyyyMM") + ") ) then " + _session.GetSelectedUnit().Id.ToString() + " else 0 end) as N1";
            if (!_session.ComparativeStudy)
                sql.AppendFormat("select {0}, {1}, {2} from (", GetSqlFieldWithAlias(detailLevel, false), GetSqlFieldWithAlias(mediaDetailLevel, true), N);
            else 
            {
                N1 = "sum(case when ((date_num >= " + comparativePeriod.Begin.ToString("yyyyMMdd") + " and date_num <= " + comparativePeriod.End.ToString("yyyyMMdd") + ") or (date_num >= " + comparativePeriod.Begin.ToString("yyyyMM") + " and date_num <= " + comparativePeriod.End.ToString("yyyyMM") + ") ) then " + _session.GetSelectedUnit().Id.ToString() + " else 0 end) as N2";
                sql.AppendFormat("select {0}, {1}, {2}, {3} from (", GetSqlFieldWithAlias(detailLevel, false), GetSqlFieldWithAlias(mediaDetailLevel, true), N, N1);
            }

            while (comparativeIndex < 2)
            {
                if (_session.ComparativeStudy && comparativeIndex == 1)
                    _period = comparativePeriod;

                // SubPeriod Management
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
                                    sql.AppendFormat("({0})", GetQuery(detailLevel, mediaDetailLevel, _period.PeriodDetailLEvel, subPeriods.SubPeriodType, Int64.Parse(listVehicles[i]), subPeriods.Items, string.Empty));
                                }
                                catch (System.Exception err)
                                {
                                    throw new AdvertisingAgencyDALException("AdvertisingAgencyDAL.GetData(WebSession _session, MediaSchedulePeriod _period)", err);
                                }
                            }
                            break;
                        case CstPeriod.PeriodBreakdownType.week:
                        case CstPeriod.PeriodBreakdownType.month:
                            if (!first) sql.Append(" union all ");
                            else first = false;

                            List<Int64> vehicleIdListTmp = new List<long>();
                            for (int i = 0; i < listVehicles.Length; i++)
                            {
                                if (VehiclesInformation.Contains(Int64.Parse(listVehicles[i]))
                                    && (VehiclesInformation.DatabaseIdToEnum(Int64.Parse(listVehicles[i])) == CstDBClassif.Vehicles.names.adnettrack
                                    || VehiclesInformation.DatabaseIdToEnum(Int64.Parse(listVehicles[i])) == CstDBClassif.Vehicles.names.evaliantMobile)
                                    && _session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
                                {
                                    vehicleIdListTmp.Add(Int64.Parse(listVehicles[i]));
                                }
                            }
                            if (vehicleIdListTmp != null && vehicleIdListTmp.Count > 0)
                                sql.AppendFormat("({0})", GetQueryForWebPlanEvaliant(detailLevel, mediaDetailLevel, _period.PeriodDetailLEvel, subPeriods.SubPeriodType, subPeriods.Items, string.Empty));
                            else
                                sql.AppendFormat("({0})", GetQuery(detailLevel, mediaDetailLevel, _period.PeriodDetailLEvel, subPeriods.SubPeriodType, -1, subPeriods.Items, string.Empty));
                            break;
                        default:
                            throw new AdvertisingAgencyDALException("Unable to determine type of subPeriod.");
                    }
                }
                comparativeIndex++;
            }

            sql.Append(") ");
            sql.AppendFormat(" group by {0}, {1} ", detailLevel.GetSqlGroupByFieldsWithoutTablePrefix(), mediaDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix());
            UnitInformation u = _session.GetSelectedUnit();
            if (u.Id == CstWeb.CustomerSessions.Unit.versionNb)
            {
                sql.AppendFormat(", {0} ", u.Id.ToString());
            }
            sql.AppendFormat(" order by {0}, {1} ", detailLevel.GetSqlOrderFieldsWithoutTablePrefix(), mediaDetailLevel.GetSqlOrderFieldsWithoutTablePrefix());
            #endregion

            #region Execution de la requête
            try
            {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new AdvertisingAgencyDALException("Unable to load Advertising Agency Data : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region GetQuery
        /// <summary>
        /// Build query
        /// </summary>
        /// <param name="detailLevel">Detail levels selection</param>
        /// <param name="mediaDetailLevel">Media detail levels selection</param>
        /// <param name="periodDisplay">Period detail display</param>
        /// <param name="periodBreakDown">Type of period (days, weeks, monthes)</param>
        /// <param name="vehicleId">Vehicle filter for desagregated data</param>
        /// <param name="beginningDate">Period Beginning</param>
        /// <param name="endDate">Period End</param>
        /// <param name="additionalConditions">Addtional conditions such as AdNetTrack Baners...</param>
        /// <returns>Sql query as a string</returns>
        protected virtual string GetQuery(GenericDetailLevel detailLevel, GenericDetailLevel mediaDetailLevel, CstPeriod.DisplayLevel periodDisplay, CstPeriod.PeriodBreakdownType periodBreakDown, Int64 vehicleId, List<PeriodItem> periodItems, string additionalConditions)
        {

            #region Variables
            StringBuilder sql = new StringBuilder();
            string list = "";
            string tableName = null;
            string productTableName = null;
            string mediaTableName = null;
            string dateFieldName = null;
            string mediaFieldName = null;
            string unitFieldName = null;
            string unitAlias = null;
            string productJoinCondition = null;
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
                tableName = FctWeb.SQLGenerator.GetDataTableName(periodBreakDown, vehicleId);
                unitFieldName = FctWeb.SQLGenerator.GetUnitFieldName(_session, vehicleId, periodBreakDown);
                // Get the classification table
                productTableName = detailLevel.GetSqlTables(_schAdexpr03.Label);
                // Get the media classification table
                mediaTableName = mediaDetailLevel.GetSqlTables(_schAdexpr03.Label);
                if (productTableName.Length > 0) productTableName += ",";
                // Get unit field
                dateFieldName = FctWeb.SQLGenerator.GetDateFieldName(periodBreakDown);
                unitAlias = FctWeb.SQLGenerator.GetUnitAlias(_session);

                // Get classification fields
                if (VehiclesInformation.Contains(vehicleId) && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.adnettrack
                    || VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.evaliantMobile))
                {
                    mediaFieldName = GetAdnettrackSqlFields(detailLevel);
                }
                else
                {
                    mediaFieldName = detailLevel.GetSqlFields();
                }

                // Get group by clause
                if (VehiclesInformation.Contains(vehicleId) && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.adnettrack
                    || VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.evaliantMobile))
                {
                    groupByFieldName = GetAdnettrackSqlGroupByFields(detailLevel);
                }
                else
                {
                    groupByFieldName = detailLevel.GetSqlGroupByFields();
                }

                // Get joins for classification
                productJoinCondition = detailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                mediaJoinCondition = mediaDetailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            }
            catch (System.Exception err)
            {
                throw (new AdvertisingAgencyDALException("Unable to build the query.", err));
            }

            // Select : Media classificaion selection
            sql.AppendFormat("select {0}, {1} ", mediaDetailLevel.GetSqlFields(), mediaFieldName);

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

            if (VehiclesInformation.Contains(vehicleId)
                && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.adnettrack
                || VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.evaliantMobile)
                && _session.GetSelectedUnit().Id == CstWeb.CustomerSessions.Unit.versionNb)
            {
                switch (periodBreakDown)
                {
                    case CstPeriod.PeriodBreakdownType.data:
                    case CstPeriod.PeriodBreakdownType.data_4m:
                        unitFieldName = string.Format(" to_char({0}.{1}) as {2} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.GetSelectedUnit().DatabaseField, _session.GetSelectedUnit().Id.ToString());
                        groupByOptional = string.Format(", {0}.{1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.GetSelectedUnit().DatabaseField);
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
            sql.AppendFormat(" from {0}{1}, {2} ", productTableName, tableName, mediaTableName);

            // Where : Conditions media
            sql.AppendFormat("where 0=0 {0}", productJoinCondition);
            sql.AppendFormat(" {0} ", mediaJoinCondition);

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

            //Inset option
            if ((vehicleInfo != null && (vehicleInfo.Id == CstDBClassif.Vehicles.names.press
                || vehicleInfo.Id == CstDBClassif.Vehicles.names.internationalPress
                || CstDBClassif.Vehicles.names.newspaper == vehicleInfo.Id
                || CstDBClassif.Vehicles.names.magazine == vehicleInfo.Id
                ))
                || (periodBreakDown != CstWeb.CustomerSessions.Period.PeriodBreakdownType.data && periodBreakDown != CstWeb.CustomerSessions.Period.PeriodBreakdownType.data_4m)
                )
            {
                sql.Append(FctWeb.SQLGenerator.GetJointForInsertDetail(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            }

            // Autopromo Evaliant
            if (VehiclesInformation.Contains(vehicleId) && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.adnettrack || VehiclesInformation.DatabaseIdToEnum(vehicleId) == CstDBClassif.Vehicles.names.evaliantMobile))
            {
                if (_session.AutopromoEvaliant) // Hors autopromo (checkbox = checked)
                    sql.AppendFormat(" and {0}.auto_promotion = 0 ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
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
                sql.AppendFormat(" and {0}.{2} = {1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.SloganIdZoom
                    , (vehicleInfo != null && (vehicleInfo.Id != CstDBClassif.Vehicles.names.adnettrack && vehicleInfo.Id != CstDBClassif.Vehicles.names.evaliantMobile)) ? "id_slogan" : "hashcode");
            }
            else
            {
                // Refine vesions
                if (slogans.Length > 0 && periodDisplay == CstPeriod.DisplayLevel.dayly)
                {
                    sql.AppendFormat(" and {0}.{2} in({1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, slogans
                        , (vehicleInfo != null && (vehicleInfo.Id != CstDBClassif.Vehicles.names.adnettrack && vehicleInfo.Id != CstDBClassif.Vehicles.names.evaliantMobile)) ? "id_slogan" : "hashcode");
                }
            }

            // Selection and right managment

            #region Nomenclature Produit (droits)
            //Access rgithDroits en accès
            sql.Append(FctWeb.SQLGenerator.getAnalyseCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            // Exclude product if radio selected)
            GetExcludeProudcts(sql);
            #endregion

            #region Nomenclature Produit (Niveau de détail)
            // Product level
            sql.Append(FctWeb.SQLGenerator.getLevelProduct(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
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
            sql.Append(FctWeb.SQLGenerator.getAnalyseCustomerMediaRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            #endregion

            #region Selection
            list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess);
            if (periodDisplay == CstPeriod.DisplayLevel.dayly)
            {
                sql.AppendFormat(" and ({0}.id_vehicle={1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, vehicleId);
            }
            else if (list.Length > 0)
            {
                sql.AppendFormat(" and ({0}.id_vehicle in ({1})) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, list);
            }
            #endregion

            //Universe media
            sql.Append(GetMediaUniverse(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

            #endregion

            // Order
            sql.AppendFormat("Group by {0} ,{1} {2} ", mediaDetailLevel.GetSqlGroupByFields(), groupByFieldName, groupByOptional);
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
        /// <param name="mediaDetailLevel">Media detail levels selection</param>
        /// <param name="periodDisplay">Period detail display</param>
        /// <param name="periodBreakDown">Type of period (days, weeks, monthes)</param>
        /// <param name="vehicleId">Vehicle filter for desagregated data</param>
        /// <param name="beginningDate">Period Beginning</param>
        /// <param name="endDate">Period End</param>
        /// <param name="additionalConditions">Addtional conditions such as AdNetTrack Baners...</param>
        /// <returns>Sql query as a string</returns>
        protected virtual string GetQueryForWebPlanEvaliant(GenericDetailLevel detailLevel, GenericDetailLevel mediaDetailLevel, CstPeriod.DisplayLevel periodDisplay, CstPeriod.PeriodBreakdownType periodBreakDown, List<PeriodItem> periodItems, string additionalConditions)
        {

            #region Variables
            StringBuilder sql = new StringBuilder();
            string list = "";
            string tableName = null;
            string productTableName = null;
            string mediaTableName = null;
            string dateFieldName = null;
            string mediaFieldName = null;
            string mediaPeriodicity = null;
            string unitFieldName = null;
            string unitAlias = null;
            string productJoinCondition = null;
            string mediaJoinCondition = null;
            string groupByFieldName = null;
            string groupByOptional = null;
            #endregion

            #region Construction de la requête
            try
            {
                // Get the name of the data table	
                switch (periodBreakDown)
                {
                    case CstPeriod.PeriodBreakdownType.month:
                        tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData).SqlWithPrefix;
                        break;
                    case CstPeriod.PeriodBreakdownType.week:
                        tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.weekData).SqlWithPrefix;
                        break;
                    default:
                        throw (new AdvertisingAgencyDALException("Unable to determine table to use."));
                }
                //Get unit field
                unitFieldName = _session.GetSelectedUnit().DatabaseMultimediaField;

                // Get the classification table
                productTableName = detailLevel.GetSqlTables(_schAdexpr03.Label);
                // Get the media classification table
                mediaTableName = mediaDetailLevel.GetSqlTables(_schAdexpr03.Label);
                if (productTableName.Length > 0) productTableName += ",";
                // Get unit field
                dateFieldName = FctWeb.SQLGenerator.GetDateFieldName(periodBreakDown);
                unitAlias = FctWeb.SQLGenerator.GetUnitAlias(_session);

                // Get classification fields				
                mediaFieldName = GetAdnettrackSqlFields(detailLevel);

                // Get group by clause				
                groupByFieldName = GetAdnettrackSqlGroupByFields(detailLevel);

                // Get joins for classification
                productJoinCondition = detailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                mediaJoinCondition = mediaDetailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            }
            catch (System.Exception err)
            {
                throw (new AdvertisingAgencyDALException("Unable to build the query.", err));
            }

            // Select : Media classificaion selection
            sql.AppendFormat("select {0}, {1} ", mediaDetailLevel.GetSqlFields(), mediaFieldName);

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

            // Periodicity selection
            sql.AppendFormat("{0}, ", mediaPeriodicity);
            switch (periodBreakDown)
            {
                case CstPeriod.PeriodBreakdownType.data:
                case CstPeriod.PeriodBreakdownType.data_4m:
                    unitFieldName = string.Format(" to_char({0}.{1}) as {2} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.GetSelectedUnit().DatabaseField, _session.GetSelectedUnit().Id.ToString());
                    groupByOptional = string.Format(", {0}.{1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.GetSelectedUnit().DatabaseField);
                    break;
                default:
                    unitFieldName = string.Format(" {0}.{1} as {2} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.GetSelectedUnit().DatabaseMultimediaField, _session.GetSelectedUnit().Id.ToString());
                    groupByOptional = string.Format(", {0}.{1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _session.GetSelectedUnit().DatabaseMultimediaField);
                    break;
            }
            sql.AppendFormat("{0}", unitFieldName);

            // From : Tables
            sql.AppendFormat(" from {0}{1}, {2} ", productTableName, tableName, mediaTableName);

            // Where : Conditions media
            sql.AppendFormat("where 0=0 {0}", productJoinCondition);
            sql.AppendFormat(" {0} ", mediaJoinCondition);

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

            // Autopromo Evaliant			
            if (_session.AutopromoEvaliant) // Hors autopromo (checkbox = checked)
                sql.AppendFormat(" and {0}.auto_promotion = 0 ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

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
            sql.Append(FctWeb.SQLGenerator.getAnalyseCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
            // Exclude product if radio selected)
            GetExcludeProudcts(sql);
            #endregion

            #region Nomenclature Produit (Niveau de détail)
            // Product level
            sql.Append(FctWeb.SQLGenerator.getLevelProduct(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
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
            sql.Append(FctWeb.SQLGenerator.getAnalyseCustomerMediaRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
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

            // Order
            sql.AppendFormat("Group by {0} ,{1} {2} ", mediaDetailLevel.GetSqlGroupByFields(), groupByFieldName, groupByOptional);
            // And date
            sql.AppendFormat(", {0} ", dateFieldName);
            #endregion

            return (sql.ToString());
        }
        #endregion

        #region Queries building stuff

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

        #region GetUnitFieldNameSumUnionWithAlias
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
                throw new AdvertisingAgencyDALException("Not managed unit (Alert Module)");
            }
        }
        #endregion

        #region GetAdnettrackSqlFields
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
                    if (currentLevel.DataBaseTableNamePrefix != null && currentLevel.DataBaseTableNamePrefix.Length > 0)
                        prefix = currentLevel.DataBaseTableNamePrefix + ".";
                    sql += "nvl(" + prefix + "hashcode,0) as " + currentLevel.DataBaseAliasIdField + ",nvl(" + prefix + "hashcode,0) as " + currentLevel.DataBaseAliasField + ",";
                }
            }
            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);
            return (sql);
        }
        #endregion

        #region GetAdnettrackSqlGroupByFields
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

        #region GetExcludeProudcts
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
        #endregion

        #region GetMediaUniverse
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
        #endregion

        #region SetGenericMediaDetailLevel
        /// <summary>
        /// Set Generic Media Detail Level
        /// </summary>
        /// <returns>Generic Detail Media Level</returns>
        protected virtual GenericDetailLevel SetGenericMediaDetailLevel()
        {

            ArrayList levelsIds = new ArrayList();

            switch (_session.PreformatedMediaDetail)
            {
                case CstFormat.PreformatedMediaDetails.vehicle:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.vehicle);
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleCategory:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.vehicle);
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.category);
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleCategoryMedia:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.vehicle);
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.category);
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.media);
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleMedia:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.vehicle);
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.media);
                    break;
                case CstFormat.PreformatedMediaDetails.vehicleMediaSeller:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.vehicle);
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.mediaSeller);
                    break;
                case CstFormat.PreformatedMediaDetails.category:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.category);
                    break;
                case CstFormat.PreformatedMediaDetails.mediaSeller:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.mediaSeller);
                    break;
                case CstFormat.PreformatedMediaDetails.mediaSellerVehicle:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.mediaSeller);
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.vehicle);
                    break;
                case CstFormat.PreformatedMediaDetails.mediaSellerMedia:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.mediaSeller);
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.media);
                    break;
                case CstFormat.PreformatedMediaDetails.mediaSellerCategory:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.mediaSeller);
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.category);
                    break;
                case CstFormat.PreformatedMediaDetails.Media:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.media);
                    break;
                default:
                    levelsIds.Add((int)DetailLevelItemInformation.Levels.vehicle);
                    break;
            }

            return new GenericDetailLevel(levelsIds, CstWeb.GenericDetailLevel.SelectedFrom.unknown);
        }
        #endregion

        #region GetSqlFieldWithAlias
        /// <summary>
        /// Add Alias To database fields
        /// </summary>
        /// <param name="detailLevel">Detail level list</param>
        /// <param name="media">Media or Product detail levels</param>
        /// <returns>Sql string</returns>
        private string GetSqlFieldWithAlias(GenericDetailLevel detailLevel, bool media)
        {
            string sql = string.Empty;
            string alias = (media) ? "m" : "p";
            int i = 1;

            foreach (DetailLevelItemInformation currentLevel in detailLevel.Levels)
            {
                sql += currentLevel.GetSqlFieldIdWithoutTablePrefix() + " as id_" + alias + i + " ," + currentLevel.GetSqlFieldWithoutTablePrefix() + " as " + alias + i + ",";
                i++;
            }

            if (sql.Length > 0) sql = sql.Substring(0, sql.Length - 1);

            return sql;
        }
        #endregion

        #endregion

    }
}
