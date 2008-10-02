using System;
using NewCreatives = TNS.AdExpressI.NewCreatives.DAL;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.NewCreatives.DAL.Default {

    public class NewCreativesDAL : NewCreatives.DAL.NewCreativesDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">WebSession</param>
        public NewCreativesDAL(WebSession session, Int64 idSector, string beginingDate, string endDate)
            : base(session, idSector, beginingDate, endDate) {
        }
        #endregion

    }
}
