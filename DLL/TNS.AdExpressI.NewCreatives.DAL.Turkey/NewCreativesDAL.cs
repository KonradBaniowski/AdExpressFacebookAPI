using System;
using System.Data;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.NewCreatives.DAL.Exceptions;
using FrameWorkResults = TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpressI.NewCreatives.DAL.Turkey
{
    public class NewCreativesDAL : NewCreatives.DAL.NewCreativesDAL
    {
        public NewCreativesDAL(WebSession session, string idSectors, string beginingDate, string endDate)
          : base(session, idSectors, beginingDate, endDate)
        {
        }

        public override DataSet GetData()
        {

            StringBuilder sql = new StringBuilder();
            Schema schAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);

            #region Execution de la requête
            try
            {
                var table = GetTable(_vehicleInformation, _session.IsSelectRetailerDisplay);
                var detailProductTablesNames = _session.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
                var detailProductFields = _session.GenericProductDetailLevel.GetSqlFields();
                var detailProductJoints = _session.GenericProductDetailLevel.GetSqlJoins(_session.DataLanguage, table.Prefix);
                var detailProductOrderBy = _session.GenericProductDetailLevel.GetSqlOrderFields();
                 TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);
               

                var productsRights = SQLGenerator.GetClassificationCustomerProductRight(_session, table.Prefix, true, module.ProductRightBranches);
               
                // select
                sql.Append("select distinct " + detailProductFields + "," + table.Prefix + ".id_slogan as versionNb ");

                switch (_session.DetailPeriod)
                {
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly:
                        sql.AppendFormat(", to_char( min({0}.date_creation) , 'YYYYMM' ) as date_creation ", table.Prefix);
                        break;
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly:
                        sql.AppendFormat(", to_char( min({0}.date_creation) , 'IYYYIW' ) as date_creation ", table.Prefix);
                        break;
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly:
                        sql.AppendFormat(", to_char( min({0}.date_creation) , 'YYYYMMDD' ) as date_creation ", table.Prefix);
                        break;

                }

                // from
                sql.Append("from " + table.SqlWithPrefix + " , ");
                sql.Append(detailProductTablesNames);

                // where
                string maxHour = "23:59:59";
                sql.Append(" where " + table.Prefix + ".date_creation >= to_date('" + _beginingDate + "','yyyymmdd') ");
                sql.Append(" and " + table.Prefix + ".date_creation <= to_date('" + _endDate + maxHour + "','yyyymmddHH24:MI:SS') ");
                sql.Append(detailProductJoints);
                sql.Append(productsRights);

                // Product Selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(table.Prefix, true));

                //SPOT SUB TYPEs
                string spotSubTypes = _session.SelectedSpotSubTypes;
                if (!string.IsNullOrEmpty(spotSubTypes))
                {
                    Table spotSubType = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.spotSubType);
                    sql.Append($" and {table.Prefix}.ID_{spotSubType.Label} in ({spotSubTypes}) ");

                }

                // group by
                sql.Append(" group by " + detailProductFields + "," + table.Prefix + ".id_slogan ");
              

                // order by
                sql.Append(" order by " + detailProductOrderBy + ", date_creation ");

                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new NewCreativesDALException("Unable to load data for new creatives : " + sql, err));
            }
            #endregion
        }

        public override DataSet GetNewCreativeDetailsData()
        {
            var sql = new StringBuilder(5000);

            try
            {
                string sqlFields = _session.GenericInsertionColumns.GetSqlFields(null);
                string sqlGroupBy = _session.GenericInsertionColumns.GetSqlGroupByFields(null);
                string sqlConstraintFields = _session.GenericInsertionColumns.GetSqlConstraintFields();
                var table = GetTable(_vehicleInformation, _session.IsSelectRetailerDisplay);
                string sqlTables = _session.GenericInsertionColumns.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, null);
                string sqlConstraintTables = _session.GenericInsertionColumns.GetSqlConstraintTables(AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA);
                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);

                //Select
                sql.Append(" select distinct");
                if (sqlFields.Length > 0) sql.AppendFormat(" {0}", sqlFields);

                if (sqlConstraintFields.Length > 0)
                {
                    sql.AppendFormat(" , {0}", sqlConstraintFields);//Fields for constraint management
                    sqlGroupBy += string.Format(" , {0}", _session.GenericInsertionColumns.GetSqlConstraintGroupByFields());
                }

                sql.Append(" from ");
                sql.AppendFormat(" {0} {1}", table.Sql, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                if (sqlTables.Length > 0) sql.AppendFormat(" ,{0}", sqlTables);

                if (sqlConstraintTables.Length > 0)
                    sql.Append(" , " + sqlConstraintTables);//Tables pour la gestion des contraintes métiers

                // Joints conditions
                string maxHour = "23:59:59";
                sql.Append(" where " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_creation >= to_date('" + _beginingDate + "','yyyymmdd') ");
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_creation <= to_date('" + _endDate + maxHour + "','yyyymmddHH24:MI:SS') ");

                // Product Selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

                if (_session.GenericInsertionColumns.GetSqlJoins(_session.DataLanguage,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null).Length > 0)
                    sql.Append("  " + _session.GenericInsertionColumns.GetSqlJoins(_session.DataLanguage,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null));
                sql.Append("  " + _session.GenericInsertionColumns.GetSqlContraintJoins());

                var productsRights = SQLGenerator.GetClassificationCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, module.ProductRightBranches);
                string orderby = " order by slo.id_slogan";

                #region Droits
                //liste des produit hap
                sql.Append(productsRights);
                #endregion

                //Group by
                sql.Append(" group by ");
                if (sqlGroupBy.Length > 0) sql.Append(sqlGroupBy);


                // Order by
                sql.AppendFormat("  {0}", orderby);

            }
            catch (System.Exception err)
            {
                throw (new NewCreativesDALException(string.Format("Impossible to build the query {0}", sql.ToString()), err));
            }

            #region Query execution
            try
            {
                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new NewCreativesDALException(string.Format("Impossible to exectute query of GetNewCreativeDetailsData : {0}", sql.ToString()), err));
            }
            #endregion
        }

        public override long CountData()
        {
            switch (_session.CurrentTab)
            {
                case FrameWorkResults.NewCreative.NEW_CREATIVE_REPORT:
                    return CountNewCreativeReportData();
                case FrameWorkResults.NewCreative.NEW_CREATIVE_DETAILS:
                    return CountNewCreativeDetailsData();
                default:
                    throw (new NewCreativesDALException("Current Tab unknown in CountData() method in New Creative module"));
            }
        }

        public long CountNewCreativeReportData()
        {
            StringBuilder sql = new StringBuilder();
            Schema schAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
            long nbRows = 0;

            #region Execution de la requête
            try
            {
                var table = GetTable(_vehicleInformation, _session.IsSelectRetailerDisplay);
                var detailProductTablesNames = _session.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
                var detailProductFields = _session.GenericProductDetailLevel.GetSqlFields();
                var detailProductJoints = _session.GenericProductDetailLevel.GetSqlJoins(_session.DataLanguage, table.Prefix);
                var detailProductOrderBy = _session.GenericProductDetailLevel.GetSqlOrderFields();
                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);


                var productsRights = SQLGenerator.GetClassificationCustomerProductRight(_session, table.Prefix, true, module.ProductRightBranches);

                // select
                sql.Append(" select count(*) as NbROWS from ( ");//start count statement
                sql.Append("select distinct " + detailProductFields + "," + table.Prefix + ".id_slogan as versionNb ");

                switch (_session.DetailPeriod)
                {
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly:
                        sql.AppendFormat(", to_char( min({0}.date_creation) , 'YYYYMM' ) as date_creation ", table.Prefix);
                        break;
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly:
                        sql.AppendFormat(", to_char( min({0}.date_creation) , 'IYYYIW' ) as date_creation ", table.Prefix);
                        break;
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly:
                        sql.AppendFormat(", to_char( min({0}.date_creation) , 'YYYYMMDD' ) as date_creation ", table.Prefix);
                        break;

                }

                // from
                sql.Append("from " + table.SqlWithPrefix + " , ");
                sql.Append(detailProductTablesNames);

                // where
                string maxHour = "23:59:59";
                sql.Append(" where " + table.Prefix + ".date_creation >= to_date('" + _beginingDate + "','yyyymmdd') ");
                sql.Append(" and " + table.Prefix + ".date_creation <= to_date('" + _endDate + maxHour + "','yyyymmddHH24:MI:SS') ");
                sql.Append(detailProductJoints);
                sql.Append(productsRights);

                // Product Selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(table.Prefix, true));

                //SPOT SUB TYPEs
                string spotSubTypes = _session.SelectedSpotSubTypes;
                if (!string.IsNullOrEmpty(spotSubTypes))
                {
                    Table spotSubType = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.spotSubType);
                    sql.Append($" and {table.Prefix}.ID_{spotSubType.Label} in ({spotSubTypes}) ");

                }

                // group by
                sql.Append(" group by " + detailProductFields + "," + table.Prefix + ".id_slogan ");


                // order by
                sql.Append(" order by " + detailProductOrderBy + ", date_creation ");

                sql.Append(" ) ");//end count data rows

                var ds = _session.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 1)
                    nbRows = (Int64.Parse(ds.Tables[0].Rows[0]["NbROWS"].ToString()));

            }
            catch (System.Exception err)
            {
                throw (new NewCreativesDALException("Unable to load data for new creatives : " + sql, err));
            }
            #endregion

            return nbRows;
        }

        public long CountNewCreativeDetailsData()
        {
            var sql = new StringBuilder(5000);
            long nbRows = 0;

            try
            {
                string sqlFields = _session.GenericInsertionColumns.GetSqlFields(null);
                string sqlGroupBy = _session.GenericInsertionColumns.GetSqlGroupByFields(null);
                string sqlConstraintFields = _session.GenericInsertionColumns.GetSqlConstraintFields();
                var table = GetTable(_vehicleInformation, _session.IsSelectRetailerDisplay);
                string sqlTables = _session.GenericInsertionColumns.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, null);
                string sqlConstraintTables = _session.GenericInsertionColumns.GetSqlConstraintTables(AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA);
                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);

                //Select
                sql.Append(" select count(*) as NbROWS from ( ");//start count statement
                sql.Append(" select distinct");
                if (sqlFields.Length > 0) sql.AppendFormat(" {0}", sqlFields);

                if (sqlConstraintFields.Length > 0)
                {
                    sql.AppendFormat(" , {0}", sqlConstraintFields);//Fields for constraint management
                    sqlGroupBy += string.Format(" , {0}", _session.GenericInsertionColumns.GetSqlConstraintGroupByFields());
                }

                sql.Append(" from ");
                sql.AppendFormat(" {0} {1}", table.Sql, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                if (_session.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                    sql.Append(", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).SqlWithPrefix);

                if (sqlTables.Length > 0) sql.AppendFormat(" ,{0}", sqlTables);

                if (sqlConstraintTables.Length > 0)
                    sql.Append(" , " + sqlConstraintTables);//Tables pour la gestion des contraintes métiers

                // Joints conditions
                string maxHour = "23:59:59";
                sql.Append(" where " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_creation >= to_date('" + _beginingDate + "','yyyymmdd') ");
                sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".date_creation <= to_date('" + _endDate + maxHour + "','yyyymmddHH24:MI:SS') ");

                // Product Selection
                if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true));

                if (_session.GenericInsertionColumns.GetSqlJoins(_session.DataLanguage,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null).Length > 0)
                    sql.Append("  " + _session.GenericInsertionColumns.GetSqlJoins(_session.DataLanguage,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null));
                sql.Append("  " + _session.GenericInsertionColumns.GetSqlContraintJoins());

                var productsRights = SQLGenerator.GetClassificationCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, module.ProductRightBranches);
                string orderby = " order by slo.id_slogan";

                #region Droits
                //liste des produit hap
                sql.Append(productsRights);
                #endregion

                //Group by
                sql.Append(" group by ");
                if (sqlGroupBy.Length > 0) sql.Append(sqlGroupBy);

                // Order by
                sql.AppendFormat("  {0}", orderby);

                sql.Append(" ) ");//end count data rows
            }
            catch (System.Exception err)
            {
                throw (new NewCreativesDALException(string.Format("Impossible to build the query {0}", sql.ToString()), err));
            }

            #region Query execution
            try
            {
                var ds = _session.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 1)
                    nbRows = (Int64.Parse(ds.Tables[0].Rows[0]["NbROWS"].ToString()));

                return nbRows;
            }
            catch (System.Exception err)
            {
                throw (new NewCreativesDALException(string.Format("Impossible to exectute query of CountNewCreativeDetailsData : {0}", sql.ToString()), err));
            }
            #endregion
        }

        protected override Table GetTable(VehicleInformation vehicle, bool isRetailerSelection)
        {
            return WebApplicationParameters.GetDataTable(TableIds.spotTv, isRetailerSelection);
        }
    }
}
