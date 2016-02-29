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
        private IMediaScheduleService _mediaSchedule;

        private string icon;
        public TestExportController(IMediaService mediaService, IWebSessionService webSessionService, IMediaScheduleService mediaSchedule)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _mediaSchedule = mediaSchedule;
        }

        // GET: TestExport
        public ActionResult Index()
        {
            var data = _mediaSchedule.GetMediaScheduleData("");

            return View();
        }
    }
}