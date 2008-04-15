#region Information
/*
 * Author : G Ragneau
 * Creation : 18/03/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion


#region Using
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.PresentAbsent;
#endregion

namespace TNS.AdExpressI.PresentAbsent.Default
{
    /// <summary>
    /// Default Present/Absent reports
    /// </summary>
    public class PresentAbsentResult : PresentAbsent.PresentAbsentResult
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public PresentAbsentResult(WebSession session):base(session)
        {
        }
        #endregion

    }
}
