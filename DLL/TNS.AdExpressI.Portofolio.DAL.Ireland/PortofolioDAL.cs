using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Portofolio.DAL.Ireland {
    /// <summary>
    /// Ireland Portofolio DAL class
    /// </summary>
    public class PortofolioDAL : TNS.AdExpressI.Portofolio.DAL.PortofolioDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="idMedia">Id media</param>
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate,
            string endDate)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate) {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="adBreak">Screen code</param>
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate,
            string endDate, string adBreak)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate, adBreak) {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="hour Beginning List">hour Beginning List</param>
        /// <param name="hourEndList">hour EndList</param>		
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate,
            string endDate, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate, hourBeginningList, hourEndList) {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="ventilationTypeList">ventilation Type List</param>	
        public PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate,
            string endDate, List<PortofolioStructure.Ventilation> ventilationTypeList)
            : base(webSession, vehicleInformation, idMedia, beginingDate, endDate, ventilationTypeList) {

        }
        #endregion

        #region Get dates list
        /// <summary>
        /// Get dates list
        /// </summary>
        /// <param name="conditionEndDate">Add condition end date</param>
        /// <returns>DataSet</returns>
        public override DataSet GetListDate(bool conditionEndDate, TableType.Type tableType) {

            string tableName;
            try {
                switch (tableType) {
                    case TableType.Type.dataVehicle4M:
                        tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id,
                            WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
                        break;
                    case TableType.Type.dataVehicle:
                        tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id,
                            WebConstantes.Module.Type.analysis, _webSession.IsSelectRetailerDisplay);
                        break;
                    default:
                        throw (new PortofolioDALException("Table type unknown"));
                }
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("Error when getting table name", err));
            }

            #region Construction de la requête
            var sql = new StringBuilder(500);

            sql.Append(" select distinct date_media_num ");

            if (_vehicleInformation.Id == Vehicles.names.press) {
                sql.Append(", date_media_num as date_cover_num");
            }
            sql.Append(", media ");
            sql.Append(" from ");
            sql.Append(tableName);


            sql.AppendFormat(",{0} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).SqlWithPrefix);
            sql.AppendFormat(" where {0}.id_media={1}.id_media" + " ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix);
            sql.AppendFormat(" and {0}.id_media={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _idMedia);
            sql.AppendFormat(" and {0}.id_language={1} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix,
                _webSession.DataLanguage);

            // Période			

            if (_beginingDate.Length > 0)
                sql.AppendFormat(" and {0}.date_media_num>={1}",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _beginingDate);
            if (_endDate.Length > 0 && conditionEndDate)
                sql.AppendFormat(" and {0}.date_media_num<={1}",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _endDate);


            // Tri			
            sql.AppendFormat(" order by {0}.date_media_num desc",
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

            #endregion

            #region Execution de la requête
            try {
                return (_webSession.Source.Fill(sql.ToString()));
            }
            catch (Exception err) {
                throw (new PortofolioDALException("Erreur lors de la sélection de la table", err));
            }
            #endregion

        }
        #endregion        

        #region Investment By Media
        /// <summary>
        /// Get Investment By Media
        /// </summary>
        /// <returns>Data</returns>
        public override Hashtable GetInvestmentByMedia() {

            #region Variables
            string sql = "";
            var htInvestment = new Hashtable();
            string product = null;
            string productsRights = null;
            string mediaRights = null;
            string listProductHap = null;
            string euroFieldNameSumWithAlias = null;
            string insertionFieldNameSumWithAlias = null;
            #endregion

            try {
                product = GetProductData();
                productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                euroFieldNameSumWithAlias = string.Format("sum({0}) as {1}",
                    UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField,
                    UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString());
                insertionFieldNameSumWithAlias = string.Format("sum({0}) as {1}",
                    UnitsInformation.List[CustomerSessions.Unit.insertion].DatabaseField,
                    UnitsInformation.List[CustomerSessions.Unit.insertion].Id.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException(string.Format("Impossible to build the request for GetInvestmentByMedia() : {0}",
                    sql), err));
            }

            #region Construction de la requête

            sql += string.Format(" select {0},{1},date_media_num date1",
                insertionFieldNameSumWithAlias, euroFieldNameSumWithAlias);
            sql += string.Format("  from {0}  {1}",
                WebApplicationParameters.GetDataTable(TableIds.dataPress,
                _webSession.IsSelectRetailerDisplay).Sql,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

            sql += string.Format(" where id_media={0} ", _idMedia);
            if (_beginingDate.Length > 0)
                sql += string.Format(" and date_media_num>={0} ", _beginingDate);
            if (_endDate.Length > 0)
                sql += string.Format("  and date_media_num<={0} ", _endDate);
            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += listProductHap;
            sql += " group by date_media_num ";
            #endregion

            #region Execution de la requête
            try {
                var ds = _webSession.Source.Fill(sql);
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) {
                    foreach (DataRow current in ds.Tables[0].Rows) {
                        var value1 = new string[2];
                        value1[0] = current[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString();
                        value1[1] = current[UnitsInformation.List[CustomerSessions.Unit.insertion].Id.ToString()].ToString();
                        htInvestment.Add(current["date1"], value1);
                    }
                }
            }
            catch (System.Exception err) {
                throw (new PortofolioDALException("error when getting investment by media", err));
            }
            #endregion

            return (htInvestment);
        }
        #endregion

        /// <summary>
        /// Get Inssertion data
        /// </summary>
        /// <returns></returns>
        public override DataSet GetInsertionData() {
            var res = new Engines.InsertionDetailEngine(_webSession,
                _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, _adBreak);
            return res.GetData();
        }

    }
}
