using ClientApi.Models;
using Facebook.Service.Contract.ContractModels.ModuleFacebook;
using Facebook.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [HttpPost]
        public List<DataFacebookContract> Get([FromBody] PostModel model)
        {
            //int idLogin, long beginDate, long endDate, List< long > idAdvertisers, List<long> idBrands
            //int idLogin
            //, long beginDate, long endDate
            //, List<long> idAdvertisers, List< long > idBrands
            //int idLogin, long beginDate, long endDate, List< long > idAdvertisers, List<long> idBrands
            //int idLogin = 1155;
            //long beginDate = 20150101;
            //long endDate = 20160301;
            //List<long> idAdvertisers = new List<long> { 1060, 332860, 48750 };
            //List<long> idBrands = null;
            return _fbsvc.GetDataFacebook(model.IdLogin , model.BeginDate, model.EndDate, model.IdAdvertisers, model.IdBrands);

        }
       
    }
}
