using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using TNS.FrameWork.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;

using TNS.FrameWork.DB.Constantes;

using TNS.Alert.Domain;
using ConstDB = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Alerts
{
    /// <summary>
    /// Abstract method implementing default behavior for
    /// alert librairies. In case you need specific information,
    /// implement that class and override methods.
    /// </summary>
    public abstract class AlertsDAL : IAlertDAL
    {
        #region Variables

        // Class stuffs
        protected IDataSource _src = null;
        protected DataSet _data = new DataSet();

        // Database variables
        private string _selectFields = "*";
        private string _updateFieldFormat = "status = {0}";
        private List<string> _updateFieldValues;

        // Alerts
        private AlertCollection _alerts;
        private AlertOccurenceCollection _alertOccurrences;

        #endregion

        #region Properties

        public string SelectFields
        {
            get { return this._selectFields; }
            set { this._selectFields = value; }
        }

        #endregion

        #region Constructor

        public AlertsDAL(IDataSource src)
        {
            this._src = src;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a dataset containing all the alerts that should
        /// be sent at the given day of week and day of month.
        /// </summary>
        /// <param name="DayOfWeek">Day of the week</param>
        /// <param name="DayOfMonth">Day of the month</param>
        /// <returns></returns>
        public virtual AlertCollection GetAlerts(int DayOfWeek, int DayOfMonth)
        {
            return (this.GetData(DayOfWeek, DayOfMonth));
        }

        public virtual AlertCollection GetAlerts(long loginId)
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("SELECT {0} ", this._selectFields);
            sql.AppendFormat("FROM {0}.{1} ", ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT);
            sql.AppendFormat("WHERE id_login = {0} AND id_alert_type = {1}", loginId, ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode());
            sql.Append("ORDER BY activation, id_alert_type");

            this._data = this._src.Fill(sql.ToString());
            fillFromDataTable(this._data.Tables[0]);

            return (this._alerts);
        }

        /// <summary>
        /// Retrieves all the expired alert occurrences. Not data alteration
        /// </summary>
        /// <returns>A AlertOccurenceCollection object</returns>
        public virtual AlertOccurenceCollection GetExpiredAlertOccurences()
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("SELECT * ", this._selectFields);
            sql.AppendFormat("FROM {0}.{1} ", ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT_OCCURENCE);
            sql.AppendFormat("WHERE DATE_END <= TO_DATE('{0}', 'YYYY-MM-DD'", DateTime.Now.ToString("yyyy-MM-dd"));

            // Retrieving data
            this._data = this._src.Fill(sql.ToString());
            if (this._alertOccurrences != null)
                this._alertOccurrences.Clear();
            else
                this._alertOccurrences = new AlertOccurenceCollection();

            foreach (DataRow row in this._data.Tables[0].Rows)
                this._alertOccurrences.Add(new AlertOccurence(row));

            // Returning rows.
            return (this._alertOccurrences);
        }

        /// <summary>
        /// Gets the occurrence corresponding to the given id. Note that there is
        /// no id login check.
        /// </summary>
        /// <param name="occId">Occurrence Id</param>
        /// <returns>An AlertOccurrence object</returns>
        public virtual AlertOccurence GetOccurrence(int occId)
        {
            return (GetOccurrence(occId, -1));
        }

        public virtual AlertOccurence GetOccurrence(int occId, int alertId)
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("SELECT * ", this._selectFields);
            sql.AppendFormat("FROM {0}.{1} ", ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT_OCCURENCE);
            sql.AppendFormat("WHERE id_alert_occurence = {0}", occId);
            if (alertId != -1)
                sql.AppendFormat(" AND id_alert = {0}", alertId);

            // Retrieving data
            this._data = this._src.Fill(sql.ToString());
            if (this._data.Tables[0].Rows.Count != 0)
                return (new AlertOccurence(this._data.Tables[0].Rows[0]));

            // No data
            return (null);
        }

        /// <summary>
        /// Retrieves all the available occurrences for the given alertId
        /// </summary>
        /// <param name="alertId">Alert Id</param>
        /// <returns>An AlertOccurrenceCollection object</returns>
        public virtual AlertOccurenceCollection GetOccurrences(int alertId)
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("SELECT * ", this._selectFields);
            sql.AppendFormat("FROM {0}.{1} ", ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT_OCCURENCE);
            sql.AppendFormat("WHERE id_alert = {0}", alertId);

            // Retrieving data
            this._data = this._src.Fill(sql.ToString());
            this.fillOccurencesFromDataTable(this._data.Tables[0]);

            // Returning rows.
            return (this._alertOccurrences);
        }

        public virtual void DeleteExpiredAlerts()
        {
            string deleteCommand =
                string.Format("DELETE FROM {0}.{1} WHERE ID_ALERT_TYPE = {2} AND (DATE_END <= '{3}' OR ACTIVATION = {4})",
                              ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT, ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode(),
                              DateTime.Now.ToString("yyyy-MM-dd"), ConstDB.Alerts.AlertStatuses.ToDelete.GetHashCode());

            this._src.Delete(deleteCommand);            
        }

        /// <summary>
        /// Deletes all the expired alerts depending on
        /// their expiration date
        /// </summary>
        public virtual void DeleteExpiredAlertOccurences()
        {
            string deleteCommand =
                string.Format("DELETE FROM {0}.{1} WHERE DATE_END <= '{2}'",
                              ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT_OCCURENCE, DateTime.Now.ToString("yyyy-MM-dd"));

            this._src.Delete(deleteCommand);
        }

        /// <summary>
        /// Updates the given alert to the given status
        /// </summary>
        /// <param name="alertId">Alert identifier</param>
        /// <param name="status">New status</param>
        public virtual void UpdateStatus(int alertId, ConstDB.Alerts.AlertStatuses status, bool updateActivationDate)
        {
            // Building update query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("UPDATE {0}.{1} ", ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT);
            sql.AppendFormat("SET ACTIVATION = {0} ", status.GetHashCode());
            if (updateActivationDate)
                sql.Append(", DATE_VALIDATE = SYSDATE ");
            sql.AppendFormat("WHERE id_alert = {0}", alertId);

            // Updating
            this._src.Update(sql.ToString());         
        }

        /// <summary>
        /// Activates an alert and updates activation date
        /// </summary>
        /// <param name="alertId">Alert identifier</param>
        public virtual void Activate(int alertId)
        {
            this.UpdateStatus(alertId, ConstDB.Alerts.AlertStatuses.Activated, true);
        }

        /// <summary>
        /// Cancels a given alert, doesn't change activation date
        /// </summary>
        /// <param name="alertId">Alert identifier</param>
        public virtual void Disactivate(int alertId)
        {
            this.UpdateStatus(alertId, ConstDB.Alerts.AlertStatuses.New, false);
        }

        /// <summary>
        /// Deletes an alert (logically or definitly)
        /// </summary>
        /// <param name="alertId">Alert identifier</param>
        /// <param name="logical">If true, just the activation status will change. If false, the row will be deleted</param>
        public virtual void Delete(int alertId, bool logical)
        {
            if (logical)
                this.UpdateStatus(alertId, ConstDB.Alerts.AlertStatuses.ToDelete, false);
            else
            {
                StringBuilder sql = new StringBuilder(100);
                sql.AppendFormat("DELETE FROM {0}.{1} WHERE id_alert = {2}",
                                 ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT, alertId);

                this._src.Delete(sql.ToString());
            }
        }

        /// <summary>
        /// Deletes all the occurrences corresponding to the given alert id
        /// </summary>
        /// <param name="alertId">Id</param>
        public virtual void DeleteOccurrences(int alertId)
        {
            string deleteCommand = String.Format("DELETE FROM {0}.{1} WHERE id_alert = {2}",
                                                 ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT_OCCURENCE, alertId);

            this._src.Delete(deleteCommand);
        }

        /// <summary>
        /// Inserts a new alert in the table
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="type">Periodicity type</param>
        /// <param name="expiry">Defines when the alert occurs (which day of week/month)</param>
        /// <param name="recepients">Email addresses separated by commas</param>
        public virtual int InsertAlertData(string title, object session, ConstDB.Alerts.AlertPeriodicity type, int expiry, string recepients, long idLogin)
        {
            throw new NotImplementedException("This method should be implemented in inheriting object");
        }

        public virtual int InsertAlertOccurrenceData(DateTime expirationDate, string beginStudy, string endStudy, int alertId)
        {
            string insertCommand = String.Format("INSERT INTO {0}.{1} VALUES({2}.SEQ_ALERT_OCCURENCE.NEXTVAL, {3}, TO_DATE('{4}', 'YYYY-MM-DD'), SYSDATE, TO_DATE('{5}', 'YYYYMMDD'), TO_DATE('{6}', 'YYYYMMDD'))",
                                                 ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT_OCCURENCE, ConstDB.Schema.ALERT_SCHEMA, alertId,
                                                 expirationDate.ToString("yyyy-MM-dd"), beginStudy, endStudy);

            this._src.Insert(insertCommand);
            return (0);
        }

        /// <summary>
        /// Retrieve an alert according to the given alert identifier
        /// </summary>
        /// <param name="alertId">Alert identifier</param>
        /// <returns>An TNS.Alert.Domain.Alert object</returns>
        public virtual TNS.Alert.Domain.Alert GetAlert(int alertId)
        {
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("SELECT {0} FROM {1}.{2} WHERE id_alert = {3} AND id_alert_type = {4}",
                             this._selectFields, ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT,
                             alertId, ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode());

            try
            {
                // Executing request
                this._data = this._src.Fill(sql.ToString());
                foreach (DataRow row in this._data.Tables[0].Rows)
                    return (new Alert.Domain.Alert(row));

                // No data was found
                return (null);
            }
            catch
            {
                return (null);
               // throw new Exception("Impossible to get alert with id " + alertId);
            }
        }

        /// <summary>
        /// Retrieves all the new alerts
        /// </summary>
        /// <returns>An alert collection</returns>
        public AlertCollection GetNewAlerts()
        {
            // See private method getNewAlerts to understand
            // that "-1" parameter
            return (getNewAlerts(-1));
        }

        /// <summary>
        /// Retrieves all the new alerts with the given periodicity
        /// </summary>
        /// <param name="periodicity">Periodicity filter</param>
        /// <returns>An AlertCollection</returns>
        public AlertCollection GetNewAlerts(ConstDB.Alerts.AlertPeriodicity periodicity)
        {
            return (getNewAlerts(periodicity.GetHashCode()));
        }

        private AlertCollection getNewAlerts(int periodicity)
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("SELECT {0} ", this._selectFields);
            sql.AppendFormat("FROM {0}.{1} ", ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT);
            sql.AppendFormat("WHERE ID_ALERT_TYPE = {0} AND DATE_VALIDATE IS NULL AND ACTIVATION = {1} ",
                             ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode(), ConstDB.Alerts.AlertStatuses.New.GetHashCode());
            if (periodicity != -1)
                sql.AppendFormat("AND ID_TYPE_PERIODICITY = {0}", periodicity);

            DataSet data = this._src.Fill(sql.ToString());
            this.fillFromDataTable(data.Tables[0]);
            return (this._alerts);
        }

        /// <summary>
        /// Retrieves alerts that should be sent on the given DayOfWeek or DayOfMonth
        /// </summary>
        /// <param name="DayOfWeek">Day of the week (1 to 7)</param>
        /// <param name="DayOfMonth">Day of the month (1 to 28)</param>
        /// <returns></returns>
        protected AlertCollection GetData(int DayOfWeek, int DayOfMonth)
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("SELECT {0} ", this._selectFields);
            sql.AppendFormat("FROM {0}.{1} ", ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT);
            sql.AppendFormat("WHERE (");
            sql.AppendFormat("  (id_type_periodicity = {0}) OR ", ConstDB.Alerts.AlertPeriodicity.Daily.GetHashCode());
            sql.AppendFormat("  (id_type_periodicity = {0} AND expiry = {1}) OR ", ConstDB.Alerts.AlertPeriodicity.Weekly.GetHashCode(), DayOfWeek);
            sql.AppendFormat("  (id_type_periodicity = {0} AND expiry = {1})", ConstDB.Alerts.AlertPeriodicity.Monthly.GetHashCode(), DayOfMonth);
            sql.AppendFormat(" ) AND activation = {0} AND id_alert_type = {1}", ConstDB.Alerts.AlertStatuses.Activated.GetHashCode(), ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode());

            // Retrieving data
            this._data = this._src.Fill(sql.ToString());

            // Filling the collection
            this.fillFromDataTable(this._data.Tables[0]);

            // Returning the data set
            return (this._alerts);
        }

        /// <summary>
        /// Retrieves all the expired alerts
        /// </summary>
        /// <returns>An AlertCollection</returns>
        public virtual AlertCollection GetExpiredAlerts()
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("SELECT {0} ", this._selectFields);
            sql.AppendFormat("FROM {0}.{1} ", ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT);
            sql.AppendFormat("WHERE DATE_END < SYSDATE AND ID_ALERT_TYPE = {0}", ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode());

            // Retrieving data
            DataSet data = this._src.Fill(sql.ToString());
            this.fillFromDataTable(data.Tables[0]);
            return (this._alerts);
        }

        /// <summary>
        /// Fill an AlertCollection object using the given DataTable
        /// </summary>
        /// <param name="data">DataTable</param>
        private void fillFromDataTable(DataTable data)
        {
            if (this._alerts != null)
                this._alerts.Clear();
            else
                this._alerts = new AlertCollection();

            if (data != null)
                foreach (DataRow dr in data.Rows)
                    this._alerts.Add(new TNS.Alert.Domain.Alert(dr));
        }

        /// <summary>
        /// Fills an AlertOccurrenceCollection object using
        /// the given DataTable
        /// </summary>
        /// <param name="data">Data</param>
        private void fillOccurencesFromDataTable(DataTable data)
        {
            if (this._alertOccurrences != null)
                this._alertOccurrences.Clear();
            else
                this._alertOccurrences = new AlertOccurenceCollection();

            foreach (DataRow row in data.Rows)
                this._alertOccurrences.Add(new AlertOccurence(row));
        }

        #endregion

    }
}
