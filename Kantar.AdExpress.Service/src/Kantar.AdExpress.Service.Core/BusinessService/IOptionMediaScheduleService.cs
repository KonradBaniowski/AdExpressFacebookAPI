using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IOptionMediaScheduleService
    {
        OptionsMediaSchedule GetOptions(string idWebSession, bool isAdNetTrack, HttpContextBase httpContext);
        void SetOptions(string idWebSession, UserFilter userFilter, bool isAdNetTrack, HttpContextBase httpContext);
    }
}
