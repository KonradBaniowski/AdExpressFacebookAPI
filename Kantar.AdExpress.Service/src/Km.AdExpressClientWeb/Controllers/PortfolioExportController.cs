using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Web.Core.Result;
using TNS.FrameWork.WebResultUI;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class PortfolioExportController : Controller
    {
        private IPortfolioService _portofolioService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;


        public PortfolioExportController(IPortfolioService portofolioService, IMediaService mediaService, IWebSessionService webSessionService)
        {
            _portofolioService = portofolioService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;

        }

        // GET: PortfolioExport
        public ActionResult Index()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _portofolioService.GetResultTable(idWebSession);

            export(data);

            return View();
        }

        public ActionResult ResultBrut()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _portofolioService.GetResultTable(idWebSession);

            export(data);

            return View();
        }

        private void export(ResultTable data)
        {
            License licence = new License();
            licence.SetLicense("Aspose.Cells.lic");

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            Worksheet sheet = document.Worksheets.Add("WorkSheet1");


            int rowStart = 1;
            int columnStart = 1;
            //int cellRow = 0;
            //int cellCol = 0;
            bool columnHide = false;


            int coltmp = columnStart;
            foreach (var item in data.HeadersIndexInResultTable)
            {
                HeaderBase header = item.Value;

                if (header is HeaderMediaSchedule)
                    continue;

                sheet.Cells[rowStart, coltmp].Value = header.Label;

                coltmp++;
            }

            rowStart++;

            for (int idxCol = 0, cellCol = columnStart; idxCol < data.ColumnsNumber; idxCol++)
            {
                columnHide = false;

                for (int idxRow = 0, cellRow = rowStart; idxRow < data.LinesNumber; idxRow++, cellRow++)
                {

                    var cell = data[idxRow, idxCol];

                    if (cell is LineStart || cell is LineStop || cell is CellImageLink)
                    {
                        columnHide = true;
                        break;
                    }

                    if (cell is CellPercent)
                    {
                        sheet.Cells[cellRow, cellCol].Value = ((CellPercent)cell).Value / 100;
                    }
                    else if (cell is CellUnit)
                    {
                        double value = ((CellUnit)cell).GetValue();
                        if (value != 0.0)
                            sheet.Cells[cellRow, cellCol].Value = ((CellUnit)cell).GetValue();
                    }
                    else if (cell is CellLabel)
                        sheet.Cells[cellRow, cellCol].Value = ((CellLabel)cell).Label;
                    else
                    {
                        int i = 0;

                    }
                }

                if (columnHide == false)
                    cellCol++;
            }

            string documentFileNameRoot;
            documentFileNameRoot = string.Format("Document.{0}", document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx");

            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";

            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));

            Response.End();
        }
    }
}