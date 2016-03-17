using AutoMapper;
using Kantar.AdExpress.Service.Core.BusinessService;
using Domain = Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.Models;
using VM = Km.AdExpressClientWeb.Models.MediaSchedule;
using KM.AdExpress.Framework.MediaSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Date;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpressI.MediaSchedule;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain;
using Newtonsoft.Json;
using Kantar.AdExpress.Service.Core;
using TNS.Classification.Universe;
using Km.AdExpressClientWeb.Models.Shared;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using TNS.AdExpress.Domain.Results;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class MediaScheduleController : Controller
    {
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IMediaScheduleService _mediaSchedule;
        private IUniverseService _universService;
        private IPeriodService _periodService;
        private IOptionService _optionService;
        private const string _controller = "MediaSchedule";
        private const int MarketPageId = 2;
        private const int MediaPageId = 6;

        private string icon;
        public MediaScheduleController(IMediaService mediaService, IWebSessionService webSessionService, IMediaScheduleService mediaSchedule, IUniverseService universService, IPeriodService periodService, IOptionService optionService)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _mediaSchedule = mediaSchedule;
            _universService = universService;
            _periodService = periodService;
            _optionService = optionService;
        }
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
            var result = _universService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.product,MarketPageId, true);
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
                Multiple = true,
                Medias = result.Media,
                IdMediasCommon = result.MediaCommon,
                Branches = new List<Models.Shared.UniversBranch>(),
                Trees = new List<Models.Shared.Tree>(),
                Dimension = Dimension.media
            };
            model.Presentation = LoadPresentationBar(result.SiteLanguage);
            foreach (var e in model.Medias)
            {
                e.icon = IconSelector.getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
            var mediaNode = new NavigationNode { Position = 2 };
            model.NavigationBar = LoadNavBar(mediaNode.Position);
            model.ErrorMessage = new  Models.Shared.ErrorMessage
            {
                EmptySelection = GestionWeb.GetWebWord(1052, result.SiteLanguage),
                SearchErrorMessage = GestionWeb.GetWebWord(3011, result.SiteLanguage),
                SocialErrorMessage = GestionWeb.GetWebWord(3030, result.SiteLanguage),
                UnitErrorMessage = GestionWeb.GetWebWord(2541, result.SiteLanguage)
            };
            model.Labels = LoadPageLabels(result.SiteLanguage);
            var response = _universService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.product, MediaPageId, true);
            model.Branches = Mapper.Map<List<UniversBranch>>(response.Branches);
            foreach (var item in response.Trees)
            {
                Models.Shared.Tree tree = new Models.Shared.Tree
                {
                    Id = item.Id,
                    LabelId = item.LabelId,
                    AccessType = item.AccessType,
                    UniversLevels = Mapper.Map<List< Models.Shared.UniversLevel>>(item.UniversLevels)
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
            var navBarModel = LoadNavBar(periodeNode.Position);

            PeriodSelectionViewModel model = new PeriodSelectionViewModel();
            model.PeriodViewModel = periodModel;
            model.NavigationBar = navBarModel;
            model.Presentation = LoadPresentationBar(result.SiteLanguage);

            return View(model);
        }

        public JsonResult CalendarValidation(string selectedStartDate, string selectedEndDate)
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            string url = string.Empty;
            var response = _periodService.CalendarValidation(idSession, selectedStartDate, selectedEndDate);

            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            if (response.Success)
                url = context.Action("Results", _controller);

            JsonResult jsonModel = Json(new { RedirectUrl = url });

            return jsonModel;
        }

        public JsonResult SlidingDateValidation(int selectedPeriod, int selectedValue)
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            string url = string.Empty;
            var response = _periodService.SlidingDateValidation(idSession, selectedPeriod, selectedValue);
            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            if (response.Success)
                url = context.Action("Results", _controller);

            JsonResult jsonModel = Json(new { RedirectUrl = url });

            return jsonModel;
        }

        public ActionResult Results()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession CustomerSession = (WebSession)WebSession.Load(idSession);
            var resultNode = new NavigationNode { Position = 4 };
            var model = new VM.ResultsViewModel
            {
                NavigationBar = LoadNavBar(resultNode.Position),
                Presentation = LoadPresentationBar(CustomerSession.SiteLanguage)
            };

            return View(model);
        }

        public JsonResult MediaScheduleResult(string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            GridResult gridResult;

            if (string.IsNullOrEmpty(zoomDate))
                gridResult = _mediaSchedule.GetGridResult(idWebSession, "");
            else
                gridResult = _mediaSchedule.GetGridResult(idWebSession, zoomDate);

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
            if (selectedMedia != null)
            {
                List<Domain.Tree> trees = (mediaSupport !=null)? mediaSupport: new List<Domain.Tree>();
                var claim = new ClaimsPrincipal(User.Identity);
                string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                response = _webSessionService.SaveMediaSelection(selectedMedia, idWebSession, trees);
            }
            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            if (response.Success)
                url = context.Action(nextStep, _controller);
            JsonResult jsonModel = Json(new { RedirectUrl = url });
            return jsonModel;
        }
        
        public ActionResult LoadUserUniversGroups()
        {
            bool showUserSavedGroups = true;
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            UserUniversGroupsModel result = new UserUniversGroupsModel
            {
                LoadUniversCode = LanguageConstantes.LoadUniversCode,
                ModuleCode = LanguageConstantes.MediaScheduleCode,
                SaveUniversCode = LanguageConstantes.SaveUniversCode,
                UserUniversGroups = new List<UserUniversGroup>(),
                UserUniversCode = LanguageConstantes.UserUniversCode,
                ErrorMsgCode = LanguageConstantes.ErrorMsgCode,
                ModuleDecriptionCode = LanguageConstantes.MediaScheduleDescriptionCode,
                ShowUserSavedGroups = showUserSavedGroups
            };
            if (showUserSavedGroups)
            {
                var data = _universService.GetUserSavedUniversGroups(webSessionId, TNS.Classification.Universe.Dimension.product, MarketPageId);
                result.SiteLanguage = data.SiteLanguage;
                result.UserUniversGroups = Mapper.Map<List<UserUniversGroup>>(data.UniversGroups);
                foreach (var group in result.UserUniversGroups)
                {
                    int count = group.Count;
                    group.FirstColumnSize = (count % 2 == 0) ? count / 2 : (count / 2) + 1;
                    group.SecondeColumnSize = count - group.FirstColumnSize;
                }
            }
            return PartialView("UserUniversGroupsContent", result);
        }

        public JsonResult GetUserUnivers(int id)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _universService.GetTreesByUserUnivers(id, idWebSession, Dimension.product, MarketPageId);
            return Json(result);
        }

        [HttpGet]
        public PartialViewResult SaveUserUnivers()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var data = _universService.GetUserUniversGroups(webSessionId, TNS.Classification.Universe.Dimension.product,MarketPageId);
            SaveUserUniversViewModel model = new SaveUserUniversViewModel
            {
                Title = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode, data.SiteLanguage),
                SelectUniversGroup = GestionWeb.GetWebWord(LanguageConstantes.SelectUniversGroup, data.SiteLanguage),
                SelectUnivers = GestionWeb.GetWebWord(LanguageConstantes.SelectUnivers, data.SiteLanguage),
                UniversLabel = GestionWeb.GetWebWord(LanguageConstantes.UniversLabel, data.SiteLanguage),
                UserGroups = new List<SelectListItem>(),
                UserUnivers = new List<SelectListItem>()
            };
            if (data.UniversGroups.Any())
            {
                var items = data.UniversGroups.Select(p => new SelectListItem()
                {
                    Value = p.Id.ToString(),
                    Text = p.Description
                }).ToList();
                items.FirstOrDefault().Selected = true;
                model.UserGroups = items;
                if (data.UniversGroups.FirstOrDefault().UserUnivers.Any())
                {
                    model.UserUnivers = data.UniversGroups.FirstOrDefault().UserUnivers.Select(m => new SelectListItem()
                    {
                        Value = m.Id.ToString(),
                        Text = m.Description
                    }).ToList();
                    model.UserUnivers.FirstOrDefault().Selected = true;
                }
            }
            return PartialView(model);
        }
        [HttpGet]
        public JsonResult GetUniversByGroup(string id)
        {
            List<SelectListItem> univers = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(id))
            {
                long groupId = long.Parse(id);
                var claim = new ClaimsPrincipal(User.Identity);
                string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                var data = _universService.GetUserUniversGroups(webSessionId, TNS.Classification.Universe.Dimension.product,MarketPageId, groupId);
                univers = data.UniversGroups.FirstOrDefault().UserUnivers.Select(m => new SelectListItem()
                {
                    Value = m.Id.ToString(),
                    Text = m.Description
                }).ToList();
            }
            return Json(new SelectList(univers, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string SaveUserUnivers(List<Tree> trees, string groupId, string universId, string name)
        {
            string error = "";
            if (trees.Any() && trees.Where(p => p.UniversLevels != null).Any() && !String.IsNullOrEmpty(groupId) && (!String.IsNullOrEmpty(universId) || !String.IsNullOrEmpty(name)))
            {
                var claim = new ClaimsPrincipal(User.Identity);
                string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                List<Tree> validTrees = trees.Where(p => p.UniversLevels.Where(x => x.UniversItems != null).Any()).ToList();
                var data = Mapper.Map<List<Domain.Tree>>(validTrees);
                Domain.UniversGroupSaveRequest request = new Domain.UniversGroupSaveRequest
                {
                    Dimension = Dimension.product,
                    Name = name,
                    UniversGroupId = long.Parse(groupId),
                    UserUniversId = long.Parse(universId),
                    WebSessionId = webSessionId,
                    Trees = Mapper.Map<List<Domain.Tree>>(validTrees),
                    IdUniverseClientDescription = 16
                };
                var result = _universService.SaveUserUnivers(request);
                error = result.ErrorMessage;
            }
            else
            {
                error = "Invalid Selection";
            }
            return error;
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
                var result = _webSessionService.SaveMarketSelection(webSessionId, data,Dimension.product,Security.full);
                if (result.Success)
                {
                    UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
                    var redirectUrl = context.Action(nextStep, _controller);
                    return Json(new { ErrorMessage = errorMessage, RedirectUrl= redirectUrl });
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
            var result = _universService.GetBranches(webSessionId, Dimension.media, MediaPageId, true);
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
                KeyWordLabel = GestionWeb.GetWebWord(LanguageConstantes.KeyWordLabelCode, siteLanguage),
                KeyWordDescription = GestionWeb.GetWebWord(LanguageConstantes.KeyWordDescriptionCode, siteLanguage),
                ErrorMessage = GestionWeb.GetWebWord(LanguageConstantes.ErrorMsgCode, siteLanguage),
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
                Refine = GestionWeb.GetWebWord(LanguageConstantes.RefineCode, siteLanguage)
            };
            return result;
        }
        private PresentationModel LoadPresentationBar(int siteLanguage, bool showCurrentSelection = true)
        {
            PresentationModel result = new PresentationModel
            {
                ModuleCode = LanguageConstantes.MediaScheduleCode,
                SiteLanguage = siteLanguage,
                ModuleDecriptionCode = LanguageConstantes.MediaScheduleDescriptionCode,
                ShowCurrentSelection = showCurrentSelection
            };
            return result;
        }

        #endregion

    }
}