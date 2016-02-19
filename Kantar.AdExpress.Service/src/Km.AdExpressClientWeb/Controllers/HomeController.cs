using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web.Mvc;
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

        public HomeController(IRightService rightService, IApplicationUserManager applicationUserManager)
        {
            _userManager = applicationUserManager;
            _rightService = rightService;
            
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

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
