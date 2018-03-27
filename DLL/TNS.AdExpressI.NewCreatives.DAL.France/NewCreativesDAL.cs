using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using CstDB = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpressI.NewCreatives.DAL.France
{
    public class NewCreativesDAL : NewCreatives.DAL.NewCreativesDAL
    {


        public NewCreativesDAL(WebSession session, string idSectors, string beginingDate, string endDate)
            : base(session, idSectors, beginingDate, endDate)
        {
        }

        protected override string AppendGad(string dataTableNameForGad, Schema schAdExpr03, Table table, ref string dataFieldsForGad,
          ref string dataJointForGad)
        {

            if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
            {
                try
                {

                    if (_session.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.Customer.DB.Flag.id.leFac.GetHashCode()))
                        dataTableNameForGad = ", " + schAdExpr03.Sql + SQLGenerator.GetTablesForLeFac(_session) + " " + CstDB.Tables.GAD_PREFIXE;
                    else
                        dataTableNameForGad = ", " + schAdExpr03.Sql + SQLGenerator.GetTablesForGad(_session) + " " + CstDB.Tables.GAD_PREFIXE;
                    dataFieldsForGad = ", " + SQLGenerator.GetFieldsAddressForGad();
                    dataJointForGad = "and " + SQLGenerator.GetJointForGad(table.Prefix);
                }
                catch (SQLGeneratorException) {; }
            }
            return dataTableNameForGad;

        }

    }
}