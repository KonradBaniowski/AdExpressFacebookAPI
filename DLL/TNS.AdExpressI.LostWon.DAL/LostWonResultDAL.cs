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
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using System.Collections.Generic;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core;
using TNS.AdExpressI.DynamicReport.DAL.Exceptions;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using System.Text.RegularExpressions;
#endregion

namespace TNS.AdExpressI.LostWon.DAL
{

    /// <summary>
    /// Dynamic Report DAL
    /// </summary>
    public abstract class LostWonResultDAL : ILostWonResultDAL
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
        /// Get Vehicle
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
        /// <param name="session"></param>
        public LostWonResultDAL(WebSession session)
        {
            this._session = session;

            #region S�lection du vehicle
            string vehicleSelection = _session.GetSelection(_session.SelectionUniversMedia, CstCustom.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new DynamicDALException("Selection of media is not correct"));
            _vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            #endregion

        }
        #endregion


        #region ILostWonResultDAL Membres

        #region GetData
        /// <summary>
        /// Get Data to build dynamic report (except summary)
        /// </summary>
        /// <returns>Dynamic Report Data</returns>
        public DataTable GetData()
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
            string unitFieldSumWithAlias = string.Empty;
            string dataFieldsForGadWithoutTablePrefix = string.Empty;
            string groupByOptional = string.Empty;
            string orderByOptional = string.Empty;
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            #endregion

            #region Construction de la requ�te
            try
            {
                orderFieldName = _session.GenericProductDetailLevel.GetSqlOrderFields();
                orderFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
                groupByFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();

                if (_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack && _session.Unit == CstWeb.CustomerSessions.Unit.versionNb)
                {
                    unitFieldSumWithAlias = string.Format("{0},lineId", FctWeb.SQLGenerator.GetUnitAlias(_session));
                    groupByOptional = string.Format(", {0}, lineId", unitFieldSumWithAlias);
                    orderByOptional = ", lineId";
                }
                else
                {
                    unitFieldSumWithAlias = FctWeb.SQLGenerator.GetUnitFieldNameSumUnionWithAlias(_session);
                }

                if (customerPeriod.Is4M)
                {
                    sql4M.Append(GetRequest(CstDB.TableType.Type.dataVehicle4M));

                    if (customerPeriod.IsDataVehicle)
                    {
                        sqlDataVehicle.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                        sqlTemp = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan)
                    {
                        sqlWebPlan.Append(GetRequest(CstDB.TableType.Type.webPlan));
                        sqlTemp = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    {
                        sqlTemp = new StringBuilder();
                        sqlTemp.AppendFormat("{0} UNION ALL {1}", sqlDataVehicle, sqlWebPlan);
                    }

                    productFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();
                    if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                        dataFieldsForGadWithoutTablePrefix = ", " + FctWeb.SQLGenerator.GetFieldsAddressForGad("");

                    sql = new StringBuilder();
                    sql.AppendFormat(" select id_media, columnDetailLevel, {0} {1}, date_num, {2}"
                        , productFieldNameWithoutTablePrefix
                        , dataFieldsForGadWithoutTablePrefix
                        , unitFieldSumWithAlias);
                    sql.Append(" from (");
                    sql.Append(sql4M);
                    sql.Append(" UNION ALL ");
                    sql.Append(sqlTemp);
                    sql.Append(" ) ");
                    groupByFieldNameWithoutTablePrefix = string.Format("id_media, columnDetailLevel, date_num, {0} {1}"
                        , groupByFieldNameWithoutTablePrefix
                        , dataFieldsForGadWithoutTablePrefix);
                    sql.AppendFormat(" group by {0}{1}", groupByFieldNameWithoutTablePrefix, groupByOptional);
                    orderFieldNameWithoutTablePrefix = string.Format(" {0}, id_media, date_num ", orderFieldNameWithoutTablePrefix);
                    sql.AppendFormat(" order by {0} {1}", orderFieldNameWithoutTablePrefix, orderByOptional);

                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan)
                {
                    sql.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                    orderFieldNameWithoutTablePrefix = string.Format(" {0}, {1}.id_media, date_num ", orderFieldName, DATA_TABLE_PREFIXE);
                    sql.AppendFormat(" order by {0} {1} ", orderFieldNameWithoutTablePrefix, orderByOptional);
                }
                else
                {

                    if (customerPeriod.IsDataVehicle)
                    {
                        sqlDataVehicle.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                        sql = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan)
                    {
                        sqlWebPlan.Append(GetRequest(CstDB.TableType.Type.webPlan));
                        sql = sqlWebPlan;
                    }

                    if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                        dataFieldsForGadWithoutTablePrefix = ", " + FctWeb.SQLGenerator.GetFieldsAddressForGad("");
                    groupByFieldNameWithoutTablePrefix = string.Format(" id_media, columnDetailLevel, date_num, {0} {1}"
                        , groupByFieldNameWithoutTablePrefix
                        , dataFieldsForGadWithoutTablePrefix);
                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    {
                        productFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();
                        sql = new StringBuilder();
                        sql.AppendFormat(" select id_media, columnDetailLevel, {0} {1}, date_num, {2}"
                            , productFieldNameWithoutTablePrefix
                            , dataFieldsForGadWithoutTablePrefix
                            , unitFieldSumWithAlias);
                        sql.Append(" from (");
                        sql.Append(sqlDataVehicle);
                        sql.Append(" UNION ALL ");
                        sql.Append(sqlWebPlan);
                        sql.Append(" ) ");
                        sql.AppendFormat(" group by {0}{1}", groupByFieldNameWithoutTablePrefix, groupByOptional);
                    }
                    orderFieldNameWithoutTablePrefix = string.Format(" {0}, id_media, date_num ", orderFieldNameWithoutTablePrefix);
                    sql.AppendFormat(" order by {0} {1}", orderFieldNameWithoutTablePrefix, orderByOptional);

                }

            }
            catch (System.Exception err)
            {
                throw (new DynamicDALException("Error while building request for dynamic report : " + sql, err));
            }
            #endregion

            #region Execution de la requ�te
            try
            {
                DataSet ds = _session.Source.Fill(sql.ToString());
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                    return null;
                DataTable dt = ds.Tables[0];
                if (_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack && _session.Unit == CstWeb.CustomerSessions.Unit.versionNb)
                {
                    Regex regex = new Regex(@"(order\s*by\s*\w*\.)|(group\s*by\s*\w*\.)|(,\s*\w*\.)|(\s*\w*\.)");
                    string order = regex.Replace(orderFieldNameWithoutTablePrefix, ",");
                    string group = regex.Replace(groupByFieldNameWithoutTablePrefix, ",");
                    if (order.StartsWith(",")) order = order.Substring(0, order.Length - 1);
                    if (group.StartsWith(",")) group = group.Substring(0, group.Length - 1);
                    string[] tOrders = order.Split(',');
                    string[] tGroups = group.Split(',');
                    List<string> lOrders = new List<string>();
                    List<string> lGroups = new List<string>();
                    List<string> lSum = new List<string>();
                    for (int i = 0; i < tOrders.Length; i++) { lOrders.Add(tOrders[i].ToUpper().Trim());}
                    for (int i = 0; i < tGroups.Length; i++) { lGroups.Add(tGroups[i].ToUpper().Trim()); }
                    lGroups.Remove("LINEID");
                    lSum.Add(FctWeb.SQLGenerator.GetUnitAlias(_session).ToUpper().Trim());
                    
                    FctUtilities.DataAdapter adapter = new FctUtilities.DataAdapter();
                    dt = adapter.GroupBy(dt, lOrders, lGroups, "LINEID", lSum);
                    

                }
                return dt;
            }
            catch (System.Exception err)
            {
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
            string unitFieldSumUnionWithAlias = string.Empty;
            string groupByOptional = string.Empty;
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
            #endregion

            #region Construction de la requ�te
            try
            {
                orderFieldName = _session.GenericProductDetailLevel.GetSqlOrderFields();
                orderFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
                groupByFieldNameWithoutTablePrefix = _session.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();
                if (_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack && _session.Unit == CstWeb.CustomerSessions.Unit.versionNb)
                {
                    unitFieldSumUnionWithAlias = FctWeb.SQLGenerator.GetUnitAlias(_session);
                    groupByOptional = string.Format(", {0}", FctWeb.SQLGenerator.GetUnitAlias(_session));
                }
                else
                {
                    unitFieldSumUnionWithAlias = FctWeb.SQLGenerator.GetUnitFieldNameSumUnionWithAlias(_session);
                }

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
                    sql.Append("  select id_sector, id_subsector, id_group_, id_advertiser, id_brand");
                    if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) sql.Append(", id_product");
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                        sql.Append(", ID_GROUP_ADVERTISING_AGENCY");
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
                        sql.Append(", ID_ADVERTISING_AGENCY");
                    sql.AppendFormat(", date_num, {0}", unitFieldSumUnionWithAlias);
                    sql.Append(" from (");
                    sql.Append(sql4M);
                    sql.Append(" UNION ");
                    sql.Append(sqlTemp);
                    sql.Append(" ) ");

                    sql.Append("  group by id_sector, id_subsector, id_group_, id_advertiser, id_brand ");
                    if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) sql.Append(", id_product");
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                        sql.Append(", ID_GROUP_ADVERTISING_AGENCY");
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
                        sql.Append(", ID_ADVERTISING_AGENCY");
                    sql.Append(", date_num");
                    sql.Append(groupByOptional);
                    sql.Append("  order by  id_sector, id_subsector,  id_group_, id_advertiser, id_brand ");
                    if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) sql.Append(", id_product");
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                        sql.Append(", ID_GROUP_ADVERTISING_AGENCY");
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
                        sql.Append(", ID_ADVERTISING_AGENCY");
                    sql.Append(", date_num");
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan)
                {
                    sql.Append(GetRequest(CstDB.TableType.Type.dataVehicle));
                    sql.AppendFormat("  order by {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
                    sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand ", DATA_TABLE_PREFIXE);
                    if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                        sql.AppendFormat(", {0}.id_product", DATA_TABLE_PREFIXE);
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                        sql.AppendFormat(",{0}.ID_GROUP_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
                        sql.AppendFormat(",{0}.ID_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
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
                        sql.Append("  select id_sector, id_subsector, id_group_, id_advertiser, id_brand ");
                        if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) sql.Append(", id_product");
                        if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                            sql.Append(", ID_GROUP_ADVERTISING_AGENCY");
                        if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
                            sql.Append(", ID_ADVERTISING_AGENCY");
                        sql.AppendFormat(", date_num, {0}", unitFieldSumUnionWithAlias);
                        sql.Append(" from (");
                        sql.Append(sqlDataVehicle);
                        sql.Append(" UNION ");
                        sql.Append(sqlWebPlan);
                        sql.Append(" ) ");
                        sql.Append("  group by id_sector, id_subsector, id_group_, id_advertiser, id_brand ");
                        if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) sql.Append(", id_product");

                        if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                            sql.Append(", ID_GROUP_ADVERTISING_AGENCY");
                        if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
                            sql.Append(", ID_ADVERTISING_AGENCY");
                        sql.Append(", date_num");
                        sql.Append(groupByOptional);

                    }

                    sql.Append("  order by  id_sector, id_subsector,  id_group_, id_advertiser, id_brand ");
                    if (_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)) sql.Append(", id_product");
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                        sql.Append(", ID_GROUP_ADVERTISING_AGENCY");
                    if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
                        sql.Append(", ID_ADVERTISING_AGENCY");
                    sql.Append(", date_num");

                }

            }
            catch (System.Exception err)
            {
                throw (new DynamicDALException("Unable to build request for synthesis report : " + sql, err));
            }
            #endregion

            #region Execution de la requ�te
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

            #region Construction de la requ�te
            sql.Append(" select id_media, ");
            if (isCategoryRequired) sql.AppendFormat("{0}.", tblCategory.Prefix);
            sql.AppendFormat("{0} as columnDetailLevel", columnDetailLevel.GetSqlFieldIdWithoutTablePrefix());
            sql.AppendFormat(" from {0}", tblMedia.SqlWithPrefix);
            if (isCategoryRequired)
            {
                sql.AppendFormat(", {0}, {1}", tblBasicMedia.SqlWithPrefix, tblCategory.SqlWithPrefix);
            }

            #region S�lection de M�dias
            while (_session.CompetitorUniversMedia[positionUnivers] != null)
            {
                mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustom.Right.type.mediaAccess) + ",";
                positionUnivers++;
            }

            sql.AppendFormat(" where id_media in ({0})", mediaList.Substring(0, mediaList.Length - 1));
            sql.AppendFormat(" and {0}.id_language = {1}", tblMedia.Prefix, _session.DataLanguage);
            #endregion

            if (isCategoryRequired)
            {
                sql.AppendFormat(" and {0}.id_basic_media = {1}.id_basic_media", tblBasicMedia.Prefix, tblMedia.Prefix);
                sql.AppendFormat(" and {0}.id_category = {1}.id_category", tblBasicMedia.Prefix, tblCategory.Prefix);
                sql.AppendFormat(" and {0}.id_language = {1}", tblBasicMedia.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.id_language = {1}", tblCategory.Prefix, _session.DataLanguage);
            }
            #endregion

            #region Execution de la requ�te
            try
            {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new DynamicDALException("Unable to load the list of media for the column details : " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #region GetNbParutionData
        /// <summary>
        /// Get Number of parution data
        /// </summary>
        /// <returns> Number of parution data</returns>
        public DataSet GetNbParutionData()
        {

            StringBuilder sql = new StringBuilder();
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;


            #region Construction de la requete

            try
            {

                sql.Append("  select id_media, count(distinct date_num ) as NbParution, yearParution ");
                sql.Append("  from ( ");

                //Get sub query for principal period
                sql.Append(GetNbParutionRequest(customerPeriod.StartDate, customerPeriod.EndDate, 1));

                sql.Append(" union all");

                //Get sub query for compative period
                sql.Append(GetNbParutionRequest(customerPeriod.ComparativeStartDate, customerPeriod.ComparativeEndDate, 2));

                sql.Append("   )  group by id_media, yearParution ");
                sql.Append("   order by id_media, yearParution");

            }
            catch (System.Exception err)
            {
                throw (new DynamicDALException("Error while building request for dynamic report " + sql, err));
            }
            #endregion

            #region Execution de la requ�te
            try
            {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new DynamicDALException("Error while executing request for dynamic report ", err));
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
            string unitFieldSumWithAlias = "";
            StringBuilder sql = new StringBuilder();
            string dataJointForInsert = "";

            string universFilter = string.Empty;
            string fromOptional = string.Empty;
            string joinOptional = string.Empty;
            string groupOptional = string.Empty;


            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0];
            Schema schAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
            Table tblWebPlan = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData);
            #endregion

            #region Construction de la requ�te
            try
            {
                // Table de donn�es
                switch (type)
                {
                    case CstDB.TableType.Type.dataVehicle4M:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.alert);
                        break;
                    case CstDB.TableType.Type.dataVehicle:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.analysis);
                        break;
                    case CstDB.TableType.Type.webPlan:
                        //dataTableName = string.Format("{0}.{1} {2}", schAdExpr03.Label, CstDB.Tables.WEB_PLAN_MEDIA_MONTH, DATA_TABLE_PREFIXE);
                        dataTableName = tblWebPlan.SqlWithPrefix;
                        break;
                    default:
                        throw (new DynamicDALException("Unable to determ�ine the type of table to use."));
                }

                // Obtient les tables de la nomenclature
                productTableName = _session.GenericProductDetailLevel.GetSqlTables(schAdExpr03.Label);
                if (productTableName != null && productTableName.Length > 0) productTableName = "," + productTableName;
                // Obtient les champs de la nomenclature
                productFieldName = _session.GenericProductDetailLevel.GetSqlFields();
                // Obtient l'ordre des champs
                orderFieldName = _session.GenericProductDetailLevel.GetSqlOrderFields();
                // obtient la clause group by
                groupByFieldName = _session.GenericProductDetailLevel.GetSqlGroupByFields();
                // Obtient les jointures pour la nomenclature
                productJoinCondition = _session.GenericProductDetailLevel.GetSqlJoins(_session.DataLanguage, DATA_TABLE_PREFIXE);
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
                //Univers filter
                universFilter = GetUniversFilter(type, dateField, customerPeriod);
                // Unit�s
                if (_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack && _session.Unit == CstWeb.CustomerSessions.Unit.versionNb)
                {
                    UnitInformation u = _session.GetSelectedUnit();
                    if (type != CstDB.TableType.Type.webPlan)
                    {
                        unitFieldSumWithAlias = string.Format("to_char({0}) as {1}, {2}.rowid as lineId ", u.DatabaseField, u.Id.ToString(), u.DatabaseField, DATA_TABLE_PREFIXE);
                        groupOptional = string.Format(",{0} , {1}.rowid ", u.DatabaseField, DATA_TABLE_PREFIXE);
                    }
                    else
                    {
                        unitFieldSumWithAlias = string.Format("to_char(t2.COLUMN_VALUE) as {0}, {1}.rowid as lineId ", u.Id.ToString(), DATA_TABLE_PREFIXE);
                        fromOptional = string.Format(", table({0}.{1}) t2", DATA_TABLE_PREFIXE, u.DatabaseMultimediaField);
                        groupOptional = string.Format(",t2.COLUMN_VALUE , {0}.rowid ", DATA_TABLE_PREFIXE);
                    }
                }
                else
                {
                    unitFieldSumWithAlias = FctWeb.SQLGenerator.GetUnitFieldNameSumWithAlias(_session, type);
                }


                //option encarts (pour la presse)
                if (CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id)
                    dataJointForInsert = FctWeb.SQLGenerator.GetJointForInsertDetail(_session, DATA_TABLE_PREFIXE);
                if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                {
                    try
                    {
                        dataTableNameForGad = ", " + schAdExpr03.Sql + FctWeb.SQLGenerator.GetTablesForGad(_session) + " " + CstDB.Tables.GAD_PREFIXE;
                        dataFieldsForGad = ", " + FctWeb.SQLGenerator.GetFieldsAddressForGad();
                        dataJointForGad = "and " + FctWeb.SQLGenerator.GetJointForGad(DATA_TABLE_PREFIXE);
                    }
                    catch (SQLGeneratorException) { ;}
                }
            }
            catch (Exception e)
            {
                throw (new DynamicDALException("Unable to init parameters for request : " + e.Message));
            }

            sql.AppendFormat(" select {0}.id_media, {0}.{1} as columnDetailLevel, {2} {3}, {4} as date_num, {5}"
                , DATA_TABLE_PREFIXE
                , columnDetailLevel.GetSqlFieldIdWithoutTablePrefix()
                , productFieldName
                , dataFieldsForGad
                , dateField
                , unitFieldSumWithAlias);
            sql.AppendFormat(" from {0} {1} {2} {3} {4}"
                , mediaAgencyTable
                , dataTableName
                , productTableName
                , dataTableNameForGad
                , fromOptional);

            //Univers filter
            sql.AppendFormat(" {0} ", universFilter);

            // Jointures Produits
            sql.AppendFormat(" {0}", productJoinCondition);
            sql.AppendFormat(" {0}", dataJointForGad);
            sql.AppendFormat(" {0}", mediaAgencyJoins);
            sql.AppendFormat(" {0}", joinOptional);

            //Jointures encart
            if (CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id)
                sql.AppendFormat(" {0}", dataJointForInsert);



            // Group by
            sql.AppendFormat(" group by {0}.id_media, {0}.{1}, {2}, {3} {4} {5}"
                , DATA_TABLE_PREFIXE
                , columnDetailLevel.GetSqlFieldIdWithoutTablePrefix()
                , dateField
                , groupByFieldName
                , dataFieldsForGad
                , groupOptional);

            #endregion

            #region Execution de la requ�te
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
        protected string GetUniversFilter(CstDB.TableType.Type type, string dateField, CustomerPeriod customerPeriod)
        {
            StringBuilder sql = new StringBuilder();

            // Period
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
                    sql.AppendFormat(" where {0}", GetMonthPeriod(dateField, customerPeriod));
                    break;
            }


            //Medias selection
            int positionUnivers = 1;
            string mediaList = "";
            while (_session.CompetitorUniversMedia[positionUnivers] != null)
            {
                mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustom.Right.type.mediaAccess) + ",";
                positionUnivers++;
            }
            if (mediaList.Length > 0)
                sql.AppendFormat(" and {0}.id_media in ({1})",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                    mediaList.Substring(0, mediaList.Length - 1)
                );
            //Media Universe
            sql.Append(FctWeb.SQLGenerator.GetResultMediaUniverse(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

            // Product Selection
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

            // Excluded Products
            string listExcludeProduct = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);//;FctWeb.SQLGenerator.GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
            sql.Append(listExcludeProduct);

            // Media Rights
            string mediaRights = FctWeb.SQLGenerator.getAnalyseCustomerMediaRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            sql.AppendFormat(" {0}", mediaRights);
            // Product rights
            string productsRights = FctWeb.SQLGenerator.getAnalyseCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            sql.AppendFormat(" {0}", productsRights);

            return sql.ToString();
        }
        #endregion

        #region Get Synthesis Request
        /// <summary>
        /// Build request to load data about synthesis report
        /// </summary>
        /// <param name="type">Type of table to use</param>
        /// <returns>Code SQL</returns>
        protected string GetSynthesisRequest(CstDB.TableType.Type type)
        {

            #region Constantes
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables
            string dateField = "";
            string dataTableName = "";
            string unitFieldSumWithAlias = "";
            StringBuilder sql = new StringBuilder(3000);
            string dataJointForInsert = "";
            string universFilter = string.Empty;
            string fromOptional = string.Empty;
            string joinOptional = string.Empty;
            string groupOptional = string.Empty;

            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;

            Table tblWepPlan = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.monthData);
            Schema schAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);

            Table tblAdvertisingAgengy = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency);
            Table tblGroupAdvertisingAgengy = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.groupAdvertisingAgency);
            #endregion

            // Table de donn�es
            switch (type)
            {
                case CstDB.TableType.Type.dataVehicle4M:
                    dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.alert);
                    break;
                case CstDB.TableType.Type.dataVehicle:
                    dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.analysis);
                    break;
                case CstDB.TableType.Type.webPlan:
                    dataTableName = tblWepPlan.SqlWithPrefix;
                    break;
                default:
                    throw (new DynamicDALException("Unable to determine the type of table to use."));
            }
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
            //Univers filter
            universFilter = GetUniversFilter(type, dateField, customerPeriod);
            // Unit�s
            if (_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack && _session.Unit == CstWeb.CustomerSessions.Unit.versionNb)
            {
                UnitInformation u = _session.GetSelectedUnit();
                if (type != CstDB.TableType.Type.webPlan)
                {
                    unitFieldSumWithAlias = string.Format("{0}.stragg({2}) as {1}", schAdExpr03.Label, u.Id.ToString(), u.DatabaseField);
                }
                else
                {
                    unitFieldSumWithAlias = string.Format("{0}.stragg(t2.COLUMN_VALUE) as {1}", schAdExpr03.Label, u.Id.ToString());
                    fromOptional = string.Format(", table({0}.{1}) t2", DATA_TABLE_PREFIXE, u.DatabaseMultimediaField);
                }
            }
            else
            {
                unitFieldSumWithAlias = FctWeb.SQLGenerator.GetUnitFieldNameSumWithAlias(_session, type);
            }

            //option encarts (pour la presse)
            if (CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id)
                dataJointForInsert = FctWeb.SQLGenerator.GetJointForInsertDetail(_session, DATA_TABLE_PREFIXE);

            sql.AppendFormat("  select {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
            sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand, {0}.id_product", DATA_TABLE_PREFIXE);
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
            {
                sql.AppendFormat(",{0}.ID_GROUP_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
            }
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
            {
                sql.AppendFormat(",{0}.ID_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
            }
            sql.AppendFormat(",{0} as date_num, {1}", dateField, unitFieldSumWithAlias);

            sql.AppendFormat(" from {0}", dataTableName);
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
                sql.AppendFormat(",{0}.{1} {2}", schAdExpr03.Label, tblAdvertisingAgengy.Label, tblAdvertisingAgengy.Prefix);
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
                sql.AppendFormat(",{0}.{1} {2}", schAdExpr03.Label, tblGroupAdvertisingAgengy.Label, tblGroupAdvertisingAgengy.Prefix);
            sql.Append(fromOptional);
            //Where
            sql.AppendFormat(" {0} ", universFilter);
            //Jointures 
            if (CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id)
                sql.AppendFormat(" {0}", dataJointForInsert);

            //Jointures groupe agences/agences	
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
            {
                sql.AppendFormat(" and {0}.ID_ADVERTISING_AGENCY(+)={1}.ID_ADVERTISING_AGENCY ", tblAdvertisingAgengy.Prefix, DATA_TABLE_PREFIXE);
                sql.AppendFormat(" and {0}.id_language(+)={1}", tblAdvertisingAgengy.Prefix, _session.DataLanguage);
            }
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
            {
                sql.AppendFormat(" and {0}.ID_GROUP_ADVERTISING_AGENCY(+)={1}.ID_GROUP_ADVERTISING_AGENCY ", tblGroupAdvertisingAgengy.Prefix, DATA_TABLE_PREFIXE);
                sql.AppendFormat(" and {0}.id_language(+)={1}", tblGroupAdvertisingAgengy.Prefix, _session.DataLanguage);
            }
            sql.Append(joinOptional);
            // Group by			
            sql.AppendFormat("  group by {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
            sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand, {0}.id_product", DATA_TABLE_PREFIXE);
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.groupMediaAgency))
            {
                sql.AppendFormat(", {0}.ID_GROUP_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
            }
            if (_vehicleInformation.AllowedMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.agency))
            {
                sql.AppendFormat(",{0}.ID_ADVERTISING_AGENCY", DATA_TABLE_PREFIXE);
            }
            sql.AppendFormat(", {0}", dateField);
            sql.Append(groupOptional);

            #region Execution de la requ�te
            try
            {
                return sql.ToString();
            }
            catch (System.Exception err)
            {
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
        protected string GetDayPeriod(string dateField, CustomerPeriod customerPeriod)
        {

            string sql = string.Empty;
            string comparativeSql = string.Empty;

            if (customerPeriod.PeriodDayBegin.Count == 2)
            {
                sql = string.Format(" (({0} >= {1} and {0} <= {2}) or ({0} >= {3} and {0} <= {4}))"
                    , dateField
                    , customerPeriod.PeriodDayBegin[0]
                    , customerPeriod.PeriodDayEnd[0]
                    , customerPeriod.PeriodDayBegin[1]
                    , customerPeriod.PeriodDayEnd[1]);
            }
            else if (customerPeriod.PeriodDayBegin.Count == 1)
            {
                sql = string.Format(" ({0} >= {1} and {0} <= {2})"
                    , dateField
                    , customerPeriod.PeriodDayBegin[0]
                    , customerPeriod.PeriodDayEnd[0]
                );
            }

            if (customerPeriod.ComparativePeriodDayBegin.Count == 2)
            {
                comparativeSql = string.Format(" (({0} >= {1} and {0} <= {2}) or ({0} >= {3} and {0} <= {4}))"
                                    , dateField
                                    , customerPeriod.ComparativePeriodDayBegin[0]
                                    , customerPeriod.ComparativePeriodDayEnd[0]
                                    , customerPeriod.ComparativePeriodDayBegin[1]
                                    , customerPeriod.ComparativePeriodDayEnd[1]
                                 );
            }
            else if (customerPeriod.ComparativePeriodDayBegin.Count == 1)
            {
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
        protected string GetMonthPeriod(string dateField, CustomerPeriod customerPeriod)
        {

            string sql = string.Empty;
            string comparativeSql = string.Empty;

            if (customerPeriod.PeriodMonthBegin.Count == 1)
            {
                sql = string.Format(" ({0} >= {1} and {0} <= {2})"
                    , dateField
                    , customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6)
                    , customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6)
                );
            }

            if (customerPeriod.ComparativePeriodMonthBegin.Count == 1)
            {
                comparativeSql = string.Format(" ({0} >= {1} and {0} <= {2})"
                    , dateField
                    , customerPeriod.ComparativePeriodMonthBegin[0].ToString().Substring(0, 6)
                    , customerPeriod.ComparativePeriodMonthEnd[0].ToString().Substring(0, 6)
                );
            }

            if (sql.Length > 0 && comparativeSql.Length > 0)
                sql = string.Format("( ({0}) or ({1}) )", sql, comparativeSql);
            else if (comparativeSql.Length > 0)
                sql = comparativeSql;

            return sql;
        }
        #endregion

        #region Get Nb Parution Request
        /// <summary>
        /// Get Nb parution for period
        /// </summary>
        /// <param name="yearParutionIndex">Index of corresponding year (current or comparative)</param>
        /// <returns>Code sql</returns>
        protected string GetNbParutionRequest(string startDate, string endDate, int yearParutionIndex)
        {

            #region Constantes
            string DATA_TABLE_PREFIXE = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix;
            #endregion

            #region Variables
            string dataTableName = "";
            string dateField = "";
            string mediaList = "";
            StringBuilder sql = new StringBuilder();
            CustomerPeriod customerPeriod = _session.CustomerPeriodSelected;
            int positionUnivers = 1;
            #endregion
            try
            {
                dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.analysis);
                dateField = DATA_TABLE_PREFIXE + "." + CstDB.Fields.DATE_MEDIA_NUM;

                sql.Append(" select " + DATA_TABLE_PREFIXE + ".id_media, date_media_num as date_num, " + yearParutionIndex + " as yearParution");
                sql.Append(" from  " + dataTableName);
                sql.Append(" where " + dateField + ">=" + startDate + " and " + dateField + " <= " + endDate);

                #region S�lection de M�dias
                while (_session.CompetitorUniversMedia[positionUnivers] != null)
                {
                    mediaList += _session.GetSelection((TreeNode)_session.CompetitorUniversMedia[positionUnivers], CstCustom.Right.type.mediaAccess) + ",";
                    positionUnivers++;
                }
                if (mediaList.Length > 0) sql.Append(" and id_media in (" + mediaList.Substring(0, mediaList.Length - 1) + ")");
                #endregion

                // Group by
                sql.Append(" group by id_media, " + dateField);

            }
            catch (Exception e)
            {
                throw (new DynamicDALException("Unable to build Number of parution request for lost won : " + e.Message));
            }

            return sql.ToString();
        }
        #endregion

        /// <summary>
        /// Get excluded products
        /// </summary>
        /// <param name="sql">String builder</param>
        /// <returns></returns>
        protected virtual string GetExcludeProducts(string prefix)
        {
            // Exclude product 
            string sql = "";
            ProductItemsList prList = Product.GetItemsList(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID);
            if (prList != null)
                sql = prList.GetExcludeItemsSql(true, prefix);
            return sql;
        }
        #endregion

    }

}
