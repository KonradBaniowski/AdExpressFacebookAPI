using System;
using System.Collections.Generic;
using ConstantesSession = TNS.AdExpress.Constantes.Web.CustomerSessions;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class UnitFilter
    {
        public List<ConstantesSession.Unit> Unit { get; set; } = new List<ConstantesSession.Unit> { ConstantesSession.Unit.none };
    }
}
