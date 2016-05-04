using Kantar.AdExpress.Service.Core.BusinessService;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.Models;
using Km.AdExpressClientWeb.Models.Home;
using Km.AdExpressClientWeb.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Date.DAL;
using Kantar.AdExpress.Service.Core.Domain;
using KM.Framework.Constantes;
using TNS.AdExpress.Web.Core.Utilities;
using Km.AdExpressClientWeb.Helpers;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private const string _cryptKey = "8!b?#B$3";

        private IRightService _rightService;
        private IApplicationUserManager _userManager;
        private IWebSessionService _webSessionService;
        private IUniverseService _universService;
        private IInfosNewsService _infosNewsService;
       

        public HomeController(IRightService rightService, IApplicationUserManager applicationUserManager, IWebSessionService webSessionService, IUniverseService universService, IInfosNewsService infosNewsService)
        {
            _userManager = applicationUserManager;
            _rightService = rightService;
            _webSessionService = webSessionService;
            _universService = universService;
            _infosNewsService = infosNewsService;
        }

        public JsonResult ChangeLanguage(string returnUrl, int siteLanguage = 33)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            _webSessionService.UpdateSiteLanguage(idWebSession, siteLanguage);           
            
            JsonResult jsonModel = Json(new { RedirectUrl = returnUrl });
           
            return jsonModel;
        }

        public ActionResult Index()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            var idLogin = cla.Claims.Where(e => e.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var login = cla.Claims.Where(e => e.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            var password = cla.Claims.Where(e => e.Type == ClaimTypes.Hash).Select(c => c.Value).SingleOrDefault();
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var resList = _rightService.GetModulesList(idWS);
            var res = _rightService.GetModules(idWS);
            List<Documents> documents = _infosNewsService.GetInfosNews(idWS);

            ViewBag.SiteLanguageName = NavigationHelper.GetSiteLanguageName(resList.First().Value.SiteLanguage);
            documents.Add(new Documents()
                        {
                            Id = 3,
                            Label = "Documents",
                            InfosNews = new List<InfosNews>()
                            {
                                new InfosNews()
                                {
                                    Label = "Aide",
                                    Url = "Page_aide.pdf"
                                },
                                new InfosNews()
                                {
                                    Label = "Dates de mises à jour",
                                    Url = "Planning mise à jour Adexpress.pdf"
                                },
                                 new InfosNews()
                                {
                                    Label = "Configuration",
                                    Url = "Configuration.pdf"
                                }
                            }
                        });

            var encryptedPassword = EncryptQueryString(password);
            var encryptedLogin = EncryptQueryString(login); 

            var Home = new HomePageViewModel()
            {
                ModuleRight = res,
                Modules = resList,
                Documents = documents,
                EncryptedLogin =encryptedLogin,
                EncryptedPassword = encryptedPassword,
                SiteLanguage = 33, // Default
            };

            Home.SiteLanguage = resList.First().Value.SiteLanguage;
            Home.Labels = LoadPageLabels(Home.SiteLanguage);

            return View(Home);
        }

        public void CurrentModule(int idModule)
        {
            if (idModule != 0)
            {
                var claim = new ClaimsPrincipal(User.Identity);
                string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                _webSessionService.SaveCurrentModule(idWebSession, idModule);
            }
            else
            {
                throw new Exception("Module pb");
            }
            //if here pb
        }

        public ActionResult MyAdExpress()
        {
            var model = new MyAdExpressViewModel
            {
                SavedResults = new Domain.AdExpressUniversResponse {  UniversType= Domain.UniversType.Result, UniversGroups = new List<Domain.UserUniversGroup>()},
                SavedUnivers = new Domain.AdExpressUniversResponse { UniversType = Domain.UniversType.Univers, UniversGroups = new List<Domain.UserUniversGroup>() },
                Alerts = new List<Domain.Alert>()
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            #region Saved Reuslt Queries
            var result = _universService.GetResultUnivers(idWebSession);
            foreach (var group in result.UniversGroups)
            {
                int count = group.Count;
                group.FirstColumnSize = (count % 2 == 0) ? count / 2 : (count / 2) + 1;
                group.SecondeColumnSize = count - group.FirstColumnSize;
            }
            model.SavedResults = result;
            #endregion
            #region Saved Univers (Market & Media)
            string branch = "2";
            string listUniversClientDescription = string.Empty;
            var univers = _universService.GetUnivers(idWebSession, branch, listUniversClientDescription);
            foreach (var group in univers.UniversGroups)
            {
                int count = group.Count;
                group.FirstColumnSize = (count % 2 == 0) ? count / 2 : (count / 2) + 1;
                group.SecondeColumnSize = count - group.FirstColumnSize;
            }
            model.SavedUnivers = univers;
            #endregion
            #region Alerts
            var alertsResponse = _universService.GetUserAlerts(idWebSession);
            model.Alerts = alertsResponse.Alerts;
            #endregion
            model.PresentationModel = LoadPresentationBar(result.SiteLanguage, false);
            model.Labels = LoadPageLabels(result.SiteLanguage);
            return View(model);
        }

        public ActionResult ReloadSession()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var model = new Domain.AdExpressUniversResponse
            {
                UniversType = Domain.UniversType.Result,
                UniversGroups = new List<Domain.UserUniversGroup>()
            };
            var result = _universService.GetResultUnivers(idWebSession);
            foreach (var group in result.UniversGroups)
            {
                int count = group.Count;
                group.FirstColumnSize = (count % 2 == 0) ? count / 2 : (count / 2) + 1;
                group.SecondeColumnSize = count - group.FirstColumnSize;
            }
            model = result;
            return  PartialView("MyAdExpressSavedResults", model);
        }


        public ActionResult ReloadUnivers()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var model = new List<Domain.Alert>();
            var alertsResponse = _universService.GetUserAlerts(idWebSession);
            model= alertsResponse.Alerts;
            return PartialView("MyAdExpressAlerts", model);

        }

        public ActionResult ReloadAlerts()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var model = new Domain.AdExpressUniversResponse
            {
                UniversType = Domain.UniversType.Univers,
                UniversGroups = new List<Domain.UserUniversGroup>()
            }; string branch = "2";
            string listUniversClientDescription = string.Empty;
            var univers = _universService.GetUnivers(idWebSession, branch, listUniversClientDescription);
            foreach (var group in univers.UniversGroups)
            {
                int count = group.Count;
                group.FirstColumnSize = (count % 2 == 0) ? count / 2 : (count / 2) + 1;
                group.SecondeColumnSize = count - group.FirstColumnSize;
            }
            model = univers;
            return PartialView("MyAdExpressSavedResults", model);

        }
        private PresentationModel LoadPresentationBar(int siteLanguage, bool showCurrentSelection = true)
        {
            PresentationModel result = new PresentationModel
            {
                ModuleCode = LanguageConstantes.MediaScheduleCode,
                SiteLanguage = siteLanguage,
                ModuleDecriptionCode = LanguageConstantes.MediaScheduleDescriptionCode,
                ShowCurrentSelection = showCurrentSelection,

                //Eviter de se garder GESTION WEB WORD DANS LES VUES CSHTML, la PLS
                ModuleDescription = GestionWeb.GetWebWord(LanguageConstantes.MonAdExpressDescription, siteLanguage),
                ModuleTitle = GestionWeb.GetWebWord(LanguageConstantes.MonAdExpressCode, siteLanguage)
            };
            return result;
        }
        private Labels LoadPageLabels(int siteLanguage)
        {
            var result = new Labels
            {
                Save = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, siteLanguage),                
                MyResults = GestionWeb.GetWebWord(LanguageConstantes.ResultsCode, siteLanguage),               
                SaveUnivers = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, siteLanguage),
                UserUniversCode = GestionWeb.GetWebWord(LanguageConstantes.UserSavedUniversCode, siteLanguage),
                MyResultsDescription = GestionWeb.GetWebWord(LanguageConstantes.MyResultsDescription, siteLanguage),
                AlertsCode= GestionWeb.GetWebWord(LanguageConstantes.AlertsCode, siteLanguage),
                NoSavedUnivers = GestionWeb.GetWebWord(LanguageConstantes.NoSavedUniversCode, siteLanguage),
                Periodicity = GestionWeb.GetWebWord(LanguageConstantes.Periodicity, siteLanguage),
                Daily = GestionWeb.GetWebWord(LanguageConstantes.Daily, siteLanguage),
                Weekly =GestionWeb.GetWebWord(LanguageConstantes.Weekly, siteLanguage),
                Monthly = GestionWeb.GetWebWord(LanguageConstantes.Monthly, siteLanguage),
                Quartly = GestionWeb.GetWebWord(LanguageConstantes.Quartly, siteLanguage),
                SaveAlert = GestionWeb.GetWebWord(LanguageConstantes.SaveAlert, siteLanguage),
                NoAlerts = GestionWeb.GetWebWord(LanguageConstantes.NoAlerts, siteLanguage),
                SendDate= GestionWeb.GetWebWord(LanguageConstantes.SendDate, siteLanguage),
                Occurrence = GestionWeb.GetWebWord(LanguageConstantes.Occurrence, siteLanguage),
                Occurrences = GestionWeb.GetWebWord(LanguageConstantes.Occurrences, siteLanguage),
                AlertsDetails = GestionWeb.GetWebWord(LanguageConstantes.AlertDetails, siteLanguage),
                Deadline = GestionWeb.GetWebWord(LanguageConstantes.Deadline, siteLanguage),
                EveryWeek= GestionWeb.GetWebWord(LanguageConstantes.EveryWeek, siteLanguage),
                EveryMonth = GestionWeb.GetWebWord(LanguageConstantes.EveryMonth, siteLanguage),
                ExpirationDate = GestionWeb.GetWebWord(LanguageConstantes.ExpirationDate, siteLanguage),
                AlertType = GestionWeb.GetWebWord(LanguageConstantes.AlertType, siteLanguage),
                Receiver= GestionWeb.GetWebWord(LanguageConstantes.Receiver, siteLanguage),
                TimeSchedule= GestionWeb.GetWebWord(LanguageConstantes.TimeSchedule, siteLanguage),
                CreateDirectory = GestionWeb.GetWebWord(LanguageConstantes.CreateFolder, siteLanguage),
                RenameDirectory = GestionWeb.GetWebWord(LanguageConstantes.RenameSelectedFolder, siteLanguage),
                DropDirectory = GestionWeb.GetWebWord(LanguageConstantes.DropFolder, siteLanguage),
                Directories = GestionWeb.GetWebWord(LanguageConstantes.Directories, siteLanguage),
                ModuleLabel = GestionWeb.GetWebWord(LanguageConstantes.ModuleLabel, siteLanguage),
                NewsLabel = GestionWeb.GetWebWord(LanguageConstantes.NewsLabel, siteLanguage),
                YourModule = GestionWeb.GetWebWord(LanguageConstantes.YourModule, siteLanguage),
                NewsDescr = GestionWeb.GetWebWord(LanguageConstantes.NewsDescr, siteLanguage),
                ContactUsLabel = GestionWeb.GetWebWord(LanguageConstantes.ContactUsLabel, siteLanguage)
            };
            return result;
        }

        public static string EncryptQueryString(string strQueryString)
        {
            Encryption64 oES =
                new Encryption64();
            return oES.Encrypt(strQueryString, _cryptKey).Replace("+", "-").Replace("/", "_");
        }

        public static string DecryptQueryString(string strQueryString)
        {
            Encryption64 oES =
                new Encryption64();
            return oES.Decrypt(strQueryString.Replace("-", "+").Replace("_", "/"), _cryptKey);
        }
    }
}
