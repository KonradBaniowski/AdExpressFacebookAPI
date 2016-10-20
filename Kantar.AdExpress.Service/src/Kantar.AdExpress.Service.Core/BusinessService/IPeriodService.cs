using Kantar.AdExpress.Service.Core.Domain.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kantar.AdExpress.Service.Core
{
   public interface IPeriodService
    {
        PeriodResponse GetPeriod(string idWebSession, HttpContextBase httpContext);

        PeriodResponse CalendarValidation(PeriodSaveRequest request, HttpContextBase httpContext);

        PeriodResponse SlidingDateValidation(PeriodSaveRequest request, HttpContextBase httpContext);
    }
}
