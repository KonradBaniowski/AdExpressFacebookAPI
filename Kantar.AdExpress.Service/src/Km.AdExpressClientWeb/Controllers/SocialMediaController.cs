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
using TNS.AdExpress.Domain.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class SocialMediaController : Controller
    {
        private IWebSessionService _webSessionService;
        private IDetailSelectionService _detailSelectionService;
        private int _siteLanguage = WebApplicationParameters.DefaultLanguage;

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
            columns.Add(new { headerText = "", key = "checkBox", dataType = "string", width = "30" });
            schemaFields.Add(new { name = "checkBox" });
            columns.Add(new { headerText = "Posts", key = "IdPageFacebook", dataType = "string", width = "84" });
            schemaFields.Add(new { name = "IdPageFacebook" });
            columns.Add(new { headerText = "URL Page", key = "Url", dataType = "string", width = "80" });
            schemaFields.Add(new { name = "Url" });
            columns.Add(new { headerText = "Page", key = "NbPage", dataType = "number", width = "80", format = "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NbPage" });
            columns.Add(new { headerText = "Fan", key = "NumberFan", dataType = "number", width = "84", format = "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberFan" });
            columns.Add(new { headerText = "Post", key = "NumberPost", dataType = "number", width = "84", format = "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberPost" });
            columns.Add(new { headerText = "Like", key = "NumberLike", dataType = "number", width = "84", format = "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberLike" });
            columns.Add(new { headerText = "Comment", key = "NumberComment", dataType = "number", width = "84", format = "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberComment" });
            columns.Add(new { headerText = "Share", key = "NumberShare", dataType = "number", width = "84", format = "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberShare" });
            columns.Add(new { headerText = "Brand exposure", key = "Expenditure", dataType = "number", width = "92", format = "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "Expenditure" });

            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                List<Domain.Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                int IndexListRef = 1;
                if (universeMarket.Count < 2)
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
                    IdPageFacebook = string.Join(",", data.Where(e => e.PID == -1).Select(e => e.IdPageFacebook).ToList()),
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
                        IdPageFacebook = string.Join(",", data.Where(e => e.PID == -1).Select(e => e.IdPageFacebook).ToList()),
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

        public ActionResult SocialMediaCreative(string ids, string type)
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

        public async Task<JsonResult> GetSocialMediaCreative(string ids, string period)
        {
            var gridResult = new GridResult();

            object[,] gridData = new object[11, 14];
            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();

            #region Hierachical ids for Treegrid
            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });
            columns.Add(new { headerText = "PostId", key = "IdPost", dataType = "string", width = "*", hidden = true });
            schemaFields.Add(new { name = "IdPost" });
            columns.Add(new { headerText = "Post", key = "IdPostFacebook", dataType = "string", width = "150px" });
            schemaFields.Add(new { name = "IdPostFacebook" });
            columns.Add(new { headerText = "Advertiser", key = "Advertiser", dataType = "string", width = "150px" });
            schemaFields.Add(new { name = "Advertiser" });
            columns.Add(new { headerText = "Brand", key = "Brand", dataType = "string", width = "150px" });
            schemaFields.Add(new { name = "Brand" });
            columns.Add(new { headerText = "Page", key = "PageName", dataType = "string", width = "150px" });
            schemaFields.Add(new { name = "PageName" });
            columns.Add(new { headerText = "Date", key = "DateCreationPost", dataType = "string", width = "150px" });
            schemaFields.Add(new { name = "DateCreationPost" });
            columns.Add(new { headerText = "Commitment", key = "Commitment", dataType = "number", width = "100px", format="number",columnCssClass="numericAlignment" });
            schemaFields.Add(new { name = "Commitment" });
            columns.Add(new { headerText = "Like", key = "NumberLike", dataType = "number", width = "75px", format = "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberLike" });
            columns.Add(new { headerText = "Share", key = "NumberShare", dataType = "number", width = "75px", format = "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberShare" });
            columns.Add(new { headerText = "Comment", key = "NumberComment", dataType = "number", width = "75px", format = "number", columnCssClass = "numericAlignment" });
            schemaFields.Add(new { name = "NumberComment" });
            #endregion

            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                List<Domain.Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession,period); //Params : 0 = Référents; 1 = Concurrents

                postModelRef.IdPages = ids.Split(',').Select(long.Parse).ToList();
                if (!postModelRef.IdPages.Any())
                {
                    postModelRef.IdAdvertisers = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                    postModelRef.IdBrands = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();
                }

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

        public async Task<ActionResult> GetPostbyIdPage(List<long> ids)
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
                foreach (var item in data)
                {
                    item.LikesChart = new Dictionary<string, string>();
                    item.LikesChart.Add("Evolution", "Nombre");

                    var likes = item.NumberLikes.Split(',');
                    var nbItems = likes.Count();
                    var comments = item.NumberComments.Split(',');
                    var shares = item.NumberShares.Split(',');
                    for (int i = nbItems - 1; i >= 0; i--)
                    {
                        item.LikesChart.Add(i.ToString(), likes[i]);
                    }

                }
                //string jsonData = JsonConvert.SerializeObject(data);
                //var obj = new {data = data };
                //JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
                //jsonModel.MaxJsonLength = Int32.MaxValue;

                return PartialView("GetTopPost", data);
            }
        }

        public async Task<ActionResult> GetRefConcChart()
        {
            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                List<Domain.Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                int IndexListRef = 1;
                if (universeMarket.Count < 2)
                {
                    IndexListRef = 0;
                }

                Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession); //Params : 0 = Concurrents; 1 = Référents
                postModelRef.IdAdvertisers = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                postModelRef.IdBrands = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                HttpResponseMessage response = client.PostAsJsonAsync(new Uri("http://localhost:9990/api/KPI"), postModelRef).Result;
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());

                List<KPIPageFacebookContract> data = JsonConvert.DeserializeObject<List<KPIPageFacebookContract>>(content);
                if (data.Count == 0)
                {
                    data.Add(new KPIPageFacebookContract());
                }

                //var mounth = string.Join(",", data.Select(e => e.Month).ToList());
                //var likes = string.Join(",", data.Select(e => e.Like).ToList());
                //var comments = string.Join(",", data.Select(e => e.Comment).ToList());
                //var shares = string.Join(",", data.Select(e => e.Share).ToList());

                //PostFacebook result = new PostFacebook { ListMonths = mounth, NumberLikes = likes, NumberComments = comments, NumberShares = shares };

                return PartialView("GetRefConcChart", data);
            }
        }

        
        public JsonResult GetDataChart(List<long> likes)
        {
            var obj = new { data = "" };
            JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
            jsonModel.MaxJsonLength = Int32.MaxValue;
            return jsonModel;
        }

        public async Task<ActionResult> GetKPIByPostId(long id)
        {
            if (id == 0) { throw new ArgumentNullException("Invalid parameter"); }
            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession);
                long idPost = id;
                int idLanguage = postModelRef.IdLanguage;
                HttpResponseMessage response = client.PostAsJsonAsync(new Uri(System.Configuration.ConfigurationManager.AppSettings["FacebookOnePostUri"]), new { idPost, idLanguage }).Result;
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());

                var data = JsonConvert.DeserializeObject<PostFacebook>(content);
                data.LikesChart = new Dictionary<string, string>();
                data.LikesChart.Add("Evolution", "Nombre");

                var likes = data.NumberLikes.Split(',');
                var nbItems = likes.Count();
                var comments = data.NumberComments.Split(',');
                var shares = data.NumberShares.Split(',');
                for (int i = nbItems - 1; i >= 0; i--)
                {
                    data.LikesChart.Add(i.ToString(), likes[i]);
                }
                var model = new PostFacebookVM(data);
                if (idLanguage > 0)
                    model.Labels = LabelsHelper.LoadPageLabels(idLanguage);
                return PartialView("Zoom", model);
            }
        }
    }
}