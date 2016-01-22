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
    /// Trends Report DAL
    /// </summary>
    public interface ITrendsDAL {

        /// <summary>
        /// Retreive the data for the trends report result
        /// </summary>
        /// <returns>
        /// DataSet Containing the following tables
        /// 
        /// Tables["TOTAL"] -> DataTable for the Total  (Media Type level) 
        /// Tables["Levels"] -> DataTable for the levels  (Media Category level\Media Vehicle level)
        DataSet GetData();
    }
}
