using System;

using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IPresentAbsentService
    {
        GridResult GetGridResult(string idWebSession);

        ResultTable GetResultTable(string idWebSession);
    }
}
