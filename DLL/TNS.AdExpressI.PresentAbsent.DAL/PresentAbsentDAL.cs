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

#endregion

namespace TNS.AdExpressI.PresentAbsent.DAL{


	/// <summary>
	/// Extract data for Present / Absent Report
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
        protected CstDBClassif.Vehicles.names _vehicle;
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
        /// Get Vehicle
        /// </summary>
        protected CstDBClassif.Vehicles.names Vehicle
        {
            get { return _vehicle; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session"></param>
        public PresentAbsentDAL(WebSession session)
        {
            this._session = session;

            #region Sélection du vehicle
            string vehicleSelection = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new PresentAbsentDALException("Selection of media is not correct"));
            _vehicle = (CstDBClassif.Vehicles.names)int.Parse(vehicleSelection);
            #endregion

        }
        #endregion

        #region Get Synthesis Data

        #region GetSynthesisRequest
        /// <summary>
        /// Build request to load data for synthesis report about ONE vehicle
        /// </summary>
        /// <param name="type">Data table type to use</param>
        /// <returns>SQL Query</returns>
        protected string GetSynthesisRequest(CstDB.TableType.Type type) {

            #region Constantes
            //préfixe table à utiliser
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables
            string dataTableName = "";
            string listExcludeProduct = "";
            string unitField = "";
            string productsRights = "";
            string mediaRights = "";
            string list = "";
            int positionUnivers = 1;
            string mediaList = "";
            bool premier;
            string dataJointForInsert = "";
            StringBuilder sql = new StringBuilder(3000);
            string dateField = "";
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
            #endregion

            #region Tables
            Table tblWebPlan = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData);
            Schema schAdExpress = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);

			Table tblAdvertisingAgengy = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency);
			Table tblGroupAdvertisingAgengy = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.groupAdvertisingAgency);
            #endregion

            #region Construction de la requête
            try {
                // Table de données
                switch (type) {
                    case CstDB.TableType.Type.dataVehicle4M:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicle, CstWeb.Module.Type.alert);
                        break;
                    case CstDB.TableType.Type.dataVehicle:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicle, CstWeb.Module.Type.analysis);
                        break;
                    case CstDB.TableType.Type.webPlan:
                        dataTableName = tblWebPlan.Label;
                        break;
                    default:
                        throw (new PresentAbsentDALException("Unable to determine type of table to use."));
                }
                // Unités
                unitField = FctWeb.SQLGenerator.GetUnitFieldName(_session, type);
                productsRights = FctWeb.SQLGenerator.getAnalyseCustomerProductRight(_session, DATA_TABLE_PREFIXE, true);
                mediaRights = FctWeb.SQLGenerator.getAnalyseCustomerMediaRight(_session, DATA_TABLE_PREFIXE, true);
                listExcludeProduct = FctWeb.SQLGenerator.GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DATA_TABLE_PREFIXE, true, false);
                // Dates
                switch (type) {
                    case CstDB.TableType.Type.dataVehicle4M:
                    case CstDB.TableType.Type.dataVehicle:
                        dateField = DATA_TABLE_PREFIXE + "." + CstDB.Fields.DATE_MEDIA_NUM;
                        break;
                    case CstDB.TableType.Type.webPlan:
                        dateField = DATA_TABLE_PREFIXE + "." + CstDB.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                        break;
                }
                //option encarts (pour la presse)
                if (CstDBClassif.Vehicles.names.press == _vehicle || CstDBClassif.Vehicles.names.internationalPress == _vehicle)
                    dataJointForInsert = FctWeb.SQLGenerator.GetJointForInsertDetail(_session, DATA_TABLE_PREFIXE);
            }
            catch (Exception e) {
                throw (new PresentAbsentDALException("Unable to init request parameters.", e));
            }
            //Select
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                sql.AppendFormat("  select {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
                sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand, {0}.id_product", DATA_TABLE_PREFIXE);
				sql.AppendFormat(",{0}.ID_GROUP_ADVERTISING_AGENCY,{0}.ID_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
                sql.AppendFormat(",{0}.id_media, {1} as date_num, sum({2}) as unit", DATA_TABLE_PREFIXE, dateField, unitField);
            }
            else {
                sql.AppendFormat("  select {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
                sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand, {0}.id_product", DATA_TABLE_PREFIXE);
				sql.AppendFormat(",{0}.ID_GROUP_ADVERTISING_AGENCY,{0}.ID_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
                sql.AppendFormat(",{0}.id_media, sum({1}) as unit", DATA_TABLE_PREFIXE, unitField);
            }
            //From
            sql.AppendFormat(" from {0}", dataTableName);
			sql.AppendFormat(",{0}.{1} {2}", schAdExpress.Label, tblAdvertisingAgengy.Label, tblAdvertisingAgengy.Prefix);
			sql.AppendFormat(",{0}.{1} {2}", schAdExpress.Label, tblGroupAdvertisingAgengy.Label, tblGroupAdvertisingAgengy.Prefix);
            //Where

            // Période
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
                        sql.AppendFormat(" and {0} <= {1})", dateField, customerPeriod.PeriodDayEnd[0]);
                        sql.AppendFormat(" or ({0} >= {1}", dateField, customerPeriod.PeriodDayBegin[1]);
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

            //Jointures encart
            if (CstDBClassif.Vehicles.names.press == _vehicle || CstDBClassif.Vehicles.names.internationalPress == _vehicle)
                sql.AppendFormat(" {0}", dataJointForInsert);

            //Jointures groupe agences/agences		
			sql.AppendFormat(" and {0}.ID_ADVERTISING_AGENCY(+)={1}.ID_ADVERTISING_AGENCY ", tblAdvertisingAgengy.Prefix, DATA_TABLE_PREFIXE);
			sql.AppendFormat(" and {0}.id_language(+)={1}", tblAdvertisingAgengy.Prefix, _session.SiteLanguage);
			sql.AppendFormat(" and {0}.ID_GROUP_ADVERTISING_AGENCY(+)={1}.ID_GROUP_ADVERTISING_AGENCY ", tblGroupAdvertisingAgengy.Prefix, DATA_TABLE_PREFIXE);
			sql.AppendFormat(" and {0}.id_language(+)={1}", tblGroupAdvertisingAgengy.Prefix, _session.SiteLanguage);

            #region Sélection de Médias
            while (_session.CompetitorUniversMedia[positionUnivers] != null) {
                mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
                positionUnivers++;
            }
            if (mediaList.Length > 0) sql.AppendFormat(" and {0}.id_media in ({1}) ", DATA_TABLE_PREFIXE, mediaList.Substring(0, mediaList.Length - 1));
            #endregion

            #region Sélection de Produits
            // Sélection en accès
            premier = true;
            // Sector
            list = _session.GetSelection(_session.CurrentUniversProduct, CstCustomer.Right.type.sectorAccess);
            if (list.Length > 0) {
                sql.AppendFormat(" and (({0}.id_sector in ({1}) ", DATA_TABLE_PREFIXE, list);
                premier = false;
            }
            // SubSector
            list = _session.GetSelection(_session.CurrentUniversProduct, CstCustomer.Right.type.subSectorAccess);
            if (list.Length > 0) {
                if (!premier) sql.Append(" or");
                else sql.Append(" and ((");
                sql.AppendFormat(" {0}.id_subsector in ({1}) ",DATA_TABLE_PREFIXE, list);
                premier = false;
            }
            // Group
            list = _session.GetSelection(_session.CurrentUniversProduct, CstCustomer.Right.type.groupAccess);
            if (list.Length > 0) {
                if (!premier) sql.Append(" or");
                else sql.Append(" and ((");
                sql.AppendFormat(" {0}.id_group_ in ({1}) ", DATA_TABLE_PREFIXE, list);
                premier = false;
            }

            if (!premier) sql.Append(" )");

            // Sélection en Exception
            // Sector
            list = _session.GetSelection(_session.CurrentUniversProduct, CstCustomer.Right.type.sectorException);
            if (list.Length > 0) {
                if (premier) sql.Append(" and (");
                else sql.Append(" and");
                sql.AppendFormat(" {0}.id_sector not in ({1}) ", DATA_TABLE_PREFIXE, list);
                premier = false;
            }
            // SubSector
            list = _session.GetSelection(_session.CurrentUniversProduct, CstCustomer.Right.type.subSectorException);
            if (list.Length > 0) {
                if (premier) sql.Append(" and (");
                else sql.Append(" and");
                sql.AppendFormat(" {0}.id_subsector not in ({1}) ", DATA_TABLE_PREFIXE, list);
                premier = false;
            }
            // Group
            list = _session.GetSelection(_session.CurrentUniversProduct, CstCustomer.Right.type.groupException);
            if (list.Length > 0) {
                if (premier) sql.Append(" and (");
                else sql.Append(" and");
                sql.AppendFormat(" {0}.id_group_ not in ({1}) ", DATA_TABLE_PREFIXE, list);
                premier = false;
            }
            if (!premier) sql.Append(" )");
            #endregion

            // Produit exclus
            sql.Append(listExcludeProduct);

            // Droits des Médias
            sql.Append(mediaRights);
            // Droits des Produits
            sql.AppendFormat(" {0}", productsRights);
            // Group by		
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                sql.AppendFormat("  group by {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
                sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand, {0}.id_product", DATA_TABLE_PREFIXE);
				sql.AppendFormat(",{0}.ID_GROUP_ADVERTISING_AGENCY,{0}.ID_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
                sql.AppendFormat(",{0}.id_media, {1} ", DATA_TABLE_PREFIXE, dateField);
            }
            else {
                sql.AppendFormat("  group by {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
                sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand, {0}.id_product", DATA_TABLE_PREFIXE);
				sql.AppendFormat(",{0}.ID_GROUP_ADVERTISING_AGENCY,{0}.ID_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
                sql.AppendFormat(",{0}.id_media ", DATA_TABLE_PREFIXE);
            }
            #endregion

            #region Execution de la requête
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
		/// Load synthesis data for ONE vehicle
		/// </summary>
		/// <returns>Competitor data</returns>
        public DataTable GetSynthesisData() {

            #region Constantes
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables
            StringBuilder sql = new StringBuilder();
            StringBuilder sql4M = new StringBuilder();
            StringBuilder sqlDataVehicle = new StringBuilder();
            StringBuilder sqlWebPlan = new StringBuilder();
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
            #endregion

            #region Construction de la requête
            try {

                if (customerPeriod.Is4M) {
                    sql4M.Append(GetSynthesisRequest(CstDB.TableType.Type.dataVehicle4M));
                    sql4M.Append("  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product");
                    sql4M.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, id_media");
                    sql = sql4M;
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
                    sql.Append(GetSynthesisRequest(CstDB.TableType.Type.dataVehicle));
                    sql.Append("  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product");
                    sql.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, id_media");
                }
                else {

                    if (customerPeriod.IsDataVehicle) {
                        sqlDataVehicle.Append(GetSynthesisRequest(CstDB.TableType.Type.dataVehicle));
                        sql = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan) {
                        sqlWebPlan.Append(GetSynthesisRequest(CstDB.TableType.Type.webPlan));
                        sql = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                        sql = new StringBuilder();

                        sql.Append("  select id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product");
                        sql.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, id_media, sum(unit) as unit");
                        sql.Append(" from (");
                        sql.Append(sqlDataVehicle);
                        sql.Append(" UNION ");
                        sql.Append(sqlWebPlan);
                        sql.Append(" ) ");
                        sql.Append("  group by id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product");
                        sql.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, id_media");

                    }

                    sql.Append("  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product");
                    sql.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, id_media");
                }
            }
            catch (System.Exception err) {
                throw (new PresentAbsentDALException("Unable to build request to get synthesis data : " + sql, err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _session.Source.Fill(sql.ToString()).Tables[0];
            }
            catch (System.Exception err) {
                throw (new PresentAbsentDALException("Unable to load synthesis data : " + sql, err));
            }

            #endregion
        
        }
        #endregion

        #endregion

        #region Get Data

        #region GetRequest
        /// <summary>
        /// Build request for competitor report
        /// </summary>
        /// <param name="type">Data table type</param>
        /// <returns>SQL Query</returns>
        public string GetRequest(CstDB.TableType.Type type) {

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
            string unitField = "";
            string productsRights = "";
            string mediaRights = "";
            string listExcludeProduct = "";
            string list = "";
            int positionUnivers = 1;
            string mediaList = "";
            bool premier;
            string dataJointForInsert = "";

            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;

            //added for layer split
            Schema schAdEx = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
            Table tblGad = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad);
            StringBuilder sql = new StringBuilder();
            string columnDetailLevel = string.Empty;
            #endregion

            #region Construction de la requête
            try {
                // Table de données, date
                switch (type) {
                    case CstDB.TableType.Type.dataVehicle4M:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicle, CstWeb.Module.Type.alert);
                        dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + CstDB.Fields.DATE_MEDIA_NUM;
                        break;
                    case CstDB.TableType.Type.dataVehicle:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicle, CstWeb.Module.Type.analysis);
                        dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + CstDB.Fields.DATE_MEDIA_NUM;
                        break;
                    case CstDB.TableType.Type.webPlan:
                        dataTableName = CstDB.Tables.WEB_PLAN_MEDIA_MONTH;
                        dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + CstDB.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                        break;
                    default:
                        throw (new PresentAbsentDALException("Unable to determine the type of table to use."));
                }

                // Obtient les tables de la nomenclature
                productTableName = _session.GenericProductDetailLevel.GetSqlTables(schAdEx.Label);
                if (productTableName != null && productTableName.Length > 0) productTableName = "," + productTableName;
                // Obtient les champs de la nomenclature
                productFieldName = _session.GenericProductDetailLevel.GetSqlFields();
                columnDetailLevel = ((DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0]).GetSqlFieldIdWithoutTablePrefix();
                // obtient la clause group by
                groupByFieldName = _session.GenericProductDetailLevel.GetSqlGroupByFields();
                // Obtient les jointures pour la nomenclature
                //mediaJoinCondition=GetMediaJoinConditions(webSession,DbTables.WEB_PLAN_PREFIXE,false);
                productJoinCondition = _session.GenericProductDetailLevel.GetSqlJoins(_session.SiteLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                // Unités
                unitField = FctWeb.SQLGenerator.GetUnitFieldName(_session, type);
                // Droits produit
                productsRights = FctWeb.SQLGenerator.getAnalyseCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                // Droits media
                mediaRights = FctWeb.SQLGenerator.getAnalyseCustomerMediaRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                // Liste des produits à exclure
                listExcludeProduct = FctWeb.SQLGenerator.GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
                //option encarts (pour la presse)
                if (CstDBClassif.Vehicles.names.press == _vehicle || CstDBClassif.Vehicles.names.internationalPress == _vehicle)
                    dataJointForInsert = FctWeb.SQLGenerator.GetJointForInsertDetail(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)) {
                    try {
                        dataTableNameForGad = ", " + tblGad.SqlWithPrefix;
                        dataFieldsForGad = ", " + FctWeb.SQLGenerator.GetFieldsAddressForGad(tblGad.Prefix);
                        dataJointForGad = "and " + FctWeb.SQLGenerator.GetJointForGad(tblGad.Prefix, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    }
                    catch (SQLGeneratorException) { ;}
                }
				////Agence_media
				//if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency) || _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency))
				//{
				//    mediaAgencyTable = schAdEx.Label + "." + _session.MediaAgencyFileYear + " " + CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ",";
				//    mediaAgencyJoins = "And " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_product=" + CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_product ";
				//    mediaAgencyJoins += "And " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_vehicle=" + CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_vehicle ";
				//}
            }
            catch (Exception e) {
                throw (new PresentAbsentDALException("Unable to intialise request parameters :" + e.Message));
            }

            try {
                if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                {
                    sql.AppendFormat(" select {0}.id_media, {1} as columnDetailLevel, {2} {3}, {4} as date_num, sum({5}) as unit"
                        , WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix
                        , columnDetailLevel
                        , productFieldName
                        , dataFieldsForGad
                        , dateField
                        , unitField
                    );
                }
                else
                {
                    sql.AppendFormat(" select {0}.id_media, {1} as columnDetailLevel, {2} {3}, sum({4}) as unit"
                        , WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix
                        , columnDetailLevel
                        , productFieldName
                        , dataFieldsForGad
                        , unitField
                    );
                }
                sql.AppendFormat(" from {0} {1} {2} {3}"
                    , mediaAgencyTable
                    , dataTableName
                    , productTableName
                    , dataTableNameForGad
                );
                // Période
                switch (type) {
                    case CstDB.TableType.Type.dataVehicle4M:
                        sql.AppendFormat(" where {0} >= {1}", dateField, customerPeriod.StartDate);
                        sql.AppendFormat(" and {0} <= {1}", dateField, customerPeriod.EndDate);
                        break;
                    case CstDB.TableType.Type.dataVehicle:
                        if (_session.CustomerPeriodSelected.PeriodDayBegin.Count == 0)
                        {
                            sql.AppendFormat(" where {0} >= {1}", dateField,  customerPeriod.StartDate);
                            sql.AppendFormat(" and {0} <= {1}", dateField, customerPeriod.EndDate);
                        }
                        else if (_session.CustomerPeriodSelected.PeriodDayBegin.Count == 2)
                        {
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
                        sql.AppendFormat(" where {0} >= {1}", dateField, customerPeriod.PeriodDayBegin[0].ToString().Substring(0, 6));
                        sql.AppendFormat(" and {0} <= {1}", dateField, customerPeriod.PeriodDayEnd[0].ToString().Substring(0, 6));
                        break;
                }
                // Jointures Produits
                sql.AppendFormat(" {0}", productJoinCondition);
                sql.AppendFormat(" {0}", dataJointForGad);
                sql.AppendFormat(" {0}", mediaAgencyJoins);
                //Jointures encart
                if (CstDBClassif.Vehicles.names.press == _vehicle || CstDBClassif.Vehicles.names.internationalPress == _vehicle)
                    sql.AppendFormat(" {0}", dataJointForInsert);

                #region Sélection de Médias
                while (_session.CompetitorUniversMedia[positionUnivers] != null) {
                    mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
                    positionUnivers++;
                }
                if (mediaList.Length > 0)
                    sql.AppendFormat(" and {0}.id_media in ({1})",
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                        mediaList.Substring(0, mediaList.Length - 1)
                    );
                #endregion

				// Sélection de Produits
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

                // Produit exclus
                sql.Append(listExcludeProduct);

                // Droits des Médias
                // Droits des Produits
                sql.AppendFormat(" {0}", productsRights);
                sql.AppendFormat(" {0}", mediaRights);
                // Group by
                if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    sql.AppendFormat(" group by {0}.id_media, {1}, {2}{3}, {4}",
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                        columnDetailLevel,
                        groupByFieldName,
                        dataFieldsForGad,
                        dateField
                    );
                else
                    sql.AppendFormat(" group by {0}.id_media, {1}, {2}{3}",
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                        columnDetailLevel,
                        groupByFieldName,
                        dataFieldsForGad
                    );

                return (sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PresentAbsentDALException("Unable to build the request required for Present/Absent report" + sql, err));
            }
            #endregion

        }
        #endregion

        #region GetData
        /// <summary>
        /// Load Data for absent/present report
        /// </summary>
        /// <returns>Present/Absent report data</returns>
        public DataSet GetData() {

            #region Constantes
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables
            StringBuilder sql = new StringBuilder();
            StringBuilder sql4M = new StringBuilder();
            StringBuilder sqlDataVehicle = new StringBuilder();
            StringBuilder sqlWebPlan = new StringBuilder();
            string groupByFieldNameWithoutTablePrefix = string.Empty;
            string orderFieldName = string.Empty, orderFieldNameWithoutTablePrefix = string.Empty;
            string productFieldNameWithoutTablePrefix = string.Empty, unitField = string.Empty, dataFieldsForGadWithoutTablePrefix = string.Empty;
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            #endregion

            #region Construction de la requête
            try {
                orderFieldName = _session.GenericProductDetailLevel.GetSqlOrderFields();
                orderFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
                groupByFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();

                if (customerPeriod.Is4M) {
                    sql4M.Append(GetRequest(CstDB.TableType.Type.dataVehicle4M));
                    sql4M.AppendFormat(" order by {0}, {1}.id_media",
                        orderFieldName,
                        DATA_TABLE_PREFIXE);
                    sql = sql4M;
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
                    sql.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                    sql.AppendFormat(" order by {0}, {1}.id_media",
                        orderFieldName,
                        DATA_TABLE_PREFIXE);
                }
                else {

                    if (customerPeriod.IsDataVehicle) {
                        sqlDataVehicle.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                        sql = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan) {
                        sqlWebPlan.Append(GetRequest(CstDB.TableType.Type.webPlan));
                        sql = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                        productFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();
                        if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                            dataFieldsForGadWithoutTablePrefix = ", " + FctWeb.SQLGenerator.GetFieldsAddressForGad("");
                        sql.AppendFormat(" select id_media, columnDetailLevel, {0}{1}, sum(unit) as unit",
                            productFieldNameWithoutTablePrefix,
                            dataFieldsForGadWithoutTablePrefix);
                        sql.Append(" from (");
                        sql.Append(sqlDataVehicle);
                        sql.Append(" UNION ");
                        sql.Append(sqlWebPlan);
                        sql.Append(" ) ");
                        sql.AppendFormat(" group by id_media, columnDetailLevel, {0}{1}",
                            groupByFieldNameWithoutTablePrefix,
                            dataFieldsForGadWithoutTablePrefix);
                    }

                    sql.AppendFormat(" order by {0}, id_media ", orderFieldNameWithoutTablePrefix);
                }
            }
            catch (System.Exception err) {
                throw (new PresentAbsentDALException("Unable to build request for present/absent report : " + sql, err));
            }
            #endregion
                
            #region Execution de la requête
            try {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PresentAbsentDALException("Unable to lad data for present/absent report : " + sql, err));
            }

            #endregion

        }
        #endregion

        #endregion

		#region GetNbParutionData
		/// <summary>
		/// Load Data for Number of parution
		/// </summary>
		/// <returns>Number of parution report data</returns>
		public DataSet GetNbParutionData() {

			#region Constantes
			string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
			#endregion

			#region Variables
			StringBuilder sql = new StringBuilder();
			string dataTableName = "";
			string dateField = "";
			string mediaList = "";
			int positionUnivers = 1;
			#endregion

			#region Construction de la requête
			try {
				CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
				dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicle, CstWeb.Module.Type.analysis);
				dateField = CstDB.Fields.DATE_MEDIA_NUM;

				sql.Append(" select id_media, count(distinct " + dateField + ") as NbParution");
				sql.Append(" from  " + dataTableName);
				sql.Append(" where " + dateField + " >= " + customerPeriod.StartDate + " and " + dateField + " <= " + customerPeriod.EndDate);

				#region Sélection de Médias
				while (_session.CompetitorUniversMedia[positionUnivers] != null) {
					mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
					positionUnivers++;
				}
				if (mediaList.Length > 0) sql.Append(" and id_media in (" + mediaList.Substring(0, mediaList.Length - 1) + ")");
				#endregion
				sql.Append(" group by id_media");
			}
			catch (System.Exception err) {
				throw (new PresentAbsentDALException("Unable to build request for Number of parutions : " + sql, err));
			}
			#endregion

			#region Execution de la requête
			try {
				return _session.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new PresentAbsentDALException("Unable to lad data for present/absent report : " + sql, err));
			}

			#endregion

		}
		#endregion

        #region Column Management

        #region GetMediaDetails
        /// <summary>
        /// Load list of media for column details
        /// </summary>
        /// <returns>List of media</returns>
        public DataSet GetMediaDetails()
        {
            return GetColumnDetails(true);

        }
        #endregion

        #region GetColumnDetails
        /// <summary>
        /// Load data to get the list of media for columns details
        /// </summary>
        /// <returns>Media List</returns>
        public DataSet GetColumnDetails()
        {

            return GetColumnDetails(false);

        }
        #endregion

        #region GetColumnDetails
        /// <summary>
        /// Load data to get the list of media for columns details
        /// </summary>
        /// <param name="withMediaLevel">Specify if media level is required</param>
        /// <returns>Media List</returns>
        protected DataSet GetColumnDetails(bool withMediaLevel)
        {

            #region Variables
            StringBuilder sql = new StringBuilder();
            string mediaList = "";
            int positionUnivers = 1;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            Table tblMedia = WebApplicationParameters.DataBaseDescription.GetTable(CstDBDesc.TableIds.media);
            Table tblCategory = WebApplicationParameters.DataBaseDescription.GetTable(CstDBDesc.TableIds.category);
            Table tblBasicMedia = WebApplicationParameters.DataBaseDescription.GetTable(CstDBDesc.TableIds.basicMedia);
            #endregion

            #region Construction de la requête
            if (withMediaLevel)
            {
                sql.Append(" select id_media, ");
            }
            else
            {
                sql.Append(" select distinct ");
            }
            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category)
                sql.AppendFormat("{0}.", tblCategory.Prefix);
            sql.AppendFormat("{0} as columnDetailLevel ", columnDetailLevel.GetSqlFieldIdWithoutTablePrefix());
            sql.AppendFormat(" from {0} ", tblMedia.SqlWithPrefix);
            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category)
            {
				sql.AppendFormat(", {0}, {1} ", tblBasicMedia.SqlWithPrefix, tblCategory.SqlWithPrefix);
            }

            #region Sélection de Médias
            while (_session.CompetitorUniversMedia[positionUnivers] != null)
            {
                mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
                positionUnivers++;
            }

            sql.AppendFormat(" where id_media in ({0})", mediaList.Substring(0, mediaList.Length - 1));
            sql.AppendFormat(" and {0}.id_language={1}", tblMedia.Prefix, _session.SiteLanguage);
            #endregion

            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category)
            {
                sql.AppendFormat(" and {0}.id_basic_media={1}.id_basic_media and {0}.id_category={2}.id_category and {0}.id_language={3} and {2}.id_language={3}"
                    , tblBasicMedia.Prefix
                    , tblMedia.Prefix
                    , tblCategory.Prefix
                    , _session.SiteLanguage);
            }

            #endregion

            #region Execution de la requête
            try
            {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new PresentAbsentDALException("Unable to load the list of supports for columns details:" + sql, err));
            }
            #endregion

        }
        #endregion

        #endregion
    }
}
