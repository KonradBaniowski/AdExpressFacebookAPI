using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
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
            _mediaSchedule = mediaSchedule;
        }

        // GET: CreativeMediaSchedule
        public ActionResult Index(string siteLanguage, string mediaTypeIds, string beginDate, string endDate, string productIds, string creativeIds)
        {
            var pageHelper = new Helpers.PageHelper();
            var model = new Models.MediaSchedule.CreativeMediaScheduleResultsViewModel();
            

            model.Labels = pageHelper.LoadPageLabels(_siteLanguage, _controller);
            model.ErrorMessages = new List<string>();

            if (string.IsNullOrEmpty(siteLanguage))
                model.ErrorMessages.Add("Erreur : Le paramètre siteLanguage est invalide");
            else
            {
                int siteLang = Convert.ToInt32(siteLanguage);
                if(siteLang != WebApplicationParameters.DefaultLanguage)
                    model.ErrorMessages.Add("Erreur : Le paramètre siteLanguage est invalide");
            }
            
            if (string.IsNullOrEmpty(mediaTypeIds))
                model.ErrorMessages.Add("Erreur : Le paramètre mediaTypeIds est invalide");

            if (string.IsNullOrEmpty(productIds))
                model.ErrorMessages.Add("Erreur : Le  paramètre productIds est invalide");

            if (beginDate == null || beginDate.Length != 8)
                model.ErrorMessages.Add("Erreur : Le  champ beginDate est invalide");

            if (endDate == null || endDate.ToString().Length != 8)
                model.ErrorMessages.Add("Erreur : Le  champ endDate est invalide");

            if(model.ErrorMessages.Count > 0)
                return View(model);

            model.SiteLanguage = Convert.ToInt32(siteLanguage);
            model.EndDate = endDate;
            model.BeginDate = beginDate;
            model.ProductIds = productIds;
            model.CreativeIds = creativeIds;
            model.MediaTypeIds = mediaTypeIds;

            return View(model);
        }

        public JsonResult Results(int siteLanguage,string mediaTypeIds,int beginDate, int endDate ,string productIds,string creativeIds)
        {
            GridResultResponse creativeMediaScheduleResponse;
            GridResult gridResult;
            JsonResult jsonModel;
            string idWebSession = string.Empty;
            CreativeMediaScheduleRequest request = new CreativeMediaScheduleRequest { SiteLanguage = siteLanguage, MediaTypeIds = mediaTypeIds, BeginDate = beginDate, EndDate = endDate, ProductIds = productIds, CreativeIds = creativeIds };

            try
            {
                creativeMediaScheduleResponse = _mediaSchedule.GetGridResult(request);
            }
            catch(Exception ex)
            {
                var response = new { success = false, errorMessage = ex.Message };
                jsonModel = Json(response, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }

            if (creativeMediaScheduleResponse.Success)
            {
                gridResult = creativeMediaScheduleResponse.GridResult;

                if (!gridResult.HasData)
                    return null;

                if (gridResult.HasMoreThanMaxRowsAllowed)
                {
                    var response = new { success = true, hasMoreThanMaxRowsAllowed = true };
                    jsonModel = Json(response, JsonRequestBehavior.AllowGet);
                    jsonModel.MaxJsonLength = Int32.MaxValue;

                    return jsonModel;
                }

                string jsonData = JsonConvert.SerializeObject(gridResult.Data);

                var obj = new { success = true, datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, columnsNotAllowedSorting = gridResult.ColumnsNotAllowedSorting, needfixedcolumns = gridResult.NeedFixedColumns, hasMSCreatives = gridResult.HasMSCreatives, unit = gridResult.Unit };
                jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;


                return jsonModel;
            }
            else
            {
                var response = new { success = false, errorMessage = creativeMediaScheduleResponse.Message };
                jsonModel = Json(response, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }
        }
    }
}