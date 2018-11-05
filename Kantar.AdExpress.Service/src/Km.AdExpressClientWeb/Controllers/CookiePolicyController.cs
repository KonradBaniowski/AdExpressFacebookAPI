using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Helpers;
using NLog;
using TNS.AdExpress.Domain.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    public class CookiePolicyController : Controller
    {
        private IApplicationUserManager _userManager;
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public CookiePolicyController(IApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public ActionResult Index(int siteLanguage = -1)
        {
            if (siteLanguage == -1) siteLanguage = WebApplicationParameters.DefaultLanguage;

            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(Convert.ToInt32(siteLanguage));
            ViewBag.SiteLanguageCode = siteLanguage;

            return View();
        }
    }
}