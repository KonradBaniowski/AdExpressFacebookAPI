using System.Collections.Generic;
using TNS.AdExpress.Domain.Results;
using TNS.Ares.Constantes;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class PortfolioAlertResultResponse
    {
        public List<PortfolioAlertData> Datas { get; set; }
        public PortfolioAlertReminder Reminder { get; set; }
        public string CouvPath { get; set; }
        public string LienCheminDeFer { get; set; }
    }
}
