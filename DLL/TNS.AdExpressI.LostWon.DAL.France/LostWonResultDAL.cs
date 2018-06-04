using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using CstDB = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpressI.LostWon.DAL.France
{
    public class LostWonResultDAL : DAL.LostWonResultDAL
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public LostWonResultDAL(WebSession session) : base(session) { }
        #endregion

        #region Group BY
        protected override string GetGroupOptional(UnitInformation u, string dataTablePrefixe)
        {
            return $", {WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label}.LISTNUM_TO_CHAR({dataTablePrefixe}.{u.DatabaseMultimediaField}) ";
        }
        #endregion

        protected override void AppendGad(ref string dataTableNameForGad, Schema schAdExpr03, string DATA_TABLE_PREFIXE,
          ref string dataFieldsForGad, ref string dataJointForGad)
        {
            if (
                _session.CustomerLogin.CustormerFlagAccess(
                    (long)TNS.AdExpress.Constantes.Customer.DB.Flag.id.leFac.GetHashCode()))
                dataTableNameForGad = ", " + schAdExpr03.Sql + AdExpress.Web.Core.Utilities.SQLGenerator.GetTablesForLeFac(_session) + " " +
                                      CstDB.Tables.GAD_PREFIXE;
            else
                dataTableNameForGad = ", " + schAdExpr03.Sql + AdExpress.Web.Core.Utilities.SQLGenerator.GetTablesForGad(_session) + " " +
                                      CstDB.Tables.GAD_PREFIXE;

            dataFieldsForGad = ", " + AdExpress.Web.Core.Utilities.SQLGenerator.GetFieldsAddressForGad();
            dataJointForGad = "and " + AdExpress.Web.Core.Utilities.SQLGenerator.GetJointForGad(DATA_TABLE_PREFIXE);

        }
    }
}
