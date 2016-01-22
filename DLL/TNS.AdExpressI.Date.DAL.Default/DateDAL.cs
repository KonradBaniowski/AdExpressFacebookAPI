#region Information
/*
 * Author : Y R'kaina
 * Created on : 09/10/2009
 * Modification:
 *      Author - Date - Description
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Date.DAL.Default {
    /// <summary>
    /// This class inherits from the Date DAL Class which provides all the methods to determine specific dates for specific modules.
    /// </summary>
    public class DateDAL : TNS.AdExpressI.Date.DAL.DateDAL {

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

    }
}
