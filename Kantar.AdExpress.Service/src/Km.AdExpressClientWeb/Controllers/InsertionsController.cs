using Kantar.AdExpress.Service.Core.BusinessService;
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
    public class InsertionsController : Controller
    {
        private IInsertionsService _insertionsService;

        public InsertionsController(IInsertionsService insertionsService)
        {
            _insertionsService = insertionsService;
        }

        // GET: Insertions
        public ActionResult Index(string ids, string zoomDate, string idUnivers, string moduleId, string idVehicle)
        {
            DateTime today = DateTime.Today;
            DateTime past = DateTime.Today.AddDays(-30).Date;

            InsertionViewModel model = new InsertionViewModel();
            var mediasTabs = new List<Medias> {
                new Medias {
                    Label = "Press",
                    LabelID = 989
                },
                new Medias  {
                    Label = "Television",
                    LabelID = 999
                }
            };
            model.Medias = mediasTabs;
            model.DateBegin = past;
            model.DateEnd = today;

            List<string> datas = new List<string>();
            datas.Add(ids);
            datas.Add(zoomDate);
            datas.Add(idUnivers);
            datas.Add(moduleId);
            datas.Add(idVehicle);
            model.datas = datas;
            return View(model);
        }

        [HttpPost]
        public JsonResult InsertionsResult(string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle)
        {
            string jsonData = "";

            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var reponse = _insertionsService.GetInsertionsGridResult(idWebSession, ids, zoomDate, idUnivers, moduleId, idVehicle);

            if (reponse.Message == null)
            {
                jsonData = JsonConvert.SerializeObject(reponse.GridResult.Data);
                JsonResult jsonModel = Json(new { datagrid = jsonData, columns = reponse.GridResult.Columns, schema = reponse.GridResult.Schema, columnsfixed = reponse.GridResult.ColumnsFixed, needfixedcolumns = reponse.GridResult.NeedFixedColumns }, JsonRequestBehavior.AllowGet);
                return jsonModel;
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


    }
}