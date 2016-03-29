﻿using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;
using TNS.Classification.Universe;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IWebSessionService
    {
        WebSessionResponse SaveMediaSelection(List<long> mediaIds, string webSessionId, List<Tree> trees, Dimension dimension, Security security, bool mediaSupportRequired);

        WebSessionResponse SaveMarketSelection(string webSessionId, List<Tree> trees, Dimension dimension, Security security);

        void SaveCurrentModule(string webSessionId, int moduleId);
    }
}
