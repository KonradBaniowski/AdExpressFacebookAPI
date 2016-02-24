using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class TestExportController : Controller
    {
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;

        private string icon;
        public TestExportController(IMediaService mediaService, IWebSessionService webSessionService)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
        }

        // GET: TestExport
        public ActionResult Index()
        {
            return View();
        }
    }
}