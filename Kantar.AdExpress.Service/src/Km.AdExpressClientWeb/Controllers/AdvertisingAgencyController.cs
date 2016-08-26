using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Web;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    public class AdvertisingAgencyController : Controller
    {
        private IWebSessionService _webSessionService;
        private const string _controller = "AdvertisingAgency";
        private int _siteLanguage = WebApplicationParameters.DefaultLanguage;

        public AdvertisingAgencyController(IWebSessionService webSessionService)
        {
            _webSessionService = webSessionService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Results()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _webSessionService.GetWebSession(idSession);
            _siteLanguage = result.WebSession.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            ViewBag.SiteLanguage = _siteLanguage;
            var resultNode = new NavigationNode { Position = 4 };
            var pageHelper = new Helpers.PageHelper();
            var model = new Models.Shared.ResultsViewModel
            {
                NavigationBar = pageHelper.LoadNavBar(idSession, _controller, _siteLanguage, 4),
                Presentation = pageHelper.LoadPresentationBar(result.WebSession.SiteLanguage, result.ControllerDetails),
                Labels = pageHelper.LoadPageLabels(result.WebSession.SiteLanguage, _controller),
                IsAlertVisible = PageHelper.IsAlertVisible(WebApplicationParameters.CountryCode, idSession),
                ExportTypeViewModels = PageHelper.GetExportTypes(WebApplicationParameters.CountryCode, WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA, _siteLanguage)
            };

            return View(model);
        }

    }
}