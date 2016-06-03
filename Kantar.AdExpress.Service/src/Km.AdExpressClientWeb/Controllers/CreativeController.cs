﻿using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;

namespace Km.AdExpressClientWeb.Controllers
{
    public class CreativeController : Controller
    {
        private ICreativeService _creativeService;
        private IUniverseService _universService;
        private IWebSessionService _webSessionService;
        public CreativeController(ICreativeService creativeService, IUniverseService universService, IWebSessionService webSessionService)
        {
            _creativeService = creativeService;
            _universService = universService;
            _webSessionService = webSessionService;
        }

        // GET: Creative
        public ActionResult Index(string ids, string zoomDate, string idUnivers, string moduleId, string idVehicle)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var model = new InsertionCreativeViewModel()
            {
                paramsUrl = new List<string>(),
                SiteLanguage = 33, // Default
            };

            model.paramsUrl.Add(ids);
            model.paramsUrl.Add(zoomDate);
            model.paramsUrl.Add(idUnivers);
            model.paramsUrl.Add(moduleId);
            model.paramsUrl.Add(idVehicle);
            var result = _webSessionService.GetSiteLanguage(idWebSession);
            model.SiteLanguage = result;
            model.Labels = LoadPageLabels(result);

            return View(model);
        }


        [HttpPost]
        public JsonResult CreativeResult(string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle, bool isVehicleChanged, List<EvaliantFilter> listEvaliantFilter)
        {

            string jsonData = "";

            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var reponse = _creativeService.GetCreativeGridResult(idWebSession, ids, zoomDate, idUnivers, moduleId, idVehicle, isVehicleChanged, listEvaliantFilter);

            try
            {
                if (!reponse.GridResult.HasData)
                    return null;

                if (reponse.Message == null)
                {
                    jsonData = JsonConvert.SerializeObject(reponse.GridResult.Data);
                    JsonResult jsonModel = Json(new { datagrid = jsonData, columns = reponse.GridResult.Columns, schema = reponse.GridResult.Schema, columnsfixed = reponse.GridResult.ColumnsFixed, needfixedcolumns = reponse.GridResult.NeedFixedColumns, filtertitle = reponse.GridResult.Filter.Title, filterdatas = reponse.GridResult.Filter.Datas }, JsonRequestBehavior.AllowGet);
                    jsonModel.MaxJsonLength = Int32.MaxValue;

                    return jsonModel;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }


        public JsonResult GetPresentVehicles(string ids, int idUnivers, long moduleId)
        {
            string jsonData = "";

            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var reponse = _creativeService.GetPresentVehicles(idWebSession, ids, idUnivers, moduleId, true);

            jsonData = JsonConvert.SerializeObject(reponse);
            JsonResult jsonModel = Json(jsonData, JsonRequestBehavior.AllowGet);

            return jsonModel;
        }

        public JsonResult GetCreativePath(string idVersion, long idVehicle)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var reponse = _creativeService.GetCreativePath(idWebSession, idVersion, idVehicle);

            return Json(new { PathReadingFile = reponse.PathReadingFile, PathDownloadingFile = reponse.PathDownloadingFile });
        }

        private Labels LoadPageLabels(int siteLanguage)
        {
            var result = new Labels
            {
                EmptyGrid = GestionWeb.GetWebWord(LanguageConstantes.EmptyGrid, siteLanguage),
                CreativeLabel = GestionWeb.GetWebWord(LanguageConstantes.CreativeLabel, siteLanguage),
                DownloadLabel = GestionWeb.GetWebWord(LanguageConstantes.DownloadLabel, siteLanguage),
                VisuelLabel = GestionWeb.GetWebWord(LanguageConstantes.VisuelLabel, siteLanguage),
                FiltreLabel = GestionWeb.GetWebWord(LanguageConstantes.FiltreLabel, siteLanguage),
                Submit = GestionWeb.GetWebWord(LanguageConstantes.Submit, siteLanguage)
            };

            return result;
        }


    }
}