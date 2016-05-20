using AutoMapper;
using Kantar.AdExpress.Service.Core;
using Kantar.AdExpress.Service.Core.BusinessService;
using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.Models;
using Km.AdExpressClientWeb.Models.Shared;
using KM.AdExpress.Framework.MediaSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.Classification.Universe;
using Domain = Kantar.AdExpress.Service.Core.Domain;

namespace Km.AdExpressClientWeb.Controllers
{
    public class SelectionController : Controller
    {
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IUniverseService _universeService;
        private IPeriodService _periodService;
        private const string MARKET = "Market";
        private const string MEDIA = "MediaSelection";
        private const string SELECTION = "Selection";
        private const string PERIOD = "PeriodSelection";
        private const string ERROR = "Invalid Selection";
        private const string CalendarFormatDays = "DD/MM/YYYY";
        private const string CalendarFormatMonths = "MM/YYYY";
        public SelectionController (IMediaService mediaService, IWebSessionService webSessionService, IUniverseService universeService, IPeriodService periodService)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _universeService = universeService;
            _periodService = periodService;
        }

        public ActionResult Market()
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
            var result = _universeService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.product, true);
            model.CurrentModule = result.ControllerDetails.ModuleId;
            #endregion

            #region Load each label's text in the appropriate language
            var helper = new Helpers.PageHelper();
            model.Labels = helper.LoadPageLabels(result.SiteLanguage, result.ControllerDetails.Name);
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
            model.Presentation = helper.LoadPresentationBar(result.SiteLanguage, result.ControllerDetails.ControllerCode);
            model.UniversGroups = new UserUniversGroupsModel
            {
                ShowUserSavedGroups = true,
                UserUniversGroups = new List<UserUniversGroup>(),
                UserUniversCode = LanguageConstantes.UserUniversCode,
                SiteLanguage = result.SiteLanguage
            };
            #endregion
            //_siteLanguage = result.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(result.SiteLanguage);
            var marketNode = new NavigationNode { Position = 1 };
            model.NavigationBar = helper.LoadNavBar(webSessionId, result.ControllerDetails.Name, result.SiteLanguage, 1);
            return View(model);
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
                var result = _webSessionService.SaveMarketSelection(webSessionId, data, Dimension.product, Security.full, true);
                if (result.Success)
                {
                    var controller = (nextStep==MARKET|| nextStep==MEDIA || nextStep == PERIOD)? SELECTION:result.ControllerDetails.Name;
                    UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
                    var redirectUrl = context.Action(nextStep, controller);
                    return Json(new { ErrorMessage = errorMessage, RedirectUrl = redirectUrl });
                }
                else
                    errorMessage = result.ErrorMessage;
            }
            else
            {
                errorMessage = ERROR;
            }
            return Json(new { ErrorMessage = errorMessage });
        }

        public ActionResult MediaSelection()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _mediaService.GetMedia(webSessionId);

            #region model data
            var model = new MediaSelectionViewModel()
            {
                Multiple = true,
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
            var helper = new Helpers.PageHelper();
            model.Presentation = helper.LoadPresentationBar(result.SiteLanguage, result.ControllerDetails.ControllerCode);
            foreach (var e in model.Medias)
            {
                e.icon = IconSelector.getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(p => p.Disabled).ToList();
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(result.SiteLanguage);
            var mediaNode = new NavigationNode { Position = 2 };
            var navigationHelper = new Helpers.PageHelper();
            model.NavigationBar = navigationHelper.LoadNavBar(webSessionId, result.ControllerDetails.Name, result.SiteLanguage, 2);
            model.ErrorMessage = new Models.Shared.ErrorMessage
            {
                EmptySelection = GestionWeb.GetWebWord(1052, result.SiteLanguage),
                SearchErrorMessage = GestionWeb.GetWebWord(3011, result.SiteLanguage),
                SocialErrorMessage = GestionWeb.GetWebWord(3030, result.SiteLanguage),
                UnitErrorMessage = GestionWeb.GetWebWord(2541, result.SiteLanguage)
            };
            model.Labels = helper.LoadPageLabels(result.SiteLanguage, result.ControllerDetails.Name);
            var response = _universeService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.media, true);
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
                response = _webSessionService.SaveMediaSelection(selectedMedia, idWebSession, trees, Dimension.media, Security.full, false);
                UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
                if (response.Success)
                {
                    var controller = (nextStep == MARKET || nextStep == MEDIA || nextStep == PERIOD) ? SELECTION : response.ControllerDetails.Name;
                    url = context.Action(nextStep, controller);
                    jsonModel = Json(new { RedirectUrl = url, ErrorMessage = errorMsg });
                }
                else
                {
                    jsonModel = Json(new { response.ErrorMessage });
                }
            }
            return jsonModel;
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
            model.Presentation = navigationHelper.LoadPresentationBar(result.SiteLanguage,result.ControllerDetails.ControllerCode);
            model.ErrorMessage = new Models.Shared.ErrorMessage
            {
                EmptySelection = GestionWeb.GetWebWord(885, result.SiteLanguage),
                PeriodErrorMessage = GestionWeb.GetWebWord(1855, result.SiteLanguage)
            };

            return View(model);
        }

    }
}