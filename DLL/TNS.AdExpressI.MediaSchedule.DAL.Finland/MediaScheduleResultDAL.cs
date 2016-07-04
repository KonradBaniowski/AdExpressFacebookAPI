#region Information
/*
 * Author : G. Facon & G. Ragneau
 * Created : 24/08/2008
 * Modifications :
 *      Author - Date - Descriptopn
 *      
*/
#endregion

#region using
using System;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;

using TNS.AdExpressI.MediaSchedule.DAL;
using TNS.AdExpressI.MediaSchedule.DAL.Exceptions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using CstPeriod=TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstDBClassif=TNS.AdExpress.Constantes.Classification.DB;
#endregion

namespace TNS.AdExpressI.MediaSchedule.DAL.Finland
{
    /// <summary>
    /// Finish Implementation of IMediaScheduleResultDAL
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

        #region Override
        /// <summary>
        /// Get field for periodicity data
        /// </summary>
        ///<param name="detailPeriod">Detail period type</param>
        ///<param name="vehicleId">Vehicle Id</param>
        /// <param name="displayPeriod">Result display level</param>
        /// <returns>Field matching period type</returns>
        protected override string GetPeriodicity(CustomerSessions.Period.PeriodBreakdownType detailPeriod,long vehicleId,CustomerSessions.Period.DisplayLevel displayPeriod) {


            switch(detailPeriod) {
                case CstPeriod.PeriodBreakdownType.month:
                    return (" max(duration) as period_count ");
                case CstPeriod.PeriodBreakdownType.week:
                    return (" max(duration) as period_count ");
                case CstPeriod.PeriodBreakdownType.data:
                case CstPeriod.PeriodBreakdownType.data_4m:
                    switch(VehiclesInformation.DatabaseIdToEnum(vehicleId)) {
                        case CstDBClassif.Vehicles.names.press:
                        case CstDBClassif.Vehicles.names.internationalPress:
                        case CstDBClassif.Vehicles.names.newspaper:
                        case CstDBClassif.Vehicles.names.magazine:
                            switch(displayPeriod) {
                                case CstPeriod.DisplayLevel.monthly:
                                    return string.Format(" max({0}DURATION_MONTH(date_media_num,duration)) as period_count ",_schAdexpr03.Sql);
                                case CstPeriod.DisplayLevel.weekly:
                                    return string.Format(" max({0}DURATION_WEEK(date_media_num,duration)) as period_count ",_schAdexpr03.Sql);
                                default:
                                    return (" trunc((max(duration)/86400)) as period_count ");
                            }
                        case CstDBClassif.Vehicles.names.outdoor:
                        case CstDBClassif.Vehicles.names.dooh:
                        case CstDBClassif.Vehicles.names.instore:
                        case CstDBClassif.Vehicles.names.indoor:
                        case CstDBClassif.Vehicles.names.radio:
                        case CstDBClassif.Vehicles.names.radioGeneral:
                        case CstDBClassif.Vehicles.names.radioSponsorship:
                        case CstDBClassif.Vehicles.names.radioMusic:
                        case CstDBClassif.Vehicles.names.tv:
                        case CstDBClassif.Vehicles.names.tvGeneral:
                        case CstDBClassif.Vehicles.names.tvSponsorship:
                        case CstDBClassif.Vehicles.names.tvNonTerrestrials:
                        case CstDBClassif.Vehicles.names.tvAnnounces:
                        case CstDBClassif.Vehicles.names.others:
                        case CstDBClassif.Vehicles.names.internet:
                        case CstDBClassif.Vehicles.names.adnettrack:
                        case CstDBClassif.Vehicles.names.cinema:
                        case CstDBClassif.Vehicles.names.search:
                            return (" 1 as period_count ");
                        case CstDBClassif.Vehicles.names.directMarketing:
                            return (" 7 as period_count ");
                        default:
                            throw (new MediaScheduleDALException("Unable to determine the media periodicity"));
                    }
                default:
                    throw (new MediaScheduleDALException("Selected period detail unvalid. Unable to determine periodicity field."));
            }
        }
        #endregion
    }
}
