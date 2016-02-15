using Km.AdExpressClientWeb.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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

            var model = new HomePageViewModel()
            {
                ModuleRight = new System.Collections.Generic.Dictionary<long, Module>()
                {
                    { 198, new Module() },
                    { 197, new Module() },
                    { 7216, new Module() },
                    { 1781, new Module() },
                    { 4370, new Module() }
                },
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
            return View(model);
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
