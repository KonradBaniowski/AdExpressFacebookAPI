﻿using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;
using TNS.Classification.Universe;


namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IUniverseService
    {
        List<UniverseItem> GetItems(int universeLevelId, string keyWord, string idSession);

        List<UniverseItem> GetItems(int levelId, string selectedClassificationItemsIds, int selectedLevelId, string idSession);

        UniversBranchResult GetBranches(string webSessionId, Dimension dimension, bool selectionPage);

        List<UserUniversGroup> GetUserSavedUniversGroups(string webSessionId, Dimension dimension, bool selectionPage=true);
    }
}
