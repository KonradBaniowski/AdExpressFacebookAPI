using Kantar.AdExpress.Service.Core.BusinessService;
using Domain=Kantar.AdExpress.Service.Core.Domain;
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

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IRightService _rightService;
        private IApplicationUserManager _userManager;
        private IWebSessionService _webSessionService;
        private IUniverseService _universService;

        public HomeController(IRightService rightService, IApplicationUserManager applicationUserManager, IWebSessionService webSessionService, IUniverseService universService)
        {
            _userManager = applicationUserManager;
            _rightService = rightService;
            _webSessionService = webSessionService;
            _universService = universService;
        }

        public ActionResult Index()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            var idLogin = cla.Claims.Where(e => e.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var login = cla.Claims.Where(e => e.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            var password = cla.Claims.Where(e => e.Type == ClaimTypes.Hash).Select(c => c.Value).SingleOrDefault();
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            //var test = GestionWeb.GetWebWord(1052, _webSession.SiteLanguage)
            var resList = _rightService.GetModulesList(idWS);
            var res = _rightService.GetModules(idWS);
            var docu = new Documents()
            {
                Id = 1,
                Label = "AdExpress News",
                InfosNews = new List<InfosNews>()
                {
                    new InfosNews()
                    {
                        Label = "Novembre 2015",
                        Url = "/AdExNews/AdexNews_201511.pdf"
                    },
                    new InfosNews()
                    {
                        Label = "Octobre 2015",
                        Url = "/AdExNews/AdexNews_201510.pdf"
                    }
                }
            };

            var Home = new HomePageViewModel()
            {
                ModuleRight = res,
                Modules = resList,
                Documents = new List<Documents>() {
                    new Documents()
                    {
                        Id = 1,
                        Label = "AdExpress News",
                        InfosNews = new List<InfosNews>()
                        {
                            new InfosNews()
                            {
                                Label = "Novembre 2015",
                                Url = "/AdExNews/AdexNews_201511.pdf"
                            },
                            new InfosNews()
                            {
                                Label = "Octobre 2015",
                                Url = "/AdExNews/AdexNews_201510.pdf"
                            }
                        }
                    },
                    new Documents()
                    {
                        Id = 2,
                        Label = "AdExpress Report",
                        InfosNews = new List<InfosNews>()
                        {
                            new InfosNews()
                            {
                                Label = "Novembre 2015",
                                Url = "/AdExReport/AdExReport_201511.pdf"
                            },
                            new InfosNews()
                            {
                                Label = "Octobre 2015",
                                Url = "/AdExReport/AdExReport_201510.pdf"
                            }
                        }
                    },
                     new Documents()
                    {
                        Id = 3,
                        Label = "Documents",
                        InfosNews = null
                    }
                }
            };
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
                SavedResults = new Domain.AdExpressUniversResponse {  UniversType= Domain.UniversType.Result, UniversGroups = new List<Domain.UserUniversGroup>() },
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
            var alertsResposne = _universService.GetUserAlerts(idWebSession);
            model.Alerts = alertsResposne.Alerts;
            #endregion
            model.PresentationModel = LoadPresentationBar(result.SiteLanguage, false);
            model.Labels = LoadPageLabels(result.SiteLanguage);
            return View(model);
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
                TimeSchedule= GestionWeb.GetWebWord(LanguageConstantes.TimeSchedule, siteLanguage)
            };
            return result;
        }
    }
}
