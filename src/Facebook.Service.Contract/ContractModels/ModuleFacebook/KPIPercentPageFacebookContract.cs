using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Contract.ContractModels.ModuleFacebook
{
    public class KPIPercentPageFacebookContract
    {
        public string Month { get; set; }
        public long ReferentPercent { get; set; }
        public long ConcurrentPercent { get; set; }
        public long ReferentFBPercent { get; set; }
        public long ConcurrentFBPercent { get; set; }
    }
}
