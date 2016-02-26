using AutoMapper;
using Kantar.AdExpress.Service.Core.BusinessService;
using Domain=Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.Models;
using VM=Km.AdExpressClientWeb.Models.MediaSchedule;
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

namespace Km.AdExpressClientWeb.Controllers
{
    [Authorize]
    public class MediaScheduleController : Controller
    {
        private IMediaService _mediaService;
        private IWebSessionService _webSessionService;
        private IMediaSchedule _mediaSchedule;
        private IUniverseService _universService;
        private const string _controller = "MediaSchedule";
        #region CODES OF MARKET'S LABELS  
        private const long SaveUniversCode = 769;
        private const long LoadUniversCode = 770;
        private const long UserSavedUniversCode = 893;        
        private const long ExceptionMsg = 922;
        private const long ErrorMsgCode = 930;
        private const long NoSavedUniversCode = 930;
        private const long KeyWordLabelCode = 972;
        private const long Capacity = 1000;
        private const long ExcludeCode = 2269;
        private const long IncludeCode = 2270;
        private const long BranchLabelCode = 2272;
        private const long ElementLabelCode = 2278;
        private const long SecurityMsg = 2285;
        private const long OverLimitMsgCode = 2286;
        private const long KeyWordDescriptionCode = 2287;
       
        #endregion

        private string icon;
        public MediaScheduleController(IMediaService mediaService, IWebSessionService webSessionService, IMediaSchedule mediaSchedule, IUniverseService universService)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
            _mediaSchedule = mediaSchedule;
            _universService = universService;
        }
        public ActionResult Index()
        {
            #region Init
            var model = new VM.MarketViewModel();
            var claim = new ClaimsPrincipal(User.Identity);
            string webSessionId = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            #endregion
            #region Load Branches
            var result = _universService.GetBranches(webSessionId, TNS.Classification.Universe.Dimension.product, true);
            #endregion
            #region Load each label's text in the appropriate language
            model.Labels = LoadPageLabels(result.SiteLanguage);
            model.Branches = Mapper.Map<List<VM.UniversBranch>>(result.Branches);           
            #endregion
            
            var marketNode = new VM.MediaPlanNavigationNode { Position = 1 };
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
            var media = _mediaService.GetMedia(idWebSession);
            var _webSession = (WebSession)WebSession.Load(idWebSession);
            #region Hardcoded model data
            var model = new VM.MediaSelectionViewModel()
            {
                Multiple = true,
                Medias =media,
            //Medias = new List<Media>()
            //{
            //    new Media()
            //    {
            //        MediaEnum = Vehicles.names.cinema,
            //        Id= 1,

            //        Label = "Cinéma",
            //        Disabled = false
            //    },
            //    new Media()
            //    {
            //          MediaEnum = Vehicles.names.search,
            //        Id = 34,
            //        Label = "Search",
            //        Disabled = false
            //    },
            //    new Media()
            //    {
            //          MediaEnum = Vehicles.names.tv,
            //        Id = 2,
            //        Label = "Télévision",
            //        Disabled = true
            //    },
            //        new Media()
            //    {
            //              MediaEnum = Vehicles.names.evaliantMobile,
            //        Id = 4,
            //        Label = "Evaliant Mobile",
            //        Disabled = false
            //    },
            //            new Media()
            //    {
            //                MediaEnum = Vehicles.names.directMarketing,
            //        Id = 10,
            //        Label = "Courrier",
            //        Disabled = false
            //    }
            //    //  new Media()
            //    //{
            //    //    Id = 6,
            //    //    Label = "Nom 6",
            //    //    Disabled = false
            //    //},
            //    //      new Media()
            //    //{
            //    //    Id = 7,
            //    //    Label = "Nom 7",
            //    //    Disabled = true
            //    //}

            //},
            IdMediasCommon =
                    new List<int>()
                    {
                    { 1 },
                    { 2 },
                    { 4 },
                    { 6 }
                    }
            };

            foreach (var e in model.Medias)
            {
                e.icon = IconSelector.getIcon(e.MediaEnum);
            }
            model.Medias = model.Medias.OrderBy(ze => ze.Disabled).ToList();
            #endregion
            var mediaNode = new VM.MediaPlanNavigationNode { Position = 2 };
            model.NavigationBar = LoadNavBar(mediaNode.Position);
            model.ErrorMessage= new VM.ErrorMessage
            {
                EmptySelection= GestionWeb.GetWebWord(1052, _webSession.SiteLanguage),
                SearchErrorMessage = GestionWeb.GetWebWord(3011, _webSession.SiteLanguage),
                SocialErrorMessage = GestionWeb.GetWebWord(3030, _webSession.SiteLanguage),
                UnitErrorMessage = GestionWeb.GetWebWord(2541, _webSession.SiteLanguage)
            };
            
            return View(model);
        }

        public ActionResult PeriodSelection()
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession CustomerSession = (WebSession)WebSession.Load(idSession);
            CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = CustomerSession;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            int startYear = dateDAL.GetCalendarStartDate();
            int endYear = DateTime.Now.Year;

            PeriodViewModel periodModel = new PeriodViewModel();

            periodModel.SiteLanguage = CustomerSession.SiteLanguage;
            periodModel.StartYear = string.Format("{0}-01-01", startYear);
            periodModel.EndYear = string.Format("{0}-12-31", endYear);

            VM.MediaPlanNavigationNode periodeNode = new VM.MediaPlanNavigationNode { Position = 3 };
            var navBarModel = LoadNavBar(periodeNode.Position);

            PeriodSelectionViewModel model = new PeriodSelectionViewModel();
            model.PeriodViewModel = periodModel;
            model.NavigationBar = navBarModel;

            return View(model);
        }

        public JsonResult CalendarValidation(string selectedStartDate, string selectedEndDate)
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession CustomerSession = (WebSession)WebSession.Load(idSession);
            DateTime lastDayEnable = DateTime.Now;

            DateTime startDate = new DateTime(Convert.ToInt32(selectedStartDate.Substring(6, 4)), Convert.ToInt32(selectedStartDate.Substring(3, 2)), Convert.ToInt32(selectedStartDate.Substring(0, 2)));
            DateTime endDate = new DateTime(Convert.ToInt32(selectedEndDate.Substring(6, 4)), Convert.ToInt32(selectedEndDate.Substring(3, 2)), Convert.ToInt32(selectedEndDate.Substring(0, 2)));

            CustomerSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly;
            CustomerSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
            CustomerSession.PeriodBeginningDate = startDate.ToString("yyyyMMdd");
            CustomerSession.PeriodEndDate = endDate.ToString("yyyyMMdd");

            if (endDate < DateTime.Now || DateTime.Now < startDate)
                CustomerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodEndDate);
            else
                CustomerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(CustomerSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));

             CustomerSession.Save();

             UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
             string url = context.Action("Results", "MediaSchedule");

             JsonResult jsonModel = Json(new { RedirectUrl = url });

             return jsonModel;
        }

        public JsonResult SlidingDateValidation(int selectedPeriod, int selectedValue)
        {
            var cla = new ClaimsPrincipal(User.Identity);
            string idSession = cla.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            WebSession CustomerSession = (WebSession)WebSession.Load(idSession);
            globalCalendar.periodDisponibilityType periodCalendarDisponibilityType = globalCalendar.periodDisponibilityType.currentDay;
            globalCalendar.comparativePeriodType comparativePeriodCalendarType = globalCalendar.comparativePeriodType.dateToDate;

            CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.date];
            IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);
           
            if (selectedValue == 0) throw new Exception(GestionWeb.GetWebWord(885, CustomerSession.SiteLanguage));
            date.SetDate(ref CustomerSession, DateTime.Now, periodCalendarDisponibilityType, comparativePeriodCalendarType, selectedPeriod, selectedValue);

            CustomerSession.Save();

            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            string url = context.Action("Results", "MediaSchedule");

            JsonResult jsonModel = Json(new { RedirectUrl = url });

            return jsonModel;
        }

        public ActionResult Results()
        {
            var resultNode = new VM.MediaPlanNavigationNode { Position = 4 };
            var model = new VM.ResultsViewModel
            {
                NavigationBar = LoadNavBar(resultNode.Position)
            };            
            return View(model);
        }

        public JsonResult MediaScheduleResult()
        {
            var data = _mediaSchedule.GetMediaScheduleData("");

            return null;
        }

        public JsonResult SaveMediaSelection(List<long> selectedMedia, string nextStep)
        {
            string url = string.Empty;
            var response = new Domain.WebSessionResponse();
            if ( selectedMedia !=null)
            {
                var claim = new ClaimsPrincipal(User.Identity);
                string idWebSession = claim.Claims.Where(e => e.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
                response =_webSessionService.SaveMediaSelection(selectedMedia, idWebSession);
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
                var response = _webSessionService.SaveMarketSelection( idWebSession);
            
            UrlHelper context = new UrlHelper(this.ControllerContext.RequestContext);
            string url = context.Action(nextStep, _controller);
            JsonResult jsonModel = Json(new { RedirectUrl = url });
            return jsonModel;
        }
        #region Private methodes
        private List< VM.MediaPlanNavigationNode> LoadNavBar(int currentPosition)
        {
            var model = new List<VM.MediaPlanNavigationNode>();
            //TODO Update Navbar according to the country selection
            #region Hardcoded  nav Bar.
            var market = new VM.MediaPlanNavigationNode
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
            var media = new VM.MediaPlanNavigationNode
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
            var dates = new VM.MediaPlanNavigationNode
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
            var result = new VM.MediaPlanNavigationNode
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
            foreach( var nav in model)
            {
                nav.IsActive = (nav.Id > currentPosition) ? false : true;
            }
            #endregion
            return model;
        }

        private VM.Labels LoadPageLabels (int siteLanguage)
        {
            var result = new VM.Labels
            {
                KeyWordLabel = GestionWeb.GetWebWord(KeyWordLabelCode, siteLanguage),
                KeyWordDescription = GestionWeb.GetWebWord(KeyWordDescriptionCode, siteLanguage),
                ErrorMessage = GestionWeb.GetWebWord(ErrorMsgCode, siteLanguage),
                BranchLabel = GestionWeb.GetWebWord(BranchLabelCode, siteLanguage),
                NoSavedUnivers = GestionWeb.GetWebWord(NoSavedUniversCode, siteLanguage),
                UserSavedUniversLabel = GestionWeb.GetWebWord(UserSavedUniversCode, siteLanguage),
                Include = GestionWeb.GetWebWord(IncludeCode,siteLanguage),
                Exclude = GestionWeb.GetWebWord(ExcludeCode,siteLanguage),
                LoadUnivers = GestionWeb.GetWebWord(LoadUniversCode,siteLanguage),
                Save =GestionWeb.GetWebWord(SaveUniversCode,siteLanguage)
            };
            return result;
        }
        #endregion

    }
}