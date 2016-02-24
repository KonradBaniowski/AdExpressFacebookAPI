using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;


namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IWebSessionService
    {
        WebSessionResponse SaveMediaSelection(List<long> mediaIds, string webSessionId);

        WebSessionResponse SaveMarketSelection(string webSessionId);

        void SaveCurrentModule(string webSessionId, int moduleId);
    }
}
