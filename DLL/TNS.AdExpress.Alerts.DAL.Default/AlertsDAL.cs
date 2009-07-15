using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using Oracle.DataAccess.Client;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TNS.AdExpress.Constantes.DB;
using ConstDB = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Alerts.DAL.Default
{
    public class AlertsDAL : TNS.AdExpress.Alerts.DAL.AlertsDAL
    {
        public AlertsDAL(WebSession session) : base(session.Source)
        { }

        public AlertsDAL(IDataSource src) : base(src)
        { }

        public override int InsertAlertData(string title, object session, ConstDB.Alerts.AlertPeriodicity type, int expiry, string recepients, long idLogin)
        {
            int idAlert = -1;
            WebSession oSession = (WebSession)session;

            // Opening connection
            OracleConnection connection = (OracleConnection) this._src.GetSource();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            string insertCommand = "";
            MemoryStream ms = new MemoryStream(); ;
            BinaryFormatter bf = new BinaryFormatter();
            byte[] binaryData = null;

            try
            {
                OracleCommand command = connection.CreateCommand();

                // Serializing session
                bf.Serialize(ms, oSession);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();

                // Preparing PL/SQL block
                insertCommand = @"
                    BEGIN
                     SELECT " + Schema.ALERT_SCHEMA + @".SEQ_ALERT.NEXTVAL INTO :new_id FROM dual;
                     INSERT INTO " + Schema.ALERT_SCHEMA + "." + Tables.ALERT + 
                        "(ID_ALERT, ID_MODULE, ID_LOGIN, ALERT, EMAIL_LIST," +
                        " ID_ALERT_TYPE, ID_USER_, DATE_BEGINNING, DATE_END, DATE_CREATION, DATE_MODIFICATION, ACTIVATION, " +
                        " ID_TYPE_PERIODICITY, EXPIRY, SESSION_) " +
                        "VALUES(:new_id, " + oSession.CurrentModule.ToString() + "," + idLogin.ToString() + ", :title, :recepients, " +
                        ConstDB.Alerts.AlertType.AdExpressAlert.GetHashCode() + ", 1079, SYSDATE, SYSDATE, SYSDATE, SYSDATE, " + ConstDB.Alerts.AlertStatuses.New.GetHashCode() + ", " +
                        type.GetHashCode() + ", " + expiry.ToString() + ", :blobtodb);" +
                   "END;";

                command.CommandText = insertCommand;

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
            catch (System.Exception e)
            {
            }
            finally
            {
                // Fermeture des structures
                if (ms != null) ms.Close();
                if (bf != null) bf = null;
                connection.Close();
            }

            return (idAlert);
        }

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
            string insertCommand = @"
                    BEGIN
                     SELECT " + Schema.ALERT_SCHEMA + @".SEQ_ALERT_OCCURENCE.NEXTVAL INTO :new_id FROM dual;" +
                     String.Format("INSERT INTO {0}.{1} VALUES(:new_id, {2}, TO_DATE('{3}', 'YYYY-MM-DD'), SYSDATE, TO_DATE('{4}', '{5}'), TO_DATE('{6}', '{7}'));",
                                                 ConstDB.Schema.ALERT_SCHEMA, ConstDB.Tables.ALERT_OCCURENCE, alertId, expirationDate.ToString("yyyy-MM-dd"), beginStudy,
                                                 formatStudyDates, endStudy, formatStudyDates) + @"
                    END;";

            // Setting parameter output to get the
            // occurrence id
            OracleParameter param = command.Parameters.Add("new_id", OracleDbType.Int64);
            param.Direction = ParameterDirection.Output;
            command.CommandText = insertCommand;

            // Executing SQL command
            command.ExecuteNonQuery();

            // Returning occurrence's id
            return (int.Parse(param.Value.ToString()));
        }
    }
}
