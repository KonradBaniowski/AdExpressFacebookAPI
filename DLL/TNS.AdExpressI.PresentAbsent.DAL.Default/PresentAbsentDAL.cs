using System;

using PresentAbsent = TNS.AdExpressI.PresentAbsentDAL;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.PresentAbsent.DAL.Default {

    public class PresentAbsentDAL:PresentAbsent.DAL.PresentAbsentDAL{
    
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        public PresentAbsentDAL(WebSession session):base(session)
        {
        }
        #endregion

    }
}
