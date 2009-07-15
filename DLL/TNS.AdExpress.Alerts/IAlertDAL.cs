using System;
using System.Collections.Generic;
using System.Text;
using TNS.Alert.Domain;

using ConstDB = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Alerts
{
    public interface IAlertDAL
    {
        AlertCollection GetNewAlerts();
        AlertCollection GetExpiredAlerts();
        AlertCollection GetAlerts(long loginId);
        AlertCollection GetAlerts(int DayOfWeek, int DayOfMonth);
        AlertCollection GetNewAlerts(ConstDB.Alerts.AlertPeriodicity periodicity);

        void DeleteOccurrences(int alertId);

        AlertOccurence GetOccurrence(int occurrenceId);
        AlertOccurence GetOccurrence(int occurrenceId, int alertId);

        AlertOccurenceCollection GetOccurrences(int alertId);
        AlertOccurenceCollection GetExpiredAlertOccurences();

        void DeleteExpiredAlerts();
        void Activate(int alertId);
        void Disactivate(int alertId);
        void DeleteExpiredAlertOccurences();
        void Delete(int alertId, bool logical);
        void UpdateStatus(int alertId, ConstDB.Alerts.AlertStatuses status, bool updateActivationDate);

        int InsertAlertData(string title, object session, ConstDB.Alerts.AlertPeriodicity type, int occurrenceParameter, string recepients, long idLogin);

        int InsertAlertOccurrenceData(DateTime expirationDate, string beginStudy, string endStudy, int alertId);

        TNS.Alert.Domain.Alert GetAlert(int alertId);
    }
}
