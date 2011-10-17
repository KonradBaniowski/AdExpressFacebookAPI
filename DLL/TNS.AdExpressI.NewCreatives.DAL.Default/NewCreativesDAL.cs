using System;
using NewCreatives = TNS.AdExpressI.NewCreatives.DAL;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.NewCreatives.DAL.Default {

    public class NewCreativesDAL : NewCreatives.DAL.NewCreativesDAL {
        public string BeginingDate { get; set; }

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">WebSession</param>
        /// <param name="idSectors">id Sectors</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        public NewCreativesDAL(WebSession session, string idSectors, string beginingDate, string endDate)
            : base(session, idSectors, beginingDate, endDate)
        {
            BeginingDate = beginingDate;
        }

        #endregion

    }
}
