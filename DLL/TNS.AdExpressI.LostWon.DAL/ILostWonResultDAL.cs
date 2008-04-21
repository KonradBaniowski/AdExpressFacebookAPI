#region Information
/*
 * Author : G Ragneau
 * Creation : 18/04/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System.Data;

using TNS.AdExpress.Web.Core.Sessions;
#endregion

namespace TNS.AdExpressI.LostWon.DAL
{

    /// <summary>
    /// Dynamic Report DAL Contract
    /// </summary>
    public interface ILostWonResultDAL
    {
        DataSet GetData();
        DataTable GetSynthesisData();
        DataSet GetMediaDetails();

    }

}
