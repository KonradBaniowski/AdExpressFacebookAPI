﻿using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IInfosNewsService
    {
        List<Documents> GetInfosNews(string idWebSession, HttpContextBase httpContext);
    }
}
