using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace Km.AdExpressClientWeb.Controllers
{
    public class AdvertisingAgencyExportController : Controller
    {
        private IWebSessionService _webSessionService;
        private IAdvertisingAgencyService _advertisingAgencyService;
        private IDetailSelectionService _detailSelectionService;

        public AdvertisingAgencyExportController(IAdvertisingAgencyService advertisingAgencyService, IWebSessionService webSessionService, IDetailSelectionService detailSelectionService)
        {
            _advertisingAgencyService = advertisingAgencyService;
            _webSessionService = webSessionService;
            _detailSelectionService = detailSelectionService;
        }

        public ActionResult Index(ResultTable.SortOrder sortOrder, int columnIndex)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            ResultTable data = _advertisingAgencyService.GetResultTable(idWebSession);
            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document, session, _detailSelectionService.GetDetailSelection(idWebSession));
            export.Export(document, data, session, false, sortOrder, columnIndex);

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