using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IOptionMediaScheduleService
    {
        OptionsMediaSchedule GetOptions(string idWebSession, bool isAdNetTrack);
        void SetOptions(string idWebSession, UserFilter userFilter, bool isAdNetTrack);
    }
}
