using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Alert.Domain;
using TNS.Ares.Alerts.DAL;
using TNS.Ares.Domain.Layers;
using TNS.Ares.Domain.LS;

namespace Km.AdExpressClientWeb.Controllers
{
    public class AlertController : Controller
    {
        private IAlertService _alertService;
        private IApplicationUserManager _userManager;

        public AlertController(IAlertService alertService, IApplicationUserManager userManager)
        {
            _alertService = alertService;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index(string idSession, int? idOcc, int? idAlert)
        {
            if (!idOcc.HasValue) idOcc = -1;
            if (!idAlert.HasValue) idAlert = -1;

            DataAccessLayer layer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
            TNS.FrameWork.DB.Common.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
            IAlertDAL alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null);
            AlertOccurence occ = alertDAL.GetOccurrence(idOcc.Value, idAlert.Value);

            if (occ != null)
            {
                Alert alert = alertDAL.GetAlert(occ.AlertId);
                if (alert != null)
                {
                    WebSession session = (WebSession)alert.Session;

                    var authenticate = await _userManager.PasswordSignIn(session.CustomerLogin.Login, session.CustomerLogin.PassWord, false, shouldLockout: false);

                    switch (authenticate)
                    {
                        case SignInStatus.Success:
                            this.Session["session"] = session;
                            this.Session["occ"] = occ;
                            return RedirectToAction("redirect");
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View("Home");
                    }
                }
            }

            return View("Home");
        }

        [Authorize]
        public ActionResult redirect()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            var idWS = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession session = (WebSession)this.Session["session"];
            AlertOccurence occ = (AlertOccurence)this.Session["occ"];

            var redirectUrl = _alertService.GetRedirectUrl(session, idWS, occ);
            return RedirectToAction("Results", redirectUrl);
        }
    }
}