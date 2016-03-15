﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class UserFilter
    {
        public GenericDetailLevelFilter GenericDetailLevelFilter { get; set; }

        public PeriodDetailFilter PeriodDetailFilter { get; set; }

        public UnitFilter UnitFilter { get; set; }

        public InsertionFilter InsertionFilter { get; set; }
    }
}