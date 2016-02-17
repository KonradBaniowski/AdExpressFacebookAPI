using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpress.Web.Core.Sessions;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class RightService : IRightService
    {
        private WebSession webSession = null;

        public Dictionary<long, Module> GetModule(string idWSession)
        {
            webSession = (WebSession)WebSession.Load(idWSession);
            var stuff = webSession.CustomerLogin.GetCustomerModuleListHierarchy();

            return new Dictionary<long, Module>();
        }
    }
}
