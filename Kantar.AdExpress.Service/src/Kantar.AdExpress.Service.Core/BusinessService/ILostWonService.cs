using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface ILostWonService
    {
        GridResult GetGridResult(string idWebSession);

        ResultTable GetResultTable(string idWebSession);
    }
}
