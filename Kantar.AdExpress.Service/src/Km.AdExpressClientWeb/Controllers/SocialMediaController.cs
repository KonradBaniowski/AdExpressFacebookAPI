using System;
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
using Domain = Kantar.AdExpress.Service.Core.Domain;
using TNS.Classification.Universe;
using Km.AdExpressClientWeb.Models.Shared;
using Km.AdExpressClientWeb.I18n;

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
            List<SelectListItem> combos = new List<SelectListItem>();
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
            columns.Add(new { headerText = "URL Page", key = "Url", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "Url" });
            columns.Add(new { headerText = "Page", key = "NbPage", dataType = "number", width = "*", format= "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NbPage" });
            columns.Add(new { headerText = "Fan", key = "NumberFan", dataType = "number", width = "*", format= "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberFan" });
            columns.Add(new { headerText = "Post", key = "NumberPost", dataType = "number", width = "*", format= "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberPost" });
            columns.Add(new { headerText = "Like", key = "NumberLike", dataType = "number", width = "*", format= "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberLike" });
            columns.Add(new { headerText = "Comment", key = "NumberComment", dataType = "number", width = "*", format= "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberComment" });
            columns.Add(new { headerText = "Share", key = "NumberShare", dataType = "number", width = "*", format= "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberShare" });
            columns.Add(new { headerText = "Brand exposure", key = "Expenditure", dataType = "number", width = "*", format= "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "Expenditure" });

            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                List<Domain.Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                int IndexListRef = 1;
                if(universeMarket.Count < 2)
                {
                    IndexListRef = 0;
                }

                Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession); //Params : 0 = Concurrents; 1 = Référents
                postModelRef.IdAdvertisers = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                postModelRef.IdBrands = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();


                response = client.PostAsJsonAsync(new Uri(System.Configuration.ConfigurationManager.AppSettings["FacebookPageUri"]), postModelRef).Result;
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

                combos.Add(new SelectListItem { Text = "TOP 3 des posts", Value = "", Selected = true });
                combos.Add(new SelectListItem { Text = par.PageName, Value = par.IdPageFacebook });
                combos.AddRange(data.Select(e => { return new SelectListItem { Text = e.PageName, Value = e.IdPageFacebook }; }).ToList());

                if (universeMarket.Count > 1)
                {
                    Domain.PostModel postModelConc = _webSessionService.GetPostModel(idSession); //Params : 0 = Référents; 1 = Concurrents
                    postModelConc.IdAdvertisers = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                    postModelConc.IdBrands = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                    response = client.PostAsJsonAsync(new Uri(System.Configuration.ConfigurationManager.AppSettings["FacebookPageUri"]), postModelConc).Result;
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
                    datas.AddRange(data.Where(e => e.PID != -1 && e.PID != 2).Select(e => e).ToList());

                    combos.Add(new SelectListItem { Text = par.PageName, Value = par.IdPageFacebook });
                    combos.AddRange(data.Select(e => { return new SelectListItem { Text = e.PageName, Value = e.IdPageFacebook }; }).ToList());
                }

                gridResult.HasData = true;
                gridResult.Columns = columns;
                gridResult.Schema = schemaFields;


                try
                {
                    if (!gridResult.HasData)
                        return null;

                    string jsonData = JsonConvert.SerializeObject(datas);
                    var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, combo = combos, unit = "number" };
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


        
        public JsonResult GetFilterPost()
        {
            List<SelectListItem> combo = new List<SelectListItem>()
            {
                new SelectListItem{
                    Text = "select a post",
                    Value = "",
                    Selected = true
                },
                new SelectListItem
                {
                    Text = "SONY PARENT ENTERN",
                    Value = "1381,1156"
                },
                new SelectListItem
                {
                    Text = "play1",
                    Value = "1381"
                },
                new SelectListItem
                {
                    Text = "play2",
                    Value = "1156"
                }
            };
            var obj = new { combo = combo };
            JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
            jsonModel.MaxJsonLength = Int32.MaxValue;
            return jsonModel;
        }

        public  ActionResult SocialMediaCreative(string ids, string type)
        {
            InsertionCreativeViewModel model = new InsertionCreativeViewModel();
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            int siteLanguage = _webSessionService.GetSiteLanguage(webSessionId);
            model.Labels = LabelsHelper.LoadPageLabels(siteLanguage);
            model.paramsUrl.Add(ids);
            model.paramsUrl.Add(type.ToString());
            return View(model);
        }

        public async Task<JsonResult> GetSocialMediaCreative(string ids, string type)
        {
            var gridResult = new GridResult();

            object[,] gridData = new object[11, 14];
            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();

            #region
            //Hierachical ids for Treegrid
            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });
            columns.Add(new { headerText = "PostId", key = "IdPost", dataType = "string", width = "*", hidden = true });
            schemaFields.Add(new { name = "IdPost" });
            columns.Add(new { headerText = "IdPostFacebook", key = "IdPostFacebook", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "IdPostFacebook" });
            columns.Add(new { headerText = "Advertiser", key = "Advertiser", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "Advertiser" });
            columns.Add(new { headerText = "Brand", key = "Brand", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "Brand" });
            columns.Add(new { headerText = "Page", key = "PageName", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "PageName" });
            columns.Add(new { headerText = "Date", key = "DateCreationPost", dataType = "string", width = "*" });
            schemaFields.Add(new { name = "DateCreationPost" });
            columns.Add(new { headerText = "Commitment", key = "Commitment", dataType = "number", width = "*" });
            schemaFields.Add(new { name = "Commitment" });
            columns.Add(new { headerText = "Like", key = "NumberLike", dataType = "number", width = "*" });
            schemaFields.Add(new { name = "NumberLike" });
            columns.Add(new { headerText = "Share", key = "NumberShare", dataType = "number", width = "*" });
            schemaFields.Add(new { name = "NumberShare" });
            columns.Add(new { headerText = "Comment", key = "NumberComment", dataType = "number", width = "*" });
            schemaFields.Add(new { name = "NumberComment" });
            //End
            #endregion
            #region Mock Data

            //Mock data
            //gridData[0, 0] = 1;
            //gridData[0, 1] = -1;
            //gridData[0, 2] = 1;
            //gridData[0, 3] = 0;
            //gridData[0, 4] = "bmw";
            //gridData[0, 5] = "bmw gamme auto";
            //gridData[0, 6] = "bmw france";
            //gridData[0, 7] = "06/01/2016";
            //gridData[0, 8] = "3596";
            //gridData[0, 9] = "2480";
            //gridData[0, 10] = "484";
            //gridData[0, 11] = "37";

            //gridData[1, 0] = 2;
            //gridData[1, 1] = -1;
            //gridData[1, 2] = 2;
            //gridData[1, 3] = 0;
            //gridData[1, 4] = "Fiat";
            //gridData[1, 5] = "Maseratti";
            //gridData[1, 6] = "Fiat auto";
            //gridData[1, 7] = "06/05/2016";
            //gridData[1, 8] = "13596";
            //gridData[1, 9] = "20480";
            //gridData[1, 10] = "1484";
            //gridData[1, 11] = "3700";

            //gridData[2, 0] = 3;
            //gridData[2, 1] = -1;
            //gridData[2, 2] = 3;
            //gridData[2, 3] = 0;
            //gridData[2, 4] = "Ferrero";
            //gridData[2, 5] = "Nutella";
            //gridData[2, 6] = "Ferrero";
            //gridData[2, 7] = "06/06/2016";
            //gridData[2, 8] = "13596";
            //gridData[2, 9] = "20480";
            //gridData[2, 10] = "1484";
            //gridData[2, 11] = "3700";

            //gridResult.HasData = true;
            //gridResult.Columns = columns;
            //gridResult.Schema = schemaFields;
            //gridResult.ColumnsFixed = columnsFixed;
            //gridResult.NeedFixedColumns = false;
            //gridResult.Data = gridData;
            //end Mock Data
            #endregion

            using (var client = new HttpClient())
            {


                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                List<Domain.Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession); //Params : 0 = Référents; 1 = Concurrents
                postModelRef.IdPages = ids.Split(',').Select(long.Parse).ToList();

                HttpResponseMessage response = client.PostAsJsonAsync(new Uri(System.Configuration.ConfigurationManager.AppSettings["FacebookPostUri"]), postModelRef).Result;
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());

                List<DataPostFacebook> data = JsonConvert.DeserializeObject<List<DataPostFacebook>>(content);
                string jsonData = JsonConvert.SerializeObject(data);
                var obj = new { datagrid = jsonData, columns = columns, schema = schemaFields, columnsfixed = columnsFixed, needfixedcolumns = false, isonecolumnline = gridResult.isOneColumnLine };
                JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }
        }

        public async Task<JsonResult> GetPostbyIdPage(List<long> ids)
        {
            if (ids == null) { throw new ArgumentNullException("Parameters are null"); }
            if (ids.Count == 0) { throw new ArgumentException("Parameters must be defined"); }

            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession);
                postModelRef.IdPages = ids;

                HttpResponseMessage response = client.PostAsJsonAsync(new Uri("http://localhost:9990/api/TopPosts"), postModelRef).Result;
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());

                var data = JsonConvert.DeserializeObject<List<PostFacebook>>(content);
                string jsonData = JsonConvert.SerializeObject(data);
                var obj = new {data = data };
                JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }
        }

    }
}