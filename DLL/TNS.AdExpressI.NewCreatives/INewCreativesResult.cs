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
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
#endregion

namespace TNS.AdExpressI.NewCreatives {
    
    /// <summary>
    /// New creatives Contract
    /// </summary>
    public interface INewCreativesResult {

        /// <summary>
        /// Compute new creatives
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetData();
        GridResult GetGridResult();
    }
}
