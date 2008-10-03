using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpressI.Insertions;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Insertions.Default
{
    public class InsertionsResult : TNS.AdExpressI.Insertions.InsertionsResult
    {
        #region Constructor
        /// <summary>
        /// DEfault constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Module ID</param>
        public InsertionsResult(WebSession session, Int64 moduleId):base(session, moduleId)
        {
        }
        #endregion

    }
}
