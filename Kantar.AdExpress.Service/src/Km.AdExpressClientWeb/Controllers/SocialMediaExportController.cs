using Aspose.Cells;
using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Models.SocialMedia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;
using Domain = Kantar.AdExpress.Service.Core.Domain;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
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
        public async Task<ActionResult> Index()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var gridResultExport = new GridResultExport();

            List<InfragisticData> gridData = new List<InfragisticData>();
            List<InfragisticData> tmpList = new List<InfragisticData>();
            List<DataFacebook> data = new List<DataFacebook>();
            List<InfragisticColumn> columns = new List<InfragisticColumn>();
            List<string> vals = new List<string>();
            InfragisticData par = new InfragisticData();
            HttpResponseMessage response = new HttpResponseMessage();
            string content = string.Empty;

            columns.Add(new InfragisticColumn { HeaderText = "", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Page", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Fan", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Post", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Like", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Share", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Comment", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Brand exposure", Hidden = false });

            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                List<Domain.Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                //int IndexListRef = 1;
                //if (universeMarket.Count < 2)
                //{
                //    IndexListRef = 0;
                //}

                //Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession); //Params : 0 = Concurrents; 1 = Référents
                //postModelRef.IdAdvertisers = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                //postModelRef.IdBrands = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession); //Params : 0 = Référents; 1 = Concurrents
                postModelRef.IdAdvertisers = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                postModelRef.IdBrands = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                response = client.PostAsJsonAsync(new Uri(System.Configuration.ConfigurationManager.AppSettings["FacebookPageUri"]), postModelRef).Result;
                content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());

                data = JsonConvert.DeserializeObject<List<DataFacebook>>(content);
                vals.Add("Référents");
                vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NbPage).ToString());
                vals.Add("");//vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NumberFan).ToString());
                vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NumberPost).ToString());
                vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NumberLike).ToString());
                vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NumberShare).ToString());
                vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NumberComment).ToString());
                vals.Add(data.Where(e => e.PID == -1).Sum(a => a.Expenditure).ToString());
                par = new InfragisticData()
                {
                    Level = 1,
                    Values = vals
                };
                gridData.Add(par);
                vals = new List<string>();

                data.Where(e => e.PID == -1).ToList().ForEach(p =>
                {
                    gridData.Add(
                           new InfragisticData
                           {
                               Level = 2,
                               Values = new List<String> {
                                    p.PageName,
                                    p.NbPage.ToString(),
                                    p.NumberFan.ToString(),
                                    p.NumberPost.ToString(),
                                    p.NumberLike.ToString(),
                                    p.NumberShare.ToString(),
                                    p.NumberComment.ToString(),
                                    p.Expenditure.ToString()
                               }
                           });

                    data.Where(c => c.PID == p.ID).ToList().ForEach(c =>
                    {
                        gridData.Add(
                        new InfragisticData
                        {
                            Level = 3,
                            Values = new List<String> {
                                c.PageName,
                                c.NbPage.ToString(),
                                c.NumberFan.ToString(),
                                c.NumberPost.ToString(),
                                c.NumberLike.ToString(),
                                c.NumberShare.ToString(),
                                c.NumberComment.ToString(),
                                c.Expenditure.ToString()
                            }
                        });
                    });
                });

               
                if (universeMarket.Count > 1)
                {
                    Domain.PostModel postModelConc = _webSessionService.GetPostModel(idSession); //Params : 0 = Référents; 1 = Concurrents
                    postModelConc.IdAdvertisers = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                    postModelConc.IdBrands = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                    response = client.PostAsJsonAsync(new Uri(System.Configuration.ConfigurationManager.AppSettings["FacebookPageUri"]), postModelConc).Result;
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                        throw new Exception(response.StatusCode.ToString());

                    data = JsonConvert.DeserializeObject<List<DataFacebook>>(content);
                    vals.Add("Concurrents");
                    vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NbPage).ToString());
                    vals.Add(""); //vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NumberFan).ToString());
                    vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NumberPost).ToString());
                    vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NumberLike).ToString());
                    vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NumberShare).ToString());
                    vals.Add(data.Where(e => e.PID == -1).Sum(a => a.NumberComment).ToString());
                    vals.Add(data.Where(e => e.PID == -1).Sum(a => a.Expenditure).ToString());
                    par = new InfragisticData()
                    {
                        Level = 1,
                        Values = vals
                    };
                    gridData.Add(par);
                    vals = new List<string>();
                    tmpList = new List<InfragisticData>();

                    data.Where(e => e.PID == -1).ToList().ForEach(p =>
                   {
                       gridData.Add(
                              new InfragisticData
                              {
                                  Level = 2,
                                  Values = new List<String> {
                                    p.PageName,
                                    p.NbPage.ToString(),
                                    p.NumberFan.ToString(),
                                    p.NumberPost.ToString(),
                                    p.NumberLike.ToString(),
                                    p.NumberShare.ToString(),
                                    p.NumberComment.ToString(),
                                    p.Expenditure.ToString()
                                  }
                              });

                       data.Where(c => c.PID == p.ID).ToList().ForEach(c =>
                       {
                           gridData.Add(
                           new InfragisticData
                           {
                               Level = 3,
                               Values = new List<String> {
                                c.PageName,
                                c.NbPage.ToString(),
                                c.NumberFan.ToString(),
                                c.NumberPost.ToString(),
                                c.NumberLike.ToString(),
                                c.NumberShare.ToString(),
                                c.NumberComment.ToString(),
                                c.Expenditure.ToString()
                               }
                           });
                       });
                   });

                }

            }

            gridResultExport.HasData = true;
            gridResultExport.Columns = columns;
            gridResultExport.Data = gridData;

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

        public async Task<ActionResult> CreativeExport(string ids, string period)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var gridResultExport = new GridResultExport();
            List<InfragisticData> gridData = new List<InfragisticData>();
            List<InfragisticData> tmpList = new List<InfragisticData>();
            List<DataPostFacebook> data = new List<DataPostFacebook>();
            List<InfragisticColumn> columns = new List<InfragisticColumn>();
            List<string> vals = new List<string>();
            InfragisticData par = new InfragisticData();
            HttpResponseMessage response = new HttpResponseMessage();
            ExportAspose export = new ExportAspose();
            Workbook document = new Workbook(FileFormatType.Excel2003XML);
            document.Worksheets.Clear();

            columns.Add(new InfragisticColumn { HeaderText = "Advertiser", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Brand", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Page", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Date", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Engagement", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Like", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Share", Hidden = false });
            columns.Add(new InfragisticColumn { HeaderText = "Comment", Hidden = false });

            if (!string.IsNullOrEmpty(ids) && !string.IsNullOrEmpty(period))
            {
                using (var client = new HttpClient())
                {
                    var cla = new ClaimsPrincipal(User.Identity);
                    string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                    List<Domain.Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                    Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession, period); //Params : 0 = Référents; 1 = Concurrents

                    postModelRef.IdPages = ids.Split(',').Select(long.Parse).ToList();
                    if (!postModelRef.IdPages.Any())
                    {
                        postModelRef.IdAdvertisers = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                        postModelRef.IdBrands = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();
                    }

                    response = client.PostAsJsonAsync(new Uri(System.Configuration.ConfigurationManager.AppSettings["FacebookPostUri"]), postModelRef).Result;
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                        throw new Exception(response.StatusCode.ToString());
                    data = JsonConvert.DeserializeObject<List<DataPostFacebook>>(content);
                    foreach (var item in data)
                    {
                        InfragisticData ig = new InfragisticData();
                        ig.Level = 2;
                        ig.Values = new List<String> {
                                                    item.Advertiser.ToString(),
                                                    item.Brand.ToString(),
                                                    item.PageName.ToString(),
                                                    item.DateCreationPost.ToString(),
                                                    item.Engagement.ToString(),
                                                    item.NumberLike.ToString(),
                                                    item.NumberShare.ToString(),
                                                    item.NumberComment.ToString()
                                                 };
                        gridData.Add(ig);
                    }
                }
                gridResultExport.HasData = true;
                gridResultExport.Columns = columns;
                gridResultExport.Data = gridData;
                WebSession session = (WebSession)WebSession.Load(idWebSession);
                export.ExportSelection(document, session, _detailSelectionService.GetDetailSelection(idWebSession));
                export.ExportFromGridResult(document, gridResultExport, session);
                document.Worksheets.ActiveSheetIndex = 1;
            }
            string documentFileNameRoot;
            documentFileNameRoot = string.Format("SocialMediaCreativeExport_{0}.{1}", DateTime.Now.ToString("ddMMyyyy"), document.FileFormat == FileFormatType.Excel97To2003 ? "xls" : "xlsx");
            Response.Clear();
            Response.AppendHeader("content-disposition", "attachment; filename=" + documentFileNameRoot);
            Response.ContentType = "application/octet-stream";
            document.Save(Response.OutputStream, new XlsSaveOptions(SaveFormat.Xlsx));
            Response.End();
            return View();

        }
    }



}