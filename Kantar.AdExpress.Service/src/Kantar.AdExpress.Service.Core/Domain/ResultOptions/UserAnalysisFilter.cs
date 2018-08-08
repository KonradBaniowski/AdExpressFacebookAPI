using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class UserAnalysisFilter
    {
        public GenericLevelFilter MediaDetailLevel { get; set; }

        public GenericLevelFilter ProductDetailLevel { get; set; }

        public ResultTypeFilter ResultTypeFilter { get; set; }

        public UnitFilter UnitFilter { get; set; }

        public bool Evol { get; set; }

        public bool PDM { get; set; }

        public bool PDV { get; set; }

        public bool IsSelectRetailerDisplay { get; set; }
    }
}
