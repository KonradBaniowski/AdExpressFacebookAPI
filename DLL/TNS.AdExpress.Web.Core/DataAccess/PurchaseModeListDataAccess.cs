using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Exceptions;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Web.Core.DataAccess {
    class PurchaseModeListDataAccess {

        /// <summary>
        /// Method used to extract the list of purchase modes
        /// </summary>
        /// <returns>Purchase Modes List</returns>
        public static DataSet GetData() {

            var sql = new StringBuilder(500);

            var dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.webAdministration);
            Table purchaseModeTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.purchaseModeMMS);

            sql.AppendFormat("Select distinct {0}.id_{1}, {0}.{1} from {2} where {0}.id_{1} not in ({4}) and {0}.activation <{3} order by {0}.{1} asc"
                , purchaseModeTable.Prefix
                , purchaseModeTable.Label
                , purchaseModeTable.SqlWithPrefix
                , DBConstantes.ActivationValues.UNACTIVATED.ToString()
                , "0");

            try {
                return dataSource.Fill(sql.ToString());
            }
            catch (Exception err) {
                throw (new PurchaseModeListDataAccessException("Impossible to get the list of purchase modes: " + sql, err));
            }
        }

    }
}
