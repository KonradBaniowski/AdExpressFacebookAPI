using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.BusinessService
{
    public interface IKPIFacebookPageService
    {
        List<KPIPageFacebookContract> GetKPIPages(int IdLogin, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage);

        List<KPIClassificationContract> GetKPIClassificationPages(int idLogin, long begin, long end, List<long> advertisers, List<long> brands, int idLanguage);

        List<KPIPercentPageFacebookContract> GetKPIPlurimediaPages(int IdLogin, long Begin, long End, List<long> AdvertiserRef, List<long> AdvertiserCon, List<long> BrandRef, List<long> BrandCon, int idLanguage);

        List<PDVByMediaPageFacebookContract> GetKPIPlurimediaStacked(int IdLogin, long Begin, long End, List<long> AdvertiserRef, List<long> AdvertiserCon, List<long> BrandRef, List<long> BrandCon, int idLanguage);
    }
}
