using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.AdvertisingAgency.DAL.France
{
    public class AdvertisingAgencyDAL : DAL.AdvertisingAgencyDAL
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User session</param>      
        public AdvertisingAgencyDAL(WebSession session, MediaSchedulePeriod period) : base(session, period) { }
        #endregion

        #region Get Unit & Group By
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
