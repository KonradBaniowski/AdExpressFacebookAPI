using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IWebSessionService
    {
        void SaveMediaSelection(List<int> mediaIds, string webSessionId);

        void SaveMarketSelection(string webSessionId);
    }
}
