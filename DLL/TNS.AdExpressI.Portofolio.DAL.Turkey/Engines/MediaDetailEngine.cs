using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Portofolio.DAL.Turkey.Engines
{
    public class MediaDetailEngine : DAL.Engines.MediaDetailEngine
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="module">Module</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public MediaDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd)
        {
        }
        #endregion

        #region Implementation abstract methods
        /// <summary>
        /// Get data for media detail
        /// </summary>	
        /// <returns>Data set</returns>
        protected override DataSet ComputeData()
        {
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.tv:
                    return GetCommercialBreak();
                default: throw new PortofolioDALException("The method to get data is not defined for this vehicle.");
            }
        }
        #endregion

        #region GetCommercialBreak
        /// <summary>
        /// Get Commercial Break For Tv & Radio
        /// </summary>
        /// <returns>Liste des codes ecrans</returns>
        protected override DataSet GetCommercialBreak()
        {

            #region Variables
            string selectFields = "";
            string tableName = "";
            string groupByFields = "";
            string listProductHap = "";
            string product = "";
            string productsRights = "";
            string mediaRights = "";
            string sql = "";
            #endregion

            #region Build query

            try
            {
                selectFields = GetFieldsDetailMedia();
                tableName = SQLGenerator.GetVehicleTableNameForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
                groupByFields = GetGroupByDetailMedia();
                //listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
                listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                product = GetProductData();
                productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to init query parameters", err));
            }

            sql += "select " + selectFields;
            sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + " ";
            sql += " where id_media =" + _idMedia + "  ";
            sql += " and date_media_num>=" + _beginingDate + " ";
            sql += " and date_media_num<=" + _endDate + " ";
            sql += GetCobrandingCondition();
            sql += listProductHap;
            sql += product;
            sql += productsRights;
            sql += " " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            sql += mediaRights;
            sql += groupByFields;


            #endregion

            #region Execution query
            try
            {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to load data for detail media portoflolio : " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #region Get Fields Detail Media For Tv & Radio
        /// <summary>
        /// Get Fields Detail Media For Tv & Radio
        /// </summary>
        /// <returns>SQL</returns>
        protected override string GetFieldsDetailMedia()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(SQLGenerator.GetUnitFieldsNameForPortofolioMulti(_webSession, TNS.AdExpress.Constantes.DB.TableType.Type.dataVehicle4M));
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.tv:
                    sql.Append(" ,(CASE   WHEN TOP_DIFFUSION  between 020000 AND 025959   THEN '02H - 03H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 030000 AND 035959   THEN '03H - 04H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 040000 AND 045959   THEN '04H - 05H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 050000 AND 055959   THEN '05H - 06H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 060000 AND 065959   THEN '06H - 07H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 070000 AND 075959   THEN '07H - 08H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 080000 AND 085959   THEN '08H - 09H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 090000 AND 095959   THEN '09H - 10H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 100000 AND 105959   THEN '10H - 11H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 110000 AND 115959   THEN '11H - 12H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 120000 AND 125959   THEN '12H - 13H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 130000 AND 135959   THEN '13H - 14H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 140000 AND 145959   THEN '14H - 15H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 150000 AND 155959   THEN '15H - 16H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 160000 AND 165959   THEN '16H - 17H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 170000 AND 175959   THEN '17H - 18H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 180000 AND 185959   THEN '18H - 19H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 190000 AND 195959   THEN '19H - 20H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 200000 AND 205959   THEN '20H - 21H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 210000 AND 215959   THEN '21H - 22H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 220000 AND 225959   THEN '22H - 23H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 230000 AND 235959   THEN '23H - 24H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 240000 AND 245959   THEN '00H - 01H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 250000 AND 255959   THEN '01H - 02H'  ");
                    sql.Append("    END) TRANCHE_HORAIRE ");
                    sql.Append("    ,date_media_num   ");
                    return sql.ToString();
                default:
                    throw new PortofolioDALException("GetFieldsDetailMedia : Vehicle unknown.");
            }
        }
        #endregion

        #region Get Group By Detail Media For Tv & Radio
        /// <summary>
        /// Get Group By Detail Media for vehicles Tv & Radio
        /// </summary>
        /// <returns>SQL</returns>
        protected override string GetGroupByDetailMedia()
        {
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.tv:
                    StringBuilder sql = new StringBuilder();
                    sql.Append("   group by date_media_num , ");
                    sql.Append(" (CASE   WHEN TOP_DIFFUSION  between 020000 AND 025959   THEN '02H - 03H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 030000 AND 035959   THEN '03H - 04H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 040000 AND 045959   THEN '04H - 05H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 050000 AND 055959   THEN '05H - 06H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 060000 AND 065959   THEN '06H - 07H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 070000 AND 075959   THEN '07H - 08H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 080000 AND 085959   THEN '08H - 09H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 090000 AND 095959   THEN '09H - 10H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 100000 AND 105959   THEN '10H - 11H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 110000 AND 115959   THEN '11H - 12H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 120000 AND 125959   THEN '12H - 13H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 130000 AND 135959   THEN '13H - 14H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 140000 AND 145959   THEN '14H - 15H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 150000 AND 155959   THEN '15H - 16H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 160000 AND 165959   THEN '16H - 17H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 170000 AND 175959   THEN '17H - 18H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 180000 AND 185959   THEN '18H - 19H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 190000 AND 195959   THEN '19H - 20H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 200000 AND 205959   THEN '20H - 21H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 210000 AND 215959   THEN '21H - 22H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 220000 AND 225959   THEN '22H - 23H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 230000 AND 235959   THEN '23H - 24H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 240000 AND 245959   THEN '00H - 01H'  ");
                    sql.Append("    WHEN TOP_DIFFUSION between 250000 AND 255959   THEN '01H - 02H'  ");
                    sql.Append("    END) ");
                    sql.Append("    order by TRANCHE_HORAIRE ");
                    return sql.ToString();
                default:
                    throw new PortofolioDALException("GetGroupByDetailMediaForTvRadio()-->Vehicle unknown.");
            }
        }
        #endregion

        #region Get cobranding condition
        /// <summary>
        /// Get cobranding condition
        /// </summary>
        /// <returns>cobranding condition sql string</returns>
        protected override string GetCobrandingCondition()
        {
            string sql = "";
            if (UnitsInformation.List.ContainsKey(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.spot))
                sql += " and " + UnitsInformation.Get(TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.spot).DatabaseField + " =  " + _cobrandindConditionValue;
            return sql;
        }
        #endregion

        protected override long CountDataRows()
        {
            #region Variables
            string selectFields = "";
            string tableName = "";
            string groupByFields = "";
            string listProductHap = "";
            string product = "";
            string productsRights = "";
            string mediaRights = "";
            string sql = "";
            long nbRows = 0;
            #endregion

            #region Build query

            try
            {
                selectFields = GetFieldsDetailMedia();
                tableName = SQLGenerator.GetVehicleTableNameForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
                groupByFields = GetGroupByDetailMedia();
                //listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, false);
                listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                product = GetProductData();
                productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to init query parameters", err));
            }
            sql +=" select count(*) as NbROWS from "; //start count
            sql += "select " + selectFields;
            sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + "." + tableName + " " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + " ";
            sql += " where id_media =" + _idMedia + "  ";
            sql += " and date_media_num>=" + _beginingDate + " ";
            sql += " and date_media_num<=" + _endDate + " ";
            sql += GetCobrandingCondition();
            sql += listProductHap;
            sql += product;
            sql += productsRights;
            sql += " " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            sql += mediaRights;
            sql += groupByFields;

            sql += " ) "; //end count
            #endregion

            #region Execution query
            try
            {
                var ds = _webSession.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 1)
                    nbRows = (Int64.Parse(ds.Tables[0].Rows[0]["NbROWS"].ToString()));
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to load data for detail media portoflolio : " + sql.ToString(), err));
            }
            #endregion

            return nbRows;
        }
    }
}
