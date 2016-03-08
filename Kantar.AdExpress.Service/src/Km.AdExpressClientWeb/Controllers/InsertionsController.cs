using Kantar.AdExpress.Service.Core.BusinessService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    public class InsertionsController : Controller
    {
        private IInsertionsService _insertionsService;

        public InsertionsController(IInsertionsService insertionsService)
        {
            _insertionsService = insertionsService;
        }

        // GET: Insertions
        public ActionResult Index(string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var gridResult = _insertionsService.GetInsertionsGridResult(idWebSession, ids, zoomDate, idUnivers, moduleId, idVehicle);
           

            return View();
        }


    }
}