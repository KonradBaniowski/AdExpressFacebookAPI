using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.DB.Common;
using Oracle.DataAccess.Client;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ConstDB = TNS.Ares.Constantes.Constantes;
using TNS.Ares.Domain.LS;
using TNS.Ares.Domain.DataBaseDescription;
using TNS.Ares.Domain.DataBase;

namespace TNS.Ares.Alerts.DAL.Default
{
    /// <summary>
    /// Alert DAL Default Layer
    /// </summary>
    public class AlertsDAL : TNS.Ares.Alerts.DAL.AlertsDAL {

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="src">Source</param>
        public AlertsDAL(IDataSource src) : base(src)
        { }
        #endregion

        #region InsertAlertData
        /// <summary>
        /// Inserts a new alert in the table
        /// </summary>
        /// <param name="session">Session</param>
        /// <param name="type">Periodicity type</param>
        /// <param name="expiry">Defines when the alert occurs (which day of week/month)</param>
        /// <param name="recepients">Email addresses separated by commas</param>
        public override int InsertAlertData(string title, byte[] binaryData, Int64 moduleId, ConstDB.Alerts.AlertPeriodicity type, int expiry, string recepients, long idLogin, Int64 idAlertSchedule)
        {
            int idAlert = -1;

            // Opening connection
            OracleConnection connection = (OracleConnection) this._src.GetSource();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            StringBuilder insertCommand;

            try {
                OracleCommand command = connection.CreateCommand();

                // Preparing PL/SQL block
                insertCommand = new StringBuilder();
                insertCommand.Append("BEGIN ");
                insertCommand.AppendFormat("SELECT {0}.SEQ_ALERT.NEXTVAL INTO :new_id FROM dual; ", DataBaseConfiguration.DataBase.GetSchema(SchemaIds.alert).Label);
                insertCommand.AppendFormat("INSERT INTO {0} ", DataBaseConfiguration.DataBase.GetTable(TableIds.alert).Sql);
                insertCommand.Append("(ID_ALERT, ID_MODULE, ID_LOGIN, ALERT, EMAIL_LIST, ID_ALERT_TYPE, ID_USER_, DATE_BEGINNING, DATE_END, DATE_CREATION, DATE_MODIFICATION, ACTIVATION, ID_TYPE_PERIODICITY, EXPIRY, ID_ALERT_SCHEDULE, SESSION_) ");
                insertCommand.Append("VALUES ");
                insertCommand.AppendFormat("(:new_id, {0}, {1}, :title, :recepients, {2}, 1079, SYSDATE, (SELECT DATE_END_MODULE FROM {7} WHERE id_module={8} AND id_login = {1}), SYSDATE, SYSDATE, {3}, {4}, {5}, {6}, :blobtodb); ",
                        moduleId.ToString(),
                        idLogin.ToString(),
                        ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode(),
                        ConstDB.Alerts.AlertStatuses.New.GetHashCode(),
                        type.GetHashCode(),
                        expiry.ToString(),
                        idAlertSchedule.ToString(),
                        DataBaseConfiguration.DataBase.GetTable(TableIds.rightModuleAssignment).Sql,
                        TNS.AdExpress.Constantes.Web.Module.Name.ALERT_ADEXPRESS.ToString());
                insertCommand.Append("END; "); ;

                command.CommandText = insertCommand.ToString();

                // Fill parametres
                OracleParameter param2 = command.Parameters.Add("new_id", OracleDbType.Int64);
                param2.Direction = ParameterDirection.Output;
                OracleParameter param3 = command.Parameters.Add("title", OracleDbType.Varchar2);
                param3.Value = title;
                param3.Direction = ParameterDirection.Input;
                OracleParameter param4 = command.Parameters.Add("recepients", OracleDbType.Varchar2);
                param4.Value = recepients;
                param4.Direction = ParameterDirection.Input;
                OracleParameter param1 = command.Parameters.Add("blobtodb", OracleDbType.Blob);
                param1.Value = binaryData;
                param1.Direction = ParameterDirection.Input;

                // Execute PL/SQL block
                command.ExecuteNonQuery();
                idAlert = int.Parse(param2.Value.ToString());
            }
            catch (System.Exception e) {
            }
            finally
            {
                // Fermeture des structures
                connection.Close();
            }

            return (idAlert);
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
        public override int InsertAlertOccurrenceData(System.DateTime expirationDate, string beginStudy, string endStudy, int alertId)
        {
            // Study dates' format
            string formatStudyDates = "YYYYMMDD".Substring(0, beginStudy.Length);

            // Getting OracleConnection
            OracleConnection connection = (OracleConnection)this._src.GetSource();

            // Opening connection
            if (connection.State != ConnectionState.Open)
                connection.Open();

            // Creating command
            OracleCommand command = connection.CreateCommand();

            // Preparing PL/SQL block
            StringBuilder insertCommand = new StringBuilder();
            insertCommand.Append("BEGIN ");
            insertCommand.AppendFormat("SELECT {0}.SEQ_ALERT_OCCURENCE.NEXTVAL INTO :new_id FROM dual; ",DataBaseConfiguration.DataBase.GetSchema(SchemaIds.alert).Label);
            insertCommand.AppendFormat("INSERT INTO {0} ", DataBaseConfiguration.DataBase.GetTable(TableIds.alertOccurence).Sql);
            insertCommand.AppendFormat("(id_alert_occurence, id_alert, date_expiration, date_send, date_begin_study, date_end_study) ");
            insertCommand.AppendFormat(" VALUES ");
            insertCommand.AppendFormat(" (:new_id, {0}, TO_DATE('{1}', 'YYYY-MM-DD'), SYSDATE, TO_DATE('{2}', '{4}'), TO_DATE('{3}', '{4}')); ",
                                                 alertId, 
                                                 expirationDate.ToString("yyyy-MM-dd"), 
                                                 beginStudy,
                                                 endStudy, 
                                                 formatStudyDates);
            insertCommand.Append("END; ");

            // Setting parameter output to get the
            // occurrence id
            OracleParameter param = command.Parameters.Add("new_id", OracleDbType.Int64);
            param.Direction = ParameterDirection.Output;
            command.CommandText = insertCommand.ToString();

            // Executing SQL command
            command.ExecuteNonQuery();

            // Returning occurrence's id
            return (int.Parse(param.Value.ToString()));
        }
        #endregion
    }
}
