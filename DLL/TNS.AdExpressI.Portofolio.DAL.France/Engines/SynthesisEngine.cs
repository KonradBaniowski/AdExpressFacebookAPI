#region Information
// Author: Y. R'kaina
// Creation date: 29/08/2008
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

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
using TNS.AdExpress.Domain.Units;

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
                DateTime DateBegin = WebFunctions.Dates.getPeriodBeginningDate(_beginingDate, _webSession.PeriodType);
                if (DateBegin > DateTime.Now)
                    isAlertModule = true;
            }
            #endregion

            #region Get Rights
            try {
                table = GetTableData(isAlertModule);
                product = GetProductData();
                productsRights = WebFunctions.SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
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


    }
}
