using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class OptionsMediaSchedule
    {
        public GenericDetailLevelOption GenericDetailLevel { get; set; }

        public PeriodDetailOption PeriodDetail { get; set; }

        public int SiteLanguage { get; set; }

        public CheckBoxOption Grp { get; set; }

        public CheckBoxOption Grp30S { get; set; }

        public CheckBoxOption SpendsGrp { get; set; }

        public bool SpendsSelected { get; set; }
    }
}
