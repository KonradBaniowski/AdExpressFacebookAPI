using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Engines;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Portofolio.DAL.Turkey.Engines
{
    public class BreakdownEngine : Engine
    {
        #region Variables
        private DetailLevelItemInformation _level;
        #endregion

        #region PortofolioDetailEngine
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        /// <param name="module">module</param>
        public BreakdownEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd, DetailLevelItemInformation level)
			: base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd) {
            _level = level;
        }
        #endregion

        #region Implementation abstract methods
        /// <summary>
        /// Get data for structure results
        /// </summary>
        /// <returns></returns>
        protected override DataSet ComputeData()
        {
            return GetBreakdownData();
        }
        #endregion

        #region Dataset for tv
        /// <summary>
        /// Get breakdown data 
        /// </summary>
        /// <remarks>Used for tv</remarks>		
        /// <returns>DataSet</returns>
        protected virtual DataSet GetBreakdownData()
        {

            #region variables
            var tableName = "";
            StringBuilder sql = new StringBuilder(2000);
            #endregion

            #region construction de la requête
            try
            {
                //Table name
                tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);

            }
            catch (Exception)
            {
                throw new PortofolioDALException("GetStructData : impossible to determine Table and Fields for the query.");
            }

            if (!string.IsNullOrEmpty(tableName))
            {

                //Fields
                var fields = GetBreakdownFields();

                // Select 
                sql.Append("  select " + fields);

                // Tables
                sql.Append(" from " + tableName + ", " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label +
                           "."+ _level.GetTableNameWithPrefix());

                //Where
                sql.Append("  where ");

                // Period conditions
                sql.Append("  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +
                           ".date_media_num >= " + _beginingDate);
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +
                           ".date_media_num <= " + _endDate);

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +
                           "." + _level.DataBaseIdField + " = " + _level.DataBaseTableNamePrefix + "." + _level.DataBaseIdField);


                #region Product Rights

                //Access Rights
                sql.Append(SQLGenerator.GetClassificationCustomerProductRight(_webSession,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true,
                    _module.ProductRightBranches));

                //list products hap
                var listProductHap =
                    GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                if (!string.IsNullOrEmpty(listProductHap))
                    sql.Append(listProductHap);

                //List selected products
                var product = GetProductData();
                if (!string.IsNullOrEmpty(product))
                    sql.Append(product);

                #endregion

                #region Nomenclature Media (Rights and selection)

                sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

                #region Media Rights

                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_webSession,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

                #endregion

                #region Media selection

                //Vehicle selection média (vehicle)

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +
                           ".id_vehicle = " + _vehicleInformation.DatabaseId.ToString());

                //Media selection	
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +
                           ".id_media = " + _idMedia.ToString());

                #endregion

                #endregion

                sql.Append(" group by " + _level.GetSqlField());
            }

            #endregion

            #region Query execution
            try
            {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (Exception err)
            {
                throw (new PortofolioDALException("Impossible to get data for GetStructData(int hourBegin, int hourEnd) : " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        #region Get Tv Breakdown Fields
        /// <summary>
        /// Get Tv Breakdown Fields
        /// </summary>		
        /// <returns>SQL</returns>
        protected virtual string GetBreakdownFields()
        {
            return _level.GetSqlField() + ", " + SQLGenerator.GetUnitFieldsNameForPortofolioMulti(_webSession, AdExpress.Constantes.DB.TableType.Type.dataVehicle, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
        }

        protected override long CountDataRows()
        {
            long nbRows = 0;
            #region variables
            var tableName = "";
            StringBuilder sql = new StringBuilder(2000);
            #endregion

            #region construction de la requête
            try
            {
                //Table name
                tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);

            }
            catch (Exception)
            {
                throw new PortofolioDALException("GetStructData : impossible to determine Table and Fields for the query.");
            }

            if (!string.IsNullOrEmpty(tableName))
            {

                //Fields
                var fields = GetBreakdownFields();

                // Select 
                sql.Append(" select count(*) as NbROWS from "); //start count
                sql.Append("  select " + fields);

                // Tables
                sql.Append(" from " + tableName + ", " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label +
                           "." + _level.GetTableNameWithPrefix());

                //Where
                sql.Append("  where ");

                // Period conditions
                sql.Append("  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +
                           ".date_media_num >= " + _beginingDate);
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +
                           ".date_media_num <= " + _endDate);

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +
                           "." + _level.DataBaseIdField + " = " + _level.DataBaseTableNamePrefix + "." + _level.DataBaseIdField);


                #region Product Rights

                //Access Rights
                sql.Append(SQLGenerator.GetClassificationCustomerProductRight(_webSession,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true,
                    _module.ProductRightBranches));

                //list products hap
                var listProductHap =
                    GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                if (!string.IsNullOrEmpty(listProductHap))
                    sql.Append(listProductHap);

                //List selected products
                var product = GetProductData();
                if (!string.IsNullOrEmpty(product))
                    sql.Append(product);

                #endregion

                #region Nomenclature Media (Rights and selection)

                sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

                #region Media Rights

                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_webSession,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

                #endregion

                #region Media selection

                //Vehicle selection média (vehicle)

                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +
                           ".id_vehicle = " + _vehicleInformation.DatabaseId.ToString());

                //Media selection	
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix +
                           ".id_media = " + _idMedia.ToString());

                #endregion

                #endregion

                sql.Append(" group by " + _level.GetSqlField());

                sql.Append(" ) "); //end count
            }

            #endregion

            #region Query execution
            try
            {
                var ds = _webSession.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 1)
                   nbRows = (Int64.Parse(ds.Tables[0].Rows[0]["NbROWS"].ToString()));
            }
            catch (Exception err)
            {
                throw (new PortofolioDALException("Impossible to get data for GetStructData(int hourBegin, int hourEnd) : " + sql.ToString(), err));
            }
            #endregion

            return nbRows;
        }

        #endregion
    }
}
