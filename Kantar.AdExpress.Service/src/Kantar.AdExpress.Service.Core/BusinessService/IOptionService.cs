﻿using Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IOptionService
    {
        Options GetOptions(string idWebSession, HttpContextBase httpContext);
        void SetOptions(string idWebSession, UserFilter userFilter, HttpContextBase httpContext);
        SaveLevelsResponse SaveCustomDetailLevels(string idWebSession, string detailLevel, string detailLevelType, HttpContextBase httpContext);
        string RemoveCustomDetailLevels(string idWebSession, string detailLevel);
    }
}
