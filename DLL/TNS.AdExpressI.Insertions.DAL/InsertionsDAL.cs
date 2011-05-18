using System;
using System.Collections.Generic;
using System.Text;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using FctWeb = TNS.AdExpress.Web.Core.Utilities;

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

namespace TNS.AdExpressI.Insertions.DAL {
    /// <summary>
    /// This class is used to extract advertising detail for insertions or versions.
    /// In this class we can use five methods, depending on what we need for our result. The three methods are :
    /// <code>public DataSet GetInsertionsData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)</code>
    /// <code>public DataSet GetCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)</code>
    /// <code>public DataSet GetMSCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)</code>
    /// <code> public virtual Int64[] GetVehiclesIds(Dictionary[DetailLevelItemInformation, Int64] filters)</code>
    /// <code>public virtual List[VehicleInformation] GetPresentVehicles(List[VehicleInformation] vehicles, string filters, int fromDate, int toDate, int universId, TNS.AdExpress.Domain.Web.Navigation.Module module, bool sloganNotNull)</code>
    /// They have as parameters : informations about the media type studied, date begin and date end of the study, 
    /// the univers identifier that represents which univers we're going to use (in the case we have many universe for our study (competitor study))
    /// and levels of classification selected by the customer (filters).
    /// The dataSet returned by the methods "GetInsertionsData","GetCreativesData","GetMSCreativesData" will be structured as follows:
    /// For insertions : 
    ///     DATATABLE[Level1_Id, Level1_Label, Level2_Id, Level2_Label, Level3_Id, Level3_Label, Column1_id, Column1_label, Column2_id, Column2_label ..., Column'1_value, Column'2_value ...]
    ///         Level1_Id and Level1_Label : corresponds to the identifier and the label of a classification level (media type, group, sub group ...)
    ///         Level2_Id and Level2_Label : corresponds to the identifier and the label of a classification level (media type, group, sub group ...)
    ///         Level3_Id and Level3_Label : corresponds to the identifier and the label of a classification level (media type, group, sub group ...)
    ///         Column1_id and Column1_label : corresponds to the identifiers and the label of a classification column (media type, group, sub group ...)
    ///         Column2_id and Column2_label : corresponds to the identifiers and the label of a classification column (media type, group, sub group ...)
    ///         Column'1_value : column'1 correspond to a classification column (color, format, page ...), in this case we get only the value of the column
    ///
    /// For versions :
    ///     DATATABLE[Column1_id, Column1_label, Column2_id, Column2_label ..., Column'1_value, Column'2_value ...]
    ///         The columns description is identical to the one above
    /// 
    /// The method "GetPresentVehicles" obtains the list of vehicles wich have data corresponding to the selected working set.
    /// <remarks>
    /// remark 1 : the three classification levels can be selected in the insertions page
    /// remark 2 : The list of column is defined in the Xml configuration file GenericColumn.xml, in this file we have also customized columns lists.
    ///            example of column description :
    ///                 <column id="1" name="Media" webTextId="363" dbId="id_vehicle" dbLabel="vehicle" dbTable="vehicle" dbTablePrefixe="vh" CellType="TNS.FrameWork.WebResultUI.CellLabel"/>
    ///            example of customized columns list :
    ///                 <detailColumn id="23" name="Spot\Advertiser\Product\Group\Time aired\Duration\Position\Adbreak duration\Nb spots/adbreak\Position\Adbreak duration\Nb spots\Price">
    ///                     <columnItem id="10" name="Spot" notInExcelExport="true"/>
    ///                     <columnItem id="7" name="Advertiser" idDetailLevelMatching="8"/>
    ///                                                 ...
    /// remark 3 : For the present/absent module we don't need informations about univers so the univers identifiers is equal to -1</remarks>
    /// remark 4 : The difference between classification levels and classification columns is that the last ones are used to know what are 
    /// the columns that we're going to show in the table result and the first ones are used to know what are the levels that we're going 
    /// to use for the group by clause in the SQL request, so we are able to group by one, two or three levels.
    /// remark 5 : For the classification column we have the constraint concept :
    ///     For some columns, we not only need information about the column but we need information about others fields related to the column
    ///     example : 
    ///         <column id="35" name="Visuel" webTextId="1909"  dbLabel="visual">
    ///             <constraints>
    ///                 <dbConstraints>
    ///                     <dbFieldConstraints>
    ///                         <dbFieldConstraint id="1" name="Visuel Disponibility" dbField="disponibility_visual" dbTablePrefixe="appliMd"/>
    ///                         <dbFieldConstraint id="2" name="Visuel Activation" dbField="activation" dbTablePrefixe="appliMd"/>
    ///                     </dbFieldConstraints>
    ///                 </dbConstraints>
    ///             </constraints>
    ///         </column>
    /// so we define in the Xml configuration file all the fields related to one column (like above), 
    /// and we can get this fileds by using the method GenericColumns.GetSqlConstraintFields(columns)
    /// </summary>
    public abstract class InsertionsDAL:IInsertionsDAL {

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
        /// Creatives configuration (True if we're using InsertionDAL to get informations about a version and not an insertion)
        /// </summary>
        protected bool _creaConfig = false;
        /// <summary>
        /// Media schedule creatives config (True if we're using InsertionDAL to get informations about specific versions for the Media Schedule module, 
        /// the diffrence between this variable and the one above is that we need specific informations about versions
        /// for the media schedule module and for pdf exports)
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
        /// <summary>
        /// Get Media types identifiers according to levels of classification selected by the customer
        /// <example> If the filters variable contain : (media type id = 1)/(sub media id = 11)
        ///     // get list of media type identifiers (in our classification we use vehicle for media type)
        ///     select distinct id_vehicle 
        ///     // A view that contains all the media classification (id_vehicle(media type), id_media(vehicle), id_interest_center, id_basic_media, id_title ...) 
        ///     from adexpr03.all_media_44
        ///     // The data is filtered by the media type 1
        ///     where id_vehicle = 1
        ///     // The data is filtered by the sub media 11
        ///     and id_category = 11
        /// </example>
        /// </summary>
        /// <param name="filters">Dictionary of [classification levels, value(corresponds to the value of the classification level in the database)]</param>
        /// <returns>Media types identifiers list</returns>
        public virtual Int64[] GetVehiclesIds(Dictionary<DetailLevelItemInformation, Int64> filters) {
            
            Int64[] ids = null;
            StringBuilder sql = new StringBuilder();

            /* Get the description of the media type table
             * The description contains : Table identifier, table name, table prefix, database schema identifier, database schema
             * */
            Table tVehicle = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);

            /* Get the description of the all media view
             * The description contains : View identifier, view name, view prefix, database schema identifier, database schema
             * This view contains (id_vehicle(media type), id_media(vehicle), id_interest_center, id_basic_media, id_title ...) 
             * */
            TNS.AdExpress.Domain.DataBaseDescription.View vMedia = WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia);

            /* SQL request building
             * */
            sql.AppendFormat("select distinct {0}.id_vehicle from {1}{2} {0} where 1=1 ", vMedia.Prefix, vMedia.Sql, _session.DataLanguage);

            /* In the view we have only vehicle classification fields so we can only filter by vehicle classification levels like below
             * */
            foreach (DetailLevelItemInformation d in filters.Keys) {

                if (filters[d] >= 0) {
                    switch (d.Id) {
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

            try {
                /* Getting the data
                 * */
                DataSet ds = _session.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null) {
                    DataTable dt = ds.Tables[0];
                    ids = new Int64[dt.Rows.Count];
                    for(int i=0; i < dt.Rows.Count; i++) {
                        ids[i] = Convert.ToInt64(dt.Rows[i][0]);
                    }
                }
            }
            catch (System.Exception err) {
                throw (new InsertionsDALException("Unable to get list of vehicles ids : " + sql, err));
            }

            return ids;

        }
        #endregion

        #region GetPresentVehicles
        /// <summary>
        /// Get list of media type for which there're data in the database
        /// </summary>
        /// <param name="vehicles">List of Media type object to check</param>
        /// <param name="filters">List identiifers selected by the customer. Used as data filters in the query ex. "</param>
        /// <param name="fromDate">User Period beginning</param>
        /// <param name="toDate">User Period End</param>
        /// <param name="universId">User Univers Selection</param>
        /// <param name="module">Current Module</param>
        /// <param name="sloganNotNull">True if slogan not null</param>
        /// <returns>List of vehicles present</returns>
        public virtual List<VehicleInformation> GetPresentVehicles(List<VehicleInformation> vehicles, string filters, int fromDate, int toDate, int universId, TNS.AdExpress.Domain.Web.Navigation.Module module, bool sloganNotNull) {
            
            StringBuilder sql = new StringBuilder();
            /* List of media type for which we have data in the database
             * The vehicleInformation object contains informations about a media type like : 
             *  databaseId : dataBase identifier, 
             *  showInsertions : if we can show insertions for this media type
             *  showCreations : if we can show versions for this media type
             *  allowedUnitsList : allowed units list
             *  detailColumnId : detail columns id 
             *  ...
             * */
            List<VehicleInformation> found = new List<VehicleInformation>();
            DataSet ds = null;
            /* For the version which don't have TODO explication of version 0
             * so when sloganNotNull is true, it means that we're treating versions informations and not insertions informations
             * */
            this._creaConfig = sloganNotNull;

            try {

                bool first = true;

                universId--;
                Table dataTable;
                /* Getting the database schema
                 * */
                Schema sAdEx = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);

                /* For each meida type we check if we have data in data base
                 * */
                foreach (VehicleInformation v in vehicles) {
                    
                    /* Get table for the current media type and according to the current module
                     * */
                    dataTable = GetDataTable(v, module.ModuleType);
                    
                    if (!first)
                        sql.Append(" UNION ");
                    else
                        first = false;

                    /* This test conerns only the French version of the site
                     * */
                    if (v.Id != CstDBClassif.Vehicles.names.internet) {
                        sql.Append(" select id_vehicle from ");
                    }
                    else {
                        sql.AppendFormat(" select {0} as id_vehicle from ", v.DatabaseId);
                    }

                    sql.AppendFormat(" {0} ", dataTable.SqlWithPrefix);
                    sql.Append(" where ");
                    /* Apply the differents univers filters : univers selected and rights
                     * this method is more detailed below
                     * */
                    AppendUniversFilters(sql, dataTable, fromDate, toDate, v, universId, filters);
                    sql.AppendFormat(" and rownum < 2 ");
                }

                /* Getting the data
                 * */
                ds = _session.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) {

                    /* initialisation of the foud list with media type for which we have foud data in the database
                     * */
                    foreach (DataRow row in ds.Tables[0].Rows) {
                        found.Add(VehiclesInformation.Get(Convert.ToInt64(row["id_vehicle"])));
                    }
                }
            }
            catch (System.Exception err) {
                throw new InsertionsDALException(string.Format("GetData::Unable to check there is data. {0}", sql.ToString()), err);
            }

            return found;
        }
        #endregion

        #region GetDataTable
        /// <summary>
        /// Get the table name for a specific media type and depending on the module type
        /// <example>Data_Press, Data_Radio, Data_Tv ...</example>
        /// </summary>
        /// <param name="vehicle">Media Type</param>
        /// <param name="moduleType">Module Type</param>
        /// <returns>Table Description</returns>
        protected virtual Table GetDataTable(VehicleInformation vehicle, CstWeb.Module.Type moduleType)
        {
            
            /* This test test concerns only the French version of the site
             * */
            if (vehicle.Id != CstDBClassif.Vehicles.names.internet) {
                /* Get the data table description object according to the media type (vehicle) and the module type (moduleType)
                 * */
                return SQLGenerator.GetDataTable(vehicle, _module.ModuleType);
            }
            else {

                switch (moduleType) {
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
        /// to build the filter clause we need to know what are the classifications levels selected by the customer and the value of each level,
        /// to do that we use detailLevels object where we save the classifications levels list and the string filters which contains 
        /// the values corresponding to the levels 
        /// 
        /// Example : 
        ///     detailLevels : (vehicle,category)
        ///     filters : (1,11)
        /// </summary>
        /// <param name="Table">Data Table Description</param>
        /// <param name="detailLevels">Detail Levels (classification levels) Selected by the customer</param>
        /// <param name="filters">Filters Identifiers (contains the values of the corresponding classification levels)</param>
        /// <param name="vehicle">Media type information</param>
        /// <returns>Filters clause</returns>
        protected virtual string GetFiltersClause(Table table, TNS.AdExpress.Domain.Level.GenericDetailLevel detailLevels, string filters, VehicleInformation vehicle) {

            StringBuilder str = new StringBuilder();
            /* Get the classification levels values
             * */
            string[] ids = filters.Split(',');

            DetailLevelItemInformation level = null;
            Int64 id = 0;
            /*  For each detail level (classification level), we add a filter in our SQL query
             * */
            for (int i = 0; i < ids.Length && i < detailLevels.Levels.Count; i++) {
                id = Convert.ToInt64(ids[i]);
                level = (DetailLevelItemInformation)detailLevels.Levels[i];
                if (id > 0
                    || (id == 0 && level.Id != DetailLevelItemInformation.Levels.slogan)
                    || (id != -1 && id != 0 && level.Id == DetailLevelItemInformation.Levels.slogan && (vehicle.Id == CstDBClassif.Vehicles.names.adnettrack || vehicle.Id == CstDBClassif.Vehicles.names.internet || vehicle.Id == CstDBClassif.Vehicles.names.evaliantMobile))) {
                    /* This test concerns only the French version of the site
                     * */
                    if (level.DataBaseIdField == CstDB.Fields.ID_VEHICLE && VehiclesInformation.Contains(CstDBClassif.Vehicles.names.internet) && id == VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.internet))
                        id = CstDBClassif.Vehicles.names.adnettrack.GetHashCode();
                    /* Add a filter to the SQL query, for the following media type : internet, adnettrack or evaliantMobile we use "hashcode" instead of the identifier,
                     * this is a specificity of the French database
                     * */
                    if (level.Id == DetailLevelItemInformation.Levels.slogan && (vehicle.Id == CstDBClassif.Vehicles.names.adnettrack || vehicle.Id == CstDBClassif.Vehicles.names.internet || vehicle.Id == CstDBClassif.Vehicles.names.evaliantMobile)) {
                        str.AppendFormat(" and {1}.hashcode = {0}", id, table.Prefix);
                    }
                    else{
                        str.AppendFormat(" and {2}.{0} = {1}", level.DataBaseIdField, id, table.Prefix);
                    }
                }
                /* When we have solgan = 0, we add the following line
                 * */
                if (id == 0 && level.Id == DetailLevelItemInformation.Levels.slogan && vehicle.Id != CstDBClassif.Vehicles.names.adnettrack && vehicle.Id != CstDBClassif.Vehicles.names.internet && vehicle.Id != CstDBClassif.Vehicles.names.evaliantMobile) {
                    str.AppendFormat(" and {2}.{0} = {1}", level.DataBaseIdField, id, table.Prefix);
                }
            }

            return str.ToString();
        }
        #endregion

        #region versions 0
        /// <summary>
        /// Test if version id is 0 TODO version 0 explication
        /// </summary>
        /// <param name="table">Data Table Description</param>
        /// <param name="detailLevels">Detail Levels Selected<</param>
        /// <param name="vehicle">Vehicle</param>
        /// <param name="filters">Filters</param>
        /// <returns>SQL</returns>
        private static string CheckZeroVersion(Table table, TNS.AdExpress.Domain.Level.GenericDetailLevel detailLevels, VehicleInformation vehicle, string filters) {
            
            Int64 id = 0;
            string[] ids = filters.Split(',');
            /* Check if the slogan level is sontained in the detailLevels object (Object that contains a list of classification levels)
             * */
            int rank = detailLevels.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan);
            /* If rank != 0 that means id_solgan is not null and we don't need to add the line below but if it's equal to 0, we have to added it
             * */
            if (rank != 0) {
                id = Convert.ToInt64(ids[rank - 1]);
                if (id == 0 && vehicle.Id != CstDBClassif.Vehicles.names.adnettrack && vehicle.Id != CstDBClassif.Vehicles.names.internet && vehicle.Id != CstDBClassif.Vehicles.names.evaliantMobile) 
                    return string.Format(" and {0}.id_slogan is null ", table.Prefix);
            }
            return ("");
        }
        #endregion

        #region Insertions
        /// <summary>
        /// Extract advertising detail for insertions details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information (differents informations about a media type like databaseId, showInsertions..., this object is more detailed above)</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="universId">User Univers Selection (correspondig to which is the current univers if we have a competitor study)</param>
        /// <param name="filters">Filters Identifiers (A list of values correspondig to the classification levels)</param>
        /// <returns>Advertising detail Data</returns>
        public virtual DataSet GetInsertionsData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)
        {
            /* (False if we're using InsertionDAL to get informations about insertions)
              * */
            _creaConfig = false;
            return GetData(vehicle, fromDate, toDate, universId, filters);
        }
        #endregion 

        #region Creatives
        /// <summary>
        /// Extract advertising detail for creatives details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information (differents informations about a media type like databaseId, showInsertions..., this object is more detailed above)</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="universId">User Univers Selection (correspondig to which is the current univers if we have a competitor study)</param>
        /// <param name="filters">Filters Identifiers (A list of values correspondig to the classification levels)</param>
        /// <returns>Advertising detail Data</returns>		
        public virtual DataSet GetCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters) {
            /* (True if we're using InsertionDAL to get informations about versions)
             * */
            _creaConfig = true;
            return GetData(vehicle, fromDate, toDate, universId, filters);
        }
        #endregion

        #region MS Creatives
        /// <summary>
        /// Extract advertising detail for media schedule creatives details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information (differents informations about a media type like databaseId, showInsertions..., this object is more detailed above)</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="universId">User Univers Selection (correspondig to which is the current univers if we have a competitor study)</param>
        /// <param name="filters">Filters Identifiers (A list of values correspondig to the classification levels)</param>
        /// <returns>Advertising detail Data</returns>		
        public virtual DataSet GetMSCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)
        {
            /* The media schedule creatives details is only called in the media schedule module
             * */
            _module = ModulesList.GetModule(CstWeb.Module.Name.ANALYSE_PLAN_MEDIA);
            /* (True if we're using InsertionDAL to get informations about specific versions for the Media Schedule module)
             * */
            _msCreaConfig = true;
            return GetData(vehicle, fromDate, toDate, universId, filters);
        }
        #endregion

        #region GetData
        /// <summary>
        /// Extract advertising detail for creatives or insertions details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="universId">The univers identifier used for the study, in the Present/Absent module it equal to -1 because we don't need this information</param>
        /// <param name="filters">Each parameter of the filters list represents a value for a classification level, the first one correspond to the Level1 the seconed to Level2</param>
        /// <returns>Advertising detail Data</returns>		
        protected virtual DataSet GetData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters) {

            StringBuilder sql = new StringBuilder(5000);
            ArrayList detailLevelsIds = new ArrayList();
            /* Get the table name for a specific vehicle and depending on the module type
             * Example : data_press, data_tv, data_radio ...
             * */
            Table tData = GetDataTable(vehicle, _module.ModuleType);
            /* Get the data base shema
             * */
            Schema sAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
            string tmp = string.Empty;
            /* Get the list of columns that are going to be used to describe the version or the insertion.
             * Example : Media, Category, Sub category, advertiser, product, time aired, duration, position ...
             * The list of columns description is defined in the XML file : GenericColumn.xml, 
             * and we can also find in this file, several customized columns lists like : 
             *  Page\Visuel\Annonceur\Groupe\Produit\Version\Centre d'interet\Régie\Format\Surface\Couleur\Prix\Descriptif
             *  or Spot\Annonceur\Produit\Version\Centre d'interet\Régie\Groupe\Top de diffusion\Durée\Position\Durée écran\Nombre Spots écran\Position hap\Durée écran hap\Nombre spots hap\Prix\Descriptif
             * We use three others configuration files, the first for insertions , the second for versions and the third for versions exports (versions used in export results like PDf or Excel), following the description of the three files :
             * MediaPlanInsertionConfiguration.xml : contains for every vehicle a customized columns list (in this file we use detailColumn Id that match the one defined in the GenericColumn.xml file).
             * CreativesConfiguration.xml          : contains for every vehicle a customized columns list (in this file we use detailColumn Id that match the one defined in the GenericColumn.xml file).
             * MSCreativesDetails.xml              : contains for every vehicle a customized columns list (in this file we use detailColumn Id that match the one defined in the GenericColumn.xml file).
             * */
            List<GenericColumnItemInformation> columns = null;
            /* Get the detail columns for differents case. detail column correspond to a description of the column,
             * it contains informations like : name, dataBaseIdField, dataBaseAliasIdField, dataBaseField, dataBaseTableNamePrefix ...
             * */
            if (_msCreaConfig)
                columns = WebApplicationParameters.MsCreativesDetail.GetDetailColumns(vehicle.DatabaseId);
            else if(_creaConfig)
                columns = WebApplicationParameters.CreativesDetail.GetDetailColumns(vehicle.DatabaseId, _module.Id);
            else
                columns = WebApplicationParameters.InsertionsDetail.GetDetailColumns(vehicle.DatabaseId, _module.Id);
            
            try {

                // Select
                sql.Append(" select distinct ");
                // Append data fields
                AppendInsertionsSqlFields(tData, vehicle, sql, detailLevelsIds, columns);
                sql.Length -= 1;

                // Append data tables
                AppendSqlTables(sAdExpr03, tData, sql, columns, detailLevelsIds);

                sql.Append(" Where ");

                // Append filters
                AppendUniversFilters(sql, tData, fromDate, toDate, vehicle, universId, filters);

                // Append Joins
                tmp = GenericColumns.GetSqlJoins(_session.DataLanguage, tData.Prefix, columns, detailLevelsIds);
                if (tmp.Length > 0) {
                    sql.AppendFormat(" {0} ", tmp);
                }
                if (!_msCreaConfig && _session.DetailLevel != null) {
                    sql.AppendFormat(" {0} ", _session.DetailLevel.GetSqlJoins(_session.DataLanguage, tData.Prefix));
                }
                sql.AppendFormat(" {0} ", GenericColumns.GetSqlContraintJoins(columns));

                // Append Group by
                AppendSqlGroupByFields(sql, tData, vehicle, detailLevelsIds, columns);

                // Append Order by 
                AppendSqlOrderFields(tData, sql, vehicle, detailLevelsIds, columns);

            }
            catch (System.Exception err) {
                throw new InsertionsDALException(string.Format("Unable to build request to get insertions details. {0}", sql.ToString()), err);
            }


            #region Execution de la requête
            try {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw new InsertionsDALException(string.Format("Unable to load data for insertions details: {0}", sql.ToString()), err);
            }
            #endregion

        }
        #endregion

        #region Sub Methods

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
        /// <param name="detailLevelsIds">List of level details Identifiers</param>
        /// <param name="columns">List of detail columns</param>
		/// <returns>True if there is detail levels selected</returns>
        protected virtual bool AppendInsertionsSqlFields(Table tData, VehicleInformation vehicle, StringBuilder sql, ArrayList detailLevelsIds, List<GenericColumnItemInformation> columns)
        {

            bool detailLevelSelected = false;
            bool hasCategory = false;
            string tmp = string.Empty;

            //Detail levels
            /* The list of detail levels Identifiers is initialized according to levels saved in the _session.DetailLevel object.
             * to get the fields corresponding to the levels we use _session.DetailLevel.GetSqlFields().
             * */
            if (!_msCreaConfig && _session.DetailLevel != null && _session.DetailLevel.Levels != null && _session.DetailLevel.Levels.Count > 0) {
                foreach (DetailLevelItemInformation detailLevelItemInformation in _session.DetailLevel.Levels) {
                    if (detailLevelItemInformation.Id == DetailLevelItemInformation.Levels.category) {
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
            /* Get the SQL code of the fields corresponding to columns (from the XML configuration files) except the ones that mutched with
             * the detail level list
             * */
            tmp = GenericColumns.GetSqlFields(columns, detailLevelsIds);
            if (tmp.Length > 0) {
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
			if (tmp.Length > 0){
				sql.AppendFormat(" {0},", tmp);//Rules constraints management
            }

            // Slogan fields
            // Add slogan field if required (only for radio because it is required to build path
            if (!_msCreaConfig)
			    AppendSloganField(sql, tData, vehicle, columns);

            //Category
            if (!_msCreaConfig && vehicle.Id == CstDBClassif.Vehicles.names.tv && !hasCategory) {
                sql.AppendFormat(" {0}.id_category,", tData.Prefix);
            }

            return detailLevelSelected;
        }
        #endregion

        #region AppendSloganField
        /// <summary>
        /// Add slogan field if required (only for radio because it is required to build path
        /// </summary>
        /// <param name="sql">Sql request container</param>
        /// <param name="tData">Data base table description</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="columns">List of detail columns</param>
        protected virtual void AppendSloganField(StringBuilder sql, Table tData, VehicleInformation vehicle, List<GenericColumnItemInformation> columns)
        {
            /* The method CustormerFlagAccess is used to check if the user has the right to the slogan level or not
             * */
            if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SLOGAN_ACCESS_FLAG)
                && vehicle.Id == CstDBClassif.Vehicles.names.radio
                && !_session.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.slogan)
                && sql.ToString().IndexOf("id_slogan") < 0
                ) {
                sql.AppendFormat(" {0}.id_slogan,", tData.Prefix);
            }
        }
        #endregion

        #region AppendSqlTables
        /// <summary>
        /// Get from clause
        /// The list of tables used in the query will be generated as following :
        ///     + Tables that correspond to the detail levels : Level1Table, Level2Table, Level3Table.
        ///       We can get this tables using _session.DetailLevel.GetSqlTables(sAdExpr03.Label)
        ///     + Tables that correspond to the list of columns, we can get this tables using GenericColumns.GetSqlTables(sAdExpr03.Label, columns, detailLevelsIds)
        ///     + Specific tables : for some columns we need to get informations not only from the column table but from others tables.
        ///       So, to do that we define in the Xml configuration file a list of tables related to a column 
        ///       and we can get this list by using GenericColumns.GetSqlConstraintTables(sAdExpr03.Label, columns)
        /// </summary>
        /// <param name="tData">Data table</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="detailLevels">Level details</param>
        protected virtual void AppendSqlTables(Schema sAdExpr03, Table tData, StringBuilder sql, List<GenericColumnItemInformation> columns, ArrayList detailLevelsIds)
        {

            string tmp = string.Empty;

            sql.AppendFormat(" from {0} ", tData.SqlWithPrefix);
            /* Get tables that corresponds to the classification levels
             * */
            if (!_msCreaConfig && _session.DetailLevel != null) {
                tmp = _session.DetailLevel.GetSqlTables(sAdExpr03.Label);
                if (tmp.Length > 0) {
                    sql.AppendFormat(", {0} ", tmp);
                }
            }
            /* Get tables that corresponds to the classification columns
             * */
            tmp = GenericColumns.GetSqlTables(sAdExpr03.Label, columns, detailLevelsIds);
            if (tmp.Length > 0) {
                sql.AppendFormat(", {0} ", tmp);
            }
            /* Get tables that corresponds to specific columns
             * */
            tmp = GenericColumns.GetSqlConstraintTables(sAdExpr03.Label, columns);
            if (tmp.Length > 0) {
                sql.AppendFormat(", {0} ", tmp);//Rules constraints management
            }
        }
        #endregion

        #region AppendUniversFilters
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
        protected virtual void AppendUniversFilters(StringBuilder sql, Table tData, int fromDate, int toDate, VehicleInformation vehicle, int universId, string filters)
        {

            #region Period
            /* Determine the period of the study
             * PeriodType correspond to the type of date selection, for example :
             *  dateToDate : means that the date format is YYYYMMDD
             *  dateToDateMonth : means that the date format is YYYYMM
             *  dateToDateWeek : means that the date format is YYYYWW
             * depending on the date format we can create the correct SQL query filter
             * */
            if (_session.PeriodType == CstWeb.CustomerSessions.Period.Type.dateToDate){

                int begin = Convert.ToInt32(_session.PeriodBeginningDate);
                if (begin > fromDate) {
                    sql.AppendFormat(" {1}.date_media_num >= {0}", begin, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES){
                        sql.AppendFormat(" and {0}.date_creation >= to_date('{1} 00:00:00','yyyymmdd HH24:MI:SS') ", tData.Prefix, begin);
                    }
                }
                else {
                    sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES){
                        sql.AppendFormat(" and {0}.date_creation >= to_date('{1} 00:00:00','yyyymmdd HH24:MI:SS') ", tData.Prefix, fromDate);
                    }
                }
                int end = Convert.ToInt32(_session.PeriodEndDate);
                if (end < toDate) {
                    sql.AppendFormat(" and {1}.date_media_num <= {0}", end, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES){
                        sql.AppendFormat(" and {0}.date_creation <= to_date('{1} 23:59:59','yyyymmdd HH24:MI:SS') ", tData.Prefix, end);
                    }
                }
                else {
                    sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES){
                        sql.AppendFormat(" and {0}.date_creation <= to_date('{1} 23:59:59','yyyymmdd HH24:MI:SS') ", tData.Prefix, toDate);
                    }
                }
            }
            else {
                sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
                sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);
                if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES){
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
            if (_module.Id == CstWeb.Module.Name.ALERTE_PLAN_MEDIA && _session.SloganIdList != null && _session.SloganIdList.Length > 0) {
                sql.AppendFormat(" and {1}.id_slogan in ( {0} ) ", _session.SloganIdList, tData.Prefix);
            }
            // Product classification selection
            /* We can get the product classification selection by using _session.PrincipalProductUniverses[universId].GetSqlConditions(tData.Prefix, true)
             * */
            if (_module.Id != CstWeb.Module.Name.NEW_CREATIVES && _session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0) {
                if (universId < 0) {
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(tData.Prefix, true));
                }
                else {
                    sql.Append(_session.PrincipalProductUniverses[universId].GetSqlConditions(tData.Prefix, true));
                }
            }
            #endregion

            #region Media modules
            /* Get the list of medias selected
             * There are several treenodes to save the univers media, so according to the module we can get the media list
             * */
            string listMediaAccess = string.Empty;
            if (_module.Id == CstWeb.Module.Name.ANALYSE_PORTEFEUILLE) {
                listMediaAccess = _session.GetSelection((TreeNode)_session.ReferenceUniversMedia, CstCustomer.Right.type.mediaAccess) + ",";
            }
            if (_module.Id == CstWeb.Module.Name.ANALYSE_CONCURENTIELLE) {
                int positionUnivers = 1;
                while (_session.CompetitorUniversMedia[positionUnivers] != null) {
                    listMediaAccess += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
                    positionUnivers++;
                }
            }
            if (_module.Id == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA) {
                string list = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
                if (list.Length > 0 && vehicle.Id == CstDBClassif.Vehicles.names.internet) list = list.Replace(vehicle.DatabaseId.ToString(), VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack).DatabaseId.ToString());
                if (list.Length > 0) sql.AppendFormat(" and ({0}.id_vehicle in ({1})) ", tData.Prefix, list);
            }
            if (listMediaAccess.Length > 0) {
                sql.AppendFormat(" and (({1}.id_media in ({0}))) ", listMediaAccess.Substring(0, listMediaAccess.Length - 1), tData.Prefix);
            }

            if (_session.SecondaryMediaUniverses != null && _session.SecondaryMediaUniverses.Count > 0)
                sql.Append(_session.SecondaryMediaUniverses[0].GetSqlConditions(tData.Prefix, true));

            #endregion

            #region Rights
            /* Get product classification rights
             * */
            if (_module == null) throw (new InsertionsDALException("_module parameter cannot be NULL"));
            sql.Append(FctWeb.SQLGenerator.GetClassificationCustomerProductRight(_session, tData.Prefix, true, _module.ProductRightBranches));


            /* Get media classification rights
             * */
            if (vehicle.Id == CstDBClassif.Vehicles.names.internet){
                sql.Append(SQLGenerator.GetAdNetTrackMediaRight(_session, tData.Prefix, true));
            }
            else{
                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session, tData.Prefix, true));
            }

            /* Get rights detail spot to spot TNT
             * */
            if (vehicle.Id == CstDBClassif.Vehicles.names.tv
                && !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG) && !_creaConfig) {
                string idTNTCategory = TNS.AdExpress.Domain.Lists.GetIdList(CstWeb.GroupList.ID.category, CstWeb.GroupList.Type.digitalTv);
                if (idTNTCategory != null && idTNTCategory.Length > 0) {
                    sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_category not in (" + idTNTCategory + ")  ");
                }
            }
            #endregion

            #region Sponsorship univers
            /* This test is specific to the sponsoring modules
             * */
            if (_module.Id == CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES
                || _module.Id == CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS) {
                string tmp = string.Empty;
                if (_session.CurrentUniversMedia != null && _session.CurrentUniversMedia.Nodes.Count > 0) {
                    tmp = _session.GetSelection(_session.CurrentUniversMedia, CstCustomer.Right.type.mediaAccess);
                }
                if (tmp.Length > 0){
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
            switch (_module.Id) {
                case CstWeb.Module.Name.ALERTE_PLAN_MEDIA:
                case CstWeb.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                case CstWeb.Module.Name.ANALYSE_POTENTIELS:
                    sql.Append(SQLGenerator.getLevelProduct(_session, tData.Prefix, true));
                    break;
            }

            /* Get radio rules
             * */
            if (_module.ModuleType == CstWeb.Module.Type.tvSponsorship){
                sql.Append(SQLGenerator.getAdExpressUniverseCondition(CstWeb.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID, tData.Prefix, tData.Prefix, tData.Prefix, true));
            }
            else{
                sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, tData.Prefix, true, false));
            }
            /* This test is specific to the French version of the site
             * */
			string idCategoryThematic = TNS.AdExpress.Domain.Lists.GetIdList(CstWeb.GroupList.ID.category, CstWeb.GroupList.Type.thematicTv);
			if (idCategoryThematic != null && idCategoryThematic.Length > 0) {
				sql.AppendFormat("  and  {0}.id_category not in ( {1}) ", tData.Prefix, idCategoryThematic);				
			}
            #endregion

            #region Banners Format Filter
            // TODO ADD RIGHT MANAGEMENT FOR BANNERS FORMAT
            // Add Banners Format Filter
            if (WebApplicationParameters.UseBannersFormatFilter && _module.Id == CstWeb.Module.Name.NEW_CREATIVES) { 
                if(_session.SelectedBannersForamtList.Length>0)
                    sql.Append(" and " + tData.Prefix + ".ID_FORMAT_BANNERS in (" + _session.SelectedBannersForamtList + ") ");
            }
            #endregion

            #region Filtres
            /* Get filter clause and check zero version
             * According to the module type, we save the classification levels in GenericProductDetailLevel or in GenericMediaDetailLevel
             * GenericProductDetailLevel : product classification levels
             * GenericMediaDetailLevel : media classification levels
             * */
            if (filters != null && filters.Length > 0) {
                GenericDetailLevel detailLevels = null;
                switch (_module.Id) {
                    case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                    case CstWeb.Module.Name.NEW_CREATIVES:
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
            if (_session.SloganIdZoom > -1) {
                sql.AppendFormat(" and wp.id_slogan={0}", _session.SloganIdZoom);
            }
            if ((_msCreaConfig || _creaConfig) && vehicle.Id != CstDBClassif.Vehicles.names.adnettrack && vehicle.Id != CstDBClassif.Vehicles.names.internet && vehicle.Id != CstDBClassif.Vehicles.names.evaliantMobile) {
                sql.AppendFormat(" and {0}.id_slogan is not null", tData.Prefix);
            }

            /* Version selection
             * */
            string slogans = _session.SloganIdList;

            /* Refine vesions
             * */
            if (slogans.Length > 0) {
                sql.AppendFormat(" and {0}.id_slogan in({1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, slogans);
            }
            #endregion

            /* For the media type press and international press we can do studies according to the inset option (total, inset excluding, inset)
             * so to get the corresponding filter we use GetJointForInsertDetail method
             * */
            if (vehicle.Id == CstDBClassif.Vehicles.names.press || vehicle.Id == CstDBClassif.Vehicles.names.internationalPress || vehicle.Id == CstDBClassif.Vehicles.names.newspaper
                || vehicle.Id == CstDBClassif.Vehicles.names.magazine)
            {
                sql.Append(FctWeb.SQLGenerator.GetJointForInsertDetail(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            }

            switch (vehicle.Id) {
                case CstDBClassif.Vehicles.names.internet:
                    sql.AppendFormat(" and {1}.id_vehicle={0} ", VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack).DatabaseId, tData.Prefix);
                    break;
                default:
                    sql.AppendFormat(" and {1}.id_vehicle={0} ", vehicle.DatabaseId, tData.Prefix);
                    break;
            }
        }
        #endregion

        #region AppendSqlGroupByFields
        /// <summary>
        /// Append Group by clause
        /// The list of fields used in the group by clause will be genrated as following :
        ///     + Fields that correspond to the detail levels : Level1_Id, Level1_Label, Level2_Id, Level2_Label, Level3_Id, Level3_Label.
        ///       we can get this fields by using : _session.DetailLevel.GetSqlGroupByFields()
        ///     + Fields that correspond to the detail columns, we can get the columns list by using GenericColumns.GetSqlGroupByFields(columns, detailLevelIds)
        ///     + Specific fields : for some column we not only need the field corresponding to the column but we need others fields related to the column
        ///       for example for the visual column we need disponibility_visual and activation fields. We can get this by using GenericColumns.GetSqlConstraintGroupByFields(columns)
        /// </summary>
        /// <param name="tData">Data Table Description</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="detailLevelIds">List of levels detail</param>
        /// <param name="columns">Classification columns list</param>
        protected virtual void AppendSqlGroupByFields(StringBuilder sql, Table tData, VehicleInformation vehicle, ArrayList detailLevelIds, List<GenericColumnItemInformation> columns)
        {

            string tmp = string.Empty;
            bool first = true;

            sql.Append(" group by");

            /* Get SQL group by fields for the classification levels selected by the customer
             * */
            if (!_msCreaConfig && _session.DetailLevel != null) {
                tmp = _session.DetailLevel.GetSqlGroupByFields();
                if (tmp.Length > 0) {
                    sql.AppendFormat(" {0}", tmp);
                    first = false;
                }
            }

            /* Get SQL group by fields for the classification columns defined in the Xml configuration file
             * */
            tmp = GenericColumns.GetSqlGroupByFields(columns, detailLevelIds);
            if (tmp.Length > 0) {
                if (!first) sql.Append(",");
                sql.AppendFormat(" {0}", tmp);
                first = false;
            }

            /* Get SQL group by fields for classification column that need others columns information
             * */
            if (columns != null) {
                tmp = GenericColumns.GetSqlConstraintGroupByFields(columns);
                if (tmp != null && tmp.Length > 0) {
                    if (!first) sql.Append(",");
                    first = false;
                    sql.AppendFormat(" {0}", tmp);
                }
            }

            if (!_msCreaConfig && _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SLOGAN_ACCESS_FLAG)
                && vehicle.Id == CstDBClassif.Vehicles.names.radio
                && !_session.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.slogan)
                ) {
                sql.AppendFormat(", {0}.id_slogan ", tData.Prefix);
            }

            if (!_msCreaConfig && !first && vehicle.Id == CstDBClassif.Vehicles.names.tv && !_session.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.category)) {
                sql.AppendFormat(" , {0}.id_category", tData.Prefix);
            }

        }
        #endregion

        #region AppendSqlOrderFields
        /// <summary>
        /// Append request "Order by" clause
        /// The list of fields used in the order by clause will be genrated as following :
        ///     + Fields that correspond to the detail levels : Level1_Id, Level1_Label, Level2_Id, Level2_Label, Level3_Id, Level3_Label.
        ///       we can get this fields by using : _session.DetailLevel.GetSqlOrderFields()
        ///     + Fields that correspond to the detail columns, we can get the columns list by using GenericColumns.GetSqlOrderFields(columns, detailLevelIds)
        ///     + Specific fields : for some column we not only need the field corresponding to the column but we need others fields related to the column
        ///       for example for the visual column we need disponibility_visual and activation fields. We can get this by using GenericColumns.GetSqlConstraintOrderFields(columns)
        /// </summary>
        /// <param name="tData">Data Table Description</param>
        /// <param name="sql">Sql request container</param>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="detailLevelIds">List of detail levels</param>
        /// <param name="columns">Classification columns list</param>
        protected virtual void AppendSqlOrderFields(Table tData, StringBuilder sql, VehicleInformation vehicle, ArrayList detailLevelIds, List<GenericColumnItemInformation> columns)
        {
            string tmp = string.Empty;
            bool first = true;

            sql.Append(" order by ");

            /* Get SQL order by fields for the classification levels selected by the customer
             * */
            if (!_msCreaConfig && _session.DetailLevel != null) {
                tmp = _session.DetailLevel.GetSqlOrderFields();
                if (tmp.Length > 0) {
                    sql.AppendFormat(" {0}", tmp);
                    first = false;
                }
            }

            /* Get SQL order by fields for the classification columns defined in the Xml configuration file
             * */
            tmp = GenericColumns.GetSqlOrderFields(columns, detailLevelIds);
            if (tmp.Length > 0) {
                if (!first) sql.Append(",");
                sql.AppendFormat(" {0}", tmp);
                first = false;
            }

            /* Get SQL order by fields for classification column that need others columns information
             * */
            if (columns != null) {
                tmp = GenericColumns.GetSqlConstraintOrderFields(columns);
                if (tmp != null && tmp.Length > 0) {
                    if (!first)sql.Append(",");
                    first = false;
                    sql.AppendFormat(" {0}", tmp);
                }
            }

            if (!_msCreaConfig)
                AppendSloganField(sql, tData, vehicle,columns);

        }
        #endregion

        #endregion

    }
}
