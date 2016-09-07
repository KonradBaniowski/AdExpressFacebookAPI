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
using Domain = Kantar.AdExpress.Service.Core.Domain;
using TNS.Classification.Universe;
using Km.AdExpressClientWeb.Models.Shared;
using Km.AdExpressClientWeb.I18n;
using TNS.AdExpress.Domain.Web;
using System.IO;
using System.Net;
using System.Configuration;
using WebConstantes = TNS.AdExpress.Constantes.Web;

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
            var model = new Models.Shared.ResultsViewModel
            {
                NavigationBar = navigationHelper.LoadNavBar(idSession, result.ControllerDetails.Name, _siteLanguage, 4),
                Presentation = navigationHelper.LoadPresentationBar(result.WebSession.SiteLanguage, result.ControllerDetails),
                Labels = navigationHelper.LoadPageLabels(_siteLanguage, result.ControllerDetails.Name),
                IsAlertVisible = PageHelper.IsAlertVisible(WebApplicationParameters.CountryCode, idSession),
                ExportTypeViewModels = PageHelper.GetExportTypes(WebApplicationParameters.CountryCode, WebConstantes.Module.Name.FACEBOOK, _siteLanguage)
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
            List<DataFacebook> datasTmp = new List<DataFacebook>();
            HttpResponseMessage response = new HttpResponseMessage();
            DataFacebook par = new DataFacebook();
            List<object> combos = new List<object>();
            List<object> tmpList = new List<object>();
            string content = string.Empty;

            //Hierachical ids for Treegrid
            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });
            columns.Add(new { headerText = "IdPage", key = "IdPage", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "IdPage" });
            
            columns.Add(new { headerText = "", key = "PageName", dataType = "string", width = "380" });
            schemaFields.Add(new { name = "PageName" });
            //columns.Add(new { headerText = "", key = "checkBox", dataType = "string", width = "30" });
            //schemaFields.Add(new { name = "checkBox" });
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

                bool concurSelected = false;
                //int IndexListRef = 1;
                //if (universeMarket.Count < 2)
                //{
                //    concurSelected = false;
                //    IndexListRef = 0;
                //}

                //Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession); //Params : 0 = Concurrents; 1 = Référents
                //postModelRef.IdAdvertisers = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                //postModelRef.IdBrands = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession); //Params : 1 = Concurrents; 0 = Référents
                postModelRef.IdAdvertisers = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                postModelRef.IdBrands = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();


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
                    NumberFan = 0,//data.Where(e => e.PID == -1).Sum(a => a.NumberFan)
                    NumberLike = data.Where(e => e.PID == -1).Sum(a => a.NumberLike),
                    NumberPost = data.Where(e => e.PID == -1).Sum(a => a.NumberPost),
                    NumberShare = data.Where(e => e.PID == -1).Sum(a => a.NumberShare),
                    PageName = "Référents"
                };
                datas.Add(par);
                //datas.AddRange(data.Where(e => e.PID == -1).Select(e => { e.PID = 1; return e; }).ToList());
                datas.AddRange(data.Where(e => e.PID == -1).Select(e => { e.PID = 1; e.NumberFan = 0; return e; }).ToList());
                datas.AddRange(data.Where(e => e.PID != -1 && e.PID != 1).Select(e => e).ToList());
                datas = datas.OrderBy(d => d.PID).ThenBy(e => e.PageName).ToList();

                combos.Add(new { Text = "Tout", Value = "*", Selected = true , Level = 0});
                combos.Add(new { Text = par.PageName, Value = par.IdPageFacebook, Level = 0 });
                data.Where(e => e.PID == 1).ToList().ForEach(j =>
                    {
                        tmpList.Add(new { Text = j.PageName, Value = j.IdPageFacebook, Level = 1 });

                        data.Where(c => c.PID == j.ID).ToList().ForEach(c =>
                        {
                            tmpList.Add(new { Text = c.PageName, Value = c.IdPageFacebook, Level = 2 });
                        });
                    }
                );
                combos.AddRange(tmpList);
                tmpList = new List<object>();

                if (universeMarket.Count > 1)
                {
                    concurSelected = true;
                    //Domain.PostModel postModelConc = _webSessionService.GetPostModel(idSession); //Params : 1 = Référents; 0 = Concurrents
                    //postModelConc.IdAdvertisers = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                    //postModelConc.IdBrands = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                    Domain.PostModel postModelConc = _webSessionService.GetPostModel(idSession); //Params : 0 = Référents; 1 = Concurrents
                    postModelConc.IdAdvertisers = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                    postModelConc.IdBrands = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                    response = client.PostAsJsonAsync(new Uri(System.Configuration.ConfigurationManager.AppSettings["FacebookPageUri"]), postModelConc).Result;
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                        throw new Exception(response.StatusCode.ToString());
                    if (content.Length == 0)
                        return null;


                    data = JsonConvert.DeserializeObject<List<DataFacebook>>(content);
                    if (data.Count == 0)
                        return null;

                    par = new DataFacebook()
                    {
                        PID = -1,
                        ID = 2,
                        Expenditure = data.Where(e => e.PID == -1).Sum(a => a.Expenditure),
                        IdPageFacebook = string.Join(",", data.Where(e => e.PID == -1).Select(e => e.IdPageFacebook).ToList()),
                        NbPage = data.Where(e => e.PID == -1).Sum(a => a.NbPage),
                        NumberComment = data.Where(e => e.PID == -1).Sum(a => a.NumberComment),
                        NumberFan = 0,//data.Where(e => e.PID == -1).Sum(a => a.NumberFan),
                        NumberLike = data.Where(e => e.PID == -1).Sum(a => a.NumberLike),
                        NumberPost = data.Where(e => e.PID == -1).Sum(a => a.NumberPost),
                        NumberShare = data.Where(e => e.PID == -1).Sum(a => a.NumberShare),
                        PageName = "Concurrents"
                    };
                    datasTmp.Add(par);
                    //datasTmp.AddRange(data.Where(e => e.PID == -1).Select(e => { e.PID = 2; return e; }).ToList());
                    datasTmp.AddRange(data.Where(e => e.PID == -1).Select(e => { e.PID = 2; e.NumberFan = 0; return e; }).ToList());
                    datasTmp.AddRange(data.Where(e => e.PID != -1 && e.PID != 2).Select(e => e).ToList());
                    datasTmp = datasTmp.OrderBy(d => d.PID).ThenBy(e => e.PageName).ToList();
                    datas.AddRange(datasTmp);

                    combos.Add(new { Text = par.PageName, Value = par.IdPageFacebook, Level = 0 });
                    data.Where(e => e.PID == 2).ToList().ForEach(j =>
                        {
                            tmpList.Add(new { Text = j.PageName, Value = j.IdPageFacebook, Level = 1 });

                            data.Where(c => c.PID == j.ID).ToList().ForEach(c =>
                            {
                                tmpList.Add(new { Text = c.PageName, Value = c.IdPageFacebook, Level = 2 });
                            });
                        }
                    );
                    combos.AddRange(tmpList);
                    tmpList = new List<object>();
                }

                gridResult.HasData = true;
                gridResult.Columns = columns;
                gridResult.Schema = schemaFields;


                try
                {
                    if (!gridResult.HasData)
                        return null;

                    string jsonData = JsonConvert.SerializeObject(datas);
                    var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, combo = combos, unit = "number", concurSelected = concurSelected };
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
                    postModelRef.IdAdvertisers = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                    postModelRef.IdBrands = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();
                }

                HttpResponseMessage response = client.PostAsJsonAsync(new Uri(System.Configuration.ConfigurationManager.AppSettings["FacebookPostUri"]), postModelRef).Result;
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());

                List<DataPostFacebook> data = JsonConvert.DeserializeObject<List<DataPostFacebook>>(content);
                data = data.OrderBy(e => e.DateCreationPost).ToList();
                if(data.Count == 0)
                {
                    return null;
                }
                string jsonData = JsonConvert.SerializeObject(data);
                var obj = new { datagrid = jsonData, columns = columns, schema = schemaFields, columnsfixed = columnsFixed, needfixedcolumns = false };
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

        public async Task<ActionResult> GetReferChart()
        {
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

                //Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession); //Params : 0 = Référents; 1 = Concurrents
                //postModelRef.IdAdvertisers = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                //postModelRef.IdBrands = universeMarket[IndexListRef].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                Domain.PostModel postModelRef = _webSessionService.GetPostModel(idSession); //Params : 0 = Référents; 1 = Concurrents
                postModelRef.IdAdvertisers = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                postModelRef.IdBrands = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

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

                return PartialView("GetReferChart", data);
            }
        }

        public async Task<ActionResult> GetPDMChart()
        {
            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                Domain.PostModel postModel = _webSessionService.GetPostModel(idSession); //Params : 0 = Concurrents; 1 = Référents

                postModel.IdAdvertisersConcur = new List<long>();
                postModel.IdBrandsConcur = new List<long>();

                List<Domain.Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                //if (universeMarket.Count > 1)
                //{

                //    postModel.IdAdvertisersConcur = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                //    postModel.IdBrandsConcur = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                //    postModel.IdAdvertisersRef = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                //    postModel.IdBrandsRef = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();
                //}
                //else
                //{
                //    postModel.IdAdvertisersRef = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                //    postModel.IdBrandsRef = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();
                //}


                postModel.IdAdvertisersRef = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                postModel.IdBrandsRef = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                if (universeMarket.Count > 1)
                {
                    postModel.IdAdvertisersConcur = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                    postModel.IdBrandsConcur = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();
                }

                HttpResponseMessage response = client.PostAsJsonAsync(new Uri("http://localhost:9990/api/KPI/Plurimedia"), postModel).Result;
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());

                List<KPIPercentPageFacebookContract> data = JsonConvert.DeserializeObject<List<KPIPercentPageFacebookContract>>(content);

                //TODO: a faire autrement
                //data = data.Where(e => e.Month != DateTime.Now.ToString("yyyyMM")).ToList();

                if (data.Count == 0)
                {
                    data.Add(new KPIPercentPageFacebookContract());
                }

                return PartialView("GetPDMChart", data);
            }
        }

        public async Task<ActionResult> GetConcurChart()
        {
            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                List<Domain.Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                if (universeMarket.Count < 2)
                {
                    throw new Exception("Pas de concurrents");
                }

                Domain.PostModel postModelConcur = _webSessionService.GetPostModel(idSession); //Params : 1 = Concurrents; 0 = Référents
                List<long> advertiserIds = new List<long>();
                advertiserIds.AddRange(universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList());
                advertiserIds.AddRange(universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList());

                List<long> brandIds = new List<long>();
                brandIds.AddRange(universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList());
                brandIds.AddRange(universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList());

                postModelConcur.IdAdvertisers = advertiserIds;
                postModelConcur.IdBrands = brandIds;

                HttpResponseMessage response = client.PostAsJsonAsync(new Uri("http://localhost:9990/api/KPI/Classification"), postModelConcur).Result;
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());

                List<KPIClassificationContract> data = JsonConvert.DeserializeObject<List<KPIClassificationContract>>(content);

                //TODO: a faire autrement
                data.ToList().ForEach(e =>
                {
                    if(universeMarket[0].UniversLevels.First().UniversItems.Where(j => j.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList().Contains(e.Id)
                        || universeMarket[0].UniversLevels.First().UniversItems.Where(j => j.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList().Contains(e.Id))
                    {
                        e.UniverseMarket = 0; // 0 = referent, 1 = concurrent
                    }
                    else{
                        e.UniverseMarket = 1;// 0 = referent, 1 = concurrent
                    }
                });

                data = data.OrderByDescending(l => l.UniverseMarket).ThenByDescending(e => e.Label).ToList();
                //if (data.Count == 0)
                //{
                //    data.Add(new KPIClassificationContract());
                //}

                return PartialView("GetConcurChart", data);
            }
        }

        public async Task<ActionResult> GetPlurimediaStackedChart()
        {
            using (var client = new HttpClient())
            {
                var cla = new ClaimsPrincipal(User.Identity);
                string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

                List<Domain.Tree> universeMarket = _detailSelectionService.GetMarket(idSession);

                Domain.PostModel postModel = _webSessionService.GetPostModel(idSession); //Params : 0 = Concurrents; 1 = Référents

                postModel.IdAdvertisersConcur = new List<long>();
                postModel.IdBrandsConcur = new List<long>();

                //if (universeMarket.Count > 1)
                //{

                //    postModel.IdAdvertisersConcur = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                //    postModel.IdBrandsConcur = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                //    postModel.IdAdvertisersRef = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                //    postModel.IdBrandsRef = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();
                //}
                //else
                //{
                //    postModel.IdAdvertisersRef = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                //    postModel.IdBrandsRef = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();
                //}

                postModel.IdAdvertisersRef = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                postModel.IdBrandsRef = universeMarket[0].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();

                if (universeMarket.Count > 1)
                {
                    postModel.IdAdvertisersConcur = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList();
                    postModel.IdBrandsConcur = universeMarket[1].UniversLevels.First().UniversItems.Where(e => e.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList();
                }

                HttpResponseMessage response = client.PostAsJsonAsync(new Uri("http://localhost:9990/api/KPI/PlurimediaStacked"), postModel).Result;
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());

                List<PDVByMediaPageFacebookContract> data = JsonConvert.DeserializeObject<List<PDVByMediaPageFacebookContract>>(content);
                if (data == null)
                    data.Add(new PDVByMediaPageFacebookContract());
                else if (data.Count == 0)
                    data.Add(new PDVByMediaPageFacebookContract());


                //TODO: a faire autrement
                data.ToList().ForEach(e =>
                {
                    if (universeMarket[0].UniversLevels.First().UniversItems.Where(j => j.IdLevelUniverse == TNSClassificationLevels.ADVERTISER).Select(z => z.Id).ToList().Contains(e.Id)
                        || universeMarket[0].UniversLevels.First().UniversItems.Where(j => j.IdLevelUniverse == TNSClassificationLevels.BRAND).Select(z => z.Id).ToList().Contains(e.Id))
                    {
                        e.UniverseMarket = 0; // 0 = referent, 1 = concurrent
                    }
                    else {
                        e.UniverseMarket = 1;// 0 = referent, 1 = concurrent
                    }
                });

                data = data.OrderByDescending(l => l.UniverseMarket).ThenByDescending(e => e.Label).ToList();

                return PartialView("GetPlurimediaStackedChart", data);
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
                HttpResponseMessage response = client.PostAsJsonAsync(new Uri(ConfigurationManager.AppSettings["FacebookOnePostUri"]), new { idPost, idLanguage }).Result;
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

        public ActionResult GetPostHtml(string id)
        {
            try
            {
                var uri = ConfigurationManager.AppSettings["FacebookPostWebUri"];
                var url = string.Format("{0}?id={1}", uri, id);
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();

                return File(response.GetResponseStream(), "text/html");
            }
            catch
            {
                return File("~/Content/img/no_visu.jpg", "image/jpg");
            }

        }
    }
}