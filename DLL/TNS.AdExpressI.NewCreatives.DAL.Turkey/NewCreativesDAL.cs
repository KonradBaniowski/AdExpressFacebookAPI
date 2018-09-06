using System.Data;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.NewCreatives.DAL.Exceptions;

namespace TNS.AdExpressI.NewCreatives.DAL.Turkey
{
    public class NewCreativesDAL : NewCreatives.DAL.NewCreativesDAL
    {
        public NewCreativesDAL(WebSession session, string idSectors, string beginingDate, string endDate)
          : base(session, idSectors, beginingDate, endDate)
        {
        }

        public DataSet GetData()
        {

            StringBuilder sql = new StringBuilder();
            Schema schAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);

            #region Execution de la requête
            try
            {


                return _session.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new NewCreativesDALException("Unable to load data for new creatives : " + sql, err));
            }
            #endregion
        }
    }
}
