#region Information
/*
 * Author : B.Masson
 * Creation : 29/09/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
#endregion

namespace TNS.AdExpressI.NewCreatives.DAL {

    /// <summary>
    /// New creatives DAL Contract
    /// </summary>
    public interface INewCreativeResultDAL {
        DataSet GetData();

        long CountData();
    }

}
