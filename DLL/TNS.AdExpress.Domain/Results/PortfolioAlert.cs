
using System.Collections.Generic;

namespace TNS.AdExpress.Domain.Results
{
    public class PortfolioAlert
    {
        public List<PortfolioAlertData> Datas{ get; set; }
        public PortfolioAlertReminder Reminder{ get; set; }
        public PortfolioAlertParams Parameters{ get; set; }
        public string CouvPath { get; set; }
        public string LienCheminDeFer { get; set; }
    }
}
