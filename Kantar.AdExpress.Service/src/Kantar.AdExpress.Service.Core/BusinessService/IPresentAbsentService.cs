using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Web;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IPresentAbsentService
    {
        GridResultResponse GetGridResult(string idWebSession, HttpContextBase httpContext);

        ResultTable GetResultTable(string idWebSession, HttpContextBase httpContext);

        long CountDataRows(string idWebSession, HttpContextBase httpContext);
    }
}
