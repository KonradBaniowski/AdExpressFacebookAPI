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

        #region Properties
        /// <summary>
        /// Define Current Module
        /// </summary>
        TNS.AdExpress.Domain.Web.Navigation.Module Module
        {
            get;
            set;
        }
        /// <summary>
        /// Report period
        /// </summary>
        MediaSchedulePeriod Period
        {
            get;
            set;
        }
        /// <summary>
        /// Report period
        /// </summary>
        MediaSchedulePeriod PeriodComparative {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Get data to build a Media Schedule Report
        /// </summary>
        DataSet GetMediaScheduleData();

        /// <summary>
        /// Get data to build a Media Schedule Report
        /// </summary>
        /// <param name="isComparative">True if we need data for comparative period</param>
        DataSet GetMediaScheduleData(bool isComparative);

        /// <summary>
        /// Get data to build a Media Schedule Report
        /// </summary>
        DataSet GetMediaScheduleDataLevels();

        /// <summary>
        /// Get Data to build an AdNettrack Media Schedule
        /// </summary>
        DataSet GetMediaScheduleAdNetTrackData();
    }
}
