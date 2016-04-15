using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    public class DetailSelectionController : Controller
    {
        private IDetailSelectionService _detailSelectionService;
        public DetailSelectionController(IDetailSelectionService detailSelectionService)
        {
            _detailSelectionService = detailSelectionService;
        }

        public ActionResult GetDetailSelection()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            _detailSelectionService.GetDetailSelection(idWS);
            return View();
        }
    }
}