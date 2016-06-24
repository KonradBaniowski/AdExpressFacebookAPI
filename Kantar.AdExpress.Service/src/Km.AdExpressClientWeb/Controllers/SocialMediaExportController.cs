using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;

namespace Km.AdExpressClientWeb.Controllers
{
    public class SocialMediaExportController : Controller
    {
        private IWebSessionService _webSessionService;
        private IDetailSelectionService _detailSelectionService;
        public SocialMediaExportController(IWebSessionService webSessionService, IDetailSelectionService detailSelectionService)
        {
            _webSessionService = webSessionService;
            _detailSelectionService = detailSelectionService;
        }


        // GET: SocialMediaExport
        public ActionResult Index()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            //GridResultExport data = serviceRomain();


            /************************** MOCK ***********************************/
            var gridResultExport = new GridResultExport();

            List<InfragisticData> gridData = new List<InfragisticData>();
            List<InfragisticColumn> columns = new List<InfragisticColumn>();
            List<string> vals = new List<string>();

            columns.Add(new InfragisticColumn { HeaderText = "", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Page", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Fan", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Post", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Like", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Share", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Comment", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Brand exposure", Hidden = false });


            vals.Add("Referents");
            vals.Add("2");
            vals.Add("100");
            vals.Add("100");
            vals.Add("100");
            vals.Add("300");
            vals.Add("20");
            vals.Add("4500");
            gridData.Add(new InfragisticData { Level = 1 , Values = vals });

                vals = new List<string>();
                vals.Add("BMW");
                vals.Add("2");
                vals.Add("100");
                vals.Add("100");
                vals.Add("100");
                vals.Add("300");
                vals.Add("20");
                vals.Add("4500");
                gridData.Add(new InfragisticData { Level = 2, Values = vals });

                    vals = new List<string>();
                    vals.Add("WoooowWw");
                    vals.Add("1");
                    vals.Add("50");
                    vals.Add("50");
                    vals.Add("50");
                    vals.Add("150");
                    vals.Add("10");
                    vals.Add("");
                    gridData.Add(new InfragisticData { Level = 3, Values = vals });

                    vals = new List<string>();
                    vals.Add("WoooowWw");
                    vals.Add("1");
                    vals.Add("50");
                    vals.Add("50");
                    vals.Add("50");
                    vals.Add("150");
                    vals.Add("10");
                    vals.Add("");
                    gridData.Add(new InfragisticData { Level = 3, Values = vals });

            vals = new List<string>();
            vals.Add("Concurrents");
            vals.Add("2");
            vals.Add("100");
            vals.Add("100");
            vals.Add("100");
            vals.Add("300");
            vals.Add("20");
            vals.Add("4500");
            gridData.Add(new InfragisticData { Level = 1, Values = vals });

                vals = new List<string>();
                vals.Add("AUDI");
                vals.Add("2");
                vals.Add("100");
                vals.Add("100");
                vals.Add("100");
                vals.Add("300");
                vals.Add("20");
                vals.Add("4500");
                gridData.Add(new InfragisticData { Level = 2, Values = vals });

                    vals = new List<string>();
                    vals.Add("YeeaaHHhh");
                    vals.Add("1");
                    vals.Add("50");
                    vals.Add("50");
                    vals.Add("50");
                    vals.Add("150");
                    vals.Add("10");
                    vals.Add("");
                    gridData.Add(new InfragisticData { Level = 3, Values = vals });

                    vals = new List<string>();
                    vals.Add("YeeaaHHhh");
                    vals.Add("1");
                    vals.Add("50");
                    vals.Add("50");
                    vals.Add("50");
                    vals.Add("150");
                    vals.Add("10");
                    vals.Add("");
                    gridData.Add(new InfragisticData { Level = 3, Values = vals });


            gridResultExport.HasData = true;
            gridResultExport.Columns = columns;
            gridResultExport.Data = gridData;

            /************************ FIN MOCK ***********************************/



            WebSession session = (WebSession)WebSession.Load(idWebSession);

            ExportAspose export = new ExportAspose();

            Workbook document = new Workbook(FileFormatType.Excel2003XML);

            document.Worksheets.Clear();

            export.ExportSelection(document, session, _detailSelectionService.GetDetailSelection(idWebSession));
            export.ExportFromGridResult(document, gridResultExport, session);

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