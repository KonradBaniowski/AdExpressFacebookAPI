using System;
using System.Text;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.MediaSchedule.Celebrities
{
    /// <summary>
    /// Celebrities Media schedule 
    /// </summary>
    public class MediaScheduleResults : Russia.MediaScheduleResults
    {
        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        /// <param name="period">Report Period</param>
        public MediaScheduleResults(WebSession session, MediaSchedulePeriod period) : base(session, period) { }
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


        /// <summary>
        /// Append Lionk to insertion popup
        /// </summary>
        /// <param name="data">Data Source</param>
        /// <param name="t">StringBuilder to fill</param>
        /// <param name="themeName">Name of the current theme</param>
        /// <param name="line">Current line</param>
        /// <param name="cssClasse">Line syle</param>
        /// <param name="level">Column index of the current level (except for level 4 which represent by level 3)</param>
        protected override void AppendInsertionLink(object[,] data, StringBuilder t, string themeName, int line, string cssClasse, int level)
        {
            if (data[line, level] != null)
            {
                t.AppendFormat("<td align=\"center\" class=\"{0}\"><a href=\"javascript:OpenInsertion('{1}','{2}','{3}','-1','{4}');\"><img border=0 src=\"/App_Themes/{5}/Images/Common/picto_plus.gif\"></a></td>"
                    , cssClasse
                    , _session.IdSession
                    , GetLevelFilter(data, line, level)
                    , _zoom
                    , CstWeb.Module.Name.CELEBRITIES
                    , themeName);
              
            }
            else
            {
                t.AppendFormat("<td align=\"center\" class=\"{0}\">&nbsp;</td>", cssClasse);
            }
        }

        /// <summary>
        /// Append Link to version popup
        /// </summary>
        /// <param name="data">Data Source</param>
        /// <param name="t">StringBuilder to fill</param>
        /// <param name="themeName">Name of the current theme</param>
        /// <param name="line">Current line</param>
        /// <param name="cssClasse">Line syle</param>
        /// <param name="level">Column index of the current level (except for level 4 which represent by level 3)</param>
        protected override void AppendCreativeLink(object[,] data, StringBuilder t, string themeName, int line, string cssClasse, int level)
        {
            if (data[line, level] != null)
            {
                t.AppendFormat("<td align=\"center\" class=\"{0}\"><a href=\"javascript:OpenCreatives('{1}','{2}','{3}','-1','{4}');\"><img border=0 src=\"/App_Themes/{5}/Images/Common/picto_plus.gif\"></a></td>"
                    , cssClasse
                    , _session.IdSession
                    , GetLevelFilter(data, line, level)
                    , _zoom
                    , CstWeb.Module.Name.CELEBRITIES
                    , themeName);
            }
            else
            {
                t.AppendFormat("<td align=\"center\" class=\"{0}\">&nbsp;</td>", cssClasse);
            }
        }

    }
}
