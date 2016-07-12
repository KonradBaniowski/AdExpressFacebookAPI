using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DomainModels.AdExprSchema
{
    public class WebPlanMediaMonth : Data
    {
        public long IdProduct { get; set; }
        public Nullable<long> TotalUnite { get; set; }
        public long MonthMediaNum { get; set; }
    }
}
