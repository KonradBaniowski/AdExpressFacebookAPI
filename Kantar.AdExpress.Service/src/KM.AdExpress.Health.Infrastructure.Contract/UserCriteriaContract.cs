using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Infrastructure.Contract
{
    public class UserCriteriaContract
    {
        public DateTime StartDate { get; set; }  
        public DateTime EndDate { get; set; }
        public List<double> CanalIds { get; set; }

        public List<double> CategoryIncludeIds { get; set; }
        public List<double> CategoryExcludeIds { get; set; }

        public List<double> ProductIncludeIds { get; set; }
        public List<double> ProductExcludeIds { get; set; }

        public List<double> GrpPharmaIncludeIds { get; set; }
        public List<double> GrpPharmaExcludeIds { get; set; }

    }
}
