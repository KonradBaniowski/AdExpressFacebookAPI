using ClientApi.Models;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ClientApi.Controllers
{
    public class KPIFacebookPageController : ApiController
    {
        private IKPIFacebookPageService _fbsvc;
        public KPIFacebookPageController(IKPIFacebookPageService fbsvc)
        {
            _fbsvc = fbsvc;
        }

        [Route("Api/KPI")]
        [HttpPost]
        public List<KPIPageFacebookContract> GetKPIPages([FromBody] PostModel model)
        {
            var svc = _fbsvc.GetKPIPages(model.IdLogin, model.BeginDate, model.EndDate, model.IdAdvertisers, model.IdBrands, model.IdLanguage);
            return svc;
        }

        [Route("Api/KPI/Plurimedia")]
        [HttpPost]
        public List<KPIPercentPageFacebookContract> GetKPIPlurimediaPages([FromBody] PostModelKPIReferents model)
        {
            var svc = _fbsvc.GetKPIPlurimediaPages(model.IdLogin, model.BeginDate, model.EndDate, model.IdAdvertisersRef, model.IdAdvertisersRef, model.IdBrandsRef, model.IdBrandsRef, model.IdLanguage);
            return svc;
        }
    }
}
