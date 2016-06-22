using Facebook.Service.BusinessLogic.ServiceImpl;
using Facebook.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientApi.Controllers
{
    public class HomeController : Controller
    {
        
        private IRightService _rightSvc;
        private IFacebookPageService _fbsvc;
        private IProductService _productSvc;

        public HomeController(IRightService rightSvc, IFacebookPageService fbsvc, IProductService productSvc)
        {
            _rightSvc = rightSvc;
            _fbsvc = fbsvc;
            _productSvc = productSvc;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Test()
        {

            var dbfb = _fbsvc.GetDataFacebook();
            //var test2 = _rightSvc.GetProductRight(1087);
            //var next = _rightSvc.GetMediaRight(1087);
            var products = _productSvc.GetItems("BOISSONS",1);
            return View("Index");
        }

    }
}
