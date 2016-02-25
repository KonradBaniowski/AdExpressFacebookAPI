using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpressI.MediaSchedule;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IMediaSchedule
    {
        MediaScheduleData GetMediaScheduleData(string idWebSession);
    }
}
