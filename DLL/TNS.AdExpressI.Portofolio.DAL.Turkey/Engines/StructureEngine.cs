using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Portofolio.DAL.Turkey.Engines
{
    public class StructureEngine : DAL.Engines.StructureEngine
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
        /// <param name="hourBeginningList">Hour beginning list</param>
        /// <param name="hourEndList">Hour end list</param>
        public StructureEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList)
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd, hourBeginningList, hourEndList)
        {
        }
        #endregion

        protected override string GetUnitFieldsNameForPortofolio()
        {
            return SQLGenerator.GetUnitFieldsNameForPortofolioMulti(_webSession, DBConstantes.TableType.Type.dataVehicle, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
        }

        #region Get Hour Interval
        /// <summary>
        /// Get Hour Interval
        /// </summary>
        /// <param name="hourBegin">Hour Begin</param>
        /// <param name="hourEnd">Hour End</param>
        /// <returns>String SQL</returns>
        protected override string GetHourInterval(double hourBegin, double hourEnd)
        {
            string sql = "";
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.tv:
                    if (hourBegin == 240000)
                    {
                        sql += " and ((" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion>= " + hourBegin;
                        sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion<= 255959)";
                        sql += " or (" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion>= 20000";
                        sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion< " + hourEnd + "))";
                    }
                    else
                    {
                        sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion>=" + hourBegin;
                        sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion<" + hourEnd;
                    }
                    return sql;
                default:
                    throw new PortofolioDALException("GetHourInterval : Vehicle unknown.");
            }
        }
        #endregion

        #region Dataset for tv or radio
        /// <summary>
        /// Get structure data 
        /// </summary>
        /// <remarks>Used for tv or radio</remarks>		
        /// <returns>DataSet</returns>
        public string  CountStructDataQuery()
        {

            #region variables
            string tableName = "";
            string fields = "";
            StringBuilder sql = new StringBuilder(2000);
            string product = "";
            double hourBegin;
            double hourEnd;
            bool start = true;
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
                foreach (KeyValuePair<string, double> kpv in _hourBeginningList)
                {
                    if (!start) sql.Append("  union all ");

                    //Fields
                    fields = GetStructFields(kpv.Key);

                    // Select 
                    sql.Append("  select " + fields);

                    // Tables
                    sql.Append(" from " + tableName + " ");

                    //Where
                    sql.Append("  where ");

                    // Period conditions
                    sql.Append("  " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num >= " + _beginingDate);
                    sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_media_num <= " + _endDate);

                    // Hour interval
                    hourBegin = kpv.Value;
                    hourEnd = _hourEndList[kpv.Key];
                    sql.Append(" " + GetHourInterval(hourBegin, hourEnd));

                    #region Product Rights

                    //Access Rights
                    sql.Append(SQLGenerator.GetClassificationCustomerProductRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches));

                    //list products hap
                    string listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    if (listProductHap != null && listProductHap.Length > 0)
                        sql.Append(listProductHap);

                    //List selected products
                    product = GetProductData();
                    if (product != null && product.Length > 0)
                        sql.Append(product);
                    #endregion

                    #region Nomenclature Media (Rights and selection)

                    sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

                    #region Media Rights
                    sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_webSession, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));
                    #endregion

                    #region Media selection
                    //Vehicle selection média (vehicle)

                    sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_vehicle = " + _vehicleInformation.DatabaseId.ToString());

                    //Media selection	
                    sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media = " + _idMedia.ToString());
                    #endregion

                    #endregion

                    start = false;
                }

            }
            #endregion

            return sql.ToString();
        }
        #endregion


        protected override long CountDataRows()
        {
            StringBuilder sql = new StringBuilder(2000);
            long nbRows = 0;

            sql.Append(" select count(*) as NbROWS from "); //start count
            sql.Append(CountStructDataQuery());
            sql.Append(" ) "); //end count
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

            return nbRows;
        }
    }
}
