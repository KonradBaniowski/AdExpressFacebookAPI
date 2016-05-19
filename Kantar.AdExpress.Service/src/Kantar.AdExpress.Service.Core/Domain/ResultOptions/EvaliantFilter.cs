using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class EvaliantFilter
    {
        public int IdFilter { get; set; }

        public List<string> ValuesFilter { get; set; }
    }
}
