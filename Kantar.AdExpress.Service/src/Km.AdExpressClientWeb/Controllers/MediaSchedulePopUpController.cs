using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using Km.AdExpressClientWeb.Models.Shared;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using CoreDomain = Kantar.AdExpress.Service.Core.Domain;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class MediaSchedulePopUpController : Controller
    {
        private IMediaScheduleService _mediaSchedule;
        private IOptionMediaScheduleService _optionMediaScheduleService;
        private ISubPeriodService _subPeriodService;

        public MediaSchedulePopUpController(IMediaScheduleService mediaSchedule, IOptionMediaScheduleService optionMediaScheduleService, ISubPeriodService subPeriodService)
        {
            _mediaSchedule = mediaSchedule;
            _optionMediaScheduleService = optionMediaScheduleService;
            _subPeriodService = subPeriodService;
        }

        // GET: MediaSchedulePopUp
        public ActionResult Index(string id, string level, string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            WebSession customerSession = (WebSession)WebSession.Load(idWebSession);

            var model = new InsertionCreativeViewModel()
            {
                paramsUrl = new List<string>(),
                SiteLanguage = WebApplicationParameters.DefaultLanguage, // Default
            };

            model.paramsUrl.Add(id);
            model.paramsUrl.Add(level);
            model.paramsUrl.Add(string.IsNullOrEmpty(zoomDate) ? zoomDate : string.Empty);
            model.SiteLanguage = customerSession.SiteLanguage;

            _mediaSchedule.SetProductLevel(idWebSession, int.Parse(id), int.Parse(level));

            return View(model);
        }

        public JsonResult MediaScheduleResult(string id, string level, string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            GridResult gridResult;

            if (string.IsNullOrEmpty(zoomDate))
                gridResult = _mediaSchedule.GetGridResult(idWebSession, "");
            else
                gridResult = _mediaSchedule.GetGridResult(idWebSession, zoomDate);

            if (!gridResult.HasData)
                return null;

            string jsonData = JsonConvert.SerializeObject(gridResult.Data);

            var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, needfixedcolumns = gridResult.NeedFixedColumns, hasMSCreatives = gridResult.HasMSCreatives };
            JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
            jsonModel.MaxJsonLength = Int32.MaxValue;

            return jsonModel;
        }

        public ActionResult MSCreativesResult(string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            CoreDomain.MSCreatives creatives = _mediaSchedule.GetMSCreatives(idWebSession, zoomDate);

            return PartialView("_MSCreativesResult", creatives);
        }

        public ActionResult ResultOptions()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            OptionsMediaSchedule options = _optionMediaScheduleService.GetOptions(idWebSession);
            return PartialView("_ResultOptions", options);
        }

        public void SetResultOptions(UserFilter userFilter)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            _optionMediaScheduleService.SetOptions(idWebSession, userFilter);
        }
    }
}