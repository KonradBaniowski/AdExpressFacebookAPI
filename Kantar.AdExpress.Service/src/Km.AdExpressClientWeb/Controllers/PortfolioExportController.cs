using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    public class PortfolioExportController : Controller
    {
        private IPortfolioService _portofolioService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        

        public PortfolioExportController(IPortfolioService portofolioService, IMediaService mediaService, IWebSessionService webSessionService)
        {
            _portofolioService = portofolioService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;
          
        }

        // GET: PortfolioExport
        public ActionResult Index()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _portofolioService.GetResultTable(idWebSession);

            return View();
        }

        public ActionResult ResultBrut()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _portofolioService.GetResultTable(idWebSession);

            return View();
        }
    }
}