#region Information
/*
 * Author : G Ragneau
 * Creation : 18/03/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System.Data;

using TNS.AdExpress.Web.Core.Sessions;
#endregion

namespace TNS.AdExpressI.PresentAbsent.DAL
{

    /// <summary>
    /// Present / Absent DAL Contract
    /// </summary>
    public interface IPresentAbsentResultDAL
    {
        DataSet GetData();
        DataTable GetSynthesisData();
        DataSet GetColumnDetails();
        DataSet GetMediaDetails();

    }

}
