using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using AbsctractDAL = TNS.AdExpressI.Portofolio.DAL.Engines;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpressI.Portofolio.DAL.Turkey.Engines
{
    public class SynthesisEngine : AbsctractDAL.SynthesisEngine
    {
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
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd)
        {
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
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd)
        {
            _synthesisDataType = synthesisDataType;
        }

        #endregion

        #region Ecran
        /// <summary>
        /// récupère les écrans
        /// </summary>
        /// <returns>Ecrans</returns>
        public override DataSet GetEcranData()
        {

            #region Variables
            string select = "";
            string table = "";
            string product = "";
            string productsRights = "";
            string mediaRights = "";
            string listProductHap = "";
            StringBuilder sql = new StringBuilder();
            UnitInformation unitInformation = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.insertion);
            #endregion

            #region Construction de la requête
            try
            {
                select = GetSelectDataEcran();
                table = GetTableData(true);
                product = GetProductData();
                productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to build the request for GetEcranData()", err));
            }

            sql.AppendFormat("select sum({0}) as {0},sum(ecran_duration) as ecran_duration ,sum(nbre_spot) as nbre_spot"
                , unitInformation.Id.ToString());

            sql.Append(" from ( ");

            sql.Append(select);
            sql.AppendFormat(" from {0}.{1} wp ", WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, table);
            sql.AppendFormat(" where id_media={0} ", _idMedia);
            if (_beginingDate.Length > 0)
                sql.AppendFormat(" and date_media_num>={0} ", _beginingDate);
            if (_endDate.Length > 0)
                sql.AppendFormat(" and date_media_num<={0} ", _endDate);

            sql.AppendFormat(" and id_spot_pos_com_break_type = 1 ");

            sql.AppendFormat(" and {0}={1}"
                            , unitInformation.DatabaseField
                            , this._cobrandindConditionValue);

            sql.Append(product);
            sql.Append(productsRights);
            sql.Append(mediaRights);
            sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(listProductHap);
            sql.Append(" )");

            #endregion

            #region Execution de la requête
            try
            {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to get data for GetEcranData() : " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #region Custom Ecran
        /// <summary>
        /// récupère les écrans
        /// </summary>
        /// <returns>Ecrans</returns>
        public DataSet GetCustomEcranData()
        {

            #region Variables
            string select = "";
            string table = "";
            string product = "";
            string productsRights = "";
            string mediaRights = "";
            string listProductHap = "";
            StringBuilder sql = new StringBuilder();
            UnitInformation unitInformation = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.insertion);
            #endregion

            #region Construction de la requête
            try
            {
                select = GetSelectDataEcran();
                table = GetTableData(true);
                product = GetProductData();
                productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to build the request for GetEcranData()", err));
            }

            sql.AppendFormat("select sum({0}) as {0},sum(ecran_duration) as ecran_duration ,sum(nbre_spot) as nbre_spot"
                , unitInformation.Id.ToString());

            sql.Append(" from ( ");

            sql.Append(select);
            sql.AppendFormat(" from {0}.{1} wp ", WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, table);
            sql.AppendFormat(" where id_media={0} ", _idMedia);
            if (_beginingDate.Length > 0)
                sql.AppendFormat(" and date_media_num>={0} ", _beginingDate);
            if (_endDate.Length > 0)
                sql.AppendFormat(" and date_media_num<={0} ", _endDate);

            sql.AppendFormat(" and {0}={1}"
                            , unitInformation.DatabaseField
                            , this._cobrandindConditionValue);

            sql.AppendFormat(" and id_spot_pos_com_break_type = 1 ");

            sql.Append(product);
            sql.Append(productsRights);
            sql.Append(mediaRights);
            sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(listProductHap);
            sql.Append(" )");

            #endregion

            #region Execution de la requête
            try
            {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to get data for GetEcranData() : " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #region Get Commercial Item Nuumber
        /// <summary>
        /// Get Commercial Item Nuumber
        /// </summary>
        /// <returns>Commercial Item Nuumber</returns>
        public override DataSet GetCommercialItemNumber()
        {
            #region Variables
            CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
            string table = string.Empty;
            #endregion

            #region Build Sql query
            //Data table			
            if (customerPeriod.IsSliding4M)
            {
                table = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
            }
            else
            {
                table = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.analysis, _webSession.IsSelectRetailerDisplay);
            }
            string product = GetProductData();
            string mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            string productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
            //list product hap
            string listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            string date = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + DBConstantes.Fields.DATE_MEDIA_NUM;

            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("select SUM(COM_ITEM_NB) as COM_ITEM_NB from ( ");
            sql.AppendFormat("select DATE_MEDIA_NUM, ID_PROGRAM, SUM(COM_ITEM_NB) as COM_ITEM_NB ");
            sql.AppendFormat(" from {0} where id_media={1}", table, _idMedia);

            // Period
            sql.AppendFormat(" and {0}>={1} and {0}<={2}", date, customerPeriod.StartDate, customerPeriod.EndDate);

            sql.Append(product);
            sql.Append(productsRights);
            sql.Append(mediaRights);
            sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            sql.Append(listProductHap);
            sql.AppendFormat(" group by DATE_MEDIA_NUM, ID_PROGRAM )");
            #endregion

            #region Execution de la requête
            try
            {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to exectue query" + sql, err));
            }
            #endregion
        }
        #endregion
    }
}
