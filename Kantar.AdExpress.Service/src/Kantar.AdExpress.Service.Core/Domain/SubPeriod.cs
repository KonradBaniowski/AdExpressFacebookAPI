﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class SubPeriod
    {
        public string PeriodLabel { get; set; }

        public string FirstPeriodLabel { get; set; }

        public List<SubPeriodItem> Items { get; set; }
    }
}