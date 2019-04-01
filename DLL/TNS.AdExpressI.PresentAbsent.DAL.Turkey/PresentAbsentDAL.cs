using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.PresentAbsentDAL.Exceptions;

namespace TNS.AdExpressI.PresentAbsent.DAL.Turkey
{
    public class PresentAbsentDAL : DAL.PresentAbsentDAL
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        public PresentAbsentDAL(WebSession session) : base(session){ }
        #endregion

        #region GetSynthesisData
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
        protected override string GetSynthesisRequest(AdExpress.Constantes.DB.TableType.Type type)
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
            bool showProduct = _session.CustomerLogin.CustormerFlagAccess(AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            #endregion

            #region Tables
            //Get SQL tables to use in the query
            Table tblWebPlan = WebApplicationParameters.GetDataTable(TableIds.monthData, _session.IsSelectRetailerDisplay);
            Schema schAdExpress = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
         
            #endregion

            #region Construction de la requête
            try
            {
                // Get table name according to the table type parameter
                switch (type)
                {
                    case AdExpress.Constantes.DB.TableType.Type.dataVehicle4M:
                        dataTableName = AdExpress.Web.Core.Utilities.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, AdExpress.Constantes.Web.Module.Type.alert, _session.IsSelectRetailerDisplay);
                        break;
                    case AdExpress.Constantes.DB.TableType.Type.dataVehicle:
                        dataTableName = AdExpress.Web.Core.Utilities.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, AdExpress.Constantes.Web.Module.Type.analysis, _session.IsSelectRetailerDisplay);
                        break;
                    case AdExpress.Constantes.DB.TableType.Type.webPlan:
                        dataTableName = tblWebPlan.SqlWithPrefix;
                        break;
                    default:
                        throw (new PresentAbsentDALException("Unable to determine type of table to use."));
                }
                // Get Date field according to the target data table. 
                switch (type)
                {
                    case AdExpress.Constantes.DB.TableType.Type.dataVehicle4M:
                    case AdExpress.Constantes.DB.TableType.Type.dataVehicle:
                        dateField = DATA_TABLE_PREFIXE + "." + AdExpress.Constantes.DB.Fields.DATE_MEDIA_NUM;
                        break;
                    case AdExpress.Constantes.DB.TableType.Type.webPlan:
                        dateField = DATA_TABLE_PREFIXE + "." + AdExpress.Constantes.DB.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                        break;
                }

                //Get Universes filters
                universFilter = GetUniversFilter(type, dateField, customerPeriod);

                // get units fields according to customer unit selection (ex. EURO)
                unitFieldNameSumWithAlias = AdExpress.Web.Core.Utilities.SQLGenerator.GetUnitFieldNameSumWithAlias(_session, type);

               
            }
            catch (Exception e)
            {
                throw (new PresentAbsentDALException("Unable to init request parameters.", e));
            }
            UnitInformation u = _session.GetSelectedUnit();

            /*SELECT clause*/

            //Get fields of the level of product classification
            sql.AppendFormat("  select {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
            sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand", DATA_TABLE_PREFIXE);
            if (showProduct) sql.AppendFormat(", {0}.id_product", DATA_TABLE_PREFIXE);

          

            //Get vehicle identifiers and date fields, according to the target data table's date format (monthly, wekly, per day).	
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
            {
                sql.AppendFormat(",{0}.id_media, {1} as date_num, {2}", DATA_TABLE_PREFIXE, dateField, unitFieldNameSumWithAlias);
            }
            else
            {
                sql.AppendFormat(",{0}.id_media, {1}", DATA_TABLE_PREFIXE, unitFieldNameSumWithAlias);
            }

            /*FROM clause detrmines the different data tab to use in the query*/
            sql.AppendFormat(" from {0}", dataTableName);
          
            sql.AppendFormat(" {0} ", fromOptional);

            /*WHERE clause detrmines the filter condition of the query*/
            //sql.AppendFormat(" WHERE {0}", dataTableName);
            sql.AppendFormat(" {0} ", universFilter);


            //Filter INSET option
            if (AdExpress.Constantes.Classification.DB.Vehicles.names.press == _vehicleInformation.Id || AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress == _vehicleInformation.Id
                 || _vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.newspaper
                || _vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.magazine)
                sql.AppendFormat(" {0}", dataJointForInsert);

          
            sql.AppendFormat(" {0} ", joinOptional);


            sql.Append(GetFormatClause(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(GetPurchaseModeClause(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

            /*GROUP BY clause */
            sql.AppendFormat("  group by {0}.id_sector,{0}.id_subsector, {0}.id_group_", DATA_TABLE_PREFIXE);
            sql.AppendFormat(", {0}.id_advertiser,{0}.id_brand", DATA_TABLE_PREFIXE);
            if (showProduct) sql.AppendFormat(", {0}.id_product", DATA_TABLE_PREFIXE);
          
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
            {
                sql.AppendFormat(",{0}.id_media, {1} ", DATA_TABLE_PREFIXE, dateField);
            }
            else
            {
                sql.AppendFormat(",{0}.id_media ", DATA_TABLE_PREFIXE);
            }
            //Add unit number of version's field
            if (u.Id == AdExpress.Constantes.Web.CustomerSessions.Unit.versionNb)
            {
                groupByOptional += string.Format(", {0}.{1}", DATA_TABLE_PREFIXE, AdExpress.Web.Core.Utilities.SQLGenerator.GetUnitFieldName(_session, type));
            }
            sql.Append(groupByOptional);
            #endregion

            #region Execution of the query
            try
            {
                return sql.ToString();
            }
            catch (System.Exception err)
            {
                throw (new PresentAbsentDALException("Unable to build request for synthesis data loading : " + sql, err));
            }
            #endregion
        }
        #endregion
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
        public override DataSet GetSynthesisData()
        {

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
            bool showProduct = _session.CustomerLogin.CustormerFlagAccess(AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

            #endregion

            #region Building SQL query
            try
            {
                //If selected period is included in the Last 4 months
                if (customerPeriod.Is4M)
                {

                    //Get query string
                    sql4M.Append(GetSynthesisRequest(AdExpress.Constantes.DB.TableType.Type.dataVehicle4M));

                    //Order clause with product classification's levels
                    sql4M.Append("  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand ");
                    if (showProduct) sql.Append(",id_product");

                  

                    //Add vehicle identiifer field
                    sql4M.Append(", id_media");

                    sql = sql4M;
                }
                //If query not concerns data per day and agrated data by month
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan)
                {
                    sql.Append(GetSynthesisRequest(AdExpress.Constantes.DB.TableType.Type.dataVehicle));
                    sql.Append("  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand");
                    if (showProduct) sql.Append(",id_product");                 
                    sql4M.Append(", id_media");
                }
                else
                {
                    //Query concerns data detailed per day
                    if (customerPeriod.IsDataVehicle)
                    {
                        sqlDataVehicle.Append(GetSynthesisRequest(AdExpress.Constantes.DB.TableType.Type.dataVehicle));
                        sql = sqlDataVehicle;
                    }
                    //Query concerns data agregated by month
                    if (customerPeriod.IsWebPlan)
                    {
                        sqlWebPlan.Append(GetSynthesisRequest(AdExpress.Constantes.DB.TableType.Type.webPlan));
                        sql = sqlWebPlan;
                    }
                    //Query concerns data detailed per day and also data agregated by month
                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    {
                        sql = new StringBuilder();
                        sql.Append("  select id_sector, id_subsector, id_group_, id_advertiser, id_brand");
                        if (showProduct) sql.Append(",id_product");                       
                        sql.Append(" , id_media");

                        sql.AppendFormat(", {0}", AdExpress.Web.Core.Utilities.SQLGenerator.GetUnitFieldNameSumUnionWithAlias(_session));
                        if ((_vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack
                            || _vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile) && _session.Unit == AdExpress.Constantes.Web.CustomerSessions.Unit.versionNb)
                        {
                            groupByOptional = string.Format(", {0}", AdExpress.Web.Core.Utilities.SQLGenerator.GetUnitAlias(_session));
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
                        sql.Append(" , id_media");
                        sql.Append(groupByOptional);

                    }
                    //ORDER BY Clause
                    sql.Append("  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand");
                    if (showProduct) sql.Append(",id_product");                  
                    sql.Append(" , id_media");
                }
            }
            catch (System.Exception err)
            {
                throw (new PresentAbsentDALException("Unable to build request to get synthesis data : " + sql, err));
            }
            #endregion

            #region Execution of the query
            try
            {
                return _session.Source.Fill(sql.ToString());//.Tables[0];
            }
            catch (System.Exception err)
            {
                throw (new PresentAbsentDALException("Unable to load synthesis data : " + sql, err));
            }

            #endregion

        }
        #endregion

        #region Get Fields For Gad Without Table Prefix
        /// <summary>
        /// Get Fields For Gad Without Table Prefix
        /// </summary>
        /// <returns></returns>
        protected override string GetFieldsForGadWithoutTablePrefix()
        {
            return "";
        }
        #endregion

        #region Init Gad Params
        /// <summary>
        /// Init Gad Params
        /// </summary>
        protected override void InitGadParams(Table tblGad, ref string dataTableNameForGad, ref string dataFieldsForGad, ref string dataJointForGad)
        {
            dataTableNameForGad = "";
            dataFieldsForGad = "";
            dataJointForGad = "";
        }
        #endregion
    }
}
