using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class Options
    {
        public GenericDetailLevelOption GenericDetailLevel { get; set; }

        public PeriodDetailOption PeriodDetail { get; set; }

        public UnitOption UnitOption { get; set; }
    }
}
