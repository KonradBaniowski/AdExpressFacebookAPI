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

        List<KPIPercentPageFacebookContract> GetKPIPlurimediaPages(int IdLogin, long Begin, long End, List<long> Advertiser, List<long> AdvertiserRef, List<long> Brand, List<long> BrandRef, int idLanguage);
    }
}
