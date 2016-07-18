using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Contract.ContractModels.ModuleFacebook
{
   public class KPIClassificationContract
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public string Month { get; set; }
        public long Like { get; set; }
        public long Share { get; set; }
        public long Comment { get; set; }
        public long Post { get; set; }
        public long Expenditure { get; set; }
        public long Commitment { get; set; }

    }
}
