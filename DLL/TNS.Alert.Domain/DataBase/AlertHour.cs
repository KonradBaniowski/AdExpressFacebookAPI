using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Alerts = TNS.Ares.Constantes.Constantes.Alerts;

namespace TNS.Alert.Domain
{
    public class AlertHour
    {
        #region Variables
        /// <summary>
        /// Data Row
        /// </summary>
        private DataRow _row;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="row"></param>
        public AlertHour(DataRow row) {
            if (row == null)
                throw new ArgumentNullException("No data in Alert Hour row");
            this._row = row;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Hours
        /// </summary>
        public TimeSpan HoursSchedule {
            get { return (GetTimeSpan(Int32.Parse(this._row["hour_beginning"].ToString()))); }
        }
        /// <summary>
        /// Id Alert Schedule
        /// </summary>
        public Int64 IdAlertSchedule {
            get { return (Int64.Parse(this._row["id_alert_schedule"].ToString())); }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Convert Time in seconde to time in TimeSpan (heure + min + sec)
        /// </summary>
        /// <param name="val">Time in seconde</param>
        /// <returns>Time in TimeSpan</returns>
        private TimeSpan GetTimeSpan(Int32 val) {
            int heures = 0;
            int minutes = 0;
            int secondes = 0;
            if (val >= 3600) {
                heures = Convert.ToInt32(Math.Floor((double)val / 3600));
            }
            else {
                heures = 0;
            }
            val = val - (val * 3600);
            if (val >= 60) {
                minutes = Convert.ToInt32(Math.Floor((double)val / 60));
            }
            else {
                minutes = 0;
            }
            val = val - (minutes * 60);

            if (val > 0) {
                secondes = val;
            }
            else {
                secondes = 0;
            }

            return new TimeSpan(heures, minutes, secondes);
        }
        #endregion
    }
}
