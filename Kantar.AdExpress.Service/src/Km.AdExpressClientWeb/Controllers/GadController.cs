using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class GadController : Controller
    {
        private IGadService _gadService;

        public GadController(IGadService gadService)
        {
            _gadService = gadService;
        }

        public PartialViewResult GadInfos(string idAddress, string advertiser)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            Gad gadInfos = _gadService.GetGadInfos(idWebSession, idAddress, advertiser, this.HttpContext);

            return PartialView(gadInfos);
        }
    }
}