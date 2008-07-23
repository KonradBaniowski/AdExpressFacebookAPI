#region Information
// Author: G. Facon
// Creation date: 26/03/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Exceptions;
using CustormerConstantes=TNS.AdExpress.Constantes.Customer;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core;

namespace TNS.AdExpressI.Portofolio.DAL {
    /// <summary>
    /// Portofolio Data Access Layer
    /// </summary>
    public abstract class PortofolioDAL:IPortofolioDAL {

        #region Variables
        /// <summary>
        /// Customer session
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Current Module
        /// </summary>
        protected Module _module;
        /// <summary>
        /// Vehicle name
        /// </summary>
        protected DBClassificationConstantes.Vehicles.names _vehicleName;
        /// <summary>
        /// Media Id
        /// </summary>
        protected Int64 _idMedia;
        /// <summary>
        /// Begining Date
        /// </summary>
        protected string _beginingDate;
        /// <summary>
        /// End Date
        /// </summary>
        protected string _endDate;
		/// <summary>
		/// Screen code
		/// </summary>
		protected string _adBreak;
		
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleName">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        protected PortofolioDAL(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName, Int64 idMedia,string beginingDate,string endDate) {
            if(webSession==null) throw (new ArgumentNullException("Customer session is null"));
            if(beginingDate==null || beginingDate.Length==0) throw (new ArgumentException("Begining Date is invalid"));
            if(endDate==null || endDate.Length==0) throw (new ArgumentException("End Date is invalid"));
            _webSession=webSession;
            _beginingDate=beginingDate;
            _endDate=endDate;
            _vehicleName=vehicleName;
            _idMedia = idMedia;
            try {
                // Module
                _module=ModulesList.GetModule(webSession.CurrentModule);

            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible to set parameters",err));
            }
        
        }
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="vehicleName">Vehicle name</param>
		/// <param name="idMedia">Media Id</param>
		/// <param name="beginingDate">begining Date</param>
		/// <param name="endDate">end Date</param>
		protected PortofolioDAL(WebSession webSession, DBClassificationConstantes.Vehicles.names vehicleName, Int64 idMedia, string beginingDate, string endDate,string adBreak) {
			if (webSession == null) throw (new ArgumentNullException("Customer session is null"));
			if (beginingDate == null || beginingDate.Length == 0) throw (new ArgumentException("Begining Date is invalid"));
			if (endDate == null || endDate.Length == 0) throw (new ArgumentException("End Date is invalid"));
			_webSession = webSession;
			_beginingDate = beginingDate;
			_endDate = endDate;
			_vehicleName = vehicleName;
			_idMedia = idMedia;
			_adBreak = adBreak;
			try {
				// Module
				_module = ModulesList.GetModule(webSession.CurrentModule);

			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to set parameters", err));
			}

		}
        #endregion

        #region IPortofolioDAL Membres

        #region Synthesis

        #region Category, Media Owner, Interest Center and Periodicity
        /// <summary>
        /// Get the following fields : Category, Media Owner, Interest Center and Periodicity for press
        /// </summary>
        /// <returns>Category, Media Owner, Interest Center and Periodicity for press</returns>
        public DataSet GetCategoryMediaSellerData() {

            #region Variables
            string sql = "";
            string tableName = "";
            string fields = "";
            string joint = "";
            string mediaRights = "";
            #endregion

            #region Construction de la requête
            try {
                tableName = GetTable();
                fields = GetFields();
                joint = GetJoint();
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix, true);
                sql += " select " + fields;
                sql += " from " + tableName;
                sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media=" + _idMedia + "";
                sql += joint;
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request", err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetCategoryMediaSellerData() : " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Total investment and date of issue
        /// <summary>
        /// Get total investment and date of issue
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetInvestment() {

            #region Construction de la requête
            string select = GetSelectData();
            string table = GetTableData();
            string product = GetProductData();
            string productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            //liste des produit hap
            string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

            string sql = select;

            sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + table + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + " ";
            sql += " where id_media=" + _idMedia + "";
            if (_beginingDate.Length > 0)
                sql += " and date_media_num>=" + _beginingDate + " ";
            if (_endDate.Length > 0)
                sql += " and date_media_num<=" + _endDate + "";

            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += listProductHap;
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetInvestment(): " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Insertions Number
        /// <summary>
        /// Get insertions number
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetInsertionNumber() {

            #region Construction de la requête

            string table = GetTableData();
            string product = GetProductData();
            string productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            //liste des produit hap
            string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);


            string insertionField = "insertion";
            if (_vehicleName == DBClassificationConstantes.Vehicles.names.outdoor)
                insertionField = "NUMBER_BOARD";

            string sql = " select sum(" + insertionField + ") as insertion ";

            sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + table + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + " ";
            sql += " where id_media=" + _idMedia + "";
            if (_beginingDate.Length > 0)
                sql += " and date_media_num>=" + _beginingDate + " ";
            if (_endDate.Length > 0)
                sql += " and date_media_num<=" + _endDate + "";

            if (_vehicleName != DBClassificationConstantes.Vehicles.names.outdoor) sql += " and insertion=1";
            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += listProductHap;

            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetInsertionNumber(): " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Type Sale outdoor
        /// <summary>
        /// Get Type Sale
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetTypeSale() {

            #region Construction de la requête

            string table = GetTableData();

            string sql = "select distinct type_sale";
            sql += " from  " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + table;
            sql += " where id_media=" + _idMedia + " ";
            if (_beginingDate.Length > 0)
                sql += " and  date_media_num>=" + _beginingDate + " ";
            if (_endDate.Length > 0)
                sql += " and  date_media_num<=" + _endDate + " ";

            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetTypeSale(): " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Pages
        /// <summary>
        /// Get pages number
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet GetPage() {

            #region Construction de la requête

            string sql = "select sum(number_page_media) page";
            sql += " from " + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.alarmMedia);
            sql += " where " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".id_media=" + _idMedia + " ";
            if (_beginingDate.Length > 0)//date_cover_num
                sql += " and  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".DATE_ALARM>=" + _beginingDate + " ";
            if (_endDate.Length > 0)//date_cover_num
                sql += " and  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".DATE_ALARM<=" + _endDate + " ";
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetPage(): " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Nombre de produit(nouveau,dans la pige), annonceurs
        /// <summary>
        /// Tableau contenant le nombre de produits, 
        /// le nombre de nouveau produit dans le support,
        /// le nombre de nouveau produit dans la pige,
        /// le nombre d'annonceurs		
        /// </summary>
        /// <returns>Données</returns>
        public object[] NumberProductAdvertiser() {

            #region Variables
            object[] tab = new object[4];
            DataSet ds = null;
            string sql = "";
            string tableName = "";
            string productsRights = null;
            string product = null;
            string mediaRights = null;
            string listProductHap = null;
            int index = 0;
            #endregion

            #region Construction de la requête
            try {
                tableName = GetTableData();
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                product = GetProductData();
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

                for (int i = 0; i <= 3; i++) {
                    if ((_vehicleName != DBClassificationConstantes.Vehicles.names.outdoor) || (i != 2 && i != 1)) {
                        if (i <= 2) {
                            sql += " select count(id_product) as total ";
                            sql += " from( ";
                            sql += " select  id_product ";
                        }
                        else {
                            sql += " select count(id_advertiser) as total ";
                            sql += " from( ";
                            sql += " select  id_advertiser ";
                        }
                        sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                        sql += " where id_media=" + _idMedia + "";
                        // Support
                        if (i == 1)
                            sql += " and new_product=1 ";
                        // Pige
                        if (i == 2)
                            sql += " and new_product=2 ";

                        if (_beginingDate.Length > 0)
                            sql += " and  DATE_MEDIA_NUM>= " + _beginingDate + " ";
                        if (_endDate.Length > 0)
                            sql += " and  DATE_MEDIA_NUM<= " + _endDate + " ";

                        sql += product;
                        sql += productsRights;
                        sql += mediaRights;
                        sql += listProductHap;

                        if (i <= 2) {
                            sql += " group by id_product )";
                        }
                        else {
                            sql += " group by id_advertiser )";
                        }

                        if (i <= 2)
                            sql += " UNION ALL ";
                    }
                }

            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build request for : NumberProductAdvertiser()" + sql, err));
            }
            #endregion

            #region Execution de la requête
            try {
                ds = _webSession.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    foreach (DataRow row in ds.Tables[0].Rows) {
                        tab[index] = row["total"].ToString();
                        if (_vehicleName == DBClassificationConstantes.Vehicles.names.outdoor){
                            index = index + 3;
                        }
                        else
                            index++;
                    }
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for NumberProductAdvertiser(): " + sql, err));
            }
            #endregion

            return tab;
        }
        #endregion

        #region Page Encart
        /// <summary>
        /// Encart
        /// </summary>
        /// <returns>Données</returns>
        public virtual object[] NumberPageEncart() {

            #region Constantes
            //préfixe table à utiliser
            const string LIST_ENCART = "85,108,999";
            #endregion

            #region Variables
            object[] tab = new object[4];
            DataSet ds = null;
            string sql = "";
            string tableName = "";
            string productsRights = null;
            string mediaRights = null;
            string product = null;
            string listProductHap = null;
            int index=0;
            #endregion

            #region Build query
            try {
                tableName = GetTableData();
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                product = GetProductData();
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

                for (int i = 0; i <= 2; i++) {

                    sql += " select sum(area_page) as page ";
                    sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                    sql += " where ID_MEDIA=" + _idMedia + " ";
                    // hors encart
                    if (i == 1) {
                        sql += " and id_inset=null ";
                    }
                    // Encart
                    if (i == 2) {
                        sql += " and id_inset in (" + LIST_ENCART + ") ";
                    }
                    if (_beginingDate.Length > 0)
                        sql += " and  DATE_MEDIA_NUM>=" + _beginingDate + " ";
                    if (_endDate.Length > 0)
                        sql += " and  DATE_MEDIA_NUM<=" + _endDate + " ";

                    sql += product;
                    sql += productsRights;
                    sql += mediaRights;
                    sql += listProductHap;

                    if (i < 2)
                        sql += " UNION ALL ";

                }
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build request for NumberPageEncart(): " + sql, err));
            }
            #endregion

            #region Execution de la requête
            try {
                ds = _webSession.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    foreach (DataRow row in ds.Tables[0].Rows) {
                        tab[index] = row["page"].ToString();
                        index++;
                    }

            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for NumberPageEncart(): " + sql, err));
            }
            #endregion

            return tab;

        }
        #endregion

        #region Investment By Media
        /// <summary>
        /// Get Investment By Media
        /// </summary>
        /// <returns>Data</returns>
        public Hashtable GetInvestmentByMedia() {

            #region Variables
            string sql = "";
            Hashtable htInvestment = new Hashtable();
            string product = null;
            string productsRights = null;
            string mediaRights = null;
            string listProductHap = null;
            #endregion

            try {
                product = GetProductData();
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request for GetInvestmentByMedia() : " + sql, err));
            }

            #region Construction de la requête
            sql += " select sum(insertion) insertion,sum(expenditure_euro) investment,date_cover_num date1";
            sql += "  from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.ALERT_DATA_PRESS + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            sql += " where id_media=" + _idMedia + " ";
            if (_beginingDate.Length > 0)
                sql += " and date_media_num>=" + _beginingDate + " ";
            if (_endDate.Length > 0)
                sql += "  and date_media_num<=" + _endDate + " ";
            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += listProductHap;
            sql += " group by date_cover_num ";	
            #endregion

            #region Execution de la requête
            try {
                DataSet ds = _webSession.Source.Fill(sql);
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
                    string[] value1 = null;
                    foreach (DataRow current in ds.Tables[0].Rows) {
                        value1 = new string[2];
                        value1[0] = current["investment"].ToString();
                        value1[1] = current["insertion"].ToString();
                        htInvestment.Add(current["date1"], value1);
                    }
                }
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("error whene getting investment by media", err));
            }
            #endregion

            return (htInvestment);
        }
        #endregion

        #region Ecran
        /// <summary>
        /// récupère les écrans
        /// </summary>
        /// <returns>Ecrans</returns>
        public DataSet GetEcranData() {

            #region Variables
            string select = "";
            string table = "";
            string product = "";
            string productsRights = "";
            string mediaRights = "";
            string listProductHap = "";
            #endregion

            #region Construction de la requête
            try {
                select = GetSelectDataEcran();
                table = GetTableData();
                product = GetProductData();
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request for GetEcranData()", err));
            }

            string sql = "select sum(insertion) as nbre_ecran,sum(ecran_duration) as ecran_duration ,sum(nbre_spot) as nbre_spot";

            sql += " from ( ";

            sql += select;
            sql += " from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + table + " wp ";
            sql += " where id_media=" + _idMedia + " ";
            if (_beginingDate.Length > 0)
                sql += " and date_media_num>=" + _beginingDate + " ";
            if (_endDate.Length > 0)
                sql += " and date_media_num<=" + _endDate + "";

            sql += " and insertion=1";
            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += listProductHap;
            sql += " )";

            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetEcranData() : " + sql, err));
            }
            #endregion

        }
        #endregion

        #endregion

		#region Get Generic Synthesis ( Used in Portofolio analysis )

		#region GetRequestForAnalysisPortofolio
		/// <summary>
		/// Build query to get total investment, nd advert, spot duration
		/// </summary>	
		/// <param name="type">Type table </param>
		/// <returns>Query string</returns>
		protected virtual string GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type type) {

			#region Build Sql query			
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;

			string table = string.Empty;
			//Data table			
			switch (type) {
				case DBConstantes.TableType.Type.dataVehicle4M:
					table = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName, WebConstantes.Module.Type.alert);					
					break;
				case DBConstantes.TableType.Type.dataVehicle:
					table = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName, WebConstantes.Module.Type.analysis);					
					break;
				case DBConstantes.TableType.Type.webPlan:
					table = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData).SqlWithPrefix; //DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
					break;
				default:
					throw (new CompetitorDataAccessException("Table type unknown"));
			}
			string product = GetProductData();
			string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
			string productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
			//list product hap
			string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
			string date = string.Empty;

			switch (type) {
				case DBConstantes.TableType.Type.dataVehicle4M:
				case DBConstantes.TableType.Type.dataVehicle:
					date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
					break;
				case DBConstantes.TableType.Type.webPlan:
					date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
					break;
			}

			string sql = null;

			switch (type) {
				case DBConstantes.TableType.Type.dataVehicle4M:
				case DBConstantes.TableType.Type.dataVehicle:
					sql += "select " + WebFunctions.SQLGenerator.getUnitFieldsNameForAnalysisPortofolio(_vehicleName, DBConstantes.TableType.Type.dataVehicle);
					break;
				case DBConstantes.TableType.Type.webPlan:
					sql += "select " + WebFunctions.SQLGenerator.getUnitFieldsNameForAnalysisPortofolio(_vehicleName, DBConstantes.TableType.Type.webPlan);
					break;
			}
			if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
				sql += ", " + date + " as date_num ";
			}
			sql += " from " + table;
			sql += " where id_media=" + _idMedia + "";
			// Period
			switch (type) {
				case DBConstantes.TableType.Type.dataVehicle4M:
					sql += " and " + date + " >=" + customerPeriod.StartDate;
					sql += " and " + date + " <=" + customerPeriod.EndDate;
					break;
				case DBConstantes.TableType.Type.dataVehicle:
					if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
						sql += " and " + date + " >=" + customerPeriod.StartDate;
						sql += " and " + date + " <=" + customerPeriod.EndDate;
					}
					else if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
						sql += " and ((" + date + " >=" + customerPeriod.PeriodDayBegin[0];
						sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[0];
						sql += " ) or (" + date + " >=" + customerPeriod.PeriodDayBegin[1];
						sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[1];
						sql += "))";
					}
					else {
						sql += " and " + date + " >=" + customerPeriod.PeriodDayBegin[0];
						sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[0];
					}
					break;
				case DBConstantes.TableType.Type.webPlan:
					sql += " and " + date + " >=" + customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6);
					sql += " and " + date + " <=" + customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6);
					break;
			}

			sql += product;
			sql += productsRights;
			sql += mediaRights;
			sql += listProductHap;
			if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
				sql += " group by " + date;
			}
			#endregion

			return sql.ToString();

		}
		#endregion

		#region GetGenericDataForAnalysisPortofolio
		///<summary>
		/// Get total investment, nb advet, spot duration
		/// </summary>		
		/// <returns>Query string</returns>
		public virtual DataSet GetSynthsesisUnitsData() {

			#region Variables
			string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			#endregion

			#region Construction de la requête
			try {

				if (customerPeriod.Is4M) {
					sql4M = GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type.dataVehicle4M);
					sql = sql4M;
				}
				else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
					sql = GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type.dataVehicle);
				}
				else {

					if (customerPeriod.IsDataVehicle) {
						sqlDataVehicle = GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type.dataVehicle);
						sql = sqlDataVehicle;
					}

					if (customerPeriod.IsWebPlan) {
						sqlWebPlan = GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type.webPlan);
						sql = sqlWebPlan;
					}

					if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
						sql = "";
						sql += " select " + WebFunctions.SQLGenerator.getUnitFieldsNameForMediaDetailResult(_vehicleName, TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS);
						sql += " from (";
						sql += sqlDataVehicle;
						sql += " UNION ";
						sql += sqlWebPlan;
						sql += " ) ";
					}

				}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build query for total investment, nb ad, sport duration" + sql, err));
			}
			#endregion

			#region Execution de la requête
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to exectue query" + sql, err));
			}
			#endregion

		}
		#endregion

		#region GetRequestForNumberProductAdvertiser
		/// <summary>
		/// Get Nb product and advetiser for a one media
		/// </summary>
		/// <param name="type">Type table </param>
		/// <param name="index">Iteration</param>
		/// <returns>object []	at index 0 the nb of product,  at index 1 the nb of advertiser</returns>
		protected virtual string GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type type, int index) {
		
			#region Variables
			string sql = "";
			string tableName = "";
			string productsRights = "";
			string product = "";
			string mediaRights = "";
			string listProductHap = "";
			string date = "";
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			#endregion

			try {
				// Table de données
				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
						tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName, WebConstantes.Module.Type.alert);
						break;
					case DBConstantes.TableType.Type.dataVehicle:
						tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName, WebConstantes.Module.Type.analysis);
						break;
					case DBConstantes.TableType.Type.webPlan:
						tableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData).SqlWithPrefix; 
						break;
					default:
						throw (new PortofolioDALException("Table type unknown"));
				}
				product = GetProductData();
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
					case DBConstantes.TableType.Type.dataVehicle:
						date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
						break;
					case DBConstantes.TableType.Type.webPlan:
						date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
						break;
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build sql query", err));
			}

			#region Construction de la requête
			if (index == 0) {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql = " select id_product, " + date + " as date_num, count(rowid) as nbLines";
				else
					sql = " select id_product, count(rowid) as nbLines";
			}
			else {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql = " select id_advertiser, " + date + " as date_num, count(rowid) as nbLines";
				else
					sql = " select id_advertiser, count(rowid) as nbLines";
			}
			sql += " from " + tableName;
			sql += " where id_media=" + _idMedia + "";
			// Period
			switch (type) {
				case DBConstantes.TableType.Type.dataVehicle4M:
					sql += " and " + date + " >=" + customerPeriod.StartDate;
					sql += " and " + date + " <=" + customerPeriod.EndDate;
					break;
				case DBConstantes.TableType.Type.dataVehicle:
					if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
						sql += " and " + date + " >=" + customerPeriod.StartDate;
						sql += " and " + date + " <=" + customerPeriod.EndDate;
					}
					else if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
						sql += " and ((" + date + " >=" + customerPeriod.PeriodDayBegin[0];
						sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[0];
						sql += " ) or (" + date + " >=" + customerPeriod.PeriodDayBegin[1];
						sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[1];
						sql += "))";
					}
					else {
						sql += " and " + date + " >=" + customerPeriod.PeriodDayBegin[0];
						sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[0];
					}
					break;
				case DBConstantes.TableType.Type.webPlan:
					sql += " and " + date + " >=" + customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6);
					sql += " and " + date + " <=" + customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6);
					break;
			}
			sql += product;
			sql += productsRights;
			sql += mediaRights;
			sql += listProductHap;
			if (index == 0) {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql += " group by id_product, " + date;
				else
					sql += " group by id_product ";
			}
			else {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql += " group by id_advertiser, " + date;
				else
					sql += " group by id_advertiser ";
			}
			#endregion

			return sql;
		}
		#endregion

		#region Get Generic Data For Number Product Advertiser
		/// <summary>
		/// Get Nb product and Nb advertiser for one media
		/// </summary>		
		/// <returns>object [] at 0 the nb product,at 1 the nb advertiser</returns>
		public virtual object[] GetNumberProductAdvertiser() {

			#region Variables
			string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			object[] tab = new object[4];
			int compteur = 0;
			DataSet ds = null;
			#endregion
		
			try {

				for (int i = 0; i <= 1; i++) {

					if (customerPeriod.Is4M) {
						sql4M = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.dataVehicle4M, i);
						sql = sql4M;
					}
					else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
						sql = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.dataVehicle, i);
					}
					else {

						if (customerPeriod.IsDataVehicle) {
							sqlDataVehicle = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.dataVehicle, i);
							sql = sqlDataVehicle;
						}

						if (customerPeriod.IsWebPlan) {
							sqlWebPlan = GetRequestForNumberProductAdvertiser(DBConstantes.TableType.Type.webPlan, i);
							sql = sqlWebPlan;
						}

						if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
							sql = "";
							if (i == 0)
								sql += " select id_product, sum(nbLines) as nbLines";
							else
								sql += " select id_advertiser, sum(nbLines) as nbLines";
							sql += " from (";
							sql += sqlDataVehicle;
							sql += " UNION ";
							sql += sqlWebPlan;
							sql += " ) ";
							if (i == 0) {
								sql += " group by id_product ";
							}
							else {
								sql += " group by id_advertiser ";
							}
						}
					}

					#region Execute query
					try {
						 ds =  _webSession.Source.Fill(sql);
					}
					catch (System.Exception err) {
						throw (new PortofolioDALException("Impossible exectute query : " + sql, err));
					}
					#endregion
					if(ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count>0){
						compteur = ds.Tables[0].Rows.Count;
					}					
					tab[i] = compteur;
					compteur = 0;
				}
			}
			catch(System.Exception ex){
				throw (new PortofolioDALException("Impossible to build table for nb advertiser and product : ", ex));
			}			
			

			return tab;
		}
		#endregion

		#endregion

        #region Media portofolio
        /// <summary>
        /// Get Data for the Media portofolio
        /// </summary>
        /// <returns>Data Set</returns>
        public virtual DataSet GetMediaPortofolio() {

            #region Variables
            string dataTableName="";
            string dataTableNameForGad="";
            string dataFieldsForGad="";
            string dataJointForGad="";
            string detailProductTablesNames="";
            string detailProductFields="";
            string detailProductJoints="";
            string detailProductOrderBy="";
            string unitsFields="";
            string productsRights="";
            string sql="";
            string mediaList="";
            string dataJointForInsert="";
            string listProductHap="";
            string mediaRights="";
            string mediaAgencyTable=string.Empty;
            string mediaAgencyJoins=string.Empty;
            #endregion

            #region Construction de la requête
            try {
                dataTableName=WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName,_module.ModuleType);
                detailProductTablesNames=_webSession.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
                detailProductFields=_webSession.GenericProductDetailLevel.GetSqlFields();
                detailProductJoints=_webSession.GenericProductDetailLevel.GetSqlJoins(_webSession.SiteLanguage,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                unitsFields = WebFunctions.SQLGenerator.GetUnitFields(_vehicleName,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);
                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);
                detailProductOrderBy=_webSession.GenericProductDetailLevel.GetSqlOrderFields();
                //option encarts (pour la presse)
                if(DBClassificationConstantes.Vehicles.names.press==_vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==_vehicleName)
                    dataJointForInsert=WebFunctions.SQLGenerator.GetJointForInsertDetail(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true,false);

                if(_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)) {
                    try {
                        dataTableNameForGad=", "+WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).SqlWithPrefix; //+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GAD+" "+DBConstantes.Tables.GAD_PREFIXE;
                        dataFieldsForGad=", "+WebFunctions.SQLGenerator.GetFieldsAddressForGad();
                        dataJointForGad="and "+WebFunctions.SQLGenerator.GetJointForGad(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    }
                    catch(SQLGeneratorException) { ;}
                }
				
            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible d'initialiser les paramètres de la requêtes",err));
            }

            sql+=" select "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media, "+detailProductFields+dataFieldsForGad+","+unitsFields;
            sql+=" from "+dataTableName ;
            if(detailProductTablesNames.Length > 0)
                sql+=", "+detailProductTablesNames;
            sql+=" "+dataTableNameForGad;
            // Période
            sql+=" where date_media_num >="+_beginingDate;
            sql+=" and date_media_num <="+_endDate;
            // Jointures Produits
            sql+=" "+detailProductJoints;
            sql+=" "+dataJointForGad;
            sql+=" "+mediaAgencyJoins;

            //Jointures encart
            if(DBClassificationConstantes.Vehicles.names.press==_vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==_vehicleName)
                sql+=" "+dataJointForInsert;

            #region Sélection de Médias
            mediaList+=_webSession.GetSelection((TreeNode)_webSession.ReferenceUniversMedia,CustormerConstantes.Right.type.mediaAccess);
            if(mediaList.Length>0) sql+=" and "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media in ("+mediaList+")";
            #endregion

            #region Sélection de Produits
            sql += " " + GetProductData();
            #endregion

            // Droits des Médias
            // Droits des Produits
            sql+=" "+productsRights;
            sql+=mediaRights;
            sql+=listProductHap;
            // Group by
            sql+=" group by "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media, "+detailProductFields+dataFieldsForGad;
            // Order by
            sql+=" order by "+detailProductOrderBy+","+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media";
            #endregion

            #region Execution de la requête
            try {
                return (_webSession.Source.Fill(sql.ToString()));
            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible de charger des données pour le détail du portefeuille: "+sql,err));
            }
            #endregion
        }
        #endregion

		#region Generic Détail media insertion
		
		/// <summary>
		/// Get media detail insertion
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="code_ecran">code ecran</param>
		/// <param name="allPeriod">vrai si le détail des insertions concerne toute la période sélectionnée</param>
		/// <returns>liste des publicités pour un média donné</returns>
		public virtual DataSet GetGenericDetailMedia() {
			
			#region Variables
			StringBuilder sql = new StringBuilder(5000);
			string sqlFields = "";
			string sqlConstraintFields = "";
			string sqlTables = "";
			string sqlConstraintTables = "";
			string listProductHap = "";
			string product = "";
			string productsRights = "";
			string mediaRights = "";
			string orderby = "";
			bool allPeriod = (_adBreak != null && _adBreak.Length>0) ? false : true;
			#endregion

			try {				
				sqlFields = _webSession.GenericInsertionColumns.GetSqlFields(null);
				sqlConstraintFields = _webSession.GenericInsertionColumns.GetSqlConstraintFields();
				string tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName,WebConstantes.Module.Type.alert); //WebFunctions.SQLGenerator.GetVehicleTableNameForAlertDetailResult(_vehicleName);
				sqlTables = _webSession.GenericInsertionColumns.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, null);
				sqlConstraintTables = _webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);

				//Select
				sql.Append(" select distinct");				
				if (sqlFields.Length > 0) sql.Append(" " + sqlFields);
				
				if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
					sql.Append(" , advertising_agency");

				if (_vehicleName == DBClassificationConstantes.Vehicles.names.press || _vehicleName == DBClassificationConstantes.Vehicles.names.internationalPress)
					sql.Append(" , date_cover_num");

				if (sqlConstraintFields.Length > 0)
					sql.Append(" , " + sqlConstraintFields);//Fields for constraint management

				sql.Append(" from ");
				sql.Append(" "  + tableName +" ");
				
				if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
					sql.Append(", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).SqlWithPrefix);
				
				if (sqlTables.Length > 0) sql.Append(" ," + sqlTables);
				
				if (sqlConstraintTables.Length > 0)
					sql.Append(" , " + sqlConstraintTables);//Tables pour la gestion des contraintes métiers

				// Joints conditions
				sql.Append(" Where ");

				sql.Append(" "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +".id_media=" + _idMedia + " ");
				sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num>=" + _beginingDate + " ");
				sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num<=" + _endDate + " ");

				if (_webSession.GenericInsertionColumns.GetSqlJoins(_webSession.SiteLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null).Length > 0)
					sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlJoins(_webSession.SiteLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null));
				sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlContraintJoins());

				if (tableName.CompareTo(DBConstantes.Tables.ALERT_DATA_RADIO) == 0 && _adBreak != null && _adBreak.Length > 0)
					sql.Append("  and commercial_break = " + _adBreak + "");


				if (tableName.CompareTo(DBConstantes.Tables.ALERT_DATA_TV) == 0 && _adBreak != null && _adBreak.Length > 0) {
					sql.Append(" and id_commercial_break = " + _adBreak + "");
				}
				

				listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
				product = GetProductData();
				productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				orderby = GetOrderByDetailMedia(allPeriod);

				if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia)) {
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix + ".id_advertising_agency(+)=" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_advertising_agency ");
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix + ".id_language(+)=" + _webSession.SiteLanguage + " ");
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix + ".activation(+)<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ");
				}

				#region Droits
				//liste des produit hap
				sql.Append(listProductHap);
				sql.Append(product);
				sql.Append(productsRights);
				sql.Append(mediaRights);

				//Rights detail spot to spot TNT
				if (_vehicleName == DBClassificationConstantes.Vehicles.names.tv
					&& _webSession.CustomerLogin.GetFlag(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG) == null)
					sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_category != " + DBConstantes.Category.ID_DIGITAL_TV + "  ");

				#endregion

				// Order by
				sql.Append(orderby);

			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to the query " + sql.ToString(), err));
			}

			#region Query execution 
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to exectute query of  media detail : " + sql.ToString(), err));
			}
			#endregion

		}
		#endregion

		#region GetGenericData
		/// <summary>
		/// Get data for media detail
		/// </summary>	
		/// <returns>Data set</returns>
		public virtual DataSet GetGenericData() {
		
			#region Variables
			string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
			string groupByFieldNameWithoutTablePrefix = string.Empty;
			string orderFieldName = string.Empty, orderFieldNameWithoutTablePrefix = string.Empty;
			string productFieldNameWithoutTablePrefix = string.Empty, unitField = string.Empty, dataFieldsForGadWithoutTablePrefix = string.Empty;
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			#endregion

			#region Construction de la requête
			try {
				orderFieldName = _webSession.GenericProductDetailLevel.GetSqlOrderFields();
				orderFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
				groupByFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();

				if (customerPeriod.Is4M) {
					sql4M = GetRequest(DBConstantes.TableType.Type.dataVehicle4M);
					sql4M += " order by " + orderFieldName + "," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media ";
					sql = sql4M;
				}
				else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
					sql = GetRequest(DBConstantes.TableType.Type.dataVehicle);
					sql += " order by " + orderFieldName + "," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media ";
				}
				else {

					if (customerPeriod.IsDataVehicle) {
						sqlDataVehicle = GetRequest(DBConstantes.TableType.Type.dataVehicle);
						sql = sqlDataVehicle;
					}

					if (customerPeriod.IsWebPlan) {
						sqlWebPlan = GetRequest(DBConstantes.TableType.Type.webPlan);
						sql = sqlWebPlan;
					}

					if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
						productFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();
						if (_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
							dataFieldsForGadWithoutTablePrefix = ", " + WebFunctions.SQLGenerator.GetFieldsAddressForGad("");
						sql = "";
						sql += " select id_media, " + productFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix + ", " + WebFunctions.SQLGenerator.getUnitFieldsNameForMediaDetailResult(_vehicleName, TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO);
						sql += " from (";
						sql += sqlDataVehicle;
						sql += " UNION ";
						sql += sqlWebPlan;
						sql += " ) ";
						sql += " group by id_media, " + groupByFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix;
					}

					sql += " order by " + orderFieldNameWithoutTablePrefix + ", id_media ";
				}
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build detail media Portotofolio query " + sql, err));
			}
			#endregion

			#region Execution de la requête
			try {
				return _webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("mpossible to exectute detail media Portotofolio query" + sql, err));
			}

			#endregion

		}
		#endregion

        #region Calendar
        /// <summary>
        /// Get Portofolio calendar
        /// </summary>
        /// <returns>Calendar data set</returns>
        public DataSet GetDataCalendar() {

            #region Variables
            string dataTableName="";
            string dataTableNameForGad="";
            string dataFieldsForGad="";
            string dataJointForGad="";
            string detailProductTablesNames="";
            string detailProductFields="";
            string detailProductJoints="";
            string detailProductOrderBy="";
            string unitField="";
            string productsRights="";
            string sql="";
            string list="";
            //int positionUnivers=1;
            string mediaList="";
            bool premier;
            string dataJointForInsert="";
            string listProductHap="";
            string mediaRights="";
            string mediaAgencyTable=string.Empty;
            string mediaAgencyJoins=string.Empty;
            #endregion

            #region Construction de la requête
            try {
                dataTableName=WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName,_module.ModuleType);
                detailProductTablesNames=_webSession.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
                detailProductFields=_webSession.GenericProductDetailLevel.GetSqlFields();
                detailProductJoints=_webSession.GenericProductDetailLevel.GetSqlJoins(_webSession.SiteLanguage,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                unitField = WebFunctions.SQLGenerator.GetUnitFieldName(_webSession);
                mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);
                productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);
                detailProductOrderBy=_webSession.GenericProductDetailLevel.GetSqlOrderFields();
                //option encarts (pour la presse)
                if(DBClassificationConstantes.Vehicles.names.press==_vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==_vehicleName)
                    dataJointForInsert=WebFunctions.SQLGenerator.GetJointForInsertDetail(_webSession,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true,false);

                if(_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)) {
                    try {
                        dataTableNameForGad=dataTableNameForGad=", "+WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).SqlWithPrefix;
                        dataFieldsForGad=", "+WebFunctions.SQLGenerator.GetFieldsAddressForGad();
                        dataJointForGad="and "+WebFunctions.SQLGenerator.GetJointForGad(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    }
                    catch(SQLGeneratorException) { ;}
                }
				////Agence_media
				//if(_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency)||_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency)) {
				//    mediaAgencyTable=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql+"."+_webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+",";
				//    mediaAgencyJoins="And "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_product="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product ";
				//    mediaAgencyJoins+="And "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_vehicle="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle ";
				//}
            }
            catch(Exception e) {
                throw (new PortofolioDALException("Impossible d'initialiser les paramètres de la requêtes"+e.Message));
            }

            sql+=" select "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media, "+detailProductFields+dataFieldsForGad+",sum("+unitField+") as unit";
            sql+=", date_media_num"; 
            sql+=" from "+mediaAgencyTable+dataTableName;
            if(detailProductTablesNames.Length > 0)
                sql+=", "+detailProductTablesNames;
            sql+=" "+dataTableNameForGad;
            // Période
            sql+=" where date_media_num >="+_beginingDate;
            sql+=" and date_media_num <="+_endDate;
            // Jointures Produits
            sql+=" "+detailProductJoints;
            sql+=" "+dataJointForGad;
            sql+=" "+mediaAgencyJoins;
            //Jointures encart
            if(DBClassificationConstantes.Vehicles.names.press==_vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==_vehicleName)
                sql+=" "+dataJointForInsert;

            #region Sélection de Médias
            mediaList+=_webSession.GetSelection((TreeNode)_webSession.ReferenceUniversMedia,CustormerConstantes.Right.type.mediaAccess);
            if(mediaList.Length>0) sql+=" and "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media in ("+mediaList+")";
            #endregion

            #region Sélection de Produits
            sql += " " + GetProductData();
            #endregion

            // Droits des Médias
            // Droits des Produits
            sql+=" "+productsRights;
            sql+=mediaRights;
            sql+=listProductHap;
            // Group by
            sql+=" group by "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media, "+detailProductFields+dataFieldsForGad;
            sql+=",date_media_num";
            // Order by
            sql+=" order by "+detailProductOrderBy+","+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media";
            sql+=",date_media_num";
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch(System.Exception err) {
                throw (new PortofolioDALException("Impossible de charger des données pour les nouveauté: "+sql,err));
            }
            #endregion

        }

        #endregion        

        #region Structure

        #region Dataset for tv or radio
        /// <summary>
        /// Get structure data 
        /// </summary>
		/// <remarks>Used for tv or radio</remarks>
		/// <param name="hourBegin">Hour Begin</param>
		/// <param name="hourEnd">Hour End</param>
        /// <returns>DataSet</returns>
        public virtual DataSet GetStructData(int hourBegin, int hourEnd) {

            #region variables
            string tableName = "";
            string fields = "";
            string idVehicle = "";
            string idMedia = "";
            StringBuilder sql = new StringBuilder(2000);
            string product = "";
            #endregion

            #region construction de la requête
            try {
                //Table name
				tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName, WebConstantes.Module.Type.alert);
                //Fields
				fields = GetStructFields();
            }
            catch (Exception) {
				throw new PortofolioDALException("GetStructData : impossible to determine Table and Fields for the query.");
            }

			if (tableName != null && tableName.Length>0 ) {

                // Select 
                sql.Append("  select " + fields);

                // Tables
                sql.Append(" from "+ tableName + " ");
               
				//Where
				 sql.Append("  where ");

                // Period conditions
				sql.Append("  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +".date_media_num >= " + _beginingDate);
				sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num <= " + _endDate);
                
				// Hour interval
                sql.Append(" "+ GetHourInterval(hourBegin, hourEnd));

                #region Product Rights

				//Access Rights
               sql.Append( WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
               
				//list products hap
                string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
				if (listProductHap != null && listProductHap.Length>0)
                     sql.Append(listProductHap);
                
				//List selected products
                product = GetProductData();
				if (product != null && product.Length>0)
                    sql.Append(product);
                #endregion

                #region Nomenclature Media (Rights and selection)

                #region Media Rights
                 sql.Append( WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
                #endregion

                #region Media selection 
                //Vehicle selection média (vehicle)

				 sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_vehicle = " + _vehicleName.GetHashCode().ToString());
                
				//Media selection	
                   sql.Append( " and "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media = " + _idMedia.ToString());
                #endregion

                #endregion

            }
            #endregion

            #region Query execution 
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to get data for GetStructData(int hourBegin, int hourEnd) : " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #region Get Press Struct Data
        /// <summary>
        ///  Get Press Struct Data
        /// </summary>
        /// <param name="ventilation">ventilation</param>
        /// <returns>DataSet</returns>
        public DataSet GetPressStructData(PortofolioStructure.Ventilation ventilation) {

            #region variables
            string tableName = "";
            string fields = "";
            string idVehicle = "";
            string idMedia = "";
			StringBuilder sql = new StringBuilder(2000);
            string product = "";
            #endregion

            #region Build query
            try {
                //Table name
				tableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName, WebConstantes.Module.Type.alert); 
                //Fields
                fields = GetPressStructFields(ventilation);
            }
            catch (Exception) {
                throw new PortofolioDALException("getPressStructFields(PortofolioStructure.Ventilation ventilation)--> Table unknown.");
            }

			if (tableName != null && tableName.Length > 0) {
                
				// Select
                sql.Append( " select " + fields);

                // Tables
                  sql.Append( " from ");
                 sql.Append( GetPressStructTables(tableName, ventilation));

                 sql.Append( " where ");
                //Choix de la langue
                  sql.Append(GetPressStructLanguage(ventilation));

                //press joints
                  sql.Append(GetPressStructJoint(ventilation));

                // Period condition
				  sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num>=" + _beginingDate);
				  sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num<=" + _endDate);

                #region Products Rights
                            
				//Access Rights
				sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

				//list products hap
				string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
				if (listProductHap != null && listProductHap.Length > 0)
					sql.Append(listProductHap);

				//List selected products
				product = GetProductData();
				if (product != null && product.Length > 0)
					sql.Append(product);
                #endregion

				#region Nomenclature Media (Rights and selection)

				#region Media Rights
				sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
				#endregion

				#region Media selection
				//Vehicle selection média (vehicle)

				sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_vehicle = " + _vehicleName.GetHashCode().ToString());

				//Media selection	
				sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media = " + _idMedia.ToString());
				#endregion

				#endregion

                #region regroupement
               sql.Append(GetPressStructGroupBy(ventilation));
                #endregion

                //Order
                sql.Append( " order by insertion desc");
            }
            #endregion

            #region Query Execution 
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetPressStructData : " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #endregion

        #region Get dates list
        /// <summary>
        /// Get dates list
        /// </summary>
        /// <param name="conditionEndDate">Add condition end date</param>
        /// <returns>DataSet</returns>
        public DataSet GetListDate(bool conditionEndDate) {

            string tableName = "";
            try {
                tableName = GetAlertTable();
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Error when getting table name", err));
            }

            #region Construction de la requête
            StringBuilder sql = new StringBuilder(500);

            sql.Append("select distinct date_media_num ");

            if (_vehicleName == DBClassificationConstantes.Vehicles.names.press
                || _vehicleName == DBClassificationConstantes.Vehicles.names.internationalPress) {
                sql.Append(", disponibility_visual ");
                sql.Append(", number_page_media ");
                sql.Append(", date_cover_num ");
            }
            sql.Append(", media ");
            sql.Append(" from ");
            sql.Append(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

            if (_vehicleName == DBClassificationConstantes.Vehicles.names.press
                || _vehicleName == DBClassificationConstantes.Vehicles.names.internationalPress) {
                sql.Append("," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).SqlWithPrefix + " ");
                sql.Append("," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).SqlWithPrefix + " ");
            }
            sql.Append("," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).SqlWithPrefix + " ");
            sql.Append(" where " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media" + " ");
            sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media=" + _idMedia + " ");
            // Période			

            if (_beginingDate.Length > 0)
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num>=" + _beginingDate);
            if (_endDate.Length > 0 && conditionEndDate)
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num<=" + _endDate);

            if (_vehicleName == DBClassificationConstantes.Vehicles.names.press
                || _vehicleName == DBClassificationConstantes.Vehicles.names.internationalPress) {

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix + ".date_debut(+) = " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num ");

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix + ".id_project(+) = " + CstProject.ADEXPRESS_ID + " ");
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix + ".id_media(+) = " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media ");

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media=al.id_media(+)");

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num=al.DATE_ALARM(+)");

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".id_media(+)=" + _idMedia + " ");
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".ID_LANGUAGE_I(+)=" + _webSession.SiteLanguage + " ");
                sql.Append(" and  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".DATE_ALARM(+)>=" + _beginingDate + " ");
                if (_endDate.Length > 0 && conditionEndDate)
                    sql.Append(" and  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix + ".DATE_ALARM(+)<=" + _endDate + " ");

            }
            // Tri			
            sql.Append(" order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num desc");

            #endregion

            #region Execution de la requête
            try {
                return (_webSession.Source.Fill(sql.ToString()));
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Erreur lors de la sélection de la table", err));
            }
            #endregion

        }
        #endregion

        #region Get Commercial Break For Tv & Radio
        /// <summary>
        /// Get Commercial Break For Tv & Radio
        /// </summary>
        /// <returns>Liste des codes ecrans</returns>
        public DataSet GetCommercialBreakForTvRadio() {

            #region Varaibles
            string selectFields = "";
            string tableName = "";
            string groupByFields = "";
            string listProductHap = "";
            string product = "";
            string productsRights = "";
            string mediaRights = "";
            string sql = "";
            #endregion

            #region Construction de la requête

            try {
                selectFields = GetFieldsDetailMediaForTvRadio();
                tableName = WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(_vehicleName,WebConstantes.Module.Type.alert);
                groupByFields = GetGroupByDetailMediaForTvRadio();
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
                product = GetProductData();
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request", err));
            }

            sql += "select " + selectFields;
            sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + " ";
            sql += " where id_media=" + _idMedia + "  ";
            sql += " and date_media_num>=" + _beginingDate + " ";
            sql += " and date_media_num<=" + _endDate + " ";
            sql += " and insertion=1 ";
            sql += listProductHap;
            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += groupByFields;


            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible de charger des données pour les nouveauté: " + sql, err));
            }
            #endregion

        }
        #endregion
        
		#region Is Media belong To Category
		/// <summary>
		/// Cheks if media belong to Category
		/// </summary>
		/// <param name="idMedia">Id media</param>
		/// <param name="idCategory">Id Category</param>
		/// <returns>True if media belong to Category</returns>
		public bool IsMediaBelongToCategory(Int64 idMedia, Int64 idCategory) {

			StringBuilder t = new StringBuilder(1000);
			DataTable dt = null;

			t.Append(" select  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".media," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category ");
			t.Append(" from  " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).SqlWithPrefix + "," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).SqlWithPrefix);
			t.Append(" " + "," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).SqlWithPrefix + " ");
			t.Append(" where ");
			t.Append(" " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_basic_media =" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_basic_media ");
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category = " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_category ");
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_category = "+idCategory);
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_language = " + _webSession.SiteLanguage);
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language = " + _webSession.SiteLanguage);
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_language = " + _webSession.SiteLanguage);
			t.Append(" and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media = " + idMedia);
			t.Append("  order by media ");

			#region Execution de la requête
			try {
				dt = _webSession.Source.Fill(t.ToString()).Tables[0];

				if (dt != null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count > 0) return true;
				else return false;
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to know if the media belong to Category  : " + t, err));
			}
			#endregion

		}
		#endregion

        #endregion

        #region Methods

        #region Get Product Data
        /// <summary>
        /// Récupère la liste produit de référence
        /// </summary>
        /// <returns>la liste produit de référence</returns>
        protected virtual string GetProductData() {
            string sql="";
            if(_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0)
                sql= _webSession.PrincipalProductUniverses[0].GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,true);

            return sql;
        }
        #endregion

        #region Get Tables
        /// <summary>
        /// Get Tables 
        /// </summary>
        /// <returns>Sql From clause</returns>
        private string GetTable() {
            string sql = "";
            sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.media) + ", ";
            sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.basicMedia) + ", ";
            sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.mediaSeller) + ", ";
            sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.category) + ", ";
            sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.interestCenter);

            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                    sql += "," + WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.periodicity);
                    return sql;
                case DBClassificationConstantes.Vehicles.names.radio:
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return sql;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return sql;
                default:
                    throw new PortofolioDALException("getTable()-->There is no table for this vehicle.");
            }
        }
        #endregion

        #region Get Alert Table
        /// <summary>
        /// Get alert table
        /// </summary>
        /// <returns>Alert table name</returns>
        private string GetAlertTable() {
            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.press:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataPressAlert).Label;
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataPressInterAlert).Label;
                case DBClassificationConstantes.Vehicles.names.radio:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataRadioAlert).Label;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataTvAlert).Label;
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataOutDoorAlert).Label;
                default:
                    throw new PortofolioDALException("GetAlertTable()-->Vehicle unknown.");
            }
        }
        #endregion

        #region Get Fields
        /// <summary>
        /// Get Fields
        /// </summary>
        /// <returns>SQL</returns>
        private string GetFields() {
            string sql = "category, media_seller, interest_center, media as support ";
            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                    sql += ", periodicity ";
                    return sql;
                case DBClassificationConstantes.Vehicles.names.radio:
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    return sql;
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return sql;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return sql;
                default:
                    throw new PortofolioDALException("GetFields()-->There are no fields for this vehicle.");
            }
        }
        #endregion

        #region Get joint
        /// <summary>
        /// Get sql joints
        /// </summary>
        /// <returns>SQL</returns>
        private string GetJoint() {
            string sql = "and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_basic_media=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_basic_media";
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_media_seller=" + WebApplicationParameters.DataBaseDescription .GetTable(TableIds.mediaSeller).Prefix + ".id_media_seller";
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_category=" + WebApplicationParameters.DataBaseDescription .GetTable(TableIds.category).Prefix + ".id_category ";
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_interest_center=" + WebApplicationParameters.DataBaseDescription .GetTable(TableIds.interestCenter).Prefix + ".id_interest_center";
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_language=" + _webSession.SiteLanguage + " ";
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix + ".id_language=" + _webSession.SiteLanguage + " ";
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.mediaSeller).Prefix + ".id_language=" + _webSession.SiteLanguage + " ";
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix + ".id_language=" + _webSession.SiteLanguage + " ";
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.interestCenter).Prefix + ".id_language=" + _webSession.SiteLanguage + " ";

            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                    sql += "and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.periodicity).Prefix + ".id_periodicity=" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix + ".id_periodicity";
                    sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.periodicity).Prefix + ".id_language=" + _webSession.SiteLanguage + " ";
                    return sql;
                case DBClassificationConstantes.Vehicles.names.radio:
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return sql;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    return sql;
                default:
                    throw new PortofolioDALException("GetJoint()-->Vehicle unknown");
            }
        }
        #endregion

        #region Get Select Data
        /// <summary>
        /// Get Select Data
        /// </summary>
        /// <returns>SQL</returns>
        private string GetSelectData() {
            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                    return " select sum(EXPENDITURE_EURO) as investment, min(DATE_MEDIA_NUM) first_date, max(DATE_MEDIA_NUM) last_date ";
                case DBClassificationConstantes.Vehicles.names.radio:
                    return " select sum(EXPENDITURE_EURO) as investment, min(DATE_MEDIA_NUM) first_date, max(DATE_MEDIA_NUM) last_date, "
                        + " sum(insertion) as insertion, sum(duration) as duration";
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    return " select sum(EXPENDITURE_EURO) as investment, min(DATE_MEDIA_NUM) first_date, max(DATE_MEDIA_NUM) last_date, "
                        + " sum(insertion) as insertion, sum(duration) as duration";
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    return " select sum(EXPENDITURE_EURO) as investment, min(DATE_CAMPAIGN_BEGINNING) first_date, max(DATE_CAMPAIGN_END) last_date, "
                        + " sum(NUMBER_BOARD) as number_board ";
                default:
                    throw new PortofolioDALException("GetSelectData()-->Vehicle unknown.");
            }
        }
        #endregion

		#region Get Table Data
		/// <summary>
		/// Get Table
		/// </summary>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Throw when the vehicle is unknown
		/// </exception>
		/// <returns>Table name</returns>
		protected virtual string GetTableData() {
			switch (_vehicleName) {
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return DBConstantes.Tables.ALERT_DATA_PRESS_INTER;
				case DBClassificationConstantes.Vehicles.names.press:
					return DBConstantes.Tables.ALERT_DATA_PRESS;
				case DBClassificationConstantes.Vehicles.names.radio:
					return DBConstantes.Tables.ALERT_DATA_RADIO;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return DBConstantes.Tables.ALERT_DATA_TV;
				case DBClassificationConstantes.Vehicles.names.outdoor:
					return DBConstantes.Tables.ALERT_DATA_OUTDOOR;
				case DBClassificationConstantes.Vehicles.names.directMarketing:
					return DBConstantes.Tables.ALERT_DATA_MARKETING_DIRECT;
				default:
					throw new PortofolioDALException("GetTableData()-->Vehicle unknown.");
			}
		}
		#endregion

        #region Get Select Data Ecran
        /// <summary>
        /// Get Select Data Ecran
        /// </summary>
        /// <returns>SQL</returns>
        protected string GetSelectDataEcran() {
            string sql = "";
            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internet:
                    return "";
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql += " select  distinct ID_COBRANDING_ADVERTISER";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " , NUMBER_spot_com_break nbre_spot";
                    sql += " , insertion ";

                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql += "select  distinct id_commercial_break ";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " ,NUMBER_MESSAGE_COMMERCIAL_BREA nbre_spot ";
                    sql += " ,insertion ";
                    return sql;
                default:
                    throw new PortofolioDALException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Vehicle unknown.");
            }
        }
        #endregion

        #region Get Select New Product
        /// <summary>
        /// Get Select New Product
        /// </summary>
        /// <returns>SQL</returns>
        protected string GetSelectNewProduct() {
            string sql = "";
            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                    sql = "select  distinct " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_product";
                    sql += " , sum(wp.expenditure_euro) as valeur ";
                    sql += " ,sum (insertion) as insertion ";
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).Prefix + ".product as produit ";
                    return sql;
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql = " select distinct " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_product ";
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).Prefix + ".product as produit ";
                    sql += " ,sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".expenditure_euro) as valeur ";
                    sql += " ,sum (insertion) as insertion ";
                    sql += " ,sum (duration) as duree ";
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql = " select distinct " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_product ";
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).Prefix + ".product as produit ";
                    sql += " ,sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".expenditure_euro) as valeur ";
                    sql += " ,sum (insertion) as insertion ";
                    sql += " ,sum(duration) as duree ";
                    return sql;
                default:
                    throw new PortofolioDALException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Vehicle unknown.");
            }

        }
        #endregion

        #region Get Table Data New Product
        /// <summary>
        /// Get Table Data New Product
        /// </summary>
        /// <returns>SQL</returns>
        private string getTableDataNewProduct() {
            string sql = "";
            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    sql += WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + DBConstantes.Tables.ALERT_DATA_PRESS_INTER + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix;
                    return sql;
                case DBClassificationConstantes.Vehicles.names.press:
                    sql += WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + DBConstantes.Tables.ALERT_DATA_PRESS + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix;
                    return sql;
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql += WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + DBConstantes.Tables.ALERT_DATA_RADIO + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix;
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql += WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + DBConstantes.Tables.ALERT_DATA_TV + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix;
                    return sql;
                default:
                    throw new PortofolioDALException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Vehicle unknown.");
            }
        }
        #endregion

        #region Get Tv Or Radio Struct Fields
        /// <summary>
        /// Get Tv Or Radio Struct Fields
        /// </summary>		
        /// <returns>SQL</returns>
        protected virtual string GetStructFields() {
            return " sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".expenditure_euro) as euros"
                + ", sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".insertion) as spot"
                + ", sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".duration) as duration";
        }
        #endregion

        #region Get Hour Interval
        /// <summary>
        /// Get Hour Interval
        /// </summary>
		/// <param name="hourBegin">Hour Begin</param>
		/// <param name="hourEnd">Hour End</param>
        /// <returns>String SQL</returns>
        protected virtual string GetHourInterval(int hourBegin, int hourEnd) {
            string sql = "";
            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion>=" + hourBegin;
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion<=" + hourEnd;
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion>=" + hourBegin;
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion<=" + hourEnd;
                    return sql;
                default:
                    throw new PortofolioDALException("GetHourInterval : Vehicle unknown.");
            }
        }
        #endregion

        #region Get Press Struct Fields
        /// <summary>
        /// Get Press Struct Fields
        /// </summary>
		/// <param name="ventilation">format or coulor or location or encarts</param>
		/// <returns>SQL</returns>
        protected virtual string GetPressStructFields(PortofolioStructure.Ventilation ventilation) {
            switch (ventilation) {
                case PortofolioStructure.Ventilation.color:
                    return " color "
                    + ", sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".insertion) as insertion";
                case PortofolioStructure.Ventilation.format:
                    return " format "
                    + ", sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".insertion) as insertion";
                case PortofolioStructure.Ventilation.insert:
                    return " inset "
                    + ", sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".insertion) as insertion";
                case PortofolioStructure.Ventilation.location:
                    return " location "
                    + ", sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".insertion) as insertion";
                default:
                    throw new PortofolioDALException("getPressStructFields(PortofolioStructure.Ventilation ventilation)--> No ventilation (format, couleur) corresponding.");
            }
        }
        #endregion

        #region Get Press Struct Tables
        /// <summary>
        /// Get Press Struct Tables
        /// </summary>
		/// <param name="ventilation">format or coulor or location or encarts</param>
		/// <param name="tableName">Tables name</param>
        /// <returns>tables name string</returns>
         protected virtual string GetPressStructTables(string tableName, PortofolioStructure.Ventilation ventilation) {
            switch (ventilation) {
                case PortofolioStructure.Ventilation.color:
                    return  tableName 
                    + ", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix;
                case PortofolioStructure.Ventilation.format:
                    return  tableName 
                    + " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix;
                case PortofolioStructure.Ventilation.insert:
                    return  tableName 
                    + " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.inset).SqlWithPrefix;
                case PortofolioStructure.Ventilation.location:
					return  tableName 
					+ " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.location).SqlWithPrefix
					 + " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.dataLocation).SqlWithPrefix;
                default:
                    throw new PortofolioDALException("getPressStructTables(PortofolioStructure.Ventilation ventilation)--> Impossible to determnie ventilation type for press vehicle.");
            }
        }
        #endregion

        #region Get Press Struct Language
        /// <summary>
        /// Get Press Struct Language
        /// </summary>
		/// <param name="ventilation">format or coulor or location or encarts</param>
		/// <returns>SQL</returns>
        protected virtual string GetPressStructLanguage(PortofolioStructure.Ventilation ventilation) {
            switch (ventilation) {
                case PortofolioStructure.Ventilation.color:
                    return " " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).Prefix + ".id_language = " + _webSession.SiteLanguage
                        + " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
                case PortofolioStructure.Ventilation.format:
                    return " " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).Prefix + ".id_language=" + _webSession.SiteLanguage
                        + " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
                case PortofolioStructure.Ventilation.insert:
                    return " " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.inset).Prefix + ".id_language=" + _webSession.SiteLanguage
                        + " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.inset).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
                case PortofolioStructure.Ventilation.location:
                    return " " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.location).Prefix + ".id_language=" + _webSession.SiteLanguage
                        + " and dl.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + ""
                        + " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.location).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
                default:
                    throw new PortofolioDALException("getPressStructLanguage(PortofolioStructure.Ventilation ventilation)--> Impossible de déterminer le type de language pour la presse.");
            }
        }
        #endregion

        #region Get Press Struct Joint
        /// <summary>
        /// Get Press Struct Joint
        /// </summary>
		/// <param name="ventilation">format or coulor or location or encarts</param>
		/// <returns>champq joints</returns>
        protected virtual string GetPressStructJoint(PortofolioStructure.Ventilation ventilation) {
            switch (ventilation) {
                case PortofolioStructure.Ventilation.color:
                    return " and  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_color = " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).Prefix + ".id_color ";
                case PortofolioStructure.Ventilation.format:
                    return " and  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_format =" + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).Prefix + ".id_format";
                case PortofolioStructure.Ventilation.insert:
                    return " and  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_inset = " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.inset).Prefix + ".id_inset"
                     + " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_inset in ( " + WebConstantes.CustomerSessions.InsertType.encart.GetHashCode() + "," + WebConstantes.CustomerSessions.InsertType.flyingEncart.GetHashCode() + " )"
                    + " and srt.id_inset in ( " + WebConstantes.CustomerSessions.InsertType.encart.GetHashCode() + "," + WebConstantes.CustomerSessions.InsertType.flyingEncart.GetHashCode() + " )";
                case PortofolioStructure.Ventilation.location:
                    return " and  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media = dl.id_media "
                        + " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.location).Prefix + ".id_location=dl.id_location "
                        //Period
                    + " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num=dl.date_media_num "
                    + "  and dl.ID_ADVERTISEMENT=" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".ID_ADVERTISEMENT ";
                default:
                    throw new PortofolioDALException("getPressStructJoint(PortofolioStructure.Ventilation ventilation) : Impossible to realise joint sql for Press vehicle.");
            }
        }
        #endregion

        #region Get Press Struct Group By
        /// <summary>
        ///	Get Press Struct Group By
        /// </summary>
		/// <param name="ventilation">format or coulor or location or encarts</param>
		/// <returns>Sql string</returns>
        protected virtual string GetPressStructGroupBy(PortofolioStructure.Ventilation ventilation) {
            switch (ventilation) {
                case PortofolioStructure.Ventilation.color:
                    return " group by color ";
                case PortofolioStructure.Ventilation.format:
                    return " group by format ";
                case PortofolioStructure.Ventilation.insert:
                    return " group by inset ";
                case PortofolioStructure.Ventilation.location:
                    return " group by location ";
                default:
                    throw new PortofolioDALException("getPressStructGroupBy(PortofolioStructure.Ventilation ventilation) : No ventilation (format, couleur) defined.");
            }
        }
        #endregion

        #region Get Fields Detail Media For Tv & Radio
        /// <summary>
        /// Get Fields Detail Media For Tv & Radio
        /// </summary>
        /// <returns>SQL</returns>
        protected virtual string GetFieldsDetailMediaForTvRadio() {
            string sql = "";
            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql += " sum(insertion) as insertion ";
                    sql += ",commercial_break as code_ecran";
                    sql += ",sum(expenditure_euro) value ";
                    sql += " , date_media_num  ";
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql += " sum(insertion) as insertion ";
                    sql += ",id_commercial_break as code_ecran";
                    sql += ",sum(expenditure_euro) value ";
                    sql += ",date_media_num  ";
                    return sql;
                default:
                    throw new PortofolioDALException("GetFieldsDetailMediaForTvRadio()-->Vehicle unknown.");
            }
        }
        #endregion

        #region Get Group By Detail Media For Tv & Radio
        /// <summary>
        /// Get Group By Detail Media For Tv & Radio
        /// </summary>
        /// <returns>SQL</returns>
        public virtual string GetGroupByDetailMediaForTvRadio() {
            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.radio:
                    return "group by date_media_num ,commercial_break order by commercial_break";
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    return "group by date_media_num ,id_commercial_break order by id_commercial_break";
                default:
                    throw new PortofolioDALException("GetGroupByDetailMediaForTvRadio()-->Vehicle unknown.");
            }
        }
        #endregion

		#region GetOrderByDetailMedia
		/// <summary>
		/// Get order for media detail insertion
		/// </summary>
		/// <param name="allPeriod">True if is for all period</param>
		/// <returns>String SQL</returns>
		protected virtual  string GetOrderByDetailMedia(bool allPeriod) {
			
			switch (_vehicleName) {
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					if (allPeriod) return " order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".Id_type_page,media_paging,id_product," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_advertisement";
					else
						return " order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".Id_type_page,media_paging, id_product," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_advertisement";
				case DBClassificationConstantes.Vehicles.names.radio:
					if (allPeriod) return "order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion";
					else return "order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion";
				case DBClassificationConstantes.Vehicles.names.tv:
					// Top diffusion
					if (allPeriod)
						return "order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion ";
					else return "order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion ";
				case DBClassificationConstantes.Vehicles.names.others:
					// order by date, scrreen code 
					if (allPeriod)
						return "order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_commercial_break ";
					else return "order by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_commercial_break";
				default:
					throw new PortofolioDALException("GetOrderByDetailMedia : This media is not treated. None related table.");
			}
		}
		#endregion

		#region GetRequest
		/// <summary>
		/// Build detail media portofolio sql query
		/// </summary>		
		/// <param name="type">Table Type  </param>
		/// <returns>Sql query</returns>
		protected virtual string GetRequest(DBConstantes.TableType.Type type) {
			#region Variables
			string dateField = "";
			string dataTableName = "";
			string dataTableNameForGad = "";
			string dataFieldsForGad = "";
			string dataJointForGad = "";
			string dataJointForInsert = "";
			string detailProductTablesNames = "";
			string detailProductFields = "";
			string detailProductJoints = "";
			string detailProductOrderBy = "";
			string unitFields = "";
			string productsRights = "";
			string sql = "";
			string list = "";
			string mediaList = "";
			bool premier;
			string mediaRights = "";
			string listProductHap = "";
			string mediaAgencyTable = string.Empty;
			string mediaAgencyJoins = string.Empty;
			CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
			#endregion

			#region Construction de la requête
			try {				
				//Data table			
				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
						dataTableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName, WebConstantes.Module.Type.alert);
						break;
					case DBConstantes.TableType.Type.dataVehicle:
						dataTableName = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleName, WebConstantes.Module.Type.analysis);
						break;
					case DBConstantes.TableType.Type.webPlan:
						dataTableName = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData).SqlWithPrefix; //DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
						break;
					default:
						throw (new CompetitorDataAccessException("Table type unknown"));
				}
				detailProductTablesNames = _webSession.GenericProductDetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
				detailProductFields = _webSession.GenericProductDetailLevel.GetSqlFields();
				detailProductJoints = _webSession.GenericProductDetailLevel.GetSqlJoins(_webSession.SiteLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
				unitFields = WebFunctions.SQLGenerator.getUnitFieldsNameForMediaDetailResult(_vehicleName, type, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
				detailProductOrderBy = _webSession.GenericProductDetailLevel.GetSqlOrderFields();								

				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
					case DBConstantes.TableType.Type.dataVehicle:
						dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
						break;
					case DBConstantes.TableType.Type.webPlan:
						dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
						break;
				}
				productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
				listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
				//option inset (for veicle press)
				if (DBClassificationConstantes.Vehicles.names.press == _vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleName)
					dataJointForInsert = WebFunctions.SQLGenerator.GetJointForInsertDetail(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, type);
				if (_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)) {
					try {
						dataTableNameForGad = ", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad).SqlWithPrefix;//+ DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.GAD + " " + DBConstantes.Tables.GAD_PREFIXE;
						dataFieldsForGad = ", " + WebFunctions.SQLGenerator.GetFieldsAddressForGad();
						dataJointForGad = "and " + WebFunctions.SQLGenerator.GetJointForGad(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
					}
					catch (PortofolioDALException) { ;}
				}				
			}
			catch (System.Exception err) {
				throw (new PortofolioAnalysisDataAccessException("Impossible d'initialiser les paramètres de la requêtes", err));
			}
			if (WebFunctions.CheckedText.IsStringEmpty(dataTableName.ToString().Trim())) {
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql += " select " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + ", " + dateField + " as date_num, " + unitFields;
				else
					sql += " select " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + "," + unitFields;
				sql += " from " + mediaAgencyTable +  dataTableName;
				if (detailProductTablesNames.Length > 0)
					sql += ", " + detailProductTablesNames;
				sql += " " + dataTableNameForGad;
				// Period
				switch (type) {
					case DBConstantes.TableType.Type.dataVehicle4M:
						sql += " where " + dateField + " >=" + customerPeriod.StartDate;
						sql += " and " + dateField + " <=" + customerPeriod.EndDate;
						break;
					case DBConstantes.TableType.Type.dataVehicle:
						if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
							sql += " where " + dateField + " >=" + customerPeriod.StartDate;
							sql += " and " + dateField + " <=" + customerPeriod.EndDate;
						}
						else if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
							sql += " where ((" + dateField + " >=" + customerPeriod.PeriodDayBegin[0];
							sql += " and " + dateField + " <=" + customerPeriod.PeriodDayEnd[0];
							sql += " ) or (" + dateField + " >=" + customerPeriod.PeriodDayBegin[1];
							sql += " and " + dateField + " <=" + customerPeriod.PeriodDayEnd[1];
							sql += "))";
						}
						else {
							sql += " where " + dateField + " >=" + customerPeriod.PeriodDayBegin[0];
							sql += " and " + dateField + " <=" + customerPeriod.PeriodDayEnd[0];
						}
						break;
					case DBConstantes.TableType.Type.webPlan:
						sql += " where " + dateField + " >=" + customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6);
						sql += " and " + dateField + " <=" + customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6);
						break;
				}
				// Joints Products
				sql += " " + detailProductJoints;
				sql += " " + dataJointForGad;
				sql += " " + mediaAgencyJoins;
				//Joints inset
				if (DBClassificationConstantes.Vehicles.names.press == _vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleName)
					sql += " " + dataJointForInsert;

				#region Media selection
				mediaList += _webSession.GetSelection((TreeNode)_webSession.ReferenceUniversMedia, CustormerConstantes.Right.type.mediaAccess);
				if (mediaList.Length > 0) sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media in (" + mediaList + ")";
				#endregion

				#region Products Selection 
				sql += GetProductData();
				#endregion				

				// Rights products
				sql += " " + productsRights;
				sql += listProductHap;
				// Rights media
				sql += mediaRights;

				// Group by
				if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
					sql += " group by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + ", " + dateField;
				else
					sql += " group by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad;
			}
			#endregion

			#region Execute query
			try {
				return sql.ToString();
			}
			catch (System.Exception err) {
				throw (new PortofolioDALException("Impossible to build detail media query : " + sql, err));
			}
			#endregion

		}
		#endregion

		#endregion
	}
}
