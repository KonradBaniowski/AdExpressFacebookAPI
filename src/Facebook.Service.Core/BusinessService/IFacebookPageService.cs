using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using System;
using System.Collections.Generic;

namespace Facebook.Service.Core.BusinessService
{
    public interface IFacebookPageService
    {
        List<DataFacebookContract> GetDataFacebook();

        List<DataFacebookContract> GetDataFacebook(int IdLogin, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage);
    }
}
