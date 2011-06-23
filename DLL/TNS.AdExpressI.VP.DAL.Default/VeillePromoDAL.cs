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
        public VeillePromoDAL(WebSession session):base(session)
        {
          
        }
        #endregion
    }
}
