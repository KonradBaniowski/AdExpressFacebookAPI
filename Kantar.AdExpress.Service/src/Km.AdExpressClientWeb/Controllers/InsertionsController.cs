using Kantar.AdExpress.Service.Core.BusinessService;
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
    [Authorize]
    public class InsertionsController : Controller
    {
        private IInsertionsService _insertionsService;
        private IDetailLevelService _detailLevelService;
        private IUniverseService _universService;
        private IWebSessionService _webSessionService;

        public InsertionsController(IInsertionsService insertionsService, IDetailLevelService detailLevelservice, IUniverseService universService, IWebSessionService webSessionService)
        {
            _insertionsService = insertionsService;
            _detailLevelService = detailLevelservice; ;
            _universService = universService;
            _webSessionService = webSessionService;
        }

        // GET: Insertions
        public ActionResult Index(string ids, string zoomDate, string idUnivers, string moduleId, string idVehicle)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var model = new InsertionCreativeViewModel()
            {
                paramsUrl = new List<string>()
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
        public JsonResult InsertionsResult(string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle, bool isVehicleChanged)
        {
            string jsonData = "";

            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var reponse = _insertionsService.GetInsertionsGridResult(idWebSession, ids, zoomDate, idUnivers, moduleId, idVehicle, isVehicleChanged);

            try
            {
                if (!reponse.GridResult.HasData)
                    return null;

                if (reponse.Message == null)
                {
                    jsonData = JsonConvert.SerializeObject(reponse.GridResult.Data);
                    JsonResult jsonModel = Json(new { datagrid = jsonData, columns = reponse.GridResult.Columns, schema = reponse.GridResult.Schema, columnsfixed = reponse.GridResult.ColumnsFixed, needfixedcolumns = reponse.GridResult.NeedFixedColumns }, JsonRequestBehavior.AllowGet);
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

            var reponse = _insertionsService.GetPresentVehicles(idWebSession, ids, idUnivers, moduleId);

            jsonData = JsonConvert.SerializeObject(reponse);
            JsonResult jsonModel = Json(jsonData, JsonRequestBehavior.AllowGet);

            return jsonModel;
        }

        public JsonResult GetCreativePath(string idVersion, long idVehicle )
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var reponse = _insertionsService.GetCreativePath(idWebSession, idVersion, idVehicle);

            return Json(new { PathReadingFile = reponse.PathReadingFile, PathDownloadingFile = reponse.PathDownloadingFile });
        }

        
        public ActionResult GetDetailLevel(int idVehicle, bool isVehicleChanged)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var detailLevel = _detailLevelService.GetDetailLevelItem(idWebSession, idVehicle, isVehicleChanged);

            ViewBag.SiteLanguage = _webSessionService.GetSiteLanguage(idWebSession);

            return PartialView("_DetailLevel", detailLevel);
        }

        public void SetDetailLevel(UserFilter userFilter)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            _detailLevelService.SetDetailLevelItem(idWebSession, userFilter);
        }       

        private Labels LoadPageLabels(int siteLanguage)
        {
            var result = new Labels
            {
                EmptyGrid = GestionWeb.GetWebWord(LanguageConstantes.EmptyGrid, siteLanguage),
                InsertionsLabel = GestionWeb.GetWebWord(LanguageConstantes.InsertionsLabel, siteLanguage),
                DownloadLabel = GestionWeb.GetWebWord(LanguageConstantes.DownloadLabel, siteLanguage),
                VisuelLabel = GestionWeb.GetWebWord(LanguageConstantes.VisuelLabel, siteLanguage)
            };

            return result;
        }

    }
}