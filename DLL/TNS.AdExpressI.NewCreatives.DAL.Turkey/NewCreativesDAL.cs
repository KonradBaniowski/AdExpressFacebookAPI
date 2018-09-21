using System.Data;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.NewCreatives.DAL.Exceptions;

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

        protected override Table GetTable(VehicleInformation vehicle, bool isRetailerSelection)
        {
            return WebApplicationParameters.GetDataTable(TableIds.spotTv, isRetailerSelection);
        }
    }
}
