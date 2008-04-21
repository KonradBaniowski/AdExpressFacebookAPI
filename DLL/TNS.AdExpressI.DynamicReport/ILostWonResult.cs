#region Information
/*
 * Author : G Ragneau
 * Creation : 15/04/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using TNS.FrameWork.WebResultUI;
#endregion

namespace TNS.AdExpressI.LostWon
{

    /// <summary>
    /// Dynamic Report Contract
    /// </summary>
    public interface ILostWonResult
    {

        /// <summary>
        /// Compute portefolio report
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetPortofolio();
        /// <summary>
        /// Compute Loyal Report
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetLoyal();
        /// <summary>
        /// Compute "Loyal In Decline" Report
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetLoyalDecline();
        /// <summary>
        /// Compute "Loyal In Progress" Report
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetLoyalRise();
        /// <summary>
        /// Compute "Won" Report
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetWon();
        /// <summary>
        /// Compute "Lost" Report
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetLost();
        /// <summary>
        /// Compute "Synthesis" Report
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetSynthesis();
        /// <summary>
        /// Compute specified result
        /// </summary>
        /// <param name="result">Type of result (DynamicAnalysis)</param>
        /// <returns>Computed data</returns>
        ResultTable GetResult(int result);
        /// <summary>
        /// Compute result specified in user session
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetResult();

    }

}
