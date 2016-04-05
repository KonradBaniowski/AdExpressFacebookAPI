using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Models.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    public class CreativeController : Controller
    {
        private ICreativeService _creativeService;
        private IUniverseService _universService;

        public CreativeController(ICreativeService creativeService, IUniverseService universService)
        {
            _creativeService = creativeService;
            _universService = universService;
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

            var result = _universService.GetBranches(idWebSession, TNS.Classification.Universe.Dimension.product, true);
            model.SiteLanguage = result.SiteLanguage;

            return View(model);
        }


        [HttpPost]
        public JsonResult CreativeResult(string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle, bool isVehicleChanged)
        {

            string jsonData = "";

            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var reponse = _creativeService.GetCreativeGridResult(idWebSession, ids, zoomDate, idUnivers, moduleId, idVehicle, isVehicleChanged);

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

            var reponse = _creativeService.GetPresentVehicles(idWebSession, ids, idUnivers, moduleId);

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


    }
}