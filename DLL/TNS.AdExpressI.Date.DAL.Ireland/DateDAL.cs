using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Date.DAL.Ireland {
    /// <summary>
    /// This class inherits from the Date DAL Class which provides all the methods to determine specific dates for specific modules.
    /// </summary>
    class DateDAL : TNS.AdExpressI.Date.DAL.DateDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DateDAL() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Client session</param>
        public DateDAL(WebSession session) : base(session) { }
        #endregion

        #region GetLastLoadedYear
        /// <summary>
        /// Get the last loaded year in the database for the recap tables (product class analysis modules)
        /// </summary>
        /// <returns>Year</returns>
        public override int GetLastLoadedYear() {
            return -1;
        }
        #endregion

    }
}
