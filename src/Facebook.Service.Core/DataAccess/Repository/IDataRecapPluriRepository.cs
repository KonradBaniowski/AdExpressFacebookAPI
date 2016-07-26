using Facebook.Service.Core.DomainModels.BusinessModel;
using Facebook.Service.Core.DomainModels.RecpaSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Core.DataAccess.Repository
{
    public interface IDataRecapPluriRepository : IGenericRepository<RecapPluri>
    {
        List<RecapPluriExpenditure> GetDataRecapPluri(List<CriteriaData> Criteria, long Begin, long End, List<long> Advertiser, List<long> Brand, int idLanguage);
    }
}
