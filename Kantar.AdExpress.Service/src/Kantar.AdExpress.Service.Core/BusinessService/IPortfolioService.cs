
using Kantar.AdExpress.Service.Core.Domain.Creative;
using System.Collections.Generic;
using System.Web;
using TNS.AdExpress.Domain.Results;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.Core.BusinessService
{
    public interface IPortfolioService
    {
        GridResult GetGridResult(string idWebSession, HttpContextBase httpContext);

        ResultTable GetResultTable(string idWebSession, HttpContextBase httpContext);

        List<GridResult> GetGraphGridResult(string idWebSession, HttpContextBase httpContext);

        List<VehicleCover> GetVehicleCovers(string idWebSession, int resultType, HttpContextBase httpContext);

        List<VehiclePage> GetVehiclePages(string idWebSession,string mediaId, string dateMediaNum, string dateCoverNum, string nbPage, string media, HttpContextBase httpContext, string subFolder = null);
    }
}
