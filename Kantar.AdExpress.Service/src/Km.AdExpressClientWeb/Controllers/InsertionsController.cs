using Kantar.AdExpress.Service.Core.BusinessService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    public class InsertionsController : Controller
    {

        private IWebSessionService _webSessionService;
        private IUniverseService _universService;

        // GET: Insertions
        public ActionResult Index()
        {
            
            return View();
        }


    }
}