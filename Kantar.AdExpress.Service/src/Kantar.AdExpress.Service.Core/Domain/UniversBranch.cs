using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class UniversBranch
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public bool IsSelected { get; set; }
    }

    public class UniversBranchResult

    {
        public List<UniversBranch> Branches { get; set; }
        public int SiteLanguage { get; set; }
        public int DefaultBranchId { get; set; }
    }
}
