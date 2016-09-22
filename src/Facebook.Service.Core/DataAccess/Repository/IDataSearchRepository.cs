using Facebook.Service.Core.DomainModels.AdExprSchema;
using Facebook.Service.Core.DomainModels.BusinessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DataAccess.Repository
{
    public interface IDataSearchRepository : IGenericRepository<DataSearch>
    {
        List<DataSearch> GetDataSearchWithCriteria(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage);

        RecapPluriExpenditure GetLastLoadedMonth();
    }
}
