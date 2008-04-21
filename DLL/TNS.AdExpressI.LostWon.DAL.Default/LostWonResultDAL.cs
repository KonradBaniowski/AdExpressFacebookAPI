#region Information
/*
 * Author : G Ragneau
 * Creation : 18/04/2008
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion

#region Using
using TNS.AdExpressI.LostWon.DAL;
using TNS.AdExpress.Web.Core.Sessions;
#endregion

namespace TNS.AdExpressI.LostWon.DAL.Default
{

    /// <summary>
    /// Default Dynamic Report DAL
    /// </summary>
    public class LostWonResultDAL : LostWon.DAL.LostWonResultDAL
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public LostWonResultDAL(WebSession session):base(session){}
        #endregion

    }

}
