﻿using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Web.Core.Sessions;

namespace Km.AdExpressClientWeb.Controllers
{
    public class PortfolioDetailMediaExportController : Controller
    {
        private IPortfolioDetailMediaService _portfolioDetailMediaService;
        private IDetailSelectionService _detailSelectionService;

        public PortfolioDetailMediaExportController(IPortfolioDetailMediaService portfolioDetailMediaService, IDetailSelectionService detailSelectionService)
        {
            _portfolioDetailMediaService = portfolioDetailMediaService;
            _detailSelectionService = detailSelectionService;
        }

        // GET: PortfolioDetailMediaExport 
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult PortfolioDetailMediaResult(string idMedia, string dayOfWeek, string ecran)
        {

            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var data = _portfolioDetailMediaService.GetDetailMediaResult(idWebSession, idMedia, dayOfWeek, ecran);

            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document, session, _detailSelectionService.GetDetailSelection(idWebSession));
            export.Export(document, data, session);

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
