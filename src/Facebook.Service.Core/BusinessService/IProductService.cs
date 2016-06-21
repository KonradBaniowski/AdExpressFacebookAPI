using Facebook.Service.Contract.ContractModels.ModuleFacebook;using System;
using System.Collections.Generic;

namespace Facebook.Service.Core.BusinessService
{
    public interface IProductService
    {
        List<LevelItemContract> GetLevelItems(string keyword, int level);
    }
}
