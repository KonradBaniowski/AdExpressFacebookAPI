using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.Models;
using TNS.AdExpress.Constantes.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = Role.ADEXPRESS)]
    public class CookieSettingsController : Controller
    {
        // GET: CookieSettings
        public ActionResult Index()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            TNS.AdExpress.Web.Core.Sessions.WebSession session = (TNS.AdExpress.Web.Core.Sessions.WebSession)TNS.AdExpress.Web.Core.Sessions.WebSession.Load(idWS);

            CookieSettingsModel model = new CookieSettingsModel();

            model.SiteLanguage = session.SiteLanguage;
            model.EnableTracking = session.EnableTracking;
            model.EnableTroubleshooting = session.EnableTroubleshooting;

            ViewBag.SiteLanguage = session.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(session.SiteLanguage);

            return View(model);
        }
    }
}