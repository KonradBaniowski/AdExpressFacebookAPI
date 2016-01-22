using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;

namespace TNS.AdExpressI.AdvertisingAgency.DAL.Default
{
    /// <summary>
    /// Extract data for different type of results of the module Advertising Agency Report.
    /// </summary>
    public class AdvertisingAgencyDAL : TNS.AdExpressI.AdvertisingAgency.DAL.AdvertisingAgencyDAL
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User session</param>      
        public AdvertisingAgencyDAL(WebSession session, MediaSchedulePeriod period) : base(session, period) { }
        #endregion

    }
}
