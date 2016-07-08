using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Contract.ContractModels.ModuleFacebook
{
    public class KPIPageFacebookContract
    {
        public string Month { get; set; }
        public long Like { get; set; }
        public long Share { get; set; }
        public long Comment { get; set; }
        public long Post { get; set; }
    }
}
