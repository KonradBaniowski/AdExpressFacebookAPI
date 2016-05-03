using TNS.Ares.Constantes;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class SaveAlertRequest
    {
        public string AlertTitle { get; set; }
        public string Email { get; set; }
        public Constantes.Alerts.AlertPeriodicity Type { get; set; }
        public string OccurrenceDate { get; set; }
        public string IdWebSession { get; set; }
    }

    public class SaveAlertResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
