using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Web;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    public class ProgramAnalysisController : Controller
    {
        private IWebSessionService _webSessionService;
        private IOptionService _optionService;
        private ICampaignAnalysisService _campaignAnalysisService;
        private const string _controller = "ProgramAnalysis";
        private int _siteLanguage = WebApplicationParameters.DefaultLanguage;

        public ProgramAnalysisController(IWebSessionService webSessionService, IOptionService optionService, ICampaignAnalysisService campaignAnalysisService)
        {
            _webSessionService = webSessionService;
            _optionService = optionService;
            _campaignAnalysisService = campaignAnalysisService;
        }

        // GET: ProgramAnalysis
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Results()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _webSessionService.GetWebSession(idSession);
            _siteLanguage = result.WebSession.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            ViewBag.SiteLanguage = _siteLanguage;
            var resultNode = new NavigationNode { Position = 4 };
            var pageHelper = new Helpers.PageHelper();
            var model = new Models.Shared.ResultsViewModel
            {
                NavigationBar = pageHelper.LoadNavBar(idSession, _controller, _siteLanguage, 4),
                Presentation = pageHelper.LoadPresentationBar(result.WebSession.SiteLanguage, result.ControllerDetails),
                Labels = pageHelper.LoadPageLabels(result.WebSession.SiteLanguage, _controller),
                IsAlertVisible = PageHelper.IsAlertVisible(WebApplicationParameters.CountryCode, idSession),
                ExportTypeViewModels = PageHelper.GetExportTypes(WebApplicationParameters.CountryCode, WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES, _siteLanguage)
            };

            return View(model);
        }

        public JsonResult ProgramAnalysisResult()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            JsonResult jsonModel;

            var gridResult = _campaignAnalysisService.GetGridResult(idWebSession, this.HttpContext);
            if (!gridResult.HasData)
                return null;

            if (gridResult.HasMoreThanMaxRowsAllowed)
            {
                var response = new { hasMoreThanMaxRowsAllowed = true };
                jsonModel = Json(response, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }

            string jsonData = JsonConvert.SerializeObject(gridResult.Data);
            var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, columnsNotAllowedSorting = gridResult.ColumnsNotAllowedSorting, needfixedcolumns = gridResult.NeedFixedColumns, unit = gridResult.Unit, sortOrder = gridResult.SortOrder, sortKey = gridResult.SortKey };
            jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
            jsonModel.MaxJsonLength = Int32.MaxValue;

            return jsonModel;
        }

        public ActionResult ResultOptions()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            Options options = _optionService.GetOptions(idWebSession, this.HttpContext);
            return PartialView("_ResultOptions", options);
        }

        public void SetResultOptions(UserFilter userFilter)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            _optionService.SetOptions(idWebSession, userFilter, this.HttpContext);
        }

        public JsonResult SaveCustomDetailLevels(string levels, string type)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            JsonResult jsonModel = new JsonResult();
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            Kantar.AdExpress.Service.Core.Domain.SaveLevelsResponse response = _optionService.SaveCustomDetailLevels(idWebSession, levels, type, this.HttpContext);
            jsonModel = Json(new { Id = response.CustomDetailLavelsId, Label = response.CustomDetailLavelsLabel, Message = response.Message });

            return jsonModel;
        }

        public JsonResult RemoveCustomDetailLevels(string detailLevel)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            JsonResult jsonModel = new JsonResult();
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            string message = _optionService.RemoveCustomDetailLevels(idWebSession, detailLevel);
            jsonModel = Json(new { Message = message });

            return jsonModel;
        }

    }
}