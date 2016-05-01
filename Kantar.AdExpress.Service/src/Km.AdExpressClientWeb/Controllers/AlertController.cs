using Kantar.AdExpress.Service.Core.BusinessService;
using Domain=Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.Models.Alert;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Alert.Domain;
using TNS.Ares.Alerts.DAL;
using TNS.Ares.Domain.Layers;
using TNS.Ares.Domain.LS;
using TNS.Ares.Constantes;

namespace Km.AdExpressClientWeb.Controllers
{
    public class AlertController : Controller
    {
        private IAlertService _alertService;
        private IApplicationUserManager _userManager;
        private IWebSessionService _webSessionService;

        public AlertController(IAlertService alertService, IApplicationUserManager userManager, IWebSessionService webSessionService)
        {
            _alertService = alertService;
            _userManager = userManager;
            _webSessionService = webSessionService;
        }

        public async Task<ActionResult> Index(string idSession, int? idOcc, int? idAlert)
        {
            if (!idOcc.HasValue) idOcc = -1;
            if (!idAlert.HasValue) idAlert = -1;

            DataAccessLayer layer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
            TNS.FrameWork.DB.Common.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
            IAlertDAL alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null);
            AlertOccurence occ = alertDAL.GetOccurrence(idOcc.Value, idAlert.Value);

            if (occ != null)
            {
                Alert alert = alertDAL.GetAlert(occ.AlertId);
                if (alert != null)
                {
                    WebSession session = (WebSession)alert.Session;

                    var authenticate = await _userManager.PasswordSignIn(session.CustomerLogin.Login, session.CustomerLogin.PassWord, false, shouldLockout: false);

                    switch (authenticate)
                    {
                        case SignInStatus.Success:
                            this.Session["session"] = session;
                            this.Session["occ"] = occ;
                            return RedirectToAction("redirect");
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View("Home");
                    }
                }
            }

            return View("Home");
        }

        [Authorize]
        public ActionResult redirect()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession session = (WebSession)this.Session["session"];
            AlertOccurence occ = (AlertOccurence)this.Session["occ"];

            var redirectUrl = _alertService.GetRedirectUrl(session, idWS, occ);
            return RedirectToAction("Results", redirectUrl);
        }

        public ActionResult CreateAlert()
        {
            var cp = new ClaimsPrincipal(User.Identity);
            var idWebSession = cp.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var webSession = (WebSession)WebSession.Load(idWebSession);
            CreateAlertModel model = new CreateAlertModel
            {
                Labels = LoadPageLabels(webSession.SiteLanguage),
                Periodicity= new List<SelectListItem>()
            };
            var daily = new SelectListItem
            {
                Text = GestionWeb.GetWebWord(LanguageConstantes.Daily, webSession.SiteLanguage),
                Value = TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity.Daily.GetHashCode().ToString(),
                Selected =true
            };
            var weekly = new SelectListItem
            {
                Text = GestionWeb.GetWebWord(LanguageConstantes.Weekly, webSession.SiteLanguage),
                Value = TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity.Weekly.GetHashCode().ToString()
            };
            var monthly = new SelectListItem
            {
                Text = GestionWeb.GetWebWord(LanguageConstantes.Monthly, webSession.SiteLanguage),
                Value = TNS.Ares.Constantes.Constantes.Alerts.AlertPeriodicity.Monthly.GetHashCode().ToString()
            };

            model.Periodicity.Add(daily);
            model.Periodicity.Add(weekly);
            model.Periodicity.Add(monthly);
            return PartialView(model);
        }
        private Labels LoadPageLabels(int siteLanguage)
        {
            Regex regex = new System.Text.RegularExpressions.Regex(@"(<br />|<br/>|</ br>|</br>)");

            var result = new Labels
            {
                SaveAlert2 = GestionWeb.GetWebWord(LanguageConstantes.SaveAlert2, siteLanguage),
                FileName = GestionWeb.GetWebWord(LanguageConstantes.FileName, siteLanguage),
                Email = GestionWeb.GetWebWord(LanguageConstantes.MailCode, siteLanguage),
                Periodicity = GestionWeb.GetWebWord(LanguageConstantes.Periodicity, siteLanguage),
                Daily = GestionWeb.GetWebWord(LanguageConstantes.Daily, siteLanguage),
                Weekly = GestionWeb.GetWebWord(LanguageConstantes.Weekly, siteLanguage),
                Monthly = GestionWeb.GetWebWord(LanguageConstantes.Monthly, siteLanguage),
                SelectDayInWeek = GestionWeb.GetWebWord(LanguageConstantes.SelectDayInWeek, siteLanguage),
                SelectDayInMonth = GestionWeb.GetWebWord(LanguageConstantes.SelectDayInMonth, siteLanguage),
                AlertInfoMessage = regex.Replace(GestionWeb.GetWebWord(LanguageConstantes.AlertInfoMessage, siteLanguage),"\r\n"),
                Submit = GestionWeb.GetWebWord(LanguageConstantes.Submit, siteLanguage),
                Close = GestionWeb.GetWebWord(LanguageConstantes.Close, siteLanguage),
                CreateAlert = GestionWeb.GetWebWord(LanguageConstantes.CreateAlert, siteLanguage)
            };
            return result;
        }
    }
}