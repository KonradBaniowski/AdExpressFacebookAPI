using System.Web;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface ILostWonService
    {
        GridResult GetGridResult(string idWebSession, HttpContextBase httpContext);

        ResultTable GetResultTable(string idWebSession, HttpContextBase httpContext);
    }
}
