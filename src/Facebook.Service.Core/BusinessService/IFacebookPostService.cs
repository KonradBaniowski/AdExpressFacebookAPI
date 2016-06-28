using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.BusinessService
{
    public interface IFacebookPostService
    {
        List<DataPostFacebookContract> GetDataPostFacebook(int idLogin, long begin, long end, List<long> advertisers, List<long> brands, List<long> posts);
    }
}
