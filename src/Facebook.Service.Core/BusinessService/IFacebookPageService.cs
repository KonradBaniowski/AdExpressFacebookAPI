using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using System;
using System.Collections.Generic;

namespace Facebook.Service.Core.BusinessService
{
    public interface IFacebookPageService
    {
        List<DataFacebookContract> GetDataFacebook();

        List<DataFacebookContract> GetDataFacebook(DateTime Begin, DateTime End, List<int> Advertiser, List<int> Brand);
    }
}
