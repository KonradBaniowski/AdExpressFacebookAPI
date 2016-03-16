
using TNS.AdExpress.Domain.Results;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IPortfolioService
    {
        GridResult GetGridResult(string idWebSession);
    }
}
