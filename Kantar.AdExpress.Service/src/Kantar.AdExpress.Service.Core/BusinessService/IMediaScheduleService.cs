using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IMediaScheduleService
    {
        object[,] GetMediaScheduleData(string idWebSession);
        GridResult GetGridResult(string idWebSession, string zoomDate);
        MSCreatives GetMSCreatives(string idWebSession, string zoomDate);
        void SetMSCreatives(string idWebSession, ArrayList slogans);
        void SetProductLevel(string idWebSession, Int64 id, int level);
    }
}
