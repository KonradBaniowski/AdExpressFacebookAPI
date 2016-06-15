using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
using System.Data.SqlClient;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using WebConstante = TNS.AdExpress.Constantes.Web;
using CustomerRightType = TNS.AdExpress.Constantes.Customer.Right.type;

using System.Reflection;
using TNS.AdExpressI.Common.DAL.Russia;
using System.Collections;

namespace TNS.AdExpressI.Classification.DAL.Russia
{
    public partial class ClassificationDAL : TNS.AdExpressI.Classification.DAL.ClassificationDAL
    {
        #region Variables
        protected string _keyWord = "";

        private readonly CommonDAL _common;

        public CommonDAL Common
        {
            get
            {
                return _common;
            }
        }

        MediaRight MediaRight
        {
            get
            {
                return Common.GetMediaRight();
            }
        }

        ProductRight ProductRight
        {
            get
            {
                return Common.GetProductRight();
            }
        }

        ProductClassification ProductClass
        {
            get
            {
                return Common.GetProductClassification();
            }
        }



        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>	
        public ClassificationDAL(WebSession session)
            : base(session)
        {
            _toLowerCase = false;
            _common = new CommonDAL(session, session.CurrentModule);
        }
        /// <summary>
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="dimension">Product or vehicle classification brand</param>
        public ClassificationDAL(WebSession session, TNS.Classification.Universe.Dimension dimension)
            : base(session, dimension)
        {
            _toLowerCase = false;
            _common = new CommonDAL(session, session.CurrentModule);
        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="genericDetailLevel">generic detail level selected by the user</param>
        /// <param name="vehicleList">List of media selected by the user</param>
        public ClassificationDAL(WebSession session, GenericDetailLevel genericDetailLevel, string vehicleList)
            : base(session, genericDetailLevel, vehicleList)
        {
            _toLowerCase = false;
            _common = new CommonDAL(session, session.CurrentModule);
        }
        #endregion

        #region Procedure names

        /// <summary>
        /// cl_vehicle_get
        /// </summary>
        private const string GetDetailMediaProcedureName = "[dbo].[cl_vehicle_get]";

        /// <summary>
        /// test_productanalysis_recap_get
        /// </summary>
        private const string GetRecapDetailMediaProcedureName = "[dbo].[rp_ax_productanalysis_recap_get]";

        /// <summary>
        /// cl_selected_items_get
        /// </summary>
        private const string GetSelectedItemsProcedureName = "[dbo].[cl_selected_items_get]";

        /// <summary>
        /// cl_sectors_get
        /// </summary>
        private const string GetSectorsProcedureName = "[dbo].[cl_sectors_get]";

        /// <summary>
        /// cl_media_get
        /// </summary>
        private const string GetMediaTypeProcedureName = "[dbo].[cl_media_get]";

        /// <summary>
        /// cl_class_by_link_get
        /// </summary>
        private const string GetItemsByLinkProcedureName = "[dbo].[cl_class_by_link_get]";

        /// <summary>
        /// cl_class_by_keyword_get
        /// </summary>
        private const string GetItemsByKeywordProcedureName = "[dbo].[cl_class_by_keyword_get]";


        #endregion

        private const int ProcedureTimeout = 180;


        /// <summary>
        /// This method provides SQL queries to get the media classification level's items.
        /// The data are filtered by customer's media rights and selected working set.		
        /// </summary>
        /// <returns>Data table 
        /// with media's identifiers ("idMediaType" column) and media's labels ("mediaType" column).
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Impossible to execute query
        /// </exception>
        public override DataSet GetMediaType()
        {

            // Lists of the current customer's Media rights

            Hashtable parameters = new Hashtable();

            /*
            ANALYSE_CONCURENTIELLE = 278; - PA module
            ANALYSE_DYNAMIQUE = 197; - Lost/Won
            ANALYSE_PLAN_MEDIA = 196; - MS Module
            ANALYSE_PORTEFEUILLE = 283; - Vehicle Portfolio module
            */

            // SP set Parameters
            parameters["@id_language"] = Convert.ToInt16(_session.DataLanguage);
            parameters["@module_media_access"] = Common.GetAllowedMediaTypes();
            parameters["@customer_media_access"] = MediaRight.MediaRightMediaAccess;
            parameters["@customer_media_except"] = MediaRight.MediaRightMediaExcept;
            parameters["@customer_vehicle_access"] = MediaRight.MediaRightVehicleAccess;
            parameters["@customer_vehicle_except"] = MediaRight.MediaRightVehicleExcept;
            parameters["@current_module"] = Convert.ToInt32(_session.CurrentModule);

            using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
            {
                return GetMediaType(conn, parameters);
            }
        }

        public static DataSet GetMediaType(SqlConnection conn, Hashtable parameters)
        {
            DataSet ds = new DataSet();

            // Lists of the current customer's Media rights

            #region Execution of the query
            try
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                // SP Construction    
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = GetMediaTypeProcedureName;
                cmd.CommandTimeout = ProcedureTimeout;

                CommonDAL.InitParams(parameters, cmd);

                // SP Execute
                SqlDataReader rs = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                using (rs)
                {
                    dt.Load(rs);
                }

                if (dt.Rows.Count > 0)
                {
                    // add datatables to dataset 
                    dt.Columns[0].ColumnName = "idMediaType";
                    dt.Columns[1].ColumnName = "mediaType";
                    ds.Tables.Add(dt);
                }

                conn.Close();
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible to execute query", err));
            }
            #endregion

            return ds;
        }



        #region GetRecapDetailMedia

        /// <summary>
        /// Only path search key words to GetDetailMedia() throw class's variable _keyWord;
        /// </summary>
        /// <param name="keyWord">Search key words from UI</param>
        /// <returns>Dataset with  a data table of 2 columns : "Id_SubMedia", "SubMedia".
        /// The column "Id_SubMedia" corresponds to the identifier of the level sub media.
        /// The column "SubMedia" corresponds to the label of the level sub media.       
        /// </returns>        
        public override DataSet GetDetailMedia(string keyWord)
        {
            _keyWord = keyWord;
            return GetDetailMedia();
        }

        /// <summary>
        /// Get the list of sub media corresponding to media type selected
        /// </summary>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
        /// <returns>Dataset with  a data table of 2 or more columns : At least "id_media", "media";
        /// Next columns depends of classification levels
        /// </returns>        
        public override DataSet GetDetailMedia()
        {
            try
            {

                string detailLevel1 = ((DetailLevelItemInformation)_genericDetailLevel.Levels[0]).DataBaseIdField;
                string detailLevel2 = (_genericDetailLevel.Levels.Count > 1) ? ((DetailLevelItemInformation)_genericDetailLevel.Levels[1]).DataBaseIdField : string.Empty;

                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    Hashtable parameters = new Hashtable();

                    parameters["@id_language"] = Convert.ToInt16(_session.DataLanguage);
                    parameters["@id_Media"] = Convert.ToInt16(_session.CustomerDataFilters.SelectedMediaType);
                    parameters["@detailLevel1"] = Common.IsNull(detailLevel1);
                    parameters["@detailLevel2"] = Common.IsNull(detailLevel2);
                    parameters["@selectedVehicle"] = Common.IsNull(_vehicleList);
                    parameters["@selectedKeyword"] = Common.IsNull(_keyWord);


                    #region Set Media Right Data

                    parameters["@mediaRightMediaAccess"] = Common.IsNull(MediaRight.MediaRightMediaAccess);
                    parameters["@mediaRightMediaExcept"] = Common.IsNull(MediaRight.MediaRightMediaExcept);
                    parameters["@mediaRightRegionAccess"] = Common.IsNull(MediaRight.MediaRightRegionAccess);
                    parameters["@mediaRightRegionExcept"] = Common.IsNull(MediaRight.MediaRightRegionExcept);
                    parameters["@mediaRightVehicleAccess"] = Common.IsNull(MediaRight.MediaRightVehicleAccess);
                    parameters["@mediaRightVehicleExcept"] = Common.IsNull(MediaRight.MediaRightVehicleExcept);

                    #endregion

                    // SP Execute
                    return GetDetailMedia(conn, parameters);
                }
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("GetDetailMedia: Impossible to execute query", err));
            }
        }

        public static DataSet GetDetailMedia(SqlConnection conn, Hashtable parameters)
        {
            try
            {
                DataSet ds = new DataSet();

                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = GetDetailMediaProcedureName;
                cmd.CommandTimeout = ProcedureTimeout;

                CommonDAL.InitParams(parameters, cmd);

        #endregion

                // SP Execute
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(ds);
                }

                conn.Close();


                return ds;
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("GetDetailMedia: Impossible to execute query", err));
            }
        }


        /// <summary>
        /// Get detailed media for Product class analysis in Russia.
        /// with fields [id_vehicle,vehicle,id_region,region,id_media,media]
        /// Where:
        /// id_vehicle : ID of  media type.
        /// id_vehicle : media type label
        /// id_region : ID of  region
        /// region : region label
        /// id_media : ID of  vehicle
        /// media : vehicle label
        /// </summary>
        /// <returns>Dataset with fields [id_vehicle,vehicle,id_region,region,id_media,media]</returns>
        public override DataSet GetRecapDetailMedia(bool getAllDetails = true)
        {
            try
            {
                TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_session.CurrentModule);
                // List of Media available for current module


                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {

                    Hashtable parameters = new Hashtable();

                    parameters["@id_language"] = Convert.ToInt16(_session.DataLanguage);
                    parameters["@allowedMediaTypes"] = Common.IsNull(Common.GetAllowedMediaTypes());

                    #region Set Media Right Data

                    parameters["@mediaRightMediaAccess"] = Common.IsNull(MediaRight.MediaRightMediaAccess);
                    parameters["@mediaRightMediaExcept"] = Common.IsNull(MediaRight.MediaRightMediaExcept);
                    parameters["@mediaRightRegionAccess"] = Common.IsNull(MediaRight.MediaRightRegionAccess);
                    parameters["@mediaRightRegionExcept"] = Common.IsNull(MediaRight.MediaRightRegionExcept);
                    parameters["@mediaRightVehicleAccess"] = Common.IsNull(MediaRight.MediaRightVehicleAccess);
                    parameters["@mediaRightVehicleExcept"] = Common.IsNull(MediaRight.MediaRightVehicleExcept);

                    #endregion

                    // SP Execute
                    return GetRecapDetailMedia(conn, parameters);
                }
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("GetRecapDetailMedia: Impossible to execute query", err));
            }
        }

        public static DataSet GetRecapDetailMedia(SqlConnection conn, Hashtable parameters)
        {
            try
            {
                DataSet ds = new DataSet();


                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = GetRecapDetailMediaProcedureName;
                cmd.CommandTimeout = ProcedureTimeout;

                CommonDAL.InitParams(parameters, cmd);


                // SP Execute
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(ds);
                }

                conn.Close();


                return ds;
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("GetRecapDetailMedia: Impossible to execute query", err));
            }
        }



        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights. It means product or media restriction.
        /// </summary>
        /// <param name="levelId">Identifer of classification level from constantes of type TNS.Classification.Universe.TNSClassificationLevels. For example, for the classification level
        /// product, Identifer will be TNS.Classification.Universe.TNSClassificationLevels.PRODUCT with value 5.         
        /// 
        /// </param>
        /// <param name="wordToSearch">Key word to search</param>
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        public override DataSet GetItems(long levelId, string wordToSearch)
        {
            DataSet ds = new DataSet();
            string classificationLevelLabel = TNS.Classification.Universe.UniverseLevels.Get(levelId).TableName;

            string SelectedMediaType = "";
            SelectedMediaType = _session.CustomerDataFilters.SelectedMediaType;

            #region Execution of the query
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    Hashtable parameters = new Hashtable();

                    parameters["@id_language"] = Convert.ToInt16(_session.DataLanguage);
                    parameters["@type"] = classificationLevelLabel;
                    parameters["@keyword"] = wordToSearch;
                    parameters["@selected_media_type"] = SelectedMediaType;

                    parameters["@customer_media_access"] = MediaRight.MediaRightMediaAccess;
                    parameters["@customer_media_except"] = MediaRight.MediaRightMediaExcept;
                    parameters["@customer_region_access"] = Common.IsNull(MediaRight.MediaRightRegionAccess);
                    parameters["@customer_region_except"] = Common.IsNull(MediaRight.MediaRightRegionExcept);
                    parameters["@customer_vehicle_access"] = MediaRight.MediaRightVehicleAccess;
                    parameters["@customer_vehicle_except"] = MediaRight.MediaRightVehicleExcept;

                    parameters["@customer_sector_access"] = ProductRight.ProductRightCategory1Access;
                    parameters["@customer_sector_except"] = ProductRight.ProductRightCategory1Except;
                    parameters["@customer_subsector_access"] = ProductRight.ProductRightCategory2Access;
                    parameters["@customer_subsector_except"] = ProductRight.ProductRightCategory2Except;
                    parameters["@customer_group_access"] = ProductRight.ProductRightCategory3Access;
                    parameters["@customer_group_except"] = ProductRight.ProductRightCategory3Except;
                    parameters["@customer_subgroup_access"] = ProductRight.ProductRightCategory4Access;
                    parameters["@customer_subgroup_except"] = ProductRight.ProductRightCategory4Except;

                    parameters["@customer_advertiser_access"] = ProductRight.ProductRightAdvertiserAccess;
                    parameters["@customer_advertiser_except"] = ProductRight.ProductRightAdvertiserExcept;
                    parameters["@customer_brand_access"] = ProductRight.ProductRightBrandAccess;
                    parameters["@customer_brand_except"] = ProductRight.ProductRightBrandExcept;

                    return GetItems(conn, parameters, GetItemsByKeywordProcedureName);
                }


            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("Impossible to execute query", err));
            }
            #endregion

        }

        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights. It means product or media restriction.
        ///  
        /// The parameter "levelId" corresponds to classification level Identifier
        /// where items must be included in SELECT clause.
        /// 
        /// The parameters "selectedClassificationLevelIds" and "selectedLevelId"
        /// correspond respectively to identifier items selected and the identifier of classification level  selected by the customer.
        /// It uses to filter data of the result level.
        /// 
        ///  To build the SQL query, the preceding parameters are used as follows :
        /// </summary>
        /// <param name="levelId">Identifer of classification level from constantes of type TNS.Classification.Universe.TNSClassificationLevels. For example, for the classification level
        /// product, Identifer will be TNS.Classification.Universe.TNSClassificationLevels.PRODUCT with value 5.                
        /// </param>
        /// <param name="selectedClassificationLevelIds"> Selected (by user) classification level identifiers list (identifers separated by comma). These identifiers are used
        /// to filter the data.</param>
        /// <param name="selectedLevelId">Selected (by user) classification level Identifier. 
        /// </param>
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        public override DataSet GetItems(long levelId, string selectedClassificationItemsIds, long selectedLevelId)
        {
            string classificationLevelLabel = TNS.Classification.Universe.UniverseLevels.Get(levelId).TableName;
            string selectedClassificationLevelLabel = TNS.Classification.Universe.UniverseLevels.Get(selectedLevelId).TableName;

            string SelectedMediaType = "";
            SelectedMediaType = _session.CustomerDataFilters.SelectedMediaType;

            #region Execution of the query
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    Hashtable parameters = new Hashtable();

                    parameters["@id_language"] = Convert.ToInt16(_session.DataLanguage);
                    parameters["@type"] = classificationLevelLabel;
                    parameters["@type_link"] = selectedClassificationLevelLabel;
                    parameters["@selected_id"] = selectedClassificationItemsIds;

                    parameters["@customer_media_access"] = MediaRight.MediaRightMediaAccess;
                    parameters["@customer_media_except"] = MediaRight.MediaRightMediaExcept;
                    parameters["@customer_region_access"] = Common.IsNull(MediaRight.MediaRightRegionAccess);
                    parameters["@customer_region_except"] = Common.IsNull(MediaRight.MediaRightRegionExcept);
                    parameters["@customer_vehicle_access"] = MediaRight.MediaRightVehicleAccess;
                    parameters["@customer_vehicle_except"] = MediaRight.MediaRightVehicleExcept;

                    parameters["@customer_sector_access"] = ProductRight.ProductRightCategory1Access;
                    parameters["@customer_sector_except"] = ProductRight.ProductRightCategory1Except;
                    parameters["@customer_subsector_access"] = ProductRight.ProductRightCategory2Access;
                    parameters["@customer_subsector_except"] = ProductRight.ProductRightCategory2Except;
                    parameters["@customer_group_access"] = ProductRight.ProductRightCategory3Access;
                    parameters["@customer_group_except"] = ProductRight.ProductRightCategory3Except;
                    parameters["@customer_subgroup_access"] = ProductRight.ProductRightCategory4Access;
                    parameters["@customer_subgroup_except"] = ProductRight.ProductRightCategory4Except;

                    parameters["@customer_advertiser_access"] = ProductRight.ProductRightAdvertiserAccess;
                    parameters["@customer_advertiser_except"] = ProductRight.ProductRightAdvertiserExcept;
                    parameters["@customer_brand_access"] = ProductRight.ProductRightBrandAccess;
                    parameters["@customer_brand_except"] = ProductRight.ProductRightBrandExcept;

                    parameters["@selected_media_type"] = SelectedMediaType;

                    return GetItems(conn, parameters, GetItemsByLinkProcedureName);
                }
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("Impossible to execute query", err));
            }
            #endregion

        }

        public static DataSet GetItems(SqlConnection conn, Hashtable parameters, string procedureName)
        {
            DataSet ds = new DataSet();

            #region Execution of the query
            try
            {

                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                // SP Construction    
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procedureName;
                cmd.CommandTimeout = ProcedureTimeout;

                CommonDAL.InitParams(parameters, cmd);

                // SP Execute
                SqlDataReader rs = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                using (rs)
                {
                    dt.Load(rs);
                }

                ds.Tables.Add(dt);
                conn.Close();

            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("Impossible to execute query", err));
            }
            #endregion

            return ds;
        }

        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights. It means product or media restriction.
        ///  <remarks>This method is use only for the modules  " Product class analysis: Graphic key reports "
        ///  and "Product class analysis: Detailed reports"
        /// 
        /// /// - We use the parameter "classificationLevelLabel" to build the SQL query fields as follows :
        ///  <code>
        ///  public virtual DataSet GetItems(string classificationLevelLabel, CustomerRightConstante.type customerRightType){
        ///  
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        ///  //Set Table of the query with the current classification level 
        ///  
        ///   /*FROM clause : get the targets tables of the query*/
        /// sql.AppendFormat(" from {0} pr ",classificationLevelLabel)
        /// 
        /// sql.AppendFormat(" order by  {0} ", classificationLevelLabel);
        /// ...
        /// }
        /// </code>
        /// 
        /// </remarks>
        ///  <example>
        /// To search data for vehicle level :
        /// <code>
        /// Select distinct wp.id_media as id_item, wp.media  as item
        /// 
        /// //Query on table of vehicle classification
        /// from media wp
        ///                
        /// //vehicle's rights
        /// and wp.id_media in (1,2,3)
        /// 
        /// ...
        /// 
        /// //Order by vehicle label
        /// order by media
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="classificationLevelLabel">Label of classification level. For example, for the classification level
        /// product, the label will be "product". It corresponds also to the current classification level's table.</param>
        /// <param name="customerRightType">Enumerator allowing to get user rights of the current classification level.</param>          
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        public override DataSet GetRecapItems(string classificationLevelLabel, CustomerRightConstante.type customerRightType)
        {
            //Calling the engine which compute data
            if (_dBSchema == null || _dBSchema.Length == 0)
                throw (new ArgumentException("Invalid dBSchema parameter"));//Excepted for france data base, can be null for other country           
            ClassificationItemsDAL engineDal = new ClassificationItemsDAL(_session, _dimension);
            engineDal.DBSchema = _dBSchema;
            engineDal.DataSource = _dataSource;
            return engineDal.GetRecapItems(classificationLevelLabel, customerRightType);
        }

        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights (it means product or media restriction) and identiifer 
        ///  list of classification level's items.
        /// <remarks>     
        /// - We use the parameter "classificationLevelLabel" to build the SQL query fields as follows :
        ///  <code>
        ///  public virtual DataSet GetItems(string classificationLevelLabel,, string idList){
        ///  
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        /// 
        ///  //Set Table of the query with the current classification level      
        /// sql.AppendFormat(" from {0} pr ",classificationLevelLabel)
        /// 
        /// /*Query conditions */
        ///   /* Restriction on the classification items selected by the customer*/
        /// sql.AppendFormat(" where wp.id_{0} in ({1})", classificationLevelLabel, idList);
        /// 
        /// //ordr fields
        /// sql.AppendFormat(" order by  {0} ", classificationLevelLabel);
        /// ...
        /// }
        /// </code>      
        /// </remarks>      
        ///  <example>
        /// To find vehicle level items with identifier list (5,56,59,56) the query string generated will be :
        /// <code>
        /// Select distinct wp.id_media as id_item, wp.media  as item
        /// 
        /// //Query on View of vehicle classification
        /// from all_media_33 wp
        /// 
        /// //Filtering with vehicles ID
        /// where wp.id_media in (5,56,59,56) 
        /// 
        /// //media's rights
        /// and wp.id_vehicle in (1,2,3)
        /// 
        /// ...
        /// 
        /// //Order by vehicle label
        /// order by media
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="classificationLevelLabel">Label of classification level. For example, for the classification level
        /// product, the label will be "product". It corresponds also to the current classification level's table.</param>
        ///<param name="selectedClassificationLevelIds"> Selected (by user) classification level identifiers list (identifers separated by comma). Identifiers are used
        /// to filter the data.</param>
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        public override DataSet GetSelectedItems(string classificationLevelLabel, string idList)
        {
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    Hashtable parameters = new Hashtable();

                    parameters["@id_language"] = Convert.ToInt16(_session.DataLanguage);
                    parameters["@list_type"] = classificationLevelLabel;
                    parameters["@id_list"] = idList;
                    parameters["@customer_sector_access"] = ProductRight.ProductRightCategory1Access;
                    parameters["@customer_sector_except"] = ProductRight.ProductRightCategory1Except;
                    parameters["@customer_subsector_access"] = ProductRight.ProductRightCategory2Access;
                    parameters["@customer_subsector_except"] = ProductRight.ProductRightCategory2Except;
                    parameters["@customer_group_access"] = ProductRight.ProductRightCategory3Access;
                    parameters["@customer_group_except"] = ProductRight.ProductRightCategory3Except;
                    parameters["@customer_subgroup_access"] = ProductRight.ProductRightCategory4Access;
                    parameters["@customer_subgroup_except"] = ProductRight.ProductRightCategory4Except;

                    return GetSelectedItems(conn, parameters);
                }
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("Impossible to execute query", err));
            }
        }

        public static DataSet GetSelectedItems(SqlConnection conn, Hashtable parameters)
        {
            DataSet ds = new DataSet();

            try
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                // SP Construction    
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = GetSelectedItemsProcedureName;
                cmd.CommandTimeout = ProcedureTimeout;

                // SP Construction    
                CommonDAL.InitParams(parameters, cmd);

                // SP Execute
                SqlDataReader rs = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                using (rs)
                {
                    dt.Load(rs);
                }

                if (dt.Rows.Count > 0)
                {
                    ds.Tables.Add(dt);

                }

                conn.Close();

            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("Impossible to execute query", err));
            }
            return ds;
        }

        #region GetSectors
        /// <summary>
        ///This method is used in Graphic key reports module  to get a list of Sectors
        ///corresponding to product classification items selected. 
        /// </summary>
        /// <returns>Dataset with  sectors list</returns>
        public override DataSet GetSectors()
        {
            try
            {
                //TODO : implements in Russia : 
                //Get Category L1 corresponding to product classifications items selected in Product class analysis: Graphic key reports
                //module.
                //return fields [id_sector,sector] for Category L1 ID and Category L1 label.
                //Filter query with product classification items selected and store in property : TNS.AdExpress.Web.Core.CustomerDataFilters.PrincipalProductUniverses
                //Filter query with _session.DataLanguage

                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {

                    ProductClassification productClass = Common.GetProductClassification();

                    Hashtable parameters = new Hashtable();

                    parameters["@id_language"] = Convert.ToInt16(_session.DataLanguage);

                    #region Set Product Classification Data

                    parameters["@productClassAdvertiserAccess1"] = Common.IsNull(productClass.AdvertiserAccess1);
                    parameters["@productClassAdvertiserAccess2"] = Common.IsNull(productClass.AdvertiserAccess2);
                    parameters["@productClassAdvertiserExcept"] = Common.IsNull(productClass.AdvertiserExcept);
                    parameters["@productClassBrandAccess1"] = Common.IsNull(productClass.BrandAccess1);
                    parameters["@productClassBrandAccess2"] = Common.IsNull(productClass.BrandAccess2);
                    parameters["@productClassBrandExcept"] = Common.IsNull(productClass.BrandExcept);
                    parameters["@productClassSubBrandAccess1"] = Common.IsNull(productClass.SubBrandAccess1);
                    parameters["@productClassSubBrandAccess2"] = Common.IsNull(productClass.SubBrandAccess2);
                    parameters["@productClassSubBrandExcept"] = Common.IsNull(productClass.SubBrandExcept);
                    parameters["@productClassProductAccess1"] = Common.IsNull(productClass.ProductAccess1);
                    parameters["@productClassProductAccess2"] = Common.IsNull(productClass.ProductAccess2);
                    parameters["@productClassProductExcept"] = Common.IsNull(productClass.ProductExcept);
                    parameters["@productClassCategory1Access1"] = Common.IsNull(productClass.Category1Access1);
                    parameters["@productClassCategory1Access2"] = Common.IsNull(productClass.Category1Access2);
                    parameters["@productClassCategory1Except"] = Common.IsNull(productClass.Category1Except);
                    parameters["@productClassCategory2Access1"] = Common.IsNull(productClass.Category2Access1);
                    parameters["@productClassCategory2Access2"] = Common.IsNull(productClass.Category2Access2);
                    parameters["@productClassCategory2Except"] = Common.IsNull(productClass.Category2Except);
                    parameters["@productClassCategory3Access1"] = Common.IsNull(productClass.Category3Access1);
                    parameters["@productClassCategory3Access2"] = Common.IsNull(productClass.Category3Access2);
                    parameters["@productClassCategory3Except"] = Common.IsNull(productClass.Category3Except);
                    parameters["@productClassCategory4Access1"] = Common.IsNull(productClass.Category4Access1);
                    parameters["@productClassCategory4Access2"] = Common.IsNull(productClass.Category4Access2);
                    parameters["@productClassCategory4Except"] = Common.IsNull(productClass.Category4Except);

                    #endregion

                    return GetSectors(conn, parameters);
                }

            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("GetSectors: Impossible to execute query", err));
            }
        }

        public static DataSet GetSectors(SqlConnection conn, Hashtable parameters)
        {
            DataSet ds = new DataSet();

            try
            {
                SqlCommand cmd = conn.CreateCommand();
                conn.Open();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = GetSectorsProcedureName;
                cmd.CommandTimeout = ProcedureTimeout;

                // SP Construction    
                CommonDAL.InitParams(parameters, cmd);

                // SP Execute
                SqlDataReader rs = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                using (rs)
                {
                    dt.Load(rs);
                }

                if (dt.Rows.Count > 0)
                {
                    ds.Tables.Add(dt);

                }

                conn.Close();

            }
            catch (System.Exception err)
            {
                throw (new Exceptions.ClassificationItemsDALException("GetSectors: Impossible to execute query", err));
            }
            return ds;
        }
        #endregion

    }
}
