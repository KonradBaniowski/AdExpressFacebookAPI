using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.MediaSchedule.DAL.France
{
    public class MediaScheduleResultDAL : DAL.MediaScheduleResultDAL
    {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period) : base(session, period) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="period">Report period</param>
        /// <param name="idVehicle">Id of the vehicle</param>
        public MediaScheduleResultDAL(WebSession session, MediaSchedulePeriod period, Int64 idVehicle) : base(session, period, idVehicle) { }
        #endregion

        #region Unit & group By
        protected override string GetUnitFieldName()
        {
            return $" {_schAdexpr03.Label}.LISTNUM_TO_CHAR({WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix}.{_session.GetSelectedUnit().DatabaseMultimediaField}) as {_session.GetSelectedUnit().Id.ToString()} ";
        }

        protected override string GetGroupByOptional()
        {
            return $", {_schAdexpr03.Label}.LISTNUM_TO_CHAR({WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix}.{_session.GetSelectedUnit().DatabaseMultimediaField}) ";
        }
        #endregion
    }
}
