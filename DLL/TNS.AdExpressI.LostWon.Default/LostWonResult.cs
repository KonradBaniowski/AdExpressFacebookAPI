#region Information
/*
 * Author : G Ragneau
 * Creation : 15/04/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion


#region Using
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;

using TNS.AdExpressI.LostWon;

#endregion

namespace TNS.AdExpressI.LostWon.Default
{
    /// <summary>
    /// Default Dynamic reports
    /// </summary>
    public class LostWonResult : LostWon.LostWonResult
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public LostWonResult(WebSession session):base(session){}
        #endregion

    }
}
