﻿using Kantar.AdExpress.Service.Core.Domain.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core
{
   public interface IPeriodService
    {
        PeriodResponse GetPeriod(string idWebSession);
    }
}
