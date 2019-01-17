using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Insertions.DAL.Exceptions;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using Table = TNS.AdExpress.Domain.DataBaseDescription.Table;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Insertions.DAL.Turkey
{
    public class InsertionsDAL : TNS.AdExpressI.Insertions.DAL.InsertionsDAL
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Current Module id</param>
        public InsertionsDAL(WebSession session, Int64 moduleId)
            : base(session, moduleId)
        {
        }
        #endregion

        #region AppendInsertionsSqlFields
        /// <summary>
        /// Append data fields
        /// The list of data fields used in the SQL query will be generated as following :
        ///     + Three detail Levels : Level1_Id, Level1_Label, Level2_Id, Level2_Label, Level3_Id, Level3_Label. 
        ///       We can get this levels from _session.DetailLevel.Levels.
        ///     + A list of detail columns that we have initialize above according to column configuration files, we have two differents type of columns :
        ///         - The first one corresponds to a classification level, like Advertiser for example, in this case we need : Level_id and Level_label
        ///         - The second one corresponds to a feature of the insertion or the version, like page or format for example, in this case we need : column_value (page = 119, format = 1 PAGE)
        /// </summary>
        /// <param name="tData">Data base table description</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="columns">List of detail columns</param>
        /// <returns>True if there is detail levels selected</returns>
        protected override bool AppendInsertionsSqlFields(Table tData, VehicleInformation vehicle,
            StringBuilder sql, List<GenericColumnItemInformation> columns)
        {

            string tmp = string.Empty;
            bool detailLevelSelected = false;

            //Insertions fields
            /* Get the SQL code of the fields corresponding to columns (from the XML configuration files) except the ones that mutched with
             * the detail level list
             * */

            tmp = GenericColumns.GetSqlFields(columns, null);           
            if (tmp.Length > 0)
            {
                sql.AppendFormat(" {0},", tmp);
            }

            //Constraints fields
            /* For some columns, we not only need information about the column but we need information about others fileds related to the column
             * example : 
             * <column id="35" name="Visuel" webTextId="1909"  dbLabel="visual">
             *  <constraints>
             *      <dbConstraints>
             *          <dbFieldConstraints>
             *              <dbFieldConstraint id="1" name="Visuel Disponibility" dbField="disponibility_visual" dbTablePrefixe="appliMd"/>
             *              <dbFieldConstraint id="2" name="Visuel Activation" dbField="activation" dbTablePrefixe="appliMd"/>
             *          </dbFieldConstraints>
             *      </dbConstraints>
             *  </constraints>
             * </column>
             * so we define in the Xml configuration file all the fields related to one column (like above), 
             * and we can get this fileds by using the method GenericColumns.GetSqlConstraintFields(columns)
             * */
            tmp = GenericColumns.GetSqlConstraintFields(columns);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(" {0},", tmp);//Rules constraints management
            }

            return detailLevelSelected;
        }
        /// <summary>
        /// Append data fields
        /// The list of data fields used in the SQL query will be generated as following :
        ///     + Three detail Levels : Level1_Id, Level1_Label, Level2_Id, Level2_Label, Level3_Id, Level3_Label. 
        ///       We can get this levels from _session.DetailLevel.Levels.
        ///     + A list of detail columns that we have initialize above according to column configuration files, we have two differents type of columns :
        ///         - The first one corresponds to a classification level, like Advertiser for example, in this case we need : Level_id and Level_label
        ///         - The second one corresponds to a feature of the insertion or the version, like page or format for example, in this case we need : column_value (page = 119, format = 1 PAGE)
        /// </summary>
        /// <param name="tData">Data base table description</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="detailLevelsIds">List of level details Identifiers</param>
        /// <param name="columns">List of detail columns</param>
        /// <returns>True if there is detail levels selected</returns>
        protected override bool AppendInsertionsSqlFields(Table tData, VehicleInformation vehicle,
            StringBuilder sql, ArrayList detailLevelsIds, List<GenericColumnItemInformation> columns)
        {

            bool detailLevelSelected = false;
            bool hasCategory = false;
            string tmp = string.Empty;

            //Detail levels
            /* The list of detail levels Identifiers is initialized according to levels saved in the _session.DetailLevel object.
             * to get the fields corresponding to the levels we use _session.DetailLevel.GetSqlFields().
             * */
            if (!_msCreaConfig && _session.DetailLevel != null &&
                _session.DetailLevel.Levels != null && _session.DetailLevel.Levels.Count > 0)
            {
                foreach (DetailLevelItemInformation detailLevelItemInformation in _session.DetailLevel.Levels)
                {
                    if (detailLevelItemInformation.Id == DetailLevelItemInformation.Levels.category)
                    {
                        hasCategory = true;
                    }
                    detailLevelsIds.Add(detailLevelItemInformation.Id.GetHashCode());
                }
                tmp = _session.DetailLevel.GetSqlFields();
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" {0}", tmp);
                    detailLevelSelected = true;
                }
            }

            //Insertions fields
            /* Get the SQL code of the fields corresponding to columns (from the XML configuration files) except the ones that mutched with
             * the detail level list
             * */

            tmp = GenericColumns.GetSqlFields(columns, detailLevelsIds);
           
            if (tmp.Length > 0)
            {
                if (detailLevelSelected) sql.Append(",");
                sql.AppendFormat(" {0},", tmp);
            }

            //Constraints fields
            /* For some columns, we not only need information about the column but we need information about others fileds related to the column
             * example : 
             * <column id="35" name="Visuel" webTextId="1909"  dbLabel="visual">
             *  <constraints>
             *      <dbConstraints>
             *          <dbFieldConstraints>
             *              <dbFieldConstraint id="1" name="Visuel Disponibility" dbField="disponibility_visual" dbTablePrefixe="appliMd"/>
             *              <dbFieldConstraint id="2" name="Visuel Activation" dbField="activation" dbTablePrefixe="appliMd"/>
             *          </dbFieldConstraints>
             *      </dbConstraints>
             *  </constraints>
             * </column>
             * so we define in the Xml configuration file all the fields related to one column (like above), 
             * and we can get this fileds by using the method GenericColumns.GetSqlConstraintFields(columns)
             * */
            tmp = GenericColumns.GetSqlConstraintFields(columns);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(" {0},", tmp);//Rules constraints management
            }

            // Slogan fields
            // Add slogan field if required (only for radio because it is required to build path
            if (!_msCreaConfig)
                AppendSloganField(sql, tData, vehicle, columns);

           

            return detailLevelSelected;
        }
        #endregion

        /// <summary>
        /// Append Filters depending on client univers selection and rights
        /// </summary>
        /// <param name="sql">Sql request container</param>
        /// <param name="tData">Data Table Description</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="vehicle">Vehicle information</param>
        /// <param name="universId">Current Univers</param>
        /// <param name="filters">FIlters to apply</param>
        protected override void AppendUniversFilters(StringBuilder sql, Table tData,
            int fromDate, int toDate, VehicleInformation vehicle, int universId, string filters)
        {

            #region Period
            /* Determine the period of the study
             * PeriodType correspond to the type of date selection, for example :
             *  dateToDate : means that the date format is YYYYMMDD
             *  dateToDateMonth : means that the date format is YYYYMM
             *  dateToDateWeek : means that the date format is YYYYWW
             * depending on the date format we can create the correct SQL query filter
             * */
            if (_session.PeriodType == AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate)
            {

                int begin = Convert.ToInt32(_session.PeriodBeginningDate);
                if (begin > fromDate)
                {
                    sql.AppendFormat(" {1}.date_media_num >= {0}", begin, tData.Prefix);
                    if (_module.Id == AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation >= to_date('{1} 00:00:00','yyyymmdd HH24:MI:SS') ", tData.Prefix, begin);
                    }
                }
                else
                {
                    sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
                    if (_module.Id == AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation >= to_date('{1} 00:00:00','yyyymmdd HH24:MI:SS') ", tData.Prefix, fromDate);
                    }
                }
                int end = Convert.ToInt32(_session.PeriodEndDate);
                if (end < toDate)
                {
                    sql.AppendFormat(" and {1}.date_media_num <= {0}", end, tData.Prefix);
                    if (_module.Id == AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation <= to_date('{1} 23:59:59','yyyymmdd HH24:MI:SS') ", tData.Prefix, end);
                    }
                }
                else
                {
                    sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);
                    if (_module.Id == AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation <= to_date('{1} 23:59:59','yyyymmdd HH24:MI:SS') ", tData.Prefix, toDate);
                    }
                }
            }
            else
            {
                sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
                sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);
                if (_module.Id == AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES)
                {
                    sql.AppendFormat(" and {0}.date_creation >= to_date('{1} 00:00:00','yyyymmdd HH24:MI:SS') ", tData.Prefix, fromDate);
                    sql.AppendFormat(" and {0}.date_creation <= to_date('{1} 23:59:59','yyyymmdd HH24:MI:SS') ", tData.Prefix, toDate);
                }
            }
            #endregion

            #region Advertiser modules
            /* Versions filter
             * If the customer has filtred the number of the original slogan, we can find this list in the SloganIdList variable.
             * in this case we add an id_slogan filter, like below
             * */
            if (_module.Id == AdExpress.Constantes.Web.Module.Name.ALERTE_PLAN_MEDIA && !string.IsNullOrEmpty(_session.SloganIdList))
            {
                sql.AppendFormat(" and {1}.id_slogan in ( {0} ) ", _session.SloganIdList, tData.Prefix);
            }
            // Product classification selection
            /* We can get the product classification selection by using _session.PrincipalProductUniverses[universId].GetSqlConditions(tData.Prefix, true)
             * */
            if (_module.Id != AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES &&
                _session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
            {
                if (universId < 0)
                {
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(tData.Prefix, true));
                }
                else
                {
                    sql.Append(_session.PrincipalProductUniverses[universId].GetSqlConditions(tData.Prefix, true));
                }
            }
            #endregion

            #region Media modules
            /* Get the list of medias selected
             * There are several treenodes to save the univers media, so according to the module we can get the media list
             * */
            string listMediaAccess = string.Empty;
            if (_module.Id == AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE || _module.Id == AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE)
            {
                Dictionary<AdExpress.Constantes.Customer.Right.type, string> selectedVehicles = _session.CustomerDataFilters.SelectedVehicles;
                if (selectedVehicles != null && selectedVehicles.ContainsKey(AdExpress.Constantes.Customer.Right.type.mediaAccess))
                {
                    listMediaAccess = selectedVehicles[AdExpress.Constantes.Customer.Right.type.mediaAccess];
                }
            }
            if (_module.Id == AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
            {
                string list = _session.GetSelection(_session.SelectionUniversMedia, AdExpress.Constantes.Customer.Right.type.vehicleAccess);

                //Switch from media Internet to media Internet Evaliant
                if (list.Length > 0 && vehicle.Id == Vehicles.names.internet)
                    list = list.Replace(vehicle.DatabaseId.ToString(),
                    VehiclesInformation.Get(Vehicles.names.adnettrack).DatabaseId.ToString());

                //Switch from media Courrier Valo  to media Courrier Crea
                if (list.Length > 0 && vehicle.Id == Vehicles.names.mailValo)
                    list = list.Replace(vehicle.DatabaseId.ToString(),
                    VehiclesInformation.Get(Vehicles.names.directMarketing).DatabaseId.ToString());

                if (list.Length > 0) sql.AppendFormat(" and ({0}.id_vehicle in ({1})) ", tData.Prefix, list);
            }
            if (listMediaAccess.Length > 0)
            {
                sql.AppendFormat(" and (({1}.id_media in ({0}))) "
                    , listMediaAccess.Substring(0, listMediaAccess.Length), tData.Prefix);
            }

            if (_session.CurrentModule == AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA && (_session.PrincipalMediaUniverses != null && _session.PrincipalMediaUniverses.Count > 0))
            {
                sql.Append((vehicle.Id == Vehicles.names.mailValo)
                               ? _session.PrincipalMediaUniverses[0].GetSqlConditions(tData.Prefix, true, ReplaceMailValoByVmc)
                               : _session.PrincipalMediaUniverses[0].GetSqlConditions(tData.Prefix, true));

            }
            #endregion

            #region Rights
            /* Get product classification rights
             * */
            if (_module == null) throw (new InsertionsDALException("_module parameter cannot be NULL"));
            sql.Append(SQLGenerator.GetClassificationCustomerProductRight(_session,
                tData.Prefix, true, _module.ProductRightBranches));


            /* Get media classification rights
             * */
          
                sql.Append(vehicle.Id == Vehicles.names.internet
                               ? SQLGenerator.GetAdNetTrackMediaRight(_session, tData.Prefix, true)
                               : SQLGenerator.getAnalyseCustomerMediaRight(_session, tData.Prefix, true));

           



            #endregion

            #region Sponsorship univers
            /* This test is specific to the sponsoring modules
             * */
            if (_module.Id == AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES
                || _module.Id == AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS)
            {
                string tmp = string.Empty;
                if (_session.CurrentUniversMedia != null && _session.CurrentUniversMedia.Nodes.Count > 0)
                {
                    tmp = _session.GetSelection(_session.CurrentUniversMedia, AdExpress.Constantes.Customer.Right.type.mediaAccess);
                }
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" and  {1}.id_media in ({0}) ", tmp, tData.Prefix);
                }
                /* Program classification
                 * */
                sql.Append(SQLGenerator.GetCustomerProgramSelection(_session, tData.Prefix, tData.Prefix, true));
                /* Type of sponsorship selection
                 * */
                sql.Append(SQLGenerator.GetCustomerSponsorshipFormSelection(_session, tData.Prefix, true));
            }
            #endregion

            #region Global rules
            /* Get filters of the product classification levels
             * */
            switch (_module.Id)
            {
                case AdExpress.Constantes.Web.Module.Name.ALERTE_PLAN_MEDIA:
                case AdExpress.Constantes.Web.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                case AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA:
                case AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                case AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE:
                case AdExpress.Constantes.Web.Module.Name.ANALYSE_POTENTIELS:
                    sql.Append(SQLGenerator.getLevelProduct(_session, tData.Prefix, true));
                    break;
            }

            /* Get radio rules
             * */
          
                sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, tData.Prefix, true, false));
           
            /* This test is specific to the French version of the site
             * */
            //string idCategoryThematic = TNS.AdExpress.Domain.Lists.GetIdList(CstWeb.GroupList.ID.category, CstWeb.GroupList.Type.thematicTv);
            //if (!string.IsNullOrEmpty(idCategoryThematic))
            //{
            //    sql.AppendFormat("  and  {0}.id_category not in ( {1}) ", tData.Prefix, idCategoryThematic);
            //}
            #endregion

         

            #region Filtres
            /* Get filter clause and check zero version
             * According to the module type, we save the classification levels in GenericProductDetailLevel or in GenericMediaDetailLevel
             * GenericProductDetailLevel : product classification levels
             * GenericMediaDetailLevel : media classification levels
             * */
            if (!string.IsNullOrEmpty(filters))
            {
                GenericDetailLevel detailLevels = null;
                switch (_module.Id)
                {
                    case AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE:
                    case AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES:
                    case AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                        detailLevels = _session.GenericProductDetailLevel;
                        break;
                    case AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA:
                    case AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE:
                    case AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    case AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES:
                        detailLevels = _session.GenericMediaDetailLevel;
                        break;
                }
                sql.Append(GetFiltersClause(tData, detailLevels, filters, vehicle));
                sql.AppendFormat(CheckZeroVersion(tData, detailLevels, vehicle, filters));
            }
            if (_session.SloganIdZoom > -1)
            {//For Russia : _session.SloganIdZoom > long.MinValue (correspond to the absence of ID for the version)
                sql.AppendFormat(" and wp.id_slogan={0}", _session.SloganIdZoom);
            }
            if ((_msCreaConfig || _creaConfig) && vehicle.Id != Vehicles.names.adnettrack &&
                vehicle.Id != Vehicles.names.internet && vehicle.Id != Vehicles.names.evaliantMobile)
            {
                sql.AppendFormat(" and {0}.id_slogan is not null", tData.Prefix);
            }

            /* Version selection
             * */
            string slogans = _session.SloganIdList;

            /* Refine vesions
             * */
            if (slogans.Length > 0)
            {
                sql.AppendFormat(" and {0}.id_slogan in({1}) "
                    , WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, slogans);
            }
            #endregion

           

            switch (vehicle.Id)
            {
                case Vehicles.names.internet:
                    sql.AppendFormat(" and {1}.id_vehicle={0} "
                        , VehiclesInformation.Get(Vehicles.names.adnettrack).DatabaseId, tData.Prefix);
                    break;
                case Vehicles.names.mailValo:
                    sql.AppendFormat(" and {1}.id_vehicle={0} "
                        , VehiclesInformation.Get(Vehicles.names.directMarketing).DatabaseId, tData.Prefix);
                    break;
                default:
                    sql.AppendFormat(" and {1}.id_vehicle={0} ", vehicle.DatabaseId, tData.Prefix);
                    break;
            }


        }

        protected override Table GetDataTable(VehicleInformation vehicle, int fromDate)
        {
            /* Get the table name for a specific vehicle and depending on the module type
                         * Example : data_press, data_tv, data_radio ...
                         * */
            Table tData;
           
             if (Dates.Is4M(fromDate))
             {
                 if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                     tData = WebApplicationParameters.GetDataTable(TableIds.dataSpotTvAlert,
                         _session.IsSelectRetailerDisplay);
                 else tData = GetDataTable(vehicle, CstWeb.Module.Type.alert);

             }
            else
            {
                if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    tData = WebApplicationParameters.GetDataTable(TableIds.dataSpotTv,
                        _session.IsSelectRetailerDisplay);
                else tData = GetDataTable(vehicle, CstWeb.Module.Type.analysis);

            }
            return tData;
        }

        protected override string GetTableName(string beginingDate, string tableName, VehicleInformation vehicleInformation)
        {

            if (Dates.Is4M(beginingDate))
            {
                if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataSpotTvAlert,
                        _session.IsSelectRetailerDisplay).Label;
                else tableName = GetTableName(vehicleInformation, CstWeb.Module.Type.alert);
            }

            else
            {
                if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataSpotTv,
                        _session.IsSelectRetailerDisplay).Label;
                else
                tableName = GetTableName(vehicleInformation, CstWeb.Module.Type.analysis);
            }
               
            return tableName;
        }

    }
}
