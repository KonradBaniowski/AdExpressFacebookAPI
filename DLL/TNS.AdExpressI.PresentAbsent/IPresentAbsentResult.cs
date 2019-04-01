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
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
#endregion

namespace TNS.AdExpressI.PresentAbsent
{

    /// <summary>
    /// Present / Absent Report Contract
    /// </summary>
    public interface IPresentAbsentResult
    {

        /// <summary>
        /// Compute result "Summary"
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetSummary();
        /// <summary>
        /// Compute result "Portofolio"
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetPortefolio();
        /// <summary>
        /// Compute result for the study "Present in more than one vehicle"
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetCommons();
        /// <summary>
        /// Compute result "Missings"
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetMissings();
        /// <summary>
        /// Compute result "Exclusives"
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetExclusives();
        /// <summary>
        /// Compute result "Strengths"
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetStrengths();
        /// <summary>
        /// Compute result "Prospects"
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetProspects();
        /// <summary>
        /// Compute specified result
        /// </summary>
        /// <param name="result">Type of result (COmpetitorMarketShare)</param>
        /// <returns>Computed data</returns>
        ResultTable GetResult(int result);
        /// <summary>
        /// Compute result specified in user session
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetResult();

        /// <summary>
        /// Compute result grid specified in user session
        /// </summary>
        /// <returns>Computed data</returns>
        GridResult GetGridResult();

        long CountData();

    }

}
