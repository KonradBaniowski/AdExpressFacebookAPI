using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace TNS.Alert.Domain
{
    public class AlertOccurence
    {
        #region Variables

        private DataRow _row;

        #endregion

        #region Properties

        /// <summary>
        /// Gets occurrence id
        /// </summary>
        public int AlertOccurrenceId
        {
            get { return (int.Parse(this._row["ID_ALERT_OCCURENCE"].ToString())); }
        }

        /// <summary>
        /// Gets alert id
        /// </summary>
        public int AlertId
        {
            get { return (int.Parse(this._row["id_alert"].ToString())); }
        }

        /// <summary>
        /// Date when the occurrence was sent to the customer
        /// </summary>
        public DateTime DateSend
        {
            get { return ((DateTime)this._row["DATE_SEND"]); }
        }

        /// <summary>
        /// Date used as the CustomerBeginningPeriod
        /// </summary>
        public DateTime DateBeginStudy
        {
            get { return ((DateTime)this._row["DATE_BEGIN_STUDY"]); }
        }

        /// <summary>
        /// Date used as the CustomerEndingPeriod
        /// </summary>
        public DateTime DateEndStudy
        {
            get { return ((DateTime)this._row["DATE_END_STUDY"]); }
        }

        #endregion

        public AlertOccurence(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException("AlertOccurrence contructor: Data row cannot be null");
            this._row = row;
        }
    }
}
