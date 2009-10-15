using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Alerts = TNS.Ares.Constantes.Constantes.Alerts;
using System.Globalization;

namespace TNS.Alert.Domain
{
    public class Alert
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
        /// <param name="row">Data Row</param>
        public Alert(DataRow row) {
            if (row == null)
                throw new ArgumentNullException("No data in alert row");
            this._row = row;
        }
        #endregion

        #region Assessor

        #region Title
        /// <summary>
        /// Get Alert Name
        /// </summary>
        public string Title
        {
            get { return (this._row["alert"].ToString()); }
        }
        #endregion

        #region AlertId
        /// <summary>
        /// Get Alert Id
        /// </summary>
        public int AlertId
        {
            get { return (int.Parse(this._row["id_alert"].ToString())); }
        }
        #endregion

        #region CustomerId
        /// <summary>
        /// Get Customer Login Id
        /// </summary>
        public int CustomerId
        {
            get { return (int.Parse(this._row["id_login"].ToString())); }
        }
        #endregion

        #region Recipients
        /// <summary>
        /// Get Recipients Mails
        /// </summary>
        public string Recipients
        {
            get { return (this._row["email_list"].ToString()); }
        }
        #endregion

        #region CreationDate
        /// <summary>
        /// Get CreationDate
        /// </summary>
        public DateTime CreationDate
        {
            get { return (DateTime.Parse(this._row["date_creation"].ToString())); }
        }
        #endregion

        #region ExpirationDate
        /// <summary>
        /// Get ExpirationDate
        /// </summary>
        public DateTime ExpirationDate
        {
            get { return (DateTime.Parse(this._row["date_end"].ToString())); }
        }
        #endregion

        #region ValidationDate
        /// <summary>
        /// Get ValidationDate
        /// </summary>
        public DateTime ValidationDate
        {
            get { return (DateTime.Parse(this._row["date_validate"].ToString())); }
        }
        #endregion

        #region Session
        /// <summary>
        /// Get Session
        /// </summary>
        public object Session
        {
            get
            {
                byte[] session = (byte[])this._row["session_"];
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                ms.Write(session, 0, session.Length);
                ms.Position = 0;
                object o = bf.Deserialize(ms);
                return (o);
            }
        }
        #endregion

        #region Status
        /// <summary>
        /// Get Status
        /// </summary>
        public Alerts.AlertStatuses Status
        {
            get { return ((Alerts.AlertStatuses)Enum.Parse(
                            typeof(Alerts.AlertStatuses),
                            this._row["activation"].ToString())); }
        }
        #endregion

        #region Periodicity
        /// <summary>
        /// Get Periodicity
        /// </summary>
        public Alerts.AlertPeriodicity Periodicity
        {
            get { return ((Alerts.AlertPeriodicity)Enum.Parse(
                            typeof(Alerts.AlertPeriodicity),
                            this._row["id_type_periodicity"].ToString()));
            }
        }
        #endregion

        #region PeriodicityValue
        /// <summary>
        /// Get PeriodicityValue
        /// </summary>
        public int PeriodicityValue
        {
            get { return (int.Parse(this._row["expiry"].ToString())); }
        }
        #endregion

        #region IdAlertSchedule
        /// <summary>
        /// Get IdAlertSchedule
        /// </summary>
        public Int64 IdAlertSchedule {
            get { return (Int64.Parse(this._row["id_alert_schedule"].ToString())); }
        }

        #endregion

        #endregion

    }
}
