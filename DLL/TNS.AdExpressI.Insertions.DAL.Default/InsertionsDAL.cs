using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpressI.Insertions.DAL;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;

namespace TNS.AdExpressI.Insertions.DAL.Default
{
    public class InsertionsDAL : TNS.AdExpressI.Insertions.DAL.InsertionsDAL
    {

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="module">Current Module</param>
        public InsertionsDAL(WebSession session, Int64 moduleId):base(session, moduleId) {
        }
        #endregion

    }
}
