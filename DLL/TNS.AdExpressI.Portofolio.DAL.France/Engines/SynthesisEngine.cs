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
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.FrameWork.Results;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpressI.Portofolio.DAL.France.Engines {

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

        #region Get Spot Data
        /// <summary>
        /// Get information about spots
        /// </summary>
        /// <returns>Spot data</returns>
        public override DataSet GetSpotData() {

            #region Variables
            string select = "";
            string table = "";
            string product = "";
            string productsRights = "";
            string mediaRights = "";
            string listProductHap = "";
            var sql = new StringBuilder();
            UnitInformation unitInformation = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.insertion);
            bool isAlertModule = _webSession.CustomerPeriodSelected.IsSliding4M;
            if (isAlertModule == false) {
                DateTime DateBegin = Dates.getPeriodBeginningDate(_beginingDate, _webSession.PeriodType);
                if (DateBegin > DateTime.Now)
                    isAlertModule = true;
            }
            #endregion

            #region Get Rights
            try {
                table = GetTableData(isAlertModule);
                product = GetProductData();
                productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to build the request for GetSpotData()", err));
            }
            #endregion

            #region Get Data
            var dt = base.GetSpotData().Tables[0]; 
            #endregion

            #region Nb Spots
            sql = new StringBuilder();
            sql.Append("select sum(insertion) as sum_spot_nb_wap ");
            sql.AppendFormat(" from {0}.{1} wp ", WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, table);
            sql.AppendFormat(" where id_media={0} ", _idMedia);
            if (_beginingDate.Length > 0)
                sql.AppendFormat(" and date_media_num>={0} ", _beginingDate);
            if (_endDate.Length > 0)
                sql.AppendFormat(" and date_media_num<={0} ", _endDate);
            sql.Append(product);
            sql.Append(productsRights);
            sql.Append(mediaRights);
            sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(listProductHap);

            #region Execution de la requête

            DataTable dtNbSpots;
            try {
                dtNbSpots = _webSession.Source.Fill(sql.ToString()).Tables[0];
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetSpotData() : " + sql.ToString(), err));
            }
            #endregion

            #endregion

            #region Nb Ecran
            sql = new StringBuilder();
            sql.Append("select count(distinct commercial_break) as com_break_nb ");
            sql.AppendFormat(" from {0}.{1} wp ", WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, table);
            sql.AppendFormat(" where id_media={0} ", _idMedia);
            if (_beginingDate.Length > 0)
                sql.AppendFormat(" and date_media_num>={0} ", _beginingDate);
            if (_endDate.Length > 0)
                sql.AppendFormat(" and date_media_num<={0} ", _endDate);
            sql.Append(product);
            sql.Append(productsRights);
            sql.Append(mediaRights);
            sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(listProductHap);

            #region Execution de la requête
            DataTable dtNbEcran;
            try {
                dtNbEcran = _webSession.Source.Fill(sql.ToString()).Tables[0];
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Impossible to get data for GetSpotData() : " + sql.ToString(), err));
            }
            #endregion

            #endregion

            var dtRetour = new DataTable();

            if (dt.Rows.Count > 0 && dtNbSpots.Rows.Count > 0 && dtNbEcran.Rows.Count > 0)
            {
                dtRetour.Columns.Add("avg_spot_nb");
                dtRetour.Columns.Add("sum_spot_nb_wap");
                dtRetour.Columns.Add("avg_dur_com_break");
                dtRetour.Columns.Add("sum_dur_com_break");
                dtRetour.Columns.Add("com_break_nb");

                var param = new object[5];
                param[0] = (decimal.Parse(dtNbSpots.Rows[0][0].ToString())/
                            decimal.Parse(dtNbEcran.Rows[0][0].ToString()));
                param[1] = dtNbSpots.Rows[0][0];
                param[2] = dt.Rows[0]["avg_dur_com_break"];
                param[3] = dt.Rows[0]["sum_dur_com_break"];
                param[4] = dtNbEcran.Rows[0][0];

                dtRetour.Rows.Add(param);
            }

            var dsRetour = new DataSet();
            dsRetour.Tables.Add(dtRetour);

            return dsRetour;

        }
        #endregion

        #region GetRequestForNumberBanner (Evaliant)
        /// <summary>
        /// Build query to get to number of banners
        /// </summary>
        /// <param name="type">Type table</param>
        /// <returns>Query string</returns>
        protected override string GetRequestForNumberBanner(DBConstantes.TableType.Type type)
        {

            #region Build Sql query
            CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;

            string table = string.Empty;
            //Data table			
            switch (type)
            {
                case DBConstantes.TableType.Type.dataVehicle4M:
                    table = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
                    break;
                case DBConstantes.TableType.Type.dataVehicle:
                    table = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis, _webSession.IsSelectRetailerDisplay);
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    table = WebApplicationParameters.GetDataTable(TableIds.monthData, _webSession.IsSelectRetailerDisplay).SqlWithPrefix;
                    break;
                default:
                    throw (new PortofolioDALException("Table type unknown"));
            }
            string product = GetProductData();
            string mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            string productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
            //list product hap
            string listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            string date = string.Empty;

            switch (type)
            {
                case DBConstantes.TableType.Type.dataVehicle4M:
                case DBConstantes.TableType.Type.dataVehicle:
                    date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                    break;
            }

            var sql = new StringBuilder();

            if (type == DBConstantes.TableType.Type.webPlan)
            {
                sql.Append("select distinct list_banners as hashcode ");
            }
            else
            {

                sql.Append("select distinct to_char(hashcode) as hashcode ");
            }

            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
            {
                sql.AppendFormat(", {0} as date_num ", date);
            }
            sql.AppendFormat(" from {0} where id_media={1}", table, _idMedia);


            // Autopromo
            string idMediaLabel = string.Empty;

            if (_vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack 
                || _vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile)
                idMediaLabel = "id_media_evaliant";
            else if (_vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.mms)
                idMediaLabel = "id_media_mms";

            if (_vehicleInformation.Autopromo && (_vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack
                || _vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile
                || _vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.mms))
            {

                Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                    sql.Append(" and auto_promotion = 0 ");
                else if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany)
                {
                    sql.Append(" and (id_media, id_holding_company) not in ( ");
                    sql.Append(" select distinct " + idMediaLabel + ", id_holding_company ");
                    sql.Append(" from " + tblAutoPromo.Sql + " ");
                    sql.Append(" where " + idMediaLabel + " is not null ");
                    sql.Append(" ) ");
                }
            }

            sql.Append(GetFormatClause(null));
            sql.Append(GetPurchaseModeClause(null));

            // Period
            switch (type)
            {
                case DBConstantes.TableType.Type.dataVehicle4M:
                    sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.StartDate, customerPeriod.EndDate);
                    break;
                case DBConstantes.TableType.Type.dataVehicle:
                    if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0)
                    {
                        sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.StartDate, customerPeriod.EndDate);
                    }
                    else if (_webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2)
                    {
                        sql.AppendFormat(" and (({0}>={1} and {0}<={2}) or ({0}>={3} and {0}<={4}))"
                            , date
                            , customerPeriod.PeriodDayBegin[0]
                            , customerPeriod.PeriodDayEnd[0]
                            , customerPeriod.PeriodDayBegin[1]
                            , customerPeriod.PeriodDayEnd[1]);
                    }
                    else
                    {
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
            sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(listProductHap);
            sql.Append(" group by ");
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
            {
                sql.AppendFormat(" {0},", date);
            }
            if (type == DBConstantes.TableType.Type.webPlan)
            {
                sql.Append("list_banners");
            }
            else
            {
                sql.Append("hashcode");
            }

            #endregion

            return sql.ToString();
        }
        #endregion


    }
}
