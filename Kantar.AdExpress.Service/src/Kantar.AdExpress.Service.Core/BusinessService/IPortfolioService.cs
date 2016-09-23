﻿
using Kantar.AdExpress.Service.Core.Domain.Creative;
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

        List<VehicleCover> GetVehicleCovers(string idWebSession, int resultType);

        List<VehiclePage> GetVehiclePages(string idWebSession,string mediaId, string dateMediaNum, string dateCoverNum, string nbPage, string media, string subFolder = null);
    }
}
