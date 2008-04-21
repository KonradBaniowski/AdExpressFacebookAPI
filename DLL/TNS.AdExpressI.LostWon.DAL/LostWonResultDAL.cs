#region Information
/*
 * Author : G Ragneau
 * Creation : 18/04/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

using CstCustom = TNS.AdExpress.Constantes.Customer;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctWeb = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core;
using TNS.AdExpressI.DynamicReport.DAL.Exceptions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Exceptions;
#endregion

namespace TNS.AdExpressI.LostWon.DAL
{

    /// <summary>
    /// Dynamic Report DAL
    /// </summary>
    public abstract class LostWonResultDAL:ILostWonResultDAL
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
        public LostWonResultDAL(WebSession session)
        {
            this._session = session;

            #region Sélection du vehicle
            string vehicleSelection = _session.GetSelection(_session.SelectionUniversMedia, CstCustom.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new DynamicDALException("Selection of media is not correct"));
            _vehicle = (CstDBClassif.Vehicles.names)int.Parse(vehicleSelection);
            #endregion

        }
        #endregion


        #region ILostWonResultDAL Membres

        #region GetData
        /// <summary>
        /// Get Data to build dynamic report (except summary)
        /// </summary>
        /// <returns>Dynamic Report Data</returns>
        public DataSet GetData()
        {
            #region Constantes
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables
            StringBuilder sql = new StringBuilder();
            StringBuilder sql4M = new StringBuilder();
            StringBuilder sqlDataVehicle = new StringBuilder();
            StringBuilder sqlWebPlan = new StringBuilder();
            StringBuilder sqlTemp = new StringBuilder();
            string groupByFieldNameWithoutTablePrefix = string.Empty;
            string orderFieldName = string.Empty;
            string orderFieldNameWithoutTablePrefix = string.Empty;
            string productFieldNameWithoutTablePrefix = string.Empty;
            string unitField = string.Empty;
            string dataFieldsForGadWithoutTablePrefix = string.Empty;
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

                    if (customerPeriod.IsDataVehicle) {
                        sqlDataVehicle.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                        sqlTemp = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan) {
                        sqlWebPlan.Append(GetRequest(CstDB.TableType.Type.webPlan));
                        sqlTemp = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                        sqlTemp = new StringBuilder();
                        sqlTemp.AppendFormat("{0} UNION {1}",sqlDataVehicle, sqlWebPlan);
                    }

                    productFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();
                    if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                        dataFieldsForGadWithoutTablePrefix = ", " + FctWeb.SQLGenerator.GetFieldsAddressForGad("");

                    sql = new StringBuilder();
                    sql.AppendFormat(" select id_media, columnDetailLevel, {0} {1}, date_num, sum(unit) as unit"
                        , productFieldNameWithoutTablePrefix
                        , dataFieldsForGadWithoutTablePrefix);
                    sql.Append(" from (");
                    sql.Append(sql4M);
                    sql.Append(" UNION ");
                    sql.Append(sqlTemp);
                    sql.Append(" ) ");
                    sql.AppendFormat(" group by id_media, columnDetailLevel, date_num, {0} {1}"
                        , groupByFieldNameWithoutTablePrefix
                        , dataFieldsForGadWithoutTablePrefix);
                    sql.AppendFormat(" order by {0}, id_media, date_num ", orderFieldNameWithoutTablePrefix);
 
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
                    sql.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                    sql.AppendFormat(" order by {0}, {1}.id_media, date_num  "
                        , orderFieldName
                        ,DATA_TABLE_PREFIXE);
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
                        sql = new StringBuilder();
                        sql.AppendFormat(" select id_media, columnDetailLevel, {0} {1}, date_num, sum(unit) as unit"
                            , productFieldNameWithoutTablePrefix
                            , dataFieldsForGadWithoutTablePrefix);
                        sql.Append(" from (");
                        sql.Append(sqlDataVehicle);
                        sql.Append(" UNION ");
                        sql.Append(sqlWebPlan);
                        sql.Append(" ) ");
                        sql.AppendFormat(" group by id_media, columnDetailLevel, date_num, {0} {1}"
                            , groupByFieldNameWithoutTablePrefix
                            , dataFieldsForGadWithoutTablePrefix);
                    }

                    sql.AppendFormat(" order by {0}, id_media, date_num "
                        , orderFieldNameWithoutTablePrefix);
                
                }

            }
            catch (System.Exception err) {
                throw (new DynamicDALException("Error while building request for dynamic report : " + sql, err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new DynamicDALException("Unable to load data for dynamic report : " + sql, err));
            }

            #endregion
        }
        #endregion

        #region GetSynthesisData
        /// <summary>
        /// Load Data for synthesis report in Dynamic Module
        /// </summary>
        /// <returns>Data</returns>
        public DataTable GetSynthesisData()
        {

            #region Constantes
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables
            StringBuilder sql = new StringBuilder();
            StringBuilder sql4M = new StringBuilder();
            StringBuilder sqlDataVehicle = new StringBuilder();
            StringBuilder sqlWebPlan = new StringBuilder();
            StringBuilder sqlTemp = new StringBuilder();
            string groupByFieldNameWithoutTablePrefix = string.Empty;
            string orderFieldName = string.Empty;
            string orderFieldNameWithoutTablePrefix = string.Empty;
            string productFieldNameWithoutTablePrefix = string.Empty;
            string unitField = string.Empty;
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
            #endregion

            #region Construction de la requête
            try
            {
                orderFieldName = _session.GenericProductDetailLevel.GetSqlOrderFields();
                orderFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
                groupByFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();

                if (customerPeriod.Is4M)
                {
                    sql4M.Append(GetSynthesisRequest(CstDB.TableType.Type.dataVehicle4M));

                    if (customerPeriod.IsDataVehicle)
                    {
                        sqlDataVehicle.Append(GetSynthesisRequest(CstDB.TableType.Type.dataVehicle));
                        sqlTemp = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan)
                    {
                        sqlWebPlan.Append(GetSynthesisRequest(CstDB.TableType.Type.webPlan));
                        sqlTemp = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    {
                        sqlTemp = new StringBuilder();
                        sqlTemp.Append(sqlDataVehicle);
                        sqlTemp.Append(" UNION ");
                        sqlTemp.Append(sqlWebPlan);
                    }

                    sql = new StringBuilder();
                    sql.Append("  select id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product");
                    sql.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, date_num, sum(unit) as unit");
                    sql.Append(" from (");
                    sql.Append(sql4M);
                    sql.Append(" UNION ");
                    sql.Append(sqlTemp);
                    sql.Append(" ) ");

                    sql.Append("  group by id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product");
                    sql.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, date_num");
                    
                    sql.Append("  order by  id_sector, id_subsector,  id_group_, id_advertiser, id_brand, id_product");
                    sql.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, date_num");
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan)
                {
                    sql.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                    sql.AppendFormat("  order by {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
                    sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand, {0}.id_product", DATA_TABLE_PREFIXE);
                    sql.AppendFormat(",{0}.ID_GROUP_ADVERTISING_AGENCY,{0}.ID_ADVERTISING_AGENCY", CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE);
                    sql.Append(", date_num");
                }
                else
                {

                    if (customerPeriod.IsDataVehicle)
                    {
                        sqlDataVehicle.Append(GetSynthesisRequest(CstDB.TableType.Type.dataVehicle));
                        sql = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan)
                    {
                        sqlWebPlan.Append(GetSynthesisRequest(CstDB.TableType.Type.webPlan));
                        sql = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    {
                        sql = new StringBuilder();
                        sql.Append("  select id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product");
                        sql.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, date_num, sum(unit) as unit");
                        sql.Append(" from (");
                        sql.Append(sqlDataVehicle);
                        sql.Append(" UNION ");
                        sql.Append(sqlWebPlan);
                        sql.Append(" ) ");
                        sql.Append("  group by id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product");
                        sql.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, date_num");
                    }

                    sql.Append("  order by  id_sector, id_subsector,  id_group_, id_advertiser, id_brand, id_product");
                    sql.Append(", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, date_num");

                }

            }
            catch (System.Exception err)
            {
                throw (new DynamicDALException("Unable to build request for synthesis report : " + sql, err));
            }
            #endregion

            #region Execution de la requête
            try
            {
                return _session.Source.Fill(sql.ToString()).Tables[0];
            }
            catch (System.Exception err)
            {
                throw (new DynamicDALException("Unable to load data for synthesis report : " + sql, err));
            }

            #endregion

        }
        #endregion

        #region GetMediaDetails
        /// <summary>
        /// Get Media for column Details
        /// </summary>
        /// <returns>DataSet with Media Details</returns>
        public DataSet GetMediaDetails()
        {
            #region Variables
            StringBuilder sql = new StringBuilder();
            string mediaList = "";
            string prefixe = "";
            int positionUnivers = 1;

            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            bool isCategoryRequired = (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category);

            Table tblCategory = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category);
            Table tblMedia = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media);
            Table tblBasicMedia = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia);
            #endregion

            #region Construction de la requête
            sql.Append(" select id_media, ");
            if (isCategoryRequired) sql.AppendFormat("{0}.", tblCategory.Prefix);
            sql.AppendFormat("{0} as columnDetailLevel", columnDetailLevel.GetSqlFieldIdWithoutTablePrefix());
            sql.AppendFormat(" from {0}", tblMedia.SqlWithPrefix);
            if (isCategoryRequired) {
                sql.AppendFormat(", {0}, {1}", tblBasicMedia.SqlWithPrefix, tblCategory.SqlWithPrefix);
            }

            #region Sélection de Médias
            while (_session.CompetitorUniversMedia[positionUnivers] != null) {
                mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustom.Right.type.mediaAccess) + ",";
                positionUnivers++;
            }

            sql.AppendFormat(" where id_media in ({0})", mediaList.Substring(0, mediaList.Length - 1));
            sql.AppendFormat(" and {0}.id_language = {1}", tblMedia.Prefix, _session.SiteLanguage);
            #endregion

            if (isCategoryRequired)
            {
                sql.AppendFormat(" and {0}.id_basic_media = {1}.id_basic_media", tblBasicMedia.Prefix, tblMedia.Prefix);
                sql.AppendFormat(" and {0}.id_category = {1}.id_category", tblBasicMedia.Prefix, tblCategory.Prefix);
                sql.AppendFormat(" and {0}.id_language = {1}", tblBasicMedia.Prefix, _session.SiteLanguage);
                sql.AppendFormat(" and {0}.id_language = {1}", tblCategory.Prefix, _session.SiteLanguage);
            }
            #endregion

            #region Execution de la requête
            try {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new DynamicDALException("Unable to load the list of media for the column details : " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #endregion

        #region Internal methods

        #region Get Request
        /// <summary>
        /// Build requets to load data for dynamic report.
        /// </summary>
        /// <param name="type">Type of table to use</param>
        /// <returns>Dynamic Report Data Request</returns>
        protected string GetRequest(CstDB.TableType.Type type)
        {

            #region Constantes
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

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
            StringBuilder sql = new StringBuilder();
            string list = "";
            int positionUnivers = 1;
            string mediaList = "";
            bool premier;
            string dataJointForInsert = "";

            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            Schema schAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
            #endregion

            #region Construction de la requête
            try
            {
                // Table de données
                switch (type)
                {
                    case CstDB.TableType.Type.dataVehicle4M:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicle, CstWeb.Module.Type.alert);
                        break;
                    case CstDB.TableType.Type.dataVehicle:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicle, CstWeb.Module.Type.analysis);
                        break;
                    case CstDB.TableType.Type.webPlan:
                        dataTableName = CstDB.Tables.WEB_PLAN_MEDIA_MONTH;
                        break;
                    default:
                        throw (new DynamicDALException("Unaable to determùine the type of table to use."));
                }

                // Obtient les tables de la nomenclature
                productTableName = _session.GenericProductDetailLevel.GetSqlTables(CstDB.Schema.ADEXPRESS_SCHEMA);
                if (productTableName != null && productTableName.Length > 0) productTableName = "," + productTableName;
                // Obtient les champs de la nomenclature
                productFieldName = _session.GenericProductDetailLevel.GetSqlFields();
                // Obtient l'ordre des champs
                orderFieldName = _session.GenericProductDetailLevel.GetSqlOrderFields();
                // obtient la clause group by
                groupByFieldName = _session.GenericProductDetailLevel.GetSqlGroupByFields();
                // Obtient les jointures pour la nomenclature
                productJoinCondition = _session.GenericProductDetailLevel.GetSqlJoins(_session.SiteLanguage, DATA_TABLE_PREFIXE);
                // Unités
                unitField = FctWeb.SQLGenerator.GetUnitFieldName(_session, type);
                // Droits produit
                productsRights = FctWeb.SQLGenerator.getAnalyseCustomerProductRight(_session, DATA_TABLE_PREFIXE, true);
                // Droits media
                mediaRights = FctWeb.SQLGenerator.getAnalyseCustomerMediaRight(_session, DATA_TABLE_PREFIXE, true);
                // Dates
                switch (type)
                {
                    case CstDB.TableType.Type.dataVehicle4M:
                    case CstDB.TableType.Type.dataVehicle:
                        dateField = DATA_TABLE_PREFIXE + "." + CstDB.Fields.DATE_MEDIA_NUM;
                        break;
                    case CstDB.TableType.Type.webPlan:
                        dateField = DATA_TABLE_PREFIXE + "." + CstDB.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                        break;
                }
                // Liste des produits à exclure
                listExcludeProduct = FctWeb.SQLGenerator.GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DATA_TABLE_PREFIXE, true, false);
                //option encarts (pour la presse)
                if (CstDBClassif.Vehicles.names.press == _vehicle || CstDBClassif.Vehicles.names.internationalPress == _vehicle)
                    dataJointForInsert = FctWeb.SQLGenerator.getJointForInsertDetail(_session, DATA_TABLE_PREFIXE, type);
                if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                {
                    try
                    {
                        dataTableNameForGad = ", " + CstDB.Schema.ADEXPRESS_SCHEMA + "." + FctWeb.SQLGenerator.GetTablesForGad(_session) + " " + CstDB.Tables.GAD_PREFIXE;
                        dataFieldsForGad = ", " + FctWeb.SQLGenerator.GetFieldsAddressForGad();
                        dataJointForGad = "and " + FctWeb.SQLGenerator.GetJointForGad(DATA_TABLE_PREFIXE);
                    }
                    catch (SQLGeneratorException) { ;}
                }
                //Agence_media
                if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency) || _session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency))
                {
                    mediaAgencyTable = CstDB.Schema.ADEXPRESS_SCHEMA + "." + _session.MediaAgencyFileYear + " " + CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ",";
                    mediaAgencyJoins = "And " + DATA_TABLE_PREFIXE + ".id_product=" + CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_product ";
                    mediaAgencyJoins += "And " + DATA_TABLE_PREFIXE + ".id_vehicle=" + CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_vehicle ";
                }
            }
            catch (Exception e)
            {
                throw (new DynamicDALException("Unable to init parameters for request : " + e.Message));
            }

            sql.AppendFormat(" select {0}.id_media, {1} as columnDetailLevel, {2} {3}, {4} as date_num, sum({5}) as unit"
                , DATA_TABLE_PREFIXE
                , columnDetailLevel.GetSqlFieldIdWithoutTablePrefix()
                , productFieldName
                , dataFieldsForGad
                , dateField
                , unitField);
            sql.AppendFormat(" from {0} {1}.{2} {3} {4} {5}"
                , mediaAgencyTable
                , schAdExpr03.Label
                , dataTableName
                , DATA_TABLE_PREFIXE
                , productTableName
                , dataTableNameForGad);

            // Période
            switch (type)
            {
                case CstDB.TableType.Type.dataVehicle4M:
                    sql.AppendFormat(" where ({0} >= {1} and {0} <= {2})"
                        , dateField
                        , customerPeriod.StartDate
                        , customerPeriod.EndDate);
                    break;
                case CstDB.TableType.Type.dataVehicle:
                    if ((_session.CustomerPeriodSelected.PeriodDayBegin.Count == 0)
                        && (_session.CustomerPeriodSelected.ComparativePeriodDayBegin.Count == 0))
                    {
                        sql.AppendFormat(" where (({0} >= {1} and {0} <= {2}) or ({0} >= {3} and {0} <= {4}))"
                            , dateField
                            , customerPeriod.StartDate
                            , customerPeriod.EndDate
                            , customerPeriod.ComparativeStartDate
                            , customerPeriod.ComparativeEndDate);
                    }
                    else
                    {
                        sql.AppendFormat(" where {0}", GetDayPeriod(dateField, customerPeriod));
                    }
                    break;
                case CstDB.TableType.Type.webPlan:
                    sql.AppendFormat(" where ",GetMonthPeriod(dateField, customerPeriod));
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
            while (_session.CompetitorUniversMedia[positionUnivers] != null)
            {
                mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustom.Right.type.mediaAccess) + ",";
                positionUnivers++;
            }
            if (mediaList.Length > 0) sql.AppendFormat(" and {0}.id_media in ({1})", DATA_TABLE_PREFIXE, mediaList.Substring(0, mediaList.Length - 1));
            #endregion

            // Sélection de Produits
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(DATA_TABLE_PREFIXE, true));

            // Produit exclus
            sql.Append(listExcludeProduct);

            // Droits des Médias
            // Droits des Produits
            sql.Append(productsRights);
            sql.AppendFormat(" {0}", mediaRights);
            // Group by
            sql.AppendFormat(" group by {0}.id_media, {1}, {2}, {3} {4}"
                , DATA_TABLE_PREFIXE
                , columnDetailLevel.GetSqlFieldIdWithoutTablePrefix()
                , dateField
                , groupByFieldName
                , dataFieldsForGad);

            #endregion

            #region Execution de la requête
            try
            {
                return (sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new DynamicDALException("Unable to build request for dynamic synthesis : ", err));
            }
            #endregion

        }
        #endregion

        #region Get Synthesis Request
        /// <summary>
        /// Build request to load data about synthesis report
        /// </summary>
        /// <param name="type">Type of table to use</param>
        /// <returns>Code SQL</returns>
        protected string GetSynthesisRequest(CstDB.TableType.Type type) {

            #region Constantes
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables
            string dateField = "";
            string dataTableName = "";
            string unitField = "";
            string productsRights = "";
            string mediaRights = "";
            string listExcludeProduct = "";
            StringBuilder sql = new StringBuilder(3000);
            string list = "";
            int positionUnivers = 1;
            string mediaList = "";
            bool premier;
            string dataJointForInsert = "";

            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;

            Table tblWepPlan = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData);
            Schema schAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
            #endregion

            // Table de données
            switch (type) {
                case CstDB.TableType.Type.dataVehicle4M:
                    dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicle, CstWeb.Module.Type.alert);
                    break;
                case CstDB.TableType.Type.dataVehicle:
                    dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicle, CstWeb.Module.Type.analysis);
                    break;
                case CstDB.TableType.Type.webPlan:
                    dataTableName = tblWepPlan.Label;
                    break;
                default:
                    throw (new DynamicDALException("Unable to determine the type of table to use."));
            }

            unitField = FctWeb.SQLGenerator.GetUnitFieldName(_session, type);
            productsRights = FctWeb.SQLGenerator.getAnalyseCustomerProductRight(_session, DATA_TABLE_PREFIXE, true);
            mediaRights = FctWeb.SQLGenerator.getAnalyseCustomerMediaRight(_session, DATA_TABLE_PREFIXE, true);
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
            listExcludeProduct = FctWeb.SQLGenerator.GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DATA_TABLE_PREFIXE, true, false);

            //option encarts (pour la presse)
            if (CstDBClassif.Vehicles.names.press == _vehicle || CstDBClassif.Vehicles.names.internationalPress == _vehicle)
                dataJointForInsert = FctWeb.SQLGenerator.getJointForInsertDetail(_session, DATA_TABLE_PREFIXE, type);

            sql.AppendFormat("  select {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
            sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand, {0}.id_product", DATA_TABLE_PREFIXE);
            sql.AppendFormat(",{0}.ID_GROUP_ADVERTISING_AGENCY,{0}.ID_ADVERTISING_AGENCY", CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE);
            sql.AppendFormat(",{0} as date_num, sum({1}) as unit", dateField, unitField);

            sql.AppendFormat(" from {0}.{1} {2}", schAdExpr03.Label, dataTableName, DATA_TABLE_PREFIXE);
            sql.AppendFormat(",{0}.{1} {2}", schAdExpr03.Label, _session.MediaAgencyFileYear, CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE);

            // Période
            switch (type) {
                case CstDB.TableType.Type.dataVehicle4M:
                    sql.AppendFormat(" where ({0} >= {1} and {0} <= {2})", dateField, customerPeriod.StartDate, customerPeriod.EndDate);
                    break;
                case CstDB.TableType.Type.dataVehicle:
                    if ((_session.CustomerPeriodSelected.PeriodDayBegin.Count == 0)
                        && (_session.CustomerPeriodSelected.ComparativePeriodDayBegin.Count == 0)) {
                            sql.AppendFormat(" where ( ({0} >= {1} and {0} <= {2}) or ({0} >= {3} and {0} <= {4}))"
                                , dateField
                                , customerPeriod.StartDate
                                , customerPeriod.EndDate
                                , customerPeriod.ComparativeStartDate
                                , customerPeriod.ComparativeEndDate);
                    }
                    else {
                        sql.AppendFormat(" where {0}", GetDayPeriod(dateField, customerPeriod));
                    }
                    break;
                case CstDB.TableType.Type.webPlan:
                    sql.AppendFormat(" where {0}", GetMonthPeriod(dateField, customerPeriod));
                    break;
            }

            //Jointures 
            if (CstDBClassif.Vehicles.names.press == _vehicle || CstDBClassif.Vehicles.names.internationalPress == _vehicle)
                sql.AppendFormat(" {0}", dataJointForInsert);

            //Jointures groupe agences/agences		
            sql.AppendFormat(" and {0}.id_product(+)={1}.id_product ", CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE, DATA_TABLE_PREFIXE);
            sql.AppendFormat(" and {0}.id_language(+)={1}", CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE, _session.SiteLanguage);
            // Vehicle
            sql.AppendFormat(" and {0}.id_vehicle(+)={1}", CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE, _vehicle.GetHashCode());

            #region Sélection de Médias
            while (_session.CompetitorUniversMedia[positionUnivers] != null) {
                mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustom.Right.type.mediaAccess) + ",";
                positionUnivers++;
            }
            if (mediaList.Length > 0) sql.AppendFormat(" and {0}.id_media in ({1})", DATA_TABLE_PREFIXE, mediaList.Substring(0, mediaList.Length - 1));
            #endregion            

			// Sélection de Produits
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(DATA_TABLE_PREFIXE, true));

            // Produit exclus
            sql.Append(listExcludeProduct);

            // Droits des Médias
            // Droits des Produits
            sql.AppendFormat(" {0}", productsRights);
            sql.AppendFormat(" {0}", mediaRights);
            // Group by			
            sql.AppendFormat("  group by {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
            sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand, {0}.id_product", DATA_TABLE_PREFIXE);
            sql.AppendFormat(", {0}.ID_GROUP_ADVERTISING_AGENCY,{0}.ID_ADVERTISING_AGENCY", CstDB.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE);
            sql.AppendFormat(", {0}", dateField);

            #region Execution de la requête
            try {
                return sql.ToString();
            }
            catch (System.Exception err) {
                throw (new DynamicDALException("Unable to build request to load data about synthesis report : ", err));
            }
            #endregion
        }
        #endregion

        #region Get Period Clause
        /// <summary>
        /// Get SQL code for period filter
        /// </summary>
        /// <returns>SQL code</returns>
        protected string GetDayPeriod(string dateField, CustomerPeriod customerPeriod) {

            string sql = string.Empty;
            string comparativeSql = string.Empty;

            if (customerPeriod.PeriodDayBegin.Count == 2) {
                sql = string.Format(" (({0} >= {1} and {0} <= {2}) or ({0} >= {3} and {0} <= {4}))"
                    , dateField
                    , customerPeriod.PeriodDayBegin[0]
                    , customerPeriod.PeriodDayEnd[0]
                    , customerPeriod.PeriodDayBegin[1]
                    , customerPeriod.PeriodDayEnd[1]);
            }
            else if (customerPeriod.PeriodDayBegin.Count == 1) {
                sql = string.Format(" ({0} >= {1} and {0} <= {2})"
                    , dateField
                    , customerPeriod.PeriodDayBegin[0]
                    , customerPeriod.PeriodDayEnd[0]
                );
            }

            if (customerPeriod.ComparativePeriodDayBegin.Count == 2) {
                comparativeSql = string.Format(" (({0} >= {1} and {0} <= {2}) or ({0} >= {3} and {0} <= {4}))"
                                    , dateField
                                    , customerPeriod.ComparativePeriodDayBegin[0]
                                    , customerPeriod.ComparativePeriodDayEnd[0]
                                    , customerPeriod.ComparativePeriodDayBegin[1]
                                    , customerPeriod.ComparativePeriodDayEnd[1]
                                 );
            }
            else if (customerPeriod.ComparativePeriodDayBegin.Count == 1) {
                comparativeSql = string.Format(" ({0} >= {1} and {0} <= {2})"
                    , dateField
                    , customerPeriod.ComparativePeriodDayBegin[0]
                    , customerPeriod.ComparativePeriodDayEnd[0]
                );
            }

            if (sql.Length > 0 && comparativeSql.Length > 0)
                sql = string.Format("( ({0}) or ({1}) )", sql, comparativeSql);
            else if (comparativeSql.Length > 0)
                sql = comparativeSql;

            return sql;
        }

        /// <summary>
        /// Get SQL code for period filter
        /// </summary>
        /// <returns>Code SQL</returns>
        protected string GetMonthPeriod(string dateField, CustomerPeriod customerPeriod) {

            string sql = string.Empty;
            string comparativeSql = string.Empty;

            if (customerPeriod.PeriodMonthBegin.Count == 1)
            {
                sql = string.Format(" ({0} >= {1} and {0} <= {2})"
                    , dateField
                    , customerPeriod.PeriodMonthBegin[0]
                    , customerPeriod.PeriodMonthEnd[0]
                );
            }

            if (customerPeriod.ComparativePeriodMonthBegin.Count == 1)
            {
                comparativeSql = string.Format(" ({0} >= {1} and {0} <= {2})"
                    , dateField
                    , customerPeriod.ComparativePeriodMonthBegin[0]
                    , customerPeriod.ComparativePeriodMonthEnd[0]
                );
            }

            if (sql.Length > 0 && comparativeSql.Length > 0)
                sql = string.Format("( ({0}) or ({1}) )", sql, comparativeSql);
            else if (comparativeSql.Length > 0)
                sql = comparativeSql;

            return sql;
        }
        #endregion

        #endregion

    }

}
