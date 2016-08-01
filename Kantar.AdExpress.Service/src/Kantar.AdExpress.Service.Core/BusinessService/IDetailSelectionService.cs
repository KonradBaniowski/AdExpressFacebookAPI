﻿using Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core.Domain.DetailSelectionDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IDetailSelectionService
    {
        DetailSelectionResponse GetDetailSelection(string idWebSession);
        DetailSelectionResponse LoadSessionDetails(string idSession, string idWebSession);
        DetailSelectionResponse LoadUniversDetails(string idUnivers, string idWebSession);
        DetailSelectionResponse LoadAlertDetails(string idAlert, string idWebSession);
        List<Tree> GetMarket(string idWebSession);
    }
}