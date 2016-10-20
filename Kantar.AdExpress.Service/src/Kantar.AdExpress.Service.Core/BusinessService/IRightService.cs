using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IRightService
    {
        Dictionary<long, Core.Domain.Module> GetModules(string idWSession, HttpContextBase httpContext);

        Dictionary<long, Core.Domain.Module> GetModulesList(string idWSession, HttpContextBase httpContext);
    }
}
