﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.Models;
using TNS.AdExpress.Domain.Results;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Km.AdExpressClientWeb.Models.SocialMedia;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.Classification.Universe;
namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class SocialMediaController : Controller
    {
        private IWebSessionService _webSessionService;
        private IDetailSelectionService _detailSelectionService;
        private int _siteLanguage = 33;

        public SocialMediaController(IWebSessionService webSessionService, IDetailSelectionService detailSelectionService)
        {
            _webSessionService = webSessionService;
            _detailSelectionService = detailSelectionService;
        }

        //GET: SocialMedia
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Results()
        {

            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            if (!_webSessionService.IsAllSelectionStep(idSession))
            {
                return RedirectToAction("Market", "Selection");
            }

            var result = _webSessionService.GetWebSession(idSession);
            _siteLanguage = result.WebSession.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            ViewBag.SiteLanguage = _siteLanguage;
            var resultNode = new NavigationNode { Position = 4 };
            var navigationHelper = new Helpers.PageHelper();
            var model = new Models.SocialMedia.ResultsViewModel
            {
                NavigationBar = navigationHelper.LoadNavBar(idSession, result.ControllerDetails.Name, _siteLanguage, 4),
                Presentation = navigationHelper.LoadPresentationBar(result.WebSession.SiteLanguage, result.ControllerDetails),
                Labels = navigationHelper.LoadPageLabels(_siteLanguage, result.ControllerDetails.Name)
            };

            return View(model);

        }


        public async Task<ActionResult> SocialMediaResult()
        {
            var gridResult = new GridResult();

            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<DataFacebook> data = new List<DataFacebook>();
            List<DataFacebook> datas = new List<DataFacebook>();
            HttpResponseMessage response = new HttpResponseMessage();
            DataFacebook par = new DataFacebook();
            string content = string.Empty;

            //Hierachical ids for Treegrid
            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });
            columns.Add(new { headerText = "IdPage", key = "IdPage", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "IdPage" });

            columns.Add(new { headerText = "", key = "PageName", dataType = "string", width = "350" });
            schemaFields.Add(new { name = "PageName" });
            columns.Add(new { headerText = "Lien vers les Post", key = "IdPageFacebook", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "IdPageFacebook" });
            columns.Add(new { headerText = "URL Page", key = "20", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "20" });
            columns.Add(new { headerText = "Page", key = "NbPage", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "NbPage" });
            columns.Add(new { headerText = "Fan", key = "NumberFan", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "NumberFan" });
            columns.Add(new { headerText = "Post", key = "NumberPost", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "NumberPost" });
            columns.Add(new { headerText = "Like", key = "NumberLike", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "NumberLike" });
            columns.Add(new { headerText = "Comment", key = "NumberComment", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "NumberComment" });
            columns.Add(new { headerText = "Share", key = "NumberShare", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "NumberShare" });
            columns.Add(new { headerText = "Brand exposure", key = "Expenditure", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "Expenditure" });

            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                List<Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                PostModel postModelRef = _webSessionService.GetPostModel(idSession); //Params : 0 = Référents; 1 = Concurrents
                postModelRef.IdAdvertisers = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                postModelRef.IdBrands = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();


                response = client.PostAsJsonAsync(new Uri("http://localhost:9990/api/FacebookPage"), postModelRef).Result;
                content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());

                data = JsonConvert.DeserializeObject<List<DataFacebook>>(content);
                par = new DataFacebook()
                {
                    PID = -1,
                    ID = 1,
                    Expenditure = data.Where(e => e.PID == -1).Sum(a => a.Expenditure),
                    IdPageFacebook = string.Join(",", data.Select(e => e.IdPageFacebook).ToList()),
                    NbPage = data.Where(e => e.PID == -1).Sum(a => a.NbPage),
                    NumberComment = data.Where(e => e.PID == -1).Sum(a => a.NumberComment),
                    NumberFan = data.Where(e => e.PID == -1).Sum(a => a.NumberFan),
                    NumberLike = data.Where(e => e.PID == -1).Sum(a => a.NumberLike),
                    NumberPost = data.Where(e => e.PID == -1).Sum(a => a.NumberPost),
                    NumberShare = data.Where(e => e.PID == -1).Sum(a => a.NumberShare),
                    PageName = "Référents"
                };
                datas.Add(par);

                datas.AddRange(data.Where(e => e.PID == -1).Select(e => { e.PID = 1; return e; }).ToList());
                datas.AddRange(data.Where(e => e.PID != -1 && e.PID != 1).Select(e => e).ToList());


                if (universeMarket.Count > 1)
                {

                    PostModel postModelConc = _webSessionService.GetPostModel(idSession); //Params : 0 = Référents; 1 = Concurrents
                    postModelConc.IdAdvertisers = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                    postModelConc.IdBrands = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                    response = client.PostAsJsonAsync(new Uri("http://localhost:9990/api/FacebookPage"), postModelConc).Result;
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                        throw new Exception(response.StatusCode.ToString());

                    data = JsonConvert.DeserializeObject<List<DataFacebook>>(content);
                    par = new DataFacebook()
                    {
                        PID = -1,
                        ID = 2,
                        Expenditure = data.Where(e => e.PID == -1).Sum(a => a.Expenditure),
                        IdPageFacebook = string.Join(",", data.Select(e => e.IdPageFacebook).ToList()),
                        NbPage = data.Where(e => e.PID == -1).Sum(a => a.NbPage),
                        NumberComment = data.Where(e => e.PID == -1).Sum(a => a.NumberComment),
                        NumberFan = data.Where(e => e.PID == -1).Sum(a => a.NumberFan),
                        NumberLike = data.Where(e => e.PID == -1).Sum(a => a.NumberLike),
                        NumberPost = data.Where(e => e.PID == -1).Sum(a => a.NumberPost),
                        NumberShare = data.Where(e => e.PID == -1).Sum(a => a.NumberShare),
                        PageName = "Concurrents"
                    };
                    datas.Add(par);
                    datas.AddRange(data.Where(e => e.PID == -1).Select(e => { e.PID = 2; return e; }).ToList());
                    datas.AddRange(data.Where(e => e.PID != -1).Select(e => e).ToList());
                }

                gridResult.HasData = true;
                gridResult.Columns = columns;
                gridResult.Schema = schemaFields;


                try
                {
                    if (!gridResult.HasData)
                        return null;

                    string jsonData = JsonConvert.SerializeObject(datas);
                    var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, needfixedcolumns = gridResult.NeedFixedColumns, isonecolumnline = gridResult.isOneColumnLine };
                    JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
                    jsonModel.MaxJsonLength = Int32.MaxValue;

                    return jsonModel;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error Social Media Result", ex);
                }
            }

        }

        public ActionResult SocialMediaCreative(int id, int type)
        {
            return View();

        }

        public JsonResult GetSocialMediaCreative(int id, int type)
        {
            var gridResult = new GridResult();

            object[,] gridData = new object[11, 14];
            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();
            #region Mock Data
            //Hierachical ids for Treegrid
            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });
            columns.Add(new { headerText = "PostId", key = "5", dataType = "string", width = "*", hidden = true });
            schemaFields.Add(new { name = "5" });
            columns.Add(new { headerText = "Post", key = "10", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "10" });
            columns.Add(new { headerText = "AdvertiserLabel", key = "20", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "20" });
            columns.Add(new { headerText = "BrandLabel", key = "30", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "30" });
            columns.Add(new { headerText = "PageLabel", key = "40", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "40" });
            columns.Add(new { headerText = "DateLabel", key = "50", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "50" });
            columns.Add(new { headerText = "Commitment", key = "60", dataType = "number", width = "*" });
            schemaFields.Add(new { name = "60" });
            columns.Add(new { headerText = "Like", key = "70", dataType = "number", width = "*" });
            schemaFields.Add(new { name = "70" });
            columns.Add(new { headerText = "Share", key = "80", dataType = "number", width = "*" });
            schemaFields.Add(new { name = "80" });
            columns.Add(new { headerText = "Comment", key = "90", dataType = "number", width = "*" });
            schemaFields.Add(new { name = "90" });
            columns.Add(new { headerText = "LevelType", key = "LevelType", dataType = "string", width = "*", hidden = true });
            schemaFields.Add(new { name = "LevelType" });
            //End
            //Mock data
            gridData[0, 0] = 1;
            gridData[0, 1] = -1;
            gridData[0, 2] = 1;
            gridData[0, 3] = 0;
            gridData[0, 4] = "bmw";
            gridData[0, 5] = "bmw gamme auto";
            gridData[0, 6] = "bmw france";
            gridData[0, 7] = "06/01/2016";
            gridData[0, 8] = "3596";
            gridData[0, 9] = "2480";
            gridData[0, 10] = "484";
            gridData[0, 11] = "37";

            gridData[1, 0] = 2;
            gridData[1, 1] = -1;
            gridData[1, 2] = 2;
            gridData[1, 3] = 0;
            gridData[1, 4] = "Fiat";
            gridData[1, 5] = "Maseratti";
            gridData[1, 6] = "Fiat auto";
            gridData[1, 7] = "06/05/2016";
            gridData[1, 8] = "13596";
            gridData[1, 9] = "20480";
            gridData[1, 10] = "1484";
            gridData[1, 11] = "3700";

            gridData[2, 0] = 3;
            gridData[2, 1] = -1;
            gridData[2, 2] = 3;
            gridData[2, 3] = 0;
            gridData[2, 4] = "Ferrero";
            gridData[2, 5] = "Nutella";
            gridData[2, 6] = "Ferrero";
            gridData[2, 7] = "06/06/2016";
            gridData[2, 8] = "13596";
            gridData[2, 9] = "20480";
            gridData[2, 10] = "1484";
            gridData[2, 11] = "3700";

            gridResult.HasData = true;
            gridResult.Columns = columns;
            gridResult.Schema = schemaFields;
            gridResult.ColumnsFixed = columnsFixed;
            gridResult.NeedFixedColumns = false;
            gridResult.Data = gridData;
            //end Mock Data
            #endregion
            if (!gridResult.HasData)
                return null;

            string jsonData = JsonConvert.SerializeObject(gridResult.Data);
            var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, needfixedcolumns = gridResult.NeedFixedColumns, isonecolumnline = gridResult.isOneColumnLine };
            JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
            jsonModel.MaxJsonLength = Int32.MaxValue;

            return jsonModel;
        }

    }
}