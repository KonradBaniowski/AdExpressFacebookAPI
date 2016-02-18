using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IRightService
    {
        Dictionary<long, Core.Domain.Module> GetModules(string idWSession);

        Dictionary<long, Core.Domain.Module> GetModulesList(string idWSession);
    }
}
