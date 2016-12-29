using KM.AdExpress.Health.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.Health.Core.Interfaces.Services
{
   public interface ICostService
    {
        List<DataCostContract> GetDataCost(UserCriteriaContract userCriteria);
    }
}
