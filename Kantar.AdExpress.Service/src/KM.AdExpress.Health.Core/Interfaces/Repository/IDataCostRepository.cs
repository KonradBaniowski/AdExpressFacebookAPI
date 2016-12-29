using KM.AdExpress.Health.Core.Model;
using KM.AdExpress.Health.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Core.Interfaces.Repository
{
   public interface IDataCostRepository :  IGenericRepository<DataCost>
    {
        List<DataCostContract> GetData(UserCriteriaContract userContract);
    }
}
