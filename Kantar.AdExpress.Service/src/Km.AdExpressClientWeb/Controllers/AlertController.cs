using Kantar.AdExpress.Service.Core.BusinessService;
using Domain = Kantar.AdExpress.Service.Core.Domain;
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
using Km.AdExpressClientWeb.Helpers;
using Newtonsoft.Json;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.DataAccess;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Alert.Domain;
using TNS.Ares.Alerts.DAL;
using TNS.Ares.Domain.Layers;
using TNS.Ares.Domain.LS;
using TNS.Ares.Constantes;
using TNS.FrameWork.WebResultUI;

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

        [Authorize(Roles = Role.ADEXPRESS)]
        public ActionResult redirect()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession session = (WebSession)this.Session["session"];
            AlertOccurence occ = (AlertOccurence)this.Session["occ"];

            HttpCookie sortKeyCookie = System.Web.HttpContext.Current.Request.Cookies["sortKey"] ?? new HttpCookie("sortKey");
            sortKeyCookie.Value = session.SortKey;
            sortKeyCookie.Expires = DateTime.Now.AddDays(1);
            System.Web.HttpContext.Current.Response.Cookies.Add(sortKeyCookie);

            HttpCookie sortOrderCookie = System.Web.HttpContext.Current.Request.Cookies["sortOrder"] ?? new HttpCookie("sortOrder");
            sortOrderCookie.Value = session.Sorting.GetHashCode().ToString();
            sortOrderCookie.Expires = DateTime.Now.AddDays(1);
            System.Web.HttpContext.Current.Response.Cookies.Add(sortOrderCookie);

            if (WebApplicationParameters.EnableGdpr)
            {
                HttpCookie cookieControlPrefs = null;
                string cookieName = "cookieControlPrefs-" + session.CustomerLogin.IdLogin.ToString().Encrypt(Helpers.SecurityHelper.CryptKey);
                var cookiesKeys = Request.Cookies.AllKeys;
                var found = cookiesKeys.FirstOrDefault(n => n == "cookieControlPrefs");

                if (!string.IsNullOrEmpty(found))
                {
                    cookieControlPrefs = Request.Cookies["cookieControlPrefs"];
                }
                else
                {
                    foreach (var key in cookiesKeys)
                    {
                        if (key.StartsWith("cookieControlPrefs"))
                        {
                            var id = key.Split('-')[1].Decrypt(Helpers.SecurityHelper.CryptKey);
                            if (id == session.CustomerLogin.IdLogin.ToString())
                            {
                                cookieControlPrefs = Request.Cookies[key];
                                break;
                            }
                        }
                    }
                }

                if (cookieControlPrefs != null)
                {
                    var cookies = JsonConvert.DeserializeObject<GdprCookie>(cookieControlPrefs.Value);

                    var enableTracking = cookies.prefs.FirstOrDefault(s => s.Contains("Statistiques"));
                    var enableTroubleshooting = cookies.prefs.FirstOrDefault(s => s.Contains("Diagnostic"));
                    int enableTrackingDb = 0;
                    int enableTroubleshootingDb = 0;

                    if (enableTracking != null)
                    {
                        session.EnableTracking = true;
                        enableTrackingDb = 1;
                    }
                    else
                        session.EnableTracking = false;

                    if (enableTroubleshooting != null)
                    {
                        session.EnableTroubleshooting = true;
                        enableTroubleshootingDb = 1;
                    }
                    else
                        session.EnableTroubleshooting = false;

                    if (!cookies.storedInDb)
                    {
                        TNS.FrameWork.DB.Common.IDataSource Source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.rights);
                        var expDateCookie = DateTime.Now.AddDays(395);
                        RightDAL.SetAllPrivacySettings(Source, session.CustomerLogin.IdLogin, enableTrackingDb, enableTroubleshootingDb, expDateCookie);

                        cookies.storedInDb = true;
                        cookies.creationDate = DateTime.Now.ToString("yyyy-MM-dd");
                        cookies.expDate = expDateCookie.ToString("yyyy-MM-dd");
                        cookies.guid = Helpers.SecurityHelper.Encrypt(session.CustomerLogin.Login, Helpers.SecurityHelper.CryptKey);
                        cookieControlPrefs.Name = cookieName;
                        cookieControlPrefs.Value = JsonConvert.SerializeObject(cookies);
                        cookieControlPrefs.Expires = expDateCookie;
                        Response.Cookies.Add(cookieControlPrefs);
                        var cookieTmp = Response.Cookies["cookieControlPrefs"];
                        if (cookieTmp != null)
                            cookieTmp.Expires = DateTime.Now.AddDays(-1);
                    }
                    else
                    {
                        TNS.FrameWork.DB.Common.IDataSource Source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.rights);
                        bool allowTracking = false;
                        bool allowTroubleshooting = false;
                        DateTime expDate = new DateTime(2000, 1, 1);
                        RightDAL.GetPrivacySettings(Source, session.CustomerLogin.IdLogin, out allowTracking, out allowTroubleshooting, out expDate);

                        cookies.prefs = new List<string>();

                        if (allowTracking)
                        {
                            cookies.prefs.Add("Statistiques");
                            session.EnableTracking = true;
                        }
                        else
                            session.EnableTracking = false;

                        if (allowTroubleshooting)
                        {
                            cookies.prefs.Add("Diagnostic");
                            session.EnableTroubleshooting = true;
                        }
                        else
                            session.EnableTroubleshooting = false;

                        cookies.creationDate = expDate.AddDays(-395).ToString("yyyy-MM-dd");
                        cookies.expDate = expDate.ToString("yyyy-MM-dd");
                        cookieControlPrefs.Expires = expDate;
                        cookieControlPrefs.Value = JsonConvert.SerializeObject(cookies);
                        Response.Cookies.Add(cookieControlPrefs);
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                session.EnableTracking = true;
                session.EnableTroubleshooting = true;
            }

            var redirectUrl = _alertService.GetRedirectUrl(session, idWS, occ, this.HttpContext);
            return RedirectToAction("Results", redirectUrl);
        }

        [Authorize(Roles = Role.ADEXPRESS)]
        public ActionResult CreateAlert()
        {
            var cp = new ClaimsPrincipal(User.Identity);
            var idWebSession = cp.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var webSession = (WebSession)WebSession.Load(idWebSession);

            string sortKey = System.Web.HttpContext.Current.Request.Cookies["sortKey"].Value;
            string sortOrder = System.Web.HttpContext.Current.Request.Cookies["sortOrder"].Value;

            if (!string.IsNullOrEmpty(sortKey) && !string.IsNullOrEmpty(sortOrder))
            {
                webSession.SortKey = sortKey;
                webSession.Sorting = (ResultTable.SortOrder)Enum.Parse(typeof(ResultTable.SortOrder), sortOrder);
                webSession.Save();
            }

            CreateAlertModel model = new CreateAlertModel
            {
                Labels = LoadPageLabels(webSession.SiteLanguage),
                Periodicity = GetAlertPeriodicity(webSession.SiteLanguage),
                WeekDays = GetWeekDays(webSession.SiteLanguage)
            };
            
            return PartialView(model);
        }

        private List<SelectListItem> GetAlertPeriodicity(int siteLanguage)
        {
            var result = new List<SelectListItem>();
            var values = Enum.GetValues(typeof(Constantes.Alerts.AlertPeriodicity));
            foreach (var value in values)
            {
                var selectListItem = new SelectListItem
                {
                    Value = value.GetHashCode().ToString()
                };
                switch (value.GetHashCode())
                {
                    case 10:
                        selectListItem.Text = GestionWeb.GetWebWord(LanguageConstantes.Daily, siteLanguage);
                        selectListItem.Selected = true;
                        break;
                    case 20:
                        selectListItem.Text = GestionWeb.GetWebWord(LanguageConstantes.Weekly, siteLanguage);
                        break;
                    case 30:
                        selectListItem.Text = GestionWeb.GetWebWord(LanguageConstantes.Monthly, siteLanguage);
                        break;
                    default:
                        break;
                }
                result.Add(selectListItem);
            }
            return result;
        }

        private List<SelectListItem> GetWeekDays (int siteLanguage)
        {
            var week = new List<SelectListItem>();
            const int start = 653;
            for (var i=1;i<8;i++)
            {
                SelectListItem day = new SelectListItem
                {
                    Text = GestionWeb.GetWebWord(start+i, siteLanguage),
                    Value = i.ToString()
                };
                week.Add(day);
            }
            return week;
        }

        [Authorize(Roles = Role.ADEXPRESS)]
        public JsonResult SaveAlert(string title, string email, string type, string date)
        {
            JsonResult result = new JsonResult();
            var cla = new ClaimsPrincipal(User.Identity);
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var periodicityType = (Constantes.Alerts.AlertPeriodicity)Enum.Parse(typeof(Constantes.Alerts.AlertPeriodicity), type);
            var request = new Domain.SaveAlertRequest
            {
                AlertTitle = title,
                Email = email,
                IdWebSession = idWS,
                Type = periodicityType,
                OccurrenceDate = !String.IsNullOrEmpty(date) ? int.Parse(date) : -1
                };
            var response = _alertService.SaveAlert(request, this.HttpContext);           
            return Json(response,JsonRequestBehavior.AllowGet);
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
                CreateAlert = GestionWeb.GetWebWord(LanguageConstantes.CreateAlert, siteLanguage),
                EveryWeek = GestionWeb.GetWebWord(LanguageConstantes.EveryWeek,siteLanguage),
                EveryMonth =GestionWeb.GetWebWord(LanguageConstantes.EveryMonth,siteLanguage)
            };
            return result;
        }
    }
}