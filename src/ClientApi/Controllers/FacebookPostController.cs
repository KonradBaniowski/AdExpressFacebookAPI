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
        public List<DataPostFacebookContract> Get([FromBody] postmodel model)
        {
            return _fbsvc.GetDataPostFacebook(model.idLogin, model.beginDate, model.endDate, model.idAdvertisers, model.idBrands, model.pages);

        }

        public class postmodel
        {
            public int idLogin { get; set; }
            public long beginDate { get; set; }
            public long endDate { get; set; }
            public List<long> idAdvertisers { get; set; }
            public List<long> idBrands { get; set; }
            public List<long> pages { get; set; }
        }
    }
}
