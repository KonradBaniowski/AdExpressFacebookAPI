using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class UserCriteria
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


        public UserCriteria()
        {
            CanalIds = new List<double>();
            CategoryIncludeIds = new List<double>();
            CategoryExcludeIds = new List<double>();
            ProductIncludeIds = new List<double>();
            ProductExcludeIds = new List<double>();
            GrpPharmaIncludeIds = new List<double>();
            GrpPharmaExcludeIds = new List<double>();
        }

    }
}
