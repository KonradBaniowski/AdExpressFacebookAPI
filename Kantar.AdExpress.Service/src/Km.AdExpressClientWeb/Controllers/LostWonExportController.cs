using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace Km.AdExpressClientWeb.Controllers
{
    public class LostWonExportController : Controller
    {
        private ILostWonService _lostWonService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IDetailSelectionService _detailSelectionService;

        public LostWonExportController(ILostWonService lostWonService, IMediaService mediaService, IWebSessionService webSessionService, IDetailSelectionService detailSelectionService)
        {
            _lostWonService = lostWonService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _detailSelectionService = detailSelectionService;
        }

        // GET: LostWonExport
        public ActionResult Index(ResultTable.SortOrder sortOrder, int columnIndex)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            ResultTable data = _lostWonService.GetResultTable(idWebSession, this.HttpContext);
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

        public ActionResult ResultBrut(ResultTable.SortOrder sortOrder, int columnIndex)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            ResultTable data = _lostWonService.GetResultTable(idWebSession, this.HttpContext);
            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document, session, _detailSelectionService.GetDetailSelection(idWebSession));
            export.Export(document, data, session, true, sortOrder, columnIndex);

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

        private void export(ResultTable data)
        {
            //PortfolioExportController pec = new PortfolioExportController(_lostWonService, _mediaService,_webSessionService)
        }
    }
}