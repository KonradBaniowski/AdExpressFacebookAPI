using Kantar.AdExpress.Service.Core.BusinessService;
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

        public HomeController(IRightService rightService, IApplicationUserManager applicationUserManager, IWebSessionService webSessionService)
        {
            _userManager = applicationUserManager;
            _rightService = rightService;
            _webSessionService = webSessionService;
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
            var model = new MyAdExpressViewModel();
            PresentationModel Presentation = new PresentationModel();
            model.PresentationModel = LoadPresentationBar(33);
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
    }
}
