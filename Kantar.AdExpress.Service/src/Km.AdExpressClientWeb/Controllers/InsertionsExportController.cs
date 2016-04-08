using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Web.Core.Sessions;

namespace Km.AdExpressClientWeb.Controllers
{
    public class InsertionsExportController : Controller
    {
        private IInsertionsService _insertionsService;

        public InsertionsExportController(IInsertionsService insertionsService)
        {
            _insertionsService = insertionsService;
        }

        // GET: InsertionsExport 
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult InsertionsResult(string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle)
        {
           
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var data = _insertionsService.GetInsertionsResult(idWebSession, ids, zoomDate, idUnivers, moduleId, idVehicle.Value);

            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document);
            export.Export(document, data, session);

            string documentFileNameRoot;
            documentFileNameRoot = string.Format("Document.{0}", document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx");

            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";

            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));

            Response.End();

            return View();
        }
    }
}