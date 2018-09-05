using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using Module = TNS.AdExpress.Domain.Web.Navigation.Module;

namespace TNS.AdExpressI.Portofolio.DAL.Turkey.Engines
{
    public class CalendarEngine : DAL.Engines.CalendarEngine
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        /// <param name="module">Module</param>
        public CalendarEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd)
        {
        }
        #endregion

        #region GetGad
        protected override void GetGad(ref string dataTableNameForGad, ref string dataFieldsForGad, ref string dataJointForGad)
        {
            dataTableNameForGad = "";
             dataFieldsForGad = "";
            dataJointForGad = "";
        }
        #endregion

        #region Global Query in case of per day/week/month option
        protected override string GetDateMediaNumFieldWithAlias()
        {
            switch (_webSession.DetailPeriod)
            {
                case CustomerSessions.Period.DisplayLevel.dayly:
                    return DBConstantes.Fields.DATE_MEDIA_NUM + " as \"date_media_num\"";
                case CustomerSessions.Period.DisplayLevel.weekly:
                    return "to_char(to_date(date_media_num,'YYYYMMDD'),'YYYYWW') as \"date_media_num\"";
                case CustomerSessions.Period.DisplayLevel.monthly:
                    return "to_char(to_date(date_media_num,'YYYYMMDD'),'YYYYMM') as \"date_media_num\"";
                default:
                    return "";
            }
        }
        protected override string GetDateMediaNumField()
        {
            switch (_webSession.DetailPeriod)
            {
                case CustomerSessions.Period.DisplayLevel.dayly:
                    return DBConstantes.Fields.DATE_MEDIA_NUM;
                case CustomerSessions.Period.DisplayLevel.weekly:
                    return "to_char(to_date(date_media_num,'YYYYMMDD'),'YYYYWW')";
                case CustomerSessions.Period.DisplayLevel.monthly:
                    return "to_char(to_date(date_media_num,'YYYYMMDD'),'YYYYMM')";
                default:
                    return "";
            }
        }
        #endregion
    }
}
