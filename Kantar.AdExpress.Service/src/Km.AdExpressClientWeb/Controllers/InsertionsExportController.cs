using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    public class InsertionsExportController : Controller
    {
        private IInsertionsService _insertionsService;

        public InsertionsExportController(IInsertionsService insertionsService)
        {
            _insertionsService = insertionsService;
        }

        // GET: InsertionsExport 
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InsertionsResult(string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle, bool isVehicleChanged)
        {
           
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var reponse = _insertionsService.GetInsertionsGridResult(idWebSession, ids, zoomDate, idUnivers, moduleId, idVehicle, isVehicleChanged);

            return View();
        }
    }
}