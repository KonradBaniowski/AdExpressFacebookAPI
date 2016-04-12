using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Web.Navigation;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class Alert
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Recipients { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime ValidationDate { get; set; }
        public AlertStatus Status { get; set; }
        public Periodicity Periodicity { get; set; }
        public string PeriodicityDescription { get; set; }//Daily, Weekly, Monthly
        public int PeriodicityValue { get; set; } // on 21st
        public string Frequency { get; set; }//EveryDay, EveryWeek, EveryMonth
        public Int64 IdAlertSchedule { get; set; }
        public List<Occurence> Occurrences { get; set; }
        public long IdModule { get; set; }
        public string TimeSchedule { get; set; }
        public string Module { get; set; }
    }
    public enum Periodicity
    {
        Daily = 10,
        Weekly = 20,
        Monthly = 30
    }
    public enum AlertStatus
    {
        Activated = 0,
        New = 10,
        ToDelete = 50
    }
    public enum AlertType
    {
        Portfolio = 1,
        AdExpressAlert = 2
    }
    public class AlertResponse
    {
        public List<Alert> Alerts { get; set; }
        public int SiteLanguage { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class Occurence
    {
        public int Id {get; set;}
        public int AlertId { get; set; }        
        public DateTime SendDate { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
