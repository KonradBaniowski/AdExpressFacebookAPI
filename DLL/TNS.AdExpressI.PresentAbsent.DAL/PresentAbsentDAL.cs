#region Information
/*
 * Author : G Ragneau
 * Created on 31/03/2008
 * Modifications :
 *      Ahtour - Date - Description
 * 
 * 
 * */
#endregion

#region Using
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;

using TNS.AdExpressI.PresentAbsentDAL.Exceptions;

using TNS.FrameWork.DB;

using CstDBDesc = TNS.AdExpress.Domain.DataBaseDescription;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctWeb = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using System.Reflection;
using System.Collections;
using TNS.AdExpress.Web.Core.Utilities;

#endregion

namespace TNS.AdExpressI.PresentAbsent.DAL{


	/// <summary>
	/// Extract data for different type of results of the module Present / Absent Report.
	/// It contains the following public methods :
	/// The method <code>DataTable GetData(); </code> get data of the following types of results :
	/// - Result "Portofolio" : get data corresponding to active products items in the set being searched.
	/// - Result "Present in more than one vehicle" : get data corresponding to products items present 
	/// in more tha one vehicle
	/// - Result "Absent" :  get data corresponding to products items absent from the reference vehicles 
	/// in comparaison with the competing vehicles
	/// - Result "Exclusive to one vehicle" : get data corresponding to the reference vehicles' exclusive 
	/// products items in comparaison with the competing vehicles.
	/// - Result "strenghs" : get data corresponding to the product items whose market share is greater 
	/// than that of the total of the product set, int the reference vehicle set.
	/// - Result "Prospects" : get data corresponding to the product items whose market share is less 
	/// than that of the total of the product set, int the reference vehicle set.
	/// 
	/// All previous results call  method <code>DataSet GetData()</code>.
	/// 
	/// - Result "Summary" : get data corresponding (calls method <code>GetSynthesisData()</code>)to the summary of Present, Absent and 
	///Exclusive items on the main hierarchical levels of the product brand classification.         
	/// 
	/// The method <code>DataSet GetSynthesisData();</code>
	/// loads data for result tab "Summary" : get data corresponding to the summary of Present, Absent and 
	///Exclusive items on the main hierarchical levels of the product brand classification. 
	///  
	/// In the result page, client can choose the vehicle-level detail in column by selecting it from the drop-down menu.
	/// The method  <code>DataSet GetColumnDetails();</code>
	/// gets the list of items corresponding to the vehicle-level selected.	
	///	The method <code>DataSet GetNbParutionData();</code>
	/// Get for each vehicles and period selected  the number of publications
	/// </summary>
	public abstract class PresentAbsentDAL:IPresentAbsentResultDAL
    {
        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current vehicle
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        #endregion

        #region Accessors
        /// <summary>
        /// Get User Session
        /// </summary>
        protected WebSession Session
        {
            get { return _session; }
        }
        /// <summary>
        /// Get medium selected
        /// </summary>
        protected VehicleInformation VehicleInformationObject
        {
            get { return _vehicleInformation; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Customer session which contains user configuration parameters and universe selection</param>
        public PresentAbsentDAL(WebSession session)
        {
            this._session = session;

            #region Sélection du vehicle
            string vehicleSelection = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new PresentAbsentDALException("Selection of media is not correct"));
            _vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            #endregion

           
        }
        #endregion

        #region Get Synthesis Data

        #region GetSynthesisRequest
		/// <summary>
		///Build SQL query result tab "Summary" : get data corresponding to the summary of Present, Absent and 
		///Exclusive items on the main hierarchical levels of the product brand classification.         
		///The data table returned contains data in the following order :
		/// <code>select id_sector,id_subsector, d_group_
		/// , id_advertiser,id_brand,id_product,ID_GROUP_ADVERTISING_AGENCY,ID_ADVERTISING_AGENCY
		/// ,id_media, date_num, euro
		/// </code>
		/// So you ca notice that the firts columns correspond to identifiers of the classification product ( from id_sector to id_product).
		/// Then the data columns of advertising agency ("ID_GROUP_ADVERTISING_AGENCY,ID_ADVERTISING_AGENCY").
		/// and column of the identifier of vehicle ("id_media"). 
		/// The column fo current dates of the selection. The "date_num" is an alias.
		/// The field euro is an example of unit's field which depends on the unit selected. 
		/// The unit label changes according to the unit selected by the client (ex. duration, spot).
		///</summary>
		/// <param name="type">Database table type to use according to the period selected format : monthly ; weekly or dayly.</param>
		///<remarks> Use the protected method <code>GetSynthesisRequest(CstDB.TableType.Type type);</code> which get summary data
		///accordind to the type of period (monthly, weekly, dayly).</remarks>
		/// <returns>SQL Query string</returns>
		/// <exception cref="NS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
		/// Exception throwed when an error occurs in the method</exception>		        
        protected virtual string GetSynthesisRequest(CstDB.TableType.Type type)
        {

            #region Constantes
            //Prefix of the table to use
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables
            string dataTableName = "";
            string unitFieldNameSumWithAlias = "";
            string dataJointForInsert = "";
            StringBuilder sql = new StringBuilder(3000);
            string dateField = "";
            string universFilter = string.Empty;

			//Customer period selected
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;

            string groupByOptional = string.Empty;
            string fromOptional = string.Empty;
            string joinOptional = string.Empty;

			//Determine if result must contain the product level
            bool showProduct = _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            #endregion

            #region Tables
			//Get SQL tables to use in the query
            Table tblWebPlan = WebApplicationParameters.GetDataTable(TableIds.monthData, _session.IsSelectRetailerDisplay);
            Schema schAdExpress = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
			Table tblAdvertisingAgengy = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency);
			Table tblGroupAdvertisingAgengy = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.groupAdvertisingAgency);
            #endregion

            #region Construction de la requête
            try {
				// Get table name according to the table type parameter
                switch (type) {
                    case CstDB.TableType.Type.dataVehicle4M:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id,CstWeb.Module.Type.alert, _session.IsSelectRetailerDisplay);
                        break;
                    case CstDB.TableType.Type.dataVehicle:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.analysis, _session.IsSelectRetailerDisplay);
                        break;
                    case CstDB.TableType.Type.webPlan:
                        dataTableName = tblWebPlan.SqlWithPrefix;
                        break;
                    default:
                        throw (new PresentAbsentDALException("Unable to determine type of table to use."));
                }
                // Get Date field according to the target data table. 
                switch (type) {
                    case CstDB.TableType.Type.dataVehicle4M:
                    case CstDB.TableType.Type.dataVehicle:
                        dateField = DATA_TABLE_PREFIXE + "." + CstDB.Fields.DATE_MEDIA_NUM;
                        break;
                    case CstDB.TableType.Type.webPlan:
                        dateField = DATA_TABLE_PREFIXE + "." + CstDB.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                        break;
                }

                //Get Universes filters
                universFilter = GetUniversFilter(type, dateField, customerPeriod);

                // get units fields according to customer unit selection (ex. EURO)
				unitFieldNameSumWithAlias = FctWeb.SQLGenerator.GetUnitFieldNameSumWithAlias(_session, type);

                //Filter option INSET for PRESS medium
                if(CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id
                      || _vehicleInformation.Id == CstDBClassif.Vehicles.names.newspaper
                || _vehicleInformation.Id == CstDBClassif.Vehicles.names.magazine)
                    dataJointForInsert = FctWeb.SQLGenerator.GetJointForInsertDetail(_session, DATA_TABLE_PREFIXE);
            }
            catch (Exception e) {
                throw (new PresentAbsentDALException("Unable to init request parameters.", e));
            }
            UnitInformation u = _session.GetSelectedUnit();

            /*SELECT clause*/
			
			//Get fields of the level of product classification
			sql.AppendFormat("  select {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
			sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand", DATA_TABLE_PREFIXE);
			if (showProduct) sql.AppendFormat(", {0}.id_product", DATA_TABLE_PREFIXE);
			
			//Get agency fields
			if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
				sql.AppendFormat(",{0}.ID_GROUP_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
			if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
				sql.AppendFormat(",{0}.ID_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);

			//Get vehicle identifiers and date fields, according to the target data table's date format (monthly, wekly, per day).	
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {                
                sql.AppendFormat(",{0}.id_media, {1} as date_num, {2}", DATA_TABLE_PREFIXE, dateField, unitFieldNameSumWithAlias);
            }
            else {                
                sql.AppendFormat(",{0}.id_media, {1}", DATA_TABLE_PREFIXE, unitFieldNameSumWithAlias);
            }

			/*FROM clause detrmines the different data tab to use in the query*/
            sql.AppendFormat(" from {0}", dataTableName);
			if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
			sql.AppendFormat(",{0}.{1} {2}", schAdExpress.Label, tblAdvertisingAgengy.Label, tblAdvertisingAgengy.Prefix);
			if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
			sql.AppendFormat(",{0}.{1} {2}", schAdExpress.Label, tblGroupAdvertisingAgengy.Label, tblGroupAdvertisingAgengy.Prefix);
            sql.AppendFormat(" {0} ", fromOptional);

			/*WHERE clause detrmines the filter condition of the query*/
            //sql.AppendFormat(" WHERE {0}", dataTableName);
            sql.AppendFormat(" {0} ", universFilter);


            //Filter INSET option
            if(CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id
                 || _vehicleInformation.Id == CstDBClassif.Vehicles.names.newspaper
                || _vehicleInformation.Id == CstDBClassif.Vehicles.names.magazine)
                sql.AppendFormat(" {0}", dataJointForInsert);

            //Joins for tures group advertising agency and advertising agency
			if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency)) {
				sql.AppendFormat(" and {0}.ID_ADVERTISING_AGENCY(+)={1}.ID_ADVERTISING_AGENCY ", tblAdvertisingAgengy.Prefix, DATA_TABLE_PREFIXE);
				sql.AppendFormat(" and {0}.id_language(+)={1}", tblAdvertisingAgengy.Prefix, _session.DataLanguage);
			}
			if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency)) {
				sql.AppendFormat(" and {0}.ID_GROUP_ADVERTISING_AGENCY(+)={1}.ID_GROUP_ADVERTISING_AGENCY ", tblGroupAdvertisingAgengy.Prefix, DATA_TABLE_PREFIXE);
				sql.AppendFormat(" and {0}.id_language(+)={1}", tblGroupAdvertisingAgengy.Prefix, _session.DataLanguage);
			}
            sql.AppendFormat(" {0} ", joinOptional);


            sql.Append(GetFormatClause(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

            /*GROUP BY clause */
			sql.AppendFormat("  group by {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
			sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand", DATA_TABLE_PREFIXE);
			if (showProduct) sql.AppendFormat(", {0}.id_product", DATA_TABLE_PREFIXE);
			if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
				sql.AppendFormat(",{0}.ID_GROUP_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
			if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
				sql.AppendFormat(",{0}.ID_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {               
                sql.AppendFormat(",{0}.id_media, {1} ", DATA_TABLE_PREFIXE, dateField);
            }
            else {                
                sql.AppendFormat(",{0}.id_media ", DATA_TABLE_PREFIXE);
            }
			//Add unit number of version's field
            if (u.Id == CstWeb.CustomerSessions.Unit.versionNb)
            {
                groupByOptional += string.Format(", {0}.{1}", DATA_TABLE_PREFIXE, FctWeb.SQLGenerator.GetUnitFieldName(_session, type));
            }
            sql.Append(groupByOptional);
            #endregion

            #region Execution of the query
            try {
                return sql.ToString();
            }
            catch (System.Exception err) {
                throw (new PresentAbsentDALException("Unable to build request for synthesis data loading : " + sql, err));
            }
            #endregion
        }
        #endregion

        #region GetSynthesisData
		/// <summary>
		///Loads data for result tab "Summary" : get data corresponding to the summary of Present, Absent and 
		///Exclusive items on the main hierarchical levels of the product brand classification.         
		///The data table returned contains data in the following order :
		/// <code>select id_sector,id_subsector, d_group_
		/// , id_advertiser,id_brand,id_product,ID_GROUP_ADVERTISING_AGENCY,ID_ADVERTISING_AGENCY
		/// ,id_media, date_num, euro
		/// </code>
		/// So you ca notice that the firts columns correspond to identifiers of the classification product ( from id_sector to id_product).
		/// Then the data columns of advertising agency ("ID_GROUP_ADVERTISING_AGENCY,ID_ADVERTISING_AGENCY").
		/// and column of the identifier of vehicle ("id_media"). 
		/// The column fo current dates of the selection. The "date_num" is an alias.
		/// The field euro is an example of unit's field which depends on the unit selected. 
		/// The unit label changes according to the unit selected by the client (ex. duration, spot).
		///</summary>
		///<remarks> Use the protected method <code>GetSynthesisRequest(CstDB.TableType.Type type);</code> which get summary data
		///accordind to the type of period (monthly, weekly, dayly).</remarks>
		/// <returns>Summary result's data set</returns>
		/// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
		/// Exception throwed when an error occurs in the method</exception>
		public virtual DataSet GetSynthesisData() {

            #region Constantes
			//Prefix of defaut result table
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables
            StringBuilder sql = new StringBuilder();
            StringBuilder sql4M = new StringBuilder();
            StringBuilder sqlDataVehicle = new StringBuilder();
            StringBuilder sqlWebPlan = new StringBuilder();

			//Customer period selected
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
            string groupByOptional = string.Empty;

			//Determine if result must contain the product level
			bool showProduct = _session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

            #endregion

            #region Building SQL query
            try {
				//If selected period is included in the Last 4 months
                if (customerPeriod.Is4M) {

					//Get query string
                    sql4M.Append(GetSynthesisRequest(CstDB.TableType.Type.dataVehicle4M));

					//Order clause with product classification's levels
                    sql4M.Append("  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand ");
					if (showProduct) sql.Append(",id_product");
					
					//Add advertising agencies fields
					if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                    sql4M.Append(", ID_GROUP_ADVERTISING_AGENCY ");
					if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
					sql4M.Append(", ID_ADVERTISING_AGENCY ");
					
					//Add vehicle identiifer field
					sql4M.Append(", id_media");

                    sql = sql4M;
                }
				//If query not concerns data per day and agrated data by month
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
                    sql.Append(GetSynthesisRequest(CstDB.TableType.Type.dataVehicle));
                    sql.Append("  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand");
					if (showProduct) sql.Append(",id_product");
					if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
						sql4M.Append(", ID_GROUP_ADVERTISING_AGENCY ");
					if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
						sql4M.Append(", ID_ADVERTISING_AGENCY ");
					sql4M.Append(", id_media");
                }
                else {
					//Query concerns data detailed per day
                    if (customerPeriod.IsDataVehicle) {
                        sqlDataVehicle.Append(GetSynthesisRequest(CstDB.TableType.Type.dataVehicle));
                        sql = sqlDataVehicle;
                    }
					//Query concerns data agregated by month
                    if (customerPeriod.IsWebPlan) {
                        sqlWebPlan.Append(GetSynthesisRequest(CstDB.TableType.Type.webPlan));
                        sql = sqlWebPlan;
                    }
					//Query concerns data detailed per day and also data agregated by month
                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                        sql = new StringBuilder();
                        sql.Append("  select id_sector, id_subsector, id_group_, id_advertiser, id_brand");
						if (showProduct) sql.Append(",id_product");
						if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
							sql.Append(", ID_GROUP_ADVERTISING_AGENCY ");
						if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
							sql.Append(", ID_ADVERTISING_AGENCY ");
                        sql.Append(" , id_media");

                        sql.AppendFormat(", {0}", FctWeb.SQLGenerator.GetUnitFieldNameSumUnionWithAlias(_session));
                        if ((_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack 
							|| _vehicleInformation.Id == CstDBClassif.Vehicles.names.evaliantMobile) && _session.Unit == CstWeb.CustomerSessions.Unit.versionNb) {
                            groupByOptional = string.Format(", {0}", FctWeb.SQLGenerator.GetUnitAlias(_session));
                        }

						//FROM clause
                        sql.Append(" from (");
                        sql.Append(sqlDataVehicle);//Add query on the tables whose data are aggregated per day 
                        sql.Append(" UNION ALL");
						sql.Append(sqlWebPlan);//Add query on the tables whose data are aggregated by month 
                        sql.Append(" ) ");

						//GROU BY Clause
                        sql.Append("  group by id_sector, id_subsector, id_group_, id_advertiser, id_brand");
						if (showProduct) sql.Append(",id_product");
						if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
							sql.Append(", ID_GROUP_ADVERTISING_AGENCY ");
						if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
							sql.Append(", ID_ADVERTISING_AGENCY ");
                        sql.Append(" , id_media");
                        sql.Append(groupByOptional);

                    }
					//ORDER BY Clause
                    sql.Append("  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand");
					if (showProduct) sql.Append(",id_product");
					if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
						sql.Append(", ID_GROUP_ADVERTISING_AGENCY ");
					if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
						sql.Append(", ID_ADVERTISING_AGENCY ");
                    sql.Append(" , id_media");
                }
            }
            catch (System.Exception err) {
                throw (new PresentAbsentDALException("Unable to build request to get synthesis data : " + sql, err));
            }
            #endregion

            #region Execution of the query
            try {
                return _session.Source.Fill(sql.ToString());//.Tables[0];
            }
            catch (System.Exception err) {
                throw (new PresentAbsentDALException("Unable to load synthesis data : " + sql, err));
            }

            #endregion
        
        }
        #endregion

        #endregion       

		#region GetData
		/// <summary>
        ///Load data for the module absent/present report.
		///The module absent/present report allows client to view all active product items
		/// from a selection of competing media vehicles (media type, media seller, media vehicle).
		/// This method returns the data for the folloing results :
		/// - Result "Portofolio" : get data corresponding to active products items in the set being searched.
		/// - Result "Present in more than one vehicle" : get data corresponding to products items present 
		/// in more tha one vehicle
		/// - Result "Absent" :  get data corresponding to products items absent from the reference vehicles 
		/// in comparaison with the competing vehicles
		/// - Result "Exclusive to one vehicle" : get data corresponding to the reference vehicles' exclusive 
		/// products items in comparaison with the competing vehicles.
		/// - Result "strenghs" : get data corresponding to the product items whose market share is greater 
		/// than that of the total of the product set, int the reference vehicle set.
		/// - Result "Prospects" : get data corresponding to the product items whose market share is less 
		/// than that of the total of the product set, int the reference vehicle set.
		/// 
		/// - Calls the following methods :
		///<code> CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
		///   DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
		///   GetSynthesisRequest(CstDB.TableType.Type type);
		///   string orderFieldName = _session.GenericProductDetailLevel.GetSqlOrderFields();
		///   string orderFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
		///   string groupByFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();
		///    string productFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();
		///  </code>
        /// </summary> 
		/// <example>
		/// GetData method is called in the Bussiness Layer like in the following example. 
		/// The DAL layer is called by reflection with method's parameters. And the the method is called to get the data table of the module.
		/// <code> Dataset dt = null;
        ///     if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
        ///    object[] parameters = new object[1];
        ///   parameters[0] = _session;
        ///   IPresentAbsentResultDAL presentAbsentDAL = (IPresentAbsentResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
		///   dt = presentAbsentDAL.GetData();
		///   </code>
		/// </example>
		/// <remarks> The Data table obtained will be structured as follows :
		/// DATATABLE [ID_MEDIA,"identifier of current column level" ,ID LEVEL 1, LABEL LEVEL 1,...,ID LEVEL N, LABEL LEVEL N,ID_ADRESS,UNIT FIELD]
		/// ID_MEDIA : contains identifiers of vehicles selected.
		/// "identifier of current column level"  : contains identifiers of vehicle-level detail in column.
		/// ID LEVEL N : contains identifiers of classification-level detail in row into the final result table.
		/// LABEL LEVEL N : contains labels of classification-level detail in row into the final result table.
		/// ID_ADRESS : Identifier for compagny description.
		/// UNIT : example EURO (according to unit selected by the client.
		/// </remarks>
		/// <returns>Present/Absent report data set</returns>
		/// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
		/// Exception throwed when an error occurs in the method</exception>
		public virtual DataSet GetData() {

            #region Constantes
			//Prefix of defaut result table
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables

            StringBuilder sql = new StringBuilder();
            StringBuilder sql4M = new StringBuilder();
            StringBuilder sqlDataVehicle = new StringBuilder();
            StringBuilder sqlWebPlan = new StringBuilder();
            string groupByFieldNameWithoutTablePrefix = string.Empty;
            string groupByOptional = string.Empty;
            string orderFieldName = string.Empty, orderFieldNameWithoutTablePrefix = string.Empty;
            string orderClause = string.Empty;
            string productFieldNameWithoutTablePrefix = string.Empty, unitField = string.Empty, dataFieldsForGadWithoutTablePrefix = string.Empty;
           
			//Customer period selected
			CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
			
			//vehicle-level detail in column
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            #endregion

            #region Building SQL query
            try {
				//Get Product detail levels fields for Sql ORDER clause
                orderFieldName = _session.GenericProductDetailLevel.GetSqlOrderFields();

				//Get Product detail levels fields for Sql ORDER clause without prefix
				orderFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();

				//Get Product detail levels fields for Sql GROUP BY clause without prefix
                groupByFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();
                
				/*To optimize the queries, data tables requested vary depending on the period,
				 *  the media selected by the customer. */
				if (customerPeriod.Is4M) {

					/*If the period is selected within the last 4 months. 
					Use the tables containing data of only last 4 months only 4 months. ex : DATA_TV_4M*/ 
                    sql4M.Append(GetRequest(CstDB.TableType.Type.dataVehicle4M));
                    orderClause = string.Format(" order by {0}, {1}.id_media", orderFieldNameWithoutTablePrefix, DATA_TABLE_PREFIXE);
                    sql = sql4M;
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
					
					/*Use the tables whose history covers the last three years and detailed per day. ex : DATA_TV */  
                    sql.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                    orderClause = string.Format(" order by {0}, {1}.id_media", orderFieldNameWithoutTablePrefix, DATA_TABLE_PREFIXE);
                }
                else {

					/*If the beginning period is less than the last 4 months 
					 * and the data are detailed per day you must select 
					 * the tables whose history covers the last three years and detailed per day. ex : DATA_TV */  
                    if (customerPeriod.IsDataVehicle) {
                        sqlDataVehicle.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                        sql = sqlDataVehicle;
                        orderClause = string.Format(" order by {0}, {1}.id_media", orderFieldNameWithoutTablePrefix, DATA_TABLE_PREFIXE);
                    }

					/*If the format of period selected is monthly (ex. 200912) you must select 
					 * the tables whose periods are detail by months . ex : ADEXPR03.WEB_PLAN_MEDIA_WEEK or ADEXPR03.WEB_PLAN_MEDIA_MONTH */
					if (customerPeriod.IsWebPlan) {
                        sqlWebPlan.Append(GetRequest(CstDB.TableType.Type.webPlan));
                        sql = sqlWebPlan;
                        orderClause = string.Format(" order by {0}, {1}.id_media", orderFieldNameWithoutTablePrefix, DATA_TABLE_PREFIXE);
                    }

					/* If the period selected contains dates in two formats : monthly and per day, So you can use the two type of table.*/
                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                       
						if ((_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack 
							|| _vehicleInformation.Id == CstDBClassif.Vehicles.names.evaliantMobile) 
							&& _session.Unit == CstWeb.CustomerSessions.Unit.versionNb)
                        {
							//Adding "slogan number" field for evaliant medium field
                            groupByOptional = string.Format(", versionNb ");
                        }

						//Get Product detail levels fields for Sql SELECT clause without prefix
                        productFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();

						//Add GAD fields for Sql SELECT clause without prefix
						if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                            dataFieldsForGadWithoutTablePrefix = ", " + FctWeb.SQLGenerator.GetFieldsAddressForGad("");
                       
						//SELECT clause
						sql = new StringBuilder();
                        if(columnDetailLevel.Id != DetailLevelItemInformation.Levels.media)
                        sql.AppendFormat(" select id_media, {2}, {0}{1}",productFieldNameWithoutTablePrefix,dataFieldsForGadWithoutTablePrefix,columnDetailLevel.DataBaseIdField);
                        else sql.AppendFormat(" select id_media, {0}{1}",productFieldNameWithoutTablePrefix,dataFieldsForGadWithoutTablePrefix);
                        
                        sql.AppendFormat(", {0}", FctWeb.SQLGenerator.GetUnitFieldNameSumUnionWithAlias(_session));

						//FROM clause
                        sql.Append(" from (");

						//UNION ALL with queries on two different tables due to the type of period (monthly or per day)
                        sql.Append(sqlDataVehicle);					
                        sql.Append(" UNION ALL");
                        sql.Append(sqlWebPlan);
                        sql.Append(" ) ");

						//GROUP BY clause
                        if (columnDetailLevel.Id != DetailLevelItemInformation.Levels.media)
                        sql.AppendFormat(" group by id_media, {3}, {0}{1} {2}",groupByFieldNameWithoutTablePrefix,dataFieldsForGadWithoutTablePrefix,groupByOptional,columnDetailLevel.DataBaseIdField);
                        else sql.AppendFormat(" group by id_media, {0}{1} {2}", groupByFieldNameWithoutTablePrefix, dataFieldsForGadWithoutTablePrefix, groupByOptional);

						//ORDER BY clause
                        orderClause = string.Format(" order by {0}, id_media", orderFieldNameWithoutTablePrefix);
                    }
                }
                sql.Append(orderClause);
            }
            catch (System.Exception err) {
                throw (new PresentAbsentDALException("Unable to build request for present/absent report : " + sql, err));
            }
            #endregion

			#region Execution of the query
			try {
                return GetDataSource().Fill(sql.ToString());				

            }
            catch (System.Exception err) {
                throw (new PresentAbsentDALException("Unable to load data for present/absent report : " + sql, err));
            }

            #endregion

        }
        #endregion   

		#region Get Request
		/// <summary>
		///Build SQL query string for the module absent/present report.
		///The module absent/present report allows client to view all active product items
		/// from a selection of competing media vehicles (media type, media seller, media vehicle).
		/// This method returns the data for the folloing results :
		/// - Result "Portofolio" : get data corresponding to active products items in the set being searched.
		/// - Result "Present in more than one vehicle" : get data corresponding to products items present 
		/// in more tha one vehicle
		/// - Result "Absent" :  get data corresponding to products items absent from the reference vehicles 
		/// in comparaiso18n with the competing vehicles
		/// - Result "Exclusive to one vehicle" : get data corresponding to the reference vehicles' exclusive 
		/// products items in comparaison with the competing vehicles.
		/// - Result "strenghs" : get data corresponding to the product items whose market share is greater 
		/// than that of the total of the product set, int the reference vehicle set.
		/// - Result "Prospects" : get data corresponding to the product items whose market share is less 
		/// than that of the total of the product set, int the reference vehicle set.			
		/// </summary> 
		/// <example>
		/// GetRequest method can be called like in the following example. 
		/// <code>    public DataSet GetSynthesisData() {	
		///	StringBuilder sql = new StringBuilder();
		///	 ...
		///   if(...)sql.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
		///   else   sqlWebPlan.Append(GetRequest(CstDB.TableType.Type.webPlan));
		///   
		/// ...		
		///   }
		///   </code>
		/// </example>
		/// <remarks> When teh qquery is executed, the Data set obtained will be structured as follows :
		/// DATATABLE [ID_MEDIA,"identifier of current column level",ID LEVEL 1, LABEL LEVEL 1,...,ID LEVEL N, LABEL LEVEL N,ID_ADRESS,UNIT FIELD]
		/// ID_MEDIA : contains identifiers of vehicles selected.
		/// "identifier of current column level" : contains identifiers of vehicle-level detail in column.
		/// ID LEVEL N : contains identifiers of classification-level detail in row into the final result table.
		/// LABEL LEVEL N : contains labels of classification-level detail in row into the final result table.
		/// ID_ADRESS : Identifier for compagny description.
		/// UNIT : example EURO (according to unit selected by the client.
		/// </remarks>
		/// <param name="type">Database table type to use according to the period selected format : monthly ; weekly or dayly.</param>
		/// <returns>Present/Absent report SQL query string</returns>
		/// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
		/// Exception throwed when an error occurs in the method</exception>
		protected virtual string GetRequest(CstDB.TableType.Type type) {

			#region Variables
			string productFieldName = string.Empty;
			string orderFieldName = string.Empty;
			string groupByFieldName = string.Empty;
			string productJoinCondition = string.Empty;
			string dataTableName = string.Empty;
			string productTableName = string.Empty;
			string mediaAgencyTable = string.Empty;
			string mediaAgencyJoins = string.Empty;
			string dateField = "";
			string dataTableNameForGad = "";
			string dataFieldsForGad = "";
			string dataJointForGad = "";
			string unitFieldNameSumWithAlias = "";
			string universFilter = string.Empty;
			string dataJointForInsert = "";
			string groupByOptional = string.Empty;
			string fromOptional = string.Empty;
			string joinOptional = string.Empty;

			//Customer period selected
			CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;

			//Get Data base schema descritpion
			Schema schAdEx = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);

			//Get Table GAD
			Table tblGad = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad);

			StringBuilder sql = new StringBuilder();
			string columnDetailLevel = string.Empty;
            DetailLevelItemInformation columnDetailLevelInformation = ((DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0]);
			#endregion

			#region Building of the SQL query

			#region Initialization of query's parameters
			try {
				// Get table name and date field according to the table type parameter
				switch (type) {
					case CstDB.TableType.Type.dataVehicle4M:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.alert, _session.IsSelectRetailerDisplay);
						dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + CstDB.Fields.DATE_MEDIA_NUM;
						break;
					case CstDB.TableType.Type.dataVehicle:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.analysis, _session.IsSelectRetailerDisplay);
						dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + CstDB.Fields.DATE_MEDIA_NUM;
						break;
					case CstDB.TableType.Type.webPlan:
						dataTableName = WebApplicationParameters.GetDataTable(TableIds.monthData, _session.IsSelectRetailerDisplay).SqlWithPrefix;
						dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + CstDB.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
						break;
					default:
						throw (new PresentAbsentDALException("Unable to determine the type of table to use."));
				}

				/* Get the SQL tables  corresponding to the classification's 
				level selected (for FROM clause).*/
				productTableName = _session.GenericProductDetailLevel.GetSqlTables(schAdEx.Label);
				if (productTableName != null && productTableName.Length > 0) productTableName = "," + productTableName;

				/* Get the SQL  fields corresponding to the classification's 
				level selected for  SELECT clause.*/
				productFieldName = _session.GenericProductDetailLevel.GetSqlFields();
				columnDetailLevel = string.Format("{0}.{1}", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, ((DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0]).GetSqlFieldIdWithoutTablePrefix());

				/* Get the SQL fields corresponding to the classification's 
				level selected for  GROUP BY clause.*/
				groupByFieldName = _session.GenericProductDetailLevel.GetSqlGroupByFields();

				/* Get the SQL joins  code .*/
				productJoinCondition = _session.GenericProductDetailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

				//Get Universe filters
				universFilter = GetUniversFilter(type, dateField, customerPeriod);

				// Get unit selected field
				unitFieldNameSumWithAlias = FctWeb.SQLGenerator.GetUnitFieldNameSumWithAlias(_session, type);

				//Treatment specific to the medium evaliant : group by list of banners
				if ((_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack || _vehicleInformation.Id == CstDBClassif.Vehicles.names.evaliantMobile)
					&& _session.Unit == CstWeb.CustomerSessions.Unit.versionNb) {
					if (type == CstDB.TableType.Type.webPlan) {
						groupByOptional = string.Format(",{0}.list_banners ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
					}
					else {
						groupByOptional = string.Format(",{0}.id_banners ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
					}
				}

				//Treatment specific to the medium PRESS : option INSET 
				if (CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id
                     || _vehicleInformation.Id == CstDBClassif.Vehicles.names.newspaper
                || _vehicleInformation.Id == CstDBClassif.Vehicles.names.magazine)
					dataJointForInsert = FctWeb.SQLGenerator.GetJointForInsertDetail(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

				//Get filter options for the GAD (company's informations)
				if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)) {
					try {
						dataTableNameForGad = ", " + tblGad.SqlWithPrefix;
						dataFieldsForGad = ", " + FctWeb.SQLGenerator.GetFieldsAddressForGad(tblGad.Prefix);
						dataJointForGad = "and " + FctWeb.SQLGenerator.GetJointForGad(tblGad.Prefix, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
					}
					catch (SQLGeneratorException) { ;}
				}
			}
			catch (Exception e) {
				throw (new PresentAbsentDALException("Unable to intialise request parameters :" + e.Message));
			}
			#endregion

			try {
				/* SELECT clause */
                if (columnDetailLevelInformation.Id != DetailLevelItemInformation.Levels.media)
                {
                    sql.AppendFormat(" select {0}.id_media, {1}, {2} {3}, {4}", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix , columnDetailLevel, productFieldName, dataFieldsForGad, unitFieldNameSumWithAlias);
                }
                else                
                    sql.AppendFormat(" select {0}.id_media, {1} {2}, {3}", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, productFieldName, dataFieldsForGad, unitFieldNameSumWithAlias);
               

				/* FROM clause */
				sql.AppendFormat(" from {0} {1} {2} {3} {4}"
					, mediaAgencyTable
					, dataTableName
					, productTableName
					, dataTableNameForGad
					, fromOptional
				);

				/* WHERE clause */

				//Adding universe filters
				sql.AppendFormat(" {0} ", universFilter);

				//Adding product's claasification filter
				sql.AppendFormat(" {0}", productJoinCondition);

				//Adding GAD filter
				sql.AppendFormat(" {0}", dataJointForGad);

				//Add Media agency filter
				sql.AppendFormat(" {0}", mediaAgencyJoins);

                sql.Append(GetFormatClause(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

				//Jointures encart
				if (CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id
                     || _vehicleInformation.Id == CstDBClassif.Vehicles.names.newspaper
                || _vehicleInformation.Id == CstDBClassif.Vehicles.names.magazine)
					sql.AppendFormat(" {0}", dataJointForInsert);
				//optional joins
				sql.Append(joinOptional);

				/* GROUP BY clause according to period selected*/
                if (columnDetailLevelInformation.Id != DetailLevelItemInformation.Levels.media)               
                    sql.AppendFormat(" group by {0}.id_media, {1}, {2}{3}{4}",WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,columnDetailLevel,groupByFieldName,dataFieldsForGad,groupByOptional);
                else sql.AppendFormat(" group by {0}.id_media, {1}{2}{3}", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,  groupByFieldName, dataFieldsForGad, groupByOptional);
                
			

				return (sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PresentAbsentDALException("Unable to build the request required for Present/Absent report" + sql, err));
			}
			#endregion

		}
		#endregion

		#region Get Number of Publication by vehicle
		/// <summary>
		/// Get for each vehicles and period selected  the number of publications		
		/// </summary>
		/// <remarks> Only for medium PRESS</remarks>
		/// <returns>Data table with in first colum the identifier of vehicle (id_media)
		/// and in the second column the number of publications (NbParution)</returns>
		/// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
		/// Exception throwed when an error occurs in the method</exception>
        public virtual DataSet GetNbParutionData()
        {

			#region Constantes
			//Prefix of defaut result table
			string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
			#endregion

			#region Variables
			StringBuilder sql = new StringBuilder();
			string dataTableName = "";
			string dateField = "";
			string mediaList = "";
			int positionUnivers = 1;
           
			#endregion

			#region Building SQL query
			try {
				//Customer period selected
				CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
               
				//Get data table name
                if (Dates.Is4M(customerPeriod.StartDate))
                    dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.alert, _session.IsSelectRetailerDisplay);
                else
                    dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.analysis, _session.IsSelectRetailerDisplay);
				//Get date field label
				dateField = CstDB.Fields.DATE_MEDIA_NUM;

				sql.Append(" select id_media, count(distinct " + dateField + ") as NbParution");
				sql.Append(" from  " + dataTableName);
				sql.Append(" where " + dateField + " >= " + customerPeriod.StartDate + " and " + dateField + " <= " + customerPeriod.EndDate);
                
              
				#region Get vehicles selected
				//Get all vehicles selected
				while (_session.CompetitorUniversMedia[positionUnivers] != null) {
					mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
					positionUnivers++;
				}
				if (mediaList.Length > 0) sql.Append(" and id_media in (" + mediaList.Substring(0, mediaList.Length - 1) + ")");
				#endregion

                /*Filter data with the identifier of the sub media selected.
              Remark : Use in Russia
              */
                string idSubMedia = _session.CustomerDataFilters.SelectedMediaCategory;
                if (idSubMedia != null && idSubMedia.Length > 0)
                    sql.AppendFormat(" and id_category ={0}", idSubMedia);


				//GROUP BY vehicles identifier
				sql.Append(" group by id_media");
			}
			catch (System.Exception err) {
				throw (new PresentAbsentDALException("Unable to build request for Number of parutions : " + sql, err));
			}
			#endregion

			#region Execution of the query
			try {
				return _session.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PresentAbsentDALException("Unable to lad data for present/absent report : " + sql, err));
			}

			#endregion

		}
		#endregion
                
        #region GetColumnDetails
		/// <summary>    
		/// In the result page, client can choose the vehicle-level detail in column by selecting it from the drop-down menu.
		/// Then this method gets the list of identifiers of items corresponding to the vehicle-level selected.		
		/// </summary>
		/// <returns>Data set with list of vehicle-level items. </returns>		
		/// <remarks>The query must always contains the field of vehicle level ( "id_media" )</remarks>
		/// <exception cref="TNS.AdExpressI.PresentAbsentDAL.Exceptions.PresentAbsentDALException">
		/// Exception throwed when an error occurs in the method</exception>
		public virtual DataSet GetColumnDetails()
        {

			#region Variables
			StringBuilder sql = new StringBuilder();
			string mediaList = "";
			int positionUnivers = 1;
			//Get Vehicle Lvel detail level informations
			DetailLevelItemInformation mediaDetailLevelItemInformation = DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.media);

			//Get vehicle-level detail in column selected by the client
			DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];

			//Get Vehicle Level database table 's informations to use in this SQL query
			Table tblMedia = WebApplicationParameters.DataBaseDescription.GetTable(CstDBDesc.TableIds.media);
			//Get Su Media database table 's informations to use in this SQL query
			Table tblCategory = WebApplicationParameters.DataBaseDescription.GetTable(CstDBDesc.TableIds.category);
			//Get Basic Media database table 's informations to use in this SQL query
			Table tblBasicMedia = WebApplicationParameters.DataBaseDescription.GetTable(CstDBDesc.TableIds.basicMedia);
           
            //Get data filters object which contains query's filters methods
            TNS.AdExpress.Web.Core.CustomerDataFilters dataFilters = new TNS.AdExpress.Web.Core.CustomerDataFilters(_session);            
      
			#endregion

			#region Building SQL query

			/* SELECT clause */

			//Start with Vehicle level's identifier
			sql.AppendFormat(" select distinct {0},", mediaDetailLevelItemInformation.DataBaseIdField);

			//Adding prefix of Sub media table use to join
			if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category)
				sql.AppendFormat("{0}.", tblCategory.Prefix);//Add field prefix if current level is media level
			//Adding field of current selected column
			sql.AppendFormat("{0} ", columnDetailLevel.GetSqlFieldIdWithoutTablePrefix());


			/* FROM clause */
			sql.AppendFormat(" from {0} ", tblMedia.SqlWithPrefix);

			if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category) {
				//Add  MEDIA table and BASIC_MEDIA Table if current level is sub media level
				sql.AppendFormat(", {0}, {1} ", tblBasicMedia.SqlWithPrefix, tblCategory.SqlWithPrefix);
			}

			//Filtering with  all  vehicles selected
			while (_session.CompetitorUniversMedia[positionUnivers] != null) {
				mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
				positionUnivers++;
			}

			/*WHERE  clause */

			//Filter data with all vehicles selected
			sql.AppendFormat(" where {1} in ({0})", mediaList.Substring(0, mediaList.Length - 1), mediaDetailLevelItemInformation.DataBaseIdField);

			//Filter data language
			sql.AppendFormat(" and {0}.id_language={1}", tblMedia.Prefix, _session.DataLanguage);

			//Joins for medium classification
			if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category) {
				//Add  joins for table MEDIA and BASIC_MEDIA if current level is sub media level
				sql.AppendFormat(" and {0}.id_basic_media={1}.id_basic_media and {0}.id_category={2}.id_category and {0}.id_language={3} and {2}.id_language={3}"
					, tblBasicMedia.Prefix
					, tblMedia.Prefix
					, tblCategory.Prefix
					, _session.DataLanguage);//Identifier of data language

                /*Filter data with the identifier of the sub media selected.
            Remark : Use in Russia
            */
                string idSubMedia = _session.CustomerDataFilters.SelectedMediaCategory;
                if (idSubMedia != null && idSubMedia.Length > 0)
                    sql.AppendFormat(" and {0}.id_category ={1}", tblCategory.Prefix, idSubMedia);

			}
		    sql.AppendFormat(" and {0}.activation<{1}", mediaDetailLevelItemInformation.DataBaseTableNamePrefix,
		               TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			#endregion


			#region Execution of the query
			try {
                return GetDataSource().Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PresentAbsentDALException("Unable to load the list of supports for columns details:" + sql, err));
			}
			#endregion

        }
        #endregion       

		#region Get Excluded Products
		/// <summary>
		/// Get SQl clause of excluded products
		/// </summary>
		/// <param name="sql">String builder</param>
		/// <returns>SQL string of excluded roduct</returns>
		protected virtual string GetExcludeProducts(string prefix) {
			// Exclude product 
			string sql = "";
			ProductItemsList prList = null; ;
			if (Product.Contains(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID) && (prList = Product.GetItemsList(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID)) != null)
				sql = prList.GetExcludeItemsSql(true, prefix);
			return sql;
		}
		#endregion		

		#region Get Universes Filter
		/// <summary>
		/// Get differents universes filters for the query. 
		/// Filters can be users rigths, classification items selected or period selected.
		/// </summary>
		/// <param name="type">Data table type</param>
		/// <param name="dateField">date field</param>
		/// <param name="customerPeriod">customer selected period</param>
		/// <returns>SQl string for universes filter</returns>
		protected virtual string GetUniversFilter(CstDB.TableType.Type type, string dateField, CustomerPeriod customerPeriod) {
			StringBuilder sql = new StringBuilder();

            //Tests products universe
            Dictionary<TNS.Classification.Universe.AccessType, List<Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string>>> test = _session.CustomerDataFilters.PrincipalProductUniverses;

			// Query filter by period
			switch (type) {
				case CstDB.TableType.Type.dataVehicle4M:
					sql.AppendFormat(" where {0} >= {1}", dateField, customerPeriod.StartDate);
					sql.AppendFormat(" and {0} <= {1}", dateField, customerPeriod.EndDate);
					break;
				case CstDB.TableType.Type.dataVehicle:
					if (_session.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
						sql.AppendFormat(" where {0} >= {1}", dateField, customerPeriod.StartDate);
						sql.AppendFormat(" and {0} <= {1}", dateField, customerPeriod.EndDate);
					}
					else if (_session.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
						sql.AppendFormat(" where (({0} >= {1}", dateField, customerPeriod.PeriodDayBegin[0]);
						sql.AppendFormat(" and {0} <= {1}", dateField, customerPeriod.PeriodDayEnd[0]);
						sql.AppendFormat(" ) or ({0} >= {1}", dateField, customerPeriod.PeriodDayBegin[1]);
						sql.AppendFormat(" and {0} <= {1}", dateField, customerPeriod.PeriodDayEnd[1]);
						sql.Append("))");
					}
					else {
						sql.AppendFormat(" where {0} >= {1}", dateField, customerPeriod.PeriodDayBegin[0]);
						sql.AppendFormat(" and {0} <= {1}", dateField, customerPeriod.PeriodDayEnd[0]);
					}
					break;
				case CstDB.TableType.Type.webPlan:
					sql.AppendFormat(" where {0} >= {1}", dateField, customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6));
					sql.AppendFormat(" and {0} <= {1}", dateField, customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6));
					break;
			}

            /*Filter data with the identifier of the sub media selected.
              Remark : Use in Russia
              */
            string idSubMedia = _session.CustomerDataFilters.SelectedMediaCategory;
            if (idSubMedia != null && idSubMedia.Length > 0)
                sql.AppendFormat(" and {0}.id_category ={1}", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, idSubMedia);

			//Medias selection
			int positionUnivers = 1;
			string mediaList = "";
			while (_session.CompetitorUniversMedia[positionUnivers] != null) {
				mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
				positionUnivers++;
			}
			if (mediaList.Length > 0)
				sql.AppendFormat(" and {0}.id_media in ({1})",
					WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
					mediaList.Substring(0, mediaList.Length - 1)
				);

			// Product Selection
			if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
				sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

			// Excluded Products
			string listExcludeProduct = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
			sql.Append(listExcludeProduct);

			// Media Rights
			string mediaRights = FctWeb.SQLGenerator.getAnalyseCustomerMediaRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
			sql.AppendFormat(" {0}", mediaRights);
			// Product rights
            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);
            string productsRights = FctWeb.SQLGenerator.GetClassificationCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, module.ProductRightBranches);
			sql.AppendFormat(" {0}", productsRights);

			// Autopromo
            string idMediaLabel = string.Empty;

            if (_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack || _vehicleInformation.Id == CstDBClassif.Vehicles.names.evaliantMobile)
                idMediaLabel = "id_media_evaliant";
            else if (_vehicleInformation.Id == CstDBClassif.Vehicles.names.mms)
                idMediaLabel = "id_media_mms";

            if (_vehicleInformation.Autopromo && (_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack 
                || _vehicleInformation.Id == CstDBClassif.Vehicles.names.evaliantMobile
                || _vehicleInformation.Id == CstDBClassif.Vehicles.names.mms)) {
                
                    Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                    if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                        sql.AppendFormat(" and {0}.auto_promotion = 0 ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    else if (_session.AutoPromo == CstWeb.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany) {
                        sql.AppendFormat(" and ({0}.id_media, {0}.id_holding_company) not in ( ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                        sql.AppendFormat(" select distinct {0}, id_holding_company ", idMediaLabel);
                        sql.AppendFormat(" from {0} ", tblAutoPromo.Sql);
                        sql.AppendFormat(" where {0} is not null ", idMediaLabel);
                        sql.AppendFormat(" ) ");
                    }
			}

			return sql.ToString();
		}

        /// <summary>
        /// Get Format Clause
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <returns>Sql Format selected Clause</returns>
        protected virtual string GetFormatClause(string prefix) {
            var sql = new StringBuilder();
            var formatIdList = _session.GetValidFormatSelectedList(new List<VehicleInformation>(new[]{_vehicleInformation}));
            if (formatIdList.Count > 0)
                sql.AppendFormat(" and {0}ID_{1} in ({2}) "
                    , ((!string.IsNullOrEmpty(prefix)) ? prefix + "." : string.Empty)
                           , WebApplicationParameters.DataBaseDescription.GetTable(WebApplicationParameters.VehiclesFormatInformation.VehicleFormatInformationList[_vehicleInformation.DatabaseId].FormatTableName).Label
                           , string.Join(",", formatIdList.ConvertAll(p => p.ToString()).ToArray()));
            return sql.ToString();
        }
		#endregion


        /// <summary>
        /// Get Data Source
        /// </summary>
        /// <returns>Data source</returns>
        protected virtual TNS.FrameWork.DB.Common.IDataSource GetDataSource()
        {
            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.sourceProvider];
            object[] param = new object[1];
            param[0] = _session;
            if (cl == null) throw (new NullReferenceException("Core layer is null for the source provider layer"));
             TNS.AdExpress.Web.Core.ISourceProvider sourceProvider = ( TNS.AdExpress.Web.Core.SourceProvider)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            return sourceProvider.GetSource();

        }

        /// <summary>
        /// Get summary product classification levels
        /// </summary>
        /// <returns>summary product classification levels</returns>
        protected virtual GenericDetailLevel GetSummaryLevels(){
            //TODO : Impelements all the mechanism via configuration file
           
            ArrayList levelsIds = new ArrayList();
           
            levelsIds.Add(11); 
            levelsIds.Add(12);
            levelsIds.Add(13);
            levelsIds.Add(14);
            levelsIds.Add(10);
            levelsIds.Add(9);
            levelsIds.Add(28); 
            levelsIds.Add(8);
            GenericDetailLevel levels = new GenericDetailLevel(levelsIds);
            return levels;
        }
	}
}
