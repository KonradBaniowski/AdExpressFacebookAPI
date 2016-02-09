using Km.AdExpressClientWeb.Models;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new HomePageViewModel()
            {
                ModuleRight = new System.Collections.Generic.Dictionary<long, Module>()
                {
                    { 196, new Module() },
                    { 197, new Module() },
                    { 198, new Module() }
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
