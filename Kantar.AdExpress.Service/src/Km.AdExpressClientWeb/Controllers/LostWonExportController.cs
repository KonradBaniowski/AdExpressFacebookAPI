using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Web.Core.Result;
using TNS.FrameWork.WebResultUI;

namespace Km.AdExpressClientWeb.Controllers
{
    public class LostWonExportController : Controller
    {
        private ILostWonService _lostWonService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;


        public LostWonExportController(ILostWonService lostWonService, IMediaService mediaService, IWebSessionService webSessionService)
        {
            _lostWonService = lostWonService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;

        }

        // GET: LostWonExport
        public ActionResult Index()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _lostWonService.GetResultTable(idWebSession);

            export(data);

            return View();
        }

        public ActionResult ResultBrut()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _lostWonService.GetResultTable(idWebSession);

            export(data);

            return View();
        }

        private void export(ResultTable data)
        {
        }
    }
}