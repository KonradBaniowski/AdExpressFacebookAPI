using System;
using System.Collections.Generic;
using System.Text;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;

using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpressI.Insertions.DAL.Exceptions;
using System.Data;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web.Navigation;
using System.Windows.Forms;
using System.Collections;


namespace TNS.AdExpressI.Insertions.DAL
{
    public abstract class InsertionsDAL:IInsertionsDAL
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module;
        /// <summary>
        /// Creatives config
        /// </summary>
        protected bool _creaConfig = false;
        /// <summary>
        /// Media schedule creatives config
        /// </summary>
        protected bool _msCreaConfig = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="module">Current Module</param>
        public InsertionsDAL(WebSession session, Int64 moduleId){
            _session = session;
            _module = ModulesList.GetModule(moduleId);
        }
        #endregion

        #region GetVehiclesIds
        public virtual Int64[] GetVehiclesIds(Dictionary<DetailLevelItemInformation, Int64> filters)
        {
            Int64[] ids = null;
            StringBuilder sql = new StringBuilder();
            Table tVehicle = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);
            TNS.AdExpress.Domain.DataBaseDescription.View vMedia = WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia);

            sql.AppendFormat("select distinct {0}.id_vehicle from {1}{2} {0} where 1=1 ", vMedia.Prefix, vMedia.Sql, _session.DataLanguage);
            foreach (DetailLevelItemInformation d in filters.Keys)
            {
                if (filters[d] >= 0)
                {
                    switch (d.Id)
                    {
                        case DetailLevelItemInformation.Levels.vehicle:
                        case DetailLevelItemInformation.Levels.category:
                        case DetailLevelItemInformation.Levels.media:
                        case DetailLevelItemInformation.Levels.interestCenter:
                        case DetailLevelItemInformation.Levels.title:
                            sql.AppendFormat(" and {0}.{1} = {2}", vMedia.Prefix, d.DataBaseIdField, filters[d]);
                            break;
                        default:
                            break;

                    }
                }
            }

            try
            {
                DataSet ds = _session.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null)
                {
                    DataTable dt = ds.Tables[0];
                    ids = new Int64[dt.Rows.Count];
                    for(int i=0; i < dt.Rows.Count; i++)
                    {
                        ids[i] = Convert.ToInt64(dt.Rows[i][0]);
                    }
                }
            }
            catch (System.Exception err)
            {
                throw (new InsertionsDALException("Unable to get list of vehicles ids : " + sql, err));
            }

            return ids;

        }
        #endregion

        #region GetPresentVehicles
        /// <summary>
        /// Return list of vehicle referenced in the user univers
        /// </summary>
        /// <param name="module">Current Module</param>
        /// <param name="filters">User filters</param>
        /// <param name="fromDate">User Period beginning</param>
        /// <param name="toDate">User Period End</param>
        /// <param name="universId">User Univers Selection</param>
        /// <param name="moduleId">User Current Module</param>
        /// <param name="vehicles">Vehicles to check</param>
        /// <returns>List of vehicles present</returns>
        public virtual List<VehicleInformation> GetPresentVehicles(List<VehicleInformation> vehicles, string filters, int fromDate, int toDate, int universId, TNS.AdExpress.Domain.Web.Navigation.Module module, bool sloganNotNull)
        {
            StringBuilder sql = new StringBuilder();
            List<VehicleInformation> found = new List<VehicleInformation>();
            DataSet ds = null;
            this._creaConfig = sloganNotNull;

            try
            {

                bool first = true;

                universId--;
                Table dataTable;
                Schema sAdEx = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
                foreach (VehicleInformation v in vehicles)
                {
                    dataTable = GetDataTable(v, module.ModuleType);

                    if (module.Id == CstWeb.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE
                        || module.Id == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE)
                    {
                        int posUnivers = 0;
                        while (_session.PrincipalProductUniverses.ContainsKey(posUnivers) && _session.PrincipalProductUniverses[posUnivers] != null)
                        {

                            if (universId < 0 || universId == posUnivers)
                            {

                                if (!first)
                                    sql.Append(" UNION ");
                                else
                                    first = false;
                                if (v.Id != CstDBClassif.Vehicles.names.internet)
                                {
                                    sql.Append(" select id_vehicle from ");
                                }
                                else
                                {
                                    sql.AppendFormat(" select {0} as id_vehicle from ", v.DatabaseId);
                                }
                                sql.AppendFormat(" {0} ", dataTable.SqlWithPrefix);
                                sql.Append(" where ");
                                AppendUniversFilters(sql, dataTable, fromDate, toDate, v, universId, filters);
                                sql.AppendFormat(" and rownum < 2 ");
                            }
                            posUnivers++;

                        }
                    }
                    else
                    {
                        if (!first)
                            sql.Append(" UNION ");
                        else
                            first = false;
                        sql.Append(" select id_vehicle from ");
                        sql.AppendFormat(" {0} ", dataTable.SqlWithPrefix);
                        sql.Append(" where ");
                        AppendUniversFilters(sql, dataTable, fromDate, toDate, v, universId, filters);
                        sql.AppendFormat(" and rownum < 2 ");
                    }

                }

                ds = _session.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        found.Add(VehiclesInformation.Get(Convert.ToInt64(row["id_vehicle"])));
                    }

                }

            }
            catch (System.Exception err)
            {
                throw new InsertionsDALException(string.Format("GetData::Unable to check there is data. {0}", sql.ToString()), err);
            }

            return found;
        }
        #endregion

        #region GetDataTable
        /// <summary>
        /// Get Table to use when the study is about internet creatives
        /// </summary>
        /// <param name="moduleType">Type of module</param>
        /// <returns>Table Description</returns>
        protected Table GetDataTable(VehicleInformation vehicle, CstWeb.Module.Type moduleType)
        {
            if (vehicle.Id != CstDBClassif.Vehicles.names.internet)
            {
                return SQLGenerator.GetDataTable(vehicle, _module.ModuleType);
            }
            else
            {

                switch (moduleType)
                {
                    case CstWeb.Module.Type.alert:
                        return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataInternetVersionAlert);
                    case CstWeb.Module.Type.analysis:
                        return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataInternetVersion);
                    default:
                        throw new ArgumentException("Type of module is not supported");
                }
            }
        }
        #endregion

        #region GetFilters
        /// <summary>
        /// Generate filters clause
        /// </summary>
        /// <param name="table">Data Table</param>
        /// <param name="detailLevels">Detail Levels Selected</param>
        /// <param name="filters">Filters Ids</param>
        /// <param name="vehicle">Vehicle</param>
        /// <returns>Filters clause</returns>
        protected virtual string GetFiltersClause(Table table, TNS.AdExpress.Domain.Level.GenericDetailLevel detailLevels, string filters, VehicleInformation vehicle)
        {

            StringBuilder str = new StringBuilder();
            string[] ids = filters.Split(',');

            DetailLevelItemInformation level = null;
            int id = 0;
            for (int i = 0; i < ids.Length && i < detailLevels.Levels.Count; i++)
            {
                id = Convert.ToInt32(ids[i]);
                level = (DetailLevelItemInformation)detailLevels.Levels[i];
                if (id > 0)
                {
                    if (level.DataBaseIdField == CstDB.Fields.ID_VEHICLE && id == VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.internet))
                        id = CstDBClassif.Vehicles.names.adnettrack.GetHashCode();
                    str.AppendFormat(" and {2}.{0} = {1}", level.DataBaseIdField, id, table.Prefix);
                }
                if (id == 0 && level.Id == DetailLevelItemInformation.Levels.slogan && vehicle.Id != CstDBClassif.Vehicles.names.adnettrack)
                {
                    str.AppendFormat(" and {2}.{0} = {1}", level.DataBaseIdField, id, table.Prefix);
                }

            }

            return str.ToString();
        }
        #endregion

        #region versions 0
        /// <summary>
        /// Test if version id is 0
        /// </summary>
        /// <param name="table">Data Table</param>
        /// <param name="filters">Filters</param>
        /// <param name="detailLevels">Detail Levels Selected<</param>
        /// <param name="vehicle">Vehicle</param>
        /// <returns>SQL</returns>
        private static string CheckZeroVersion(Table table, TNS.AdExpress.Domain.Level.GenericDetailLevel detailLevels, VehicleInformation vehicle, string filters)
        {
            int id = 0;
            string[] ids = filters.Split(',');
            int rank = detailLevels.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan);
            if (rank != 0)
            {
                id = Convert.ToInt32(ids[rank - 1]);
                if (id == 0 && vehicle.Id != CstDBClassif.Vehicles.names.adnettrack) 
                    return string.Format(" and {0}.id_slogan is null ", table.Prefix);
            }
            return ("");
        }
        #endregion

        #region Insertions
        /// <summary>
        /// Extract advertising detail for insertions details 
        /// </summary>
        /// <param name="dateBegin">Beginning of the period</param>
        /// <param name="dateEnd">End of the period</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <returns>Advertising detail Data</returns>		
        public DataSet GetInsertionsData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)
        {
            _creaConfig = false;
            return GetData(vehicle, fromDate, toDate, universId, filters);
        }
        #endregion 

        #region Creatives
        /// <summary>
        /// Extract advertising detail for creatives details 
        /// </summary>
        /// <param name="dateBegin">Beginning of the period</param>
        /// <param name="dateEnd">End of the period</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <returns>Advertising detail Data</returns>		
        public DataSet GetCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)
        {
            _creaConfig = true;
            return GetData(vehicle, fromDate, toDate, universId, filters);
        }
        #endregion

        #region MS Creatives
        /// <summary>
        /// Extract advertising detail for media schedule creatives details 
        /// </summary>
        /// <param name="dateBegin">Beginning of the period</param>
        /// <param name="dateEnd">End of the period</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <returns>Advertising detail Data</returns>		
        public DataSet GetMSCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters) {
            _msCreaConfig = true;
            return GetData(vehicle, fromDate, toDate, universId, filters);
        }
        #endregion

        #region GetData
        /// <summary>
        /// Extract advertising detail for creatives or insertions details 
        /// </summary>
        /// <param name="dateBegin">Beginning of the period</param>
        /// <param name="dateEnd">End of the period</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="insertionDetail">Insertion detail if true, creatives if false</param>
        /// <returns>Advertising detail Data</returns>		
        protected DataSet GetData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)
        {

            StringBuilder sql = new StringBuilder(5000);
            ArrayList detailLevelsIds = new ArrayList();
            Table tData = GetDataTable(vehicle, _module.ModuleType);
            Schema sAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
            string tmp = string.Empty;
            List<GenericColumnItemInformation> columns = null;
            if(_msCreaConfig)
                columns = WebApplicationParameters.MsCreativesDetail.GetDetailColumns(vehicle.DatabaseId);
            else if(_creaConfig)
                columns = WebApplicationParameters.CreativesDetail.GetDetailColumns(vehicle.DatabaseId, _module.Id);
            else
                columns = WebApplicationParameters.InsertionsDetail.GetDetailColumns(vehicle.DatabaseId, _module.Id);
            try
            {

                //Select
                sql.Append(" select distinct ");
                AppendInsertionsSqlFields(tData, vehicle, sql, detailLevelsIds, columns);
                sql.Length -= 1;

                //Tables
                AppendSqlTables(sAdExpr03, tData, sql, columns, detailLevelsIds);

                // Joins
                sql.Append(" Where ");

                AppendUniversFilters(sql, tData, fromDate, toDate, vehicle, universId, filters);

                tmp = GenericColumns.GetSqlJoins(_session.DataLanguage, tData.Prefix, columns, detailLevelsIds);
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" {0} ", tmp);
                }
                if (_session.DetailLevel != null)
                {
                    sql.AppendFormat(" {0} ", _session.DetailLevel.GetSqlJoins(_session.DataLanguage, tData.Prefix));
                }
                sql.AppendFormat(" {0} ", GenericColumns.GetSqlContraintJoins(columns));


                //Group by
                AppendSqlGroupByFields(sql, tData, vehicle, detailLevelsIds, columns);

                //Order by 
                AppendSqlOrderFields(tData, sql, vehicle, detailLevelsIds, columns);

            }
            catch (System.Exception err)
            {
                throw new InsertionsDALException(string.Format("Unable to build request to get insertions details. {0}", sql.ToString()), err);
            }


            #region Execution de la requête
            try
            {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw new InsertionsDALException(string.Format("Unable to load data for insertions details: {0}", sql.ToString()), err);
            }
            #endregion

        }

        #region Sub Methods
		/// <summary>
		/// Append data fields
		/// </summary>
		/// <param name="vehicle">Vehicle Information</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="detailLevels">Lis of level details Id</param>
		/// <returns>True if there is detail levels are selected</returns>
        protected bool AppendInsertionsSqlFields(Table tData, VehicleInformation vehicle, StringBuilder sql, ArrayList detailLevelsIds, List<GenericColumnItemInformation> columns)
        {
			
            bool detailLevelSelected = false;
            bool hasSlogan = false;
            bool hasCategory = false;
            bool hasInsertionFields = false;
            string tmp = string.Empty;

            //Detail levels
			if (_session.DetailLevel != null && _session.DetailLevel.Levels != null && _session.DetailLevel.Levels.Count > 0) {
				foreach (DetailLevelItemInformation detailLevelItemInformation in _session.DetailLevel.Levels) {
                    if (detailLevelItemInformation.Id == DetailLevelItemInformation.Levels.category)
                    {
                        hasCategory = true;
                    }
					detailLevelsIds.Add(detailLevelItemInformation.Id.GetHashCode());
				}
                tmp = _session.DetailLevel.GetSqlFields();
				if (tmp.Length > 0) {
					sql.AppendFormat(" {0}", tmp);
					detailLevelSelected = true;
				}

			}

            //Insertions fields
            tmp = GenericColumns.GetSqlFields(columns, detailLevelsIds);
            if (tmp.Length > 0)
            {
                if (detailLevelSelected) sql.Append(",");
                sql.AppendFormat(" {0},", tmp);
            }

            //Constraints fields
            tmp = GenericColumns.GetSqlConstraintFields(columns);
			if (tmp.Length > 0){
				sql.AppendFormat(" {0},", tmp);//Rules constraints management
            }

            //Slogan fields
			AppendSloganField(sql, tData, vehicle, columns);

            //Category
            if (vehicle.Id == CstDBClassif.Vehicles.names.tv && !hasCategory)
            {
                sql.AppendFormat(" {0}.id_category,", tData.Prefix);
            }

            return detailLevelSelected;
        }

        /// <summary>
        /// Add slogan field if required (only for radio because it is required to build path
        /// </summary>
        /// <param name="sql">Sql request container</param>
        /// <param name="vehicle">Vehicle Information</param>
        protected void AppendSloganField(StringBuilder sql, Table tData, VehicleInformation vehicle, List<GenericColumnItemInformation> columns)
        {
            if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SLOGAN_ACCESS_FLAG)
                && vehicle.Id == CstDBClassif.Vehicles.names.radio
                && !_session.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.slogan)
                && sql.ToString().IndexOf("id_slogan") < 0
                )
            {
                sql.AppendFormat(" {0}.id_slogan,", tData.Prefix);
            }
        }

        /// <summary>
        /// Get from clause
        /// </summary>
        /// <param name="tData">Data table</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="detailLevels">Level details</param>
        protected void AppendSqlTables(Schema sAdExpr03, Table tData, StringBuilder sql, List<GenericColumnItemInformation> columns, ArrayList detailLevelsIds)
        {

            string tmp = string.Empty;

            sql.AppendFormat(" from {0} ", tData.SqlWithPrefix);
            if (_session.DetailLevel != null)
            {
                tmp = _session.DetailLevel.GetSqlTables(sAdExpr03.Label);
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(", {0} ", tmp);
                }
            }
            tmp = GenericColumns.GetSqlTables(sAdExpr03.Label, columns, detailLevelsIds);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(", {0} ", tmp);
            }
            tmp = GenericColumns.GetSqlConstraintTables(sAdExpr03.Label, columns);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(", {0} ", tmp);//Rules constraints management
            }

        }

        /// <summary>
        /// Append Filters depending on clien tunivers selection and filters
        /// </summary>
        /// <param name="sql">Sql request container</param>
        /// <param name="tData">Data Table</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">Beginning of the period</param>
        /// <param name="vehicle">Vehicle information</param>
        /// <param name="universId">Current Univers</param>
        /// <param name="filters">FIlters to apply</param>
        protected void AppendUniversFilters(StringBuilder sql, Table tData, int fromDate, int toDate, VehicleInformation vehicle, int universId, string filters)
        {

            #region Period
            if (_session.PeriodType == CstWeb.CustomerSessions.Period.Type.dateToDate)
            {
                int begin = Convert.ToInt32(_session.PeriodBeginningDate);
                if (begin > fromDate)
                {
                    sql.AppendFormat(" {1}.date_media_num >= {0}", begin, tData.Prefix);
                }
                else
                {
                    sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
                }
                int end = Convert.ToInt32(_session.PeriodEndDate);
                if (end < toDate)
                {
                    sql.AppendFormat(" and {1}.date_media_num <= {0}", end, tData.Prefix);
                }
                else
                {
                    sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);
                }
            }
            else
            {
                sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
                sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);
            }
            #endregion

            #region Advertiser modules
            //Versions filter
            if (_module.Id == CstWeb.Module.Name.ALERTE_PLAN_MEDIA && _session.SloganIdList != null && _session.SloganIdList.Length > 0)
            {
                sql.AppendFormat(" and {1}.id_slogan in ( {0} ) ", _session.SloganIdList, tData.Prefix);
            }
            //if (vehicle.Id != CstDBClassif.Vehicles.names.adnettrack){
            //    sql.AppendFormat(" and {0}.id_slogan is not null ", tData.Prefix);
            //}

            //Product classification selection
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
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

            string listMediaAccess = string.Empty;
            if (_module.Id == CstWeb.Module.Name.ANALYSE_PORTEFEUILLE)
            {
                listMediaAccess = _session.GetSelection((TreeNode)_session.ReferenceUniversMedia, CstCustomer.Right.type.mediaAccess) + ",";
            }
            if (_module.Id == CstWeb.Module.Name.ANALYSE_CONCURENTIELLE)
            {
                int positionUnivers = 1;
                while (_session.CompetitorUniversMedia[positionUnivers] != null)
                {
                    listMediaAccess += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
                    positionUnivers++;
                }


            }
            if (_module.Id == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA)
            {
                string list = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
                if (list.Length > 0 && vehicle.Id == CstDBClassif.Vehicles.names.internet) list = list.Replace(vehicle.DatabaseId.ToString(), VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack).DatabaseId.ToString());
                if (list.Length > 0) sql.AppendFormat(" and ({0}.id_vehicle in ({1})) ", tData.Prefix, list);
            }
            if (listMediaAccess.Length > 0)
            {
                sql.AppendFormat(" and (({1}.id_media in ({0}))) ", listMediaAccess.Substring(0, listMediaAccess.Length - 1), tData.Prefix);
            }

            if (_session.SecondaryMediaUniverses != null && _session.SecondaryMediaUniverses.Count > 0)
                sql.Append(_session.SecondaryMediaUniverses[0].GetSqlConditions(tData.Prefix, true));

            #endregion

            #region Rights

            //Product classification rights
            sql.Append(SQLGenerator.getAnalyseCustomerProductRight(_session, tData.Prefix, true));

            //Media classification rights
            if (vehicle.Id == CstDBClassif.Vehicles.names.adnettrack){
                sql.Append(SQLGenerator.GetAdNetTrackMediaRight(_session, tData.Prefix, true));
            }
            else{
                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session, tData.Prefix, true));
            }

            #endregion

            #region Sponsorship univers
            if (_module.Id == CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES
                || _module.Id == CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS)
            {
                string tmp = string.Empty;
                if (_session.CurrentUniversMedia != null && _session.CurrentUniversMedia.Nodes.Count > 0)
                {
                    tmp = _session.GetSelection(_session.CurrentUniversMedia, CstCustomer.Right.type.mediaAccess);
                }
                if (tmp.Length > 0){
                    sql.AppendFormat(" and  {1}.id_media in ({0}) ", tmp, tData.Prefix);
                }
                //Program classification
                sql.Append(SQLGenerator.GetCustomerProgramSelection(_session, tData.Prefix, tData.Prefix, true));
                //Type of sponsorship selection
                sql.Append(SQLGenerator.GetCustomerSponsorshipFormSelection(_session, tData.Prefix, true));
            }
            #endregion

            #region Global rules
            // Filtre Niveau Nomenclature produits
            switch (_module.Id)
            {
                case CstWeb.Module.Name.ALERTE_PLAN_MEDIA:
                case CstWeb.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                case CstWeb.Module.Name.ANALYSE_POTENTIELS:
                    sql.Append(SQLGenerator.getLevelProduct(_session, tData.Prefix, true));
                    break;
            }


            // Radio rules
            if (_module.ModuleType == CstWeb.Module.Type.tvSponsorship){
                sql.Append(SQLGenerator.getAdExpressUniverseCondition(CstWeb.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID, tData.Prefix, tData.Prefix, tData.Prefix, true));
            }
            else{
                sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, tData.Prefix, true, false));
            }
            sql.AppendFormat(" and {0}.id_category<>35  ", tData.Prefix);//NO TV NAT thématiques
            #endregion

            #region Filtres
            if (filters != null && filters.Length > 0)
            {
                GenericDetailLevel detailLevels = null;
                switch (_module.Id)
                {
                    case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                    case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                        detailLevels = _session.GenericProductDetailLevel;
                        break;
                    case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                    case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                    case CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    case CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES:
                        detailLevels = _session.GenericMediaDetailLevel;
                        break;
                }
                sql.Append(GetFiltersClause(tData, detailLevels, filters, vehicle));
                sql.AppendFormat(CheckZeroVersion(tData, detailLevels,vehicle, filters));
            }
            if (_session.SloganIdZoom > -1)
            {
                sql.AppendFormat(" and wp.id_slogan={0}", _session.SloganIdZoom);
            }
            if (_creaConfig && vehicle.Id != CstDBClassif.Vehicles.names.adnettrack && vehicle.Id != CstDBClassif.Vehicles.names.internet)
            {
                sql.AppendFormat(" and {0}.id_slogan is not null", tData.Prefix);
            }

            #endregion

            if (vehicle.Id != CstDBClassif.Vehicles.names.internet)
            {
                sql.AppendFormat(" and {1}.id_vehicle={0} ", vehicle.DatabaseId, tData.Prefix);
            }
            else
            {
                sql.AppendFormat(" and {1}.id_vehicle={0} ", VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack).DatabaseId, tData.Prefix);
            }

        }

        /// <summary>
        /// Append Group byb clause
        /// </summary>
        /// <param name="tData">Data Table</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="detailLevelIds">List of levels detail</param>
        protected void AppendSqlGroupByFields(StringBuilder sql, Table tData, VehicleInformation vehicle, ArrayList detailLevelIds, List<GenericColumnItemInformation> columns)
        {

            string tmp = string.Empty;
            bool first = true;

            sql.Append(" group by");
            if (_session.DetailLevel != null){
                tmp = _session.DetailLevel.GetSqlGroupByFields();
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" {0}", tmp);
                    first = false;
                }
            }

            tmp = GenericColumns.GetSqlGroupByFields(columns, detailLevelIds);
            if (tmp.Length > 0)
            {
                if (!first) sql.Append(",");
                sql.AppendFormat(" {0}", tmp);
                first = false;
            }

            if (columns != null)
            {
                tmp = GenericColumns.GetSqlConstraintGroupByFields(columns);
                if (tmp != null && tmp.Length > 0)
                {
                    if (!first) sql.Append(",");
                    first = false;
                    sql.AppendFormat(" {0}", tmp);
                }
            }

            if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SLOGAN_ACCESS_FLAG)
                && vehicle.Id == CstDBClassif.Vehicles.names.radio
                && !_session.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.slogan)
                )
            {
                sql.AppendFormat(", {0}.id_slogan ", tData.Prefix);
            }

            if (!first && vehicle.Id == CstDBClassif.Vehicles.names.tv && !_session.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.category))
            {
                sql.AppendFormat(" , {0}.id_category", tData.Prefix);
            }

        }

        /// <summary>
        /// Append request "Order by" clause
        /// </summary>
        /// <param name="sql">Sql request container</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="detailLevelList">List of detail levels</param>
        protected void AppendSqlOrderFields(Table tData, StringBuilder sql, VehicleInformation vehicle, ArrayList detailLevelIds, List<GenericColumnItemInformation> columns)
        {
            string tmp = string.Empty;
            bool first = true;

            sql.Append(" order by ");
            if (_session.DetailLevel != null)
            {
                tmp = _session.DetailLevel.GetSqlOrderFields();
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" {0}", tmp);
                    first = false;
                }
            }

            tmp = GenericColumns.GetSqlOrderFields(columns, detailLevelIds);
            if (tmp.Length > 0)
            {
                if (!first) sql.Append(",");
                sql.AppendFormat(" {0}", tmp);
                first = false;
            }

            if (columns != null)
            {
                tmp = GenericColumns.GetSqlConstraintOrderFields(columns);
                if (tmp != null && tmp.Length > 0){
                    if (!first)sql.Append(",");
                    first = false;
                    sql.AppendFormat(" {0}", tmp);
                }
            }

            AppendSloganField(sql, tData, vehicle,columns);

        }
        #endregion

        #endregion

    }
}
