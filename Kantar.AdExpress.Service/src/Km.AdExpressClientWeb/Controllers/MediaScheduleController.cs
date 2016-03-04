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
        private const string _controller = "MediaSchedule";

        private string icon;
        public MediaScheduleController(IMediaService mediaService, IWebSessionService webSessionService, IMediaScheduleService mediaSchedule, IUniverseService universService, IPeriodService periodService)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _mediaSchedule = mediaSchedule;
            _universService = universService;
            _periodService = periodService;
        }
        public ActionResult Index()
        {
            #region Init
            var model = new VM.MarketViewModel
            {
                Trees = new List<VM.Tree>(),
                Branches = new List<VM.UniversBranch>(),
                UniversGroups = new VM.UserUniversGroupsModel()
            };
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            #endregion
            #region Load Branches
            var result = _universService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.product, true);
            #endregion
            #region Load each label's text in the appropriate language
            model.Labels = LoadPageLabels(result.SiteLanguage);
            model.Branches = Mapper.Map<List<VM.UniversBranch>>(result.Branches);
            foreach ( var item in result.Trees)
            {
                VM.Tree tree = new VM.Tree
                {
                    Id = item.Id,
                    LabelId = item.LabelId,
                    AccessType = item.AccessType,
                    UniversLevels = Mapper.Map<List<VM.UniversLevel>>(item.UniversLevels)
                };
                tree.Label = (tree.AccessType == TNS.Classification.Universe.AccessType.includes) ? model.Labels.IncludedElements : model.Labels.ExcludedElements;
                model.Trees.Add(tree);
            }            
            #endregion
            #region Presentation
            model.Presentation = LoadPresentationBar(result.SiteLanguage);
            model.UniversGroups = new VM.UserUniversGroupsModel
            {
                ShowUserSavedGroups = true,
                UserUniversGroups = new List<VM.UserUniversGroup>(),
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
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _mediaService.GetMedia(idWebSession);

            #region model data
            //var idMediasCommon = Array.ConvertAll(Lists.GetIdList(GroupList.ID.media, GroupList.Type.mediaInSelectAll).Split(','), Convert.ToInt32).ToList();
            var model = new VM.MediaSelectionViewModel()
            {
                Multiple = true,
                Medias = result.Media,
                IdMediasCommon = result.MediaCommon
            };
            model.Presentation = LoadPresentationBar(result.SiteLanguage);
            foreach (var e in model.Medias)
            {
                e.icon = IconSelector.getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
            var mediaNode = new NavigationNode { Position = 2 };
            model.NavigationBar = LoadNavBar(mediaNode.Position);
            model.ErrorMessage = new VM.ErrorMessage
            {
                EmptySelection = GestionWeb.GetWebWord(1052, result.SiteLanguage),
                SearchErrorMessage = GestionWeb.GetWebWord(3011, result.SiteLanguage),
                SocialErrorMessage = GestionWeb.GetWebWord(3030, result.SiteLanguage),
                UnitErrorMessage = GestionWeb.GetWebWord(2541, result.SiteLanguage)
            };
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
                 url = context.Action("Results", "MediaSchedule");

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
                 url = context.Action("Results", "MediaSchedule");

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

        public JsonResult MediaScheduleResult(string selectedPeriodType)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var gridResult = _mediaSchedule.GetGridResult(idWebSession, selectedPeriodType);

            string jsonData = JsonConvert.SerializeObject(gridResult.Data);

            JsonResult jsonModel = Json(new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, needfixedcolumns = gridResult.NeedFixedColumns }, JsonRequestBehavior.AllowGet);

            return jsonModel;
        }

        public JsonResult SaveMediaSelection(List<long> selectedMedia, string nextStep)
        {
            string url = string.Empty;
            var response = new Domain.WebSessionResponse();
            if (selectedMedia != null)
            {
                var claim = new ClaimsPrincipal(User.Identity);
                string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                response = _webSessionService.SaveMediaSelection(selectedMedia, idWebSession);
            }
            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            if (response.Success)
                url = context.Action(nextStep, _controller);
            JsonResult jsonModel = Json(new { RedirectUrl = url });
            return jsonModel;
        }

        public JsonResult SaveMarketSelection(string nextStep)
        {

            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var response = _webSessionService.SaveMarketSelection(idWebSession);

            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            string url = context.Action(nextStep, _controller);
            JsonResult jsonModel = Json(new { RedirectUrl = url });
            return jsonModel;
        }
        public ActionResult LoadUserUniversGroups()
        {
            bool showUserSavedGroups = true;
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            VM.UserUniversGroupsModel result = new VM.UserUniversGroupsModel
            {
                LoadUniversCode = LanguageConstantes.LoadUniversCode,
                ModuleCode = LanguageConstantes.MediaScheduleCode,
                SaveUniversCode = LanguageConstantes.SaveUniversCode,
                UserUniversGroups = new List<VM.UserUniversGroup>(),
                UserUniversCode = LanguageConstantes.UserUniversCode,
                ErrorMsgCode = LanguageConstantes.ErrorMsgCode,
                ModuleDecriptionCode = LanguageConstantes.MediaScheduleDescriptionCode,
                ShowUserSavedGroups = showUserSavedGroups
            };
            if (showUserSavedGroups)
            {
                var data = _universService.GetUserSavedUniversGroups(webSessionId, TNS.Classification.Universe.Dimension.product);
                result.SiteLanguage = data.SiteLanguage;
                result.UserUniversGroups = Mapper.Map<List<VM.UserUniversGroup>>(data.UniversGroups);
                foreach (var group in result.UserUniversGroups)
                {
                    int count = group.Count;
                    group.FirstColumnSize = (count % 2 == 0) ? count / 2 : (count / 2) + 1;
                    group.SecondeColumnSize = count - group.FirstColumnSize;
                }
            }
            return PartialView("UserUniversGroupsContent", result);
        }

        public ActionResult GetUserUnivers(int id)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _universService.GetTreesByUserUnivers(id, idWebSession,Dimension.product);
            return View();
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
                ExcludedElements=GestionWeb.GetWebWord(LanguageConstantes.ExcludedElements,siteLanguage)
            };
            return result;
        }
        private Models.MediaSchedule.PresentationModel LoadPresentationBar(int siteLanguage, bool showCurrentSelection = true)
        {
            Models.MediaSchedule.PresentationModel result = new Models.MediaSchedule.PresentationModel
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