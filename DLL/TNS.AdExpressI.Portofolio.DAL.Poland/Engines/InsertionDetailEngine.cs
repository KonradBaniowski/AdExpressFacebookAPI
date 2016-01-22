using System.Data;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpressI.Portofolio.DAL.Poland.Engines
{
    public class InsertionDetailEngine : DAL.Engines.InsertionDetailEngine
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public InsertionDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module,
            long idMedia, string periodBeginning, string periodEnd, string adBreak) :
            base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd, adBreak)
        {
        }

        #region ComputeData

        /// <summary>
        /// Get data for media detail insertion
        /// </summary>	
        /// <returns>liste des publicités pour un média donné</returns>
        protected override DataSet ComputeData()
        {

            #region Variables
            var sql = new StringBuilder(5000);
            bool allPeriod = string.IsNullOrEmpty(_adBreak);
            #endregion

            try
            {
                string sqlFields = _webSession.GenericInsertionColumns.GetSqlFields(null);
                string sqlGroupBy = _webSession.GenericInsertionColumns.GetSqlGroupByFields(null);
                string sqlConstraintFields = _webSession.GenericInsertionColumns.GetSqlConstraintFields();
                string tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert,
                    _webSession.IsSelectRetailerDisplay);
                string sqlTables = _webSession.GenericInsertionColumns.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, null);
                string sqlConstraintTables = _webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);

                //Select
                sql.Append(" select distinct");
                if (sqlFields.Length > 0) sql.AppendFormat(" {0}", sqlFields);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                {
                    sql.Append(" , advertising_agency");
                    sqlGroupBy += " , advertising_agency";
                }

                GetDateCoverField(sql, ref sqlGroupBy);

                if (sqlConstraintFields.Length > 0)
                {
                    sql.AppendFormat(" , {0}", sqlConstraintFields);//Fields for constraint management
                    sqlGroupBy += string.Format(" , {0}", _webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields());
                }

                sql.Append(" from ");
                sql.AppendFormat(" {0} ", tableName);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                    sql.Append(", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).SqlWithPrefix);

                if (sqlTables.Length > 0) sql.AppendFormat(" ,{0}", sqlTables);

                if (sqlConstraintTables.Length > 0)
                    sql.Append(" , " + sqlConstraintTables);//Tables pour la gestion des contraintes métiers

                // Joints conditions
                sql.Append(" Where ");

                sql.AppendFormat(" {0}.id_media={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _idMedia);
                sql.AppendFormat(" and {0}.date_media_num>={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _beginingDate);
                sql.AppendFormat(" and {0}.date_media_num<={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _endDate);

                if (_webSession.GenericInsertionColumns.GetSqlJoins(_webSession.DataLanguage,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null).Length > 0)
                    sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlJoins(_webSession.DataLanguage,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null));
                sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlContraintJoins());

                if ((_vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.radio)
                    && !string.IsNullOrEmpty(_adBreak))
                    sql.AppendFormat("  and commercial_break = {0}", _adBreak);


                if ((_vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.tv)
                    && !string.IsNullOrEmpty(_adBreak))
                {
                    sql.AppendFormat(" and id_commercial_break = {0}", _adBreak);
                }


                string listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                string product = GetProductData();
                string productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession,
                 WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                string mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                string orderby = GetOrderByDetailMedia(allPeriod);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                {
                    sql.AppendFormat(" and {0}.id_advertising_agency(+)={1}.id_advertising_agency ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql.AppendFormat(" and {0}.id_language(+)={1} ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix, _webSession.DataLanguage);
                    sql.AppendFormat(" and {0}.activation(+)<{1} ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix,
                        TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
                }

                #region Droits
                //liste des produit hap
                sql.Append(listProductHap);
                sql.Append(product);
                sql.Append(productsRights);
                sql.Append(mediaRights);

                //Media Universe
                sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

                //Rights detail spot to spot TNT
                if ((_vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.tv)
                    && !_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG))
                {
                    string idTNTCategory = TNS.AdExpress.Domain.Lists.GetIdList(WebConstantes.GroupList.ID.category, WebConstantes.GroupList.Type.digitalTv);
                    if (!string.IsNullOrEmpty(idTNTCategory))
                    {
                        sql.AppendFormat(" and {0}.id_category not in ({1})  ",
                            WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, idTNTCategory);
                    }
                }

                #endregion

                //Group by
                sql.Append(" group by ");
                if (sqlGroupBy.Length > 0) sql.Append(sqlGroupBy);


                // Order by
                sql.AppendFormat("  {0}", orderby);

            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException(string.Format("Impossible to build the query {0}", sql.ToString()), err));
            }

            #region Query execution
            try
            {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException(string.Format("Impossible to exectute query of  media detail : {0}",
                    sql.ToString()), err));
            }
            #endregion

        }

    

        #endregion

        protected override void GetDateCoverField(StringBuilder sql, ref string sqlGroupBy)
        {
        }
    }
}
