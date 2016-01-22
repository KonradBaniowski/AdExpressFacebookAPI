using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.AdvertisingAgency.Default
{
    /// <summary>
    /// Default Advertising Agency Reports
    /// </summary>
    public class AdvertisingAgencyResult : TNS.AdExpressI.AdvertisingAgency.AdvertisingAgencyResult
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        public AdvertisingAgencyResult(WebSession session):base(session){}
        #endregion

    }
}
