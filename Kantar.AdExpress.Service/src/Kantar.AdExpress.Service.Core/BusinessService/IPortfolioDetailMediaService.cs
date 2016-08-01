﻿using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
   public interface IPortfolioDetailMediaService
    {
        GridResult GetDetailMediaGridResult(string idWebSession, string idMedia,string dayOfWeek,string ecran);
        bool IsIndeRadioMessage(string idWebSession);

    }
}