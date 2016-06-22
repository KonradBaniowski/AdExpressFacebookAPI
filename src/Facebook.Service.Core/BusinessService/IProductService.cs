using Facebook.Service.Contract.ContractModels.ModuleFacebook;using System;
using System.Collections.Generic;

namespace Facebook.Service.Core.BusinessService
{
    public interface IProductService
    {
        List<LevelItemContract> GetItems(string keyword, int level);
        List<LevelItemContract> GetItems(int level, string selectedItemIds, int selectedLevel);
    }
}
