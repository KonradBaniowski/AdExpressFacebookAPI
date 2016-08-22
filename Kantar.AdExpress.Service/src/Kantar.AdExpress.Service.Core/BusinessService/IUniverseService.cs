using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;
using TNS.Classification.Universe;


namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IUniverseService
    {
        List<UniversItem> GetItems(SearchRequest request, out int nbItem);

        List<UniversItem> GetItems(int levelId, string selectedClassificationItemsIds, int selectedLevelId, string idSession, Dimension dimension, List<int> idMedias, out int nbItems);

        UniversBranchResult GetBranches(string webSessionId, Dimension dimension, bool selectionPage = true, int MaxIncludeNbr = 2, int MaxExcludeNbr = 1);

        UniversGroupsResponse GetUserSavedUniversGroups(string webSessionId, Dimension dimension, bool selectionPage=true);

        UniversResponse GetTreesByUserUnivers(int userUniversId, string webSessionId, Dimension dimension);

        UniversGroupSaveResponse SaveUserUnivers(UniversGroupSaveRequest request);
        UniversGroupsResponse GetUserUniversGroups(string webSessionId, Dimension dimension, long idGroup=0);

        AdExpressUniversResponse GetResultUnivers(string webSessionId);
        AdExpressUniversResponse GetUnivers(string webSessionId, string branch, string listUniverseClientDescription);
        AlertResponse GetUserAlerts(string webSessionId);
        string SaveUserResult(string webSessionId, string folderId, string saveAsResultId, string saveResult);
        List<UniversItem> GetGategoryItems( SearchItemsCriteria search, out int nbItems, Dimension dimension = Dimension.product);

        List<UserUnivers> GetUniverses(Dimension dimension, string webSessionId);

        void ChangeMarketUniverse(long universeId, string webSessionId);
    }
}
