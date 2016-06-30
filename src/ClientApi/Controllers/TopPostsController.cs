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
    public class TopPostsController : ApiController
    {

        private IRightService _rightSvc;
        private IFacebookPostService _fbsvc;
        private IProductService _productSvc;

        public TopPostsController(IRightService rightSvc, IFacebookPostService fbsvc, IProductService productSvc)
        {
            _rightSvc = rightSvc;
            _fbsvc = fbsvc;
            _productSvc = productSvc;
        }

        [Route("Api/TopPosts")]
        [HttpPost]
        public List<PostFacebookContract> Get([FromBody] PostModel model)
        {
            return _fbsvc.GetTopPostFacebook(model.IdLogin, model.BeginDate, model.EndDate, model.IdAdvertisers, model.IdBrands, model.IdPages);

        }

        [Route("Api/OnePost")]
        [HttpPost]
        public PostFacebookContract Get([FromBody] long idPostFacebook)
        {
            return _fbsvc.GetPostFacebook(idPostFacebook);

        }

    }
}
