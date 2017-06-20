
using System.Collections.Generic;

namespace TNS.AdExpress.Domain.Results
{
    public class PortfolioAlertReminder
    {
        public bool Inset { get; set; }
        public bool Autopromo { get; set; }
        public List<string> Sectors { get; set; }
        public List<string> SubSectors { get; set; }
        public List<string> Groups { get; set; }
        public List<string> Segments { get; set; }
    }
}
