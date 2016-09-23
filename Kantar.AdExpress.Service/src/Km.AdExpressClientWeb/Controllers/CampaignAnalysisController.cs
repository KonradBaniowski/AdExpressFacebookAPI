using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    public class CampaignAnalysisController : Controller
    {

        private IWebSessionService _webSessionService;
        private IOptionService _optionService;
        private const string _controller = "CampaignAnalysis";
        private int _siteLanguage = WebApplicationParameters.DefaultLanguage;

        public CampaignAnalysisController(IWebSessionService webSessionService, IOptionService optionService)
        {
            _webSessionService = webSessionService;
            _optionService = optionService;
        }

        // GET: CampaignAnalysis
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Results()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            //WebSession CustomerSession = (WebSession)WebSession.Load(idSession);
            var resultNode = new NavigationNode { Position = 4 };
            var pageHelper = new Helpers.PageHelper();
            var result = _webSessionService.GetWebSession(idSession);
            _siteLanguage = result.WebSession.SiteLanguage;
            //var model = new Models.Shared.ResultsViewModel
            //{
            //    NavigationBar = pageHelper.LoadNavBar(idSession, _controller, _siteLanguage, 4),
            //    Presentation = pageHelper.LoadPresentationBar(_siteLanguage, result.ControllerDetails),
            //    Labels = LabelsHelper.LoadPageLabels(_siteLanguage),
            //    IsAlertVisible = PageHelper.IsAlertVisible(WebApplicationParameters.CountryCode, idSession),
            //    ExportTypeViewModels = PageHelper.GetExportTypes(WebApplicationParameters.CountryCode, Module.Name., _siteLanguage)
            //};

            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            ViewBag.SiteLanguage = _siteLanguage;

            return View();
        }

    }
}