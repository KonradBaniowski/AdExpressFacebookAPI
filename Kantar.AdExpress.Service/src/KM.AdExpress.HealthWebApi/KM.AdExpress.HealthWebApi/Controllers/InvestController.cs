using KM.AdExpress.Health.Core.Interfaces.Services;
using KM.AdExpress.Health.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KM.AdExpress.HealthWebApi.Controllers
{
    public class InvestController : ApiController
    {

        private ICostService _costsvc;

        public InvestController(ICostService costsvc)
        {
            _costsvc = costsvc;
        }

        [Route("Api/Invest")]
        [HttpPost]
        public List<DataCostContract> Get([FromBody] UserCriteriaContract model)
        {
            var result = _costsvc.GetDataCost(model);
            return result;
        }


    }
}
