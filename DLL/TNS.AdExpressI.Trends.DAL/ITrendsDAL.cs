#region Information
/*
 * Author : G Facon
 * Creation : 09/10/2009
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
#endregion


namespace TNS.AdExpressI.Trends.DAL {

    /// <summary>
    /// 
    /// </summary>
    public interface ITrendsDAL {

        /// <summary>
        /// Retreive the data for the trends report result
        /// </summary>
        /// <returns>
        /// DataSet Containing the following tables
        /// 
        /// Tables[0] -> DataTable for the Total  (Media Type level)
        /// Tables[1] -> DataTable for the second (Media Category level)
        /// Tables[3] -> DataTable for the third  (Media Category level)
        /// </returns>
        DataSet GetData();
    }
}
