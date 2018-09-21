using System;
using System.Data;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpress.Web.Core.DataAccess
{
    /// <summary>
    /// Spot Sub Types DAL
    /// </summary>
    public class SpotSubTypesDAL
    {
        /// <summary>
        /// Method used to extract the list of spot sub type
        /// </summary>
        /// <returns>Purchase Modes List</returns>
        public static DataSet GetData()
        {

            var sql = new StringBuilder(500);


            try
            {
              
                var dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.webAdministration);
                Table spotSubType = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.spotSubType);

                sql.AppendFormat("Select distinct {0}.id_{1}, {0}.{1},id_language from {2} where {0}.id_{1} not in ({4}) and {0}.activation <{3} order by {0}.{1} asc"
                    , spotSubType.Prefix
                    , spotSubType.Label
                    , spotSubType.SqlWithPrefix
                    , Constantes.DB.ActivationValues.UNACTIVATED.ToString()
                    , "0");

                return dataSource.Fill(sql.ToString());
            }
            catch (Exception err)
            {
                throw (new Exception("Impossible to get the list of spot sub type list : " + sql, err));
            }
        }

    }
}