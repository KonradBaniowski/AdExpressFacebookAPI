﻿using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Km.AdExpressClientWeb.Helpers;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = Role.ADEXPRESS)]
    public class PresentAbsentExportController : Controller
    {

        private IPresentAbsentService _presentAbsentService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IDetailSelectionService _detailSelectionService;

        public PresentAbsentExportController(IPresentAbsentService presentAbsentService,  IMediaService mediaService, IWebSessionService webSessionService, IDetailSelectionService detailSelectionService)
        {
            _presentAbsentService = presentAbsentService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _detailSelectionService = detailSelectionService;
        }

        // GET: PresentAbsent
        public ActionResult Index(ResultTable.SortOrder sortOrder, int columnIndex)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.TURKEY))
            {
                var nbRows = _presentAbsentService.CountDataRows(idWebSession, this.HttpContext);
                if (nbRows > Core.MAX_ALLOWED_EXCEL_ROWS_NB)
                {
                    return Content(PageHelper.GetContent(idWebSession));
                }
            }
            ResultTable data = _presentAbsentService.GetResultTable(idWebSession, this.HttpContext);
            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document, session, _detailSelectionService.GetDetailSelection(idWebSession));
            export.Export(document, data, session, false, sortOrder, columnIndex);

            string documentFileNameRoot;
            documentFileNameRoot = $"Export_{DateTime.Now:ddMMyyyy}.{(document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx")}";

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
            if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.TURKEY))
            {
                var nbRows = _presentAbsentService.CountDataRows(idWebSession, this.HttpContext);
                if (nbRows > Core.MAX_ALLOWED_EXCEL_ROWS_NB)
                {
                    return Content(PageHelper.GetContent(idWebSession));
                }
            }
            ResultTable data = _presentAbsentService.GetResultTable(idWebSession, this.HttpContext);
            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document, session, _detailSelectionService.GetDetailSelection(idWebSession));
            export.Export(document, data, session, true, sortOrder, columnIndex);

            document.Worksheets.ActiveSheetIndex = 1;

            string documentFileNameRoot;
            documentFileNameRoot = $"Export_{DateTime.Now:ddMMyyyy}.{(document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx")}";

            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";

            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));

            Response.End();

            return View();
        }

        private void export(ResultTable data)
        {
        }

     
    }
}