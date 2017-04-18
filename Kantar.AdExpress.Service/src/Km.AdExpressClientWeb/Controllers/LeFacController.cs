using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = Role.ADEXPRESS)]
    public class LeFacController : Controller
    {
        private ILeFacService _leFacService;

        public LeFacController(ILeFacService leFacService)
        {
            _leFacService = leFacService;
        }

        public PartialViewResult LeFacInfos(string idAddress, string advertiser)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            LeFac leFacInfos = _leFacService.GetLeFacInfos(idWebSession, idAddress, advertiser, this.HttpContext);

            return PartialView(leFacInfos);
        }
    }
}