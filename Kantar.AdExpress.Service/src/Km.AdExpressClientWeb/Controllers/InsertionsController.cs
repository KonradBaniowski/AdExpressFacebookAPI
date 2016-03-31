using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using Km.AdExpressClientWeb.Models.Insertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class InsertionsController : Controller
    {
        private IInsertionsService _insertionsService;
        private IDetailLevelService _detailLevelService;

        public InsertionsController(IInsertionsService insertionsService, IDetailLevelService detailLevelservice)
        {
            _insertionsService = insertionsService;
            _detailLevelService = detailLevelservice;
        }

        // GET: Insertions
        public ActionResult Index(string ids, string zoomDate, string idUnivers, string moduleId, string idVehicle)
        {
            List<string> paramsUrl = new List<string>();
            paramsUrl.Add(ids);
            paramsUrl.Add(zoomDate);
            paramsUrl.Add(idUnivers);
            paramsUrl.Add(moduleId);
            paramsUrl.Add(idVehicle);
            return View(paramsUrl);
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

        
        public ActionResult GetDetailLevel(int idVehicle)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var detailLevel = _detailLevelService.GetDetailLevelItem(idWebSession, idVehicle);

            return PartialView("_DetailLevel", detailLevel);
        }

        public void SetDetailLevel(UserFilter userFilter)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            _detailLevelService.SetDetailLevelItem(idWebSession, userFilter);
        }

    }
}