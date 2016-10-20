using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;
using System.Web;
using TNS.Classification.Universe;


namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IUniverseService
    {
        List<UniversItem> GetItems(SearchRequest request, out int nbItem, HttpContextBase httpContext);

        List<UniversItem> GetItems(int levelId, string selectedClassificationItemsIds, int selectedLevelId, string idSession, Dimension dimension, List<int> idMedias, out int nbItems, HttpContextBase httpContext);

        UniversBranchResult GetBranches(string webSessionId, Dimension dimension, HttpContextBase httpContext, bool selectionPage = true, int MaxIncludeNbr = 2, int MaxExcludeNbr = 1);

        UniversGroupsResponse GetUserSavedUniversGroups(string webSessionId, Dimension dimension, HttpContextBase httpContext, bool selectionPage=true);

        UniversResponse GetTreesByUserUnivers(int userUniversId, string webSessionId, Dimension dimension, HttpContextBase httpContext);

        UniversGroupSaveResponse SaveUserUnivers(UniversGroupSaveRequest request, HttpContextBase httpContext);
        UniversGroupsResponse GetUserUniversGroups(string webSessionId, Dimension dimension, HttpContextBase httpContext, long idGroup=0);

        AdExpressUniversResponse GetResultUnivers(string webSessionId, HttpContextBase httpContext);
        AdExpressUniversResponse GetUnivers(string webSessionId, string branch, string listUniverseClientDescription, HttpContextBase httpContext);
        AlertResponse GetUserAlerts(string webSessionId, HttpContextBase httpContext);
        string SaveUserResult(string webSessionId, string folderId, string saveAsResultId, string saveResult, HttpContextBase httpContext);
        List<UniversItem> GetGategoryItems( SearchItemsCriteria search, out int nbItems, HttpContextBase httpContext, Dimension dimension = Dimension.product);

        List<UserUnivers> GetUniverses(Dimension dimension, string webSessionId, HttpContextBase httpContext);

        void ChangeMarketUniverse(long universeId, string webSessionId, HttpContextBase httpContext);
    }
}
