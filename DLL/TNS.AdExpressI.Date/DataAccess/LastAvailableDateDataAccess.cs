#region Information
// Author: Y. R'kaina
// Creation date: 08/09/2009
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using System.Data;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpressI.Date.DataAccess {
    
    public class LastAvailableDateDataAccess {
        /// <summary>
        /// Get the last available date for which we have data in the data base
        /// </summary>
        /// <param name="vehicleId">Vehicle Identifier</param>
        /// <returns>DataSet containing the result of the SQL request</returns>
        public static DataSet GetLastAvailableDate(Int64 vehicleId) {

            StringBuilder sql = new StringBuilder(500);

            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.webAdministration);
            
            string tableName = TNS.AdExpress.Web.Core.Utilities.SQLGenerator.GetDataTableName(TNS.AdExpress.Constantes.Web.CustomerSessions.Period.PeriodBreakdownType.data, vehicleId, false);
            sql.Append("Select max( date_media_num ) as availableDate ");
            sql.Append(" from " + tableName);
            
            #region Request Execution
            try {
                return dataSource.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new Exception.LastAvailableDateException("Impossible to get the last available date : " + sql, err));
            }
            #endregion
        
        }

    }
}
