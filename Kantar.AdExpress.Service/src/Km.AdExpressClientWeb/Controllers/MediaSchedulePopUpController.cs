using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Models.Shared;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using CoreDomain = Kantar.AdExpress.Service.Core.Domain;
using FrameWorkResults = TNS.AdExpress.Constantes.FrameWork.Results;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = Role.ADEXPRESS)]
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
        public ActionResult Index(string id, string level, string zoomDate, string idVehicle)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            WebSession customerSession = (WebSession)WebSession.Load(idWebSession);

            var model = new InsertionCreativeViewModel()
            {
                paramsUrl = new List<string>(),
                SiteLanguage = WebApplicationParameters.DefaultLanguage, // Default
                Labels = LabelsHelper.LoadPageLabels(customerSession.SiteLanguage)
            };

            model.paramsUrl.Add(id);
            model.paramsUrl.Add(level);
            model.paramsUrl.Add(!string.IsNullOrEmpty(zoomDate) ? zoomDate : string.Empty);
            model.paramsUrl.Add(!string.IsNullOrEmpty(idVehicle) ? idVehicle : string.Empty);
            model.IsAdNetTrack = !string.IsNullOrEmpty(idVehicle) ? true : false;
            model.SiteLanguage = customerSession.SiteLanguage;

            if(!string.IsNullOrEmpty(idVehicle))
            {
                customerSession.AdNetTrackSelection = new AdNetTrackProductSelection((FrameWorkResults.AdNetTrackMediaSchedule.Type)int.Parse(level), int.Parse(id));
                customerSession.Save();
            }
            else
            {
                _mediaSchedule.SetProductLevel(idWebSession, Int64.Parse(id), int.Parse(level));
            }

            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(customerSession.SiteLanguage);
            ViewBag.SiteLanguage = customerSession.SiteLanguage;

            return View(model);
        }

        public JsonResult MediaScheduleResult(string id, string level, string zoomDate, string idVehicle)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            GridResult gridResult;

            if(string.IsNullOrEmpty(idVehicle)) {
                if (string.IsNullOrEmpty(zoomDate))
                    gridResult = _mediaSchedule.GetGridResult(idWebSession, "", "", this.HttpContext);
                else
                    gridResult = _mediaSchedule.GetGridResult(idWebSession, zoomDate, "", this.HttpContext);
            }
            else {
                if (string.IsNullOrEmpty(zoomDate))
                    gridResult = _mediaSchedule.GetGridResult(idWebSession, "", idVehicle, this.HttpContext);
                else
                    gridResult = _mediaSchedule.GetGridResult(idWebSession, zoomDate, idVehicle, this.HttpContext);
            }
            

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

            CoreDomain.MSCreatives creatives = _mediaSchedule.GetMSCreatives(idWebSession, zoomDate, this.HttpContext);

            return PartialView("_MSCreativesResult", creatives);
        }

        public ActionResult ResultOptions(bool isAdNetTrack)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            OptionsMediaSchedule options = _optionMediaScheduleService.GetOptions(idWebSession, isAdNetTrack, this.HttpContext);
            return PartialView("_ResultOptions", options);
        }

        public void SetResultOptions(UserFilter userFilter, bool isAdNetTrack)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            _optionMediaScheduleService.SetOptions(idWebSession, userFilter, isAdNetTrack, this.HttpContext);
        }
    }
}