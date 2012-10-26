using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Rolex.Default
{
    public class RolexResults : Rolex.RolexResults
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session"></param>
        public RolexResults(WebSession session):base(session)
        {           
        }

        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="periodBeginningDate">period Beginning Date</param>
        /// <param name="periodEndDate">period End Date</param>
        public RolexResults(WebSession session, string periodBeginningDate, string periodEndDate):base(session,periodBeginningDate,periodEndDate)
        {
         
        }

        #endregion
    }
}
