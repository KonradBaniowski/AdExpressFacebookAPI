﻿using Kantar.AdExpress.Service.Core.BusinessService;
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
    [Authorize]
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
                    IdWebSession= idWS,
                    Type =periodicityType,
                    OccurrenceDate = date
                };
            var response = _alertService.SaveAlert(request);           
            return result;
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