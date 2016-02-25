using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IUniverseService
    {
        List<UniverseItem> GetItems(int universeLevelId, string keyWord, string idSession);

        List<UniverseItem> GetItems(int levelId, string selectedClassificationItemsIds, int selectedLevelId, string idSession);
    }
}
