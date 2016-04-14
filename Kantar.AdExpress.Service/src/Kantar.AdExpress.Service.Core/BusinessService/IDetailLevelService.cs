using Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IDetailLevelService
    {
        List<DetailLevel> GetDetailLevelItem(string idWebSession, int vehicleId, bool isVehicleChanged);

        void SetDetailLevelItem(string idWebSession, UserFilter userFilter);
    }
}
