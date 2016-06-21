using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.BusinessService
{
    public interface IFacebookPageService
    {
        List<DataFacebookContract> GetDataFacebook();
    }
}
