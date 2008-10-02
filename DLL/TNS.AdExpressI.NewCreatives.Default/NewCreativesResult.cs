#region Information
/*
 * Author : B.Masson
 * Creation : 29/09/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using System;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.NewCreatives;
#endregion

namespace TNS.AdExpressI.NewCreatives.Default {
    /// <summary>
    /// Default new creatives
    /// </summary>
    public class NewCreativesResult:NewCreatives.NewCreativesResult {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public NewCreativesResult(WebSession session): base(session) {
        }
        #endregion

    }
}
