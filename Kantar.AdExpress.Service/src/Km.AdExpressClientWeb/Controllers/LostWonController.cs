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
using VM = Km.AdExpressClientWeb.Models.LostWon;
using KM.AdExpress.Framework.MediaSelection;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core;
using AutoMapper;
using Newtonsoft.Json;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using TNS.Classification.Universe;
using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;

namespace Km.AdExpressClientWeb.Controllers
{
    public class LostWonController : Controller
    {
        private ILostWonService _lostWonService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IUniverseService _universService;
        private IPeriodService _periodService;
        private IOptionService _optionService;
        private const string _controller = "LostWon";
        private const int MarketPageId = 2;
        private const int MediaPageId = 6;
        private const int MaxIncludeNbr = 2;
        private const int MaxExcludeNbr = 1;
        private int _siteLanguage = 33;

        public LostWonController(ILostWonService lostWonService, IMediaService mediaService, IWebSessionService webSessionService, IUniverseService universService, IPeriodService periodService, IOptionService optionService)
        {
            _lostWonService = lostWonService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _universService = universService;
            _periodService = periodService;
            _optionService = optionService;
        }

        // GET: LostWon
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
            var result = _universService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.product, true, MaxIncludeNbr,MaxExcludeNbr);
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
            model.Presentation = LoadPresentationBar(result.SiteLanguage);
            model.UniversGroups = new UserUniversGroupsModel
            {
                ShowUserSavedGroups = true,
                UserUniversGroups = new List<UserUniversGroup>(),
                UserUniversCode = LanguageConstantes.UserUniversCode,
                SiteLanguage = result.SiteLanguage
            };
            #endregion
            var marketNode = new NavigationNode { Position = 1 };
            var navigationHelper = new Helpers.NavigationHelper();
            model.NavigationBar = navigationHelper.LoadNavBar(marketNode.Position, _controller, _siteLanguage);
            return View(model);
        }

        public ActionResult MediaSelection()
        {
            //var model = new MediaSelectionViewModel();

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
            model.Presentation = LoadPresentationBar(result.SiteLanguage);
            foreach (var e in model.Medias)
            {
                e.icon = IconSelector.getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
            var mediaNode = new NavigationNode { Position = 2 };
            var navigationHelper = new Helpers.NavigationHelper();
            model.NavigationBar = navigationHelper.LoadNavBar(mediaNode.Position, _controller, _siteLanguage);
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
                    Id = 1,
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

            NavigationNode periodeNode = new NavigationNode { Position = 3 };
            var navigationHelper = new Helpers.NavigationHelper();
            var navBarModel = navigationHelper.LoadNavBar(periodeNode.Position, _controller, _siteLanguage);

            PeriodSelectionViewModel model = new PeriodSelectionViewModel();
            model.PeriodViewModel = periodModel;
            model.NavigationBar = navBarModel;
            model.Presentation = LoadPresentationBar(result.SiteLanguage);

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

            string url = string.Empty;
            var response = _periodService.CalendarValidation(idSession, selectedStartDate, selectedEndDate);

            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            if (response.Success)
                url = context.Action(nextStep, _controller);

            JsonResult jsonModel = Json(new { RedirectUrl = url });

            return jsonModel;
        }

        public JsonResult SlidingDateValidation(int selectedPeriod, int selectedValue, string nextStep)
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            string url = string.Empty;
            var response = _periodService.SlidingDateValidation(idSession, selectedPeriod, selectedValue);
            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            if (response.Success)
                url = context.Action(nextStep, _controller);

            JsonResult jsonModel = Json(new { RedirectUrl = url });

            return jsonModel;
        }
        public ActionResult Results()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession CustomerSession = (WebSession)WebSession.Load(idSession);
            var resultNode = new NavigationNode { Position = 4 };
            var navigationHelper = new Helpers.NavigationHelper();          
            var model = new Models.LostWon.ResultsViewModel
            {
                NavigationBar = navigationHelper.LoadNavBar(resultNode.Position, _controller, _siteLanguage),
                Presentation = LoadPresentationBar(CustomerSession.SiteLanguage),
                Labels =LoadPageLabels(CustomerSession.SiteLanguage)
            };

            return View(model);
        }

        public JsonResult LostWonResult()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var gridResult = _lostWonService.GetGridResult(idWebSession);

            if (!gridResult.HasData) return null;

            string jsonData = JsonConvert.SerializeObject(gridResult.Data);

            var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, needfixedcolumns = gridResult.NeedFixedColumns };
            JsonResult jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
            jsonModel.MaxJsonLength = Int32.MaxValue;

            return jsonModel;
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
            if (selectedMedia != null)
            {
                List<Domain.Tree> trees = (mediaSupport != null) ? mediaSupport : new List<Domain.Tree>();
                trees = trees.Where(p => p.UniversLevels.Where(x => x.UniversItems != null).Any()).ToList();
                var claim = new ClaimsPrincipal(User.Identity);
                string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                response = _webSessionService.SaveMediaSelection(selectedMedia, idWebSession, trees, Dimension.media, Security.full,true);
                UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
                if(response.Success)
                {
                    url = context.Action(nextStep, _controller);
                    jsonModel = Json(new { RedirectUrl = url, ErrorMessage = errorMsg });
                }
                else
                {
                    jsonModel = Json(new { RedirectUrl = url, ErrorMessage = errorMsg });
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
                var result = _webSessionService.SaveMarketSelection(webSessionId, data, Dimension.product, Security.full);
                if (result.Success)
                {
                    UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
                    var redirectUrl = context.Action(nextStep, _controller);
                    return Json(new { ErrorMessage = errorMessage, RedirectUrl = redirectUrl });
                }
                else
                    errorMessage = result.ErrorMessage;
            }
            else
            {
                errorMessage = "Invalid Selection";
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

        #region Private methodes
      
        private Labels LoadPageLabels(int siteLanguage)
        {
            var result = new Labels
            {
                CurrentController = _controller,
                KeyWordLabel = GestionWeb.GetWebWord(LanguageConstantes.KeyWordLabelCode, siteLanguage),
                KeyWordDescription = GestionWeb.GetWebWord(LanguageConstantes.KeyWordDescriptionCode, siteLanguage),
                ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.NoSavedUniversCode, siteLanguage),
                BranchLabel = GestionWeb.GetWebWord(LanguageConstantes.BranchLabelCode, siteLanguage),
                NoSavedUnivers = GestionWeb.GetWebWord(LanguageConstantes.NoSavedUniversCode, siteLanguage),
                UserSavedUniversLabel = GestionWeb.GetWebWord(LanguageConstantes.UserSavedUniversCode, siteLanguage),
                Include = GestionWeb.GetWebWord(LanguageConstantes.IncludeCode, siteLanguage),
                Exclude = GestionWeb.GetWebWord(LanguageConstantes.ExcludeCode, siteLanguage),
                LoadUnivers = GestionWeb.GetWebWord(LanguageConstantes.LoadUniversCode, siteLanguage),
                SaveUnivers = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, siteLanguage),
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
                DeleteAll = GestionWeb.GetWebWord(LanguageConstantes.DeleteAllcode, siteLanguage),
                ErrorOnlyOneItemAllowed = GestionWeb.GetWebWord(LanguageConstantes.ErrorOnlyOneItemAllowed, siteLanguage),
                ErrorOverLimit = GestionWeb.GetWebWord(LanguageConstantes.ErrorOverLimit, siteLanguage),
                SelectMedia = GestionWeb.GetWebWord(LanguageConstantes.SelectMedia, siteLanguage),
                PreSelection = GestionWeb.GetWebWord(LanguageConstantes.PreSelection, siteLanguage),
                Results = GestionWeb.GetWebWord(LanguageConstantes.Results, siteLanguage),
                Save = GestionWeb.GetWebWord(LanguageConstantes.Save, siteLanguage),
                CreateAlert = GestionWeb.GetWebWord(LanguageConstantes.CreateAlert, siteLanguage)
            };
            return result;
        }
        private PresentationModel LoadPresentationBar(int siteLanguage, bool showCurrentSelection = true)
        {
            PresentationModel result = new PresentationModel
            {
                ModuleCode = LanguageConstantes.LostWonCode,
                SiteLanguage = siteLanguage,
                ModuleDecriptionCode = LanguageConstantes.LostWonDescriptionCode,
                ShowCurrentSelection = showCurrentSelection
            };
            return result;
        }
        #endregion
    }
}