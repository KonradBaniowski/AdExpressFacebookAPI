using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Trends.Default
{
    public class Trends : TNS.AdExpressI.Trends.Trends
    {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public Trends(WebSession session):base(session)
        {            
        }
        #endregion
    }
}
