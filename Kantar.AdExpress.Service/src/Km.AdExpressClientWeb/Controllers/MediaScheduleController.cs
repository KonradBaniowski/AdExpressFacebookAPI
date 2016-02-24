using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.Models;
using Km.AdExpressClientWeb.Models.MediaSchedule;
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
        private const string _controller = "MediaSchedule";

        private string icon;
        public MediaScheduleController(IMediaService mediaService, IWebSessionService webSessionService)
        {
            _mediaService = mediaService;
            _webSessionService = webSessionService;
        }
        public ActionResult Index()
        {
            var marketNode = new MediaPlanNavigationNode { Position = 1 };
            var model = LoadNavBar(marketNode.Position);
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
            var model = new MediaSelectionViewModel()
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
            var mediaNode = new MediaPlanNavigationNode { Position = 2 };
            model.NavigationBar = LoadNavBar(mediaNode.Position);
            model.ErrorMessage= new ErrorMessage
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

            MediaPlanNavigationNode periodeNode = new MediaPlanNavigationNode { Position = 3 };
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
            var resultNode = new MediaPlanNavigationNode { Position = 4 };
            var model = new ResultsViewModel
            {
                NavigationBar = LoadNavBar(resultNode.Position)
            };            
            return View(model);
        }

        public JsonResult MediaScheduleResult()
        {
            WebSession CustomerSession = (WebSession)WebSession.Load("201602241628131084");

            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
            MediaScheduleData result = null;
            MediaSchedulePeriod period = null;
            Int64 moduleId = CustomerSession.CurrentModule;
            ConstantePeriod.DisplayLevel periodDisplay = CustomerSession.DetailPeriod;
            WebConstantes.CustomerSessions.Unit oldUnit = CustomerSession.Unit;
            // TODO : Commented temporarily for new AdExpress
            //if (UseCurrentUnit) webSession.Unit = CurrentUnit;
            object[] param = null;
            long oldCurrentTab = CustomerSession.CurrentTab;
            System.Windows.Forms.TreeNode oldReferenceUniversMedia = CustomerSession.ReferenceUniversMedia;

            #region Period Detail
            DateTime begin;
            DateTime end;
            string _zoomDate = string.Empty;
            if (!string.IsNullOrEmpty(_zoomDate))
            {
                if (CustomerSession.DetailPeriod == ConstantePeriod.DisplayLevel.weekly)
                {
                    begin = Dates.getPeriodBeginningDate(_zoomDate, ConstantePeriod.Type.dateToDateWeek);
                    end = Dates.getPeriodEndDate(_zoomDate, ConstantePeriod.Type.dateToDateWeek);
                }
                else
                {
                    begin = Dates.getPeriodBeginningDate(_zoomDate, ConstantePeriod.Type.dateToDateMonth);
                    end = Dates.getPeriodEndDate(_zoomDate, ConstantePeriod.Type.dateToDateMonth);
                }
                begin = Dates.Max(begin,
                    Dates.getPeriodBeginningDate(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodType));
                end = Dates.Min(end,
                    Dates.getPeriodEndDate(CustomerSession.PeriodEndDate, CustomerSession.PeriodType));

                CustomerSession.DetailPeriod = ConstantePeriod.DisplayLevel.dayly;
                if (CustomerSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && CustomerSession.CurrentModule
                    == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly, CustomerSession.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly);

            }
            else
            {
                begin = Dates.getPeriodBeginningDate(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodType);
                end = Dates.getPeriodEndDate(CustomerSession.PeriodEndDate, CustomerSession.PeriodType);
                if (CustomerSession.DetailPeriod == ConstantePeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                {
                    CustomerSession.DetailPeriod = ConstantePeriod.DisplayLevel.monthly;
                }

                if (CustomerSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && CustomerSession.CurrentModule
                    == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(begin, end, CustomerSession.DetailPeriod, CustomerSession.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(begin, end, CustomerSession.DetailPeriod);

            }
            #endregion

            if (_zoomDate.Length > 0)
            {
                param = new object[3];
                param[2] = _zoomDate;
            }
            else
            {
                param = new object[2];
            }
            CustomerSession.CurrentModule = module.Id;
            if (CustomerSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA) CustomerSession.CurrentTab = 0;
            CustomerSession.ReferenceUniversMedia = new System.Windows.Forms.TreeNode("media");
            param[0] = CustomerSession;
            param[1] = period;
            var mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, module.CountryRulesLayer.AssemblyName), module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            mediaScheduleResult.Module = module;
            result = mediaScheduleResult.GetHtml();

            return null;
        }

        public JsonResult SaveMediaSelection(List<long> selectedMedia, string nextStep)
        {
            string url = string.Empty;
            var response = new WebSessionResponse();
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
        private List< MediaPlanNavigationNode> LoadNavBar(int currentPosition)
        {
            var model = new List<MediaPlanNavigationNode>();
            //TODO Update Navbar according to the country selection
            #region Hardcoded  nav Bar.
            var market = new MediaPlanNavigationNode
            {
                Id = 1,
                IsActive = false,
                Description = "Market",
                Title = "Marché",
                Action = "Index",
                Controller = "MediaSchedule"
            };
            model.Add(market);
            var media = new MediaPlanNavigationNode
            {
                Id = 2,
                IsActive = false,
                Description = "Media",
                Title = "Media",
                Action = "MediaSelection",
                Controller = "MediaSchedule"
            };
            model.Add(media);
            var dates = new MediaPlanNavigationNode
            {
                Id = 3,
                IsActive = false,
                Description = "Dates",
                Title = "Dates",
                Action = "PeriodSelection",
                Controller = "MediaSchedule"
            };
            model.Add(dates);
            var result = new MediaPlanNavigationNode
            {
                Id = 4,
                IsActive = false,
                Description = "Results",
                Title = "Resultats",
                Action = "Results",
                Controller = "MediaSchedule"
            };
            model.Add(result);
            foreach( var nav in model)
            {
                nav.IsActive = (nav.Id > currentPosition) ? false : true;
            }
            #endregion
            return model;
        }
        #endregion

    }
}