using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.BusinessService;
using System.Collections.Generic;
using System.Web.Http;

namespace ClientApi.Controllers
{
    public class FacebookPageController : ApiController
    {
        private IRightService _rightSvc;
        private IFacebookPageService _fbsvc;
        private IProductService _productSvc;

        public FacebookPageController(IRightService rightSvc, IFacebookPageService fbsvc, IProductService productSvc)
        {
            _rightSvc = rightSvc;
            _fbsvc = fbsvc;
            _productSvc = productSvc;
        }

        //[HttpGet]
        public IEnumerable<DataFacebookContract> Get()
        {
            //int idLogin, long beginDate, long endDate, List< long > idAdvertisers, List<long> idBrands
            int idLogin = 1155;
            long beginDate = 20150101;
            long endDate = 20160301;
            List<long> idAdvertisers = new List<long> { 1060, 332860, 48750 };
            List<long> idBrands = null;

            return _fbsvc.GetDataFacebook(idLogin, beginDate, endDate, idAdvertisers, idBrands);
        }
    }
}
