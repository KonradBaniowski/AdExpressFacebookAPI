using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class SubPeriod
    {
        public string LeaveZoom { get; set; }

        public string PeriodLabel { get; set; }

        public string FirstPeriodLabel { get; set; }

        public string LastPeriodLabel { get; set; }

        public string BeginPeriodLabel { get; set; }

        public string EndPeriodLabel { get; set; }

        public string AllPeriodLabel { get; set; }

        public string AllLabel { get; set; }

        public List<SubPeriodItem> Items { get; set; }

        public bool HideExistButton { get; set; }
    }
}
