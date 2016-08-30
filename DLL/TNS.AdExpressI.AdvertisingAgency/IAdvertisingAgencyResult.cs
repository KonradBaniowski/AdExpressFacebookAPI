using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.AdvertisingAgency{
    /// <summary>
    /// Advertising Agency Report Contract
    /// </summary>
    public interface IAdvertisingAgencyResult{

        /// <summary>
        /// Compute result specified in user session
        /// </summary>
        /// <returns>Computed data</returns>
        ResultTable GetResult();

        /// <summary>
        /// Get Grid Result
        /// </summary>
        /// <returns>GridResult</returns>
        GridResult GetGridResult();

    }
}