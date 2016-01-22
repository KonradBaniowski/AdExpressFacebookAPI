using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Ares.Domain.LS
{
    public class PluginExec {

        #region variables
        /// <summary>
        /// Day Of Week From
        /// </summary>
        private DayOfWeek _dayOfWeekFrom;
        /// <summary>
        /// Day Of Week To
        /// </summary>
        private DayOfWeek _dayOfWeekTo;
        /// <summary>
        /// Day Of Week Hour To
        /// </summary>
        private TimeSpan _dayOfWeekHourTo;
        #endregion

        #region Assessor
        /// <summary>
        /// Get Day Of Week From
        /// </summary>
        public DayOfWeek DayOfWeekFrom {
            get { return (this._dayOfWeekFrom); }
        }
        /// <summary>
        /// Get Day Of Week To
        /// </summary>
        public DayOfWeek DayOfWeekTo {
            get { return (this._dayOfWeekTo); }
        }
        /// <summary>
        /// Get Day Of Week Hour To
        /// </summary>
        public TimeSpan DayOfWeekHourTo {
            get { return (this._dayOfWeekHourTo); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dayOfWeekFrom">Day Of Week From</param>
        /// <param name="dayOfWeekTo">Day Of Week To</param>
        /// <param name="dayOfWeekHourTo">Day Of Week Hour To</param>
        public PluginExec(DayOfWeek dayOfWeekFrom, DayOfWeek dayOfWeekTo, TimeSpan dayOfWeekHourTo) {
            this._dayOfWeekFrom = dayOfWeekFrom;
            this._dayOfWeekTo = dayOfWeekTo;
            this._dayOfWeekHourTo = dayOfWeekHourTo;
        }
        #endregion
    }
}
