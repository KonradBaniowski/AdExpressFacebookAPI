
#region using
using System;
using System.Text;

using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
#endregion

namespace TNS.AdExpressI.MediaSchedule.Appm
{
    /// <summary>
    /// IMediaScheduleResults implementation for APPM Media Plan
    /// </summary>
    public class MediaScheduleResults : TNS.AdExpressI.MediaSchedule.MediaScheduleResults
    {

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period):base(session, period){
            _module = ModulesList.GetModule(CstWeb.Module.Name.BILAN_CAMPAGNE);
        }
        /// <summary>
        /// Constructor of a Media Schedule on a specifi vehicle
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="idVehicle">Vehicle Filter</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, Int64 idVehicle):base(session, period, idVehicle){
            _module = ModulesList.GetModule(CstWeb.Module.Name.BILAN_CAMPAGNE);
        }
        /// <summary>
        /// Constructor of a Media Schedule on a zoomed period
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="zoom">Report zoom</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, string zoom) : base(session, period, zoom) {
            _module = ModulesList.GetModule(CstWeb.Module.Name.BILAN_CAMPAGNE);
        }
        /// <summary>
        /// Constructor of a Media Schedule on a specifi vehicle and a zoomed period
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        /// <param name="idVehicle">Vehicle Id</param>
        /// <param name="zoom">Report zoom</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period, Int64 idVehicle, string zoom): base(session, period, idVehicle, zoom){
            _module = ModulesList.GetModule(CstWeb.Module.Name.BILAN_CAMPAGNE);
        }
        #endregion

        #region Design table
        /// <summary>
        /// Append Link to insertion popup
        /// </summary>
        /// <param name="data">Data Source</param>
        /// <param name="t">StringBuilder to fill</param>
        /// <param name="themeName">Name of the current theme</param>
        /// <param name="line">Current line</param>
        /// <param name="cssClasse">Line syle</param>
        /// <param name="level">Column index of the current level (except for level 4 which represent by level 3)</param>
        protected override void AppendInsertionLink(object[,] data, StringBuilder t, string themeName, int line, string cssClasse, int level)
        {

            if (level >= L2_COLUMN_INDEX && data[line, level] != null)
            {
                t.AppendFormat("<td align=\"center\" class=\"{0}\"><a href=\"javascript:PopUpInsertion('{1}','{2}');\"><img border=0 src=\"/App_Themes/{4}/Images/Common/picto_plus.gif\"></a></td>"
                    , cssClasse
                    , _session.IdSession
                    , GetLevelFilter(data, line, level)
                    , _zoom
                    , themeName
                );
            }
            else
            {
                t.AppendFormat("<td align=\"center\" class=\"{0}\">&nbsp;</td>", cssClasse);
            }
        }
        /// <summary>
        /// Build level filters
        /// </summary>
        /// <param name="data">Data source</param>
        /// <param name="line">Current line</param>
        /// <param name="level">Current level</param>
        /// <returns>Filters as "id1" (idX replace by -1 if required depending on the current level)</returns>
        protected override string GetLevelFilter(object[,] data, int line, int level)
        {
            return string.Format("{0}", data[line, L2_ID_COLUMN_INDEX]);
        }
        #endregion

    }
}
