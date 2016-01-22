using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using TNS.FrameWork.DB;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Domain.DataBaseDescription;

using TNS.FrameWork.DB.Constantes;

using TNS.Ares.Domain;
using ConstDB = TNS.Ares.Constantes.Constantes;
using TNS.Alert.Domain;
using TNS.Ares.Domain.LS;
using TNS.Ares.Domain.DataBase;

namespace TNS.Ares.Alerts.DAL
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
        private string _updateFieldFormat = "status = {0}";
        private List<string> _updateFieldValues;

        // Alerts
        private AlertCollection _alerts;
        private AlertOccurenceCollection _alertOccurrences;
        private AlertHourCollection _alertHours;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="src">Data Source</param>
        public AlertsDAL(IDataSource src)
        {
            this._src = src;
        }

        #endregion

        #region Public Methods

        #region GetAlerts
        /// <summary>
        /// Returns a dataset containing all the alerts that should
        /// be sent at the given day of week and day of month.
        /// </summary>
        /// <param name="DayOfWeek">Day of the week</param>
        /// <param name="DayOfMonth">Day of the month</param>
        /// <param name="hourBeginning">Hour of launch alert (in second)</param>
        /// <returns></returns>
        public virtual AlertCollection GetAlerts(int DayOfWeek, int DayOfMonth, Int64 hourBeginning)
        {
            return (this.GetData(DayOfWeek, DayOfMonth, hourBeginning));
        }

        /// <summary>
        /// Returns a AlertCollection object
        /// </summary>
        /// <param name="loginId">loginId</param>
        /// <returns>AlertCollection Object</returns>
        public virtual AlertCollection GetAlerts(long loginId)
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("SELECT {0}.id_alert, {0}.alert, {0}.id_login, {0}.email_list, {0}.date_creation, {1}.date_end_module as date_end, {0}.date_validate, {0}.session_, {0}.id_type_periodicity, {0}.expiry, {0}.activation, {0}.id_alert_schedule ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("FROM {0}, {1} ", 
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).SqlWithPrefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).SqlWithPrefix);
            sql.AppendFormat("WHERE {0}.id_login = {1}.id_login ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("AND {0}.id_module = {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix,
                TNS.AdExpress.Constantes.Web.Module.Name.ALERT_ADEXPRESS.ToString());
            sql.AppendFormat("AND {0}.id_login = {1} AND {0}.id_alert_type = {2} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                loginId, 
                ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode());
            sql.AppendFormat("ORDER BY {0}.activation, {0}.id_alert_type ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix);

            this._data = this._src.Fill(sql.ToString());
            FillFromDataTable(this._data.Tables[0]);

            return (this._alerts);
        }
        #endregion

        #region GetExpiredAlertOccurences
        /// <summary>
        /// Retrieves all the expired alert occurrences. Not data alteration
        /// </summary>
        /// <returns>A AlertOccurenceCollection object</returns>
        public virtual AlertOccurenceCollection GetExpiredAlertOccurences()
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.Append("SELECT * ");
            sql.AppendFormat("FROM {0} ", DataBaseConfiguration.DataBase.GetTable(TableIds.alertOccurence).Sql);
            sql.AppendFormat("WHERE DATE_EXPIRATION <= TO_DATE('{0}', 'YYYY-MM-DD'", DateTime.Now.ToString("yyyy-MM-dd"));

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
        #endregion

        #region GetOccurrence
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

        /// <summary>
        /// Gets the occurrence corresponding to the given id. Note that there is
        /// no id login check.
        /// </summary>
        /// <param name="occId">Occurrence Id</param>
        /// <param name="alertId">Alert Id</param>
        /// <returns>An AlertOccurrence object</returns>
        public virtual AlertOccurence GetOccurrence(int occId, int alertId)
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.Append("SELECT * ");
            sql.AppendFormat("FROM {0} ", DataBaseConfiguration.DataBase.GetTable(TableIds.alertOccurence).Sql);
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
            sql.Append("SELECT * ");
            sql.AppendFormat("FROM {0} ", DataBaseConfiguration.DataBase.GetTable(TableIds.alertOccurence).Sql);
            sql.AppendFormat("WHERE id_alert = {0} ", alertId);
            sql.AppendFormat("ORDER BY date_send DESC ");

            // Retrieving data
            this._data = this._src.Fill(sql.ToString());
            this.FillOccurencesFromDataTable(this._data.Tables[0]);

            // Returning rows.
            return (this._alertOccurrences);
        }
        #endregion

        #region DeleteExpiredAlerts
        /// <summary>
        /// Delete Expired Alerts
        /// </summary>
        public virtual void DeleteExpiredAlerts()
        {

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT {0}.date_end_module as date_end ",
                    DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("FROM {0}, {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).SqlWithPrefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).SqlWithPrefix);
            sql.AppendFormat("WHERE {0}.id_login = {1}.id_login ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("AND {0}.id_module = {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix,
                TNS.AdExpress.Constantes.Web.Module.Name.ALERT_ADEXPRESS.ToString());


            StringBuilder deleteCommand = new StringBuilder();
                deleteCommand.AppendFormat("DELETE FROM {0} WHERE ID_ALERT_TYPE = {1} AND (({4}) <= '{2}' OR ACTIVATION = {3})",
                              DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Sql, 
                              ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode(),
                              DateTime.Now.ToString("yyyy-MM-dd"), 
                              ConstDB.Alerts.AlertStatuses.ToDelete.GetHashCode(),
                              sql.ToString());

            this._src.Delete(deleteCommand.ToString());
        }
        #endregion

        #region DeleteExpiredAlertOccurences
        /// <summary>
        /// Deletes all the expired alerts depending on
        /// their expiration date
        /// </summary>
        public virtual void DeleteExpiredAlertOccurences()
        {
            string deleteCommand =
                string.Format("DELETE FROM {0} WHERE DATE_EXPIRATION <= '{1}'",
                              DataBaseConfiguration.DataBase.GetTable(TableIds.alertOccurence).Sql, DateTime.Now.ToString("yyyy-MM-dd"));

            this._src.Delete(deleteCommand);
        }
        #endregion

        #region UpdateStatus
        /// <summary>
        /// Updates the given alert to the given status
        /// </summary>
        /// <param name="alertId">Alert identifier</param>
        /// <param name="status">New status</param>
        public virtual void UpdateStatus(int alertId, ConstDB.Alerts.AlertStatuses status, bool updateActivationDate)
        {
            // Building update query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("UPDATE {0} ", DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Sql);
            sql.AppendFormat("SET ACTIVATION = {0} ", status.GetHashCode());
            if (updateActivationDate)
                sql.Append(", DATE_VALIDATE = SYSDATE ");
            sql.AppendFormat("WHERE id_alert = {0}", alertId);

            // Updating
            this._src.Update(sql.ToString());         
        }
        #endregion

        #region Activate
        /// <summary>
        /// Activates an alert and updates activation date
        /// </summary>
        /// <param name="alertId">Alert identifier</param>
        public virtual void Activate(int alertId)
        {
            this.UpdateStatus(alertId, ConstDB.Alerts.AlertStatuses.Activated, true);
        }
        #endregion

        #region Disactivate
        /// <summary>
        /// Cancels a given alert, doesn't change activation date
        /// </summary>
        /// <param name="alertId">Alert identifier</param>
        public virtual void Disactivate(int alertId)
        {
            this.UpdateStatus(alertId, ConstDB.Alerts.AlertStatuses.New, false);
        }
        #endregion

        #region Delete
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
                sql.AppendFormat("DELETE FROM {0} WHERE id_alert = {1}",
                                 DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Sql, alertId);

                this._src.Delete(sql.ToString());
            }
        }
        #endregion

        #region DeleteOccurrences
        /// <summary>
        /// Deletes all the occurrences corresponding to the given alert id
        /// </summary>
        /// <param name="alertId">Id</param>
        public virtual void DeleteOccurrences(int alertId)
        {
            string deleteCommand = String.Format("DELETE FROM {0} WHERE id_alert = {1}",
                                                 DataBaseConfiguration.DataBase.GetTable(TableIds.alertOccurence).Sql, alertId);

            this._src.Delete(deleteCommand);
        }
        #endregion

        #region InsertAlertData
        /// <summary>
        /// Inserts a new alert in the table
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="type">Periodicity type</param>
        /// <param name="expiry">Defines when the alert occurs (which day of week/month)</param>
        /// <param name="recepients">Email addresses separated by commas</param>
        public virtual int InsertAlertData(string title, byte[] binaryData, Int64 moduleId, ConstDB.Alerts.AlertPeriodicity type, int expiry, string recepients, long idLogin, Int64 idAlertSchedule)
        {
            throw new NotImplementedException("This method should be implemented in inheriting object");
        }
        #endregion

        #region InsertAlertOccurrenceData
        /// <summary>
        /// Inserts a new alert Occurrence in the table
        /// </summary>
        /// <param name="expirationDate">expirationDate</param>
        /// <param name="beginStudy">beginStudy</param>
        /// <param name="endStudy">endStudy</param>
        /// <param name="alertId">alertId</param>
        /// <returns>Id Of insertion</returns>
        public virtual int InsertAlertOccurrenceData(DateTime expirationDate, string beginStudy, string endStudy, int alertId)
        {
            string insertCommand = String.Format("INSERT INTO {0} VALUES({1}.SEQ_ALERT_OCCURENCE.NEXTVAL, {2}, TO_DATE('{3}', 'YYYY-MM-DD'), SYSDATE, TO_DATE('{4}', 'YYYYMMDD'), TO_DATE('{5}', 'YYYYMMDD'))",
                                                 DataBaseConfiguration.DataBase.GetTable(TableIds.alertOccurence).Sql, DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix, alertId,
                                                 expirationDate.ToString("yyyy-MM-dd"), beginStudy, endStudy);

            this._src.Insert(insertCommand);
            return (0);
        }
        #endregion

        #region GetAlert
        /// <summary>
        /// Retrieve an alert according to the given alert identifier
        /// </summary>
        /// <param name="alertId">Alert identifier</param>
        /// <returns>An TNS.Alert.Domain.Alert object</returns>
        public virtual TNS.Alert.Domain.Alert GetAlert(int alertId)
        {
            StringBuilder sql = new StringBuilder(500);

            sql.AppendFormat("SELECT {0}.id_alert, {0}.alert, {0}.id_login, {0}.email_list, {0}.date_creation, {1}.date_end_module as date_end, {0}.date_validate, {0}.session_, {0}.id_type_periodicity, {0}.expiry, {0}.id_alert_schedule ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("FROM {0}, {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).SqlWithPrefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).SqlWithPrefix);
            sql.AppendFormat("WHERE {0}.id_login = {1}.id_login ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("AND {0}.id_module = {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix,
                TNS.AdExpress.Constantes.Web.Module.Name.ALERT_ADEXPRESS.ToString());
            sql.AppendFormat("AND {0}.id_alert = {1} AND {0}.id_alert_type = {2} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                alertId,
                ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode());
            sql.AppendFormat("ORDER BY {0}.activation, {0}.id_alert_type ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix);


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
        #endregion

        #region GetNewAlerts
        /// <summary>
        /// Retrieves all the new alerts
        /// </summary>
        /// <returns>An alert collection</returns>
        public AlertCollection GetNewAlerts()
        {
            // See private method getNewAlerts to understand
            // that "-1" parameter
            return (GetNewAlerts(-1));
        }

        /// <summary>
        /// Retrieves all the new alerts with the given periodicity
        /// </summary>
        /// <param name="periodicity">Periodicity filter</param>
        /// <returns>An AlertCollection</returns>
        public AlertCollection GetNewAlerts(ConstDB.Alerts.AlertPeriodicity periodicity)
        {
            return (GetNewAlerts(periodicity.GetHashCode()));
        }
        #endregion 

        #region GetExpiredAlerts
        /// <summary>
        /// Retrieves all the expired alerts
        /// </summary>
        /// <returns>An AlertCollection</returns>
        public virtual AlertCollection GetExpiredAlerts()
        {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);

            sql.AppendFormat("SELECT {0}.id_alert, {0}.alert, {0}.id_login, {0}.email_list, {0}.date_creation, {1}.date_end_module as date_end, {0}.date_validate, {0}.session_, {0}.id_type_periodicity, {0}.expiry, {0}.id_alert_schedule ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("FROM {0}, {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).SqlWithPrefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).SqlWithPrefix);
            sql.AppendFormat("WHERE {0}.id_login = {1}.id_login ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("AND {0}.id_module = {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix,
                TNS.AdExpress.Constantes.Web.Module.Name.ALERT_ADEXPRESS.ToString());
            sql.AppendFormat("AND {0}.id_alert_type = {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode());
            sql.AppendFormat("AND {0}.date_end_module < SYSDATE ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("ORDER BY {0}.activation, {0}.id_alert_type ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix);

            // Retrieving data
            DataSet data = this._src.Fill(sql.ToString());
            this.FillFromDataTable(data.Tables[0]);
            return (this._alerts);
        }
        #endregion

        #region GetAlertHours
        /// <summary>
        /// Retrieves all the available Alert Hour
        /// </summary>
        /// <returns>An AlertHourCollection object</returns>
        public TNS.Alert.Domain.AlertHourCollection GetAlertHours() {

            // Creating select query
            StringBuilder sql = new StringBuilder(500);
            sql.AppendFormat("SELECT id_alert_schedule, hour_beginning ");
            sql.AppendFormat("FROM {0} ", DataBaseConfiguration.DataBase.GetTable(TableIds.alertSchedule).Sql);
            sql.AppendFormat("WHERE activation < {0} ", TNS.FrameWork.DB.Constantes.Activation.UNACTIVATED);
            sql.Append("ORDER BY hour_beginning ASC");

            DataSet data = this._src.Fill(sql.ToString());

            _alertHours = new AlertHourCollection();
            if (data.Tables[0] != null)
                foreach (DataRow dr in data.Tables[0].Rows)
                    this._alertHours.Add(new TNS.Alert.Domain.AlertHour(dr));

            return _alertHours;
        }
        #endregion

        #endregion

        #region Private Methods

        #region GetNewAlerts
        /// <summary>
        /// Retrieves all the new alerts with the given periodicity
        /// </summary>
        /// <param name="periodicity">Id Periodicity</param>
        /// <returns>AlertCollection Object</returns>
        private AlertCollection GetNewAlerts(int periodicity) {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);

            sql.AppendFormat("SELECT {0}.id_alert, {0}.alert, {0}.id_login, {0}.email_list, {0}.date_creation, {1}.date_end_module as date_end, {0}.date_validate, {0}.session_, {0}.id_type_periodicity, {0}.expiry, {0}.id_alert_schedule ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("FROM {0}, {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).SqlWithPrefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).SqlWithPrefix);
            sql.AppendFormat("WHERE {0}.id_login = {1}.id_login ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("AND {0}.id_module = {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix,
                TNS.AdExpress.Constantes.Web.Module.Name.ALERT_ADEXPRESS.ToString());
            sql.AppendFormat("AND {0}.DATE_VALIDATE IS NULL AND {0}.id_alert_type = {1} AND ACTIVATION = {2} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode(),
                ConstDB.Alerts.AlertStatuses.New.GetHashCode());
            if (periodicity != -1)
                sql.AppendFormat("AND {0}.ID_TYPE_PERIODICITY = {1} ", 
                    DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                    periodicity);
            sql.AppendFormat("ORDER BY {0}.activation, {0}.id_alert_type ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix);




            DataSet data = this._src.Fill(sql.ToString());
            this.FillFromDataTable(data.Tables[0]);
            return (this._alerts);
        }
        #endregion

        #region GetData
        /// <summary>
        /// Retrieves alerts that should be sent on the given DayOfWeek or DayOfMonth
        /// </summary>
        /// <param name="DayOfWeek">Day of the week (1 to 7)</param>
        /// <param name="DayOfMonth">Day of the month (1 to 28)</param>
        /// <returns></returns>
        protected AlertCollection GetData(int DayOfWeek, int DayOfMonth, Int64 hourBeginning) {
            // Creating select query
            StringBuilder sql = new StringBuilder(500);


            sql.AppendFormat("SELECT {0}.id_alert, {0}.alert, {0}.id_login, {0}.email_list, {0}.date_creation, {1}.date_end_module as date_end, {0}.date_validate, {0}.session_, {0}.id_type_periodicity, {0}.expiry, {0}.id_alert_schedule ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("FROM {0}, {1}, {2} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).SqlWithPrefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).SqlWithPrefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.alertSchedule).SqlWithPrefix);
            sql.AppendFormat("WHERE {0}.id_login = {1}.id_login ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("AND {0}.id_module = {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix,
                TNS.AdExpress.Constantes.Web.Module.Name.ALERT_ADEXPRESS.ToString());
            sql.AppendFormat("AND {0}.id_alert_schedule = {1}.id_alert_schedule ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.alertSchedule).Prefix);
            sql.AppendFormat("AND {0}.hour_beginning = {1} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alertSchedule).Prefix,
                hourBeginning.ToString());
            sql.AppendFormat("AND {0}.date_end_module >= SYSDATE ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Prefix);
            sql.AppendFormat("AND {0}.activation = {2} AND {1}.activation = {2} AND {0}.id_alert_type = {3} ",
                DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix,
                DataBaseConfiguration.DataBase.GetTable(TableIds.alertSchedule).Prefix,
                ConstDB.Alerts.AlertStatuses.Activated.GetHashCode(),
                ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode());
            sql.Append("AND ( ");
            sql.AppendFormat("  ({0}.id_type_periodicity = {1}) OR ", DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix, ConstDB.Alerts.AlertPeriodicity.Daily.GetHashCode());
            sql.AppendFormat("  ({0}.id_type_periodicity = {1} AND {0}.expiry = {2}) OR ", DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix, ConstDB.Alerts.AlertPeriodicity.Weekly.GetHashCode(), DayOfWeek);
            sql.AppendFormat("  ({0}.id_type_periodicity = {1} AND {0}.expiry = {2})", DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Prefix, ConstDB.Alerts.AlertPeriodicity.Monthly.GetHashCode(), DayOfMonth);
            sql.Append(" ) ");


            // Retrieving data
            this._data = this._src.Fill(sql.ToString());

            // Filling the collection
            this.FillFromDataTable(this._data.Tables[0]);

            // Returning the data set
            return (this._alerts);
        }
        #endregion

        #region FillFromDataTable
        /// <summary>
        /// Fill an AlertCollection object using the given DataTable
        /// </summary>
        /// <param name="data">DataTable</param>
        private void FillFromDataTable(DataTable data) {
            if (this._alerts != null)
                this._alerts.Clear();
            else
                this._alerts = new AlertCollection();

            if (data != null)
                foreach (DataRow dr in data.Rows)
                    this._alerts.Add(new TNS.Alert.Domain.Alert(dr));
        }
        #endregion

        #region FillOccurencesFromDataTable
        /// <summary>
        /// Fills an AlertOccurrenceCollection object using
        /// the given DataTable
        /// </summary>
        /// <param name="data">Data</param>
        private void FillOccurencesFromDataTable(DataTable data) {
            if (this._alertOccurrences != null)
                this._alertOccurrences.Clear();
            else
                this._alertOccurrences = new AlertOccurenceCollection();

            foreach (DataRow row in data.Rows)
                this._alertOccurrences.Add(new AlertOccurence(row));
        }
        #endregion

        #endregion
    }
}
