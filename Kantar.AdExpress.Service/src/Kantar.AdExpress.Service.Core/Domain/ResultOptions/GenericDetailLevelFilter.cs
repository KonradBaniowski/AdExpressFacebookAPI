using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class GenericDetailLevelFilter
    {
        public int DefaultDetailValue { get; set; }

        public int CustomDetailValue { get; set; }

        public int L1DetailValue { get; set; }

        public int L2DetailValue { get; set; }

        public int L3DetailValue { get; set; }

        public int L4DetailValue { get; set; }
    }
}
