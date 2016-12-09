using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IMediaScheduleService
    {
        object[,] GetMediaScheduleData(string idWebSession, HttpContextBase httpContext);
        GridResultResponse GetMediaScheduleCreativeData(CreativeMediaScheduleRequest creativeMediaScheduleRequest);
        GridResult GetGridResult(string idWebSession, string zoomDate, HttpContextBase httpContext);
        GridResult GetGridResult(string idWebSession, string zoomDate, string isVehicle, HttpContextBase httpContext);
        GridResultResponse GetGridResult( CreativeMediaScheduleRequest creativeMediaScheduleRequest);
        MSCreatives GetMSCreatives(string idWebSession, string zoomDate, HttpContextBase httpContext);
        void SetMSCreatives(string idWebSession, ArrayList slogans);
        void SetProductLevel(string idWebSession, Int64 id, int level);
    }
}
