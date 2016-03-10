using Kantar.AdExpress.Service.Core.BusinessService;
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
            List<string> datas = new List<string>();
            datas.Add(ids);
            datas.Add(zoomDate);
            datas.Add(idUnivers);
            datas.Add(moduleId);
            datas.Add(idVehicle);

            return View(datas);
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


    }
}