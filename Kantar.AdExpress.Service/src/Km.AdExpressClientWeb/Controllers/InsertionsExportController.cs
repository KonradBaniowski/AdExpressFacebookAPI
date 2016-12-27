using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = Role.ADEXPRESS)]
    public class InsertionsExportController : Controller
    {
        private IInsertionsService _insertionsService;
        private IDetailSelectionService _detailSelectionService;

        public InsertionsExportController(IInsertionsService insertionsService, IDetailSelectionService detailSelectionService)
        {
            _insertionsService = insertionsService;
            _detailSelectionService = detailSelectionService;
        }

        // GET: InsertionsExport 
        [Authorize(Roles = Role.ADEXPRESS)]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult InsertionsResult(string ids, string zoomDate, int idUnivers, long moduleId, long? idVehicle)
        {
           
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var data = _insertionsService.GetInsertionsResult(idWebSession, ids, zoomDate, idUnivers, moduleId, idVehicle.Value, this.HttpContext, true);

            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document, session, _detailSelectionService.GetDetailSelection(idWebSession));
            export.Export(document, data, session, true, ResultTable.SortOrder.NONE, 1, true);

            document.Worksheets.ActiveSheetIndex = 1;

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