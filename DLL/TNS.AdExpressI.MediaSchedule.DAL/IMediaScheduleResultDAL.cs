#region Information
/*
 * Author : G Ragneau
 * Created on : 30/04/2008
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
using System.Data;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;
#endregion

namespace TNS.AdExpressI.MediaSchedule.DAL
{
    /// <summary>
    /// Media Schedule DataAccess Contract
    /// </summary>
    public interface IMediaScheduleResultDAL
    {

        /// <summary>
        /// Get data to build a Media Schedule Report
        /// </summary>
        DataSet GetMediaScheduleData();

        /// <summary>
        /// Get Data to build an AdNettrack Media Schedule
        /// </summary>
        DataSet GetMediaScheduleAdNetTrackData();
    }
}
