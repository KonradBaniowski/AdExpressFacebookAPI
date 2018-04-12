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
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.Insertions.Cells;
using CoreDomain = Kantar.AdExpress.Service.Core.Domain;
using System.Collections;
using KM.Framework.Constantes;
using Km.AdExpressClientWeb.Helpers;
using Kantar.AdExpress.Service.Core.Domain.BusinessService;

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize(Roles = Role.ADEXPRESS)]
    public class MediaScheduleController : Controller
    {
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IMediaScheduleService _mediaSchedule;
        private IUniverseService _universService;
        private IPeriodService _periodService;
        private IOptionService _optionService;
        private ISubPeriodService _subPeriodService;
        private const string _controller = "MediaSchedule";
        private const int MarketPageId = 2;
        private const int MediaPageId = 6;
        private const string CalendarFormatDays = "DD/MM/YYYY";
        private const string CalendarFormatMonths = "MM/YYYY";
        private const string CALENDARLANGUAGEEN = "En";
        private const string CALENDARLANGUAGEFR = "fr";
        private const string CALENDARLANGUAGEFI = "fi";
        private const string CALENDARLANGUAGESK = "sk";
        private int _siteLanguage = WebApplicationParameters.DefaultLanguage;


        private string icon;
        public MediaScheduleController(IMediaService mediaService, IWebSessionService webSessionService, IMediaScheduleService mediaSchedule, IUniverseService universService, IPeriodService periodService, IOptionService optionService, ISubPeriodService subPeriodService)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _mediaSchedule = mediaSchedule;
            _universService = universService;
            _periodService = periodService;
            _optionService = optionService;
            _subPeriodService = subPeriodService;
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
            var result = _universService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.product, this.HttpContext, true);
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
            _siteLanguage = result.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            ViewBag.SiteLanguage = _siteLanguage;
            var marketNode = new NavigationNode { Position = 1 };
            var navigationHelper = new Helpers.PageHelper();
           
            model.NavigationBar = navigationHelper.LoadNavBar(webSessionId, _controller, _siteLanguage, 1);
        
            return View(model);
        }

        public ActionResult MediaSelection()
        {
            //var model = new MediaSelectionViewModel();

            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _mediaService.GetMedia(webSessionId, this.HttpContext);

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
            var pageHelper = new Helpers.PageHelper();
            model.Presentation = pageHelper.LoadPresentationBar(result.SiteLanguage, result.ControllerDetails);
            foreach (var e in model.Medias)
            {
                e.icon = IconSelector.getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
            _siteLanguage = result.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            ViewBag.SiteLanguage = _siteLanguage;
            var mediaNode = new NavigationNode { Position = 2 };
            var navigationHelper = new Helpers.PageHelper();
            model.NavigationBar = navigationHelper.LoadNavBar(webSessionId, _controller, _siteLanguage, 2);
            model.ErrorMessage = new  Models.Shared.ErrorMessage
            {
                EmptySelection = GestionWeb.GetWebWord(1052, result.SiteLanguage),
                SearchErrorMessage = GestionWeb.GetWebWord(3011, result.SiteLanguage),
                SocialErrorMessage = GestionWeb.GetWebWord(3030, result.SiteLanguage),
                UnitErrorMessage = GestionWeb.GetWebWord(2541, result.SiteLanguage)
            };
            model.Labels = LoadPageLabels(result.SiteLanguage);
            var response = _universService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.media, this.HttpContext, true);
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

            var result = _periodService.GetPeriod(idSession, this.HttpContext);

            PeriodViewModel periodModel = new PeriodViewModel();
            periodModel.SlidingYearsNb = WebApplicationParameters.DataNumberOfYear;
            periodModel.IsSlidingYearsNbVisible = true;
            periodModel.SiteLanguage = result.SiteLanguage;
            periodModel.StartYear = string.Format("{0}-01-01", result.StartYear);
            periodModel.EndYear = string.Format("{0}-12-31", result.EndYear);
            switch (result.ControllerDetails.ModuleId)
            {
                case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    periodModel.CalendarFormat = CalendarFormatDays;
                    break;
                case WebConstantes.Module.Name.INDICATEUR:
                case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                    periodModel.CalendarFormat = CalendarFormatMonths;
                    break;
                default:
                    periodModel.CalendarFormat = CalendarFormatDays;
                    break;

            }
            switch (result.SiteLanguage)
            {
                case TNS.AdExpress.Constantes.DB.Language.FRENCH:
                    periodModel.LanguageName = CALENDARLANGUAGEFR;
                    break;
                case TNS.AdExpress.Constantes.DB.Language.ENGLISH:
                    periodModel.LanguageName = CALENDARLANGUAGEEN;
                    break;
                case TNS.AdExpress.Constantes.DB.Language.FINNOIS:
                    periodModel.LanguageName = CALENDARLANGUAGEFI;
                    break;
                case TNS.AdExpress.Constantes.DB.Language.SLOVAKIA:
                    periodModel.LanguageName = CALENDARLANGUAGESK;
                    break;
                default:
                    periodModel.LanguageName = CALENDARLANGUAGEEN;
                    break;
            }

            _siteLanguage = result.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            ViewBag.SiteLanguage = _siteLanguage;
            NavigationNode periodeNode = new NavigationNode { Position = 3 };
            var navigationHelper = new Helpers.PageHelper();
            var navBarModel = navigationHelper.LoadNavBar(idSession, _controller, _siteLanguage, 3);
        
            PeriodSelectionViewModel model = new PeriodSelectionViewModel();
            model.PeriodViewModel = periodModel;
            model.NavigationBar = navBarModel;
            var pageHelper = new Helpers.PageHelper();
            model.Presentation = pageHelper.LoadPresentationBar(result.SiteLanguage, result.ControllerDetails);

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
            PeriodSaveRequest request = new PeriodSaveRequest(idSession, selectedStartDate, selectedEndDate, nextStep);
            PeriodResponse response = _periodService.CalendarValidation(request, this.HttpContext);
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
            int studyId = 5;//TO BE VERIFIED
            PeriodSaveRequest request = new PeriodSaveRequest(idSession, selectedPeriod, selectedValue, nextStep, studyId);
            var response = _periodService.SlidingDateValidation(request, this.HttpContext);
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
            var result = _webSessionService.GetWebSession(idSession);
            _siteLanguage = result.WebSession.SiteLanguage;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(_siteLanguage);
            ViewBag.SiteLanguage = _siteLanguage;
            var resultNode = new NavigationNode { Position = 4 };
            var pageHelper = new Helpers.PageHelper();
            var model = new Models.Shared.ResultsViewModel
            {
                NavigationBar = pageHelper.LoadNavBar(idSession, _controller, _siteLanguage, 4),
                Presentation = pageHelper.LoadPresentationBar(result.WebSession.SiteLanguage, result.ControllerDetails),
                Labels = pageHelper.LoadPageLabels(result.WebSession.SiteLanguage, _controller),
                IsAlertVisible = PageHelper.IsAlertVisible(WebApplicationParameters.CountryCode, idSession),
                ExportTypeViewModels = PageHelper.GetExportTypes(WebApplicationParameters.CountryCode, WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA, _siteLanguage)
            };

            return View(model);
        }

        public JsonResult MediaScheduleResult(string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            GridResult gridResult;
            JsonResult jsonModel;

            if (string.IsNullOrEmpty(zoomDate))
                gridResult = _mediaSchedule.GetGridResult(idWebSession, "", this.HttpContext);
            else
                gridResult = _mediaSchedule.GetGridResult(idWebSession, zoomDate, this.HttpContext);

            if (!gridResult.HasData)
                return null;

            if (gridResult.HasMoreThanMaxRowsAllowed)
            {
                var response = new { hasMoreThanMaxRowsAllowed = true };
                jsonModel = Json(response, JsonRequestBehavior.AllowGet);
                jsonModel.MaxJsonLength = Int32.MaxValue;

                return jsonModel;
            }

            string jsonData = JsonConvert.SerializeObject(gridResult.Data);

            var obj = new { datagrid = jsonData, columns = gridResult.Columns, schema = gridResult.Schema, columnsfixed = gridResult.ColumnsFixed, columnsNotAllowedSorting = gridResult.ColumnsNotAllowedSorting, needfixedcolumns = gridResult.NeedFixedColumns, hasMSCreatives = gridResult.HasMSCreatives, unit = gridResult.Unit };
            jsonModel = Json(obj, JsonRequestBehavior.AllowGet);
            jsonModel.MaxJsonLength = Int32.MaxValue;

            return jsonModel;
        }

        public ActionResult MSCreativesResult(string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            CoreDomain.MSCreatives creatives = _mediaSchedule.GetMSCreatives(idWebSession, zoomDate, this.HttpContext);

            VM.MSCreativesViewModel msCreativesViewModel = new VM.MSCreativesViewModel { MSCreatives = creatives };

            var pageHelper = new Helpers.PageHelper();
            msCreativesViewModel.Labels = pageHelper.LoadPageLabels(creatives.SiteLanguage);

            return PartialView("_MSCreativesResult", msCreativesViewModel);
        }

        public void SetMSCreatives(Int64[] slogans)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            ArrayList sloganList = new ArrayList();

            if (slogans != null)
            {
                foreach (var slogan in slogans)
                    sloganList.Add(slogan);
            }
            else
                sloganList = null;

            _mediaSchedule.SetMSCreatives(idWebSession, sloganList);
        }

        public ActionResult SubPeriodSelection(string zoomDate)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var subPeriod = _subPeriodService.GetSubPeriod(idWebSession, zoomDate, this.HttpContext);

            return PartialView("_SubPeriodSelection", subPeriod);
        }

        public ActionResult ResultOptions()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            Options options = _optionService.GetOptions(idWebSession, this.HttpContext);
            return PartialView("_ResultOptions", options);
        }

        public void SetResultOptions(UserFilter userFilter)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            _optionService.SetOptions(idWebSession, userFilter, this.HttpContext);
        }

        public JsonResult SaveCustomDetailLevels(string levels, string type)
        {
            var claim = new ClaimsPrincipal(User.Identity);
            JsonResult jsonModel = new JsonResult();
            string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            Domain.SaveLevelsResponse response = _optionService.SaveCustomDetailLevels(idWebSession, levels, type, this.HttpContext);
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
                List<Domain.Tree> trees = (mediaSupport !=null)? mediaSupport: new List<Domain.Tree>();
                trees = trees.Where(p => p.UniversLevels.Where(x => x.UniversItems != null).Any()).ToList();
                var claim = new ClaimsPrincipal(User.Identity);
                string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                Domain.SaveMediaSelectionRequest request = new Domain.SaveMediaSelectionRequest(selectedMedia, idWebSession, trees, Dimension.media, Security.full, false, nextStep);
                response = _webSessionService.SaveMediaSelection(request, this.HttpContext);
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
        
        public JsonResult GetMediaSupport()
        {
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var result = _universService.GetBranches(webSessionId, Dimension.media, this.HttpContext, true);
            return Json(result);
        }

        #region Private methodes
     

        private Labels LoadPageLabels(int siteLanguage)
        {
            var result = new Labels
            {
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
                ErrorMessageSameLevel = GestionWeb.GetWebWord(LanguageConstantes.SameLevel, siteLanguage),
                ErrorMininumInclude = GestionWeb.GetWebWord(LanguageConstantes.MininumInclude, siteLanguage),
                ErrorItemExceeded = GestionWeb.GetWebWord(LanguageConstantes.ItemExceeded, siteLanguage),
                ErrorMediaSelected = GestionWeb.GetWebWord(LanguageConstantes.MediaSelected, siteLanguage),
                ErrorNoSupport = GestionWeb.GetWebWord(LanguageConstantes.NoSupport, siteLanguage),
                DeleteAll = GestionWeb.GetWebWord(LanguageConstantes.DeleteAllcode, siteLanguage),
                ErrorOnlyOneItemAllowed = GestionWeb.GetWebWord(LanguageConstantes.ErrorOnlyOneItemAllowed, siteLanguage),
                ErrorOverLimit = GestionWeb.GetWebWord(LanguageConstantes.ErrorOverLimit, siteLanguage),
                SaveUnivers = GestionWeb.GetWebWord(LanguageConstantes.SaveUniversCode,siteLanguage),
                UnityError =GestionWeb.GetWebWord(LanguageConstantes.UnityError,siteLanguage),
                SelectMedia = GestionWeb.GetWebWord(LanguageConstantes.SelectMedia, siteLanguage),
                PreSelection = GestionWeb.GetWebWord(LanguageConstantes.PreSelection, siteLanguage),
                Results = GestionWeb.GetWebWord(LanguageConstantes.Results, siteLanguage),
                Save = GestionWeb.GetWebWord(LanguageConstantes.Save, siteLanguage),
                CreateAlert= GestionWeb.GetWebWord(LanguageConstantes.CreateAlert, siteLanguage),
                ExportFormattedResult = GestionWeb.GetWebWord(LanguageConstantes.ExportFormattedResult, siteLanguage),
                ExportResultWithValue = GestionWeb.GetWebWord(LanguageConstantes.ExportResultWithValue, siteLanguage),
                ExportGrossResult = GestionWeb.GetWebWord(LanguageConstantes.ExportGrossResult, siteLanguage),
                ExportPdfResult = GestionWeb.GetWebWord(LanguageConstantes.ExportPdfResult, siteLanguage),
                ExportPptResult = GestionWeb.GetWebWord(LanguageConstantes.ExportPptResult, siteLanguage),
                Search = GestionWeb.GetWebWord(LanguageConstantes.Search, siteLanguage),
                Timeout = GestionWeb.GetWebWord(LanguageConstantes.Timeout, siteLanguage),
                TimeoutBis = GestionWeb.GetWebWord(LanguageConstantes.TimeoutBis, siteLanguage),
                MaxAllowedRows = GestionWeb.GetWebWord(LanguageConstantes.MaxAllowedRows, siteLanguage),
                MaxAllowedRowsBis = GestionWeb.GetWebWord(LanguageConstantes.MaxAllowedRowsBis, siteLanguage),
                MaxAllowedRowsRefine = GestionWeb.GetWebWord(LanguageConstantes.MaxAllowedRowsRefine, siteLanguage)
            };

            if (WebApplicationParameters.CountryCode.Equals(CountryCode.FINLAND)
                || WebApplicationParameters.CountryCode.Equals(CountryCode.SLOVAKIA)
                || WebApplicationParameters.CountryCode.Equals(CountryCode.POLAND))
                result.PreSelection = GestionWeb.GetWebWord(LanguageConstantes.PreSelectionWithoutEvaliant, siteLanguage);

            return result;
        }
        #endregion

    }
}