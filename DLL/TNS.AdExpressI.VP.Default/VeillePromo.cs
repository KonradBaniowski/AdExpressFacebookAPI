using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.VP.Default {
    public class VeillePromo : TNS.AdExpressI.VP.VeillePromo
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public VeillePromo(WebSession session):base(session)
        {
           
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="idDataPromotion">Id data promotion</param>
        public VeillePromo(WebSession session, long idDataPromotion):base(session,idDataPromotion)
        {           
        }
        #endregion

    }
}
