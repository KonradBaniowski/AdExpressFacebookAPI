#region Information
/*
 * Author : G Ragneau
 * Created on  : 28/04/2007
 * Modification : 
 *      Author - Date - Description
 * 
 * 
 * 
 * */
#endregion


#region using
using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;

using TNS.AdExpressI.MediaSchedule;
#endregion

namespace TNS.AdExpressI.MediaSchedule.Default
{
    /// <summary>
    /// Default IMediaScheduleResults implementation
    /// </summary>
    public class MediaScheduleResults:MediaSchedule.MediaScheduleResults
    {

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period):base(session, period){}
        /// <summary>
        /// Constructor of a Media Schedule on a specifi vehicle
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="idVehicle">Vehicle Filter</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, Int64 idVehicle) : base(session, period, idVehicle) { }
        /// <summary>
        /// Constructor of a Media Schedule on a zoomed period
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="zoom">Report zoom</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, string zoom) : base(session, period, zoom) { }
        /// <summary>
        /// Constructor of a Media Schedule on a specifi vehicle and a zoomed period
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="idVehicle">Vehicle Id</param>
        /// <param name="zoom">Report zoom</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, Int64 idVehicle, string zoom) : base(session, period, idVehicle, zoom) { }
        #endregion

    }
}
