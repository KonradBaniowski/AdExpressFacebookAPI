using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Common.DAL.Russia;

namespace TNS.AdExpressI.MediaSchedule.DAL.Celebrities
{
    public class MediaScheduleResultDAL : Russia.MediaScheduleResultDAL
    {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period)
            : base(session, period)
        {
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        /// <param name="idVehicle">Id of the vehicle</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period, Int64 idVehicle)
            : base(session, period, idVehicle)
        {
        }
        #endregion

        #region GetMediaScheduleData RUSSIA
        /// <summary>
        /// Get Data tables  to build Media Schedule report
        /// The method will return :
        /// - 1 Data table for line TOTAL
        /// - N data tables for N levels
        ///
        /// The format of the data will be :
        /// [identifier Level1, Label level 1,...,identifier LevelN, Label level N,date_num,period_count,unit selected]
        /// </summary>
        /// <returns>DataSet containing Data</returns>
        public override DataSet GetMediaScheduleData()
        {
            var ds = new DataSet();
            return ds;
        }
        #endregion
    }
}
