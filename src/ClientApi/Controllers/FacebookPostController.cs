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
    public class FacebookPostController : ApiController
    {
        private IRightService _rightSvc;
        private IFacebookPostService _fbsvc;
        private IProductService _productSvc;

        public FacebookPostController(IRightService rightSvc, IFacebookPostService fbsvc, IProductService productSvc)
        {
            _rightSvc = rightSvc;
            _fbsvc = fbsvc;
            _productSvc = productSvc;
        }

        [HttpPost]
        public List<DataPostFacebookContract> Get([FromBody] PostModel model)
        {
            return _fbsvc.GetDataPostFacebook(model.IdLogin , model.BeginDate, model.EndDate, model.IdAdvertisers, model.IdBrands, model.IdPages,model.IdLanguage);
        }
    }
}
