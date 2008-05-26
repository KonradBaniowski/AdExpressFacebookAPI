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
using TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpress.Portofolio.DAL {
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
                            sql = " select count(id_product) as total ";
                            sql = " from( ";
                            sql = " select  id_product ";
                        }
                        else {
                            sql = " select count(id_advertiser) as total ";
                            sql = " from( ";
                            sql = " select  id_advertiser ";
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
        public object[] NumberPageEncart() {

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

            #region Construction de la requête
            try {
                tableName = GetTableData();
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                product = GetProductData();
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

                for (int i = 0; i <= 2; i++) {

                    sql = " select sum(area_page) as page ";
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

        #region Media portofolio
        /// <summary>
        /// Get Data for the Media portofolio
        /// </summary>
        /// <returns>Data Set</returns>
        public DataSet GetMediaPortofolio() {

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
                //Agence_media
                if(_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency)||_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency)) {
                    mediaAgencyTable=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql+"."+_webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+",";
                    mediaAgencyJoins="And "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_product="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product ";
                    mediaAgencyJoins+="And "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_vehicle="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle ";
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
                //Agence_media
                if(_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency)||_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency)) {
                    mediaAgencyTable=WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql+"."+_webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+",";
                    mediaAgencyJoins="And "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_product="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product ";
                    mediaAgencyJoins+="And "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_vehicle="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle ";
                }
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

        #region Novelty
        /// <summary>
        /// Dataset for the new products
        /// </summary>
        /// <returns>List of new products</returns>
        public DataSet GetNewProduct() {

            #region Variables
            string sql = "";
            string tableName = "";
            string selectNewProduct = "";
            string productsRights = "";
            string product = "";
            string mediaRights = "";
            string listProductHap = "";
            #endregion

            try {
                selectNewProduct = GetSelectNewProduct();
                tableName = getTableDataNewProduct();

                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                product = GetProductData();
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request for GetNewProduct() ", err));
            }

            #region Construction de la requête
            sql = selectNewProduct;
            sql += " from " + tableName;
            sql += " where id_media=" + _idMedia + "";

            //Jointure
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).Prefix + ".id_product=" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_product ";

            // activation
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";

            // langue
            sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).Prefix + ".id_language=" + _webSession.SiteLanguage + " ";

            if (_beginingDate.Length > 0)
                sql += " and  DATE_MEDIA_NUM>= " + _beginingDate + " ";
            if (_endDate.Length > 0)
                sql += " and  DATE_MEDIA_NUM<= " + _endDate + " ";

            if (_vehicleName == DBClassificationConstantes.Vehicles.names.press
                || _vehicleName == DBClassificationConstantes.Vehicles.names.internationalPress) {
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).Prefix + ".id_color=" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+ ".id_color ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).Prefix + ".id_language=" + _webSession.SiteLanguage + " ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";

                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).Prefix + ".id_format=" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+ ".id_format ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).Prefix + ".id_language=" + _webSession.SiteLanguage + " ";
                sql += " and " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + "";
            }

            sql += " and wp.id_product in( ";

            sql += " select distinct id_product ";
            sql += " from   " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + "." + GetTableData() + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            if (_webSession.NewProduct == TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.support) {
                // Cas nouveau dans le support	
                sql += " where new_product in (1,2) ";
            }
            // cas dans la pige
            else {
                sql += " where new_product=2 ";
            }

            sql += " and id_media=" + _idMedia + " ";
            sql += " and  DATE_MEDIA_NUM>= " + _beginingDate + " ";
            sql += " and  DATE_MEDIA_NUM<= " + _endDate + " ";
            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += listProductHap;

            sql += " ) ";

            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += listProductHap;

            sql += " group by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_product," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).Prefix + ".product ";

            sql += " order by product ";
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

        #region Structure

        #region Dataset for tv or radio
        /// <summary>
        /// Get structure data for tv or radio
        /// </summary>
        /// <param name="HourBegin">Hour Begin</param>
        /// <param name="HourEnd">Hour End</param>
        /// <returns>DataSet</returns>
        public DataSet GetTvOrRadioStructData(int HourBegin, int HourEnd) {

            #region variables
            string tableName = "";
            string fields = "";
            string idVehicle = "";
            string idMedia = "";
            string sql = "";
            string product = "";
            #endregion

            #region construction de la requête
            try {
                //Table name
                tableName = GetTableData();
                //Fields
                fields = GetTvOrRadioStructFields();
            }
            catch (Exception) {
                throw new PortofolioDALException("GetTvOrRadioStructData(PortofolioStructure.Ventilation ventilation)--> Table unknown.");
            }

            if (WebFunctions.CheckedText.IsStringEmpty(tableName.ToString().ToString())) {
                // Sélection de la nomenclature Support
                sql += " select " + fields;
                // Tables
                sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
                sql += " where ";
                // Période
                sql += "  wp.date_media_num>=" + _beginingDate;
                sql += " and wp.date_media_num<=" + _endDate;
                // Tranche horaire
                sql += GetHourInterval(HourBegin, HourEnd);

                #region Nomenclature Produit (droits)
                //premier=true;
                //Droits en accès
                sql += WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                //liste des produit hap
                string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
                if (WebFunctions.CheckedText.IsStringEmpty(listProductHap.ToString().Trim()))
                    sql += listProductHap;
                //Liste des produits sélectionnés
                product = GetProductData();
                if (WebFunctions.CheckedText.IsStringEmpty(product.ToString().Trim()))
                    sql += product;
                #endregion

                #region Nomenclature Media (droits et sélection)

                #region Droits
                sql += WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                #endregion

                #region Sélection média
                //sélection média (vehicle)
                if (WebFunctions.CheckedText.IsStringEmpty(idVehicle.ToString().Trim()))
                    sql += " and "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_vehicle=" + idVehicle.ToString();
                //sélection support	
                if (WebFunctions.CheckedText.IsStringEmpty(idMedia.ToString().Trim()))
                    sql += " and "+WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix+".id_media=" + idMedia.ToString();
                #endregion

                #endregion

            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetTvOrRadioStructData(int HourBegin, int HourEnd) : " + sql, err));
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
            string sql = "";
            string product = "";
            #endregion

            #region construction de la requête
            try {
                //Table name
                tableName = GetTableData();
                //Fields
                fields = GetPressStructFields(ventilation);
            }
            catch (Exception) {
                throw new PortofolioDALException("getPressStructFields(PortofolioStructure.Ventilation ventilation)--> Table unknown.");
            }

            if (WebFunctions.CheckedText.IsStringEmpty(tableName.ToString().Trim())) {
                // Sélection de la nomenclature Support
                sql += " select " + fields;
                // Tables
                sql += " from ";
                sql += GetPressStructTables(tableName, ventilation);
                sql += " where ";
                //Choix de la langue
                sql += GetPressStructLanguage(ventilation);
                //Jointures
                sql += GetPressStructJoint(ventilation);
                // Période
                sql += " and wp.date_media_num>=" + _beginingDate;
                sql += " and wp.date_media_num<=" + _endDate;

                #region Nomenclature Produit (droits)
                //premier=true;
                //Droits en accès
                sql += WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                //liste des produit hap
                string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
                if (WebFunctions.CheckedText.IsStringEmpty(listProductHap.ToString().Trim()))
                    sql += listProductHap;
                //Liste des produits sélectionnés
                product = GetProductData();
                if (WebFunctions.CheckedText.IsStringEmpty(product.ToString().Trim()))
                    sql += product;
                #endregion

                #region Sélection média

                #region Droits média
                sql += WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                #endregion

                //sélection média (vehicle)
                if (WebFunctions.CheckedText.IsStringEmpty(idVehicle.ToString().Trim()))
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_vehicle=" + idVehicle.ToString();
                //sélection support	
                if (WebFunctions.CheckedText.IsStringEmpty(idMedia.ToString().Trim()))
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media=" + idMedia.ToString();
                #endregion

                #region regroupement
                sql += GetPressStructGroupBy(ventilation);
                #endregion

                //tri
                sql += " order by insertion desc";
            }
            #endregion

            #region Execution de la requête
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetPressStructData : " + sql, err));
            }
            #endregion

        }
        #endregion

        #endregion

        #endregion

        #region Methods

        #region Get Product Data
        /// <summary>
        /// Récupère la liste produit de référence
        /// </summary>
        /// <returns>la liste produit de référence</returns>
        private string GetProductData() {
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
            sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.media);
            sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.basicMedia);
            sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.mediaSeller);
            sql += WebApplicationParameters.DataBaseDescription.GetSqlTableLabelWithPrefix(TableIds.category);
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
        private string GetTableData() {
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
        private string GetTvOrRadioStructFields() {
            return " sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".expenditure_euro) as euros"
                + ", sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".insertion) as spot"
                + ", sum(" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".duration) as duration";
        }
        #endregion

        #region Get Hour Interval
        /// <summary>
        /// Get Hour Interval
        /// </summary>
        /// <param name="HourBegin">Hour Begin</param>
        /// <param name="HourEnd">Hour End</param>
        /// <returns>SQL</returns>
        private string GetHourInterval(int HourBegin, int HourEnd) {
            string sql = "";
            switch (_vehicleName) {
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion>=" + HourBegin;
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_top_diffusion<=" + HourEnd;
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion>=" + HourBegin;
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion<=" + HourEnd;
                    return sql;
                default:
                    throw new PortofolioDALException("getHourInterval(int HourBegin,int HourEnd,DBClassificationConstantes.Vehicles.names idVehicle)--> Vehicle unknown.");
            }
        }
        #endregion

        #region Get Press Struct Fields
        /// <summary>
        /// Get Press Struct Fields
        /// </summary>
        /// <param name="ventilation">une ventilation par format, type de couleur,encarts</param>		
        /// <returns>SQL</returns>
        private string GetPressStructFields(PortofolioStructure.Ventilation ventilation) {
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
                    throw new PortofolioDALException("getPressStructFields(PortofolioStructure.Ventilation ventilation)--> Pas de ventilation (format, couleur) déterminé.");
            }
        }
        #endregion

        #region Get Press Struct Tables
        /// <summary>
        /// Get Press Struct Tables
        /// </summary>
        /// <param name="ventilation">format ou couleur ou emplacements ou encarts</param>
        /// <param name="tableName">nom de la table média sélectionné</param>
        /// <returns>tables</returns>
        private string GetPressStructTables(string tableName, PortofolioStructure.Ventilation ventilation) {
            switch (ventilation) {
                case PortofolioStructure.Ventilation.color:
                    return " " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03) + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix
                    + ", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix;
                case PortofolioStructure.Ventilation.format:
                    return " " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03) + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix
                    + " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix;
                case PortofolioStructure.Ventilation.insert:
                    return " " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03) + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix
                    + " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.inset).SqlWithPrefix;
                case PortofolioStructure.Ventilation.location:
                    return " " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03) + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix
                    + " , " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.location).SqlWithPrefix
                    + " , " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_LOCATION + "  dl ";
                default:
                    throw new PortofolioDALException("getPressStructTables(PortofolioStructure.Ventilation ventilation)--> Impossible de déterminer le type de ventilation pour le média presse.");
            }
        }
        #endregion

        #region Get Press Struct Language
        /// <summary>
        /// Get Press Struct Language
        /// </summary>
        /// <param name="ventilation">format ou couleur ou emplacements ou encarts</param>
        /// <returns>SQL</returns>
        private string GetPressStructLanguage(PortofolioStructure.Ventilation ventilation) {
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
        /// <param name="ventilation">format ou couleur ou emplacements ou encarts</param>
        /// <returns>champq joints</returns>
        private string GetPressStructJoint(PortofolioStructure.Ventilation ventilation) {
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
                        // Période
                    + " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num=dl.date_media_num "
                    + "  and dl.ID_ADVERTISEMENT=" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".ID_ADVERTISEMENT ";
                default:
                    throw new PortofolioDALException("getPressStructJoint(PortofolioStructure.Ventilation ventilation)--> Impossible d'effectuer des jointures pour la presse.");
            }
        }
        #endregion

        #region Get Press Struct Group By
        /// <summary>
        ///	Get Press Struct Group By
        /// </summary>
        /// <param name="ventilation">une ventilation par format, type de couleur,encarts</param>		
        /// <returns>Chaine contenant les champs sélectionnés groupées</returns>
        private string GetPressStructGroupBy(PortofolioStructure.Ventilation ventilation) {
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
                    throw new PortofolioDALException("getPressStructGroupBy(PortofolioStructure.Ventilation ventilation)--> Pas de ventilation (format, couleur) déterminé.");
            }
        }
        #endregion

        #endregion
    }
}
