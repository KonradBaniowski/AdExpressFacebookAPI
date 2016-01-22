#region Information
// Author: Y. R'kaina
// Creation date: 29/08/2008
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web.Navigation;
using AbsctractDAL = TNS.AdExpressI.Portofolio.DAL.Engines;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.FrameWork.Results;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;

namespace TNS.AdExpressI.Portofolio.DAL.Finland.Engines {

    public class SynthesisEngine : AbsctractDAL.SynthesisEngine {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        /// <param name="module">Module</param>
        public SynthesisEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicle">Vehicle</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        /// <param name="module">Module</param>
        /// <param name="synthesisDataType">Synthesis data type</param>
        public SynthesisEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd, PortofolioSynthesis.dataType synthesisDataType)
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
            _synthesisDataType = synthesisDataType;
        }

        #endregion

        #region GetRequestForAnalysisPortofolio
        /// <summary>
        /// Build query to get total investment, nd advert, spot duration
        /// </summary>	
        /// <param name="type">Type table </param>
        /// <returns>Query string</returns>
        protected override string GetRequestForAnalysisPortofolio(DBConstantes.TableType.Type type) {

            #region Build Sql query
            CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;

            string table = string.Empty;
            //Data table			
            switch (type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                    table = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
                    break;
                case DBConstantes.TableType.Type.dataVehicle:
                    table = WebFunctions.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis, _webSession.IsSelectRetailerDisplay);
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    table = WebApplicationParameters.GetDataTable(TableIds.monthData, _webSession.IsSelectRetailerDisplay).SqlWithPrefix; //DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
                    break;
                default:
                    throw (new PortofolioDALException("Table type unknown"));
            }
            string product = GetProductData();
            string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            string productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
            //list product hap
			string listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
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

            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("select {0}", WebFunctions.SQLGenerator.GetUnitFieldsNameForPortofolio(_webSession, type));

            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                sql.AppendFormat(", {0} as date_num ", date);
            }
            sql.AppendFormat(" from {0} where id_media={1}", table, _idMedia);
            // Period
            switch (type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                    sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.StartDate, customerPeriod.EndDate);
                    break;
                case DBConstantes.TableType.Type.dataVehicle:
                    if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
                        sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.StartDate, customerPeriod.EndDate);
                    }
                    else if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
                        sql.AppendFormat(" and (({0}>={1} and {0}<={2}) or ({0}>={3} and {0}<={4}))"
                            , date
                            , customerPeriod.PeriodDayBegin[0]
                            , customerPeriod.PeriodDayEnd[0]
                            , customerPeriod.PeriodDayBegin[1]
                            , customerPeriod.PeriodDayEnd[1]);
                    }
                    else {
                        sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.PeriodDayBegin[0], customerPeriod.PeriodDayEnd[0]);
                    }
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    sql.AppendFormat(" and {0}>={1} and {0}<={2}"
                        , date
                        , customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6)
                        , customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6));
                    break;
            }

            sql.Append(product);
            sql.Append(productsRights);
            sql.Append(mediaRights);
            sql.Append(listProductHap);
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                sql.AppendFormat(" group by {0}", date);
            }
            #endregion

            return sql.ToString();

        }
        #endregion

        #region Get Period
        /// <summary>
        /// Get date of issue
        /// </summary>
        /// <returns>DataSet</returns>
        protected override DataSet GetPeriod() {

            #region Construction de la requ�te
            string table = GetTableData(true);
            string product = GetProductData();
            string productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
            string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            //liste des produit hap
            string listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);

            string sql;

            sql = "select  min(DATE_MEDIA_NUM) first_date, max(DATE_MEDIA_NUM) last_date";

            sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Sql + table + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + " ";
            sql += " where id_media = " + _idMedia + "";
            if (_beginingDate.Length > 0)
                sql += " and date_media_num>=" + _beginingDate + " ";
            if (_endDate.Length > 0)
                sql += " and date_media_num<=" + _endDate + "";

            sql += product;
            sql += productsRights;
            sql += GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            sql += mediaRights;
            sql += listProductHap;
            #endregion

            #region Execution de la requ�te
            try {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetPeriod(): " + sql, err));
            }
            #endregion

        }
        #endregion

    }
}
