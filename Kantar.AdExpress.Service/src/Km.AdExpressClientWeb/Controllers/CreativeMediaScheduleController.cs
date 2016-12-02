using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Web;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    public class CreativeMediaScheduleController : Controller
    {
        private IWebSessionService _webSessionService;
        private int _siteLanguage = WebApplicationParameters.DefaultLanguage;
        private IMediaScheduleService _mediaSchedule;
        private const string _controller = "CreativeMediaSchedule";

        public CreativeMediaScheduleController( IWebSessionService webSessionService, IMediaScheduleService mediaSchedule)
        {
           
            _webSessionService = webSessionService;
         
        }

        // GET: CreativeMediaSchedule
        public ActionResult Index(int siteLanguage, string mediaTypeIds, int beginDate, int endDate, string productIds, string creativeIds)
        {
            if (siteLanguage > 0) _siteLanguage = siteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            ViewBag.SiteLanguage = _siteLanguage;
            var pageHelper = new Helpers.PageHelper();
            var model = new Models.MediaSchedule.CreativeMediaScheduleResultsViewModel
            {
                SiteLanguage = _siteLanguage,
                EndDate = endDate,
                BeginDate = beginDate,
                productIds = productIds,
                CreativeIds = creativeIds,
                MediaTypeIds = mediaTypeIds,
                Labels = pageHelper.LoadPageLabels(_siteLanguage, _controller),
                IsAlertVisible = false,
                ExportTypeViewModels = PageHelper.GetExportTypes(WebApplicationParameters.CountryCode, WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA, _siteLanguage)
            };
            if (string.IsNullOrEmpty(mediaTypeIds)) model.ErrorMessage = "Erreur : Le  paramètre mediaTypeIds est invalide";
            if (string.IsNullOrEmpty(productIds)) model.ErrorMessage = "Erreur : Le  paramètre productIds est invalide";
            if (beginDate.ToString().Length != 8) model.ErrorMessage = "Erreur : Le  champ beginDate est invalide";
            if (endDate.ToString().Length != 8) model.ErrorMessage = "Erreur : Le  champ endDate est invalide";


            return View(model);
        }

        public JsonResult Results(int siteLanguage,string mediaTypeIds,int beginDate, int endDate ,string productIds,string creativeIds)
        {
            GridResult gridResult;
            JsonResult jsonModel;
            string idWebSession = string.Empty;

           gridResult = _mediaSchedule.GetGridResult(idWebSession, "", this.HttpContext);

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

            var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, columnsNotAllowedSorting = gridResult.ColumnsNotAllowedSorting, needfixedcolumns = gridResult.NeedFixedColumns, hasMSCreatives = gridResult.HasMSCreatives, unit = gridResult.Unit };
            jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
            jsonModel.MaxJsonLength = Int32.MaxValue;

          
            return jsonModel;
        }
    }
}