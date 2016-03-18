using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class SubPeriodItem
    {
        public bool IsSelected { get; set; }

        public string Label { get; set; }

        public string PeriodLabel { get; set; }

        public string Period { get; set; }
    }
}
