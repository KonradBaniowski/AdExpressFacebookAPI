using System;
using System.Collections.Generic;
using System.Text;
using TNS.Alert.Domain;

using ConstAres = TNS.Ares.Constantes.Constantes;

namespace TNS.Ares.Alerts.DAL
{
    public interface IAlertDAL
    {
        AlertCollection GetNewAlerts();
        AlertCollection GetExpiredAlerts();
        AlertCollection GetAlerts(long loginId);
        AlertCollection GetAlerts(int DayOfWeek, int DayOfMonth, Int64 hourBeginning);
        AlertCollection GetNewAlerts(ConstAres.Alerts.AlertPeriodicity periodicity);

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
        void UpdateStatus(int alertId, ConstAres.Alerts.AlertStatuses status, bool updateActivationDate);

        int InsertAlertData(string title, byte[] binaryData, Int64 moduleId, ConstAres.Alerts.AlertPeriodicity type, int occurrenceParameter, string recepients, Int64 idLogin, Int64 idAlertSchedule);

        int InsertAlertOccurrenceData(DateTime expirationDate, string beginStudy, string endStudy, int alertId);

        TNS.Alert.Domain.Alert GetAlert(int alertId);

        TNS.Alert.Domain.AlertHourCollection GetAlertHours();
    }
}
