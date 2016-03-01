using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class PortfolioController : Controller
    {
        private IPortfolioService _portoFolioService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IUniverseService _universeService;
        private const string _controller = "Portfolio";
        public PortfolioController(IPortfolioService portofolioService, IMediaService mediaService, IWebSessionService webSessionService, IUniverseService universeService)
        {
            _portoFolioService = portofolioService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _universeService = universeService;
        }
        // GET: Portfolio
        public ActionResult Index()
        {
            return View();
        }
    }
}