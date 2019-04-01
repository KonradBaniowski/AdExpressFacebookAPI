using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Classification.DAL.Turkey
{
    public class VehiclesDAL : DAL.VehiclesDAL
    {
        public VehiclesDAL(WebSession session) : base(session)
        {
        }

        protected override string GetMediaRights(bool beginByAnd)
        {
            string sql = "";

            /*Get vehicle classification rights for modules  " Product class analysis: Graphic key reports "
            *  and "Product class analysis: Detailed reports"*/
            if (_session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
                || _session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE)
            {
                sql += GetRecapMediaConditions(vehicleTable, categoryTable, mediaTable, true);
            }
            else  /*Get vehicle classification rights for modules the others modules*/
                sql += GetMediaRights(vehicleTable, categoryTable, mediaTable, beginByAnd);          

            return sql;
        }

    }
}
