using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using VM = Km.AdExpressClientWeb.Models.MediaSchedule;
using KM.AdExpress.Framework.MediaSelection;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core;
using AutoMapper;
using Newtonsoft.Json;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using TNS.Classification.Universe;
using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;
using Km.AdExpressClientWeb.Helpers;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class PortfolioController : Controller
    {
        private IPortfolioService _portofolioService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IUniverseService _universService;
        private IPeriodService _periodService;
        private IOptionService _optionService;
        private const string _controller = "Portfolio";
        private const string MARKET = "Market";
        private const string MEDIA = "MediaSelection";
        private const string SELECTION = "Selection";
        private const string INDEX = "Index";
        private const string PERIOD = "PeriodSelection";
        private const string ERROR = "Invalid Selection";
        private const string CalendarFormatDays = "DD/MM/YYYY";
        private const string CalendarFormatMonths = "MM/YYYY";
        private const int MarketPageId = 2;
        private const int MediaPageId = 6;
        private int _siteLanguage = 33;

        public PortfolioController(IPortfolioService portofolioService, IMediaService mediaService, IWebSessionService webSessionService, IUniverseService universService, IPeriodService periodService, IOptionService optionService)
        {
            _portofolioService = portofolioService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _universService = universService;
            _periodService = periodService;
            _optionService = optionService;
        }

        // GET: Portfolio
        public ActionResult Index()
        {
            #region Init
            var model = new MarketViewModel
            {
                Trees = new List<Tree>(),
                Branches = new List<UniversBranch>(),
                UniversGroups = new UserUniversGroupsModel(),
                Dimension = Dimension.product
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            #endregion
            #region Load Branches
            var result = _universService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.product, true);
            #endregion
            #region Load each label's text in the appropriate language
            model.Labels = LoadPageLabels(result.SiteLanguage);
            model.Branches = Mapper.Map<List<UniversBranch>>(result.Branches);
            foreach (var item in result.Trees)
            {
                Tree tree = new Tree
                {
                    Id = item.Id,
                    LabelId = item.LabelId,
                    AccessType = item.AccessType,
                    UniversLevels = Mapper.Map<List<UniversLevel>>(item.UniversLevels)
                };
                tree.Label = (tree.AccessType == TNS.Classification.Universe.AccessType.includes) ? model.Labels.IncludedElements : model.Labels.ExcludedElements;
                model.Trees.Add(tree);
            }
            #endregion
            #region Presentation
            var pageHelper = new Helpers.PageHelper();
            model.Presentation = pageHelper.LoadPresentationBar(result.SiteLanguage, result.ControllerDetails);
            model.UniversGroups = new UserUniversGroupsModel
            {
                ShowUserSavedGroups = true,
                UserUniversGroups = new List<UserUniversGroup>(),
                UserUniversCode = LanguageConstantes.UserUniversCode,
                SiteLanguage = result.SiteLanguage
            };
            #endregion
            var marketNode = new NavigationNode { Position = 1 };
            var navigationHelper = new Helpers.PageHelper();
            model.NavigationBar = navigationHelper.LoadNavBar(webSessionId, _controller, _siteLanguage,1);
            return View(model);
        }

        public ActionResult Presentation()
        {
            return View();
        }

        public ActionResult MediaSelection()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _mediaService.GetMedia(webSessionId);

            #region model data
            var model = new MediaSelectionViewModel()
            {
                Multiple = false,
                Medias = result.Media,
                IdMediasCommon = result.MediaCommon,
                Branches = new List<Models.Shared.UniversBranch>(),
                Trees = new List<Models.Shared.Tree>(),
                Dimension = Dimension.media,
                UniversGroups = new UserUniversGroupsModel()
            };
            model.UniversGroups = new UserUniversGroupsModel
            {
                ShowUserSavedGroups = true,
                UserUniversGroups = new List<UserUniversGroup>(),
                UserUniversCode = LanguageConstantes.UserUniversCode,
                SiteLanguage = result.SiteLanguage
            };
            var pageHelper = new Helpers.PageHelper();
            model.Presentation = pageHelper.LoadPresentationBar(result.SiteLanguage, result.ControllerDetails);
            foreach (var e in model.Medias)
            {
                e.icon = IconSelector.getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
            _siteLanguage = result.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            var mediaNode = new NavigationNode { Position = 2 };
            var navigationHelper = new Helpers.PageHelper();
            model.NavigationBar = navigationHelper.LoadNavBar(webSessionId, _controller, _siteLanguage,2);
            model.ErrorMessage = new Models.Shared.ErrorMessage
            {
                EmptySelection = GestionWeb.GetWebWord(1052, result.SiteLanguage),
                SearchErrorMessage = GestionWeb.GetWebWord(3011, result.SiteLanguage),
                SocialErrorMessage = GestionWeb.GetWebWord(3030, result.SiteLanguage),
                UnitErrorMessage = GestionWeb.GetWebWord(2541, result.SiteLanguage)
            };
            model.Labels = LoadPageLabels(result.SiteLanguage);
            var response = _universService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.media, true, 1, 0);
            model.Branches = Mapper.Map<List<UniversBranch>>(response.Branches);
            foreach (var item in response.Trees)
            {
                Models.Shared.Tree tree = new Models.Shared.Tree
                {
                    Id = item.Id,
                    LabelId = item.LabelId,
                    AccessType = item.AccessType,
                    UniversLevels = Mapper.Map<List<Models.Shared.UniversLevel>>(item.UniversLevels)
                };
                tree.Label = (tree.AccessType == TNS.Classification.Universe.AccessType.includes) ? model.Labels.IncludedElements : model.Labels.ExcludedElements;
                model.Trees.Add(tree);
            }
            #endregion
            return View(model);
        }

        public ActionResult PeriodSelection()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var result = _periodService.GetPeriod(idSession);

            PeriodViewModel periodModel = new PeriodViewModel();
            periodModel.SiteLanguage = result.SiteLanguage;
            periodModel.StartYear = string.Format("{0}-01-01", result.StartYear);
            periodModel.EndYear = string.Format("{0}-12-31", result.EndYear);
            switch (result.ControllerDetails.ModuleId)
            {
                case Module.Name.ANALYSE_PLAN_MEDIA:
                case Module.Name.ANALYSE_PORTEFEUILLE:
                case Module.Name.ANALYSE_DYNAMIQUE:
                case Module.Name.ANALYSE_CONCURENTIELLE:
                    periodModel.CalendarFormat = CalendarFormatDays;
                    break;
                case Module.Name.INDICATEUR:
                case Module.Name.TABLEAU_DYNAMIQUE:
                    periodModel.CalendarFormat = CalendarFormatMonths;
                    break;
                default:
                    periodModel.CalendarFormat = CalendarFormatDays;
                    break;

            }

            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(result.SiteLanguage);
            NavigationNode periodeNode = new NavigationNode { Position = 3 };
            var navigationHelper = new Helpers.PageHelper();
            var navBarModel = navigationHelper.LoadNavBar(idSession, result.ControllerDetails.Name, result.SiteLanguage, 3);

            PeriodSelectionViewModel model = new PeriodSelectionViewModel();
            model.PeriodViewModel = periodModel;
            model.NavigationBar = navBarModel;
            model.Presentation = navigationHelper.LoadPresentationBar(result.SiteLanguage, result.ControllerDetails);
            model.ErrorMessage = new Models.Shared.ErrorMessage
            {
                EmptySelection = GestionWeb.GetWebWord(885, result.SiteLanguage),
                PeriodErrorMessage = GestionWeb.GetWebWord(1855, result.SiteLanguage)
            };

            return View(model);
        }
        public JsonResult CalendarValidation(string selectedStartDate, string selectedEndDate, string nextStep)
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            JsonResult jsonModel = new JsonResult();
            string url = string.Empty;

            var response = _periodService.CalendarValidation(idSession, selectedStartDate, selectedEndDate, nextStep);
            var controller = response.ControllerDetails.Name;
            string action = (controller == SELECTION && nextStep == INDEX) ? MARKET : nextStep;
            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            if (response.Success)
            {
                url = context.Action(action, controller);
                jsonModel = Json(new { Success = true, RedirectUrl = url });
            }
            else
            {
                jsonModel = Json(new { Success = false, ErrorMessage = response.ErrorMessage });
            }
            return jsonModel;
        }

        public JsonResult SlidingDateValidation(int selectedPeriod, int selectedValue, string nextStep)
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            JsonResult jsonModel = new JsonResult();
            string url = string.Empty;

            var response = _periodService.SlidingDateValidation(idSession, selectedPeriod, selectedValue, nextStep);
            var controller = response.ControllerDetails.Name;
            string action = (controller == SELECTION && nextStep == INDEX) ? MARKET : nextStep;
            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            if (response.Success)
            {
                url = context.Action(action, controller);
                jsonModel = Json(new { Success = true, RedirectUrl = url });
            }
            else
            {
                jsonModel = Json(new { Success = false, ErrorMessage = response.ErrorMessage });
            }
            return jsonModel;
        }

        public ActionResult Results()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _webSessionService.GetWebSession(idSession);
            _siteLanguage = result.WebSession.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            var resultNode = new NavigationNode { Position = 4 };
            var navigationHelper = new Helpers.PageHelper();
            var model = new Models.Portfolio.ResultsViewModel
            {
                NavigationBar = navigationHelper.LoadNavBar(idSession, _controller, _siteLanguage,4),
                Presentation = navigationHelper.LoadPresentationBar(result.WebSession.SiteLanguage, result.ControllerDetails),
                Labels = LoadPageLabels(result.WebSession.SiteLanguage)
            };

            return View(model);
        }

        public JsonResult PortfolioResult()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var gridResult = _portofolioService.GetGridResult(idWebSession);

            try
            {
                if (!gridResult.HasData)
                    return null;

                string jsonData = JsonConvert.SerializeObject(gridResult.Data);
                var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, needfixedcolumns = gridResult.NeedFixedColumns };
                JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        
        public ActionResult PortfolioGraphResult()
        {
            int i = -1;
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var gridResult = _portofolioService.GetGraphGridResult(idWebSession);
            List<object> objList = new List<object>();

            try
            {
                if (!gridResult[0].HasData)
                    return null;

                foreach (var gr in gridResult)
                {
                    i++;
                    string jsonData = JsonConvert.SerializeObject(gridResult[i].Data);
                    object obj = new { datachart = jsonData, schema = gridResult[i].Schema, title = gridResult[i].Title };
                    objList.Add(obj);
                }
                
                JsonResult jsonModel = Json(objList, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult ResultOptions()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            Options options = _optionService.GetOptions(idWebSession);
            return PartialView("_ResultOptions", options);
        }

        public void SetResultOptions(UserFilter userFilter)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            _optionService.SetOptions(idWebSession, userFilter);
        }

        public JsonResult SaveCustomDetailLevels(string levels, string type)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            JsonResult jsonModel = new JsonResult();
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            Domain.SaveLevelsResponse response = _optionService.SaveCustomDetailLevels(idWebSession, levels, type);
            jsonModel = Json(new { Id = response.CustomDetailLavelsId, Label = response.CustomDetailLavelsLabel, Message = response.Message });

            return jsonModel;
        }

        public JsonResult RemoveCustomDetailLevels(string detailLevel)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            JsonResult jsonModel = new JsonResult();
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            string message = _optionService.RemoveCustomDetailLevels(idWebSession, detailLevel);
            jsonModel = Json(new { Message = message });

            return jsonModel;
        }

        public JsonResult SaveMediaSelection(List<long> selectedMedia, List<Domain.Tree> mediaSupport, string nextStep)
        {
            string url = string.Empty;
            var response = new Domain.WebSessionResponse();
            var errorMsg = String.Empty;
            JsonResult jsonModel = new JsonResult();
            if (selectedMedia != null & mediaSupport !=null)
            {
                List<Domain.Tree> trees = (mediaSupport != null) ? mediaSupport : new List<Domain.Tree>();
                trees = trees.Where(p => p.UniversLevels.Where(x => x.UniversItems != null).Any()).ToList();
                var claim = new ClaimsPrincipal(User.Identity);
                string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                Domain.SaveMediaSelectionRequest request = new Domain.SaveMediaSelectionRequest(selectedMedia, idWebSession, trees, Dimension.media, Security.full, true, nextStep);
                response = _webSessionService.SaveMediaSelection(request);
                UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
                if (response.Success)
                {
                    url = context.Action(nextStep, _controller);
                    jsonModel = Json(new { RedirectUrl = url, ErrorMessage = errorMsg });
                }
                else
                {
                    jsonModel = Json(new { response.ErrorMessage });
                }
            } 
            return jsonModel;
        }
        
        [HttpPost]
        public JsonResult SaveMarketSelection(List<Tree> trees, string nextStep)
        {
            string errorMessage = string.Empty;
            if (trees.Any() && trees.Where(p => p.UniversLevels != null).Any())
            {
                var claim = new ClaimsPrincipal(User.Identity);
                string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                List<Tree> validTrees = trees.Where(p => p.UniversLevels != null && p.UniversLevels.Where(x => x.UniversItems != null).Any()).ToList();
                var data = Mapper.Map<List<Domain.Tree>>(validTrees);
                Domain.SaveMarketSelectionRequest request = new Domain.SaveMarketSelectionRequest(webSessionId, data, Dimension.product, Security.full, false, nextStep);
                var result = _webSessionService.SaveMarketSelection(request);
                if (result.Success)
                {
                    UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
                    var redirectUrl = context.Action(nextStep, _controller);
                    return Json(new { ErrorMessage = errorMessage, RedirectUrl = redirectUrl });
                }
                else
                    errorMessage = result.ErrorMessage;
            }
            return Json(new { ErrorMessage = errorMessage });
        }
        public JsonResult GetMediaSupport()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _universService.GetBranches(webSessionId, Dimension.media, true);
            return Json(result);
        }

        public JsonResult VehicleCovers(int resultType)
        {
           
            var claim = new ClaimsPrincipal(User.Identity);
           
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var coversResult = _portofolioService.GetVehicleCovers(idWebSession, resultType);
            if (coversResult != null && coversResult.Any())
            {
                JsonResult jsonModel = new JsonResult();
                jsonModel = Json(coversResult, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;
            }

            return null;
        }

        public JsonResult VehiclePages(string mediaId,string dateMediaNum,string nbPage,string media)
        {
            
            var claim = new ClaimsPrincipal(User.Identity);

            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var pagesResult = _portofolioService.GetVehiclePages(idWebSession,mediaId, dateMediaNum,nbPage, media);
            if(pagesResult !=null && pagesResult.Any())
            {
                JsonResult jsonModel = new JsonResult();
                jsonModel = Json(pagesResult, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;
            }


            return null;
        }

        #region Private methodes       

        private Labels LoadPageLabels(int siteLanguage)
        {
            var result = new Labels
            {
                CurrentController = "Portfolio",
                KeyWordLabel = GestionWeb.GetWebWord(LanguageConstantes.KeyWordLabelCode, siteLanguage),
                KeyWordDescription = GestionWeb.GetWebWord(LanguageConstantes.KeyWordDescriptionCode, siteLanguage),
                ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.NoSavedUniversCode, siteLanguage),
                BranchLabel = GestionWeb.GetWebWord(LanguageConstantes.BranchLabelCode, siteLanguage),
                NoSavedUnivers = GestionWeb.GetWebWord(LanguageConstantes.NoSavedUniversCode, siteLanguage),
                UserSavedUniversLabel = GestionWeb.GetWebWord(LanguageConstantes.UserSavedUniversCode, siteLanguage),
                Include = GestionWeb.GetWebWord(LanguageConstantes.IncludeCode, siteLanguage),
                Exclude = GestionWeb.GetWebWord(LanguageConstantes.ExcludeCode, siteLanguage),
                LoadUnivers = GestionWeb.GetWebWord(LanguageConstantes.LoadUniversCode, siteLanguage),
                IncludedElements = GestionWeb.GetWebWord(LanguageConstantes.IncludedElements, siteLanguage),
                ExcludedElements = GestionWeb.GetWebWord(LanguageConstantes.ExcludedElements, siteLanguage),
                MyResults = GestionWeb.GetWebWord(LanguageConstantes.ResultsCode, siteLanguage),
                Refine = GestionWeb.GetWebWord(LanguageConstantes.RefineCode, siteLanguage),
                ErrorMessageLimitKeyword = GestionWeb.GetWebWord(LanguageConstantes.LimitKeyword, siteLanguage),
                ErrorMessageLimitUniverses = GestionWeb.GetWebWord(LanguageConstantes.LimitUniverses, siteLanguage),
                ErrorMininumInclude = GestionWeb.GetWebWord(LanguageConstantes.MininumInclude, siteLanguage),
                ErrorItemExceeded = GestionWeb.GetWebWord(LanguageConstantes.ItemExceeded, siteLanguage),
                ErrorMediaSelected = GestionWeb.GetWebWord(LanguageConstantes.MediaSelected, siteLanguage),
                ErrorNoSupport = GestionWeb.GetWebWord(LanguageConstantes.NoSupport, siteLanguage),
                DeleteAll= GestionWeb.GetWebWord(LanguageConstantes.DeleteAllcode, siteLanguage),
                ErrorOnlyOneItemAllowed = GestionWeb.GetWebWord(LanguageConstantes.ErrorOnlyOneItemAllowed, siteLanguage),
                ErrorOverLimit = GestionWeb.GetWebWord(LanguageConstantes.ErrorOverLimit, siteLanguage),
                SaveUnivers = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, siteLanguage),
                SelectMedia = GestionWeb.GetWebWord(LanguageConstantes.SelectMedia, siteLanguage),
                PreSelection = GestionWeb.GetWebWord(LanguageConstantes.PreSelection, siteLanguage),
                Results = GestionWeb.GetWebWord(LanguageConstantes.Results, siteLanguage),
                Save = GestionWeb.GetWebWord(LanguageConstantes.Save, siteLanguage),
                CreateAlert = GestionWeb.GetWebWord(LanguageConstantes.CreateAlert, siteLanguage),
                ExportFormattedResult = GestionWeb.GetWebWord(LanguageConstantes.ExportFormattedResult, siteLanguage),
                ExportResultWithValue = GestionWeb.GetWebWord(LanguageConstantes.ExportResultWithValue, siteLanguage),
                ExportGrossResult = GestionWeb.GetWebWord(LanguageConstantes.ExportGrossResult, siteLanguage),
                ExportPdfResult = GestionWeb.GetWebWord(LanguageConstantes.ExportPdfResult, siteLanguage),
                ExportPptResult = GestionWeb.GetWebWord(LanguageConstantes.ExportPptResult, siteLanguage),
                Search = GestionWeb.GetWebWord(LanguageConstantes.Search, siteLanguage)
            };
            return result;
        }

        #endregion
    }
}