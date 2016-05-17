using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class AnalysisController : Controller
    {
        // GET: Analysis
        public ActionResult Market()
        {
            return View();
        }
    }
}