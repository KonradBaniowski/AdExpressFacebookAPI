﻿using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;
using TNS.Classification.Universe;


namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IUniverseService
    {
        List<UniversItem> GetItems(int universeLevelId, string keyWord, string idSession, out int nbItem);

        List<UniversItem> GetItems(int levelId, string selectedClassificationItemsIds, int selectedLevelId, string idSession, out int nbItems);

        UniversBranchResult GetBranches(string webSessionId, Dimension dimension, bool selectionPage);

        UniversGroupsResponse GetUserSavedUniversGroups(string webSessionId, Dimension dimension, bool selectionPage=true);

        UniversResponse GetTreesByUserUnivers(int userUniversId, string webSessionId, Dimension dimension);

        UniversGroupSaveResponse SaveUserUnivers(UniversGroupSaveRequest request);
        UniversGroupsResponse GetUserUniversGroups(string webSessionId, Dimension dimension, long idGroup=0);
    }
}
