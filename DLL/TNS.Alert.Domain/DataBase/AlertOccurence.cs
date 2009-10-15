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

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="row">Data Row</param>
        public AlertOccurence(DataRow row) {
            if (row == null)
                throw new ArgumentNullException("AlertOccurrence contructor: Data row cannot be null");
            this._row = row;
        }
        #endregion

        #region Assessor

        #region AlertOccurrenceId
        /// <summary>
        /// Gets occurrence id
        /// </summary>
        public int AlertOccurrenceId
        {
            get { return (int.Parse(this._row["ID_ALERT_OCCURENCE"].ToString())); }
        }
        #endregion

        #region AlertId
        /// <summary>
        /// Gets alert id
        /// </summary>
        public int AlertId
        {
            get { return (int.Parse(this._row["id_alert"].ToString())); }
        }
        #endregion

        #region DateSend
        /// <summary>
        /// Date when the occurrence was sent to the customer
        /// </summary>
        public DateTime DateSend
        {
            get { return ((DateTime)this._row["DATE_SEND"]); }
        }
        #endregion

        #region DateBeginStudy
        /// <summary>
        /// Date used as the CustomerBeginningPeriod
        /// </summary>
        public DateTime DateBeginStudy
        {
            get { return ((DateTime)this._row["DATE_BEGIN_STUDY"]); }
        }
        #endregion

        #region DateEndStudy
        /// <summary>
        /// Date used as the CustomerEndingPeriod
        /// </summary>
        public DateTime DateEndStudy
        {
            get { return ((DateTime)this._row["DATE_END_STUDY"]); }
        }
        #endregion

        #endregion

    }
}
