using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class UnitSelectControl : SelectControl
    {
        public List<string> SelectedIds { get; set; }

        public bool EnableMultiple { get; set; }

        public List<GroupItems> Groups { get; set; }

        public bool EnableGroups { get; set; }
    }
}
