
using System.Collections.Generic;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IPortfolioService
    {
        GridResult GetGridResult(string idWebSession);

        ResultTable GetResultTable(string idWebSession);
        List<GridResult> GetGraphGridResult(string idWebSession);
    }
}
