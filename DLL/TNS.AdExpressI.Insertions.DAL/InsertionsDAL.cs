using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.Classification.Universe;
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
using Table = TNS.AdExpress.Domain.DataBaseDescription.Table;

namespace TNS.AdExpressI.Insertions.DAL
{
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
    public abstract class InsertionsDAL : IInsertionsDAL
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
        public InsertionsDAL(WebSession session, Int64 moduleId)
        {
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
        public virtual Int64[] GetVehiclesIds(Dictionary<DetailLevelItemInformation, Int64> filters)
        {

            Int64[] ids = null;
            var sql = new StringBuilder();

            /* Get the description of the media type table
             * The description contains : Table identifier, table name, table prefix, database schema identifier, database schema
             * */
            Table tVehicle = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);

            /* Get the description of the all media view
             * The description contains : View identifier, view name, view prefix, database schema identifier, database schema
             * This view contains (id_vehicle(media type), id_media(vehicle), id_interest_center, id_basic_media, id_title ...) 
             * */
            TNS.AdExpress.Domain.DataBaseDescription.View vMedia =
                WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMedia);

            /* SQL request building
             * */
            sql.AppendFormat("select distinct {0}.id_vehicle from {1}{2} {0} where 1=1 ", vMedia.Prefix, vMedia.Sql,
                             _session.DataLanguage);

            /* In the view we have only vehicle classification fields so we can only filter by vehicle classification levels like below
             * */
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
                /* Getting the data
                 * */
                DataSet ds = _session.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null)
                {
                    DataTable dt = ds.Tables[0];
                    ids = new Int64[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
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
        public virtual List<VehicleInformation> GetPresentVehicles(List<VehicleInformation> vehicles, string filters
                                                                   , int fromDate, int toDate, int universId,
                                                                   TNS.AdExpress.Domain.Web.Navigation.Module module,
                                                                   bool sloganNotNull)
        {

            var sql = new StringBuilder();
            /* List of media type for which we have data in the database
             * The vehicleInformation object contains informations about a media type like : 
             *  databaseId : dataBase identifier, 
             *  showInsertions : if we can show insertions for this media type
             *  showCreations : if we can show versions for this media type
             *  allowedUnitsList : allowed units list
             *  detailColumnId : detail columns id 
             *  ...
             * */
            var found = new List<VehicleInformation>();
            DataSet ds = null;
            /* For the version which don't have TODO explication of version 0
             * so when sloganNotNull is true, it means that we're treating versions informations and not insertions informations
             * */
            this._creaConfig = sloganNotNull;

            try
            {

                bool first = true;

                universId--;
                Table dataTable;
                /* Getting the database schema
                 * */
                Schema sAdEx = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);

                /* For each meida type we check if we have data in data base
                 * */
                foreach (VehicleInformation v in vehicles)
                {

                    /* Get table for the current media type and according to the current module
                     * */
                    if (_module.ModuleType == CstWeb.Module.Type.tvSponsorship)
                        dataTable = GetDataTable(v, CstWeb.Module.Type.tvSponsorship);
                    else if (Dates.Is4M(fromDate))
                        dataTable = GetDataTable(v, CstWeb.Module.Type.alert);
                    else
                        dataTable = GetDataTable(v, CstWeb.Module.Type.analysis);

                    if (!first)
                        sql.Append(" UNION ");
                    else
                        first = false;

                    /* This test conerns only the French version of the site
                     * */
                    if (v.Id != Vehicles.names.internet &&
                        v.Id != Vehicles.names.mailValo)
                    {
                        sql.Append(" select id_vehicle from ");
                    }
                    else
                    {
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
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    /* initialisation of the foud list with media type for which we have foud data in the database
                     * */
                    found.AddRange(
                        ds.Tables[0].Rows.Cast<DataRow>()
                                    .Select(row => VehiclesInformation.Get(Convert.ToInt64(row["id_vehicle"]))));
                }
            }
            catch (System.Exception err)
            {
                throw new InsertionsDALException(
                    string.Format("GetData::Unable to check there is data. {0}", sql.ToString()), err);
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
            if (vehicle.Id != Vehicles.names.internet && vehicle.Id != Vehicles.names.mailValo)
            {
                /* Get the data table description object according to the media type (vehicle) and the module type (moduleType)
                 * */
                return SQLGenerator.GetDataTable(vehicle, moduleType, _session.IsSelectRetailerDisplay);
            }
            else
            {

                switch (moduleType)
                {
                    case CstWeb.Module.Type.alert:
                        return (vehicle.Id == Vehicles.names.internet)
                                   ? WebApplicationParameters.GetDataTable(TableIds.dataInternetVersionAlert,
                                                                           _session.IsSelectRetailerDisplay)
                                   : WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirectAlert,
                                                                           _session.IsSelectRetailerDisplay);
                    case CstWeb.Module.Type.analysis:
                        return (vehicle.Id == Vehicles.names.internet)
                                   ? WebApplicationParameters.GetDataTable(TableIds.dataInternetVersion,
                                                                           _session.IsSelectRetailerDisplay)
                                   : WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirect,
                                                                           _session.IsSelectRetailerDisplay);
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
        protected virtual string GetFiltersClause(Table table, GenericDetailLevel detailLevels, string filters,
                                                  VehicleInformation vehicle)
        {

            var str = new StringBuilder();
            /* Get the classification levels values
             * */
            string[] ids = filters.Split(',');

            DetailLevelItemInformation level = null;
            Int64 id = 0;
            /*  For each detail level (classification level), we add a filter in our SQL query
             * */
            for (int i = 0; i < ids.Length && i < detailLevels.Levels.Count; i++)
            {
                id = Convert.ToInt64(ids[i]);
                level = (DetailLevelItemInformation)detailLevels.Levels[i];
                if (id > 0
                    || (id == 0 && level.Id != DetailLevelItemInformation.Levels.slogan)
                    || (id > long.MinValue && id != 0 && level.Id == DetailLevelItemInformation.Levels.slogan
                        && (vehicle.Id == Vehicles.names.adnettrack || vehicle.Id == Vehicles.names.internet
                            || vehicle.Id == Vehicles.names.evaliantMobile
                           )
                       )
                    )
                {
                    str.Append(AppendSelectedLevelFilters(level, vehicle, id, table));
                }
                /* When we have solgan = 0, we add the following line
                 * */
                if (id == 0 && level.Id == DetailLevelItemInformation.Levels.slogan &&
                    vehicle.Id != Vehicles.names.adnettrack && vehicle.Id != Vehicles.names.internet
                    && vehicle.Id != Vehicles.names.evaliantMobile)
                {
                    str.AppendFormat(" and {2}.{0} = {1}", level.DataBaseIdField, id, table.Prefix);
                }
            }

            return str.ToString();
        }

        protected virtual string AppendSelectedLevelFilters(DetailLevelItemInformation level, VehicleInformation vehicle
                                                            , Int64 id, Table table)
        {
            var str = new StringBuilder();


            /* This test concerns only the French version of the site
                    * */
            if (level.Id == DetailLevelItemInformation.Levels.vehicle //.DataBaseIdField == CstDB.Fields.ID_VEHICLE
                && VehiclesInformation.Contains(Vehicles.names.internet)
                && id == VehiclesInformation.EnumToDatabaseId(Vehicles.names.internet))
                id = VehiclesInformation.Get(Vehicles.names.adnettrack).DatabaseId;
            /* Add a filter to the SQL query, for the following media type : internet, adnettrack or evaliantMobile we use "hashcode" instead of the identifier,
             * this is a specificity of the French database
             * */
            string classifIds = id.ToString();

            //Swithc 
            if (vehicle.Id == Vehicles.names.mailValo)
                classifIds = ReplaceMailValoByVmc(level, id);

            if (level.Id == DetailLevelItemInformation.Levels.slogan &&
                (vehicle.Id == Vehicles.names.adnettrack
                 || vehicle.Id == Vehicles.names.internet
                 || vehicle.Id == Vehicles.names.evaliantMobile))
            {
                str.AppendFormat(" and {1}.hashcode = {0}"
                                 , classifIds, table.Prefix);
            }
            else
            {
                str.AppendFormat(" and {2}.{0} in ({1})"
                                 , level.DataBaseIdField, classifIds, table.Prefix);
            }

            return str.ToString();
        }

        protected string ReplaceMailValoByVmc(DetailLevelItemInformation level, long id)
        {
            string ids = id.ToString();
            switch (level.Id)
            {
                case DetailLevelItemInformation.Levels.vehicle:
                    if (VehiclesInformation.Contains(Vehicles.names.mailValo)
                        && id == VehiclesInformation.EnumToDatabaseId(Vehicles.names.mailValo))
                        ids = VehiclesInformation.Get(Vehicles.names.directMarketing).DatabaseId.ToString();
                    break;

                case DetailLevelItemInformation.Levels.category:
                    ids = SwitchToCategoryVmc(id);
                    break;

                case DetailLevelItemInformation.Levels.media:
                    ids = SwitchToMediaVmc(id);
                    break;
            }

            return ids;
        }

        protected Func<string, long, List<long>, string> replaceId = (idList, oldId, newIds) =>
        {
            string ids = idList;
            var list = new List<long>(idList.Split(',').Select(p => Convert.ToInt64(p)));
            int index = list.FindIndex(p => p == oldId);
            if (index >= 0)
            {
                list.RemoveAt(index);
                list.AddRange(newIds);
                ids = string.Join(",", list.ToArray());
            }
            return ids;
        };


        protected string ReplaceMailValoByVmc(long levelId, string idList)
        {
            string ids = idList;


            switch (levelId)
            {
                case TNSClassificationLevels.VEHICLE:
                    ids = replaceId(idList, VehiclesInformation.Get(Vehicles.names.mailValo).DatabaseId
                                    , new List<long> { VehiclesInformation.Get(Vehicles.names.directMarketing).DatabaseId });

                    break;
                case TNSClassificationLevels.CATEGORY:
                    ids = replaceId(idList, CstDB.Category.COURRIER_ADRESSE_VALO
                                       , new List<long> { Convert.ToInt64(CstDB.Category.COURRIER_ADRESSE) });
                    ids = replaceId(ids, CstDB.Category.IMPRIME_PUBLICITAIRE_VALO
                                       , new List<long> { Convert.ToInt64(CstDB.Category.PUBLICITE_NON_ADRESSEE) });
                    break;

                case TNSClassificationLevels.MEDIA:
                    ids = replaceId(idList, CstDB.Media.COURRIER_ADRESSE_VALO
                                      , new List<long> { Convert.ToInt64(CstDB.Media.COURRIER_ADRESSE_GENERAL)
                  , Convert.ToInt64(CstDB.Media.COURRIER_ADRESSE_GESTION), Convert.ToInt64(CstDB.Media.COURRIER_ADRESSE_PRESSE)});

                    ids = replaceId(ids, CstDB.Media.IMPRIME_PUBLICITAIRE_VALO
                                            , new List<long> { Convert.ToInt64(CstDB.Media.PUBLICITE_NON_ADRESSEE) });
                    break;
            }
            return ids;
        }
        protected string ReplaceMailValoByVmc(CstCustomer.Right.type levelId, string idList)
        {
            string ids = idList;

            switch (levelId)
            {
                case CstCustomer.Right.type.vehicleAccess:
                case CstCustomer.Right.type.vehicleException:
                    return ReplaceMailValoByVmc(TNSClassificationLevels.VEHICLE, idList);
                case CstCustomer.Right.type.categoryAccess:
                case CstCustomer.Right.type.categoryException:
                    return ReplaceMailValoByVmc(TNSClassificationLevels.CATEGORY, idList);
                case CstCustomer.Right.type.mediaAccess:
                case CstCustomer.Right.type.mediaException:
                    return ReplaceMailValoByVmc(TNSClassificationLevels.MEDIA, idList);
            }
            return ids;
        }


        protected string SwitchToCategoryVmc(long id)
        {
            switch (id)
            {
                case CstDB.Category.COURRIER_ADRESSE_VALO:
                    return string.Format("{0}",
                                         CstDB.Category.COURRIER_ADRESSE);

                case CstDB.Category.IMPRIME_PUBLICITAIRE_VALO:
                    return string.Format("{0}", CstDB.Category.PUBLICITE_NON_ADRESSEE);
                default:
                    throw new InsertionsDALException(" Impossible to identify Mail Category");
            }
        }
        private string SwitchToMediaVmc(long id)
        {
            switch (id)
            {
                case CstDB.Media.COURRIER_ADRESSE_VALO:
                    return string.Format("{0},{1},{2}",
                                         CstDB.Media.COURRIER_ADRESSE_GENERAL, CstDB.Media.COURRIER_ADRESSE_PRESSE,
                                         CstDB.Media.COURRIER_ADRESSE_GESTION);

                case CstDB.Media.IMPRIME_PUBLICITAIRE_VALO:
                    return string.Format("{0}", CstDB.Media.PUBLICITE_NON_ADRESSEE);
                default:
                    throw new InsertionsDALException(" Impossible to identify Mail Vehicle");
            }
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
        protected virtual string CheckZeroVersion(Table table,
            TNS.AdExpress.Domain.Level.GenericDetailLevel detailLevels, VehicleInformation vehicle, string filters)
        {

            Int64 id = 0;
            string[] ids = filters.Split(',');
            /* Check if the slogan level is sontained in the detailLevels object (Object that contains a list of classification levels)
             * */
            int rank = detailLevels.GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels.slogan);
            /* If rank != 0 that means id_solgan is not null and we don't need to add the line below but if it's equal to 0, we have to added it
             * */
            if (rank != 0)
            {
                id = Convert.ToInt64(ids[rank - 1]);
                if (id == 0 && vehicle.Id != Vehicles.names.adnettrack &&
                    vehicle.Id != Vehicles.names.internet
                    && vehicle.Id != Vehicles.names.evaliantMobile)
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
        public virtual DataSet GetCreativesData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)
        {
            /* (True if we're using InsertionDAL to get informations about versions)
             * */
            _creaConfig = true;
            return GetData(vehicle, fromDate, toDate, universId, filters);
        }
        /// <summary>
        /// Extract advertising detail for creatives details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information (differents informations about a media type like databaseId, showInsertions..., this object is more detailed above)</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="columns">columns</param>
        /// <returns>Advertising detail Data</returns>		
        public virtual DataSet GetCreativesData(VehicleInformation vehicle, int fromDate, int toDate, List<GenericColumnItemInformation> columns)
        {
            /* (True if we're using InsertionDAL to get informations about versions)
             * */
            _creaConfig = true;
            return GetData(vehicle, fromDate, toDate, columns);


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


        #region CountCreativeData
        /// <summary>
        /// Count creative data
        /// </summary>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// Note that if the value of the level is long.MinValue  (-9223372036854775808), it's mean that the level was not selected.
        /// 
        /// <param name="columns">column list </param>
        /// <returns>Advertising detail Data</returns>		
        public virtual long CountCreativeData(VehicleInformation vehicle, int fromDate, int toDate, List<GenericColumnItemInformation> columns)
        {
            _creaConfig = true;
            var sql = new StringBuilder(5000);
            /* Get the table name for a specific vehicle and depending on the module type
             * Example : data_press, data_tv, data_radio ...
             * */
            Table tData;
            if (_module.ModuleType == CstWeb.Module.Type.tvSponsorship)
                tData = GetDataTable(vehicle, CstWeb.Module.Type.tvSponsorship);
            else if (Dates.Is4M(fromDate))
                tData = GetDataTable(vehicle, CstWeb.Module.Type.alert);
            else
                tData = GetDataTable(vehicle, CstWeb.Module.Type.analysis);
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

            try
            {

                // Select
                sql.Append(" select count(*) from (");
                sql.Append(" select distinct ");
                // Append data fields
                AppendInsertionsSqlFields(tData, vehicle, sql, columns);
                sql.Length -= 1;

                // Append data tables
                AppendSqlTables(sAdExpr03, tData, sql, columns);

                sql.Append(" Where ");

                // Append filters
                AppendUniversFilters(sql, tData, fromDate, toDate, vehicle);

                // Append Joins
                tmp = GenericColumns.GetSqlJoins(_session.DataLanguage, tData.Prefix, columns, null);
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" {0} ", tmp);
                }

                sql.AppendFormat(" {0} ", GenericColumns.GetSqlContraintJoins(columns));

                sql.Append(" )");
            }
            catch (System.Exception err)
            {
                throw new InsertionsDALException(string.Format("Unable to build request to count version number : {0}", sql.ToString()), err);
            }


            #region Execution de la requête
            try
            {

                DataSet ds = _session.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 1) return (Int64.Parse(ds.Tables[0].Rows[0][0].ToString()));
                throw new InsertionsDALException(string.Format("Unable to build request to count version number: {0}", sql.ToString()));
            }
            catch (System.Exception err)
            {
                throw new InsertionsDALException(string.Format("Unable to load data for insertions details: {0}", sql.ToString()), err);
            }
            #endregion

        }
        #endregion



        #region GetData
        /// <summary>
        /// Extract advertising detail for creatives or insertions details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// Note that if the value of the level is long.MinValue  (-9223372036854775808), it's mean that the level was not selected.
        /// 
        /// <param name="columns">column list </param>
        /// <returns>Advertising detail Data</returns>		
        protected virtual DataSet GetData(VehicleInformation vehicle, int fromDate, int toDate, List<GenericColumnItemInformation> columns)
        {

            var sql = new StringBuilder(5000);
            /* Get the table name for a specific vehicle and depending on the module type
             * Example : data_press, data_tv, data_radio ...
             * */
            Table tData;
            if (_module.ModuleType == CstWeb.Module.Type.tvSponsorship)
                tData = GetDataTable(vehicle, CstWeb.Module.Type.tvSponsorship);
            else if (Dates.Is4M(fromDate))
                tData = GetDataTable(vehicle, CstWeb.Module.Type.alert);
            else
                tData = GetDataTable(vehicle, CstWeb.Module.Type.analysis);
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

            try
            {

                // Select
                sql.Append(" select distinct ");
                // Append data fields
                AppendInsertionsSqlFields(tData, vehicle, sql, columns);
                sql.Length -= 1;

                // Append data tables
                AppendSqlTables(sAdExpr03, tData, sql, columns);

                sql.Append(" Where ");

                // Append filters
                AppendUniversFilters(sql, tData, fromDate, toDate, vehicle);

                // Append Joins
                tmp = GenericColumns.GetSqlJoins(_session.DataLanguage, tData.Prefix, columns, null);
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" {0} ", tmp);
                }

                sql.AppendFormat(" {0} ", GenericColumns.GetSqlContraintJoins(columns));

                // Append Group by
                AppendSqlGroupByFields(sql, tData, vehicle, columns);

                // Append Order by 
                AppendSqlOrderFields(tData, sql, vehicle, columns);

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
            catch (Exception err)
            {
                throw new InsertionsDALException(string.Format("Unable to load data for insertions details: {0}", sql.ToString()), err);
            }
            #endregion

        }

        /// <summary>
        /// Extract advertising detail for creatives or insertions details 
        /// </summary>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="universId">The univers identifier used for the study, in the Present/Absent module it equal to -1 because we don't need this information</param>
        /// <param name="filters">Each parameter of the filters list represents a value for a classification level, the first one correspond to the Level1 the seconed to Level2
        /// Note that if the value of the level is long.MinValue  (-9223372036854775808), it's mean that the level was not selected.
        /// </param>
        /// <returns>Advertising detail Data</returns>		
        protected virtual DataSet GetData(VehicleInformation vehicle, int fromDate, int toDate, int universId, string filters)
        {

            var sql = new StringBuilder(5000);
            var detailLevelsIds = new ArrayList();
            /* Get the table name for a specific vehicle and depending on the module type
             * Example : data_press, data_tv, data_radio ...
             * */
            Table tData;
            if (_module.ModuleType == CstWeb.Module.Type.tvSponsorship)
                tData = GetDataTable(vehicle, CstWeb.Module.Type.tvSponsorship);
            else if (Dates.Is4M(fromDate))
                tData = GetDataTable(vehicle, CstWeb.Module.Type.alert);
            else
                tData = GetDataTable(vehicle, CstWeb.Module.Type.analysis);
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
            else if (_creaConfig)
                columns = WebApplicationParameters.CreativesDetail.GetDetailColumns(vehicle.DatabaseId, _module.Id);
            else
                columns = WebApplicationParameters.InsertionsDetail.GetDetailColumns(vehicle.DatabaseId, _module.Id);

            try
            {

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
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" {0} ", tmp);
                }
                if (!_msCreaConfig && _session.DetailLevel != null)
                {
                    sql.AppendFormat(" {0} ", _session.DetailLevel.GetSqlJoins(_session.DataLanguage, tData.Prefix));
                }
                sql.AppendFormat(" {0} ", GenericColumns.GetSqlContraintJoins(columns));

                // Append Group by
                AppendSqlGroupByFields(sql, tData, vehicle, detailLevelsIds, columns);

                // Append Order by 
                AppendSqlOrderFields(tData, sql, vehicle, detailLevelsIds, columns);

            }
            catch (Exception err)
            {
                throw new InsertionsDALException(string.Format("Unable to build request to get insertions details. {0}", sql.ToString()), err);
            }


            #region Execution de la requête

            try
            {

//                string sql2 = @"
                    
                    
//select  distinct ct.id_category,ct.category,md.id_media,md.media,wp.date_media_num as date_num,wp.date_media_num as date_media_num, wp.id_advertisement,to_char(adexpr03.stragg2(distinct visual)) as visual,media_paging,ad.id_advertiser,ad.advertiser,gr.id_group_,gr.group_,pr.id_product,pr.product,nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan,ic.id_interest_center,ic.interest_center,ms.id_media_seller,ms.media_seller,fo.id_format,fo.format,area_page,cl.id_color,cl.color,rank_media,expenditure_euro,to_char(adexpr03.stragg2(distinct location)) as location,wp.date_cover_num, appliMd.disponibility_visual,appliMd.activation,wp.id_advertisement from  adexpr03.data_press_4M wp , adexpr03.category ct,adexpr03.media md , adexpr03.advertiser ad,adexpr03.group_ gr,adexpr03.product pr,adexpr03.interest_center ic,adexpr03.media_seller ms,adexpr03.format fo,adexpr03.color cl,adexpr03.location lo , adexpr03.application_media  appliMd,adexpr03.data_location  dl  Where  wp.date_media_num >= 20160201 and wp.date_media_num <= 20160301 and (wp.id_vehicle in (1,2,3))  and ((( wp.id_vehicle  in  (9,13,20,4,19,10,15,1,8,18,12,5) )  or ( wp.id_category  in  (221,38,21,24,27,37,31,61,311,39,35,68,78,301,261,44) )  or ( wp.id_media  in  (10066,23155,14681,14746,10123,10606,23587,13781,10065,15990,11443,11596,10553,13708,10253,10945,10510,19904,14751,10961,10633,18260,11047,10252,10177,14811,10645,18301,10534,20042,19843,11564,10067,11191,20340,11076,10564,11540,11805,11548,10068,10579,20883,11268,10768,20190,20511,14477,10536,10620,10537,10883,11067,11068,10044,10070,10602,10538,10539,11485,11362,11542,13176,18060,11179,14755,11181,10661,10445,10680,20004,11645,10155,11619,19968,17130,10072,16720,14580,10610,15046,11513,19852,15071,14752,23506,11326,11072,10963,10847,16673,10468,10936,11243,10581,14473,10165,10551,11582,11613,10159,10980,12113,23076,13178,10800,16580,10597,10926,11053,10098,11523,10663,10569,10512,10570,18110,10073,19830,20404,12386,11706,11236,11208,10907,10513,10106,10438,10565,10514,11642,13418,20420,16750,14471,11631,23651,11771,23671,10786,10515,14595,19999,10617,10508,10887,10157,11363,18641,10788,14745,11632,20151,11901,20346,11507,20260,11821,15880,10594,10317,20333,15733,11982,19847,15549,10076,19918,10790,10026,11069,10522,20896,20312,12273,10846,24657,11791,10038,11626,11250,15073,11377,11444,10524,11597,10913,20338,10021,10892,20331,10600,17132,11428,11139,11458,10914,10529,11497,10915,14780,11264,11276,13761,10918,10836,10712,14880,11180,10178,10972,11445,11514,20512,11473,11532,10251,10981,15364,16880,11342,10631,23073,10895,11046,19000,10692,23175,19221,20290,11144,9439,6246,14889,8176,6280,7842,1596,1316,1291,1783,1353,1293,16919,6124,4283,4578,9828,17379,17382,17386,17380,20865,17383,17384,8633,24634,24633,1318,24960,4479,7354,18139,1979,4639,1446,1215,1086,1449,6191,6261,8068,8035,8036,12221,14010,1069,3617,1826,1348,24328,1857,20425,9328,1771,1458,3140,3139,5252,1477,13087,25365,1320,1462,18559,9725,9438,8069,8033,8034,8077,8076,8058,24444,24445,24446,24412,24413,24414,24409,24410,24411,19214,19212,19213,19211,19209,19210,24785,24441,24442,24443,24421,24422,24423,24424,24425,24465,8110,1323,1496,5653,1266,5355,5362,1341,6868,1636,12201,4171,1576,8660,8654,9992,14791,19609,3125,18729,1002,9367,24947,12263,1270,5504,6219,1609,6062,6902,1392,1934,24904,1003,24907,5144,24905,1005,1949,24906,1624,1625,1849,1331,8630,5007,24418,24419,24420,24415,24416,24417,8070,18689,8008,8015,24784,24426,24427,24428,24429,24430,24431,6461,1364,4772,1464,5413,5628,13107,13109,13108,1747,13110,8945,5246,18919,7451,19539,7591,5728,3168,14019,3120,3123,3130,3133,3132,3129,3190,3124,6492,15239,14069,1762,4770,7975,9610,6914,6459,8390,8906,1740,12661,7981,3613,8403,5702,2637,13237,3188,1951,9326,7519,7230,8073,8067,8074,7603,18859,15719,6776,9891,7827,1376,8026,24447,24448,24449,24438,24439,8064,8063,24440,17892,8027,24711,24700,17891,24432,24433,24434,24435,24436,24437,9460,6532,8627,15849,5501,1344,7372,1180,1365,1687,2715,4182,2759,7576,20469,6812,1903,6604,20429,16849,6342,7954,7368,1630,1588,9260,6482)  or  wp.id_media  in  (1359,5764,1455,6384,8828,7224,7829,13088,19780,17798,1922,1308,5538,6212,1961,1313,1411,14699,20691,19449,2280,15939,9178,15940,9733,6159,1919,9346,19699,11212,15410,11991,11174,25408,24354,25224,19887,17385,17381,17387,17388,8224,5045,7266,1761,12223,1297,1361,6935,6057,9544,4192,16800,17599,9269,16802,9270,16801,16799,6937,2496,7056,12222,1639,1490,5592,1492,1494,4502,3161,5758,7301,8393,9262,1197,1300,1301,9364,19129,19139,3126,3127,5505,25320,1303,1859,12265,13404,1594,19809,1670,4234,24613,1306,1370,1373,5713,4191,5262,9133,1504,1374,24329,1894,1506,2491,6267,4169,6675,6672,1821,15269,1380,1626,1967,9985,6036,9404,1605,17619,7400,1044,8902,6429,1869,9327,9103,9109,2777,6501,1977,1387,1592,6344,1388,9442,6293,1390,1391,24959,4260,24520,24836,13407,1525,1724,2495,1395,5120,6069,6169,1399,1614,6695,1962,9510,1888,1400,6400,24377,1402,7414,6123,3110,3157,5304,3194,6415,6528,13784,19130,19131,9679,6153,6846,1864,19929,1733,4500,7960,8938,8939,13089,18359,18360,18361,18362,20719,20720,10441,10627,14600,10682,20632,17000,20893,11659,10628,11056,23167,10849,17361,14001,11229,20142,10029,10842,10642,10443,10030,24350,14830,11078,11772,15341,17583,10000,11471,10601,11579,11200,10108,11129,10950,10255,11105,19856,20339,20872,15901,15610,10567,11233,10978,11432,20350,11914,20410,10031,11366,10449,14430,10615,11338,10930,11422,10634,10928,11392,14394,20349,20301,10621,20220,10595,11581,20163,20143,11572,20144,11077,24929,14812,15017,11405,11091,11516,14262,11198,11609,11107,11011,11490,20040,11087,11086,19896,10006,10451,14740,10008,10009,20332,20152,20503,11197,20348,10011,10920,10599,10036,10959,10557,10616,11974,10452,10558,10637,20211,11070,19671,11130,10708,16670,11084,11140,17420,11209,19969,10643,18762,18760,18502,23647,19913,10853,19907,17461,10625,20886,10034,15018,12603,10855,11272,10612,11634,11316,19914,11045,10951,14748,10970,10666,10546,10743,11196,10035,15206,16351,11188,10619,18302,17195,10173,20504,20265,11295,11004,16090,20162,10086,20268,20374,11595,11517,14390,11467,10670,11550,10718,20500,10429,10179,10973,18040,10012,11094,10163,11166,11729,11463,10965,11271,18850,11649,20631,10583,10079,11270,11413,10857,10087,17423,11429,11646,14756,10037,10843,10859,16170,15830,20398,11195,10984,10577,11121,23589,23759,11132,11080,10083,17570,11207,10923,10861,10458,10844,20164,10421,12142,16962,23669,10582,20149,11430,10764,10609,20264,11656,18400,19960,13621,11537,10943,10088,10419,10459,11096,10696,11083,10460,11108,10417,13658,11615,11055,11644,10904,11273,11079,20424,10039,10862,10584,10462,20082,10976,10576,20081,10519,20092,11407,10463,11120,12172,11419,11640,11253,12256,10466,11337,13530,20305,11808,23762,10629,11117,11118,10732,11116,16860,10575,10089,14398,10090,11062,11041,14672,18022,10116,19886,10953,19992,10954,10042,11653,20887,10905,11194,10598,10611,20167,11216,11545,20210,14662,12064,10890,10139)  or  wp.id_media  in  (10908,11269,11293,23493,23168,10585,24655,11781,10154,13668,19901,20373,10678,11103,23977,17362,11915,10472,23170,18250,11249,11008,11311,19898,10605,20311,10119,10403,23110,10671,10944,11213,10476,10402,11518,11074,11034,11424,11093,11251,10015,10740,10523,18492,11630,15890,19674,24841,11217,10899,20878,20329,11205,19902,10046,10211,11100,20880,10016,10967,11535,11315,11090,11227,16152,10639,10080,11266,13888,11442,10479,10480,11735,20891,10772,10446,10209,10091,10092,20322,23151,24483,10481,10017,10093,23591,18501,10482,20402,19894,10949,10644,19963,11981,19993,10483,11505,20168,13548,19964,12175,10018,14120,10095,20192,17550,11506,10161,20266,11977,11082,10455,17443,16200,16157,20209,10587,17250,11625,19895,10672,18490,23649,10096,11431,10528,10747,10485,10097,10486,10487,20882,10748,10254,10019,10665,24509,10050,11171,10160,10667,11185,10099,11328,10588,20240,10020,13039,19844,10051,15416,11654,11317,10382,12042,13229,10100,10561,14170,10110,15840,10547,13738,20710,10664,10868,20150,10589,14840,10934,14482,14483,14479,10081,10640,11182,10369,12257,11980,10136,11192,10492,10493,11319,10101,11109,11049,11050,24653,11044,11157,20508,17822,19888,10135,10751,14404,13903,10053,10752,10494,10871,20204,20234,20202,20233,12742,20203,20232,20422,24507,11330,14589,10495,10004,19912,13892,13627,20330,20308,11051,11339,10496,15170,11060,11636,11126,10082,23776,20347,11283,10641,10054,10498,11113,10754,10632,16487,11667,11345,16610,16677,12022,10563,11199,11379,10239,10052,11420,11092,10501,10679,11193,10502,10102,10874,11073,23242,10638,10875,20395,10758,10823,10503,13550,10059,11164,14236,10669,16674,10128,11058,10504,10555,19917,19010,11383,10957,10103,11240,10061,10909,10242,19970,11057,10104,20873,10760,10674,10507,13881,10439,17552,11669,20390,23659,10063,11048,11574,11702,20892,10676,10541,11465,11414,24463,24464,24462,24460,1517) )  ) and ( wp.id_media  not in  (22610) )  ) And wp.id_product  not in(50000,50001,180000)  and wp.id_vehicle in (1) and wp.id_category in (11) and wp.id_vehicle=1   and ad.id_advertiser=wp.id_advertiser and ad.id_language=33 and ad.activation<50 and gr.id_group_=wp.id_group_ and gr.id_language=33 and gr.activation<50 and pr.id_product=wp.id_product and pr.id_language=33 and pr.activation<50 and ic.id_interest_center=wp.id_interest_center and ic.id_language=33 and ic.activation<50 and ms.id_media_seller=wp.id_media_seller and ms.id_language=33 and ms.activation<50 and fo.id_format(+)=wp.id_format and fo.id_language(+)=33 and fo.activation(+)<50 and cl.id_color(+)=wp.id_color and cl.id_language(+)=33 and cl.activation(+)<50 and lo.id_location(+)=dl.id_location and lo.id_language(+)=33 and lo.activation(+)<50   and ct.id_category=wp.id_category and ct.id_language=33 and ct.activation<50 and md.id_media=wp.id_media and md.id_language=33 and md.activation<50   and appliMd.id_media(+)=wp.id_media and appliMd.date_debut(+)=wp.date_media_num and appliMd.id_project(+)=1 and appliMd.activation(+)<50 and dl.id_advertisement(+)=wp.id_advertisement and dl.id_media(+)=wp.id_media and dl.date_media_num(+)=wp.date_media_num and dl.activation(+)<50  

//and rownum < 200

//group by ct.id_category,ct.category,md.id_media,md.media,wp.date_media_num,wp.date_media_num, wp.id_advertisement,media_paging,ad.id_advertiser,ad.advertiser,gr.id_group_,gr.group_,pr.id_product,pr.product,id_slogan,id_slogan,ic.id_interest_center,ic.interest_center,ms.id_media_seller,ms.media_seller,fo.id_format,fo.format,area_page,cl.id_color,cl.color,rank_media,expenditure_euro,wp.date_cover_num, appliMd.disponibility_visual,appliMd.activation,wp.id_advertisement order by  ct.category,ct.id_category,md.media,md.id_media,date_media_num,date_num, wp.id_advertisement,media_paging,ad.advertiser,ad.id_advertiser,gr.group_,gr.id_group_,pr.product,pr.id_product,slogan,id_slogan,ic.interest_center,ic.id_interest_center,ms.media_seller,ms.id_media_seller,fo.format,fo.id_format,area_page,cl.color,cl.id_color,rank_media,expenditure_euro,wp.date_cover_num, wp.id_advertisement


                    
//                    ";

                return _session.Source.Fill(sql.ToString());
            }
            catch (Exception err)
            {
                throw new InsertionsDALException(string.Format("Unable to load data for insertions details: {0}", sql.ToString()), err);
            }
            #endregion

        }
        #endregion

        #region Get version  detail

        /// <summary>
        /// Get one verion informations like :
        /// ID version , product label,group label,advertiser label
        /// </summary>
        /// <remarks>Use only for media type Tv in France</remarks>
        /// <param name="idVersion">ID version</param> 
        /// <param name="idVehicle">ID Media type</param>
        /// <returns>ID version , product label,group label,advertiser label</returns>
        public virtual DataSet GetVersion(string idVersion, long idVehicle)
        {
            var sql = new StringBuilder(1000);
            Schema sAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
            //Get table version
            Table sloganTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.slogan);
            //Get View
            TNS.AdExpress.Domain.DataBaseDescription.View oView =
                WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allProduct);

            try
            {
                //SELECT id version , product label, group label, advertiser label
                sql.AppendFormat("  SELECT id_slogan,{0}.product,{0}.group_,{0}.advertiser ", oView.Prefix);
                //FROM
                sql.AppendFormat("  FROM {0} , {1}{2} {3}", sloganTable.SqlWithPrefix, oView.Sql, _session.DataLanguage, oView.Prefix);
                //WHERE
                sql.AppendFormat("  WHERE {1}.id_slogan={0}", idVersion, sloganTable.Prefix);//Filtering with version ID
                sql.AppendFormat("  and {0}.id_product = {1}.id_product ", sloganTable.Prefix, oView.Prefix);//Joins

                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception exc)
            {
                throw new InsertionsDALException(" Error impossible to obtain version data", exc);
            }

        }

        /// <summary>
        /// Get version(s) data. The query will return the following fields, in order :
        /// "id_advertiser" : advertiser identifier.
        /// "advertiser" : advertiser label.
        /// "id_product" : product identifier.
        /// "product" : product label.
        /// "id_vehicle" : media type identifier.
        /// "vehicle" :  media type label.
        /// "id_slogan" : version identifier.
        /// "date_media_num" : publication date with format YYYYMMDD.
        /// "id_media" : vehicle identifier.
        /// "advertDimension" : It will be the duration of spot for media type RADIO  and TV. The FORMAT of Ad for media type PRESS. The type of board for media type Outdoor.
        /// "associated_file" : creative file.
        /// </summary>
        /// <param name="beginningDate">date beginning (YYYYMMDD)</param>
        /// <param name="endDate">date end (YYYYMMDD)</param>
        /// <returns>versions data ["id_advertiser","advertiser","id_product","product","id_vehicle","vehicle","id_slogan","date_media_num","id_media","advertDimension", "associated_file" </returns>
        public virtual DataSet GetVersions(string beginningDate, string endDate)
        {
            var sql = new System.Text.StringBuilder(1000);
            string tempSql = "";
            bool first = true;

            try
            {
                //Get media type ID list ex. 1,2,3
                string mediaTypeStringIds = _session.CustomerDataFilters.SelectedMediaType;

                //Media type array
                string[] mediaTypeList = mediaTypeStringIds.Split(new char[] { ',' });


                //Build SQL query for each Media type
                for (int i = 0; i < mediaTypeList.Length; i++)
                {
                    try
                    {
                        tempSql = GetSQLQuery(mediaTypeList[i], beginningDate, endDate);
                        if (tempSql.Length > 0)
                        {
                            if (!first) sql.Append("  union  ");
                            sql.Append(tempSql);
                            first = false;
                        }
                    }
                    catch (Exception err)
                    {
                        throw new InsertionsDALException(" Impossible to bulid SQL query for media type : " + mediaTypeList[i], err);
                    }

                }

                //Set final SQL query (all media type)
                if (sql.Length > 0)
                {
                    tempSql = sql.ToString();
                    sql = new StringBuilder(1000);
                    sql.Append(" select * from ( ");
                    sql.Append(tempSql);
                    sql.Append(" ) order by advertiser,id_advertiser,product ,id_product, vehicle,id_vehicle,id_slogan,associated_file,date_media_num");

                    return _session.Source.Fill(sql.ToString());
                }

                return null;
            }
            catch (System.Exception exc)
            {
                throw new InsertionsDALException(" Error impossible to obtain versions data", exc);
            }
        }

        /// <summary>
        /// Get Version Min Parution Date
        /// </summary>
        /// <param name="idVersion"></param>
        /// <param name="vehicleInformation">vehicle infos</param>
        /// <returns></returns>
        public string GetVersionMinParutionDate(string idVersion, VehicleInformation vehicleInformation)
        {
            string minDate = string.Empty;
            Table sloganTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.slogan);
            var sql = new StringBuilder(1000);

            sql.AppendFormat(" select to_char(DATE_CREATION,'yyyymmdd')  from {0} where id_slogan={1}", sloganTable.SqlWithPrefix, idVersion);

            var ds = _session.Source.Fill(sql.ToString());

            if (ds != null && ds.Tables[0] != null)
            {
                minDate = Convert.ToString(ds.Tables[0].Rows[0][0]);
            }
            return minDate;
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
        /// <param name="columns">List of detail columns</param>
        /// <returns>True if there is detail levels selected</returns>
        protected virtual bool AppendInsertionsSqlFields(Table tData, VehicleInformation vehicle,
            StringBuilder sql, List<GenericColumnItemInformation> columns)
        {

            string tmp = string.Empty;
            bool detailLevelSelected = false;

            //Insertions fields
            /* Get the SQL code of the fields corresponding to columns (from the XML configuration files) except the ones that mutched with
             * the detail level list
             * */

            tmp = GenericColumns.GetSqlFields(columns, null);
            if (vehicle.Id == Vehicles.names.directMarketing
                 || vehicle.Id == Vehicles.names.mailValo)
            {
                tmp = tmp.Replace("nvl(id_slogan,0)", "nvl(wp.id_slogan,0)");
            }
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
        protected virtual bool AppendInsertionsSqlFields(Table tData, VehicleInformation vehicle,
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
            if (vehicle.Id == Vehicles.names.directMarketing
                || vehicle.Id == Vehicles.names.mailValo)
            {
                tmp = tmp.Replace("nvl(id_slogan,0)", "nvl(wp.id_slogan,0)");
            }
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

            //Category
            if (!_msCreaConfig && (vehicle.Id == CstDBClassif.Vehicles.names.tv
                || vehicle.Id == CstDBClassif.Vehicles.names.tvGeneral
                || vehicle.Id == CstDBClassif.Vehicles.names.tvAnnounces
                || vehicle.Id == CstDBClassif.Vehicles.names.tvSponsorship
                || vehicle.Id == CstDBClassif.Vehicles.names.tvNonTerrestrials
                ) && !hasCategory)
            {
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
                && (vehicle.Id == Vehicles.names.radio
                  || vehicle.Id == Vehicles.names.radioGeneral
                  || vehicle.Id == Vehicles.names.radioSponsorship
                || vehicle.Id == Vehicles.names.radioMusic)
                && !_session.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.slogan)
                && sql.ToString().IndexOf("id_slogan") < 0
                )
            {
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
        protected virtual void AppendSqlTables(Schema sAdExpr03, Table tData
            , StringBuilder sql, List<GenericColumnItemInformation> columns)
        {

            string tmp = string.Empty;

            sql.AppendFormat(" from {0} ", tData.SqlWithPrefix);
            /* Get tables that corresponds to the classification columns
             * */
            tmp = GenericColumns.GetSqlTables(sAdExpr03.Label, columns, null);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(", {0} ", tmp);
            }
            /* Get tables that corresponds to specific columns
             * */
            tmp = GenericColumns.GetSqlConstraintTables(sAdExpr03.Label, columns);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(", {0} ", tmp);//Rules constraints management
            }
        }
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
        /// <param name="detailLevelsIds">Level details</param>
        protected virtual void AppendSqlTables(Schema sAdExpr03, Table tData, StringBuilder sql,
            List<GenericColumnItemInformation> columns, ArrayList detailLevelsIds)
        {

            string tmp = string.Empty;

            sql.AppendFormat(" from {0} ", tData.SqlWithPrefix);
            /* Get tables that corresponds to the classification levels
             * */
            if (!_msCreaConfig && _session.DetailLevel != null)
            {
                tmp = _session.DetailLevel.GetSqlTables(sAdExpr03.Label);
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(", {0} ", tmp);
                }
            }
            /* Get tables that corresponds to the classification columns
             * */
            tmp = GenericColumns.GetSqlTables(sAdExpr03.Label, columns, detailLevelsIds);
            if (tmp.Length > 0)
            {
                sql.AppendFormat(", {0} ", tmp);
            }
            /* Get tables that corresponds to specific columns
             * */
            tmp = GenericColumns.GetSqlConstraintTables(sAdExpr03.Label, columns);
            if (tmp.Length > 0)
            {
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
        protected virtual void AppendUniversFilters(StringBuilder sql, Table tData,
            int fromDate, int toDate, VehicleInformation vehicle)
        {

            #region Period

            sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
            sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);

            #endregion

            #region Advertiser modules
            /* Versions filter
             * If the customer has filtred the number of the original slogan, we can find this list in the SloganIdList variable.
             * in this case we add an id_slogan filter, like below
             * */
            if (_module.Id == CstWeb.Module.Name.ALERTE_PLAN_MEDIA && !string.IsNullOrEmpty(_session.SloganIdList))
            {
                sql.AppendFormat(" and {1}.id_slogan in ( {0} ) ", _session.SloganIdList, tData.Prefix);
            }
            // Product classification selection           
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(tData.Prefix, true));

            #endregion

            #region Media selection
            /* Get the list of medias selected
             * There are several treenodes to save the univers media, so according to the module we can get the media list
             * */
            #region Old
            //Medias selection
            //int positionUnivers = 1;
            //string mediaList = string.Empty;
            //while (_session.CompetitorUniversMedia[positionUnivers] != null)
            //{
            //    mediaList += _session.GetSelection((TreeNode)
            //        _session.CompetitorUniversMedia[positionUnivers],
            //        CstCustomer.Right.type.mediaAccess) + ",";
            //    positionUnivers++;
            //}
            #endregion
            string mediaList = GetCompetitormedias();
            if (!string.IsNullOrEmpty(mediaList)) {
                sql.AppendFormat(" and {0}.id_media in ({1})",
                   tData.Prefix,
                    mediaList);
            }

            #endregion

            #region Rights
            /* Get product classification rights
             * */
            if (_module == null) throw (new InsertionsDALException("_module parameter cannot be NULL"));
            sql.Append(FctWeb.SQLGenerator.
                GetClassificationCustomerProductRight(_session, tData.Prefix, true, _module.ProductRightBranches));


            /* Get media classification rights
             * */
            if (vehicle.Id == CstDBClassif.Vehicles.names.internet)
            {
                sql.Append(SQLGenerator.GetAdNetTrackMediaRight(_session, tData.Prefix, true));
            }
            else
            {
                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session, tData.Prefix, true));
            }

            // Autopromo
            string idMediaLabel = string.Empty;

            if (vehicle.Id == CstDBClassif.Vehicles.names.adnettrack || vehicle.Id == CstDBClassif.Vehicles.names.evaliantMobile)
                idMediaLabel = "id_media_evaliant";
            else if (vehicle.Id == CstDBClassif.Vehicles.names.mms)
                idMediaLabel = "id_media_mms";

            if (vehicle.Autopromo && (vehicle.Id == CstDBClassif.Vehicles.names.adnettrack
                || vehicle.Id == CstDBClassif.Vehicles.names.evaliantMobile
                || vehicle.Id == CstDBClassif.Vehicles.names.mms))
            {

                Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                    sql.AppendFormat(" and {0}.auto_promotion = 0 ", tData.Prefix);
                else if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany)
                {
                    sql.AppendFormat(" and ({0}.id_media, {0}.id_holding_company) not in ( ", tData.Prefix);
                    sql.AppendFormat(" select distinct {0}, id_holding_company ", idMediaLabel);
                    sql.AppendFormat(" from {0} ", tblAutoPromo.Sql);
                    sql.AppendFormat(" where {0} is not null ", idMediaLabel);
                    sql.AppendFormat(" ) ");
                }
            }

            #endregion

            #region Global rules


            /* Get radio rules
             * */
            if (_module.ModuleType == CstWeb.Module.Type.tvSponsorship)
            {
                sql.Append(SQLGenerator.
                    getAdExpressUniverseCondition(CstWeb.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID
                    , tData.Prefix, tData.Prefix, tData.Prefix, true));
            }
            else
            {
                sql.Append(SQLGenerator.
                    GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse
                    .EXCLUDE_PRODUCT_LIST_ID, tData.Prefix, true, false));
            }

            #endregion

            #region Banners Format Filter
            List<Int64> formatIdList = null;
            VehicleInformation cVehicleInfo = null;
            if (vehicle.Id == CstDBClassif.Vehicles.names.internet)
                cVehicleInfo = VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack);
            else
                cVehicleInfo = vehicle;
            formatIdList = _session.GetValidFormatSelectedList(new List<VehicleInformation>(new[] { cVehicleInfo }));
            if (formatIdList.Count > 0)
                sql.AppendFormat(" and {0}.ID_{1} in ({2}) ", tData.Prefix,
                    WebApplicationParameters.DataBaseDescription.GetTable(WebApplicationParameters.
                    VehiclesFormatInformation.VehicleFormatInformationList[cVehicleInfo.DatabaseId].
                    FormatTableName).Label, string.Join(",", formatIdList.ConvertAll(p => p.ToString()).ToArray()));
            #endregion

            #region Filtres

            if (_session.SloganIdZoom > -1)
            {//For Russia : _session.SloganIdZoom > long.MinValue (correspond to the absence of ID for the version)
                sql.AppendFormat(" and wp.id_slogan={0}", _session.SloganIdZoom);
            }
            if ((_msCreaConfig || _creaConfig) && vehicle.Id != CstDBClassif.Vehicles.names.adnettrack &&
                vehicle.Id != CstDBClassif.Vehicles.names.internet &&
                vehicle.Id != CstDBClassif.Vehicles.names.evaliantMobile
               )
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
                sql.AppendFormat(" and {0}.id_slogan in({1}) ", tData.Prefix, slogans);
            }
            #endregion

            /* For the media type press and international press we can do studies according to the inset option (total, inset excluding, inset)
             * so to get the corresponding filter we use GetJointForInsertDetail method
             * */
            if (vehicle.Id == Vehicles.names.press ||
                vehicle.Id == Vehicles.names.internationalPress
                || vehicle.Id == Vehicles.names.newspaper
                || vehicle.Id == Vehicles.names.magazine)
            {
                sql.Append(FctWeb.SQLGenerator.GetJointForInsertDetail(_session,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            }

            switch (vehicle.Id)
            {
                case CstDBClassif.Vehicles.names.internet:
                    sql.AppendFormat(" and {1}.id_vehicle={0} ",
                        VehiclesInformation.Get(Vehicles.names.adnettrack)
                        .DatabaseId, tData.Prefix);
                    break;
                default:
                    sql.AppendFormat(" and {1}.id_vehicle={0} "
                        , vehicle.DatabaseId, tData.Prefix);
                    break;
            }
        }



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
        protected virtual void AppendUniversFilters(StringBuilder sql, Table tData,
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
            if (_session.PeriodType == CstWeb.CustomerSessions.Period.Type.dateToDate)
            {

                int begin = Convert.ToInt32(_session.PeriodBeginningDate);
                if (begin > fromDate)
                {
                    sql.AppendFormat(" {1}.date_media_num >= {0}", begin, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation >= to_date('{1} 00:00:00','yyyymmdd HH24:MI:SS') ", tData.Prefix, begin);
                    }
                }
                else
                {
                    sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation >= to_date('{1} 00:00:00','yyyymmdd HH24:MI:SS') ", tData.Prefix, fromDate);
                    }
                }
                int end = Convert.ToInt32(_session.PeriodEndDate);
                if (end < toDate)
                {
                    sql.AppendFormat(" and {1}.date_media_num <= {0}", end, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation <= to_date('{1} 23:59:59','yyyymmdd HH24:MI:SS') ", tData.Prefix, end);
                    }
                }
                else
                {
                    sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation <= to_date('{1} 23:59:59','yyyymmdd HH24:MI:SS') ", tData.Prefix, toDate);
                    }
                }
            }
            else
            {
                sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
                sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);
                if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
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
            if (_module.Id == CstWeb.Module.Name.ALERTE_PLAN_MEDIA && !string.IsNullOrEmpty(_session.SloganIdList))
            {
                sql.AppendFormat(" and {1}.id_slogan in ( {0} ) ", _session.SloganIdList, tData.Prefix);
            }
            // Product classification selection
            /* We can get the product classification selection by using _session.PrincipalProductUniverses[universId].GetSqlConditions(tData.Prefix, true)
             * */
            if (_module.Id != CstWeb.Module.Name.NEW_CREATIVES &&
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
            if (_module.Id == CstWeb.Module.Name.ANALYSE_CONCURENTIELLE || _module.Id == CstWeb.Module.Name.ANALYSE_PORTEFEUILLE)
            {
                Dictionary<CstCustomer.Right.type, string> selectedVehicles = _session.CustomerDataFilters.SelectedVehicles;
                if (selectedVehicles != null && selectedVehicles.ContainsKey(CstCustomer.Right.type.mediaAccess))
                {
                    listMediaAccess = selectedVehicles[CstCustomer.Right.type.mediaAccess];
                }
            }
            if (_module.Id == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA)
            {
                string list = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);

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

            if (_session.SecondaryMediaUniverses != null && _session.SecondaryMediaUniverses.Count > 0)
            {
                sql.Append((vehicle.Id == Vehicles.names.mailValo)
                               ? _session.SecondaryMediaUniverses[0].GetSqlConditions(tData.Prefix, true, ReplaceMailValoByVmc)
                               : _session.SecondaryMediaUniverses[0].GetSqlConditions(tData.Prefix, true));

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
            if (vehicle.Id == Vehicles.names.mailValo)
                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session, tData.Prefix, true, ReplaceMailValoByVmc));
            else
                sql.Append(vehicle.Id == Vehicles.names.internet
                               ? SQLGenerator.GetAdNetTrackMediaRight(_session, tData.Prefix, true)
                               : SQLGenerator.getAnalyseCustomerMediaRight(_session, tData.Prefix, true));

            /* Get rights detail spot to spot TNT
             * */
            if (vehicle.Id == Vehicles.names.tv
              && !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG) && !_creaConfig)
            {
                string idTNTCategory = TNS.AdExpress.Domain.Lists
                    .GetIdList(CstWeb.GroupList.ID.category, CstWeb.GroupList.Type.digitalTv);
                if (!string.IsNullOrEmpty(idTNTCategory))
                {
                    sql.AppendFormat(" and {0}.id_category not in ({1})  ", WebApplicationParameters
                   .DataBaseDescription.DefaultResultTablePrefix, idTNTCategory);
                }
            }



            #endregion

            #region Sponsorship univers
            /* This test is specific to the sponsoring modules
             * */
            if (_module.Id == CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES
                || _module.Id == CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS)
            {
                string tmp = string.Empty;
                if (_session.CurrentUniversMedia != null && _session.CurrentUniversMedia.Nodes.Count > 0)
                {
                    tmp = _session.GetSelection(_session.CurrentUniversMedia, CstCustomer.Right.type.mediaAccess);
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
            if (_module.ModuleType == CstWeb.Module.Type.tvSponsorship)
            {
                sql.Append(SQLGenerator.getAdExpressUniverseCondition
                    (CstWeb.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID, tData.Prefix, tData.Prefix, tData.Prefix, true));
            }
            else
            {
                sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, tData.Prefix, true, false));
            }
            /* This test is specific to the French version of the site
             * */
            //string idCategoryThematic = TNS.AdExpress.Domain.Lists.GetIdList(CstWeb.GroupList.ID.category, CstWeb.GroupList.Type.thematicTv);
            //if (!string.IsNullOrEmpty(idCategoryThematic))
            //{
            //    sql.AppendFormat("  and  {0}.id_category not in ( {1}) ", tData.Prefix, idCategoryThematic);
            //}
            #endregion

            #region Banners Format Filter
            List<Int64> formatIdList = null;
            VehicleInformation cVehicleInfo = null;
            if (vehicle.Id == Vehicles.names.internet)
                cVehicleInfo = VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack);
            else
                cVehicleInfo = vehicle;
            formatIdList = _session.GetValidFormatSelectedList(new List<VehicleInformation>(new[] { cVehicleInfo }));
            if (formatIdList.Count > 0)
                sql.Append(" and " + tData.Prefix + ".ID_" + WebApplicationParameters.DataBaseDescription.
                    GetTable(WebApplicationParameters.VehiclesFormatInformation.VehicleFormatInformationList[cVehicleInfo.DatabaseId].FormatTableName).Label
                    + " in (" + string.Join(",", formatIdList.ConvertAll(p => p.ToString()).ToArray()) + ") ");
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

            /* For the media type press and international press we can do studies according to the inset option (total, inset excluding, inset)
             * so to get the corresponding filter we use GetJointForInsertDetail method
             * */
            if (vehicle.Id == Vehicles.names.press || vehicle.Id == Vehicles.names.internationalPress
                || vehicle.Id == Vehicles.names.newspaper
                || vehicle.Id == Vehicles.names.magazine)
            {
                sql.Append(FctWeb.SQLGenerator.GetJointForInsertDetail(_session,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            }

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
        /// <param name="columns">Classification columns list</param>
        protected virtual void AppendSqlGroupByFields(StringBuilder sql, Table tData,
            VehicleInformation vehicle, List<GenericColumnItemInformation> columns)
        {

            string tmp = string.Empty;
            bool first = true;

            sql.Append(" group by");


            /* Get SQL group by fields for the classification columns defined in the Xml configuration file
             * */
            tmp = GenericColumns.GetSqlGroupByFields(columns, null);
            if (vehicle.Id == Vehicles.names.directMarketing
                || vehicle.Id == Vehicles.names.mailValo)
            {
                tmp = tmp.Replace("id_slogan", "wp.id_slogan");
            }
            //tmp = tmp.Replace("id_slogan", "wp.id_slogan");
            //tmp = tmp.Replace("wp.wp.id_slogan", "wp.id_slogan");
            if (tmp.Length > 0)
            {
                sql.AppendFormat(" {0}", tmp);
                first = false;
            }

            /* Get SQL group by fields for classification column that need others columns information
             * */
            if (columns != null)
            {
                tmp = GenericColumns.GetSqlConstraintGroupByFields(columns);
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (!first) sql.Append(",");
                    first = false;
                    sql.AppendFormat(" {0}", tmp);
                }
            }



        }


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
        protected virtual void AppendSqlGroupByFields(StringBuilder sql, Table tData,
            VehicleInformation vehicle, ArrayList detailLevelIds, List<GenericColumnItemInformation> columns)
        {

            string tmp = string.Empty;
            bool first = true;

            sql.Append(" group by");

            /* Get SQL group by fields for the classification levels selected by the customer
             * */
            if (!_msCreaConfig && _session.DetailLevel != null)
            {
                tmp = _session.DetailLevel.GetSqlGroupByFields();
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" {0}", tmp);
                    first = false;
                }
            }

            /* Get SQL group by fields for the classification columns defined in the Xml configuration file
             * */

            tmp = GenericColumns.GetSqlGroupByFields(columns, detailLevelIds);
            if (vehicle.Id == Vehicles.names.directMarketing
                || vehicle.Id == Vehicles.names.mailValo)
            {
                tmp = tmp.Replace("id_slogan", "wp.id_slogan");
            }
            if (tmp.Length > 0)
            {
                if (!first) sql.Append(",");
                sql.AppendFormat(" {0}", tmp);
                first = false;
            }

            /* Get SQL group by fields for classification column that need others columns information
             * */
            if (columns != null)
            {
                tmp = GenericColumns.GetSqlConstraintGroupByFields(columns);
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (!first) sql.Append(",");
                    first = false;
                    sql.AppendFormat(" {0}", tmp);
                }
            }

            if (!_msCreaConfig && _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SLOGAN_ACCESS_FLAG)
                && (vehicle.Id == Vehicles.names.radio
                || vehicle.Id == Vehicles.names.radioGeneral
                || vehicle.Id == Vehicles.names.radioSponsorship
                || vehicle.Id == Vehicles.names.radioMusic)
                && !_session.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.slogan)
                )
            {
                sql.AppendFormat(", {0}.id_slogan ", tData.Prefix);
            }

            if (!_msCreaConfig && !first
                && (vehicle.Id == Vehicles.names.tv
                || vehicle.Id == Vehicles.names.tvGeneral
                || vehicle.Id == Vehicles.names.tvAnnounces
                || vehicle.Id == Vehicles.names.tvSponsorship
                || vehicle.Id == Vehicles.names.tvNonTerrestrials)
                && !_session.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.category))
            {
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
        /// <param name="columns">Classification columns list</param>
        protected virtual void AppendSqlOrderFields(Table tData, StringBuilder sql,
            VehicleInformation vehicle, List<GenericColumnItemInformation> columns)
        {
            string tmp = string.Empty;
            bool first = true;

            sql.Append(" order by ");



            /* Get SQL order by fields for the classification columns defined in the Xml configuration file
             * */

            tmp = GenericColumns.GetSqlOrderFields(columns, null);
            if (tmp.Length > 0)
            {
                if (!first) sql.Append(",");
                sql.AppendFormat(" {0}", tmp);
                first = false;
            }

            /* Get SQL order by fields for classification column that need others columns information
             * */
            if (columns != null)
            {
                tmp = GenericColumns.GetSqlConstraintOrderFields(columns);
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (!first) sql.Append(",");
                    first = false;
                    sql.AppendFormat(" {0}", tmp);
                }
            }

            if (!_msCreaConfig)
                AppendSloganField(sql, tData, vehicle, columns);

        }

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
        protected virtual void AppendSqlOrderFields(Table tData, StringBuilder sql,
            VehicleInformation vehicle, ArrayList detailLevelIds, List<GenericColumnItemInformation> columns)
        {
            string tmp = string.Empty;
            bool first = true;

            sql.Append(" order by ");

            /* Get SQL order by fields for the classification levels selected by the customer
             * */
            if (!_msCreaConfig && _session.DetailLevel != null)
            {
                tmp = _session.DetailLevel.GetSqlOrderFields();
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" {0}", tmp);
                    first = false;
                }
            }

            /* Get SQL order by fields for the classification columns defined in the Xml configuration file
             * */

            tmp = GenericColumns.GetSqlOrderFields(columns, detailLevelIds);
            if (tmp.Length > 0)
            {
                if (!first) sql.Append(",");
                sql.AppendFormat(" {0}", tmp);
                first = false;
            }

            /* Get SQL order by fields for classification column that need others columns information
             * */
            if (columns != null)
            {
                tmp = GenericColumns.GetSqlConstraintOrderFields(columns);
                if (!string.IsNullOrEmpty(tmp))
                {
                    if (!first) sql.Append(",");
                    first = false;
                    sql.AppendFormat(" {0}", tmp);
                }
            }

            if (!_msCreaConfig)
                AppendSloganField(sql, tData, vehicle, columns);

        }
        #endregion

        #region GetSQLQuery
        /// <summary>
        /// Get versions SQL query
        /// </summary>
        /// <param name="idVehicle">ID media type</param>
        /// <param name="beginingDate">beginning date</param>
        /// <param name="endDate">end date</param>
        /// <returns>versions SQL query</returns>
        protected virtual string GetSQLQuery(string idVehicle, string beginingDate, string endDate)
        {

            StringBuilder sql = new StringBuilder(1000);

            VehicleInformation vehicleInformation = null;

            //Get vehicle information
            if (idVehicle != null)
                vehicleInformation = VehiclesInformation.Get(Int64.Parse(idVehicle));

            //Get tables description
            Table TblVehicle = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);
            Table TblProduct = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product);
            Table TblAdvertiser = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertiser);
            Table TblFormat = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format);

            //Ge table name
            string tableName = string.Empty;

            if (_module.ModuleType == CstWeb.Module.Type.tvSponsorship)
                tableName = GetTableName(vehicleInformation, CstWeb.Module.Type.tvSponsorship);
            else if (Dates.Is4M(beginingDate))
                tableName = GetTableName(vehicleInformation, CstWeb.Module.Type.alert);
            else
                tableName = GetTableName(vehicleInformation, CstWeb.Module.Type.analysis);

            //Ge table prefix
            string tablePrefixe = GetTablePrefix();


            if (CanBuildSqlQuery(vehicleInformation))
            {
                //SELECT clause
                sql.AppendFormat(" select distinct {0}.id_advertiser,advertiser,{0}.id_product,product,{0}.id_vehicle, vehicle "
                    , tablePrefixe);

                //Get version ID field
                sql.Append(GetSloganField(vehicleInformation, tablePrefixe));

                //Date field
                sql.Append(GetDateFields(vehicleInformation));

                //Fields specific to a media type
                sql.Append(GetFields(vehicleInformation.Id, tablePrefixe));

                //FROM clause                
                sql.Append(" from ");
                //Tables 
                AppendTables(sql, vehicleInformation, TblVehicle, TblProduct, TblAdvertiser, TblFormat, tableName, tablePrefixe);

                //WHERE clause
                AppendTablesJoins(sql, vehicleInformation, TblVehicle, TblProduct, TblAdvertiser, TblFormat, tableName, tablePrefixe);

                //period
                sql.AppendFormat(" and date_media_num >={0} ", beginingDate);
                sql.AppendFormat(" and date_media_num <={0} ", endDate);

                #region Products rights
                //Products rights
                sql.Append(SQLGenerator.getAnalyseCustomerProductRight(_session, tablePrefixe, true));

                // Product to exclude 
                ProductItemsList productItemsList = null;
                if (Product.Contains(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID)
                    && (productItemsList = Product.GetItemsList(TNS.AdExpress.Constantes.Web
                    .AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID)) != null)
                {
                    sql.Append(productItemsList.GetExcludeItemsSql(true, tablePrefixe));
                }
                #endregion

                #region Product selection
                //product selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(tablePrefixe, true));

                #endregion

                #region Media rights
                // Media rights
                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session, tablePrefixe, true));
                #endregion

                #region Media Universe
                //Media Universe
                sql.Append(SQLGenerator.GetResultMediaUniverse(_session, tablePrefixe));
                #endregion

                #region AppendFilters
                //Append Filters
                AppendFilters(sql, vehicleInformation, idVehicle, tablePrefixe);
                #endregion
            }

            return sql.ToString();
        }

        protected virtual string GetSloganField(VehicleInformation vehicleInformation, string tablePrefixe)
        {
            StringBuilder sql = new System.Text.StringBuilder(500);
            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE)
            {
                sql.AppendFormat(", nvl({0}.id_slogan, 0) as id_slogan", tablePrefixe);
            }
            else
            {
                if (vehicleInformation.Id != CstDBClassif.Vehicles.names.adnettrack
                   && vehicleInformation.Id != CstDBClassif.Vehicles.names.evaliantMobile)
                {
                    sql.AppendFormat(" , nvl({0}.id_slogan,0) as id_slogan", tablePrefixe);
                }
                else
                {
                    sql.AppendFormat(" , nvl({0}.hashcode,0) as id_slogan", tablePrefixe);
                }
            }
            return sql.ToString();
        }


        #endregion

        #region AppendFilters
        /// <summary>
        /// Append Filters
        /// </summary>
        /// <param name="sql">string builder</param>
        /// <param name="vehicleInformation">vehicle Information</param>
        /// <param name="idVehicle">ID media type</param>
        /// <param name="tablePrefixe">tablePrefixe</param>
        protected virtual void AppendFilters(StringBuilder sql,
            VehicleInformation vehicleInformation, string idVehicle, string tablePrefixe)
        {
            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE)
            {
                Int64 idAdditionalTarget = Int64.Parse(_session
                    .GetSelection(_session.SelectionUniversAEPMTarget, CstCustomer.Right.type.aepmTargetAccess));
                //on one target
                sql.AppendFormat(" and {0}.id_target in({1}) ",
                    WebApplicationParameters.DataBaseDescription.
                    GetTable(TableIds.appmTargetMediaAssignment).Prefix, idAdditionalTarget);

                //outside encart
                sql.AppendFormat(" and {0}.id_inset is null ",
                    tablePrefixe);

            }
            else
            {

                // Niveau de produit
                sql.Append(SQLGenerator.getLevelProduct(_session, tablePrefixe, true));

                //ID Vehicle
                sql.AppendFormat(" and (({0}.id_vehicle= {1})) ", tablePrefixe, idVehicle);

                //Adnettrack and Evaliant mobile
                if (vehicleInformation.Id != Vehicles.names.adnettrack
                   && vehicleInformation.Id != Vehicles.names.evaliantMobile)
                {
                    sql.AppendFormat(" and {0}.id_slogan!=0 ", tablePrefixe);
                }
            }
        }
        #endregion


        #region AppendTablesJoins

        /// <summary>
        /// Append Tables Joins
        /// </summary>
        /// <param name="sql">sql builder</param>
        /// <param name="vehicleInformation">vehicle Information</param>
        /// <param name="TblVehicle"> Table Media type</param>
        /// <param name="TblProduct">Table product</param>
        /// <param name="TblAdvertiser">Table advertiser</param>
        /// <param name="TblFormat">Table format</param>
        /// <param name="tableName">table name</param>
        /// <param name="tablePrefixe">table Prefixe</param>
        private void AppendTablesJoins(StringBuilder sql, VehicleInformation vehicleInformation, Table TblVehicle,
            Table TblProduct, Table TblAdvertiser, Table TblFormat, string tableName, string tablePrefixe)
        {
            sql.Append(" Where ");
            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE)
            {
                sql.AppendFormat(" {0}.id_slogan!=0 and  ", tablePrefixe);
            }
            sql.AppendFormat("  {0}.id_vehicle={1}.id_vehicle ", TblVehicle.Prefix, tablePrefixe);
            sql.AppendFormat(" and {0}.id_language={1}", TblVehicle.Prefix, _session.DataLanguage.ToString());
            sql.AppendFormat(" and {0}.activation<{1}", TblVehicle.Prefix, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and {0}.id_product={1}.id_product ", TblProduct.Prefix, tablePrefixe);
            sql.AppendFormat(" and {0}.activation<{1}", TblProduct.Prefix, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and {0}.id_language={1}", TblProduct.Prefix, _session.DataLanguage.ToString());
            sql.AppendFormat(" and {0}.id_advertiser={1}.id_advertiser ", TblAdvertiser.Prefix, tablePrefixe);
            sql.AppendFormat(" and {0}.id_language={1}", TblAdvertiser.Prefix, _session.DataLanguage.ToString());
            sql.AppendFormat(" and {0}.activation<{1}", TblAdvertiser.Prefix, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);

            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE
                  || vehicleInformation.Id == Vehicles.names.press
                  || vehicleInformation.Id == Vehicles.names.internationalPress
                  || vehicleInformation.Id == Vehicles.names.newspaper
                  || vehicleInformation.Id == Vehicles.names.magazine)
            {
                sql.AppendFormat(" and {0}.id_format(+)={1}.id_format ", TblFormat.Prefix, tablePrefixe);
                sql.AppendFormat(" and {0}.id_language(+)={1}", TblFormat.Prefix, _session.DataLanguage.ToString());
                sql.AppendFormat(" and {0}.activation(+)<{1}", TblFormat.Prefix, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            }
            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE)
            {
                sql.AppendFormat(" and {0}.id_media_secodip = {1}.id_media ",
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.appmTargetMediaAssignment).Prefix, tablePrefixe);
            }
        }

        protected virtual void AppendTables(StringBuilder sql, VehicleInformation vehicleInformation,
            Table TblVehicle, Table TblProduct, Table TblAdvertiser, Table TblFormat, string tableName, string tablePrefixe)
        {
            sql.AppendFormat(" {0}, {1}, {2} ", TblVehicle.SqlWithPrefix, TblProduct.SqlWithPrefix, TblAdvertiser.SqlWithPrefix);
            if (vehicleInformation.Id == Vehicles.names.press
               || vehicleInformation.Id == Vehicles.names.internationalPress
               || vehicleInformation.Id == Vehicles.names.newspaper
               || vehicleInformation.Id == Vehicles.names.magazine
               )
            {
                sql.AppendFormat(",{0} ", TblFormat.SqlWithPrefix);
            }
            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE)
            {
                sql.AppendFormat(" ,{0} ", tableName);
                sql.AppendFormat(" ,{0} ", WebApplicationParameters.
                    DataBaseDescription.GetTable(TableIds.appmTargetMediaAssignment).SqlWithPrefix);
            }
            else sql.AppendFormat(" ,{0}.{1} {2} ", WebApplicationParameters.
                DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, tableName, tablePrefixe);


        }

        #endregion


        #region GetDateFields
        /// <summary>
        /// Ge date fields
        /// </summary>
        /// <param name="vehicleInformation">vehicle Information</param>
        /// <returns>date fields</returns>
        protected virtual string GetDateFields(VehicleInformation vehicleInformation)
        {
            string sql = "";

            // Date fields
            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE)
            {
                sql += ", date_media_num,date_cover_num ";
            }
            else
            {
                if (vehicleInformation.Id == Vehicles.names.press
                    || vehicleInformation.Id == Vehicles.names.internationalPress
                    || vehicleInformation.Id == Vehicles.names.newspaper
                    || vehicleInformation.Id == Vehicles.names.magazine
                    )
                    sql += ", date_cover_num as date_media_num ";
                else sql += ", date_media_num ";
            }
            return sql;
        }
        #endregion

        #region CanBuildSqlQuery
        /// <summary>
        /// Check if can build sql query
        /// </summary>
        /// <param name="vehicleInformation">vehicle Information</param>
        /// <returns>True if can build sql query</returns>
        protected virtual bool CanBuildSqlQuery(VehicleInformation vehicleInformation)
        {
            return (vehicleInformation != null
                && ((vehicleInformation.AllowedMediaLevelItemsEnumList.
                Contains(DetailLevelItemInformation.Levels.slogan)
                && vehicleInformation.Id != CstDBClassif.Vehicles.names.internet
                 && vehicleInformation.Id != CstDBClassif.Vehicles.names.cinema
                 && vehicleInformation.Id != CstDBClassif.Vehicles.names.mms
                 && vehicleInformation.Id != CstDBClassif.Vehicles.names.search
                   && vehicleInformation.Id != CstDBClassif.Vehicles.names.social
                 && vehicleInformation.Id != CstDBClassif.Vehicles.names.mailValo)
                || _session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE));
        }

        #endregion


        #region GetTableName
        /// <summary>
        /// Get table name
        /// </summary>
        /// <param name="vehicleInformation">vehicle Information</param>
        /// <param name="currentModuleDescription">current Module Description</param>
        /// <returns>table name</returns>
        protected virtual string GetTableName(VehicleInformation vehicleInformation,
            CstWeb.Module.Type moduleType)
        {
            string tableName = string.Empty; ;

            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE)
            {
                tableName = string.Format(" {0}",
                    WebApplicationParameters.DataBaseDescription.
                    GetTable(TableIds.dataPressAPPM).SqlWithPrefix);
            }
            else
            {
                tableName = FctWeb.SQLGenerator.
                    GetVehicleTableNameForDetailResult(vehicleInformation.Id,
                    moduleType, _session.IsSelectRetailerDisplay);
            }
            return tableName;
        }

        #endregion

        #region GetTablePrefix
        /// <summary>
        /// Get table prefix
        /// </summary>      
        /// <returns>table name</returns>
        protected virtual string GetTablePrefix()
        {
            string tablePrefixe = string.Empty;

            if (_session.CurrentModule == CstWeb.Module.Name.BILAN_CAMPAGNE)
            {
                tablePrefixe = WebApplicationParameters.DataBaseDescription.
                    GetTable(TableIds.dataPressAPPM).Prefix;

            }
            else
            {
                tablePrefixe = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            }
            return tablePrefixe;
        }

        #endregion

        #region GetFields
        /// <summary>
        /// Get SqL fields specific to one Mdia type
        /// </summary>
        /// <param name="idVehicle">ID media type</param>
        /// <param name="prefixeTable">Table Prefixe</param>
        /// <returns> champs de requêtes </returns>
        protected virtual string GetFields(Vehicles.names idVehicle, string prefixeTable)
        {
            switch (idVehicle)
            {
                case Vehicles.names.radio:
                case Vehicles.names.radioGeneral:
                case Vehicles.names.radioSponsorship:
                case Vehicles.names.radioMusic:
                case Vehicles.names.tv:
                case Vehicles.names.tvGeneral:
                case Vehicles.names.tvSponsorship:
                case Vehicles.names.tvAnnounces:
                case Vehicles.names.tvNonTerrestrials:
                case Vehicles.names.others:
                    return ",id_media,TO_CHAR( duration)  as advertDimension, TO_CHAR(associated_file) as associated_file";
                case Vehicles.names.internationalPress:
                case Vehicles.names.press:
                case Vehicles.names.magazine:
                case Vehicles.names.newspaper:
                    return ",id_media,format as advertDimension, visual as associated_file";
                case Vehicles.names.outdoor:
                case Vehicles.names.dooh:
                case Vehicles.names.indoor:
                    return ",id_media,type_board as advertDimension, associated_file as associated_file";
                case Vehicles.names.mailValo:
                case Vehicles.names.directMarketing:
                    return ",id_media,TO_CHAR(weight) as advertDimension, TO_CHAR(associated_file) as associated_file";
                case Vehicles.names.adnettrack:
                case Vehicles.names.evaliantMobile:
                    return ",id_media, (dimension || ' / ' || format) as advertDimension, TO_CHAR(associated_file) as associated_file";
                default: return "";
            }
        }
        #endregion

        #endregion
        private string GetCompetitormedias()
        {
            string mediaList = string.Empty;
            List<long> ids = new List<long>();
            for (int p = 0; p < _session.PrincipalMediaUniverses.Count; p++)
            {
                ids.AddRange(_session.PrincipalMediaUniverses[p].GetLevelValue(TNSClassificationLevels.MEDIA, AccessType.includes));
            }
            if (ids.Count > 0) return String.Join(",", ids);
            return mediaList;
        }
    }
}
