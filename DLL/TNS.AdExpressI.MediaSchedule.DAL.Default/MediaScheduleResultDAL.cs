#region Information
/*
 * Author : G. Facon & G. Ragneau
 * Created : 21/11/2007
 * Modifications :
 *      Author - Date - Descriptopn
 *      G Ragneau - 30/04/2008 - Mise en couches
 *      
*/
#endregion

#region using
using System;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;

using TNS.AdExpressI.MediaSchedule.DAL;
#endregion

namespace TNS.AdExpressI.MediaSchedule.DAL.Default
{
    /// <summary>
    /// Default Implementation of IMediaScheduleResultDAL
    /// </summary>
    public class MediaScheduleResultDAL : DAL.MediaScheduleResultDAL
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period):base(session, period){}
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        /// <param name="idVehicle">Id of the vehicle</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period, Int64 idVehicle):base(session, period,idVehicle ){}
        #endregion

    }
}
