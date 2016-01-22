using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.VP.DAL.Default {
    public class VeillePromoDAL : TNS.AdExpressI.VP.DAL.VeillePromoDAL{
        
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        ///  <param name="session">session</param>
        public VeillePromoDAL(WebSession session):base(session)
        {
          
        }
        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="periodBeginningDate">period Beginning Date</param>
        /// <param name="periodEndDate">period End Date</param>
        public VeillePromoDAL(WebSession session, string periodBeginningDate, string periodEndDate):base(session,periodBeginningDate,periodEndDate)
        {
          
        }
        #endregion
    }
}
