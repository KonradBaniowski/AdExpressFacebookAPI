
using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;
using System.Web;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IMediaService
    {
        MediaResponse GetMedia(string idWebSession, HttpContextBase httpContext);

        SponsorshipMediaResponse GetSponsorshipMedia(string idWebSession, HttpContextBase httpContext);

        HealthMediaResponse GetHealthMedia(string idWebSession, HealthMediaRequest healthMediaRequest);
    }
}
