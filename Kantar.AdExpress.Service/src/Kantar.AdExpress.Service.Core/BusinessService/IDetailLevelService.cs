using Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IDetailLevelService
    {
        List<DetailLevel> GetDetailLevelItem(string idWebSession, int vehicleId, bool isVehicleChanged, HttpContextBase httpContext);

        void SetDetailLevelItem(string idWebSession, UserFilter userFilter, HttpContextBase httpContext);
    }
}
