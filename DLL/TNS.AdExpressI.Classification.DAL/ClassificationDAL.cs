#region Information
/*
 * Author : Y Rkaina && D. Mussuma
 * Created on : 15/07/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using VehicleClassificationCst = TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using DBClassificationCst = TNS.AdExpress.Constantes.Classification.DB;

using TNS.FrameWork.DB.Common;
using TNS.Classification.Universe;
using System.Reflection;

namespace TNS.AdExpressI.Classification.DAL {

	/// <summary>
    /// This class provides all the SQL queries to search or select items of the product or vehicle
    /// clasification brand.
    /// The data can be filtered according to the rights or the selections of the customer.
    /// It contains the methods :
    /// - <code>GetMediaType();</code> Which provides to the customer the media items to select into a module.
    /// - <code>GetDetailMedia();</code> get the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
    /// According to the vehicle classification levels choosen by the customer.
    /// - <code>GetDetailMedia(string keyWord);</code> Obtains the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
    /// According to the vehicle classification levels choosen by the customer. Depends also on the classification label searched by the customer (keyWord).
    /// <code>GetItems;</code> All the methods with the same name provide to the customer the possibility to search items in product or vehicle classification brand.
    /// <code>GetRecapItems</code> provides to the customer the possibility to search items in product or vehicle classification brand. 
    /// <remarks>This methods is used only intio the modules
    /// " Product class analysis: Graphic key reports " and "Product class analysis: Detailed reports".</remarks>
	/// </summary>
    /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
    /// Impossible to execute query
    /// </exception>
    /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
    /// execution or building of the query to search classification items</exception>
	public abstract class ClassificationDAL : IClassificationDAL {

		#region Variables
		/// <summary>
		/// User session
		/// </summary>
		protected WebSession _session = null;
		/// <summary>
		/// Generic detail level selected by the customer (ex. Sub Media\Vehicle)
		/// </summary>
		protected GenericDetailLevel _genericDetailLevel = null;
		/// <summary>
		/// Identifier list of vehicle to find (ex. 20,205,301)
		/// </summary>
        protected string _vehicleList = "";
		/// <summary>
		/// Current classification brand (product or vehicle)
		/// </summary>
		protected  TNS.Classification.Universe.Dimension _dimension;
        /// <summary>
        /// Data base schema
        /// </summary>
        protected string _dBSchema = null;
        /// <summary>
        /// Filters (that we can apply for a specific level)
        /// we can add severals filters
        /// The key represents the level filter
        /// The value represents the list of ids to exclude (example of a list : 9999,999,2541)
        /// </summary>
        protected Dictionary<long, string> _filters = new Dictionary<long, string>();
        /// <summary>
        /// Get if data items shiould be in lower case
        /// </summary>
        protected bool _toLowerCase = false;
        /// <summary>
        /// Check if can filtered query with selection
        /// </summary>
        protected bool _filterWithProductSelection = false;

        /// <summary>
        /// Data Source
        /// </summary>
        protected TNS.FrameWork.DB.Common.IDataSource _dataSource = null;
        /// <summary>
		#endregion

		#region Constructors
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>		
		public ClassificationDAL(WebSession session) {
            //Set  customer web session
			_session = session;
            //Get data source
            _dataSource = GetDataSource();
            _toLowerCase = true;
		}
		
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
        /// <param name="dimension">Product or vehicle classification brand</param>
		public ClassificationDAL(WebSession session, TNS.Classification.Universe.Dimension dimension)
			: this(session) {
                //Set  Current classification brand (product or vehicle)
			_dimension = dimension;
            //Get data source
            _dataSource = GetDataSource();
            _toLowerCase = true;
		}
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="session">User session</param>
        /// <param name="genericDetailLevel">Generic detail level selected by the customer (ex. Sub Media\Vehicle)</param>
        /// <param name="vehicleList">List of vehicles selected by the user</param>
        public ClassificationDAL(WebSession session, GenericDetailLevel genericDetailLevel, string vehicleList)
			: this(session) {
                //Set Generic detail level selected by the customer (ex. Sub Media\Vehicle)
			_genericDetailLevel = genericDetailLevel;
            //Set List of vehicles selected by the user
            _vehicleList = vehicleList;
            //Get data source
            _dataSource = GetDataSource();
            _toLowerCase = true;
		}
		#endregion

        #region Properties
        /// <summary>
        /// Database schema
        /// <remarks>Can be null for others country execpted for france</remarks>
        /// </summary>
        public string DBSchema{
            set {
                _dBSchema = value;
            }        
        }
        /// <summary>
        /// Get/Set Control filters
        /// </summary>
        public Dictionary<long, string> Filters{
            get { return _filters; }
            set { _filters = value; }
        }
        /// <summary>
        /// Get if can filtered query with selection
        /// </summary>
        public bool FilterWithProductSelection
        {
            get { return _filterWithProductSelection; }
            set { _filterWithProductSelection = value; }
        }

        /// <summary>
        ///  Get if data items shiould be in lower case
        /// </summary>
        public bool ToLowerCase
        {
            get
            {
                return _toLowerCase;
            }
        }

        #endregion

        #region IClassificationDAL Implementation

        #region Get Media Type
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
		public virtual DataSet GetMediaType() {
            //Calling the engine which compute data
			VehiclesDAL engineDal = new VehiclesDAL(_session);
            engineDal.DataSource = _dataSource;
			return engineDal.GetData();
		}
		#endregion

        #region Get Detail Product
        /// <summary>
        /// Get the list of vehicles organised by product
        /// According to the vehicle classification levels choosen by the customer.
        /// See Vehicles.xml configuration file (ProductSelectionDetailLevel tag)
        /// </summary>	
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.
        /// </exception>
        public virtual DataSet GetDetailProduct() {
            //Calling the engine which compute data
            DetailProductDAL engineDal = new DetailProductDAL(_session, _genericDetailLevel, _vehicleList);
            return engineDal.GetData();
        }
        #endregion

        #region Get Detail Media
        /// <summary>
        /// Get the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
        /// According to the vehicle classification levels choosen by the customer.
        /// <example>
        /// If the user wants to select vehicles displayed by Sub media\Vehicle, the clause select of the query will be
        /// : select distinct id_category as idDetailMedia, category as detailMedia, id_media, media from ...
        /// </example>
        /// </summary>	
        /// <returns>Dataset with  a dataset with table of 4 columns : "idDetailMedia", "detailMedia", "id_media", "media".
        /// The column "idDetailMedia" corresponds to the identifier of the level parent.
        /// The column "detailMedia" corresponds to the label of the level parent.
        /// The column "id_media" corresponds to the identifier of the vehicle.
        /// The column "media" corresponds to the label of the vehicle.
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
		public virtual DataSet GetDetailMedia() {
            //Calling the engine which compute data
			DetailMediaDAL engineDal = new DetailMediaDAL(_session,_genericDetailLevel,_vehicleList);
            engineDal.DataSource = _dataSource;
			return engineDal.GetData();
		}

        /// <summary>
        /// Get the list of vehicles organised by Media genre or Sub media or Media Owner or Title.
        /// According to the vehicle classification levels choosen by the customer.
        /// <example>
        /// If the user wants to select vehicles displayed by Sub media\Vehicle, the clause select of the query will be
        /// : select distinct id_category as idDetailMedia, category as detailMedia, id_media, media from ...
        /// </example>
        /// </summary>	
        /// <param name="keyWord">key word to search  corresponding vehicles </param>
        /// <returns>Dataset with  a data set with table of 4 columns : "idDetailMedia", "detailMedia", "id_media", "media".
        /// The column "idDetailMedia" corresponds to the identifier of the level parent.
        /// The column "detailMedia" corresponds to the label of the level parent.
        /// The column "id_media" corresponds to the identifier of the vehicle.
        /// The column "media" corresponds to the label of the vehicle.
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
		public virtual DataSet GetDetailMedia(string keyWord) {
            //Calling the engine which compute data
			DetailMediaDAL engineDal = new DetailMediaDAL(_session, _genericDetailLevel, _vehicleList);
            engineDal.DataSource = _dataSource;
			return engineDal.GetData(keyWord);
		}
		#endregion

        #region  GetSubMediaData
        /// <summary>
        /// Get the list of sub media corresponding to media type selected
        /// </summary>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Exception throwed when an error occurs in the building or execution of the SQL query.</exception>
        /// <returns>Dataset with  a data table of 2 columns : "Id_SubMedia", "SubMedia".
        /// The column "Id_SubMedia" corresponds to the identifier of the level sub media.
        /// The column "SubMedia" corresponds to the label of the level sub media.       
        /// </returns>
        public virtual DataSet GetSubMediaData()
        {
            DetailMediaDAL engineDal = new DetailMediaDAL(_session, _genericDetailLevel, _vehicleList);
            engineDal.DataSource = _dataSource;
            return engineDal.GetSubMediaData();
        }
        #endregion

        #region GetRecapDetailMedia
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
        public virtual DataSet GetRecapDetailMedia()
        {
            WebSession webSession = _session;
            DataSet ds = new DataSet();
            string sql = "";
            bool isRecap = true;
            Table vehicleTable = null, categoryTable = null, basicMediaTable = null, mediaTable = null;
            vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapVehicle);
            categoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCategory);
            mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMedia);
            int activationCode = (isRecap) ? DBConstantes.ActivationValues.DEAD : DBConstantes.ActivationValues.UNACTIVATED;
            WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);

            sql = "Select distinct " + vehicleTable.Prefix + ".id_vehicle," + vehicleTable.Prefix + ".vehicle ";
            if (isRecap) sql += ", " + categoryTable.Prefix + ".id_category as id_region," + categoryTable.Prefix + ".category as region, " + mediaTable.Prefix + ".id_media ," + mediaTable.Prefix + ".media";
            sql += " from " + vehicleTable.SqlWithPrefix + ",";
            sql += categoryTable.SqlWithPrefix + ",";
            if (!isRecap) sql += basicMediaTable.SqlWithPrefix + ",";
            sql += mediaTable.SqlWithPrefix + " ";
            sql += " where";
            // Langue
            sql += " " + vehicleTable.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
            sql += " and " + categoryTable.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
            if (!isRecap) sql += " and " + basicMediaTable.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
            sql += " and " + mediaTable.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
            // Activation
            sql += " and " + vehicleTable.Prefix + ".activation<" + activationCode;
            sql += " and " + categoryTable.Prefix + ".activation<" + activationCode;
            if (!isRecap) sql += " and " + basicMediaTable.Prefix + ".activation<" + activationCode;
            sql += " and " + mediaTable.Prefix + ".activation<" + activationCode;

            // Jointure
            if (isRecap)
            {
                sql += " and " + vehicleTable.Prefix + ".id_vehicle=" + categoryTable.Prefix + ".id_vehicle";
                sql += " and " + categoryTable.Prefix + ".id_category=" + mediaTable.Prefix + ".id_category";
            }
            else
            {
                sql += " and " + vehicleTable.Prefix + ".id_vehicle=" + categoryTable.Prefix + ".id_vehicle";
                sql += " and " + categoryTable.Prefix + ".id_category=" + basicMediaTable.Prefix + ".id_category";
                sql += " and " + basicMediaTable.Prefix + ".id_basic_media=" + mediaTable.Prefix + ".id_basic_media";
            }

            //Media universe
            if (module != null)
                sql += module.GetAllowedMediaUniverseSql(vehicleTable.Prefix, categoryTable.Prefix, mediaTable.Prefix, true);

            //Media Rights
            sql += TNS.AdExpress.Web.Core.Utilities.SQLGenerator.getAccessVehicleList(webSession, vehicleTable.Prefix, true);


            sql += " order by " + vehicleTable.Prefix + ".vehicle," + vehicleTable.Prefix + ".id_vehicle";
            if (isRecap) sql += ", " + categoryTable.Prefix + ".category," + categoryTable.Prefix + ".id_category, " + mediaTable.Prefix + ".media ," + mediaTable.Prefix + ".id_media";

            #region Execution of the query
            try
            {
                //Execution of the query
                return WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis,WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].NlsSort).Fill(sql.ToString());

            }
            catch (System.Exception err)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible to load data for the media detail", err));
            }
            #endregion

           // throw new NotImplementedException(" This query should be only implemented in Russia");
        }
        #endregion

        #region Get Items
        /// <summary>
        ///  Search function for Product or vehicle classification Items. The data will be filter 
        ///  with customer classification rights. It means product or media restriction.
        ///  <example>
        /// To search data for vehicle level :
        /// <code>
        /// Select distinct wp.id_media as id_item, wp.media  as item
        /// 
        /// //Query on View of media classification
        /// from all_media_33 wp
        /// 
        /// //Search by label
        /// where wp.media like '%CNN%'
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
        /// <param name="levelId">Identifer of classification level from constantes of type TNS.Classification.Universe.TNSClassificationLevels. For example, for the classification level
        /// product, Identifer will be TNS.Classification.Universe.TNSClassificationLevels.PRODUCT with value 5.         
        /// 
        /// </param>
        /// <param name="wordToSearch">Key word to search</param>
        /// <remarks>     
        /// - We use the parameter "levelId" to build the SQL query fields as follows :
        ///  <code>
        ///  public virtual DataSet GetItems(string levelId, string wordToSearch){
        ///  
        ///    string classificationLevelLabel = UniverseLevels.Get(levelId).TableName;
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        ///  //Set Table of the query with the current classification level      
        /// sql.AppendFormat(" from {0} pr ",classificationLevelLabel)
        /// sql.AppendFormat(" order by  {0} ", classificationLevelLabel);
        /// ...
        /// }
        /// </code>
        /// 
        /// Then if parameter "classificationLevelLabel" is equal to "product " the query string will be as follows :
        /// <code>
        /// select distinct pr.id_product as id_item, pr.product as item
        /// from product pr 
        /// order by product
        /// </code>
        /// </remarks>        
        /// <returns>Data set with data table[id_item,item] : identifer and label of a level of brand classification</returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationItemsDALException">Throw exception when error occurs during 
        /// execution or building of the query</exception>
        public virtual DataSet GetItems(long levelId , string wordToSearch)
        {
           string classificationLevelLabel = UniverseLevels.Get(levelId).TableName;
            //Calling the engine which compute data
            if (_dBSchema == null || _dBSchema.Length == 0)
                throw (new ArgumentException("Invalid dBSchema parameter"));//Excepted for france data base, can be null for other country         
            ClassificationItemsDAL engineDal = new ClassificationItemsDAL(_session,_dimension);
            engineDal.DBSchema = _dBSchema;
            engineDal.Filters = _filters;
            engineDal.FilterWithProductSelection = _filterWithProductSelection;            
            engineDal.DataSource = _dataSource;
            return engineDal.GetItems(classificationLevelLabel, wordToSearch);
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
        ///  <example>              
        ///  <code>
        ///  public virtual DataSet GetItems(string levelId, string selectedClassificationLevelIds, string selectedLevelId){
        ///   string classificationLevelLabel = UniverseLevels.Get(levelId).TableName;
        ///  string selectedClassificationLevelLabel = UniverseLevels.Get(selectedLevelId).TableName; 
        ///  View oView = ...//Get classification brand view : all_product_33 or all_media_44 (each one contains all items of its classification) 
        /// ...
        /// //Set fields of the query with the current classification level
        /// sql.AppendFormat(" select distinct pr.id_{0} as id_item, pr.{0} as item ", classificationLevelLabel);
        /// 
        ///   /*FROM clause : Obtains the targets tables of the query according to the web site data language*/
        ///  sql.AppendFormat(" from {0}{1} wp", oView.Sql,_session.DataLanguage.ToString());
        /// 
        ///  /*Query conditions */ 
        ///    * Restriction on the classification items selected by the customer*/
        ///	    sql.AppendFormat(" where wp.id_{0} in ( {1} ) ",selectedClassificationLevelLabel,selectedClassificationLevelIds);
        /// ...
        /// 
        ///  /*Query orders fields by items' labels */
        ///     sql.AppendFormat(" order by {0} ", classificationLevelLabel);
        /// }
        /// </code>
        /// 
        /// Then if parameters of the method are :
        ///  <code>
        ///  //Current classification level is product     
        /// Will have value TNS.Classification.Universe.TNSClassificationLevels.PRODUCT
        ///
        /// //If user has selected advertiser level
        /// string  selectedClassificationLevelLabel="advertiser";
        /// string selectedClassificationLevelIds ="1,35,56,35";
        ///  
        ///  </code>
        ///  
        /// The query string will be as follows :       
        /// <code>
        /// 
        /// //Get product ID and label
        /// Select distinct wp.id_product as id_item, wp.product  as item
        /// 
        /// //Query on product classification View
        /// from all_product_33 wp
        /// 
        /// //Filter by advertiser to obtain associated product
        /// where wp.advertiser in (1,35,56,35) 
        /// 
        /// //product's rights      
        /// ...
        /// 
        /// //Order by product label
        /// order by product
        /// </code>
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
        public virtual DataSet GetItems(long levelId, string selectedClassificationItemsIds, long selectedLevelId)
        {
            string classificationLevelLabel = UniverseLevels.Get(levelId).TableName;
            string selectedClassificationLevelLabel = UniverseLevels.Get(selectedLevelId).TableName; 

            //Calling the engine which compute data
            if (_dBSchema == null || _dBSchema.Length == 0)
                throw (new ArgumentException("Invalid dBSchema parameter"));//Excepted for france data base, can be null for other country         
            ClassificationItemsDAL engineDal = new ClassificationItemsDAL(_session, _dimension);
            engineDal.DBSchema = _dBSchema;
            engineDal.Filters = _filters;
            engineDal.FilterWithProductSelection = _filterWithProductSelection;           
            engineDal.DataSource = _dataSource;
            return engineDal.GetItems(classificationLevelLabel, selectedClassificationItemsIds, selectedClassificationLevelLabel);
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
        public virtual DataSet GetSelectedItems(string classificationLevelLabel, string idList)
        {
            //Calling the engine which compute data
            if (_dBSchema == null || _dBSchema.Length == 0)
                throw (new ArgumentException("Invalid dBSchema parameter"));//Excepted for france data base, can be null for other country         
            ClassificationItemsDAL engineDal = new ClassificationItemsDAL(_session, _dimension);
            engineDal.DBSchema = _dBSchema;
            engineDal.FilterWithProductSelection = _filterWithProductSelection;
            engineDal.DataSource = _dataSource;
            return engineDal.GetSelectedItems(classificationLevelLabel, idList);
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
        public virtual DataSet GetRecapItems(string classificationLevelLabel, CustomerRightConstante.type customerRightType)
        {
            //Calling the engine which compute data
            if (_dBSchema == null || _dBSchema.Length == 0)
                throw (new ArgumentException("Invalid dBSchema parameter"));//Excepted for france data base, can be null for other country           
            ClassificationItemsDAL engineDal = new ClassificationItemsDAL(_session, _dimension);
            engineDal.DBSchema = _dBSchema;
            engineDal.DataSource = _dataSource;
            return engineDal.GetRecapItems(classificationLevelLabel, customerRightType);
		}
		#endregion

        #region GetSectors
        /// <summary>
        ///This method is used in Graphic key reports module  to get a list of Sectors
        ///corresponding to product classification items selected. 
        /// </summary>
        /// <returns>Dataset with  sectors list</returns>
        public virtual DataSet GetSectors()
        {
            #region Variables
            StringBuilder sql = new StringBuilder(2000);
            DataSet ds = new DataSet();            
           
            #endregion

            #region Construction de la requête
            try
            {
                View oView = WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allRecapProduct);
                Table sectorTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.sector);

                // The query that is being used to collect sectors from the database using selected groups.
                sql.Append("Select distinct ");

                sql.Append(oView.Prefix + ".ID_" + sectorTable.Label + "," + oView.Prefix + "." + sectorTable.Label);

                sql.Append(" from " + oView.Sql + _session.DataLanguage + " " + oView.Prefix );


                // Product Selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                {
                    sql.Append(" where ");
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(oView.Prefix, false));
                }
           
            }
            catch (System.Exception e)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible to build sql query " + e.Message));
            }
            #endregion

            #region Execution de la requête
            try
            {
                IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].NlsSort);
                ds = source.Fill(sql.ToString());
                return (ds);
            }
            catch (System.Exception err)
            {
                throw (new Exceptions.DetailMediaDALException("Impossible de charger une liste de Famille pour le rappel de la sélection", err));
            }
            #endregion
        }
        #endregion

       

        /// <summary>
        /// Get Data Source
        /// </summary>
        /// <returns></returns>
        protected virtual TNS.FrameWork.DB.Common.IDataSource GetDataSource()
        {
            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.sourceProvider];
            object[] param = new object[1];
            param[0] = _session;
            if (cl == null) throw (new NullReferenceException("Core layer is null for the source provider layer"));
            TNS.AdExpress.Web.Core.ISourceProvider sourceProvider = (TNS.AdExpress.Web.Core.ISourceProvider)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            return sourceProvider.GetSource();

        }



        

		#endregion
	}
}
