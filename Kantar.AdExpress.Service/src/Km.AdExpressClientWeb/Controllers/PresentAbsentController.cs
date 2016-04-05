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
using VM = Km.AdExpressClientWeb.Models.PresentAbsent;
using KM.AdExpress.Framework.MediaSelection;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core;
using AutoMapper;
using Newtonsoft.Json;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using TNS.Classification.Universe;
using Km.AdExpressClientWeb.Models.Shared;

namespace Km.AdExpressClientWeb.Controllers
{
    public class PresentAbsentController : Controller
    {
        private IPresentAbsentService _presentAbsentService;
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IUniverseService _universService;
        private IPeriodService _periodService;
        private IOptionService _optionService;
        private const string _controller = "PresentAbsent";
        private const int MarketPageId = 2;
        private const int MediaPageId = 6;
        private const int MaxIncludeNbr = 1;
        private const int MaxExcludeNbr = 0;


        public PresentAbsentController(IPresentAbsentService presentAbsentService, IMediaService mediaService, IWebSessionService webSessionService, IUniverseService universService, IPeriodService periodService, IOptionService optionService)
        {
            _presentAbsentService = presentAbsentService;
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _universService = universService;
            _periodService = periodService;
            _optionService = optionService;
        }

        // GET: PresentAbsent
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
            model.NavigationBar = LoadNavBar(marketNode.Position);
            return View(model);
        }

        public ActionResult Presentation()
        {
            return View();
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
            model.NavigationBar = LoadNavBar(mediaNode.Position);
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
                    Label =item.Label,
                    UniversLevels = Mapper.Map<List<Models.Shared.UniversLevel>>(item.UniversLevels)
                };
                tree.Label = (tree.Id==0) ? model.Labels.Referent : model.Labels.Concurrent;
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
            var navBarModel = LoadNavBar(periodeNode.Position);

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
            var model = new Models.PresentAbsent.ResultsViewModel
            {
                NavigationBar = LoadNavBar(resultNode.Position),
                Presentation = LoadPresentationBar(CustomerSession.SiteLanguage)
            };

            return View(model);
        }

        public JsonResult PresentAbsentResult()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var gridResult = _presentAbsentService.GetGridResult(idWebSession);

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
        private List<NavigationNode> LoadNavBar(int currentPosition)
        {
            var model = new List<NavigationNode>();
            //TODO Update Navbar according to the country selection
            #region Hardcoded  nav Bar.
            var market = new NavigationNode
            {
                Id = 1,
                IsActive = false,
                Description = "Market",
                Title = "Marché",
                Action = "Index",
                Controller = _controller,
                IconCssClass = "fa fa-file-text"
            };
            model.Add(market);
            var media = new NavigationNode
            {
                Id = 2,
                IsActive = false,
                Description = "Media",
                Title = "Media",
                Action = "MediaSelection",
                Controller = _controller,
                IconCssClass = "fa fa-eye"
            };
            model.Add(media);
            var dates = new NavigationNode
            {
                Id = 3,
                IsActive = false,
                Description = "Dates",
                Title = "Dates",
                Action = "PeriodSelection",
                Controller = _controller,
                IconCssClass = "fa fa-calendar"
            };
            model.Add(dates);
            var result = new NavigationNode
            {
                Id = 4,
                IsActive = false,
                Description = "Results",
                Title = "Resultats",
                Action = "Results",
                Controller = _controller,
                IconCssClass = "fa fa-check"
            };
            model.Add(result);
            foreach (var nav in model)
            {
                nav.IsActive = (nav.Id > currentPosition) ? false : true;
            }
            #endregion
            return model;
        }

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
                Save = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, siteLanguage),
                IncludedElements = GestionWeb.GetWebWord(LanguageConstantes.IncludedElements, siteLanguage),
                ExcludedElements = GestionWeb.GetWebWord(LanguageConstantes.ExcludedElements, siteLanguage),
                Results = GestionWeb.GetWebWord(LanguageConstantes.ResultsCode, siteLanguage),
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
                SaveUnivers = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, siteLanguage),
                AddConcurrent = GestionWeb.GetWebWord(LanguageConstantes.AddConcurrentCode, siteLanguage),
                Concurrent = GestionWeb.GetWebWord(LanguageConstantes.Concurrent, siteLanguage),
                Referent = GestionWeb.GetWebWord(LanguageConstantes.Referent,siteLanguage),
                CantAddItemToConcurrentUnivers = GestionWeb.GetWebWord(LanguageConstantes.CantAddItemToConcurrentUnivers, siteLanguage)
            };
            return result;
        }
        private PresentationModel LoadPresentationBar(int siteLanguage, bool showCurrentSelection = true)
        {
            PresentationModel result = new PresentationModel
            {
                ModuleCode = LanguageConstantes.PresentAbsentCode,
                SiteLanguage = siteLanguage,
                ModuleDecriptionCode = LanguageConstantes.PresentAbsentDescriptionCode,
                ShowCurrentSelection = showCurrentSelection
            };
            return result;
        }
        #endregion
    }
}