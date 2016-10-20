using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;
using System.Web;
using TNS.AdExpress.Domain.Results;
using TNS.Classification.Universe;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IWebSessionService
    {
        WebSessionResponse SaveMediaSelection(SaveMediaSelectionRequest request, HttpContextBase httpContext);

        WebSessionResponse SaveMarketSelection(SaveMarketSelectionRequest request, HttpContextBase httpContext);

        void SaveCurrentModule(string webSessionId, int moduleId, HttpContextBase httpContext);

        void UpdateSiteLanguage(string webSessionId, int siteLanguage);
        int GetSiteLanguage(string webSessionId);
        WebSessionDetails GetWebSession(string webSessionId);

        bool IsAllSelectionStep(string webSessionId);

        PostModel GetPostModel(string webSessionId,string period="");
    }
}
