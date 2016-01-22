#region Information
/*
 * Author : G Facon
 * Creation : 12/10/2009
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion


using Trends=TNS.AdExpressI.Trends.DAL;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Trends.DAL.Default {
    /// <summary>
    /// Default  module trends report data access layer.
    /// Uses the methods defined in the parent class.
    /// </summary>
    public class TrendsDAL:Trends.DAL.TrendsDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Customer session which contains user configuration parameters and universe selection</param>
        public TrendsDAL(WebSession session) : base(session) {
        }
        #endregion

    }
}
