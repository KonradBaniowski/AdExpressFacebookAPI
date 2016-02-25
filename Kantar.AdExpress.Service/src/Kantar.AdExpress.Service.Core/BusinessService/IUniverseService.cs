using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;


namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IUniverseService
    {
        List<UniverseItem> GetItems(int universeLevelId, string keyWord, string idSession);

        List<UniverseItem> GetItems(int levelId, string selectedClassificationItemsIds, int selectedLevelId, string idSession);

        List<UniversBranch> GetBranches(string webSessionId);

    }
}
